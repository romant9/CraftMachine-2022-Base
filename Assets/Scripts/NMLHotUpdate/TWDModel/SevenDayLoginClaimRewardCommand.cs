using BaseModel;

namespace TWDModel
{
	public class SevenDayLoginClaimRewardCommand : ModelCommand
	{
		public int Day { get; set; }

		public SevenDayLoginRewardType ClaimRewardType { get; set; }

		public SevenDayLoginClaimRewardCommand(int day, SevenDayLoginRewardType claimRewardType)
		{
			Day = day;
			ClaimRewardType = claimRewardType;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelResult result = TWDModelResult.Error;
			if (!(manager is TWDModelManager tWDModelManager))
			{
				return new NGModelCommandRespond(this, result);
			}
			if (tWDModelManager.Player == null)
			{
				return new NGModelCommandRespond(this, result);
			}
			if (tWDModelManager.Player.SevenDayLoginManager == null)
			{
				return new NGModelCommandRespond(this, result);
			}
			SevenDayLoginPeriodModel currentPeriodModel = tWDModelManager.Player.SevenDayLoginManager.CurrentPeriodModel;
			if (currentPeriodModel == null)
			{
				return new NGModelCommandRespond(this, result);
			}
			if (tWDModelManager.Player.SevenDayLoginManager.CurrentPeriodId != currentPeriodModel.PeriodId)
			{
				return new NGModelCommandRespond(this, result);
			}
			if (currentPeriodModel.TryClaimReward(Day, ClaimRewardType))
			{
				result = TWDModelResult.OK;
			}
			return new NGModelCommandRespond(this, result);
		}
	}
}
