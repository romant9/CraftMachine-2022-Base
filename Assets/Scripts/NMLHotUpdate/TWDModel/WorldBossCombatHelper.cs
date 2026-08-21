using System.Collections.Generic;
using System.Text;
using BaseModel;

namespace TWDModel
{
	public static class WorldBossCombatHelper
	{
		private const long SettleSkipOvertimeThresholdMs = 30000L;

		public static long GetParticipationScore(WorldBossMissionType missionType, WorldBossConfig config)
		{
			return missionType switch
			{
				WorldBossMissionType.PVE => GetPositiveScore(config?.PlayerScorePVE ?? 0), 
				WorldBossMissionType.PVP => GetPositiveScore(config?.PlayerScorePVP ?? 0), 
				WorldBossMissionType.BOSS => GetPositiveScore(config?.PlayerScoreBossBattle ?? 0), 
				_ => 0L, 
			};
		}

		public static long GetSuccessScore(WorldBossMissionType missionType, WorldBossConfig config, long bossScore = 0L)
		{
			return missionType switch
			{
				WorldBossMissionType.PVE => GetPositiveScore(config?.PlayerScorePVE ?? 0), 
				WorldBossMissionType.PVP => GetPositiveScore(config?.PlayerScorePVPSuccess ?? 0), 
				WorldBossMissionType.BOSS => GetPositiveScore(bossScore), 
				_ => 0L, 
			};
		}

		public static int GetBattleScoreChange(WorldBossMissionType missionType, WorldBossConfig config, bool isWin, bool isTimeout, long bossScore)
		{
			long participationScore = GetParticipationScore(missionType, config);
			long num = GetSuccessScore(missionType, config, bossScore);
			if (missionType != WorldBossMissionType.BOSS && (!isWin || isTimeout))
			{
				num = 0L;
			}
			if (participationScore >= int.MaxValue || num >= int.MaxValue - participationScore)
			{
				return int.MaxValue;
			}
			return (int)(participationScore + num);
		}

		public static string BuildHeroWeaponUse(IList<SurvivorModel> missionRoster)
		{
			StringBuilder stringBuilder = new StringBuilder("{");
			int num = 0;
			int num2 = 0;
			while (missionRoster != null && num2 < missionRoster.Count && num < 3)
			{
				SurvivorModel survivorModel = missionRoster[num2];
				if (survivorModel != null)
				{
					EquipmentItemModel weaponEquipment = survivorModel.GetWeaponEquipment();
					EquipmentItemModel equipmentOfCategory = survivorModel.GetEquipmentOfCategory(EquipmentCategory.Armor);
					if (num > 0)
					{
						stringBuilder.Append(',');
					}
					stringBuilder.Append(survivorModel.ActorDefinitionID ?? string.Empty).Append(":{").Append(weaponEquipment?.EquipmentDefinitionIdentifier ?? string.Empty)
						.Append("},{")
						.Append(equipmentOfCategory?.EquipmentDefinitionIdentifier ?? string.Empty)
						.Append('}');
					num++;
				}
				num2++;
			}
			return stringBuilder.Append('}').ToString();
		}

		private static long GetPositiveScore(long score)
		{
			if (score <= 0)
			{
				return 0L;
			}
			return score;
		}

		public static bool IsObjectiveType(WorldBossMissionType missionType, CheckedObjectiveType objectiveType)
		{
			return missionType switch
			{
				WorldBossMissionType.PVE => objectiveType == CheckedObjectiveType.SurvKillAllWalkers, 
				WorldBossMissionType.PVP => objectiveType == CheckedObjectiveType.SurvKillAllRaiders, 
				WorldBossMissionType.BOSS => objectiveType == CheckedObjectiveType.Unspecified, 
				_ => false, 
			};
		}

		public static void ApplyMissionSetup(CombatModel combat, WorldBossMissionModel mission)
		{
			if (combat != null && mission != null && mission.WorldBossMissionType == WorldBossMissionType.PVP)
			{
				DisableWalkerSpawners(combat);
				ApplyPvpDefenderSpawners(combat);
			}
		}

		private static void DisableWalkerSpawners(CombatModel combat)
		{
			List<TWDModelObject> models = combat.GetModels<ActorSpawnPointModel>();
			for (int i = 0; i < models.Count; i++)
			{
				if (models[i] is WalkerSpawnPointModel walkerSpawnPointModel)
				{
					walkerSpawnPointModel.SpawnCountPerAction = 0;
					walkerSpawnPointModel.TotalSpawnCount = 0;
				}
			}
		}

		private static void ApplyPvpDefenderSpawners(CombatModel combat)
		{
			List<TWDModelObject> models = combat.GetModels<ActorSpawnPointModel>();
			PlayerModel player = combat.manager.Player;
			GuildBattlePvpTeam guildBattlePvpTeam = player.WorldBossModelManager?.GetCurrentDefenderTeam();
			if (guildBattlePvpTeam?.Survivors == null)
			{
				combat.Debug.LogError("WorldBoss PVP defender team is null");
				return;
			}
			int num = 0;
			for (int i = 0; i < models.Count; i++)
			{
				if (models[i] is RaiderSpawnPointModel { SpawnUsed: false })
				{
					num++;
				}
			}
			if (num == 0)
			{
				combat.Debug.LogWarning("WorldBoss PVP has no free raider spawner");
				return;
			}
			int num2 = 3 / num;
			int num3 = 3 - num2 * num;
			int[] array = new int[num];
			for (int j = 0; j < num3; j++)
			{
				array[j] = 1;
			}
			player.PlayerRandom.ShuffleArray(array);
			int num4 = 0;
			int num5 = 0;
			for (int k = 0; k < models.Count; k++)
			{
				if (models[k] is RaiderSpawnPointModel { SpawnUsed: false } raiderSpawnPointModel2)
				{
					int num6 = num2 + array[num5];
					bool flag = num6 > 0 && num4 < guildBattlePvpTeam.Survivors.Count;
					raiderSpawnPointModel2.ReplaceWithSurvivorPlayerIndex = (flag ? num4 : (-1));
					raiderSpawnPointModel2.SpawnUsed = flag;
					raiderSpawnPointModel2.SpawnCountInUse = true;
					raiderSpawnPointModel2.SpawnCountPerAction = (flag ? num6 : 0);
					raiderSpawnPointModel2.TotalSpawnCount = (flag ? num6 : 0);
					if (flag)
					{
						num4++;
					}
					num5++;
				}
			}
		}

		public static void SettleCombatResult(TWDModelManager mgr, bool isWin, bool isTimeout)
		{
			WorldBossAttackTargetData worldBossAttackTargetData = (mgr?.Player?.WorldBossModelManager)?.AttackTarget;
			if (worldBossAttackTargetData == null || !worldBossAttackTargetData.IsActive)
			{
				if (mgr?.ServerService != null)
				{
					mgr.Debug.LogInfo("[WorldBossSettleTrace] SettleCombatResult skipped: AttackTarget missing or inactive. " + $"Player={mgr.Player?.HashedId}, IsWin={isWin}, IsTimeout={isTimeout}, HasAttackTarget={worldBossAttackTargetData != null}");
				}
			}
			else
			{
				if (mgr.ServerService == null)
				{
					return;
				}
				mgr.Debug.LogInfo("[WorldBossSettleTrace] SettleCombatResult entered. GroupId=" + mgr.Player?.GuildId + ", " + $"Player={mgr.Player?.HashedId}, SeasonId={worldBossAttackTargetData.SeasonId}, CycleId={worldBossAttackTargetData.CycleId}, " + $"CapturePoint={worldBossAttackTargetData.CapturePoint}, Cell={worldBossAttackTargetData.Cell}, IsWin={isWin}, " + $"IsTimeout={isTimeout}, HasSettled={worldBossAttackTargetData.HasSettled}");
				if (worldBossAttackTargetData.HasSettled)
				{
					mgr.Debug.LogInfo("[WorldBossSettleTrace] SettleCombatResult skipped: already settled. " + $"GroupId={mgr.Player?.GuildId}, Player={mgr.Player?.HashedId}, SeasonId={worldBossAttackTargetData.SeasonId}, " + $"CycleId={worldBossAttackTargetData.CycleId}, CapturePoint={worldBossAttackTargetData.CapturePoint}, Cell={worldBossAttackTargetData.Cell}");
					return;
				}
				if (worldBossAttackTargetData.IsBossBattle)
				{
					bool flag = SettleBoss(mgr, worldBossAttackTargetData, isWin, isTimeout);
					if (flag)
					{
						worldBossAttackTargetData.HasSettled = true;
					}
					mgr.Debug.LogInfo("[WorldBossSettleTrace] SettleBoss completed. BossBattleId=" + worldBossAttackTargetData.BossBattleId + ", " + $"BossScore={worldBossAttackTargetData.BossScore}, BossDamage={worldBossAttackTargetData.BossDamage}, Success={flag}");
					return;
				}
				if (isTimeout && worldBossAttackTargetData.MissionModel != null)
				{
					long num = mgr.Player.UtcTimeStamp - (worldBossAttackTargetData.MissionModel.BattleStartUtcMs + worldBossAttackTargetData.MissionModel.TimeLimitMs);
					if (num > 30000)
					{
						mgr.Debug.LogInfo("[WorldBossSettleTrace] SettleCombatResult skipped: timeout exceeded late-settle threshold. " + $"GroupId={mgr.Player?.GuildId}, Player={mgr.Player?.HashedId}, SeasonId={worldBossAttackTargetData.SeasonId}, " + $"CycleId={worldBossAttackTargetData.CycleId}, CapturePoint={worldBossAttackTargetData.CapturePoint}, Cell={worldBossAttackTargetData.Cell}, " + $"OvertimeMs={num}, ThresholdMs={30000L}");
						worldBossAttackTargetData.HasSettled = true;
						return;
					}
				}
				bool flag2 = SettleCell(mgr, worldBossAttackTargetData.SeasonId, worldBossAttackTargetData.CycleId, worldBossAttackTargetData.CapturePoint, worldBossAttackTargetData.Cell, isWin, isTimeout, worldBossAttackTargetData.ParticipantSurvivorIds, worldBossAttackTargetData.KilledDefenderCount, worldBossAttackTargetData.BossScore, worldBossAttackTargetData.MissionModel?.WorldBossMissionType);
				if (flag2)
				{
					worldBossAttackTargetData.HasSettled = true;
				}
				mgr.Debug.LogInfo("[WorldBossSettleTrace] SettleCombatResult completed. GroupId=" + mgr.Player?.GuildId + ", " + $"Player={mgr.Player?.HashedId}, SeasonId={worldBossAttackTargetData.SeasonId}, CycleId={worldBossAttackTargetData.CycleId}, " + $"CapturePoint={worldBossAttackTargetData.CapturePoint}, Cell={worldBossAttackTargetData.Cell}, IsWin={isWin}, " + $"IsTimeout={isTimeout}, SettleSucceeded={flag2}, HasSettled={worldBossAttackTargetData.HasSettled}");
			}
		}

		private static bool SettleBoss(TWDModelManager mgr, WorldBossAttackTargetData attackTarget, bool isWin, bool isTimeout)
		{
			if (mgr == null || mgr.ServerService == null || attackTarget == null || string.IsNullOrWhiteSpace(attackTarget.BossBattleId))
			{
				mgr?.Debug?.LogError("WorldBossCombatHelper: BossBattleId empty when settling BOSS battle");
				return false;
			}
			return mgr.ServerService.WorldBossSettleBoss(new WorldBossSettleBossOperationRequest
			{
				GroupId = mgr.Player.GuildId,
				PlayerHashedId = mgr.Player.HashedId,
				SeasonId = attackTarget.SeasonId,
				CycleId = attackTarget.CycleId,
				BossBattleId = attackTarget.BossBattleId,
				IsWin = isWin,
				IsTimeout = isTimeout,
				BossScore = ((attackTarget.BossScore > 0) ? attackTarget.BossScore : 0),
				BossDamage = ((attackTarget.BossDamage > 0) ? attackTarget.BossDamage : 0),
				EndBattleUtcMs = mgr.Player.UtcTimeStamp
			})?.Success ?? false;
		}

		public static bool SettleCell(TWDModelManager mgr, int seasonId, int cycleId, string capturePoint, string cell, bool isWin, bool isTimeout, List<string> participantSurvivorIds, int killedDefenderCount, long bossScore = -1L, WorldBossMissionType? missionType = null)
		{
			if (mgr == null || mgr.ServerService == null)
			{
				return false;
			}
			if (mgr.Player == null || mgr.GameEconomyData == null)
			{
				mgr.Debug?.LogError("WorldBossCombatHelper: player/game economy data is null");
				return false;
			}
			WorldBossConfig worldBossConfig = mgr.GameEconomyData.WorldBossConfig;
			if (worldBossConfig == null)
			{
				mgr.Debug?.LogError("WorldBossCombatHelper: WorldBossConfig is null");
				return false;
			}
			mgr.Debug.LogInfo("[WorldBossSettleTrace] SettleCell preparing request. GroupId=" + mgr.Player?.GuildId + ", " + $"Player={mgr.Player?.HashedId}, SeasonId={seasonId}, CycleId={cycleId}, " + $"CapturePoint={capturePoint}, Cell={cell}, IsWin={isWin}, IsTimeout={isTimeout}, " + $"KilledDefenderCount={killedDefenderCount}, BossScore={bossScore}");
			WorldBossModelManager worldBossModelManager = mgr.Player?.WorldBossModelManager;
			WorldBossAttackTargetData worldBossAttackTargetData = worldBossModelManager?.AttackTarget;
			bool flag = worldBossAttackTargetData != null && worldBossAttackTargetData.IsActive && worldBossAttackTargetData.SeasonId == seasonId && worldBossAttackTargetData.CycleId == cycleId && worldBossAttackTargetData.CapturePoint == capturePoint && worldBossAttackTargetData.Cell == cell;
			if (string.IsNullOrEmpty(capturePoint) || string.IsNullOrEmpty(cell))
			{
				mgr.Debug.LogError("WorldBossCombatHelper: capturePoint/cell empty");
				return false;
			}
			WorldBossSeasonDefinition worldBossSeasonDefinition = mgr.GameEconomyData.FindWorldBossSeasonDefinition(seasonId);
			if (worldBossSeasonDefinition == null || !worldBossSeasonDefinition.IsOpen(mgr.Player.UtcTimeStamp))
			{
				mgr.Debug.LogError($"WorldBossCombatHelper: season {seasonId} not found or not open");
				return false;
			}
			WorldBossCycleDefinition worldBossCycleDefinition = mgr.GameEconomyData.FindWorldBossCycleDefinition(seasonId, cycleId);
			if (worldBossCycleDefinition == null || !worldBossCycleDefinition.IsOpen(mgr.Player.UtcTimeStamp))
			{
				mgr.Debug.LogError($"WorldBossCombatHelper: cycle {cycleId} not found or not open");
				return false;
			}
			bool flag2;
			WorldBossMissionType worldBossMissionType;
			if (flag)
			{
				flag2 = worldBossAttackTargetData.IsPVECapturePoint;
				worldBossMissionType = (WorldBossMissionType)(((int?)missionType) ?? ((int?)worldBossAttackTargetData.MissionModel?.WorldBossMissionType) ?? ((!flag2) ? 1 : 0));
				mgr.Debug.LogInfo("[WorldBossSettleTrace] Settlement context resolved from AttackTarget. " + $"SeasonId={seasonId}, CycleId={cycleId}, CapturePoint={capturePoint}, Cell={cell}, " + $"IsPVECapturePoint={flag2}, MissionType={worldBossMissionType}, " + $"SnapshotAvailable={worldBossModelManager?.WorldBossGuildFullSnapshot?.Match != null}");
			}
			else
			{
				WorldBossGuildFullSnapshot worldBossGuildFullSnapshot = worldBossModelManager?.WorldBossGuildFullSnapshot;
				if (worldBossGuildFullSnapshot == null || worldBossGuildFullSnapshot.Match == null)
				{
					mgr.Debug.LogError("WorldBossCombatHelper: matching AttackTarget and WorldBoss snapshot/match are both unavailable. " + $"SeasonId={seasonId}, CycleId={cycleId}, CapturePoint={capturePoint}, Cell={cell}");
					return false;
				}
				flag2 = mgr.GameEconomyData.FindWorldBossBattlegroundDefinitionByCapturePoint(capturePoint, worldBossGuildFullSnapshot.Match.BattleDifficulty)?.IsPVECapturePointType() ?? false;
				worldBossMissionType = (WorldBossMissionType)(((int?)missionType) ?? ((!flag2) ? 1 : 0));
				mgr.Debug.LogInfo("[WorldBossSettleTrace] Settlement context resolved from snapshot fallback. " + $"SeasonId={seasonId}, CycleId={cycleId}, CapturePoint={capturePoint}, Cell={cell}, " + $"IsPVECapturePoint={flag2}, MissionType={worldBossMissionType}");
			}
			string defenderInfo = null;
			if (isWin && !isTimeout && !flag2 && participantSurvivorIds != null && participantSurvivorIds.Count > 0)
			{
				GuildBattleParticipantInfo guildBattleParticipantInfo = GvGModelHelper.CreateParticipantFromSurvivorIds(mgr.Player, mgr.GameEconomyData, participantSurvivorIds);
				if (guildBattleParticipantInfo != null)
				{
					defenderInfo = mgr.GetMessageSerializer().Serialize(guildBattleParticipantInfo);
				}
			}
			long withdrawDurationMs = (long)worldBossConfig.Withdraw * 1000L;
			long successScore = GetSuccessScore(worldBossMissionType, worldBossConfig, bossScore);
			int num = killedDefenderCount;
			if (num < 0)
			{
				num = 0;
			}
			if (flag2)
			{
				num = 0;
			}
			else
			{
				int num2 = (flag ? worldBossAttackTargetData.DefenderTeam : null)?.Count ?? 0;
				if (num > num2)
				{
					mgr.Debug.LogInfo($"[WorldBossSettleTrace] KilledDefenderCount clamped from {num} to {num2}. CapturePoint={capturePoint}, Cell={cell}");
					num = num2;
				}
			}
			WorldBossOperationResult worldBossOperationResult = mgr.ServerService.WorldBossSettleCell(new WorldBossSettleCellOperationRequest
			{
				GroupId = mgr.Player.GuildId,
				PlayerHashedId = mgr.Player.HashedId,
				SeasonId = seasonId,
				CycleId = cycleId,
				CapturePoint = capturePoint,
				Cell = cell,
				IsWin = isWin,
				IsTimeout = isTimeout,
				IsPVECapturePoint = flag2,
				EnemyDurability = worldBossConfig.EnemyDurability,
				PVEGuardianLoss = worldBossConfig.PVEEnemyLoss,
				PVPGuardianLoss = worldBossConfig.PVPEnemyLoss,
				PVPGuardianDiePerHero = worldBossConfig.PVPEnemyPerDieLoss,
				KilledDefenderCount = num,
				EndBattleUTCMs = mgr.Player.UtcTimeStamp,
				DefenderInfo = defenderInfo,
				SurvivorIds = participantSurvivorIds,
				WithdrawDurationMs = withdrawDurationMs,
				BeforeProtectionSeconds = worldBossConfig.BeforeProtection,
				ProtectionConfig = worldBossConfig.Protection,
				TowerAConfig = worldBossConfig.TowerA,
				TowerAEffConfig = worldBossConfig.TowerAEff,
				TowerBConfig = worldBossConfig.TowerB,
				TowerBEffConfig = worldBossConfig.TowerBEff,
				DepotConfig = worldBossConfig.Depot,
				DepotEffConfig = worldBossConfig.DepotEff,
				DepotEffBossBattleTimeConfig = worldBossConfig.DepotEffBossBattleTime,
				WinScore = successScore,
				PlayerName = mgr.Player.Name,
				PlayerEmblem = AttackWorldBossCellCommand.SerializeEmblem(mgr.Player.PlayerEmblem)
			});
			if (worldBossOperationResult == null || !worldBossOperationResult.Success)
			{
				mgr.Debug.LogError("[WorldBossSettleTrace] IServerService.WorldBossSettleCell failed. " + $"GroupId={mgr.Player?.GuildId}, Player={mgr.Player?.HashedId}, SeasonId={seasonId}, CycleId={cycleId}, " + $"CapturePoint={capturePoint}, Cell={cell}, IsWin={isWin}, IsTimeout={isTimeout}, Message={worldBossOperationResult?.Message}");
				return false;
			}
			mgr.Debug.LogInfo("[WorldBossSettleTrace] IServerService.WorldBossSettleCell returned. " + $"GroupId={mgr.Player?.GuildId}, Player={mgr.Player?.HashedId}, SeasonId={seasonId}, CycleId={cycleId}, " + $"CapturePoint={capturePoint}, Cell={cell}, IsWin={isWin}, IsTimeout={isTimeout}, " + $"Success={worldBossOperationResult.Success}, Message={worldBossOperationResult.Message}");
			return true;
		}
	}
}
