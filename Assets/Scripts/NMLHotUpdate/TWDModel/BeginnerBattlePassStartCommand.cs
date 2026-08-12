using BaseModel;

namespace TWDModel
{
	public class BeginnerBattlePassStartCommand : ModelCommand
	{
		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelResult result = TWDModelResult.Error;
			if (manager is TWDModelManager tWDModelManager && tWDModelManager.Player.BeginnerBattlePassInfo.StartSeason(tWDModelManager.Player.UtcTimeStamp))
			{
				tWDModelManager.Player.BattlePass.RefreshActiveSeason();
				result = TWDModelResult.OK;
			}
			return new NGModelCommandRespond(this, result);
		}
	}
}
