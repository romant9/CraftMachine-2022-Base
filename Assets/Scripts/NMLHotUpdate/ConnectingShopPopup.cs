using UnityEngine;

public class ConnectingShopPopup : HUDElement
{
	[SerializeField]
	private UILabel MessageLabel;

	public void SetMessageText(string text)
	{
		MessageLabel.text = text;
	}

	public void SetMessageLocalizationKey(string localizationKey)
	{
		MessageLabel.GetComponent<LocalizationUIUpdater>().LocalizationKey = localizationKey;
	}
}
