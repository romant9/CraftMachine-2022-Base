using BaseModel;

namespace TWDModel
{
	public class BattlePassSeasonRefreshCommand : ModelCommand
	{
		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelResult result = TWDModelResult.Error;
			if (manager is TWDModelManager tWDModelManager)
			{
				tWDModelManager.Player.BattlePass.RefreshActiveSeason();
				result = TWDModelResult.OK;
			}
			return new NGModelCommandRespond(this, result);
		}
	}
}
