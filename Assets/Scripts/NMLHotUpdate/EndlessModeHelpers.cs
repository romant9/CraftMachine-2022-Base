using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using BaseModel;
using TWDModel;
using TWDModel.ContentTypes;

public static class EndlessModeHelpers
{
	public const string ScoreText = "000000000000";

	public static EndlessModeCombatModel EndlessModeCombatModel => GameManager.Instance.playerModel.Combat.EndlessModeCombatModel;

	public static EndlessModeConfig EndlessModeConfig => EndlessManagerModel().EndlessModeConfig;

	public static bool IsEndlessBattleMission => GameManager.Instance.playerModel.Combat.MapCategory == MapCategory.Endless;

	public static long LeaderboardCacheTime => UtilsDateTime.MinuteInMilliseconds * 25;

	public static int CurrentSpawnCount => GetCurrentWaveWalkerTypes().Count;

	public static int OverAllWaveCount => EndlessModeCombatModel.GetCurrentOverAllWaveIndex;

	public static List<EndlessModeAttemptData> GetEndlessNormalModeAttemptData => EndlessManagerModel().EndlessNormalAttemptData;

	public static List<EndlessModeAttemptData> GetEndlessExpertModeAttemptData => EndlessManagerModel().EndlessExpertAttemptData;

	public static List<int> GetClaimedNormalProgressRewardIndex => EndlessManagerModel().ClaimedNormalProgressRewardIndex;

	public static string GetNormalCurrentEndlessModeMapName => LocalizationManager.GetText("Map.EndlessMode." + GetCurrentMissionModel(EndlessModeGameModeType.Normal)?.MissionData.DisplayTextID);

	public static string GetExpertCurrentEndlessModeMapName => LocalizationManager.GetText("Map.EndlessMode." + GetCurrentMissionModel(EndlessModeGameModeType.Expert)?.MissionData.DisplayTextID);

	public static string GetNextNormalEndlessModeMapName => LocalizationManager.GetText("Map.EndlessMode." + GetNormalNextMissionModel()?.MissionData.DisplayTextID);

	public static string GetNextExpertEndlessModeMapName => LocalizationManager.GetText("Map.EndlessMode." + GetExpertNextMissionModel()?.MissionData.DisplayTextID);

	public static string GetFormattedWaveNotificationBody
	{
		get
		{
			if (GetCurrentWaveSurviveRewardPoints() <= 0)
			{
				return "";
			}
			return "+" + GetCurrentWaveSurviveRewardPoints();
		}
	}

	public static long GetCurrentCycleTimeLeft => GetTimeLeftDependingState(EndlessModePanelState.Active);

	public static int GetEndlessTokenPriceInGold => EndlessModeConfig.MissionBaseCost * EndlessModeConfig.MissionTicketCost;

	public static int GetExpertEndlessTokenPriceInGold => EndlessModeConfig.MissionBaseCost * EndlessModeConfig.MissionTicketCostExpert;

	public static int GetCurrentGoldAttemptCount => EndlessManagerModel().CurrentGoldAttemptCount;

	public static int GetExpertCurrentGoldAttemptCount => EndlessManagerModel().CurrentExpertGoldAttemptCount;

	public static bool UsedAllGoldAttempts => GetCurrentGoldAttemptCount >= EndlessModeConfig.DailyGoldAttemptCount;

	public static bool UsedAllGoldExpertAttempts => GetExpertCurrentGoldAttemptCount >= EndlessModeConfig.DailyGoldExpertAttemptCount;

	public static bool UnSeenEndlessPassTokens
	{
		get
		{
			if (!GameManager.Instance.playerModel.Blackboard.IsToggleOn("Toggle.ToggleEndlessModeHighlightExpired") && GameManager.Instance.playerModel.Blackboard.IsToggleOn("ToggleEndlessModeIntroductionPopup"))
			{
				return GameManager.Instance.playerModel.Blackboard.IsToggleOn("ToggleEndlessModeIntroductionPopup");
			}
			return false;
		}
	}

	public static List<ActorDefinition> GetExpertModeActorDefinitions => EndlessManagerModel().GetExpertModeActorDefinitions();

	public static string GetExpertModeFinalScoreMultiplier => EndlessModeConfig.ExpertModeTotalScoreMultiplier.ToString();

	public static bool IsExpertMdeLockedByCouncilLevel => GameManager.Instance.playerModel.Camp.GetCouncilLevel() < GetExpertModeCouncilLockLevel;

	public static int GetExpertModeCouncilLockLevel => EndlessModeConfig.ExpertModeCouncilLockLevel;

	public static bool HasGeneratedExpertModeActors => EndlessManagerModel().CurrentExpertModeHeroes.Count == EndlessModeConfig.ExpertModeHeroAmount;

	public static List<WalkerType> GetCurrentWaveWalkerTypes()
	{
		return EndlessModeCombatModel?.GetCurrentSpawnWalkerTypes.Values.SelectMany((List<WalkerType> x) => x).ToList();
	}

	public static long GetCurrentAttemptScore()
	{
		long currentScore = EndlessModeCombatModel.CurrentScore;
		if (EndlessManagerModel().EndlessModeGameModeType == EndlessModeGameModeType.Expert)
		{
			if (currentScore >= 1000000)
			{
				return EndlessManagerModel().GetOverflowScoreMultiplied(currentScore, (double)EndlessModeConfig.ExpertModeTotalScoreMultiplier);
			}
			return (long)FixedPoint.Ceiling(currentScore * EndlessModeConfig.ExpertModeTotalScoreMultiplier);
		}
		return currentScore;
	}

	public static int GetCurrentAttemptRanking()
	{
		List<EndlessModeAttemptData> orderedExpertAttemptDataByScore = GetOrderedExpertAttemptDataByScore();
		EndlessModeCombatModel endlessModeCombatModel = EndlessModeCombatModel;
		if (orderedExpertAttemptDataByScore == null || endlessModeCombatModel == null)
		{
			return 0;
		}
		if (orderedExpertAttemptDataByScore.Count == 1)
		{
			return 1;
		}
		long currentScore = EndlessManagerModel().GetOverAllGameScore(endlessModeCombatModel.CurrentScore);
		return orderedExpertAttemptDataByScore.FindIndex((EndlessModeAttemptData x) => x.Score == currentScore) + 1;
	}

	public static string GetFormattedScoreText(long score)
	{
		string text = "000000000000";
		if (score.ToString().Length >= text.Length)
		{
			return score.ToString();
		}
		int length = score.ToString().Length;
		int startIndex = "000000000000".Length - length;
		return text.Remove(startIndex, length).Insert(startIndex, score.ToString());
	}

	public static string GetFormattedScoreMultiplier(FixedPoint multiplier)
	{
		return "x" + $"{(float)multiplier:0.00}";
	}

	public static int GetCurrentWaveSurviveRewardPoints()
	{
		if (IsEndlessExpertMode())
		{
			return (int)FixedPoint.Ceiling(EndlessModeCombatModel.PreviousWaveSurviveRewardPoints * EndlessModeConfig.ExpertModeTotalScoreMultiplier);
		}
		return EndlessModeCombatModel.PreviousWaveSurviveRewardPoints;
	}

	public static EndlessModeCalendarDefinition GetNextEndlessModeCalendarDefinition()
	{
		EndlessModeManagerModel endlessModeManagerModel = EndlessManagerModel();
		long utcTimeStamp = GameManager.Instance.playerModel.UtcTimeStamp;
		return GameManager.Instance.gameEconomyData.GetNextEndlessCalendarDefinition((endlessModeManagerModel.CurrentEndlessModeCalendarDefinition == null) ? 0 : endlessModeManagerModel.CurrentEndlessModeCalendarDefinition.EndTimeMilliseconds, utcTimeStamp);
	}

	public static bool IsLockedByCouncilLevel()
	{
		if (GameManager.Instance.playerModel == null || GameManager.Instance.playerModel.EndlessModeManager == null)
		{
			return true;
		}
		return GameManager.Instance.playerModel.EndlessModeManager.IsLockedByCouncilLevel;
	}

	public static EndlessModePanelState GetEndlessHubPanelState()
	{
		if (IsLockedByCouncilLevel())
		{
			return EndlessModePanelState.Locked;
		}
		if (IsEndlessModeActive())
		{
			return EndlessModePanelState.Active;
		}
		return EndlessModePanelState.InActive;
	}

	public static long GetTimeLeftDependingState(EndlessModePanelState currentState)
	{
		EndlessModeManagerModel endlessModeManagerModel = EndlessManagerModel();
		if (currentState == EndlessModePanelState.InActive)
		{
			EndlessModeCalendarDefinition nextEndlessModeCalendarDefinition = GetNextEndlessModeCalendarDefinition();
			if (nextEndlessModeCalendarDefinition == null)
			{
				return 0L;
			}
			return nextEndlessModeCalendarDefinition.StartTimeMilliseconds - GameManager.Instance.playerModel.UtcTimeStamp;
		}
		if (endlessModeManagerModel?.CurrentEndlessModeCalendarDefinition != null)
		{
			return endlessModeManagerModel.CurrentEndlessModeCalendarDefinition.EndTimeMilliseconds - GameManager.Instance.playerModel.UtcTimeStamp;
		}
		return 0L;
	}

	public static List<EndlessModeAttemptData> GetOrderedNormalAttemptDataByScore()
	{
		return GetEndlessNormalModeAttemptData?.OrderByDescending((EndlessModeAttemptData x) => x.Score).ThenBy((EndlessModeAttemptData x) => x.TimeStamp).ToList();
	}

	public static List<EndlessModeAttemptData> GetOrderedExpertAttemptDataByScore()
	{
		return GetEndlessExpertModeAttemptData?.OrderByDescending((EndlessModeAttemptData x) => x.Score).ThenBy((EndlessModeAttemptData x) => x.TimeStamp).ToList();
	}

	public static int GetCurrentNormalAttemptCount()
	{
		return GetEndlessNormalModeAttemptData?.Count ?? 0;
	}

	public static int GetCurrentExpertAttemptCount()
	{
		return GetEndlessExpertModeAttemptData?.Count ?? 0;
	}

	public static long GetAttemptsScoreNormal()
	{
		long num = 0L;
		int currentNormalAttemptCount = GetCurrentNormalAttemptCount();
		int attemptsToSumForFinalScoreNormal = EndlessModeConfig.AttemptsToSumForFinalScoreNormal;
		List<EndlessModeAttemptData> getEndlessNormalModeAttemptData = GetEndlessNormalModeAttemptData;
		for (int i = 0; i < Math.Min(currentNormalAttemptCount, attemptsToSumForFinalScoreNormal); i++)
		{
			num += getEndlessNormalModeAttemptData[i].Score;
		}
		return num;
	}

	public static bool IsScoreGetMaxReward(long score)
	{
		long num = GetAttemptsScoreNormal();
		int currentNormalAttemptCount = GetCurrentNormalAttemptCount();
		int attemptsToSumForFinalScoreNormal = EndlessModeConfig.AttemptsToSumForFinalScoreNormal;
		List<EndlessModeAttemptData> getEndlessNormalModeAttemptData = GetEndlessNormalModeAttemptData;
		if (currentNormalAttemptCount >= attemptsToSumForFinalScoreNormal)
		{
			num -= getEndlessNormalModeAttemptData[attemptsToSumForFinalScoreNormal - 1].Score;
		}
		num += score;
		return num >= GetMaxEndlessNormalModeScore();
	}

	public static string GetFormattedOverAllAttemptsScoreNormal()
	{
		return GetFormattedScoreText(GetAttemptsScoreNormal());
	}

	public static string GetFormattedOverAllAttemptsScoreExpert()
	{
		return GetFormattedScoreText(GetAllAttemptsScoreExpert());
	}

	public static long GetAllAttemptsScoreExpert()
	{
		long num = 0L;
		int currentExpertAttemptCount = GetCurrentExpertAttemptCount();
		int attemptsToSumForFinalScoreExpert = EndlessModeConfig.AttemptsToSumForFinalScoreExpert;
		List<EndlessModeAttemptData> getEndlessExpertModeAttemptData = GetEndlessExpertModeAttemptData;
		for (int i = 0; i < Math.Min(currentExpertAttemptCount, attemptsToSumForFinalScoreExpert); i++)
		{
			num += getEndlessExpertModeAttemptData[i].Score;
		}
		return num;
	}

	public static EndlessModeAttemptData GetMaxAttemptDataExpertBySurvivorClass(SurvivorClass survivorClass)
	{
		Dictionary<SurvivorClass, List<EndlessModeAttemptData>> endlessExpertLeaderSurvivorClassAttemptData = EndlessManagerModel().EndlessExpertLeaderSurvivorClassAttemptData;
		long num = 0L;
		EndlessModeAttemptData result = new EndlessModeAttemptData();
		if (endlessExpertLeaderSurvivorClassAttemptData.TryGetValue(survivorClass, out var value))
		{
			foreach (EndlessModeAttemptData item in value)
			{
				if (num < item.Score)
				{
					num = item.Score;
					result = item;
				}
			}
		}
		return result;
	}

	public static List<EndlessModeAttemptData> GetEndlessExpertModeAttemptDataListBySurvivorClass(SurvivorClass survivorClass)
	{
		if (EndlessManagerModel().EndlessExpertLeaderSurvivorClassAttemptData.TryGetValue(survivorClass, out var value))
		{
			return value;
		}
		return null;
	}

	public static Rewards GetRankedLeaderBoardRewardByRank(int rank)
	{
		EndlessModeCalendarDefinition currentEndlessModeCalendarDefinition = EndlessManagerModel().CurrentEndlessModeCalendarDefinition;
		if (currentEndlessModeCalendarDefinition == null)
		{
			return null;
		}
		string setId = currentEndlessModeCalendarDefinition.LeaderBoardRewardSetID;
		if (!string.IsNullOrEmpty(setId))
		{
			EndlessModeLeaderBoardReward endlessModeLeaderBoardReward = Array.Find(GameManager.Instance.gameEconomyData.EndlessModeLeaderBoardRewards, (EndlessModeLeaderBoardReward x) => x.RewardSetID == setId && x.RewardBracket == rank.ToString() && x.RewardType == EndlessModeLeaderBoardRewardType.Ranked);
			if (endlessModeLeaderBoardReward != null)
			{
				return new Rewards(endlessModeLeaderBoardReward.Rewards);
			}
		}
		return null;
	}

	public static EndlessModeLeaderBoardReward GetEndlessModeLeaderBoardRewardByRank(int rank)
	{
		EndlessModeCalendarDefinition currentEndlessModeCalendarDefinition = EndlessManagerModel().CurrentEndlessModeCalendarDefinition;
		if (currentEndlessModeCalendarDefinition == null)
		{
			return null;
		}
		string setId = currentEndlessModeCalendarDefinition.LeaderBoardRewardSetID;
		if (!string.IsNullOrEmpty(setId))
		{
			EndlessModeLeaderBoardReward endlessModeLeaderBoardReward = Array.Find(GameManager.Instance.gameEconomyData.EndlessModeLeaderBoardRewards, delegate(EndlessModeLeaderBoardReward x)
			{
				if (x.RewardType == EndlessModeLeaderBoardRewardType.Ranked && x.RewardBracket.Contains("-"))
				{
					string[] array = x.RewardBracket.Split("-");
					if (int.TryParse(array[0], out var result) && int.TryParse(array[1], out var result2) && rank >= result && rank <= result2)
					{
						if (x.RewardSetID == setId)
						{
							return x.RewardType == EndlessModeLeaderBoardRewardType.Ranked;
						}
						return false;
					}
					return false;
				}
				return x.RewardSetID == setId && x.RewardBracket == rank.ToString() && x.RewardType == EndlessModeLeaderBoardRewardType.Ranked;
			});
			if (endlessModeLeaderBoardReward != null)
			{
				return endlessModeLeaderBoardReward;
			}
		}
		return null;
	}

	public static EndlessModeLeaderBoardReward[] GetCurrentCycleLeaderBoardRewards()
	{
		EndlessModeManagerModel endlessModeManagerModel = EndlessManagerModel();
		if (endlessModeManagerModel != null)
		{
			EndlessModeLeaderBoardReward[] endlessModeLeaderBoardRewards = GameManager.Instance.gameEconomyData.EndlessModeLeaderBoardRewards;
			if (endlessModeLeaderBoardRewards != null)
			{
				string leaderBoardRewardSetId = endlessModeManagerModel.CurrentEndlessModeCalendarDefinition.LeaderBoardRewardSetID;
				return Array.FindAll(endlessModeLeaderBoardRewards, (EndlessModeLeaderBoardReward x) => x.RewardSetID == leaderBoardRewardSetId);
			}
		}
		return null;
	}

	public static string GetLocalisedRewardTierTitle(int rank)
	{
		return rank switch
		{
			1 => LocalizationManager.GetText("Endless.RewardTier.1st.Long"),
			2 => LocalizationManager.GetText("Endless.RewardTier.2nd.Long"),
			3 => LocalizationManager.GetText("Endless.RewardTier.3rd.Long"),
			_ => "",
		};
	}

	private static bool DoWeHaveUnclaimedAttemptRewards()
	{
		if (EndlessManagerModel().PendingAttemptRewards == null)
		{
			return EndlessManagerModel().PendingAttemptRegularRewards != null;
		}
		return true;
	}

	public static string GetLocalisedLeaderBoardRewardBracketTitle(string rewardBracket, EndlessModeLeaderBoardRewardType endlessModeLeaderBoardRewardType)
	{
		if (rewardBracket.Length == 1)
		{
			if (endlessModeLeaderBoardRewardType == EndlessModeLeaderBoardRewardType.Ranked)
			{
				string text = LocalizationManager.GetText("Endless.RewardTier.NumericRange{Parameters}", rewardBracket, rewardBracket);
				return text.Substring(0, text.IndexOf('-'));
			}
			return LocalizationManager.GetText("Endless.RewardTier.PercentageLessThan{Threshold}", rewardBracket);
		}
		string[] array = rewardBracket.Split('-');
		if (int.TryParse(array[0], out var result) && int.TryParse(array[1], out var result2))
		{
			if (endlessModeLeaderBoardRewardType == EndlessModeLeaderBoardRewardType.Ranked)
			{
				return LocalizationManager.GetText("Endless.RewardTier.NumericRange{Parameters}", result, result2);
			}
			return LocalizationManager.GetText("Endless.RewardTier.PercentageLessThan{Threshold}", result2);
		}
		return "";
	}

	public static string GetLastPlaceLocalisedLeaderBoardRewardBracketTitle()
	{
		int num = GetCurrentCycleLeaderBoardRewards().Length - 2;
		EndlessModeLeaderBoardReward obj = GetCurrentCycleLeaderBoardRewards()[num];
		string text = ((obj != null) ? obj.RewardBracket.Split('-')[1] : null);
		return LocalizationManager.GetText("Endless.RewardTier.PercentageMoreThan{Threshold}", text);
	}

	public static MapMissionModel GetCurrentMissionModel(EndlessModeGameModeType type)
	{
		MapMissionModel mapMissionModel = null;
		MapContainerModel mapContainerModel = GameManager.Instance.playerModel.MapContainerModel;
		string text = ((type != EndlessModeGameModeType.Expert) ? EndlessManagerModel().CurrentEndlessModeCalendarDefinition?.MapID : EndlessManagerModel().CurrentEndlessModeCalendarDefinition?.ExpertMapID);
		if (string.IsNullOrEmpty(text))
		{
			return null;
		}
		foreach (MapMissionGroupModel mapMissionGroup in mapContainerModel.MapMissionGroups)
		{
			foreach (MapMissionModel mission in mapMissionGroup.Missions)
			{
				if (mission?.MissionData?.Id == text)
				{
					mapMissionModel = mission;
					break;
				}
			}
			if (mapMissionModel != null)
			{
				break;
			}
		}
		return mapMissionModel;
	}

	public static MapMissionModel GetNormalNextMissionModel()
	{
		MapMissionModel mapMissionModel = null;
		MapContainerModel mapContainerModel = GameManager.Instance.playerModel.MapContainerModel;
		string text = GetNextEndlessModeCalendarDefinition()?.MapID;
		if (string.IsNullOrEmpty(text))
		{
			return null;
		}
		foreach (MapMissionGroupModel mapMissionGroup in mapContainerModel.MapMissionGroups)
		{
			foreach (MapMissionModel mission in mapMissionGroup.Missions)
			{
				if (mission.MissionData?.Id == text)
				{
					mapMissionModel = mission;
					break;
				}
			}
			if (mapMissionModel != null)
			{
				break;
			}
		}
		return mapMissionModel;
	}

	public static MapMissionModel GetExpertNextMissionModel()
	{
		MapMissionModel mapMissionModel = null;
		MapContainerModel mapContainerModel = GameManager.Instance.playerModel.MapContainerModel;
		string text = GetNextEndlessModeCalendarDefinition()?.ExpertMapID;
		if (string.IsNullOrEmpty(text))
		{
			return null;
		}
		foreach (MapMissionGroupModel mapMissionGroup in mapContainerModel.MapMissionGroups)
		{
			foreach (MapMissionModel mission in mapMissionGroup.Missions)
			{
				if (mission.MissionData?.Id == text)
				{
					mapMissionModel = mission;
					break;
				}
			}
			if (mapMissionModel != null)
			{
				break;
			}
		}
		return mapMissionModel;
	}

	public static int GetMissionDifficulty()
	{
		return Math.Max(GameManager.Instance.playerModel.SurvivorContainer.GetEndlessBaseDifficultyFromSurvivors(), GameManager.Instance.playerModel.Equipment.GetHighestEquipableEquipmentLevel());
	}

	public static List<WalkerType> GetEndlessBattleMissionWalkerTypes()
	{
		GameEconomyData gameEconomyData = GameManager.Instance.gameEconomyData;
		List<WalkerType> list = new List<WalkerType>();
		List<string> list2 = new List<string>();
		string spawnSetupId = EndlessManagerModel().CurrentEndlessModeCalendarDefinition.SpawnSetupID;
		List<string> source = (from x in Array.FindAll(gameEconomyData.EndlessModelSpawnDefinitions, (EndlessModeSpawnDefinition x) => x.SpawnSetupID == spawnSetupId)
			select x.SpawnCompositionID).ToList();
		source = source.Select((string x) => ReplaceNewlines(x).Replace(';', ',')).ToList();
		for (int num = 0; num < source.Count; num++)
		{
			string[] compositionDefinitions = source[num].Split(',');
			int j;
			for (j = 0; j < compositionDefinitions.Length; j++)
			{
				string spawmComposition = Array.Find(gameEconomyData.EndlessModeSpawnCompositionDefinitions, (EndlessModeSpawnCompositionDefinition x) => x.ID == compositionDefinitions[j]).SpawmComposition;
				if (!string.IsNullOrEmpty(spawmComposition))
				{
					spawmComposition = spawmComposition.Replace(';', ',');
					List<string> collection = spawmComposition.Split(',').ToList();
					list2.AddRange(collection);
				}
			}
		}
		list2 = list2.Distinct().ToList();
		for (int num2 = 0; num2 < list2.Count; num2++)
		{
			string catalogId = ReplaceNewlines(list2[num2]);
			List<WalkerType> list3 = (from x in Array.Find(gameEconomyData.EndlessModeWaveCatalogs, (EndlessModeWaveCatalog x) => x.ID == catalogId).SpawnComposition.Split(',').ToList()
				select Enum.Parse(typeof(WalkerType), x)).Cast<WalkerType>().ToList();
			for (int num3 = 0; num3 < list3.Count; num3++)
			{
				WalkerType item = list3[num3];
				if (!list.Contains(item) && !item.ToString().Contains("Boss"))
				{
					list.Add(item);
				}
			}
		}
		return list;
		static string ReplaceNewlines(string text)
		{
			return Regex.Replace(text, "\\t|\\n|\\r", "");
		}
	}

	public static List<WalkerType> GetEndlessExpertBattleMissionWalkerTypes()
	{
		GameEconomyData gameEconomyData = GameManager.Instance.gameEconomyData;
		List<WalkerType> list = new List<WalkerType>();
		List<string> list2 = new List<string>();
		string spawnSetupId = EndlessManagerModel().CurrentEndlessModeCalendarDefinition.SpawnSetIDExpert;
		List<string> source = (from x in Array.FindAll(gameEconomyData.EndlessModelSpawnDefinitions, (EndlessModeSpawnDefinition x) => x.SpawnSetupID == spawnSetupId)
			select x.SpawnCompositionID).ToList();
		source = source.Select((string x) => ReplaceNewlines(x).Replace(';', ',')).ToList();
		for (int num = 0; num < source.Count; num++)
		{
			string[] compositionDefinitions = source[num].Split(',');
			int j;
			for (j = 0; j < compositionDefinitions.Length; j++)
			{
				string spawmComposition = Array.Find(gameEconomyData.EndlessModeSpawnCompositionDefinitions, (EndlessModeSpawnCompositionDefinition x) => x.ID == compositionDefinitions[j]).SpawmComposition;
				if (!string.IsNullOrEmpty(spawmComposition))
				{
					spawmComposition = spawmComposition.Replace(';', ',');
					List<string> collection = spawmComposition.Split(',').ToList();
					list2.AddRange(collection);
				}
			}
		}
		list2 = list2.Distinct().ToList();
		for (int num2 = 0; num2 < list2.Count; num2++)
		{
			string catalogId = ReplaceNewlines(list2[num2]);
			List<WalkerType> list3 = (from x in Array.Find(gameEconomyData.EndlessModeWaveCatalogs, (EndlessModeWaveCatalog x) => x.ID == catalogId).SpawnComposition.Split(',').ToList()
				select Enum.Parse(typeof(WalkerType), x)).Cast<WalkerType>().ToList();
			for (int num3 = 0; num3 < list3.Count; num3++)
			{
				WalkerType item = list3[num3];
				if (!list.Contains(item) && !item.ToString().Contains("Boss"))
				{
					list.Add(item);
				}
			}
		}
		return list;
		static string ReplaceNewlines(string text)
		{
			return Regex.Replace(text, "\\t|\\n|\\r", "");
		}
	}

	public static bool CanAttemptNormalMode()
	{
		MapMissionModel currentMissionModel = GetCurrentMissionModel(EndlessModeGameModeType.Normal);
		int value = GameManager.Instance.playerModel.GetCurrency(CurrencyType.Diamonds).Value;
		if (!currentMissionModel.GetStartMissionCashier().CanAfford() && (value < GetEndlessTokenPriceInGold || GetCurrentGoldAttemptCount >= EndlessModeConfig.DailyGoldAttemptCount))
		{
			if (EndlessModeConfig.DailyGoldAttemptCount == 0)
			{
				return currentMissionModel.GetStartMissionCashier().CanAfford();
			}
			return false;
		}
		return true;
	}

	public static bool CanAttemptExpertMode()
	{
		MapMissionModel currentMissionModel = GetCurrentMissionModel(EndlessModeGameModeType.Expert);
		int value = GameManager.Instance.playerModel.GetCurrency(CurrencyType.Diamonds).Value;
		if (!currentMissionModel.GetStartMissionExpertModeCashier().CanAfford() && (value < GetExpertEndlessTokenPriceInGold || GetExpertCurrentGoldAttemptCount >= EndlessModeConfig.DailyGoldExpertAttemptCount))
		{
			if (EndlessModeConfig.DailyGoldExpertAttemptCount == 0)
			{
				return currentMissionModel.GetStartMissionExpertModeCashier().CanAfford();
			}
			return false;
		}
		return true;
	}

	public static int GetLatestNormalEndlessModeAttemptIndex()
	{
		if (GetEndlessNormalModeAttemptData.Count > 0)
		{
			EndlessModeAttemptData attemptData = GetEndlessNormalModeAttemptData.OrderBy((EndlessModeAttemptData x) => Math.Abs(x.TimeStamp - GameManager.Instance.playerModel.UtcTimeStamp)).First();
			return GetEndlessNormalModeAttemptData.FindIndex((EndlessModeAttemptData x) => x.TimeStamp == attemptData.TimeStamp);
		}
		return -1;
	}

	public static int GetLatestExpertEndlessModeAttemptIndex()
	{
		if (GetEndlessExpertModeAttemptData.Count > 0)
		{
			EndlessModeAttemptData attemptData = GetEndlessExpertModeAttemptData.OrderBy((EndlessModeAttemptData x) => Math.Abs(x.TimeStamp - GameManager.Instance.playerModel.UtcTimeStamp)).First();
			return GetEndlessExpertModeAttemptData.FindIndex((EndlessModeAttemptData x) => x.TimeStamp == attemptData.TimeStamp);
		}
		return -1;
	}

	public static EndlessModeManagerModel EndlessManagerModel()
	{
		if (!IsLockedByCouncilLevel())
		{
			GameManager.Instance.StartNextEndlessCycle();
		}
		return GameManager.Instance.playerModel.EndlessModeManager;
	}

	public static bool IsEndlessModeActive()
	{
		if (EndlessManagerModel().CurrentEndlessModeCalendarDefinition == null || EndlessManagerModel().CurrentEndlessModeCalendarDefinition.EndTimeMilliseconds < GameManager.Instance.playerModel.UtcTimeStamp)
		{
			return false;
		}
		return true;
	}

	public static void CheckForUnclaimedRewards()
	{
		if (!DoWeHaveUnclaimedAttemptRewards())
		{
			return;
		}
		OpenAttemptRewardBoxCommand openAttemptRewardBoxCommand = new OpenAttemptRewardBoxCommand();
		if (Helpers.ExecuteCommand(openAttemptRewardBoxCommand) == TWDModelResult.OK)
		{
			IAPConfirmPopupNew iAPConfirmPopupNew = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.IAPConfirmPopupNew) as IAPConfirmPopupNew;
			if (iAPConfirmPopupNew != null)
			{
				iAPConfirmPopupNew.OpenForRewards(openAttemptRewardBoxCommand.Rewards.RewardsList);
				iAPConfirmPopupNew.SetContent(LocalizationManager.GetText("Popup.IAPConfirm.Title.GenericReward"), null);
			}
		}
	}

	public static void GetCurrentLeaderboardPosition(Action<LeaderboardPosition> callback)
	{
		int identifier = EndlessManagerModel().CurrentEndlessModeCalendarDefinition.Identifier;
		GetLeaderboardPosition(callback, identifier);
	}

	public static void GetPreviousLeaderboardPosition(Action<LeaderboardPosition> callback)
	{
		int leaderboardId = EndlessManagerModel().CurrentEndlessModeCalendarDefinition.Identifier - 1;
		GetLeaderboardPosition(callback, leaderboardId);
	}

	private static void GetLeaderboardPosition(Action<LeaderboardPosition> callback, int leaderboardId)
	{
		LeaderboardPositionProvider leaderboardPositionProvider = GameManager.Instance.CachedLeaderboardsManager.GetLeaderboardPositionProvider(leaderboardId);
		if (leaderboardPositionProvider == null)
		{
			Debug.LogError($"Leaderboard provider not found for {leaderboardId}");
			callback?.Invoke(null);
		}
		else
		{
			leaderboardPositionProvider.GetLeaderboardPosition(callback);
		}
	}

	public static int GetStartingDifficulty(EndlessModeGameModeType endlessModeGameModeType)
	{
		int result = 0;
		if (EndlessModeConfig != null)
		{
			if (endlessModeGameModeType == EndlessModeGameModeType.Normal)
			{
				result = EndlessModeConfig.NormalModeStartingLevel;
			}
			if (endlessModeGameModeType == EndlessModeGameModeType.Expert)
			{
				result = EndlessModeConfig.NormalModeStartingLevel;
			}
		}
		return result;
	}

	public static int GetMaxEndlessNormalModeScore()
	{
		return GameManager.Instance.gameEconomyData.EndlessModeNormalRewardDefinitons.Max((EndlessModeNormalRewardDefiniton reward) => reward.Score);
	}

	public static bool IsSurvivorAvailableForCombat(SurvivorModel survivorModel, MapMissionModel missionModel)
	{
		if (missionModel == null)
		{
			return true;
		}
		bool flag = missionModel.IsEndlessMission && EndlessManagerModel().EndlessModeGameModeType == EndlessModeGameModeType.Expert;
		if (!GameManager.Instance.gameEconomyData.ConfigData.DisableOutpostHeroLimits && GameManager.Instance.playerModel.SurvivorContainer.OutpostDefendingSurvivors.Contains(survivorModel) && flag)
		{
			return false;
		}
		if (!flag || !survivorModel.Definition.ID.Contains("Hero"))
		{
			return true;
		}
		return GetExpertModeActorDefinitions.Exists((ActorDefinition x) => x.ID == survivorModel.Definition.ID);
	}

	public static bool IsEndlessExpertMode()
	{
		return EndlessManagerModel().EndlessModeGameModeType == EndlessModeGameModeType.Expert;
	}

	public static bool HasExpertModeHero(ActorDefinition actorDefinition)
	{
		return GameManager.Instance.playerModel.SurvivorContainer.Survivors.Any((SurvivorModel x) => x.ActorDefinitionID == actorDefinition.ID);
	}

	public static string GetLeaderBoardRewardBySurvivorClass(EndlessModeLeaderBoardReward endlessModeLeaderBoardReward, SurvivorClass survivorClass)
	{
		return survivorClass switch
		{
			SurvivorClass.None => endlessModeLeaderBoardReward.Rewards,
			SurvivorClass.Assault => endlessModeLeaderBoardReward.RewardAssault,
			SurvivorClass.Bruiser => endlessModeLeaderBoardReward.RewardBruiser,
			SurvivorClass.Hunter => endlessModeLeaderBoardReward.RewardHunter,
			SurvivorClass.Scout => endlessModeLeaderBoardReward.RewardScout,
			SurvivorClass.Warrior => endlessModeLeaderBoardReward.RewardWarrior,
			SurvivorClass.Shooter => endlessModeLeaderBoardReward.RewardShooter,
			_ => endlessModeLeaderBoardReward.Rewards,
		};
	}
}
