using BaseModel;

namespace TWDModel
{
	public class OutpostNextMatchCommand : ConsumeCurrencyCommand
	{
		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelManager tWDModelManager = manager as TWDModelManager;
			TWDModelResult tWDModelResult = tWDModelManager.Player.OutpostModel.GetNextMatchCashier().Pay();
			if (tWDModelResult == TWDModelResult.OK)
			{
				tWDModelManager.Player.OutpostModel.MatchMakingPaid = true;
			}
			return new NGModelCommandRespond(this, tWDModelResult);
		}
	}
}
