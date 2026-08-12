using System;

namespace TWDModel
{
	[Serializable]
	public class OutpostTier
	{
		public string Id;

		public int TierSetId;

		public string LocalizationKey;

		public TierType TierType;

		public int Rank;

		public int MinInfluence;

		public int MaxInfluence;

		public string Reward;

		public int ResetInfluence;

		public int AttackerWinInfluence;

		public int AttackerLossInfluence;

		public int DefenderWinInfluence;

		public int DefenderLossInfluence;

		public Rewards GetRewards()
		{
			return new Rewards(Reward);
		}
	}
}
