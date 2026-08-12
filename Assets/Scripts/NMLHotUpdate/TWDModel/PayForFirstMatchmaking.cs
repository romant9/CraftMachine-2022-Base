using BaseModel;

namespace TWDModel
{
	public class PayForFirstMatchmaking : ConsumeCurrencyCommand
	{
		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelManager tWDModelManager = manager as TWDModelManager;
			Cashier nextMatchCashier = tWDModelManager.Player.OutpostModel.GetNextMatchCashier();
			nextMatchCashier.UseDiamondsAmount = base.UseDiamondsAmount;
			TWDModelResult tWDModelResult = nextMatchCashier.Pay();
			if (tWDModelResult == TWDModelResult.OK)
			{
				tWDModelManager.Player.OutpostModel.MatchMakingPaid = true;
			}
			return new NGModelCommandRespond(this, tWDModelResult);
		}
	}
}
