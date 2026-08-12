using TWDModel;

public class MissionHubPanelSeasons : MissionHubGameModePanel
{
	protected override void OpenDialog()
	{
		if (!base.isLocked)
		{
			MissionHubNavigation.OpenSeasonSelector();
		}
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
		if (GameManager.Instance.playerModel.SurvivorContainer.StoryTeller != null)
		{
			CheckLockedState();
		}
		Helpers.GameObjectSetActive(unlockedEffect, FeatureUIHighlights.IsActive(FeatureUIHighlights.FeaturesIds.SeasonModeUnlocked));
	}

	public override void CheckLockedState()
	{
		StoryTellerModel storyTeller = GameManager.Instance.playerModel.SurvivorContainer.StoryTeller;
		bool flag = storyTeller.GetCurrentUncompletedQuestDefinition() != null && storyTeller.GetCurrentUncompletedQuestDefinition().Order > 0;
		UpdateLockedState(!flag);
	}

	protected override void ButtonMainClicked(UIButtonExtended button)
	{
		base.ButtonMainClicked(button);
		EventManager.NotifyClick("Seasons");
	}
}
