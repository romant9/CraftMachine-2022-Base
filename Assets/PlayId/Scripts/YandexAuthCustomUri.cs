using Newtonsoft.Json.Linq;
using PlayId.Scripts.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class YandexAuthCustomUri
{
	private CustomAuthSettings _settings;
	// ВАЖНО: Должно совпадать с Redirect URI в Identity Hub
	private string RedirectUri = "simple.oauth://oauth2/playid";
	private string RedirectUriScheme;
	private string ClientId = "aje0klo54cjknb5366dk";

	private string AuthorizationEndpoint = "https://auth.yandex.cloud/oauth/authorize";
	private string TokenEndpoint = "https://auth.yandex.cloud/oauth/token";

	private string _codeVerifier;
	private string _state;
	private TaskCompletionSource<string> _callbackTcs;
	private bool isInitialized;

	public YandexAuthCustomUri(CustomAuthSettings settings) { Initialize(settings); }

	// Подписка на Deep Link происходит в Awake
	private void Initialize(CustomAuthSettings settings)
	{
		_settings = settings;
		ClientId = _settings.ClientId;
		AuthorizationEndpoint = _settings.AuthorizationEndpoint;
		TokenEndpoint = _settings.TokenEndpoint;
		RedirectUriScheme = _settings.RedirectUriScheme;
		RedirectUri = _settings.RedirectUri.FirstOrDefault(x=>x.StartsWith(RedirectUriScheme));

		Application.deepLinkActivated -= OnDeepLinkActivated;
		Application.deepLinkActivated += OnDeepLinkActivated;

		_callbackTcs = new TaskCompletionSource<string>();

		isInitialized = true;
	}

	/// <summary>
	/// Основной метод входа. Вызывается по кнопке UI.
	/// </summary>
	public async Task<string> GetIdTokenFromYandex()
	{
		if (!isInitialized) return null;

		_codeVerifier = null;
		_state = Guid.NewGuid().ToString("N");

		var (verifier, challenge) = GeneratePkcePair();
		_codeVerifier = verifier;

		string authUrl = BuildAuthorizationUrl(challenge);
		Debug.Log($"[Auth] Открываем браузер: {authUrl}");

#if UNITY_STANDALONE_WIN
        WindowsDeepLinking.Initialize(_settings.RedirectUriScheme, OnDeepLinkActivated);
#endif
		Application.OpenURL(authUrl);

		if (_settings.UseTimeout)
		{
			// Создаём задачу с таймаутом
			var cts = new CancellationTokenSource(TimeSpan.FromSeconds(_settings.TimeoutSeconds));
			var timeoutTask = Task.Delay(_settings.TimeoutSeconds * 1000, cts.Token);

			// Ждём либо получения редиректа, либо истечения таймаута
			var completedTask = await Task.WhenAny(_callbackTcs.Task, timeoutTask);

			if (completedTask == timeoutTask)
			{
				// Таймаут истёк
				throw new OperationCanceledException("Время ожидания авторизации истекло. Пользователь не завершил вход.");
			}
		}

		// Сюда попадаем, если пришёл редирект
		string redirectUrl = await _callbackTcs.Task;
		return await ProcessCallbackAndGetToken(redirectUrl);
	}

	private async Task<string> ProcessCallbackAndGetToken(string redirectUrl)
	{
		var (code, returnedState, error) = ParseRedirectResponse(redirectUrl);

		if (!string.IsNullOrEmpty(error))
		{
			var dict = ParseQueryString(new Uri(redirectUrl).Query);
			var desc = dict.TryGetValue("error_description", out var d) ? d : "";
			throw new Exception($"Ошибка OAuth: {error}. Описание: {desc}");
		}

		if (string.IsNullOrEmpty(code))
			throw new Exception("Не удалось получить код авторизации.");

		if (returnedState != _state)
			throw new Exception("Ошибка безопасности: несоответствие state.");

		// Обмениваем code на токены
		var tokens = await ExchangeCodeForTokensAsync(code, _codeVerifier);

		if (!tokens.TryGetValue("id_token", out var idToken))
			throw new Exception("В ответе сервера нет id_token.");

		return idToken;
	}

	private (string verifier, string challenge) GeneratePkcePair()
	{
		using var rng = RandomNumberGenerator.Create();
		byte[] bytes = new byte[32];
		rng.GetBytes(bytes);

		string verifier = Convert.ToBase64String(bytes)
			.Replace('+', '-').Replace('/', '_').TrimEnd('=');

		using var sha256 = SHA256.Create();
		byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(verifier));
		string challenge = Convert.ToBase64String(hash)
			.Replace('+', '-').Replace('/', '_').TrimEnd('=');

		return (verifier, challenge);
	}

	private string BuildAuthorizationUrl(string codeChallenge)
	{
		var query = new Dictionary<string, string>
		{
			{ "client_id", ClientId },
			{ "redirect_uri", RedirectUri },
			{ "response_type", "code" },
			{ "scope", "openid profile email" },
			{ "code_challenge", codeChallenge },
			{ "code_challenge_method", "S256" },
			{ "state", _state }
		};

		var sb = new StringBuilder(AuthorizationEndpoint).Append('?');
		bool first = true;
		foreach (var kvp in query)
		{
			if (!first) sb.Append('&');
			first = false;
			sb.Append(Uri.EscapeDataString(kvp.Key))
			  .Append('=')
			  .Append(Uri.EscapeDataString(kvp.Value));
		}
		return sb.ToString();
	}

	private (string code, string state, string error) ParseRedirectResponse(string url)
	{
		try
		{
			var uri = new Uri(url);
			var dict = ParseQueryString(uri.Query);
			return (
				dict.TryGetValue("code", out var c) ? c : null,
				dict.TryGetValue("state", out var s) ? s : null,
				dict.TryGetValue("error", out var e) ? e : null
			);
		}
		catch (Exception ex)
		{
			Debug.LogError("[Auth] Ошибка парсинга URL: " + ex.Message);
			return (null, null, "parse_error");
		}
	}

	private Dictionary<string, string> ParseQueryString(string query)
	{
		var dict = new Dictionary<string, string>();
		if (string.IsNullOrEmpty(query)) return dict;
		query = query.TrimStart('?');
		foreach (var pair in query.Split('&', StringSplitOptions.RemoveEmptyEntries))
		{
			var parts = pair.Split('=', 2);
			if (parts.Length == 2)
				dict[parts[0]] = parts[1];
		}
		return dict;
	}

	private async Task<Dictionary<string, string>> ExchangeCodeForTokensAsync(string code, string codeVerifier)
	{
		var content = new FormUrlEncodedContent(new Dictionary<string, string>
		{
			{ "grant_type", "authorization_code" },
			{ "code", code },
			{ "redirect_uri", RedirectUri },
			{ "client_id", ClientId },
			{ "code_verifier", codeVerifier }
		});

		using var client = new HttpClient();
		var response = await client.PostAsync(TokenEndpoint, content);
		response.EnsureSuccessStatusCode();

		string json = await response.Content.ReadAsStringAsync();

		// Парсим через JObject — это намного надёжнее, чем ручной split
		var jobject = JObject.Parse(json);
		var tokens = new Dictionary<string, string>();

		foreach (var prop in jobject.Properties())
		{
			// Берём только строковые значения (id_token, access_token и т.п.)
			if (prop.Value.Type == JTokenType.String)
			{
				tokens[prop.Name] = (string)prop.Value;
			}
			else if (prop.Value.Type == JTokenType.Null)
			{
				// Можно сохранить null как пустую строку или пропустить
				tokens[prop.Name] = string.Empty;
			}
		}
		return tokens;
	}

	/// <summary>
	/// Этот метод вызывается Unity автоматически, когда приложение активируется по deep link
	/// </summary>
	private void OnDeepLinkActivated(string url)
	{
		Debug.Log($"[Auth] Получен Deep Link: {url}");

		if (string.IsNullOrEmpty(url)) return;

		// Проверяем, что это наш редирект (на случай, если другие фичи тоже используют deep links)
		if (url.StartsWith(RedirectUriScheme))
		{
			// Если TCS уже завершён (например, пользователь сделал два входа подряд без сброса),
			// создаём новый, чтобы следующий вызов GetIdTokenFromYandex не завис
			if (_callbackTcs.Task.IsCompleted)
			{
				_callbackTcs = new TaskCompletionSource<string>();
			}
			_callbackTcs.TrySetResult(url);
		}
	}
}