using BaseModel;

namespace TWDModel
{
	public class ClaimDailyLoginCampaignRewardCommand : ModelCommand
	{
		public int RewardId { get; set; }

		public ClaimDailyLoginCampaignRewardCommand()
		{
		}

		public ClaimDailyLoginCampaignRewardCommand(int rewardId)
		{
			RewardId = rewardId;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelResult result = TWDModelResult.Error;
			if (manager is TWDModelManager { Player: not null } tWDModelManager && tWDModelManager.Player.DailyLoginCalendar != null && tWDModelManager.Player.DailyLoginCalendar.TryClaimReward(RewardId))
			{
				result = TWDModelResult.OK;
			}
			return new NGModelCommandRespond(this, result);
		}
	}
}
