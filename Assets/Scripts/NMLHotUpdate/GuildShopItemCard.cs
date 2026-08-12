using System;
using System.Collections.Generic;
using BaseModel;
using TWDModel;
using UnityEngine;

public class GuildShopItemCard : ShopCardBase<GuildShopItemInfo>
{
	[Header("Generic")]
	[SerializeField]
	private UILabel TierLabel;

	[Tooltip("Used when personal highest is higher than current guild tier")]
	[SerializeField]
	private GameObject PersonalTierContainer;

	[SerializeField]
	private UILabel TitleLabel;

	[SerializeField]
	private UIButtonWithLabel Button;

	[SerializeField]
	private GameObject amountAvailableContainer;

	[SerializeField]
	private ShowTooltip restockInfoButton;

	[SerializeField]
	private UILabel amountAvailableLabel;

	[SerializeField]
	private GameObject newItemContainer;

	[SerializeField]
	private UISprite backgroundImage;

	[SerializeField]
	private Color defaultCardColor;

	[SerializeField]
	private Color[] categoryCardColors;

	[Header("Reward")]
	[SerializeField]
	private UIAtlas UIShopAtlas;

	[SerializeField]
	private UIAtlas UIShopSurivorTokensAtlas;

	[SerializeField]
	private UISprite RewardIconSprite;

	[SerializeField]
	private UILabel RewardAmountLabel;

	[SerializeField]
	private UILabel rewardDrawLabel;

	[SerializeField]
	private GameObject equipmentCardContainer;

	[SerializeField]
	private GameObject equipmentCardPrefab;

	[SerializeField]
	private UITexture crateTexture;

	[SerializeField]
	private UITexture weaponTexture;

	[SerializeField]
	private GameObject tooltipParent;

	[Header("Locked State")]
	[SerializeField]
	private GameObject lockedContainer;

	[SerializeField]
	private UISprite lockedTierIcon;

	[SerializeField]
	private UILabel lockedVpLabel;

	[SerializeField]
	private UILabel lockedPriceLabel;

	[Header("Locked State - Next Tier")]
	[SerializeField]
	private GameObject lockedNextTier;

	[SerializeField]
	private UISprite lockedNextTierIcon;

	[SerializeField]
	private UILabel lockedNextVpLabel;

	[SerializeField]
	private UIProgressBar lockedNextProgressbar;

	[SerializeField]
	private UILabel lockedNextPriceLabel;

	[Header("Locked State - Special")]
	[SerializeField]
	private GameObject lockedSpecial;

	[SerializeField]
	private UILabel lockedSpecialLabel;

	[Header("Sold Out State")]
	[SerializeField]
	private GameObject boughtAlreadyContainer;

	[SerializeField]
	private UILabel boughtAlreadyLabel;

	[SerializeField]
	private ShowTooltip soldOutRestockInfoButton;

	[Header("Price")]
	[SerializeField]
	private UISprite PriceIconSprite;

	[SerializeField]
	private UILabel PriceAmountLabel;

	[SerializeField]
	private UILabel PriceFreeLabel;

	[SerializeField]
	private UIButton infoButton;

	[SerializeField]
	[Tooltip("The tint color for labels when the user has enough currency.")]
	private Color availableCurrencyColor = Color.white;

	[SerializeField]
	[Tooltip("The tint color for labels when the user has NOT enough currency.")]
	private Color unavailableCurrencyColor = new Color(0.511f, 0.129f, 0.027f, 1f);

	[Header("Tween Groups")]
	[Tooltip("What Tweens to trigger when this item was bought")]
	[SerializeField]
	private int onPurchaseTweenGroup;

	[Tooltip("What Tweens to trigger when trade shop reset happends")]
	[SerializeField]
	private int onResetTweenGroup = 1;

	private EquipmentButton equipmentButton;

	private bool showBoughtEffectOnNextUpdateUI;

	public override void AddListeners()
	{
		base.AddListeners();
		if (Button != null)
		{
			Button.SetClickCallback(OnButtonClicked);
		}
		UIEvent.OnUIEvent -= UIEvent_OnUIEvent;
		UIEvent.OnUIEvent += UIEvent_OnUIEvent;
	}

	public void OnInfoClicked()
	{
		GuildShopItemInfo data = GetData();
		if (data != null && data.ItemDefinition.ContentRewards.RewardsList[0] is RewardTradeCrate rewardTradeCrate)
		{
			DropType usedDropType = DropType.Regular;
			if (GameManager.Instance.gameEconomyData != null && GameManager.Instance.playerModel != null)
			{
				List<ItemAmountProbabilityData> probabilities = GameManager.Instance.gameEconomyData.GetCurrencyProbabilities(DropEventDefinition.DropEventType.TradeCrate, DropType.Regular, DropEventDefinition.DropEventContext.Normal, (DropEventDefinition.DropEventTag)Enum.Parse(typeof(DropEventDefinition.DropEventTag), rewardTradeCrate.TradeCrateId), GameManager.Instance.playerModel.Level, out usedDropType, GameManager.Instance.playerModel.ActivityManager);
				DropRatesNamesHelper.GetNamesForDropCurrencies(ref probabilities);
				DropRatesInfoPopup obj = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.DropRatesInfoPopup) as DropRatesInfoPopup;
				DropTableItem dropTableItem = new DropTableItem
				{
					DropName = HelpersLocalization.GetTradeCrateName(rewardTradeCrate.TradeCrateId),
					Description = HelpersLocalization.GetShopTooltipForIReward(data.ItemDefinition.ContentRewards.RewardsList[0]),
					Probabilities = probabilities
				};
				obj.TryOpenWithNormalData(dropTableItem);
			}
		}
	}

	public override void RemoveListeners()
	{
		base.RemoveListeners();
		if (Button != null)
		{
			Button.RemoveClickCallback(OnButtonClicked);
		}
		UIEvent.OnUIEvent -= UIEvent_OnUIEvent;
	}

	public virtual void OnPoolReturn()
	{
		Clear();
	}

	public override void Clear()
	{
		base.Clear();
		if (equipmentButton != null && equipmentButton.gameObject != null)
		{
			Helpers.DestroyOrCache(equipmentButton.gameObject);
			equipmentButton = null;
		}
		showBoughtEffectOnNextUpdateUI = false;
		if (Button != null)
		{
			Button.Clear();
		}
	}

	private void UIEvent_OnUIEvent(string type, object parameter)
	{
		if (type == "OnGuildShopItemPurchased")
		{
			if (parameter != null && parameter is int)
			{
				if ((int)parameter == GetData().ItemDefinition.ID)
				{
					showBoughtEffectOnNextUpdateUI = true;
				}
				else
				{
					UpdateUI();
				}
			}
		}
		else if (type == "OnPopUpClose" && (parameter is IAPConfirmPopupNew || parameter is OpenLootInUi))
		{
			UpdateUI();
		}
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
		GuildShopItemInfo data = GetData();
		if (data != null)
		{
			ResetContainers();
			SetTitleInfo(data);
			SetTierInfo(data);
			SetRewardInfo(data);
			SetPriceInfo(data);
			if (!data.Unlocked)
			{
				SetLockedState(data);
			}
			else
			{
				Helpers.GameObjectSetActive(Button, value: true);
				SetNewState(data);
				SetStockInfo(data);
			}
			if (showBoughtEffectOnNextUpdateUI)
			{
				showBoughtEffectOnNextUpdateUI = false;
				TriggerPurchaseEffects();
			}
		}
		else
		{
			Debug.LogError("GuildShopItemCard: ItemDefinition is NULL");
			base.gameObject.SetActive(value: false);
		}
	}

	private void ResetContainers()
	{
		Helpers.GameObjectSetActive(PriceFreeLabel, value: false);
		Helpers.GameObjectSetActive(crateTexture, value: false);
		Helpers.GameObjectSetActive(boughtAlreadyContainer, value: false);
		Helpers.GameObjectSetActive(lockedContainer, value: false);
		Helpers.GameObjectSetActive(lockedNextTier, value: false);
		Helpers.GameObjectSetActive(lockedSpecial, value: false);
		Helpers.GameObjectSetActive(newItemContainer, value: false);
		Helpers.GameObjectSetActive(amountAvailableContainer, value: false);
		Helpers.GameObjectSetActive(infoButton, value: false);
		Helpers.GameObjectSetActive(PersonalTierContainer, value: false);
		Helpers.GameObjectSetActive(Button, value: false);
	}

	private void SetTierInfo(GuildShopItemInfo itemInfo)
	{
		Color color = defaultCardColor;
		int tierRequirement = itemInfo.ItemDefinition.TierRequirement;
		GuildTierDefinition guildTierDefinition = GameManager.Instance.gameEconomyData.GetGuildTierDefinition(tierRequirement);
		if (guildTierDefinition != null)
		{
			if (guildTierDefinition.Category < categoryCardColors.Length)
			{
				color = categoryCardColors[guildTierDefinition.Category - 1];
			}
			if (backgroundImage != null)
			{
				backgroundImage.color = color;
			}
			HelpersUI.SetContentToLabel(TierLabel, SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText(guildTierDefinition.NameLocalizationKey));
		}
		if (itemInfo.Unlocked)
		{
			GuildTierDefinition currentGuildTier = GuildTierHelper.GetCurrentGuildTier();
			if (currentGuildTier == null || currentGuildTier.Tier > itemInfo.ItemDefinition.TierRequirement)
			{
				Helpers.GameObjectSetActive(PersonalTierContainer, value: true);
			}
		}
	}

	private void SetNewState(GuildShopItemInfo itemInfo)
	{
		Helpers.GameObjectSetActive(newItemContainer, !itemInfo.Seen);
	}

	private void SetLockedState(GuildShopItemInfo itemInfo)
	{
		PlayerModel playerModel = GameManager.Instance.playerModel;
		if (!playerModel.IsGuildMember)
		{
			Helpers.GameObjectSetActive(lockedSpecial, value: true);
			HelpersUI.SetContentToLabel(lockedSpecialLabel, SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("GuildShop.Locked.JoinGuild"));
			return;
		}
		if (itemInfo.ItemDefinition.TierRequirement >= playerModel.GuildModel.GuildBattleTier)
		{
			Helpers.GameObjectSetActive(lockedSpecial, value: true);
			HelpersUI.SetContentToLabel(lockedSpecialLabel, SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("GuildShop.Locked.Participate"));
			return;
		}
		GuildTierDefinition guildTierDefinition = GameManager.Instance.gameEconomyData.GetGuildTierDefinition(itemInfo.ItemDefinition.TierRequirement);
		if (guildTierDefinition != null)
		{
			if (guildTierDefinition.Tier + 1 == playerModel.GuildModel.GuildBattleTier)
			{
				Helpers.GameObjectSetActive(lockedNextTier, value: true);
				HelpersUI.SetSprite(lockedNextTierIcon, guildTierDefinition.IconSprite);
				HelpersUI.SetContentToLabel(lockedNextVpLabel, guildTierDefinition.VictoryPointsRequired.ToString());
				lockedNextProgressbar.Set(GuildTierHelper.GetCurrentProgressToNextTier());
			}
			else
			{
				Helpers.GameObjectSetActive(lockedContainer, value: true);
				HelpersUI.SetSprite(lockedTierIcon, guildTierDefinition.IconSprite);
				HelpersUI.SetContentToLabel(lockedVpLabel, guildTierDefinition.VictoryPointsRequired.ToString());
			}
		}
	}

	private void SetStockInfo(GuildShopItemInfo itemInfo)
	{
		Helpers.GameObjectSetActive(boughtAlreadyContainer, itemInfo.SoldOut);
		Helpers.GameObjectSetActive(amountAvailableContainer, !itemInfo.SoldOut);
		if (itemInfo.ItemDefinition.LimitedPurchases)
		{
			HelpersUI.SetContentToLabel(amountAvailableLabel, SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("GvG.GuildShopCard.PurchaseAmount{parameter}", itemInfo.AvailableAmount));
			bool flag = GuildWarHelper.IsNextWarDuringCurrentSeason();
			if (GuildWarHelper.IsSeasonOngoing())
			{
				string text = "";
				if (itemInfo.ItemDefinition.RestockOnNewTier > 0 && !flag && itemInfo.ItemDefinition.RestockOnNewWar > 0)
				{
					text = "GvG.GuildShopCard.Tooltip.TierWarRestock";
				}
				else if (itemInfo.ItemDefinition.RestockOnNewTier > 0)
				{
					text = "GvG.GuildShopCard.Tooltip.TierRestock";
				}
				else if (!flag && itemInfo.ItemDefinition.RestockOnNewWar > 0)
				{
					text = "GvG.GuildShopCard.Tooltip.WarRestock";
				}
				if (restockInfoButton != null)
				{
					restockInfoButton.LocalizationKey = text;
				}
				if (soldOutRestockInfoButton != null)
				{
					soldOutRestockInfoButton.LocalizationKey = text;
				}
				Helpers.GameObjectSetActive(soldOutRestockInfoButton, !string.IsNullOrEmpty(text));
				Helpers.GameObjectSetActive(restockInfoButton, !string.IsNullOrEmpty(text));
			}
			else
			{
				Helpers.GameObjectSetActive(soldOutRestockInfoButton, value: false);
				Helpers.GameObjectSetActive(restockInfoButton, value: false);
			}
		}
		else
		{
			HelpersUI.SetContentToLabel(amountAvailableLabel, SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("GvG.GuildShopCard.PurchaseUnlimited"));
		}
	}

	private void SetRewardInfo(GuildShopItemInfo itemInfo)
	{
		if (itemInfo.ItemDefinition.ContentRewards.RewardsList[0] is RewardCurrency)
		{
			RewardCurrency rewardCurrency = itemInfo.ItemDefinition.ContentRewards.RewardsList[0] as RewardCurrency;
			HelpersGfx.SetShopAtlasToSprite(rewardCurrency.CurrencyType, RewardIconSprite, UIShopAtlas, UIShopSurivorTokensAtlas);
			HelpersUI.SetSprite(RewardIconSprite, HelpersGfx.GetCurrencyIconName(rewardCurrency.CurrencyType, GameManager.Instance.playerModel));
			HelpersUI.SetContentToLabel(RewardAmountLabel, rewardCurrency.Amount.ToString());
			Helpers.GameObjectSetActive(weaponTexture, value: false);
		}
		else if (itemInfo.ItemDefinition.ContentRewards.RewardsList[0] is RewardTimedBonus rewardTimedBonus)
		{
			if (RewardIconSprite.atlas != UIShopAtlas)
			{
				RewardIconSprite.atlas = UIShopAtlas;
			}
			HelpersUI.SetSprite(RewardIconSprite, HelpersGfx.GetRewardTimedBonusIcon(rewardTimedBonus));
			Helpers.GameObjectSetActive(RewardAmountLabel, value: false);
			Helpers.GameObjectSetActive(weaponTexture, value: false);
		}
		else
		{
			Helpers.GameObjectSetActive(RewardIconSprite, value: false);
			Helpers.GameObjectSetActive(RewardAmountLabel, value: false);
		}
		if (itemInfo.ItemDefinition.ContentRewards.RewardsList[0] is RewardEquipment rewardEquipment && rewardEquipment.IsConsumableReward(GameManager.Instance.modelManager))
		{
			weaponTexture.mainTexture = HelpersGfx.GetTextureForEquipmentReward(rewardEquipment);
			HelpersUI.SetContentToLabel(RewardAmountLabel, rewardEquipment.Amount.ToString());
			Helpers.GameObjectSetActive(RewardAmountLabel, value: true);
			Helpers.GameObjectSetActive(weaponTexture, value: true);
		}
		else
		{
			if (!(equipmentCardPrefab != null) || !(equipmentCardContainer != null) || (!(itemInfo.ItemDefinition.ContentRewards.RewardsList[0] is RewardEquipment) && !(itemInfo.ItemDefinition.ContentRewards.RewardsList[0] is RewardRandomEquipment)))
			{
				return;
			}
			Helpers.GameObjectSetActive(weaponTexture, value: false);
			if (itemInfo.ItemDefinition.ContentRewards.RewardsList[0] is RewardEquipment)
			{
				RewardEquipment rewardEquipment2 = itemInfo.ItemDefinition.ContentRewards.RewardsList[0] as RewardEquipment;
				if (equipmentButton == null)
				{
					equipmentButton = Helpers.InstantiateWithComponent<EquipmentButton>(equipmentCardPrefab, equipmentCardContainer);
				}
				if (equipmentButton != null)
				{
					EquipmentDefinition equipmentDefinition = rewardEquipment2.EquipmentDefinition(GameManager.Instance.modelManager);
					bool flag = equipmentDefinition != null && equipmentDefinition.TraitsOverride != null && equipmentDefinition.TraitsOverride.Count > 0;
					equipmentButton.Setup(rewardEquipment2, allowClick: true, !flag);
				}
			}
			else
			{
				RewardRandomEquipment rewardRandomEquipment = itemInfo.ItemDefinition.ContentRewards.RewardsList[0] as RewardRandomEquipment;
				int levelOut = 0;
				EquipmentDefinition randomEquipmentDefinition = rewardRandomEquipment.GetRandomEquipmentDefinition(GameManager.Instance.modelManager, new ModelRandom(GameManager.Instance.playerModel.GuildShopModel.RandomSeed + itemInfo.ItemDefinition.ID), out levelOut);
				if (equipmentButton == null)
				{
					equipmentButton = Helpers.InstantiateWithComponent<EquipmentButton>(equipmentCardPrefab, equipmentCardContainer);
				}
				if (equipmentButton != null)
				{
					equipmentButton.Setup(randomEquipmentDefinition, rewardRandomEquipment.RarityLevel, levelOut);
				}
			}
		}
	}

	private void SetPriceInfo(GuildShopItemInfo itemInfo)
	{
		CurrencyType priceCurrency = itemInfo.ItemDefinition.PriceCurrency;
		int priceAmount = itemInfo.ItemDefinition.PriceAmount;
		if (!itemInfo.Unlocked)
		{
			HelpersUI.SetContentToLabel(lockedPriceLabel, priceAmount.ToString());
			HelpersUI.SetContentToLabel(lockedNextPriceLabel, priceAmount.ToString());
			return;
		}
		HelpersUI.SetSprite(PriceIconSprite, HelpersGfx.GetCurrencyIconName(priceCurrency));
		if (PriceAmountLabel != null)
		{
			if (priceAmount == 0 && priceCurrency != CurrencyType.Diamonds)
			{
				Helpers.GameObjectSetActive(PriceAmountLabel, value: false);
				Helpers.GameObjectSetActive(PriceFreeLabel, value: true);
			}
			else
			{
				string content = Helpers.FormatNumber(priceAmount);
				HelpersUI.SetContentToLabel(PriceAmountLabel, content);
				Helpers.GameObjectSetActive(PriceIconSprite, value: true);
			}
			if (Cashier.CreateOneItemCashier(GameManager.Instance.modelManager, PurchaseType.TradeCrate, priceCurrency, priceAmount).CanAfford())
			{
				PriceAmountLabel.color = availableCurrencyColor;
			}
			else
			{
				PriceAmountLabel.color = unavailableCurrencyColor;
			}
		}
	}

	private void SetTitleInfo(GuildShopItemInfo itemInfo)
	{
		string content = "";
		Helpers.GameObjectSetActive(rewardDrawLabel, value: false);
		if (itemInfo.ItemDefinition.ContentRewards.RewardsList[0] is RewardEquipment)
		{
			if (itemInfo.ItemDefinition.ContentRewards.RewardsList[0] is RewardEquipment rewardEquipment)
			{
				content = HelpersLocalization.GetEquipmentName(rewardEquipment.EquipmentId);
			}
		}
		else if (itemInfo.ItemDefinition.ContentRewards.RewardsList[0] is RewardCurrency)
		{
			if (itemInfo.ItemDefinition.ContentRewards.RewardsList[0] is RewardCurrency rewardCurrency)
			{
				content = HelpersLocalization.GetCurrencyName(rewardCurrency.CurrencyType);
			}
		}
		else if (itemInfo.ItemDefinition.ContentRewards.RewardsList[0] is RewardRandomEquipment)
		{
			if (itemInfo.ItemDefinition.ContentRewards.RewardsList[0] is RewardRandomEquipment rewardRandomEquipment)
			{
				int levelOut = 0;
				content = HelpersLocalization.GetEquipmentName(rewardRandomEquipment.GetRandomEquipmentDefinition(GameManager.Instance.modelManager, new ModelRandom(GameManager.Instance.playerModel.GuildShopModel.RandomSeed), out levelOut).ID);
			}
		}
		else if (!(itemInfo.ItemDefinition.ContentRewards.RewardsList[0] is RewardTradeCrate))
		{
			content = ((!(itemInfo.ItemDefinition.ContentRewards.RewardsList[0] is RewardTimedBonus reward)) ? ("Slot " + itemInfo.ItemDefinition.ID + " \nitem " + itemInfo.ItemDefinition.Content) : HelpersLocalization.GetBundleTitleForIReward(reward));
		}
		else if (itemInfo.ItemDefinition.ContentRewards.RewardsList[0] is RewardTradeCrate rewardTradeCrate)
		{
			content = HelpersLocalization.GetTradeCrateName(rewardTradeCrate.TradeCrateId);
			HelpersUI.SetContentToLabel(rewardDrawLabel, LocalizationManager.GetText("TradeItems.Card.Content." + rewardTradeCrate.TradeCrateId));
			HelpersUI.SetTextureMaterial(crateTexture, HelpersGfx.GetTradeCrateMaterial(rewardTradeCrate.TradeCrateId));
		}
		HelpersUI.SetContentToLabel(TitleLabel, content);
	}

	public override void SetData(GuildShopItemInfo data)
	{
		base.SetData(data);
		UpdateUI();
	}

	public void TriggerPurchaseEffects()
	{
		TweenManager.PlayTweenGroup(base.gameObject, onPurchaseTweenGroup);
	}

	public void TriggerResetEffects()
	{
		TweenManager.PlayTweenGroup(base.gameObject, onResetTweenGroup);
	}

	protected override void OnClickedTooltipButton(UIButtonExtended button)
	{
		base.OnClickedTooltipButton(button);
		GuildShopItemInfo data = GetData();
		if (data != null && data.ItemDefinition.ContentRewards != null && data.ItemDefinition.ContentRewards.Count > 0)
		{
			TooltipManager.OpenTextBoxWithText(base.gameObject, HelpersLocalization.GetShopTooltipForIReward(data.ItemDefinition.ContentRewards.RewardsList[0]));
		}
	}

	public void OnButtonClicked(UIButtonExtended button)
	{
		ShopPopup shopPopup = SingularityMonoBehaviour<HUDManager>.Instance.GetNoCreation(UIType.ShopPopup) as ShopPopup;
		if (shopPopup != null)
		{
			shopPopup.SetLastButtonClicked(this);
		}
		GuildShopItemInfo data = GetData();
		if (data == null || data.SoldOut)
		{
			return;
		}
		Cashier cashier = null;
		CurrencyType priceCurrency = data.ItemDefinition.PriceCurrency;
		int priceAmount = data.ItemDefinition.PriceAmount;
		cashier = Cashier.CreateOneItemCashier(GameManager.Instance.modelManager, PurchaseType.TradeCrate, priceCurrency, priceAmount);
		if ((data.ItemDefinition.ContentRewards.RewardsList[0] is RewardEquipment rewardEquipment && !rewardEquipment.IsConsumableReward(GameManager.Instance.modelManager)) || data.ItemDefinition.ContentRewards.RewardsList[0] is RewardRandomEquipment)
		{
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_click");
			EquipmentUpgradePopup obj = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CampEquipmentLevelUpPopup) as EquipmentUpgradePopup;
			obj.ShowNextLevel = false;
			obj.OpenForGuildShopItem(data);
			CampHUD.Get().PauseCurrencyMeters = false;
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/equipment_click");
		}
		else
		{
			if (GameManager.Instance.playerModel == null)
			{
				return;
			}
			BuyGuildShopItemCommand buyGuildShopItemCommand = new BuyGuildShopItemCommand(data.ItemDefinition.ID);
			buyGuildShopItemCommand.Cashier = cashier;
			if (cashier.CanAfford())
			{
				ConsumeCurrencyCommandUtils.Execute(buyGuildShopItemCommand, itemPurchasedCallback);
				return;
			}
			bool num = GameManager.Instance.gameEconomyData.FindNextGuildWarWithinSeason(GameManager.Instance.playerModel.UtcTimeStamp, data.ItemDefinition.Season, includeCurrentWar: true) != null;
			bool flag = GameManager.Instance.modelManager.Player.GvGSeasonModel?.FindNextSeason(GameManager.Instance.playerModel.UtcTimeStamp) != null;
			if (num || flag)
			{
				SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.NotEnoughRpPopup).Open();
			}
			else
			{
				AlertPopup.ShowPopup("", SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Popup.GuildShop.NotEnoughRP"), LocalizationManager.GetText("Button.Ok"));
			}
		}
	}

	private void itemPurchasedCallback(TWDModelResult result)
	{
		GuildShopItemInfo data = GetData();
		if (result != TWDModelResult.OK)
		{
			return;
		}
		PlayerModel playerModel = GameManager.Instance.playerModel;
		UIEvent.Send("OnGuildShopItemPurchased", data.ItemDefinition.ID);
		if (data.ItemDefinition.ContentRewards.RewardsList[0] is RewardTradeCrate)
		{
			OpenLootInUi openLootInUi = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.OpenLootInUi) as OpenLootInUi;
			if (openLootInUi != null)
			{
				SingularityMonoBehaviour<HUDManager>.Instance.CloseIfExists(UIType.CampBuildMenu);
				openLootInUi.OpenForModel(playerModel.LootManager);
				openLootInUi.ShowShopWhenClosed = true;
			}
		}
		else if (data.ItemDefinition.ContentRewards.RewardsList[0] is RewardCurrency)
		{
			IAPConfirmPopupNew obj = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.IAPConfirmPopupNew) as IAPConfirmPopupNew;
			obj.OpenForCurrency(data.ItemDefinition.ContentRewards.RewardsList[0] as RewardCurrency, isGift: false);
			obj.ShowShopWhenClosed = true;
		}
		else if (data.ItemDefinition.ContentRewards.RewardsList[0] is RewardEquipment rewardEquipment && rewardEquipment.IsConsumableReward(GameManager.Instance.modelManager))
		{
			IAPConfirmPopupNew iAPConfirmPopupNew = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.IAPConfirmPopupNew) as IAPConfirmPopupNew;
			if (playerModel.LootManager.LastTradedEquipment != null)
			{
				iAPConfirmPopupNew.OpenForConsumable(rewardEquipment);
			}
			iAPConfirmPopupNew.ShowShopWhenClosed = true;
		}
		else if (data.ItemDefinition.ContentRewards.RewardsList[0] is RewardTimedBonus)
		{
			IAPConfirmPopupNew obj2 = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.IAPConfirmPopupNew) as IAPConfirmPopupNew;
			obj2.OpenForTimedReward(data.ItemDefinition.ContentRewards.RewardsList[0] as RewardTimedBonus);
			obj2.ShowShopWhenClosed = true;
		}
		else
		{
			Debug.LogError("Reward type " + data.ItemDefinition.ContentRewards.RewardsList[0].Type.ToString() + " not supported in guild shop");
		}
	}
}
