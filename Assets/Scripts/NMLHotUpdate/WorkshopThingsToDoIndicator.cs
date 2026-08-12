using BaseModel;
using TWDModel;
using UnityEngine;

public class WorkshopThingsToDoIndicator : ThingsToDoIndicatorBuildingBase
{
	[SerializeField]
	protected GameObject apocalypticBackground;

	public override void OnEnable()
	{
		base.OnEnable();
		InitCurrentBuilding("Workshop");
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
		if (AmountLabel != null)
		{
			int count = GameManager.Instance.playerModel.Equipment.GetAllUpgradeableEquipments().Count;
			if (count > 0)
			{
				bool upgrading = IsBuildingUpgrading("Workshop") || GameManager.Instance.playerModel.Equipment.GetUpgradingEquipment() != null;
				AmountLabel.text = count.ToString();
				SetActiveAllChildren(active: true, upgrading);
			}
			else
			{
				SetActiveAllChildren(active: false);
			}
		}
		if (apocalypticBackground != null)
		{
			bool value = GameManager.Instance.playerModel.EquipTokenContainer.CanAssemble();
			Helpers.GameObjectSetActive(apocalypticBackground, value);
		}
	}

	protected override void CampModelChanged(ModelObject m, string changed, object args)
	{
		base.CampModelChanged(m, changed, args);
		switch (changed)
		{
		case "EventLevelUpBuilding":
		case "EventBuildingCollected":
		case "EventAddBuilding":
		case "EventUpgradeBuilding":
			UpdateUI();
			break;
		}
	}

	protected override void OnUIEvent(string type, object parameter)
	{
		base.OnUIEvent(type, parameter);
		switch (type)
		{
		case "OnEquipmentUpdated":
		case "EquipmentUpgraded":
		case "EquipmentInstantUpgraded":
		case "EquipmentStartUpgrade":
			UpdateUI();
			break;
		}
	}

	protected override void EquipmentModelChanged(ModelObject m, string changed, object args)
	{
		base.EquipmentModelChanged(m, changed, args);
		if (changed == EquipmentModel.EquipmentTypeUpgradedEvent || changed == "EquipmentTokenTypeUnlockEvent" || changed == "EquipmentTokenTypeUpdateEvent")
		{
			UpdateUI();
		}
	}

	protected override void BuildingModelChanged(ModelObject m, string changed, object args)
	{
		base.BuildingModelChanged(m, changed, args);
		switch (changed)
		{
		case "UpgradeSeen":
		case "UpgradingItemReady":
		case "UpgradingItemCancelled":
			UpdateUI();
			break;
		}
	}
}
