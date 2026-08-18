using System;
using UnityEngine;
using System.Threading.Tasks;

#if UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE_WIN
using Gree.UnityWebView;
#endif

public class WebViewAuthHandler : MonoBehaviour
{
    public string AuthUrl { get; set; }
    public OidcTokenExchangeAsync tokenExchange;

    public WebViewObject _webViewObj;

    void Start()
    {
        
    }

    public async void StartURL(string url)
    {
        AuthUrl = url;
        //AuthUrl = "https://github.com/gree/unity-webview?ysclid=msyzrbbas709157511";
        if (tokenExchange == null)
        {
            Debug.LogError("[WebViewAuth] tokenExchange не назначен в инспекторе!");
            return;
        }

#if UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE_WIN
        if (_webViewObj == null)
        {
            Debug.LogError(
                "[WebViewAuth] На объекте нет компонента WebViewObject! " +
                "Добавь компонент WebViewObject из плагина unity-webview на этот GameObject."
            );
            return;
        }

        // Все колбэки — Action<string>, чтобы точно совпасть с твоей сборкой
        _webViewObj.Init(
            cb: msg => {
                // Общие сообщения плагина (можно раскомментировать для отладки)
                // Debug.Log($"[WebView] cb: {msg}");
            },
            err: msg => HandleErrorChanged(msg),
            httpErr: msg => HandleHttpErrorChanged(msg), // один string
            ld: url => HandleUrlChanged(url),                              // в этой сборке ld — это Action<string> с URL
            started: msg => { },
            hooked: msg => { },
            cookies: msg => { }
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
#else
        Debug.LogWarning("[WebViewAuth] Плагин unity-webview не поддерживается на этой платформе (заглушка).");
#endif
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
        //var js = "";
        //_webViewObj.EvaluateJS(js + @"Unity.call('ua=' + navigator.userAgent)");

        // Проверяем наш callback-редирект
        if (!url.StartsWith(tokenExchange.redirectUri))
        {
            Debug.Log("Фиксируем редирект");
            // Все остальные URL просто игнорируем — WebView сам их отображает
            return;
        }

        Debug.Log($"[WebViewAuth] Перехвачен редирект: {url}");

        string code = ExtractCodeFromUrl(url);
        if (!string.IsNullOrEmpty(code))
        {
            Debug.Log($"[WebViewAuth] Извлечён code: {code}");
            tokenExchange.ExchangeCodeForTokens(code);

            // Опционально: закрываем WebView после получения кода
            // _webViewObj.Dispose();
        }
        else
        {
            Debug.LogError("[WebViewAuth] Не удалось извлечь code из URL.");
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
