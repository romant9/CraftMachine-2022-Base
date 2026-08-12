using BaseModel;
using TWDModel;

public class WorkshopView : BuildingView
{
	private SurvivorUpgradeDoneIndicator upgradeDoneIndicator;

	public override bool OnSelected(bool forcedSelection)
	{
		base.OnSelected(forcedSelection);
		if (upgradeDoneIndicator != null && !forcedSelection)
		{
			upgradeDoneIndicator.OnClickUpgradedSurvivor();
			return true;
		}
		return false;
	}

	public SurvivorUpgradeDoneIndicator GetDoneIndicator()
	{
		return upgradeDoneIndicator;
	}

	protected override void ResetVisualization(bool updateBuildingGraphics = true)
	{
		base.ResetVisualization(updateBuildingGraphics);
		WorkshopBuildingModel workshopBuildingModel = base.Model as WorkshopBuildingModel;
		if (workshopBuildingModel.UpgradingModel != null)
		{
			BuildingUpgradeIndicator buildingUpgradeIndicator = CampView.Instance.BuildingsHud.CreateUpgradeIndicator(this);
			buildingUpgradeIndicator.SetType(UpgradeType.EquipmentUpgrading);
			indicators.Add(buildingUpgradeIndicator);
		}
		else if (workshopBuildingModel.UpgradedUnseenModel != null)
		{
			upgradeDoneIndicator = CampView.Instance.BuildingsHud.CreateSurvivorUpgradeDoneIndicator(this);
			upgradeDoneIndicator.Reset();
			indicators.Add(upgradeDoneIndicator);
		}
	}

	protected override void OnModelChange(ModelObject model, string changed, object args)
	{
		base.OnModelChange(model, changed, args);
		switch (changed)
		{
		case "NewItemStartedUpgrading":
			ResetVisualization(updateBuildingGraphics: false);
			EventManager.NotifyEvent(EventManager.EventType.CampVisualizationChanged);
			SetIndicatorInsideBuildingUpgradeAvailable();
			break;
		case "UpgradingItemCancelled":
			SetIndicatorInsideBuildingUpgradeAvailable();
			break;
		case "UpgradingItemReady":
			ResetVisualization();
			break;
		}
	}
}
