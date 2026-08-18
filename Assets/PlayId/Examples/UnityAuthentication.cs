using Assets.PlayId.Scripts;
using Assets.PlayId.Scripts.Data;
using Assets.PlayId.Scripts.Enums;
using System;
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

		public void SignInWithYandex()
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
					user.Internals.RequestIdTokenForPlatform(Platform.Any, refresh: true, OnGetIdToken);
				}
			}

			async void OnGetIdToken(bool success, string error, string idToken)
			{
				if (success)
				{
					await Unity.Services.Core.UnityServices.InitializeAsync();

					var authService = Unity.Services.Authentication.AuthenticationService.Instance;

					if (authService.IsSignedIn) authService.SignOut();

					await authService.SignInWithOpenIdConnectAsync("oidc-yandex", idToken);

					Output.text = authService.IsAuthorized ? $"Player Yandex ID: {authService.PlayerInfo.Id}!" : "Unable to authorize.";
				}
				else
				{
					Output.text = error;
				}
			}
		}

		public async void SignInWithYandex2()
		{
			await Unity.Services.Core.UnityServices.InitializeAsync();

			var authService = Unity.Services.Authentication.AuthenticationService.Instance;

			if (authService.IsSignedIn) authService.SignOut();

			string idToken = await yandexAuth.GetIdTokenFromYandexViaWebViewAsync();

			try
			{
				await authService.SignInWithOpenIdConnectAsync("oidc-yandex", idToken);
				Output.text = authService.IsAuthorized ? $"Player Yandex ID: {authService.PlayerInfo.Id}!" : "Unable to authorize.";

			}
			catch (Exception ex)
			{
				Debug.LogError("Auth Yandex failed: " + ex.Message);
			}
		}


		public void SignInUnityPlayer()
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

                    //

					await authService.SignInWithGoogleAsync(idToken);

					Output.text = authService.IsAuthorized ? $"Player ID: {authService.PlayerInfo.Id}!" : "Unable to authorize.";
				}
				else
				{
					Output.text = error;
				}
			}
		}

		private async void Awake()
		{
			// 1. Инициализируем сервисы Unity при запуске игры
			try
			{
				await UnityServices.InitializeAsync();
				Debug.Log("Unity Services успешно инициализированы.");

				//await TryAutomaticSignInAsync();
				// Подписываемся на событие успешного входа в Player Accounts
				//PlayerAccountService.Instance.SignedIn += OnPlayerAccountSignedIn;
			}
			catch (Exception e)
			{
				Debug.LogError($"Ошибка инициализации сервисов: {e.Message}");
			}
		}

		// 2. Метод, который нужно повесить на кнопку интерфейса "Войти через Unity"
		public async void StartLoginFlow()
		{
			// Если игрок уже авторизован в системе Player Accounts, сразу выполняем вход в UGS
			if (PlayerAccountService.Instance.IsSignedIn)
			{
				await SignInWithUnityAuthentication();
				return;
			}

			try
			{
				// Открывает веб-браузер для ввода логина/пароля Unity аккаунта
				await PlayerAccountService.Instance.StartSignInAsync();
			}
			catch (Exception e)
			{
				Debug.LogError($"Не удалось запустить окно авторизации: {e.Message}");
			}
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
			try
			{
				// Получаем Access Token от Player Accounts
				string accessToken = PlayerAccountService.Instance.AccessToken;

				// Авторизуем игрока в основном сервисе аутентификации UGS
				await AuthenticationService.Instance.SignInWithUnityAsync(accessToken);

				Debug.Log($"[Успех] Игрок полностью авторизован в UGS!");
				Debug.Log($"Player ID: {AuthenticationService.Instance.PlayerId}");
				Output.text = $"Player ID: {AuthenticationService.Instance.PlayerInfo.Id}!";
			}
			catch (AuthenticationException ex)
			{
				Debug.LogError($"Ошибка аутентификации UGS: {ex.Message}");
			}
			catch (Exception e)
			{
				Debug.LogError($"Непредвиденная ошибка: {e.Message}");
			}
		}

		private async Task TryAutomaticSignInAsync()
		{
			// Проверяем, заходил ли игрок ранее на этом устройстве
			if (AuthenticationService.Instance.SessionTokenExists)
			{
				Debug.Log("[Auth] Найден сохраненный токен сессии. Пытаемся войти автоматически...");

				try
				{
					// Метод SignInAnonymouslyAsync в Unity автоматически подхватывает 
					// существующий сессионный токен игрока, если он сохранен на устройстве,
					// и восстанавливает его полноценный профиль (Google или Логин/Пароль).
					await AuthenticationService.Instance.SignInAnonymouslyAsync();

					Debug.Log($"[Auth] Авто-вход успешен! Игрок ID: {AuthenticationService.Instance.PlayerId}");
				}
				catch (AuthenticationException ex)
				{
					Debug.LogWarning($"[Auth] Не удалось войти по токену сессии (возможно, он устарел): {ex.Message}");
				}
				catch (RequestFailedException ex)
				{
					Debug.LogError($"[Auth] Сетевая ошибка при авто-входе: {ex.Message}");
					// Тут можно предложить игроку "Повторить попытку" или войти в автономном режиме
				}
			}
			else
			{
				Debug.Log("[Auth] Сохраненная сессия не найдена. Требуется ручной вход.");
			}
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