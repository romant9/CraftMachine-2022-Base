using BaseModel;

namespace TWDModel
{
	public class ClaimWorldBossSettlementRewardCommand : TWDWorldBossInternalCommand
	{
		public Rewards ClaimedRewards { get; set; }

		public int RewardDifficulty { get; set; }

		public long MyGuildScore { get; set; }

		public long OpponentGuildScore { get; set; }

		public long PassScore { get; set; }

		public ClaimWorldBossSettlementRewardCommand()
		{
		}

		public ClaimWorldBossSettlementRewardCommand(int seasonId, int cycleId, int rewardDifficulty, long myGuildScore, long opponentGuildScore, long passScore)
			: base(seasonId, cycleId)
		{
			RewardDifficulty = rewardDifficulty;
			MyGuildScore = myGuildScore;
			OpponentGuildScore = opponentGuildScore;
			PassScore = passScore;
		}

		protected override TWDModelResult ValidateCommand(TWDModelManager manager)
		{
			WorldBossModelManager worldBossModelManager = manager.Player?.WorldBossModelManager;
			if (worldBossModelManager == null || base.SeasonId <= 0 || base.CycleId <= 0)
			{
				manager.Debug.LogError("ClaimWorldBossSettlementRewardCommand: invalid WorldBoss settlement target");
				return TWDModelResult.Error;
			}
			if (worldBossModelManager.GetSettlementRewardDefinition(base.SeasonId, RewardDifficulty) == null)
			{
				manager.Debug.LogError($"ClaimWorldBossSettlementRewardCommand: no WorldBossDifficulty config for season {base.SeasonId} difficulty {RewardDifficulty}");
				return TWDModelResult.Error;
			}
			return TWDModelResult.OK;
		}

		public override IModelCommandRespond Execute(ModelManager modelManager)
		{
			IModelCommandRespond modelCommandRespond = base.Execute(modelManager);
			if (modelCommandRespond == null || modelCommandRespond.Code != 0)
			{
				return modelCommandRespond;
			}
			TWDModelManager tWDModelManager = modelManager as TWDModelManager;
			WorldBossModelManager worldBossModelManager = tWDModelManager?.Player?.WorldBossModelManager;
			if (worldBossModelManager == null)
			{
				tWDModelManager?.Debug.LogError("ClaimWorldBossSettlementRewardCommand: WorldBossModelManager is null when granting settlement reward");
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			Rewards rewards = new WorldBossBaseSnapshotHelper().BuildSettlementRewards(worldBossModelManager, base.SeasonId, RewardDifficulty, MyGuildScore, OpponentGuildScore, PassScore);
			if (rewards == null)
			{
				tWDModelManager.Debug.LogError($"ClaimWorldBossSettlementRewardCommand: no reward config for season {base.SeasonId} difficulty {RewardDifficulty}");
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			rewards.Give(tWDModelManager);
			ClaimedRewards = rewards;
			worldBossModelManager.MarkSettlementShown(base.SeasonId, base.CycleId);
			return modelCommandRespond;
		}

		protected override TWDModelResult ExecuteOnServer(TWDModelManager manager)
		{
			if (manager.Player?.WorldBossModelManager == null)
			{
				manager.Debug.LogError("ClaimWorldBossSettlementRewardCommand: WorldBossModelManager is null on server");
				return TWDModelResult.Error;
			}
			WorldBossClaimSettlementResult worldBossClaimSettlementResult = manager.ServerService.WorldBossClaimSettlementReward(new WorldBossClaimSettlementRewardOperationRequest
			{
				GroupId = manager.Player.GuildId,
				PlayerHashedId = manager.Player.HashedId,
				SeasonId = base.SeasonId,
				CycleId = base.CycleId
			});
			if (worldBossClaimSettlementResult == null || !worldBossClaimSettlementResult.Success)
			{
				manager.Debug.LogError("ClaimWorldBossSettlementRewardCommand: IServerService.WorldBossClaimSettlementReward failed: " + worldBossClaimSettlementResult?.Message);
				return TWDModelResult.Error;
			}
			return TWDModelResult.OK;
		}
	}
}
