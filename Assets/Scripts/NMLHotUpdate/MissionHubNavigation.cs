using TwdCustomMod;
using TWDModel;

public class MissionHubNavigation : MonoBehaviourExtended
{
	public static void ContinueStoryMap()
	{
		StoryTellerModel storyTeller = GameManager.Instance.playerModel.SurvivorContainer.StoryTeller;
		if (storyTeller == null)
		{
			Debug.LogWarning("MissionHubNavigation::ContinueStoryMap(): CANNOT continue -> storyTellerModel is NULL");
			return;
		}
		if (storyTeller.CanCompleteQuest || storyTeller.CanAcceptQuest)
		{
			SingularityMonoBehaviour<HUDManager>.Instance.CloseAllOpenPopupsAndDialogs();
			StoryTellerFlow.StartFlow(storyTeller);
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("camp/storyteller_click");
			return;
		}
		QuestDefinition questDefinition = storyTeller.CurrentQuestDefinition;
		if (questDefinition == null)
		{
			questDefinition = storyTeller.GetCurrentUncompletedQuestDefinition();
		}
		if (questDefinition != null)
		{
			MapMissionGroupModel unlockedEpisode = questDefinition.GetUnlockedEpisode(GameManager.Instance.modelManager);
			if (unlockedEpisode != null)
			{
				CampManager.Instance.GoToMap(unlockedEpisode);
			}
		}
	}

	public static void OpenSeasonMap(SeasonDefinition definition)
	{
		if (definition != null)
		{
			OpenSeasonMap(definition.Id);
		}
		else
		{
			Debug.LogWarning("MissionHubNavigation::OpenSeasonMap(): CANNOT continue -> definition is NULL or Empty");
		}
	}

	public static void OpenSeasonMap(string seasonId)
	{
		if (!string.IsNullOrEmpty(seasonId))
		{
			CampManager.Instance.GoToSeasonMap(seasonId);
		}
		else
		{
			Debug.LogWarning("MissionHubNavigation::OpenSeasonMap(): CANNOT continue -> seasonId is NULL or Empty");
		}
	}

	public static void TryOpenChallengeMap()
	{
		CampHUD.TryToAccessChallenges(OpenChallengeMap);
	}

	private static void OpenChallengeMap()
	{
		if (CanAccessChallengeMap(out var mapMissionGroupModel))
		{
			CampManager.Instance.GoToMap(mapMissionGroupModel);
		}
	}

	public static void TryOpenApocalypticChallengeMap()
	{
		CampHUD.TryToAccessChallenges(OpenApocalypticChallengeMap);
	}

	private static void OpenApocalypticChallengeMap()
	{
		if (CanAccessApocalypticChallengeMap(out var mapMissionGroupModel))
		{
			Helpers.ExecuteCommand(new StartApocalypseChallengeCycleCommand(isNextCircle: false));
			CampManager.Instance.GoToMap(mapMissionGroupModel);
		}
	}

	public static void TryOpenSurvivalMap()
	{
		CampHUD.TryToAccessSurvival(OpenSurvivalMap);
	}

	private static void OpenSurvivalMap()
	{
		if (CanAccessSurvivalMap(out var mapMissionGroupModel))
		{
			CampManager.Instance.GoToMap(mapMissionGroupModel);
		}
	}

	public static void TryOpenGvGBattleMap(bool isSpectator = false)
	{
		if (OfflineManager.IsLoadDataManager && HelpersModel.IsUnlockPVP)
		{
			OpenGuildBattleMap();
		}
		else
		{
			CampHUD.TryToAccessGuildBattle(OpenGuildBattleMap, isSpectator);
		}
	}

	public static void OpenGuildBattleMap()
	{
		CampManager.Instance.GoToGuildBattleMap();
	}

	public static void OpenWorldBoss()
	{
		MissionHubPopup missionHubPopup = SingularityMonoBehaviour<HUDManager>.Instance.GetNoCreation(UIType.MissionHubPopup) as MissionHubPopup;
		if (missionHubPopup != null)
		{
			missionHubPopup.Close();
		}
		WorldBossMainPopup worldBossMainPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.WorldBossMainPopup) as WorldBossMainPopup;
		if (worldBossMainPopup != null)
		{
			worldBossMainPopup.Open();
		}
	}

	public static void OpenSeasonSelector()
	{
		HUDElement hUDElement = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.SelectSeasonPopup);
		if (hUDElement != null)
		{
			hUDElement.Open();
		}
	}

	public static void TryOpenOutpost()
	{
		CampHUD.TryOpenOutpostTutorial(CampHUD.OpenOutpostPopupAfterChecks);
	}

	public static void OpenScavenge()
	{
		ScavengePopup scavengePopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.ScavengePopup) as ScavengePopup;
		if (scavengePopup != null)
		{
			SingularityMonoBehaviour<HUDManager>.Instance.CloseAllOpenPopupsAndDialogs();
			scavengePopup.Open();
		}
	}

	public static void TryOpenEndlessMode()
	{
		CampHUD.TryToAccessEndlessMode(OpenEndlessMode);
	}

	public static void OpenEndlessMode()
	{
		EndlessModeGameDifficultySelectionPopup endlessModeGameDifficultySelectionPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.EndlessModeExpertModeDifficultyPopup) as EndlessModeGameDifficultySelectionPopup;
		if (endlessModeGameDifficultySelectionPopup != null)
		{
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_click");
			endlessModeGameDifficultySelectionPopup.Open();
		}
	}

	public static bool CanAccessChallengeMap(out MapMissionGroupModel mapMissionGroupModel)
	{
		WeeklyChallengeModel weeklyChallengeModel = WeeklyChallengeHelper.GetWeeklyChallengeModel();
		if (weeklyChallengeModel != null)
		{
			mapMissionGroupModel = weeklyChallengeModel.GetCurrentOrNextMapMissionGroupModel();
			if (mapMissionGroupModel == null)
			{
				return false;
			}
			return true;
		}
		mapMissionGroupModel = null;
		return false;
	}

	public static bool CanAccessApocalypticChallengeMap(out MapMissionGroupModel mapMissionGroupModel)
	{
		ApocalypseWeeklyChallengeModel weeklyApocalypticChallengeModel = WeeklyChallengeHelper.GetWeeklyApocalypticChallengeModel();
		if (weeklyApocalypticChallengeModel != null)
		{
			mapMissionGroupModel = weeklyApocalypticChallengeModel.GetCurrentOrNextMapMissionGroupModel();
			if (mapMissionGroupModel == null)
			{
				return false;
			}
			return true;
		}
		mapMissionGroupModel = null;
		return false;
	}

	public static bool CanAccessSurvivalMap(out MapMissionGroupModel mapMissionGroupModel)
	{
		WeeklySurvivalModel weeklySurvivalModel = WeeklySurvivalHelper.GetWeeklySurvivalModel();
		if (weeklySurvivalModel != null)
		{
			mapMissionGroupModel = weeklySurvivalModel.GetCurrentOrNextMapMissionGroupModel();
			if (mapMissionGroupModel == null)
			{
				return false;
			}
			return true;
		}
		mapMissionGroupModel = null;
		return false;
	}
}
