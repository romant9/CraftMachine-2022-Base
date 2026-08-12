using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class ShopGoldDetail : MonoBehaviour
{
	[SerializeField]
	private UIButtonWithLabel button;

	[SerializeField]
	private UILabel itemNameLabel;

	[SerializeField]
	private UILabel LimitLabel;

	[SerializeField]
	private UILabel NumsTxt;

	[SerializeField]
	private UILabel valueBadge;

	[SerializeField]
	private GameObject scrollContainer;

	[SerializeField]
	private NUIScrollableList scrollableList;

	[SerializeField]
	private GameObject descriptionScrollContainer;

	[SerializeField]
	private UILabel DescriptionLable;

	[SerializeField]
	private UILabel priceTxt;

	[SerializeField]
	private UISprite priceIconSprite;

	[SerializeField]
	private GameObject MainContent;

	[Header("Tween Groups")]
	[SerializeField]
	private int ItemBoughtTweenGroup = 4;

	[SerializeField]
	private int ItemBoughtAndRemovedTweenGroup = 5;

	private GoldShopDefinition bindData;

	[SerializeField]
	private UILabel timeLeftTxt;

	[SerializeField]
	private GameObject timeContainer;

	public const string defaultItemPrefabName = "Bundle_List_Item";

	public const string defaultEquipmentPrefabName = "Bundle_List_Equipment";

	public const string defaultConsumablePrefabName = "Bundle_List_Consumable";

	public const string defaultComponentPrefabName = "Component_List_Item";

	private void OnEnable()
	{
		Helpers.GameObjectSetActive(timeContainer, value: false);
		SingularityMonoBehaviour<LocalizationManager>.Instance.OnLocalizationLanguageChanged += OnLocalizationLanguageChanged;
	}

	private void OnDisable()
	{
		SingularityMonoBehaviour<LocalizationManager>.Instance.OnLocalizationLanguageChanged -= OnLocalizationLanguageChanged;
	}

	public void Update()
	{
		if (bindData != null && !string.IsNullOrEmpty(bindData.ItemId))
		{
			LimitedBundleData initiatedLimitedBundle = GameManager.Instance.playerModel.GoldShopDefinitionManager.GetInitiatedLimitedBundle(bindData.ItemId);
			bool showTimerInCard = bindData.ShowTimerInCard;
			Helpers.GameObjectSetActive(timeContainer, value: false);
			if (timeLeftTxt != null && showTimerInCard && initiatedLimitedBundle != null)
			{
				Helpers.GameObjectSetActive(timeContainer, value: true);
				HelpersUI.SetContentToLabel(timeLeftTxt, Helpers.FormatTimeNoZero(initiatedLimitedBundle.Timer));
			}
		}
	}

	public void UpdateUI(GoldShopDefinition newSelect)
	{
		bindData = newSelect;
		string text = "";
		HelpersUI.SetContentToLabel(content: string.IsNullOrEmpty(newSelect.OverrideTitleLocalization) ? LocalizationManager.GetText("GoldShopItem." + newSelect.ItemId + ".Name") : LocalizationManager.GetText("IAPCard.ItemName." + newSelect.OverrideTitleLocalization), label: itemNameLabel);
		if (newSelect.IsNewVersion)
		{
			SetDescriptionUI();
		}
		if (!string.IsNullOrEmpty(bindData.ValueBadgeLocalisation))
		{
			HelpersUI.SetContentToLabel(valueBadge, LocalizationManager.GetText(bindData.ValueBadgeLocalisation));
		}
		else
		{
			Helpers.GameObjectSetActive(valueBadge, value: false);
		}
		Helpers.GameObjectSetActive(NumsTxt, value: false);
		if (bindData.RewardEntries != null)
		{
			int numsForIReward = Helpers.GetNumsForIReward(bindData.RewardEntries.RewardsList[0]);
			if (numsForIReward > 0)
			{
				Helpers.GameObjectSetActive(NumsTxt, value: true);
				HelpersUI.SetContentToLabel(NumsTxt, "x" + numsForIReward);
			}
		}
		Helpers.GameObjectSetActive(scrollContainer, value: false);
		Helpers.GameObjectSetActive(descriptionScrollContainer, value: false);
		if (priceTxt != null)
		{
			int price = bindData.Price;
			string text2 = (((float)price > 0f) ? (price.ToString() ?? "") : LocalizationManager.GetText("Generic.Free"));
			priceTxt.text = text2;
			priceIconSprite.spriteName = HelpersGfx.GetCurrencyIconName(CurrencyType.Diamonds);
			Helpers.GameObjectSetActive(priceIconSprite, price > 0);
		}
		if (newSelect.IsSingleReward)
		{
			Helpers.GameObjectSetActive(descriptionScrollContainer, value: true);
		}
		else
		{
			Helpers.GameObjectSetActive(scrollContainer, value: true);
			UpdateScrollableList();
		}
		SetBuyLimitStatus();
	}

	private void UpdateScrollableList()
	{
		if (bindData.IsNewVersion)
		{
			SetScrollNewVersion();
		}
		else
		{
			SetScrollOldVersion();
		}
	}

	public void OnInfoClicked()
	{
		GoldShopDefinition goldShopDefinition = bindData;
		if (goldShopDefinition == null)
		{
			return;
		}
		int buildingLevel = GameManager.Instance.modelManager.Player.Camp.GetBuildingLevel("Scavenger");
		List<ItemAmountProbabilityData> componentProbabilities = GameManager.Instance.gameEconomyData.GetComponentProbabilities(buildingLevel, DropEventDefinition.DropEventTag.ComponentCrate, GameManager.Instance.playerModel.ActivityManager);
		for (int i = 0; i < componentProbabilities.Count; i++)
		{
			if (componentProbabilities[i].ItemEnumType == typeof(CurrencyType))
			{
				componentProbabilities[i].Name = HelpersLocalization.GetComponentName((CurrencyType)componentProbabilities[i].ItemEnumValue);
			}
		}
		DropRatesInfoPopup obj = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.DropRatesInfoPopup) as DropRatesInfoPopup;
		DropTableItem dropTableItem = new DropTableItem
		{
			DropName = LocalizationManager.GetText("GoldShopItem." + goldShopDefinition.ItemId + ".Name"),
			Description = "TBD, Where we get component description",
			Probabilities = componentProbabilities
		};
		obj.TryOpenWithNormalData(dropTableItem);
	}

	public void OnButtonClicked()
	{
		ConsumeCurrencyCommandUtils.Execute(new BuyGoldShopDefinitionCommand(bindData.ItemId)
		{
			Cashier = BuyGoldShopDefinitionCommand.GetCashierForItem(bindData, GameManager.Instance.modelManager)
		}, OnBuyCommandCompleted);
	}

	private void OnBuyCommandCompleted(TWDModelResult result)
	{
		if (result == TWDModelResult.OK)
		{
			IAPConfirmPopupNew obj = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.IAPConfirmPopupNew) as IAPConfirmPopupNew;
			obj.ShowShopWhenClosed = true;
			obj.OpenForRewards(new BundleContentDefinition
			{
				RewardEntries = GameManager.Instance.playerModel.GoldShopDefinitionManager.LastReceivedComponents
			}.RewardEntries.RewardsList);
			obj.SetCloseAnimOverCallback(AfterBuysucceeded);
		}
	}

	private void AfterBuysucceeded()
	{
		UpdateUI(bindData);
	}

	private void SetDescriptionUI()
	{
		if (bindData.RewardEntries == null || bindData.RewardEntries.RewardsList.Count <= 0)
		{
			return;
		}
		string text = "";
		if (!string.IsNullOrEmpty(bindData.DescriptionLocalization))
		{
			text = LocalizationManager.GetText(bindData.DescriptionLocalization);
		}
		else
		{
			IReward reward = bindData.RewardEntries.RewardsList[0];
			if (reward is RewardEquipment rewardEquipment)
			{
				EquipmentDefinition equipmentDefinition = rewardEquipment.EquipmentDefinition(GameManager.Instance.modelManager);
				if (equipmentDefinition.Category != EquipmentCategory.Armor || string.IsNullOrEmpty(equipmentDefinition.SpecialTrait))
				{
					text = ((equipmentDefinition.Category != EquipmentCategory.Utility) ? HelpersLocalization.GetEquipmentSpecialDescription(equipmentDefinition) : HelpersLocalization.GetShopTooltipForIReward(reward));
				}
				else
				{
					TraitDefinition traitDefinition = GameManager.Instance.gameEconomyData.GetTraitDefinition(equipmentDefinition.SpecialTrait);
					text = ((traitDefinition == null) ? LocalizationManager.GetText(equipmentDefinition.SpecialTrait) : HelpersLocalization.GetTraitDescription(traitDefinition));
				}
			}
			else
			{
				text = HelpersLocalization.GetShopTooltipForIReward(reward);
			}
		}
		HelpersUI.SetContentToLabel(DescriptionLable, text);
	}

	private void SetBuyLimitStatus()
	{
		int maxPurchases = bindData.MaxPurchases;
		bool showMaxPurchases = bindData.ShowMaxPurchases;
		string itemId = bindData.ItemId;
		int num = 0;
		GoldShopDefinitionManagerModel goldShopDefinitionManager = GameManager.Instance.playerModel.GoldShopDefinitionManager;
		bool flag = goldShopDefinitionManager.CanBuyBundle(bindData);
		if (goldShopDefinitionManager.BoughtBundlesAmount != null && goldShopDefinitionManager.BoughtBundlesAmount.ContainsKey(itemId))
		{
			num = goldShopDefinitionManager.BoughtBundlesAmount[itemId];
		}
		Helpers.GameObjectSetActive(LimitLabel, value: false);
		if (showMaxPurchases && maxPurchases > 0)
		{
			Helpers.GameObjectSetActive(LimitLabel, value: true);
			LimitLabel.text = LocalizationManager.GetText("ShopUI.DetailPage.PurchaseLimit", maxPurchases - num, maxPurchases);
		}
		if (flag)
		{
			TweenManager.ResetToBeginningTweenGroup(base.gameObject, ItemBoughtAndRemovedTweenGroup);
		}
		else
		{
			TweenManager.PlayTweenGroup(base.gameObject, ItemBoughtAndRemovedTweenGroup, forward: true, OnCompleteSoldOut);
		}
	}

	public void OnCompleteSoldOut()
	{
		Helpers.GameObjectSetActive(this, value: false);
	}

	private void SetScrollOldVersion()
	{
		GoldShopDefinition goldShopDefinition = bindData;
		if (goldShopDefinition == null || goldShopDefinition.SubItems == null || !(scrollableList != null))
		{
			return;
		}
		scrollableList.Clear();
		NUIListItem<ComponentCrateItem> nUIListItem = null;
		for (int i = 0; i < goldShopDefinition.SubItems.Count; i++)
		{
			ComponentCrateItem componentCrateItem = goldShopDefinition.SubItems[i];
			if (componentCrateItem != null)
			{
				nUIListItem = scrollableList.InstantiateAdd("Component_List_Item") as NUIListItem<ComponentCrateItem>;
				if (nUIListItem != null)
				{
					nUIListItem.SetData(componentCrateItem);
				}
			}
		}
		scrollableList.SortAndReset();
		for (int j = 0; j < scrollableList.currentItemsList.Count; j++)
		{
			NestedUIDragScrollView nestedUIDragScrollView = Helpers.AddComponent<NestedUIDragScrollView>(scrollableList.currentItemsList[j].gameObject);
			if (nestedUIDragScrollView != null)
			{
				nestedUIDragScrollView.target = GetComponent<UIDragScrollView>();
			}
		}
	}

	private void SetScrollNewVersion()
	{
		GoldShopDefinition goldShopDefinition = bindData;
		if (goldShopDefinition == null || goldShopDefinition.RewardEntries == null || !(scrollableList != null))
		{
			return;
		}
		scrollableList.Clear();
		NUIListItem<IReward> nUIListItem = null;
		for (int i = 0; i < goldShopDefinition.RewardEntries.RewardsList.Count; i++)
		{
			IReward reward = goldShopDefinition.RewardEntries.RewardsList[i];
			if (reward == null)
			{
				continue;
			}
			if (reward.Type == RewardType.Equipment || reward.Type == RewardType.RandomEquipment || reward.Type == RewardType.EquipToken)
			{
				if (reward.Type == RewardType.Equipment)
				{
					RewardEquipment obj = reward as RewardEquipment;
					if (obj != null && obj.IsConsumableReward(GameManager.Instance.modelManager))
					{
						nUIListItem = scrollableList.InstantiateAdd("Bundle_List_Consumable") as NUIListItem<IReward>;
						goto IL_00de;
					}
				}
				nUIListItem = scrollableList.InstantiateAdd("Bundle_List_Equipment") as NUIListItem<IReward>;
			}
			else
			{
				nUIListItem = scrollableList.InstantiateAdd("Bundle_List_Item") as NUIListItem<IReward>;
			}
			goto IL_00de;
			IL_00de:
			if (nUIListItem != null)
			{
				nUIListItem.SetData(reward);
			}
		}
		scrollableList.SortAndReset();
		for (int j = 0; j < scrollableList.currentItemsList.Count; j++)
		{
			NestedUIDragScrollView nestedUIDragScrollView = Helpers.AddComponent<NestedUIDragScrollView>(scrollableList.currentItemsList[j].gameObject);
			if (nestedUIDragScrollView != null)
			{
				nestedUIDragScrollView.target = GetComponent<UIDragScrollView>();
			}
		}
	}

	private void OnLocalizationLanguageChanged(string newLanguage)
	{
		UpdateUI(bindData);
	}
}
