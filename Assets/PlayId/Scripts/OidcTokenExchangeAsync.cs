using Assets.PlayId.Scripts.Data;
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
    public string redirectUri = "craftapp://oauth/yandex";
    //romant9@yandex.ru

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

    /// <summary>
    /// Вызывайте после получения code из redirect URI.
    /// </summary>
    public async void ExchangeCodeForTokens(string authorizationCode)
    {
        _authorizationCode = authorizationCode;
        if (string.IsNullOrEmpty(_authorizationCode) || string.IsNullOrEmpty(_codeVerifier))
        {
            Debug.LogError("[OidcTokenExchange] Missing code or code_verifier.");
            return;
        }

        try
        {
            await PerformTokenExchangeWithPkce();
        }
        catch (Exception e)
        {
            Debug.LogError($"[OidcTokenExchange] Exception: {e.Message}\n{e.StackTrace}");
        }
    }

    private async Task PerformTokenExchangeWithPkce()
    {
        // 1. Получаем OIDC конфигурацию (чтобы достать token_endpoint)
        string configUrl = $"{issuerUrl}/.well-known/openid-configuration";
        var configRequest = UnityWebRequest.Get(configUrl);
        await SendWebRequestTask(configRequest);

        if (configRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"[OidcTokenExchange] Config request failed: {configRequest.error}");
            return;
        }

        var config = JsonUtility.FromJson<OidcConfiguration>(configRequest.downloadHandler.text);
        if (string.IsNullOrEmpty(config.token_endpoint))
        {
            Debug.LogError("[OidcTokenExchange] token_endpoint not found in OIDC configuration.");
            return;
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
            var tokenResponse = JsonUtility.FromJson<TokenResponse>(tokenRequest.downloadHandler.text);
            if (!string.IsNullOrEmpty(tokenResponse.IdToken))
            {
                Debug.Log("[OidcTokenExchange] ID token received. Signing in...");
                var signInTask = AuthenticationService.Instance.SignInWithOpenIdConnectAsync("oidc-yandex", tokenResponse.IdToken);
                await signInTask;

                if (signInTask.IsCompletedSuccessfully)
                    Debug.Log("[Auth] Signed in successfully.");
                else
                    Debug.LogError("[Auth] Sign-in failed: " + signInTask.Exception?.Message);
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

    public WebViewAuthHandler webViewHandler;

    public void OnLoginButtonClick()
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

        webViewHandler.StartURL(authUrl);
        Debug.Log("[Auth] URL авторизации подготовлен. WebView откроется при инициализации.");
    }
}

[Serializable]
public class OidcConfiguration
{
    public string token_endpoint;
}

//[Serializable]
//public class TokenResponse
//{
//    public string access_token;
//    public string id_token;
//    public int expires_in;
//    public string token_type;
//}
