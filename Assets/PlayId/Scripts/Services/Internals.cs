using System;
using System.Collections.Generic;
using Assets.PlayId.Scripts.Enums;
using Assets.PlayId.Scripts.Helpers;
using UnityEngine.Networking;

namespace Assets.PlayId.Scripts.Services
{
    /// <summary>
    /// This class provides data originally retrieved by Play ID from OAuth platforms.
    /// You can find platform specific information about users here.
    /// Available data and format are different for different platforms.
    /// </summary>
    public class Internals : Logger
    {
        #if _DEBUG
        private const string Endpoint = "https://localhost:7297/internal";
        #else
        private const string Endpoint = "https://playid.org/internal";
        #endif

        private readonly string _accessToken;

        public Internals(string accessToken)
        {
            _accessToken = accessToken;
        }

        /// <summary>
        /// Returns original user info for the selected platform. It may contain additional information about the user.
        /// </summary>
        public void RequestUserInfoForPlatform(Platform platform, Action<bool, string, string> callback)
        {
            var request = UnityWebRequest.Post($"{Endpoint}/user_info", new Dictionary<string, string> { { "platform", ((int)platform).ToString() } });

            request.SetRequestHeader("Authorization", $"Bearer {_accessToken}");

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

        /// <summary>
        /// Returns an original ID token (JWT) for the selected platform.
        /// </summary>
        public void RequestIdTokenForPlatform(Platform platform, bool refresh, Action<bool, string, string> callback)
        {
            var request = UnityWebRequest.Post($"{Endpoint}/id_token", new Dictionary<string, string> { { "platform", ((int)platform).ToString() }, { "refresh", refresh.ToString() } });

            request.SetRequestHeader("Authorization", $"Bearer {_accessToken}");

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