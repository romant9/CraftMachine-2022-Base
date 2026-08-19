using Supabase.TWD;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.Networking;

public class OidcTokenExchangeAsync : MonoBehaviour
{
    [Header("OIDC Settings")]
    public string issuerUrl = "https://auth.yandex.cloud";
    public string clientId = "aje0klo54cjknb5366dk";
    // Должен совпадать с Redirect URI, зарегистрированным в Identity Hub
    public string redirectUri = "simple.oauth://oauth2/playid"; //"craftapp://oauth/yandex"; //"http://localhost/callback";

    private string _codeVerifier;
    private string _authorizationCode;

    /// <summary>
    /// Генерирует PKCE code_verifier и code_challenge.
    /// Работает во всех версиях Unity (без HashData).
    /// </summary>
    public (string codeVerifier, string codeChallenge) GeneratePkce()
    {
        // 1. Генерируем случайный code_verifier (32 байта)
        var randomBytes = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
            rng.GetBytes(randomBytes);

        string codeVerifier = Convert.ToBase64String(randomBytes)
            .Replace('+', '-').Replace('/', '_').Replace("=", "");

        // 2. Вычисляем code_challenge: SHA‑256(code_verifier) → Base64URL
        byte[] verifierBytes = Encoding.UTF8.GetBytes(codeVerifier);

        byte[] challengeBytes;
        using (var sha256 = SHA256.Create())
        {
            challengeBytes = sha256.ComputeHash(verifierBytes);
        }

        string codeChallenge = Convert.ToBase64String(challengeBytes)
            .Replace('+', '-').Replace('/', '_').Replace("=", "");

        _codeVerifier = codeVerifier;
        return (codeVerifier, codeChallenge);
    }

	public async Task<string> PerformTokenExchangeWithPkce(string code)
	{
        _authorizationCode = code;
		// 1. Получаем OIDC конфигурацию (чтобы достать token_endpoint)
		string configUrl = $"{issuerUrl}/.well-known/openid-configuration";
		var configRequest = UnityWebRequest.Get(configUrl);
		await SendWebRequestTask(configRequest);

		if (configRequest.result != UnityWebRequest.Result.Success)
		{
			Debug.LogError($"[OidcTokenExchange] Config request failed: {configRequest.error}");
			return null;
		}

		var config = JsonUtility.FromJson<OidcConfiguration>(configRequest.downloadHandler.text);
		if (string.IsNullOrEmpty(config.token_endpoint))
		{
			Debug.LogError("[OidcTokenExchange] token_endpoint not found in OIDC configuration.");
			return null;
		}

		// 2. POST на token_endpoint (PKCE, без client_secret)
		var form = new WWWForm();
		form.AddField("grant_type", "authorization_code");
		form.AddField("code", _authorizationCode);
		form.AddField("client_id", clientId);
		form.AddField("redirect_uri", redirectUri);
		form.AddField("code_verifier", _codeVerifier);

		var tokenRequest = UnityWebRequest.Post(config.token_endpoint, form);
		tokenRequest.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");

		await SendWebRequestTask(tokenRequest);

		if (tokenRequest.result == UnityWebRequest.Result.Success)
		{
			var tokenResponse = JsonUtility.FromJson<TokenResponseOidc>(tokenRequest.downloadHandler.text);
			string id_token = tokenResponse.id_token;
			//string id_token = ExtractCodeFromUrl(tokenRequest.downloadHandler.text);

			if (!string.IsNullOrEmpty(id_token))
			{
                return id_token;
			}
			else
			{
				Debug.LogError("[OidcTokenExchange] id_token missing in response.");
				Debug.Log(tokenRequest.downloadHandler.text);
			}
		}
		else
		{
			Debug.LogError($"[OidcTokenExchange] Token request failed: {tokenRequest.error}\n{tokenRequest.downloadHandler.text}");
		}
        return null;
	}

	// Обертка, чтобы сделать UnityWebRequest awaitable через .completed
	private static Task SendWebRequestTask(UnityWebRequest request)
    {
        var tcs = new TaskCompletionSource<bool>();
        request.SendWebRequest()
            .completed += operation =>
            {
                if (request.result == UnityWebRequest.Result.Success)
                    tcs.TrySetResult(true);
                else
                    tcs.TrySetException(new Exception(request.error));
            };
        return tcs.Task;
    }

    public string GetAuthUrl()
    {
        var (verifier, challenge) = GeneratePkce();
        string state = System.Guid.NewGuid().ToString();

        string authUrl = $"https://auth.yandex.cloud/oauth/authorize?" +
            $"client_id={clientId}" +
            $"&redirect_uri={redirectUri}" +
            $"&response_type=code" +
            $"&scope=openid%20profile" +
            $"&state={state}" +
            $"&code_challenge={challenge}" +
            $"&code_challenge_method=S256";
        return authUrl;
    }

	/// <summary>
	/// вариант парсинга redirect url, чтобы взятьл code или id_token
	/// </summary>
	/// <param name="url"></param>
	/// <returns></returns>
	private string ExtractCodeFromUrl(string url)
	{
		var uri = new System.Uri(url);
		var queryDict = HttpUtility.ParseQueryString(uri.Query); // или свой простой парсер
		return queryDict["code"];
	}
}

[Serializable]
public class OidcConfiguration
{
    public string token_endpoint;
}

[Serializable]
public class TokenResponseOidc
{
    public string access_token;
    public string id_token;
    public int expires_in;
    public string token_type;
}
