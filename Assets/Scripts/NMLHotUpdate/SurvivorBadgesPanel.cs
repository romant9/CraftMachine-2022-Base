using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TwdCustomMod;
using TWDModel;
using UnityEngine;
using UnityEngine.Serialization;

public class SurvivorBadgesPanel : MonoBehaviourExtended
{
	[SerializeField]
	public SurvivorBadgesIcon[] badgesIconsArray;

	public UIButtonExtended craftBadgesButton;

	public GameObject setBonusContainer;

	public GameObject lockedContainer;

	[SerializeField]
	public GameObject[] hiddenGameObjectsWhenLocked;

	public UILabel lockedMessageLabel;

	public GameObject bonusSetActivationEffect;

	public SurvivorCardBadgeElement miniatureBadges;

	public SurvivorBadgeSetsPanel badgeSetsPanel;

	public SurvivorDamageHealthPanel damagePanel;

	public SurvivorDamageHealthPanel healthPanel;

	public Dictionary<string, BadgeInfo> allBadgesDictionary;

	private List<BadgeInfo> filteredBadges = new List<BadgeInfo>();

	[FormerlySerializedAs("BadgeFilteringController")]
	[SerializeField]
	private BadgeFilteringController badgeFilteringController;

	[SerializeField]
	private NUIScrollableList inventoryList;

	[SerializeField]
	private UILabel labelTotalAmountsFiltered;

	private int selectedBadgePositionFilter;

	[FormerlySerializedAs("showEquipped")]
	[SerializeField]
	private UIToggle showEquippedfilter;

	private SurvivorModel currentSurvivorModel;

	[SerializeField]
	private GameObject noBadgesAvailable;

	[SerializeField]
	private float delayToShowBadgesWhenChangingSurvivors = 1.1f;

	private IEnumerator uselessVarToRunCoroutine;

	private void OnEnable()
	{
		BadgeFilteringController obj = badgeFilteringController;
		obj.OnBadgesUpdated = (Action<List<BadgeInfo>>)Delegate.Combine(obj.OnBadgesUpdated, new Action<List<BadgeInfo>>(OnBadgesFilterUpdated));
		EventDelegate.Add(showEquippedfilter.onChange, OnShowEquippedFilterEventHandler);
		UIEvent.OnUIEvent += OnUIEventEventHandler;
		allBadgesDictionary = BadgeUtils.GetAllBadgesAsDictionary();
		for (int i = 0; i < badgesIconsArray.Length; i++)
		{
			badgesIconsArray[i].SetId(i.ToString());
			badgesIconsArray[i].SetClickCallbacks(OnPositionClicked, OnPositionClicked);
		}
	}

	private void OnDisable()
	{
		BadgeFilteringController obj = badgeFilteringController;
		obj.OnBadgesUpdated = (Action<List<BadgeInfo>>)Delegate.Remove(obj.OnBadgesUpdated, new Action<List<BadgeInfo>>(OnBadgesFilterUpdated));
		EventDelegate.Remove(showEquippedfilter.onChange, OnShowEquippedFilterEventHandler);
		UIEvent.OnUIEvent -= OnUIEventEventHandler;
		if (inventoryList != null)
		{
			inventoryList.Clear();
		}
		filteredBadges.Clear();
		allBadgesDictionary.Clear();
	}

	private void OnUIEventEventHandler(string type, object parameter)
	{
		if (type == "OnBadgeEquipped" || type == "OnBadgeUnequipped")
		{
			allBadgesDictionary = BadgeUtils.GetAllBadgesAsDictionary();
			badgeFilteringController.ForceUpdate();
		}
	}

	private void OnShowEquippedFilterEventHandler()
	{
		badgeFilteringController.ForceUpdate();
	}

	private void OnPositionClicked(UIButtonExtended button)
	{
		selectedBadgePositionFilter = Mathf.Clamp(Convert.ToInt32(button.id), 0, badgesIconsArray.Length);
		UpdateSelectedIndicator();
		badgeFilteringController.ForceUpdate();
	}

	private void UpdateSelectedIndicator()
	{
		UIButtonExtended BadgeUnequipButton = null;
		UIButtonExtended BadgeDetailPopupButton = null;
		if (OfflineManager.IsLoadDataManager)
		{
			BadgeUnequipButton = DataManager.Instance.SurvivorManagementPopUp.SurvivorInfoPopupCurrent.BadgeUnequipButton;
			BadgeDetailPopupButton = DataManager.Instance.SurvivorManagementPopUp.SurvivorInfoPopupCurrent.BadgeDetailPopupButton;

			if (BadgeUnequipButton) BadgeUnequipButton.isEnabled = false;
			if (BadgeDetailPopupButton) BadgeDetailPopupButton.isEnabled = false;
		}

		for (int i = 0; i < badgesIconsArray.Length; i++)
		{
			badgesIconsArray[i].SetSelected(i == selectedBadgePositionFilter);
			if (!OfflineManager.IsLoadDataManager)
			{
				badgesIconsArray[i].ShowUnequippedButton(i == selectedBadgePositionFilter && badgesIconsArray[i].GetData()?.Model != null);
			}
			else
			{
				bool isUnequipped = badgesIconsArray[i].GetData()?.Model != null;

				badgesIconsArray[i].ShowUnequippedButton(false);

				if (i == selectedBadgePositionFilter && isUnequipped && BadgeUnequipButton)
				{
					DataManager.Instance.SurvivorManagementPopUp.SurvivorInfoPopupCurrent.SurvivorBadgeIndex = i;

					BadgeUnequipButton.isEnabled = true;
					BadgeDetailPopupButton.isEnabled = true;
				}
			}
		}
	}

	private void OnBadgesFilterUpdated(List<BadgeInfo> filteredBadgesReceived)
	{
		filteredBadges = filteredBadgesReceived.Where((BadgeInfo badge) => badge.Model.SlotIndex == selectedBadgePositionFilter && (showEquippedfilter.value || string.IsNullOrEmpty(badge.OwnerName))).ToList();
		filteredBadges.Sort(BadgeUtils.BadgesSortAlgorithm);
		UpdateMaxSimilarBadgesReached(filteredBadges);
		UpdateInventory();
	}

	private void UpdateMaxSimilarBadgesReached(List<BadgeInfo> badgesToUpdate)
	{
		int maxSimilarBadgeCount = GameManager.Instance.gameEconomyData.ConfigData.MaxSimilarBadgeCount;
		SurvivorBadgesIcon[] array = badgesIconsArray;
		BadgeModel excludingBadge = ((array == null) ? null : array[selectedBadgePositionFilter]?.GetData()?.Model);
		if (currentSurvivorModel == null)
		{
			return;
		}
		foreach (BadgeInfo item in badgesToUpdate)
		{
			int similarBadgeCount = currentSurvivorModel.BadgeContainer.GetSimilarBadgeCount(item.Model, excludingBadge);
			if (maxSimilarBadgeCount != 0 && similarBadgeCount >= maxSimilarBadgeCount)
			{
				item.MaxSimilarBadgesReached = true;
			}
		}
	}

	public void UpdateInventory()
	{
		HelpersUI.SetContentToLabel(labelTotalAmountsFiltered, filteredBadges.Count.ToString());
		inventoryList.UpdateWithList(filteredBadges, "BadgeCardSmall", "BadgeCardEmpty", callUpdateUI: true);
		noBadgesAvailable.SetActive(inventoryList.currentItemsCount == 0);
		if (inventoryList.currentItemsCount > 0)
		{
			inventoryList.Sort();
			inventoryList.RepositionItemsFillDownwards();
			inventoryList.ResetScrollPosition();
			SetInventoryIconInfos();
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
		if (!allBadgesDictionary.TryGetValue(button.id, out var badge) || currentSurvivorModel.BadgeContainer.Badges.Any((BadgeModel x) => x.ModelId == badge.Model.ModelId))
		{
			return;
		}
		if (!string.IsNullOrEmpty(badge.OwnerName))
		{
			if (!SingularityMonoBehaviour<HUDManager>.Instance.IsOpen(UIType.MapTeamSelection))
			{
				Callback okCallback = delegate
				{
					Helpers.StartCoroutine(GameManager.Instance, ShowBadgesFromOtherSurvivor(badge.Model.ModelId), ref uselessVarToRunCoroutine);
				};
				Callback cancelCallback = delegate
				{
				};
				ConfirmationPopup.ShowPopup(LocalizationManager.GetText("Popup.ReplaceBadge.BadgeAlreadyEquipped"), LocalizationManager.GetText("Popup.ReplaceBadge.GoToSurvivorPanel{name}", badge.OwnerName), LocalizationManager.GetText("Button.Ok"), okCallback, LocalizationManager.GetText("Button.Cancel"), cancelCallback);
			}
			return;
		}
		foreach (NUIListItemBase currentItems in inventoryList.currentItemsList)
		{
			if (currentItems is SurvivorBadgesIcon survivorBadgesIcon && survivorBadgesIcon.GetData().ModelId == badge.ModelId)
			{
				if (survivorBadgesIcon.GetData().MaxSimilarBadgesReached)
				{
					return;
				}
				break;
			}
		}
		UIEvent.Send("OnClickBadgeIconEquip", badge);
		inventoryList.UpdateUIICurrentItems();
	}

	private IEnumerator ShowBadgesFromOtherSurvivor(int badgeModelId)
	{
		SurvivorModel parameter = GameManager.Instance.playerModel.SurvivorContainer.Survivors.First((SurvivorModel x) => x.BadgeContainer.Badges.Any((BadgeModel badge) => badge.ModelId == badgeModelId));
		UIEvent.Send("OnNewSurvivorSelected", parameter);
		yield return new WaitForSeconds(delayToShowBadgesWhenChangingSurvivors);
		UnityEngine.Object.FindObjectOfType<SurvivorInfoPopup>()?.ShowBadges();
	}

	private void LateUpdate()
	{
		if (!(inventoryList == null))
		{
			inventoryList.UpdateVisibleItems<BadgeInfo>("BadgeCardSmall");
			SetInventoryIconInfos();
		}
	}

	public void UpdateWith(SurvivorModel survivorModel)
	{
		currentSurvivorModel = survivorModel;
		if (survivorModel == null || badgesIconsArray == null)
		{
			Clear();
			return;
		}
		if (miniatureBadges != null)
		{
			miniatureBadges.SetDataForSurvivor(survivorModel);
			Helpers.GameObjectSetActive(miniatureBadges, value: true);
		}
		for (int i = 0; i < badgesIconsArray.Length; i++)
		{
			if (badgesIconsArray[i] != null)
			{
				BadgeInfo badgeInfo = new BadgeInfo(survivorModel.GetBadgeWithSlotIndex(i));
				badgeInfo.SetBonusActive = badgeInfo.Model != null && survivorModel.BadgeContainer.HasSetBonus(badgeInfo.Model.Type);
				badgesIconsArray[i].SetData(badgeInfo);
				badgesIconsArray[i].SetSlotIndex(i);
				badgesIconsArray[i].UpdateUI();
				UIButtonExtended component = badgesIconsArray[i].GetComponent<UIButtonExtended>();
				if (component != null)
				{
					component.isEnabled = GameManager.Instance.playerModel.IsCraftingAvailable;
				}
			}
		}
		bool flag = survivorModel.BadgeContainer.HasAnySetBonus();
		if (!OfflineManager.IsLoadDataManager)
		{
			Helpers.GameObjectSetActive(setBonusContainer, flag);
			if (flag)
			{
				UnityEngine.Object.Instantiate(bonusSetActivationEffect).transform.SetParent(setBonusContainer.transform, worldPositionStays: false);
			}
		}

		PlayerModel playerModel = GameManager.Instance.playerModel;
		BuildingUpgradeLevel buildingUpgradeLevel = GameManager.Instance.gameEconomyData.GetBuildingUpgradeLevel("Residence", 1);
		if (playerModel.CouncilLevel < buildingUpgradeLevel.DependencyLevelRequired)
		{
			HelpersUI.SetContentToLabel(lockedMessageLabel, LocalizationManager.GetText("Popup.SurvivorInfoPopup.Badges.UnlockAtCouncilLevel", buildingUpgradeLevel.DependencyLevelRequired));
			Helpers.GameObjectSetActive(lockedContainer, value: true);
			if (hiddenGameObjectsWhenLocked != null)
			{
				for (int j = 0; j < hiddenGameObjectsWhenLocked.Length; j++)
				{
					Helpers.GameObjectSetActive(hiddenGameObjectsWhenLocked[j], value: false);
				}
			}
		}
		else if (playerModel.Camp.GetBuildingLevel("Residence") < 1)
		{
			HelpersUI.SetContentToLabel(lockedMessageLabel, LocalizationManager.GetText("Popup.SurvivorInfoPopup.Badges.UnlockWithBuilding"));
			Helpers.GameObjectSetActive(lockedContainer, value: true);
			if (hiddenGameObjectsWhenLocked != null)
			{
				for (int k = 0; k < hiddenGameObjectsWhenLocked.Length; k++)
				{
					Helpers.GameObjectSetActive(hiddenGameObjectsWhenLocked[k], value: false);
				}
			}
		}
		else
		{
			Helpers.GameObjectSetActive(lockedContainer, value: false);
			if (hiddenGameObjectsWhenLocked != null)
			{
				for (int l = 0; l < hiddenGameObjectsWhenLocked.Length; l++)
				{
					Helpers.GameObjectSetActive(hiddenGameObjectsWhenLocked[l], value: true);
				}
			}
		}
		if (badgeSetsPanel != null)
		{
			badgeSetsPanel.SetInfo(survivorModel);
		}
		if (damagePanel != null)
		{
			damagePanel.setAmount(survivorModel.GetCommonDamage().ToString());
		}
		if (healthPanel != null)
		{
			healthPanel.setAmount(survivorModel.GetCommonHealth().ToString());
		}
		badgeFilteringController.ForceUpdate();
		UpdateSelectedIndicator();
	}

	public override void Clear()
	{
		base.Clear();
		if (badgesIconsArray == null)
		{
			return;
		}
		for (int i = 0; i < badgesIconsArray.Length; i++)
		{
			if (badgesIconsArray[i] != null)
			{
				badgesIconsArray[i].Clear();
			}
		}
	}

	public void CheckIfFirstTime()
	{
		if (OfflineManager.IsLoadDataManager)
		{
			DebugTWD.LogMycode("if (OfflineManager.IsLoadDataManager) return");
			DebugTWD.Log("Ignore CheckIfFirstTime");
			return;
		}
		if (!GameManager.Instance.Blackboard.IsToggleOn("Toggle.ResidenceSeen"))
		{
			HUDElement hUDElement = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.BadgeInfoPopup);
			if (hUDElement != null)
			{
				hUDElement.Open();
				Helpers.ExecuteCommand(new FeatureUnlockedSeenCommand("Toggle.ResidenceSeen"));
			}
		}
	}
}
