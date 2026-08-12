using System;

namespace TWDModel
{
	[Serializable]
	public class BattlePassConfig
	{
		public string CapRefreshUTC;

		public int BCPerKill;

		public int MaxDailyBCFromKills;

		public int BonusChestCost;

		public int CouncilLockLevel;

		public string TierUnlockGoldPrice;
	}
}
