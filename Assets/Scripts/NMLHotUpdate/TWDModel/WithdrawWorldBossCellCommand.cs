using System.Collections.Generic;
using BaseModel;

namespace TWDModel
{
	public class WithdrawWorldBossCellCommand : TWDWorldBossInternalCommand
	{
		public string CapturePoint { get; set; }

		public string Cell { get; set; }

		public List<string> SurvivorIds { get; set; }

		public WithdrawWorldBossCellCommand()
		{
		}

		public WithdrawWorldBossCellCommand(int seasonId, int cycleId, string capturePoint, string cell, List<string> survivorIds = null)
			: base(seasonId, cycleId)
		{
			CapturePoint = capturePoint;
			Cell = cell;
			SurvivorIds = survivorIds;
		}

		protected override TWDModelResult ValidateCommand(TWDModelManager manager)
		{
			if (WorldBossGuildFullSnapshot == null)
			{
				manager.Debug.LogError("WithdrawWorldBossCellCommand: Guild has no WorldBossGuildMatchSnapshot");
				return TWDModelResult.Error;
			}
			if (string.IsNullOrEmpty(CapturePoint))
			{
				manager.Debug.LogError("WithdrawWorldBossCellCommand: CapturePoint is empty");
				return TWDModelResult.Error;
			}
			if (string.IsNullOrEmpty(Cell))
			{
				manager.Debug.LogError("WithdrawWorldBossCellCommand: Cell is empty");
				return TWDModelResult.Error;
			}
			WorldBossSeasonDefinition worldBossSeasonDefinition = manager.GameEconomyData.FindWorldBossSeasonDefinition(base.SeasonId);
			if (worldBossSeasonDefinition == null)
			{
				manager.Debug.LogError("WithdrawWorldBossCellCommand: Season definition not found: " + base.SeasonId);
				return TWDModelResult.Error;
			}
			if (!worldBossSeasonDefinition.IsOpen(manager.Player.UtcTimeStamp))
			{
				manager.Debug.LogError("WithdrawWorldBossCellCommand: Season is not open: " + base.SeasonId);
				return TWDModelResult.Error;
			}
			WorldBossCycleDefinition worldBossCycleDefinition = manager.GameEconomyData.FindWorldBossCycleDefinition(base.SeasonId, base.CycleId);
			if (worldBossCycleDefinition == null)
			{
				manager.Debug.LogError("WithdrawWorldBossCellCommand: Cycle definition not found: " + base.CycleId);
				return TWDModelResult.Error;
			}
			if (!worldBossCycleDefinition.IsOpen(manager.Player.UtcTimeStamp))
			{
				manager.Debug.LogError("WithdrawWorldBossCellCommand: Cycle combat window is not open for CycleId: " + base.CycleId);
				return TWDModelResult.Error;
			}
			if (WorldBossGuildFullSnapshot.Match == null)
			{
				manager.Debug.LogError("WithdrawWorldBossCellCommand: WorldBossGuildFullSnapshot.Match is null");
				return TWDModelResult.Error;
			}
			if (WorldBossGuildFullSnapshot.Match.CycleId > 0 && WorldBossGuildFullSnapshot.Match.CycleId != base.CycleId)
			{
				manager.Debug.LogError($"WithdrawWorldBossCellCommand: CycleId mismatch. Command={base.CycleId}, Model={WorldBossGuildFullSnapshot.Match.CycleId}");
				return TWDModelResult.Error;
			}
			if (WorldBossGuildFullSnapshot.Match.SeasonId > 0 && WorldBossGuildFullSnapshot.Match.SeasonId != base.SeasonId)
			{
				manager.Debug.LogError($"WithdrawWorldBossCellCommand: SeasonId mismatch. Command={base.SeasonId}, Model={WorldBossGuildFullSnapshot.Match.SeasonId}");
				return TWDModelResult.Error;
			}
			WorldBossBattlegroundDefinition worldBossBattlegroundDefinition = manager.GameEconomyData.FindWorldBossBattlegroundDefinitionByCapturePoint(CapturePoint, WorldBossGuildFullSnapshot.Match.BattleDifficulty);
			if (worldBossBattlegroundDefinition != null && worldBossBattlegroundDefinition.IsPVECapturePointType())
			{
				manager.Debug.LogError("WithdrawWorldBossCellCommand: Withdraw is not allowed on PVE capture point: " + CapturePoint);
				return TWDModelResult.Error;
			}
			return TWDModelResult.OK;
		}

		protected override TWDModelResult ExecuteOnServer(TWDModelManager manager)
		{
			string guildId = manager.Player.GuildId;
			WorldBossConfig worldBossConfig = manager.GameEconomyData.WorldBossConfig;
			long withdrawDurationMs = (long)worldBossConfig.Withdraw * 1000L;
			WorldBossOperationResult worldBossOperationResult = manager.ServerService.WorldBossWithdrawCell(new WorldBossWithdrawCellOperationRequest
			{
				GroupId = guildId,
				PlayerHashedId = manager.Player.HashedId,
				SeasonId = base.SeasonId,
				CycleId = base.CycleId,
				CapturePoint = CapturePoint,
				Cell = Cell,
				SurvivorIds = SurvivorIds,
				WithdrawDurationMs = withdrawDurationMs,
				BeforeProtectionSeconds = worldBossConfig.BeforeProtection,
				ProtectionConfig = worldBossConfig.Protection,
				TowerAConfig = worldBossConfig.TowerA,
				TowerAEffConfig = worldBossConfig.TowerAEff,
				TowerBConfig = worldBossConfig.TowerB,
				TowerBEffConfig = worldBossConfig.TowerBEff,
				DepotConfig = worldBossConfig.Depot,
				DepotEffConfig = worldBossConfig.DepotEff,
				DepotEffBossBattleTimeConfig = worldBossConfig.DepotEffBossBattleTime
			});
			if (worldBossOperationResult == null || !worldBossOperationResult.Success)
			{
				manager.Debug.LogError("WithdrawWorldBossCellCommand: IServerService.WorldBossWithdrawCell returned null");
				return TWDModelResult.Error;
			}
			return TWDModelResult.OK;
		}
	}
}
