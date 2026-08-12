using BaseModel;

namespace TWDModel
{
	public class ActiveFoundationClaimRewardCommand : ModelCommand
	{
		public int Day { get; set; }

		public ActiveFoundationClaimRewardCommand()
		{
		}

		public ActiveFoundationClaimRewardCommand(int day)
		{
			Day = day;
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
			if (tWDModelManager.Player.ActiveFoundationManager == null)
			{
				return new NGModelCommandRespond(this, result);
			}
			ActiveFoundationPeriodModel currentPeriodModel = tWDModelManager.Player.ActiveFoundationManager.CurrentPeriodModel;
			if (currentPeriodModel == null)
			{
				return new NGModelCommandRespond(this, result);
			}
			if (tWDModelManager.Player.ActiveFoundationManager.CurrentPeriodId != currentPeriodModel.PeriodId)
			{
				return new NGModelCommandRespond(this, result);
			}
			if (currentPeriodModel.TryClaimReward(Day))
			{
				result = TWDModelResult.OK;
			}
			return new NGModelCommandRespond(this, result);
		}
	}
}
