using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TWDModel
{
	public class SubscriptionManager : TWDModelObject, IActivityManagerIntegrationInterface
	{
		public Dictionary<string, long> SubscriptionExpireDictionary { get; private set; }

		[JsonIgnore]
		public bool IsShowSubscription => base.manager.Player.CouncilLevel >= base.manager.GameEconomyData.SubscriptionConfig.CouncilLockLevel;

		[JsonIgnore]
		public SubscriptionSyncStatus WeeklySyncStatus { get; set; }

		[JsonIgnore]
		public SubscriptionSyncStatus MonthlySyncStatus { get; set; }

		[JsonIgnore]
		private long MaxExpiryTimeMillis
		{
			get
			{
				long num = 0L;
				if (SubscriptionExpireDictionary != null)
				{
					foreach (KeyValuePair<string, long> item in SubscriptionExpireDictionary)
					{
						if (item.Value > num)
						{
							num = item.Value;
						}
					}
				}
				return num;
			}
		}

		[JsonIgnore]
		public bool IsSubscriptionActive => base.manager.Player.UtcTimeStamp <= MaxExpiryTimeMillis;

		[JsonIgnore]
		public bool IsActiveWeeklySubscription
		{
			get
			{
				string weeklySubscriptionPrice = base.manager.GameEconomyData.SubscriptionConfig.WeeklySubscriptionPrice;
				if (SubscriptionExpireDictionary.TryGetValue(weeklySubscriptionPrice, out var value))
				{
					return base.manager.Player.UtcTimeStamp <= value;
				}
				return false;
			}
		}

		[JsonIgnore]
		public bool IsActiveMonthlySubscription
		{
			get
			{
				string monthlySubscriptionPrice = base.manager.GameEconomyData.SubscriptionConfig.MonthlySubscriptionPrice;
				if (SubscriptionExpireDictionary.TryGetValue(monthlySubscriptionPrice, out var value))
				{
					return base.manager.Player.UtcTimeStamp <= value;
				}
				return false;
			}
		}

		[JsonIgnore]
		public long WeeklySubscriptionExpiryMillis
		{
			get
			{
				string weeklySubscriptionPrice = base.manager.GameEconomyData.SubscriptionConfig.WeeklySubscriptionPrice;
				long result = 0L;
				if (SubscriptionExpireDictionary.TryGetValue(weeklySubscriptionPrice, out var value) && base.manager.Player.UtcTimeStamp <= value)
				{
					result = value;
				}
				return result;
			}
		}

		[JsonIgnore]
		public long MonthlySubscriptionExpiryMillis
		{
			get
			{
				string monthlySubscriptionPrice = base.manager.GameEconomyData.SubscriptionConfig.MonthlySubscriptionPrice;
				long result = 0L;
				if (SubscriptionExpireDictionary.TryGetValue(monthlySubscriptionPrice, out var value) && base.manager.Player.UtcTimeStamp <= value)
				{
					result = value;
				}
				return result;
			}
		}

		public override bool IsValid()
		{
			return true;
		}

		public override void Start()
		{
			base.Start();
			if (SubscriptionExpireDictionary == null)
			{
				SubscriptionExpireDictionary = new Dictionary<string, long>();
			}
		}

		public void ClearSubscriptionExpireDictionary()
		{
			SubscriptionExpireDictionary.Clear();
			MonthlySyncStatus = SubscriptionSyncStatus.AlreadySync;
			WeeklySyncStatus = SubscriptionSyncStatus.AlreadySync;
		}

		private static string ToMainProductId(string platformProductId)
		{
			if (platformProductId.EndsWith("_LV"))
			{
				return platformProductId.Substring(0, platformProductId.Length - 3);
			}
			return platformProductId;
		}

		public void SyncSubscriptionExpireDictionary(string subscriptionId, long expiryTimeMillis, long? buyTime = null)
		{
			if (SubscriptionExpireDictionary.TryGetValue(subscriptionId, out var _))
			{
				SubscriptionExpireDictionary[subscriptionId] = expiryTimeMillis;
			}
			else
			{
				SubscriptionExpireDictionary.Add(subscriptionId, expiryTimeMillis);
			}
			if (subscriptionId == base.manager.GameEconomyData.SubscriptionConfig.WeeklySubscriptionPrice)
			{
				UpdateWeeklySubscriptionStatus(SubscriptionSyncStatus.AlreadySync);
			}
			else if (subscriptionId == base.manager.GameEconomyData.SubscriptionConfig.MonthlySubscriptionPrice)
			{
				UpdateMonthlySubscriptionStatus(SubscriptionSyncStatus.AlreadySync);
			}
			BundleContentDefinition bundleContentDefinition = base.manager.Player.gameEconomyData.GetBundleContentDefinition(subscriptionId);
			InAppPurchaseProductApple inAppPurchaseProduct = base.manager.Player.gameEconomyData.GetInAppPurchaseProduct(ToMainProductId(bundleContentDefinition.IAPProduct));
			base.manager.Player.RFMGiftManager.AddPurchargeInfo(inAppPurchaseProduct.PriceUSD, buyTime);
			if (IsSubscriptionActive)
			{
				base.manager.Player.EndlessModeManager.UseSubscriptionConfig = true;
				CurrencyModel currency = base.manager.Player.GetCurrency(CurrencyType.EndlessPassToken);
				int val = base.manager.Player.gameEconomyData.EndlessModeConfig.SubscriptionMaxPasses - currency.Value;
				int amount = Math.Min(base.manager.Player.gameEconomyData.EndlessModeConfig.SubscriptionPassesGivenPerRefresh - base.manager.Player.gameEconomyData.EndlessModeConfig.PassesGivenPerRefresh, val);
				currency.Add(amount);
				base.manager.Player.EndlessModeManager.UseSubscriptionConfig = true;
				CurrencyModel currency2 = base.manager.Player.GetCurrency(CurrencyType.EndlessPassExpertToken);
				int val2 = base.manager.Player.gameEconomyData.EndlessModeConfig.SubscriptionMaxExpertPasses - currency2.Value;
				int amount2 = Math.Min(base.manager.Player.gameEconomyData.EndlessModeConfig.SubscriptionExpertPassesGivenPerRefresh - base.manager.Player.gameEconomyData.EndlessModeConfig.EndlessExpertPassesGivenPerRefresh, val2);
				currency2.Add(amount2);
			}
		}

		public bool UpdateWeeklySubscriptionStatus(SubscriptionSyncStatus weeklySubscriptionSyncStatus)
		{
			WeeklySyncStatus = weeklySubscriptionSyncStatus;
			return true;
		}

		public bool UpdateMonthlySubscriptionStatus(SubscriptionSyncStatus monthlySubscriptionSyncStatus)
		{
			MonthlySyncStatus = monthlySubscriptionSyncStatus;
			return true;
		}

		public string GetIntegrationEventId()
		{
			return "Subscription";
		}

		public bool CanShowInActivityList()
		{
			return IsShowSubscription;
		}

		public bool AreThereAnyUnclaimedReward()
		{
			return false;
		}

		public bool AreThereCanCompleteTask()
		{
			return false;
		}

		public bool IsActivityOpen()
		{
			return false;
		}
	}
}
