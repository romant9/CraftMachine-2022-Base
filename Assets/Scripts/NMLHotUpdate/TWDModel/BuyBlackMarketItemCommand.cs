using System.Linq;
using BaseModel;

namespace TWDModel
{
	public class BuyBlackMarketItemCommand : ConsumeCurrencyCommand
	{
		public int ItemId;

		public BuyBlackMarketItemCommand()
		{
		}

		public BuyBlackMarketItemCommand(int itemId)
		{
			ItemId = itemId;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelManager tWDModelManager = manager as TWDModelManager;
			BlackMarketDefinition blackMarketDefinition = tWDModelManager.Player.gameEconomyData.BlackMarketDefinitions.FirstOrDefault((BlackMarketDefinition x) => x.UniqueId == ItemId);
			if (blackMarketDefinition == null)
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			Cashier obj = new Cashier(tWDModelManager)
			{
				UsedReason = "BlackMarket"
			};
			CurrencyType currencyType = blackMarketDefinition.GetCurrencyType();
			CashierItem cashierItem = new CashierItem(PurchaseType.BlackMarket);
			cashierItem.SetCost(currencyType, blackMarketDefinition.GetPrice(tWDModelManager));
			obj.AddItem(cashierItem);
			int num = tWDModelManager.GameEconomyData.CurrencyToDiamonds(blackMarketDefinition.GetCurrencyType(), blackMarketDefinition.GetPrice(tWDModelManager) - tWDModelManager.Player.GetCurrencyAmount(blackMarketDefinition.GetCurrencyType()));
			obj.UseDiamondsAmount = (obj.CanAfford() ? base.UseDiamondsAmount : num);
			TWDModelResult tWDModelResult = obj.Pay(blackMarketDefinition);
			if (tWDModelResult == TWDModelResult.OK)
			{
				tWDModelResult = tWDModelManager.Player.BlackMarket.GiveReward(blackMarketDefinition);
				tWDModelManager.Player.BlackMarket.AddToPurchaseHistory(blackMarketDefinition);
			}
			return new NGModelCommandRespond(this, tWDModelResult);
		}
	}
}
