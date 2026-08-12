using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TWDModel;
using UnityEngine;

public class WorkshopTokenClassListPanel : ScrollableListPanel<WorkshopEquipmentRow>
{
	private SurvivorClassFilter classFilter;

	private bool shouldResetScrollBar;

	protected override bool LastEntryAtTop => false;

	private void OnEnable()
	{
		shouldResetScrollBar = true;
		UIEvent.OnUIEvent += OnUIEvent;
		SurvivorClass filteredClass = ((classFilter != null && classFilter.CurrentSelectedFilter != null) ? classFilter.CurrentSelectedFilter.ClassFilter : SurvivorClass.None);
		SetupCardsForClass(filteredClass);
	}

	private void OnDisable()
	{
		UIEvent.OnUIEvent -= OnUIEvent;
		//CardsList = null;
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
		classFilter.UpdatePositionAndState();
	}

	private void OnClassFilterButtonClicked(SurvivorClass selectedClass)
	{
		SetupCardsForClass(selectedClass);
	}

	public void SetupCardsForClass(SurvivorClass filteredClass)
	{
		List<WorkshopEquipmentRow> list = new List<WorkshopEquipmentRow>();
		WorkshopEquipmentRow workshopEquipmentRow = new WorkshopEquipmentRow();
		workshopEquipmentRow.SurvivorClass = filteredClass;
		workshopEquipmentRow.IsArmor = false;
		list.Add(workshopEquipmentRow);
		WorkshopEquipmentRow workshopEquipmentRow2 = new WorkshopEquipmentRow();
		workshopEquipmentRow2.SurvivorClass = filteredClass;
		workshopEquipmentRow2.IsArmor = true;
		list.Add(workshopEquipmentRow2);
		SetCards(list, shouldResetScrollBar);
		if (OfflineManager.IsLoadDataManager)
		{
			//CardsList = list;
			SortByEquipType(tabs.GetUIButtonToggleList[tabIndex]);
		}
	}



	#region myparams
	public UILabel TokenAmountLabel;
	public UIButtonToggleSet tabs;
	public int tabIndex = 0;
	//private List<WorkshopEquipmentRow> CardsList;
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

	public void SetCurrencyAmount()
	{
		int tokenAmount;
		try
		{
			tokenAmount = Convert.ToInt32(TokenAmountLabel.text.ToString());
		}
		catch
		{
			tokenAmount = 0;
		}
		GameManager.Instance.playerModel.SetCurrency(CurrencyType.ApocalypticEquipToken, tokenAmount);
		var items = transform.GetComponentsInChildren<EquipmentTokenButton>();
		if (items != null && items.Count() > 0)
		{
			foreach (var item in items)
			{
				item.SetAmount(1);
			}
		}
	}
	#endregion
}
