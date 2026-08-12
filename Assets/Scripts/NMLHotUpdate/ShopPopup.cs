using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BaseModel;
using Client.BlackMarket;
using Client.HCoin;
using TWDModel;
using UnityEngine;

public class ShopPopup : HUDElement
{
	[SerializeField]
	[Header("Main List")]
	private NUIScrollableList scrollableList;

	[SerializeField]
	private GameObject newMainContent;

	[SerializeField]
	[Header("Main Tabs")]
	private UIButtonToggleSet tabButtons;

	[SerializeField]
	[Header("Trade Timers")]
	private GameObject restockTimerParent;

	[SerializeField]
	private UILabel restockTimerLabel;

	[SerializeField]
	private GameObject componentShopTab;

	[SerializeField]
	private GameObject BundleClassPrefab;

	[SerializeField]
	private GameObject BundleClassScrollContainer;

	[SerializeField]
	private UIScrollView BundleClassScrollView;

	[SerializeField]
	private UITable BundleClassTable;

	[SerializeField]
	private GameObject BundleClassContainer;

	[SerializeField]
	private UILabel currentBundleClassTxt;

	private BundleClassification CurrentClassification = BundleClassification.Max;

	[SerializeField]
	private UILabel XShopLable;

	[SerializeField]
	private GameObject rightTabsContainer;

	[SerializeField]
	private UIButtonToggleSet toggleSet;

	[SerializeField]
	private GameObject toggleSetContainer;

	[SerializeField]
	private UINewShopMain UINewShopMain;

	private int currentToggleIndex = -1;

	private BundleClassification currentBundleClassification;

	public const int ToggleIndexTradefairShop = 0;

	public const int ToggleIndexHCoinShop = 1;

	[SerializeField]
	private UIButtonToggleSet bundleToggleSet;

	[SerializeField]
	private GameObject bundleToggleSetContainer;

	private int currentBundleToggleIndex = -1;

	public const int ToggleIndexNormalBundleShop = 0;

	public const int ToggleIndexOptionalBundleShop = 1;

	public const int TabsIndexFeaturedShop = 0;

	public const int TabsIndexXShop = 1;

	public const int TabsIndexResourceShop = 2;

	public const int TabsIndexTradeShop = 3;

	public const int TabsIndexBlackMarket = 4;

	public const int TabsIndexGoldShop = 5;

	private const float TimerUpdateInterval = 0.1f;

	public const string DefaultTradeCardPrefabName = "Shop_Trade_Card";

	public const string DefaultShopCardPrefabName = "Shop_Item_Card";

	public const string DefaultShopCardTradeFairPrefabName = "Shop_Item_Card_TradeFair";

	public const string CraftingShopCardPrefabName = "Shop_Component_Card";

	public const string BlackMarketHeroCardPrefabName = "Shop_BlackMarket_Card";

	public const string HCoinCardPrefabName = "Shop_HCoin_Card";

	public const string BlackMarketItemCardPrefabName = "Shop_BlackMarket_Item_Card";

	public const string HCoinItemCardPrefabName = "Shop_HCoin_Item_Card";

	public const string ShopOptionalCardPrefabName = "Shop_Optional_Card";

	private List<BundleStoreDefinition> currentBundleIapList;

	private List<TradeSlotInfo> currentTradeList;

	private bool showResetEffectOnCards;

	private int lastSelectedTabIndex = -1;

	private Coroutine updateTimerCoroutine;

	private UIScrollView scrollView;

	private UIWidget tabButtonsWidget;

	public int IndexOfLastItemClicked { get; private set; }

	public static int GetDefinitionByIndex(int index)
	{
		return index switch
		{
			0 => 0,
			2 => 2,
			3 => 3,
			_ => index,
		};
	}

	private void Awake()
	{
		tabButtons.GetUIButtonToggleList[1].gameObject.SetActive(value: false);
		Dictionary<int, List<TweenerPlayer>> dictionary = new Dictionary<int, List<TweenerPlayer>>();
		List<int> list = new List<int>();
		list.Add(0);
		list.Add(2);
		list.Add(3);
		list.Add(4);
		list.Add(5);
		for (int num = list.Count - 1; num >= 0; num--)
		{
			if (list[num] != num)
			{
				tabButtons.GetUIButtonToggleList[list[num]].transform.localPosition = new Vector3(tabButtons.GetUIButtonToggleList[num].transform.localPosition.x, tabButtons.GetUIButtonToggleList[list[num]].transform.localPosition.y, tabButtons.GetUIButtonToggleList[list[num]].transform.localPosition.z);
				TweenerPlayer component = tabButtons.GetUIButtonToggleList[list[num]].GetComponent<TweenerPlayer>();
				TweenerPlayer component2 = tabButtons.GetUIButtonToggleList[num].GetComponent<TweenerPlayer>();
				dictionary.Clear();
				foreach (KeyValuePair<int, List<UITweener>> item in component.AnimationsIndexedByGroupId())
				{
					if (component2.AnimationsIndexedByGroupId().ContainsKey(item.Key))
					{
						if (item.Value[0] is TweenWidth)
						{
							TweenWidth obj = (TweenWidth)item.Value[0];
							TweenWidth tweenWidth = (TweenWidth)component2.AnimationsIndexedByGroupId()[item.Key][0];
							obj.from = tweenWidth.from;
							obj.to = tweenWidth.to;
						}
						else if (item.Value[0] is TweenPosition)
						{
							TweenPosition obj2 = (TweenPosition)item.Value[0];
							TweenPosition tweenPosition = (TweenPosition)component2.AnimationsIndexedByGroupId()[item.Key][0];
							obj2.from = tweenPosition.from;
							obj2.to = tweenPosition.to;
						}
					}
					else
					{
						if (!dictionary.ContainsKey(item.Key))
						{
							dictionary[item.Key] = new List<TweenerPlayer>();
						}
						dictionary[item.Key].Add(component);
					}
				}
				foreach (KeyValuePair<int, List<TweenerPlayer>> item2 in dictionary)
				{
					foreach (TweenerPlayer item3 in item2.Value)
					{
						item3.AnimationsIndexedByGroupId().Remove(item2.Key);
					}
				}
			}
		}
		scrollView = scrollableList.GetComponent<UIScrollView>();
		tabButtonsWidget = tabButtons.GetComponent<UIWidget>();
	}

	public override void Open()
	{
		OpenForTab(0);
	}

	public void OpenForTab(int tab)
	{
		if (base.IsOpen)
		{
			tabButtons.SetSelectedIndex(tab);
			return;
		}
		base.Open();
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/open_shop");
		if (tabButtons != null)
		{
			tabButtons.SetChangeCallback(OnNewTabSelected);
			int initialToggle = Mathf.Clamp(tab, 0, tabButtons.GetUIButtonToggleList.Length - 1);
			tabButtons.SetInitialToggle(initialToggle);
			if (tabButtons.GetUIButtonToggleList != null && tabButtons.GetUIButtonToggleList.Length > 2 && tabButtons.GetUIButtonToggleList[3] != null)
			{
				Helpers.GameObjectSetActive(tabButtons.GetUIButtonToggleList[3], ShopPopupHelper.IsTradeShopAvailableAndUnlocked());
			}
		}
		if (updateTimerCoroutine == null)
		{
			updateTimerCoroutine = StartCoroutine(UpdateTimer(0.1f));
		}
		if (CampView.Instance != null && !SingularityMonoBehaviour<HUDManager>.Instance.IsOpen(UIType.AdPopupView))
		{
			CampView.Instance.Hud.EnableAllButtons(enable: false);
		}
		if (GameManager.Instance.playerModel.IsCraftingAvailable)
		{
			Helpers.GameObjectSetActive(componentShopTab, value: true);
			tabButtonsWidget.leftAnchor.absolute = -50;
		}
		else
		{
			Helpers.GameObjectSetActive(componentShopTab, value: false);
			tabButtonsWidget.leftAnchor.absolute = 50;
		}
		InitUIBundleClass();
		InitBundleToggleSet();
		if (GameManager.Instance.IsConnectedToServer)
		{
			GameManager.Instance.SendWebShopRequest();
		}
	}

	private void OnEnable()
	{
		GameManager.Instance.playerModel.Changed += OnPlayerModelChanged;
		UIEvent.OnUIEvent += OnUiEvent;
		SingularityMonoBehaviour<LocalizationManager>.Instance.OnLocalizationLanguageChanged += OnLocalizationLanguageChanged;
		if (GameManager.Instance.playerModel.CustomizedBundleManager != null)
		{
			GameManager.Instance.playerModel.CustomizedBundleManager.Changed += OnBundleManagerChanged;
		}
	}

	private void OnDisable()
	{
		GameManager.Instance.playerModel.Changed -= OnPlayerModelChanged;
		UIEvent.OnUIEvent -= OnUiEvent;
		SingularityMonoBehaviour<LocalizationManager>.Instance.OnLocalizationLanguageChanged -= OnLocalizationLanguageChanged;
		if (GameManager.Instance.playerModel.CustomizedBundleManager != null)
		{
			GameManager.Instance.playerModel.CustomizedBundleManager.Changed -= OnBundleManagerChanged;
		}
	}

	public void OnBundleManagerChanged(ModelObject m, string changed, object args)
	{
		if (changed == "LimitedCustomBundleAvailableEvent" || changed == "LimitedCustomBundleExpiredEvent")
		{
			UpdateBundleToggleSet(0, 1, resetPosition: false);
		}
	}

	private void OnLocalizationLanguageChanged(string newLanguage)
	{
		if (currentBundleClassTxt != null)
		{
			currentBundleClassTxt.text = LocalizationManager.GetText("BundleClass." + CurrentClassification);
		}
		UpdateSelectedTab();
	}

	public override void Close()
	{
		if (CampView.Instance != null && !SingularityMonoBehaviour<HUDManager>.Instance.IsOpen(UIType.AdPopupView))
		{
			CampView.Instance.Hud.EnableAllButtons(enable: true);
		}
		base.Close();
		CampHUD.SetBlackMarketHudCurrencyVisibility(visibility: false);
		CampHUD.SetHillTopCoinHudCurrencyVisibility(visibility: false);
		CampHUD.SetBluePrintHudCurrencyVisibility(visibility: false);
		CampHUD.SetTopLeftContainerVisibility(visibility: true);
		CampHUD.SetTradeFairHudCurrencyVisibility(visibility: false);
		CampHUD.SetSPTraitsUpgradeTokensHudCurrencyVisibility(visibility: true);
		Helpers.GameObjectSetActive(BundleClassContainer, value: false);
	}

	private void Clear()
	{
		if (scrollableList != null)
		{
			scrollableList.Clear();
		}
		if (tabButtons != null)
		{
			tabButtons.Clear();
		}
		currentBundleIapList = new List<BundleStoreDefinition>();
		currentTradeList = new List<TradeSlotInfo>();
		if (UINewShopMain != null)
		{
			UINewShopMain.ClearRight();
		}
		StopCoroutine(updateTimerCoroutine);
		updateTimerCoroutine = null;
		showResetEffectOnCards = false;
		lastSelectedTabIndex = -1;
	}

	public void HideMainContent()
	{
		Helpers.GameObjectSetActive(scrollableList, value: false);
	}

	public void ShowMainContent()
	{
		int currentTabIndex = GetCurrentTabIndex();
		ShowMainContent(currentTabIndex, currentToggleIndex);
	}

	public void ShowMainContent(int newSelectedTabIndex, int newToggleIndex)
	{
		Helpers.GameObjectSetActive(newMainContent, value: false);
		Helpers.GameObjectSetActive(scrollableList, value: false);
		if (Helpers.IsNewShopVersion() && ((newSelectedTabIndex == 1 && newToggleIndex == 0) || newSelectedTabIndex == 5 || newSelectedTabIndex == 0))
		{
			Helpers.GameObjectSetActive(newMainContent, value: true);
			Helpers.GameObjectSetActive(scrollableList, value: false);
		}
		else
		{
			Helpers.GameObjectSetActive(newMainContent, value: false);
			Helpers.GameObjectSetActive(scrollableList, value: true);
		}
	}

	public int GetCurrentTabIndex()
	{
		if (!(tabButtons == null))
		{
			return tabButtons.GetSelectedIndex();
		}
		return -1;
	}

	public void UpdateSelectedTab()
	{
		if (!(tabButtons == null))
		{
			if (scrollableList != null)
			{
				scrollableList.SaveCurrentScrollPosition();
			}
			tabButtons.SetSelectedIndex(tabButtons.GetSelectedIndex());
		}
	}

	public void UpdateCardUIs()
	{
		scrollableList.currentItemsList.ForEach(delegate(NUIListItemBase x)
		{
			x.UpdateUI();
		});
	}

	private IEnumerator UpdateTimer(float interval)
	{
		while (true)
		{
			if (restockTimerParent != null && restockTimerParent.activeInHierarchy)
			{
				HelpersUI.SetContentToLabel(restockTimerLabel, LocalizationManager.GetText("Popup.BuildMenu.NextTradeRefresh{timeToRefresh}", Helpers.FormatTime(GameManager.Instance.playerModel.GetTimeLeftToTradeShopRefresh())));
			}
			yield return new WaitForSeconds(interval);
		}
	}

	private void OnNewTabSelected(UIButtonExtended toggle)
	{
		BlackMarketShopController.Instance.HideContent();
		HCoinShopController.Instance.HideContent();
		Helpers.GameObjectSetActive(restockTimerParent, value: false);
		if (tabButtons == null || scrollableList == null)
		{
			return;
		}
		bool flag = false;
		int selectedIndex = tabButtons.GetSelectedIndex();
		if (selectedIndex != lastSelectedTabIndex)
		{
			flag = true;
			UIEvent.Send("NewShopTabChanagedEvent", new object[2] { selectedIndex, lastSelectedTabIndex });
			if (lastSelectedTabIndex > -1 && lastSelectedTabIndex != 3)
			{
				GameManager.Instance.BundleSource = Metrics.BundleSource.Shop;
				ShopPopupHelper.SendEndShopVisitAnalytics(currentBundleIapList, this);
			}
			CreateOpenedTimeStamp();
		}
		PlayUITabAnimations(selectedIndex);
		scrollView.contentPivot = UIWidget.Pivot.TopLeft;
		CampHUD.SetHillTopCoinHudCurrencyVisibility(visibility: false);
		CampHUD.SetSPTraitsUpgradeTokensHudCurrencyVisibility(visibility: false);
		CampHUD.SetTopLeftContainerVisibility(visibility: true);
		CampHUD.SetBlackMarketHudCurrencyVisibility(selectedIndex > 0 && selectedIndex != 1);
		CampHUD.SetTradeFairHudCurrencyVisibility(selectedIndex == 0);
		CampHUD.SetBluePrintHudCurrencyVisibility(selectedIndex == 0);
		Helpers.GameObjectSetActive(toggleSetContainer, value: false);
		Helpers.GameObjectSetActive(bundleToggleSetContainer, value: false);
		Helpers.GameObjectSetActive(BundleClassContainer, value: false);
		if (flag)
		{
			currentBundleClassification = BundleClassification.All;
		}
		if ((uint)selectedIndex <= 1u)
		{
			ShowMainContent(selectedIndex, 0);
		}
		else
		{
			ShowMainContent(selectedIndex, -1);
		}
		switch (selectedIndex)
		{
		case 1:
			if (GameManager.Instance.IsConnectedToServer)
			{
				GameManager.Instance.SendWebShopRequest();
			}
			toggleSet.SetSelectedIndex(0);
			UpdateToggleSet(selectedIndex, 0, currentBundleClassification, flag);
			break;
		case 0:
			bundleToggleSet.SetSelectedIndex(0);
			UpdateBundleToggleSet(selectedIndex, 0, flag);
			break;
		case 2:
			currentBundleIapList = GameManager.Instance.playerModel.BundleManager.GetOrderedAvailableBundlesWithShopTabIndex(1);
			if (Helpers.GetShopRoleType() == ShopRoleType.IOS)
			{
				if (GameManager.Instance.gameEconomyData.ConfigData.ResourceBundleSwitch)
				{
					currentBundleIapList.RemoveAll((BundleStoreDefinition item) => item.BundleIdentifier == "TWD_BOX_OF_FAIRCOINS");
				}
			}
			else if (GameManager.Instance.gameEconomyData.ConfigData.ControlResourceBundle)
			{
				currentBundleIapList.RemoveAll((BundleStoreDefinition item) => item.BundleIdentifier == "TWD_BOX_OF_FAIRCOINS");
			}
			currentBundleIapList.RemoveAll(delegate(BundleStoreDefinition x)
			{
				if (!string.IsNullOrEmpty(x.BundleIdentifier))
				{
					BundleContentDefinition bundleContentDefinition = GameManager.Instance.gameEconomyData.GetBundleContentDefinition(x.BundleIdentifier);
					if (bundleContentDefinition != null && bundleContentDefinition.IsThirdParty == ThirdPartyName.banana)
					{
						return true;
					}
				}
				return false;
			});
			ShopPopupHelper.UpdateListWithData(scrollableList, currentBundleIapList, flag, isTabsIndexFeaturedShop: false);
			break;
		case 3:
			if (!ShopPopupHelper.IsTradeShopAvailableAndUnlocked())
			{
				currentBundleIapList = new List<BundleStoreDefinition>();
				ShopPopupHelper.UpdateListWithData(scrollableList, currentBundleIapList, flag, isTabsIndexFeaturedShop: false);
				break;
			}
			currentBundleIapList = new List<BundleStoreDefinition>();
			SetupTradeItemsData();
			ShopPopupHelper.UpdateListWithData(scrollableList, currentTradeList, flag, isTabsIndexFeaturedShop: false);
			Helpers.GameObjectSetActive(restockTimerParent, value: true);
			if (showResetEffectOnCards && scrollableList.currentItemsList != null)
			{
				showResetEffectOnCards = false;
				foreach (NUIListItemBase currentItems in scrollableList.currentItemsList)
				{
					if (currentItems != null && currentItems is TradeItemCard tradeItemCard)
					{
						tradeItemCard.TriggerResetEffects();
					}
				}
			}
			TWDPlayerPrefs.SetString("TradeGoodShopVisitTime", GameManager.Instance.playerModel.LifeTime.ToString());
			break;
		case 5:
		{
			List<GoldShopDefinition> orderedAvailableBundles = GameManager.Instance.playerModel.GoldShopDefinitionManager.GetOrderedAvailableBundles();
			ShopPopupHelper.UpdateListWithData(scrollableList, orderedAvailableBundles, flag, isTabsIndexFeaturedShop: false);
			UINewShopMain.UpdateUI(BundleClassification.All, null, null, orderedAvailableBundles);
			break;
		}
		case 4:
		{
			if (GameManager.Instance.playerModel.BlackMarket.NeedToUpdate())
			{
				Helpers.ExecuteCommand(new UpdateBlackMarketCommand());
			}
			List<BlackMarketHeroSlot> dataList = GameManager.Instance.playerModel.BlackMarket.Slots.ToList();
			ShopPopupHelper.UpdateListWithData(scrollableList, dataList, flag, isTabsIndexFeaturedShop: false);
			break;
		}
		default:
			currentBundleIapList = new List<BundleStoreDefinition>();
			ShopPopupHelper.UpdateListWithData(scrollableList, currentBundleIapList, flag, isTabsIndexFeaturedShop: false);
			break;
		}
		lastSelectedTabIndex = selectedIndex;
		if (flag)
		{
			scrollableList.ResetScrollPosition();
		}
	}

	private void PlayUITabAnimations(int openedTab)
	{
		for (int i = 0; i < tabButtons.GetUIButtonToggleList.Length; i++)
		{
			TweenerPlayer component = tabButtons.GetUIButtonToggleList[i].GetComponent<TweenerPlayer>();
			if (i == openedTab)
			{
				component.PlayGroup(10, lastSelectedTabIndex == -1);
			}
			else if (i == lastSelectedTabIndex || (i == 0 && lastSelectedTabIndex == -1))
			{
				component.PlayGroup(11, lastSelectedTabIndex == -1);
			}
			int num = Mathf.Min(lastSelectedTabIndex, openedTab);
			int num2 = Mathf.Max(lastSelectedTabIndex, openedTab);
			if (i >= num && i <= num2)
			{
				if (lastSelectedTabIndex < i && openedTab >= i)
				{
					component.PlayGroup(12, lastSelectedTabIndex == -1);
				}
				if (lastSelectedTabIndex >= i && openedTab < i)
				{
					component.PlayGroup(13, lastSelectedTabIndex == -1);
				}
			}
		}
	}

	private void SetupTradeItemsData()
	{
		currentTradeList = new List<TradeSlotInfo>();
		foreach (TradeSlotInfo currentTradeSlot in GameManager.Instance.playerModel.CurrentTradeSlots)
		{
			if ((!currentTradeSlot.CurrentTradeDefinition.HasDateLimit || !currentTradeSlot.Bought) && (!(currentTradeSlot.CurrentTradeDefinition.SoldItems.RewardsList[0] is RewardOutfit rewardOutfit) || !GameManager.Instance.playerModel.SurvivorContainer.HasOutfit(rewardOutfit.PreferredOrder[0])))
			{
				currentTradeList.Add(currentTradeSlot);
			}
		}
	}

	private void OnPlayerModelChanged(ModelObject m, string changed, object args)
	{
		if (changed == "TradeShopRefreshed" && tabButtons != null && tabButtons.GetSelectedIndex() == 3)
		{
			showResetEffectOnCards = true;
			UpdateSelectedTab();
		}
	}

	private void OnUiEvent(string type, object parameter)
	{
		if (scrollableList == null)
		{
			return;
		}
		switch (type)
		{
		case "OnCustomRequestShopUpdate":
			UpdateBundleToggleSet(0, 1, resetPosition: false);
			break;
		case "OnRequestShopUpdate":
		case "OnPurchaseInterrupted":
			UpdateSelectedTab();
			break;
		case "OnPopUpClose":
			if (parameter is ShopPopup)
			{
				if (tabButtons != null && tabButtons.GetSelectedIndex() != 3)
				{
					GameManager.Instance.BundleSource = Metrics.BundleSource.Shop;
					ShopPopupHelper.SendEndShopVisitAnalytics(currentBundleIapList, this);
				}
				Clear();
			}
			break;
		case "SendEndShopVisitAnalytics":
			if (parameter is ShopItemCard shopItemCard && currentBundleIapList != null && currentBundleIapList.Contains(shopItemCard.GetData()))
			{
				GameManager.Instance.BundleSource = Metrics.BundleSource.Shop;
				ShopPopupHelper.SendEndShopVisitAnalytics(currentBundleIapList, this);
			}
			break;
		case "NewShopTabChanagedEvent":
		case "NewShopToggleChanagedEvent":
		case "NewShopFilterChanagedEvent":
			UINewShopMain?.ResetItemSelectedInfo();
			break;
		}
	}

	public void SetLastButtonClicked(NUIListItemBase buttonClicked)
	{
		IndexOfLastItemClicked = scrollableList.currentItemsList.IndexOf(buttonClicked);
	}

	public void InitUIBundleClass()
	{
		if (BundleClassScrollView != null)
		{
			BundleClassScrollView.ResetPosition();
			for (BundleClassification bundleClassification = BundleClassification.All; bundleClassification < BundleClassification.Max; bundleClassification++)
			{
				Helpers.InstantiateToParent(BundleClassPrefab, BundleClassScrollView.gameObject).GetComponent<BundleClassItem>().SetKey(this, bundleClassification);
			}
			BundleClassScrollView.ResetPosition();
			BundleClassTable.repositionNow = true;
		}
	}

	public void SetBundleClassFilter(BundleClassification newClass)
	{
		int selectedIndex = tabButtons.GetSelectedIndex();
		currentBundleClassification = newClass;
		UpdateBundleClassFilter(selectedIndex, currentToggleIndex, newClass, resetPosition: true);
	}

	public void UpdateBundleClassFilter(int newSelectedTabIndex, int newToggleIndex, BundleClassification newClass, bool resetPosition)
	{
		CampHUD.SetTopLeftContainerVisibility(visibility: false);
		Helpers.GameObjectSetActive(BundleClassContainer, value: true);
		Helpers.GameObjectSetActive(BundleClassScrollContainer, value: false);
		currentBundleClassTxt.text = LocalizationManager.GetText("BundleClass." + newClass);
		if (CurrentClassification != newClass)
		{
			UIEvent.Send("NewShopFilterChanagedEvent", new object[2] { CurrentClassification, newClass });
		}
		switch (newSelectedTabIndex)
		{
		case 1:
		{
			List<TradefairBundleStoreDefinition> orderedAvailableBundles = GameManager.Instance.playerModel.TradefairManager.GetOrderedAvailableBundles();
			if (Helpers.GetShopRoleType() == ShopRoleType.IOS)
			{
				orderedAvailableBundles.RemoveAll(delegate(TradefairBundleStoreDefinition bundle)
				{
					string bundleIdentifier3 = bundle.BundleIdentifier;
					return GameManager.Instance.gameEconomyData.GetTradefairBundleContentDefinition(bundleIdentifier3)?.PayBanana ?? false;
				});
			}
			else
			{
				orderedAvailableBundles.RemoveAll((TradefairBundleStoreDefinition item) => item.BundleIdentifier == "TWD_BOX_OF_FAIRCOINS");
				if (!GameManager.Instance.gameEconomyData.ConfigData.PayBananaSwitch)
				{
					orderedAvailableBundles.RemoveAll(delegate(TradefairBundleStoreDefinition bundle)
					{
						string bundleIdentifier3 = bundle.BundleIdentifier;
						TradefairBundleContentDefinition tradefairBundleContentDefinition2 = GameManager.Instance.gameEconomyData.GetTradefairBundleContentDefinition(bundleIdentifier3);
						return tradefairBundleContentDefinition2 != null && tradefairBundleContentDefinition2.PayBanana && tradefairBundleContentDefinition2.HideCoinPurchase;
					});
				}
			}
			orderedAvailableBundles.RemoveAll(delegate(TradefairBundleStoreDefinition x)
			{
				if (!string.IsNullOrEmpty(x.BundleIdentifier))
				{
					TradefairBundleContentDefinition tradefairBundleContentDefinition2 = GameManager.Instance.gameEconomyData.GetTradefairBundleContentDefinition(x.BundleIdentifier);
					if (tradefairBundleContentDefinition2 != null && tradefairBundleContentDefinition2.IsThirdParty == ThirdPartyName.banana)
					{
						return true;
					}
				}
				return false;
			});
			if (newClass == BundleClassification.All)
			{
				ShopPopupHelper.UpdateListWithData(scrollableList, orderedAvailableBundles, resetPosition, isTabsIndexFeaturedShop: true);
				UINewShopMain.UpdateUI(newClass, orderedAvailableBundles, null, null);
				break;
			}
			List<TradefairBundleStoreDefinition> list2 = new List<TradefairBundleStoreDefinition>();
			for (int num2 = 0; num2 < orderedAvailableBundles.Count; num2++)
			{
				string bundleIdentifier2 = orderedAvailableBundles[num2].BundleIdentifier;
				TradefairBundleStoreDefinition bundleTradefairDefinition = GameManager.Instance.gameEconomyData.GetBundleTradefairDefinition(bundleIdentifier2);
				TradefairBundleContentDefinition tradefairBundleContentDefinition = GameManager.Instance.gameEconomyData.GetTradefairBundleContentDefinition(bundleIdentifier2);
				if (tradefairBundleContentDefinition != null && tradefairBundleContentDefinition.Classification == newClass)
				{
					list2.Add(bundleTradefairDefinition);
				}
			}
			ShopPopupHelper.UpdateListWithData(scrollableList, list2, resetPosition, isTabsIndexFeaturedShop: true);
			UINewShopMain.UpdateUI(newClass, list2, null, null);
			break;
		}
		case 0:
		{
			currentBundleIapList = GameManager.Instance.playerModel.BundleManager.GetOrderedAvailableBundlesWithShopTabIndex(0);
			currentBundleIapList.RemoveAll(delegate(BundleStoreDefinition x)
			{
				if (!string.IsNullOrEmpty(x.BundleIdentifier))
				{
					BundleContentDefinition bundleContentDefinition2 = GameManager.Instance.gameEconomyData.GetBundleContentDefinition(x.BundleIdentifier);
					if (bundleContentDefinition2 != null && bundleContentDefinition2.IsThirdParty == ThirdPartyName.banana)
					{
						return true;
					}
				}
				return false;
			});
			bool bundleButtonSwitch = Helpers.GetBundleButtonSwitch();
			if (newClass == BundleClassification.All)
			{
				ShopPopupHelper.UpdateListWithData(scrollableList, currentBundleIapList, resetPosition, bundleButtonSwitch);
				UINewShopMain.UpdateUI(newClass, null, currentBundleIapList, null);
				break;
			}
			List<BundleStoreDefinition> list = new List<BundleStoreDefinition>();
			for (int num = 0; num < currentBundleIapList.Count; num++)
			{
				string bundleIdentifier = currentBundleIapList[num].BundleIdentifier;
				BundleStoreDefinition bundleStoreDefinition = GameManager.Instance.gameEconomyData.GetBundleStoreDefinition(bundleIdentifier);
				BundleContentDefinition bundleContentDefinition = GameManager.Instance.gameEconomyData.GetBundleContentDefinition(bundleIdentifier);
				if (bundleContentDefinition != null && bundleContentDefinition.Classification == newClass)
				{
					list.Add(bundleStoreDefinition);
				}
			}
			ShopPopupHelper.UpdateListWithData(scrollableList, list, resetPosition, bundleButtonSwitch);
			UINewShopMain.UpdateUI(newClass, null, list, null);
			break;
		}
		}
		CurrentClassification = newClass;
	}

	public void ToggleBundleScroll()
	{
		Helpers.GameObjectSetActive(BundleClassScrollContainer, !BundleClassScrollContainer.activeSelf);
		BundleClassScrollView.ResetPosition();
	}

	public void InitToggleSet()
	{
		toggleSet.SetChangeCallback(OnToggleChange);
	}

	private void OnToggleChange(UIButtonExtended toggleButton)
	{
		int result = -1;
		int.TryParse(toggleButton.id, out result);
		result = ((result > 0) ? 1 : 0);
		if (currentToggleIndex != result)
		{
			int selectedIndex = tabButtons.GetSelectedIndex();
			currentBundleClassification = BundleClassification.All;
			UpdateToggleSet(selectedIndex, result, currentBundleClassification, resetPosition: true);
		}
	}

	public void UpdateToggleSet(int newSelectedTabIndex, int newToggleIndex, BundleClassification newClass, bool resetPosition)
	{
		CampHUD.SetBlackMarketHudCurrencyVisibility(visibility: false);
		CampHUD.SetHillTopCoinHudCurrencyVisibility(visibility: false);
		CampHUD.SetTradeFairHudCurrencyVisibility(visibility: false);
		CampHUD.SetBluePrintHudCurrencyVisibility(visibility: false);
		Helpers.GameObjectSetActive(toggleSetContainer, value: true);
		if (newSelectedTabIndex == 1)
		{
			ShowMainContent(newSelectedTabIndex, newToggleIndex);
			if (currentToggleIndex != newToggleIndex)
			{
				UIEvent.Send("NewShopToggleChanagedEvent", new object[2] { currentToggleIndex, newToggleIndex });
			}
			switch (newToggleIndex)
			{
			case 0:
				CampHUD.SetTradeFairHudCurrencyVisibility(visibility: true);
				CampHUD.SetHillTopCoinHudCurrencyVisibility(visibility: false);
				XShopLable.text = LocalizationManager.GetText("Popup.Shop.Tab.TradeFair");
				UpdateBundleClassFilter(newSelectedTabIndex, newToggleIndex, newClass, resetPosition);
				break;
			case 1:
			{
				XShopLable.text = LocalizationManager.GetText("Shop.HillTopStore.Title");
				CampHUD.SetTopLeftContainerVisibility(visibility: true);
				Helpers.GameObjectSetActive(BundleClassContainer, value: false);
				Helpers.GameObjectSetActive(BundleClassScrollContainer, value: false);
				CampHUD.SetTradeFairHudCurrencyVisibility(visibility: false);
				CampHUD.SetHillTopCoinHudCurrencyVisibility(visibility: true);
				Helpers.ExecuteCommand(new UpdateHillTopStoreCommand());
				List<HillTopStoreSlot> dataList = GameManager.Instance.playerModel.HillTopStore.Slots.ToList();
				ShopPopupHelper.UpdateListWithData(scrollableList, dataList, resetPosition, isTabsIndexFeaturedShop: false);
				break;
			}
			}
			currentToggleIndex = newToggleIndex;
		}
	}

	public void SetToggleSetVisibility(bool show)
	{
		Helpers.GameObjectSetActive(toggleSetContainer, show);
	}

	private void InitBundleToggleSet()
	{
		bundleToggleSet.SetChangeCallback(OnBundleToggleChange);
	}

	private void OnBundleToggleChange(UIButtonExtended toggleButton)
	{
		int result = -1;
		int.TryParse(toggleButton.id, out result);
		result = ((result > 0) ? 1 : 0);
		if (currentBundleToggleIndex != result)
		{
			int selectedIndex = tabButtons.GetSelectedIndex();
			UpdateBundleToggleSet(selectedIndex, result, resetPosition: true);
		}
	}

	public void UpdateBundleToggleSet(int newSelectedTabIndex, int newToggleIndex, bool resetPosition)
	{
		Helpers.GameObjectSetActive(bundleToggleSetContainer, GameManager.Instance.gameEconomyData.ConfigData.CustomBundleSwitch && GameManager.Instance.playerModel.CustomizedBundleManager.IsCouncilLevelValid);
		Helpers.GameObjectSetActive(newMainContent, value: false);
		Helpers.GameObjectSetActive(scrollableList, value: false);
		Helpers.GameObjectSetActive(BundleClassContainer, value: false);
		if (newSelectedTabIndex != 0)
		{
			return;
		}
		switch (newToggleIndex)
		{
		case 0:
			CampHUD.SetBluePrintHudCurrencyVisibility(visibility: false);
			ShowMainContent();
			UpdateBundleClassFilter(newSelectedTabIndex, 0, currentBundleClassification, resetPosition);
			break;
		case 1:
		{
			CampHUD.SetBluePrintHudCurrencyVisibility(visibility: true);
			Helpers.GameObjectSetActive(scrollableList, value: true);
			if (!resetPosition)
			{
				scrollableList.SaveCurrentScrollPosition();
			}
			List<CustomBundleDefinition> orderedAvailableBundles = GameManager.Instance.playerModel.CustomizedBundleManager.GetOrderedAvailableBundles();
			ShopPopupHelper.UpdateListWithData(scrollableList, orderedAvailableBundles, resetPosition, isTabsIndexFeaturedShop: false);
			break;
		}
		}
		currentBundleToggleIndex = newToggleIndex;
	}
}
