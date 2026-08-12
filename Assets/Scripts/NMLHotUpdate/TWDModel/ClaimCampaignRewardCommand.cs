using BaseModel;

namespace TWDModel
{
	public class ClaimCampaignRewardCommand : ModelCommand
	{
		public int RewardId { get; set; }

		public ClaimCampaignRewardCommand()
		{
		}

		public ClaimCampaignRewardCommand(int rewardId)
		{
			RewardId = rewardId;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelResult result = TWDModelResult.Error;
			if (manager is TWDModelManager { Player: not null } tWDModelManager && tWDModelManager.Player.CampaignModel != null && tWDModelManager.Player.CampaignModel.CanClaimRewards && tWDModelManager.Player.CampaignModel.TryClaimReward(RewardId))
			{
				result = TWDModelResult.OK;
			}
			return new NGModelCommandRespond(this, result);
		}
	}
}
