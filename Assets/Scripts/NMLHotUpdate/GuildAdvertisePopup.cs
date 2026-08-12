using TWDModel;
using UnityEngine;

public class GuildAdvertisePopup : ConfirmationPopup
{
	[SerializeField]
	private PayButton payButton;

	[SerializeField]
	private UILabel adTimeLabel;

	[SerializeField]
	private GuildAdListCard adPreview;

	private Cashier cashier;

	public virtual void SetContent(string title, string info, int amount, CurrencyType currencyType = CurrencyType.Diamonds)
	{
		SetContent(title, info);
		cashier = GameManager.Instance.playerModel.GetCashierForGuildAd();
		payButton.UpdateUI(cashier);
		if (adTimeLabel != null)
		{
			string text = Helpers.FormatTimeNoZero(GameManager.Instance.gameEconomyData.ConfigData.GuildAdExpirationTime * 1000);
			adTimeLabel.text = LocalizationManager.GetText("Popup.GuildAdvertisement.PublishInfo{time}", text);
		}
		if (adPreview != null)
		{
			adPreview.Item = GameManager.Instance.guildModel;
			adPreview.gameObject.SetActive(value: true);
			adPreview.UpdateUI();
		}
	}

	public override void OkPressed()
	{
		EventManager.NotifyClick("OkPressed");
		if (okCallback != null)
		{
			okCallback();
		}
	}
}
