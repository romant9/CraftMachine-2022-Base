using Assets.PlayId.Scripts;
using Assets.PlayId.Scripts.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.PlayId.Examples
{
    public class RefreshAccessToken : MonoBehaviour
    {
        public Text Output;
        
        public void Refresh()
        {
            if (PlayIdServices.Instance.Auth.SavedUser == null)
            {
                Output.text = "Please sign in first.";
            }
            else
            {
                if (PlayIdServices.Instance.Auth.SavedUser.TokenResponse.Expired)
                {
                    PlayIdServices.Instance.Auth.RefreshAccessToken(OnRefreshAccessToken);
                }
                else
                {
                    Output.text = "Not expired yet.";
                }
            }

            void OnRefreshAccessToken(bool success, string error, TokenResponse tokenResponse)
            {
                Output.text = success ? tokenResponse.AccessToken : error;
            }
        }
    }
}