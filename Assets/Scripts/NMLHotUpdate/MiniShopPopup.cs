using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class MiniShopPopup : HUDElement
{
	[Header("Main List")]
	[SerializeField]
	private NUIScrollableList scrollableList;

	[Header("Title Label")]
	[SerializeField]
	private UILabel titleLabel;

	[Header("Open Shop Button")]
	[SerializeField]
	private UIButtonExtended shopButton;

	[Header("List Items Scale")]
	[SerializeField]
	private float listItemsScale = 0.75f;

	private List<BundleStoreDefinition> currentItems = new List<BundleStoreDefinition>();

	private const int maxDiamondsMissDelta = 14000;

	private static void OpenWithCurrencyAmountImpl(CurrencyType currencyType, int amount, bool amountIsTotalRequiredAmount)
	{
		int value = amount;
		if (amountIsTotalRequiredAmount)
		{
			CurrencyModel currency = GameManager.Instance.playerModel.GetCurrency(currencyType);
			if (currency != null)
			{
				value = amount - currency.Value;
			}
		}
		value = Mathf.Abs(value);
		List<BundleStoreDefinition> list = null;
		list = currencyType switch
		{
			CurrencyType.SPTraitsRemoldToken => FindSuitableDefinitionsWithSPTraitsRemoldToken(),
			CurrencyType.GoldRadio => FindSuitableDefinitionsWithGoldRadio(GameManager.Instance.playerModel.UtcTimeStamp, currencyType, value, BundleContentDefinition.CategoryGoldPack, 2),
			CurrencyType.Fairmoney => FindSuitableDefinitionsWithFairmoney(GameManager.Instance.playerModel.UtcTimeStamp, currencyType, value, BundleContentDefinition.CategoryGoldPack, 2),
			_ => FindSuitableDefinitionsWithMissingParams(GameManager.Instance.playerModel.UtcTimeStamp, currencyType, value, BundleContentDefinition.CategoryGoldPack, 2),
		};
		MiniShopPopup miniShopPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.ShopPopupMini) as MiniShopPopup;
		if (miniShopPopup != null && list != null && list.Count > 0)
		{
			GameManager.Instance.BundleSource = Metrics.BundleSource.MiniShop;
			miniShopPopup.Open();
			miniShopPopup.UpdateWithData(list, currencyType, value);
			if (!OfflineManager.IsLoadDataManager)
			{
				CampHUD campHUD = CampHUD.Get();
				if (campHUD != null)
				{
					campHUD.PauseCurrencyMeters = false;
				}
			}		
		}
	}

	public static void OpenWithTotalRequiredCurrencyAmount(CurrencyType currencyType, int totalRequiredAmount)
	{
		OpenWithCurrencyAmountImpl(currencyType, totalRequiredAmount, amountIsTotalRequiredAmount: true);
	}

	public static void OpenWithMissingCurrencyAmount(CurrencyType currencyType, int amountMissingToRequired)
	{
		OpenWithCurrencyAmountImpl(currencyType, amountMissingToRequired, amountIsTotalRequiredAmount: false);
	}

	public static void HideShopButton()
	{
		MiniShopPopup miniShopPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.ShopPopupMini) as MiniShopPopup;
		if (miniShopPopup != null)
		{
			miniShopPopup.shopButton.gameObject.SetActive(value: false);
		}
	}

	public override void Open()
	{
		base.Open();
		if (shopButton != null)
		{
			shopButton.SetClickCallback(OnClickedShop);
			Helpers.GameObjectSetActive(shopButton, value: true);
		}
	}

	public override void Close()
	{
		base.Close();
		UIEvent.Send("miniShopCLoseEvent");
	}

	public virtual void Clear()
	{
		if (scrollableList != null)
		{
			scrollableList.Clear();
		}
		if (shopButton != null)
		{
			shopButton.Clear();
		}
		currentItems = new List<BundleStoreDefinition>();
	}

	public void UpdateWithData(List<BundleStoreDefinition> data, CurrencyType currencyType, int missingDelta)
	{
		if (scrollableList != null && data != null)
		{
			currentItems = data;
			scrollableList.UpdateWithList(data, "Shop_Item_Card", null);
			for (int i = 0; i < scrollableList.currentItemsList.Count; i++)
			{
				if (scrollableList.currentItemsList[i] != null && scrollableList.currentItemsList[i].transform != null)
				{
					scrollableList.currentItemsList[i].transform.localScale = new Vector3(listItemsScale, listItemsScale, listItemsScale);
					if (scrollableList.currentItemsList[i] is ShopItemCard)
					{
						(scrollableList.currentItemsList[i] as ShopItemCard).OverrideSalesBadge(i == scrollableList.currentItemsList.Count - 1, "IAPCard.SaleLabel.BestValue");
					}
				}
			}
			scrollableList.SortAndReset();
		}
		string currencyName = HelpersLocalization.GetCurrencyName(currencyType);
		HelpersUI.SetContentToLabel(titleLabel, LocalizationManager.GetText("Popup.MiniShop.Title.MissingCurrency{missingDelta}{currencyName}", missingDelta, currencyName));
		Helpers.OpenMiniShopEvent(currencyType);
	}

	private void OnEnable()
	{
		UIEvent.OnUIEvent -= OnUIEvent;
		UIEvent.OnUIEvent += OnUIEvent;
	}

	private void OnDisable()
	{
		UIEvent.OnUIEvent -= OnUIEvent;
		if (GameManager.Instance.Blackboard.IsToggleOn("BuyJustEnoughGasForMission"))
		{
			Helpers.ExecuteCommand(new ClearBlackboardToggleCommand("BuyJustEnoughGasForMission"));
		}
	}

	private void OnClickedShop(UIButtonExtended button)
	{
		if (button != null)
		{
			button.Clear();
		}
		OnClickClose();
		SingularityMonoBehaviour<HUDManager>.Instance.CloseIfExists(UIType.BuyResourcesPopup);
		SingularityMonoBehaviour<HUDManager>.Instance.CloseIfExists(UIType.BuyEnergyPopup);
		SingularityMonoBehaviour<HUDManager>.Instance.CloseIfExists(UIType.CampSurvivorInfoPopup);
		SingularityMonoBehaviour<HUDManager>.Instance.CloseIfExists(UIType.CampEquipmentLevelUpPopup);
		SingularityMonoBehaviour<HUDManager>.Instance.CloseIfExists(UIType.GuildAdvertisePopup);
		SingularityMonoBehaviour<HUDManager>.Instance.CloseIfExists(UIType.SocialSendGuildGift);
		SingularityMonoBehaviour<HUDManager>.Instance.CloseIfExists(UIType.MergeBundlePopup);
		SingularityMonoBehaviour<HUDManager>.Instance.CloseIfExists(UIType.SPRemoldMainPopup);
		SingularityMonoBehaviour<HUDManager>.Instance.CloseIfExists(UIType.CampEquipmentLevelUpPopupNew);
		ShopPopupHelper.OpenWithIndex(2);
	}

	private void OnUIEvent(string type, object parameter)
	{
		if (type == "OnBundleBought")
		{
			OnClickClose();
		}
		else if (type == "OnPopUpClose" && parameter is MiniShopPopup)
		{
			ShopPopupHelper.SendEndShopVisitAnalytics(currentItems, this);
			Clear();
		}
		else if (type == "SendEndShopVisitAnalytics" && parameter != null && parameter is ShopItemCard && currentItems != null && currentItems.Contains((parameter as ShopItemCard).GetData()))
		{
			ShopPopupHelper.SendEndShopVisitAnalytics(currentItems, this);
		}
	}

	private static List<BundleStoreDefinition> FindSuitableDefinitionsWithGoldRadio(long currentTimeMs, CurrencyType currency, int missingDelta, string category, int returnMax = -1)
	{
		List<BundleStoreDefinition> orderedStoreBundles = GameManager.Instance.gameEconomyData.GetOrderedStoreBundles(currentTimeMs);
		List<BundleContentDefinition> bundleContentDefinitionsWithCategory = GameManager.Instance.gameEconomyData.GetBundleContentDefinitionsWithCategory(category);
		List<BundleStoreDefinition> list = new List<BundleStoreDefinition>();
		BundleStoreDefinition bundleStoreDefinition = GameManager.Instance.gameEconomyData.GetBundleStoreDefinition("TWD_GoldRadio");
		BundleStoreDefinition bundleStoreDefinition2 = GameManager.Instance.gameEconomyData.GetBundleStoreDefinition("TWD_GoldRadio_90");
		if (bundleStoreDefinition != null)
		{
			GameManager.Instance.playerModel.gameEconomyData.GetBundleContentDefinition(bundleStoreDefinition.BundleIdentifier);
			list.Add(bundleStoreDefinition);
		}
		if (bundleStoreDefinition2 != null)
		{
			GameManager.Instance.playerModel.gameEconomyData.GetBundleContentDefinition(bundleStoreDefinition2.BundleIdentifier);
			list.Add(bundleStoreDefinition2);
		}
		return list;
	}

	private static List<BundleStoreDefinition> FindSuitableDefinitionsWithMissingParams(long currentTimeMs, CurrencyType currency, int missingDelta, string category, int returnMax = -1)
	{
		List<BundleStoreDefinition> orderedStoreBundles = GameManager.Instance.gameEconomyData.GetOrderedStoreBundles(currentTimeMs);
		List<BundleContentDefinition> bundleContentDefinitionsWithCategory = GameManager.Instance.gameEconomyData.GetBundleContentDefinitionsWithCategory(category);
		List<BundleStoreDefinition> list = new List<BundleStoreDefinition>();
		if (currency == CurrencyType.Diamonds)
		{
			missingDelta = ((missingDelta > 14000) ? 14000 : missingDelta);
		}
		for (int i = 0; i < bundleContentDefinitionsWithCategory.Count; i++)
		{
			if (bundleContentDefinitionsWithCategory[i] == null || bundleContentDefinitionsWithCategory[i].RewardEntries == null || bundleContentDefinitionsWithCategory[i].RewardEntries.GetTotalCurrencyRewardAmount(currency) < missingDelta || bundleContentDefinitionsWithCategory[i].IsThirdParty != ThirdPartyName.None || !bundleContentDefinitionsWithCategory[i].IsAPP)
			{
				continue;
			}
			for (int j = 0; j < orderedStoreBundles.Count; j++)
			{
				if (orderedStoreBundles[j] != null && bundleContentDefinitionsWithCategory[i] != null && orderedStoreBundles[j].BundleIdentifier == bundleContentDefinitionsWithCategory[i].Identifier)
				{
					list.Add(orderedStoreBundles[j]);
					if (returnMax != -1 && list.Count >= returnMax)
					{
						return list;
					}
				}
			}
		}
		return list;
	}

	private static List<BundleStoreDefinition> FindSuitableDefinitionsWithSPTraitsRemoldToken()
	{
		List<BundleStoreDefinition> list = new List<BundleStoreDefinition>();
		BundleStoreDefinition bundleStoreDefinition = GameManager.Instance.gameEconomyData.GetBundleStoreDefinition("TWD_MODELLING_SPTraitsRemoldToken");
		if (bundleStoreDefinition != null)
		{
			list.Add(bundleStoreDefinition);
		}
		return list;
	}

	private static List<BundleStoreDefinition> FindSuitableDefinitionsWithFairmoney(long currentTimeMs, CurrencyType currency, int missingDelta, string category, int returnMax = -1)
	{
		GameManager.Instance.gameEconomyData.GetOrderedStoreBundles(currentTimeMs);
		GameManager.Instance.gameEconomyData.GetBundleContentDefinitionsWithCategory(category);
		List<BundleStoreDefinition> list = new List<BundleStoreDefinition>();
		BundleStoreDefinition bundleStoreDefinition = GameManager.Instance.gameEconomyData.GetBundleStoreDefinition("TWD_RESOURCE_FAIRCOIN1399_BUNDLE");
		BundleStoreDefinition bundleStoreDefinition2 = GameManager.Instance.gameEconomyData.GetBundleStoreDefinition("TWD_BOX_OF_FAIRCOINS");
		if (bundleStoreDefinition != null)
		{
			GameManager.Instance.playerModel.gameEconomyData.GetBundleContentDefinition(bundleStoreDefinition.BundleIdentifier);
			list.Add(bundleStoreDefinition);
		}
		if (bundleStoreDefinition2 != null)
		{
			GameManager.Instance.playerModel.gameEconomyData.GetBundleContentDefinition(bundleStoreDefinition2.BundleIdentifier);
			list.Add(bundleStoreDefinition2);
		}
		return list;
	}
}
