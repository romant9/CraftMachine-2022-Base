using UnityEngine;

public class AutoScrapItem2 : MonoBehaviour
{
	[SerializeField]
	private UILabel ScrapLabel;

	private WorkshopPopup popup;

	private string autoScrapKey;

	public void SetKey(WorkshopPopup popup, string key)
	{
		this.popup = popup;
		autoScrapKey = key;
		ScrapLabel.text = LocalizationManager.GetText(key);
	}

	private void OnLanguageChanged()
	{
		ScrapLabel.text = LocalizationManager.GetText(autoScrapKey);
	}

	public void OnClick()
	{
		popup.SetAutoScrap(autoScrapKey);
	}
}
