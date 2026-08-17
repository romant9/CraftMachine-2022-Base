using System;
using System.Collections.Generic;
using System.Text;
using Assets.PlayId.Scripts.Helpers;
using UnityEngine.Networking;

namespace Assets.PlayId.Scripts.Services
{
    public class CloudSaves : Logger
    {
        #if _DEBUG
        private const string Endpoint = "https://localhost:7297/cloud";
        #else
        private const string Endpoint = "https://playid.org/cloud";
        #endif

        private readonly string _accessToken;

        public CloudSaves(string accessToken)
        {
            _accessToken = accessToken;
        }

        /// <summary>
        /// Saves string `data` to cloud storage.
        /// </summary>
        public void Save(string data, Action<bool, string> callback)
        {
            Save(Encoding.UTF8.GetBytes(data), callback);
        }

        /// <summary>
        /// Saves byte[] `data` to cloud storage.
        /// </summary>
        public void Save(byte[] data, Action<bool, string> callback)
        {
            if (data.Length > 4096) throw new ArgumentException("Max data length is 4096 bytes.");

            var request = UnityWebRequest.Post($"{Endpoint}/save", new Dictionary<string, string> { { "data", Convert.ToBase64String(data) } });

            request.SetRequestHeader("Authorization", $"Bearer {_accessToken}");

            Log($"Downloading: {request.url}");

            request.SendWebRequest().completed += _ =>
            {
                if (request.result == UnityWebRequest.Result.Success)
                {
                    callback(true, null);
                }
                else
                {
                    callback(false, request.GetError());
                }

                request.Dispose();
            };
        }

        /// <summary>
        /// Loads byte[] `data` from cloud storage.
        /// </summary>
        public void Load(Action<bool, string, byte[]> callback)
        {
            var request = UnityWebRequest.PostWwwForm($"{Endpoint}/load", "");

            request.SetRequestHeader("Authorization", $"Bearer {_accessToken}");

            Log($"Downloading: {request.url}");

            request.SendWebRequest().completed += _ =>
            {
                if (request.result == UnityWebRequest.Result.Success)
                {
                    callback(true, null, Convert.FromBase64String(request.downloadHandler.text));
                }
                else
                {
                    callback(false, request.GetError(), null);
                }

                request.Dispose();
            };
        }

        /// <summary>
        /// Loads string `data` from cloud storage.
        /// </summary>
        public void LoadString(Action<bool, string, string> callback)
        {
            Load((success, error, data) => callback(success, error, data == null ? null : Encoding.UTF8.GetString(data)));
        }
    }
}