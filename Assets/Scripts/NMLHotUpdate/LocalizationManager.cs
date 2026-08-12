using Client.Connectivity;
using Mono.Posix;
using System;
using System.Collections;
using System.Collections.Generic;
using TwdCustomMod;
using TWDModel;
using TWDModel.ContentTypes;
using UnityEngine;

public class LocalizationManager : SingularityMonoBehaviour<LocalizationManager>
{
	public string StartLanguage = "en";

	public bool UseOnlyLocalFiles = true;

	public string CurrentLanguage;

	private string _cachedUrl;

	private Dictionary<string, string> _terms;

	private bool _loading;

	private const string _cacheType = "LocalizationFile";

	public List<string> SupportedLanguages => ContentTypeDefinitions.SupportedLanguages;

	public event LanguageChangedCallback OnLocalizationLanguageChanged;

	public void Load(string language, bool forceUpdate = false)
	{
		if (!SupportedLanguages.Contains(language))
		{
			Debug.LogError("Unsupported language " + language);
			return;
		}
		if (language != CurrentLanguage || forceUpdate)
		{
			ContentCache cache = ContentManager.Instance.GetCache("LocalizationFile");
			_cachedUrl = cache.GetUrlById(language);
			string text = cache.GetContentById<string>(language);
			if (string.IsNullOrEmpty(text))
			{
                var asset = (TextAsset)Resources.Load($"Localization/SourceFiles/{language}");
				if (asset)
				{
                    text = asset.text;
                }
            }
			if (string.IsNullOrEmpty(text))
			{
				UseOnlyLocalFiles = false;

                if (SignalRClient.Instance && SignalRClient.Instance.state == SignalRClientState.Disconnected && SignalRClient.Instance.IsTokenAwailable)
                {
                    SignalRClient.Instance.IsOnlyGetImagesData = true;
					SignalRClient.Instance.TryReconnect();
                }
            }
			else
			{
                LoadLanguage(language, text);
            }
			_loading = false;
		}
		if (UseOnlyLocalFiles || _loading)
		{
			return;
		}
		if (OfflineManager.IsLoadDataManager && !OfflineManager.IsInternetOn)
		{
			DebugTWD.LogMycode("if (OfflineManager.IsLoadDataManager && !OfflineManager.IsInternetOn) return");
			return;
		}

		_loading = true;
		ContentManager.Instance.LoadContent("Localization/" + language, delegate (string transactionId, bool loaded)
		{
			if (!loaded)
			{
				_loading = false;
			}
			else
			{
				string content = ContentManager.Instance.GetContent(transactionId);
                List<string> list = null;
                try
                {
                    list = GameManager.Instance.jsonSerializer.DeserializeObject<List<string>>(content);
                }
                catch (Exception ex)
                {
                    Debug.LogError("Localization URL Deserialization Exception: " + ex.Message);
                    if (!HelpersModel.IsOffThinkingAnalytics) AnalyticsManager.instance.CreateEvent("Load_Localisation_UrlDeserialization").AddProperty("LocalisationUrl", content).Send();
                }
                if (list == null || list.Count == 0 || string.IsNullOrEmpty(list[0]))
                {
                    Debug.LogWarning("No localization files received for language " + language);
                    _loading = false;
                }
                else
                {
                    GameManager.TryExtractChecksumFromUrl(list[0], out string extractedChecksum, "Load_Localisation", "Localisation");
                    if (list[0] != _cachedUrl)
                    {
                        ContentManager.Instance.GetCDNContent(list[0], "LocalizationFile", language, delegate (string cdnContent)
                        {
                            if (cdnContent != null)
                            {
                                LoadLanguage(language, cdnContent);
                            }
                            _loading = false;
                        }, extractedChecksum);
                    }
                    else
                    {
                        _loading = false;
                    }
                }
            }
		});
	}

	private void LoadLanguage(string language, string json)
	{
		if (string.IsNullOrEmpty(json))
		{
			Debug.LogError("Empty localization for " + language);
			return;
		}
		LanguageDto languageDto = null;
		try
		{
			languageDto = JsonUtility.FromJson<LanguageDto>(json);
		}
		catch (Exception ex)
		{
			Debug.LogError("Localization Deserialization Exception: " + ex.Message);
			return;
		}
		if (languageDto == null)
		{
			Debug.LogError("Localization was null after Deserialization" + language);
			return;
		}
		int num = languageDto.terms.Length;
		_terms = new Dictionary<string, string>(num / 2);
		for (int i = 1; i < num; i += 2)
		{
			string key = languageDto.terms[i - 1];
			string value = languageDto.terms[i];
			_terms[key] = value;
		}
		CurrentLanguage = language;
		UIRoot.Broadcast("OnLanguageChanged", this);
		if (this.OnLocalizationLanguageChanged != null)
		{
			this.OnLocalizationLanguageChanged(CurrentLanguage);
		}
		DebugTWD.Log("Load language " + CurrentLanguage, DebugType.Load);
	}

	public bool ShouldWaitForLocalizations()
	{
		return _loading;
	}

	public IEnumerator WaitForLocalizations()
	{
		while (_loading)
		{
			yield return null;
		}
	}

	protected override void AwakeInternal()
	{
		if (OfflineManager.IsLoadDataManager)
		{
			UseOnlyLocalFiles = true;
			Load("ru", forceUpdate: true);
			return;
		}

		UseOnlyLocalFiles = true;
		string playerLanguageKey = GetPlayerLanguageKey();
		Load(playerLanguageKey);
	}

	public string GetLocalizedText(string textId, params object[] arguments)
	{
		if (textId == null)
		{
			Debug.LogError("Trying to get null entry from localization manager!");
			return "<null>";
		}
		if (_terms.TryGetValue(textId, out var value))
		{
			try
			{
				return string.Format(value, arguments).Replace("\\n", "\n");
			}
			catch (FormatException)
			{
				Debug.LogWarning("LocalizationManager failed to format string " + value);
				return value;
			}
		}
		if (GameConfiguration.Instance.Config.ShowDebugMenu || OfflineManager.IsDebugLocalization)
		{
			return textId;
		}
		return "...";
	}

	public bool HasLocalizedText(string textId)
	{
		string value;
		return _terms.TryGetValue(textId, out value);
	}

	public static string GetText(string textId, params object[] arguments)
	{
		if (SingularityMonoBehaviour<LocalizationManager>.Instance == null)
		{
			return "";
		}
		return SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText(textId, arguments);
	}

	public List<string> GetKeysThatContain(string text)
	{
		List<string> list = new List<string>();
		foreach (string key in _terms.Keys)
		{
			if (key.Contains(text))
			{
				list.Add(key);
			}
		}
		return list;
	}

	public bool LocalizationExists(string textId)
	{
		if (_terms.TryGetValue(textId, out var _))
		{
			return true;
		}
		return false;
	}

	protected string GetSystemLanguageKey()
	{
		string result = StartLanguage;
		switch (Application.systemLanguage)
		{
		case SystemLanguage.German:
			result = "de";
			break;
		case SystemLanguage.Spanish:
			result = "es";
			break;
		case SystemLanguage.French:
			result = "fr";
			break;
		case SystemLanguage.Italian:
			result = "it";
			break;
		case SystemLanguage.Portuguese:
			result = "pt-br";
			break;
		case SystemLanguage.Russian:
			result = "ru";
			break;
		case SystemLanguage.Turkish:
			result = "tr";
			break;
		case SystemLanguage.Chinese:
			result = "zh-cn";
			break;
		case SystemLanguage.ChineseSimplified:
			result = "zh-cn";
			break;
		case SystemLanguage.ChineseTraditional:
			result = "zh-tw";
			break;
		case SystemLanguage.Japanese:
			result = "ja";
			break;
		case SystemLanguage.Korean:
			result = "ko";
			break;
		}
		return result;
	}

	protected string GetPlayerLanguageKey()
	{
		string startLanguage = StartLanguage;
		if (TWDPlayerPrefs.HasKey("PlayerSelectedLanguage"))
		{
			startLanguage = TWDPlayerPrefs.GetString("PlayerSelectedLanguage");
			switch (startLanguage)
			{
			case "pt":
				startLanguage = "pt-br";
				TWDPlayerPrefs.SetString("PlayerSelectedLanguage", startLanguage);
				break;
			case "cn":
				startLanguage = "zh-cn";
				TWDPlayerPrefs.SetString("PlayerSelectedLanguage", startLanguage);
				break;
			case "tw":
				startLanguage = "zh-tw";
				TWDPlayerPrefs.SetString("PlayerSelectedLanguage", startLanguage);
				break;
			}
		}
		else
		{
			startLanguage = GetSystemLanguageKey();
		}
		return startLanguage;
	}



	#region mycode
	public static string GetCustomText(string textId, params object[] arguments)
	{
		if (textId == null)
		{
			Debug.LogError("Trying to get null entry from localization manager!");
			return "<null>";
		}
		if (OfflineManager.IsBundlesLoaded && GameConfiguration.Instance.Config.ShowDebugMenu)
		{
			return textId;
		}
		string value = CustomLocalization.GetText(textId);
        try
        {
            return string.Format(value, arguments).Replace("\\n", "\n");
        }
        catch (FormatException)
        {
            Debug.LogWarning("LocalizationManager failed to format string " + value);
            return value;
        }
    }
	#endregion
}
