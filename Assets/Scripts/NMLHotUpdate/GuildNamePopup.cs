using TWDModel;
using UnityEngine;

public class GuildNamePopup : HUDElement
{
	[SerializeField]
	private UIInput nameInput;

	[SerializeField]
	private UILabel title;

	[SerializeField]
	private UILabel subtitle;

	[Header("Price")]
	[SerializeField]
	private UILabel priceLabel;

	[SerializeField]
	private UISprite priceSprite;

	private string GuildName;

	public override void Open()
	{
		base.Open();
		string guildName = GameManager.Instance.playerModel.GuildName;
		nameInput.value = guildName;
		string textId = "Popup.GuildName.Comfirm";
		string textId2 = "Popup.GuildName.Input";
		if (title != null)
		{
			title.text = LocalizationManager.GetText(textId);
		}
		if (subtitle != null)
		{
			subtitle.text = LocalizationManager.GetText(textId2);
		}
		nameInput.characterLimit = 15;
		nameInput.defaultText = LocalizationManager.GetText("Popup.CreateGuild.EnterName");
		priceSprite.spriteName = HelpersGfx.GetCurrencyIconName(CurrencyType.Diamonds);
		HelpersUI.SetContentToLabel(priceLabel, GameManager.Instance.gameEconomyData.ConfigData.GuildNameChangeCost.ToString());
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("camp/open_radiotent");
	}

	public void OnOkClicked()
	{
		GuildModel guildModel = GameManager.Instance.guildModel;
		GuildName = nameInput.value.Trim();
		if (!guildModel.IsValidNameLength(GuildName))
		{
			HUDNotification.Error(LocalizationManager.GetText("Popup.EnterName.InvalidLength{Min}{Max}", 3, 15));
			return;
		}
		if (!guildModel.IsValidCharacters(GuildName))
		{
			HUDNotification.Error(LocalizationManager.GetText("Popup.EnterName.InvalidName"));
			return;
		}
		int guildNameChangeCost = GameManager.Instance.gameEconomyData.ConfigData.GuildNameChangeCost;
		CurrencyType currencyType = CurrencyType.Diamonds;
		if (GameManager.Instance.playerModel.GetCurrencyAmount(currencyType) >= guildNameChangeCost)
		{
			ConsumeCurrencyCommandUtils.ExecuteForSocialCommands(GameManager.Instance.GuildManager.GetChangeGuildNameCashier(), ChangeGuidlName);
		}
		else
		{
			MiniShopPopup.OpenWithTotalRequiredCurrencyAmount(CurrencyType.Diamonds, guildNameChangeCost);
		}
	}

	private void ChangeGuidlName(TWDModelResult result)
	{
		if (result == TWDModelResult.OK)
		{
			Helpers.ExecuteCommand(new ChangeGuildNameCommand
			{
				Name = GuildName
			});
			Close();
		}
	}
}
