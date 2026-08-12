using System.Collections.Generic;
using TWDModel;

public class WeeklyChallengeHelper
{
	public static bool IsApocalypticUnlocked
	{
		get
		{
			WeeklyChallengeModel weeklyChallengeModel = GetWeeklyChallengeModel();
			if (weeklyChallengeModel == null)
			{
				return false;
			}
			bool flag = IsChallengeOngoing();
			bool num = weeklyChallengeModel.CurrentDefinition != null && weeklyChallengeModel.CurrentDefinition.ApocalypticMapId != 0;
			bool openedApocalypseWeeklyChallenge = weeklyChallengeModel.OpenedApocalypseWeeklyChallenge;
			return num && openedApocalypseWeeklyChallenge && flag;
		}
	}

	public static bool IsNormalChallenge
	{
		get
		{
			bool result = true;
			DetailMapPopUp detailMapPopUp = SingularityMonoBehaviour<HUDManager>.Instance.GetNoCreation(UIType.DetailMapPopUp) as DetailMapPopUp;
			if (detailMapPopUp != null && detailMapPopUp.CurrentMap?.MissionSpawnPointGroup != null)
			{
				result = detailMapPopUp.CurrentMap.MissionSpawnPointGroup.Category == MapCategory.Challenge;
			}
			return result;
		}
	}

	private static bool IsApocalypticChallenge
	{
		get
		{
			bool result = false;
			DetailMapPopUp detailMapPopUp = SingularityMonoBehaviour<HUDManager>.Instance.GetNoCreation(UIType.DetailMapPopUp) as DetailMapPopUp;
			if (detailMapPopUp != null && detailMapPopUp.CurrentMap?.MissionSpawnPointGroup != null)
			{
				result = detailMapPopUp.CurrentMap.MissionSpawnPointGroup.Category == MapCategory.ApocalypticChallenge;
			}
			return result;
		}
	}

	public static bool FeaturedStarHeroActive => GameManager.Instance.gameEconomyData.GetActiveFeaturedHero(GameManager.Instance.playerModel.UtcTimeStamp) != null;

	public static WeeklyChallengeReward GetNextReward(bool personal)
	{
		if (GetWeeklyChallengeModel() != null)
		{
			return GetWeeklyChallengeModel().GetNextReward(personal);
		}
		return null;
	}

	public static WeeklyChallengeReward GetLastReward(bool personal)
	{
		if (GetWeeklyChallengeModel() != null)
		{
			WeeklyChallengeReward[] weeklyChallengeRewards = GameManager.Instance.playerModel.gameEconomyData.WeeklyChallengeRewards;
			for (int num = weeklyChallengeRewards.Length - 1; num > -1; num--)
			{
				WeeklyChallengeReward weeklyChallengeReward = weeklyChallengeRewards[num];
				if (personal)
				{
					if (weeklyChallengeReward.RewardType == WeeklyChallengeReward.ChallengeRewardType.PersonalStars && GetWeeklyChallengeModel().NumberStars > weeklyChallengeReward.Control)
					{
						return weeklyChallengeReward;
					}
				}
				else if (weeklyChallengeReward.RewardType == WeeklyChallengeReward.ChallengeRewardType.GuildStars && GetWeeklyChallengeModel().NumberStarsGuild > weeklyChallengeReward.Control)
				{
					return weeklyChallengeReward;
				}
			}
		}
		return null;
	}

	public static long GetTimeLeftToNextChallenge()
	{
		if (GetWeeklyChallengeModel() != null && GetWeeklyChallengeModel().NextWeeklyChallenge != null)
		{
			return GetWeeklyChallengeModel().TimeLeftToNextChallenge();
		}
		return 0L;
	}

	public static string GetFormatedTimeToNextChallengeStart()
	{
		return Helpers.FormatTime(GetTimeLeftToNextChallenge());
	}

	public static long GetTimeLeftToCurrentChallengeEnd()
	{
		if (GetWeeklyChallengeModel() != null && GetWeeklyChallengeModel().CurrentDefinition != null)
		{
			return GetWeeklyChallengeModel().CurrentDefinition.EndTimeMilliseconds - GameManager.Instance.playerModel.UtcTimeStamp;
		}
		return 0L;
	}

	public static string GetFormatedTimeLeftToCurrentChallengeEnd()
	{
		if (GetWeeklyChallengeModel() != null && GetWeeklyChallengeModel().CurrentDefinition != null)
		{
			return Helpers.FormatTime(GetWeeklyChallengeModel().CurrentDefinition.EndTimeMilliseconds - GameManager.Instance.playerModel.UtcTimeStamp);
		}
		return Helpers.FormatTime(0L);
	}

	public static string GetFormatedTimeLeftToUnlockNextCycle()
	{
		if (GetWeeklyChallengeModel() != null && GetWeeklyChallengeModel().IsNewCycleLockedByTimer())
		{
			return Helpers.FormatTime(GetWeeklyChallengeModel().GetMillisecondsToUnlockNewCycle());
		}
		return Helpers.FormatTime(0L);
	}

	public static WeeklyChallengeModel GetWeeklyChallengeModel()
	{
		if (GameManager.Instance != null && GameManager.Instance.playerModel != null && GameManager.Instance.playerModel.WeeklyChallenge != null && !IsLockedByCouncilLevelOrTutorial())
		{
			GameManager.Instance.StartNextChallenge();
			return GameManager.Instance.playerModel.WeeklyChallenge;
		}
		return null;
	}

	public static ApocalypseWeeklyChallengeModel GetWeeklyApocalypticChallengeModel()
	{
		if (GameManager.Instance != null && GameManager.Instance.playerModel != null && GameManager.Instance.playerModel.WeeklyChallenge != null && !IsLockedByCouncilLevelOrTutorial())
		{
			GameManager.Instance.StartNextChallenge();
			return GameManager.Instance.playerModel.ApocalypseWeeklyChallenge;
		}
		return null;
	}

	public static int GetCurrentDifficulty()
	{
		if (IsNormalChallenge)
		{
			WeeklyChallengeModel weeklyChallengeModel = GetWeeklyChallengeModel();
			if (weeklyChallengeModel != null)
			{
				if (weeklyChallengeModel.CurrentCircleDefinition != null)
				{
					return weeklyChallengeModel.CurrentCircleDefinition.Difficulty;
				}
				MapMissionGroupModel mapMissionGroupModel = weeklyChallengeModel.GetMapMissionGroupModel();
				if (mapMissionGroupModel != null)
				{
					return (int)mapMissionGroupModel.AverageRequiredSurvivorLevel();
				}
			}
		}
		else
		{
			ApocalypseWeeklyChallengeModel weeklyApocalypticChallengeModel = GetWeeklyApocalypticChallengeModel();
			if (weeklyApocalypticChallengeModel != null && weeklyApocalypticChallengeModel.CurrentCircleDefinition != null)
			{
				return weeklyApocalypticChallengeModel.CurrentCircleDefinition.Difficulty;
			}
		}
		return 0;
	}

	public static int GetGasCost()
	{
		if (GetWeeklyChallengeModel() != null && GetWeeklyChallengeModel().GetMapMissionGroupModel() != null)
		{
			return (int)GetWeeklyChallengeModel().GetMapMissionGroupModel().AverageGroupGasCost(CurrencyType.ReplayToken);
		}
		return 0;
	}

	public static FixedPoint GetProgressUntilNextDifficulty()
	{
		if (IsNormalChallenge)
		{
			if (GetWeeklyChallengeModel() != null && (float)GetWeeklyChallengeModel().TotalCyclesSinceDifficultyChanged > 0f && (float)GetWeeklyChallengeModel().NumberOfCyclesUntilNewDifficulty > 0f)
			{
				return GetWeeklyChallengeModel().TotalCyclesSinceDifficultyChanged / ((FixedPoint)GetWeeklyChallengeModel().TotalCyclesSinceDifficultyChanged + (FixedPoint)GetWeeklyChallengeModel().NumberOfCyclesUntilNewDifficulty);
			}
			return 0.0;
		}
		GetWeeklyApocalypticChallengeModel();
		return 0.0;
	}

	public static int TotalCyclesInCurrent()
	{
		if (IsNormalChallenge)
		{
			if (GetWeeklyChallengeModel() != null)
			{
				return GetWeeklyChallengeModel().TotalCyclesSinceDifficultyChanged + GetWeeklyChallengeModel().NumberOfCyclesUntilNewDifficulty;
			}
		}
		else if (GetWeeklyApocalypticChallengeModel() != null)
		{
			return 1;
		}
		return 0;
	}

	public static WeeklyChallengeReward GetCurrentCycleCompletionReward()
	{
		if (IsNormalChallenge)
		{
			WeeklyChallengeModel weeklyChallengeModel = GetWeeklyChallengeModel();
			GameEconomyData gameEconomyData = GameManager.Instance.gameEconomyData;
			if (weeklyChallengeModel != null && gameEconomyData != null)
			{
				return gameEconomyData.GetWeeklyChallengeReward(WeeklyChallengeReward.ChallengeRewardType.RoundCompletion, weeklyChallengeModel.CurrentCycle + 1, controlExactMatch: false);
			}
		}
		else
		{
			ApocalypseWeeklyChallengeModel weeklyApocalypticChallengeModel = GetWeeklyApocalypticChallengeModel();
			GameEconomyData gameEconomyData2 = GameManager.Instance.gameEconomyData;
			if (weeklyApocalypticChallengeModel != null && gameEconomyData2 != null)
			{
				return gameEconomyData2.GetWeeklyChallengeReward(WeeklyChallengeReward.ChallengeRewardType.ApocalypticRoundStars, weeklyApocalypticChallengeModel.CurrentCycle, controlExactMatch: false);
			}
		}
		return null;
	}

	public static void CalculateTotalMissions(out int completedCount, out int missionCount)
	{
		completedCount = 0;
		missionCount = 0;
		MapMissionGroupModel mapMissionGroupModel = ((!IsNormalChallenge) ? ((GetWeeklyApocalypticChallengeModel() != null) ? GetWeeklyApocalypticChallengeModel().GetMapMissionGroupModel() : null) : ((GetWeeklyChallengeModel() != null) ? GetWeeklyChallengeModel().GetMapMissionGroupModel() : null));
		if (mapMissionGroupModel == null)
		{
			return;
		}
		for (int i = 0; i < mapMissionGroupModel.Missions.Count; i++)
		{
			if (mapMissionGroupModel.Missions[i] != null)
			{
				if (!mapMissionGroupModel.Missions[i].IsMasterMission)
				{
					if (mapMissionGroupModel.Missions[i].State == MapMissionState.Unlocked && mapMissionGroupModel.Missions[i].Stars.NumberStars > 0)
					{
						completedCount++;
					}
					missionCount++;
				}
			}
			else
			{
				missionCount++;
			}
		}
	}

	public static void CalculateChallengeStars(out int collectedStars, out int maxStars)
	{
		collectedStars = 0;
		maxStars = 0;
		MapMissionGroupModel mapMissionGroupModel = ((!IsNormalChallenge) ? ((GetWeeklyApocalypticChallengeModel() != null) ? GetWeeklyApocalypticChallengeModel().GetMapMissionGroupModel() : null) : ((GetWeeklyChallengeModel() != null) ? GetWeeklyChallengeModel().GetMapMissionGroupModel() : null));
		if (mapMissionGroupModel == null)
		{
			return;
		}
		for (int i = 0; i < mapMissionGroupModel.Missions.Count; i++)
		{
			if (mapMissionGroupModel.Missions[i] != null && !mapMissionGroupModel.Missions[i].IsMasterMission && mapMissionGroupModel.Missions[i].State == MapMissionState.Unlocked)
			{
				collectedStars += mapMissionGroupModel.Missions[i].Stars.TotalStars;
				maxStars += mapMissionGroupModel.Missions[i].Stars.Stars.Length;
				if (FeaturedStarHeroActive)
				{
					maxStars++;
				}
			}
		}
	}

	public static bool HasCompletedTheFinalRound()
	{
		if (!IsNormalChallenge)
		{
			return GetWeeklyApocalypticChallengeModel().HasCompleteMaxRound();
		}
		return GetWeeklyChallengeModel().HasCompletedMaxCycles();
	}

	public static bool IsCurrentRoundFinal()
	{
		if (GameManager.Instance.gameEconomyData.GetFeature("UseChallengeRoundCap").Enabled)
		{
			if (!IsNormalChallenge)
			{
				return GetWeeklyApocalypticChallengeModel().CurrentCycle >= GameManager.Instance.gameEconomyData.ConfigData.ChallengeApocalypticModeMaxRound;
			}
			return GetWeeklyChallengeModel().CurrentCycle + 1 >= GameManager.Instance.gameEconomyData.ConfigData.ChallengeRoundCap;
		}
		return false;
	}

	public static bool CanAccessNextCycle()
	{
		if (GetWeeklyChallengeModel().IsNewCycleLockedByTimer())
		{
			return false;
		}
		return HasCompletedMissions();
	}

	public static bool HasCompletedMissions()
	{
		if (IsChallengeOngoing())
		{
			int completedCount = 0;
			int missionCount = 0;
			CalculateTotalMissions(out completedCount, out missionCount);
			if (missionCount > 0)
			{
				return completedCount >= missionCount;
			}
			return false;
		}
		return false;
	}

	public static string GetCurrentChallengeName()
	{
		if (IsNormalChallenge)
		{
			if (GetWeeklyChallengeModel() != null)
			{
				return HelpersLocalization.GetEpisodeName(GetWeeklyChallengeModel().GetMissionSpawnPointGroup());
			}
		}
		else if (GetWeeklyApocalypticChallengeModel() != null)
		{
			return HelpersLocalization.GetEpisodeName(GetWeeklyApocalypticChallengeModel().GetMissionSpawnPointGroup());
		}
		return "";
	}

	public static WeeklyChallenge GetNextChallenge()
	{
		if (GetWeeklyChallengeModel() != null)
		{
			return GetWeeklyChallengeModel().NextWeeklyChallenge;
		}
		return null;
	}

	public static string GetNextChallengeName()
	{
		if (GetWeeklyChallengeModel() != null)
		{
			WeeklyChallenge nextWeeklyChallenge = GetWeeklyChallengeModel().NextWeeklyChallenge;
			if (nextWeeklyChallenge != null)
			{
				MissionSpawnPointGroup spawnPointGroup = GameManager.Instance.gameEconomyData.MissionSpawnPointData.GetSpawnPointGroup(IsNormalChallenge ? nextWeeklyChallenge.DetailMapId : nextWeeklyChallenge.ApocalypticMapId);
				if (spawnPointGroup != null)
				{
					return HelpersLocalization.GetEpisodeName(spawnPointGroup);
				}
			}
		}
		return "";
	}

	public static bool IsChallengeOngoing()
	{
		if (!IsLockedByCouncilLevelOrTutorial() && !GetWeeklyChallengeModel().Finished)
		{
			return GetWeeklyChallengeModel().CurrentDefinition != null;
		}
		return false;
	}

	public static bool IsNextChallengePossible()
	{
		if (!IsLockedByCouncilLevelOrTutorial() && GetWeeklyChallengeModel().Finished)
		{
			return GetWeeklyChallengeModel().NextWeeklyChallenge != null;
		}
		return false;
	}

	public static bool IsLockedByCouncilLevelOrTutorial()
	{
		return IsLockedByCouncilLevel();
	}

	public static bool IsLockedByCouncilLevel()
	{
		if (GameManager.Instance.playerModel == null || GameManager.Instance.playerModel.WeeklyChallenge == null)
		{
			return true;
		}
		return GameManager.Instance.playerModel.WeeklyChallenge.IsLockedByCouncilLevel;
	}

	public static bool IsLockedByTutorial()
	{
		if (!string.IsNullOrEmpty(GameManager.Instance.gameEconomyData.ConfigData.ChallengesUnlockAtAfterTutorialPartId))
		{
			return !GameManager.Instance.playerModel.Tutorial.HasCompletedPart(GameManager.Instance.gameEconomyData.ConfigData.ChallengesUnlockAtAfterTutorialPartId);
		}
		return false;
	}

	public static void MarkDifficultyProgressionAsSeen(FixedPoint progressionSeen)
	{
		if (IsNormalChallenge)
		{
			if (progressionSeen > -1.0 && GetWeeklyChallengeModel() != null && GetWeeklyChallengeModel().LastSeenChallengeDifficultyProgression != progressionSeen)
			{
				Helpers.ExecuteCommand(new WeeklyChallengeSeenCommand(GetWeeklyChallengeModel())
				{
					DifficultyProgressionSeen = ((progressionSeen == 0.0) ? ((FixedPoint)(-1L)) : progressionSeen)
				});
			}
		}
		else if (progressionSeen > -1.0 && GetWeeklyApocalypticChallengeModel() != null && GetWeeklyApocalypticChallengeModel().LastSeenChallengeDifficultyProgression != progressionSeen)
		{
			Helpers.ExecuteCommand(new ApocalypticWeeklyChallengeSeenCommand(GetWeeklyApocalypticChallengeModel())
			{
				DifficultyProgressionSeen = ((progressionSeen == 0.0) ? ((FixedPoint)(-1L)) : progressionSeen)
			});
		}
	}

	public static void MarkDifficultyAsSeen(int difficulty)
	{
		if (IsNormalChallenge)
		{
			if ((float)difficulty > -1f && GetWeeklyChallengeModel() != null && GetWeeklyChallengeModel().LastSeenChallengeDifficulty != difficulty)
			{
				Helpers.ExecuteCommand(new WeeklyChallengeSeenCommand(GetWeeklyChallengeModel())
				{
					DifficultySeen = difficulty
				});
			}
		}
		else if ((float)difficulty > -1f && GetWeeklyApocalypticChallengeModel() != null && GetWeeklyApocalypticChallengeModel().LastSeenChallengeDifficulty != difficulty)
		{
			Helpers.ExecuteCommand(new ApocalypticWeeklyChallengeSeenCommand(GetWeeklyApocalypticChallengeModel())
			{
				DifficultySeen = difficulty
			});
		}
	}

	public static void MarkCycleAsSeen(int cycle)
	{
		if (IsNormalChallenge)
		{
			if (cycle > -1 && GetWeeklyChallengeModel() != null && GetWeeklyChallengeModel().LastSeenCycleCount != cycle)
			{
				Helpers.ExecuteCommand(new WeeklyChallengeSeenCommand(GetWeeklyChallengeModel())
				{
					CycleSeen = cycle
				});
			}
		}
		else if (cycle > -1 && GetWeeklyApocalypticChallengeModel() != null && GetWeeklyApocalypticChallengeModel().LastSeenCycleCount != cycle)
		{
			Helpers.ExecuteCommand(new ApocalypticWeeklyChallengeSeenCommand(GetWeeklyApocalypticChallengeModel())
			{
				CycleSeen = cycle
			});
		}
	}

	public static void MarkCurrentChallengeAsSeen()
	{
		if (GetWeeklyChallengeModel() != null && IsChallengeOngoing() && !GetWeeklyChallengeModel().ChallengeStartedSeen)
		{
			Helpers.ExecuteCommand(new WeeklyChallengeSeenCommand(GetWeeklyChallengeModel())
			{
				MarkChallengeStartedAsSeen = true
			});
		}
	}

	public static void MarkActiveSkipTokensAsSeen()
	{
		if (GetWeeklyChallengeModel() != null && IsChallengeOngoing() && !GetWeeklyChallengeModel().SkipTokensAvailableSeen)
		{
			Helpers.ExecuteCommand(new WeeklyChallengeSeenCommand(GetWeeklyChallengeModel())
			{
				MarkActiveSkipTokensAsSeen = true
			});
		}
	}

	public static void MarkPersonalStarsAsSeen()
	{
		if (IsNormalChallenge)
		{
			if (GetWeeklyChallengeModel() != null && !GetWeeklyChallengeModel().HasSeenLatestPersonalStars())
			{
				Helpers.ExecuteCommand(new WeeklyChallengeSeenCommand(GetWeeklyChallengeModel())
				{
					PersonalStarsSeen = GetWeeklyChallengeModel().NumberStars
				});
			}
		}
		else if (GetWeeklyApocalypticChallengeModel() != null && !GetWeeklyApocalypticChallengeModel().HasSeenLatestPersonalStars())
		{
			Helpers.ExecuteCommand(new ApocalypticWeeklyChallengeSeenCommand(GetWeeklyApocalypticChallengeModel())
			{
				PersonalStarsSeen = GetWeeklyApocalypticChallengeModel().NumberStars
			});
		}
	}

	public static void MarkGuildStarsAsSeen()
	{
		if (GetWeeklyChallengeModel() != null && !GetWeeklyChallengeModel().HasSeenLatestGuildStars())
		{
			Helpers.ExecuteCommand(new WeeklyChallengeSeenCommand(GetWeeklyChallengeModel())
			{
				GuildStarsSeen = GetWeeklyChallengeModel().NumberStarsGuild
			});
		}
	}

	public static bool WasLastCompletedMissionTheMasterMission()
	{
		if (GameManager.Instance.playerModel.IsMasterMissionUnlocked)
		{
			MapMissionGroupModel mapMissionGroupModel = (IsNormalChallenge ? GetWeeklyChallengeModel().GetMapMissionGroupModel() : GetWeeklyApocalypticChallengeModel().GetMapMissionGroupModel());
			if (mapMissionGroupModel != null)
			{
				return mapMissionGroupModel.Missions.Models.Exists((MapMissionModel t) => t.StarsFromMasterMission > 0);
			}
		}
		return false;
	}

	public static bool HasUnLockedFeaturedHero()
	{
		FeaturedHeroDefinition activeFeaturedHero = GameManager.Instance.gameEconomyData.GetActiveFeaturedHero(GameManager.Instance.playerModel.UtcTimeStamp);
		if (activeFeaturedHero != null)
		{
			SurvivorModel heroById = GameManager.Instance.playerModel.SurvivorContainer.GetHeroById(activeFeaturedHero.ActorDefinitionID);
			if (heroById != null)
			{
				return GameManager.Instance.playerModel.SurvivorContainer.HasUnLockedHero(heroById.Definition);
			}
			return false;
		}
		return false;
	}

	public static int GetChallengeActorHit(ActorModel actor)
	{
		if (!IsApocalypticChallenge)
		{
			return -1;
		}
		List<DifficultyIncrementalDebuff> challengeDebuffs = GetChallengeDebuffs();
		if (challengeDebuffs == null)
		{
			return -1;
		}
		int num = (int)(MapMissionModel.GetChallengeActorHit(actor, challengeDebuffs, GameManager.Instance.gameEconomyData.ConfigData.MinHit) / 100L);
		if (num <= 100)
		{
			return num;
		}
		return 100;
	}

	private static List<DifficultyIncrementalDebuff> GetChallengeDebuffs()
	{
		if (IsNormalChallenge)
		{
			return GameManager.Instance.playerModel?.WeeklyChallenge?.GetChallengeDebuffs();
		}
		return GameManager.Instance.playerModel?.ApocalypseWeeklyChallenge?.GetChallengeDebuffs();
	}
}
