using BaseModel;

namespace TWDModel
{
	public class BuyBundleCommand : ConsumeCurrencyCommand
	{
		public string BundleId { get; private set; }

		public BuyBundleCommand()
		{
		}

		public BuyBundleCommand(string bundleId)
		{
			BundleId = bundleId;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelManager tWDModelManager = manager as TWDModelManager;
			TWDModelResult tWDModelResult = TWDModelResult.Error;
			PlayerModel player = tWDModelManager.Player;
			BundleStoreDefinition bundleStoreDefinition = player.gameEconomyData.GetBundleStoreDefinition(BundleId);
			BundleContentDefinition bundleContentDefinition = player.gameEconomyData.GetBundleContentDefinition(BundleId);
			if (bundleStoreDefinition == null || bundleContentDefinition == null)
			{
				manager.Debug.LogError("BuyBundleCommand Failed. item definition not found with ID: " + BundleId);
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			if (bundleContentDefinition.IsNormalBundle() && !player.BundleManager.CanBuyBundle(bundleStoreDefinition))
			{
				manager.Debug.LogError("BuyBundleCommand Failed. NomarlBundle item definition can not buy with ID: " + BundleId);
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			if ((bundleContentDefinition.BundleType == BundleType.NormalBP && player.BattlePass.PremiumActive) || (bundleContentDefinition.BundleType == BundleType.BeginerBP && player.BattlePass.PremiumActive))
			{
				manager.Debug.LogError("BuyBundleCommand Failed. BattlePass item definition Had buy with ID: " + BundleId);
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			if (bundleContentDefinition.BundleType == BundleType.SevenDayPremium)
			{
				if (player.SevenDayLoginManager.CurrentPeriodModel == null)
				{
					manager.Debug.LogError("BuyTradefairBundleCommand Failed. SevenDayLogin is null");
					return new NGModelCommandRespond(this, TWDModelResult.Error);
				}
				if (player.SevenDayLoginManager.CurrentPeriodModel.IsUnlockPremium)
				{
					manager.Debug.LogError("BuyTradefairBundleCommand Failed. SevenDayLogin had unlock premium");
					return new NGModelCommandRespond(this, TWDModelResult.Error);
				}
			}
			Cashier cashier = new Cashier(tWDModelManager);
			CashierItem cashierItem = new CashierItem(PurchaseType.BundleStore);
			cashierItem.SetCost(CurrencyType.Fairmoney, bundleContentDefinition.TradeFairPriceNew);
			cashier.AddItem(cashierItem);
			tWDModelResult = cashier.Pay(bundleContentDefinition);
			if (tWDModelResult == TWDModelResult.OK)
			{
				player.BundleManager.BuyBundle(bundleStoreDefinition, givenBySupport: false, Metrics.BundleSource.TradeFairPay, 0L);
			}
			return new NGModelCommandRespond(this, tWDModelResult);
		}
	}
}
