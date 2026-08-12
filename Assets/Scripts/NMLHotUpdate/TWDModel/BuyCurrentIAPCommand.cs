using BaseModel;

namespace TWDModel
{
	public class BuyCurrentIAPCommand : ModelCommand
	{
		public int ShopTabIndex { get; set; }

		public int ShopPosition { get; set; }

		public Metrics.BundleSource BundleSource { get; set; }

		public BuyCurrentIAPCommand()
		{
		}

		public BuyCurrentIAPCommand(Metrics.BundleSource bundleSource, int shopTabIndex, int shopPosition)
		{
			ShopTabIndex = shopTabIndex;
			ShopPosition = shopPosition;
			BundleSource = bundleSource;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			if (manager == null)
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			if (!(manager is TWDModelManager { Player: var player } tWDModelManager))
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			StorePurchaseInfo currentIAP = player.CurrentIAP;
			if (currentIAP == null || currentIAP.Transaction == null)
			{
				tWDModelManager.Debug.LogError("Current purchase or current purchase transaction are null ");
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			if (tWDModelManager.ServerService != null && !tWDModelManager.ServerService.ValidateReceiptV2(currentIAP.Transaction.TransactionIdentifier))
			{
				PurchaseAnalyticsHelper.SendValidationEvent(tWDModelManager, PurchaseValidationResult.ServerValidationFailed, currentIAP.Transaction.TrackingId, BundleSource);
				tWDModelManager.Debug.LogWarning("Buying bundle failed: ServerService.ValidateReceipt returned false for TransactionId " + currentIAP.Transaction.TransactionIdentifier);
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			currentIAP.BundleId = tWDModelManager.GetBattlePassRealBundleId(currentIAP.BundleId);
			BundleStoreDefinition bundleStoreDefinition = player.gameEconomyData.GetBundleStoreDefinition(currentIAP.BundleId);
			if (bundleStoreDefinition == null)
			{
				if (!player.CustomizedBundleManager.CustomizedBundleClaimReward(currentIAP.BundleId))
				{
					if (tWDModelManager.Mode == ModelManagerMode.Server)
					{
						PurchaseAnalyticsHelper.SendValidationEvent(tWDModelManager, PurchaseValidationResult.ServerCommandFailed, currentIAP.Transaction.TrackingId, BundleSource);
					}
					tWDModelManager.Debug.LogError("Buying bundle failed: BundleManager.BuyBundle(" + bundleStoreDefinition.BundleIdentifier + ")");
					return new NGModelCommandRespond(this, TWDModelResult.Error);
				}
				player.RegisterCustomBundleIAPPurchase(currentIAP.Transaction, player.gameEconomyData.GetCustomBundleDefinition(currentIAP.BundleId));
				player.RemovePendingPurchase(currentIAP.Transaction.TransactionIdentifier);
			}
			else
			{
				BundleContentDefinition bundleContentDefinition = player.gameEconomyData.GetBundleContentDefinition(currentIAP.BundleId);
				string metricsResourceChangeObtainReason = "";
				if (bundleContentDefinition != null && string.IsNullOrEmpty(bundleContentDefinition.IAPProduct))
				{
					metricsResourceChangeObtainReason = "BundleGift";
				}
				if (!player.BundleManager.BuyBundle(bundleStoreDefinition, givenBySupport: false, BundleSource, 0L, null, metricsResourceChangeObtainReason))
				{
					if (tWDModelManager.Mode == ModelManagerMode.Server)
					{
						PurchaseAnalyticsHelper.SendValidationEvent(tWDModelManager, PurchaseValidationResult.ServerCommandFailed, currentIAP.Transaction.TrackingId, BundleSource);
					}
					tWDModelManager.Debug.LogError("Buying bundle failed: BundleManager.BuyBundle(" + bundleStoreDefinition.BundleIdentifier + ")");
					return new NGModelCommandRespond(this, TWDModelResult.Error);
				}
				player.RegisterIAPPurchase(currentIAP.Transaction, player.gameEconomyData.GetBundleContentDefinition(currentIAP.BundleId));
				player.RemovePendingPurchase(currentIAP.Transaction.TransactionIdentifier);
			}
			tWDModelManager.Save(SaveType.Player);
			if (tWDModelManager.Mode == ModelManagerMode.Server)
			{
				PurchaseAnalyticsHelper.SendValidationEvent(tWDModelManager, PurchaseValidationResult.OK, currentIAP.Transaction.TrackingId, BundleSource, ShopTabIndex, ShopPosition);
			}
			player.BundleManager.SetInitiatedBundlePurchase(null);
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
