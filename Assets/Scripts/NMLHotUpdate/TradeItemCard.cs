using System;
using System.Collections.Generic;
using BaseModel;
using TWDModel;
using UnityEngine;

public class TradeItemCard : ShopCardBase<TradeSlotInfo>
{
	[Header("Generic")]
	[SerializeField]
	private UILabel TitleLabel;

	[SerializeField]
	private UIButtonWithLabel button;

	[Header("Reward")]
	[SerializeField]
	private UIAtlas UIShopAtlas;

	[SerializeField]
	private UIAtlas UIShopSurivorTokensAtlas;

	[SerializeField]
	private UISprite RewardIconSprite;

	[SerializeField]
	private UITexture rewardConsumableTexure;

	[SerializeField]
	private UILabel RewardAmountLabel;

	[SerializeField]
	private GameObject rewardDraw3Stars;

	[SerializeField]
	private GameObject rewardDraw4Stars;

	[SerializeField]
	private GameObject rewardDraw5Stars;

	[SerializeField]
	private UILabel rewardDrawLabel;

	[SerializeField]
	private UILabel timeLeftLabel;

	[SerializeField]
	private GameObject lockedContainer;

	[SerializeField]
	private GameObject boughtAlreadyContainer;

	[SerializeField]
	private UILabel boughtAlreadyLabel;

	[SerializeField]
	private UILabel lockedLabel;

	[SerializeField]
	private GameObject lockedGoldContainer;

	[SerializeField]
	private UISprite lockedGoldCurrency;

	[SerializeField]
	private UILabel lockedGoldPrice;

	[SerializeField]
	private GameObject specialOfferSticker;

	[SerializeField]
	private GameObject equipmentCardContainer;

	[SerializeField]
	private GameObject equipmentTokenCardContainer;

	[SerializeField]
	private GameObject equipmentCardPrefab;

	[SerializeField]
	private GameObject equipmentTokenCardPrefab;

	[SerializeField]
	private UILabel progressLabel;

	[SerializeField]
	private UIProgressBar progressBar;

	[SerializeField]
	private UITexture crateTexture;

	[SerializeField]
	private GameObject tooltipParent;

	[Header("Price")]
	[SerializeField]
	private UISprite PriceIconSprite;

	[SerializeField]
	private UILabel PriceAmountLabel;

	[SerializeField]
	private UILabel PriceFreeLabel;

	[SerializeField]
	private UISprite outfitIcon;

	[SerializeField]
	private UISprite defaultOutfitIcon;

	[SerializeField]
	private UIButton buyButton;

	[SerializeField]
	private GameObject normalPurchaseContainer;

	[SerializeField]
	private GameObject goldPurchaseContainer;

	[SerializeField]
	private UIButtonWithLabel goldBuyButton;

	[SerializeField]
	private UILabel goldPriceAmountLabel;

	[SerializeField]
	private UIButton infoButton;

	[SerializeField]
	[Tooltip("The tint color for labels when the user has enough currency.")]
	private ColorAsset availableCurrencyColor;

	[SerializeField]
	[Tooltip("The tint color for labels when the user has NOT enough currency.")]
	private ColorAsset unavailableCurrencyColor;

	[Header("Tween Groups")]
	[Tooltip("What Tweens to trigger when this item was bought")]
	[SerializeField]
	private int onPurchaseTweenGroup;

	[Tooltip("What Tweens to trigger when trade shop reset happends")]
	[SerializeField]
	private int onResetTweenGroup = 1;

	[Tooltip("What Tweens to trigger when the card is updated, e.g. price changes")]
	[SerializeField]
	private int onUpdateTweenGroup = 5;

	private EquipmentButton equipmentButton;

	private EquipmentTokenButton equipmentTokenButton;

	private bool showBoughtEffectOnNextUpdateUI;

	public override void AddListeners()
	{
		base.AddListeners();
		if (button != null)
		{
			button.SetClickCallback(OnButtonClicked);
		}
		if (goldBuyButton != null)
		{
			goldBuyButton.SetClickCallback(OnButtonClicked);
		}
		UIEvent.OnUIEvent -= UIEvent_OnUIEvent;
		UIEvent.OnUIEvent += UIEvent_OnUIEvent;
	}

	public void OnInfoClicked()
	{
		TradeSlotInfo data = GetData();
		if (data != null && data.CurrentTradeDefinition.SoldItems.RewardsList[0] is RewardTradeCrate rewardTradeCrate)
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
					Description = HelpersLocalization.GetShopTooltipForIReward(data.CurrentTradeDefinition.SoldItems.RewardsList[0]),
					Probabilities = probabilities
				};
				obj.TryOpenWithNormalData(dropTableItem);
			}
		}
	}

	public override void RemoveListeners()
	{
		base.RemoveListeners();
		if (button != null)
		{
			button.RemoveClickCallback(OnButtonClicked);
		}
		if (goldBuyButton != null)
		{
			goldBuyButton.RemoveClickCallback(OnButtonClicked);
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
		if (equipmentTokenButton != null && equipmentTokenButton.gameObject != null)
		{
			Helpers.DestroyOrCache(equipmentTokenButton.gameObject);
			equipmentTokenButton = null;
		}
		showBoughtEffectOnNextUpdateUI = false;
		if (button != null)
		{
			button.Clear();
		}
	}

	private void UIEvent_OnUIEvent(string type, object parameter)
	{
		switch (type)
		{
		case "OnTradeCrateSlotPurchased":
		case "OnTradeCratePurchased":
		case "OnTradeEquipmentPurchased":
			if (parameter != null && parameter is TradeSlotDefinition && GetData() != null && GetData().Bought && GetData().SlotDefinition != null)
			{
				if ((parameter as TradeSlotDefinition).SlotId == GetData().SlotDefinition.SlotId)
				{
					showBoughtEffectOnNextUpdateUI = true;
				}
				else
				{
					UpdateUI();
				}
			}
			break;
		case "OnPopUpClose":
			if (parameter is IAPConfirmPopupNew || parameter is OpenLootInUi)
			{
				UpdateUI();
			}
			break;
		}
	}

	public void Update()
	{
		if (!(timeLeftLabel != null) || !timeLeftLabel.gameObject.activeInHierarchy)
		{
			return;
		}
		TradeSlotInfo data = GetData();
		if (data != null)
		{
			long timeLeft = data.CurrentTradeDefinition.GetTimeLeft(GameManager.Instance.playerModel.UtcTimeStamp);
			if (timeLeft > 0)
			{
				timeLeftLabel.text = Helpers.FormatTime(timeLeft);
			}
			else
			{
				timeLeftLabel.text = LocalizationManager.GetText("Popup.BuildMenu.NoTimeLeft");
			}
		}
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
		TradeSlotInfo data = GetData();
		Dictionary<string, GameObject> dictionary = new Dictionary<string, GameObject>();
		dictionary.Add("TradeCrateGearLow", rewardDraw3Stars);
		dictionary.Add("TradeCrateGearMid", rewardDraw4Stars);
		dictionary.Add("TradeCrateGearHigh", rewardDraw5Stars);
		Helpers.GameObjectSetActive(rewardDraw3Stars, value: false);
		Helpers.GameObjectSetActive(rewardDraw4Stars, value: false);
		Helpers.GameObjectSetActive(rewardDraw5Stars, value: false);
		if (data == null)
		{
			Debug.LogError("TradeItemCard: TradeDefinition is NULL");
			base.gameObject.SetActive(value: false);
			return;
		}
		if (buyButton != null)
		{
			buyButton.gameObject.SetActive(value: true);
		}
		if (goldPurchaseContainer != null)
		{
			goldPurchaseContainer.gameObject.SetActive(value: false);
		}
		if (defaultOutfitIcon != null)
		{
			defaultOutfitIcon.gameObject.SetActive(value: false);
		}
		if (timeLeftLabel != null)
		{
			timeLeftLabel.gameObject.SetActive(value: false);
		}
		if (specialOfferSticker != null)
		{
			specialOfferSticker.SetActive(value: false);
		}
		if (lockedGoldContainer != null)
		{
			lockedGoldContainer.SetActive(value: false);
		}
		if (outfitIcon != null)
		{
			outfitIcon.gameObject.SetActive(value: false);
		}
		if (PriceFreeLabel != null)
		{
			PriceFreeLabel.gameObject.SetActive(value: false);
		}
		if (crateTexture != null)
		{
			crateTexture.gameObject.SetActive(value: false);
		}
		if (rewardConsumableTexure != null)
		{
			rewardConsumableTexure.gameObject.SetActive(value: false);
		}
		if (timeLeftLabel != null && specialOfferSticker != null && data.CurrentTradeDefinition.HasDateLimit && !data.Bought)
		{
			timeLeftLabel.gameObject.SetActive(value: true);
			specialOfferSticker.SetActive(value: true);
		}
		if (RewardIconSprite != null && RewardAmountLabel != null && data.CurrentTradeDefinition.SoldItems.RewardsList[0] is RewardCurrency)
		{
			RewardCurrency rewardCurrency = data.CurrentTradeDefinition.SoldItems.RewardsList[0] as RewardCurrency;
			HelpersGfx.SetShopAtlasToSprite(rewardCurrency.CurrencyType, RewardIconSprite, UIShopAtlas, UIShopSurivorTokensAtlas);
			RewardIconSprite.spriteName = HelpersGfx.GetCurrencyIconName(rewardCurrency.CurrencyType, GameManager.Instance.playerModel);
			RewardIconSprite.gameObject.SetActive(value: true);
			RewardAmountLabel.text = rewardCurrency.Amount.ToString();
			RewardAmountLabel.gameObject.SetActive(value: true);
		}
		else if (RewardIconSprite != null && RewardAmountLabel != null)
		{
			RewardIconSprite.gameObject.SetActive(value: false);
			RewardAmountLabel.gameObject.SetActive(value: false);
		}
		if (data.CurrentTradeDefinition.SoldItems.RewardsList[0] is RewardEquipment rewardEquipment && rewardEquipment.IsConsumableReward(GameManager.Instance.modelManager))
		{
			rewardConsumableTexure.mainTexture = HelpersGfx.GetTextureForEquipmentReward(rewardEquipment);
			Helpers.GameObjectSetActive(rewardConsumableTexure, value: true);
			RewardAmountLabel.text = rewardEquipment.Amount.ToString();
			Helpers.GameObjectSetActive(RewardAmountLabel, value: true);
		}
		else if (equipmentCardPrefab != null && equipmentCardContainer != null && (data.CurrentTradeDefinition.SoldItems.RewardsList[0] is RewardEquipment || data.CurrentTradeDefinition.SoldItems.RewardsList[0] is RewardRandomEquipment))
		{
			if (data.CurrentTradeDefinition.SoldItems.RewardsList[0] is RewardEquipment)
			{
				RewardEquipment rewardEquipment2 = data.CurrentTradeDefinition.SoldItems.RewardsList[0] as RewardEquipment;
				if (equipmentButton == null)
				{
					equipmentButton = Helpers.InstantiateWithComponent<EquipmentButton>(equipmentCardPrefab, equipmentCardContainer);
				}
				if (equipmentButton != null)
				{
					EquipmentDefinition equipmentDefinition = rewardEquipment2.EquipmentDefinition(GameManager.Instance.modelManager);
					bool flag = equipmentDefinition?.TraitsOverride != null && equipmentDefinition.TraitsOverride.Count > 0;
					equipmentButton.Setup(rewardEquipment2, allowClick: false, !flag);
				}
			}
			else
			{
				RewardRandomEquipment rewardRandomEquipment = data.CurrentTradeDefinition.SoldItems.RewardsList[0] as RewardRandomEquipment;
				int levelOut;
				EquipmentDefinition randomEquipmentDefinition = rewardRandomEquipment.GetRandomEquipmentDefinition(GameManager.Instance.modelManager, new ModelRandom((int)GameManager.Instance.playerModel.LastTradeShopRefreshTime + data.CurrentTradeDefinition.UniqueId), out levelOut);
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
		else if (equipmentTokenCardPrefab != null && equipmentTokenCardContainer != null && data.CurrentTradeDefinition.SoldItems.RewardsList[0] is RewardEquipToken)
		{
			RewardEquipToken rewardEquipToken = data.CurrentTradeDefinition.SoldItems.RewardsList[0] as RewardEquipToken;
			if (equipmentTokenButton == null)
			{
				equipmentTokenButton = Helpers.InstantiateWithComponent<EquipmentTokenButton>(equipmentTokenCardPrefab, equipmentTokenCardContainer);
			}
			if (equipmentTokenButton != null)
			{
				equipmentTokenButton.SetUpForTrade(rewardEquipToken);
				RewardAmountLabel.text = rewardEquipToken.RewardAmount.ToString();
				Helpers.GameObjectSetActive(RewardAmountLabel, value: true);
			}
		}
		bool flag2 = false;
		Helpers.GameObjectSetActive(rewardDrawLabel, value: false);
		IReward reward = data.CurrentTradeDefinition.SoldItems.RewardsList[0];
		if (!(reward is RewardTradeCrate rewardTradeCrate))
		{
			if (!(reward is RewardOutfit rewardOutfit))
			{
				if (reward is RewardTimedBonus rewardTimedBonus)
				{
					RewardIconSprite.spriteName = HelpersGfx.GetRewardTimedBonusIcon(rewardTimedBonus);
					RewardIconSprite.gameObject.SetActive(value: true);
				}
			}
			else
			{
				OutfitDefinition outfitDefinition = GameManager.Instance.gameEconomyData.GetOutfitDefinition(rewardOutfit.PreferredOrder[0]);
				if (outfitDefinition != null && outfitIcon != null && defaultOutfitIcon != null)
				{
					if (string.IsNullOrEmpty(outfitDefinition.BundleSprite))
					{
						defaultOutfitIcon.gameObject.SetActive(value: true);
					}
					else
					{
						outfitIcon.gameObject.SetActive(value: true);
						outfitIcon.spriteName = outfitDefinition.BundleSprite;
					}
				}
			}
		}
		else
		{
			foreach (KeyValuePair<string, GameObject> item in dictionary)
			{
				if (item.Value != null)
				{
					item.Value.SetActive(rewardTradeCrate.TradeCrateId == item.Key);
				}
			}
			if (rewardDrawLabel != null)
			{
				rewardDrawLabel.gameObject.SetActive(value: true);
				rewardDrawLabel.text = LocalizationManager.GetText("TradeItems.Card.Content." + rewardTradeCrate.TradeCrateId);
			}
			if (crateTexture != null)
			{
				crateTexture.gameObject.SetActive(value: true);
				crateTexture.material = HelpersGfx.GetTradeCrateMaterial(rewardTradeCrate.TradeCrateId);
			}
		}
		bool flag3 = false;
		CurrencyType currencyType;
		int purchasePrice = data.GetPurchasePrice(out currencyType);
		if (!string.IsNullOrEmpty(data.SlotDefinition.UnlockRequirement) && data.SlotDefinition.CurrencyUnlock != CurrencyType.None && GameManager.Instance.playerModel.BoughtTradeCrateSlotAmount < data.GoldUnlockSlot)
		{
			flag2 = true;
			if (lockedGoldCurrency != null)
			{
				lockedGoldCurrency.spriteName = HelpersGfx.GetCurrencyIconName(data.SlotDefinition.CurrencyUnlock);
			}
			if (lockedGoldPrice != null)
			{
				lockedGoldPrice.text = Helpers.FormatNumber(data.SlotDefinition.CurrencyUnlockAmount);
				if (Cashier.CreateOneItemCashier(GameManager.Instance.modelManager, PurchaseType.TradeCrateSlot, data.SlotDefinition.CurrencyUnlock, data.SlotDefinition.CurrencyUnlockAmount).CanAfford())
				{
					lockedGoldPrice.color = availableCurrencyColor.Color;
				}
				else
				{
					lockedGoldPrice.color = unavailableCurrencyColor.Color;
				}
			}
			if (lockedGoldContainer != null)
			{
				lockedGoldContainer.SetActive(value: true);
			}
			if (GameManager.Instance.playerModel.BoughtTradeCrateSlotAmount + 1 < data.GoldUnlockSlot && buyButton != null)
			{
				buyButton.gameObject.SetActive(value: false);
			}
		}
		if (PriceIconSprite != null)
		{
			PriceIconSprite.spriteName = HelpersGfx.GetCurrencyIconName(currencyType);
		}
		if (lockedContainer != null && !string.IsNullOrEmpty(data.SlotDefinition.UnlockRequirement) && data.SlotDefinition.CurrencyUnlock == CurrencyType.None)
		{
			bool flag4 = GameManager.Instance.playerModel.RankingScore >= data.SlotDefinition.CurrencyUnlockAmount;
			lockedContainer.SetActive(!flag4);
			if (lockedLabel != null && progressBar != null && progressLabel != null && !flag4)
			{
				flag2 = true;
				lockedLabel.gameObject.SetActive(value: true);
				lockedLabel.text = LocalizationManager.GetText("Popup.BuildMenu.Unlock.Slot.Influence{amount}", data.SlotDefinition.CurrencyUnlockAmount);
				progressLabel.text = GameManager.Instance.playerModel.RankingScore + "/" + data.SlotDefinition.CurrencyUnlockAmount;
				progressBar.value = Mathf.InverseLerp(0f, data.SlotDefinition.CurrencyUnlockAmount, GameManager.Instance.playerModel.RankingScore);
			}
		}
		UILabel priceAmountLabel = PriceAmountLabel;
		if (currencyType == CurrencyType.Diamonds)
		{
			priceAmountLabel = goldPriceAmountLabel;
			Helpers.GameObjectSetActive(normalPurchaseContainer, value: false);
			Helpers.GameObjectSetActive(goldPurchaseContainer, value: true);
		}
		else
		{
			Helpers.GameObjectSetActive(normalPurchaseContainer, value: true);
			Helpers.GameObjectSetActive(goldPurchaseContainer, value: false);
		}
		Helpers.GameObjectSetActive(infoButton, data.CurrentTradeDefinition.ShowProbability);
		if (priceAmountLabel != null && !flag2)
		{
			if (purchasePrice == 0 && currencyType != CurrencyType.Diamonds)
			{
				PriceAmountLabel.gameObject.SetActive(value: false);
				if (PriceFreeLabel != null)
				{
					PriceFreeLabel.gameObject.SetActive(value: true);
				}
			}
			else
			{
				priceAmountLabel.gameObject.SetActive(value: true);
				string text = Helpers.FormatNumber(purchasePrice);
				if (priceAmountLabel.text != text && GetData().PurchaseCount > 0 && !GetData().Bought)
				{
					flag3 = true;
				}
				priceAmountLabel.text = text;
				Helpers.GameObjectSetActive(PriceIconSprite, value: true);
			}
			if (Cashier.CreateOneItemCashier(GameManager.Instance.modelManager, PurchaseType.TradeCrate, currencyType, purchasePrice).CanAfford())
			{
				priceAmountLabel.color = availableCurrencyColor.Color;
			}
			else
			{
				priceAmountLabel.color = unavailableCurrencyColor.Color;
			}
		}
		if (TitleLabel != null)
		{
			TitleLabel.text = HelpersLocalization.GetRewardLocalizedName(data.CurrentTradeDefinition.SoldItems.RewardsList[0], (int)GameManager.Instance.playerModel.LastTradeShopRefreshTime + data.CurrentTradeDefinition.UniqueId);
		}
		if (boughtAlreadyContainer != null)
		{
			boughtAlreadyContainer.SetActive(data.Bought);
			if (data.Bought)
			{
				string text2 = "";
				text2 = ((data.SlotDefinition.PriceCategory == PriceCategory.Discount) ? ((data.CurrentTradeDefinition.PriceDiscountAmount != 0) ? LocalizationManager.GetText("Popup.BuildMenu.Sold") : LocalizationManager.GetText("Popup.BuildMenu.Claimed")) : ((data.CurrentTradeDefinition.PriceNormalAmount != 0) ? LocalizationManager.GetText("Popup.BuildMenu.Sold") : LocalizationManager.GetText("Popup.BuildMenu.Claimed")));
				if (boughtAlreadyLabel != null)
				{
					boughtAlreadyLabel.text = text2;
				}
				if (PriceIconSprite != null)
				{
					PriceIconSprite.gameObject.SetActive(value: false);
				}
				if (PriceAmountLabel != null)
				{
					PriceAmountLabel.gameObject.SetActive(value: false);
				}
			}
		}
		if (showBoughtEffectOnNextUpdateUI)
		{
			showBoughtEffectOnNextUpdateUI = false;
			TriggerPurchaseEffects();
		}
		if (flag3)
		{
			TriggerUpdateEffects();
		}
	}

	public override void SetData(TradeSlotInfo data)
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

	public void TriggerUpdateEffects()
	{
		TweenManager.PlayTweenGroup(base.gameObject, onUpdateTweenGroup);
	}

	protected override void OnClickedTooltipButton(UIButtonExtended button)
	{
		base.OnClickedTooltipButton(button);
		TradeSlotInfo data = GetData();
		if (data != null && data.CurrentTradeDefinition?.SoldItems?.RewardsList != null && data.CurrentTradeDefinition.SoldItems.RewardsList.Count > 0)
		{
			TooltipManager.OpenTextBoxWithText(base.gameObject, HelpersLocalization.GetShopTooltipForIReward(data.CurrentTradeDefinition.SoldItems.RewardsList[0]));
		}
	}

	public void OnButtonClicked(UIButtonExtended button)
	{
		ShopPopup shopPopup = SingularityMonoBehaviour<HUDManager>.Instance.GetNoCreation(UIType.ShopPopup) as ShopPopup;
		if (shopPopup != null)
		{
			shopPopup.SetLastButtonClicked(this);
		}
		TradeSlotInfo tradeSlot = GetData();
		if (tradeSlot == null || tradeSlot.Bought || GameManager.Instance.playerModel.BoughtTradeCrateSlotAmount + 1 < tradeSlot.GoldUnlockSlot)
		{
			return;
		}
		Cashier cashier = null;
		bool flag = !string.IsNullOrEmpty(tradeSlot.SlotDefinition.UnlockRequirement) && tradeSlot.SlotDefinition.CurrencyUnlock != CurrencyType.None && GameManager.Instance.playerModel.BoughtTradeCrateSlotAmount < tradeSlot.GoldUnlockSlot;
		if (flag)
		{
			cashier = Cashier.CreateOneItemCashier(GameManager.Instance.modelManager, PurchaseType.TradeCrateSlot, tradeSlot.SlotDefinition.CurrencyUnlock, tradeSlot.SlotDefinition.CurrencyUnlockAmount);
		}
		else
		{
			CurrencyType currencyType;
			int purchasePrice = tradeSlot.GetPurchasePrice(out currencyType);
			if (currencyType != CurrencyType.None)
			{
				cashier = Cashier.CreateOneItemCashier(GameManager.Instance.modelManager, PurchaseType.TradeCrate, currencyType, purchasePrice);
			}
		}
		bool flag2 = true;
		if (!string.IsNullOrEmpty(tradeSlot.SlotDefinition.UnlockRequirement) && tradeSlot.SlotDefinition.CurrencyUnlock == CurrencyType.None)
		{
			flag2 = GameManager.Instance.playerModel.RankingScore >= tradeSlot.SlotDefinition.CurrencyUnlockAmount;
		}
		if (!flag2)
		{
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/invalid_action");
			TooltipManager.OpenTextBoxWithText(tooltipParent, LocalizationManager.GetText("Tooltip.UnlockTradeSlotInfluence"));
		}
		else if (flag)
		{
			BuyResourcesPopup buyResourcesPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.BuyResourcesPopup) as BuyResourcesPopup;
			buyResourcesPopup.SetContent(LocalizationManager.GetText("Popup.BuyResources.TradeCrateSlot"), LocalizationManager.GetText("Popup.BuyResources.TradeCrateSlot.Description"), tradeSlot.SlotDefinition.CurrencyUnlockAmount, tradeSlot.SlotDefinition.CurrencyUnlock);
			buyResourcesPopup.SetMissingCurrencies(cashier, showDiamonds: false);
			buyResourcesPopup.SetCallbacks(delegate
			{
				if (GameManager.Instance.playerModel.LootManager.GetCashierForTradeSlot(tradeSlot).CanAfford())
				{
					PlayerModel playerModel = GameManager.Instance.playerModel;
					if (Helpers.ExecuteCommand(new BuyTradeCrateSlotCommand(tradeSlot.SlotDefinition.SlotId)
					{
						Cashier = playerModel.LootManager.GetCashierForTradeCrate(tradeSlot)
					}) == TWDModelResult.OK)
					{
						SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/purchase");
					}
					UIEvent.Send("OnTradeCrateSlotPurchased", tradeSlot.SlotDefinition);
				}
				else
				{
					UIPanel component = buyResourcesPopup.GetComponent<UIPanel>();
					if (component != null)
					{
						component.depth = BuyResourcesPopup.DefaultDepth;
					}
					ShopPopupHelper.OpenForMissingCurrencyWithTotalRequiredAmount(tradeSlot.SlotDefinition.CurrencyUnlockAmount);
				}
			});
			buyResourcesPopup.Open();
			if (buyResourcesPopup.GetComponent<UIPanel>() != null)
			{
				buyResourcesPopup.GetComponent<UIPanel>().depth = BuyResourcesPopup.TradeShopDepth;
			}
		}
		else if ((tradeSlot.CurrentTradeDefinition.SoldItems.RewardsList[0] is RewardEquipment rewardEquipment && !rewardEquipment.IsConsumableReward(GameManager.Instance.modelManager)) || tradeSlot.CurrentTradeDefinition.SoldItems.RewardsList[0] is RewardRandomEquipment)
		{
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_click");
			EquipmentUpgradePopup obj = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CampEquipmentLevelUpPopup) as EquipmentUpgradePopup;
			obj.ShowNextLevel = false;
			obj.OpenForEquipmentTradeItem(tradeSlot);
			CampHUD.Get().PauseCurrencyMeters = false;
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/equipment_click");
		}
		else
		{
			if (GameManager.Instance.playerModel == null)
			{
				return;
			}
			CurrencyType purchaseCurrency;
			int purchasePrice2 = tradeSlot.GetPurchasePrice(out purchaseCurrency);
			BuyTradeCrateCommand buyTradeCommand = new BuyTradeCrateCommand(tradeSlot.SlotDefinition.SlotId);
			buyTradeCommand.Cashier = Cashier.CreateOneItemCashier(GameManager.Instance.modelManager, PurchaseType.TradeCrate, purchaseCurrency, purchasePrice2);
			if (buyTradeCommand.Cashier.IsFree())
			{
				ConsumeCurrencyCommandUtils.Execute(buyTradeCommand, tradePurchaseCallback);
				return;
			}
			if (!buyTradeCommand.Cashier.CanAfford())
			{
				ConsumeCurrencyCommandUtils.Execute(buyTradeCommand, tradePurchaseCallback);
				return;
			}
			BuyResourcesPopup buyResourcesPopup2 = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.BuyResourcesPopup) as BuyResourcesPopup;
			buyResourcesPopup2.SetCallbacks(delegate
			{
				if (purchaseCurrency == CurrencyType.Diamonds && buyTradeCommand.Cashier.CanAfford())
				{
					TWDModelResult result = Helpers.ExecuteCommand(buyTradeCommand);
					tradePurchaseCallback(result);
				}
				else
				{
					ConsumeCurrencyCommandUtils.Execute(buyTradeCommand, tradePurchaseCallback);
				}
			});
			buyResourcesPopup2.Open();
			buyResourcesPopup2.SetConfirmContent(LocalizationManager.GetText("Popup.BuyResources.TradeCrate"), "", purchasePrice2, purchaseCurrency, tradeSlot.CurrentTradeDefinition.SoldItems.RewardsList[0]);
			if (buyResourcesPopup2.GetComponent<UIPanel>() != null)
			{
				buyResourcesPopup2.GetComponent<UIPanel>().depth = BuyResourcesPopup.TradeShopDepth;
			}
		}
	}

	private void tradePurchaseCallback(TWDModelResult result)
	{
		TradeSlotInfo data = GetData();
		if (result != TWDModelResult.OK)
		{
			return;
		}
		PlayerModel playerModel = GameManager.Instance.playerModel;
		UIEvent.Send("OnTradeCratePurchased", data.SlotDefinition);
		if (data.CurrentTradeDefinition.SoldItems.RewardsList[0] is RewardTradeCrate)
		{
			OpenLootInUi openLootInUi = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.OpenLootInUi) as OpenLootInUi;
			if (openLootInUi != null)
			{
				SingularityMonoBehaviour<HUDManager>.Instance.CloseIfExists(UIType.CampBuildMenu);
				openLootInUi.OpenForModel(playerModel.LootManager);
				openLootInUi.ShowShopWhenClosed = true;
			}
			else
			{
				Debug.LogError("Reward type " + data.CurrentTradeDefinition.SoldItems.RewardsList[0].Type.ToString() + " not supported in trade shop");
			}
			(SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CampBuildMenu) as BuildMenu).Close();
		}
		else if (data.CurrentTradeDefinition.SoldItems.RewardsList[0] is RewardCurrency)
		{
			IAPConfirmPopupNew obj = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.IAPConfirmPopupNew) as IAPConfirmPopupNew;
			obj.OpenForCurrency(data.CurrentTradeDefinition.SoldItems.RewardsList[0] as RewardCurrency, isGift: false);
			obj.ShowShopWhenClosed = true;
		}
		else if (data.CurrentTradeDefinition.SoldItems.RewardsList[0] is RewardOutfit)
		{
			RewardOutfit rewardOutfit = data.CurrentTradeDefinition.SoldItems.RewardsList[0] as RewardOutfit;
			IAPConfirmPopupNew obj2 = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.IAPConfirmPopupNew) as IAPConfirmPopupNew;
			obj2.OpenForOutfit(rewardOutfit);
			obj2.ShowShopWhenClosed = true;
		}
		else if (data.CurrentTradeDefinition.SoldItems.RewardsList[0] is RewardEquipment rewardEquipment && rewardEquipment.IsConsumableReward(GameManager.Instance.modelManager))
		{
			IAPConfirmPopupNew iAPConfirmPopupNew = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.IAPConfirmPopupNew) as IAPConfirmPopupNew;
			if (playerModel.LootManager.LastTradedEquipment != null)
			{
				iAPConfirmPopupNew.OpenForConsumable(rewardEquipment);
			}
			iAPConfirmPopupNew.ShowShopWhenClosed = true;
		}
		else if (data.CurrentTradeDefinition.SoldItems.RewardsList[0] is RewardEquipToken rewardEquipToken)
		{
			IAPConfirmPopupNew obj3 = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.IAPConfirmPopupNew) as IAPConfirmPopupNew;
			obj3.OpenForEquipmentToken(rewardEquipToken.FakeRewardEquipTokenItemModel(GameManager.Instance.modelManager));
			obj3.ShowShopWhenClosed = true;
		}
		else if (data.CurrentTradeDefinition.SoldItems.RewardsList[0] is RewardTimedBonus)
		{
			IAPConfirmPopupNew obj4 = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.IAPConfirmPopupNew) as IAPConfirmPopupNew;
			obj4.OpenForTimedReward(data.CurrentTradeDefinition.SoldItems.RewardsList[0] as RewardTimedBonus);
			obj4.ShowShopWhenClosed = true;
		}
		else
		{
			Debug.LogError("Reward type " + data.CurrentTradeDefinition.SoldItems.RewardsList[0].Type.ToString() + " not supported in trade shop");
		}
		(SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CampBuildMenu) as BuildMenu).Close();
	}
}
