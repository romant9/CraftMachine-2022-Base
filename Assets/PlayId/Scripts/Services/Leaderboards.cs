using System;
using System.Collections.Generic;
using System.Linq;
using Assets.PlayId.Scripts.Data;
using Assets.PlayId.Scripts.Helpers;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.PlayId.Scripts.Services
{
    public class Leaderboards : Logger
    {
        #if _DEBUG
        private const string Endpoint = "https://localhost:7297/scores";
        #else
        private const string Endpoint = "https://playid.org/scores";
        #endif

        private readonly string _accessToken;
        private readonly int _userId;

        public Leaderboards(string accessToken, int userId)
        {
            _accessToken = accessToken;
            _userId = userId;
        }

        /// <summary>
        /// Reports leaderboard score.
        /// </summary>
        public void ReportScore(string leaderboardId, long score, Action<bool, string> callback)
        {
            var request = UnityWebRequest.Post($"{Endpoint}/report", new Dictionary<string, string> { { "leaderboard_id", leaderboardId }, { "score", score.ToString() } });

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
        /// Returns `top` leaderboard scores for `period` days.
        /// </summary>
        public void LoadScores_(string leaderboardId, int top, int period, List<int> friends, Action<bool, string, List<Score>> callback)
        {
            var scope = friends.ToList();

            if (!scope.Contains(_userId)) scope.Add(_userId);

            LoadScores(leaderboardId, top, period, scope, callback);
        }

        /// <summary>
        /// Returns `top` leaderboard scores for `period` days.
        /// </summary>
        public static void LoadScores(string leaderboardId, int top, int period, List<int> scope, Action<bool, string, List<Score>> callback)
        {
            var form = new Dictionary<string, string>
            {
                { "client_id", PlayIdServices.Instance.Auth.ClientId },
                { "leaderboard_id", leaderboardId },
                { "top", top.ToString() },
                { "period", period.ToString() },
                { "scope", JsonConvert.SerializeObject(scope) }
            };

            var request = UnityWebRequest.Post($"{Endpoint}/load", form);

            Debug.Log($"Downloading: {request.url}");

            request.SendWebRequest().completed += _ =>
            {
                if (request.result == UnityWebRequest.Result.Success)
                {
                    callback(true, null, JsonConvert.DeserializeObject<List<Score>>(request.downloadHandler.text));
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