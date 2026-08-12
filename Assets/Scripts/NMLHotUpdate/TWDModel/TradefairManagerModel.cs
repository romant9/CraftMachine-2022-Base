using System;
using System.Collections.Generic;

namespace TWDModel
{
	public class TradefairManagerModel : TWDModelObject
	{
		private static long CheckForNewBundlesDefaultTime = 60000L;

		private static long CheckForLimitedBundlesCombatInterval = 5000L;

		private static long CheckForLimitedBundlesNonCombatInterval = 500L;

		public Dictionary<string, int> BoughtBundlesAmount { get; set; }

		public Dictionary<string, long> BoughtBundlesLastPurchaseTime { get; set; }

		public List<LimitedBundleData> InitiatedLimitedBundles { get; set; }

		public long CheckForNewBundlesTimer { get; set; }

		public long CheckForLimitedBundlesTimer { get; set; }

		public override void Initialize()
		{
			base.Initialize();
			BoughtBundlesAmount = new Dictionary<string, int>();
			BoughtBundlesLastPurchaseTime = new Dictionary<string, long>();
			InitiatedLimitedBundles = new List<LimitedBundleData>();
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
			CheckForNewBundlesTimer = 0L;
		}

		private void CheckForNewLimitedBundles()
		{
			List<TradefairBundleStoreDefinition> orderedAvailableBundles = GetOrderedAvailableBundles();
			for (int i = 0; i < orderedAvailableBundles.Count; i++)
			{
				TradefairBundleStoreDefinition tradefairBundleStoreDefinition = orderedAvailableBundles[i];
				if ((tradefairBundleStoreDefinition.HasDateLimit || tradefairBundleStoreDefinition.AvailabilityTime > 0) && GetInitiatedLimitedBundle(tradefairBundleStoreDefinition.BundleIdentifier) == null && !tradefairBundleStoreDefinition.IsPartOfRotation)
				{
					SetupNewLimitedBundle(tradefairBundleStoreDefinition);
				}
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
				TradefairBundleStoreDefinition bundleTradefairDefinition = base.manager.Player.gameEconomyData.GetBundleTradefairDefinition(item.Key);
				if (bundleTradefairDefinition != null && bundleTradefairDefinition.HasDateLimit && bundleTradefairDefinition.StartTimeMilliseconds > item.Value)
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

		private void SetupNewLimitedBundle(TradefairBundleStoreDefinition bundle, bool skipValidation = false)
		{
			if (skipValidation || (IsBundleAvailableForStore(bundle) && GetInitiatedLimitedBundle(bundle.BundleIdentifier) == null && (bundle.HasDateLimit || bundle.AvailabilityTime > 0)))
			{
				LimitedBundleData limitedBundleData = new LimitedBundleData();
				limitedBundleData.BundleID = bundle.BundleIdentifier;
				limitedBundleData.IsAvailable = true;
				if (bundle.HasDateLimit && bundle.AvailabilityTime <= 0)
				{
					long utcTimeStamp = base.manager.Player.UtcTimeStamp;
					long bundleStoreEndTime = GetBundleStoreEndTime(bundle);
					limitedBundleData.Timer = Math.Max(0L, bundleStoreEndTime - utcTimeStamp);
				}
				else
				{
					limitedBundleData.Timer = bundle.AvailabilityTime * 1000;
				}
				limitedBundleData.StartTimestamp = bundle.StartTimestamp;
				limitedBundleData.EndTimestamp = bundle.EndTimestamp;
				limitedBundleData.AvailabilityTime = bundle.AvailabilityTime;
				limitedBundleData.MinTimeFromLastCategoryBought = bundle.MinTimeFromLastCategoryBought;
				InitiatedLimitedBundles.Add(limitedBundleData);
			}
		}

		private void TickRegisteredLimitedBundles(long deltaTime)
		{
			List<LimitedBundleData> list = null;
			for (int i = 0; i < InitiatedLimitedBundles.Count; i++)
			{
				LimitedBundleData limitedBundleData = InitiatedLimitedBundles[i];
				TradefairBundleStoreDefinition bundleTradefairDefinition = base.manager.Player.gameEconomyData.GetBundleTradefairDefinition(limitedBundleData.BundleID);
				if (bundleTradefairDefinition == null || !IsBundleAvailableForStore(bundleTradefairDefinition) || HasLimitedBundleDefinitionChanged(limitedBundleData))
				{
					if (list == null)
					{
						list = new List<LimitedBundleData>();
					}
					list.Add(limitedBundleData);
					continue;
				}
				long num = GetBundleStoreEndTime(bundleTradefairDefinition) - base.manager.Player.UtcTimeStamp;
				long timer = limitedBundleData.Timer;
				limitedBundleData.Timer -= deltaTime;
				if (num > 0)
				{
					limitedBundleData.Timer = Math.Min(limitedBundleData.Timer, num);
				}
				if (limitedBundleData.Timer > 0)
				{
					continue;
				}
				limitedBundleData.IsAvailable = !limitedBundleData.IsAvailable;
				if (limitedBundleData.IsAvailable)
				{
					if (bundleTradefairDefinition.HasDateLimit && bundleTradefairDefinition.AvailabilityTime <= 0)
					{
						long utcTimeStamp = base.manager.Player.UtcTimeStamp;
						long bundleStoreEndTime = GetBundleStoreEndTime(bundleTradefairDefinition);
						limitedBundleData.Timer = Math.Max(0L, bundleStoreEndTime - utcTimeStamp);
					}
					else
					{
						limitedBundleData.Timer = bundleTradefairDefinition.AvailabilityTime * 1000;
					}
				}
				else
				{
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

		public override bool IsValid()
		{
			return true;
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

		public bool CanBuyBundle(TradefairBundleStoreDefinition bundleStoreEntry)
		{
			if (bundleStoreEntry == null)
			{
				return false;
			}
			PlayerModel player = base.manager.Player;
			GameEconomyData gameEconomyData = base.manager.Player.gameEconomyData;
			if (player != null && gameEconomyData != null)
			{
				LimitedBundleData initiatedLimitedBundle = GetInitiatedLimitedBundle(bundleStoreEntry.BundleIdentifier);
				if (initiatedLimitedBundle != null && !HasLimitedBundleDefinitionChanged(initiatedLimitedBundle) && !initiatedLimitedBundle.IsAvailable)
				{
					base.Debug.Log("Bundle can't be bought because it is during cooldown timer");
					return false;
				}
				return IsBundleAvailableForStore(bundleStoreEntry);
			}
			return false;
		}

		private bool IsBundleAvailableForStore(TradefairBundleStoreDefinition bundleStoreEntry)
		{
			if (bundleStoreEntry == null)
			{
				return false;
			}
			PlayerModel player = base.manager.Player;
			GameEconomyData gameEconomyData = base.manager.Player.gameEconomyData;
			if (player != null && gameEconomyData != null)
			{
				if (bundleStoreEntry.MaxPurchases >= 0 && (bundleStoreEntry.MaxPurchases == 0 || (BoughtBundlesAmount != null && BoughtBundlesAmount.ContainsKey(bundleStoreEntry.BundleIdentifier) && BoughtBundlesAmount[bundleStoreEntry.BundleIdentifier] >= bundleStoreEntry.MaxPurchases)))
				{
					return false;
				}
				bool flag = string.IsNullOrEmpty(bundleStoreEntry.PreviousBundle) || (BoughtBundlesAmount != null && BoughtBundlesAmount.ContainsKey(bundleStoreEntry.PreviousBundle));
				if (string.IsNullOrEmpty(bundleStoreEntry.EquivalentPreviousRotation) && !flag)
				{
					return false;
				}
				if (!flag && string.IsNullOrEmpty(bundleStoreEntry.EquivalentPreviousRotation))
				{
					return false;
				}
				if (bundleStoreEntry.SurvivorClassRequired != SurvivorClass.None && !player.SurvivorContainer.IsSurvivorClassUnlocked(bundleStoreEntry.SurvivorClassRequired) && !player.SurvivorContainer.IsHeroTypeUnlocked(bundleStoreEntry.SurvivorClassRequired))
				{
					return false;
				}
				if (!string.IsNullOrEmpty(bundleStoreEntry.MapIdRequired))
				{
					MissionSpawnPoint spawnPoint = player.gameEconomyData.MissionSpawnPointData.GetSpawnPoint(bundleStoreEntry.MapIdRequired, bundleStoreEntry.MissionIndexRequired);
					if (spawnPoint != null && !player.MapContainerModel.IsMissionCompleted(spawnPoint))
					{
						return false;
					}
				}
				if (bundleStoreEntry.HasDateLimit && (player.UtcTimeStamp < bundleStoreEntry.StartTimeMilliseconds || player.UtcTimeStamp > GetBundleStoreEndTime(bundleStoreEntry)))
				{
					return false;
				}
				if (!bundleStoreEntry.IsPartOfRotation && bundleStoreEntry.MinTimeFromLastCategoryBought > 0 && !string.IsNullOrEmpty(bundleStoreEntry.BundleIdentifier))
				{
					TradefairBundleContentDefinition tradefairBundleContentDefinition = gameEconomyData.GetTradefairBundleContentDefinition(bundleStoreEntry.BundleIdentifier);
					if (tradefairBundleContentDefinition != null && !string.IsNullOrEmpty(tradefairBundleContentDefinition.Category))
					{
						long lastCategoryBoughtTimestamp = GetLastCategoryBoughtTimestamp(tradefairBundleContentDefinition.Category);
						if (lastCategoryBoughtTimestamp > 0)
						{
							return (player.UtcTimeStamp - lastCategoryBoughtTimestamp) / 1000 > bundleStoreEntry.MinTimeFromLastCategoryBought;
						}
					}
				}
				return true;
			}
			return false;
		}

		private long GetLastCategoryBoughtTimestamp(string category)
		{
			long num = 0L;
			if (!string.IsNullOrEmpty(category) && BoughtBundlesLastPurchaseTime != null)
			{
				foreach (KeyValuePair<string, long> item in BoughtBundlesLastPurchaseTime)
				{
					TradefairBundleStoreDefinition bundleTradefairDefinition = base.manager.Player.gameEconomyData.GetBundleTradefairDefinition(item.Key);
					if (bundleTradefairDefinition != null)
					{
						TradefairBundleContentDefinition tradefairBundleContentDefinition = base.manager.Player.gameEconomyData.GetTradefairBundleContentDefinition(bundleTradefairDefinition.BundleIdentifier);
						if (tradefairBundleContentDefinition != null && tradefairBundleContentDefinition.Category == category && num < item.Value)
						{
							num = item.Value;
						}
					}
				}
			}
			return num;
		}

		private bool HasLimitedBundleDefinitionChanged(LimitedBundleData bundleData)
		{
			if (base.manager != null && base.manager.Player != null && bundleData != null)
			{
				TradefairBundleStoreDefinition bundleTradefairDefinition = base.manager.Player.gameEconomyData.GetBundleTradefairDefinition(bundleData.BundleID);
				if (bundleTradefairDefinition != null && !(bundleTradefairDefinition.StartTimestamp != bundleData.StartTimestamp) && !(bundleTradefairDefinition.EndTimestamp != bundleData.EndTimestamp) && bundleTradefairDefinition.AvailabilityTime == bundleData.AvailabilityTime)
				{
					return bundleTradefairDefinition.MinTimeFromLastCategoryBought != bundleData.MinTimeFromLastCategoryBought;
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

		public List<TradefairBundleStoreDefinition> GetOrderedAvailableBundles()
		{
			if (base.manager != null && base.manager.Player != null)
			{
				PlayerModel player = base.manager.Player;
				List<TradefairBundleStoreDefinition> list = new List<TradefairBundleStoreDefinition>();
				List<TradefairBundleStoreDefinition> orderedTradefairBundles = player.gameEconomyData.GetOrderedTradefairBundles(player.UtcTimeStamp);
				for (int i = 0; i < orderedTradefairBundles.Count; i++)
				{
					TradefairBundleStoreDefinition tradefairBundleStoreDefinition = orderedTradefairBundles[i];
					if (CanBuyBundle(tradefairBundleStoreDefinition))
					{
						list.Add(tradefairBundleStoreDefinition);
					}
				}
				return list;
			}
			return null;
		}

		public bool BuyBundle(TradefairBundleStoreDefinition bundleDefinition, TradeFairPurchaseType payType = TradeFairPurchaseType.None, string metricsResourceChangeObtainReason = "")
		{
			base.Debug.LogInfo($"BuyBundle buy bundle, order.PurchaseSource:{payType},order.BundleId :{bundleDefinition.BundleIdentifier}");
			if (bundleDefinition != null)
			{
				TradefairBundleContentDefinition tradefairBundleContentDefinition = base.manager.GameEconomyData.GetTradefairBundleContentDefinition(bundleDefinition.BundleIdentifier);
				if (BoughtBundlesAmount == null)
				{
					BoughtBundlesAmount = new Dictionary<string, int>();
				}
				if (BoughtBundlesAmount.ContainsKey(bundleDefinition.BundleIdentifier))
				{
					BoughtBundlesAmount[bundleDefinition.BundleIdentifier] = BoughtBundlesAmount[bundleDefinition.BundleIdentifier] + 1;
				}
				else
				{
					BoughtBundlesAmount.Add(bundleDefinition.BundleIdentifier, 1);
				}
				if (BoughtBundlesLastPurchaseTime == null)
				{
					BoughtBundlesLastPurchaseTime = new Dictionary<string, long>();
				}
				if (BoughtBundlesLastPurchaseTime.ContainsKey(bundleDefinition.BundleIdentifier))
				{
					BoughtBundlesLastPurchaseTime[bundleDefinition.BundleIdentifier] = base.manager.Player.UtcTimeStamp;
				}
				else
				{
					BoughtBundlesLastPurchaseTime.Add(bundleDefinition.BundleIdentifier, base.manager.Player.UtcTimeStamp);
				}
				TradefairBundleContentDefinition tradefairBundleContentDefinition2 = base.manager.Player.gameEconomyData.GetTradefairBundleContentDefinition(bundleDefinition.BundleIdentifier);
				if (tradefairBundleContentDefinition2 != null && tradefairBundleContentDefinition2.RewardEntries != null)
				{
					if (tradefairBundleContentDefinition2.RewardEntries != null && tradefairBundleContentDefinition2.RewardEntries.RewardsList != null && tradefairBundleContentDefinition2.RewardEntries.RewardsList.Count > 0)
					{
						Metrics.MetricsResourcesData metricsResourcesData = new Metrics.MetricsResourcesData();
						for (int i = 0; i < tradefairBundleContentDefinition2.RewardEntries.RewardsList.Count; i++)
						{
							IReward reward = tradefairBundleContentDefinition2.RewardEntries.RewardsList[i];
							reward.Give(base.manager, new object[1] { base.manager.Player.PlayerRandom });
							if (tradefairBundleContentDefinition != null && reward is RewardCurrency)
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
						if (!string.IsNullOrEmpty(metricsResourceChangeObtainReason))
						{
							base.manager.Metrics.ResourceChangeObtainReason = metricsResourceChangeObtainReason;
						}
						base.manager.Metrics.AddFind().AddResources(metricsResourcesData, freeResource: false, combineDuplicates: true).Send();
					}
					if ((payType == TradeFairPurchaseType.TradeFairXSolla || payType == TradeFairPurchaseType.TradeFairAppcharge) && tradefairBundleContentDefinition2 != null && tradefairBundleContentDefinition2.ExtraRewardEntries != null && tradefairBundleContentDefinition2.ExtraRewardEntries != null && tradefairBundleContentDefinition2.ExtraRewardEntries.RewardsList != null && tradefairBundleContentDefinition2.ExtraRewardEntries.RewardsList.Count > 0)
					{
						Metrics.MetricsResourcesData metricsResourcesData2 = new Metrics.MetricsResourcesData();
						for (int j = 0; j < tradefairBundleContentDefinition2.ExtraRewardEntries.RewardsList.Count; j++)
						{
							IReward reward2 = tradefairBundleContentDefinition2.ExtraRewardEntries.RewardsList[j];
							reward2.Give(base.manager, new object[1] { base.manager.Player.PlayerRandom });
							if (tradefairBundleContentDefinition != null && reward2 is RewardCurrency)
							{
								RewardCurrency rewardCurrency2 = reward2 as RewardCurrency;
								if (rewardCurrency2.Amount > 0)
								{
									metricsResourcesData2.SetOrAdd(rewardCurrency2.CurrencyType, rewardCurrency2.AmountActuallyAdded, rewardCurrency2.GetOverflowAmount());
								}
								else if (rewardCurrency2.Amount == -1)
								{
									metricsResourcesData2.SetOrAdd(rewardCurrency2.CurrencyType, rewardCurrency2.AmountActuallyAdded);
								}
							}
						}
						base.manager.Metrics.AddFind().AddResources(metricsResourcesData2, freeResource: false, combineDuplicates: true).Send();
					}
					return true;
				}
				base.Debug.LogError("BuyBundle failed: invalid bundleContentDefinition");
				return false;
			}
			base.Debug.LogError("BuyBundle failed: invalid bundleDefinition");
			LimitedBundleData initiatedLimitedBundle = GetInitiatedLimitedBundle(bundleDefinition.BundleIdentifier);
			if (initiatedLimitedBundle != null && InitiatedLimitedBundles.Contains(initiatedLimitedBundle))
			{
				initiatedLimitedBundle.IsAvailable = false;
				initiatedLimitedBundle.Timer = initiatedLimitedBundle.MinTimeFromLastCategoryBought * 1000;
				TradefairBundleStoreDefinition bundleTradefairDefinition = base.manager.Player.gameEconomyData.GetBundleTradefairDefinition(initiatedLimitedBundle.BundleID);
				if (bundleTradefairDefinition != null && !IsBundleAvailableForStore(bundleTradefairDefinition))
				{
					InitiatedLimitedBundles.Remove(initiatedLimitedBundle);
				}
			}
			return false;
		}

		private LimitedBundleData GetHighestPriorityLimitedBundle(bool isAvailable)
		{
			List<LimitedBundleData> list = new List<LimitedBundleData>();
			if (InitiatedLimitedBundles != null && InitiatedLimitedBundles.Count > 0)
			{
				for (int i = 0; i < InitiatedLimitedBundles.Count; i++)
				{
					LimitedBundleData limitedBundleData = InitiatedLimitedBundles[i];
					if (limitedBundleData != null && isAvailable == limitedBundleData.IsAvailable)
					{
						list.Add(limitedBundleData);
					}
				}
			}
			if (list.Count > 0)
			{
				list.StableSort(delegate(LimitedBundleData a, LimitedBundleData b)
				{
					if (a == null || b == null)
					{
						return 0;
					}
					TradefairBundleStoreDefinition bundleTradefairDefinition = base.manager.Player.gameEconomyData.GetBundleTradefairDefinition(a.BundleID);
					TradefairBundleStoreDefinition bundleTradefairDefinition2 = base.manager.Player.gameEconomyData.GetBundleTradefairDefinition(b.BundleID);
					if (bundleTradefairDefinition == null || bundleTradefairDefinition2 == null)
					{
						return 0;
					}
					if (bundleTradefairDefinition.DisplayOrder < bundleTradefairDefinition2.DisplayOrder)
					{
						return -1;
					}
					return (bundleTradefairDefinition.DisplayOrder > bundleTradefairDefinition2.DisplayOrder) ? 1 : 0;
				});
				return list[0];
			}
			return null;
		}

		private long GetBundleStoreEndTime(TradefairBundleStoreDefinition bundleStoreDefinition)
		{
			return bundleStoreDefinition?.EndTimeMilliseconds ?? 0;
		}
	}
}
