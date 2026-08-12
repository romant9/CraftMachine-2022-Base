using BaseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using TwdCustomMod;
using TWDModel;
using UnityEngine;

public class ResidenceBadgeInventoryTab : MonoBehaviour
{
	public List<BadgeInfo> allBadges;

	public BadgeFilteringController BadgeFilteringController;

	private List<BadgeInfo> filteredBadges = new List<BadgeInfo>();

	[SerializeField]
	private ResidenceBadgeInventoryList inventoryList;

	[SerializeField]
	private UILabel labelTotalAmounts;

	[SerializeField]
	private UILabel labelTotalAmountsFiltered;

	private BadgeScrapController badgeScrapController;

	private void Awake()
	{
		badgeScrapController = GetComponent<BadgeScrapController>();
	}

	private void OnEnable()
	{
		BadgeFilteringController badgeFilteringController = BadgeFilteringController;
		badgeFilteringController.OnBadgesUpdated = (Action<List<BadgeInfo>>)Delegate.Combine(badgeFilteringController.OnBadgesUpdated, new Action<List<BadgeInfo>>(OnBadgesFilterUpdated));
		allBadges = IsLoadDataManager ? DataManager.Instance.PlayerBadges : BadgeUtils.GetAllBadges();
		UIEvent.OnUIEvent += OnUIEvent;
		UpdateTotalBadgesLabel();

        traitRerollMeter.gameObject.SetActive(true);
		CraftSettings.Instance.CurrencyMeterContainer.Reposition();

        int currencyAmount = GameManager.Instance.playerModel.GetCurrencyAmount(CurrencyType.TraitRerollToken);
        traitRerollMeter.SetValue(currencyAmount);
    }

    private void OnUIEvent(string type, object parameter)
	{
		switch (type)
		{
		case "OnBadgeScraped":
		case "OnBadgeEquipped":
		case "OnBadgeUnequipped":
		case "OnBadgeRerolled":
			ForceUpdate();
			break;
		}
	}

	private void OnBadgesFilterUpdated(List<BadgeInfo> obj)
	{
		IsLateUpdate = false;

		filteredBadges = obj;
		filteredBadges.Sort(BadgeUtils.BadgesSortAlgorithm);
		UpdateInventory();
	}

	public void UpdateInventory()
	{
		List<BadgeInfo> list = allBadges.Intersect(filteredBadges).ToList();
		list.Sort(BadgeUtils.BadgesSortAlgorithm);
		HelpersUI.SetContentToLabel(labelTotalAmountsFiltered, list.Count.ToString());
		inventoryList.UpdateWithList(list, "BadgeCard", "BadgeCardEmpty", callUpdateUI: true);
		if (inventoryList.currentItemsCount > 0)
		{
			inventoryList.Sort();
			inventoryList.RepositionItemsFillDownwards();
			inventoryList.ResetScrollPosition();
			SetInventoryIconInfos();
		}
	}

	private void OnDisable()
	{
		UIEvent.OnUIEvent -= OnUIEvent;
		BadgeFilteringController badgeFilteringController = BadgeFilteringController;
		badgeFilteringController.OnBadgesUpdated = (Action<List<BadgeInfo>>)Delegate.Remove(badgeFilteringController.OnBadgesUpdated, new Action<List<BadgeInfo>>(OnBadgesFilterUpdated));
		if (IsLoadDataManager)
		{
			DebugTWD.LogMycode("if (IsLoadDataManager) return");
			DebugTWD.Log("Inventory was Disable : " + allBadges.Count);
            traitRerollMeter.gameObject.SetActive(false);
            CraftSettings.Instance.CurrencyMeterContainer.Reposition();
            return;
		}
		if (inventoryList != null)
		{
			inventoryList.Clear();
		}
		filteredBadges.Clear();
		allBadges.Clear();
    }

    private void LateUpdate()
	{
		if (!(inventoryList == null))
		{
			if (IsLoadDataManager)
			{
				//DebugTWD.LogMycode("if (IsLoadDataManager)");
				if (!IsLateUpdate || inventoryList.uiScrollView.IsMooving)
				{
					IsLateUpdate = true;
					inventoryList.UpdateVisibleItems<BadgeInfo>("BadgeCard");
					SetInventoryIconInfos();
				}
			}
			else
			{
				inventoryList.UpdateVisibleItems<BadgeInfo>("BadgeCard");
				SetInventoryIconInfos();
			}
		}
	}

	private void SetInventoryIconInfos()
	{
		for (int i = 0; i < inventoryList.currentItemsCount; i++)
		{
			SurvivorBadgesIcon survivorBadgesIcon = inventoryList.currentItemsList[i] as SurvivorBadgesIcon;
			if (!(survivorBadgesIcon == null) && survivorBadgesIcon.GetData() != null && !string.IsNullOrEmpty(survivorBadgesIcon.GetData().ModelId))
			{
				survivorBadgesIcon.SetId(survivorBadgesIcon.GetData().ModelId);
				survivorBadgesIcon.SetClickCallbacks(OnItemClicked, OnItemClicked);
			}
		}
	}

	private void OnItemClicked(UIButtonExtended button)
	{
		BadgeInfo badgeInfo = IsLoadDataManager ? button.GetComponent<SurvivorBadgesIcon>().GetData() : allBadges.First((BadgeInfo x) => x.Model.ModelId.ToString() == button.id);
		if (badgeInfo == null)
		{
			return;
		}
		if (badgeScrapController.ScrapModeActive)
		{
			badgeScrapController.OnBadgeClicked(badgeInfo);
		}
		else
		{
			if (IsLoadDataManager)
			{
				DebugTWD.LogMycode("if (IsLoadDataManager)");
				BadgeDetailsPopup badgeDetailsPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.BadgeDetailsPopup, HUDManager.Instance.UIContainer) as BadgeDetailsPopup;
				if (badgeDetailsPopup == null) return;

				BadgeCraft.Instance.modelRandomReroll = new ModelRandom(BadgeCraft.Instance.modelRandomLast);
				badgeDetailsPopup.SetData(this);
				badgeDetailsPopup.OpenForModel(badgeInfo.Model);
				BadgeCraft.Instance.SetOriginBadgeData(badgeInfo.Model);
				DebugTWD.Log("Change random to Reroll " + BadgeCraft.Instance.modelRandomLast.State);
			}
			else
			{
				HUDElement hUDElement = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.BadgeDetailsPopup);
				if (hUDElement == null)
				{
					return;
				}
				hUDElement.OpenForModel(badgeInfo.Model);
			}
		}
		inventoryList.UpdateUIICurrentItems();
	}

	public void ForceUpdate()
	{
		allBadges = IsLoadDataManager ? DataManager.Instance.PlayerBadges : BadgeUtils.GetAllBadges();
		BadgeFilteringController.ForceUpdate();
		UpdateTotalBadgesLabel();
	}

	private void UpdateTotalBadgesLabel()
	{
		int maximumBadgeCount = GameManager.Instance.playerModel.SurvivorContainer.MaximumBadgeCount;
		HelpersUI.SetContentToLabel(labelTotalAmounts, allBadges.Count + " / " + maximumBadgeCount);
	}



	#region myparams
	public bool IsLateUpdate;
	public SurvivorBadgesIcon BadgePrefab;
	public BadgeRerollPopupCustom BadgeRerollPopupList;
	[Header("Trait reroll meter")]
	public HUDMeter traitRerollMeter;
	public UILabel rerollExtraAmountLabel;
	private bool IsLoadDataManager => OfflineManager.IsLoadDataManager;
	#endregion

	#region mycode
	public void SetBadgesList()
	{
		allBadges = DataManager.Instance.PlayerBadges;
		DebugTWD.Log("AllBadges Count : " + allBadges.Count);
	}
	public void SetCurrencyAmount()
	{
		int callExtraAmount;
		try
		{
			callExtraAmount = Convert.ToInt32(rerollExtraAmountLabel.text.ToString());
		}
		catch
		{
			callExtraAmount = 0;
		}
		GameManager.Instance.playerModel.SetCurrency(CurrencyType.TraitRerollToken, callExtraAmount);
		int currencyAmount = GameManager.Instance.playerModel.GetCurrencyAmount(CurrencyType.TraitRerollToken);
		traitRerollMeter.SetValue(currencyAmount);
	}

	public void SetCurrency()
	{
		int currencyAmount = GameManager.Instance.playerModel.GetCurrencyAmount(CurrencyType.TraitRerollToken);
		DebugTWD.Log("CurrencyAmount of " + CurrencyType.TraitRerollToken.ToString() + " : " + currencyAmount);
		traitRerollMeter.SetCurrencyType(CurrencyType.TraitRerollToken);
		traitRerollMeter.SetValue(currencyAmount);
	}
	#endregion
}
