using System;
using System.Text;
using System.Threading.Tasks;
using BaseModel;
using Client.Connectivity;
using TWDModel;
using UnityEngine;

public class ActiveFoundationPurchaseInfoPopup : HUDElement
{
	[SerializeField]
	private UILabel remainingTimeLabel;

	[SerializeField]
	private GameObject purchaseButton;

	[SerializeField]
	private GameObject purchaseFairButton;

	[SerializeField]
	private UILabel priceLabel;

	[SerializeField]
	private UILabel TradeFairPriceLabel;

	private TradefairBundleContentDefinition contentDefinition;

	private TaskCompletionSource<bool> completionSource;

	private bool cancelledToggle;

	private ActiveFoundationManager activeFoundation;

	public override void Open()
	{
		base.Open();
		cancelledToggle = false;
		completionSource = null;
		activeFoundation = GameManager.Instance.playerModel.ActiveFoundationManager;
		contentDefinition = GameManager.Instance.playerModel.gameEconomyData.GetTradefairBundleContentDefinition(activeFoundation.CurrentPeriodModel.BundleIdentifier);
		if (contentDefinition != null)
		{
			HelpersUI.SetContentToLabel(TradeFairPriceLabel, contentDefinition.IAPProduct.ToString() ?? "");
		}
		TimeSpan timeSpan = TimeSpan.FromMilliseconds(activeFoundation.CurrentPeriodModel.CurrentPeriodEndTimeUtc - activeFoundation.manager.Player.UtcTime.TotalMilliseconds());
		HelpersUI.SetContentToLabel(remainingTimeLabel, LocalizationManager.GetText("ActiveFoundation.PurchaseInfo.Timer", timeSpan.Days, timeSpan.Hours, timeSpan.Minutes));
		Helpers.GameObjectSetActive(purchaseButton, !activeFoundation.CurrentPeriodModel.IsUnlockPremium);
		Helpers.GameObjectSetActive(purchaseFairButton, !activeFoundation.CurrentPeriodModel.IsUnlockPremium);
		if (contentDefinition == null)
		{
			Helpers.GameObjectSetActive(purchaseFairButton, value: false);
		}
		Helpers.GameObjectSetActive(purchaseButton, value: true);
		Helpers.GameObjectSetActive(purchaseFairButton, value: false);
		purchaseButton.transform.position = (purchaseButton.transform.position + purchaseFairButton.transform.position) / 2f;
		if (!activeFoundation.CurrentPeriodModel.IsUnlockPremium)
		{
			GameEconomyData gameEconomyData = GameManager.Instance.gameEconomyData;
			string iAPProduct = gameEconomyData.GetBundleContentDefinition(activeFoundation.CurrentPeriodModel.BundleIdentifier).IAPProduct;
			string content = ((gameEconomyData.GetInAppPurchaseProduct(iAPProduct).PriceUSD > 0f) ? GameManager.Instance.IAPManager.GetFormattedPrice(iAPProduct) : LocalizationManager.GetText("Generic.Free"));
			HelpersUI.SetContentToLabel(priceLabel, content);
		}
	}

	public void Cancel()
	{
		if (!cancelledToggle)
		{
			Close();
			completionSource?.SetResult(result: false);
			cancelledToggle = true;
		}
	}

	public Task<bool> OpenWithConfirmationAsync()
	{
		Open();
		completionSource = new TaskCompletionSource<bool>();
		return completionSource.Task;
	}

	public void ClickPurchase()
	{
		if (activeFoundation != null && activeFoundation.CurrentPeriodId >= 0)
		{
			GameEconomyData gameEconomyData = GameManager.Instance.gameEconomyData;
			BundleStoreDefinition bundleStoreDefinition = gameEconomyData.GetBundleStoreDefinition(activeFoundation.CurrentPeriodModel.BundleIdentifier);
			BundleContentDefinition bundleContentDefinition = gameEconomyData.GetBundleContentDefinition(activeFoundation.CurrentPeriodModel.BundleIdentifier);
			GameManager.Instance.IAPManager.Buy(bundleStoreDefinition, bundleContentDefinition);
		}
	}

	private void OnEnable()
	{
		GameManager.Instance.playerModel.ActiveFoundationManager.CurrentPeriodModel.Changed += ActiveFoundationOnChanged;
	}

	private void OnDisable()
	{
		GameManager.Instance.playerModel.ActiveFoundationManager.CurrentPeriodModel.Changed -= ActiveFoundationOnChanged;
	}

	private void ActiveFoundationOnChanged(ModelObject modelObject, string changed, object args)
	{
		if (changed == "UnlockedPremiumEvent")
		{
			ConfirmationPopup confirmationPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.ActiveFoundationPurchasedPopup) as ConfirmationPopup;
			if ((bool)confirmationPopup)
			{
				confirmationPopup.Open();
			}
			Close();
		}
	}

	public void ClickTradeFairPurchase()
	{
		if (activeFoundation == null || activeFoundation.CurrentPeriodId < 0)
		{
			return;
		}
		if (contentDefinition == null)
		{
			Debug.LogError("Battle Pass : Not Find Content ID");
			return;
		}
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
		Helpers.ExecuteCommand(new BuyTradefairBundleCommand(activeFoundation.CurrentPeriodModel.BundleIdentifier));
	}

	private void GoBanana()
	{
		if (GameManager.Instance.gameEconomyData?.ConfigData == null)
		{
			return;
		}
		if (GameManager.Instance.gameEconomyData.ConfigData.IngameBanana)
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
}
