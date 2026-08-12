using BaseModel;

public class DispatchView : BuildingView
{
	private SurvivorUpgradeDoneIndicator upgradeDoneIndicator;

	public override bool OnSelected(bool forcedSelection)
	{
		base.OnSelected(forcedSelection);
		return false;
	}

	protected override void ResetVisualization(bool updateBuildingGraphics = true)
	{
		base.ResetVisualization(updateBuildingGraphics);
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
