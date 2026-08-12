namespace TWDModel
{
	public class CampaignRewardTarget : CampaignRewardItem
	{
		public int Control { get; private set; }

		public bool Claimable => false;

		public bool Claimed => false;

		public IReward Reward { get; private set; }

		public CampaignRewardsDefinition RewardsDefinition { get; private set; }

		public CampaignRewardTarget(int control, IReward reward)
		{
			Control = control;
			Reward = reward;
		}

		public CampaignRewardTarget(CampaignRewardsDefinition definition)
		{
			RewardsDefinition = definition;
			Control = definition.Control;
			Reward = definition.RewardEntries.RewardsList[0];
		}
	}
}
