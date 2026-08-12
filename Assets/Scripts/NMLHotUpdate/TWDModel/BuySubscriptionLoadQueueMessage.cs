using System;
using System.Collections.Generic;

namespace TWDModel
{
	public class BuySubscriptionLoadQueueMessage : SupportLoadQueueMessage
	{
		public string SubscriptionId { get; set; }

		public int SubscriptionPlatform { get; set; }

		public long ExpiryTimeMillis { get; set; }

		public long BuyTime { get; set; }

		public int GiveExtraReward { get; set; }

		public BuySubscriptionLoadQueueMessage()
		{
		}

		public BuySubscriptionLoadQueueMessage(string subscriptionId, int subscriptionPlatform, long expiryTimeMillis, int giveExtraReward, long buyTime)
		{
			SubscriptionId = subscriptionId;
			SubscriptionPlatform = subscriptionPlatform;
			ExpiryTimeMillis = expiryTimeMillis;
			GiveExtraReward = giveExtraReward;
			BuyTime = buyTime;
		}

		public override bool Execute(TWDModelManager manager)
		{
			if (manager.Player != null && manager.Player.BundleManager != null && manager.Player.gameEconomyData != null)
			{
				if (manager.Player.SubscriptionBuyedBundleIds == null)
				{
					manager.Player.SubscriptionBuyedBundleIds = new List<string>();
				}
				if (GiveExtraReward == 1)
				{
					BundleStoreDefinition bundleStoreDefinition = manager.Player.gameEconomyData.GetBundleStoreDefinition(SubscriptionId);
					if (!manager.Player.BundleManager.BuyBundle(bundleStoreDefinition, givenBySupport: false, Metrics.BundleSource.Subscription, 0L))
					{
						return false;
					}
					manager.Player.SubscriptionBuyedBundleIds.Add(SubscriptionId);
				}
				manager.Player.SubscriptionManager.SyncSubscriptionExpireDictionary(SubscriptionId, ExpiryTimeMillis, BuyTime);
				if (GiveExtraReward == 0 && manager.Player.SubscriptionManager.IsSubscriptionActive)
				{
					manager.Player.EndlessModeManager.UseSubscriptionConfig = true;
					if (!manager.Player.EndlessModeManager.SubscriptionGivedToken)
					{
						CurrencyModel currency = manager.Player.GetCurrency(CurrencyType.EndlessPassToken);
						int val = manager.Player.gameEconomyData.EndlessModeConfig.SubscriptionMaxPasses - currency.Value;
						int num = Math.Min(manager.Player.gameEconomyData.EndlessModeConfig.SubscriptionPassesGivenPerRefresh - manager.Player.gameEconomyData.EndlessModeConfig.PassesGivenPerRefresh, val);
						currency.Add(num);
						manager.Player.EndlessModeManager.SubscriptionGivedToken = true;
						manager.Player.manager.Metrics.AddFind().AddResources(CurrencyType.EndlessPassToken, manager.Player.gameEconomyData.EndlessModeConfig.PassesGivenPerRefresh, num).AddEndlessSubscriptionAdd()
							.Send();
						CurrencyModel currency2 = manager.Player.GetCurrency(CurrencyType.EndlessPassExpertToken);
						int val2 = manager.Player.gameEconomyData.EndlessModeConfig.SubscriptionMaxExpertPasses - currency2.Value;
						int num2 = Math.Min(manager.Player.gameEconomyData.EndlessModeConfig.SubscriptionExpertPassesGivenPerRefresh - manager.Player.gameEconomyData.EndlessModeConfig.EndlessExpertPassesGivenPerRefresh, val2);
						currency2.Add(num2);
						manager.Player.EndlessModeManager.SubscriptionGivedToken = true;
						manager.Player.manager.Metrics.AddFind().AddResources(CurrencyType.EndlessPassExpertToken, manager.Player.gameEconomyData.EndlessModeConfig.EndlessExpertPassesGivenPerRefresh, num2).AddEndlessSubscriptionAdd()
							.Send();
					}
				}
				return true;
			}
			return false;
		}
	}
}
