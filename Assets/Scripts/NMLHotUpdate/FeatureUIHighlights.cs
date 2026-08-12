using TWDModel;

public class FeatureUIHighlights
{
	public enum FeaturesIds
	{
		WeeklyChallengeUnlocked = 0,
		WeeklySurvivalUnlocked = 1,
		DoubleXpUIBadges = 2,
		EndlessModeUnlocked = 3,
		SeasonModeUnlocked = 4,
		ScavengeModeUnlocked = 5
	}

	public static bool IsActive(FeaturesIds featureId)
	{
		switch (featureId)
		{
		case FeaturesIds.WeeklyChallengeUnlocked:
			if (WeeklyChallengeHelper.IsChallengeOngoing())
			{
				return !GameManager.Instance.playerModel.Blackboard.IsToggleOn("Toggle.ToggleWeeklyChallengeHighlight");
			}
			return false;
		case FeaturesIds.WeeklySurvivalUnlocked:
			if (WeeklySurvivalHelper.IsSurvivalOngoing())
			{
				return !GameManager.Instance.playerModel.Blackboard.IsToggleOn("Toggle.ToggleWeeklySurvivalHighlight");
			}
			return false;
		case FeaturesIds.SeasonModeUnlocked:
		{
			StoryTellerModel storyTeller = GameManager.Instance.playerModel.SurvivorContainer.StoryTeller;
			if (storyTeller.GetCurrentUncompletedQuestDefinition() != null && storyTeller.GetCurrentUncompletedQuestDefinition().Order > 0)
			{
				return !GameManager.Instance.playerModel.Blackboard.IsToggleOn("Toggle.ToggleSeasonHighlight");
			}
			return false;
		}
		case FeaturesIds.ScavengeModeUnlocked:
			if (GameManager.Instance.playerModel.Tutorial.HasCompletedPart("EndTutorial"))
			{
				return !GameManager.Instance.playerModel.Blackboard.IsToggleOn("Toggle.ToggleScavengeHighlight");
			}
			return false;
		case FeaturesIds.DoubleXpUIBadges:
			return GameManager.Instance.playerModel.ActivityManager.IsActivityOpen(ActivityType.DoubleXPFromKills);
		case FeaturesIds.EndlessModeUnlocked:
			if ((!EndlessModeHelpers.IsEndlessModeActive() || GameManager.Instance.playerModel.Blackboard.IsToggleOn("Toggle.ToggleEndlessModeHighlightExpired")) && !EndlessModeHelpers.EndlessManagerModel().DoWeHaveRewardsUnclaimed())
			{
				return EndlessModeHelpers.UnSeenEndlessPassTokens;
			}
			return true;
		default:
			return false;
		}
	}

	public static void MarkHighlightExpired(FeaturesIds featureId)
	{
		switch (featureId)
		{
		case FeaturesIds.WeeklyChallengeUnlocked:
			if (!GameManager.Instance.playerModel.Blackboard.IsToggleOn("Toggle.ToggleWeeklyChallengeHighlight"))
			{
				Helpers.ExecuteCommand(new SetBlackboardToggleCommand("Toggle.ToggleWeeklyChallengeHighlight"));
			}
			break;
		case FeaturesIds.WeeklySurvivalUnlocked:
			if (!GameManager.Instance.playerModel.Blackboard.IsToggleOn("Toggle.ToggleWeeklySurvivalHighlight"))
			{
				Helpers.ExecuteCommand(new SetBlackboardToggleCommand("Toggle.ToggleWeeklySurvivalHighlight"));
			}
			break;
		case FeaturesIds.SeasonModeUnlocked:
			if (!GameManager.Instance.playerModel.Blackboard.IsToggleOn("Toggle.ToggleSeasonHighlight"))
			{
				Helpers.ExecuteCommand(new SetBlackboardToggleCommand("Toggle.ToggleSeasonHighlight"));
			}
			break;
		case FeaturesIds.ScavengeModeUnlocked:
			if (!GameManager.Instance.playerModel.Blackboard.IsToggleOn("Toggle.ToggleScavengeHighlight"))
			{
				Helpers.ExecuteCommand(new SetBlackboardToggleCommand("Toggle.ToggleScavengeHighlight"));
			}
			break;
		case FeaturesIds.EndlessModeUnlocked:
			if (!GameManager.Instance.playerModel.Blackboard.IsToggleOn("Toggle.ToggleEndlessModeHighlightExpired"))
			{
				Helpers.ExecuteCommand(new SetBlackboardToggleCommand("Toggle.ToggleEndlessModeHighlightExpired"));
			}
			break;
		}
	}
}
