using BaseModel;

namespace TWDModel
{
	public class BuyTradeShopRefreshCommand : ConsumeCurrencyCommand
	{
		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelManager tWDModelManager = manager as TWDModelManager;
			TWDModelResult tWDModelResult = TWDModelResult.Error;
			Cashier cashier = new Cashier(tWDModelManager);
			CashierItem cashierItem = new CashierItem(PurchaseType.TradeShopRefresh);
			cashierItem.SetCost(CurrencyType.Diamonds, tWDModelManager.GameEconomyData.ConfigData.TradeShopRefreshCost);
			cashier.AddItem(cashierItem);
			tWDModelResult = cashier.Pay();
			if (tWDModelResult == TWDModelResult.OK)
			{
				tWDModelResult = tWDModelManager.Player.BuyTradeShopRefresh();
			}
			return new NGModelCommandRespond(this, tWDModelResult);
		}
	}
}
