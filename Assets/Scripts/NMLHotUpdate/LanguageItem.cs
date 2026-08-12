using UnityEngine;
using TwdCustomMod;

public class LanguageItem : MonoBehaviour
{
	[SerializeField]
	private UILabel language;

	private SettingsPopup popup;

	private string languageKey;

	public void SetKey(SettingsPopup popup, string key)
	{
		this.popup = popup;
		languageKey = key;
		language.text = LocalizationManager.GetText("LanguageName." + key.ToLower());
	}

	public void OnClick()
	{
		if (popup == null && OfflineManager.IsLoadDataManager)
		{
			CraftSettings.Instance.SetLanguage(languageKey);
		}
		else
		{
			popup.SetLanguage(languageKey);
		}
	}
}
