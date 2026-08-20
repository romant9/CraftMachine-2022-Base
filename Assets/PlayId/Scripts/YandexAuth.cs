using Newtonsoft.Json.Linq;
using PlayId.Scripts.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class YandexAuth
{
	private YandexSettings _settings;
	// Лучше вынести в ScriptableObject / конфиг, но пока оставим здесь
	private string ClientId = "aje0klo54cjknb5366dk";

	// ВАЖНО: должно точно совпадать с Redirect URI в Identity Hub
	private string RedirectUri = "http://localhost/callback"; //"http://localhost:8080/callback"; //"simple.oauth://oauth2/playid"; 
	private string AuthorizationEndpoint = "https://auth.yandex.cloud/oauth/authorize";
	private string TokenEndpoint = "https://auth.yandex.cloud/oauth/token";

	private string _codeVerifier;
	private string _state;
	private HttpListener _listener;
	private TaskCompletionSource<string> _redirectTcs;
	private bool isInitialized;

	public YandexAuth(YandexSettings settings) { Initialize(settings); }

	private void Initialize(YandexSettings settings)
	{
		_settings = settings;
		ClientId = _settings.ClientId;
		RedirectUri = _settings.RedirectUri.FirstOrDefault(x => x.StartsWith("http"));
		AuthorizationEndpoint = _settings.AuthorizationEndpoint;
		TokenEndpoint = _settings.TokenEndpoint;
		isInitialized = true;
	}

	public async Task<string> GetIdTokenFromYandex()
	{
		if (!isInitialized) return null;

		StopLocalHttpServer();

		(string codeVerifier, string codeChallenge) = GeneratePkcePair();
		_codeVerifier = codeVerifier;
		_state = Guid.NewGuid().ToString("N");

		string authUrl = BuildAuthorizationUrl(codeChallenge);

		_redirectTcs = new TaskCompletionSource<string>();
		var cts = new CancellationTokenSource(TimeSpan.FromSeconds(_settings.TimeoutSeconds));

		StartLocalServer(cts.Token);

		Debug.Log($"Opening browser with: {authUrl}");
		//Process.Start(new ProcessStartInfo
		//{
		//	FileName = authUrl,
		//	UseShellExecute = true
		//});

		Application.OpenURL(authUrl);

		if (_settings.UseTimeout)
		{
			var timeoutDelay = TimeSpan.FromSeconds(_settings.TimeoutSeconds);
			var timeoutTask = Task.Delay(timeoutDelay, cts.Token);
			var completed = await Task.WhenAny(_redirectTcs.Task, timeoutTask);

			if (completed == timeoutTask)
			{
				// Таймаут или отмена
				StopLocalHttpServer();
				throw new OperationCanceledException($"Authentication timed out after {timeoutDelay}");
			}
		}

		// Сюда попадаем, если пришёл редирект
		string redirectUrl = await _redirectTcs.Task;
		return await ProcessRedirectAndGetIdToken(redirectUrl);
	}

	private async Task<string> ProcessRedirectAndGetIdToken(string redirectUrl)
	{
		var (code, returnedState, error) = ParseRedirectResponse(redirectUrl);

		if (!string.IsNullOrEmpty(error))
		{
			var errorDesc = ParseQueryString(new Uri(redirectUrl).Query).TryGetValue("error_description", out var desc) ? desc : "";
			throw new Exception($"OAuth error: {error}. Description: {errorDesc}");
		}

		if (string.IsNullOrEmpty(code) || returnedState != _state)
		{
			if (returnedState != _state) Debug.LogError("[Auth] State mismatch!");
			throw new Exception("Missing code or state mismatch.");
		}

		var tokens = await ExchangeCodeForTokensAsync(code, _codeVerifier);
		if (!tokens.TryGetValue("id_token", out var idToken))
			throw new Exception("No id_token in response.");

		return idToken;
	}

	private void StartLocalServer(CancellationToken token)
	{
		_listener = new HttpListener();
		_listener.Prefixes.Add(RedirectUri + "/");
		_listener.Start();

		Task.Run(async () =>
		{
			while (_listener.IsListening && !token.IsCancellationRequested)
			{
				HttpListenerContext context;
				try
				{
					context = await _listener.GetContextAsync();
				}
				catch (ObjectDisposedException) 
				{
					StopLocalHttpServer();
					return; 
				}

				var request = context.Request;
				var response = context.Response;

				string url = request.Url.ToString();
				Debug.Log("[Server] Received: " + url);

				// Проверяем, что это наш callback
				if (url.StartsWith(RedirectUri))
				{
					_redirectTcs.TrySetResult(url);

					byte[] buffer = Encoding.UTF8.GetBytes(
						"<html><body><h1>Authentication successful!</h1><p>You can close this tab.</p></body></html>");
					response.ContentLength64 = buffer.Length;
					await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
					response.OutputStream.Close();

					StopLocalHttpServer();
					break;
				}
				else
				{
					response.StatusCode = 404;
					byte[] buffer = Encoding.UTF8.GetBytes("Not Found");
					response.ContentLength64 = buffer.Length;
					await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
					response.OutputStream.Close();
				}
			}
		}, token);
	}

	public void StopLocalHttpServer()
	{
		if (_listener == null) return;
		try { _listener.Stop(); } catch { }
		_listener.Close();
		_listener = null;
		Debug.Log("[Server] Локальный сервер остановлен.");
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

	private (string code, string state, string error) ParseRedirectResponse(string redirectUrl)
	{
		var uri = new Uri(redirectUrl);
		var dict = ParseQueryString(uri.Query);
		return (
			dict.TryGetValue("code", out var c) ? c : null,
			dict.TryGetValue("state", out var s) ? s : null,
			dict.TryGetValue("error", out var e) ? e : null
		);
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
		var dict = new Dictionary<string, string>
		{
			{ "grant_type", "authorization_code" },
			{ "code", code },
			{ "redirect_uri", RedirectUri },
			{ "client_id", ClientId },
			{ "code_verifier", codeVerifier }
		};

		using var client = new HttpClient();
		var response = await client.PostAsync(TokenEndpoint, new FormUrlEncodedContent(dict));
		response.EnsureSuccessStatusCode();
		var json = await response.Content.ReadAsStringAsync();

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
}
