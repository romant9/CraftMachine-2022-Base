using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public static class ShopPopupHelper
{
	public static void OpenForMissingCurrencyWithTotalRequiredAmount(int totalRequiredAmount, CurrencyType currency = CurrencyType.Diamonds)
	{
		MiniShopPopup.OpenWithTotalRequiredCurrencyAmount(currency, totalRequiredAmount);
	}

	public static void OpenForMissingCurrencyWithMissingAmount(int missingAmount, CurrencyType currency = CurrencyType.Diamonds)
	{
		MiniShopPopup.OpenWithMissingCurrencyAmount(currency, missingAmount);
	}

	public static void UpdateCurrentTabIfOpen()
	{
		ShopPopup shopPopup = SingularityMonoBehaviour<HUDManager>.Instance.GetNoCreation(UIType.ShopPopup) as ShopPopup;
		if (shopPopup != null && shopPopup.IsOpen && !shopPopup.IsClosing)
		{
			shopPopup.UpdateSelectedTab();
		}
	}

	public static void OpenWithIndex(int index)
	{
		if (index == 3 && TutorialView.Instance.Running && GameManager.Instance.playerModel.Combat != null)
		{
			Debug.LogError("ShopPopup: Don't open TradeShop when still in tutorial or combat!");
			return;
		}
		if (index == 3 && !IsTradeShopAvailableAndUnlocked())
		{
			Debug.LogError("ShopPopup: Can't open TradeShop still locked!");
			return;
		}
		ShopPopup shopPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.ShopPopup) as ShopPopup;
		if (shopPopup != null)
		{
			shopPopup.OpenForTab(index);
			CampHUD campHUD = CampHUD.Get();
			if (campHUD != null)
			{
				campHUD.PauseCurrencyMeters = false;
			}
		}
	}

	public static void SendEndShopVisitAnalytics(List<BundleStoreDefinition> currentShopContent, HUDElement hudElement)
	{
		if (currentShopContent == null || hudElement == null)
		{
			return;
		}
		string text = "";
		for (int i = 0; i < currentShopContent.Count; i++)
		{
			if (currentShopContent[i] != null && !string.IsNullOrEmpty(currentShopContent[i].BundleIdentifier))
			{
				if (i > 0)
				{
					text += ";";
				}
				text += currentShopContent[i].BundleIdentifier;
			}
		}
		int shopTabIndex = -1;
		if (hudElement is ShopPopup shopPopup)
		{
			shopTabIndex = shopPopup.GetCurrentTabIndex();
		}
		Helpers.ExecuteCommand(new SendMetricCommand(SendMetricCommand.MetricType.ShopViewEnd)
		{
			BundleIds = text,
			BundleSource = GameManager.Instance.BundleSource,
			ViewTimeInSeconds = hudElement.GetPopupOpenInSeconds(),
			ShopTabIndex = shopTabIndex
		});
		hudElement.CreateOpenedTimeStamp();
	}

	public static bool ContainsAnyFreeItems()
	{
		if (ContainsFreeTradeShopItems())
		{
			return true;
		}
		return GetFirstFreeIapItem() != null;
	}

	public static BundleStoreDefinition GetFirstFreeIapItem(int shopTabIndex = -1)
	{
		shopTabIndex = ShopPopup.GetDefinitionByIndex(shopTabIndex);
		PlayerModel playerModel = GameManager.Instance.playerModel;
		List<BundleStoreDefinition> list = playerModel?.gameEconomyData.GetOrderedStoreBundles(playerModel.UtcTimeStamp);
		if (list == null)
		{
			return null;
		}
		foreach (BundleStoreDefinition item in list)
		{
			if (item != null && item.IsTaggedAsFreeItem && (shopTabIndex == -1 || shopTabIndex == item.ShopTabIndex) && GameManager.Instance.playerModel.BundleManager.CanBuyBundle(item))
			{
				return item;
			}
		}
		return null;
	}

	public static TradefairBundleStoreDefinition GetFirstFreeIapItemTradeFair()
	{
		List<TradefairBundleStoreDefinition> list = GameManager.Instance.playerModel?.TradefairManager.GetOrderedAvailableBundles();
		if (list == null)
		{
			return null;
		}
		foreach (TradefairBundleStoreDefinition item in list)
		{
			if (item != null && item.IsTaggedAsFreeItem && GameManager.Instance.playerModel.TradefairManager.CanBuyBundle(item))
			{
				return item;
			}
		}
		return null;
	}

	public static bool ContainsFreeTradeShopItems()
	{
		if (IsTradeShopAvailableAndUnlocked())
		{
			return GameManager.Instance.playerModel.GetFreeTradeShopItemsCount() > 0;
		}
		return false;
	}

	public static bool IsTradeShopAvailableAndUnlocked()
	{
		bool tradeCratesEnabled = GameManager.Instance.gameEconomyData.ConfigData.TradeCratesEnabled;
		bool flag = GameManager.Instance.playerModel.Camp.GetCouncilLevel() >= 3;
		bool flag2 = GameManager.Instance.playerModel.Combat == null;
		bool flag3 = !SingularityMonoBehaviour<HUDManager>.Instance.IsOpen(UIType.AdPopupView);
		return tradeCratesEnabled && flag && flag2 && flag3;
	}

	public static void UpdateListWithData<T>(NUIScrollableList scrollableList, List<T> dataList, bool resetScrollPosition, bool isTabsIndexFeaturedShop, string prefabResourceOverride = "") where T : class
	{
		if (scrollableList == null)
		{
			Debug.LogError("ShopPopup: No Prefab Reference to a NUIScrollableList defined!");
			return;
		}
		Vector2 scrollPosition = scrollableList.GetScrollPosition();
		scrollableList.Clear();
		if (dataList == null)
		{
			Debug.LogError("shoppopup:no data");
			return;
		}
		foreach (T data in dataList)
		{
			string text = "";
			string text2 = "";
			if (data == null)
			{
				continue;
			}
			if (string.IsNullOrEmpty(prefabResourceOverride))
			{
				if (!(data is BundleStoreDefinition bundleStoreDefinition))
				{
					if (!(data is TradefairBundleStoreDefinition tradefairBundleStoreDefinition))
					{
						if (!(data is TradeSlotInfo tradeSlotInfo))
						{
							if (!(data is GoldShopDefinition goldShopDefinition))
							{
								if (!(data is BlackMarketHeroSlot blackMarketHeroSlot))
								{
									if (!(data is HillTopStoreSlot hillTopStoreSlot))
									{
										if (!(data is BlackMarketDefinition blackMarketDefinition))
										{
											if (!(data is HillTopStoreDefinition hillTopStoreDefinition))
											{
												if (data is CustomBundleDefinition)
												{
													text = "Shop_Optional_Card";
												}
												else
												{
													text2 = data.ToString();
												}
											}
											else
											{
												text2 = hillTopStoreDefinition.UniqueId.ToString();
												text = "Shop_HCoin_Item_Card";
											}
										}
										else
										{
											text2 = blackMarketDefinition.UniqueId.ToString();
											text = "Shop_BlackMarket_Item_Card";
										}
									}
									else
									{
										text2 = hillTopStoreSlot.SlotType.ToString();
										text = "Shop_HCoin_Card";
									}
								}
								else
								{
									text2 = blackMarketHeroSlot.ActiveActorDefinitionID;
									text = "Shop_BlackMarket_Card";
								}
							}
							else
							{
								text2 = goldShopDefinition.ItemId;
								text = "Shop_Component_Card";
							}
						}
						else
						{
							text2 = tradeSlotInfo.SlotDefinition.ToString();
							text = "Shop_Trade_Card";
						}
					}
					else
					{
						text2 = tradefairBundleStoreDefinition.BundleIdentifier;
						text = (string.IsNullOrEmpty(tradefairBundleStoreDefinition.CardPrefab) ? "Shop_Item_Card_TradeFair" : tradefairBundleStoreDefinition.CardPrefab);
					}
				}
				else
				{
					text2 = bundleStoreDefinition.BundleIdentifier;
					text = (string.IsNullOrEmpty(bundleStoreDefinition.CardPrefab) ? "Shop_Item_Card" : bundleStoreDefinition.CardPrefab);
					BundleContentDefinition bundleContentDefinition = GameManager.Instance.playerModel.gameEconomyData.GetBundleContentDefinition(bundleStoreDefinition.BundleIdentifier);
					if (bundleContentDefinition != null && !bundleContentDefinition.IsEpic)
					{
						continue;
					}
				}
			}
			else
			{
				text = prefabResourceOverride;
			}
			NUIListItem<T> nUIListItem = scrollableList.InstantiateAdd(text) as NUIListItem<T>;
			if (nUIListItem != null)
			{
				nUIListItem.SetData(data);
				continue;
			}
			Debug.LogError("ShopPopup: Could not load Prefab from: " + text + "Type:" + data?.ToString() + " Item: " + text2);
		}
		if (resetScrollPosition)
		{
			scrollableList.SortAndReset();
			return;
		}
		scrollableList.SortAndRepositionItems();
		scrollableList.SetScrollPosition(scrollPosition);
	}
}
