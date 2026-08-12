using System;
using BaseModel;
using Newtonsoft.Json;

namespace TWDModel
{
	public class PurchaseAnalyticsHelper
	{
		public static void SendStartEvent(TWDModelManager manager, Metrics.BundleSource bundleSource)
		{
			string lastInitiatedBundleId = manager.Player.BundleManager.LastInitiatedBundleId;
			BundleStoreDefinition bundleStoreDefinition = manager.GameEconomyData.GetBundleStoreDefinition(lastInitiatedBundleId);
			CustomBundleDefinition customBundleDefinition = null;
			if (bundleStoreDefinition == null)
			{
				customBundleDefinition = manager.GameEconomyData.GetCustomBundleDefinition(lastInitiatedBundleId);
			}
			if (bundleStoreDefinition != null)
			{
				manager.Metrics.Reset().AddBuy().AddBundle(bundleStoreDefinition, bundleSource)
					.AddIAPInitiation()
					.Send();
			}
			else if (customBundleDefinition != null)
			{
				manager.Metrics.Reset().AddBuy().AddCustomBundle(customBundleDefinition, bundleSource)
					.AddIAPInitiation()
					.Send();
			}
			else
			{
				manager.Debug.LogWarning("Bundle not found - bundle identifier '" + (lastInitiatedBundleId ?? "NULL") + "'");
			}
		}

		public static void SendConfirmationEvent(TWDModelManager manager, PurchaseConfirmationResult result, Metrics.BundleSource bundleSource)
		{
			string lastInitiatedBundleId = manager.Player.BundleManager.LastInitiatedBundleId;
			BundleStoreDefinition bundleStoreDefinition = manager.GameEconomyData.GetBundleStoreDefinition(lastInitiatedBundleId);
			CustomBundleDefinition customBundleDefinition = null;
			if (bundleStoreDefinition == null)
			{
				customBundleDefinition = manager.GameEconomyData.GetCustomBundleDefinition(lastInitiatedBundleId);
			}
			if (bundleStoreDefinition != null)
			{
				manager.Metrics.Reset().AddBuy().AddBundle(bundleStoreDefinition, bundleSource)
					.AddIAPConfirmationResult(result)
					.Send();
			}
			else if (customBundleDefinition != null)
			{
				manager.Metrics.Reset().AddBuy().AddCustomBundle(customBundleDefinition, bundleSource)
					.AddIAPConfirmationResult(result)
					.Send();
			}
			else
			{
				manager.Debug.LogWarning("Bundle not found - bundle identifier '" + (lastInitiatedBundleId ?? "NULL") + "'");
			}
		}

		public static void SendValidationEvent(TWDModelManager manager, PurchaseValidationResult result, string trackingId, Metrics.BundleSource bundleSource, int shopTabIndex = -1, int shopPosition = -1)
		{
			manager.Debug.LogWarning("SendValidationEvent: start");
			if (manager == null || manager.Player == null)
			{
				return;
			}
			string lastInitiatedBundleId = manager.Player.BundleManager.LastInitiatedBundleId;
			string text = ((lastInitiatedBundleId == null) ? "null" : lastInitiatedBundleId);
			manager.Debug.LogWarning("SendValidationEvent: bundleId " + text);
			if (!string.IsNullOrEmpty(lastInitiatedBundleId))
			{
				BundleStoreDefinition bundleStoreDefinition = manager.GameEconomyData.GetBundleStoreDefinition(lastInitiatedBundleId);
				CustomBundleDefinition customBundleDefinition = null;
				if (bundleStoreDefinition == null)
				{
					customBundleDefinition = manager.GameEconomyData.GetCustomBundleDefinition(lastInitiatedBundleId);
				}
				int councilLevel = manager.Player.CouncilLevel;
				manager.Debug.LogWarning($"SendValidationEvent: bundleStore1 {bundleStoreDefinition == null}");
				if (bundleStoreDefinition != null)
				{
					manager.Metrics.Reset().AddBuy().AddBundle(bundleStoreDefinition, bundleSource)
						.AddIAPValidationResult(result, trackingId, councilLevel, shopTabIndex, shopPosition)
						.Send();
					BundleContentDefinition bundleContentDefinition = manager.GameEconomyData.GetBundleContentDefinition(lastInitiatedBundleId);
					manager.Debug.LogWarning($"SendValidationEvent: bundleContent {bundleContentDefinition == null}");
					BundleStoreDefinition bundleStoreDefinition2 = manager.GameEconomyData.GetBundleStoreDefinition(lastInitiatedBundleId);
					manager.Debug.LogWarning($"SendValidationEvent: bundleStore2 {bundleStoreDefinition == null}");
					string text2 = "";
					text2 = ((bundleStoreDefinition2.ShopTabIndex != 0) ? "resource" : "bundle");
					StorePurchaseInfo currentIAP = manager.Player.CurrentIAP;
					string text3 = ((currentIAP != null) ? JsonConvert.SerializeObject(currentIAP) : "null");
					manager.Debug.LogWarning("SendValidationEvent: CurrentIAP " + text3);
					manager.TdMetrics.SetEventType("recharge_result").AddProperty("#event_id", trackingId).AddProperty("tracking_id", trackingId)
						.AddProperty("order_id", currentIAP.Transaction.TransactionIdentifier)
						.AddProperty("order_status", result)
						.AddProperty("order_create_time", DateTime.UtcNow)
						.AddProperty("product_value", (int)Math.Floor(currentIAP.Product.PriceUSD * 100f + 0.5f))
						.AddProperty("currency_type", currentIAP.Product.CurrencyCode)
						.AddProperty("pay_amount_usd", currentIAP.Product.PriceUSD)
						.AddProperty("pay_amount", currentIAP.Product.Price)
						.AddProperty("product_type", text2)
						.AddProperty("purchase_channel", currentIAP.Store.ToString())
						.AddProperty("bundle_Id", currentIAP.BundleId)
						.AddProperty("product_id", bundleContentDefinition.IAPProduct)
						.AddProperty("product_detail", bundleContentDefinition.RewardEntries.RewardResources)
						.Send();
				}
				else if (customBundleDefinition != null)
				{
					manager.Metrics.Reset().AddBuy().AddCustomBundle(customBundleDefinition, bundleSource)
						.AddIAPValidationResult(result, trackingId, councilLevel, shopTabIndex, shopPosition)
						.Send();
					manager.Debug.LogWarning($"SendValidationEvent: bundleStore2 {customBundleDefinition == null}");
					string value = "";
					StorePurchaseInfo currentIAP2 = manager.Player.CurrentIAP;
					string text4 = ((currentIAP2 != null) ? JsonConvert.SerializeObject(currentIAP2) : "null");
					manager.Debug.LogWarning("SendValidationEvent: CurrentIAP " + text4);
					manager.TdMetrics.SetEventType("recharge_result").AddProperty("#event_id", trackingId).AddProperty("tracking_id", trackingId)
						.AddProperty("order_id", currentIAP2.Transaction.TransactionIdentifier)
						.AddProperty("order_status", result)
						.AddProperty("order_create_time", DateTime.UtcNow)
						.AddProperty("product_value", (int)Math.Floor(currentIAP2.Product.PriceUSD * 100f + 0.5f))
						.AddProperty("currency_type", currentIAP2.Product.CurrencyCode)
						.AddProperty("pay_amount_usd", currentIAP2.Product.PriceUSD)
						.AddProperty("pay_amount", currentIAP2.Product.Price)
						.AddProperty("product_type", value)
						.AddProperty("purchase_channel", currentIAP2.Store.ToString())
						.AddProperty("bundle_Id", currentIAP2.BundleId)
						.AddProperty("product_id", customBundleDefinition.IAPProduct)
						.AddProperty("product_detail", customBundleDefinition.RewardEntries.RewardResources)
						.Send();
				}
				else
				{
					manager.Debug.LogWarning("Bundle not found - bundle identifier '" + (lastInitiatedBundleId ?? "NULL") + "'");
				}
			}
		}

		public static void SendValidationEventDebug(TWDModelManager manager, PurchaseValidationResult result, string trackingId, Metrics.BundleSource bundleSource, StorePurchaseInfo currentIap, int shopTabIndex = -1)
		{
			if (manager != null && manager.Player != null)
			{
				string lastInitiatedBundleId = manager.Player.BundleManager.LastInitiatedBundleId;
				if (manager.GameEconomyData.GetBundleStoreDefinition(lastInitiatedBundleId) != null)
				{
					BundleContentDefinition bundleContentDefinition = manager.GameEconomyData.GetBundleContentDefinition(lastInitiatedBundleId);
					BundleStoreDefinition bundleStoreDefinition = manager.GameEconomyData.GetBundleStoreDefinition(lastInitiatedBundleId);
					string text = "";
					text = ((bundleStoreDefinition.ShopTabIndex != 0) ? "resource" : "bundle");
					manager.TdMetrics.SetEventType("recharge_result").AddProperty("#event_id", trackingId).AddProperty("tracking_id", trackingId)
						.AddProperty("order_id", currentIap.Transaction.TransactionIdentifier)
						.AddProperty("order_status", result)
						.AddProperty("order_create_time", DateTime.UtcNow)
						.AddProperty("product_value", (int)Math.Floor(currentIap.Product.PriceUSD * 100f + 0.5f))
						.AddProperty("currency_type", currentIap.Product.CurrencyCode)
						.AddProperty("pay_amount_usd", currentIap.Product.PriceUSD)
						.AddProperty("pay_amount", currentIap.Product.Price)
						.AddProperty("product_type", text)
						.AddProperty("purchase_channel", currentIap.Store.ToString())
						.AddProperty("bundle_Id", currentIap.BundleId)
						.AddProperty("product_id", bundleContentDefinition.IAPProduct)
						.AddProperty("product_detail", bundleContentDefinition.RewardEntries.RewardResources)
						.Send();
				}
				else
				{
					manager.Debug.LogWarning("Bundle not found - bundle identifier '" + (lastInitiatedBundleId ?? "NULL") + "'");
				}
			}
		}
	}
}
