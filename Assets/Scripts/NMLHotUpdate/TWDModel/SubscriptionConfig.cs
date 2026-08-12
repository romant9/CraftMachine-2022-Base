using System;

namespace TWDModel
{
	[Serializable]
	public class SubscriptionConfig
	{
		public string WeeklySubscriptionPrice;

		public string MonthlySubscriptionPrice;

		public float RecoveryFactor;

		public int LastStandFreeChance;

		public int CouncilLockLevel;
	}
}
