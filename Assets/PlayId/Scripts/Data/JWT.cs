using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using PlayId.Scripts.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace PlayId.Scripts.Data
{
    /// <summary>
    /// JWT debugger: https://jwt.io/
    /// </summary>
    public class JWT
    {
        public readonly string Encoded;

        public string Header => Base64UrlEncoder.Decode(Encoded.Split('.')[0]);
        public string Payload => Base64UrlEncoder.Decode(Encoded.Split('.')[1]);
        public string SignedData => Encoded.Split('.')[0] + "." + Encoded.Split('.')[1];
        public string Signature => Encoded.Split('.')[2];

        public const string JwksUri = "https://playid.org/.well-known/jwks";

        private static Dictionary<string, Dictionary<string, string>> KnownPublicKeys
        {
            get => PlayerPrefs.HasKey(JwksUri) ? JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(PlayerPrefs.GetString(JwksUri)) : new Dictionary<string, Dictionary<string, string>>();
            set => PlayerPrefs.SetString(JwksUri, JsonConvert.SerializeObject(value));
        }

        public JWT(string encoded)
        {
            Encoded = encoded;
        }

        /// <summary>
        /// More info: https://github.com/hippogamesunity/PlayID/wiki/ID-Token-validation
        /// Signature validation makes sense on a backend only in most cases.
        /// </summary>
        public void ValidateSignature(string clientId, Action<bool, string> callback)
        {
            var header = JObject.Parse(Header);

            if ((string)header["typ"] != "JWT")
            {
                callback(false, "Unexpected header (typ).");
                return;
            }

            if ((string)header["alg"] != "RS256")
            {
                callback(false, "Unexpected header (alg).");
                return;
            }

            var payload = JObject.Parse(Payload);

            if ((string)payload["iss"] != "https://playid.org")
            {
                callback(false, "Unexpected payload (iss).");
                return;
            }

            if ((string)payload["aud"] != clientId)
            {
                callback(false, "Unexpected payload (aud).");
                return;
            }

            var exp = (long)payload["exp"];

            if (exp < ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds())
            {
                callback(false, "JWT expired.");
                return;
            }

            var kid = (string)header["kid"] ?? throw new Exception("Key missed (kid).");

            if (KnownPublicKeys.ContainsKey(kid))
            {
                var verified = VerifySignature(KnownPublicKeys[kid]["n"], KnownPublicKeys[kid]["e"]);

                if (!verified)
                {
                    callback(false, "Invalid JWT signature.");
                    return;
                }

                callback(true, null);

                return;
            }

            var request = UnityWebRequest.Get(JwksUri);

            request.SendWebRequest().completed += _ =>
            {
                if (request.result == UnityWebRequest.Result.Success)
                {
                    var responseText = request.downloadHandler.text;

                    request.Dispose();

                    var certs = JObject.Parse(responseText);
                    var keys = (certs["keys"] ?? throw new Exception("Key missed (keys).")).ToDictionary(i => i["kid"].Value<string>(), i => i.ToObject<Dictionary<string, string>>());

                    KnownPublicKeys = keys;

                    if (!keys.TryGetValue(kid, out var key) || key == null)
                    {
                        callback(false, $"Public key not found (kid={kid}).");
                        return;
                    }

                    if (!key.TryGetValue("n", out var modulus))
                    {
                        callback(false, $"Invalid modulus (kid={kid}).");
                        return;
                    }

                    if (!key.TryGetValue("e", out var exponent))
                    {
                        callback(false, $"Invalid exponent (kid={kid}).");
                        return;
                    }

                    var verified = VerifySignature(modulus, exponent);

                    if (!verified)
                    {
                        callback(false, "Invalid JWT signature.");
                        return;
                    }

                    callback(true, null);
                }
                else
                {
                    callback(false, request.error);
                    request.Dispose();
                }
            };
        }

        private bool VerifySignature(string modulus, string exponent)
        {
            var parameters = new RSAParameters
            {
                Modulus = Base64UrlEncoder.DecodeBytes(modulus),
                Exponent = Base64UrlEncoder.DecodeBytes(exponent)
            };
            var provider = new RSACryptoServiceProvider();

            provider.ImportParameters(parameters);

            var signature = Base64UrlEncoder.DecodeBytes(Signature);
            var data = Encoding.UTF8.GetBytes(SignedData);
            var verified = provider.VerifyData(data, SHA256.Create(), signature);

            return verified;
        }
    }
}