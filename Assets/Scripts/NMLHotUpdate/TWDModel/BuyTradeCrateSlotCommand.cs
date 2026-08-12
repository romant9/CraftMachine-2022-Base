using BaseModel;

namespace TWDModel
{
	public class BuyTradeCrateSlotCommand : ConsumeCurrencyCommand
	{
		public int TradeSlotId { get; private set; }

		public BuyTradeCrateSlotCommand()
		{
		}

		public BuyTradeCrateSlotCommand(int tradeSlotId)
		{
			TradeSlotId = tradeSlotId;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelManager tWDModelManager = manager as TWDModelManager;
			TWDModelResult tWDModelResult = TWDModelResult.Error;
			TradeSlotInfo currentTradeSlotDefinitionById = tWDModelManager.Player.GetCurrentTradeSlotDefinitionById(TradeSlotId);
			if (currentTradeSlotDefinitionById != null)
			{
				Cashier cashier = new Cashier(tWDModelManager);
				CashierItem cashierItem = new CashierItem(PurchaseType.TradeCrateSlot);
				cashierItem.SetCost(currentTradeSlotDefinitionById.SlotDefinition.CurrencyUnlock, currentTradeSlotDefinitionById.SlotDefinition.CurrencyUnlockAmount);
				cashier.AddItem(cashierItem);
				cashier.UseDiamondsAmount = base.UseDiamondsAmount;
				tWDModelResult = cashier.Pay(currentTradeSlotDefinitionById);
				if (tWDModelResult == TWDModelResult.OK)
				{
					tWDModelManager.Player.UnlockNextTradeCrateSlot();
				}
			}
			return new NGModelCommandRespond(this, tWDModelResult);
		}
	}
}
