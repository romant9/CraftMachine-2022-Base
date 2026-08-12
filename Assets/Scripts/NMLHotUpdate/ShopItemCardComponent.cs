using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class ShopItemCardComponent : ShopCardBase<GoldShopDefinition>
{
	[SerializeField]
	private UIButtonWithLabel button;

	[SerializeField]
	private UILabel itemNameLabel;

	[SerializeField]
	private UISprite itemSprite;

	[Header("Bundle Items List")]
	[SerializeField]
	private NUIScrollableList scrollableList;

	[SerializeField]
	private UIButton infoButton;

	public const string defaultItemPrefabName = "Component_List_Item";

	public virtual void OnPoolReturn()
	{
		Clear();
	}

	public void OnInfoClicked()
	{
		GoldShopDefinition data = GetData();
		if (data == null)
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
			DropName = LocalizationManager.GetText("CraftingShopItem." + data.ItemId + ".Name"),
			Description = "TBD, Where we get component description",
			Probabilities = componentProbabilities
		};
		obj.TryOpenWithNormalData(dropTableItem);
	}

	public override void AddListeners()
	{
		base.AddListeners();
		if (button != null)
		{
			button.SetClickCallback(OnButtonClicked);
		}
	}

	public override void RemoveListeners()
	{
		base.RemoveListeners();
		if (button != null)
		{
			button.RemoveClickCallback(OnButtonClicked);
		}
	}

	public override void Clear()
	{
		base.Clear();
		if (button != null)
		{
			button.Clear();
		}
	}

	public override void SetData(GoldShopDefinition item)
	{
		base.SetData(item);
		UpdateUI();
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
		GoldShopDefinition data = GetData();
		if (data == null)
		{
			return;
		}
		string text = LocalizationManager.GetText("GoldShopItem." + data.ItemId + ".Name");
		HelpersUI.SetContentToLabel(itemNameLabel, text);
		if (!string.IsNullOrEmpty(data.ItemSpriteName))
		{
			HelpersUI.SetSprite(itemSprite, data.ItemSpriteName);
		}
		else
		{
			Helpers.GameObjectSetActive(itemSprite, value: false);
		}
		if (scrollableList != null)
		{
			scrollableList.Clear();
			NUIListItem<ComponentCrateItem> nUIListItem = null;
			for (int i = 0; i < data.SubItems.Count; i++)
			{
				ComponentCrateItem componentCrateItem = data.SubItems[i];
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
		if (button != null)
		{
			button.SetContentToLabelOne(data.Price.ToString());
		}
	}

	public void OnButtonClicked(UIButtonExtended button)
	{
		ShopPopup shopPopup = SingularityMonoBehaviour<HUDManager>.Instance.GetNoCreation(UIType.ShopPopup) as ShopPopup;
		if (shopPopup != null)
		{
			shopPopup.SetLastButtonClicked(this);
		}
		ConsumeCurrencyCommandUtils.Execute(new BuyGoldShopDefinitionCommand(GetData().ItemId)
		{
			Cashier = BuyGoldShopDefinitionCommand.GetCashierForItem(GetData(), GameManager.Instance.modelManager)
		}, OnBuyCommandCompleted);
	}

	private void OnBuyCommandCompleted(TWDModelResult result)
	{
		if (result != TWDModelResult.OK)
		{
			return;
		}
		IAPConfirmPopupNew iAPConfirmPopupNew = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.IAPConfirmPopupNew) as IAPConfirmPopupNew;
		iAPConfirmPopupNew.ShowShopWhenClosed = true;
		BundleContentDefinition bundleContentDefinition = new BundleContentDefinition();
		bundleContentDefinition.RewardEntries = new Rewards();
		List<CurrencyType> lastReceivedComponents = GameManager.Instance.playerModel.LootManager.LastReceivedComponents;
		if (lastReceivedComponents != null && lastReceivedComponents.Count > 0)
		{
			for (int i = 0; i < lastReceivedComponents.Count; i++)
			{
				bundleContentDefinition.RewardEntries.AddRewardCurrency(lastReceivedComponents[i], 1, isDiamondExchange: false, canOverflowMax: false);
			}
		}
		iAPConfirmPopupNew.OpenForBundleContentDefinition(null, bundleContentDefinition, givenBySupport: false);
	}
}
