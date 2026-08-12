using BaseModel;

namespace TWDModel
{
	public class ClaimAllPastCampaignRewardsCommand : ModelCommand
	{
		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelResult result = TWDModelResult.Error;
			if (manager is TWDModelManager { Player: not null } tWDModelManager && tWDModelManager.Player.CampaignModel != null && tWDModelManager.Player.CampaignModel.ContainsPastCampaignRewards())
			{
				int num = tWDModelManager.Player.CampaignModel.UnclaimedPastRewards.Count;
				for (int i = 0; i < tWDModelManager.Player.CampaignModel.UnclaimedPastRewards.Count; i++)
				{
					CampaignRewardModelItem campaignRewardModelItem = tWDModelManager.Player.CampaignModel.UnclaimedPastRewards[i];
					if (campaignRewardModelItem != null && tWDModelManager.Player.CampaignModel.TryClaimReward(campaignRewardModelItem.ModelId))
					{
						num--;
					}
				}
				if (num == 0)
				{
					result = TWDModelResult.OK;
				}
			}
			return new NGModelCommandRespond(this, result);
		}
	}
}
