using BaseModel;

namespace TWDModel
{
	public class ReturnThreeDayRewardCommand : ModelCommand
	{
		public int RewardIndex { get; set; }

		public ReturnThreeDayRewardCommand()
		{
		}

		public ReturnThreeDayRewardCommand(int index)
		{
			RewardIndex = index;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			if (!(manager is TWDModelManager { Player: not null } tWDModelManager) || tWDModelManager.Player.ReturnActivityManager == null)
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			ReturnThreeDayModel returnThreeDay = tWDModelManager.Player.ReturnActivityManager.ReturnThreeDay;
			if (returnThreeDay == null || RewardIndex < 0 || !returnThreeDay.RewardIndex(RewardIndex))
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
