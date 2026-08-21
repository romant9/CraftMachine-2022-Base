using Google;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.CloudCode;
using Unity.Services.Core;
using UnityEngine;

namespace UnityAuth
{
	public class UnityAuthManager : MonoBehaviour
	{
		public static UnityAuthManager Instance;
		private const string webClientId = "331314739002-f3cp7u6p8dolhfmhalg6n8o3bl3lrmj3.apps.googleusercontent.com";
		[SerializeField]
		private string userName;
		[SerializeField]
		private string password;

		private string googleIdToken;
		private string googleMail;

		private void Awake()
		{
			Instance = this;
		}

		private void Start()
		{
			Initialize();
		}

		public async void Initialize()
		{
			if (OfflineManager.UseSupabase) return;
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
			catch (RequestFailedException ex)
			{
				// Сюда попадут только общие сетевые сбои (например, у пользователя пропал интернет посреди запроса)
				Debug.LogError($"[Link] Сетевая ошибка сервера: {ex.Message} (Код: {ex.ErrorCode})");
				OnLinkFailed("Проблема со связью с сервером. Попробуйте позже.");
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

		public async Task LinkUsernamePasswordAsync(string password)
		{
			// 1. Проверяем авторизацию в Unity
			if (!AuthenticationService.Instance.IsSignedIn)
			{
				OnLinkFailed("Вы не авторизованы в системе.");
				return;
			}

			// ПРОВЕРКА 2: Получаем Email. Если в кэше пусто (например, после авто-входа), дергаем Silent Sign-In
			if (string.IsNullOrEmpty(googleMail))
			{
				try
				{
					Debug.Log("[Link] Кэш пуст. Запрашиваем данные у Google в фоне...");
					// Нативный метод плагина для бесшумного получения данных текущего юзера Google
					GoogleSignInUser silentUser = await GoogleSignIn.DefaultInstance.SignInSilently();
					if (silentUser != null)
					{
						googleMail = silentUser.Email;
					}
				}
				catch (Exception ex)
				{
					Debug.LogError($"[Link] Не удалось получить Email через SignInSilently: {ex.Message}");
				}
			}

			// Если и фоновый запрос не вернул почту, прерываем
			if (string.IsNullOrEmpty(googleMail))
			{
				OnLinkFailed("Не удалось получить Email от сервисов Google. Требуется ручной перезапуск окна входа.");
				return;
			}

			try
			{
				Debug.Log($"[Link] Email успешно подтянут: {googleMail}. Привязываем пароль...");
				await AuthenticationService.Instance.AddUsernamePasswordAsync(googleMail, password);

				Debug.Log("[Link] Пароль успешно добавлен к вашей учетной записи Google!");
				OnLinkSuccess("Username/Password");
			}
			catch (AuthenticationException ex)
			{
				if (ex.ErrorCode == AuthenticationErrorCodes.AccountAlreadyLinked)
				{
					OnLinkFailed($"Этот Google Email ({googleMail}) уже занят другим игровым профилем.");
				}
				else if (ex.ErrorCode == AuthenticationErrorCodes.InvalidParameters)
				{
					OnLinkFailed("Пароль слишком простой. Нужна заглавная буква, цифра и спецсимвол.");
				}
				else
				{
					OnLinkFailed($"Ошибка SDK: {ex.Message}");
				}
			}
			catch (RequestFailedException)
			{
				OnLinkFailed("Ошибка соединения с сервером Unity Cloud.");
			}
		}

		/// <summary>
		/// Изменить текущий пароль игрока в Unity Cloud
		/// </summary>
		/// <param name="currentPassword">Старый (текущий) пароль</param>
		/// <param name="newPassword">Новый придуманный пароль</param>
		public async Task ChangePasswordAsync(string currentPassword, string newPassword)
		{			
			// 1. Проверяем, вошел ли вообще игрок в систему
			if (!AuthenticationService.Instance.IsSignedIn)
			{
				Debug.LogError("[Password] Ошибка: Нельзя изменить пароль, если вы не авторизованы.");
				OnChangePasswordFailed("Вы не авторизованы в системе.");
				return;
			}

			// Локальная экспресс-проверка совпадения старого и нового пароля
			if (currentPassword == newPassword)
			{
				OnChangePasswordFailed("Новый пароль не должен совпадать со старым.");
				return;
			}

			try
			{
				Debug.Log("[Password] Отправка запроса на смену пароля...");

				// 2. Вызываем метод SDK для обновления пароля
				await AuthenticationService.Instance.UpdatePasswordAsync(currentPassword, newPassword);

				Debug.Log("[Password] Пароль успешно изменен в Unity Cloud!");
				OnChangePasswordSuccess();
			}
			catch (AuthenticationException ex)
			{
				// Перехватываем специфические ошибки валидации со стороны SDK Unity
				if (ex.ErrorCode == AuthenticationErrorCodes.InvalidParameters)
				{
					// Неверный формат нового пароля или не совпал старый пароль
					Debug.LogWarning($"[Password] Ошибка параметров (Код: {ex.ErrorCode}): {ex.Message}");
					OnChangePasswordFailed("Неверный текущий пароль или новый пароль не соответствует правилам безопасности Unity (нужна заглавная буква, цифра и спецсимвол).");
				}
				else
				{
					Debug.LogError($"[Password] Ошибка SDK при смене пароля: {ex.Message} (Код: {ex.ErrorCode})");
					OnChangePasswordFailed($"Ошибка авторизации: {ex.Message}");
				}
			}
			catch (RequestFailedException ex)
			{
				// Сетевые ошибки или ошибки сервера (например, если старый пароль указан неверно, сервер также вернет ошибку запроса)
				Debug.LogWarning($"[Password] Ошибка сервера: {ex.Message} (Код: {ex.ErrorCode})");
				OnChangePasswordFailed("Не удалось обновить пароль. Проверьте правильность ввода текущего пароля.");
			}
			catch (Exception ex)
			{
				Debug.LogError($"[Password] Непредвиденная системная ошибка: {ex.Message}");
				OnChangePasswordFailed(ex.Message);
			}
		}

		#region Restore Password
		// Ответ сервера при успешном запросе PIN-кода (Режим "request")
		[Serializable]
		public class CloudCodeRequestResponse
		{
			public bool success;
			public string message;
			public string debugPin; // Будет содержать PIN только при тестах в Dashboard
		}

		// Ответ сервера при успешном сбросе пароля (Режим "confirm")
		[Serializable]
		public class CloudCodeConfirmResponse
		{
			public bool success;
			public string message;
		}
		// Имя нашего монолитного скрипта, которое мы указали в Dashboard
		private const string CloudScriptEndpoint = "PasswordRecoveryManager";

		/// <summary>
		/// ЭТАП 1: Запросить PIN-код восстановления доступа на указанную почту
		/// </summary>
		public async Task<CloudCodeRequestResponse> RequestPasswordResetAsync(string email)
		{
			try
			{
				Debug.Log($"[Client -> Cloud] Запрос PIN для почты: {email}...");

				// Готовим параметры для передачи в JS-скрипт (имена ключей должны строго совпадать с params в JS!)
				var requestParameters = new Dictionary<string, object>
			{
				{ "action", "request" },
				{ "email", email }
			};

				// Вызываем облачную функцию и сразу десериализуем ответ в класс CloudCodeRequestResponse
				CloudCodeRequestResponse response = await CloudCodeService.Instance.CallEndpointAsync<CloudCodeRequestResponse>(
					CloudScriptEndpoint,
					requestParameters
				);

				Debug.Log($"[Cloud -> Client] Сервер обработал запрос. Статус: {response.message}");
				return response;
			}
			catch (CloudCodeException ex)
			{
				Debug.LogError($"[CloudCode Error] Не удалось вызвать скрипт: {ex.Message} (Код: {ex.ErrorCode})");
				return null;
			}
			catch (Exception ex)
			{
				Debug.LogError($"[System Error] Ошибка запроса восстановления: {ex.Message}");
				return null;
			}
		}

		/// <summary>
		/// ЭТАП 2: Передать PIN-код и новый пароль для принудительного сброса в базе UGS
		/// </summary>
		public async Task<bool> ConfirmPasswordResetAsync(string email, string pinCode, string newPassword)
		{
			try
			{
				Debug.Log($"[Client -> Cloud] Отправка PIN-кода на верификацию для: {email}...");

				// Готовим параметры подтверждения для режима "confirm"
				var confirmParameters = new Dictionary<string, object>
			{
				{ "action", "confirm" },
				{ "email", email },
				{ "pinCode", pinCode },
				{ "newPassword", newPassword }
			};

				// Вызываем тот же эндпоинт, но с другими параметрами
				CloudCodeConfirmResponse response = await CloudCodeService.Instance.CallEndpointAsync<CloudCodeConfirmResponse>(
					CloudScriptEndpoint,
					confirmParameters
				);

				if (response != null && response.success && response.message == "PASSWORD_RESET_SUCCESS")
				{
					Debug.Log("[Cloud -> Client] Доступ успешно восстановлен! Пароль изменен в UGS.");
					return true;
				}

				return false;
			}
			catch (CloudCodeException ex)
			{
				// Сюда приложение зайдет, если JS-скрипт выбросил throw new Error("INVALID_PIN_CODE") или аналогичную
				Debug.LogWarning($"[CloudCode Rejected] Сервер отклонил сброс пароля: {ex.Message}");
				return false;
			}
			catch (Exception ex)
			{
				Debug.LogError($"[System Error] Непредвиденная ошибка на этапе подтверждения: {ex.Message}");
				return false;
			}
		}
		#endregion

		// --- Коллбэки для UI интерфейса ---

		private void OnChangePasswordSuccess()
		{
			// ТУТ ВАШ КОД: Показать окно "Пароль успешно изменен!"
			Debug.Log("[UI] Оповещаем игрока об успешной смене пароля.");
		}

		private void OnChangePasswordFailed(string errorMessage)
		{
			// ТУТ ВАШ КОД: Показать ошибку (например, "Неверный старый пароль")
			Debug.Log($"[UI] Ошибка смены пароля: {errorMessage}");
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
		public async void SignInWithGoogle()
		{
			// Конфигурируем запрос к Google
			GoogleSignIn.Configuration = new GoogleSignInConfiguration
			{
				WebClientId = webClientId,
				RequestIdToken = true,
				RequestEmail = true
			};

			try
			{
				Debug.Log("Запуск окна выбора аккаунта Google...");

				// Ждем ответа от Google. Поток Unity при этом НЕ замораживается!
				GoogleSignInUser googleUser = await GoogleSignIn.DefaultInstance.SignIn();

				// Как только мы прошли await, мы СНОВА находимся в основном потоке Unity!
				if (googleUser != null)
				{
					googleIdToken = googleUser.IdToken;
					googleMail = googleUser.Email;
					var userID = googleUser.UserId;
					var userName = googleUser.DisplayName;

					Debug.Log($"Google вернул данные для: {userName} ({googleMail})");

					// Абсолютно безопасно вызываем метод авторизации Unity
					OnGoogleButtonClick(googleIdToken);
				}
			}
			catch (System.Exception ex)
			{
				// Сюда попадут и отмена пользователем, и ошибки сети
				Debug.LogError($"Ошибка вызова Google Sign-In: {ex.Message}");
			}
		}

  //      public void SignInWithGoogle()
		//{
		//	// Конфигурируем запрос к Google, явно запрашивая ID Token
		//	GoogleSignIn.Configuration = new GoogleSignInConfiguration
		//	{
		//		WebClientId = webClientId,
		//		RequestIdToken = true, // ОБЯЗАТЕЛЬНО: просим именно ID токен
		//		RequestEmail = true
		//	};

		//	// Запуск окна выбора аккаунта Google
		//	GoogleSignIn.DefaultInstance.SignIn().ContinueWith(task =>
		//	{
		//		if (task.IsFaulted)
		//		{
		//			Debug.LogError("Ошибка вызова Google Sign-In");
		//		}
		//		else if (task.IsCanceled)
		//		{
		//			Debug.Log("Вход отменен пользователем");
		//		}
		//		else
		//		{
		//			// Успешно получили данные от Google
		//			GoogleSignInUser googleUser = task.Result;

		//			// Вот он — нужный нам токен!
		//			googleIdToken = googleUser.IdToken;
		//			// ЗАПОМИНАЕМ EMAIL: Сохраняем почту в локальный кэш менеджера
		//			googleMail = googleUser.Email;
		//			var userID = googleUser.UserId;
		//			var userName = googleUser.DisplayName;
		//			// Передаем его в поток Unity (желательно выполнять в основном потоке Unity)
		//			OnGoogleButtonClick(googleIdToken);
		//		}
		//	});
		//}

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
