using System.Collections.Generic;

namespace TWDModel
{
	public class BuyBundleLoadQueueMessage : SupportLoadQueueMessage
	{
		public string BundleId { get; set; }

		public double PaidPrice { get; set; }

		public new long SupportGivenTimestamp { get; set; }

		public string PurchaseSource { get; set; }

		public BuyBundleLoadQueueMessage()
		{
		}

		public BuyBundleLoadQueueMessage(string bundleid, double paidPrice, long supportGivenTimestamp, string purchaseSource)
		{
			BundleId = bundleid;
			PaidPrice = paidPrice;
			SupportGivenTimestamp = supportGivenTimestamp;
			PurchaseSource = purchaseSource;
		}

		public override bool Execute(TWDModelManager manager)
		{
			if (manager.Player != null && manager.Player.BundleManager != null && BundleId != null)
			{
				if (PurchaseSource == "tradefair")
				{
					TradefairBundleStoreDefinition bundleTradefairDefinition = manager.GameEconomyData.GetBundleTradefairDefinition(BundleId);
					manager.GameEconomyData.GetTradefairBundleContentDefinition(BundleId);
					if (bundleTradefairDefinition != null)
					{
						bool num = manager.Player.TradefairManager.BuyBundle(bundleTradefairDefinition, TradeFairPurchaseType.TradeFairXSolla);
						if (num)
						{
							if (manager.Player.WebShopBuyedTradeFairBundleIds == null)
							{
								manager.Player.WebShopBuyedTradeFairBundleIds = new List<string>();
							}
							manager.Player.WebShopBuyedTradeFairBundleIds.Add(BundleId);
						}
						return num;
					}
					manager.Debug.LogError("TradeFairBundle reward failed, couldn't find bundle with bundleId: '" + BundleId + "'. SupportEntityGUID: '" + base.SupportEntityGUID + "'");
				}
				else
				{
					BundleStoreDefinition bundleStoreDefinition = manager.GameEconomyData.GetBundleStoreDefinition(BundleId);
					if (bundleStoreDefinition != null)
					{
						bool num2 = manager.Player.BundleManager.BuyBundle(bundleStoreDefinition, givenBySupport: true, Metrics.BundleSource.Banana, SupportGivenTimestamp, base.SupportEntityGUID);
						if (num2)
						{
							if (manager.Player.WebShopBuyedBundleIds == null)
							{
								manager.Player.WebShopBuyedBundleIds = new List<string>();
							}
							manager.Player.WebShopBuyedBundleIds.Add(BundleId);
							if (PaidPrice > 0.0)
							{
								if (manager.Player.WebshopBuyedBundleSingularSyncDatas == null)
								{
									manager.Player.WebshopBuyedBundleSingularSyncDatas = new List<WebshopBuyedBundleSingularSyncData>();
								}
								manager.Player.WebshopBuyedBundleSingularSyncDatas.Add(new WebshopBuyedBundleSingularSyncData
								{
									BundleId = BundleId,
									PaidPrice = PaidPrice,
									BuyTime = SupportGivenTimestamp
								});
							}
						}
						return num2;
					}
					manager.Debug.LogError("Bundle reward failed, couldn't find bundle with bundleId: '" + BundleId + "'. SupportEntityGUID: '" + base.SupportEntityGUID + "'");
				}
			}
			else
			{
				manager.Debug.LogError("Bundle reward failed, missing bundle id or invalid player. SupportEntityGUID: '" + base.SupportEntityGUID + "'");
			}
			return true;
		}
	}
}
