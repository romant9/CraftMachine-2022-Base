using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class YandexAuth : MonoBehaviour
{
	private const string ClientId = "aje0klo54cjknb5366dk";
	// ВАЖНО: это должно совпадать с Redirect URI в Identity Hub
	private const string RedirectUri = "http://localhost/callback"; //"simple.oauth://oauth2/playid"; 
	private const string AuthorizationEndpoint = "https://auth.yandex.cloud/oauth/authorize";
	private const string TokenEndpoint = "https://auth.yandex.cloud/oauth/token";

	private string _codeVerifier;
	private string _state;
	private HttpListener _listener;
	private TaskCompletionSource<string> _redirectTcs;

	public async Task<string> GetIdTokenFromYandexViaWebViewAsync()
	{
		StopLocalHttpServer();
		// 1. PKCE и state
		(string codeVerifier, string codeChallenge) = GeneratePkcePair();
		_codeVerifier = codeVerifier;
		_state = Guid.NewGuid().ToString("N");

		// 2. Формируем URL авторизации
		string authUrl = BuildAuthorizationUrl(codeChallenge);

		// 3. Запускаем локальный сервер и открываем браузер
		_redirectTcs = new TaskCompletionSource<string>();
		StartLocalServer();

		Debug.Log($"Opening browser with: {authUrl}");
		System.Diagnostics.Process.Start(authUrl); // системный браузер

		// Ждём, пока сервер поймает редирект
		string redirectUrl = await _redirectTcs.Task;

		// 4. Парсим code и state
		var (code, returnedState) = ParseRedirectResponse(redirectUrl);
		if (code == null || returnedState != _state)
			throw new Exception("State mismatch or missing code");

		// 5. Обмениваем code на токены
		var tokens = await ExchangeCodeForTokensAsync(code, _codeVerifier);
		if (!tokens.TryGetValue("id_token", out var idToken))
			throw new Exception("No id_token in response");

		return idToken;
	}

	// --- Локальный HTTP-сервер (только для Windows Standalone) ---
	private void StartLocalServer()
	{
		_listener = new HttpListener();
		_listener.Prefixes.Add("http://localhost/");
		_listener.Start();

		Task.Run(async () =>
		{
			while (_listener.IsListening)
			{
				var context = await _listener.GetContextAsync();
				var request = context.Request;
				var response = context.Response;

				string url = request.Url.ToString();
				Debug.Log("HTTP listener received: " + url);

				// Проверяем, что это наш callback
				if (url.StartsWith(RedirectUri))
				{
					// Сохраняем полный URL с параметрами
					_redirectTcs.TrySetResult(url);

					// Отправляем простой HTML-ответ, чтобы браузер не висел
					byte[] buffer = Encoding.UTF8.GetBytes("<html><body><h1>Authentication successful!</h1><p>You can close this tab.</p></body></html>");
					response.ContentLength64 = buffer.Length;
					var output = response.OutputStream;
					await output.WriteAsync(buffer, 0, buffer.Length);
					output.Close();

					// Останавливаем сервер после первого успешного редиректа
					_listener.Stop();
					break;
				}
				else
				{
					// Для всех остальных запросов — 404
					response.StatusCode = 404;
					byte[] buffer = Encoding.UTF8.GetBytes("Not Found");
					response.ContentLength64 = buffer.Length;
					var output = response.OutputStream;
					await output.WriteAsync(buffer, 0, buffer.Length);
					output.Close();
				}
			}
		});
	}

	public void StopLocalHttpServer()
	{
		if (_listener != null && _listener.IsListening)
		{
			_listener.Stop();
			_listener.Close();
			Debug.Log("[Server] Локальный сервер остановлен.");
		}
	}

	// --- Вспомогательные методы (те же, что и раньше) ---

	private (string verifier, string challenge) GeneratePkcePair()
	{
		using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
		byte[] bytes = new byte[32];
		rng.GetBytes(bytes);
		string verifier = Convert.ToBase64String(bytes)
			.Replace('+', '-').Replace('/', '_').TrimEnd('=');

		using var sha256 = System.Security.Cryptography.SHA256.Create();
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

		var sb = new System.Text.StringBuilder(AuthorizationEndpoint).Append('?');
		bool first = true;
		foreach (var kvp in query)
		{
			if (!first) sb.Append('&');
			first = false;
			sb.Append(System.Uri.EscapeDataString(kvp.Key))
			  .Append('=')
			  .Append(System.Uri.EscapeDataString(kvp.Value));
		}
		return sb.ToString();
	}

	private (string code, string state) ParseRedirectResponse(string redirectUrl)
	{
		var uri = new Uri(redirectUrl);
		var dict = ParseQueryString(uri.Query);
		return (dict.TryGetValue("code", out var c) ? c : null,
				dict.TryGetValue("state", out var s) ? s : null);
	}

	private Dictionary<string, string> ParseQueryString(string query)
	{
		var dict = new Dictionary<string, string>();
		if (string.IsNullOrEmpty(query)) return dict;
		query = query.TrimStart('?');
		foreach (var pair in query.Split('&'))
		{
			var parts = pair.Split('=');
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

		using var client = new System.Net.Http.HttpClient();

		var response = await client.PostAsync(TokenEndpoint, new System.Net.Http.FormUrlEncodedContent(dict));
		response.EnsureSuccessStatusCode();
		var json = await response.Content.ReadAsStringAsync();

		// Простой парсер (для продакшена лучше использовать System.Text.Json)
		var tokens = new Dictionary<string, string>();
		json = json.Replace(" ", "").Replace("\"", "");
		foreach (var part in json.Split(',', StringSplitOptions.RemoveEmptyEntries))
		{
			var kv = part.Split(':');
			if (kv.Length == 2)
				tokens[kv[0]] = kv[1];
		}
		return tokens;
	}
}
