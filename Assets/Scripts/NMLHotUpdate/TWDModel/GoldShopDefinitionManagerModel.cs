using System;
using System.Collections.Generic;

namespace TWDModel
{
	public class GoldShopDefinitionManagerModel : TWDModelObject
	{
		private static long CheckForNewBundlesDefaultTime = 60000L;

		private static long CheckForLimitedBundlesCombatInterval = 5000L;

		private static long CheckForLimitedBundlesNonCombatInterval = 500L;

		public Dictionary<string, int> BoughtBundlesAmount { get; set; }

		public Dictionary<string, long> BoughtBundlesLastPurchaseTime { get; set; }

		public List<LimitedBundleData> InitiatedLimitedBundles { get; set; }

		public long CheckForNewBundlesTimer { get; set; }

		public long CheckForLimitedBundlesTimer { get; set; }

		public Rewards LastReceivedComponents { get; set; }

		public override void Initialize()
		{
			base.Initialize();
			BoughtBundlesAmount = new Dictionary<string, int>();
			BoughtBundlesLastPurchaseTime = new Dictionary<string, long>();
			InitiatedLimitedBundles = new List<LimitedBundleData>();
			LastReceivedComponents = new Rewards();
			CheckForNewBundlesTimer = 0L;
		}

		public override void Start()
		{
			base.Start();
			if (BoughtBundlesAmount == null)
			{
				BoughtBundlesAmount = new Dictionary<string, int>();
			}
			if (BoughtBundlesLastPurchaseTime == null)
			{
				BoughtBundlesLastPurchaseTime = new Dictionary<string, long>();
			}
			if (InitiatedLimitedBundles == null)
			{
				InitiatedLimitedBundles = new List<LimitedBundleData>();
			}
			if (LastReceivedComponents == null)
			{
				LastReceivedComponents = new Rewards();
			}
			CheckForNewBundlesTimer = 0L;
		}

		public override void Tick(long deltaTime)
		{
			base.Tick(deltaTime);
			long num = CheckForLimitedBundlesNonCombatInterval;
			if (base.manager.Player.Combat != null)
			{
				num = CheckForLimitedBundlesCombatInterval;
			}
			CheckForLimitedBundlesTimer += deltaTime;
			if (CheckForLimitedBundlesTimer >= num)
			{
				TickRegisteredLimitedBundles(CheckForLimitedBundlesTimer);
				CheckForLimitedBundlesTimer = 0L;
			}
			CheckForNewBundlesTimer -= deltaTime;
			if (CheckForNewBundlesTimer <= 0)
			{
				CheckForNewLimitedBundles();
				CheckForNewBundlesTimer = CheckForNewBundlesDefaultTime;
			}
		}

		private void TickRegisteredLimitedBundles(long deltaTime)
		{
			List<LimitedBundleData> list = null;
			for (int i = 0; i < InitiatedLimitedBundles.Count; i++)
			{
				LimitedBundleData limitedBundleData = InitiatedLimitedBundles[i];
				GoldShopDefinition goldShopDefinition = base.manager.Player.gameEconomyData.GetGoldShopDefinition(limitedBundleData.BundleID);
				if (goldShopDefinition == null || !IsBundleAvailableForStore(goldShopDefinition) || HasLimitedBundleDefinitionChanged(limitedBundleData))
				{
					if (list == null)
					{
						list = new List<LimitedBundleData>();
					}
					list.Add(limitedBundleData);
					continue;
				}
				long num = goldShopDefinition.EndTimeMilliseconds - base.manager.Player.UtcTimeStamp;
				long timer = limitedBundleData.Timer;
				limitedBundleData.Timer -= deltaTime;
				if (num > 0)
				{
					limitedBundleData.Timer = Math.Min(limitedBundleData.Timer, num);
				}
				if (limitedBundleData.Timer <= 0)
				{
					limitedBundleData.IsAvailable = !limitedBundleData.IsAvailable;
					limitedBundleData.Timer = limitedBundleData.MinTimeFromLastCategoryBought * 1000;
					long num2 = deltaTime - timer;
					limitedBundleData.Timer = Math.Max(0L, Math.Min(limitedBundleData.Timer, limitedBundleData.Timer - num2));
				}
			}
			if (list != null)
			{
				for (int j = 0; j < list.Count; j++)
				{
					_ = list[j]?.BundleID;
					InitiatedLimitedBundles.Remove(list[j]);
				}
			}
			ResetLimitedBundlesAmountCounter();
		}

		private void CheckForNewLimitedBundles()
		{
			List<GoldShopDefinition> orderedAvailableBundles = GetOrderedAvailableBundles();
			for (int i = 0; i < orderedAvailableBundles.Count; i++)
			{
				GoldShopDefinition goldShopDefinition = orderedAvailableBundles[i];
				if (goldShopDefinition.HasDateLimit && GetInitiatedLimitedBundle(goldShopDefinition.ItemId) == null)
				{
					SetupNewLimitedBundle(goldShopDefinition);
				}
			}
		}

		private void SetupNewLimitedBundle(GoldShopDefinition bundle, bool skipValidation = false)
		{
			if (skipValidation || (IsBundleAvailableForStore(bundle) && GetInitiatedLimitedBundle(bundle.ItemId) == null && bundle.HasDateLimit))
			{
				LimitedBundleData limitedBundleData = new LimitedBundleData();
				limitedBundleData.BundleID = bundle.ItemId;
				limitedBundleData.IsAvailable = true;
				if (bundle.HasDateLimit)
				{
					long utcTimeStamp = base.manager.Player.UtcTimeStamp;
					long endTimeMilliseconds = bundle.EndTimeMilliseconds;
					limitedBundleData.Timer = Math.Max(0L, endTimeMilliseconds - utcTimeStamp);
				}
				limitedBundleData.StartTimestamp = bundle.StartTimeUTC;
				limitedBundleData.EndTimestamp = bundle.EndTimeUTC;
				limitedBundleData.MinTimeFromLastCategoryBought = bundle.MinTimeFromLastCategoryBought;
				InitiatedLimitedBundles.Add(limitedBundleData);
			}
		}

		private void ResetLimitedBundlesAmountCounter()
		{
			if (BoughtBundlesLastPurchaseTime == null)
			{
				return;
			}
			List<string> list = null;
			foreach (KeyValuePair<string, long> item in BoughtBundlesLastPurchaseTime)
			{
				GoldShopDefinition goldShopDefinition = base.manager.Player.gameEconomyData.GetGoldShopDefinition(item.Key);
				if (goldShopDefinition != null && goldShopDefinition.HasDateLimit && goldShopDefinition.StartTimeMilliseconds > item.Value)
				{
					if (list == null)
					{
						list = new List<string>();
					}
					list.Add(item.Key);
				}
			}
			if (list == null)
			{
				return;
			}
			for (int i = 0; i < list.Count; i++)
			{
				string key = list[i];
				if (BoughtBundlesAmount.ContainsKey(key))
				{
					BoughtBundlesAmount.Remove(key);
				}
				if (BoughtBundlesLastPurchaseTime.ContainsKey(key))
				{
					BoughtBundlesLastPurchaseTime.Remove(key);
				}
			}
		}

		public List<GoldShopDefinition> GetOrderedAvailableBundles()
		{
			if (base.manager != null && base.manager.Player != null)
			{
				PlayerModel player = base.manager.Player;
				List<GoldShopDefinition> list = new List<GoldShopDefinition>();
				List<GoldShopDefinition> orderedGoldShopDefinitionBundles = player.gameEconomyData.GetOrderedGoldShopDefinitionBundles(player, player.UtcTimeStamp);
				for (int i = 0; i < orderedGoldShopDefinitionBundles.Count; i++)
				{
					GoldShopDefinition goldShopDefinition = orderedGoldShopDefinitionBundles[i];
					if (CanBuyBundle(goldShopDefinition))
					{
						list.Add(goldShopDefinition);
					}
				}
				return list;
			}
			return null;
		}

		public bool CanBuyBundle(GoldShopDefinition bundleStoreEntry)
		{
			if (bundleStoreEntry == null)
			{
				return false;
			}
			PlayerModel player = base.manager.Player;
			GameEconomyData gameEconomyData = base.manager.Player.gameEconomyData;
			if (player != null && gameEconomyData != null)
			{
				LimitedBundleData initiatedLimitedBundle = GetInitiatedLimitedBundle(bundleStoreEntry.ItemId);
				if (initiatedLimitedBundle != null && !HasLimitedBundleDefinitionChanged(initiatedLimitedBundle) && !initiatedLimitedBundle.IsAvailable)
				{
					base.Debug.Log("Bundle can't be bought because it is during cooldown timer");
					return false;
				}
				return IsBundleAvailableForStore(bundleStoreEntry);
			}
			return false;
		}

		private bool HasLimitedBundleDefinitionChanged(LimitedBundleData bundleData)
		{
			if (base.manager != null && base.manager.Player != null && bundleData != null)
			{
				GoldShopDefinition goldShopDefinition = base.manager.Player.gameEconomyData.GetGoldShopDefinition(bundleData.BundleID);
				if (goldShopDefinition != null && !(goldShopDefinition.StartTimeUTC != bundleData.StartTimestamp))
				{
					return goldShopDefinition.EndTimeUTC != bundleData.EndTimestamp;
				}
				return true;
			}
			return false;
		}

		public bool BuyBundle(GoldShopDefinition shopDefinition)
		{
			if (shopDefinition != null)
			{
				GoldShopDefinition goldShopDefinition = base.manager.GameEconomyData.GetGoldShopDefinition(shopDefinition.ItemId);
				if (BoughtBundlesAmount == null)
				{
					BoughtBundlesAmount = new Dictionary<string, int>();
				}
				if (BoughtBundlesAmount.ContainsKey(goldShopDefinition.ItemId))
				{
					BoughtBundlesAmount[goldShopDefinition.ItemId] = BoughtBundlesAmount[goldShopDefinition.ItemId] + 1;
				}
				else
				{
					BoughtBundlesAmount.Add(goldShopDefinition.ItemId, 1);
				}
				if (BoughtBundlesLastPurchaseTime == null)
				{
					BoughtBundlesLastPurchaseTime = new Dictionary<string, long>();
				}
				if (BoughtBundlesLastPurchaseTime.ContainsKey(goldShopDefinition.ItemId))
				{
					BoughtBundlesLastPurchaseTime[goldShopDefinition.ItemId] = base.manager.Player.UtcTimeStamp;
				}
				else
				{
					BoughtBundlesLastPurchaseTime.Add(goldShopDefinition.ItemId, base.manager.Player.UtcTimeStamp);
				}
				if (goldShopDefinition.RewardEntries != null && goldShopDefinition.RewardEntries.RewardsList != null && goldShopDefinition.RewardEntries.RewardsList.Count > 0)
				{
					Metrics.MetricsResourcesData metricsResourcesData = new Metrics.MetricsResourcesData();
					for (int i = 0; i < goldShopDefinition.RewardEntries.RewardsList.Count; i++)
					{
						IReward reward = goldShopDefinition.RewardEntries.RewardsList[i];
						reward.Give(base.manager, new object[1] { base.manager.Player.PlayerRandom });
						if (reward is RewardCurrency)
						{
							RewardCurrency rewardCurrency = reward as RewardCurrency;
							if (rewardCurrency.Amount > 0)
							{
								metricsResourcesData.SetOrAdd(rewardCurrency.CurrencyType, rewardCurrency.AmountActuallyAdded, rewardCurrency.GetOverflowAmount());
							}
							else if (rewardCurrency.Amount == -1)
							{
								metricsResourcesData.SetOrAdd(rewardCurrency.CurrencyType, rewardCurrency.AmountActuallyAdded);
							}
						}
					}
					LastReceivedComponents = goldShopDefinition.RewardEntries;
					if (shopDefinition.Price > 0)
					{
						base.manager.Metrics.ResourceChangeObtainReason = "GoldShop";
					}
					else
					{
						base.manager.Metrics.ResourceChangeObtainReason = "GoldShopGift";
					}
					base.manager.Metrics.AddFind().AddResources(metricsResourcesData, freeResource: false, combineDuplicates: true).Send();
					return true;
				}
				LimitedBundleData initiatedLimitedBundle = GetInitiatedLimitedBundle(shopDefinition.ItemId);
				if (initiatedLimitedBundle != null && InitiatedLimitedBundles.Contains(initiatedLimitedBundle))
				{
					initiatedLimitedBundle.IsAvailable = false;
					initiatedLimitedBundle.Timer = initiatedLimitedBundle.MinTimeFromLastCategoryBought * 1000;
					if (!IsBundleAvailableForStore(shopDefinition))
					{
						InitiatedLimitedBundles.Remove(initiatedLimitedBundle);
					}
				}
				return false;
			}
			base.Debug.LogError("BuyBundle failed: invalid bundleContentDefinition");
			return false;
		}

		private bool IsBundleAvailableForStore(GoldShopDefinition bundleStoreEntry)
		{
			if (bundleStoreEntry == null)
			{
				return false;
			}
			PlayerModel player = base.manager.Player;
			GameEconomyData gameEconomyData = base.manager.Player.gameEconomyData;
			if (player != null && gameEconomyData != null)
			{
				if (!bundleStoreEntry.IsNewVersion)
				{
					return true;
				}
				if (bundleStoreEntry.MaxPurchases >= 0 && (bundleStoreEntry.MaxPurchases == 0 || (BoughtBundlesAmount != null && BoughtBundlesAmount.ContainsKey(bundleStoreEntry.ItemId) && BoughtBundlesAmount[bundleStoreEntry.ItemId] >= bundleStoreEntry.MaxPurchases)))
				{
					return false;
				}
				if (bundleStoreEntry.HasDateLimit && (player.UtcTimeStamp < bundleStoreEntry.StartTimeMilliseconds || player.UtcTimeStamp > bundleStoreEntry.EndTimeMilliseconds))
				{
					return false;
				}
				return true;
			}
			return false;
		}

		public LimitedBundleData GetInitiatedLimitedBundle(string bundleID)
		{
			if (InitiatedLimitedBundles == null)
			{
				return null;
			}
			for (int i = 0; i < InitiatedLimitedBundles.Count; i++)
			{
				LimitedBundleData limitedBundleData = InitiatedLimitedBundles[i];
				if (limitedBundleData.BundleID == bundleID)
				{
					return limitedBundleData;
				}
			}
			return null;
		}

		public override bool IsValid()
		{
			return true;
		}
	}
}
