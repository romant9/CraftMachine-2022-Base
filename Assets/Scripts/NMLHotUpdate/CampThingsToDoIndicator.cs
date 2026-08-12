using BaseModel;
using TWDModel;
using UnityEngine;

public class CampThingsToDoIndicator : MonoBehaviour
{
	[SerializeField]
	private UILabel AmountLabel;

	private void OnEnable()
	{
		if (GameManager.Instance != null)
		{
			UpdateUI();
			GameManager.Instance.playerModel.Camp.Changed += CampModelChanged;
			UIEvent.OnUIEvent += OnUIEvent;
		}
	}

	private void OnDisable()
	{
		if (GameManager.Instance != null)
		{
			GameManager.Instance.playerModel.Camp.Changed -= CampModelChanged;
			UIEvent.OnUIEvent -= OnUIEvent;
		}
	}

	private void CampModelChanged(ModelObject m, string changed, object args)
	{
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

	private void OnUIEvent(string type, object parameter)
	{
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

	public void UpdateUI()
	{
		if (!(AmountLabel != null))
		{
			return;
		}
		int num = 0;
		ModelList<BuildingModel> buildings = GameManager.Instance.playerModel.Camp.Buildings;
		for (int i = 0; i < buildings.Count; i++)
		{
			if (buildings[i] != null && ((buildings[i].CanCollect && buildings[i].Producer.HasEnoughToCollect) || (!buildings[i].IsUpgrading && buildings[i].CanPayUpgrade)))
			{
				num++;
			}
		}
		if (num > 0)
		{
			AmountLabel.text = num.ToString();
			NGUITools.SetActiveChildren(base.gameObject, state: true);
		}
		else
		{
			NGUITools.SetActiveChildren(base.gameObject, state: false);
		}
	}
}
