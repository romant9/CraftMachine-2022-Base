using BaseModel;

namespace TWDModel
{
	public class RefreshBlackMarketSlotCommand : ConsumeCurrencyCommand
	{
		public string ActorId { get; set; }

		public RefreshBlackMarketSlotCommand()
		{
		}

		public RefreshBlackMarketSlotCommand(string actorId)
		{
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelManager tWDModelManager = (TWDModelManager)manager;
			Cashier cashier = Cashier.CreateOneItemCashier(tWDModelManager, PurchaseType.RefreshBlackMarket, CurrencyType.Diamonds, tWDModelManager.GameEconomyData.ConfigData.BlackMarketRefreshCost);
			if (!cashier.CanAfford())
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			if (tWDModelManager.Player.BlackMarket.RefreshHero(ActorId))
			{
				TWDModelResult result = cashier.Pay();
				return new NGModelCommandRespond(this, result);
			}
			return new NGModelCommandRespond(this, TWDModelResult.Error);
		}
	}
}
