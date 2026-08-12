using System.Collections.Generic;
using BaseModel;
using TWDModel;
using UnityEngine;

public class SurvivorsThingsToDoIndicator : ThingsToDoIndicatorBuildingBase
{
	[SerializeField]
	private bool CombatTeamOnly;

	private bool updagradeActive;

	private static bool IsAnySurvivorUpgrading => GameManager.Instance.playerModel.SurvivorContainer.HasUpgradingSurvivor;

	private void Update()
	{
		if (!IsAnySurvivorUpgrading && updagradeActive)
		{
			updagradeActive = false;
			UpdateUI();
		}
	}

	public override void OnEnable()
	{
		base.OnEnable();
		updagradeActive = IsAnySurvivorUpgrading;
		UpdateUI();
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
		if (!(AmountLabel != null))
		{
			return;
		}
		List<SurvivorModel> list = ((!CombatTeamOnly) ? GameManager.Instance.playerModel.SurvivorContainer.GetUpgradeableSurvivors() : GameManager.Instance.playerModel.SurvivorContainer.GetUpgradeableCombatSurvivors());
		int num = 0;
		if (BuildingHasUnseenUpgrade("TrainingGround"))
		{
			num++;
		}
		int upgradableSupportCount = GameManager.Instance.playerModel.GetUpgradableSupportCount();
		if ((list != null && list.Count > 0) || num > 0 || upgradableSupportCount > 0)
		{
			bool upgrading = IsAnySurvivorUpgrading || IsBuildingUpgrading("TrainingGround");
			int num2 = 0;
			if (list != null)
			{
				num2 = list.Count;
			}
			num2 += num;
			num2 += upgradableSupportCount;
			num2 += Helpers.GetRedSurvivalManualNum();
			AmountLabel.text = num2.ToString();
			SetActiveAllChildren(active: true, upgrading);
		}
		else
		{
			SetActiveAllChildren(active: false);
		}
	}

	protected override void PlayerModelChanged(ModelObject m, string changed, object args)
	{
		switch (changed)
		{
		case "combatSurvivorsChanged":
		case "addSurvivor":
		case "survivorDemoted":
			UpdateUI();
			break;
		}
	}

	protected override void CampModelChanged(ModelObject m, string changed, object args)
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

	protected override void OnUIEvent(string type, object parameter)
	{
		switch (type)
		{
		case "OnSurvivorUpgradeStarted":
			updagradeActive = IsAnySurvivorUpgrading;
			UpdateUI();
			break;
		case "OnSurvivorInstantUpgraded":
			UpdateUI();
			break;
		case "OnSurvivorUpgradeComplete":
			UpdateUI();
			break;
		}
	}

	protected override void BuildingModelChanged(ModelObject m, string changed, object args)
	{
		if (changed == "UpgradeSeen" || changed == "UpgradingItemReady")
		{
			UpdateUI();
		}
	}
}
