using System;
using BaseModel;

namespace TWDModel
{
	public sealed class WorldBossBaseSnapshotHelper
	{
		public WorldBossGuildBaseSnapshot Snapshot { get; private set; }

		public WorldBossGuildBaseState GuildBaseState => Snapshot?.GuildBaseState;

		public void SetSnapshot(WorldBossGuildBaseSnapshot snapshot)
		{
			Snapshot = snapshot;
		}

		public WorldBossCycleSettlementSnapshot GetLatestPendingSettlement(WorldBossModelManager worldBossModelManager)
		{
			int seasonId = worldBossModelManager?.GetCurrentSeasonId() ?? 0;
			return GetLatestPendingSettlement(worldBossModelManager, seasonId);
		}

		public WorldBossCycleSettlementSnapshot GetLatestPendingSettlement(WorldBossModelManager worldBossModelManager, int seasonId)
		{
			WorldBossCycleSettlementSnapshot worldBossCycleSettlementSnapshot = Snapshot?.Settlement;
			if (worldBossModelManager == null || seasonId <= 0 || worldBossCycleSettlementSnapshot == null || worldBossCycleSettlementSnapshot.SeasonId != seasonId || worldBossModelManager.IsSettlementShown(seasonId, worldBossCycleSettlementSnapshot.CycleId))
			{
				return null;
			}
			return worldBossCycleSettlementSnapshot;
		}

		public Rewards BuildSettlementRewards(WorldBossModelManager worldBossModelManager, int seasonId, int difficulty, long myGuildScore, long opponentGuildScore, long passScore)
		{
			WorldBossDifficultyDefinition worldBossDifficultyDefinition = worldBossModelManager?.GetSettlementRewardDefinition(seasonId, difficulty);
			if (worldBossDifficultyDefinition == null)
			{
				return null;
			}
			int amount = CalculateSettlementRewardAmount(worldBossDifficultyDefinition.Guarantee, worldBossDifficultyDefinition.VSReward, myGuildScore, opponentGuildScore, passScore);
			Rewards rewards = new Rewards();
			rewards.AddRewardCurrency(CurrencyType.WorldBossExchangeCoin, amount, isDiamondExchange: false, canOverflowMax: false);
			return rewards;
		}

		public static int CalculateSettlementRewardAmount(int guarantee, int vsReward, long myGuildScore, long opponentGuildScore, long passScore)
		{
			decimal num = Math.Max(guarantee, 0);
			if (myGuildScore >= passScore && vsReward > 0)
			{
				decimal num2 = Math.Max(myGuildScore, 0L);
				decimal num3 = Math.Max(opponentGuildScore, 0L);
				decimal num4 = num2 + num3;
				if (num4 > 0m)
				{
					num += decimal.Ceiling((decimal)vsReward * num2 / num4);
				}
			}
			if (!(num >= 2147483647m))
			{
				return (int)num;
			}
			return int.MaxValue;
		}
	}
}
