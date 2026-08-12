using BaseModel;

namespace TWDModel
{
	public class BuyCustomizedBundleCommand : ConsumeCurrencyCommand
	{
		public string BundleId { get; private set; }

		public CustomizedBundlePayType PayType { get; set; }

		public BuyCustomizedBundleCommand()
		{
		}

		public BuyCustomizedBundleCommand(string bundleId)
		{
			BundleId = bundleId;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelManager tWDModelManager = manager as TWDModelManager;
			TWDModelResult tWDModelResult = TWDModelResult.Error;
			PlayerModel player = tWDModelManager.Player;
			CustomBundleDefinition customBundleDefinition = player.gameEconomyData.GetCustomBundleDefinition(BundleId);
			if (customBundleDefinition == null)
			{
				manager.Debug.LogError("BuyTradefairBundleCommand Failed. item definition not found with ID: " + BundleId);
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			if (PayType == CustomizedBundlePayType.None)
			{
				manager.Debug.LogError("BuyTradefairBundleCommand Failed. item definition PayType == CustomizedBundlePayType.None bundleid: " + BundleId);
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			Cashier cashier = new Cashier(tWDModelManager);
			CashierItem cashierItem = new CashierItem(PurchaseType.TradeCrate);
			if (PayType == CustomizedBundlePayType.TradeFairPay)
			{
				cashierItem.SetCost(CurrencyType.Fairmoney, customBundleDefinition.TradefairPrice);
			}
			else if (PayType == CustomizedBundlePayType.BluePrintTokenPay)
			{
				cashierItem.SetCost(CurrencyType.BulePrintToken, customBundleDefinition.FragmentPrice);
			}
			else if (PayType == CustomizedBundlePayType.DiamondsPay)
			{
				cashierItem.SetCost(CurrencyType.Diamonds, customBundleDefinition.GoldPrice);
			}
			cashier.AddItem(cashierItem);
			tWDModelResult = cashier.Pay(customBundleDefinition);
			if (tWDModelResult == TWDModelResult.OK)
			{
				player.CustomizedBundleManager.CustomizedBundleClaimReward(BundleId);
			}
			if (PayType == CustomizedBundlePayType.TradeFairPay)
			{
				tWDModelManager.TdMetrics.SetEventType("currency_redeem").AddProperty("resource_id", CurrencyType.Fairmoney.ToString()).AddProperty("currency_used_num", customBundleDefinition.TradefairPrice)
					.AddProperty("bundle_id", customBundleDefinition.Identifier)
					.AddProperty("payment_type", "TradefairPrice")
					.AddProperty("product_detail", customBundleDefinition.RewardEntries.RewardResources)
					.Send();
			}
			else if (PayType == CustomizedBundlePayType.BluePrintTokenPay)
			{
				tWDModelManager.TdMetrics.SetEventType("currency_redeem").AddProperty("resource_id", CurrencyType.Fairmoney.ToString()).AddProperty("currency_used_num", customBundleDefinition.FragmentPrice)
					.AddProperty("bundle_id", customBundleDefinition.Identifier)
					.AddProperty("payment_type", "FragmentPrice")
					.AddProperty("product_detail", customBundleDefinition.RewardEntries.RewardResources)
					.Send();
			}
			else if (PayType == CustomizedBundlePayType.DiamondsPay)
			{
				tWDModelManager.TdMetrics.SetEventType("currency_redeem").AddProperty("resource_id", CurrencyType.Fairmoney.ToString()).AddProperty("currency_used_num", customBundleDefinition.GoldPrice)
					.AddProperty("bundle_id", customBundleDefinition.Identifier)
					.AddProperty("payment_type", "FragmentPrice")
					.AddProperty("product_detail", customBundleDefinition.RewardEntries.RewardResources)
					.Send();
			}
			else
			{
				InAppPurchaseProductApple inAppPurchaseProduct = player.gameEconomyData.GetInAppPurchaseProduct(customBundleDefinition.IAPProduct);
				tWDModelManager.TdMetrics.SetEventType("currency_redeem").AddProperty("resource_id", CurrencyType.Fairmoney.ToString()).AddProperty("currency_used_num", inAppPurchaseProduct.PriceTier)
					.AddProperty("bundle_id", customBundleDefinition.Identifier)
					.AddProperty("payment_type", "USD")
					.AddProperty("product_detail", customBundleDefinition.RewardEntries.RewardResources)
					.Send();
			}
			return new NGModelCommandRespond(this, tWDModelResult);
		}
	}
}
