using BaseModel;
using TWDModel;

public class CageView : BuildingView
{
	private SurvivorUpgradeDoneIndicator upgradeDoneIndicator;

	public override bool OnSelected(bool forcedSelection)
	{
		base.OnSelected(forcedSelection);
		if (upgradeDoneIndicator != null)
		{
			upgradeDoneIndicator.OnClickUpgradedSurvivor();
			return true;
		}
		return false;
	}

	protected override void ResetVisualization(bool updateBuildingGraphics = true)
	{
		base.ResetVisualization(updateBuildingGraphics);
		CageBuildingModel cageBuildingModel = base.Model as CageBuildingModel;
		if (cageBuildingModel.UpgradingWalker != null)
		{
			BuildingUpgradeIndicator buildingUpgradeIndicator = CampView.Instance.BuildingsHud.CreateUpgradeIndicator(this);
			buildingUpgradeIndicator.SetType(UpgradeType.UpgradeWalker);
			indicators.Add(buildingUpgradeIndicator);
		}
		else if (cageBuildingModel.UpgradedUnseenModel != null)
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
