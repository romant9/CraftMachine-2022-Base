using PlayId.Scripts.Data;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class OidcTokenExchangeAsync
{
    private CustomAuthSettings _settings;

	public string redirectUri = "simple.oauth://oauth2/playid";
	private string RedirectUriScheme;

	private string AuthorizationEndpoint = "https://auth.yandex.cloud/oauth/authorize";
	private string TokenEndpoint = "https://auth.yandex.cloud/oauth/token";

	public string issuerUrl = "https://auth.yandex.cloud";
    public string clientId = "aje0klo54cjknb5366dk";
    // Должен совпадать с Redirect URI, зарегистрированным в Identity Hub
    //public string redirectUri = "simple.oauth://oauth2/playid"; //"craftapp://oauth/yandex"; //"http://localhost/callback";

    private string _codeVerifier;
    private string _authorizationCode;

    public OidcTokenExchangeAsync(CustomAuthSettings settings) { Initialize(settings); }

	private void Initialize(CustomAuthSettings settings)
    {
        _settings = settings;
		clientId = settings.ClientId;
		RedirectUriScheme = settings.RedirectUriScheme;
		redirectUri = _settings.RedirectUri.FirstOrDefault(x => x.StartsWith(RedirectUriScheme));
		AuthorizationEndpoint = _settings.AuthorizationEndpoint;
		TokenEndpoint = _settings.TokenEndpoint;
	}

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

        return (codeVerifier, codeChallenge);
    }

	public async Task<string> PerformTokenExchangeWithPkce(string code)
	{
        _authorizationCode = code;

		var form = new WWWForm();
		form.AddField("grant_type", "authorization_code");
		form.AddField("code", _authorizationCode);
		form.AddField("client_id", clientId);
		form.AddField("redirect_uri", redirectUri);
		form.AddField("code_verifier", _codeVerifier);

		var tokenRequest = UnityWebRequest.Post(TokenEndpoint, form);
		tokenRequest.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");

		await SendWebRequestTask(tokenRequest);

		if (tokenRequest.result == UnityWebRequest.Result.Success)
		{
			var tokenResponse = JsonUtility.FromJson<TokenResponseOidc>(tokenRequest.downloadHandler.text);
			string id_token = tokenResponse.id_token;
			//string id_token = HttpHelper.ExtractCodeFromUrl(tokenRequest.downloadHandler.text);

			if (!string.IsNullOrEmpty(id_token))
			{
                return id_token;
			}
			Debug.LogError("[OidcTokenExchange] id_token missing in response.");
			Debug.Log(tokenRequest.downloadHandler.text);
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
		_codeVerifier = verifier;

		string state = System.Guid.NewGuid().ToString();

        string authUrl = $"{AuthorizationEndpoint}?" +
            $"client_id={clientId}" +
            $"&redirect_uri={redirectUri}" +
            $"&response_type=code" +
            $"&scope=openid%20profile" +
            $"&state={state}" +
            $"&code_challenge={challenge}" +
            $"&code_challenge_method=S256";
        return authUrl;
    }
}

[Serializable]
public class TokenResponseOidc
{
	public string access_token;
    public string id_token;
    public int expires_in;
    public string token_type;
}
