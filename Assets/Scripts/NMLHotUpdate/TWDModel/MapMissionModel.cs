using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using TWDModel.ContentTypes;

namespace TWDModel
{
	public class MapMissionModel : TWDModelObject, IMapMissionModel, IChallengeDebuffProvider
	{
		public const string StateChanged = "StateChanged";

		public const string TrialReset = "TrialReset";

		public bool IsMasterMission;

		[JsonIgnore]
		public bool CompletedFromMasterMission;

		public bool ClassTeamRewardGiven;

		[JsonIgnore]
		public bool FeaturedHeroExtraChallengeStarFromMasterMission;

		[JsonIgnore]
		public int StarsFromMasterMission;

		[JsonIgnore]
		public ECombatResult LatestRunResult;

		public int ChallengeRandomSeed { get; set; }

		public int MissionSpawnPointGroupId { get; set; }

		[JsonIgnore]
		public bool IsEndlessMission
		{
			get
			{
				if (base.manager == null)
				{
					return false;
				}
				MissionSpawnPointGroup spawnPointGroup = base.manager.GameEconomyData.MissionSpawnPointData.GetSpawnPointGroup(MissionSpawnPointGroupId);
				if (spawnPointGroup != null)
				{
					return spawnPointGroup.Category == MapCategory.Endless;
				}
				return false;
			}
		}

		[JsonIgnore]
		public bool IsGrindMission
		{
			get
			{
				MissionSpawnPointGroup spawnPointGroup = base.manager.GameEconomyData.MissionSpawnPointData.GetSpawnPointGroup(MissionSpawnPointGroupId);
				if (spawnPointGroup != null)
				{
					return spawnPointGroup.Category == MapCategory.Grind;
				}
				return false;
			}
		}

		[JsonIgnore]
		public bool IsStoryMission
		{
			get
			{
				MissionSpawnPointGroup spawnPointGroup = base.manager.GameEconomyData.MissionSpawnPointData.GetSpawnPointGroup(MissionSpawnPointGroupId);
				if (spawnPointGroup != null)
				{
					return spawnPointGroup.Category == MapCategory.Story;
				}
				return false;
			}
		}

		[JsonIgnore]
		public int MaxTeamSize
		{
			get
			{
				if (MissionData == null)
				{
					return 3;
				}
				return MissionData.MaxTeamSize;
			}
		}

		[JsonIgnore]
		public bool IsInWeeklyChallenge
		{
			get
			{
				if (base.manager == null || base.manager.Player == null || base.manager.Player.WeeklyChallenge == null)
				{
					return false;
				}
				WeeklyChallengeModel weeklyChallenge = base.manager.Player.WeeklyChallenge;
				if (weeklyChallenge.Finished || ChallengeId < 0)
				{
					return false;
				}
				if (weeklyChallenge.CurrentDefinition == null || weeklyChallenge.CurrentDefinition.DetailMapId != MissionSpawnPointGroupId)
				{
					return false;
				}
				return weeklyChallenge.Id == ChallengeId;
			}
		}

		[JsonIgnore]
		public bool IsInApocalyptiWeeklyChallenge
		{
			get
			{
				if (base.manager == null || base.manager.Player == null || base.manager.Player.WeeklyChallenge == null)
				{
					return false;
				}
				WeeklyChallengeModel weeklyChallenge = base.manager.Player.WeeklyChallenge;
				if (weeklyChallenge.Finished || ChallengeId < 0)
				{
					return false;
				}
				if (MissionSpawnPointGroup.Category != MapCategory.ApocalypticChallenge)
				{
					return false;
				}
				if (weeklyChallenge.CurrentDefinition == null || weeklyChallenge.CurrentDefinition.ApocalypticMapId <= 0)
				{
					return false;
				}
				return weeklyChallenge.Id == ChallengeId;
			}
		}

		[JsonIgnore]
		public bool IsWorldBoss
		{
			get
			{
				if (base.manager == null)
				{
					return false;
				}
				MissionSpawnPointGroup spawnPointGroup = base.manager.GameEconomyData.MissionSpawnPointData.GetSpawnPointGroup(MissionSpawnPointGroupId);
				if (spawnPointGroup != null)
				{
					if (spawnPointGroup.Category != MapCategory.GuildBoss && spawnPointGroup.Category != MapCategory.GuildBossPVE)
					{
						return spawnPointGroup.Category == MapCategory.GuildBossPVP;
					}
					return true;
				}
				return false;
			}
		}

		[JsonIgnore]
		public bool IsInWeeklySurvival
		{
			get
			{
				if (base.manager == null || base.manager.Player == null || base.manager.Player.WeeklySurvival == null)
				{
					return false;
				}
				WeeklySurvivalModel weeklySurvival = base.manager.Player.WeeklySurvival;
				if (weeklySurvival.Finished || SurvivalId < 0)
				{
					return false;
				}
				if (weeklySurvival.CurrentDefinition == null || weeklySurvival.CurrentDefinition.DetailMapId != MissionSpawnPointGroupId)
				{
					return false;
				}
				return weeklySurvival.Id == SurvivalId;
			}
		}

		[JsonIgnore]
		public GuildBattleMapMissionModel.MissionType Type => GuildBattleMapMissionModel.MissionType.Invalid;

		[JsonIgnore]
		public MissionSpawnPoint MissionSpawnPoint => MissionSpawnPointGroup?.GetSpawnPointByMissionId(MissionId);

		[JsonIgnore]
		public MissionSpawnPointGroup MissionSpawnPointGroup
		{
			get
			{
				if (base.manager == null)
				{
					return null;
				}
				return base.manager.GameEconomyData.MissionSpawnPointData.GetSpawnPointGroup(MissionSpawnPointGroupId);
			}
		}

		[JsonIgnore]
		public int MissionStarsDisplayedInUI { get; set; }

		public bool IsDeadly { get; set; }

		public DropEventDefinition.DropEventTag LootTag { get; set; }

		public DropEventDefinition.DropEventContext DropContext { get; set; }

		public int GrindButtonDefinitionId { get; set; }

		public string MissionId { get; set; }

		public int MissionLevel { get; set; }

		public int MissionBaseLevel { get; set; }

		public MapMissionState State { get; set; }

		public int RespawnTimer { get; set; }

		public MapMissionStars Stars { get; set; }

		public int PreviousNumberStars { get; set; }

		public int ChallengeId { get; set; }

		public int SurvivalId { get; set; }

		public int CostIndex { get; set; }

		public int CompletionTimes { get; set; }

		[JsonIgnore]
		public MissionData MissionData => base.manager.GameEconomyData.GetMissionData(MissionId);

		[JsonIgnore]
		public int RequiredSurvivorLevel => base.gameEconomyData.GetMissionGenerationData(MissionLevel)?.MaxWalkerLevel ?? 1;

		[JsonIgnore]
		public MissionDifficulty MissionDifficulty
		{
			get
			{
				switch (base.manager.Player.SurvivorContainer.NumberCombatSurvivorsHaveRequiredLevelForMission(RequiredSurvivorLevel))
				{
				case 3:
					return MissionDifficulty.Easy;
				case 2:
					return MissionDifficulty.Normal;
				case 1:
					return MissionDifficulty.Hard;
				default:
					if (IsInWeeklyChallenge || IsInWeeklySurvival || IsInApocalyptiWeeklyChallenge)
					{
						return MissionDifficulty.Hard;
					}
					if (base.manager.Player.SurvivorContainer.NumberAnySurvivorsHaveRequiredLevelForMission(RequiredSurvivorLevel) > 0)
					{
						return MissionDifficulty.NoTeamSurvivorsHaveRequired;
					}
					return MissionDifficulty.NoSurvivorsHaveRequired;
				}
			}
		}

		[JsonIgnore]
		public bool IsLocked => State == MapMissionState.Locked;

		[JsonIgnore]
		public bool IsCompleted => State == MapMissionState.Completed;

		[JsonIgnore]
		public bool IsLastInGroup
		{
			get
			{
				if (MissionSpawnPointGroup != null && MissionSpawnPointGroup.MissionSpawnPoints != null && MissionSpawnPointGroup.MissionSpawnPoints.Count > 0 && MissionSpawnPointGroup.MissionSpawnPoints[MissionSpawnPointGroup.MissionSpawnPoints.Count - 1].MissionId == MissionId)
				{
					return true;
				}
				return false;
			}
		}

		[JsonIgnore]
		public bool IsFirstInGroup
		{
			get
			{
				if (MissionSpawnPointGroup != null && MissionSpawnPointGroup.MissionSpawnPoints != null && MissionSpawnPointGroup.MissionSpawnPoints.Count > 0 && MissionSpawnPointGroup.MissionSpawnPoints[0].MissionId == MissionId)
				{
					return true;
				}
				return false;
			}
		}

		[JsonIgnore]
		public bool IsFixedSurvivorSeasonMission
		{
			get
			{
				MissionSpawnPointGroup missionSpawnPointGroup = MissionSpawnPointGroup;
				if (missionSpawnPointGroup != null && missionSpawnPointGroup.Category == MapCategory.Season)
				{
					return !IsLastInGroup;
				}
				return false;
			}
		}

		public bool IsUsingSurvivalConfig()
		{
			return IsInWeeklySurvival;
		}

		public SurvivalDifficulty GetDifficultyInWeeklySurvival()
		{
			if (!IsInWeeklySurvival)
			{
				return SurvivalDifficulty.None;
			}
			return base.manager.Player.WeeklySurvival.CurrentDifficulty;
		}

		public SurvivalMissionConfig SolveCurrentlyApplicableSurvivalConfigForOrderNumber(int missionOrderNum)
		{
			if (!IsInWeeklySurvival)
			{
				return null;
			}
			WeeklySurvivalModel weeklySurvival = base.manager.Player.WeeklySurvival;
			string[] array = new string[3];
			int[] array2 = new int[3];
			array[0] = weeklySurvival.CurrentDefinition.SurvivalMissionConfig1;
			array[1] = weeklySurvival.CurrentDefinition.SurvivalMissionConfig2;
			array[2] = weeklySurvival.CurrentDefinition.SurvivalMissionConfig3;
			array2[0] = weeklySurvival.CurrentDefinition.SectionMissionCount1;
			array2[1] = weeklySurvival.CurrentDefinition.SectionMissionCount2;
			array2[2] = weeklySurvival.CurrentDefinition.SectionMissionCount3;
			if (string.IsNullOrEmpty(array[0]) || string.IsNullOrEmpty(array[1]) || string.IsNullOrEmpty(array[2]))
			{
				base.Debug.LogError("Weekly survival is missing some of the SurvivalMissionConfig1/2/3 value (null or empty).");
				return null;
			}
			if (array2[0] < 1 || array2[1] < 1 || array2[2] < 1)
			{
				base.Debug.LogError("Weekly survival is section mission counts must be positive values.");
				return null;
			}
			int num = 0;
			int num2 = 0;
			bool flag = false;
			for (int i = 0; i < array2.Length; i++)
			{
				int num3 = array2[i];
				if (!flag && missionOrderNum < num2 + num3)
				{
					num = i;
					flag = true;
					break;
				}
				num2 += num3;
			}
			int num4 = missionOrderNum - num2;
			if (!flag)
			{
				base.Debug.LogError("Failed to properly solve mission numbers in sections. (Possibly the current mission is out of survival section ranges.)");
				num4 = 0;
			}
			for (int j = 0; j < base.manager.GameEconomyData.SurvivalMissionConfigs.Length; j++)
			{
				if (base.manager.GameEconomyData.SurvivalMissionConfigs[j].ConfigName == array[num] && base.manager.GameEconomyData.SurvivalMissionConfigs[j].MissionOrderInSection == num4)
				{
					return base.manager.GameEconomyData.SurvivalMissionConfigs[j];
				}
			}
			base.Debug.LogError("Data mismatch: MapMissionModel.SolveCurrentlyApplicableSurvivalConfigForOrderNumber - survival config '" + array[num] + "' with matching mission order in section " + num4 + " not found in GED!");
			return null;
		}

		private SurvivalMissionConfig SolveCurrentlyApplicableSurvivalConfig()
		{
			if (!IsInWeeklySurvival)
			{
				return null;
			}
			int missionOrderNum = SolveOrderNumberInGroup();
			return SolveCurrentlyApplicableSurvivalConfigForOrderNumber(missionOrderNum);
		}

		public SurvivalMissionConfig SolveSurvivalConfigForCurrentMission()
		{
			if (IsInWeeklySurvival)
			{
				return SolveCurrentlyApplicableSurvivalConfig();
			}
			return null;
		}

		public override string ToString()
		{
			return "MissionId = '" + MissionId + "'\n\tLevel = " + MissionLevel + ", IsDeadly = " + IsDeadly;
		}

		public int SolveOrderNumberInGroup()
		{
			if (MissionSpawnPointGroup != null && MissionSpawnPointGroup.MissionSpawnPoints != null)
			{
				for (int i = 0; i < MissionSpawnPointGroup.MissionSpawnPoints.Count; i++)
				{
					if (MissionSpawnPointGroup.MissionSpawnPoints[i].MissionId == MissionId)
					{
						return i;
					}
				}
			}
			base.Debug.LogError("MapMissionModel.SolveOrderNumberInGroup - Failed to solve the order number of the mission.");
			return 0;
		}

		public bool ResetIfTrialMission()
		{
			if (IsLastInGroup && MissionSpawnPointGroup.Category == MapCategory.Season)
			{
				CompletionTimes = 0;
				RespawnTimer = 0;
				if (State != MapMissionState.Locked)
				{
					State = MapMissionState.Unlocked;
				}
				CalculateSeasonTrialDifficulty();
				NotifyChange("TrialReset");
				return true;
			}
			return false;
		}

		public void Unlock()
		{
			if (State == MapMissionState.Locked)
			{
				State = MapMissionState.Unlocked;
				NotifyChange("StateChanged");
			}
		}

		public void ForceState(MapMissionState newState)
		{
			if (State != newState)
			{
				State = newState;
				NotifyChange("StateChanged");
			}
		}

		public void InitExplicitMission(MissionSpawnPoint spawnPoint)
		{
			MissionId = spawnPoint.MissionId;
			if (base.manager.GameEconomyData.GetMissionData(MissionId) == null)
			{
				base.Debug.LogError("Data mismatch: Spawn point references mission  " + spawnPoint.MissionId + " - mission not found in GED!");
				return;
			}
			MissionData missionData = MissionData;
			MissionStarCondition[] conditions = missionData.MissionStarConditions.Conditions;
			if (missionData != null && conditions != null && conditions.Length == 3)
			{
				PreviousNumberStars = 0;
				Stars = new MapMissionStars();
				Stars.SetManager(base.manager);
				Stars.Initialize();
				UpdateModelObjects();
			}
		}

		public override void Start()
		{
			base.Start();
		}

		public override void Initialize()
		{
			base.Initialize();
			ChallengeId = -1;
			SurvivalId = -1;
		}

		public void GiveSurvivalCompletions()
		{
			if (IsInWeeklySurvival)
			{
				WeeklySurvivalModel weeklySurvival = base.manager.Player.WeeklySurvival;
				weeklySurvival.AddPersonalCompletions(1);
				base.manager.Player.MissionStatistics.AddSurvivalMissionCompletions(1);
				if (weeklySurvival.CurrentDefinition != null && weeklySurvival.NextMissionOrderNumber + 1 >= weeklySurvival.CurrentDefinition.TotalMissionCount)
				{
					weeklySurvival.AddFullCompletions(1);
					base.manager.Player.MissionStatistics.AddSurvivalFullCompletions(1);
				}
			}
		}

		public void GiveStars(bool giveStarsFromMasterMissionCompletion = false)
		{
			if (Stars == null)
			{
				return;
			}
			bool isInWeeklyChallenge = IsInWeeklyChallenge;
			bool featuredHeroExtraChallengeStar = Stars.FeaturedHeroExtraChallengeStar;
			PreviousNumberStars = Stars.NumberStars;
			int num = Stars.GiveStars(MissionData.MissionStarConditions.Conditions, isInWeeklyChallenge || IsInApocalyptiWeeklyChallenge);
			int num2 = ((base.manager.Player.AchievementManager != null) ? base.manager.Player.AchievementManager.GetQuestChallengeBonusStars() : 0);
			Stars.TotalBonusStars += num2;
			num += num2;
			if (IsInWeeklyChallenge && num > 0)
			{
				if (IsMasterMission)
				{
					base.manager.Player.WeeklyChallenge.CompleteMissionsInCycle();
				}
				else
				{
					base.manager.Player.WeeklyChallenge.AddPersonalStars(num, giveStarsFromMasterMissionCompletion);
					base.manager.Player.MissionStatistics.AddStars(num);
					if (base.manager.GameEconomyData.GetFeature("ChallengePersonalHighScoreRewardsEnabled").Enabled)
					{
						base.manager.Player.WeeklyChallenge.AddPersonalHighScoreRewards(base.manager.Player);
					}
					if (base.manager.GameEconomyData.GetFeature("ChallengeCycleEnabled").Enabled)
					{
						RecalculateWeeklyChallengeMissionLevel();
					}
					else
					{
						int num3 = (int)FixedPoint.Round((float)Stars.TotalStars * base.gameEconomyData.ConfigData.ChallengeMissionLevelMultiplier);
						MissionLevel = MissionSpawnPoint.MissionLevel + num3;
					}
					if (!giveStarsFromMasterMissionCompletion)
					{
						base.manager.Player.WeeklyChallenge.UpdateChallengePlayerLeaderboards();
					}
					else
					{
						if (!featuredHeroExtraChallengeStar && Stars.FeaturedHeroExtraChallengeStar)
						{
							FeaturedHeroExtraChallengeStarFromMasterMission = true;
							num--;
						}
						StarsFromMasterMission = Math.Min(num, 3);
						CompletedFromMasterMission = PreviousNumberStars == 0;
					}
				}
			}
			if (!IsInApocalyptiWeeklyChallenge || num <= 0)
			{
				return;
			}
			if (IsMasterMission)
			{
				base.manager.Player.ApocalypseWeeklyChallenge.CompleteMissionsInCycle();
				return;
			}
			base.manager.Player.ApocalypseWeeklyChallenge.AddPersonalStars(num);
			if (!giveStarsFromMasterMissionCompletion)
			{
				base.manager.Player.ApocalypseWeeklyChallenge.UpdateChallengePlayerLeaderboards();
				return;
			}
			if (!featuredHeroExtraChallengeStar && Stars.FeaturedHeroExtraChallengeStar)
			{
				FeaturedHeroExtraChallengeStarFromMasterMission = true;
				num--;
			}
			StarsFromMasterMission = Math.Min(num, 3);
			CompletedFromMasterMission = PreviousNumberStars == 0;
		}

		public void RecalculateWeeklyChallengeMissionLevel()
		{
			if (base.manager == null || base.manager.GameEconomyData == null || base.manager.Player == null || !base.manager.GameEconomyData.GetFeature("ChallengeCycleEnabled").Enabled || !IsInWeeklyChallenge)
			{
				return;
			}
			int num = base.manager.Player.WeeklyChallenge.CurrentRequiredSurvivorLevel + (IsMasterMission ? base.manager.GameEconomyData.ConfigData.ChallengeMasterMissionDifficultyOffset : 0);
			if (num > 0)
			{
				MissionGenerationData missionGenerationDataForMaxWalkerLevel = base.gameEconomyData.GetMissionGenerationDataForMaxWalkerLevel(num);
				if (missionGenerationDataForMaxWalkerLevel != null)
				{
					MissionLevel = missionGenerationDataForMaxWalkerLevel.MissionLevel;
				}
			}
		}

		public void RecalculateWeeklySurvivalMissionLevel()
		{
			if (base.manager == null || base.manager.GameEconomyData == null || base.manager.Player == null || !IsInWeeklySurvival)
			{
				return;
			}
			SurvivalDifficulty currentDifficulty = base.manager.Player.WeeklySurvival.CurrentDifficulty;
			int missionOrderNumber = SolveOrderNumberInGroup();
			int num = SurvivalMissionDifficultyLevelHelper.CalculateResultingSurvivalMissionLevel(base.gameEconomyData, missionOrderNumber, base.manager.Player.CouncilLevel, currentDifficulty);
			if (num > 0)
			{
				MissionGenerationData missionGenerationData = base.gameEconomyData.GetMissionGenerationData(num);
				if (missionGenerationData != null)
				{
					MissionLevel = missionGenerationData.MissionLevel;
				}
			}
		}

		public bool UpdateSurvivalMapState()
		{
			if (MissionSpawnPointGroup.Category == MapCategory.Survival)
			{
				if (base.manager.Player.WeeklySurvival != null)
				{
					if (base.manager.Player.WeeklySurvival.NextMissionOrderNumber == SolveOrderNumberInGroup())
					{
						if (State != MapMissionState.Unlocked)
						{
							State = MapMissionState.Unlocked;
							return true;
						}
					}
					else if (base.manager.Player.WeeklySurvival.NextMissionOrderNumber > SolveOrderNumberInGroup())
					{
						if (State != MapMissionState.Completed)
						{
							State = MapMissionState.Completed;
							return true;
						}
					}
					else if (State != MapMissionState.Locked)
					{
						State = MapMissionState.Locked;
						return true;
					}
				}
			}
			else
			{
				base.Debug.LogError("UpdateSurvivalMissionState was called for non-survival MapMissionModel.");
			}
			return false;
		}

		public void UpdateMapState()
		{
			MapCategory category = MissionSpawnPointGroup.Category;
			if (category == MapCategory.Survival)
			{
				UpdateSurvivalMapState();
			}
			else
			{
				if (State != MapMissionState.Unlocked)
				{
					return;
				}
				MapMissionGroupModel missionGroupModelForSpawnPointGroup = base.manager.Player.MapContainerModel.GetMissionGroupModelForSpawnPointGroup(MissionSpawnPointGroup);
				switch (category)
				{
				case MapCategory.Season:
					if (IsLastInGroup && CompletionTimes >= base.manager.GameEconomyData.ConfigData.SeasonTrialDifficultyLevels.Count)
					{
						State = MapMissionState.Completed;
					}
					else if (IsLastInGroup)
					{
						State = MapMissionState.Respawning;
						RespawnTimer = base.manager.GameEconomyData.ConfigData.SeasonMissionGateTime;
					}
					else
					{
						State = MapMissionState.Completed;
						missionGroupModelForSpawnPointGroup.GetNextMissionModel(MissionSpawnPoint).SetSeasonRespawn();
					}
					return;
				case MapCategory.Challenge:
				case MapCategory.ApocalypticChallenge:
					if (!base.manager.GameEconomyData.GetFeature("ChallengeCycleEnabled").Enabled)
					{
						State = MapMissionState.Respawning;
						RespawnTimer = base.manager.GameEconomyData.ConfigData.ChallengeMissionRespawnTime;
						ChallengeRandomSeed = base.manager.Player.PlayerRandom.Next(int.MaxValue);
						return;
					}
					break;
				}
				switch (category)
				{
				case MapCategory.Grind:
					State = MapMissionState.Completed;
					break;
				case MapCategory.Story:
					State = MapMissionState.Completed;
					if (missionGroupModelForSpawnPointGroup.AreAllStoryMissionsCompleted())
					{
						MapContainerModel mapContainerModel = base.manager.Player.MapContainerModel;
						MapMissionGroupModel harderVersion = mapContainerModel.GetHarderVersion(missionGroupModelForSpawnPointGroup);
						if (harderVersion != null)
						{
							mapContainerModel.SpawnMissionsForGroup(harderVersion.MissionSpawnPointGroup);
						}
					}
					break;
				default:
					if (base.manager.GameEconomyData.ConfigData.OutpostTutorialMissionId == MissionId)
					{
						State = MapMissionState.Completed;
					}
					break;
				}
			}
		}

		public void SetSeasonRespawn()
		{
			if (IsLastInGroup)
			{
				CalculateSeasonTrialDifficulty();
			}
			State = MapMissionState.Unlocked;
			NotifyChange("StateChanged");
		}

		public int GetRandomSeed()
		{
			if (ChallengeRandomSeed != 0)
			{
				return ChallengeRandomSeed;
			}
			return MissionId.GetHashCode();
		}

		public override void Tick(long deltaTime)
		{
			base.Tick(deltaTime);
			if (State == MapMissionState.Respawning)
			{
				RespawnTimer -= Math.Max(0, (int)deltaTime);
				if (RespawnTimer <= 0)
				{
					CalculateSeasonTrialDifficulty();
					State = MapMissionState.Unlocked;
					NotifyChange("StateChanged");
				}
			}
		}

		private void CalculateSeasonTrialDifficulty()
		{
			if (CompletionTimes == 0)
			{
				MissionBaseLevel = base.manager.Player.SurvivorContainer.GetAverageSurvivorLevelFromTop3() * 3;
			}
			if (MissionSpawnPointGroup != null && MissionSpawnPointGroup.Category == MapCategory.Season && IsLastInGroup && CompletionTimes <= base.gameEconomyData.ConfigData.SeasonTrialDifficultyLevels.Count - 1)
			{
				MissionLevel = Math.Max(1, MissionBaseLevel + base.gameEconomyData.ConfigData.SeasonTrialDifficultyLevels[Math.Min(CompletionTimes, base.gameEconomyData.ConfigData.SeasonTrialDifficultyLevels.Count - 1)]);
			}
		}

		public override bool IsValid()
		{
			return true;
		}

		public MapMissionParameters ToMissionParameters()
		{
			return new MapMissionParameters
			{
				MissionId = MissionId,
				MissionLevel = MissionLevel,
				MissionSectorId = -1,
				IsDeadly = IsDeadly,
				LootTag = LootTag,
				RandomSeed = GetRandomSeed(),
				IsSurvival = (MissionSpawnPointGroup.Category == MapCategory.Survival),
				GuildBattleState = GuildBattleMapMissionModel.MissionState.None
			};
		}

		private int GetMissionGasCost()
		{
			MapCategory category = MissionSpawnPointGroup.Category;
			ConfigData configData = base.manager.GameEconomyData.ConfigData;
			if (base.manager.Player.IsTimedBonusActive(TimedBonusType.UnlimitedGas))
			{
				return 0;
			}
			switch (category)
			{
			case MapCategory.Season:
				return configData.SeasonMissionGasPrice;
			case MapCategory.Grind:
				return configData.GetGrindMissionCost(base.manager.Player.Level);
			default:
			{
				int num = base.manager.GameEconomyData.GetMissionCost(CostIndex)?.EnergyCost ?? 1;
				int num2 = 0;
				if (IsInWeeklyChallenge)
				{
					num2 = (int)FixedPoint.Ceiling((float)(base.manager.Player.WeeklyChallenge.CurrentRequiredSurvivorLevel * 3) * base.manager.GameEconomyData.ConfigData.ChallengeGasCostMultiplier);
				}
				if (IsInApocalyptiWeeklyChallenge)
				{
					num2 = base.manager.Player.ApocalypseWeeklyChallenge.CurrentCircleDefinition.GasCost;
				}
				return (num + num2) * ((!IsMasterMission) ? 1 : configData.ChallengeMasterMissionGasCostMultiplier);
			}
			}
		}

		public Cashier GetStartMissionCashier()
		{
			return GetStartMissionCashier(base.manager);
		}

		public Cashier GetStartMissionCashier(TWDModelManager twdManager)
		{
			if (MissionSpawnPointGroup.Category == MapCategory.Endless)
			{
				int cost = 0;
				if (CheckMissionEndlessNormalCanConsume(twdManager))
				{
					cost = twdManager.GameEconomyData.EndlessModeConfig.MissionBaseCost;
				}
				return Cashier.CreateOneItemCashier(twdManager, PurchaseType.EndlessPass, CurrencyType.EndlessPassToken, cost);
			}
			int cost2 = UtilsMath.Clamp(GetMissionGasCost(), 0, twdManager.Player.GetCurrency(CurrencyType.ReplayToken).Max);
			return Cashier.CreateOneItemCashier(twdManager, PurchaseType.RechargeCurrency, CurrencyType.ReplayToken, cost2);
		}

		private bool CheckMissionEndlessNormalCanConsume(TWDModelManager twdManager)
		{
			if (twdManager.Player.EndlessModeManager == null)
			{
				return true;
			}
			if (twdManager.Player.EndlessModeManager.EndlessModeGameModeType != EndlessModeGameModeType.Normal)
			{
				return true;
			}
			long num = twdManager.GameEconomyData.EndlessModeNormalRewardDefinitons.Max((EndlessModeNormalRewardDefiniton reward) => reward.Score);
			if (num == 0L)
			{
				return true;
			}
			if (twdManager.Player.EndlessModeManager.GetOverAllScoreForFinalScoreNormal() < num)
			{
				return true;
			}
			if (twdManager.GameEconomyData.EndlessModeConfig.MaxoutRetryPass)
			{
				return true;
			}
			return false;
		}

		public Cashier GetStartMissionExpertModeCashier()
		{
			return GetStartMissionExpertModeCashier(base.manager);
		}

		public Cashier GetStartMissionExpertModeCashier(TWDModelManager twdManager)
		{
			int missionBaseCost = twdManager.GameEconomyData.EndlessModeConfig.MissionBaseCost;
			return Cashier.CreateOneItemCashier(twdManager, PurchaseType.EndlessPass, CurrencyType.EndlessPassExpertToken, missionBaseCost);
		}

		public Rewards GetStoryMissionRewards(int completionOffset = 0)
		{
			if (MissionData == null || MissionData.MissionType == MissionType.Rescue || IsInWeeklyChallenge || IsInApocalyptiWeeklyChallenge || IsInWeeklySurvival || IsGrindMission)
			{
				return null;
			}
			Rewards result = null;
			MissionRewards missionRewardsData = base.gameEconomyData.GetMissionRewardsData(MissionData.DisplayTextID);
			string text = null;
			if (missionRewardsData != null && MissionSpawnPointGroup != null)
			{
				if (MissionSpawnPointGroup.Category == MapCategory.Season && CompletionTimes - completionOffset >= base.gameEconomyData.ConfigData.SeasonTrialDifficultyLevels.Count && IsLastInGroup)
				{
					return null;
				}
				if (MissionSpawnPointGroup.Category == MapCategory.Season && CompletionTimes - completionOffset == 0)
				{
					text = missionRewardsData.Reward;
				}
				else if (MissionSpawnPointGroup.Category == MapCategory.Season && IsLastInGroup && CompletionTimes - completionOffset == 1)
				{
					text = missionRewardsData.RewardLvl2;
				}
				else if (MissionSpawnPointGroup.Category == MapCategory.Season && IsLastInGroup && CompletionTimes - completionOffset == 2)
				{
					text = missionRewardsData.RewardLvl3;
				}
				else if (MissionSpawnPointGroup.Category == MapCategory.Season && IsLastInGroup && CompletionTimes - completionOffset == 3)
				{
					text = missionRewardsData.RewardLvl4;
				}
				else if (MissionSpawnPointGroup.Category == MapCategory.Season && IsLastInGroup && CompletionTimes - completionOffset == 4)
				{
					text = missionRewardsData.RewardLvl5;
				}
				else
				{
					if (MissionSpawnPointGroup.Category == MapCategory.Season)
					{
						return null;
					}
					if (MissionSpawnPointGroup.EpisodeDifficultyLevel == 1)
					{
						text = missionRewardsData.Reward;
					}
					else if (MissionSpawnPointGroup.EpisodeDifficultyLevel == 2)
					{
						text = missionRewardsData.RewardLvl2;
					}
					else if (MissionSpawnPointGroup.EpisodeDifficultyLevel == 3)
					{
						text = missionRewardsData.RewardLvl3;
					}
				}
			}
			if (string.IsNullOrEmpty(text))
			{
				text = base.gameEconomyData.ConfigData.DefaultStaticMissionReward;
			}
			if (!string.IsNullOrEmpty(text))
			{
				result = new Rewards(text, base.manager, MissionLevel, EquipmentSource.MissionLoot);
			}
			return result;
		}

		public bool HasStoryMissionRewardOfType(RewardType rewardType)
		{
			Rewards storyMissionRewards = GetStoryMissionRewards();
			if (storyMissionRewards != null)
			{
				List<IReward> rewardsOfType = storyMissionRewards.GetRewardsOfType(rewardType);
				if (rewardsOfType != null && rewardsOfType.Count > 0)
				{
					return true;
				}
			}
			return false;
		}

		public bool HasStoryMissionRewardOfSpeedUpToken()
		{
			Rewards storyMissionRewards = GetStoryMissionRewards();
			if (storyMissionRewards != null)
			{
				List<IReward> rewardsOfType = storyMissionRewards.GetRewardsOfType(RewardType.Currency);
				if (rewardsOfType != null && rewardsOfType.Count > 0 && rewardsOfType[0] is RewardCurrency rewardCurrency && ComponentHelper.IsSpeedUpToken(rewardCurrency.CurrencyType))
				{
					return true;
				}
			}
			return false;
		}

		public SupportModel GetFixedSupport(int equipIndex)
		{
			if (MissionData.FixedSupports != null)
			{
				string text = MissionData.FixedSupports[equipIndex];
				if (!string.IsNullOrEmpty(text))
				{
					string[] array = text.Split('-');
					SupportModel supportModel = new SupportModel(array[0]);
					supportModel.Level = int.Parse(array[1]);
					supportModel.SetManager(base.manager);
					return supportModel;
				}
			}
			return null;
		}

		public List<DifficultyIncrementalDebuff> GetChallengeDebuffs()
		{
			if (IsInApocalyptiWeeklyChallenge)
			{
				return base.manager.Player.ApocalypseWeeklyChallenge.GetChallengeDebuffs();
			}
			if (IsInWeeklyChallenge)
			{
				return base.manager.Player.WeeklyChallenge.GetChallengeDebuffs();
			}
			if (IsEndlessMission && base.manager.Player.EndlessModeManager != null && base.manager.Player.EndlessModeManager.EndlessModeGameModeType == EndlessModeGameModeType.Expert)
			{
				return base.manager.Player.EndlessModeManager.GetEndlessModeExpertDebuffConfigs();
			}
			return new List<DifficultyIncrementalDebuff>();
		}

		public bool IsSupportCoolDown(SupportDefinition definition)
		{
			foreach (List<FixedPoint> item in ChallengeDebufHelps.GetDebufAllParam(GetChallengeDebuffs(), ChallengeDebuffType.Supportcooldown))
			{
				if (item.Contains(definition.Index))
				{
					return true;
				}
			}
			return false;
		}

		public bool CheckChallengeDebuffAvoid(ChallengeDebuffType challengeDebuffType, RollDiceType rollDiceType)
		{
			return MapMissionDebuffHelper.CheckChallengeDebuffAvoid(this, base.manager, challengeDebuffType, rollDiceType);
		}

		public bool CheckChallengeHardtoAim(ActorModel source, ActorModel target)
		{
			return MapMissionDebuffHelper.CheckChallengeHardtoAim(this, base.manager, source, target);
		}

		public static FixedPoint GetChallengeActorHit(ActorModel acotr, List<DifficultyIncrementalDebuff> debuffs, int MinHit)
		{
			FixedPoint debufTotalFirstParam = ChallengeDebufHelps.GetDebufTotalFirstParam(debuffs, ChallengeDebuffType.WalkerDodge);
			return FixedPoint.Max(MinHit, acotr.Hit - debufTotalFirstParam);
		}

		public bool CheckChallengeWalkerDodge(ActorModel source, ActorModel target)
		{
			return MapMissionDebuffHelper.CheckChallengeWalkerDodge(this, base.manager, source, target);
		}

		public bool IsCombatSameClass()
		{
			return MapMissionDebuffHelper.IsCombatSameClass(base.manager);
		}

		public void VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			MapMissionDebuffHelper.VisitChallengeDebuffActions(this, base.manager, action, actor, addedActions);
		}
	}
}
