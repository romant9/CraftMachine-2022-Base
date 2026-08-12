using BaseModel;

namespace TWDModel
{
	public class BuyTradefairBundleCommand : ConsumeCurrencyCommand
	{
		public string BundleId { get; private set; }

		public BuyTradefairBundleCommand()
		{
		}

		public BuyTradefairBundleCommand(string bundleId)
		{
			BundleId = bundleId;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelManager tWDModelManager = manager as TWDModelManager;
			TWDModelResult tWDModelResult = TWDModelResult.Error;
			PlayerModel player = tWDModelManager.Player;
			TradefairBundleStoreDefinition bundleTradefairDefinition = player.gameEconomyData.GetBundleTradefairDefinition(BundleId);
			TradefairBundleContentDefinition tradefairBundleContentDefinition = player.gameEconomyData.GetTradefairBundleContentDefinition(BundleId);
			if (bundleTradefairDefinition == null || tradefairBundleContentDefinition == null)
			{
				manager.Debug.LogError("BuyTradefairBundleCommand Failed. item definition not found with ID: " + BundleId);
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			if (tradefairBundleContentDefinition.IsNormalBundle() && !player.TradefairManager.CanBuyBundle(bundleTradefairDefinition))
			{
				manager.Debug.LogError("BuyTradefairBundleCommand Failed. NomarlBundle item definition can not buy with ID: " + BundleId);
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			if ((tradefairBundleContentDefinition.BundleType == BundleType.NormalBP && player.BattlePass.PremiumActive) || (tradefairBundleContentDefinition.BundleType == BundleType.BeginerBP && player.BattlePass.PremiumActive))
			{
				manager.Debug.LogError("BuyTradefairBundleCommand Failed. BattlePass item definition Had buy with ID: " + BundleId);
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			if (tradefairBundleContentDefinition.BundleType == BundleType.SevenDayPremium)
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
			CashierItem cashierItem = new CashierItem(PurchaseType.TradeCrate);
			cashierItem.SetCost(CurrencyType.Fairmoney, tradefairBundleContentDefinition.IAPProduct);
			cashier.AddItem(cashierItem);
			tWDModelResult = cashier.Pay(tradefairBundleContentDefinition);
			if (tWDModelResult == TWDModelResult.OK)
			{
				string metricsResourceChangeObtainReason = "";
				if (tradefairBundleContentDefinition.IAPProduct == 0)
				{
					metricsResourceChangeObtainReason = "TradefairGift";
				}
				player.TradefairManager.BuyBundle(bundleTradefairDefinition, TradeFairPurchaseType.None, metricsResourceChangeObtainReason);
			}
			tWDModelManager.TdMetrics.SetEventType("currency_redeem").AddProperty("resource_id", CurrencyType.Fairmoney.ToString()).AddProperty("currency_used_num", tradefairBundleContentDefinition.IAPProduct)
				.AddProperty("bundle_id", tradefairBundleContentDefinition.Identifier)
				.AddProperty("product_detail", tradefairBundleContentDefinition.RewardEntries.RewardResources)
				.Send();
			return new NGModelCommandRespond(this, tWDModelResult);
		}
	}
}
