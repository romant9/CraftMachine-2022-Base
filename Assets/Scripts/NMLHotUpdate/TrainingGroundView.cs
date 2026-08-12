using BaseModel;
using TWDModel;

public class TrainingGroundView : BuildingView
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

	public SurvivorUpgradeDoneIndicator GetDoneIndicator()
	{
		return upgradeDoneIndicator;
	}

	protected override void ResetVisualization(bool updateBuildingGraphics = true)
	{
		base.ResetVisualization(updateBuildingGraphics);
		TrainingGroundBuildingModel trainingGroundBuildingModel = base.Model as TrainingGroundBuildingModel;
		if (trainingGroundBuildingModel.UpgradingSurvivor != null)
		{
			SurvivorUpgradeIndicator survivorUpgradeIndicator = CampView.Instance.BuildingsHud.CreateSurvivorUpgradeIndicator(this);
			survivorUpgradeIndicator.SetType(UpgradeType.UpgradeSurvivor);
			indicators.Add(survivorUpgradeIndicator);
		}
		else if (trainingGroundBuildingModel.UpgradedUnseenModel != null)
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
			UIEvent.Send("OnSurvivorUpgradeComplete");
			ResetVisualization();
			break;
		}
	}
}
