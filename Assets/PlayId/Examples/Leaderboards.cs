using System.Collections.Generic;
using System.Linq;
using Assets.PlayId.Scripts;
using Assets.PlayId.Scripts.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.PlayId.Examples
{
    public class Leaderboards : MonoBehaviour
    {
        public Text Output;

        private readonly string _accessToken;

        /// <summary>
        /// A constructor.
        /// </summary>
        public Leaderboards(string accessToken)
        {
            _accessToken = accessToken;
        }

        public void ReportScore()
        {
            PlayIdServices.Instance.Auth.SignIn(OnSignIn);

            void OnSignIn(bool success, string error, User user)
            {
                if (success)
                {
                    user.Leaderboards.ReportScore("gold", Random.Range(0, 10000), OnReport);
                }
                else
                {
                    Output.text = error;
                }
            }

            void OnReport(bool success, string error)
            {
                Output.text = success ? "Score reported!" : error;
            }
        }

        public void LoadScores()
        {
            var scope = new List<int>(); // A list of users that should be guaranteed included to scores.

            if (PlayIdServices.Instance.Auth.SavedUser != null)
            {
                scope.Add(PlayIdServices.Instance.Auth.SavedUser.Id);
            }

            Scripts.Services.Leaderboards.LoadScores("gold", 10, 90, scope, OnLoadScores);

            void OnLoadScores(bool success, string error, List<Score> scores)
            {
                Output.text = success ? string.Join('\n', scores.Select(i => $"#{i.Position} - {i.UserName} - {i.Value}")) : error;
            }
        }
    }
}