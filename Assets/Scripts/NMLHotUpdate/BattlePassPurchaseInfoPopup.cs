using System;
using System.Text;
using System.Threading.Tasks;
using BaseModel;
using Client.Connectivity;
using TWDModel;
using UnityEngine;

public class BattlePassPurchaseInfoPopup : HUDElement
{
	private const string BattlePassPremiumBundleId = "BattlePassPremium";

	[SerializeField]
	private GameObject rewardEntryPrefab;

	[SerializeField]
	private GameObject rewardEntryParent;

	[SerializeField]
	private UILabel remainingTimeLabel;

	[SerializeField]
	private UILabel seasonNoLabel;

	[SerializeField]
	private GameObject purchaseButton;

	[SerializeField]
	private GameObject purchaseFairButton;

	[SerializeField]
	private UILabel priceLabel;

	[SerializeField]
	private UILabel battlePassPriceLabel;

	[SerializeField]
	private DiscountButtonController discountButton;

	[SerializeField]
	private UITexture[] showRewardIcons;

	[SerializeField]
	private GameObject[] showRewardObjects;

	private TradefairBundleContentDefinition contentDefinition;

	private TaskCompletionSource<bool> completionSource;

	private bool cancelledToggle;

	public override void Open()
	{
		base.Open();
		cancelledToggle = false;
		completionSource = null;
		BattlePassModel battlePass = GameManager.Instance.modelManager.Player.BattlePass;
		contentDefinition = GameManager.Instance.playerModel.gameEconomyData.GetTradefairBundleContentDefinition(battlePass.BundleIdentifier);
		if (contentDefinition != null)
		{
			HelpersUI.SetContentToLabel(battlePassPriceLabel, contentDefinition.IAPProduct.ToString() ?? "");
		}
		TimeSpan timeSpan = TimeSpan.FromMilliseconds(battlePass.CurrentSeasonEndDate - battlePass.manager.Player.UtcTimeStamp);
		HelpersUI.SetContentToLabel(remainingTimeLabel, LocalizationManager.GetText("BattlePass.PurchaseInfo.Timer", timeSpan.Days, timeSpan.Hours, timeSpan.Minutes));
		HelpersUI.SetContentToLabel(seasonNoLabel, LocalizationManager.GetText("BattlePass.PurchaseInfo.Season", battlePass.CurrentSeasonId - 1));
		Helpers.GameObjectSetActive(seasonNoLabel, !battlePass.IsBeginnerBattlePass);
		foreach (Transform item in rewardEntryParent.transform)
		{
			item.SetParent(null);
			UnityEngine.Object.Destroy(item.gameObject);
		}
		foreach (IReward rewards in battlePass.GetAllReachedPremiumRewards().RewardsList)
		{
			Helpers.InstantiateToParent(rewardEntryPrefab, rewardEntryParent).GetComponent<BattlePassTrophyRoadRewardEntry>().Bind(rewards, 0, premium: false, 0, interactable: false);
		}
		Helpers.GameObjectSetActive(purchaseButton, !battlePass.PremiumActive);
		Helpers.GameObjectSetActive(purchaseFairButton, !battlePass.PremiumActive);
		if (contentDefinition == null)
		{
			Helpers.GameObjectSetActive(purchaseFairButton, value: false);
		}
		Helpers.GameObjectSetActive(purchaseFairButton, value: false);
		purchaseButton.transform.position = (purchaseButton.transform.position + purchaseFairButton.transform.position) / 2f;
		if (!battlePass.PremiumActive)
		{
			GameEconomyData gameEconomyData = GameManager.Instance.gameEconomyData;
			string iAPProduct = gameEconomyData.GetBundleContentDefinition(battlePass.BundleIdentifier).IAPProduct;
			string content = ((gameEconomyData.GetInAppPurchaseProduct(iAPProduct).PriceUSD > 0f) ? GameManager.Instance.IAPManager.GetFormattedPrice(iAPProduct) : LocalizationManager.GetText("Generic.Free"));
			HelpersUI.SetContentToLabel(priceLabel, content);
			BundleStoreDefinition bundleStoreDefinition = GameManager.Instance.playerModel.gameEconomyData.GetBundleStoreDefinition(battlePass.BundleIdentifier);
			discountButton.InitializedeData(bundleStoreDefinition);
		}
		ApplySeasonPopupConfig(battlePass);
	}

	private void ApplySeasonPopupConfig(BattlePassModel battlePass)
	{
		BattlePassSeasonDefinition[] array = (GameManager.Instance?.gameEconomyData)?.BattlePassSeasonDefinitions;
		if (array == null || array.Length == 0)
		{
			return;
		}
		BattlePassSeasonDefinition battlePassSeasonDefinition = null;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] != null && array[i].Id == battlePass.CurrentSeasonId)
			{
				battlePassSeasonDefinition = array[i];
				break;
			}
		}
		if (battlePassSeasonDefinition != null && battlePass.PopupIcons != null)
		{
			ApplyShowRewardNum(battlePass.PopupIcons.Length);
			ApplyPopupIcon(battlePass.PopupIcons);
		}
	}

	private void ApplyShowRewardNum(int showRewardNum)
	{
		if (showRewardObjects == null)
		{
			return;
		}
		int num = Mathf.Clamp(showRewardNum, 0, showRewardObjects.Length);
		for (int i = 0; i < showRewardObjects.Length; i++)
		{
			Helpers.GameObjectSetActive(showRewardObjects[i], i < num);
		}
		if (showRewardIcons != null)
		{
			for (int j = 0; j < showRewardIcons.Length; j++)
			{
				Helpers.GameObjectSetActive(showRewardIcons[j], j < num);
			}
		}
	}

	private void ApplyPopupIcon(string[] popupIcons)
	{
		if (showRewardIcons != null && showRewardIcons.Length != 0)
		{
			int num = ((popupIcons != null) ? popupIcons.Length : 0);
			for (int i = 0; i < showRewardIcons.Length; i++)
			{
				UITexture textureTarget = showRewardIcons[i];
				string text = ((i < num) ? popupIcons[i] : null);
				LoadImageFromCdn.LoadImageToTarget(textureTarget, string.IsNullOrEmpty(text) ? null : text);
			}
		}
	}

	private void OnEnable()
	{
		GameManager.Instance.playerModel.BattlePass.Changed += BattlePassOnChanged;
	}

	private void OnDisable()
	{
		GameManager.Instance.playerModel.BattlePass.Changed -= BattlePassOnChanged;
	}

	private void BattlePassOnChanged(ModelObject modelObject, string changed, object args)
	{
		if (changed == "PremiumActivated")
		{
			BattlePassClientHelpers.StartPremiumActivationFlow(delegate
			{
				completionSource?.SetResult(result: true);
			});
			Close();
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
		BattlePassModel battlePass = GameManager.Instance.playerModel.BattlePass;
		GameEconomyData gameEconomyData = GameManager.Instance.gameEconomyData;
		BundleStoreDefinition bundleStoreDefinition = gameEconomyData.GetBundleStoreDefinition(battlePass.BundleIdentifier);
		BundleContentDefinition bundleContentDefinition = gameEconomyData.GetBundleContentDefinition(battlePass.BundleIdentifier);
		GameManager.Instance.IAPManager.Buy(bundleStoreDefinition, bundleContentDefinition);
	}

	public void ClickTradeFairPurchase()
	{
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
		Helpers.ExecuteCommand(new BuyTradefairBundleCommand(GameManager.Instance.playerModel.BattlePass.BundleIdentifier));
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
}
