using System.Linq;
using BaseModel;

namespace TWDModel
{
	public class BuyHillTopItemCommand : ConsumeCurrencyCommand
	{
		public int ItemId;

		public BuyHillTopItemCommand()
		{
		}

		public BuyHillTopItemCommand(int itemId)
		{
			ItemId = itemId;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelManager tWDModelManager = manager as TWDModelManager;
			HillTopStoreDefinition hillTopStoreDefinition = tWDModelManager.Player.gameEconomyData.HillTopStoreDefinitions.FirstOrDefault((HillTopStoreDefinition x) => x.UniqueId == ItemId);
			if (hillTopStoreDefinition == null)
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			Cashier obj = new Cashier(tWDModelManager)
			{
				UsedReason = "HillTopStore"
			};
			CurrencyType currencyType = CurrencyType.HillTopCoin;
			CashierItem cashierItem = new CashierItem(PurchaseType.HillTopStore);
			cashierItem.SetCost(currencyType, hillTopStoreDefinition.Score);
			obj.AddItem(cashierItem);
			TWDModelResult tWDModelResult = obj.Pay(hillTopStoreDefinition);
			if (tWDModelResult == TWDModelResult.OK)
			{
				tWDModelResult = tWDModelManager.Player.HillTopStore.GiveReward(hillTopStoreDefinition);
				tWDModelManager.Player.HillTopStore.AddToPurchaseHistory(hillTopStoreDefinition);
				tWDModelManager.TdMetrics.SetEventType("currency_redeem").AddProperty("resource_id", CurrencyType.HillTopCoin.ToString()).AddProperty("currency_used_num", hillTopStoreDefinition.Score)
					.AddProperty("product_detail", hillTopStoreDefinition.RewardEntries.RewardResources)
					.Send();
			}
			return new NGModelCommandRespond(this, tWDModelResult);
		}
	}
}
