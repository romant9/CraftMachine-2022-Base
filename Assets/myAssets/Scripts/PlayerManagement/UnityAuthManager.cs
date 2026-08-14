using Google;
using System;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using static UnityAuth.TaskResult;
using TaskStatus = UnityAuth.TaskResult.TaskStatus;

namespace UnityAuth
{
	public class TaskResult
	{
		public enum TaskStatus
		{
			Success,
			Fail,
			NeedAuth,
			Error,
			Offline,
			Exception
		}

		public TaskStatus Status { get; set; }
		public string Message { get; set; }
		public Exception Exception { get; set; }

		public TaskResult(TaskStatus status, string message, Exception ex = null) 
		{ 
			Status = status; Message = message; Exception = ex;
			if (ex == null)
			{
				DebugTWD.Log($"[{status}]{message}");
			}
			else
			{
				DebugTWD.LogError($"[{status}]{message}\n{ex.Message}");
			}
		}
	}

	public class UnityAuthManager : MonoBehaviour
	{
		public static UnityAuthManager Instance;
		private const string webClientId = "331314739002-f3cp7u6p8dolhfmhalg6n8o3bl3lrmj3.apps.googleusercontent.com";
		[SerializeField]
		private string userName;
		[SerializeField]
		private string password;

		private void Awake()
		{
			Instance = this;
		}

		private void Start()
		{
			Initialize();
		}

		private async void Initialize()
		{
			// 1. Обязательно инициализируем сервисы при старте сцены
			try
			{
				await UnityServices.InitializeAsync();
				Debug.Log("[Auth] Unity Services успешно инициализированы.");

				// 2. Сразу запускаем проверку на автоматический вход
				await TryAutomaticSignInAsync();
			}
			catch (Exception e)
			{
				Debug.LogError($"[Auth] Ошибка инициализации Unity Services: {e.Message}");
			}
		}

		/// <summary>
		/// Попытка автоматического входа по сохраненному токену сессии
		/// </summary>
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
					OnAuthSuccess();
				}
				catch (AuthenticationException ex)
				{
					Debug.LogWarning($"[Auth] Не удалось войти по токену сессии (возможно, он устарел): {ex.Message}");
					OnAuthRequired(); // Токен протух, показываем экран логина
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
				OnAuthRequired();
			}
		}
		#region Callback
		private void OnAuthSuccess()
		{
			Debug.Log("[UI] Переходим в главное меню игры...");
		}

		private void OnAuthRequired()
		{
			Debug.Log("[UI] Показываем игроку форму авторизации...");
		}

		private void OnLinkSuccess(string providerName)
		{
			Debug.Log($"[UI] Оповещаем игрока об успешной привязке {providerName}");
		}

		private void OnLinkFailed(string errorMessage)
		{
			Debug.Log($"[UI] Показываем ошибку связывания: {errorMessage}");
		}

		private void OnUnlinkSuccess(string providerName)
		{
			Debug.Log($"[UI] Оповещаем игрока об успешной отвязке {providerName}");
		}

		private void OnUnlinkFailed(string errorMessage)
		{
			Debug.Log($"[UI] Показываем ошибку отвязывания: {errorMessage}");
		}
		#endregion

		/// <summary>
		/// Привязать Google-аккаунт к текущему вошедшему профилю.
		/// </summary>
		/// <param name="googleIdToken">Токен, полученный от плагина Google Sign-In</param>
		public async Task LinkGoogleAccountAsync(string googleIdToken)
		{
			// Проверяем, вошел ли вообще игрок в систему
			if (!AuthenticationService.Instance.IsSignedIn)
			{
				Debug.LogError("[Link] Ошибка: Нельзя привязать аккаунт, если вы не вошли в игровой профиль.");
				return;
			}

			try
			{
				Debug.Log("[Link] Попытка привязать Google к текущему профилю...");

				// Вызываем специальный метод связывания вместо обычного входа
				await AuthenticationService.Instance.LinkWithGoogleAsync(googleIdToken);

				Debug.Log("[Link] Google-аккаунт успешно привязан!");
				OnLinkSuccess("Google");
			}
			catch (AuthenticationException ex)
			{
				if (ex.ErrorCode == AuthenticationErrorCodes.AccountAlreadyLinked)
				{
					// Важный кейс: этот Google-аккаунт уже занят другим игроком в вашей базе данных!
					Debug.LogError("[Link] Этот Google-аккаунт уже привязан к другому игровому профилю.");
					OnLinkFailed("Этот Google-аккаунт уже используется другим игроком.");
				}
				else
				{
					Debug.LogError($"[Link] Ошибка аутентификации Unity при связывании: {ex.Message}");
					OnLinkFailed(ex.Message);
				}
			}
			catch (Exception ex)
			{
				Debug.LogError($"[Link] Непредвиденная ошибка связывания: {ex.Message}");
				OnLinkFailed(ex.Message);
			}
		}

		/// <summary>
		/// Отвязать провайдер от текущего профиля по его ID (например, "google.com")
		/// </summary>
		/// <param name="providerId">Строковый идентификатор провайдера</param>
		public async Task UnlinkProviderAsync(string providerId)
		{
			// 1. Проверяем, вошел ли игрок в систему
			if (!AuthenticationService.Instance.IsSignedIn)
			{
				Debug.LogError("[Unlink] Ошибка: Нельзя отвязать провайдер, если вы не авторизованы.");
				OnUnlinkFailed("Вы не авторизованы в системе.");
				return;
			}

			try
			{
				Debug.Log($"[Unlink] Начало процесса отвязки для провайдера: {providerId}...");

				// 2. Распределяем логику в зависимости от переданного ID провайдера
				await AuthenticationService.Instance.UnlinkGoogleAsync();

				// 3. Если метод выше выполнился без ошибок — отвязка успешна
				Debug.Log($"[Unlink] Провайдер {providerId} успешно отвязан от облачного профиля.");
				OnUnlinkSuccess(providerId);
			}
			catch (AuthenticationException ex)
			{
				// Перехватываем специфические ошибки Unity Cloud
				string userFriendlyError = "Не удалось отключить аккаунт.";

				// Распространенный кейс: попытка отвязать единственный метод входа
				if (ex.ErrorCode == AuthenticationErrorCodes.InvalidParameters)
				{
					userFriendlyError = "Нельзя отключить этот способ входа, так как он остался единственным для вашего аккаунта.";
				}

				Debug.LogError($"[Unlink] Ошибка Unity Authentication (Код: {ex.ErrorCode}): {ex.Message}");
				OnUnlinkFailed(userFriendlyError);

				// Пробрасываем исключение дальше, чтобы вызвать catch в UI контроллере
				throw;
			}
			catch (Exception ex)
			{
				// Перехватываем системные ошибки или наши кастомные исключения из switch
				Debug.LogError($"[Unlink] Системная ошибка при отвязке: {ex.Message}");
				OnUnlinkFailed(ex.Message);
				throw;
			}
		}

		/// <summary>
		/// Привязать Логин/Пароль к текущему профилю (например, если игрок зашел через Google и хочет создать пароль для ПК-версии).
		/// </summary>
		public async Task LinkUsernamePasswordAsync(string username, string password)
		{
			if (!AuthenticationService.Instance.IsSignedIn)
			{
				Debug.LogError("[Link] Ошибка: Вы должны быть авторизованы.");
				return;
			}

			try
			{
				Debug.Log("[Link] Попытка привязать Логин/Пароль...");

				await AuthenticationService.Instance.AddUsernamePasswordAsync(username, password);

				Debug.Log("[Link] Логин и Пароль успешно привязаны к профилю!");
				OnLinkSuccess("Username/Password");
			}
			catch (AuthenticationException ex)
			{
				if (ex.ErrorCode == AuthenticationErrorCodes.AccountAlreadyLinked)
				{
					Debug.LogError("[Link] Такой логин уже занят в системе.");
					OnLinkFailed("Данное имя пользователя уже занято.");
				}
				else
				{
					Debug.LogError($"[Link] Ошибка связывания пароля: {ex.Message}");
					OnLinkFailed(ex.Message);
				}
			}
			catch (Exception ex)
			{
				Debug.LogError($"[Link] Ошибка: {ex.Message}");
				OnLinkFailed(ex.Message);
			}
		}

		[ContextMenu("SignOut")]
		/// <summary>
		/// Выход из аккаунта (вызывать, если игрок нажал кнопку "Выйти" в настройках)
		/// </summary>
		public void SignOut()
		{
			if (AuthenticationService.Instance.IsSignedIn)
			{
				// Очищает локальный токен сессии на устройстве
				AuthenticationService.Instance.SignOut(clearCredentials: true);
				Debug.Log("[Auth] Игрок вышел из аккаунта. Токен сессии удален.");
				OnAuthRequired();
			}
		}

		// 1. Вход по Логину и Паролю
		public async Task<TaskResult> SignInWithPasswordAsync(string username, string password)
		{
			TaskStatus taskStatus;
			string message = string.Empty;
			Exception ex = null;
			try
			{
				await AuthenticationService.Instance.SignInWithUsernamePasswordAsync(username, password);
				taskStatus = TaskStatus.Success;
				message = $"Вход успешен! Игрок ID: {AuthenticationService.Instance.PlayerId}";
			}
			catch (AuthenticationException e)
			{
				message = $"Ошибка авторизации: {e.Message}";
				taskStatus = TaskStatus.Exception;
			}
			return new TaskResult(taskStatus, message, ex);
		}

		[ContextMenu("SignInWith Google")]
		public void SignInWithGoogle()
		{
			// Конфигурируем запрос к Google, явно запрашивая ID Token
			GoogleSignIn.Configuration = new GoogleSignInConfiguration
			{
				WebClientId = webClientId,
				RequestIdToken = true, // ОБЯЗАТЕЛЬНО: просим именно ID токен
				RequestEmail = true
			};

			// Запуск окна выбора аккаунта Google
			GoogleSignIn.DefaultInstance.SignIn().ContinueWith(task =>
			{
				if (task.IsFaulted)
				{
					Debug.LogError("Ошибка вызова Google Sign-In");
				}
				else if (task.IsCanceled)
				{
					Debug.Log("Вход отменен пользователем");
				}
				else
				{
					// Успешно получили данные от Google
					GoogleSignInUser googleUser = task.Result;

					// Вот он — нужный нам токен!
					string googleIdToken = googleUser.IdToken;

					// Передаем его в поток Unity (желательно выполнять в основном потоке Unity)
					OnGoogleButtonClick(googleIdToken);
				}
			});
		}

		public bool CheckIfGoogleIsLinked()
		{
			// Получаем информацию о текущем игроке
			PlayerInfo playerInfo = AuthenticationService.Instance.PlayerInfo;

			if (playerInfo != null && playerInfo.Identities != null)
			{
				foreach (var identity in playerInfo.Identities)
				{
					// В Unity Cloud идентификатор Google-провайдера называется "google.com", по логину/паролю - "usernamepassword"
					if (identity.TypeId == "google.com")
					{
						Debug.Log($"Google уже подключен! Внутренний ID пользователя в Google: {identity.UserId}");
						return true;
					}
				}
			}
			Debug.Log("Этот аккаунт не связан с Google (игрок вошел по Логину/Паролю).");
			return false;
		}

		public async Task<string> GetGoogleTokenSilentAsync()
		{
			string token = string.Empty;
			await GoogleSignIn.DefaultInstance.SignInSilently().ContinueWith(task =>
			{
				if (!task.IsFaulted && !task.IsCanceled)
				{
					var player = task.Result.DisplayName;
					Debug.Log($"Получен свежий Google ID Token: {player}");
					token = task.Result.IdToken;
				}
			});
			return token;
		}

		public async Task<string> GetGoogleTokenAsync()
		{
			try
			{
				Debug.Log("[Google] Запуск окна авторизации Google...");

				// Обязательно используем await, чтобы дождаться окончания операции входа
				GoogleSignInUser googleUser = await GoogleSignIn.DefaultInstance.SignIn();

				if (googleUser != null && !string.IsNullOrEmpty(googleUser.IdToken))
				{
					Debug.Log("[Google] Токен успешно получен.");
					return googleUser.IdToken;
				}

				Debug.LogWarning("[Google] Пользователь вошел, но ID Токен оказался пуст.");
			}
			catch (AggregateException ex)
			{
				// Плагин Google Sign-In часто оборачивает свои ошибки в AggregateException
				foreach (var inner in ex.InnerExceptions)
				{
					if (inner is GoogleSignIn.SignInException signInEx)
					{
						Debug.LogError($"[Google] Ошибка Google Sign-In (Код {signInEx.Status}): {signInEx.Message}");
					}
					else
					{
						Debug.LogError($"[Google] Системная ошибка: {inner.Message}");
					}
				}
			}
			catch (Exception ex)
			{
				Debug.LogError($"[Google] Непредвиденная ошибка при получении токена: {ex.Message}");
			}
			return string.Empty;
		}

		// Метод авторизации, принимающий два коллбэка: на успех и на ошибку
		public async void SignInWithGoogle(string googleIdToken, Action onSuccess, Action<string> onFail)
		{
			try
			{
				// 1. Настраиваем SignInOptions
				var options = new SignInOptions { CreateAccount = true };

				// 2. Выполняем асинхронный запрос в облако Unity
				await AuthenticationService.Instance.SignInWithGoogleAsync(googleIdToken, options);

				// 3. Если строка выше выполнилась без ошибок — это УСПЕХ
				Debug.Log($"[Auth] Успешно! ID Игрока: {AuthenticationService.Instance.PlayerId}");

				// Вызываем коллбэк успешного выполнения
				onSuccess?.Invoke();
			}
			catch (AuthenticationException authException)
			{
				// Сюда попадают специфические ошибки авторизации Unity (например, токен протух)
				string errorMsg = $"Ошибка аутентификации Unity: {authException.Message} (Код: {authException.ErrorCode})";
				Debug.LogError(errorMsg);

				// Вызываем коллбэк неудачи
				onFail?.Invoke(errorMsg);
			}
			catch (RequestFailedException requestException)
			{
				// Сюда попадают сетевые ошибки (нет интернета, упали сервера)
				string errorMsg = $"Ошибка сети/запроса: {requestException.Message}";
				Debug.LogError(errorMsg);

				onFail?.Invoke(errorMsg);
			}
			catch (Exception generalException)
			{
				// Любые другие непредвиденные ошибки
				string errorMsg = $"Непредвиденная ошибка: {generalException.Message}";
				Debug.LogError(errorMsg);

				onFail?.Invoke(errorMsg);
			}
		}

		// Пример того, как этот метод вызывать из другого скрипта (например, по нажатию на UI кнопку)
		public void OnGoogleButtonClick(string googleIdToken)
		{
			SignInWithGoogle(
				googleIdToken,
				onSuccess: () =>
				{
					// Логика перехода на сцену главного меню
					Debug.Log("Коллбэк: Игрок вошел, загружаем меню...");
					OnAuthSuccess();
				},
				onFail: (errorMessage) =>
				{
					// Логика показа окна "Ошибка входа" пользователю
					Debug.Log($"Коллбэк: Показываем UI ошибку: {errorMessage}");
				}
			);
		}
	}

}
