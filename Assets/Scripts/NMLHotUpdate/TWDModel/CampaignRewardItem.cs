namespace TWDModel
{
	public interface CampaignRewardItem
	{
		int Control { get; }

		bool Claimable { get; }

		bool Claimed { get; }

		IReward Reward { get; }

		CampaignRewardsDefinition RewardsDefinition { get; }
	}
}
