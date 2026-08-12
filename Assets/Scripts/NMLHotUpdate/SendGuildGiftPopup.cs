using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class SendGuildGiftPopup : HUDElement
{
	[SerializeField]
	private UIInput messageInput;

	[SerializeField]
	private UILabel guildInfoLabel;

	[SerializeField]
	private UILabel giftValueLabel;

	[SerializeField]
	private UIButton dropRateInfoButton;

	[SerializeField]
	private PayButton claimWithTokenButton;

	[SerializeField]
	private UILabel claimButtonCurrencyAmountLabel;

	public override void Open()
	{
		base.Open();
		defaultPopup.ShowPayButtons();
		defaultPopup.HideInstantPayButton();
		defaultPopup.SetPayButton(LocalizationManager.GetText("Popup.SendGift.Button"), GameManager.Instance.playerModel.GetCashierForGuildGift(usePerk: false));
		defaultPopup.SetPayButtonClickCallback(OnSendGift);
		RefreshClaimButton();
		GuildModel guildModel = GameManager.Instance.playerModel.GuildModel;
		if (messageInput != null)
		{
			messageInput.defaultText = LocalizationManager.GetText("Popup.SendGift.DefaultGiftMessage");
		}
		if (guildInfoLabel != null && guildModel != null)
		{
			guildInfoLabel.text = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Popup.SendGift.Banner", guildModel.GuildMembers.Count - 1);
		}
		if (giftValueLabel != null && guildModel != null)
		{
			int guildGiftSingleGoldValue = GameManager.Instance.gameEconomyData.ConfigData.GuildGiftSingleGoldValue;
			giftValueLabel.text = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Popup.SendGift.Value", guildGiftSingleGoldValue * (guildModel.GuildMembers.Count - 1));
		}
	}

	public override void Close()
	{
		base.Close();
		SingularityMonoBehaviour<HUDManager>.Instance.CloseIfExists(UIType.IngameLoading);
	}

	public void OnDropRateClicked()
	{
		if (GameManager.Instance.gameEconomyData != null && GameManager.Instance.playerModel != null)
		{
			DropRatesInfoPopup obj = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.DropRatesInfoPopup) as DropRatesInfoPopup;
			DropType usedDropType = DropType.None;
			List<ItemAmountProbabilityData> probabilities = GameManager.Instance.gameEconomyData.GetCurrencyProbabilities(DropEventDefinition.DropEventType.GuildGift, DropType.Gold, DropEventDefinition.DropEventContext.Normal, DropEventDefinition.DropEventTag.None, GameManager.Instance.playerModel.Level, out usedDropType, GameManager.Instance.playerModel.ActivityManager);
			DropRatesNamesHelper.GetNamesForDropCurrencies(ref probabilities);
			DropTableItem dropTableItem = new DropTableItem
			{
				DropName = LocalizationManager.GetText("Droprate.Table.Name.GuildGift"),
				Description = LocalizationManager.GetText("Droprate.Table.Description.GuildGift"),
				Probabilities = probabilities
			};
			obj.TryOpenWithNormalData(dropTableItem);
		}
	}

	private void OnSendGift()
	{
		PlayerModel playerModel = GameManager.Instance.playerModel;
		GuildModel guildModel = GameManager.Instance.guildModel;
		if (playerModel != null && guildModel != null)
		{
			ConsumeCurrencyCommandUtils.ExecuteForSocialCommands(GameManager.Instance.playerModel.GetCashierForGuildGift(usePerk: false), SendGiftCallback);
		}
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_click");
	}

	private void SendGiftCallback(TWDModelResult result)
	{
		if (result == TWDModelResult.OK)
		{
			SendGift(usePerk: false);
		}
	}

	private void SendGift(bool usePerk)
	{
		PlayerModel playerModel = GameManager.Instance.playerModel;
		GuildModel guildModel = GameManager.Instance.guildModel;
		if (playerModel != null && guildModel != null && playerModel.CanGiveGuildGift() && guildModel.CanGiveGift())
		{
			GiveGuildGiftCommand obj = new GiveGuildGiftCommand
			{
				GiftSenderName = playerModel.Name,
				GiftSenderId = playerModel.HashedId,
				GiftType = DropType.Gold,
				ExpirationTimeMs = GameManager.Instance.playerModel.gameEconomyData.ConfigData.GuildGiftExpireTimer * 1000,
				UsePerk = usePerk
			};
			string message = ((messageInput != null && !string.IsNullOrEmpty(messageInput.value)) ? messageInput.value : LocalizationManager.GetText("Popup.SendGift.DefaultGiftMessage"));
			obj.Message = message;
			Helpers.ExecuteCommand(obj);
			UIEvent.Send("OnGuildGiftSent");
			Close();
		}
	}

	private void RefreshClaimButton()
	{
		PlayerModel playerModel = GameManager.Instance.playerModel;
		Cashier cashierForGuildGift = playerModel.GetCashierForGuildGift(usePerk: true);
		CurrencyModel currency = playerModel.GetCurrency(CurrencyType.FreeGuildGiftPerk);
		BattlePassModel battlePass = playerModel.BattlePass;
		claimWithTokenButton.UpdateUI(cashierForGuildGift, LocalizationManager.GetText("Consumable.Menu.Button.Use"));
		HelpersUI.SetContentToLabel(claimButtonCurrencyAmountLabel, "1 (" + ((currency.Value > 99) ? "99+" : currency.Value.ToString()) + ")");
		Helpers.GameObjectSetActive(claimWithTokenButton, (battlePass.IsSeasonActive && !battlePass.PremiumActive) || cashierForGuildGift.CanAfford());
	}

	public async void ClaimClick()
	{
		if (GameManager.Instance.playerModel.GetCashierForGuildGift(usePerk: true).CanAfford())
		{
			SendGift(usePerk: true);
		}
		else if (!GameManager.Instance.playerModel.BattlePass.PremiumActive && await ((BattlePassPurchaseInfoPopup)SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.BattlePassPremiumPurchaseInfoPopup)).OpenWithConfirmationAsync())
		{
			RefreshClaimButton();
		}
	}

	public void FreeGuildPerkLabelClicked(GameObject clickedObject)
	{
		TooltipManager.OpenTextBoxWithText(clickedObject, LocalizationManager.GetText("Tooltip.GuildInfo.SendGift.FreeGuildPerk"));
	}
}
