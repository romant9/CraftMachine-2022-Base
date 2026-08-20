using Google.Apis.Sheets.v4.Data;
using Newtonsoft.Json;
using PlayId.Scripts.Data;
using PlayId.Scripts.Enums;
using PlayId.Scripts.Helpers;
using PlayId.Scripts.Utils;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace PlayId.Scripts.Services
{
    public partial class Auth : Logger
    {
        public SavedUser SavedUser { get; private set; }

        #if _DEBUG
        private const string AuthorizationEndpoint = "https://localhost:7297/auth";
        private const string TokenEndpoint = "https://localhost:7297/token";
        private const string UserEndpoint = "https://localhost:7297/user";
        #else
        private const string AuthorizationEndpoint = "https://playid.org/auth";
        private const string TokenEndpoint = "https://playid.org/token";
        private const string UserEndpoint = "https://playid.org/user";
        #endif

        private readonly AuthSettings _settings;
        private string _state, _nonce, _redirectUri;
        private Action<bool, string, User> _callback;
        private bool _tokenResponse;

        public string ClientId => _settings.ClientId;

        public Auth(AuthSettings settings = null)
        {
            _settings = settings == null ? Resources.Load<AuthSettings>("AuthSettings") : settings;

            SavedUser = SavedUser.Load(_settings.ClientId);
            Application.deepLinkActivated += OnDeepLinkActivated;

            #if UNITY_IOS && !UNITY_EDITOR

            SafariViewController.DidCompleteInitialLoad += DidCompleteInitialLoad;
            SafariViewController.DidFinish += UserCancelledHook;

            #endif
        }

        ~Auth()
        {
            Application.deepLinkActivated -= OnDeepLinkActivated;

            #if UNITY_IOS && !UNITY_EDITOR

            SafariViewController.DidCompleteInitialLoad -= DidCompleteInitialLoad;
            SafariViewController.DidFinish -= UserCancelledHook;

            #endif
        }

        public void SignIn(Action<bool, string, User> callback, Platform platforms = Platform.Any, bool caching = true, string nonce = null)
        {
            _callback = callback;
            _tokenResponse = false;

            if (SavedUser == null)
            {
                Authenticate(platforms, nonce: nonce);
            }
            else if (caching && !SavedUser.TokenResponse.Expired)
            {
                SavedUser.InitializeServices();
                callback(true, null, SavedUser);
            }
            else
            {
                UseSavedToken(platforms);
            }
        }

        private void Authenticate(Platform platforms, bool link = false, string nonce = null)
        {
            _state = Guid.NewGuid().ToString("N");
            _nonce = nonce ?? Guid.NewGuid().ToString("N");
            _redirectUri = $"{_settings.RedirectUriScheme}://oauth2/playid";

            PlayerPrefs.SetString(TempKey, $"{_state}|{_redirectUri}");
            PlayerPrefs.Save();

            var endpoint = link ? AuthorizationEndpoint + "/link" : AuthorizationEndpoint;
            var url = $"{endpoint}?client_id={_settings.ClientId}&state={_state}&nonce={_nonce}&device={Md5.ComputeHash(SystemInfo.deviceUniqueIdentifier)}";

            #if UNITY_WEBGL || UNITY_EDITOR

            ApplicationFocusHook.Create(() => DownloadTokenResponse(_state));

            #else

            url += $"&redirect_uri={Uri.EscapeDataString(_redirectUri)}";

            if (!_settings.ManualCancellation)
            {
                ApplicationFocusHook.Create(UserCancelledHook);
            }

            #if UNITY_STANDALONE_WIN

            WindowsDeepLinking.Initialize(_settings.RedirectUriScheme, OnDeepLinkActivated);

            #endif

            #endif

            if (platforms != Platform.Any) url += $"&platforms={(int)platforms}";
            if (link) url += $"&access_token={SavedUser.TokenResponse.AccessToken}";

            #if UNITY_IOS && !UNITY_EDITOR

            SafariViewController.OpenURL(url);

            #else

            Application.OpenURL(url);

            #endif
        }

        private void UseSavedToken(Platform platforms)
        {
            if (SavedUser == null || SavedUser.ClientId != _settings.ClientId)
            {
                SignOut();
                SignIn(_callback, platforms);
            }
            else if (!SavedUser.TokenResponse.Expired)
            {
                Log("Using saved access token...");
                RequestUserInfo(SavedUser.TokenResponse.AccessToken, (success, _, user) =>
                {
                    if (success)
                    {
                        _callback(true, null, user);
                    }
                    else
                    {
                        SignOut();
                        SignIn(_callback, platforms);
                    }
                });
            }
            else
            {
                Log("Refreshing expired access token...");
                RefreshAccessToken((success, _, _) =>
                {
                    if (success)
                    {
                        RequestUserInfo(SavedUser.TokenResponse.AccessToken, _callback);
                    }
                    else
                    {
                        SignOut();
                        SignIn(_callback, platforms);
                    }
                });
            }
        }

        public void SignOut(bool revokeAccessToken = false)
        {
            if (SavedUser != null)
            {
                if (revokeAccessToken && SavedUser.TokenResponse != null)
                {
                    RevokeAccessToken(SavedUser.TokenResponse.AccessToken);
                }

                SavedUser.Delete();
                SavedUser = null;
            }
        }

        private void DownloadTokenResponse(string state)
        {
            var request = UnityWebRequest.Post($"{TokenEndpoint}/get", new Dictionary<string, string> { { "client_id", _settings.ClientId }, { "state", state } });

            Log($"Downloading: {request.url}");

            request.SendWebRequest().completed += _ =>
            {
                if (request.result == UnityWebRequest.Result.Success)
                {
                    var tokenResponse = request.downloadHandler.text;

                    OnTokenResponse(tokenResponse);
                }
                else if (request.downloadHandler.text == "Unauthorized.")
                {
                    _callback(false, "User cancelled.", null);
                }
                else
                {
                    LogError(request.GetError());
                    _callback(false, request.GetError(), null);
                }

                request.Dispose();
            };
        }

        /// <summary>
        /// Force cancel.
        /// </summary>
        public void Cancel()
        {
            ApplicationFocusHook.Cancel();
        }

        private const string TempKey = "oauth.playid";

        /// <summary>
        /// This can be called on app startup to continue OAuth.
        /// In some scenarios, the app may be terminated while the user performs sign-in on Google website.
        /// </summary>
        public void TryResume(Action<bool, string, User> callback)
        {
            if (string.IsNullOrEmpty(Application.absoluteURL) || !PlayerPrefs.HasKey(TempKey)) return;

            var parts = PlayerPrefs.GetString(TempKey).Split('|');

            if (!Application.absoluteURL.StartsWith(parts[1])) return;

            _state = parts[0];
            _redirectUri = parts[1];
            _callback = callback;

            OnDeepLinkActivated(Application.absoluteURL);
        }

        private void DidCompleteInitialLoad(bool loaded)
        {
            if (loaded) return;

            _callback?.Invoke(false, "Failed to load auth screen.", null);
        }

        private async void UserCancelledHook()
        {
            if (_settings.ManualCancellation) return;

            var time = DateTime.UtcNow;

            while ((DateTime.UtcNow - time).TotalSeconds < 1)
            {
                await Task.Yield();
            }

            if (!_tokenResponse)
            {
                _callback.Invoke(false, "User cancelled.", null);
            }
        }

        private void OnDeepLinkActivated(string deepLink)
        {
            Log($"Deep link activated: {deepLink}");

            deepLink = deepLink.Replace(":///", ":/"); // Some browsers may add extra slashes.

            if (_redirectUri == null || !deepLink.StartsWith(_redirectUri) || _callback == null)
            {
                Log("Unexpected deep link.");
                return;
            }

            #if UNITY_IOS && !UNITY_EDITOR

            SafariViewController.Close();
            
            #endif

            var parameters = ParseQueryString(deepLink);
            var error = parameters.Get("error");

            if (error != null)
            {
                _callback?.Invoke(false, error, null);
                return;
            }

            var state = parameters.Get("state"); // TODO:
            var token = parameters.Get("token");

            if (state == null || token == null) return;

            if (state == _state)
            {
                if (PlayerPrefs.HasKey(TempKey))
                {
                    PlayerPrefs.DeleteKey(TempKey);
                    PlayerPrefs.Save();
                }

                var tokenResponse = Encoding.UTF8.GetString(Convert.FromBase64String(token));

                OnTokenResponse(tokenResponse);
            }
            else
            {
                Log("Unexpected state.");
            }
        }

        public void OnTokenResponse(string json)
        {
			//{\"access_token\":\"b735aa33ef30404cbaa5fd10febbd9cd\",\"token_type\":\"bearer\",\"expires_in\":7200,\"scope\":\"openid email profile\",\"refresh_token\":\"a64666aac0164a4a8f37c438ed96186d\",\"id_token\":\"eyJhbGciOiJSUzI1NiIsImtpZCI6IjUxY2NmMGQ0ZjA5M2MwMjI5ZTVmMzA4MjIwMjk3NDY3IiwidHlwIjoiSldUIn0.eyJpc3MiOiJodHRwczovL3BsYXlpZC5vcmciLCJhdWQiOiJlOTYwZDk0NGY3ODg0YTBkYTVlYzBmOWRmMGU2ODc2NSIsInN1YiI6NDQ1MSwibmFtZSI6IlJvbWFuIEFtZWxjaGVua28iLCJlbWFpbCI6ImFtZWxjaGVua29ydkBnbWFpbC5jb20iLCJwbGF0Ijo2NSwiaWF0IjoxNzg3MjMzODg2LCJleHAiOjE3ODcyNDEwODYsInN0YXRlIjoiNDU1OWUwMWJmYjQzNGFhNjlhZmUzZWNlZGIxM2Q3NDAiLCJub25jZSI6IjMzMTQ5YTRkMzE4YTRiZDJiNzIyMjFlNTA4NjQzZjZlIn0.NgE58PiQFHSz-5ZkvyEGCOEZ4l8IxFMw1N4wMP9-QYQyozWxHDJUU8xspeCe4OsVCTJ-T6G9a8JXVHw3CrtBWMRoB6G2aA-AhWSPtck8z1jw_yZ8eWPP8a8CWEePRP1ZlDu8YOMl3vgffrDCvbB8hTBTbUgaQQb5i_HyZNeoUYIB6eNiIr_SIX8_St9wN8Caeq6WrxLj2EGz849XiDay9JMBYJxhhpsZWCp2WjV3nrYYfGORplRa4Pta3LknCTN0G2tdSx5EyxFzE0fFaoB0kwJV46i_zillOTiwbrFsSXbBo68FXvY3t0T_UkyV25yf15ti2YLBMOW7VEvm4T5oHw\"}
			//{\"iss\":\"https://playid.org\",\"aud\":\"e960d944f7884a0da5ec0f9df0e68765\",\"sub\":4451,\"name\":\"Roman Amelchenko\",\"email\":\"amelchenkorv@gmail.com\",\"plat\":65,\"iat\":1787236666,\"exp\":1787243866,\"state\":\"11dc305693954bb7bda3020522e3f771\",\"nonce\":\"fa5c9444baf24160813571b1e705b81b\"

			_tokenResponse = true;

            Log($"TokenResponse={json}");

            var tokenResponse = TokenResponse.Parse(json);

            SavedUser = new SavedUser(tokenResponse, _settings.ClientId);
            SavedUser.Save();
            SavedUser.InitializeServices();

            //RequestUserInfo(tokenResponse.AccessToken, _callback); // We don't need to request user info as it has been already obtained from JWT.
            _callback(true, null, SavedUser);
        }

		public void RequestUserInfo(string accessToken, Action<bool, string, User> callback)
        {
            var request = UnityWebRequest.PostWwwForm($"{UserEndpoint}/info", "");

            request.SetRequestHeader("Authorization", $"Bearer {accessToken}");

            Log($"Downloading: {request.url}");

            request.SendWebRequest().completed += _ =>
            {
                if (request.result == UnityWebRequest.Result.Success)
                {
                    Log($"User={request.downloadHandler.text}");

                    var user = JsonConvert.DeserializeObject<User>(request.downloadHandler.text);

                    SavedUser.Id = user.Id;
                    SavedUser.Name = user.Name;
                    SavedUser.Email = user.Email;
                    SavedUser.Platforms = user.Platforms;
                    SavedUser.InitializeServices();
                    SavedUser.Save();
                    callback(true, null, SavedUser);
                }
                else
                {
                    LogError(request.GetError());
                    callback(false, request.GetError(), null);
                }

                request.Dispose();
            };
        }

        public void RefreshAccessToken(Action<bool, string, TokenResponse> callback)
        {
            if (SavedUser == null) throw new Exception("Initial authorization is required.");

            var formFields = new Dictionary<string, string>
            {
                { "client_id", _settings.ClientId },
                { "refresh_token", SavedUser.TokenResponse.RefreshToken },
                { "grant_type", "refresh_token" }
            };

            var request = UnityWebRequest.Post(TokenEndpoint + "/refresh", formFields);

            Log($"Access token refresh: {request.url}");

            request.SendWebRequest().completed += _ =>
            {
                if (request.result == UnityWebRequest.Result.Success)
                {
                    Log($"TokenExchangeResponse={request.downloadHandler.text}");

                    var tokenResponse = TokenResponse.Parse(request.downloadHandler.text);

                    SavedUser.TokenResponse.AccessToken = tokenResponse.AccessToken;
                    SavedUser.TokenResponse.ExpiresIn = tokenResponse.ExpiresIn;
                    SavedUser.TokenResponse.Expiration = tokenResponse.Expiration;
                    SavedUser.InitializeServices();
                    SavedUser.Save();
                    callback(true, null, tokenResponse);
                }
                else
                {
                    LogError(request.GetError());
                    callback(false, request.GetError(), null);
                }

                request.Dispose();
            };
        }

        public void RevokeAccessToken(string accessToken)
        {
            var request = UnityWebRequest.PostWwwForm($"{TokenEndpoint}/revoke?token={accessToken}", "");

            Log($"Revoking access token: {request.url}");

            request.SendWebRequest().completed += _ =>
            {
                Log(request.error ?? "Access token revoked!");
                request.Dispose();
            };
        }

        public void Link(Action<bool, string, User> callback, Platform platforms = Platform.Any)
        {
            if (SavedUser == null) throw new Exception("Not signed in.");

            _callback = callback;
            _tokenResponse = false;

            Authenticate(platforms, link: true);
        }

        public void Unlink(Action<bool, string, User> callback, Platform platform)
        {
            if (SavedUser == null || !SavedUser.Platforms.HasFlag(platform)) throw new Exception($"Not signed in with {platform}.");

            var request = UnityWebRequest.Post($"{AuthorizationEndpoint}/unlink", new Dictionary<string, string> { { "platform", ((int)platform).ToString() } });

            request.SetRequestHeader("Authorization", $"Bearer {SavedUser.TokenResponse.AccessToken}");

            Log($"Downloading: {request.url}");

            request.SendWebRequest().completed += _ =>
            {
                if (request.result == UnityWebRequest.Result.Success)
                {
                    Log($"User={request.downloadHandler.text}");

                    SavedUser.Platforms &= ~platform;
                    SavedUser.Save();
                    callback(true, null, SavedUser);
                }
                else
                {
                    LogError(request.GetError());
                    callback(false, request.GetError(), null);
                }

                request.Dispose();
            };
        }

        private static NameValueCollection ParseQueryString(string url)
        {
            var result = new NameValueCollection();

            foreach (Match match in Regex.Matches(url, @"(?<key>\w+)=(?<value>[^&#]+)"))
            {
                result.Add(match.Groups["key"].Value, Uri.UnescapeDataString(match.Groups["value"].Value));
            }

            return result;
        }
    }
}