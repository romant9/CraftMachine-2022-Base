using BaseModel;

namespace TWDModel
{
	public class StartIAPPurchaseCommand : ModelCommand
	{
		public Metrics.BundleSource bundleSource { get; set; }

		public StorePurchaseInfo currentPurchase { get; set; }

		public StartIAPPurchaseCommand()
		{
		}

		public StartIAPPurchaseCommand(StorePurchaseInfo currentPurchase, Metrics.BundleSource bundleSource)
		{
			this.bundleSource = bundleSource;
			this.currentPurchase = currentPurchase;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelManager tWDModelManager = (TWDModelManager)manager;
			if (tWDModelManager == null)
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			PlayerModel player = tWDModelManager.Player;
			if (player == null)
			{
				tWDModelManager.Debug.LogError("PlayerModel is null");
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			tWDModelManager.Debug.Log("IAP purchase started: productId=" + currentPurchase.BundleId + ",price=" + currentPurchase.Product.FormattedPrice + ", currencyCode=" + currentPurchase.Product.CurrencyCode);
			BundleContentDefinition bundleContentDefinition = player.gameEconomyData.GetBundleContentDefinition(currentPurchase.BundleId);
			InAppPurchaseProductApple inAppPurchaseProduct;
			if (bundleContentDefinition == null)
			{
				CustomBundleDefinition customBundleDefinition = player.gameEconomyData.GetCustomBundleDefinition(currentPurchase.BundleId);
				inAppPurchaseProduct = player.gameEconomyData.GetInAppPurchaseProduct(ToMainProductId(customBundleDefinition.IAPProduct));
			}
			else
			{
				inAppPurchaseProduct = player.gameEconomyData.GetInAppPurchaseProduct(ToMainProductId(bundleContentDefinition.IAPProduct));
			}
			if (inAppPurchaseProduct == null)
			{
				manager.Debug.LogError("FillProduct: unknown product " + currentPurchase.Product.ProductIdentifier);
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			currentPurchase.Product.PriceTier = inAppPurchaseProduct.PriceTier;
			currentPurchase.Product.PriceUSD = inAppPurchaseProduct.PriceUSD;
			player.CurrentIAP = currentPurchase;
			if (currentPurchase.Store == IAPStore.SteamStore)
			{
				ulong orderId = GenerateOrderId(tWDModelManager);
				if (manager.ServerService != null)
				{
					SteamStoreConfig steamStoreConfig = tWDModelManager.GameEconomyData.GetSteamStoreConfig(currentPurchase.Product.ProductIdentifier);
					if (steamStoreConfig == null)
					{
						manager.Debug.LogError("FillSteamStoreConfig: unknown ProductIdentifier " + currentPurchase.Product.ProductIdentifier);
						return new NGModelCommandRespond(this, TWDModelResult.Error);
					}
					string text = currentPurchase.Product.CurrencySymbol;
					string text2 = (string)steamStoreConfig.GetFieldValue("Des_" + text);
					if (text2 == null)
					{
						text2 = (string)steamStoreConfig.GetFieldValue("Des_en");
						text = "en";
					}
					string text3 = manager.ServerService.GetSteamUserInfo(player.PcPlatform.PcAccountId).Result;
					int num = (int)steamStoreConfig.GetFieldValue(text3);
					if (num == 0)
					{
						num = (int)steamStoreConfig.GetFieldValue("USD");
						text3 = "USD";
					}
					manager.ServerService.CreateSteamOrder(orderId, player.PcPlatform.PcAccountId, 1u, text, text3, 1u, 1, num, text2);
				}
			}
			player.BundleManager.SetInitiatedBundlePurchase(player.CurrentIAP.BundleId);
			PurchaseAnalyticsHelper.SendStartEvent(tWDModelManager, bundleSource);
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}

		private static string ToMainProductId(string platformProductId)
		{
			if (platformProductId.EndsWith("_LV"))
			{
				return platformProductId.Substring(0, platformProductId.Length - 3);
			}
			return platformProductId;
		}

		public static ulong GenerateOrderId(TWDModelManager twdModelManager)
		{
			long lifeTime = twdModelManager.Player.LifeTime;
			ulong num = (ulong)twdModelManager.Player.PlayerRandom.GetRandomInRange(0, int.MaxValue);
			return (ulong)(lifeTime << 32) | num;
		}
	}
}
