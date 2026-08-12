using System;
using System.Collections.Generic;
using System.Text;
using BaseModel;
using Client.Connectivity;
using TWDModel;
using UnityEngine;

public class MergeBundlePopup : HUDElement
{
	[SerializeField]
	private UIGrid storageContainer;

	[SerializeField]
	private OptionalItemCard baseReward;

	[SerializeField]
	private UILabel priceTxt;

	[SerializeField]
	private UILabel tradeFairPriceTxt;

	[SerializeField]
	private UILabel priceTxt1;

	[SerializeField]
	private UILabel priceTxt2;

	[SerializeField]
	private UILabel limitLabel;

	[SerializeField]
	private StorageRewardListPanel storageRewardListPanel;

	[SerializeField]
	private Transform firstReward;

	[SerializeField]
	private Transform addUi;

	[SerializeField]
	private UILabel itemName;

	[SerializeField]
	private UIButton payButton;

	[SerializeField]
	private UIButton tradeButton;

	[SerializeField]
	private UIButton payButton1;

	[SerializeField]
	private UIButton payButton2;

	[SerializeField]
	private UISprite paySprite1;

	[SerializeField]
	private UISprite paySprite2;

	[SerializeField]
	[Tooltip("The tint color for labels when the user has enough currency.")]
	private ColorAsset availableCurrencyColor;

	[SerializeField]
	[Tooltip("The tint color for labels when the user has NOT enough currency.")]
	private ColorAsset unavailableCurrencyColor;

	private List<MergeCustomAddUI> _customAddUis = new List<MergeCustomAddUI>();

	private List<MergeBundleData> _mergeBundleDatas = new List<MergeBundleData>();

	private CustomBundleDefinition customBundleDefinition;

	private CustomizedBundlePayType _button1Type;

	private CustomizedBundlePayType _button2Type;

	private int _buttonNum;

	public void Bind(CustomBundleDefinition definition, int index)
	{
		customBundleDefinition = definition;
		GameManager.Instance.playerModel.CustomizedBundleManager.currentSelectIndex = index;
	}

	public override void Open()
	{
		base.Open();
		UpdateUI();
		ShowStorageReward();
	}

	public override void Close()
	{
		base.Close();
		payButton1.onClick.Clear();
		payButton2.onClick.Clear();
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
		CustomizedBundleManager customizedBundleManager = GameManager.Instance.playerModel.CustomizedBundleManager;
		if (customizedBundleManager == null || customBundleDefinition == null)
		{
			return;
		}
		string text = LocalizationManager.GetText("IAPCard.ItemName." + customBundleDefinition.OverrideTitleLocalization);
		HelpersUI.SetContentToLabel(itemName, text);
		if (payButton1 != null && payButton2 != null)
		{
			bool flag = true;
			LimitedCustomBundleData initiatedLimitedBundle = customizedBundleManager.GetInitiatedLimitedBundle(customBundleDefinition.Identifier);
			if (initiatedLimitedBundle != null)
			{
				flag = initiatedLimitedBundle.IsCanBy;
			}
			bool flag2 = customizedBundleManager.IsCanPay(customBundleDefinition);
			HelpersUI.SetButtonState(payButton1, (!(flag2 && flag)) ? UIButtonColor.State.Disabled : UIButtonColor.State.Normal);
			HelpersUI.SetButtonState(payButton2, (!(flag2 && flag)) ? UIButtonColor.State.Disabled : UIButtonColor.State.Normal);
		}
		GetBundleTypeByConfig();
		if (priceTxt1 != null)
		{
			SetButtonData(_button1Type, priceTxt1, paySprite1, 170, 90, new Vector3(23f, 0f, 0f));
		}
		if (priceTxt2 != null)
		{
			if (_buttonNum == 2)
			{
				Helpers.GameObjectSetActive(payButton2, value: true);
				SetButtonData(_button2Type, priceTxt2, paySprite2, 170, 90, new Vector3(23f, 0f, 0f));
			}
			else
			{
				Helpers.GameObjectSetActive(payButton2, value: false);
			}
		}
		Helpers.GameObjectSetActive(limitLabel, value: false);
		if (customBundleDefinition.ShowMaxPurchases)
		{
			Helpers.GameObjectSetActive(limitLabel, value: true);
			customizedBundleManager.CustomBundleBoughtBundlesAmount.TryGetValue(customBundleDefinition.Identifier, out var value);
			limitLabel.text = LocalizationManager.GetText("ShopUI.DetailPage.PurchaseLimit", customBundleDefinition.MaxPurchases - value, customBundleDefinition.MaxPurchases);
		}
		if (baseReward != null && customBundleDefinition.RewardEntries != null)
		{
			IReward rewardAt = customBundleDefinition.RewardEntries.GetRewardAt(0);
			if (rewardAt != null)
			{
				baseReward.Init(rewardAt);
			}
		}
		_customAddUis.Clear();
		for (int i = 0; i < storageContainer.transform.childCount; i++)
		{
			Helpers.GameObjectSetActive(storageContainer.transform.GetChild(i).gameObject, value: false);
		}
		List<int> storageID = customBundleDefinition.StorageID;
		if (storageContainer != null && storageID != null)
		{
			for (int j = 0; j < storageID.Count; j++)
			{
				Transform child = storageContainer.transform.GetChild(j);
				Helpers.GameObjectSetActive(child.gameObject, value: true);
				MergeCustomAddUI component = child.GetComponent<MergeCustomAddUI>();
				if (component != null)
				{
					_customAddUis.Add(component);
					component.Init(j);
				}
			}
			List<IReward> selectReward = customizedBundleManager.GetSelectReward(customBundleDefinition.Identifier);
			if (selectReward != null && selectReward.Count > 0)
			{
				for (int k = 0; k < selectReward.Count; k++)
				{
					if (k < _customAddUis.Count)
					{
						_customAddUis[k].ShowReward(selectReward[k]);
					}
				}
			}
		}
		storageContainer.repositionNow = true;
	}

	private void ShowStorageReward()
	{
		CustomizedBundleManager customizedBundleManager = GameManager.Instance.playerModel.CustomizedBundleManager;
		if (customizedBundleManager == null || customBundleDefinition == null)
		{
			return;
		}
		_mergeBundleDatas.Clear();
		if (!(storageRewardListPanel != null))
		{
			return;
		}
		int currentSelectIndex = customizedBundleManager.currentSelectIndex;
		if (currentSelectIndex < 0 || currentSelectIndex >= customBundleDefinition.StorageID.Count)
		{
			return;
		}
		Rewards customBundleStorageRewards = GameManager.Instance.gameEconomyData.GetCustomBundleStorageRewards(customBundleDefinition.StorageID[currentSelectIndex]);
		if (customBundleStorageRewards != null && customBundleStorageRewards.RewardsList != null && customBundleStorageRewards.RewardsList.Count > 0)
		{
			for (int i = 0; i < customBundleStorageRewards.RewardsList.Count; i++)
			{
				_mergeBundleDatas.Add(new MergeBundleData(currentSelectIndex, customBundleStorageRewards.RewardsList[i], customBundleDefinition));
			}
			storageRewardListPanel.Init(_mergeBundleDatas);
		}
	}

	public override void Update()
	{
		base.Update();
		if (firstReward != null && baseReward != null && addUi != null)
		{
			float x = (firstReward.transform.localPosition.x - baseReward.transform.localPosition.x) * 0.5f + baseReward.transform.localPosition.x;
			addUi.transform.localPosition = new Vector3(x, addUi.transform.localPosition.y, addUi.transform.localPosition.z);
		}
	}

	public void ClickPurchase()
	{
		if (customBundleDefinition != null)
		{
			CustomizedBundleManager customizedBundleManager = GameManager.Instance.playerModel.CustomizedBundleManager;
			if (customizedBundleManager != null && customizedBundleManager.IsCanPay(customBundleDefinition))
			{
				GameManager.Instance.IAPManager.BuyCustomBundle(customBundleDefinition);
			}
		}
	}

	public void ClickCustomizedBundlePurchase(CustomizedBundlePayType payType)
	{
		if (customBundleDefinition == null)
		{
			Debug.LogError("Custom Bundle : Not Find Content ID");
			return;
		}
		CustomizedBundleManager customizedBundleManager = GameManager.Instance.playerModel.CustomizedBundleManager;
		if (customizedBundleManager == null || !customizedBundleManager.IsCanPay(customBundleDefinition))
		{
			return;
		}
		int priceToPay = 0;
		CurrencyType currencyType;
		switch (payType)
		{
		case CustomizedBundlePayType.TradeFairPay:
			priceToPay = customBundleDefinition.TradefairPrice;
			currencyType = CurrencyType.Fairmoney;
			break;
		case CustomizedBundlePayType.DiamondsPay:
			priceToPay = customBundleDefinition.GoldPrice;
			currencyType = CurrencyType.Diamonds;
			break;
		case CustomizedBundlePayType.BluePrintTokenPay:
			priceToPay = customBundleDefinition.FragmentPrice;
			currencyType = CurrencyType.BulePrintToken;
			break;
		default:
			currencyType = CurrencyType.None;
			break;
		}
		int currencyAmount = GameManager.Instance.playerModel.GetCurrencyAmount(currencyType);
		if (currencyAmount >= priceToPay)
		{
			BuyResourcesPopup obj = (BuyResourcesPopup)SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.BuyResourcesPopup);
			obj.SetConfirmContent(LocalizationManager.GetText("Popup.BuyResources.TradeCrate"), LocalizationManager.GetText("Bp.Trade.Confrim.Faircoin"), priceToPay, currencyType);
			obj.SetCallbacks(delegate
			{
				ExecuteBuyCommand(payType);
			});
			obj.Open();
			return;
		}
		BuyResourcesPopup buyResourcesPopup = (BuyResourcesPopup)SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.BuyResourcesPopup);
		buyResourcesPopup.SetYesContent(LocalizationManager.GetText("Banana.Guidance"), string.Empty, priceToPay, currencyType, new RewardCurrency
		{
			Amount = priceToPay - currencyAmount,
			CurrencyType = currencyType
		});
		switch (currencyType)
		{
		case CurrencyType.Fairmoney:
			buyResourcesPopup.SetCallbacks(GoBanana);
			buyResourcesPopup.Open();
			break;
		case CurrencyType.Diamonds:
			buyResourcesPopup.SetCallbacks(delegate
			{
				GoDiamonds(priceToPay);
			});
			buyResourcesPopup.Open();
			break;
		case CurrencyType.BulePrintToken:
			buyResourcesPopup.SetCallbacks(GoBluePrint);
			buyResourcesPopup.Open();
			break;
		}
	}

	private void ExecuteBuyCommand(CustomizedBundlePayType payType)
	{
		BuyCustomizedBundleCommand buyCustomizedBundleCommand = new BuyCustomizedBundleCommand(customBundleDefinition.Identifier);
		buyCustomizedBundleCommand.PayType = payType;
		OnBuyCommandCompleted(Helpers.ExecuteCommand(buyCustomizedBundleCommand));
	}

	private void OnBuyCommandCompleted(TWDModelResult result)
	{
		if (result == TWDModelResult.OK)
		{
			IAPConfirmPopupNew.OpenCustomBundleContent(customBundleDefinition);
		}
	}

	private void GoDiamonds(int priceToPay)
	{
		MiniShopPopup.OpenWithTotalRequiredCurrencyAmount(CurrencyType.Diamonds, priceToPay);
	}

	private void GoBluePrint()
	{
		Close();
		SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.ShopPopup).Close();
		NewPhonePopup.OpenRadiophoneFeaturePopup();
		(SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.NewRadioPopup) as NewPhonePopup).OnClickWeapon();
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
			Close();
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

	private void OnEnable()
	{
		UIEvent.OnUIEvent += OnUIEventHandler;
	}

	private void OnDisable()
	{
		UIEvent.OnUIEvent -= OnUIEventHandler;
	}

	private void OnUIEventHandler(string type, object parameter)
	{
		switch (type)
		{
		case "SelectCustomStorageEvent":
			UpdateUI();
			ShowStorageReward();
			break;
		case "SelectCustomRewardEvent":
			UpdateUI();
			break;
		case "CustomRewardBundleBoughtEvent":
			OnClickClose();
			break;
		}
	}

	private void GetBundleTypeByConfig()
	{
		_buttonNum = 0;
		if (customBundleDefinition.IAPProduct != null)
		{
			_buttonNum++;
			SetButtonType(CustomizedBundlePayType.None);
		}
		if (customBundleDefinition.TradefairPrice > 0)
		{
			_buttonNum++;
			SetButtonType(CustomizedBundlePayType.TradeFairPay);
		}
		if (customBundleDefinition.GoldPrice > 0 && _buttonNum < 2)
		{
			_buttonNum++;
			SetButtonType(CustomizedBundlePayType.DiamondsPay);
		}
		if (customBundleDefinition.FragmentPrice > 0 && _buttonNum < 2)
		{
			_buttonNum++;
			SetButtonType(CustomizedBundlePayType.BluePrintTokenPay);
		}
	}

	private void SetButtonType(CustomizedBundlePayType currencyType)
	{
		if (_buttonNum == 1)
		{
			_button1Type = currencyType;
		}
		else
		{
			_button2Type = currencyType;
		}
	}

	private void SetButtonData(CustomizedBundlePayType currencyType, UILabel payTxt, UISprite sprite, int cashWidth, int customizedWidth, Vector3 customizedVector3)
	{
		int num = 0;
		CurrencyType currencyType2 = CurrencyType.None;
		switch (currencyType)
		{
		case CustomizedBundlePayType.None:
		{
			string text = LocalizationManager.GetText("Generic.Free");
			InAppPurchaseProductApple inAppPurchaseProduct = GameManager.Instance.gameEconomyData.GetInAppPurchaseProduct(customBundleDefinition.IAPProduct);
			if (inAppPurchaseProduct != null)
			{
				text = ((inAppPurchaseProduct.PriceUSD > 0f) ? GameManager.Instance.IAPManager.GetFormattedPrice(customBundleDefinition.IAPProduct) : LocalizationManager.GetText("Generic.Free"));
			}
			payTxt.text = text;
			payTxt.gameObject.transform.localPosition = Vector3.zero;
			payTxt.width = cashWidth;
			Helpers.GameObjectSetActive(sprite.gameObject, value: false);
			break;
		}
		case CustomizedBundlePayType.TradeFairPay:
			payTxt.text = customBundleDefinition.TradefairPrice.ToString() ?? "";
			payTxt.gameObject.transform.localPosition = customizedVector3;
			payTxt.width = customizedWidth;
			Helpers.GameObjectSetActive(sprite.gameObject, value: true);
			sprite.spriteName = "Ui_Icon_Resource_Fairmoney";
			num = customBundleDefinition.TradefairPrice;
			currencyType2 = CurrencyType.Fairmoney;
			break;
		case CustomizedBundlePayType.DiamondsPay:
			payTxt.text = customBundleDefinition.GoldPrice.ToString() ?? "";
			payTxt.gameObject.transform.localPosition = customizedVector3;
			payTxt.width = customizedWidth;
			Helpers.GameObjectSetActive(sprite.gameObject, value: true);
			sprite.spriteName = "Ui_Icon_Resource_Gold";
			num = customBundleDefinition.GoldPrice;
			currencyType2 = CurrencyType.Diamonds;
			break;
		case CustomizedBundlePayType.BluePrintTokenPay:
			payTxt.text = customBundleDefinition.FragmentPrice.ToString() ?? "";
			payTxt.gameObject.transform.localPosition = customizedVector3;
			payTxt.width = customizedWidth;
			Helpers.GameObjectSetActive(sprite.gameObject, value: true);
			sprite.spriteName = "Ui_Icon_Resource_BluePrintMoney";
			num = customBundleDefinition.FragmentPrice;
			currencyType2 = CurrencyType.BulePrintToken;
			break;
		}
		int currencyAmount = GameManager.Instance.playerModel.GetCurrencyAmount(currencyType2);
		if (num <= currencyAmount)
		{
			payTxt.color = availableCurrencyColor.Color;
		}
		else
		{
			payTxt.color = unavailableCurrencyColor.Color;
		}
	}

	public void OnBuyButton1Click()
	{
		if (_button1Type == CustomizedBundlePayType.None)
		{
			ClickPurchase();
		}
		else
		{
			ClickCustomizedBundlePurchase(_button1Type);
		}
	}

	public void OnBuyButton2Click()
	{
		if (_button2Type == CustomizedBundlePayType.None)
		{
			ClickPurchase();
		}
		else
		{
			ClickCustomizedBundlePurchase(_button2Type);
		}
	}
}
