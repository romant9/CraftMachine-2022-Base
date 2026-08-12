using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using BaseModel;
using Client.Connectivity;
using Newtonsoft.Json;
using TWDModel;
using UnityEngine;

public class ShopItemCardTradeFair : ShopCardBase<TradefairBundleStoreDefinition>
{
	private enum AfterPurchaseState
	{
		None = 0,
		CanBuyMore = 1,
		RemovedFromStore = 2
	}

	public class HttpData
	{
		public string id;

		public string code;

		public string BundleId;

		public int PeriodId;

		public string PurchaseSource;

		public string GameURL;

		public bool IsNewVersion;

		public string DeviceId;

		public string OS;
	}

	[SerializeField]
	private UIButtonWithLabel button;

	[SerializeField]
	private GameObject buttonTDUSA;

	[SerializeField]
	private GameObject buttonTDBA;

	[SerializeField]
	private UIButtonWithLabel buttonTD;

	[SerializeField]
	private UIButtonWithLabel buttonBA1;

	[SerializeField]
	private UILabel buttonBA1Label;

	[SerializeField]
	private UILabel buttonBA1Label2;

	[SerializeField]
	private UISprite buttonBA1Icon;

	[SerializeField]
	private UIButtonWithLabel buttonBA2;

	[SerializeField]
	private UISprite buttonBA2Icon;

	[SerializeField]
	private UILabel buttonBA2Label;

	[SerializeField]
	private UILabel buttonBA2Label2;

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
	private UISprite priceIconSprite;

	[SerializeField]
	[Tooltip("The tint color for labels when the user has enough currency.")]
	private ColorAsset availableCurrencyColor;

	[SerializeField]
	[Tooltip("The tint color for labels when the user has NOT enough currency.")]
	private ColorAsset unavailableCurrencyColor;

	[SerializeField]
	private UIAtlas monochromeAtlas;

	[SerializeField]
	private UIAtlas shopAtlas;

	[SerializeField]
	private UIAtlas uiCampAtlas;

	private static readonly byte[] key = new byte[32]
	{
		42, 107, 197, 142, 50, 233, 250, 125, 54, 64,
		195, 162, 95, 130, 71, 24, 76, 188, 157, 126,
		58, 249, 96, 1, 85, 147, 40, 182, 79, 138,
		60, 206
	};

	private static readonly byte[] iv = new byte[16]
	{
		117, 26, 109, 11, 197, 47, 138, 109, 35, 132,
		151, 19, 236, 117, 41, 63
	};

	[SerializeField]
	private DiscountButtonController buttonWebShopDiscount;

	private AfterPurchaseState postPurchaseState;

	protected TradefairBundleContentDefinition contentDefinition;

	private RewardTimedBonus rewardTimedBonus;

	private int iapPurchaseProduct;

	private LimitedBundleData limitedBundleData;

	private bool speedTokenPass = true;

	private List<IReward> OverSpeedRewardsList = new List<IReward>();

	private int OverSpeedConvertedAmount;

	public TradefairBundleStoreDefinition storeDefinition => GetData();

	public override void AddListeners()
	{
		base.AddListeners();
		if (button != null)
		{
			button.SetClickCallback(OnButtonClicked);
		}
		if (buttonTD != null)
		{
			buttonTD.SetClickCallback(OnButtonClicked);
		}
		if (buttonTDUSA != null)
		{
			buttonTDUSA.SetActive(value: false);
		}
		if (buttonBA1 != null)
		{
			buttonBA1.SetClickCallback(OnButtonClicked3);
		}
		if (buttonBA2 != null)
		{
			buttonBA2.SetClickCallback(OnButtonClicked3);
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
		if (buttonTD != null)
		{
			buttonTD.RemoveClickCallback(OnButtonClicked);
		}
		if (buttonBA1 != null)
		{
			buttonBA1.RemoveClickCallback(OnButtonClicked3);
		}
		if (buttonBA2 != null)
		{
			buttonBA2.RemoveClickCallback(OnButtonClicked3);
		}
		UIEvent.OnUIEvent -= OnUiEvent;
	}

	public virtual void DisableMainShopButton()
	{
		if (button != null)
		{
			button.isEnabled = false;
		}
		if (buttonTD != null)
		{
			buttonTD.isEnabled = false;
		}
		if (buttonBA1 != null)
		{
			buttonBA1.isEnabled = false;
		}
		if (buttonBA2 != null)
		{
			buttonBA2.isEnabled = false;
		}
	}

	public virtual void EnableMainShopButton()
	{
		if (button != null)
		{
			button.isEnabled = true;
		}
		if (buttonTD != null)
		{
			buttonTD.isEnabled = true;
		}
		if (buttonBA1 != null)
		{
			buttonBA1.isEnabled = true;
		}
		if (buttonBA2 != null)
		{
			buttonBA2.isEnabled = true;
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
		if (buttonTD != null)
		{
			buttonTD.Clear();
		}
		if (buttonBA1 != null)
		{
			buttonBA1.Clear();
		}
		if (buttonBA2 != null)
		{
			buttonBA2.Clear();
		}
		contentDefinition = null;
		rewardTimedBonus = null;
		iapPurchaseProduct = 0;
		limitedBundleData = null;
		postPurchaseState = AfterPurchaseState.None;
	}

	public override void SetData(TradefairBundleStoreDefinition bundleStoreDefinition)
	{
		base.SetData(bundleStoreDefinition);
		if (storeDefinition != null)
		{
			limitedBundleData = GameManager.Instance.playerModel.TradefairManager.GetInitiatedLimitedBundle(storeDefinition.BundleIdentifier);
			Helpers.GameObjectSetActive(limitedTimeLabel, value: false);
			contentDefinition = GameManager.Instance.playerModel.gameEconomyData.GetTradefairBundleContentDefinition(storeDefinition.BundleIdentifier);
			if (contentDefinition != null)
			{
				iapPurchaseProduct = contentDefinition.IAPProduct;
				if (contentDefinition.RewardEntries != null && contentDefinition.RewardEntries.RewardsList.Count == 1)
				{
					rewardTimedBonus = contentDefinition.RewardEntries.RewardsList[0] as RewardTimedBonus;
				}
				EnableMainShopButton();
			}
			else
			{
				Debug.LogError("ShopItemCard: Could not find TradefairBundleContentDefinition with BundleIdentifier: " + storeDefinition.BundleIdentifier);
			}
		}
		else
		{
			Debug.LogError("ShopItemCard: TradefairBundleStoreDefinition NULL!");
		}
		UpdateUI();
	}

	public void UpdateWebShopDiscountUI()
	{
		if (buttonWebShopDiscount != null)
		{
			buttonWebShopDiscount.ClearData();
		}
	}

	public virtual void Update()
	{
		if (limitedTimeLabel != null && storeDefinition != null && storeDefinition.ShowTimerInCard && limitedBundleData != null)
		{
			HelpersUI.SetContentToLabel(limitedTimeLabel, Helpers.FormatTimeNoZero(limitedBundleData.Timer));
		}
		else
		{
			Helpers.GameObjectSetActive(limitedTimeLabel, value: false);
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

	public override void UpdateUI()
	{
		base.UpdateUI();
		UpdateWebShopDiscountUI();
		if (storeDefinition == null || contentDefinition == null)
		{
			return;
		}
		HelpersUI.SetContentToLabel(itemNameLabel, GetItemNameContent());
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
		if (button != null || contentDefinition.HideCoinPurchase)
		{
			if (storeDefinition.BundleIdentifier != "TWD_BOX_OF_FAIRCOINS")
			{
				string text = "";
				text = (((float)iapPurchaseProduct > 0f) ? (iapPurchaseProduct.ToString() ?? "") : LocalizationManager.GetText("Generic.Free"));
				int currencyAmount = GameManager.Instance.playerModel.GetCurrencyAmount(CurrencyType.Fairmoney);
				bool flag = iapPurchaseProduct > 0 && currencyAmount < iapPurchaseProduct;
				button.SetContentToLabelOne(text, flag ? unavailableCurrencyColor.Color : availableCurrencyColor.Color);
				if (priceIconSprite != null)
				{
					priceIconSprite.spriteName = HelpersGfx.GetCurrencyIconName(CurrencyType.Fairmoney);
					Helpers.GameObjectSetActive(priceIconSprite, iapPurchaseProduct > 0);
				}
			}
			else
			{
				BundleContentDefinition bundleContentDefinition = GameManager.Instance.playerModel.gameEconomyData.GetBundleContentDefinition(storeDefinition.BundleIdentifier);
				InAppPurchaseProductApple inAppPurchaseProduct = GameManager.Instance.gameEconomyData.GetInAppPurchaseProduct(bundleContentDefinition.IAPProduct);
				string content = LocalizationManager.GetText("Generic.Free");
				if (inAppPurchaseProduct != null)
				{
					content = ((inAppPurchaseProduct.PriceUSD > 0f) ? GameManager.Instance.IAPManager.GetFormattedPrice(bundleContentDefinition.IAPProduct) : LocalizationManager.GetText("Generic.Free"));
				}
				button.SetContentToLabelOne(content);
				Helpers.GameObjectSetActive(priceIconSprite, value: false);
			}
			button.gameObject.SetActive(value: true);
			if (buttonTDBA != null)
			{
				buttonTDBA.SetActive(value: false);
			}
		}
		if (buttonTDBA != null && storeDefinition.BundleIdentifier != "TWD_BOX_OF_FAIRCOINS" && contentDefinition.PayBanana && GameManager.Instance.gameEconomyData.ConfigData.PayBananaSwitch && Helpers.GetShopRoleType() == ShopRoleType.DefaultType)
		{
			if (contentDefinition.HideCoinPurchase)
			{
				string text2 = "";
				text2 = "$" + contentDefinition.ShowPrice;
				buttonBA2.SetContentToLabelOne(text2);
				string spriteName = "";
				if (contentDefinition.ExtraRewardEntries != null && contentDefinition.ExtraRewardEntries.RewardsList[0] != null)
				{
					UIAtlas iconNameForIReward = HelpersGfx.GetIconNameForIReward(contentDefinition.ExtraRewardEntries.RewardsList[0], out spriteName, monochromeAtlas, shopAtlas, uiCampAtlas);
					HelpersUI.SetSpriteAndAtlas(buttonBA2Icon, spriteName, iconNameForIReward);
					buttonBA2Icon.gameObject.SetActive(value: true);
					buttonBA2Label.gameObject.SetActive(value: true);
					buttonBA2Label2.gameObject.SetActive(value: true);
					buttonBA2Label.text = "+" + Helpers.GetNumsForIReward(contentDefinition.ExtraRewardEntries.RewardsList[0]);
				}
				else
				{
					buttonBA2Icon.gameObject.SetActive(value: false);
					buttonBA2Label.gameObject.SetActive(value: false);
					buttonBA2Label2.gameObject.SetActive(value: false);
				}
				buttonTD.gameObject.SetActive(value: false);
				buttonBA1.gameObject.SetActive(value: false);
				buttonBA2.gameObject.SetActive(value: true);
			}
			else
			{
				string text3 = "";
				text3 = "$" + contentDefinition.ShowPrice;
				buttonBA1.SetContentToLabelOne(text3);
				string spriteName2 = "";
				if (contentDefinition.ExtraRewardEntries != null && contentDefinition.ExtraRewardEntries.RewardsList[0] != null)
				{
					UIAtlas iconNameForIReward2 = HelpersGfx.GetIconNameForIReward(contentDefinition.ExtraRewardEntries.RewardsList[0], out spriteName2, monochromeAtlas, shopAtlas, uiCampAtlas);
					HelpersUI.SetSpriteAndAtlas(buttonBA1Icon, spriteName2, iconNameForIReward2);
					buttonBA1Icon.gameObject.SetActive(value: true);
					buttonBA1Label.gameObject.SetActive(value: true);
					buttonBA1Label2.gameObject.SetActive(value: true);
					buttonBA1Label.text = "+" + Helpers.GetNumsForIReward(contentDefinition.ExtraRewardEntries.RewardsList[0]);
				}
				else
				{
					buttonBA1Icon.gameObject.SetActive(value: false);
					buttonBA1Label.gameObject.SetActive(value: false);
					buttonBA1Label2.gameObject.SetActive(value: false);
				}
				string text4 = "";
				text4 = (((float)iapPurchaseProduct > 0f) ? (iapPurchaseProduct.ToString() ?? "") : LocalizationManager.GetText("Generic.Free"));
				int currencyAmount2 = GameManager.Instance.playerModel.GetCurrencyAmount(CurrencyType.Fairmoney);
				bool flag2 = iapPurchaseProduct > 0 && currencyAmount2 < iapPurchaseProduct;
				buttonTD.SetContentToLabelOne(text4, flag2 ? unavailableCurrencyColor.Color : availableCurrencyColor.Color);
				buttonTD.gameObject.SetActive(value: true);
				buttonBA1.gameObject.SetActive(value: true);
				buttonBA2.gameObject.SetActive(value: false);
			}
			buttonTDBA.SetActive(value: true);
			button.gameObject.SetActive(value: false);
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
		ShowOriginalSalePrice();
		bool num = storeDefinition.CardCurrencyToShow != CurrencyType.None;
		int num2 = 0;
		if (num && contentDefinition.RewardEntries != null)
		{
			num2 = contentDefinition.RewardEntries.GetTotalCurrencyRewardAmount(storeDefinition.CardCurrencyToShow);
		}
		if (num2 > 0)
		{
			HelpersUI.SetContentToLabel(currencyAmountLabel, num2.ToString());
			HelpersUI.SetSprite(currencyIconSprite, HelpersGfx.GetCurrencyIconName(storeDefinition.CardCurrencyToShow));
			Helpers.GameObjectSetActive(currencyParent, value: true);
		}
		else
		{
			Helpers.GameObjectSetActive(currencyParent, value: false);
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
			bool flag = strikePricePercentage != 0 && (float)iapPurchaseProduct > 0f && iAPManager.IsInitialized();
			Helpers.GameObjectSetActive(strikePriceContainer, flag);
			if (!flag)
			{
				return;
			}
			string text = contentDefinition.IAPProduct.ToString() ?? "";
			if (!text.Contains("$") && !text.Contains(Convert.ToChar(8364).ToString()) && !text.Contains(Convert.ToChar(163).ToString()))
			{
				Helpers.GameObjectSetActive(strikePriceContainer, value: false);
				return;
			}
			Match match = Regex.Match(text, "([-+]?[0-9]*[.,]?[0-9]+)");
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
				string text2 = text.Replace(value, num.ToString() ?? "");
				if (text.Contains(".") && text2.Contains(","))
				{
					text2 = text2.Replace(",", ".");
				}
				else if (text.Contains(",") && text2.Contains("."))
				{
					text2 = text2.Replace(".", ",");
				}
				strikePriceLabel.text = text2;
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
		if (storeDefinition.BundleIdentifier != "TWD_BOX_OF_FAIRCOINS")
		{
			ShopPopup shopPopup = SingularityMonoBehaviour<HUDManager>.Instance.GetNoCreation(UIType.ShopPopup) as ShopPopup;
			if (shopPopup != null)
			{
				shopPopup.SetLastButtonClicked(this);
			}
			if (storeDefinition != null && contentDefinition != null)
			{
				int iAPProduct = contentDefinition.IAPProduct;
				CurrencyType currencyType = CurrencyType.Fairmoney;
				int currencyAmount = GameManager.Instance.playerModel.GetCurrencyAmount(currencyType);
				if (iAPProduct <= 0)
				{
					ExecuteBuyCommand();
				}
				else if (currencyAmount >= iAPProduct)
				{
					BuyResourcesPopup obj = (BuyResourcesPopup)SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.BuyResourcesPopup);
					obj.SetConfirmContent(LocalizationManager.GetText("Popup.BuyResources.TradeCrate"), GetItemNameContent(), iAPProduct, currencyType);
					obj.SetCallbacks(ExecuteBuyCommand);
					obj.Open();
				}
				else
				{
					BuyResourcesPopup obj2 = (BuyResourcesPopup)SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.BuyResourcesPopup);
					obj2.SetYesContent(LocalizationManager.GetText("Banana.Guidance"), string.Empty, iAPProduct, currencyType, new RewardCurrency
					{
						Amount = iAPProduct - currencyAmount,
						CurrencyType = CurrencyType.Fairmoney
					});
					obj2.SetCallbacks(GoBanana);
					obj2.Open();
				}
			}
		}
		else
		{
			ShopPopup shopPopup2 = SingularityMonoBehaviour<HUDManager>.Instance.GetNoCreation(UIType.ShopPopup) as ShopPopup;
			if (shopPopup2 != null)
			{
				shopPopup2.SetLastButtonClicked(this);
			}
			if (storeDefinition != null && contentDefinition != null)
			{
				GoPurchase();
			}
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

	private void GoPurchase()
	{
		if (SingularityMonoBehaviour<AudioManager>.Instance != null)
		{
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/purchase_start");
		}
		UIEvent.Send("SendEndShopVisitAnalytics", this);
		BundleContentDefinition bundleContentDefinition = GameManager.Instance.playerModel.gameEconomyData.GetBundleContentDefinition(storeDefinition.BundleIdentifier);
		BundleStoreDefinition bundleStoreDefinition = GameManager.Instance.playerModel.gameEconomyData.GetBundleStoreDefinition(storeDefinition.BundleIdentifier);
		GameManager.Instance.IAPManager.Buy(bundleStoreDefinition, bundleContentDefinition);
	}

	public static string EncryptFields(HttpData data, byte[] key, byte[] iv)
	{
		string text = JsonConvert.SerializeObject(data);
		using Aes aes = Aes.Create();
		aes.Key = key;
		aes.IV = iv;
		return Convert.ToBase64String(aes.CreateEncryptor().TransformFinalBlock(Encoding.UTF8.GetBytes(text), 0, text.Length));
	}

	public void OnButtonClicked3(UIButtonExtended button)
	{
		GoBananaAndBuy();
	}

	private void GoBananaAndBuy()
	{
		if (Helpers.ExecuteCommand(new BuyTradefairBundleXsollaCommand(storeDefinition.BundleIdentifier)) == TWDModelResult.OK && GameManager.Instance.IsConnectedToServer)
		{
			SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.IngameLoading).Open();
			SignalRClient.Instance.RequestCommand("GetBananaLoginCode", OnGetTransferCode2, waitForResponse: true);
		}
	}

	private void OnGetTransferCode2(string message)
	{
		if (!CheckError(message))
		{
			SingularityMonoBehaviour<HUDManager>.Instance.CloseIfExists(UIType.IngameLoading);
			TransferCode transferCode = GameManager.Instance.modelManager.GetMessageSerializer().DeserializeObject<TransferCode>(message);
			if (transferCode != null && !string.IsNullOrEmpty(transferCode.Code))
			{
				GoWebShopUrl(transferCode);
			}
			else
			{
				CheckError("");
			}
		}
	}

	private void GoWebShopUrl(TransferCode transferCode)
	{
		if (transferCode == null || string.IsNullOrEmpty(transferCode.Code))
		{
			return;
		}
		PlayerModel playerModel = GameManager.Instance.playerModel;
		string text = "https://webshop-dev.drillerservices.com/EmptyPage?EncryptFields=";
		if (playerModel.HashedId != null)
		{
			if (!string.IsNullOrEmpty(Helpers.GetBananaURL()))
			{
				text = Helpers.GetBananaURL();
				text += "/EmptyPage?EncryptFields=";
			}
			string id = Convert.ToBase64String(Encoding.UTF8.GetBytes("ydldeca" + playerModel.HashedId + "twd"));
			string text2 = EncryptFields(new HttpData
			{
				id = id,
				code = transferCode.Code,
				BundleId = contentDefinition.Identifier,
				PeriodId = 1,
				PurchaseSource = "tradefair",
				GameURL = BuildConfigurationManager.Instance.ActiveConfiguration.UrlScheme + "://",
				IsNewVersion = true,
				DeviceId = GameManager.Instance.LoginRequest.Device.DeviceId,
				OS = Helpers.GetPlatformName(Application.platform)
			}, key, iv);
			text2 = text2.Replace("+", "%2B");
			text += text2;
			Application.OpenURL(text);
		}
	}

	private void ExecuteBuyCommand()
	{
		BuyTradefairBundleCommand command = new BuyTradefairBundleCommand(storeDefinition.BundleIdentifier);
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
		if (type == "OnBundleBought" && parameter != null && GetData() != null && parameter is TradefairBundleStoreDefinition && (parameter as TradefairBundleStoreDefinition).BundleIdentifier == GetData().BundleIdentifier)
		{
			if (GameManager.Instance.playerModel.TradefairManager.CanBuyBundle(parameter as TradefairBundleStoreDefinition))
			{
				postPurchaseState = AfterPurchaseState.CanBuyMore;
			}
			else
			{
				postPurchaseState = AfterPurchaseState.RemovedFromStore;
			}
			DisableMainShopButton();
		}
		else if (type == "OnPopUpClose" && (parameter is IAPConfirmPopupNew || parameter is OpenLootInUi || parameter is IAPConfirmBananaPopupNew))
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

	private void GoBanana()
	{
		if (GameManager.Instance.gameEconomyData?.ConfigData == null)
		{
			return;
		}
		if (Helpers.GetClickInternal())
		{
			if (GameManager.Instance.IsConnectedToServer)
			{
				SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.IngameLoading).Open();
				SignalRClient.Instance.RequestCommand("GetBananaLoginCode", OnGetTransferCode, waitForResponse: true);
			}
		}
		else
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
}
