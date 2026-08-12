using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using TwdCustomMod;

namespace TWDModel
{
	[Serializable]
	public class GuildBattleMapMissionModel : TWDGroupModelChild, IMapMissionModel, IAttackTargetModel
	{
		public enum MissionType
		{
			PVP = 0,
			PVE = 1,
			Invalid = 2
		}

		public enum MissionState
		{
			None = 0,
			PVP = 1,
			PVE = 2
		}

		[JsonIgnore]
		public int SectorIdOwner;

		[JsonIgnore]
		public int MissionQueueIndex = -1;

		[JsonIgnore]
		public int MissionPositionWithinArea;

		[JsonIgnore]
		public SurvivalMissionConfig GeneratedSurvivalMissionConfig;

		[JsonIgnore]
		public SurvivalMissionConfig GeneratedSurvivalMissionConfigPVP;

		private PlayerModel player;

		public string Id { get; set; }

		public int CompletionAmount { get; private set; }

		public int AreaIndex { get; set; }

		public string PvpPlayerHashedId { get; set; }

		public List<string> PvpParticipants { get; set; }

		public List<int> SavedData { get; set; }

		[JsonIgnore]
		public int MissionConfigIndexObjective { get; set; }

		[JsonIgnore]
		public int MissionConfigIndexEnemies { get; set; }

		[JsonIgnore]
		public MissionType Type { get; set; }

		[JsonIgnore]
		public string OverrideMissionConfigName { get; set; }

		[JsonIgnore]
		public int OverrideMissionConfigIndexObjective { get; set; }

		[JsonIgnore]
		public int OverrideMissionConfigIndexEnemies { get; set; }

		[JsonIgnore]
		public int RestoreOrderNumberInPool { get; set; }

		[JsonIgnore]
		public int OrderNumberInPool { get; set; }

		[JsonIgnore]
		public int CostIndex { get; set; }

		[JsonIgnore]
		public string MissionIdFromDefinition => base.gameEconomyData.GuildBattleMissionPoolDefinitionGrouped[SectorModelOwner.MissionPoolName][OrderNumberInPool];

		[JsonIgnore]
		public int MissionDifficultyLevel => RecalculateMissionDifficulty();

		[JsonIgnore]
		public int MissionLevel => base.gameEconomyData.GetMissionGenerationDataForMaxWalkerLevel(MissionDifficultyLevel)?.MissionLevel ?? 1;

		[JsonIgnore]
		public int RequiredSurvivorLevel => base.gameEconomyData.GetMissionGenerationDataForMaxWalkerLevel(MissionDifficultyLevel)?.MaxWalkerLevel ?? 1;

		[JsonIgnore]
		public int RequiredSurvivorLevelPVE => base.gameEconomyData.GetMissionGenerationDataForMaxWalkerLevel(RecalculateMissionDifficultyPvE())?.MaxWalkerLevel ?? 1;

		[JsonIgnore]
		public MissionDifficulty MissionDifficulty => MissionDifficulty.Normal;

		[JsonIgnore]
		public int MaxTeamSize => 3;

		[JsonIgnore]
		public bool IsDisabledOnGED => false;

		[JsonIgnore]
		public int AttackTargetId => SectorIdOwner;

		[JsonIgnore]
		public bool IsPVPTeamKilled
		{
			get
			{
				if (SavedData != null)
				{
					return SavedData.Count == 3;
				}
				return false;
			}
		}

		[JsonIgnore]
		public GuildBattleModel CurrentBattle
		{
			get
			{
				if (OfflineManager.IsLoadDataManager)
				{
					return GWTeamUtils.Instance.GuildModel?.GuildWarModel.CurrentBattle ?? null;
				}
				if (!(base.root is GvGSeasonModel gvGSeasonModel))
				{
					return null;
				}
				return gvGSeasonModel.GuildWarModel.CurrentBattle;
			}
		}

		[JsonIgnore]
		public GuildBattleMapSectorModel SectorModelOwner => CurrentBattle?.CurrentMapModel.GetSectorModel(SectorIdOwner);

		[JsonIgnore]
		public bool HasMissionConfigOverride
		{
			get
			{
				if (OverrideMissionConfigIndexEnemies > -1)
				{
					return OverrideMissionConfigIndexObjective > -1;
				}
				return false;
			}
		}

		public GuildBattleMapMissionModel()
		{
			MissionConfigIndexObjective = -1;
			MissionConfigIndexEnemies = -1;
			SavedData = new List<int>();
			PvpParticipants = new List<string>();
			AreaIndex = -1;
		}

		public override void Start()
		{
			OverrideMissionConfigName = null;
			OverrideMissionConfigIndexObjective = -1;
			OverrideMissionConfigIndexEnemies = -1;
			RestoreOrderNumberInPool = -1;
		}

		public bool IsCompleted()
		{
			return IsMissionCompleted(CompletionAmount);
		}

		public bool IsEnemyUnlocked()
		{
			return IsMissionEnemyUnlocked(Type, SectorModelOwner, AreaIndex);
		}

		public bool AllPvEMissionsInAreaCompleted()
		{
			return SectorModelOwner?.PvEMissionsInAreaCompleted(AreaIndex) ?? false;
		}

		public bool IsMissionPveComplete()
		{
			return IsMissionPveComplete(Type, CompletionAmount);
		}

		public bool IsPvpComplete()
		{
			if (Type == MissionType.PVP)
			{
				if (HelpersModel.IsUnlockAllSectors) return false;
				return CompletionAmount > 0;
			}
			return false;
		}

		public void AddMissionCompletions()
		{
			CompletionAmount = UtilsMath.Clamp(CompletionAmount + 1, 0, 1);
		}

		public static bool IsMissionCompleted(int completionAmount)
		{
			if (HelpersModel.IsUnlockAllSectors) return false;
			return completionAmount > 0;
		}

		public static bool IsMissionPveComplete(MissionType type, int completionAmount)
		{
			if (type != MissionType.PVP)
			{
				if (HelpersModel.IsUnlockAllSectors) return true;
				return completionAmount > 0;
			}
			return true;
		}

		public static bool IsMissionEnemyUnlocked(MissionType type, GuildBattleMapSectorModel sectorModel, int areaIndex)
		{
			if (type == MissionType.PVP && sectorModel != null)
			{
				if (HelpersModel.IsUnlockPVP) return true;
				return sectorModel.PvEMissionsInAreaCompleted(areaIndex);
			}
			return false;
		}

		private MissionState GetCurrentMissionState()
		{
			if (Type != MissionType.PVP)
			{
				_ = 1;
				return MissionState.PVE;
			}
			if (!IsEnemyUnlocked())
			{
				return MissionState.PVE;
			}
			return MissionState.PVP;
		}

		public Cashier GetStartMissionCashier(TWDModelManager manager)
		{
			var currency = manager.Player.GetCurrency(CurrencyType.GvGMissionKey);
			int max = currency.Max;
			int cost = UtilsMath.Clamp(GetMissionGasCost(), 0, max);

			if (HelpersModel.IsUnlockPVP && currency.Value < cost)
			{
				//мечи
				manager.Player.SetCurrency(CurrencyType.GvGMissionKey, cost - currency.Value);
			}
			return Cashier.CreateOneItemCashier(manager, PurchaseType.GuildBattleAttackMission, CurrencyType.GvGMissionKey, cost);
		}

		public Cashier GetStartMissionExpertModeCashier(TWDModelManager twdManager)
		{
			return new Cashier(twdManager);
		}

		public Cashier GetRetryGvGMissionCashier(TWDModelManager manager)
		{
			var currency = manager.Player.GetCurrency(CurrencyType.GvGGas);
			int max = currency.Max;
			int cost = UtilsMath.Clamp(GetMissionRetryGasCost(manager), 0, max);
			if (HelpersModel.IsUnlockPVP && currency.Value < cost)
			{
				//красный газ
				manager.Player.SetCurrency(CurrencyType.GvGGas, cost - currency.Value);
			}
			return Cashier.CreateOneItemCashier(manager, PurchaseType.GvGMissionRetry, CurrencyType.GvGGas, cost);
		}

		public MapMissionParameters ToMissionParameters()
		{
			return new MapMissionParameters
			{
				MissionId = MissionIdFromDefinition,
				MissionLevel = MissionLevel,
				MissionSectorId = SectorIdOwner,
				IsDeadly = false,
				LootTag = DropEventDefinition.DropEventTag.None,
				RandomSeed = GetRandomSeed(),
				IsSurvival = false,
				GuildBattleState = GetCurrentMissionState()
			};
		}

		public int GetRandomSeed()
		{
			return MissionIdFromDefinition.GetHashCode();
		}

		public void SendGuildProgressToGuild(ECombatResult result, TWDModelManager manager, bool retriedMission, List<int> savedData = null)
		{
			GuildBattleMapMissionModel obj = manager.Player.GetAttackTargetMissionModel() as GuildBattleMapMissionModel;
			bool flag = false; // pvpMissionPlayed
			if (obj != null)
			{
				flag = manager.Player.GvGSeasonModelPlayer.GuildWarModelPlayer.GuildBattleModel.AttackTargetMission.IsPvPCombat;
			}
			if (!manager.Player.IsGuildMember)
			{
				manager.Debug.LogWarning($"[SendGuildProgressToGuild] SKIP: Player not in guild. MissionId: {Id}, Result: {result}");
				return;
			}
			if (manager.Player.GuildWarModel == null)
			{
				manager.Debug.LogWarning($"[SendGuildProgressToGuild] SKIP: Player.GuildWarModel is null. MissionId: {Id}, Result: {result}");
				return;
			}
			GuildBattleModel currentBattle = manager.Player.GuildWarModel.CurrentBattle;
			if (currentBattle == null)
			{
				manager.Debug.LogWarning($"[SendGuildProgressToGuild] SKIP: CurrentBattle is null. MissionId: {Id}, Result: {result}");
				return;
			}
			bool flag2 = currentBattle.HasEnded();
			bool flag3 = currentBattle.IsBiggerThanEndBattleTimeStamp(manager.Player.UtcTimeStamp);
			manager.Debug.LogInfo($"[SendGuildProgressToGuild] Check: MissionId={Id}, Result={result}, BattleEnded={flag2}, PastEndTime={flag3}, Retried={retriedMission}, CurrentTime={manager.Player.UtcTimeStamp}");
			if (flag2)
			{
				manager.Debug.LogWarning($"[SendGuildProgressToGuild] SKIP: Battle already ended. MissionId: {Id}, Result: {result}, TimeSinceEnd: {manager.Player.UtcTimeStamp}ms");
				return;
			}
			if (flag3)
			{
				manager.Debug.LogWarning($"[SendGuildProgressToGuild] SKIP: Current time past battle end time. MissionId: {Id}, Result: {result}, CurrentTime: {manager.Player.UtcTimeStamp}");
				return;
			}
			manager.Debug.LogInfo("SendGuildProgressToGuild " + Environment.StackTrace);
			AddGuildBattleProgressionGroupCommand command = new AddGuildBattleProgressionGroupCommand(manager.Player.GuildWarModel.WarDefinitionId, SectorIdOwner, Id, flag, result, retriedMission, savedData);
			manager.Debug.LogInfo($"[SendGuildProgressToGuild] SENDING command: MissionId={Id}, Result={result}, PVPMissionPlayed={flag}, RetriedMission={retriedMission}, SectorId={SectorIdOwner}");
			HelpersModel.ExecuteGroupCommand(manager, command);
		}

		public SurvivalMissionConfig SolveSurvivalConfigForCurrentMission()
		{
			Tuple<int, int> tuple = null;
			string text = null;
			if (HasMissionConfigOverride)
			{
				text = OverrideMissionConfigName;
				tuple = new Tuple<int, int>(OverrideMissionConfigIndexObjective, OverrideMissionConfigIndexEnemies);
				GeneratedSurvivalMissionConfig = GenerateSurvivalMissionConfig(text, tuple, base.gameEconomyData);
				return GeneratedSurvivalMissionConfig;
			}
			text = SectorModelOwner.MissionConfigPoolName;
			tuple = new Tuple<int, int>(MissionConfigIndexObjective, MissionConfigIndexEnemies);
			if (!IsEnemyUnlocked())
			{
				if (GeneratedSurvivalMissionConfig == null)
				{
					GeneratedSurvivalMissionConfig = GenerateSurvivalMissionConfig(text, tuple, base.gameEconomyData);
				}
				return GeneratedSurvivalMissionConfig;
			}
			if (GeneratedSurvivalMissionConfigPVP == null)
			{
				GeneratedSurvivalMissionConfigPVP = GenerateSurvivalMissionConfigPVP(text, tuple, base.gameEconomyData);
			}
			return GeneratedSurvivalMissionConfigPVP;
		}

		public bool IsUsingSurvivalConfig()
		{
			return true;
		}

		public override void SetPlayerOwnerAndGameEconomyData(GameEconomyData ged, TWDGroupModelChild root, PlayerModel player = null)
		{
			base.SetPlayerOwnerAndGameEconomyData(ged, root, player);
			this.player = player;
		}

		private int GetMissionGasCost()
		{
			return base.gameEconomyData.GetMissionCost(CostIndex)?.EnergyCost ?? 1;
		}

		private int GetMissionRetryGasCost(TWDModelManager manager)
		{
			return manager.GameEconomyData.GuildWarConfig.GetRetryCost(manager.Player.GuildBattlePlayer.TotalMissionsRetried);
		}

		private int RecalculateMissionDifficulty()
		{
			if (!IsEnemyUnlocked())
			{
				return RecalculateMissionDifficultyPvE();
			}
			return RecalculateMissionDifficultyPvP();
		}

		private int RecalculateMissionDifficultyPvP()
		{
			if (CurrentBattle?.CurrentMapModel == null)
			{
				return -1;
			}
			int column = 0;
			if (AreaIndex != -1)
			{
				column = AreaIndex;
			}
			return GvGModelHelper.GetPlayerSpecificDifficulty(player) + base.gameEconomyData.GetGuildBattleSectorMissionDifficulty(SectorIdOwner, column, isPvP: true);
		}

		private int RecalculateMissionDifficultyPvE()
		{
			if (CurrentBattle?.CurrentMapModel == null)
			{
				return -1;
			}
			int playerSpecificDifficulty = GvGModelHelper.GetPlayerSpecificDifficulty(player);
			int column = 0;
			if (AreaIndex != -1)
			{
				column = AreaIndex;
			}
			return playerSpecificDifficulty + base.gameEconomyData.GetGuildBattleSectorMissionDifficulty(SectorIdOwner, column);
		}

		public static string GenerateId(string missionPool, int sectorId, int runningNumber)
		{
			return $"{missionPool}_{sectorId}_{runningNumber}";
		}

		private static SurvivalMissionConfig FindMissionConfigAtIndex(string configName, int index, GameEconomyData gameEconomyData)
		{
			if (!gameEconomyData.GuildBattleMissionConfigPoolDefinitionGrouped.TryGetValue(configName, out var value))
			{
				return null;
			}
			if (index < value.Count)
			{
				return value[index];
			}
			return null;
		}

		private static T FindMissionConfig<T>(string columnName, string configName, int configIndex, GameEconomyData gameEconomyData) where T : GuildBattleMissionConfigBase
		{
			string groupKey = GuildBattleMissionConfig.GetGroupKey(columnName, configName);
			if (gameEconomyData.GuildBattleMissionConfigsGrouped.ContainsKey(groupKey))
			{
				List<GuildBattleMissionConfigBase> list = gameEconomyData.GuildBattleMissionConfigsGrouped[groupKey];
				if (list != null && configIndex != -1 && list.Count > configIndex)
				{
					return (T)list[configIndex];
				}
			}
			return null;
		}

		public static SurvivalMissionConfig GenerateSurvivalMissionConfig(string missionConfigName, Tuple<int, int> configIndexes, GameEconomyData gameEconomyData)
		{
			GuildBattleMissionConfigObjective objective = FindMissionConfig<GuildBattleMissionConfigObjective>("Objectives", missionConfigName, configIndexes.First, gameEconomyData);
			GuildBattleMissionConfigEnemies enemies = FindMissionConfig<GuildBattleMissionConfigEnemies>("Enemies", missionConfigName, configIndexes.Second, gameEconomyData);
			return GenerateSurvivalMissionConfigInternal(missionConfigName, objective, enemies);
		}

		public static SurvivalMissionConfig GenerateSurvivalMissionConfigPVP(string missionConfigName, Tuple<int, int> configIndexes, GameEconomyData gameEconomyData)
		{
			GuildBattleMissionConfigObjective guildBattleMissionConfigObjective = FindMissionConfig<GuildBattleMissionConfigObjective>("Objectives", missionConfigName, configIndexes.First, gameEconomyData);
			GuildBattleMissionConfigEnemies guildBattleMissionConfigEnemies = FindMissionConfig<GuildBattleMissionConfigEnemies>("Enemies", missionConfigName, configIndexes.Second, gameEconomyData);
			SurvivalMissionConfig survivalMissionConfig = new SurvivalMissionConfig();
			if (guildBattleMissionConfigObjective == null || guildBattleMissionConfigEnemies == null)
			{
				return null;
			}
			survivalMissionConfig.ConfigName = missionConfigName;
			survivalMissionConfig.MissionType = SurvivalMissionConfig.Type.GuildBattle;
			survivalMissionConfig.KillsRequired = guildBattleMissionConfigObjective.KillsRequired;
			survivalMissionConfig.ThreatStart = guildBattleMissionConfigObjective.ThreatStart;
			survivalMissionConfig.ThreatFrequency = guildBattleMissionConfigObjective.ThreatFrequency;
			survivalMissionConfig.SpawnerCount = guildBattleMissionConfigObjective.SpawnerCount;
			survivalMissionConfig.InteractiveDuration = guildBattleMissionConfigObjective.InteractiveDuration;
			survivalMissionConfig.SurviveDuration = guildBattleMissionConfigObjective.SurviveDuration;
			survivalMissionConfig.ObjectiveType = SurvivalMissionConfig.SurvivalObjectiveType.KillAllRaiders;
			survivalMissionConfig.Raiders = CreateRaidersConfig(guildBattleMissionConfigObjective.Raiders, 3);
			survivalMissionConfig.UpdateRaiderTypesCounts();
			survivalMissionConfig.UpdateBossTypesMask();
			survivalMissionConfig.UpdateBurningTypesMask();
			return survivalMissionConfig;
		}

		public static SurvivalMissionConfig GenerateSurvivalMissionConfigDebug(string objectiveString, string enemyString)
		{
			List<GuildBattleMissionConfigBase> list = new List<GuildBattleMissionConfigBase>();
			List<FixedPoint> weightsList = new List<FixedPoint>();
			GameEconomyData.ParseGuildBattleMissionConfigSingleRow<GuildBattleMissionConfigObjective>(ref list, ref weightsList, objectiveString);
			GameEconomyData.ParseGuildBattleMissionConfigSingleRow<GuildBattleMissionConfigEnemies>(ref list, ref weightsList, enemyString);
			int num = 0;
			GuildBattleMissionConfigObjective objective = null;
			GuildBattleMissionConfigEnemies enemies = null;
			if (!string.IsNullOrEmpty(objectiveString))
			{
				objective = list[num] as GuildBattleMissionConfigObjective;
				num++;
			}
			if (!string.IsNullOrEmpty(enemyString))
			{
				enemies = list[num] as GuildBattleMissionConfigEnemies;
				num++;
			}
			return GenerateSurvivalMissionConfigInternal("Debug", objective, enemies);
		}

		private static SurvivalMissionConfig GenerateSurvivalMissionConfigInternal(string configName, GuildBattleMissionConfigObjective objective, GuildBattleMissionConfigEnemies enemies)
		{
			SurvivalMissionConfig survivalMissionConfig = new SurvivalMissionConfig();
			if (objective == null || enemies == null)
			{
				return null;
			}
			survivalMissionConfig.ConfigName = configName;
			survivalMissionConfig.MissionType = SurvivalMissionConfig.Type.GuildBattle;
			survivalMissionConfig.ObjectiveType = objective.ObjectiveType;
			survivalMissionConfig.KillsRequired = objective.KillsRequired;
			survivalMissionConfig.ThreatStart = objective.ThreatStart;
			survivalMissionConfig.ThreatFrequency = objective.ThreatFrequency;
			survivalMissionConfig.SpawnerCount = objective.SpawnerCount;
			survivalMissionConfig.InteractiveDuration = objective.InteractiveDuration;
			survivalMissionConfig.SurviveDuration = objective.SurviveDuration;
			survivalMissionConfig.WalkersNormal = enemies.GetAmountForType(WalkerType.WalkerNormal, objective.BossType);
			survivalMissionConfig.WalkersTank = enemies.GetAmountForType(WalkerType.WalkerTank, objective.BossType);
			survivalMissionConfig.WalkersArmored = enemies.GetAmountForType(WalkerType.WalkerArmored, objective.BossType);
			survivalMissionConfig.WalkersExplosive = enemies.GetAmountForType(WalkerType.WalkerExplosive, objective.BossType);
			survivalMissionConfig.WalkersSpiked = enemies.GetAmountForType(WalkerType.WalkerSpiked, objective.BossType);
			survivalMissionConfig.WalkersMetalhead = enemies.GetAmountForType(WalkerType.WalkerMetalhead, objective.BossType);
			survivalMissionConfig.WalkersFast = enemies.GetAmountForType(WalkerType.WalkerFast, objective.BossType);
			survivalMissionConfig.WalkersWhisperer = enemies.GetAmountForType(WalkerType.WalkerWhisperer, objective.BossType) + enemies.GetAmountForType(WalkerType.WalkerWhispererMelee, objective.BossType);
			survivalMissionConfig.WalkersGoo = enemies.GetAmountForType(WalkerType.WalkerGoo, objective.BossType);
			survivalMissionConfig.WalkersCommonWealth = enemies.GetAmountForType(WalkerType.WalkerCommonWealth, objective.BossType);
			survivalMissionConfig.ExplosiveBarrels = enemies.GetAmountForType(WalkerType.ExplosiveBarrel, objective.BossType);
			survivalMissionConfig.Raiders = objective.Raiders;
			survivalMissionConfig.BossTypes = (string.IsNullOrEmpty(objective.BossType) ? enemies.BossTypes : (objective.BossType + ", " + enemies.BossTypes));
			survivalMissionConfig.BurningTypes = enemies.BurningTypes;
			survivalMissionConfig.UpdateRaiderTypesCounts();
			survivalMissionConfig.UpdateBossTypesMask();
			survivalMissionConfig.UpdateBurningTypesMask();
			return survivalMissionConfig;
		}

		private static string CreateRaidersConfig(string raiders, int amountSurvivorEnemies)
		{
			if (string.IsNullOrEmpty(raiders))
			{
				return CreateSurvivorPlayers(amountSurvivorEnemies);
			}
			return raiders + "," + CreateSurvivorPlayers(amountSurvivorEnemies);
		}

		private static string CreateSurvivorPlayers(int amount)
		{
			return SurvivalMissionConfig.SurvivorPlayerConst + "(" + amount + ")";
		}

		public override string ToString()
		{
			string text = "Mission Details: \nMissionId : " + MissionIdFromDefinition + "\nOrder Num : " + OrderNumberInPool + "\nType : " + Type.ToString() + "\nSector Id : " + SectorIdOwner + "\nObjective Index : " + MissionConfigIndexObjective + "\nEnemy Index : " + MissionConfigIndexEnemies + "\nUsed Pool : " + SectorModelOwner.MissionPoolName + "\nDifficulty Level : " + MissionLevel + "\nCompletion : " + CompletionAmount + "\n";
			if (Type == MissionType.PVP)
			{
				text += CurrentBattle.CurrentMapModel.GetPvpTeamForMission(Id);
			}
			return text;
		}

		public bool UpdateSaveData(List<int> newSavedData)
		{
			if (newSavedData == null || newSavedData.Count == 0)
			{
				return false;
			}
			if (SavedData.Count == 3)
			{
				return false;
			}
			for (int i = 0; i < newSavedData.Count; i++)
			{
				int item = newSavedData[i];
				if (!SavedData.Contains(item))
				{
					SavedData.Add(item);
					if (SavedData.Count == 3)
					{
						break;
					}
				}
			}
			if (SavedData.Count < 3)
			{
				NotifyChange("GuildBattleMissionPvPEnemiesUpdated", this);
			}
			return true;
		}

		public void AddPvpParticipant(string hashedId)
		{
			if (!PvpParticipants.Contains(hashedId))
			{
				PvpParticipants.Add(hashedId);
			}
		}

		public void ClearMissionConfigOverride()
		{
			if (RestoreOrderNumberInPool != -1)
			{
				OrderNumberInPool = RestoreOrderNumberInPool;
			}
			OverrideMissionConfigIndexEnemies = -1;
			OverrideMissionConfigIndexObjective = -1;
			OverrideMissionConfigName = null;
		}
	}
}
