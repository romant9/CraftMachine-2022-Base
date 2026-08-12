using BaseModel;

namespace TWDModel
{
	public class BattlePassPremiumInfoPopupViewedCommand : ModelCommand
	{
		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelResult result = TWDModelResult.Error;
			if (manager is TWDModelManager tWDModelManager && tWDModelManager.Player?.BattlePass != null && tWDModelManager.Player.BattlePass.MarkPremiumInfoPopupViewed())
			{
				result = TWDModelResult.OK;
			}
			return new NGModelCommandRespond(this, result);
		}
	}
}
