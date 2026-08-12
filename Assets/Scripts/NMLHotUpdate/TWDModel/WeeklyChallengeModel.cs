using System;
using System.Collections.Generic;
using System.Linq;
using BaseModel;
using Newtonsoft.Json;

namespace TWDModel
{
	public class WeeklyChallengeModel : TWDModelObject
	{
		public const string GuildRewardAdded = "GuildRewardAdded";

		public const string ChallengeCycleStarted = "ChallengeCycleStarted";

		public const string RewardsDedicatedRandomId = "WeeklyChallengeDedicatedRandom";

		public List<IncrementalDifficultyEffectDefinition> AppendDifficultyEffect { get; set; }

		public int RerollApocalypseBuffCount { get; set; }

		public List<WeeklyChallengeApocalypseBuff> PendingSelectApocalypseBuffs { get; set; }

		public List<WeeklyChallengeApocalypseBuff> weeklyChallengeApocalypseBuffs { get; set; }

		public int Id { get; set; }

		public WeeklyChallengeZoneModel WeeklyChallengeZoneModel { get; set; }

		public int NumberStars { get; private set; }

		public List<GuildStarData> NumberStarsPerGuild { get; private set; }

		public int LastSeenNumberStars { get; set; }

		public FixedPoint PersonalHighScoreGrantedCompletionRatio { get; set; }

		public int PersonalHighScoreAtBeginningOfChallenge { get; set; }

		public int AllTimeNumberStars { get; private set; }

		public int PreviousNumberStars { get; private set; }

		public int PreviousChallengeHighestDifficulty { get; set; }

		public int LastSeenChallengeDifficulty { get; set; }

		public FixedPoint LastSeenChallengeDifficultyProgression { get; set; }

		public int LastSeenCycleCount { get; set; }

		public ModelList<LootEntry> Rewards { get; set; }

		public int LastNumberOfGuildStars { get; private set; }

		public int LastSeenNumberOfGuildStars { get; set; }

		public int LastCycleBonusStars { get; private set; }

		public List<int> ClaimedGuildRewardStars { get; private set; }

		public int CurrentCycle { get; set; }

		public int PendingSkipTokens { get; set; }

		public int PendingSkipTokensCollectedInChallengeId { get; set; }

		public int ActiveSkipTokens { get; set; }

		[JsonIgnore]
		public bool DoubleRewardsActive => ActiveSkipTokens > 0;

		public int PreviousChallengeSkipTokens { get; set; }

		public bool SkipTokensAvailableSeen { get; set; }

		public FixedPoint DifficultyBeforeSkips { get; set; }

		public int CycleCountWithinTimerPeriod { get; set; }

		public int LastPTSUpdateCycle { get; set; }

		public int CurrentPotentialTeamStrength { get; set; }

		public int PTSAtChallengeStart { get; set; }

		public int CurrentRequiredSurvivorLevel { get; set; }

		public int NumberOfCyclesUntilNewDifficulty { get; set; }

		public int TotalCyclesSinceDifficultyChanged { get; set; }

		public long NewCycleTimerPeriodTimeStamp { get; set; }

		public long NewCycleTimerLockedTimeStamp { get; set; }

		public bool ChallengeStartedSeen { get; set; }

		public bool OpenedApocalypseWeeklyChallenge { get; set; }

		public bool ChallengeEndedSeen { get; set; }

		[JsonIgnore]
		public int NumberStarsGuild
		{
			get
			{
				if (base.manager.Player.IsGuildMember)
				{
					GuildModel guildModel = base.manager.Player.GuildModel;
					if (guildModel != null)
					{
						return guildModel.GetChallengeStars(Id.ToString() ?? "");
					}
				}
				return 0;
			}
		}

		[JsonIgnore]
		public WeeklyChallenge CurrentDefinition => base.gameEconomyData.GetWeeklyChallenge(Id);

		[JsonIgnore]
		public WeeklyChallengeDeBuffSet CurrentCircleDefinition => base.gameEconomyData.GetWeeklyChallengeDeBuffSet(CurrentCycle + 1);

		[JsonIgnore]
		public bool Finished
		{
			get
			{
				if (Id == -1)
				{
					return true;
				}
				if (CurrentDefinition == null)
				{
					return true;
				}
				return CurrentDefinition.EndTimeMilliseconds < base.manager.Player.UtcTimeStamp;
			}
		}

		[JsonIgnore]
		public bool CanCollectRewards => GetRewardsPerType(LootEntryType.ChallengeGuildReward, LootEntryType.ChallengePersonalReward, LootEntryType.ChallengeRoundCompletionReward, LootEntryType.ChallengePersonalHighScore, LootEntryType.ChallengeGuildAchiever).Count > 0;

		[JsonIgnore]
		public List<LootEntry> GetCollectRewards => GetRewardsPerType(LootEntryType.ChallengeGuildReward, LootEntryType.ChallengePersonalReward, LootEntryType.ChallengeRoundCompletionReward, LootEntryType.ChallengePersonalHighScore, LootEntryType.ChallengeGuildAchiever);

		[JsonIgnore]
		public bool CanCollectApocalypticRewards => GetRewardsPerType(LootEntryType.ApocalypticStars, LootEntryType.ApocalypticRoundStars).Count > 0;

		[JsonIgnore]
		public List<LootEntry> GetCollectApocalypticRewards => GetRewardsPerType(LootEntryType.ApocalypticStars, LootEntryType.ApocalypticRoundStars);

		[JsonIgnore]
		public LootEntry FirstCollectableGuildReward
		{
			get
			{
				if (Rewards != null)
				{
					for (int i = 0; i < Rewards.Count; i++)
					{
						if (Rewards[i] != null && Rewards[i].Type == LootEntryType.ChallengeGuildReward)
						{
							return Rewards[i];
						}
					}
				}
				return null;
			}
		}

		[JsonIgnore]
		public LootEntry FirstCollectablePersonalReward
		{
			get
			{
				if (Rewards != null)
				{
					for (int i = 0; i < Rewards.Count; i++)
					{
						if (Rewards[i] != null && Rewards[i].Type == LootEntryType.ChallengePersonalReward)
						{
							return Rewards[i];
						}
					}
				}
				return null;
			}
		}

		[JsonIgnore]
		public WeeklyChallenge NextWeeklyChallenge => base.gameEconomyData.GetNextWeeklyChallege((CurrentDefinition == null) ? 0 : CurrentDefinition.EndTimeMilliseconds, base.manager.Player.UtcTimeStamp);

		[JsonIgnore]
		public bool CanPlayWeeklyChallenge
		{
			get
			{
				if (!Finished)
				{
					return true;
				}
				if (CanPlayNextWeeklyChallenge)
				{
					return true;
				}
				return false;
			}
		}

		[JsonIgnore]
		public bool CanPlayNextWeeklyChallenge
		{
			get
			{
				WeeklyChallenge nextWeeklyChallenge = NextWeeklyChallenge;
				if (nextWeeklyChallenge != null && base.manager.Player.UtcTimeStamp >= nextWeeklyChallenge.StartTimeMilliseconds)
				{
					return base.manager.Player.UtcTimeStamp < nextWeeklyChallenge.EndTimeMilliseconds;
				}
				return false;
			}
		}

		[JsonIgnore]
		public bool IsLockedByCouncilLevel => base.manager.CampModel.GetCouncilLevel() < base.manager.Player.gameEconomyData.ConfigData.ChallengesUnlockAtCouncilLevel;

		[JsonIgnore]
		public bool HasShownCycleEndedOnClient { get; set; }

		[JsonIgnore]
		public bool CanAccessMasterMission => base.manager.Player.CouncilLevel >= base.manager.GameEconomyData.ConfigData.ChallangeMasterMissionCouncilLevelUnlock;

		[JsonIgnore]
		public int NumberStarsInCurrentGuild
		{
			get
			{
				if (NumberStarsPerGuild == null)
				{
					return 0;
				}
				string text = "";
				if (base.manager.Player.IsGuildMember)
				{
					text = base.manager.Player.GuildId;
				}
				for (int i = 0; i < NumberStarsPerGuild.Count; i++)
				{
					if (NumberStarsPerGuild[i].GuildId == text)
					{
						return NumberStarsPerGuild[i].StarCount;
					}
				}
				return 0;
			}
		}

		public override void Initialize()
		{
			base.Initialize();
			Id = -1;
			Rewards = new ModelList<LootEntry>();
			ClaimedGuildRewardStars = new List<int>();
			CurrentCycle = -1;
			PersonalHighScoreAtBeginningOfChallenge = 0;
			NewCycleTimerPeriodTimeStamp = 0L;
			WeeklyChallengeZoneModel = new WeeklyChallengeZoneModel
			{
				Id2ZoneIdDict = new Dictionary<int, int>()
			};
			StartNewCycle();
		}

		public override void Start()
		{
			base.Start();
			if (WeeklyChallengeZoneModel == null)
			{
				WeeklyChallengeZoneModel = new WeeklyChallengeZoneModel
				{
					Id2ZoneIdDict = new Dictionary<int, int>()
				};
			}
		}

		public override bool IsValid()
		{
			return true;
		}

		public int GetSkipTokenGain(int roundNumber)
		{
			int result = 0;
			WeeklyChallengeRoundPassConfig currentCycleRoundPassConfig = GetCurrentCycleRoundPassConfig();
			if (currentCycleRoundPassConfig != null)
			{
				result = (((roundNumber + 1) % currentCycleRoundPassConfig.RoundsToSkipToken == 0) ? 1 : 0);
			}
			return result;
		}

		public int DetermineReceivedSkipTokenCount(int currentCycle)
		{
			if (currentCycle < 0)
			{
				base.Debug.LogWarning("currentCycle had an invalid value: was negative.");
				currentCycle = 0;
			}
			if (base.gameEconomyData.GetFeature("DynamicRoundPassAmount").Enabled)
			{
				return GetSkipTokenGain(currentCycle);
			}
			WeeklyChallenge weeklyChallenge = base.gameEconomyData.GetWeeklyChallenge(Id);
			if (weeklyChallenge == null)
			{
				return 0;
			}
			if ((currentCycle + 1) % weeklyChallenge.RoundsToSkipToken != 0)
			{
				return 0;
			}
			return 1;
		}

		public WeeklyChallengeRoundPassConfig GetCurrentCycleRoundPassConfig()
		{
			return base.manager.GameEconomyData.GetChallengeRoundPassConfig(CurrentCycle);
		}

		public int GetCurrentCycleRoundsToSkipToken()
		{
			if (!base.gameEconomyData.GetFeature("DynamicRoundPassAmount").Enabled)
			{
				return CurrentDefinition.RoundsToSkipToken;
			}
			WeeklyChallengeRoundPassConfig currentCycleRoundPassConfig = GetCurrentCycleRoundPassConfig();
			if (currentCycleRoundPassConfig == null)
			{
				base.Debug.LogWarning("RoundPassConfig is null! Returning default RoundsToSkipToken");
				return CurrentDefinition.RoundsToSkipToken;
			}
			return currentCycleRoundPassConfig.RoundsToSkipToken;
		}

		public int GetRoundsToNextSkipToken()
		{
			WeeklyChallengeRoundPassConfig currentCycleRoundPassConfig = GetCurrentCycleRoundPassConfig();
			if (currentCycleRoundPassConfig == null)
			{
				return 0;
			}
			return currentCycleRoundPassConfig.RoundsToSkipToken - (CurrentCycle - currentCycleRoundPassConfig.FromRound) % currentCycleRoundPassConfig.RoundsToSkipToken;
		}

		private void AddAutomaticallyReceivedSkipTokens(int roundsSkipped)
		{
			if (roundsSkipped > 0)
			{
				int num = 0;
				for (int i = 0; i < roundsSkipped; i++)
				{
					num += DetermineReceivedSkipTokenCount(i);
				}
				base.manager.Metrics.AddFind().AddSkipTokens(num, PendingSkipTokens).AddChallenge()
					.AddSkipRounds(roundsSkipped, (int)DifficultyBeforeSkips)
					.Send();
				PendingSkipTokens += num;
				PendingSkipTokensCollectedInChallengeId = Id;
			}
		}

		public void AddCollectedSkipTokens()
		{
			int num = DetermineReceivedSkipTokenCount(CurrentCycle);
			base.manager.Metrics.AddFind().AddSkipTokens(num, PendingSkipTokens).AddChallenge()
				.AddChallengeRoundReward()
				.Send();
			PendingSkipTokens += num;
			PendingSkipTokensCollectedInChallengeId = Id;
		}

		public int CalculateRoundsToNextSkipToken()
		{
			if (base.gameEconomyData.GetFeature("DynamicRoundPassAmount").Enabled)
			{
				return GetRoundsToNextSkipToken();
			}
			WeeklyChallenge weeklyChallenge = base.gameEconomyData.GetWeeklyChallenge(Id);
			if (weeklyChallenge == null || weeklyChallenge.RoundsToSkipToken == 0)
			{
				return 0;
			}
			return weeklyChallenge.RoundsToSkipToken - CurrentCycle % weeklyChallenge.RoundsToSkipToken;
		}

		public MissionSpawnPointGroup GetMissionSpawnPointGroup()
		{
			if (CurrentDefinition != null)
			{
				int detailMapId = CurrentDefinition.DetailMapId;
				MissionSpawnPointGroup spawnPointGroup = base.gameEconomyData.MissionSpawnPointData.GetSpawnPointGroup(detailMapId);
				if (spawnPointGroup == null)
				{
					base.manager.Debug.LogError("Could not find spawn point group for '" + detailMapId + "' cannot start challenge!");
				}
				return spawnPointGroup;
			}
			return null;
		}

		public MapMissionGroupModel GetMapMissionGroupModel()
		{
			if (CurrentDefinition != null)
			{
				int detailMapId = CurrentDefinition.DetailMapId;
				MapMissionGroupModel missionGroupModelForSpawnPointGroup = base.manager.Player.MapContainerModel.GetMissionGroupModelForSpawnPointGroup(detailMapId);
				if (missionGroupModelForSpawnPointGroup == null)
				{
					base.manager.Debug.LogError("Could not find group model for detailmap id = " + detailMapId);
				}
				return missionGroupModelForSpawnPointGroup;
			}
			return null;
		}

		public MapMissionGroupModel GetCurrentOrNextMapMissionGroupModel()
		{
			MapMissionGroupModel mapMissionGroupModel = GetMapMissionGroupModel();
			if (mapMissionGroupModel == null)
			{
				WeeklyChallenge nextWeeklyChallenge = NextWeeklyChallenge;
				if (nextWeeklyChallenge != null)
				{
					MissionSpawnPointGroup spawnPointGroup = base.gameEconomyData.MissionSpawnPointData.GetSpawnPointGroup(nextWeeklyChallenge.DetailMapId);
					if (spawnPointGroup != null)
					{
						mapMissionGroupModel = base.manager.Player.MapContainerModel.GetMissionGroupModelForSpawnPointGroup(spawnPointGroup);
					}
				}
			}
			return mapMissionGroupModel;
		}

		public void Reset(int identifier)
		{
			ChallengeStartedSeen = false;
			ChallengeEndedSeen = false;
			SkipTokensAvailableSeen = false;
			if (Id != -1)
			{
				if (base.manager.Player.IsGuildMember)
				{
					CheckGuildStarsReward();
				}
				base.manager.Player.MapContainerModel.GetMissionGroupModelForSpawnPointGroup(base.gameEconomyData.MissionSpawnPointData.GetSpawnPointGroup(CurrentDefinition.DetailMapId))?.RemoveMissions();
				base.manager.Player.MapContainerModel.GetMissionGroupModelForSpawnPointGroup(base.gameEconomyData.MissionSpawnPointData.GetSpawnPointGroup(CurrentDefinition.ApocalypticMapId))?.RemoveMissions();
			}
			if (base.manager.Player != null)
			{
				base.manager.Player.HighestWeeklyChallengeScore = Math.Max(base.manager.Player.HighestWeeklyChallengeScore, NumberStars);
				PersonalHighScoreAtBeginningOfChallenge = base.manager.Player.HighestWeeklyChallengeScore;
				if (PersonalHighScoreAtBeginningOfChallenge < 1 && base.manager.GameEconomyData.ConfigData != null && base.manager.GameEconomyData.ConfigData.ChallengeMinimumPersonalHighScore != null && base.manager.GameEconomyData.ConfigData.ChallengeMinimumPersonalHighScore.Count > 0)
				{
					int index = Math.Max(0, Math.Min(base.manager.Player.TeamPotentialStrength - 1, base.manager.GameEconomyData.ConfigData.ChallengeMinimumPersonalHighScore.Count - 1));
					PersonalHighScoreAtBeginningOfChallenge = base.manager.GameEconomyData.ConfigData.ChallengeMinimumPersonalHighScore[index];
				}
			}
			PreviousChallengeHighestDifficulty = CurrentRequiredSurvivorLevel;
			Id = identifier;
			if (base.manager.Player != null)
			{
				GameEconomyData obj = base.manager.GameEconomyData;
				if (obj != null && obj.ConfigData?.WeeklyChallengeWarZone == true)
				{
					WeeklyChallengeWarZone weeklyChallengeWarZoneByCouncilLevel = base.manager.GameEconomyData.GetWeeklyChallengeWarZoneByCouncilLevel(base.manager.Player.CouncilLevel);
					if (weeklyChallengeWarZoneByCouncilLevel != null && WeeklyChallengeZoneModel != null)
					{
						WeeklyChallengeZoneModel.FeatureEnabled = true;
						WeeklyChallengeZoneModel.Id2ZoneIdDict.Add(Id, weeklyChallengeWarZoneByCouncilLevel.Id);
					}
				}
			}
			PreviousNumberStars = NumberStars;
			NumberStars = 0;
			if (NumberStarsPerGuild == null)
			{
				NumberStarsPerGuild = new List<GuildStarData>();
			}
			NumberStarsPerGuild.Clear();
			LastSeenNumberStars = 0;
			LastSeenChallengeDifficulty = 0;
			LastSeenChallengeDifficultyProgression = 0.0;
			LastSeenCycleCount = 0;
			if (LastNumberOfGuildStars != -1)
			{
				LastNumberOfGuildStars = 0;
				LastSeenNumberOfGuildStars = 0;
			}
			ClaimedGuildRewardStars = new List<int>();
			PersonalHighScoreGrantedCompletionRatio = 0L;
			MissionSpawnPointGroup spawnPointGroup = base.gameEconomyData.MissionSpawnPointData.GetSpawnPointGroup(CurrentDefinition.DetailMapId);
			if (spawnPointGroup != null)
			{
				MapMissionGroupModel missionGroupModelForSpawnPointGroup = base.manager.Player.MapContainerModel.GetMissionGroupModelForSpawnPointGroup(spawnPointGroup);
				missionGroupModelForSpawnPointGroup.RemoveMissions();
				base.manager.Player.MapContainerModel.SpawnMissionsForGroup(spawnPointGroup);
				foreach (MapMissionModel mission in missionGroupModelForSpawnPointGroup.Missions)
				{
					mission.ChallengeId = Id;
				}
			}
			UpdateGuildChallenge();
			CurrentCycle = -1;
			LastPTSUpdateCycle = -1;
			NewCycleTimerPeriodTimeStamp = 0L;
			HasShownCycleEndedOnClient = false;
			StartNewCycle();
		}

		public void StartNewCycle()
		{
			if (!base.manager.GameEconomyData.GetFeature("ChallengeCycleEnabled").Enabled)
			{
				return;
			}
			long num = base.manager.Player.UtcTimeStamp - NewCycleTimerPeriodTimeStamp;
			if (base.manager.Player.gameEconomyData.ConfigData.ChallengeRoundTimerPeriod > 0 && num > base.manager.Player.gameEconomyData.ConfigData.ChallengeRoundTimerPeriod)
			{
				CycleCountWithinTimerPeriod = 0;
				NewCycleTimerPeriodTimeStamp = base.manager.Player.UtcTimeStamp;
			}
			else
			{
				CycleCountWithinTimerPeriod++;
			}
			if (base.manager.Player.gameEconomyData.ConfigData.ChallengeTimerFreeCount > 0 && CycleCountWithinTimerPeriod >= base.manager.Player.gameEconomyData.ConfigData.ChallengeTimerFreeCount)
			{
				NewCycleTimerLockedTimeStamp = base.manager.Player.UtcTimeStamp;
			}
			int num2 = 1;
			if (CurrentCycle < 0)
			{
				if (PendingSkipTokensCollectedInChallengeId + 1 == Id)
				{
					ActiveSkipTokens = PendingSkipTokens;
				}
				else
				{
					ActiveSkipTokens = 0;
				}
				PreviousChallengeSkipTokens = ActiveSkipTokens;
				PendingSkipTokens = 0;
				PendingSkipTokensCollectedInChallengeId = 0;
				num2 = ActiveSkipTokens + 1;
				SkipTokensAvailableSeen = ActiveSkipTokens == 0;
			}
			else if (ActiveSkipTokens > 0)
			{
				int activeSkipTokens = ActiveSkipTokens - 1;
				ActiveSkipTokens = activeSkipTokens;
			}
			for (int i = 0; i < num2; i++)
			{
				CurrentCycle++;
				bool flag = false;
				if (CurrentCycle == 0)
				{
					FixedPoint fixedPoint = ((base.manager.GameEconomyData.ConfigData.ChallengeDifficultyStartDifficultyMultiplier != 0L) ? base.manager.GameEconomyData.ConfigData.ChallengeDifficultyStartDifficultyMultiplier : ((FixedPoint)1L));
					CurrentPotentialTeamStrength = base.manager.Player.TeamPotentialStrength;
					PTSAtChallengeStart = CurrentPotentialTeamStrength;
					FixedPoint fixedPoint2 = 0.5;
					CurrentRequiredSurvivorLevel = (int)(CurrentPotentialTeamStrength * fixedPoint + fixedPoint2);
					TotalCyclesSinceDifficultyChanged = 0;
					LastPTSUpdateCycle = 0;
					flag = true;
				}
				else
				{
					TotalCyclesSinceDifficultyChanged++;
					NumberOfCyclesUntilNewDifficulty--;
					if (NumberOfCyclesUntilNewDifficulty <= 0 && CurrentRequiredSurvivorLevel < base.manager.GameEconomyData.ConfigData.ChallengeDifficultyHardLimit)
					{
						CurrentRequiredSurvivorLevel++;
						flag = true;
						TotalCyclesSinceDifficultyChanged = 0;
						if (CurrentCycle - LastPTSUpdateCycle > base.manager.GameEconomyData.ConfigData.ChallengeDifficultyCyclesAmountToRecalculatePTS)
						{
							CurrentPotentialTeamStrength = base.manager.Player.TeamPotentialStrength;
							LastPTSUpdateCycle = CurrentCycle;
						}
					}
				}
				if (flag)
				{
					FixedPoint fixedPoint3 = ((base.manager.GameEconomyData.ConfigData.ChallengeDifficultyCycleLowLevelPTSRatio != 0L) ? base.manager.GameEconomyData.ConfigData.ChallengeDifficultyCycleLowLevelPTSRatio : ((FixedPoint)1L));
					int num3 = Math.Max(0, CurrentCycle - 1);
					NumberOfCyclesUntilNewDifficulty = ((base.manager.GameEconomyData.ConfigData.ChallengeDifficultyCycleNormalSpeed <= 0) ? 1 : base.manager.GameEconomyData.ConfigData.ChallengeDifficultyCycleNormalSpeed);
					if (CurrentRequiredSurvivorLevel <= CurrentPotentialTeamStrength - base.manager.GameEconomyData.ConfigData.ChallengeDifficultyUnderThreshold)
					{
						NumberOfCyclesUntilNewDifficulty = 1;
					}
					else if (CurrentRequiredSurvivorLevel >= base.manager.GameEconomyData.ConfigData.ChallengeDifficultyEndBrakeLevel)
					{
						NumberOfCyclesUntilNewDifficulty = base.manager.GameEconomyData.ConfigData.ChallengeDifficultyEndBrakeNumCycles;
					}
					else if (num3 >= (int)FixedPoint.Ceiling(CurrentPotentialTeamStrength * fixedPoint3))
					{
						NumberOfCyclesUntilNewDifficulty = 1;
					}
				}
				if (CurrentCycle == 0)
				{
					RecalculateMissionDifficulties();
					MapMissionGroupModel mapMissionGroupModel = GetMapMissionGroupModel();
					DifficultyBeforeSkips = 0L;
					if (mapMissionGroupModel != null)
					{
						DifficultyBeforeSkips = mapMissionGroupModel.AverageRequiredSurvivorLevel();
					}
					base.manager.Metrics.AddSpend().AddSkipTokens(-ActiveSkipTokens, PendingSkipTokens).AddChallenge()
						.AddSkipRounds(ActiveSkipTokens, (int)DifficultyBeforeSkips)
						.Send();
					AddAutomaticallyReceivedSkipTokens(ActiveSkipTokens);
				}
				base.manager.Player.RFMGiftManager.TriggerRFMEvent(RFMEvent.challengeLevelReached, CurrentCycle.ToString());
			}
			base.manager.Debug.Log("StartNewCycle (" + CurrentCycle + ") Difficulty=" + CurrentRequiredSurvivorLevel + " PTS=" + CurrentPotentialTeamStrength + " TotalCyclesSinceDifficultyChanged=" + TotalCyclesSinceDifficultyChanged + " NumberOfCyclesUntilNewDifficulty=" + NumberOfCyclesUntilNewDifficulty);
			OpenedApocalypseWeeklyChallenge |= HasCompletedMaxCycles();
			SpawnMasterMission();
			RecalculateMissionDifficulties();
			HasShownCycleEndedOnClient = false;
			NotifyChange("ChallengeCycleStarted");
		}

		private void RecalculateMissionDifficulties()
		{
			MapMissionGroupModel mapMissionGroupModel = GetMapMissionGroupModel();
			if (mapMissionGroupModel != null)
			{
				int count = mapMissionGroupModel.Missions.Count;
				for (int i = 0; i < count; i++)
				{
					MapMissionModel mapMissionModel = mapMissionGroupModel.Missions[i];
					if (mapMissionModel != null && mapMissionModel.Stars != null)
					{
						mapMissionModel.State = MapMissionState.Unlocked;
						mapMissionModel.Stars.ResetStarsForNewChallengeCycle();
						mapMissionModel.RecalculateWeeklyChallengeMissionLevel();
						mapMissionModel.CompletedFromMasterMission = false;
						mapMissionModel.StarsFromMasterMission = 0;
					}
				}
			}
			base.manager.Player.HighestWeeklyChallengeDifficulty = Math.Max(base.manager.Player.HighestWeeklyChallengeDifficulty, CurrentRequiredSurvivorLevel);
		}

		private void SpawnMasterMission()
		{
			MapMissionGroupModel mapMissionGroupModel = GetMapMissionGroupModel();
			if (mapMissionGroupModel != null)
			{
				mapMissionGroupModel.RemoveMission(mapMissionGroupModel.Missions.Models.Find((MapMissionModel t) => t.IsMasterMission));
				MapMissionModel mapMissionModel = base.manager.Player.MapContainerModel.CreateMissionModel(mapMissionGroupModel.Missions[CurrentCycle % mapMissionGroupModel.Missions.Count].MissionSpawnPoint);
				mapMissionModel.ChallengeId = Id;
				mapMissionModel.IsMasterMission = true;
				mapMissionGroupModel.AddMission(mapMissionModel);
			}
		}

		public void CompleteMissionsInCycle()
		{
			MapMissionGroupModel mapMissionGroupModel = GetMapMissionGroupModel();
			if (mapMissionGroupModel == null)
			{
				return;
			}
			int count = mapMissionGroupModel.Missions.Count;
			for (int i = 0; i < count; i++)
			{
				MapMissionModel mapMissionModel = mapMissionGroupModel.Missions[i];
				if (mapMissionModel != null && !mapMissionModel.IsMasterMission)
				{
					mapMissionModel.GiveStars(giveStarsFromMasterMissionCompletion: true);
				}
			}
			UpdateChallengePlayerLeaderboards();
			UpdateGuildChallenge(updateStars: true);
		}

		public bool IsNewCycleLockedByTimer()
		{
			if (base.manager.Player.gameEconomyData.ConfigData.ChallengeTimerFreeCount > 0 && base.manager.Player.gameEconomyData.ConfigData.ChallengeRoundTimer > 0)
			{
				if (base.manager.Player.UtcTimeStamp - NewCycleTimerLockedTimeStamp < base.manager.Player.gameEconomyData.ConfigData.ChallengeRoundTimer)
				{
					return CycleCountWithinTimerPeriod >= base.manager.Player.gameEconomyData.ConfigData.ChallengeTimerFreeCount;
				}
				return false;
			}
			return false;
		}

		public bool HasCompletedMaxCycles()
		{
			if (base.manager.GameEconomyData.GetFeature("UseChallengeRoundCap").Enabled)
			{
				return CurrentCycle >= base.manager.GameEconomyData.ConfigData.ChallengeRoundCap;
			}
			return false;
		}

		public List<DifficultyIncrementalDebuff> GetChallengeDebuffs()
		{
			if (CurrentCircleDefinition != null)
			{
				return CurrentCircleDefinition.DebuffConfigs;
			}
			return new List<DifficultyIncrementalDebuff>();
		}

		public bool IsDebufCycles()
		{
			return CurrentCircleDefinition != null;
		}

		public long GetMillisecondsToUnlockNewCycle()
		{
			if (IsNewCycleLockedByTimer())
			{
				return base.manager.Player.gameEconomyData.ConfigData.ChallengeRoundTimer - (base.manager.Player.UtcTimeStamp - NewCycleTimerLockedTimeStamp);
			}
			return 0L;
		}

		[ModelAvailableTimer]
		public long TimeLeftToNextChallenge()
		{
			if (NextWeeklyChallenge != null)
			{
				return NextWeeklyChallenge.StartTimeMilliseconds - base.manager.Player.UtcTimeStamp;
			}
			return 0L;
		}

		public bool CanStartNextCycle()
		{
			if (IsNewCycleLockedByTimer())
			{
				return false;
			}
			if (HasCompletedMaxCycles())
			{
				return false;
			}
			MapMissionGroupModel mapMissionGroupModel = GetMapMissionGroupModel();
			if (mapMissionGroupModel != null)
			{
				int count = mapMissionGroupModel.Missions.Count;
				for (int i = 0; i < count; i++)
				{
					MapMissionModel mapMissionModel = mapMissionGroupModel.Missions[i];
					if (!mapMissionModel.IsMasterMission && (mapMissionModel.State != MapMissionState.Unlocked || mapMissionModel.Stars.NumberStars < 1))
					{
						return false;
					}
				}
				return true;
			}
			return false;
		}

		public void UpdateGuildChallenge(bool updateStars = false)
		{
			PlayerModel player = base.manager.Player;
			if (!player.IsGuildMember || base.manager.ServerService == null || !(base.manager.GetGroupModel(player.GuildId) is GuildModel guildModel))
			{
				return;
			}
			string text = Id.ToString();
			bool flag = guildModel.CurrentChallengeId != text && !player.WeeklyChallenge.Finished;
			if (flag)
			{
				StartChallengeGroupCommand groupCommand = new StartChallengeGroupCommand(Id.ToString());
				base.manager.SendGroupCommand(groupCommand);
			}
			if (!updateStars)
			{
				return;
			}
			GuildMemberInfo memberInfo = guildModel.GetMemberInfo(player.HashedId);
			if (memberInfo != null && (flag || (guildModel.CurrentChallengeId == text && memberInfo.CurrentChallengeStars != NumberStarsInCurrentGuild)))
			{
				if (guildModel.CurrentChallengeId == text && memberInfo.CurrentChallengeStars > NumberStarsInCurrentGuild)
				{
					base.Debug.LogWarning("Member has more stars in guild model than player model: " + memberInfo.CurrentChallengeStars + " vs " + NumberStarsInCurrentGuild + " (challenge " + guildModel.CurrentChallengeId + ")");
				}
				UpdateGuildStars();
			}
		}

		private void AddStarsToGuild(string guildId, int amount)
		{
			if (NumberStarsPerGuild == null)
			{
				NumberStarsPerGuild = new List<GuildStarData>();
			}
			for (int i = 0; i < NumberStarsPerGuild.Count; i++)
			{
				if (NumberStarsPerGuild[i].GuildId == guildId)
				{
					NumberStarsPerGuild[i].StarCount += amount;
					return;
				}
			}
			NumberStarsPerGuild.Add(new GuildStarData(guildId, amount));
		}

		public void ResolveSkipTokensForMission(ref int starCount)
		{
			if (ActiveSkipTokens > 0)
			{
				starCount *= 2;
			}
		}

		public void AddPersonalStars(int amount, bool givegiveStarsFromMasterMissionCompletion = false)
		{
			ResolveSkipTokensForMission(ref amount);
			List<WeeklyChallengeReward> personalRewardsBetween = GetPersonalRewardsBetween(NumberStars, NumberStars + amount);
			NumberStars += amount;
			AllTimeNumberStars += amount;
			string guildId = ((base.manager.Player.GuildId != null) ? base.manager.Player.GuildId : "");
			AddStarsToGuild(guildId, amount);
			if (personalRewardsBetween != null)
			{
				for (int i = 0; i < personalRewardsBetween.Count; i++)
				{
					WeeklyChallengeReward challengeReward = personalRewardsBetween[i];
					AddReward(challengeReward);
				}
			}
			if (!givegiveStarsFromMasterMissionCompletion)
			{
				UpdateGuildStars();
			}
		}

		public void AddPersonalHighScoreRewards(PlayerModel player)
		{
			if (PersonalHighScoreAtBeginningOfChallenge <= 0)
			{
				return;
			}
			FixedPoint fixedPoint = (FixedPoint)NumberStars / (FixedPoint)PersonalHighScoreAtBeginningOfChallenge;
			Rewards weeklyChallengePersonalHighScoreRewards = base.manager.GameEconomyData.GetWeeklyChallengePersonalHighScoreRewards(player.Level, fixedPoint, PersonalHighScoreGrantedCompletionRatio);
			if (weeklyChallengePersonalHighScoreRewards == null)
			{
				return;
			}
			PersonalHighScoreGrantedCompletionRatio = fixedPoint;
			LootEntry lootEntry = null;
			for (int i = 0; i < weeklyChallengePersonalHighScoreRewards.RewardsList.Count; i++)
			{
				IReward reward = weeklyChallengePersonalHighScoreRewards.RewardsList[i];
				if (reward != null && reward.Type != RewardType.TradeCrate)
				{
					lootEntry = base.manager.Player.LootManager.CreateCurrencyLoot(reward, DropType.Gold, DropCurrenciesProbabilitiesDefinition.DropCurrency.AnyCurrency);
				}
				else if (reward != null && reward.Type == RewardType.TradeCrate && reward is RewardTradeCrate)
				{
					RewardTradeCrate rewardTradeCrate = reward as RewardTradeCrate;
					lootEntry = base.manager.Player.LootManager.CreateTradeCrateLoot(rewardTradeCrate.TradeCrateId, DropEventDefinition.DropEventType.MissionChallenge, ignoreCummulativeProbability: true, "WeeklyChallengeDedicatedRandom");
				}
				if (lootEntry != null)
				{
					lootEntry.Type = LootEntryType.ChallengePersonalHighScore;
					Rewards.Add(lootEntry);
				}
			}
		}

		public void UpdateGuildStars()
		{
			if (base.manager.ServerService != null && base.manager.Player.IsGuildMember)
			{
				AddChallengeStarsGroupCommand addChallengeStarsGroupCommand = new AddChallengeStarsGroupCommand();
				addChallengeStarsGroupCommand.ChallengeId = Id.ToString();
				addChallengeStarsGroupCommand.MemberId = base.manager.Player.HashedId;
				addChallengeStarsGroupCommand.NewCurrentChallengeStars = NumberStarsInCurrentGuild;
				addChallengeStarsGroupCommand.IsChallengeFinished = Finished;
				base.manager.SendGroupCommand(addChallengeStarsGroupCommand);
			}
		}

		public void CheckGuildStarsReward()
		{
			foreach (WeeklyChallengeReward guildReward in GetGuildRewards())
			{
				AddReward(guildReward);
			}
			LastNumberOfGuildStars = NumberStarsGuild;
		}

		public int DetermineFinalRewardCurrencyAmount(RewardCurrency reward)
		{
			int starCount = reward.Amount;
			ResolveSkipTokensForMission(ref starCount);
			return starCount;
		}

		public int DetermineFinalRewardCurrencyAmount(RewardSkipChallange reward)
		{
			int starCount = reward.Amount;
			ResolveSkipTokensForMission(ref starCount);
			return starCount;
		}

		public int DetermineFinalRewardStarAmount(int starCount)
		{
			int starCount2 = starCount;
			ResolveSkipTokensForMission(ref starCount2);
			return starCount2;
		}

		public void AddCycleCompleteRewards(bool isSkip = false)
		{
			if (base.manager == null || base.manager.GameEconomyData == null || base.manager.Player == null || !base.manager.GameEconomyData.GetFeature("ChallengeCycleEnabled").Enabled)
			{
				return;
			}
			WeeklyChallengeReward weeklyChallengeReward = base.manager.GameEconomyData.GetWeeklyChallengeReward(WeeklyChallengeReward.ChallengeRewardType.RoundCompletion, CurrentCycle + 1, controlExactMatch: false);
			if (weeklyChallengeReward != null)
			{
				AddReward(weeklyChallengeReward);
			}
			AddCollectedSkipTokens();
			int num = (LastCycleBonusStars = GetBonusStarsAtCurrentCycleCompletion());
			if (num <= 0)
			{
				return;
			}
			int starCount = num;
			ResolveSkipTokensForMission(ref starCount);
			base.manager.Metrics.AddFind().AddBonusStars(starCount).AddChallenge()
				.AddChallengeRoundReward()
				.Send();
			AddPersonalStars(num, isSkip);
			if (base.manager.ServerService != null)
			{
				LeaderboardEntry entry = Leaderboards.CreateCurrentChallengeLeaderboardEntry(base.manager.Player);
				string challengeId = Id.ToString();
				base.manager.ServerService.SaveLeaderboardEntry(Leaderboards.GetPlayerChallengeWeeklyLeaderboardName(challengeId), entry);
				if (base.manager.Player.Country != null)
				{
					base.manager.ServerService.SaveLeaderboardEntry(Leaderboards.GetPlayerChallengeWeeklyCountryLeaderboardName(base.manager.Player.Country, challengeId), entry);
				}
				if (WeeklyChallengeZoneModel != null && WeeklyChallengeZoneModel.FeatureEnabled)
				{
					int zoneIdById = WeeklyChallengeZoneModel.GetZoneIdById(Id);
					base.manager.ServerService.SaveLeaderboardEntry(Leaderboards.GetPlayerChallengeWeeklyLeaderboardNameWithZoneId(challengeId, zoneIdById), entry);
					if (base.manager.Player.Country != null)
					{
						base.manager.ServerService.SaveLeaderboardEntry(Leaderboards.GetPlayerChallengeWeeklyCountryLeaderboardNameWithZoneId(base.manager.Player.Country, challengeId, zoneIdById), entry);
					}
				}
			}
			base.manager.Player.MissionStatistics.AddStars(num);
		}

		public int GetBonusStarsAtCurrentCycleCompletion()
		{
			if (base.manager != null && base.manager.GameEconomyData != null && base.manager.Player != null)
			{
				WeeklyChallengeReward weeklyChallengeReward = base.manager.GameEconomyData.GetWeeklyChallengeReward(WeeklyChallengeReward.ChallengeRewardType.RoundCompletion, CurrentCycle + 1, controlExactMatch: false);
				if (weeklyChallengeReward != null)
				{
					FixedPoint fixedPoint = weeklyChallengeReward.BonusStarsMultiplier;
					if (fixedPoint == 0.0)
					{
						fixedPoint = 1.0;
					}
					return (int)(CurrentRequiredSurvivorLevel * fixedPoint);
				}
			}
			return 0;
		}

		public bool ShouldUpdateGuildRewards()
		{
			if (LastNumberOfGuildStars != -1)
			{
				return GetGuildRewards().Count > 0;
			}
			return true;
		}

		public void AddReward(WeeklyChallengeReward challengeReward)
		{
			bool flag = false;
			if (challengeReward != null && challengeReward.RewardEntries != null && challengeReward.RewardEntries.RewardsList != null)
			{
				LootEntry lootEntry = null;
				for (int i = 0; i < challengeReward.RewardEntries.RewardsList.Count; i++)
				{
					IReward reward = challengeReward.RewardEntries.RewardsList[i];
					if (reward == null)
					{
						continue;
					}
					if (reward.Type == RewardType.TradeCrate && reward is RewardTradeCrate)
					{
						RewardTradeCrate rewardTradeCrate = reward as RewardTradeCrate;
						lootEntry = base.manager.Player.LootManager.CreateTradeCrateLoot(rewardTradeCrate.TradeCrateId, DropEventDefinition.DropEventType.MissionChallenge, ignoreCummulativeProbability: true, "WeeklyChallengeDedicatedRandom");
					}
					else if (reward.Type != RewardType.Equipment)
					{
						lootEntry = ((reward.Type != RewardType.Avatars) ? base.manager.Player.LootManager.CreateCurrencyLoot(reward, DropType.Gold, DropCurrenciesProbabilitiesDefinition.DropCurrency.AnyCurrency) : base.manager.Player.LootManager.CreateAvatarsLootEntry(reward, DropType.Gold));
					}
					else
					{
						RewardEquipment reward2 = reward as RewardEquipment;
						lootEntry = base.manager.Player.LootManager.CreateConsumablesLoot(reward2, DropType.Gold);
						lootEntry.DropEventDefinition = base.manager.GameEconomyData.GetDropEvent(DropEventDefinition.DropEventType.MissionChallenge, DropEventDefinition.DropEventContext.Normal, DropEventDefinition.DropEventTag.ChallengeCrateGold);
					}
					if (lootEntry != null)
					{
						lootEntry.Type = LootManagerModel.GetLootEntryTypeFromChallengeReward(challengeReward);
						if (lootEntry.Type == LootEntryType.ChallengeRoundCompletionReward)
						{
							int starCount = 1;
							ResolveSkipTokensForMission(ref starCount);
							lootEntry.ChallengeRoundCompletionRewardMultiplier = starCount;
						}
						lootEntry.Control = challengeReward.Control;
						Rewards.Add(lootEntry);
					}
				}
				flag = true;
			}
			if (flag)
			{
				NotifyChange("GuildRewardAdded");
			}
			if (challengeReward.RewardType == WeeklyChallengeReward.ChallengeRewardType.GuildStars)
			{
				if (ClaimedGuildRewardStars == null)
				{
					ClaimedGuildRewardStars = new List<int>();
				}
				ClaimedGuildRewardStars.Add(challengeReward.Control);
			}
		}

		public LootEntry GiveReward()
		{
			LootEntry lootEntry = null;
			if (Rewards.Count > 0)
			{
				lootEntry = Rewards[0];
				base.manager.Player.LootManager.GiveLoot(lootEntry);
				ReportChallengeAnalytics(lootEntry);
				Rewards.RemoveAt(0);
			}
			return lootEntry;
		}

		private void ReportChallengeAnalytics(LootEntry lootEntry)
		{
			if (base.manager != null && base.manager.Player != null && lootEntry != null)
			{
				Metrics metrics = base.manager.Metrics;
				metrics.AddFind().AddLoot(lootEntry).AddChallenge();
				if (lootEntry.DropEventDefinition != null && lootEntry.DropEventDefinition.EventType == DropEventDefinition.DropEventType.MissionChallenge)
				{
					metrics.AddLootCrate(lootEntry);
				}
				if (lootEntry.Type == LootEntryType.ChallengePersonalReward)
				{
					metrics.AddChallengeReward(lootEntry);
					metrics.ResourceChangeUsedReason = "ChallengePersonalReward";
				}
				else if (lootEntry.Type == LootEntryType.ChallengeRoundCompletionReward)
				{
					metrics.AddChallengeRoundReward();
					metrics.ResourceChangeUsedReason = "ChallengeRoundReward";
				}
				else if (lootEntry.Type == LootEntryType.ChallengeGuildReward)
				{
					metrics.AddChallengeReward(lootEntry);
					metrics.ResourceChangeUsedReason = "ChallengeGuildReward";
				}
				metrics.TdEventType = "Find_resource_Challenge";
				metrics.TdEventPropertyTypes = new List<string> { "ChallengeReward", "ChallengeReward_challenge_type", "RadioCall_Acceptance", "ChallengeReward_challenge_equip" };
				metrics.SendTdEvent();
				metrics.Send();
			}
		}

		public List<WeeklyChallengeReward> GetPersonalRewardsBetween(int fromStars, int toStars)
		{
			List<WeeklyChallengeReward> list = new List<WeeklyChallengeReward>();
			for (int i = 0; i < ((base.gameEconomyData.WeeklyChallengeRewards != null) ? base.gameEconomyData.WeeklyChallengeRewards.Length : 0); i++)
			{
				WeeklyChallengeReward weeklyChallengeReward = base.gameEconomyData.WeeklyChallengeRewards[i];
				if (weeklyChallengeReward.RewardType == WeeklyChallengeReward.ChallengeRewardType.PersonalStars && weeklyChallengeReward.Control > fromStars && weeklyChallengeReward.Control <= toStars)
				{
					list.Add(weeklyChallengeReward);
				}
			}
			return list;
		}

		public WeeklyChallengeReward GetNextReward(bool personal)
		{
			for (int i = 0; i < ((base.gameEconomyData.WeeklyChallengeRewards != null) ? base.gameEconomyData.WeeklyChallengeRewards.Length : 0); i++)
			{
				WeeklyChallengeReward weeklyChallengeReward = base.gameEconomyData.WeeklyChallengeRewards[i];
				if (personal)
				{
					if (weeklyChallengeReward.RewardType == WeeklyChallengeReward.ChallengeRewardType.PersonalStars && weeklyChallengeReward.Control > NumberStars)
					{
						return weeklyChallengeReward;
					}
				}
				else if (weeklyChallengeReward.RewardType == WeeklyChallengeReward.ChallengeRewardType.GuildStars && weeklyChallengeReward.Control > NumberStarsGuild)
				{
					return weeklyChallengeReward;
				}
			}
			return null;
		}

		public void ReturnRewardsInBatches(int rewardTypeBitmask, int minStarCount, int maxStarCount, int rewardsPerBatch, out List<List<WeeklyChallengeReward>> returnList, out int batchCount, int firsBatchCountOffset = 0)
		{
			returnList = new List<List<WeeklyChallengeReward>>();
			List<WeeklyChallengeReward> list = new List<WeeklyChallengeReward>();
			returnList.Add(list);
			batchCount = 0;
			if (rewardsPerBatch <= 0)
			{
				return;
			}
			for (int i = 0; i < base.gameEconomyData.WeeklyChallengeRewards.Length; i++)
			{
				WeeklyChallengeReward weeklyChallengeReward = base.gameEconomyData.WeeklyChallengeRewards[i];
				if (weeklyChallengeReward == null || !UtilsMath.BitmaskContains(1 << (int)weeklyChallengeReward.RewardType, rewardTypeBitmask))
				{
					continue;
				}
				WeeklyChallengeReward weeklyChallengeReward2 = ((list.Count > 0) ? list[list.Count - 1] : null);
				int num = ((batchCount <= 0) ? (rewardsPerBatch + firsBatchCountOffset) : rewardsPerBatch);
				if (list.Count < num)
				{
					list.Add(weeklyChallengeReward);
					continue;
				}
				if (weeklyChallengeReward2 != null && weeklyChallengeReward2.Control < maxStarCount)
				{
					if (weeklyChallengeReward2.Control < minStarCount)
					{
						returnList.Remove(list);
					}
					list = new List<WeeklyChallengeReward>();
					returnList.Add(list);
					list.Add(weeklyChallengeReward2);
					list.Add(weeklyChallengeReward);
					batchCount++;
					continue;
				}
				break;
			}
		}

		private List<WeeklyChallengeReward> GetGuildRewards()
		{
			List<WeeklyChallengeReward> list = new List<WeeklyChallengeReward>();
			if (LastNumberOfGuildStars == -1)
			{
				LastNumberOfGuildStars = NumberStarsGuild;
			}
			WeeklyChallengeReward[] weeklyChallengeRewards = base.gameEconomyData.WeeklyChallengeRewards;
			foreach (WeeklyChallengeReward weeklyChallengeReward in weeklyChallengeRewards)
			{
				if (weeklyChallengeReward.RewardType == WeeklyChallengeReward.ChallengeRewardType.GuildStars && LastNumberOfGuildStars < weeklyChallengeReward.Control && NumberStarsGuild >= weeklyChallengeReward.Control && (ClaimedGuildRewardStars == null || !ClaimedGuildRewardStars.Contains(weeklyChallengeReward.Control)))
				{
					list.Add(weeklyChallengeReward);
				}
			}
			return list;
		}

		public void ResetLastNumberOfGuildStars()
		{
			LastNumberOfGuildStars = -1;
		}

		public void MarkChallengeStartedAsSeen()
		{
			ChallengeStartedSeen = true;
		}

		public void MarkChallengeEndedAsSeen()
		{
			ChallengeEndedSeen = true;
		}

		public void MarkSkipTokensAvailableSeen()
		{
			SkipTokensAvailableSeen = true;
		}

		public bool HasSeenLatestPersonalStars()
		{
			return NumberStars == LastSeenNumberStars;
		}

		public List<LootEntry> GetRewardsPerType(params LootEntryType[] type)
		{
			List<LootEntry> list = new List<LootEntry>();
			if (Rewards != null)
			{
				for (int i = 0; i < Rewards.Count; i++)
				{
					if (Rewards[i] != null && type.Contains(Rewards[i].Type))
					{
						list.Add(Rewards[i]);
					}
				}
			}
			return list;
		}

		public TWDModelResult GiveRewardsPerType(LootEntryType[] type, List<LootEntry> lootEntries)
		{
			List<LootEntry> list = new List<LootEntry>();
			for (int i = 0; i < ((Rewards != null) ? Rewards.Count : 0); i++)
			{
				LootEntry lootEntry = Rewards[i];
				if (lootEntry != null && type.Contains(lootEntry.Type))
				{
					base.manager.Player.LootManager.GiveLoot(lootEntry);
					ReportChallengeAnalytics(lootEntry);
					list.Add(lootEntry);
					lootEntries.Add(lootEntry);
				}
			}
			for (int j = 0; j < list.Count; j++)
			{
				LootEntry model = list[j];
				Rewards.Remove(model);
			}
			return TWDModelResult.OK;
		}

		public bool HasSeenLatestGuildStars()
		{
			return NumberStarsGuild == LastSeenNumberOfGuildStars;
		}

		public void DEBUG_giveReward(WeeklyChallengeReward.ChallengeRewardType rewardType, int control)
		{
			WeeklyChallengeReward weeklyChallengeReward = base.manager.GameEconomyData.GetWeeklyChallengeReward(rewardType, control, controlExactMatch: false);
			if (weeklyChallengeReward != null)
			{
				AddReward(weeklyChallengeReward);
				base.manager.Player.LootManager.GiveLoot(Rewards[Rewards.Count - 1]);
			}
		}

		public void DEBUG_clearAllPendingRewards()
		{
			Rewards.Clear();
		}

		public void SkipToCircle(int circle)
		{
			int num = circle - CurrentCycle - 1;
			for (int i = 0; i < num; i++)
			{
				MapMissionGroupModel currentOrNextMapMissionGroupModel = GetCurrentOrNextMapMissionGroupModel();
				if (currentOrNextMapMissionGroupModel == null)
				{
					return;
				}
				foreach (MapMissionModel mission in currentOrNextMapMissionGroupModel.Missions)
				{
					if (!mission.IsMasterMission && mission.Stars.TotalStars == 0)
					{
						mission.Stars.Stars = new bool[3] { true, true, true };
						mission.Stars.TotalStars = 4;
						int amount = 4;
						AddPersonalStars(amount, givegiveStarsFromMasterMissionCompletion: true);
						base.manager.Player.MissionStatistics.AddStars(amount);
					}
					else if (!mission.IsMasterMission && mission.Stars.TotalStars > 0 && mission.Stars.TotalStars < 4)
					{
						int amount = 4 - mission.Stars.TotalStars;
						mission.Stars.Stars = new bool[3] { true, true, true };
						mission.Stars.TotalStars = 4;
						AddPersonalStars(amount, givegiveStarsFromMasterMissionCompletion: true);
						base.manager.Player.MissionStatistics.AddStars(amount);
					}
					mission.NotifyChange("StateChanged");
				}
				AddCycleCompleteRewards(isSkip: true);
				if (HasCompletedMaxCycles())
				{
					break;
				}
				StartNewCycle();
			}
			UpdateGuildStars();
		}

		public void UpdateChallengePlayerLeaderboards()
		{
			if (base.manager.ServerService == null)
			{
				return;
			}
			LeaderboardEntry entry = Leaderboards.CreateChallengeLeaderboardEntry(base.manager.Player);
			base.manager.ServerService.SaveLeaderboardEntry(Leaderboards.ChallengeStarsGlobal, entry);
			if (base.manager.Player.Country != null)
			{
				base.manager.ServerService.SaveLeaderboardEntry(Leaderboards.ChallengeStarsCountryPrefix + base.manager.Player.Country, entry);
			}
			entry = Leaderboards.CreateCurrentChallengeLeaderboardEntry(base.manager.Player);
			string challengeId = base.manager.Player.WeeklyChallenge.Id.ToString();
			base.manager.ServerService.SaveLeaderboardEntry(Leaderboards.GetPlayerChallengeWeeklyLeaderboardName(challengeId), entry);
			if (base.manager.Player.Country != null)
			{
				base.manager.ServerService.SaveLeaderboardEntry(Leaderboards.GetPlayerChallengeWeeklyCountryLeaderboardName(base.manager.Player.Country, challengeId), entry);
			}
			if (WeeklyChallengeZoneModel != null && WeeklyChallengeZoneModel.FeatureEnabled)
			{
				int zoneIdById = WeeklyChallengeZoneModel.GetZoneIdById(Id);
				base.manager.ServerService.SaveLeaderboardEntry(Leaderboards.GetPlayerChallengeWeeklyLeaderboardNameWithZoneId(challengeId, zoneIdById), entry);
				if (base.manager.Player.Country != null)
				{
					base.manager.ServerService.SaveLeaderboardEntry(Leaderboards.GetPlayerChallengeWeeklyCountryLeaderboardNameWithZoneId(base.manager.Player.Country, challengeId, zoneIdById), entry);
				}
			}
		}
	}
}
