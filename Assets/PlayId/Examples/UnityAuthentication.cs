using Assets.PlayId.Scripts;
using Assets.PlayId.Scripts.Data;
using Assets.PlayId.Scripts.Enums;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.PlayId.Examples
{
    public class UnityAuthentication : MonoBehaviour
    {
        public Text Output;

        public void SignInWithGoogle()
        {
            Output.text = "Please import Unity Authentication package and uncomment code below in Examples/UnityAuthentication.cs.";

            PlayIdServices.Instance.Auth.SignIn(OnSignIn, caching: false, platforms: Platform.Google);

            void OnSignIn(bool success, string error, User user)
            {
                user.Internals.RequestIdTokenForPlatform(Platform.Google, refresh: true, OnGetIdToken);
            }

            async void OnGetIdToken(bool success, string error, string idToken)
            {
                if (success)
                {
                    await Unity.Services.Core.UnityServices.InitializeAsync();

                    var authService = Unity.Services.Authentication.AuthenticationService.Instance;

                    if (authService.IsSignedIn) authService.SignOut();

                    await authService.SignInWithGoogleAsync(idToken);

                    Output.text = authService.IsAuthorized ? $"Player ID: {authService.PlayerInfo.Id}!" : "Unable to authorize.";
                }
                else
                {
                    Output.text = error;
                }
            }
        }

        public void SignInWithApple()
        {
            Output.text = "Please import Unity Authentication package and uncomment code below in Examples/UnityAuthentication.cs.";

            //PlayIdServices.Instance.Auth.SignIn(OnSignIn, caching: false, platforms: Platform.Apple);

            //void OnSignIn(bool success, string error, User user)
            //{
            //    user.Internals.RequestIdTokenForPlatform(Platform.Apple, refresh: true, OnGetIdToken);
            //}

            //async void OnGetIdToken(bool success, string error, string idToken)
            //{
            //    if (success)
            //    {
            //        await Unity.Services.Core.UnityServices.InitializeAsync();

            //        var authService = Unity.Services.Authentication.AuthenticationService.Instance;

            //        if (authService.IsSignedIn) authService.SignOut();

            //        await authService.SignInWithAppleAsync(idToken);

            //        Output.text = authService.IsAuthorized ? $"Player ID: {authService.PlayerInfo.Id}!" : "Unable to authorize.";
            //    }
            //    else
            //    {
            //        Output.text = error;
            //    }
            //}
        }
    }
}