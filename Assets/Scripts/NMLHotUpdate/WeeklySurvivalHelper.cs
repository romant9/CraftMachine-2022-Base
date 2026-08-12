using TWDModel;
using UnityEngine;

public class WeeklySurvivalHelper : MonoBehaviour
{
	public static WeeklySurvivalReward GetNextReward()
	{
		if (GetWeeklySurvivalModel() != null)
		{
			return GetWeeklySurvivalModel().GetNextReward();
		}
		return null;
	}

	public static long GetTimeLeftToNextSurvival()
	{
		if (GetWeeklySurvivalModel() != null)
		{
			return GetWeeklySurvivalModel().TimeLeftToNextSurvival();
		}
		return 0L;
	}

	public static long GetTimeLeftToCurrentSurvivalEnd()
	{
		if (GetWeeklySurvivalModel() != null && GetWeeklySurvivalModel().CurrentDefinition != null)
		{
			return GetWeeklySurvivalModel().CurrentDefinition.EndTimeMilliseconds - GameManager.Instance.playerModel.UtcTimeStamp;
		}
		return 0L;
	}

	public static string GetFormatedTimeLeftToCurrentSurvivalEnd()
	{
		if (GetWeeklySurvivalModel() != null && GetWeeklySurvivalModel().CurrentDefinition != null)
		{
			return Helpers.FormatTime(GetWeeklySurvivalModel().CurrentDefinition.EndTimeMilliseconds - GameManager.Instance.playerModel.UtcTimeStamp);
		}
		return Helpers.FormatTime(0L);
	}

	public static WeeklySurvivalModel GetWeeklySurvivalModel()
	{
		if (GameManager.Instance != null && GameManager.Instance.playerModel != null && GameManager.Instance.playerModel.WeeklySurvival != null && !IsLockedByCouncilLevelOrTutorial())
		{
			GameManager.Instance.StartNextSurvival();
			return GameManager.Instance.playerModel.WeeklySurvival;
		}
		return null;
	}

	public static int GetGasCost()
	{
		if (GetWeeklySurvivalModel() != null && GetWeeklySurvivalModel().GetMapMissionGroupModel() != null)
		{
			return (int)GetWeeklySurvivalModel().GetMapMissionGroupModel().AverageGroupGasCost(CurrencyType.ReplayToken);
		}
		return 0;
	}

	public static bool CanResetSurvivalMap()
	{
		if (!IsSurvivalOngoing())
		{
			return false;
		}
		return GetWeeklySurvivalModel().CanRestartMapOrDoubleRewards();
	}

	public static string GetCurrentSurvivalName()
	{
		if (GetWeeklySurvivalModel() != null)
		{
			return HelpersLocalization.GetEpisodeName(GetWeeklySurvivalModel().GetMissionSpawnPointGroup());
		}
		return "";
	}

	public static WeeklySurvival GetNextSurvival()
	{
		if (GetWeeklySurvivalModel() != null)
		{
			return GetWeeklySurvivalModel().NextWeeklySurvival;
		}
		return null;
	}

	public static string GetNextSurvivalName()
	{
		if (GetWeeklySurvivalModel() != null)
		{
			WeeklySurvival nextWeeklySurvival = GetWeeklySurvivalModel().NextWeeklySurvival;
			if (nextWeeklySurvival != null)
			{
				MissionSpawnPointGroup spawnPointGroup = GameManager.Instance.gameEconomyData.MissionSpawnPointData.GetSpawnPointGroup(nextWeeklySurvival.DetailMapId);
				if (spawnPointGroup != null)
				{
					return HelpersLocalization.GetEpisodeName(spawnPointGroup);
				}
			}
		}
		return "";
	}

	public static bool IsSurvivalOngoing()
	{
		if (!IsLockedByCouncilLevelOrTutorial() && !GetWeeklySurvivalModel().Finished)
		{
			return GetWeeklySurvivalModel().CurrentDefinition != null;
		}
		return false;
	}

	public static bool IsNextSurvivalPossible()
	{
		if (!IsLockedByCouncilLevelOrTutorial() && GetWeeklySurvivalModel().Finished)
		{
			return GetWeeklySurvivalModel().NextWeeklySurvival != null;
		}
		return false;
	}

	public static bool IsLockedByCouncilLevelOrTutorial()
	{
		if (!IsLockedByCouncilLevel())
		{
			return IsLockedByTutorial();
		}
		return true;
	}

	public static bool IsLockedByCouncilLevel()
	{
		if (GameManager.Instance.playerModel == null || GameManager.Instance.playerModel.WeeklySurvival == null)
		{
			return true;
		}
		return GameManager.Instance.playerModel.WeeklySurvival.IsLockedByCouncilLevel;
	}

	public static bool IsLockedByTutorial()
	{
		if (!string.IsNullOrEmpty(GameManager.Instance.gameEconomyData.ConfigData.SurvivalUnlockAtAfterTutorialPartId))
		{
			return !GameManager.Instance.playerModel.Tutorial.HasCompletedPart(GameManager.Instance.gameEconomyData.ConfigData.SurvivalUnlockAtAfterTutorialPartId);
		}
		return false;
	}

	public static void MarkCurrentSurvivalAsSeen()
	{
		if (GetWeeklySurvivalModel() != null && IsSurvivalOngoing() && !GetWeeklySurvivalModel().SurvivalStartedSeen)
		{
			Helpers.ExecuteCommand(new WeeklySurvivalSeenCommand(GetWeeklySurvivalModel())
			{
				MarkSurvivalStartedAsSeen = true
			});
		}
	}

	public static void MarkPersonalNumberCompletionedAsSeen()
	{
		if (GetWeeklySurvivalModel() != null && !GetWeeklySurvivalModel().HasSeenLatestCompletions())
		{
			Helpers.ExecuteCommand(new WeeklySurvivalSeenCommand(GetWeeklySurvivalModel())
			{
				NumberCompletedSeen = GetWeeklySurvivalModel().NumberCompleted
			});
		}
	}
}
