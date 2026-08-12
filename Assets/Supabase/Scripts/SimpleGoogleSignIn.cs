using Google.Apis.Auth;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Supabase.Gotrue;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using static Supabase.Gotrue.Constants;

namespace Supabase.TWD
{
	public class SimpleGoogleSignIn : MonoBehaviour
	{
		// Paste your Google Cloud Console credentials here
		private string clientId => UseSupabaseGoogle ? "331314739002-kf5oc1n4blo1r97t8f7jsfkg5labdieq.apps.googleusercontent.com" : "331314739002-f3cp7u6p8dolhfmhalg6n8o3bl3lrmj3.apps.googleusercontent.com";
		private string clientSecret => UseSupabaseGoogle ? "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" : "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";

		private string[] scopes = new[] { "openid", "email", "profile" };
		private string rCode;
		private string userToken;
		public string userEmail;
		public bool UseGoogleApis = true;
		public bool UseSupabaseGoogle = true;
		public SupabaseManager SupabaseManager = null;

		private Thread th;

		public string GetRedirectUri()
		{
			return UseSupabaseGoogle ? "https://ytzxmcoxbzppvoggmqnh.supabase.co/auth/v1/callback" : "http://localhost:8080/callback/";
			//return $"http://localhost:{GetFreeSocketPort()}/";
		}

		private HttpListener httpListener;

		[ContextMenu("StartSignInFlow Google.Apis")]
		public async void StartSignInFlow()
		{
			if (PlayerPrefs.HasKey("userToken"))
			{
				Debug.Log("userToken ... " + userToken);

				userToken = PlayerPrefs.GetString("userToken");
				if (UseGoogleApis)
				{
					var payload = await GoogleJsonWebSignature.ValidateAsync(userToken);

					// 4. Получаем email и статус подтверждения
					string email = payload.Email;
					bool isEmailVerified = payload.EmailVerified;

					if (!isEmailVerified)
					{
						throw new Exception("Email пользователя не подтвержден в системе Google.");
					}
					Debug.Log("User name is " + payload.Name);
					userEmail = email;
				}
				else
				{
					//string email = await GetEmailByTokenAsync(userToken);
					var tokenDecode = JWTDecoder.Decoder.DecodeToken(userToken).Payload;
					TokenInfo tokenInfo = JsonUtility.FromJson<TokenInfo>(tokenDecode);
					var email = tokenInfo.email;

					if (email != null && tokenInfo.email_verified == true)
					{
						// Proceed with user registration or login logic in your game
						Debug.Log($"Welcome back, {email}!");
					}
					userEmail = email;
				}
				Debug.Log("userEmail is ... " + userEmail);
				return;
			}
			// 1. Start a local HTTP server to catch the callback redirect
			var uri = GetRedirectUri();
			httpListener = new HttpListener();
			httpListener.Prefixes.Add(uri + '/');
			if (httpListener.IsListening) httpListener.Stop();
			httpListener.Start();
			//httpListener.BeginGetContext(new AsyncCallback(OnOAuthCallback), httpListener);

			// 2. Build the authorization URL
			string authUrl = string.Format(
				"https://accounts.google.com/o/oauth2/v2/auth?client_id={0}&redirect_uri={1}&response_type=code&scope=openid%20profile%20email",
				clientId, UnityWebRequest.EscapeURL(uri)
			);

			// 3. Open the user's default system browser to sign in
			Application.OpenURL(authUrl);
			Debug.Log("Browser opened for Google Sign-In... " + authUrl);

			//
			HttpListenerContext context = await httpListener.GetContextAsync();

			// 5. Parse the authorization code from the query string
			rCode = context.Request.QueryString["code"];
			if (string.IsNullOrEmpty(rCode))
			{
				Debug.LogError("Google registration code is null");
				httpListener.Stop();
				return;
			}

			PlayerPrefs.SetString("google_reg_code", rCode);
			// 6. Send a friendly response back to the browser window
			HttpListenerResponse response = context.Response;
			string responseString = "<html><body><h1>Authentication successful!</h1><p>You can close this tab.</p></body></html>";
			byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
			response.ContentLength64 = buffer.Length;
			using Stream output = response.OutputStream;
			await output.WriteAsync(buffer, 0, buffer.Length);

			httpListener.Stop();

			if (UseGoogleApis)
			{
				userEmail = await GetUserEmailAsync(rCode);
			}
			else
			{

				userEmail = await ExchangeCodeForToken(rCode);
			}
			Debug.Log("google_reg_code was recived from response... " + rCode);
			Debug.Log("User is ... " + userEmail);
		}

		[ContextMenu("StartSignInFlow Google.Apis Supa")]
		public async void StartSignInFlowSupa()
		{

			// 1. Start a local HTTP server to catch the callback redirect
			var uri = "http://localhost:3000/auth/callback/";
			httpListener = new HttpListener();
			httpListener.Prefixes.Add(uri);
			if (httpListener.IsListening) httpListener.Stop();
			httpListener.Start();
			//httpListener.BeginGetContext(new AsyncCallback(OnOAuthCallback), httpListener);

			// 2. Build the authorization URL
			string authUrl = "https://ytzxmcoxbzppvoggmqnh.supabase.co/auth/v1/authorize?provider=google";

			// 3. Open the user's default system browser to sign in
			Application.OpenURL(authUrl);
			Debug.Log("Browser opened for Google Sign-In... " + authUrl);

			//
			HttpListenerContext context = await httpListener.GetContextAsync();

			// 5. Parse the authorization code from the query string
			rCode = context.Request.QueryString["code"];
			if (string.IsNullOrEmpty(rCode))
			{
				Debug.LogError("Google registration code is null");
				httpListener.Stop();
				return;
			}

			PlayerPrefs.SetString("google_reg_code", rCode);
			// 6. Send a friendly response back to the browser window
			HttpListenerResponse response = context.Response;
			string responseString = "<html><body><h1>Authentication successful!</h1><p>You can close this tab.</p></body></html>";
			byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
			response.ContentLength64 = buffer.Length;
			using Stream output = response.OutputStream;
			await output.WriteAsync(buffer, 0, buffer.Length);

			httpListener.Stop();

			if (UseGoogleApis)
			{
				userEmail = await GetUserEmailAsync(rCode);
			}
			else
			{

				userEmail = await ExchangeCodeForToken(rCode);
			}
			Debug.Log("google_reg_code was recived from response... " + rCode);
			Debug.Log("User is ... " + userEmail);
		}

		[ContextMenu("GetMail Google.Apis")]
		public async void GetMail2()
		{
			await StartSignInFlow2();
			Debug.Log("authorizationCode is ... " + rCode);
			Debug.Log("userEmail is ... " + userEmail);
		}

		public async Task StartSignInFlow2()
		{
			string authUrl = $"https://accounts.google.com/o/oauth2/v2/auth?" +
							 $"client_id={HttpUtility.UrlEncode(clientId)}&" +
							 $"redirect_uri={HttpUtility.UrlEncode(GetRedirectUri())}&" +
							 $"response_type=code&" +
							 $"scope={HttpUtility.UrlEncode(string.Join(' ', scopes))}&" +
							 $"access_type=offline&" +
							 $"prompt=consent";

			// 2. Start a local HTTP Listener to intercept the redirect
			using var listener = new HttpListener();
			listener.Prefixes.Add(GetRedirectUri() + '/');
			if (listener.IsListening) httpListener.Stop();
			listener.Start();
			Debug.Log("Listening for Google redirect... \n" + authUrl);

			// 3. Open the URL in the user's default system browser
			//Process.Start(new ProcessStartInfo
			//{
			//    FileName = authUrl,
			//    UseShellExecute = true
			//});
			Application.OpenURL(authUrl);

			// 4. Wait for the browser redirect context
			HttpListenerContext context = await listener.GetContextAsync();

			// 5. Parse the authorization code from the query string
			rCode = context.Request.QueryString["code"];

			// 6. Send a friendly response back to the browser window
			HttpListenerResponse response = context.Response;
			string responseString = "<html><body><h1>Authentication successful!</h1><p>You can close this tab.</p></body></html>";
			byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
			response.ContentLength64 = buffer.Length;
			using Stream output = response.OutputStream;
			await output.WriteAsync(buffer, 0, buffer.Length);

			listener.Stop();

			userEmail = await GetUserEmailAsync(rCode);
		}

		public async Task<string> GetUserEmailAsync(string authorizationCode)
		{
			var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
			{
				ClientSecrets = new ClientSecrets
				{
					ClientId = clientId,
					ClientSecret = clientSecret
				},
				// Обязательно запрашиваем openid и email
				Scopes = new[] { "openid", "email", "profile" }
			});

			string redirectUri = GetRedirectUri();
			string userId = "Bloodymary";

			// 1. Обмениваем код авторизации на токены
			TokenResponse tokenResponse = await flow.ExchangeCodeForTokenAsync(
				userId,
				authorizationCode,
				redirectUri,
				CancellationToken.None
			);

			// 2. Извлекаем IdToken из ответа Google
			string idToken = tokenResponse.IdToken;

			if (string.IsNullOrEmpty(idToken))
			{
				throw new Exception("IdToken не был получен. Проверьте настройки Scopes.");
			}

			userToken = idToken;
			PlayerPrefs.SetString("userToken", userToken);
			// 3. Валидируем токен и извлекаем из него данные (Payload)
			// Метод автоматически проверит подпись Google, время жизни и ClientId
			var payload = await GoogleJsonWebSignature.ValidateAsync(idToken);

			// 4. Получаем email и статус подтверждения
			string email = payload.Email;
			bool isEmailVerified = payload.EmailVerified;

			if (!isEmailVerified)
			{
				throw new Exception("Email пользователя не подтвержден в системе Google.");
			}
			Debug.Log("User name is " + payload.Name);

			return email;
		}

		public string BuildGoogleAuthUrl()
		{
			string baseUrl = "https://accounts.google.com/o/oauth2/v2/auth";

			var parameters = HttpUtility.ParseQueryString(string.Empty);
			parameters["client_id"] = clientId;
			parameters["redirect_uri"] = GetRedirectUri();
			parameters["response_type"] = "code";
			parameters["scope"] = "openid email profile"; // Space-separated scopes
			parameters["state"] = "secure_random_state_string"; // Protects against CSRF
			parameters["access_type"] = "offline"; // Optional: requested if you need a refresh token

			return $"{baseUrl}?{parameters.ToString()}";
		}

		public static int GetFreeSocketPort()
		{
			using var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

			// Bind to the loopback address and let the OS choose the port (0)
			socket.Bind(new IPEndPoint(IPAddress.Loopback, 0));

			return ((IPEndPoint)socket.LocalEndPoint).Port;
		}

		private void OnOAuthCallback(IAsyncResult result)
		{
			var context = httpListener.EndGetContext(result);
			var request = context.Request;

			// Extract the authorization code from URL parameters
			string code = request.QueryString.Get("code");
			//Debug.Log("Code is... " + code);

			// Send a success message back to the browser tab
			var response = context.Response;
			string responseString = "<html><body><h2>Sign-in successful! You can close this tab and return to the game.</h2></body></html>";
			byte[] buffer = Encoding.UTF8.GetBytes(responseString);
			response.ContentLength64 = buffer.Length;
			response.OutputStream.Write(buffer, 0, buffer.Length);
			response.OutputStream.Close();
			httpListener.Stop();
			if (!string.IsNullOrEmpty(code))
			{
				rCode = code;
			}
		}

		public void LoadInfoAsync(WWWForm form)
		{
			if (th != null && th.IsAlive)
			{
				th.Abort();
				th.Join();
				//Debug.Log("Прервали загрузку информации из дампа...");
			}

			th = new Thread(async () =>
			{
				userEmail = await ExchangeCodeForToken(rCode);
				//Debug.Log("Загрузка информации из дампа завершена!");
			});
			th.Start();
		}

		[Serializable]
		public class GoogleTokenInfo
		{
			public string email;
			public string email_verified;
			public string aud; // Client ID
			public string sub; // Unique Google User ID
		}

		/// <summary>
		/// Asynchronously fetches the user's email using a Google ID token or Access Token.
		/// </summary>
		/// <param name="token">The token string received from Google Sign-In.</param>
		public async Task<string> GetEmailByTokenAsync(string token)
		{
			// Use Google's oauth2 tokeninfo endpoint
			string url = $"https://googleapis.com?access_token={token}";

			// Note: If you are using an Access Token instead of an ID Token,
			// change the URL parameter to: ?access_token={token}

			using UnityWebRequest webRequest = UnityWebRequest.Get(url);
			// Send the request and await until it finishes
			var operation = webRequest.SendWebRequest();

			while (!operation.isDone)
			{
				await Task.Yield(); // Hands control back to Unity frame-by-frame
			}

			// Check for network errors or server-side bad requests
			if (webRequest.result != UnityWebRequest.Result.Success)
			{
				Debug.LogError($"Failed to validate token: {webRequest.error}");
				return null;
			}

			// Parse the response
			string jsonResult = webRequest.downloadHandler.text;
			GoogleTokenInfo tokenInfo = JsonUtility.FromJson<GoogleTokenInfo>(jsonResult);

			if (tokenInfo != null && !string.IsNullOrEmpty(tokenInfo.email))
			{
				Debug.Log($"Successfully retrieved email: {tokenInfo.email}");
				return tokenInfo.email;
			}

			Debug.LogError("Token was verified but no email was attached.");
			return null;
		}

		private void Update()
		{
		}

		private WWWForm GetForm(string code)
		{
			WWWForm form = new();
			form.AddField("code", code);
			form.AddField("client_id", clientId);
			form.AddField("client_secret", clientSecret);
			form.AddField("redirect_uri", GetRedirectUri());
			form.AddField("grant_type", "authorization_code");
			return form;
		}

		[ContextMenu("ResetToket")]
		public void ResetToket()
		{
			PlayerPrefs.DeleteKey("userToken");
		}

		[System.Serializable]
		public class TokenResponseCustom
		{
			public string access_token;
			public string expires_in;
			public string refresh_token;
			public string token_type;
			public string id_token;
		}

		[System.Serializable]
		public class TokenInfo
		{
			public string iss; // "https://accounts.google.com",
			public string azp; // "331314739002-f3cp7u6p8dolhfmhalg6n8o3bl3lrmj3.apps.googleusercontent.com",
			public string aud; //"331314739002-f3cp7u6p8dolhfmhalg6n8o3bl3lrmj3.apps.googleusercontent.com",
			public string sub; // "105954597999371955582",
			public string email; // "amelchenkorv @gmail.com",
			public bool email_verified; // true,
			public string at_hash; // "v6_fw1MpvZkm8DrRX4_iDg",
			public string name; // "Roman Amelchenko",
			public string picture; // "https://lh3.googleusercontent.com/a/ACg8ocI-H9AohhrFpphb3S7y1fi8D_5iKHIo-QoWP0Qs4U2f93Dusw=s96-c",
			public string given_name; // "Roman",
			public string family_name; // "Amelchenko",
			public DateTime iat; // 1780259149,
			public DateTime exp; // 1780262749
		}

		private async Task<string> ExchangeCodeForToken(string code)
		{
			string tokenUrl = "https://oauth2.googleapis.com/token";

			// Create form data matching Google's required OAuth2 parameters
			Dictionary<string, string> formFields = new Dictionary<string, string>
		{
			{ "code", code },
			{ "client_id", clientId },
			{ "client_secret", clientSecret },
			{ "redirect_uri", GetRedirectUri() },
			{ "grant_type", "authorization_code" }
		};

			using UnityWebRequest webRequest = UnityWebRequest.Post(tokenUrl, formFields);
			// 3. Start the request payload transmission
			UnityWebRequestAsyncOperation operation = webRequest.SendWebRequest();

			// 4. Periodically yield control back to Unity until the server responds
			while (!operation.isDone)
			{
				await Task.Yield(); // Keeps the main thread perfectly fluid
			}

			// 5. Evaluate the HTTP request results
			string email = null;
			if (webRequest.result == UnityWebRequest.Result.Success)
			{
				var result = webRequest.downloadHandler.text;
				Debug.Log("Successfully Authenticated! Token Payload: " + result);
				TokenResponseCustom response = JsonUtility.FromJson<TokenResponseCustom>(result);
				userToken = response.id_token;
				Debug.Log($"Access Token: {userToken}");
				PlayerPrefs.SetString("userToken", userToken);

				var tokenDecode = JWTDecoder.Decoder.DecodeToken(userToken).Payload;
				TokenInfo tokenInfo = JsonUtility.FromJson<TokenInfo>(tokenDecode);
				email = tokenInfo.email;

				if (email != null && tokenInfo.email_verified == true)
				{
					// Proceed with user registration or login logic in your game
					Debug.Log($"Welcome back, {email}!");
				}
				// Parse JSON token data here to login to your backend or Firebase
			}
			else
			{
				Debug.LogError("Token Exchange Failed: " + webRequest.error);
			}
			return email;
		}

		public async Task<Session> SignInWithGoogleTask()
		{
			try
			{
				// We start setting up the client here
				// 1. Initialize your Supabase Client (if not already done globally)
				var url = SupabaseManager.SupabaseSettings.SupabaseURL;
				var key = SupabaseManager.SupabaseSettings.SupabaseAnonKey;
				var soptions = new SupabaseOptions { AutoRefreshToken = true, AutoConnectRealtime = true };
				var supabase = new Supabase.Client(url, key, soptions);
				await supabase.InitializeAsync();

				var options = new SignInOptions
				{
					FlowType = OAuthFlowType.PKCE,
					RedirectTo = url + "/auth/v1/callback"
				};

				var authState = await supabase.Auth.SignIn(Constants.Provider.Google, options);

				string pkceVerifier = authState.PKCEVerifier;

				var authUrl = authState.Uri.ToString();

				using var listener = new HttpListener();
				listener.Prefixes.Add("http://localhost:3000/auth/callback/");
				if (listener.IsListening) httpListener.Stop();
				listener.Start();
				Debug.Log("Listening for Google redirect... \n" + authUrl);

				Application.OpenURL(authUrl);

				// 4. Wait for the browser redirect context
				HttpListenerContext context = await listener.GetContextAsync();

				// 5. Parse the authorization code from the query string
				var code = context.Request.QueryString["code"];

				// 6. Send a friendly response back to the browser window
				HttpListenerResponse response = context.Response;
				string responseString = "<html><body><h1>Authentication successful!</h1><p>You can close this tab.</p></body></html>";
				byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
				response.ContentLength64 = buffer.Length;
				using Stream output = response.OutputStream;
				await output.WriteAsync(buffer, 0, buffer.Length);

				listener.Stop();
				if (!string.IsNullOrEmpty(code))
				{
					// Exchange the code and your saved PKCE Verifier for an official user session
					var session = await supabase.Auth.ExchangeCodeForSession(pkceVerifier, code);
					Debug.Log($"Successfully signed in! User Email: {session.User.Email}");
					return session;
				}
				return null;
			}
			catch (Exception ex)
			{
				Debug.LogError($"Authentication failed: {ex.Message}");
				return null;
			}
		}
	}
}
