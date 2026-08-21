using System;
using System.Collections.Generic;
using BaseModel;

namespace TWDModel
{
	public class AttackWorldBossTankCommand : WorldBossParticipantAttackCommand
	{
		public string BossBattleId { get; private set; }

		public AttackWorldBossTankCommand()
		{
		}

		public AttackWorldBossTankCommand(int seasonId, int cycleId, List<string> participantSurvivorIds = null)
			: base(seasonId, cycleId)
		{
			base.ParticipantSurvivorIds = participantSurvivorIds;
			BossBattleId = Guid.NewGuid().ToString("N");
		}

		public override IModelCommandRespond Execute(ModelManager modelManager)
		{
			IModelCommandRespond modelCommandRespond = base.Execute(modelManager);
			if (modelCommandRespond != null && modelCommandRespond.Code == 0 && modelManager is TWDModelManager { Player: not null } tWDModelManager)
			{
				ApplyBossBattleConsumption(tWDModelManager);
				tWDModelManager.Player.MapContainerModel.ClearAttackTargetMissionData();
				tWDModelManager.Player.WorldBossModelManager.RecordParticipation(base.SeasonId, base.CycleId);
				tWDModelManager.Player.WorldBossModelManager.SetAttackTankTarget(base.SeasonId, base.CycleId, BossBattleId);
			}
			return modelCommandRespond;
		}

		protected override TWDModelResult ValidateCommand(TWDModelManager manager)
		{
			if (WorldBossGuildFullSnapshot == null)
			{
				manager.Debug.LogError("AttackWorldBossCommand: Guild has no WorldBossGuildFullSnapshot");
				return TWDModelResult.Error;
			}
			if (WorldBossGuildFullSnapshot.Match == null)
			{
				manager.Debug.LogError("AttackWorldBossCommand: WorldBossGuildFullSnapshot.Match is null");
				return TWDModelResult.Error;
			}
			if (string.IsNullOrWhiteSpace(BossBattleId))
			{
				manager.Debug.LogError("AttackWorldBossCommand: BossBattleId is empty");
				return TWDModelResult.Error;
			}
			WorldBossSeasonDefinition worldBossSeasonDefinition = manager.GameEconomyData.FindWorldBossSeasonDefinition(base.SeasonId);
			if (worldBossSeasonDefinition == null || !worldBossSeasonDefinition.IsOpen(manager.Player.UtcTimeStamp))
			{
				manager.Debug.LogError("AttackWorldBossCommand: Season not found or not open: " + base.SeasonId);
				return TWDModelResult.Error;
			}
			WorldBossCycleDefinition worldBossCycleDefinition = manager.GameEconomyData.FindWorldBossCycleDefinition(base.SeasonId, base.CycleId);
			if (worldBossCycleDefinition == null || !worldBossCycleDefinition.IsOpen(manager.Player.UtcTimeStamp))
			{
				manager.Debug.LogError("AttackWorldBossCommand: Cycle not found or combat window not open: " + base.CycleId);
				return TWDModelResult.Error;
			}
			if (WorldBossGuildFullSnapshot.Match.SeasonId > 0 && WorldBossGuildFullSnapshot.Match.SeasonId != base.SeasonId)
			{
				manager.Debug.LogError($"AttackWorldBossCommand: SeasonId mismatch. Command={base.SeasonId}, Model={WorldBossGuildFullSnapshot.Match.SeasonId}");
				return TWDModelResult.Error;
			}
			if (WorldBossGuildFullSnapshot.Match.CycleId > 0 && WorldBossGuildFullSnapshot.Match.CycleId != base.CycleId)
			{
				manager.Debug.LogError($"AttackWorldBossCommand: CycleId mismatch. Command={base.CycleId}, Model={WorldBossGuildFullSnapshot.Match.CycleId}");
				return TWDModelResult.Error;
			}
			TWDModelResult tWDModelResult = ValidateParticipantSurvivorsAndFatigue(manager);
			if (tWDModelResult != TWDModelResult.OK)
			{
				return tWDModelResult;
			}
			return ValidateDailyBossBattleTimes(manager);
		}

		protected override TWDModelResult ExecuteOnServer(TWDModelManager manager)
		{
			long participationScore = WorldBossCombatHelper.GetParticipationScore(WorldBossMissionType.BOSS, manager.GameEconomyData?.WorldBossConfig);
			WorldBossOperationResult worldBossOperationResult = manager.ServerService.WorldBossAttackBoss(new WorldBossAttackBossOperationRequest
			{
				GroupId = manager.Player.GuildId,
				PlayerHashedId = manager.Player.HashedId,
				SeasonId = base.SeasonId,
				CycleId = base.CycleId,
				BossBattleId = BossBattleId,
				StartBattleUtcMs = manager.Player.UtcTimeStamp,
				ParticipationScore = participationScore
			});
			if (worldBossOperationResult == null || !worldBossOperationResult.Success)
			{
				return TWDModelResult.Error;
			}
			return TWDModelResult.OK;
		}

		private void ApplyBossBattleConsumption(TWDModelManager manager)
		{
			if (!TryPrepareParticipantCharges(manager, out var preparedCharges))
			{
				manager.Debug.LogError("AttackWorldBossTankCommand: prepare charges failed after validation; skip consumption");
				return;
			}
			if (!TryPrepareDailyBossBattleCount(manager, out var refreshStartUtcMs, out var nextBattleCount))
			{
				manager.Debug.LogError("AttackWorldBossTankCommand: prepare daily boss battle count failed after validation; skip consumption");
				return;
			}
			ApplyPreparedParticipantCharges(manager, preparedCharges);
			manager.Player.WorldBossDailyBattleRefreshUtcMs = refreshStartUtcMs;
			manager.Player.WorldBossDailyBattleCount = nextBattleCount;
		}

		private TWDModelResult ValidateDailyBossBattleTimes(TWDModelManager manager)
		{
			if (!TryPrepareDailyBossBattleCount(manager, out var _, out var _))
			{
				manager.Debug.LogError("AttackWorldBossTankCommand: daily boss battle limit reached or config is invalid");
				return TWDModelResult.Error;
			}
			return TWDModelResult.OK;
		}

		private static bool TryPrepareDailyBossBattleCount(TWDModelManager manager, out long refreshStartUtcMs, out int nextBattleCount)
		{
			refreshStartUtcMs = 0L;
			nextBattleCount = 0;
			PlayerModel playerModel = manager?.Player;
			WorldBossModelManager worldBossModelManager = playerModel?.WorldBossModelManager;
			WorldBossConfig worldBossConfig = manager?.GameEconomyData?.WorldBossConfig;
			if (playerModel == null || worldBossModelManager == null || worldBossConfig == null || !WorldBossModelManager.TryGetDailyRefreshStartUtcMs(playerModel.UtcTimeStamp, worldBossConfig.DailyRefresh, out refreshStartUtcMs))
			{
				return false;
			}
			long dailyBossBattleLimit = worldBossModelManager.GetDailyBossBattleLimit();
			if (dailyBossBattleLimit <= 0)
			{
				return false;
			}
			int num = ((playerModel.WorldBossDailyBattleRefreshUtcMs == refreshStartUtcMs) ? playerModel.WorldBossDailyBattleCount : 0);
			if (num < 0)
			{
				num = 0;
			}
			if (num >= dailyBossBattleLimit || num == int.MaxValue)
			{
				return false;
			}
			nextBattleCount = num + 1;
			return true;
		}
	}
}
