using System.Collections.Generic;
using BaseModel;

namespace TWDModel
{
	public class AttackWorldBossCellCommand : WorldBossParticipantAttackCommand
	{
		public string GroupId { get; set; }

		public string CapturePoint { get; set; }

		public string Cell { get; set; }

		public bool IsDebugMode { get; set; }

		private long _startBattleUTCMs { get; set; }

		private int _battleTimeLimitMs { get; set; }

		private bool _isPVECapturePoint { get; set; }

		private WorldBossMissionType _missionType { get; set; }

		public AttackWorldBossCellCommand()
		{
		}

		public AttackWorldBossCellCommand(int seasonId, int cycleId, string groupId, string capturePoint, string cell, List<string> participantSurvivorIds = null, bool isDebugMode = false)
			: base(seasonId, cycleId)
		{
			GroupId = groupId;
			CapturePoint = capturePoint;
			Cell = cell;
			base.ParticipantSurvivorIds = participantSurvivorIds;
			IsDebugMode = isDebugMode;
		}

		public override IModelCommandRespond Execute(ModelManager modelManager)
		{
			IModelCommandRespond modelCommandRespond = base.Execute(modelManager);
			if (modelCommandRespond != null && modelCommandRespond.Code == 0 && modelManager is TWDModelManager { Player: not null } tWDModelManager)
			{
				List<WorldBossModelManager.WorldBossFatigueChargeSnapshot> fatigueSnapshots = new List<WorldBossModelManager.WorldBossFatigueChargeSnapshot>();
				if (!IsDebugMode)
				{
					if (TryPrepareParticipantCharges(tWDModelManager, out var preparedCharges))
					{
						fatigueSnapshots = CaptureFatigueChargeSnapshots(tWDModelManager);
						ApplyPreparedParticipantCharges(tWDModelManager, preparedCharges);
					}
					else
					{
						tWDModelManager.Debug.LogError("AttackWorldBossCellCommand: prepare charges failed after attack succeeded; skip fatigue deduction");
					}
				}
				tWDModelManager.Player.MapContainerModel.ClearAttackTargetMissionData();
				WorldBossModelManager worldBossModelManager = tWDModelManager.Player.WorldBossModelManager;
				if (worldBossModelManager.RecordParticipation(base.SeasonId, base.CycleId) && tWDModelManager.ServerService == null)
				{
					worldBossModelManager.TrackOptimisticParticipation(base.SequenceId, base.SeasonId, base.CycleId);
				}
				worldBossModelManager.SetAttackCellTarget(base.SeasonId, base.CycleId, CapturePoint, Cell, base.ParticipantSurvivorIds);
				if (tWDModelManager.ServerService == null)
				{
					worldBossModelManager.TrackOptimisticAttackCell(base.SequenceId, fatigueSnapshots);
				}
			}
			return modelCommandRespond;
		}

		protected override TWDModelResult ValidateCommand(TWDModelManager manager)
		{
			if (WorldBossGuildFullSnapshot == null)
			{
				manager.Debug.LogError("AttackWorldBossCellCommand: Guild has no WorldBossGuildMatchSnapshot");
				return TWDModelResult.Error;
			}
			if (string.IsNullOrEmpty(CapturePoint))
			{
				manager.Debug.LogError("AttackWorldBossCellCommand: CapturePoint is empty");
				return TWDModelResult.Error;
			}
			if (string.IsNullOrEmpty(Cell))
			{
				manager.Debug.LogError("AttackWorldBossCellCommand: Cell is empty");
				return TWDModelResult.Error;
			}
			WorldBossSeasonDefinition worldBossSeasonDefinition = manager.GameEconomyData.FindWorldBossSeasonDefinition(base.SeasonId);
			if (worldBossSeasonDefinition == null)
			{
				manager.Debug.LogError("AttackWorldBossCellCommand: Season definition not found: " + base.SeasonId);
				return TWDModelResult.Error;
			}
			if (!worldBossSeasonDefinition.IsOpen(manager.Player.UtcTimeStamp))
			{
				manager.Debug.LogError("AttackWorldBossCellCommand: Season is not open: " + base.SeasonId);
				return TWDModelResult.Error;
			}
			WorldBossCycleDefinition worldBossCycleDefinition = manager.GameEconomyData.FindWorldBossCycleDefinition(base.SeasonId, base.CycleId);
			if (worldBossCycleDefinition == null)
			{
				manager.Debug.LogError("AttackWorldBossCellCommand: Cycle definition not found: " + base.CycleId);
				return TWDModelResult.Error;
			}
			if (!worldBossCycleDefinition.IsOpen(manager.Player.UtcTimeStamp))
			{
				manager.Debug.LogError("AttackWorldBossCellCommand: Cycle combat window is not open for CycleId: " + base.CycleId);
				return TWDModelResult.Error;
			}
			if (WorldBossGuildFullSnapshot.Match == null)
			{
				manager.Debug.LogError("AttackWorldBossCellCommand: WorldBossGuildFullSnapshot.Match is null");
				return TWDModelResult.Error;
			}
			_isPVECapturePoint = false;
			WorldBossBattlegroundDefinition worldBossBattlegroundDefinition = manager.GameEconomyData.FindWorldBossBattlegroundDefinitionByCapturePoint(CapturePoint, WorldBossGuildFullSnapshot.Match.BattleDifficulty);
			if (worldBossBattlegroundDefinition != null)
			{
				_isPVECapturePoint = worldBossBattlegroundDefinition.IsPVECapturePointType();
			}
			_missionType = manager.Player.WorldBossModelManager.ResolveMissionTypeForCell(worldBossBattlegroundDefinition, CapturePoint, Cell);
			if (_isPVECapturePoint)
			{
				if (WorldBossGuildFullSnapshot.Match.GroupIdA == GroupId)
				{
					if (!CapturePoint.EndsWith("-Blue"))
					{
						manager.Debug.LogError("AttackWorldBossCellCommand: CapturePoint does not match GroupIdA");
						return TWDModelResult.Error;
					}
				}
				else
				{
					if (!(WorldBossGuildFullSnapshot.Match.GroupIdB == GroupId))
					{
						manager.Debug.LogError("AttackWorldBossCellCommand: GroupId not found in WorldBossGuildFullSnapshot.Match");
						return TWDModelResult.Error;
					}
					if (!CapturePoint.EndsWith("-Red"))
					{
						manager.Debug.LogError("AttackWorldBossCellCommand: CapturePoint does not match GroupIdB");
						return TWDModelResult.Error;
					}
				}
			}
			if (WorldBossGuildFullSnapshot.Match.CycleId > 0 && WorldBossGuildFullSnapshot.Match.CycleId != base.CycleId)
			{
				manager.Debug.LogError($"AttackWorldBossCellCommand: CycleId mismatch. Command={base.CycleId}, Model={WorldBossGuildFullSnapshot.Match.CycleId}");
				return TWDModelResult.Error;
			}
			if (WorldBossGuildFullSnapshot.Match.SeasonId > 0 && WorldBossGuildFullSnapshot.Match.SeasonId != base.SeasonId)
			{
				manager.Debug.LogError($"AttackWorldBossCellCommand: SeasonId mismatch. Command={base.SeasonId}, Model={WorldBossGuildFullSnapshot.Match.SeasonId}");
				return TWDModelResult.Error;
			}
			TWDModelResult tWDModelResult = (IsDebugMode ? ValidateParticipantHeroCharges(manager) : ValidateParticipantSurvivorsAndFatigue(manager));
			if (tWDModelResult != TWDModelResult.OK)
			{
				return tWDModelResult;
			}
			if (!_isPVECapturePoint)
			{
				PlayerModel player = manager.Player;
				if (player != null && player.WorldBossModelManager?.GetCellEnterAction(CapturePoint, Cell) == WorldBossCellEnterAction.DirectOccupy)
				{
					manager.Debug.LogError("AttackWorldBossCellCommand: direct PVP occupation must use OccupyWorldBossEmptyCellCommand");
					return TWDModelResult.Error;
				}
			}
			if (base.ParticipantSurvivorIds != null && base.ParticipantSurvivorIds.Count > 0)
			{
				HashSet<string> hashSet = manager.Player?.WorldBossModelManager?.GetMyDeployedSurvivorIds();
				if (hashSet != null)
				{
					foreach (string participantSurvivorId in base.ParticipantSurvivorIds)
					{
						if (!string.IsNullOrEmpty(participantSurvivorId) && hashSet.Contains(participantSurvivorId))
						{
							manager.Debug.LogError("AttackWorldBossCellCommand: hero already deployed elsewhere: " + participantSurvivorId);
							return TWDModelResult.Error;
						}
					}
				}
			}
			if (base.ParticipantSurvivorIds != null && base.ParticipantSurvivorIds.Count > 0)
			{
				HashSet<string> hashSet2 = manager.Player?.WorldBossModelManager?.GetMyReturningSurvivorIds();
				if (hashSet2 != null)
				{
					foreach (string participantSurvivorId2 in base.ParticipantSurvivorIds)
					{
						if (!string.IsNullOrEmpty(participantSurvivorId2) && hashSet2.Contains(participantSurvivorId2))
						{
							manager.Debug.LogError("AttackWorldBossCellCommand: hero is returning and cannot be deployed: " + participantSurvivorId2);
							return TWDModelResult.Error;
						}
					}
				}
			}
			if (!_isPVECapturePoint)
			{
				int valueOrDefault = (manager.GameEconomyData?.WorldBossConfig?.TeamLimit).GetValueOrDefault();
				if (valueOrDefault > 0)
				{
					int valueOrDefault2 = (manager.Player?.WorldBossModelManager?.GetMyDispatchedTeamCount()).GetValueOrDefault();
					if (valueOrDefault2 >= valueOrDefault)
					{
						manager.Debug.LogError($"AttackWorldBossCellCommand: deployed team limit reached ({valueOrDefault2}/{valueOrDefault})");
						return TWDModelResult.Error;
					}
				}
			}
			return TWDModelResult.OK;
		}

		protected override TWDModelResult ExecuteOnServer(TWDModelManager manager)
		{
			_startBattleUTCMs = manager.Player.UtcTimeStamp;
			WorldBossConfig worldBossConfig = manager.GameEconomyData?.WorldBossConfig;
			if (worldBossConfig == null)
			{
				manager.Debug.LogError("AttackWorldBossCellCommand: WorldBossConfig is null");
				return TWDModelResult.Error;
			}
			_battleTimeLimitMs = worldBossConfig.BattleTimeLimit * 1000;
			string defenderInfo = null;
			if (!_isPVECapturePoint && base.ParticipantSurvivorIds != null && base.ParticipantSurvivorIds.Count > 0)
			{
				GuildBattleParticipantInfo guildBattleParticipantInfo = GvGModelHelper.CreateParticipantFromSurvivorIds(manager.Player, manager.GameEconomyData, base.ParticipantSurvivorIds);
				if (guildBattleParticipantInfo != null)
				{
					defenderInfo = manager.GetMessageSerializer().Serialize(guildBattleParticipantInfo);
				}
			}
			string guildId = manager.Player.GuildId;
			long participationScore = WorldBossCombatHelper.GetParticipationScore(_missionType, worldBossConfig);
			WorldBossOperationResult worldBossOperationResult = manager.ServerService.WorldBossAttackCell(new WorldBossAttackCellOperationRequest
			{
				GroupId = guildId,
				PlayerHashedId = manager.Player.HashedId,
				SeasonId = base.SeasonId,
				CycleId = base.CycleId,
				CapturePoint = CapturePoint,
				Cell = Cell,
				StartBattleUTCMs = _startBattleUTCMs,
				TimeLimitMs = _battleTimeLimitMs,
				IsPVECapturePoint = _isPVECapturePoint,
				CellHasNoBattle = false,
				DefenderInfo = defenderInfo,
				SurvivorIds = base.ParticipantSurvivorIds,
				EnemyDurability = worldBossConfig.EnemyDurability,
				PVEGuardianLoss = worldBossConfig.PVEEnemyLoss,
				PVPGuardianLoss = worldBossConfig.PVPEnemyLoss,
				BeforeProtectionSeconds = worldBossConfig.BeforeProtection,
				ProtectionConfig = worldBossConfig.Protection,
				TowerAConfig = worldBossConfig.TowerA,
				TowerAEffConfig = worldBossConfig.TowerAEff,
				TowerBConfig = worldBossConfig.TowerB,
				TowerBEffConfig = worldBossConfig.TowerBEff,
				DepotConfig = worldBossConfig.Depot,
				DepotEffConfig = worldBossConfig.DepotEff,
				DepotEffBossBattleTimeConfig = worldBossConfig.DepotEffBossBattleTime,
				ParticipationScore = participationScore,
				PlayerName = manager.Player.Name,
				PlayerEmblem = SerializeEmblem(manager.Player.PlayerEmblem)
			});
			if (worldBossOperationResult == null || !worldBossOperationResult.Success)
			{
				manager.Debug.LogError("AttackWorldBossCellCommand: IServerService.WorldBossAttackCell returned null");
				if (!(worldBossOperationResult?.Message == "cell_occupied"))
				{
					return TWDModelResult.Error;
				}
				return TWDModelResult.WorldBossCellOccupied;
			}
			return TWDModelResult.OK;
		}

		internal static string SerializeEmblem(PlayerEmblem emblem)
		{
			if (emblem != null)
			{
				return $"{emblem.IconIndex};{emblem.BorderIndex};{emblem.ColorIndex}";
			}
			return null;
		}
	}
}
