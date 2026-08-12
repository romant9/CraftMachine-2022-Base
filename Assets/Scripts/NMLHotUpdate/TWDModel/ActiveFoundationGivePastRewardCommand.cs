using BaseModel;

namespace TWDModel
{
	public class ActiveFoundationGivePastRewardCommand : ModelCommand
	{
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
			tWDModelManager.Player.ActiveFoundationManager.GivePastPeriodsRewards();
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
