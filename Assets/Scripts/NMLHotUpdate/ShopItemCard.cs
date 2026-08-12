using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using BaseModel;
using TWDModel;
using UnityEngine;

public class ShopItemCard : ShopCardBase<BundleStoreDefinition>
{
	private enum AfterPurchaseState
	{
		None = 0,
		CanBuyMore = 1,
		RemovedFromStore = 2
	}

	[SerializeField]
	private UIButtonWithLabel button;

	[SerializeField]
	private GameObject buttonTDUSA;

	[SerializeField]
	private UIButtonWithLabel buttonTD;

	[SerializeField]
	private UIButtonWithLabel buttonUSA;

	[SerializeField]
	private GameObject buttonTDBA;

	[SerializeField]
	private UILabel itemNameLabel;

	[SerializeField]
	private UISprite itemSprite;

	[Tooltip("Image from GED/URL")]
	[SerializeField]
	private UITexture itemDynamicTexture;

	[Tooltip("Image from GED/Dashboard")]
	[SerializeField]
	private UITexture itemDynamicTextureItem;

	[Tooltip("Image from GED/Dashboard")]
	[SerializeField]
	private UITexture itemDynamicTextureHero;

	[SerializeField]
	private GameObject currencyParent;

	[SerializeField]
	private UILabel currencyAmountLabel;

	[SerializeField]
	private UISprite currencyIconSprite;

	[SerializeField]
	private UILabel limitedTimeLabel;

	[SerializeField]
	private UILabel salesBadge;

	[SerializeField]
	private UILabel valueBadge;

	[SerializeField]
	private GameObject strikePriceContainer;

	[SerializeField]
	private UILabel strikePriceLabel;

	[Header("Tween Groups")]
	[Tooltip("Tween group will be called on the whole card when the item is purchased")]
	[SerializeField]
	private int ItemBoughtTweenGroup = 4;

	[Tooltip("Tween group will be called on the whole card when the item is purchased and should be removed from the store after the purchase")]
	[SerializeField]
	private int ItemBoughtAndRemovedTweenGroup = 5;

	[Tooltip("Tween group will be called on the itemDynamicTextureHero and itemDynamicTextureItem when the images are loaded")]
	[SerializeField]
	private int ImageLoadCompleteTweenGroup = 10;

	[SerializeField]
	[Tooltip("The tint color for labels when the user has enough currency.")]
	private ColorAsset availableCurrencyColor;

	[SerializeField]
	[Tooltip("The tint color for labels when the user has NOT enough currency.")]
	private ColorAsset unavailableCurrencyColor;

	[Header("Button Web Shop Discount")]
	[SerializeField]
	private DiscountButtonController discountButtonController;

	[Header("Discount UI")]
	[SerializeField]
	private UILabel newAmountLabel;

	[SerializeField]
	private UISprite newIconCurrency;

	[SerializeField]
	private UIButtonWithLabel newButton;

	[SerializeField]
	private GameObject newCurrencyParent;

	private AfterPurchaseState postPurchaseState;

	protected BundleContentDefinition contentDefinition;

	private RewardTimedBonus rewardTimedBonus;

	private InAppPurchaseProductApple iapPurchaseProduct;

	private LimitedBundleData limitedBundleData;

	private ConditionBundleDefinition conditionBundleData;

	private WebShopBundleContent webShopBundleContent;

	private bool speedTokenPass = true;

	private List<IReward> OverSpeedRewardsList = new List<IReward>();

	private int OverSpeedConvertedAmount;

	public BundleStoreDefinition storeDefinition => GetData();

	public override void AddListeners()
	{
		base.AddListeners();
		if (button != null)
		{
			button.SetClickCallback(OnButtonClicked);
		}
		if (newButton != null)
		{
			newButton.SetClickCallback(OnButtonClicked);
		}
		if (buttonUSA != null)
		{
			buttonUSA.SetClickCallback(OnButtonClicked);
		}
		if (buttonTD != null)
		{
			buttonTD.SetClickCallback(OnButtonClicked2);
		}
		if (buttonTDBA != null)
		{
			buttonTDBA.gameObject.SetActive(value: false);
		}
		UIEvent.OnUIEvent -= OnUiEvent;
		UIEvent.OnUIEvent += OnUiEvent;
	}

	public override void RemoveListeners()
	{
		base.RemoveListeners();
		if (button != null)
		{
			button.RemoveClickCallback(OnButtonClicked);
		}
		if (newButton != null)
		{
			newButton.RemoveClickCallback(OnButtonClicked);
		}
		if (buttonUSA != null)
		{
			buttonUSA.RemoveClickCallback(OnButtonClicked);
		}
		if (buttonTD != null)
		{
			buttonTD.RemoveClickCallback(OnButtonClicked2);
		}
		UIEvent.OnUIEvent -= OnUiEvent;
	}

	public virtual void DisableMainShopButton()
	{
		if (button != null)
		{
			button.isEnabled = false;
		}
		if (newButton != null)
		{
			newButton.isEnabled = false;
		}
		if (buttonUSA != null)
		{
			buttonUSA.isEnabled = false;
		}
		if (buttonTD != null)
		{
			buttonTD.isEnabled = false;
		}
	}

	public virtual void EnableMainShopButton()
	{
		if (button != null)
		{
			button.isEnabled = true;
		}
		if (newButton != null)
		{
			newButton.isEnabled = true;
		}
		if (buttonUSA != null)
		{
			buttonUSA.isEnabled = true;
		}
		if (buttonTD != null)
		{
			buttonTD.isEnabled = true;
		}
	}

	public virtual void OnPoolReturn()
	{
		Clear();
		TweenManager.ResetToBeginningTweenGroup(base.gameObject, ItemBoughtTweenGroup);
		TweenManager.ResetToBeginningTweenGroup(base.gameObject, ItemBoughtAndRemovedTweenGroup);
	}

	public override void Clear()
	{
		base.Clear();
		if (button != null)
		{
			button.Clear();
		}
		if (newButton != null)
		{
			newButton.Clear();
		}
		if (discountButtonController != null)
		{
			discountButtonController.ClearData();
		}
		contentDefinition = null;
		rewardTimedBonus = null;
		iapPurchaseProduct = null;
		limitedBundleData = null;
		conditionBundleData = null;
		webShopBundleContent = null;
		postPurchaseState = AfterPurchaseState.None;
	}

	public override void SetData(BundleStoreDefinition bundleStoreDefinition)
	{
		base.SetData(bundleStoreDefinition);
		if (storeDefinition != null)
		{
			conditionBundleData = GameManager.Instance.gameEconomyData.GetConditionBundleDefinition(storeDefinition.BundleIdentifier);
			limitedBundleData = GameManager.Instance.playerModel.BundleManager.GetInitiatedLimitedBundle(storeDefinition.BundleIdentifier);
			Helpers.GameObjectSetActive(limitedTimeLabel, value: false);
			contentDefinition = GameManager.Instance.playerModel.gameEconomyData.GetBundleContentDefinition(storeDefinition.BundleIdentifier);
			if (discountButtonController != null)
			{
				discountButtonController.InitializedeData(storeDefinition);
			}
			if (contentDefinition != null)
			{
				iapPurchaseProduct = GameManager.Instance.gameEconomyData.GetInAppPurchaseProduct(contentDefinition.IAPProduct);
				if (contentDefinition.RewardEntries != null && contentDefinition.RewardEntries.RewardsList.Count == 1)
				{
					rewardTimedBonus = contentDefinition.RewardEntries.RewardsList[0] as RewardTimedBonus;
				}
				EnableMainShopButton();
			}
			else
			{
				Debug.LogError("ShopItemCard: Could not find BundleContentDefinition with BundleIdentifier: " + storeDefinition.BundleIdentifier);
			}
		}
		else
		{
			Debug.LogError("ShopItemCard: BundleStoreDefinition NULL!");
		}
		UpdateUI();
	}

	public virtual void Update()
	{
		if (limitedTimeLabel != null && storeDefinition != null && storeDefinition.ShowTimerInCard && limitedBundleData != null)
		{
			HelpersUI.SetContentToLabel(limitedTimeLabel, Helpers.FormatTimeNoZero(limitedBundleData.Timer));
		}
		else if (limitedTimeLabel != null && storeDefinition != null && conditionBundleData != null)
		{
			long giftLeftTime = GameManager.Instance.playerModel.RFMGiftManager.GetGiftLeftTime(conditionBundleData.BundleIdentifier);
			if (giftLeftTime > 0)
			{
				Helpers.GameObjectSetActive(limitedTimeLabel, value: true);
				HelpersUI.SetContentToLabel(limitedTimeLabel, Helpers.FormatTimeNoZero(giftLeftTime));
			}
			else
			{
				Helpers.GameObjectSetActive(limitedTimeLabel, value: false);
			}
		}
		else
		{
			Helpers.GameObjectSetActive(limitedTimeLabel, value: false);
		}
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
		if (storeDefinition == null || contentDefinition == null)
		{
			return;
		}
		string text = "";
		HelpersUI.SetContentToLabel(content: (!string.IsNullOrEmpty(storeDefinition.OverrideTitleLocalizationDetail)) ? LocalizationManager.GetText("IAPCard.ItemName." + storeDefinition.OverrideTitleLocalizationDetail) : ((!string.IsNullOrEmpty(storeDefinition.OverrideTitleLocalization)) ? LocalizationManager.GetText("IAPCard.ItemName." + storeDefinition.OverrideTitleLocalization) : ((rewardTimedBonus == null) ? LocalizationManager.GetText("IAPCard.ItemName." + storeDefinition.BundleIdentifier) : HelpersLocalization.GetTimedBonusTitle(rewardTimedBonus.TimedBonusType, rewardTimedBonus.Duration))), label: itemNameLabel);
		if (!string.IsNullOrEmpty(storeDefinition.CardMainSpriteName))
		{
			HelpersUI.SetSprite(itemSprite, storeDefinition.CardMainSpriteName);
		}
		else
		{
			Helpers.GameObjectSetActive(itemSprite, value: false);
		}
		if (itemDynamicTexture != null && !string.IsNullOrEmpty(storeDefinition.CardImageURL))
		{
			LoadImageFromUrl loadImageFromUrl = Helpers.AddComponent<LoadImageFromUrl>(base.gameObject);
			if (loadImageFromUrl != null)
			{
				loadImageFromUrl.LoadImage(storeDefinition.CardImageURL, itemDynamicTexture, 1024);
			}
		}
		Helpers.GameObjectSetActive(itemDynamicTexture, !string.IsNullOrEmpty(storeDefinition.CardImageURL));
		LoadImageFromCdn.LoadImageToTarget(itemDynamicTextureItem, storeDefinition.CardImageContentPathItem, clearLocalCachedUrls: false, ImageLoadCompleteTweenGroup);
		LoadImageFromCdn.LoadImageToTarget(itemDynamicTextureHero, storeDefinition.CardImageContentPathHero, clearLocalCachedUrls: false, ImageLoadCompleteTweenGroup);
		int tradeFairPriceNew = contentDefinition.TradeFairPriceNew;
		if (tradeFairPriceNew == 0 || buttonTDUSA == null)
		{
			if (button != null)
			{
				string text2 = "";
				text2 = ((iapPurchaseProduct == null || OfflineManager.IsLoadDataManager) ? LocalizationManager.GetText("Generic.Free") : ((iapPurchaseProduct.PriceUSD > 0f) ? GameManager.Instance.IAPManager.GetFormattedPrice(contentDefinition.IAPProduct) : LocalizationManager.GetText("Generic.Free")));
				button.SetContentToLabelOne(text2, Color.white);
				if (newButton != null)
				{
					newButton.SetContentToLabelOne(text2, Color.white);
				}
			}
			if (buttonTDUSA != null)
			{
				buttonTDUSA.SetActive(value: false);
			}
			button.gameObject.SetActive(value: true);
			Helpers.GameObjectSetActive(newButton, value: true);
		}
		else
		{
			if (buttonUSA != null)
			{
				string text3 = "";
				text3 = ((iapPurchaseProduct == null) ? LocalizationManager.GetText("Generic.Free") : ((iapPurchaseProduct.PriceUSD > 0f) ? GameManager.Instance.IAPManager.GetFormattedPrice(contentDefinition.IAPProduct) : LocalizationManager.GetText("Generic.Free")));
				buttonUSA.SetContentToLabelOne(text3, Color.white);
			}
			if (buttonTD != null)
			{
				int currencyAmount = GameManager.Instance.playerModel.GetCurrencyAmount(CurrencyType.Fairmoney);
				bool flag = tradeFairPriceNew > 0 && currencyAmount < tradeFairPriceNew;
				buttonTD.SetContentToLabelOne(tradeFairPriceNew.ToString(), flag ? unavailableCurrencyColor.Color : availableCurrencyColor.Color);
			}
			if (buttonTDUSA != null)
			{
				buttonTDUSA.SetActive(value: true);
			}
			button.gameObject.SetActive(value: false);
			Helpers.GameObjectSetActive(newButton, value: false);
		}
		if (!string.IsNullOrEmpty(storeDefinition.SalesBadgeLocalisation))
		{
			HelpersUI.SetContentToLabel(salesBadge, LocalizationManager.GetText(storeDefinition.SalesBadgeLocalisation));
		}
		else
		{
			Helpers.GameObjectSetActive(salesBadge, value: false);
		}
		if (!string.IsNullOrEmpty(storeDefinition.ValueBadgeLocalisation))
		{
			HelpersUI.SetContentToLabel(valueBadge, LocalizationManager.GetText(storeDefinition.ValueBadgeLocalisation));
		}
		else
		{
			Helpers.GameObjectSetActive(valueBadge, value: false);
		}
		if (!OfflineManager.IsLoadDataManager) ShowOriginalSalePrice();
		bool num = storeDefinition.CardCurrencyToShow != CurrencyType.None;
		int num2 = 0;
		if (num && contentDefinition.RewardEntries != null)
		{
			num2 = contentDefinition.RewardEntries.GetTotalCurrencyRewardAmount(storeDefinition.CardCurrencyToShow);
		}
		if (num2 > 0)
		{
			HelpersUI.SetContentToLabel(currencyAmountLabel, num2.ToString());
			HelpersUI.SetContentToLabel(newAmountLabel, num2.ToString());
			HelpersUI.SetSprite(currencyIconSprite, HelpersGfx.GetCurrencyIconName(storeDefinition.CardCurrencyToShow));
			HelpersUI.SetSprite(newIconCurrency, HelpersGfx.GetCurrencyIconName(storeDefinition.CardCurrencyToShow));
			Helpers.GameObjectSetActive(currencyParent, value: true);
			Helpers.GameObjectSetActive(newCurrencyParent, value: true);
		}
		else
		{
			Helpers.GameObjectSetActive(currencyParent, value: false);
			Helpers.GameObjectSetActive(newCurrencyParent, value: false);
		}
		if (postPurchaseState == AfterPurchaseState.CanBuyMore)
		{
			TweenManager.PlayTweenGroup(base.gameObject, ItemBoughtTweenGroup, forward: true, OnCompleteBoughtTween);
		}
		else if (postPurchaseState == AfterPurchaseState.RemovedFromStore)
		{
			UIEvent.Send("NewShopRemovedFromStoreEvent", storeDefinition);
			TweenManager.PlayTweenGroup(base.gameObject, ItemBoughtAndRemovedTweenGroup, forward: true, OnCompleteBoughtTween);
		}
		postPurchaseState = AfterPurchaseState.None;
	}

	private void ShowOriginalSalePrice()
	{
		try
		{
			int strikePricePercentage = contentDefinition.StrikePricePercentage;
			IAPManager iAPManager = GameManager.Instance.IAPManager;
			bool flag = strikePricePercentage != 0 && iapPurchaseProduct != null && iapPurchaseProduct.PriceUSD > 0f && iAPManager.IsInitialized();
			Helpers.GameObjectSetActive(strikePriceContainer, flag);
			if (!flag)
			{
				return;
			}
			string formattedPrice = GameManager.Instance.IAPManager.GetFormattedPrice(contentDefinition.IAPProduct);
			if (!formattedPrice.Contains("$") && !formattedPrice.Contains(Convert.ToChar(8364).ToString()) && !formattedPrice.Contains(Convert.ToChar(163).ToString()))
			{
				Helpers.GameObjectSetActive(strikePriceContainer, value: false);
				return;
			}
			Match match = Regex.Match(formattedPrice, "([-+]?[0-9]*[.,]?[0-9]+)");
			if (match.Success)
			{
				string value = match.Groups[1].Value;
				float num = float.Parse(value.Replace(",", ".")) * (100f + (float)strikePricePercentage) / 100f;
				if (num % 1f <= 0.5f)
				{
					num = Mathf.Floor(num) + 0.49f;
				}
				else if (num % 1f <= 0.99f)
				{
					num = Mathf.Floor(num) + 0.99f;
				}
				string text = formattedPrice.Replace(value, num.ToString() ?? "");
				if (formattedPrice.Contains(".") && text.Contains(","))
				{
					text = text.Replace(",", ".");
				}
				else if (formattedPrice.Contains(",") && text.Contains("."))
				{
					text = text.Replace(".", ",");
				}
				strikePriceLabel.text = text;
			}
			else
			{
				Helpers.GameObjectSetActive(strikePriceContainer, value: false);
			}
		}
		catch (Exception)
		{
			Helpers.GameObjectSetActive(strikePriceContainer, value: false);
		}
	}

	public virtual void OnCompleteBoughtTween()
	{
		UIEvent.Send("OnRequestShopUpdate", this);
	}

	public void OnButtonClicked(UIButtonExtended button)
	{
		ShopPopup shopPopup = SingularityMonoBehaviour<HUDManager>.Instance.GetNoCreation(UIType.ShopPopup) as ShopPopup;
		if (shopPopup != null)
		{
			shopPopup.SetLastButtonClicked(this);
		}
		if (storeDefinition != null && contentDefinition != null)
		{
			CheckSpeedToken();
			if (speedTokenPass)
			{
				GoPurchase();
			}
		}
	}

	public void OnButtonClicked2(UIButtonExtended button)
	{
		ShopPopup shopPopup = SingularityMonoBehaviour<HUDManager>.Instance.GetNoCreation(UIType.ShopPopup) as ShopPopup;
		if (shopPopup != null)
		{
			shopPopup.SetLastButtonClicked(this);
		}
		if (storeDefinition != null && contentDefinition != null)
		{
			int tradeFairPriceNew = contentDefinition.TradeFairPriceNew;
			CurrencyType currencyType = CurrencyType.Fairmoney;
			int currencyAmount = GameManager.Instance.playerModel.GetCurrencyAmount(currencyType);
			if (tradeFairPriceNew <= 0)
			{
				ExecuteBuyCommand();
			}
			else if (currencyAmount >= tradeFairPriceNew)
			{
				BuyResourcesPopup obj = (BuyResourcesPopup)SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.BuyResourcesPopup);
				obj.SetConfirmContent(LocalizationManager.GetText("Popup.BuyResources.TradeCrate"), GetItemNameContent(), tradeFairPriceNew, currencyType);
				obj.SetCallbacks(ExecuteBuyCommand);
				obj.Open();
			}
			else
			{
				BuyResourcesPopup obj2 = (BuyResourcesPopup)SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.BuyResourcesPopup);
				obj2.SetYesContent(LocalizationManager.GetText("Banana.Guidance"), string.Empty, tradeFairPriceNew, currencyType, new RewardCurrency
				{
					Amount = tradeFairPriceNew - currencyAmount,
					CurrencyType = CurrencyType.Fairmoney
				});
				obj2.SetCallbacks(GoBanana);
				obj2.Open();
			}
		}
	}

	private string GetItemNameContent()
	{
		string result = "";
		if (storeDefinition != null && contentDefinition != null)
		{
			result = ((!string.IsNullOrEmpty(storeDefinition.OverrideTitleLocalizationDetail)) ? LocalizationManager.GetText("IAPCard.ItemName." + storeDefinition.OverrideTitleLocalizationDetail) : ((!string.IsNullOrEmpty(storeDefinition.OverrideTitleLocalization)) ? LocalizationManager.GetText("IAPCard.ItemName." + storeDefinition.OverrideTitleLocalization) : ((rewardTimedBonus == null) ? LocalizationManager.GetText("IAPCard.ItemName." + storeDefinition.BundleIdentifier) : HelpersLocalization.GetTimedBonusTitle(rewardTimedBonus.TimedBonusType, rewardTimedBonus.Duration))));
		}
		return result;
	}

	private void ExecuteBuyCommand()
	{
		BuyBundleCommand command = new BuyBundleCommand(storeDefinition.BundleIdentifier);
		OnBuyCommandCompleted(Helpers.ExecuteCommand(command));
	}

	private void OnBuyCommandCompleted(TWDModelResult result)
	{
		if (result != TWDModelResult.OK)
		{
			return;
		}
		IAPConfirmPopupNew iAPConfirmPopupNew = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.IAPConfirmPopupNew) as IAPConfirmPopupNew;
		if (iAPConfirmPopupNew != null)
		{
			iAPConfirmPopupNew.ShowShopWhenClosed = true;
			iAPConfirmPopupNew.OpenForRewards(contentDefinition.RewardEntries.RewardsList);
			iAPConfirmPopupNew.SetContent(LocalizationManager.GetText("Popup.IAPConfirm.Title.GenericReward"), null);
			UIEvent.Send("OnBundleBought", storeDefinition);
			if (SingularityMonoBehaviour<AudioManager>.Instance != null)
			{
				SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/purchase");
			}
		}
	}

	private void GoBanana()
	{
		if (GameManager.Instance.gameEconomyData?.ConfigData != null)
		{
			ShopPopupHelper.OpenWithIndex(2);
		}
	}

	private void OnGetTransferCode(string message)
	{
		if (CheckError(message))
		{
			return;
		}
		SingularityMonoBehaviour<HUDManager>.Instance.CloseIfExists(UIType.IngameLoading);
		TransferCode transferCode = GameManager.Instance.modelManager.GetMessageSerializer().DeserializeObject<TransferCode>(message);
		if (transferCode != null && !string.IsNullOrEmpty(transferCode.Code))
		{
			PlayerModel playerModel = GameManager.Instance.playerModel;
			string bananaURL = Helpers.GetBananaURL();
			if (playerModel != null && playerModel.HashedId != null)
			{
				string text = Convert.ToBase64String(Encoding.UTF8.GetBytes("ydldeca" + playerModel.HashedId + "twd"));
				string deviceId = GameManager.Instance.LoginRequest.Device.DeviceId;
				bananaURL = bananaURL + "?id=" + text + "&code=" + transferCode.Code + "&DeviceId=" + deviceId + "&OS=" + Helpers.GetPlatformName(Application.platform);
				Application.OpenURL(bananaURL);
			}
		}
		else
		{
			CheckError("");
		}
	}

	private bool CheckError(string message)
	{
		SingularityMonoBehaviour<HUDManager>.Instance.CloseIfExists(UIType.IngameLoading);
		if (string.IsNullOrEmpty(message) || message == "null")
		{
			AlertPopup.ShowPopupGetText("Error.Error", "Error.ErrorGeneric", "Button.Ok", null);
			return true;
		}
		return false;
	}

	private void GoPurchase()
	{
		if (SingularityMonoBehaviour<AudioManager>.Instance != null)
		{
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/purchase_start");
		}
		UIEvent.Send("SendEndShopVisitAnalytics", this);
		GameManager.Instance.IAPManager.Buy(storeDefinition, contentDefinition);
	}

	public void OverrideSalesBadge(bool show, string localizationKey)
	{
		HelpersUI.SetContentToLabel(salesBadge, LocalizationManager.GetText(localizationKey), show);
	}

	public override int GetSortValue()
	{
		if (storeDefinition != null)
		{
			return 1000 - storeDefinition.DisplayOrder;
		}
		return base.GetSortValue();
	}

	private void OnUiEvent(string type, object parameter)
	{
		if (type == "OnBundleBought" && parameter != null && GetData() != null && parameter is BundleStoreDefinition && (parameter as BundleStoreDefinition).BundleIdentifier == GetData().BundleIdentifier)
		{
			if (GameManager.Instance.playerModel.BundleManager.CanBuyBundle(parameter as BundleStoreDefinition))
			{
				postPurchaseState = AfterPurchaseState.CanBuyMore;
			}
			else
			{
				postPurchaseState = AfterPurchaseState.RemovedFromStore;
			}
			DisableMainShopButton();
		}
		else if (type == "OnPopUpClose" && (parameter is IAPConfirmPopupNew || parameter is OpenLootInUi))
		{
			UpdateUI();
		}
	}

	protected override void OnClickedTooltipButton(UIButtonExtended button)
	{
		base.OnClickedTooltipButton(button);
		if (contentDefinition != null && contentDefinition.RewardEntries != null && contentDefinition.RewardEntries.RewardsList.Count > 0)
		{
			TooltipManager.OpenTextBoxWithText(base.gameObject, HelpersLocalization.GetShopTooltipForIReward(contentDefinition.RewardEntries.RewardsList[0]));
		}
	}

	private void CheckSpeedToken()
	{
		speedTokenPass = true;
		OverSpeedConvertedAmount = 0;
		OverSpeedRewardsList.Clear();
		if (contentDefinition == null)
		{
			return;
		}
		Rewards rewards = null;
		try
		{
			rewards = new Rewards(contentDefinition.Rewards, null, 0, EquipmentSource.Bundle);
		}
		catch (Exception)
		{
			rewards = new Rewards();
		}
		List<IReward> rewardsList = rewards.RewardsList;
		if (rewardsList.Count <= 0)
		{
			return;
		}
		for (int i = 0; i < rewardsList.Count; i++)
		{
			if (!(rewardsList[i] is RewardCurrency))
			{
				continue;
			}
			RewardCurrency rewardCurrency = rewardsList[i] as RewardCurrency;
			if (GameManager.Instance.gameEconomyData.IsSpeedUpTokenCurrencyType(rewardCurrency.CurrencyType))
			{
				PlayerModel playerModel = GameManager.Instance.playerModel;
				int currencyAmount = playerModel.GetCurrencyAmount(rewardCurrency.CurrencyType);
				int max = playerModel.GetCurrency(rewardCurrency.CurrencyType).Max;
				if (currencyAmount > max)
				{
					OverSpeedRewardsList.Add(rewardCurrency);
					OverSpeedConvertedAmount += GameManager.Instance.modelManager.GameEconomyData.CurrencyToDiamonds(rewardCurrency.CurrencyType, rewardCurrency.Amount, GameManager.Instance.modelManager.Player);
				}
				else if (currencyAmount + rewardCurrency.Amount > max)
				{
					OverSpeedRewardsList.Add(rewardCurrency);
					OverSpeedConvertedAmount += GameManager.Instance.modelManager.GameEconomyData.CurrencyToDiamonds(rewardCurrency.CurrencyType, currencyAmount + rewardCurrency.Amount - max, GameManager.Instance.modelManager.Player);
				}
			}
		}
		if (OverSpeedConvertedAmount > 0)
		{
			speedTokenPass = false;
			TokenConversionPopup obj = (TokenConversionPopup)SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.TokenConversionPopup);
			obj.OpenForCurrency(OverSpeedConvertedAmount);
			obj.SetConversionCallbacks(GoPurchase);
		}
	}
}
