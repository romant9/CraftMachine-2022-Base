namespace TWDModel
{
	public struct ClaimRewardAnalyticsEntry
	{
		public int Tier;

		public int Index;

		public bool IsPremium;

		public bool IsAutoClaimed;

		public int? OverrideSeasonId;

		public ClaimRewardAnalyticsEntry(int tier, int index, bool isPremium, bool isAutoClaimed, int? overrideSeasonId = null)
		{
			Tier = tier;
			Index = index;
			IsPremium = isPremium;
			IsAutoClaimed = isAutoClaimed;
			OverrideSeasonId = overrideSeasonId;
		}
	}
}
