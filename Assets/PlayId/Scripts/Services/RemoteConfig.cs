using System;
using System.Collections.Generic;
using Assets.PlayId.Scripts.Helpers;
using UnityEngine.Networking;

namespace Assets.PlayId.Scripts.Services
{
    public class RemoteConfig : Logger
    {
        #if _DEBUG
        private const string Endpoint = "https://localhost:7297/rconfig";
        #else
        private const string Endpoint = "https://playid.org/rconfig";
        #endif

        private readonly string _clientId;

        /// <summary>
        /// A constructor.
        /// </summary>
        public RemoteConfig(string clientId)
        {
            _clientId = clientId;
        }

        /// <summary>
        /// Loads remote configuration.
        /// </summary>
        public void Load(Action<bool, string, string> callback)
        {
            var request = UnityWebRequest.Post($"{Endpoint}/load", new Dictionary<string, string> { { "client_id", _clientId } });

            Log($"Downloading: {request.url}");

            request.SendWebRequest().completed += _ =>
            {
                if (request.result == UnityWebRequest.Result.Success)
                {
                    callback(true, null, request.downloadHandler.text);
                }
                else
                {
                    callback(false, request.GetError(), null);
                }

                request.Dispose();
            };
        }
    }
}