using System.Collections.Generic;
using BaseModel;

namespace TWDModel
{
	public class OccupyWorldBossEmptyCellCommand : TWDWorldBossInternalCommand
	{
		public string CapturePoint { get; set; }

		public string Cell { get; set; }

		public List<string> ParticipantSurvivorIds { get; set; }

		public OccupyWorldBossEmptyCellCommand()
		{
		}

		public OccupyWorldBossEmptyCellCommand(int seasonId, int cycleId, string capturePoint, string cell, List<string> participantSurvivorIds = null)
			: base(seasonId, cycleId)
		{
			CapturePoint = capturePoint;
			Cell = cell;
			ParticipantSurvivorIds = participantSurvivorIds;
		}

		public override IModelCommandRespond Execute(ModelManager modelManager)
		{
			IModelCommandRespond modelCommandRespond = base.Execute(modelManager);
			if (modelCommandRespond != null && modelCommandRespond.Code == 0)
			{
				TWDModelManager tWDModelManager = modelManager as TWDModelManager;
				WorldBossModelManager worldBossModelManager = tWDModelManager?.Player?.WorldBossModelManager;
				if (worldBossModelManager != null && worldBossModelManager.RecordParticipation(base.SeasonId, base.CycleId) && tWDModelManager.ServerService == null)
				{
					worldBossModelManager.TrackOptimisticParticipation(base.SequenceId, base.SeasonId, base.CycleId);
				}
			}
			return modelCommandRespond;
		}

		protected override TWDModelResult ValidateCommand(TWDModelManager manager)
		{
			if (WorldBossGuildFullSnapshot?.Match == null)
			{
				manager.Debug.LogError("OccupyWorldBossEmptyCellCommand: WorldBoss snapshot/match is null");
				return TWDModelResult.Error;
			}
			if (string.IsNullOrEmpty(CapturePoint) || string.IsNullOrEmpty(Cell))
			{
				manager.Debug.LogError("OccupyWorldBossEmptyCellCommand: CapturePoint/Cell is empty");
				return TWDModelResult.Error;
			}
			WorldBossSeasonDefinition worldBossSeasonDefinition = manager.GameEconomyData.FindWorldBossSeasonDefinition(base.SeasonId);
			WorldBossCycleDefinition worldBossCycleDefinition = manager.GameEconomyData.FindWorldBossCycleDefinition(base.SeasonId, base.CycleId);
			if (worldBossSeasonDefinition == null || !worldBossSeasonDefinition.IsOpen(manager.Player.UtcTimeStamp) || worldBossCycleDefinition == null || !worldBossCycleDefinition.IsOpen(manager.Player.UtcTimeStamp))
			{
				manager.Debug.LogError("OccupyWorldBossEmptyCellCommand: season/cycle is not open");
				return TWDModelResult.Error;
			}
			WorldBossMatchSnapshot match = WorldBossGuildFullSnapshot.Match;
			if ((match.SeasonId > 0 && match.SeasonId != base.SeasonId) || (match.CycleId > 0 && match.CycleId != base.CycleId))
			{
				manager.Debug.LogError("OccupyWorldBossEmptyCellCommand: season/cycle mismatch");
				return TWDModelResult.Error;
			}
			WorldBossBattlegroundDefinition worldBossBattlegroundDefinition = manager.GameEconomyData.FindWorldBossBattlegroundDefinitionByCapturePoint(CapturePoint, match.BattleDifficulty);
			if (worldBossBattlegroundDefinition == null || worldBossBattlegroundDefinition.IsPVECapturePointType())
			{
				manager.Debug.LogError("OccupyWorldBossEmptyCellCommand: only PVP capture points support direct occupation");
				return TWDModelResult.Error;
			}
			if (ParticipantSurvivorIds == null || ParticipantSurvivorIds.Count == 0)
			{
				manager.Debug.LogError("OccupyWorldBossEmptyCellCommand: participant survivors are empty");
				return TWDModelResult.Error;
			}
			SurvivorContainerModel survivorContainer = manager.Player.SurvivorContainer;
			if (survivorContainer == null || survivorContainer.Survivors == null)
			{
				manager.Debug.LogError("OccupyWorldBossEmptyCellCommand: player survivor container/collection is null");
				return TWDModelResult.Error;
			}
			ModelList<SurvivorModel> survivors = survivorContainer.Survivors;
			HashSet<string> hashSet = new HashSet<string>();
			foreach (string participantSurvivorId in ParticipantSurvivorIds)
			{
				if (string.IsNullOrEmpty(participantSurvivorId))
				{
					manager.Debug.LogError("OccupyWorldBossEmptyCellCommand: participant survivor id is empty");
					return TWDModelResult.Error;
				}
				if (!hashSet.Add(participantSurvivorId))
				{
					manager.Debug.LogError("OccupyWorldBossEmptyCellCommand: duplicate survivor: " + participantSurvivorId);
					return TWDModelResult.Error;
				}
				bool flag = false;
				for (int i = 0; i < survivors.Count; i++)
				{
					SurvivorModel survivorModel = survivors[i];
					if (survivorModel != null && survivorModel.IdForAnalytics == participantSurvivorId)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					manager.Debug.LogError("OccupyWorldBossEmptyCellCommand: survivor does not belong to player: " + participantSurvivorId);
					return TWDModelResult.Error;
				}
			}
			WorldBossModelManager worldBossModelManager = manager.Player?.WorldBossModelManager;
			if (worldBossModelManager == null || worldBossModelManager.GetCellEnterAction(CapturePoint, Cell) != WorldBossCellEnterAction.DirectOccupy)
			{
				manager.Debug.LogError("OccupyWorldBossEmptyCellCommand: cell is not directly occupiable");
				return TWDModelResult.Error;
			}
			HashSet<string> myDeployedSurvivorIds = worldBossModelManager.GetMyDeployedSurvivorIds();
			HashSet<string> myReturningSurvivorIds = worldBossModelManager.GetMyReturningSurvivorIds();
			foreach (string participantSurvivorId2 in ParticipantSurvivorIds)
			{
				if ((myDeployedSurvivorIds != null && myDeployedSurvivorIds.Contains(participantSurvivorId2)) || (myReturningSurvivorIds != null && myReturningSurvivorIds.Contains(participantSurvivorId2)))
				{
					manager.Debug.LogError("OccupyWorldBossEmptyCellCommand: hero is already deployed or returning: " + participantSurvivorId2);
					return TWDModelResult.Error;
				}
			}
			int valueOrDefault = (manager.GameEconomyData?.WorldBossConfig?.TeamLimit).GetValueOrDefault();
			if (valueOrDefault > 0 && worldBossModelManager.GetMyDispatchedTeamCount() >= valueOrDefault)
			{
				manager.Debug.LogError($"OccupyWorldBossEmptyCellCommand: deployed team limit reached ({worldBossModelManager.GetMyDispatchedTeamCount()}/{valueOrDefault})");
				return TWDModelResult.Error;
			}
			return TWDModelResult.OK;
		}

		protected override TWDModelResult ExecuteOnServer(TWDModelManager manager)
		{
			string defenderInfo = null;
			if (ParticipantSurvivorIds != null && ParticipantSurvivorIds.Count > 0)
			{
				GuildBattleParticipantInfo guildBattleParticipantInfo = GvGModelHelper.CreateParticipantFromSurvivorIds(manager.Player, manager.GameEconomyData, ParticipantSurvivorIds);
				if (guildBattleParticipantInfo != null)
				{
					defenderInfo = manager.GetMessageSerializer().Serialize(guildBattleParticipantInfo);
				}
			}
			WorldBossConfig worldBossConfig = manager.GameEconomyData.WorldBossConfig;
			WorldBossCellDefinition worldBossCellDefinition = manager.GameEconomyData.FindWorldBossCellDefinition(CapturePoint, Cell);
			bool isPVECapturePoint = manager.GameEconomyData.FindWorldBossBattlegroundDefinitionByCapturePoint(CapturePoint, WorldBossGuildFullSnapshot.Match.BattleDifficulty)?.IsPVECapturePointType() ?? false;
			bool cellHasNoBattle = worldBossCellDefinition != null && !worldBossCellDefinition.HaveBattle;
			WorldBossOperationResult worldBossOperationResult = manager.ServerService.WorldBossOccupyEmptyCell(new WorldBossOccupyEmptyCellOperationRequest
			{
				GroupId = manager.Player.GuildId,
				PlayerHashedId = manager.Player.HashedId,
				SeasonId = base.SeasonId,
				CycleId = base.CycleId,
				CapturePoint = CapturePoint,
				Cell = Cell,
				IsPVECapturePoint = isPVECapturePoint,
				CellHasNoBattle = cellHasNoBattle,
				DefenderInfo = defenderInfo,
				SurvivorIds = ParticipantSurvivorIds,
				EnemyDurability = worldBossConfig.EnemyDurability,
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
				PlayerName = manager.Player.Name,
				PlayerEmblem = AttackWorldBossCellCommand.SerializeEmblem(manager.Player.PlayerEmblem)
			});
			if (worldBossOperationResult == null || !worldBossOperationResult.Success)
			{
				manager.Debug.LogError("OccupyWorldBossEmptyCellCommand: occupy failed: " + worldBossOperationResult?.Message);
				if (!(worldBossOperationResult?.Message == "cell_occupied"))
				{
					return TWDModelResult.Error;
				}
				return TWDModelResult.WorldBossCellOccupied;
			}
			return TWDModelResult.OK;
		}
	}
}
