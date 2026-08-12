using BaseModel;

namespace TWDModel
{
	public class UnlockFreeBundleCommand : ModelCommand
	{
		public BundleStoreDefinition BundleDefinition { get; set; }

		public Metrics.BundleSource BundleSource { get; set; }

		public UnlockFreeBundleCommand()
		{
		}

		public UnlockFreeBundleCommand(BundleStoreDefinition bundleDefinition, PlayerModel player, Metrics.BundleSource bundleSource)
			: base(player)
		{
			BundleDefinition = bundleDefinition;
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
			BundleStoreDefinition bundleStoreDefinition = player.gameEconomyData.GetBundleStoreDefinition(BundleDefinition.BundleIdentifier);
			BundleContentDefinition bundleContentDefinition = player.gameEconomyData.GetBundleContentDefinition(BundleDefinition.BundleIdentifier);
			if (bundleContentDefinition == null)
			{
				tWDModelManager.Debug.LogWarning("Unknown bundle " + bundleStoreDefinition.BundleIdentifier);
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			if (!string.IsNullOrEmpty(bundleContentDefinition.IAPProduct))
			{
				tWDModelManager.Debug.LogWarning("Bundle must be purchased " + bundleStoreDefinition.BundleIdentifier);
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			if (!player.BundleManager.CanBuyBundle(bundleStoreDefinition))
			{
				tWDModelManager.Debug.LogWarning("UnlockFreeBundleCommand failed - bundle not available for purchase: " + bundleStoreDefinition.BundleIdentifier);
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			if (tWDModelManager.Debug != null)
			{
				string text = ((bundleStoreDefinition != null) ? bundleStoreDefinition.BundleIdentifier : "null");
				tWDModelManager.Debug.LogWarning("UnlockFreeBundleCommand success, bundleId: " + text);
			}
			if (!player.BundleManager.BuyBundle(bundleStoreDefinition, givenBySupport: false, BundleSource, 0L))
			{
				if (tWDModelManager.Mode == ModelManagerMode.Server)
				{
					PurchaseAnalyticsHelper.SendValidationEvent(tWDModelManager, PurchaseValidationResult.ServerCommandFailed, null, BundleSource);
				}
				tWDModelManager.Debug.LogError("Buying bundle failed: BundleManager.BuyBundle(" + bundleStoreDefinition.BundleIdentifier + ")");
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			tWDModelManager.Save(SaveType.Player);
			player.BundleManager.SetInitiatedBundlePurchase(null);
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
