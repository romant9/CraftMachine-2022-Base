using BaseModel;

namespace TWDModel
{
	public class ClaimReturnEndlessDealFreePackCommand : ModelCommand
	{
		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelManager tWDModelManager = manager as TWDModelManager;
			if (tWDModelManager?.Player?.ReturnActivityManager?.ReturnEndlessDeal == null)
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			bool flag = tWDModelManager.Player.ReturnActivityManager.ReturnEndlessDeal.ClaimFreePack();
			return new NGModelCommandRespond(this, (!flag) ? TWDModelResult.Error : TWDModelResult.OK);
		}
	}
}
