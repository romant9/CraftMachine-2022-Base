using Gree.UnityWebView;
using PlayId.Scripts.Data;
using System;
using System.Threading.Tasks;
using UnityEngine;

public class WebViewAuthHandler : MonoBehaviour
{
    public string AuthUrl { get; set; }
	private CustomAuthSettings _settings;

    public enum AuthSettingsType
    {
        YandexSettings,
        GoogleSettings
    }

    public AuthSettingsType _AuthSettingsType;

	private OidcTokenExchangeAsync tokenExchange;

    private WebViewObject _webViewObj;
    [SerializeField]
    private string altUrl;
    public delegate void OnTokenRecieved(string token);
    private OnTokenRecieved onTokenRecieved;

	void Start()
    {
        
    }

    public void OnClickLogin(OnTokenRecieved callback)
    {
        onTokenRecieved = callback;

        if (string.IsNullOrEmpty(altUrl))
        {
            AuthUrl = altUrl;
		}
        else
        {
			_settings = Resources.Load<CustomAuthSettings>(Enum.GetName(typeof(AuthSettingsType), _AuthSettingsType));
			tokenExchange = new OidcTokenExchangeAsync(_settings);
            tokenExchange.GetAuthUrl();
		}

		Debug.Log("[WebViewAuth] URL авторизации подготовлен. WebView откроется при инициализации.");

		StartURL();
	}

	private async void StartURL()
    {
        if (_webViewObj == null)
        {
			_webViewObj = new GameObject("WebViewObject").AddComponent<WebViewObject>();
        }

        // Все колбэки — Action<string>, чтобы точно совпасть с твоей сборкой
        _webViewObj.Init(
            cb: msg => 
            {
				// Общие сообщения плагина (можно раскомментировать для отладки)
				Debug.Log($"[WebView] cb: {msg}");
            },
            err: msg => HandleErrorChanged(msg),
            httpErr: msg => HandleHttpErrorChanged(msg), // один string
            ld: url => HandleUrlChanged(url), // в этой сборке ld — это Action<string> с URL
            started: msg =>
            {
				Debug.Log($"[WebView] started: {msg}");
				HandleUrlChanged(msg);
			},

			hooked: msg => { Debug.Log($"[WebView] hooked: {msg}"); },
            cookies: msg => { Debug.Log($"[WebView] cookies: {msg}"); }
        );

        while (!_webViewObj.IsInitialized())
        {
            await Task.Yield();
        }

        _webViewObj.SetMargins(5, 100, 5, 5);
        _webViewObj.SetTextZoom(100);
        _webViewObj.SetVisibility(true);

        Debug.Log("[WebViewAuth] WebView инициализирован. Загружаем авторизацию...");
        _webViewObj.LoadURL(AuthUrl);
    }

    private void StopWebView()
    {
		if (_webViewObj != null)
		{
			Debug.Log($"[WebViewAuth] Закрываем WebView");
			Destroy(_webViewObj.gameObject);
			_webViewObj = null;
		}
	}

    private void HandleErrorChanged(string message)
    {
        Debug.LogError($"[WebView] HTTP error: {message}");
    }

    private void HandleHttpErrorChanged(string message)
    {
        Debug.LogError($"[WebView] err: {message}");
    }

    // Сюда приходит URL от колбэка ld
    private void HandleUrlChanged(string url)
    {
        // Проверяем наш callback-редирект
        if (!url.StartsWith(tokenExchange.redirectUri))
        {
            return;
        }

        Debug.Log($"[WebViewAuth] Перехвачен редирект: {url}");

        ExchangeIdTokenFromUrl(url);
    }

    private async void ExchangeIdTokenFromUrl(string url)
    {
		string code = ExtractCodeFromUrl(url);
        StopWebView();

		if (string.IsNullOrEmpty(code))
        {
			Debug.LogError("[WebViewAuth] Не удалось извлечь code из URL.");
            return;
		}
		Debug.Log($"[WebViewAuth] Извлечён code: {code}");

		string idToken = await tokenExchange.PerformTokenExchangeWithPkce(code);

		if (!string.IsNullOrEmpty(idToken))
		{
			Debug.Log("[WebViewAuth] Извлечён idToken. Signing in...");

            onTokenRecieved?.Invoke(idToken);
		}
		else
		{
			Debug.LogError("[WebViewAuth] id_token missing in response.");
		}
	}

	private string ExtractCodeFromUrl(string url)
    {
        var queryPart = url.Split('?');
        if (queryPart.Length < 2) return null;

        var pairs = queryPart[1].Split('&');
        foreach (var pair in pairs)
        {
            var parts = pair.Split('=');
            if (parts.Length == 2 && parts[0] == "code")
            {
                return parts[1];
            }
        }
        return null;
    }
}
