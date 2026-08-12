using PocketBaseSdk;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Pocketbase.TWD
{
    [Serializable]
    public class PocketBaseAuthResponse
    {
        public string token; // JWT-токен для будущих запросов
        public SystemUser record; // Данные созданного/авторизованного пользователя
    }

    [Serializable]
    public class SystemUser
    {
        public string id; // Это и есть UID для вашей таблицы cm_users
        public string email;
        public string username;
    }

    [Serializable]
    public class AuthProviderInfo
    {
        public string name;           // "google"
        public string state;          // Случайная строка для защиты от CSRF
        public string codeVerifier;   // Тот самый PKCE Verifier
        public string codeChallenge;  // Хэш от codeVerifier
        public string authUrl;        // Ссылка, на которую нужно отправить игрока в браузер
    }

    [Serializable]
    public class GoogleAuthPayload
    {
        public string provider = "google";
        public string token; // Сюда передается idToken, полученный от Google SDK в Unity
    }

    public class PocketbaseManager : MonoBehaviour
    {
        private string baseUrl = "http://127.0.0";

        // Переменная для хранения токена в текущей сессии игры
        public string AuthToken { get; private set; }
        public string UserUID { get; private set; }

        public bool IsLoginWithGoogle { get; private set; }

        public PocketBaseUserManagerAsync pbManager;

        // Ключи для сохранения сессии в PlayerPrefs
        private const string TokenKey = "PB_AuthToken";
        private const string UidKey = "PB_UserUID";

        // Переменные для хранения состояния текущего запроса OAuth2
        private string savedState;
        private string savedCodeVerifier;

        private async void Start()
        {
            // При запуске игры пытаемся восстановить старую сессию
            if (TryLoadSession()) return;

            //if (IsLoginWithGoogle)
            // 1. Сначала регистрируем/логиним системного пользователя
            string myUID = await RegisterWithEmailAsync("player@test.com", "SuperPlayer", "P@ssword12345");

            // (Или через Google, если получили токен от Google SDK):
            // string myUID = await LoginWithGoogleAsync("полученный_от_google_id_token");

            if (string.IsNullOrEmpty(myUID))
            {
                Debug.LogError("Авторизация провалена. Дальнейшие шаги отменены.");
                return;
            }

            // 2. Если UID успешно получен, сразу синхронизируем его игровой профиль cm_users
            CMUser localProfile = new CMUser
            {
                UserName = "SuperPlayer",
                Email = "player@test.com",
                DeviceInfo = SystemInfo.deviceModel,
                ClientVersion = Application.version
            };

            CMUser activeProfile = await pbManager.SyncUserAsync(myUID, localProfile);

            if (activeProfile != null)
            {
                Debug.Log($"Всё готово! Профиль синхронизирован. Общее число сессий: {activeProfile.TimesRun}");
            }
            else
            {
                Debug.LogError("Не удалось синхронизировать профиль с PocketBase.");
            }
        }

        /// <summary>
        /// Регистрация нового аккаунта по Email и Паролю.
        /// Возвращает UID пользователя, если регистрация успешна.
        /// </summary>
        public async Task<string> RegisterWithEmailAsync(string email, string username, string password)
        {
            string url = $"{baseUrl}/records";

            // Формируем JSON-полезную нагрузку для PocketBase
            // Поля passwordConfirm и emailVisibility обязательны/рекомендуемы
            string jsonPayload = $"{{" +
                                 $"\"email\":\"{email}\"," +
                                 $"\"username\":\"{username}\"," +
                                 $"\"password\":\"{password}\"," +
                                 $"\"passwordConfirm\":\"{password}\"," +
                                 $"\"emailVisibility\":true" +
                                 $"}}";

            using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
            {
                byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonPayload);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");

                await request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    // Пользователь создан! Но PocketBase при создании не авторизует автоматически.
                    // Чтобы сразу залогинить игрока и получить токен, вызываем метод авторизации:
                    return await LoginWithEmailAsync(email, password);
                }

                Debug.LogError($"Ошибка регистрации: {request.error}\n{request.downloadHandler.text}");
                return null;
            }
        }

        /// <summary>
        /// Авторизация (Логин) по Email и Паролю.
        /// </summary>
        public async Task<string> LoginWithEmailAsync(string email, string password)
        {
            string url = $"{baseUrl}/auth-with-password";
            string jsonPayload = $"{{\"identity\":\"{email}\",\"password\":\"{password}\"}}";

            using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
            {
                byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonPayload);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");

                await request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    var authData = JsonUtility.FromJson<PocketBaseAuthResponse>(request.downloadHandler.text);

                    // Сохраняем токен для последующих защищенных запросов (например, для cm_users)
                    AuthToken = authData.token;

                    Debug.Log($"Успешный вход! Токен получен. UID пользователя: {authData.record.id}");
                    return authData.record.id; // Возвращаем UID
                }

                Debug.LogError($"Ошибка входа: {request.error}\n{request.downloadHandler.text}");
                return null;
            }
        }

        /// <summary>
        /// Шаг 1: Запрос параметров PKCE у PocketBase и открытие браузера
        /// </summary>
        public async Task StartGoogleAuthAsync()
        {
            string url = $"{baseUrl}/auth-with-oauth2";

            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                await request.SendWebRequest();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError($"Ошибка получения провайдеров: {request.error}");
                    return;
                }

                var authProviders = JsonUtility.FromJson<List<AuthProviderInfo>>(request.downloadHandler.text);
                var googleProvider = authProviders.Find(p => p.name == "google");

                if (googleProvider == null)
                {
                    Debug.LogError("Провайдер Google не настроен в Admin UI PocketBase!");
                    return;
                }

                // ОБЯЗАТЕЛЬНО сохраняем state и codeVerifier во временные переменные.
                // Они понадобятся на Шаге 2 для подтверждения подлинности.
                savedState = googleProvider.state;
                savedCodeVerifier = googleProvider.codeVerifier;

                // Открываем системный браузер ПК/смартфона для авторизации игрока
                // PocketBase сам добавил в эту ссылку сгенерированный codeChallenge
                Application.OpenURL(googleProvider.authUrl + "http://localhost:8090/redirect");
                Debug.Log("Браузер открыт. Ожидание авторизации...");
            }
        }

        /// <summary>
        /// Шаг 2: Завершение авторизации (Вызывать, когда получили code из редиректа)
        /// </summary>
        /// <param name="returnedCode">Код, который Google вернул в URL после успешного входа</param>
        public async Task<string> CompleteGoogleAuthAsync(string returnedCode)
        {
            string url = $"{baseUrl}/auth-with-oauth2";

            // Формируем JSON, отправляя обратно полученный код и наш сохраненный PKCE Verifier
            string jsonPayload = $"{{" +
                                 $"\"provider\":\"google\"," +
                                 $"\"code\":\"{returnedCode}\"," +
                                 $"\"codeVerifier\":\"{savedCodeVerifier}\"," +
                                 $"\"redirectUrl\":\"http://localhost:8090/redirect\"" +
                                 $"}}";

            using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
            {
                byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonPayload);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");

                await request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    var authData = JsonUtility.FromJson<PocketBaseAuthResponse>(request.downloadHandler.text);

                    // Сохраняем сессию
                    SaveSession(authData.token, authData.record.id);

                    Debug.Log($"Вход через Google (PKCE) успешен! UID: {UserUID}");
                    return UserUID;
                }

                Debug.LogError($"Ошибка PKCE обмена: {request.error}\n{request.downloadHandler.text}");
                return null;
            }
        }

        /// <summary>
        /// Сохранение сессии в PlayerPrefs
        /// </summary>
        private void SaveSession(string token, string uid)
        {
            AuthToken = token;
            UserUID = uid;

            PlayerPrefs.SetString(TokenKey, token);
            PlayerPrefs.SetString(UidKey, uid);
            PlayerPrefs.Save();
            Debug.Log("Сессия авторизации сохранена на устройстве.");
        }

        /// <summary>
        /// Загрузка и проверка существующей сессии
        /// </summary>
        public bool TryLoadSession()
        {
            if (PlayerPrefs.HasKey(TokenKey) && PlayerPrefs.HasKey(UidKey))
            {
                AuthToken = PlayerPrefs.GetString(TokenKey);
                UserUID = PlayerPrefs.GetString(UidKey);
                Debug.Log($"Найдена сохраненная сессия игрока! UID: {UserUID}");

                // Токен загружен. В идеале здесь стоит сделать легкий проверочный запрос к /api/collections/users/auth-refresh,
                // чтобы убедиться, что токен не просрочен.
                return true;
            }

            Debug.Log("Сохраненных сессий не найдено. Требуется первый вход.");
            return false;
        }

        /// <summary>
        /// Выход из аккаунта (Очистка сессии)
        /// </summary>
        public void Logout()
        {
            AuthToken = null;
            UserUID = null;
            PlayerPrefs.DeleteKey(TokenKey);
            PlayerPrefs.DeleteKey(UidKey);
            PlayerPrefs.Save();
            Debug.Log("Игрок вышел из аккаунта. Сессия стерта.");
        }
    }
}
