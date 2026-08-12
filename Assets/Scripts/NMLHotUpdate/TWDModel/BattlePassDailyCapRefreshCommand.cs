using BaseModel;

namespace TWDModel
{
	public class BattlePassDailyCapRefreshCommand : ModelCommand
	{
		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelResult result = TWDModelResult.Error;
			if (manager is TWDModelManager tWDModelManager)
			{
				tWDModelManager.Player.BattlePass.CheckAndUpdateCapExpiry();
				result = TWDModelResult.OK;
			}
			return new NGModelCommandRespond(this, result);
		}
	}
}
