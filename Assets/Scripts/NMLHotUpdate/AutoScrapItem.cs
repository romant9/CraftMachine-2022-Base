using UnityEngine;

public class AutoScrapItem : MonoBehaviour
{
	[SerializeField]
	private UILabel ScrapLabel;

	private SettingsPopup popup;

	private string autoScrapKey;

	public void SetKey(SettingsPopup popup, string key)
	{
		this.popup = popup;
		autoScrapKey = key;
		ScrapLabel.text = LocalizationManager.GetText(key);
	}

	public void OnClick()
	{
		if ((bool)popup)
		{
			popup.SetAutoScrap(autoScrapKey);
		}
	}
}
