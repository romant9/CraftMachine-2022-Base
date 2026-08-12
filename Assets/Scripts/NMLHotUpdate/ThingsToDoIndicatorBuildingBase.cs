using BaseModel;
using TWDModel;
using UnityEngine;

public class ThingsToDoIndicatorBuildingBase : MonoBehaviour
{
	[SerializeField]
	protected UILabel AmountLabel;

	[SerializeField]
	protected GameObject BuildingUpdatingParent;

	protected BuildingModel currentBuilding;

	public virtual void OnEnable()
	{
		if (GameManager.Instance != null)
		{
			UpdateUI();
			GameManager.Instance.playerModel.SurvivorContainer.Changed += PlayerModelChanged;
			GameManager.Instance.playerModel.Equipment.Changed += EquipmentModelChanged;
			GameManager.Instance.playerModel.EquipTokenContainer.Changed += EquipmentModelChanged;
			GameManager.Instance.playerModel.Camp.Changed += CampModelChanged;
			UIEvent.OnUIEvent += OnUIEvent;
		}
	}

	public virtual void OnDisable()
	{
		if (GameManager.Instance != null)
		{
			GameManager.Instance.playerModel.SurvivorContainer.Changed -= PlayerModelChanged;
			GameManager.Instance.playerModel.Equipment.Changed -= EquipmentModelChanged;
			GameManager.Instance.playerModel.EquipTokenContainer.Changed -= EquipmentModelChanged;
			GameManager.Instance.playerModel.Camp.Changed -= CampModelChanged;
			UIEvent.OnUIEvent -= OnUIEvent;
		}
		NullCurrentBuilding();
	}

	public virtual void UpdateUI()
	{
	}

	protected virtual void SetActiveAllChildren(bool active, bool upgrading = false)
	{
		NGUITools.SetActiveChildren(base.gameObject, active);
		if (BuildingUpdatingParent != null)
		{
			BuildingUpdatingParent.SetActive(upgrading);
		}
	}

	protected virtual bool IsBuildingUpgrading(string typeName)
	{
		InitCurrentBuilding(typeName);
		return IsCurrentBuildingUprading();
	}

	protected virtual bool BuildingHasUnseenUpgrade(string typeName)
	{
		InitCurrentBuilding(typeName);
		return CurrentBuildingUpgradedUnseenModel();
	}

	protected virtual void PlayerModelChanged(ModelObject m, string changed, object args)
	{
	}

	protected virtual void CampModelChanged(ModelObject m, string changed, object args)
	{
	}

	protected virtual void EquipmentModelChanged(ModelObject m, string changed, object args)
	{
	}

	protected virtual void OnUIEvent(string type, object parameter)
	{
	}

	protected virtual void BuildingModelChanged(ModelObject m, string changed, object args)
	{
	}

	protected void InitCurrentBuilding(string typeName)
	{
		if (currentBuilding == null && typeName != "")
		{
			BuildingModel building = GameManager.Instance.playerModel.Camp.GetBuilding(typeName);
			if (building != null)
			{
				currentBuilding = building;
				currentBuilding.Changed += BuildingModelChanged;
			}
		}
	}

	private void NullCurrentBuilding()
	{
		if (currentBuilding != null)
		{
			currentBuilding.Changed -= BuildingModelChanged;
		}
		currentBuilding = null;
	}

	private bool CurrentBuildingUpgradedUnseenModel()
	{
		if (currentBuilding is ModelUpgraderBuildingModel)
		{
			if (!(currentBuilding is ModelUpgraderBuildingModel modelUpgraderBuildingModel))
			{
				return false;
			}
			return modelUpgraderBuildingModel.UpgradedUnseenModel != null;
		}
		return false;
	}

	private bool IsCurrentBuildingUprading()
	{
		if (currentBuilding == null)
		{
			return false;
		}
		return currentBuilding.IsUpgrading;
	}
}
