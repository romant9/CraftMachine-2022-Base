using System;
using System.Collections.Generic;
using System.Text;
using BaseModel;
using Client.Connectivity;
using TWDModel;
using UnityEngine;

public class LoginSevenDayPopup : MonoBehaviour
{
	[SerializeField]
	private List<LoginSevenDayReward> loginSevenDayFreeRewards;

	[SerializeField]
	private List<LoginSevenDayReward> loginSevenDayPremiumRewards;

	[SerializeField]
	private UILabel tradeFairPriceLabel;

	[SerializeField]
	private UILabel priceLabel;

	[SerializeField]
	private UILabel titleLabel;

	[SerializeField]
	private UILabel descLabel;

	[SerializeField]
	private UILabel premiumLabel;

	[SerializeField]
	private GameObject buttonParent;

	[SerializeField]
	private UILabel timeLabel;

	[SerializeField]
	private UILabel timeToNextReward;

	[SerializeField]
	private UITexture backgroundIcon;

	[SerializeField]
	private DiscountButtonController discountButtonController;

	private TradefairBundleContentDefinition _contentDefinition;

	private SevenDayLoginPeriodModel _sevenDayLoginPeriodModel;

	private Vector3 _originalPositionButtonPay;

	private Vector3 _originalPositionButtonTrade;

	private bool _isInitialized;

	[SerializeField]
	[Tooltip("Label show when there is some info")]
	private UILabel infoLabel;

	[SerializeField]
	private UISprite infoSprite;

	[SerializeField]
	[Tooltip("Time to show in seconds")]
	private float timeToShow = 2f;

	[SerializeField]
	private Color errorColor;

	[SerializeField]
	private Color normalColor;

	private void OnEnable()
	{
		UIEvent.OnUIEvent += OnUIEvent;
		SevenDayLoginPeriodModel sevenDayLoginPeriodModel = GameManager.Instance.playerModel?.SevenDayLoginManager?.CurrentPeriodModel;
		if (sevenDayLoginPeriodModel != null)
		{
			sevenDayLoginPeriodModel.Changed += OnModelChanged;
		}
	}

	private void OnDisable()
	{
		UIEvent.OnUIEvent -= OnUIEvent;
		SevenDayLoginPeriodModel sevenDayLoginPeriodModel = GameManager.Instance.playerModel?.SevenDayLoginManager?.CurrentPeriodModel;
		if (sevenDayLoginPeriodModel != null)
		{
			sevenDayLoginPeriodModel.Changed -= OnModelChanged;
		}
	}

	public void Open()
	{
		Helpers.GameObjectSetActive(base.gameObject, value: true);
		_originalPositionButtonPay = buttonParent.transform.Find("Button_Pay").localPosition;
		_originalPositionButtonTrade = buttonParent.transform.Find("Button_Trade").localPosition;
		UpdateUI();
		InternalHide();
	}

	public void Close()
	{
		Helpers.GameObjectSetActive(base.gameObject, value: false);
	}

	private void OnModelChanged(ModelObject m, string changed, object args)
	{
		if (changed == "SevenDayLoginChangeToday")
		{
			UpdateUI();
		}
	}

	private void OnUIEvent(string type, object parameter)
	{
		switch (type)
		{
		case "UpdateLoginSevenDayEvent":
		case "OnBundleBought":
			UpdateUI();
			break;
		case "ShowLoginSevenDayInfoEvent":
			ShowInfo(LocalizationManager.GetText("Popup.SevenDayLogin.NotEnoughRemedy"));
			break;
		}
	}

	private void UpdateUI()
	{
		Transform obj = buttonParent.transform.Find("Button_Pay");
		Transform transform = buttonParent.transform.Find("Button_Trade");
		obj.gameObject.SetActive(value: true);
		transform.gameObject.SetActive(value: false);
		obj.localPosition = new Vector3((_originalPositionButtonPay.x + transform.localPosition.x) / 2f, _originalPositionButtonPay.y, _originalPositionButtonPay.z);
		SevenDayLoginManager sevenDayLoginManager = GameManager.Instance.playerModel?.SevenDayLoginManager;
		string identifier = GameManager.Instance.playerModel?.SevenDayLoginManager?.BundleIdentifier;
		BundleStoreDefinition bundleStoreDefinition = GameManager.Instance.gameEconomyData.GetBundleStoreDefinition(identifier);
		discountButtonController.InitializedeData(bundleStoreDefinition);
		_sevenDayLoginPeriodModel = sevenDayLoginManager?.CurrentPeriodModel;
		if (sevenDayLoginManager == null || _sevenDayLoginPeriodModel == null)
		{
			return;
		}

		if (IsLoadDataManager)
		{
			premiumLabel.text = "Все награды за сегодня получены!";
		}
		for (int i = 0; i < _sevenDayLoginPeriodModel.RewardDays.Count; i++)
		{
			LoginSevenDayReward loginSevenDayReward = loginSevenDayFreeRewards[i];
			if (IsLoadDataManager)
			{
				var dayModel = _sevenDayLoginPeriodModel.RewardDays[i];
				_sevenDayLoginPeriodModel.changeDayItemModelStatus(dayModel);
				if (dayModel.DayStatus == SevenDayLoginDayStatus.TodayCanClaim)
				{
					premiumLabel.text = "Награда (" + dayModel.Day + ") не получена!";
				}
			}
			LoginSevenDayReward loginSevenDayReward2 = loginSevenDayPremiumRewards[i];
			if (loginSevenDayReward != null)
			{
				loginSevenDayReward.UpdateUI(_sevenDayLoginPeriodModel.RewardDays[i], SevenDayLoginRewardType.Free);
			}
			if (loginSevenDayReward2 != null)
			{
				loginSevenDayReward2.UpdateUI(_sevenDayLoginPeriodModel.RewardDays[i], SevenDayLoginRewardType.Premium);
			}
		}
		bool isUnlockPremium = _sevenDayLoginPeriodModel.IsUnlockPremium;
		Helpers.GameObjectSetActive(buttonParent, !isUnlockPremium);
		if (!IsLoadDataManager) HelpersUI.SetContentToLabel(premiumLabel, isUnlockPremium ? LocalizationManager.GetText("Popup.SevenDayLogin.UnlockedPremium") : LocalizationManager.GetText("Popup.SevenDayLogin.UnlockPremium"));
		string identifier2 = GameManager.Instance.playerModel?.SevenDayLoginManager?.BundleIdentifier;
		_contentDefinition = GameManager.Instance.playerModel?.gameEconomyData?.GetTradefairBundleContentDefinition(identifier2);
		if (_contentDefinition != null)
		{
			HelpersUI.SetContentToLabel(tradeFairPriceLabel, _contentDefinition.IAPProduct.ToString() ?? "");
		}
		GameEconomyData gameEconomyData = GameManager.Instance.gameEconomyData;
		string iAPProduct = gameEconomyData.GetBundleContentDefinition(identifier2).IAPProduct;
		string content;
		if (!IsLoadDataManager)
		{
			content = ((gameEconomyData.GetInAppPurchaseProduct(iAPProduct).PriceUSD > 0f) ? GameManager.Instance.IAPManager.GetFormattedPrice(iAPProduct) : LocalizationManager.GetText("Generic.Free"));
		}
		else
		{
			DebugTWD.Log("iAPProduct is " + iAPProduct, DebugType.System);
			content = LocalizationManager.GetText("Generic.Free");
		}
		HelpersUI.SetContentToLabel(priceLabel, content);
		SevenDaysDefinition currentPeriodSevenDaysDefinition = sevenDayLoginManager.CurrentPeriodSevenDaysDefinition;
		if (currentPeriodSevenDaysDefinition == null)
		{
			return;
		}
		var localTimeStart = currentPeriodSevenDaysDefinition.StartDateTime.ToLocalTime();
		var localTimeEnd = currentPeriodSevenDaysDefinition.EndDateTime.ToLocalTime();

		string text = $"{localTimeStart.Year}/{localTimeStart.Month}/{localTimeStart.Day} {localTimeStart.Hour}:{localTimeStart.Minute}";
		string text2 = $"{localTimeEnd.Year}/{localTimeEnd.Month}/{localTimeEnd.Day} {localTimeEnd.Hour}:{localTimeEnd.Minute}";
		HelpersUI.SetContentToLabel(timeLabel, SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Popup.SevenDayLogin.ActivityTime", '\n' + text + "-" + text2));
		int refreshTime = GameManager.Instance.gameEconomyData.SevenDayConfig.RefreshTime;
		HelpersUI.SetContentToLabel(timeToNextReward, SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Popup.SevenDayLogin.RefreshTime", Helpers.FormatTime(refreshTime * 1000)));
		if (backgroundIcon != null)
		{
			UnityEngine.Object obj2 = UnityUtils.LoadFromAssetBundle(currentPeriodSevenDaysDefinition.Background, "itemgraphics");
			if (obj2 != null)
			{
				backgroundIcon.mainTexture = (Texture)obj2;
			}
		}
		HelpersUI.SetColor(descLabel, NGUIText.ParseColor(currentPeriodSevenDaysDefinition.DescColor));
		titleLabel.gradientTop = NGUIText.ParseColor(currentPeriodSevenDaysDefinition.TitleColor1);
		titleLabel.gradientBottom = NGUIText.ParseColor(currentPeriodSevenDaysDefinition.TitleColor2);
	}

	private void Update()
	{
		if (!_isInitialized && GameManager.Instance.IAPManager && GameManager.Instance.IAPManager.IsInitialized())
		{
			_isInitialized = true;
			string identifier = GameManager.Instance.playerModel?.SevenDayLoginManager?.BundleIdentifier;
			GameEconomyData gameEconomyData = GameManager.Instance.gameEconomyData;
			string iAPProduct = gameEconomyData.GetBundleContentDefinition(identifier).IAPProduct;
			string content;
			if (!IsLoadDataManager)
			{
				content = ((gameEconomyData.GetInAppPurchaseProduct(iAPProduct).PriceUSD > 0f) ? GameManager.Instance.IAPManager.GetFormattedPrice(iAPProduct) : LocalizationManager.GetText("Generic.Free"));
			}
			else
			{
				DebugTWD.Log("iAPProduct is " + iAPProduct, DebugType.System);
				content = LocalizationManager.GetText("Generic.Free");
			}
			HelpersUI.SetContentToLabel(priceLabel, content);
		}
	}

	public void OnBuyClick()
	{
		string identifier = GameManager.Instance.playerModel?.SevenDayLoginManager?.BundleIdentifier;
		GameEconomyData gameEconomyData = GameManager.Instance.gameEconomyData;
		BundleStoreDefinition bundleStoreDefinition = gameEconomyData.GetBundleStoreDefinition(identifier);
		BundleContentDefinition bundleContentDefinition = gameEconomyData.GetBundleContentDefinition(identifier);
		GameManager.Instance.IAPManager.Buy(bundleStoreDefinition, bundleContentDefinition);
	}

	public void OnBuyTradeClick()
	{
		if (_contentDefinition == null)
		{
			Debug.LogError("[LoginSevenDayPopup] Not Find Content ID");
			return;
		}
		int iAPProduct = _contentDefinition.IAPProduct;
		CurrencyType currencyType = CurrencyType.Fairmoney;
		int currencyAmount = GameManager.Instance.playerModel.GetCurrencyAmount(currencyType);
		if (iAPProduct <= 0)
		{
			ExecuteBuyCommand();
		}
		else if (currencyAmount >= iAPProduct)
		{
			BuyResourcesPopup obj = (BuyResourcesPopup)SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.BuyResourcesPopup);
			obj.SetConfirmContent(LocalizationManager.GetText("Popup.BuyResources.TradeCrate"), LocalizationManager.GetText("Bp.Trade.Confrim.Faircoin"), iAPProduct, currencyType);
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

	private void ExecuteBuyCommand()
	{
		BuyTradefairBundleCommand command = new BuyTradefairBundleCommand(GameManager.Instance.playerModel?.SevenDayLoginManager?.BundleIdentifier);
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
			iAPConfirmPopupNew.OpenForRewards(_contentDefinition.RewardEntries.RewardsList);
			iAPConfirmPopupNew.SetContent(LocalizationManager.GetText("Popup.IAPConfirm.Title.GenericReward"), null);
			string identifier = GameManager.Instance.playerModel?.SevenDayLoginManager?.BundleIdentifier;
			TradefairBundleStoreDefinition parameter = GameManager.Instance.playerModel?.gameEconomyData?.GetBundleTradefairDefinition(identifier);
			UIEvent.Send("OnBundleBought", parameter);
			if (SingularityMonoBehaviour<AudioManager>.Instance != null)
			{
				SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/purchase");
			}
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
			return;
		}
		HUDElement noCreation = SingularityMonoBehaviour<HUDManager>.Instance.GetNoCreation(UIType.ActivityPopup);
		if (noCreation != null)
		{
			noCreation.Close();
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

	private void ShowInfo(string text, bool isError = false)
	{
		InternalHide();
		SetInfoText(infoLabel, text);
		infoSprite.color = (isError ? errorColor : normalColor);
	}

	private void SetInfoText(UILabel label, string text)
	{
		if (label != null && label.gameObject != null)
		{
			label.gameObject.SetActive(value: true);
			label.text = text;
			CancelInvoke("InternalHide");
			Invoke("InternalHide", timeToShow);
		}
		else
		{
			Debug.LogError("HUDNotification: Could not show notification because label is NULL!");
		}
	}

	private void InternalHide()
	{
		if (infoLabel != null && infoLabel.gameObject != null)
		{
			infoLabel.gameObject.SetActive(value: false);
		}
	}



	#region myparams
	//public UILabel NeedToClaimLabel;
	private bool IsLoadDataManager => OfflineManager.IsLoadDataManager;
	#endregion
}
