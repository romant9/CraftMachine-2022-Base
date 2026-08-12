using System.Collections.Generic;
using TWDModel;

public class EquipSkillRecommendListPanel : ScrollableListPanel<EquipmentButton>
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
		List<EquipmentButton> items = new List<EquipmentButton>();
		SetCards(items, shouldResetScrollBar);
	}
}
