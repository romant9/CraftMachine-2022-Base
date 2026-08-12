using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TWDModel;
using UnityEngine;

public class WorkshopClassListPanel : ScrollableListPanel<WorkshopEquipmentRow>
{
	private SurvivorClassFilter classFilter;

	private bool shouldResetScrollBar;

	protected override bool LastEntryAtTop => false;

	private void OnEnable()
	{
		shouldResetScrollBar = true;
		UIEvent.OnUIEvent += OnUIEvent;
	}

	private void OnDisable()
	{
		UIEvent.OnUIEvent -= OnUIEvent;
	}

	private void OnUIEvent(string type, object parameter)
	{
		if (type == "OnEquipmentTypeChosen")
		{
			shouldResetScrollBar = true;
		}
		else if (type == "OnPopUpClose" && parameter is WorkshopPopup)
		{
			ClearCards();
			shouldResetScrollBar = true;
		}
		else if (type == "OnEquipmentUpdated" || type == "EquipmentInstantUpgraded")
		{
			shouldResetScrollBar = false;
			SurvivorClass filteredClass = ((classFilter != null && classFilter.CurrentSelectedFilter != null) ? classFilter.CurrentSelectedFilter.ClassFilter : SurvivorClass.None);
			SetupCardsForClass(filteredClass);
		}
		else
		{
			shouldResetScrollBar = true;
		}
	}

	public void SetClassFilter(SurvivorClassFilter aclassFilter)
	{
		if (classFilter != null)
		{
			classFilter.OnClassFilterSelected -= OnClassFilterButtonClicked;
		}
		classFilter = aclassFilter;
		classFilter.OnClassFilterSelected += OnClassFilterButtonClicked;
		for (int i = 0; i < 6; i++)
		{
			classFilter.EnableButtonForClass((SurvivorClass)i, SurvivorClassHasEquipmentInInventory((SurvivorClass)i));
		}
		classFilter.UpdatePositionAndState();
	}

	private void OnClassFilterButtonClicked(SurvivorClass selectedClass)
	{
		SetupCardsForClass(selectedClass);
	}

	private bool SurvivorClassHasEquipmentInInventory(SurvivorClass survivorClass)
	{
		foreach (EquipmentItemModel allEquipment in GameManager.Instance.playerModel.Equipment.GetAllEquipments())
		{
			if (allEquipment.Definition.CanBeEquippedBySurvivorClass(survivorClass) && allEquipment.CanBeManipulated())
			{
				return true;
			}
		}
		return false;
	}

	public void SetupCardsForClass(SurvivorClass filteredClass)
	{
		List<WorkshopEquipmentRow> list = new List<WorkshopEquipmentRow>();
		if (filteredClass == SurvivorClass.None)
		{
			foreach (SurvivorClass value in Enum.GetValues(typeof(SurvivorClass)))
			{
				if (value != SurvivorClass.None && SurvivorClassHasEquipmentInInventory(value))
				{
					WorkshopEquipmentRow workshopEquipmentRow = new WorkshopEquipmentRow();
					workshopEquipmentRow.SurvivorClass = value;
					workshopEquipmentRow.IsArmor = false;
					list.Add(workshopEquipmentRow);
				}
			}
		}
		else if (SurvivorClassHasEquipmentInInventory(filteredClass))
		{
			WorkshopEquipmentRow workshopEquipmentRow2 = new WorkshopEquipmentRow();
			workshopEquipmentRow2.SurvivorClass = filteredClass;
			workshopEquipmentRow2.IsArmor = false;
			list.Add(workshopEquipmentRow2);
			WorkshopEquipmentRow workshopEquipmentRow3 = new WorkshopEquipmentRow();
			workshopEquipmentRow3.SurvivorClass = filteredClass;
			workshopEquipmentRow3.IsArmor = true;
			list.Add(workshopEquipmentRow3);
		}
		SetCards(list, shouldResetScrollBar);
		if (OfflineManager.IsLoadDataManager)
		{
			SortByEquipType(tabs.GetUIButtonToggleList[tabIndex]);
		}
	}



	#region myparams
	public UIButtonToggleSet tabs;
	public int tabIndex = 0;
	#endregion

	#region mycode
	public void SortByEquipType(UIButtonToggle isWeaponTg)
	{
		tabIndex = tabs.GetUIButtonToggleList.ToList().IndexOf(isWeaponTg);
		if (!this.gameObject.activeSelf) return;
		StartCoroutine(SortByEquipTypeI());
	}

	private IEnumerator SortByEquipTypeI()
	{
		yield return new WaitUntil(() => cardsContainer.transform.childCount == 2);

		if (tabIndex == 0)
		{
			cardsContainer.transform.GetChild(0).gameObject.SetActive(true);
			cardsContainer.transform.GetChild(1).gameObject.SetActive(false);
		}
		else
		{
			cardsContainer.transform.GetChild(0).gameObject.SetActive(false);
			cardsContainer.transform.GetChild(1).gameObject.SetActive(true);
			cardsContainer.transform.GetChild(1).localPosition = Vector3.one;
		}

		GetComponent<UIScrollView>().ResetPosition();
	}
	#endregion
}
