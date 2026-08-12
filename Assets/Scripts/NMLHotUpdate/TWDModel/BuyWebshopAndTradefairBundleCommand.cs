using System.Collections.Generic;
using BaseModel;

namespace TWDModel
{
	public class BuyWebshopAndTradefairBundleCommand : ConsumeCurrencyCommand
	{
		public BuyBundleResultInfoList buyBundleResultInfo;

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelManager tWDModelManager = manager as TWDModelManager;
			TWDModelResult result = TWDModelResult.BuyWebshopAndTradefairBundleCommandSuc;
			PlayerModel player = tWDModelManager.Player;
			int count = buyBundleResultInfo.BuyBundleResultList.Count;
			BuyBundleResultInfoList buyBundleResultInfoList = null;
			BuyBundleResultInfoList buyBundleResultInfoList2 = new BuyBundleResultInfoList();
			buyBundleResultInfoList2.BuyBundleResultList = new List<BuyBundleResultInfo>();
			BuyBundleResultInfoList buyBundleResultInfoList3 = new BuyBundleResultInfoList();
			buyBundleResultInfoList3.BuyBundleResultList = new List<BuyBundleResultInfo>();
			if (manager.ServerService != null)
			{
				buyBundleResultInfoList = manager.ServerService.VerifiedWebshopBuyBundleResultInfoList(buyBundleResultInfo);
			}
			for (int i = 0; i < count; i++)
			{
				BuyBundleResultInfo bundleinfo = buyBundleResultInfo.BuyBundleResultList[i];
				if (manager.ServerService != null)
				{
					if (buyBundleResultInfoList == null || buyBundleResultInfoList.BuyBundleResultList == null || buyBundleResultInfoList.BuyBundleResultList.Count == 0)
					{
						manager.Debug.LogError("Empty verifiedOkBuyBundleResultInfoList");
						continue;
					}
					if (buyBundleResultInfoList.BuyBundleResultList.Find((BuyBundleResultInfo x) => x.TransactionId == bundleinfo.TransactionId) == null)
					{
						manager.Debug.LogError("Didn't match verifiedOkBuyBundleResultInfoList");
						continue;
					}
				}
				string pCPlatformType = player.gameEconomyData.ConfigData.GetPCPlatformType(0);
				if (bundleinfo.PurchaseSource == "tradefair" || bundleinfo.PurchaseSource == pCPlatformType)
				{
					TradefairBundleStoreDefinition bundleTradefairDefinition = player.gameEconomyData.GetBundleTradefairDefinition(bundleinfo.BundleId);
					TradefairBundleContentDefinition tradefairBundleContentDefinition = player.gameEconomyData.GetTradefairBundleContentDefinition(bundleinfo.BundleId);
					if (bundleTradefairDefinition == null || tradefairBundleContentDefinition == null)
					{
						manager.Debug.LogError("Didn't find tradefair bundle, HashId: " + bundleinfo.HashId + ", TransactionId: " + bundleinfo.TransactionId);
						buyBundleResultInfoList2.BuyBundleResultList.Add(bundleinfo);
						continue;
					}
					TradeFairPurchaseType payType = TradeFairPurchaseType.None;
					if (bundleinfo.PurchaseSource == "tradefair")
					{
						payType = TradeFairPurchaseType.TradeFairXSolla;
					}
					else if (bundleinfo.PurchaseSource == pCPlatformType)
					{
						payType = TradeFairPurchaseType.TradeFairAppcharge;
					}
					player.TradefairManager.BuyBundle(bundleTradefairDefinition, payType);
					tWDModelManager.TdMetrics.SetEventType("currency_redeem").AddProperty("resource_id", CurrencyType.Fairmoney.ToString()).AddProperty("currency_used_num", tradefairBundleContentDefinition.IAPProduct)
						.AddProperty("bundle_id", tradefairBundleContentDefinition.Identifier)
						.AddProperty("product_detail", tradefairBundleContentDefinition.RewardEntries.RewardResources)
						.Send();
				}
				else
				{
					string text = "";
					Metrics.BundleSource bundleSource = Metrics.BundleSource.Banana;
					if (bundleinfo.PurchaseSource == "IAPBundle")
					{
						manager.Debug.LogInfo("BundleId is " + bundleinfo.BundleId);
						bundleSource = Metrics.BundleSource.IAPBundleBanana;
						text = bundleinfo.BundleId;
					}
					else
					{
						text = (bundleinfo.IsFreeDailyBundle ? bundleinfo.RandomResultBundleId : bundleinfo.BundleId);
					}
					if (string.IsNullOrEmpty(text))
					{
						manager.Debug.LogError("Didn't find bundle, realBundleId is empty, HashId: " + bundleinfo.HashId + ", TransactionId: " + bundleinfo.TransactionId);
						buyBundleResultInfoList2.BuyBundleResultList.Add(bundleinfo);
						continue;
					}
					BundleStoreDefinition bundleStoreDefinition = player.gameEconomyData.GetBundleStoreDefinition(text);
					BundleContentDefinition bundleContentDefinition = player.gameEconomyData.GetBundleContentDefinition(text);
					if (bundleStoreDefinition == null || bundleContentDefinition == null)
					{
						manager.Debug.LogError("Didn't find bundle, HashId: " + bundleinfo.HashId + ", TransactionId: " + bundleinfo.TransactionId);
						buyBundleResultInfoList2.BuyBundleResultList.Add(bundleinfo);
						continue;
					}
					string metricsResourceChangeObtainReason = "";
					if (string.IsNullOrEmpty(bundleContentDefinition.IAPProduct) && bundleContentDefinition.BundleType == BundleType.WebshopGift)
					{
						metricsResourceChangeObtainReason = "WebshopGift";
					}
					player.BundleManager.BuyBundle(bundleStoreDefinition, givenBySupport: false, bundleSource, 0L, null, metricsResourceChangeObtainReason);
				}
				manager.Debug.Log("Can delivery success, HashId: " + bundleinfo.HashId + ", TransactionId: " + bundleinfo.TransactionId);
				buyBundleResultInfoList3.BuyBundleResultList.Add(bundleinfo);
			}
			if (manager.ServerService != null)
			{
				if (buyBundleResultInfoList3.BuyBundleResultList.Count > 0)
				{
					manager.ServerService.ChangeWebshopPaySucModelsStateDeliverySuccess(buyBundleResultInfoList3);
				}
				if (buyBundleResultInfoList2.BuyBundleResultList.Count > 0)
				{
					manager.ServerService.ChangeWebshopPaySucModelsStateNotFound(buyBundleResultInfoList2);
				}
			}
			return new NGModelCommandRespond(this, result);
		}
	}
}
