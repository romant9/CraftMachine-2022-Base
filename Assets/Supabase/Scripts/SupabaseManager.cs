using Newtonsoft.Json;
using Supabase.Gotrue;
using Supabase.Gotrue.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TwdCustomMod;
using UnityEngine;
using UnityEngine.Networking;
using User = Supabase.Gotrue.User;

namespace Supabase.TWD
{
	public class SupabaseManager : MonoBehaviour
	{
		public static SupabaseManager Instance;

		public SessionListener SessionListener = null;
		public SupabaseSessionHandler SessionHandler = null;
		public SupabaseSettings SupabaseSettings = null;

		private HttpListener httpListener;
		private const string LocalhostCallback = @"http://*:8080/auth/callback/";
			//@"http://localhost:8080/auth/callback/";
		private const string SupaCallback = @"https://ytzxmcoxbzppvoggmqnh.supabase.co/auth/v1/callback";

		[Multiline]
		public string ErrorText;
		public string ErrorTextRu;

		public bool IsGoogleBotRequest => !DataManager.Instance.IsVpnON;

		// Public in case other components are interested in network status
		private readonly NetworkStatus _networkStatus = new();

		public Client GetClient() => _client;
		private Client _client;

		public User GetUser() => _currentUser;
		private User _currentUser;

		public CMUser GetCMUser() => _currentCMUser;
		private CMUser _currentCMUser;

		public void SetCMUser(CMUser cmUser)
		{
			_currentCMUser = cmUser;
		}

		public TWDAccount GetTWDAccount() => _currentTWDAccount;
		private TWDAccount _currentTWDAccount;

		public void SetTWDAccount(TWDAccount account)
		{
			_currentTWDAccount = account;
		}

		public void SetUser(User user)
		{
			_currentUser = user;
		}


		public bool IsSignedIn { get; set; } // _currentUser != null
		public static bool IsOnline { get { try { return Instance.GetClient().Auth.Online; } catch { return false; } } }
		public static bool IsGoogleProvider { get; private set; }
		public static int errorsCount { get; set; }

		private void Awake()
		{
			DebugTWD.LogMycode("SupabaseManager Awake");

			if (Instance != null)
			{
				UnityEngine.Object.Destroy(base.gameObject);
				return;
			}
			Instance = this;
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		}

		private void Start()
		{
		}

		private void InjectHttpClientViaReflection(object targetModule, HttpClient customClient)
		{
			if (targetModule == null) return;

			Type type = targetModule.GetType();

			// Ищем поле по распространенным в этих библиотеках именам (зависит от конкретной версии под-пакета)
			FieldInfo httpClientField = type.GetField("httpClient", BindingFlags.NonPublic | BindingFlags.Instance)
										?? type.GetField("_httpClient", BindingFlags.NonPublic | BindingFlags.Instance)
										?? type.GetField("client", BindingFlags.NonPublic | BindingFlags.Instance);

			if (httpClientField != null)
			{
				// Подменяем старый HttpClient на наш новый
				httpClientField.SetValue(targetModule, customClient);
			}
			else
			{
				// Если поле не найдено как Field, проверяем приватные Свойства (Property)
				PropertyInfo httpClientProp = type.GetProperty("HttpClient", BindingFlags.NonPublic | BindingFlags.Instance)
											 ?? type.GetProperty("Client", BindingFlags.NonPublic | BindingFlags.Instance);

				if (httpClientProp != null && httpClientProp.CanWrite)
				{
					httpClientProp.SetValue(targetModule, customClient, null);
				}
				else
				{
					Debug.LogWarning($"[Reflection] Не удалось найти внутренний HttpClient в модуле {type.Name}");
				}
			}
		}

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

			public TaskResult(TaskStatus status, string message, Exception ex = null) { Status = status; Message = message; Exception = ex; }
		}

		public async Task<TaskResult> SetClient()
		{
			if (_currentUser != null) return new TaskResult(TaskResult.TaskStatus.Success, ErrorText);

			SupabaseOptions options = new()
			{
				AutoConnectRealtime = true,
				AutoRefreshToken = true    // ВАЖНО: включает обновление access-токена
										   // SessionHandler = new SupabaseSessionHandler()
			};

			_client = new(SupabaseSettings.SupabaseURL, SupabaseSettings.SupabaseAnonKey, options);

			// The first thing we do is attach the debug listener
			_client.Auth.AddDebugListener(DebugListener);

			// Next we set up the network status listener and tell it to turn the client online/offline
			_networkStatus.Client = _client.Auth;

			// Next we set up the session persistence - without this the client will forget the session
			// each time the app is restarted
			//_client.Auth.SetPersistence(new UnitySession());
			SessionHandler = new SupabaseSessionHandler();
			_client.Auth.SetPersistence(SessionHandler);

			// This will be called whenever the session changes
			_client.Auth.AddStateChangedListener(SessionListener.UnityAuthListener);

			// Создаем кастомный HttpClient на базе обработчика
			HttpClient customHttpClient = new HttpClient(new HttpClientHandler
			{
				// Отключаем авто-редиректы, чтобы VPN не подсовывал левые страницы авторизации
				AllowAutoRedirect = false,
				// Если ваш VPN подменяет SSL-сертификаты (MITM), можно временно разрешить их для тестов (осторожно в продакшене!)
				ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
			});
			//InjectHttpClientViaReflection(_client.Auth, customHttpClient);
			//InjectHttpClientViaReflection(_client.Postgrest, customHttpClient);
			//InjectHttpClientViaReflection(_client.Storage, customHttpClient);

			// Allow unconfirmed user sessions. If you turn this on you will have to complete the
			// email verification flow before you can use the session.
			//_client.Auth.Options.AllowUnconfirmedUserSessions = true;

			// We check the network status to see if we are online or offline using a request to fetch
			// the server settings from our project. Here's how we build that URL.
			string url = $"{SupabaseSettings.SupabaseURL}/auth/v1/settings?apikey={SupabaseSettings.SupabaseAnonKey}";
			try
			{
				// This will get the current network status
				_client.Auth.Online = await _networkStatus.StartAsync(url);
			}
			catch (NotSupportedException)
			{
				// Some platforms don't support network status checks, so we just assume we are online
				_client.Auth.Online = true;
			}
			catch (Exception e)
			{
				// Something else went wrong, so we assume we are offline
				ErrorText = "Supabase Error: " + e.Message;
				ErrorTextRu = "Ошибка Supabase: " + e.Message;
				errorsCount++;
				Debug.Log(e.Message, gameObject);

				_client.Auth.Online = false;
				return new TaskResult(TaskResult.TaskStatus.Exception, e.Message);
			}

			if (_client.Auth.Online)
			{
				// Предпочтение IPv4
				System.Net.ServicePointManager.DnsRefreshTimeout = 0;
				// Now we start up the client, which will in turn start up the background thread.
				// This will attempt to refresh the session token, which in turn may send a second
				// user login event to the UnityAuthListener.
				var initialization = await _client.InitializeAsync();
				Settings serverConfiguration = null;
				try
				{
					// Here we fetch the server settings and log them to the console
					serverConfiguration = await _client.Auth.Settings();
					Debug.Log($"Auto-confirm emails on this server: {serverConfiguration.MailerAutoConfirm}");
				}
				catch (UriFormatException ex)
				{
					ErrorText = $"Connection error. You need to turn the VPN off or on.\n {ex.Message}";
					ErrorTextRu = $"Ошибка подключения. Нужно отключить или включить VPN:\n {ex.Message}";
					errorsCount++;
					Debug.LogError(ex.Message);

					var email = UserPrefsKeys.User_Mail;
					var pass = UserPrefsKeys.User_Pass;
					if (!string.IsNullOrEmpty(email))
					{
						var resultFix = await CustomSignInWithVpnFix(email, pass);
						return resultFix;
					}
				}

				if (serverConfiguration == null)
				{
					var serverConfigurationJson = await GetSupabaseSettingsWithVpnFix(SupabaseSettings.SupabaseURL, SupabaseSettings.SupabaseAnonKey);
					serverConfiguration = JsonConvert.DeserializeObject<Settings>(serverConfigurationJson);
					Debug.Log($"Auto-confirm emails on this server: {serverConfiguration.MailerAutoConfirm}");
				}

				_client.Auth.LoadSession();

				if (_client.Auth.CurrentSession != null)
				{
					try
					{
						// Принудительно обновляем сессию через сервер Supabase, используя refresh_token
						var session = await _client.Auth.RetrieveSessionAsync();

						if (session != null)
						{
							var option = new UserIdentity() { Provider = "google" };
							IsGoogleProvider = session.User.Identities.Contains(option);

							ErrorText = $"Session {(IsGoogleProvider ? "Google" : "Mail")} successfully restored for {session.User.Email}!";
							ErrorTextRu = $"Сессия {(IsGoogleProvider ? "Google" : "Mail")} успешно восстановлена для {session.User.Email}!";

							Debug.Log(ErrorText);

							SetUser(session.User);

							IsSignedIn = true;

							return new TaskResult(TaskResult.TaskStatus.Success, ErrorText);
						}
						else
						{
							ErrorText = $"Failed to update the session. Try logging out and trying again";
							ErrorTextRu = $"Не удалось обновить сессию. Попробуйте произвести выход и повторить";

							Debug.Log(ErrorText);
							return new TaskResult(TaskResult.TaskStatus.NeedAuth, ErrorText);
						}
					}
					catch (Exception ex)
					{
						// Если refresh_token устарел или отозван, очищаем сессию
						ErrorText = $"Failed to update the session: {ex.Message}";
						ErrorTextRu = $"Не удалось обновить сессию: {ex.Message}";

						Debug.Log(ErrorText);
						return new TaskResult(TaskResult.TaskStatus.NeedAuth, ErrorText);
					}
				}
				else
				{
					ErrorText = "User is not logged in. Login via Google or email is required.";
					ErrorTextRu = "Пользователь не авторизован. Требуется вход через Google или почту.";

					Debug.Log(ErrorText);
					return new TaskResult(TaskResult.TaskStatus.NeedAuth, ErrorText);
				}
			}
			else
			{
				ErrorText = "Turn on the Internet to authorize in the mod";
				ErrorTextRu = "Включите интернет для авторизации в моде";

				Debug.Log(ErrorText);
				return new TaskResult(TaskResult.TaskStatus.Offline, ErrorText);
			}
		}

		private async Task<string> GetSupabaseSettingsWithVpnFix(string supabaseUrl, string anonKey)
		{
			// Очищаем базовый URL от возможных лишних слэшей
			string baseUrl = supabaseUrl.TrimEnd('/');

			// Формируем эндпоинт настроек
			string url = $"{baseUrl}/auth/v1/settings";

			// Для GET-запросов используем специальный фабричный метод Unity
			using (UnityWebRequest request = UnityWebRequest.Get(url))
			{
				// Передаем авторизационные заголовки Supabase
				request.SetRequestHeader("apikey", anonKey);
				request.SetRequestHeader("Authorization", $"Bearer {anonKey}");

				// Отправляем запрос через VPN-туннель
				var operation = request.SendWebRequest();
				while (!operation.isDone)
					await Task.Yield();

				if (request.result != UnityWebRequest.Result.Success)
				{
					Debug.LogError($"[Ошибка Settings] Код: {request.responseCode}. Текст: {request.downloadHandler.text}");
					return null;
				}

				string rawJsonResponse = request.downloadHandler.text;
				Debug.Log($"[Успех Settings] Конфигурация сервера получена: {rawJsonResponse}");

				return rawJsonResponse; // Возвращает чистый JSON без вылетов по UriFormatException!
			}
		}

		public async Task<TaskResult> CustomSignInWithVpnFix(string email, string password)
		{
			// Формируем тело запроса
			string jsonBody = $"{{\"email\":\"{email}\",\"password\":\"{password}\"}}";
			byte[] rawBody = Encoding.UTF8.GetBytes(jsonBody);
			//var anon = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6Inl0enhtY294YnpwcHZvZ2dtcW5oIiwicm9sZSI6ImFub24iLCJpYXQiOjE3Nzk1NTQ5MTQsImV4cCI6MjA5NTEzMDkxNH0.A0RY-ijc0JmWpIXDoGvUs00fA7YGR0JHNSqIiaznY18";

			//string url = $"{SupabaseSettings.SupabaseURL}/auth/v1/settings?apikey={SupabaseSettings.SupabaseAnonKey}";
			string loginUrl = $"{SupabaseSettings.SupabaseURL}/auth/v1/token?grant_type=password";
			using (UnityWebRequest request = new UnityWebRequest(loginUrl, "POST"))
			{
				request.uploadHandler = new UploadHandlerRaw(rawBody);
				request.downloadHandler = new DownloadHandlerBuffer();

				request.SetRequestHeader("Content-Type", "application/json");
				request.SetRequestHeader("apikey", SupabaseSettings.SupabaseAnonKey);
				request.SetRequestHeader("Authorization", $"Bearer {SupabaseSettings.SupabaseAnonKey}");

				var operation = request.SendWebRequest();
				while (!operation.isDone)
				{
					await Task.Yield();
				}

				// Если сетевой запрос завершился неудачей
				if (request.result != UnityWebRequest.Result.Success)
				{
					// Здесь мы поймаем то, что на самом деле возвращает ваш VPN вместо ответа Supabase!
					ErrorTextRu = $"[VPN Блокировка] Код ответа: {request.responseCode}. Текст ответа: {request.downloadHandler.text}";
					ErrorText = $"[VPN Block] Response code: {request.responseCode}. Message: {request.downloadHandler.text}";
					Debug.LogError(ErrorText);
					return new TaskResult(TaskResult.TaskStatus.Fail, ErrorText);
				}

				string rawJsonResponse = request.downloadHandler.text;
				Debug.Log($"[Успех] Получен чистый JSON: {rawJsonResponse}");

				try
				{
					// 2. ДЕСЕРИАЛИЗАЦИЯ: Заменяем ParseSession на стандартный Newtonsoft.Json
					Session session = JsonConvert.DeserializeObject<Session>(rawJsonResponse);

					if (session != null && !string.IsNullOrEmpty(session.AccessToken))
					{
						// 3. УСТАНОВКА СЕССИИ: Принудительно передаем токены в клиент Supabase
						// В версии 0.13.x это делается методом SetSession
						//await _client.Auth.SetSession(session.AccessToken, session.RefreshToken);

						Type authType = _client.Auth.GetType();

						// 1. Принудительно записываем объект сессии в приватное/публичное поле CurrentSession
						PropertyInfo currentSessionProp = authType.GetProperty("CurrentSession", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
						if (currentSessionProp != null && currentSessionProp.CanWrite)
						{
							currentSessionProp.SetValue(_client.Auth, session);
						}
						else
						{
							// Если свойство только для чтения, ищем приватное фоновое поле (обычно backing field)
							FieldInfo currentSessionField = authType.GetField("currentSession", BindingFlags.NonPublic | BindingFlags.Instance)
														 ?? authType.GetField("_currentSession", BindingFlags.NonPublic | BindingFlags.Instance);
							currentSessionField?.SetValue(_client.Auth, session);
						}

						// 2. Внедряем AccessToken в заголовки по умолчанию для всех дочерних модулей
						// Это самое важное, чтобы работали запросы к базе данных (Postgrest)
						if (_client.Postgrest != null)
						{
							// В версии 0.13.3 токены обычно передаются через свойства или методы и заголовки добавляются автоматически,
							// но мы можем явно обновить заголовки в HttpClient, который мы подменили ранее:
							_client.Postgrest.GetHeaders()["Authorization"] = $"Bearer {session.AccessToken}";
						}

						if (_client.Storage != null)
						{
							_client.Storage.GetHeaders()["Authorization"] = $"Bearer {session.AccessToken}";
						}

						var user = _client.Auth.CurrentSession?.User;
						if (user != null)
						{
							SetUser(user);

							IsSignedIn = true;

							Debug.Log("[Supabase] Сессия и токены принудительно внедрены в память в обход багов VPN!");

							ErrorTextRu = $"[Supabase] Авторизация успешно выполнена для {user.Email} в обход UriFormatException!";
							ErrorText = $"[Supabase] Authorization successful for {user}, bypassing UriFormatException!";

							Debug.Log(ErrorText);
							return new TaskResult(TaskResult.TaskStatus.Success, ErrorText);
						}
						else
						{
							ErrorText = $"Failed! Not Signed In. User with {email} does not exist";
							ErrorTextRu = $"Ошибка! Вход не выполнен. Пользователь с адресом {email} не существует";
							Debug.Log(ErrorText);

							return new TaskResult(TaskResult.TaskStatus.Fail, ErrorText);
						}						
					}
					else
					{
						ErrorTextRu = "[Supabase] Полученный JSON пустой или не содержит AccessToken.";
						ErrorText = "[Supabase] The received JSON is empty or does not contain an AccessToken.";
						Debug.LogError(ErrorText);
						return new TaskResult(TaskResult.TaskStatus.Fail, ErrorText);
					}				
				}
				catch (Exception ex)
				{
					ErrorTextRu = $"[Ошибка парсинга JSON] Сессия не распознана. Ошибка: {ex.Message}. Ответ сервера был: {rawJsonResponse}";
					ErrorText = $"[JSON parsing error] Session not recognized. Error: {ex.Message}. Server response was: {rawJsonResponse}";
					Debug.LogError(ErrorText);
					return new TaskResult(TaskResult.TaskStatus.Exception, ErrorText);
				}
			}
		}

		public async Task<TaskResult> SignUpTask(string mail, string pass)
		{
			if (IsSignedIn)
			{
				ErrorText = $"User {_currentUser.Email} is signedIn yet";
				ErrorTextRu = $"Пользователь {_currentUser.Email} уже вошел в систему";
				return new TaskResult(TaskResult.TaskStatus.Fail, ErrorText);
			}
			if (_client == null)
			{
				ErrorText = "The Supabase server is not connected. Try restarting the mod";
				ErrorTextRu = "Сервер Supabase не подключен. Попробуйте перезапуск мода";
				return new TaskResult(TaskResult.TaskStatus.Fail, ErrorText);
			}
			if (!_client.Auth.Online)
			{
				ErrorText = "The Supabase server is not connected. Try restarting the mod";
				ErrorTextRu = "Сервер Supabase не подключен. Попробуйте перезапуск мода";
				return new TaskResult(TaskResult.TaskStatus.Fail, ErrorText);
			}
			try
			{
				Session session = await _client.Auth.SignUp(mail, pass);
				if (session.User != null)
				{
					SetUser(session.User);
					ErrorText = $"Success! Signed Up as {session.User.Email}";
					ErrorTextRu = $"Успешно! Вы зарегистрировались как {session.User.Email}";
					IsSignedIn = true;
					return new TaskResult(TaskResult.TaskStatus.Success, ErrorText);
				}
				else
				{
					ErrorText = $"Failed! Not Signed Up. User with {mail} does not exist";
					ErrorTextRu = $"Ошибка! Регистрация не выполнена. Пользователь с адресом {mail} не существует.";
					return new TaskResult(TaskResult.TaskStatus.Fail, ErrorText);
				}
			}
			catch (GotrueException goTrueException)
			{
				ErrorText = $"{goTrueException.Reason}\n{goTrueException.Message}";
				ErrorTextRu = ErrorText;

				Debug.LogError(ErrorText);
				return new TaskResult(TaskResult.TaskStatus.Exception, ErrorText, goTrueException);
			}
			catch (UriFormatException ex)
			{
				ErrorText = $"Connection error. You need to turn the VPN off or on.\n {ex.Message}";
				ErrorTextRu = $"Ошибка подключения. Нужно отключить или включить VPN:\n {ex.Message}";
				errorsCount++;
				Debug.LogError(ErrorText);

				return new TaskResult(TaskResult.TaskStatus.Exception, ErrorText, ex);
			}
			catch (Exception e)
			{
				ErrorText = $"Unknown error: \n{e.Message}";
				ErrorTextRu = $"Неизвестная ошибка: \n{e.Message}";

				Debug.LogError(ErrorText);
				return new TaskResult(TaskResult.TaskStatus.Exception, ErrorText, e);
			}
		}

		public async Task<TaskResult> SignInTask(string mail, string pass)
		{
			if (IsSignedIn)
			{
				ErrorText = $"User {_currentUser.Email} is signedIn yet";
				ErrorTextRu = $"Пользователь {_currentUser.Email} уже вошел в систему";
				return new TaskResult(TaskResult.TaskStatus.Fail, ErrorText);
			}
			if (_client == null)
			{
				ErrorText = "The Supabase server is not connected. Try restarting the mod";
				ErrorTextRu = "Сервер Supabase не подключен. Попробуйте перезапуск мода";
				return new TaskResult(TaskResult.TaskStatus.Fail, ErrorText);
			}
			if (!_client.Auth.Online)
			{
				ErrorText = "Ошибка подключения к базе пользователей. Проверьте интернет";
				ErrorTextRu = "Error connecting to the user database. Check your internet connection";
				return new TaskResult(TaskResult.TaskStatus.Fail, ErrorText);
			}
			try
			{
				Session session = await _client.Auth.SignIn(mail, pass);
				if (session.User != null)
				{
					SetUser(session.User);
					ErrorText = $"Success! Signed In as {session.User.Email}";
					ErrorTextRu = $"Успешно! Вы вошли как {session.User.Email}";

					IsSignedIn = true;
					return new TaskResult(TaskResult.TaskStatus.Success, ErrorText);
				}
				else
				{
					ErrorText = $"Failed! Not Signed In. User with {mail} does not exist";
					ErrorTextRu = $"Ошибка! Вход не выполнен. Пользователь с адресом {mail} не существует";
					return new TaskResult(TaskResult.TaskStatus.Fail, ErrorText);
				}
			}
			catch (GotrueException goTrueException)
			{
				string prefixEn = "";
				string prefixRu = "";

				if (goTrueException.Reason.ToString() == "UserBadLogin")
				{
					prefixEn = $"User with {mail} does not exist!\nTry SignUp!";
					prefixRu = $"Пользователь с адресом {mail} не найден!\nПопробуйте зарегистрироваться";
				}
				ErrorText = $"{goTrueException.Reason}\n{prefixEn}\n{goTrueException.Message}";
				ErrorTextRu = $"{goTrueException.Reason}\n{prefixRu}\n{goTrueException.Message}";
				Debug.LogError(ErrorText);

				return new TaskResult(TaskResult.TaskStatus.Exception, ErrorText, goTrueException);
			}
			catch (UriFormatException ex)
			{
				ErrorText = $"Connection error. You need to turn the VPN off or on.\n {ex.Message}";
				ErrorTextRu = $"Ошибка подключения. Нужно отключить или включить VPN:\n {ex.Message}";
				errorsCount++;
				Debug.LogError(ErrorText);

				return new TaskResult(TaskResult.TaskStatus.Exception, ErrorText, ex);
			}
			catch (Exception e)
			{
				ErrorText = $"Unknown error: \n{e.Message}";
				ErrorTextRu = $"Неизвестная ошибка: \n{e.Message}";

				Debug.LogError(ErrorText);
				return new TaskResult(TaskResult.TaskStatus.Exception, ErrorText, e);
			}
		}

		// OnClick SignIn With Google Quick
		[ContextMenu("SignIn With Google Quick")]
		public async Task<TaskResult> SignInGoogleQuick()
		{
			if (IsSignedIn)
			{
				ErrorText = $"User {_currentUser.Email} is signedIn yet";
				ErrorTextRu = $"Пользователь {_currentUser.Email} уже вошел в систему";
				return new TaskResult(TaskResult.TaskStatus.Fail, ErrorText);
			}
			if (_client == null)
			{
				ErrorText = "The Supabase server is not connected. Try restarting the mod";
				ErrorTextRu = $"Сервер Supabase не подключен. Попробуйте перезапуск мода";
				return new TaskResult(TaskResult.TaskStatus.Fail, ErrorText);
			}
			if (!_client.Auth.Online)
			{
				ErrorText = $"Ошибка подключения к базе пользователей. Проверьте интернет";
				ErrorTextRu = "Error connecting to the user database. Check your internet connection";
				return new TaskResult(TaskResult.TaskStatus.Fail, ErrorText);
			}
			try
			{
				Session session = await SignInWithGoogleQuickTask();
				if (session != null)
				{
					SetUser(session.User);
					ErrorText = $"Success! Signed In Google as: {_currentUser.Email}";
					ErrorTextRu = $"Успешно! Вы вошли через Google как {session.User.Email}";
					Debug.Log(ErrorText);

					IsSignedIn = true;
					return new TaskResult(TaskResult.TaskStatus.Success, ErrorText);
				}
				else
				{
					ErrorText = $"Failed! Not Signed In. User does not exist";
					ErrorTextRu = $"Ошибка! Вход не выполнен. Пользователь не существует";
					return new TaskResult(TaskResult.TaskStatus.Fail, ErrorText);
				}
			}
			catch (UriFormatException ex)
			{
				ErrorText = $"Connection error. You need to turn the VPN off or on.\n {ex.Message}";
				ErrorTextRu = $"Ошибка подключения. Нужно отключить или включить VPN:\n {ex.Message}";
				errorsCount++;
				Debug.LogError(ErrorText);

				return new TaskResult(TaskResult.TaskStatus.Exception, ErrorText, ex);
			}
			catch (Exception e)
			{
				ErrorText = $"Unknown error: \n{e.Message}";
				ErrorTextRu = $"Неизвестная ошибка: \n{e.Message}";

				Debug.LogError(ErrorText);
				return new TaskResult(TaskResult.TaskStatus.Exception, ErrorText, e);
			}
		}
	
		[ContextMenu("SignIn With Google")]
		public async Task<TaskResult> SignInGoogle()
		{
			if (IsSignedIn)
			{
				ErrorText = $"User {_currentUser.Email} is signedIn yet";
				ErrorTextRu = $"Пользователь {_currentUser.Email} уже вошел в систему";
				return new TaskResult(TaskResult.TaskStatus.Fail, ErrorText);
			}
			if (_client == null)
			{
				ErrorText = "The Supabase server is not connected. Try restarting the mod";
				ErrorTextRu = $"Сервер Supabase не подключен. Попробуйте перезапуск мода";
				return new TaskResult(TaskResult.TaskStatus.Fail, ErrorText);
			}
			if (!_client.Auth.Online)
			{
				ErrorText = $"Ошибка подключения к базе пользователей. Проверьте интернет";
				ErrorTextRu = "Error connecting to the user database. Check your internet connection";
				return new TaskResult(TaskResult.TaskStatus.Fail, ErrorText);
			}
			try
			{
				Session session = await SignInWithGoogleTask();
				if (session != null)
				{
					SetUser(session.User);
					ErrorText = $"Success! Signed In Google as: {_currentUser.Email}";
					ErrorTextRu = $"Успешно! Вы вошли через Google как {session.User.Email}";
					Debug.Log(ErrorText);

					IsSignedIn = true;
					return new TaskResult(TaskResult.TaskStatus.Success, ErrorText);
				}
				else
				{
					ErrorText = $"Failed! Not Signed In. User does not exist";
					ErrorTextRu = $"Ошибка! Вход не выполнен. Пользователь не существует";
					return new TaskResult(TaskResult.TaskStatus.Fail, ErrorText);
				}
			}
			catch (UriFormatException ex)
			{
				ErrorText = $"Connection error. You need to turn the VPN off or on.\n {ex.Message}";
				ErrorTextRu = $"Ошибка подключения. Нужно отключить или включить VPN:\n {ex.Message}";
				errorsCount++;
				Debug.LogError(ErrorText);

				return new TaskResult(TaskResult.TaskStatus.Exception, ErrorText, ex);
			}
			catch (Exception e)
			{
				ErrorText = $"Unknown error: \n{e.Message}";
				ErrorTextRu = $"Неизвестная ошибка: \n{e.Message}";

				Debug.LogError(ErrorText);
				return new TaskResult(TaskResult.TaskStatus.Exception, ErrorText, e);
			}
		}

		private async Task<Session> SignInWithGoogleQuickTask()
		{
			StartLocalHttpServer();

			try
			{
				var options = new SignInOptions
				{
					FlowType = Constants.OAuthFlowType.PKCE,
					RedirectTo = SupaCallback
				};

				var authState = await _client.Auth.SignIn(Constants.Provider.Google, options);

				string pkceVerifier = authState.PKCEVerifier;

				var authUrl = authState.Uri.ToString();

				//using var listener = new HttpListener();
				//listener.Prefixes.Add(LocalhostCallback);
				//listener.Start();
				Debug.Log("Listening for Google redirect... \n" + authUrl);

				Application.OpenURL(authUrl);

				// 4. Wait for the browser redirect context
				HttpListenerContext context = await httpListener.GetContextAsync();

				// 5. Parse the authorization code from the query string
				var code = context.Request.QueryString["code"];

				// 6. Send a friendly response back to the browser window
				//HttpListenerResponse response = context.Response;
				//string responseString = "<html><body><h1>Authentication successful!</h1><p>You can close this tab.</p></body></html>";
				//byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
				//response.ContentLength64 = buffer.Length;
				//using Stream output = response.OutputStream;
				//await output.WriteAsync(buffer, 0, buffer.Length);

				StopLocalHttpServer();
				//listener.Stop();

				if (!string.IsNullOrEmpty(code))
				{
					// Exchange the code and your saved PKCE Verifier for an official user session
					var session = await _client.Auth.ExchangeCodeForSession(pkceVerifier, code);

					return session;
				}
				return null;
			}
			catch (Exception ex)
			{
				ErrorText = $"Authentication failed: {ex.Message}";
				ErrorTextRu = $"Ошибка аутентификации: {ex.Message}";

				Debug.LogError(ErrorText);
				StopLocalHttpServer();
				return null;
			}
		}

		private async Task<Session> SignInWithGoogleTask()
		{
			// 1. Запускаем локальное прослушивание порта
			StartLocalHttpServer();

			try
			{
				// 2. Настраиваем опции входа.
				// Flow Type ОБЯЗАТЕЛЬНО ставим PKCE, чтобы браузер передал ?code= на локальный сервер
				var signInOptions = new SignInOptions
				{
					RedirectTo = "http://*:8080/",
					FlowType = Constants.OAuthFlowType.PKCE,
					QueryParams = new Dictionary<string, string>
					{
						{ "prompt", "consent" },
						{ "access_type", "offline" }
					}
				};

				// 3. Получаем объект ответа, содержащий сгенерированный URL
				var authResponse = await _client.Auth.SignIn(Constants.Provider.Google, signInOptions);

				string pkceVerifier = authResponse.PKCEVerifier;

				// Извлекаем чистую строку URL для браузера
				string authUrl = authResponse.Uri.ToString();

				Debug.Log($"[Supabase] Открываем ссылку авторизации: {authUrl}");

				// 4. Теперь передаем корректную строку в браузер
				Application.OpenURL(authUrl);

				// 5. Ожидаем редирект от браузера
				string code = await ListenForOAuthCallback();

				if (!string.IsNullOrEmpty(code))
				{
					// Передаем URL с кодом PKCE, библиотека сама сделает обмен на токены
					//var session = await _client.Auth.GetSessionFromUrl(new Uri(callbackUrl));
					var session = await _client.Auth.ExchangeCodeForSession(pkceVerifier, code);

					if (session != null)
					{
						SetUser(session.User);
						IsSignedIn = true;
						ErrorText = $"Success! Signed In Google as {session.User.Email}";
						ErrorTextRu = $"Успешно! Вы вошли через Google как {session.User.Email}";

						Debug.Log(ErrorText);
						StopLocalHttpServer();
						return session;
					}
					else
					{
						IsSignedIn = false;
						ErrorText = "Authorization error. Try signing out and try again";
						ErrorTextRu = $"Ошибка авторизации: Попробуйте выполнить SignOut";
						Debug.LogError(ErrorText);
					}
				}
			}
			catch (Exception ex)
			{
				IsSignedIn = false;
				ErrorText = $"Authorization error. Try signing out and try again\n{ex.Message}";
				ErrorTextRu = $"Ошибка авторизации: Попробуйте выполнить SignOut\n{ex.Message}";
				Debug.LogError($"[Supabase] Ошибка авторизации: {ex.Message}");
			}
			StopLocalHttpServer();
			return null;
		}

		public async Task AddPasswordToGoogleAccount(string pass)
		{
			try
			{
				// Создаем объект с новыми параметрами пользователя
				var attributes = new UserAttributes
				{
					Password = pass
				};

				// Обновляем текущего (уже вошедшего через Google) пользователя
				var updatedUser = await _client.Auth.Update(attributes);

				if (updatedUser != null)
				{
					ErrorText = "Password successfully added! An email/password pair is now linked to the account.";
					ErrorTextRu = "Пароль успешно добавлен! Теперь к аккаунту привязана связка Email/Password.";
				}
				else
				{
					ErrorText = "Failed to add the password. Make sure the user is logged in";
					ErrorTextRu = "Не удалось добавить пароль. Убедитесь, что пользователь выполнил вход";
				}
				Debug.Log(ErrorTextRu);
			}
			catch (Exception ex)
			{
				ErrorText = $"Failed to add the password. {ex.Message}";
				ErrorTextRu = $"Не удалось добавить пароль: {ex.Message}";
				Debug.LogError(ErrorText);
			}
		}

		private async Task<string> ListenForOAuthCallback()
		{
			try
			{
				// Ожидаем запрос от браузера
				HttpListenerContext context = await httpListener.GetContextAsync();

				HttpListenerRequest request = context.Request;

				// Формируем полный URL, который прислал Supabase (содержит #access_token=...)
				// Примечание: Браузеры не всегда шлют фрагмент (#) на сервер напрямую,
				// поэтому иногда Supabase возвращает параметры через Query String (?code=...)
				// В случае с # нам нужен небольшой JS костыль, либо обрабатываем URL целиком.
				string rawUrl = request.Url.ToString();

				string code;
				string codeBreak = "?code=";
				if (rawUrl.Contains(codeBreak))
				{
					code = rawUrl.Split(codeBreak)[1];
					//StopLocalHttpServer();
					return code;
				}

				code = context.Request.QueryString["code"];

				HttpListenerResponse response = context.Response;
				string responseString = "<html><body><h1>Authentication successful!</h1><p>You can close this tab.</p></body></html>";
				byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
				response.ContentLength64 = buffer.Length;
				using Stream output = response.OutputStream;
				await output.WriteAsync(buffer, 0, buffer.Length);

				return code;
			}
			catch (Exception ex)
			{
				ErrorText = $"Error while waiting for a callback{ex.Message}";
				ErrorTextRu = $"Ошибка при ожидании колбэка{ex.Message}";
				Debug.LogError(ErrorText);
				StopLocalHttpServer();
				return null;
			}
		}

		private void StartLocalHttpServer()
		{
			if (httpListener != null && httpListener.IsListening) return;

			httpListener = new HttpListener();
			httpListener.Prefixes.Add("http://*:8080/"); //"http://*:8080/"
			httpListener.Start();
			Debug.Log($"[Server] Локальный сервер запущен на {LocalhostCallback}");
		}

		private void StopLocalHttpServer()
		{
			if (httpListener != null && httpListener.IsListening)
			{
				httpListener.Stop();
				httpListener.Close();
				Debug.Log("[Server] Локальный сервер остановлен.");
			}
		}

		public async Task SignOutTask()
		{
			if (IsSignedIn)
			{
				await _client.Auth.SignOut();
			}
			SessionHandler?.DestroySession();
			IsSignedIn = false;
			StopLocalHttpServer();

			ErrorText = $"No user logged in";
			ErrorTextRu = "Осуществлен выход. Пользователь не авторизован";
			Debug.Log("No user logged in");
		}

		public string[] GetLogMessage()
		{
			return new string[] { ErrorTextRu, ErrorText };
		}

		private void DebugListener(string message, Exception e)
		{
			ErrorText = message;
			Debug.Log(message, gameObject);
			if (e != null) Debug.LogException(e, gameObject);
		}

		private void CloseClient()
		{
			if (_client != null)
			{
				_client?.Auth.Shutdown();
				_client = null;
			}
		}

		private void OnApplicationQuit()
		{
			OnApplicationQuitTask();
		}

		private async void OnApplicationQuitTask()
		{
			StopLocalHttpServer();

			try
			{
				await DataManager.Instance.DatabaseManager.OnApplicationQuit().ConfigureAwait(false);
			}
			catch { }
			CloseClient();
		}
	}
}
