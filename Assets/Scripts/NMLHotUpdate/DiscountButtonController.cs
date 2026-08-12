using System;
using System.Collections;
using System.Text;
using BaseModel;
using Client.Connectivity;
using TWDModel;
using UnityEngine;

public class DiscountButtonController : MonoBehaviour
{
	[Header("Button Discount")]
	[SerializeField]
	private GameObject newDiscountUI;

	[SerializeField]
	private GameObject oldUI;

	[SerializeField]
	private UISprite compCoin;

	[SerializeField]
	private UILabel newPrice;

	[SerializeField]
	private UILabel oldPrice;

	[SerializeField]
	private UILabel discount;

	[SerializeField]
	private bool useDoubleShow;

	[SerializeField]
	private GameObject newDoubleUI;

	[SerializeField]
	private GameObject extraGiftObj;

	[SerializeField]
	private UISprite extraGiftIcon;

	[SerializeField]
	private UILabel extraGiftLabel;

	[SerializeField]
	private GameObject discountGiftObj;

	private BundleStoreDefinition storeDefinition;

	private WebShopBundleContent webShopBundleContent;

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

	public void InitializedeData(BundleStoreDefinition _storeDefinition)
	{
		storeDefinition = _storeDefinition;
		webShopBundleContent = GameManager.Instance.playerModel.gameEconomyData.GetWebshopBundleContentByBundleContentBundleId(storeDefinition.BundleIdentifier);
	}

	public void ClearData()
	{
		storeDefinition = null;
		webShopBundleContent = null;
	}

	private void Start()
	{
		Helpers.GameObjectSetActive(newDiscountUI, value: false);
	}

	public void Update()
	{
		UpdateWebShopDiscountUI();
	}

	public virtual void UpdateWebShopDiscountUI()
	{
		Helpers.GameObjectSetActive(newDiscountUI, value: false);
		Helpers.GameObjectSetActive(oldUI, value: true);
		Helpers.GameObjectSetActive(discountGiftObj, value: true);
		if (Helpers.IsPCPlatform() || !CheckBanana() || storeDefinition == null || newDiscountUI == null || webShopBundleContent == null)
		{
			return;
		}
		if (useDoubleShow)
		{
			Helpers.GameObjectSetActive(oldUI, value: false);
			Helpers.GameObjectSetActive(newDoubleUI, value: true);
		}
		float discountPrice = webShopBundleContent.DiscountPrice;
		float price = webShopBundleContent.Price;
		if (discountPrice == price)
		{
			if (GameManager.Instance.gameEconomyData.ConfigData.IsPriceInRange(price))
			{
				Helpers.GameObjectSetActive(newDiscountUI, value: true);
				Helpers.GameObjectSetActive(oldPrice, value: false);
				Helpers.GameObjectSetActive(compCoin, value: false);
				Helpers.GameObjectSetActive(discountGiftObj, value: false);
				HelpersUI.SetContentToLabel(newPrice, "$ " + discountPrice);
				Helpers.GameObjectSetActive(extraGiftObj, value: true);
				Rewards extraGiftRewards = GameManager.Instance.gameEconomyData.ConfigData.GetExtraGiftRewards();
				if (extraGiftRewards != null && extraGiftRewards.RewardsList.Count > 0 && extraGiftRewards.RewardsList[0] is RewardCurrency rewardCurrency)
				{
					extraGiftIcon.spriteName = HelpersGfx.GetCurrencyIconName(rewardCurrency.CurrencyType);
					HelpersUI.SetContentToLabel(extraGiftLabel, "Free + " + rewardCurrency.Amount);
				}
			}
			else
			{
				Helpers.GameObjectSetActive(extraGiftObj, value: false);
			}
			return;
		}
		Helpers.GameObjectSetActive(extraGiftObj, value: false);
		Helpers.GameObjectSetActive(newDiscountUI, value: true);
		int num = (int)Math.Round((1f - discountPrice / price) * 100f);
		HelpersUI.SetContentToLabel(oldPrice, "[s]$ " + price + "[/s]");
		HelpersUI.SetContentToLabel(newPrice, "$ " + discountPrice);
		oldPrice.color = Color.gray;
		string content = "-" + num + "%";
		HelpersUI.SetContentToLabel(discount, content);
		string reward = webShopBundleContent.Reward;
		Helpers.GameObjectSetActive(compCoin, value: false);
		if (reward.Contains("HillTopCoin"))
		{
			compCoin.spriteName = "Ui_Icon_Resource_HillCoin";
			Helpers.GameObjectSetActive(compCoin, value: true);
		}
		else if (reward.Contains("Fairmoney") && reward.Contains(";"))
		{
			compCoin.spriteName = "Ui_Icon_Resource_Fairmoney";
			Helpers.GameObjectSetActive(compCoin, value: true);
		}
	}

	private bool CheckBanana()
	{
		bool ingameBanana = Helpers.GetIngameBanana();
		double totalUSDSpent = GameManager.Instance.playerModel.TotalUSDSpent;
		if (ingameBanana && totalUSDSpent > 0.0)
		{
			return true;
		}
		return false;
	}

	public void GoBananaAndBuyOnClick()
	{
		if (Helpers.ExecuteCommand(new BuyBundleViaWebshopCheckCommand(storeDefinition.BundleIdentifier)) == TWDModelResult.OK && GameManager.Instance.IsConnectedToServer)
		{
			SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.IngameLoading).Open();
			SignalRClient.Instance.RequestCommand("GetBananaLoginCode", OnGetTransferCode2, waitForResponse: true);
		}
		StartCoroutine(ClosePopupsWithDelay(1f));
	}

	private IEnumerator ClosePopupsWithDelay(float delay)
	{
		yield return new WaitForSeconds(delay);
		SingularityMonoBehaviour<HUDManager>.Instance.CloseIfExists(UIType.ShopPopupMini);
		SingularityMonoBehaviour<HUDManager>.Instance.CloseIfExists(UIType.BuyResourcesPopup);
		SingularityMonoBehaviour<HUDManager>.Instance.CloseIfExists(UIType.BundleCardPopup);
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

	private void GoWebShopUrl(TransferCode transferCode)
	{
		if (transferCode == null || string.IsNullOrEmpty(transferCode.Code))
		{
			return;
		}
		PlayerModel playerModel = GameManager.Instance.playerModel;
		string text = "https://webshop-dev.drillerservices.com/iap-middle-page?EncryptFields=";
		if (playerModel.HashedId != null)
		{
			if (!string.IsNullOrEmpty(Helpers.GetBananaURL()))
			{
				text = Helpers.GetBananaURL();
				text += "/iap-middle-page?EncryptFields=";
			}
			string id = Convert.ToBase64String(Encoding.UTF8.GetBytes("ydldeca" + playerModel.HashedId + "twd"));
			string text2 = ShopItemCardTradeFair.EncryptFields(new ShopItemCardTradeFair.HttpData
			{
				id = id,
				code = transferCode.Code,
				BundleId = webShopBundleContent.Bundleid,
				PeriodId = 1,
				PurchaseSource = "IAPBundle",
				GameURL = GameManager.ActiveConfiguration.UrlScheme + "://",
				IsNewVersion = true,
				DeviceId = GameManager.Instance.LoginRequest.Device.DeviceId,
				OS = Helpers.GetPlatformName(Application.platform)
			}, key, iv);
			text2 = text2.Replace("+", "%2B");
			text += text2;
			Application.OpenURL(text);
		}
	}
}
