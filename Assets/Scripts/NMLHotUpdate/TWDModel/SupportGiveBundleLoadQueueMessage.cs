namespace TWDModel
{
	public class SupportGiveBundleLoadQueueMessage : SupportLoadQueueMessage
	{
		public string BundleId { get; set; }

		public SupportGiveBundleLoadQueueMessage()
		{
		}

		public SupportGiveBundleLoadQueueMessage(string bundleId)
		{
			BundleId = bundleId;
		}

		public override bool Execute(TWDModelManager manager)
		{
			if (manager.Player != null && manager.Player.BundleManager != null && BundleId != null)
			{
				BundleStoreDefinition bundleStoreDefinition = manager.GameEconomyData.GetBundleStoreDefinition(BundleId);
				if (bundleStoreDefinition != null)
				{
					return manager.Player.BundleManager.BuyBundle(bundleStoreDefinition, givenBySupport: true, Metrics.BundleSource.Support, base.SupportGivenTimestamp, base.SupportEntityGUID);
				}
				manager.Debug.LogError("Bundle reward failed, couldn't find bundle with bundleId: '" + BundleId + "'. SupportEntityGUID: '" + base.SupportEntityGUID + "'");
			}
			else
			{
				manager.Debug.LogError("Bundle reward failed, missing bundle id or invalid player. SupportEntityGUID: '" + base.SupportEntityGUID + "'");
			}
			return true;
		}
	}
}
