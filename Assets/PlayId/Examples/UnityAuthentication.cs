using Newtonsoft.Json.Linq;
using PlayId.Scripts;
using PlayId.Scripts.Data;
using PlayId.Scripts.Enums;
using System;
using System.Text;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Authentication.PlayerAccounts;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.PlayId.Examples
{
	public class UnityAuthentication : MonoBehaviour
	{
		public Text Output;
		public YandexAuth yandexAuth;
		public YandexAuthCustomUri yandexAuthCustomUri;

		public WebViewAuthHandler webViewAuthHandler;

		private async void Awake()
		{
			// 1. Инициализируем сервисы Unity при запуске игры
			try
			{
				await UnityServices.InitializeAsync();
				Debug.Log("Unity Services успешно инициализированы.");

				await TryAutomaticSignInAsync();
				// Подписываемся на событие успешного входа в Player Accounts
				PlayerAccountService.Instance.SignedIn += OnPlayerAccountSignedIn;
				AuthenticationService.Instance.SignedIn += OnAuthenticationSignedIn;
			}
			catch (Exception e)
			{
				Debug.LogError($"Ошибка инициализации сервисов: {e.Message}");
			}
		}

		private void OnAuthenticationSignedIn()
		{
			Debug.Log("Успешный вход");
		}

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

			PlayIdServices.Instance.Auth.SignIn(OnSignIn, caching: false, platforms: Platform.Apple);

			void OnSignIn(bool success, string error, User user)
			{
				user.Internals.RequestIdTokenForPlatform(Platform.Apple, refresh: true, OnGetIdToken);
			}

			async void OnGetIdToken(bool success, string error, string idToken)
			{
				if (success)
				{
					await Unity.Services.Core.UnityServices.InitializeAsync();

					var authService = Unity.Services.Authentication.AuthenticationService.Instance;

					if (authService.IsSignedIn) authService.SignOut();

					await authService.SignInWithAppleAsync(idToken);

					Output.text = authService.IsAuthorized ? $"Player ID: {authService.PlayerInfo.Id}!" : "Unable to authorize.";
				}
				else
				{
					Output.text = error;
				}
			}
		}

		public void SignInWithYandexTest()
		{
			Output.text = "Please import Unity Authentication package and uncomment code below in Examples/UnityAuthentication.cs.";

			PlayIdServices.Instance.Auth.SignIn(OnSignIn, caching: false, platforms: Platform.Yandex);

			void OnSignIn(bool success, string error, User user)
			{
				//user.Internals.RequestIdTokenForPlatform(Platform.Yandex, refresh: true, OnGetIdToken);

				if (user != null && user.TokenResponse != null)
				{
					OnGetIdToken(success, error, user.TokenResponse.IdToken);
				}
				else
				{
					user.Internals.RequestIdTokenForPlatform(Platform.Yandex, refresh: true, OnGetIdToken);
				}
			}

			async void OnGetIdToken(bool success, string error, string idToken)
			{
				if (success)
				{
					await Unity.Services.Core.UnityServices.InitializeAsync();

					var authService = Unity.Services.Authentication.AuthenticationService.Instance;

					if (authService.IsSignedIn) authService.SignOut();

					await Unity.Services.Authentication.AuthenticationService.Instance.SignInWithOpenIdConnectAsync("oidc-yandex", idToken);

					Output.text = authService.IsAuthorized ? $"Player Yandex ID: {authService.PlayerInfo.Id}!" : "Unable to authorize.";
				}
				else
				{
					Output.text = error;
				}
			}
		}

		public async void SignInWithYandex()
		{
			await Unity.Services.Core.UnityServices.InitializeAsync();

			var authService = Unity.Services.Authentication.AuthenticationService.Instance;

			if (authService.IsSignedIn) authService.SignOut();

			var yandexSettings = Resources.Load<CustomAuthSettings>("YandexSettings");
#if UNITY_EDITOR
			string idToken = await new YandexAuth(yandexSettings).GetIdTokenFromYandex();
#else
			string idToken = await new YandexAuthCustomUri(yandexSettings).GetIdTokenFromYandex();
#endif
			try
			{
				await authService.SignInWithOpenIdConnectAsync("oidc-yandex", idToken);
				Output.text = authService.IsAuthorized ? $"Player Yandex ID: {authService.PlayerInfo.Id}!" : "Unable to authorize.";

				if (authService.IsAuthorized)
				{
					//var tokenResponse = Encoding.UTF8.GetString(Convert.FromBase64String(idToken));
					//PlayIdServices.Instance.Auth.OnTokenResponse(tokenResponse);
				}
			}
			catch (Exception ex)
			{
				Output.text = "Auth Yandex failed: " + ex.Message;
			}
		}

		public async void SignInWithYandexWebView()
		{
			await Unity.Services.Core.UnityServices.InitializeAsync();

			var authService = Unity.Services.Authentication.AuthenticationService.Instance;

			if (authService.IsSignedIn) authService.SignOut();

			webViewAuthHandler.OnClickLogin(SignInWithYandexWebViewCallback);
		}

		private async void SignInWithYandexWebViewCallback(string token)
		{
			var authService = Unity.Services.Authentication.AuthenticationService.Instance;

			try
			{
				await authService.SignInWithOpenIdConnectAsync("oidc-yandex", token);
				Output.text = authService.IsAuthorized ? $"Player Yandex ID: {authService.PlayerInfo.Id}!" : "Unable to authorize.";
			}
			catch (Exception ex)
			{
				Output.text = "Auth Yandex failed: " + ex.Message;
			}
		}

		// 2. Метод, который нужно повесить на кнопку интерфейса "Войти через Unity"
		public async void SignInWithUnityPlayer()
		{
			await Unity.Services.Core.UnityServices.InitializeAsync();

			var authService = Unity.Services.Authentication.AuthenticationService.Instance;

			if (authService.IsSignedIn) authService.SignOut();

			var playerService = PlayerAccountService.Instance;
			// Если игрок уже авторизован в системе Player Accounts, сразу выполняем вход в UGS
			if (playerService.IsSignedIn)
			{
				playerService.SignOut();
			}
			string message = "";
			try
			{
				// Открывает веб-браузер для ввода логина/пароля Unity аккаунта
				await PlayerAccountService.Instance.StartSignInAsync();
				//message = $"Player ID: {AuthenticationService.Instance.PlayerId}";
				//Debug.Log(message);
			}
			catch (Exception e)
			{
				message = $"Не удалось запустить окно авторизации: {e.Message}";
				Debug.LogError(message);
			}
			Output.text = message;
		}

		// 3. Обработчик события успешного входа на веб-странице Player Accounts
		private async void OnPlayerAccountSignedIn()
		{
			Debug.Log("Игрок успешно вошел в Unity Player Account. Обмениваем токен...");
			await SignInWithUnityAuthentication();
		}

		// 4. Обмен токена Player Accounts на сессию Unity Gaming Services
		private async Task SignInWithUnityAuthentication()
		{
			string error = "";
			try
			{
				// Получаем Access Token от Player Accounts
				string accessToken = PlayerAccountService.Instance.AccessToken;
				var authService = AuthenticationService.Instance;
				// Авторизуем игрока в основном сервисе аутентификации UGS
				await authService.SignInWithUnityAsync(accessToken);

				Output.text = authService.IsAuthorized ? $"Player ID: {authService.PlayerInfo.Id}!" : "Unable to authorize.";
				Debug.Log(error);
				Output.text = error;
				if (authService.IsAuthorized)
				{
					//var tokenResponse = Encoding.UTF8.GetString(Convert.FromBase64String(idToken));
					//PlayIdServices.Instance.Auth.OnTokenResponse(tokenResponse);
				}
				return;
			}
			catch (AuthenticationException ex)
			{
				error = $"Ошибка аутентификации UGS: {ex.Message}";
				Debug.LogError(error);
			}
			catch (Exception e)
			{
				error = $"Непредвиденная ошибка: {e.Message}";
				Debug.LogError(error);
			}
			Output.text = error;
		}

		private async Task TryAutomaticSignInAsync()
		{
			// Проверяем, заходил ли игрок ранее на этом устройстве
			string error = "";

			if (AuthenticationService.Instance.SessionTokenExists)
			{
				Debug.Log("[Auth] Найден сохраненный токен сессии. Пытаемся войти автоматически...");
				try
				{
					// Метод SignInAnonymouslyAsync в Unity автоматически подхватывает
					// существующий сессионный токен игрока, если он сохранен на устройстве,
					// и восстанавливает его полноценный профиль (Google или Логин/Пароль).
					await AuthenticationService.Instance.SignInAnonymouslyAsync();

					Output.text = $"Player ID: {AuthenticationService.Instance.PlayerId}!";
					Debug.Log($"[Auth] Авто-вход успешен! Игрок ID: {AuthenticationService.Instance.PlayerId}");
					return;
				}
				catch (AuthenticationException ex)
				{
					error = $"[Auth] Не удалось войти по токену сессии (возможно, он устарел): {ex.Message}";
					Debug.LogWarning(error);
				}
				catch (RequestFailedException ex)
				{
					error = $"[Auth] Сетевая ошибка при авто-входе: {ex.Message}";
					Debug.LogError(error);
					// Тут можно предложить игроку "Повторить попытку" или войти в автономном режиме
				}
			}
			else
			{
				error = "[Auth] Сохраненная сессия не найдена. Требуется ручной вход.";
				Debug.Log(error);
			}
			Output.text = error;
		}

		private void OnDestroy()
		{
			// Отписываемся от событий во избежание утечек памяти
			if (PlayerAccountService.Instance != null)
			{
				PlayerAccountService.Instance.SignedIn -= OnPlayerAccountSignedIn;
			}
		}
		
	}
}