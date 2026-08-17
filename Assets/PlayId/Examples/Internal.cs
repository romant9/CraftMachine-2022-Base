using Assets.PlayId.Scripts;
using Assets.PlayId.Scripts.Data;
using Assets.PlayId.Scripts.Enums;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.PlayId.Examples
{
    public class Internal : MonoBehaviour
    {
        public Text Output;

        public void GetGoogleUserInfo()
        {
            PlayIdServices.Instance.Auth.SignIn(OnSignIn);

            void OnSignIn(bool success, string error, User user)
            {
                if (success)
                {
                    user.Internals.RequestUserInfoForPlatform(Platform.Google, OnGetGoogleUserInfo);
                }
                else
                {
                    Output.text = error;
                }
            }

            void OnGetGoogleUserInfo(bool success, string error, string userInfo)
            {
                Output.text = success ? userInfo : error;
            }
        }

        public void GetGoogleIdToken()
        {
            PlayIdServices.Instance.Auth.SignIn(OnSignIn);

            void OnSignIn(bool success, string error, User user)
            {
                if (success)
                {
                    user.Internals.RequestIdTokenForPlatform(Platform.Google, refresh: false, OnGetGoogleIdToken);
                }
                else
                {
                    Output.text = error;
                }
            }

            void OnGetGoogleIdToken(bool success, string error, string idToken)
            {
                Output.text = success ? idToken : error;
            }
        }
    }
}