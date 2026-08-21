using System;

namespace TWDModel
{
	[Serializable]
	public class WorldBossConfig
	{
		public int BattleTimeLimit;

		public int EnemyDurability;

		public int PVEEnemyLoss;

		public int PVPEnemyLoss;

		public int PVPEnemyPerDieLoss;

		public int Withdraw;

		public int WithdrawGoldCost;

		public string TowerA;

		public string TowerAEff;

		public string TowerB;

		public string TowerBEff;

		public string Depot;

		public string DepotEff;

		public int BeforeProtection;

		public string Protection;

		public int DepotEffBossBattleTime;

		public int DailyRefresh;

		public int DailyBossBattleTime;

		public int TeamLimit;

		public int DailyHeroBattleLimit;

		public int HeroBattleRecoverDuration;

		public long PlayerScorePVE;

		public long PlayerScorePVP;

		public long PlayerScorePVPSuccess;

		public long PlayerScoreBossBattle;

		public int LeaderboardThumbTime;

		public string LeaderboardThumbReward;

		public int SignUpNumNeed;

		public int SignUpCloseTime;

		public int SelectDiffcCloseTime;

		public int MatchBeforeStart;
	}
}
