using BaseModel;

namespace TWDModel
{
	public class ClaimReturnLoginRewardCommand : ModelCommand
	{
		public int Day { get; set; }

		public ClaimReturnLoginRewardCommand(int day)
		{
			Day = day;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelManager tWDModelManager = manager as TWDModelManager;
			if (tWDModelManager?.Player?.ReturnActivityManager?.ReturnLogin == null)
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			bool flag = tWDModelManager.Player.ReturnActivityManager.ReturnLogin.TryClaimReward(Day);
			return new NGModelCommandRespond(this, (!flag) ? TWDModelResult.Error : TWDModelResult.OK);
		}
	}
}
