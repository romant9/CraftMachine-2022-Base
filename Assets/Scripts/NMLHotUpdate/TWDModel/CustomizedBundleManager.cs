using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TWDModel
{
	public class CustomizedBundleManager : TWDModelObject
	{
		private static long CheckForNewBundlesDefaultTime = 60000L;

		private static long CheckForLimitedBundlesCombatInterval = 5000L;

		private static long CheckForLimitedBundlesNonCombatInterval = 500L;

		public List<IReward> LastCustomReward = new List<IReward>();

		public const string LimitedCustomBundleAvailableEvent = "LimitedCustomBundleAvailableEvent";

		public const string LimitedCustomBundleExpiredEvent = "LimitedCustomBundleExpiredEvent";

		[JsonIgnore]
		public int currentSelectIndex;

		public Dictionary<string, int> CustomBundleBoughtBundlesAmount { get; set; }

		public Dictionary<string, long> BoughtBundlesLastPurchaseTime { get; set; }

		public List<LimitedCustomBundleData> InitiatedLimitedBundles { get; set; }

		public long CheckForNewBundlesTimer { get; set; }

		public long CheckForLimitedBundlesTimer { get; set; }

		public List<string> CustomBundleOneTime { get; set; }

		[JsonIgnore]
		public Dictionary<string, List<IReward>> CustomRewards { get; set; }

		[JsonIgnore]
		public bool IsCouncilLevelValid => base.manager.Player.CouncilLevel >= base.manager.GameEconomyData.ConfigData.CustomCouncilLevel;

		public override void Initialize()
		{
			base.Initialize();
			CustomBundleBoughtBundlesAmount = new Dictionary<string, int>();
			BoughtBundlesLastPurchaseTime = new Dictionary<string, long>();
			InitiatedLimitedBundles = new List<LimitedCustomBundleData>();
			CustomRewards = new Dictionary<string, List<IReward>>();
			LastCustomReward = new List<IReward>();
			CheckForNewBundlesTimer = 0L;
		}

		public override void Start()
		{
			base.Start();
			if (CustomBundleBoughtBundlesAmount == null)
			{
				CustomBundleBoughtBundlesAmount = new Dictionary<string, int>();
			}
			if (BoughtBundlesLastPurchaseTime == null)
			{
				BoughtBundlesLastPurchaseTime = new Dictionary<string, long>();
			}
			if (InitiatedLimitedBundles == null)
			{
				InitiatedLimitedBundles = new List<LimitedCustomBundleData>();
			}
			if (CustomRewards == null)
			{
				CustomRewards = new Dictionary<string, List<IReward>>();
			}
			if (LastCustomReward == null)
			{
				LastCustomReward = new List<IReward>();
			}
			if (InitiatedLimitedBundles.Count > 0)
			{
				CheckCustomBundleVariation();
			}
			CheckForNewBundlesTimer = 0L;
		}

		public void CheckCustomBundleVariation()
		{
			if (InitiatedLimitedBundles == null || InitiatedLimitedBundles.Count <= 0)
			{
				return;
			}
			long utcTimeStamp = base.manager.Player.UtcTimeStamp;
			List<LimitedCustomBundleData> list = null;
			for (int i = 0; i < InitiatedLimitedBundles.Count; i++)
			{
				LimitedCustomBundleData limitedCustomBundleData = InitiatedLimitedBundles[i];
				if (!limitedCustomBundleData.IsAvailable)
				{
					continue;
				}
				CustomBundleDefinition customBundleDefinition = base.gameEconomyData.GetCustomBundleDefinition(limitedCustomBundleData.Identifier);
				if (customBundleDefinition == null)
				{
					if (list == null)
					{
						list = new List<LimitedCustomBundleData>();
					}
					list.Add(limitedCustomBundleData);
				}
				else if (limitedCustomBundleData.customType != customBundleDefinition.CustomBundleType)
				{
					if (customBundleDefinition.CustomBundleType == CustomizedBundleType.Loop)
					{
						limitedCustomBundleData.customType = customBundleDefinition.CustomBundleType;
						limitedCustomBundleData.Timer = 0L;
						limitedCustomBundleData.IsAvailable = true;
						limitedCustomBundleData.IsCanBy = true;
					}
					else if (customBundleDefinition.CustomBundleType == CustomizedBundleType.TimeBundle)
					{
						limitedCustomBundleData.customType = customBundleDefinition.CustomBundleType;
						limitedCustomBundleData.Timer = 0L;
						limitedCustomBundleData.IsAvailable = true;
						limitedCustomBundleData.IsCanBy = true;
						long endTimeMilliseconds = customBundleDefinition.EndTimeMilliseconds;
						limitedCustomBundleData.Timer = Math.Max(0L, endTimeMilliseconds - utcTimeStamp);
					}
					else
					{
						limitedCustomBundleData.customType = customBundleDefinition.CustomBundleType;
						limitedCustomBundleData.Timer = 0L;
						limitedCustomBundleData.IsAvailable = true;
						limitedCustomBundleData.IsCanBy = true;
					}
					limitedCustomBundleData.StartTimestamp = limitedCustomBundleData.StartTimestamp;
					limitedCustomBundleData.EndTimestamp = limitedCustomBundleData.EndTimestamp;
				}
			}
			if (list != null)
			{
				for (int j = 0; j < list.Count; j++)
				{
					_ = list[j]?.Identifier;
					InitiatedLimitedBundles.Remove(list[j]);
				}
			}
			ResetLimitedBundlesAmountCounter();
		}

		public bool UpgradeCustomRewards(string identifier, int index, IReward reward)
		{
			CustomBundleDefinition customBundleDefinition = base.manager.Player.gameEconomyData.GetCustomBundleDefinition(identifier);
			if (customBundleDefinition != null)
			{
				if (CustomRewards == null)
				{
					CustomRewards = new Dictionary<string, List<IReward>>();
				}
				if (!CanBuyBundle(customBundleDefinition))
				{
					return false;
				}
				if (!CustomRewards.TryGetValue(identifier, out var value))
				{
					value = new List<IReward> { null, null, null };
					CustomRewards[identifier] = value;
				}
				if (index < 0 || index >= value.Count)
				{
					return false;
				}
				value[index] = reward;
				return true;
			}
			return false;
		}

		public bool CustomizedBundleClaimReward(string identifier)
		{
			if (identifier != null && CustomRewards.TryGetValue(identifier, out var _))
			{
				CustomBundleDefinition customBundleDefinition = base.manager.Player.gameEconomyData.GetCustomBundleDefinition(identifier);
				if (!IsCanPay(customBundleDefinition))
				{
					base.manager.Debug.LogError("CustomizedBundleClaimReward: Bundle content count does not match the number of rewards; identifier :" + identifier);
					return false;
				}
				if (customBundleDefinition != null)
				{
					if (CustomBundleBoughtBundlesAmount == null)
					{
						CustomBundleBoughtBundlesAmount = new Dictionary<string, int>();
					}
					if (CustomBundleBoughtBundlesAmount.ContainsKey(customBundleDefinition.Identifier))
					{
						CustomBundleBoughtBundlesAmount[customBundleDefinition.Identifier]++;
					}
					else
					{
						CustomBundleBoughtBundlesAmount.Add(customBundleDefinition.Identifier, 1);
					}
					if (BoughtBundlesLastPurchaseTime == null)
					{
						BoughtBundlesLastPurchaseTime = new Dictionary<string, long>();
					}
					if (BoughtBundlesLastPurchaseTime.ContainsKey(customBundleDefinition.Identifier))
					{
						BoughtBundlesLastPurchaseTime[customBundleDefinition.Identifier] = base.manager.Player.UtcTimeStamp;
					}
					else
					{
						BoughtBundlesLastPurchaseTime.Add(customBundleDefinition.Identifier, base.manager.Player.UtcTimeStamp);
					}
					CustomBundleDefinition customBundleDefinition2 = base.manager.Player.gameEconomyData.GetCustomBundleDefinition(identifier);
					List<IReward> list = new List<IReward>();
					if (customBundleDefinition2 != null && customBundleDefinition2.RewardEntries != null)
					{
						if (customBundleDefinition2.RewardEntries != null && customBundleDefinition2.RewardEntries.RewardsList != null && customBundleDefinition2.RewardEntries.RewardsList.Count > 0)
						{
							Metrics.MetricsResourcesData metricsResourcesData = new Metrics.MetricsResourcesData();
							list.AddRange(customBundleDefinition2.RewardEntries.RewardsList);
							for (int i = 0; i < customBundleDefinition2.RewardEntries.RewardsList.Count; i++)
							{
								IReward reward = customBundleDefinition2.RewardEntries.RewardsList[i];
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
							base.manager.Metrics.AddFind().AddResources(metricsResourcesData, freeResource: false, combineDuplicates: true).Send();
						}
						List<Dictionary<string, object>> list2 = new List<Dictionary<string, object>>();
						string text = "";
						if (customBundleDefinition2.RewardEntries.RewardsList.Count > 0 && customBundleDefinition2.RewardEntries.RewardResources != null && customBundleDefinition2.RewardEntries.RewardResources.Count > 0)
						{
							customBundleDefinition2.RewardEntries.RewardResources[0].TryGetValue("resource_name", out var value2);
							customBundleDefinition2.RewardEntries.RewardResources[0].TryGetValue("resource_num", out var value3);
							text += value2.ToString();
							text += "(";
							text += value3.ToString();
							text += ")";
						}
						list2.Add(new Dictionary<string, object> { { "FixedReward", text } });
						if (Incentive(identifier, customBundleDefinition, list2))
						{
							if (customBundleDefinition.CustomBundleType == CustomizedBundleType.Loop)
							{
								LimitedCustomBundleData initiatedLimitedBundle = GetInitiatedLimitedBundle(customBundleDefinition.Identifier);
								if (initiatedLimitedBundle != null && CustomBundleBoughtBundlesAmount[customBundleDefinition.Identifier] >= customBundleDefinition2.MaxPurchases)
								{
									initiatedLimitedBundle.IsAvailable = true;
									initiatedLimitedBundle.RefreshTime = customBundleDefinition.RefreshTime;
									initiatedLimitedBundle.Timer = customBundleDefinition.RefreshTime;
									initiatedLimitedBundle.IsCanBy = false;
								}
							}
							if (CustomRewards == null)
							{
								return false;
							}
							list.AddRange(CustomRewards[identifier]);
							if (LastCustomReward == null)
							{
								LastCustomReward = new List<IReward>();
							}
							LastCustomReward = list;
							CustomRewards.Remove(identifier);
							base.manager.TdMetrics.SetEventType("custom_bundle").AddProperty("Item_Id", identifier).AddProperty("Rewards", list2)
								.Send();
						}
					}
					return true;
				}
				base.manager.Debug.LogError("customBundle 不存在 Identifier : " + identifier);
				LimitedCustomBundleData initiatedLimitedBundle2 = GetInitiatedLimitedBundle(customBundleDefinition.Identifier);
				if (initiatedLimitedBundle2 != null && InitiatedLimitedBundles.Contains(initiatedLimitedBundle2))
				{
					CustomBundleDefinition customBundleDefinition3 = base.manager.Player.gameEconomyData.GetCustomBundleDefinition(initiatedLimitedBundle2.Identifier);
					if (customBundleDefinition3 != null && !IsBundleAvailableForStore(customBundleDefinition3))
					{
						if (customBundleDefinition3.CustomBundleType == CustomizedBundleType.Loop)
						{
							initiatedLimitedBundle2.Timer = customBundleDefinition3.RefreshTime;
							initiatedLimitedBundle2.RefreshTime = customBundleDefinition3.RefreshTime;
							initiatedLimitedBundle2.IsCanBy = false;
						}
						else if (customBundleDefinition3.CustomBundleType == CustomizedBundleType.TimeBundle)
						{
							initiatedLimitedBundle2.IsCanBy = false;
							initiatedLimitedBundle2.IsAvailable = false;
							InitiatedLimitedBundles.Remove(initiatedLimitedBundle2);
						}
						else
						{
							initiatedLimitedBundle2.IsCanBy = false;
							initiatedLimitedBundle2.IsAvailable = false;
							InitiatedLimitedBundles.Remove(initiatedLimitedBundle2);
						}
					}
				}
			}
			return false;
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
				CustomBundleDefinition customBundleDefinition = base.manager.Player.gameEconomyData.GetCustomBundleDefinition(item.Key);
				if (customBundleDefinition == null)
				{
					if (list == null)
					{
						list = new List<string>();
					}
					list.Add(item.Key);
				}
				else if (customBundleDefinition.RefreshTime <= 0 && customBundleDefinition.HasDateLimit && customBundleDefinition.StartTimeMilliseconds > item.Value)
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
				if (CustomBundleBoughtBundlesAmount.ContainsKey(key))
				{
					CustomBundleBoughtBundlesAmount.Remove(key);
				}
				if (BoughtBundlesLastPurchaseTime.ContainsKey(key))
				{
					BoughtBundlesLastPurchaseTime.Remove(key);
				}
			}
		}

		private bool HasLimitedBundleDefinitionChanged(LimitedCustomBundleData bundleData)
		{
			if (base.manager != null && base.manager.Player != null && bundleData != null && CustomBundleBoughtBundlesAmount.ContainsKey(bundleData.Identifier))
			{
				CustomBundleDefinition customBundleDefinition = base.manager.Player.gameEconomyData.GetCustomBundleDefinition(bundleData.Identifier);
				if (customBundleDefinition.CustomBundleType == CustomizedBundleType.Loop)
				{
					if (customBundleDefinition != null)
					{
						return CustomBundleBoughtBundlesAmount[bundleData.Identifier] >= customBundleDefinition.MaxPurchases;
					}
					return true;
				}
				if (customBundleDefinition.CustomBundleType == CustomizedBundleType.TimeBundle)
				{
					if (customBundleDefinition != null && !(customBundleDefinition.StartTimestamp != bundleData.StartTimestamp))
					{
						return customBundleDefinition.EndTimestamp != bundleData.EndTimestamp;
					}
					return true;
				}
				if (customBundleDefinition != null)
				{
					return CustomBundleBoughtBundlesAmount[bundleData.Identifier] >= customBundleDefinition.MaxPurchases;
				}
				return true;
			}
			return false;
		}

		private void SetupNewLimitedBundle(CustomBundleDefinition bundle, bool skipValidation = false)
		{
			if (skipValidation || (IsBundleAvailableForStore(bundle) && GetInitiatedLimitedBundle(bundle.Identifier) == null))
			{
				LimitedCustomBundleData limitedCustomBundleData = new LimitedCustomBundleData();
				limitedCustomBundleData.Identifier = bundle.Identifier;
				limitedCustomBundleData.IsAvailable = true;
				limitedCustomBundleData.IsCanBy = true;
				long utcTimeStamp = base.manager.Player.UtcTimeStamp;
				if (bundle.CustomBundleType == CustomizedBundleType.Loop)
				{
					limitedCustomBundleData.Timer = 0L;
					limitedCustomBundleData.customType = bundle.CustomBundleType;
				}
				else if (bundle.CustomBundleType == CustomizedBundleType.TimeBundle)
				{
					long endTimeMilliseconds = bundle.EndTimeMilliseconds;
					limitedCustomBundleData.customType = bundle.CustomBundleType;
					limitedCustomBundleData.Timer = Math.Max(0L, endTimeMilliseconds - utcTimeStamp);
				}
				else
				{
					limitedCustomBundleData.customType = bundle.CustomBundleType;
					limitedCustomBundleData.Timer = 0L;
				}
				limitedCustomBundleData.StartTimestamp = bundle.StartTimestamp;
				limitedCustomBundleData.EndTimestamp = bundle.EndTimestamp;
				limitedCustomBundleData.MinTimeFromLastCategoryBought = bundle.MinTimeFromLastCategoryBought;
				InitiatedLimitedBundles.Add(limitedCustomBundleData);
			}
		}

		public LimitedCustomBundleData GetInitiatedLimitedBundle(string bundleID)
		{
			if (InitiatedLimitedBundles == null)
			{
				return null;
			}
			for (int i = 0; i < InitiatedLimitedBundles.Count; i++)
			{
				LimitedCustomBundleData limitedCustomBundleData = InitiatedLimitedBundles[i];
				if (limitedCustomBundleData.Identifier == bundleID)
				{
					return limitedCustomBundleData;
				}
			}
			return null;
		}

		private bool IsBundleAvailableForStore(CustomBundleDefinition bundleStoreEntry)
		{
			if (bundleStoreEntry == null)
			{
				return false;
			}
			PlayerModel player = base.manager.Player;
			GameEconomyData gameEconomyData = base.manager.Player.gameEconomyData;
			if (player != null && gameEconomyData != null)
			{
				if (bundleStoreEntry.SpenderTier != null && bundleStoreEntry.SpenderTier.Count > 0)
				{
					long lifeTimeInDays = player.LifeTimeInDays;
					long secondsSinceLastPurchaseThatCostMoney = GetSecondsSinceLastPurchaseThatCostMoney();
					bool flag = false;
					for (int i = 0; i < bundleStoreEntry.SpenderTier.Count; i++)
					{
						if (gameEconomyData.IsInSpenderTier(player, bundleStoreEntry.SpenderTier[i], player.TotalUSDSpent, (int)lifeTimeInDays, player.GetTotalPurchases(), secondsSinceLastPurchaseThatCostMoney, player.CreationTimeStamp, player.CouncilLevel))
						{
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						return false;
					}
				}
				if (bundleStoreEntry.MaxPurchases >= 0 && (bundleStoreEntry.MaxPurchases == 0 || (CustomBundleBoughtBundlesAmount != null && CustomBundleBoughtBundlesAmount.ContainsKey(bundleStoreEntry.Identifier) && CustomBundleBoughtBundlesAmount[bundleStoreEntry.Identifier] >= bundleStoreEntry.MaxPurchases)))
				{
					return false;
				}
				if (bundleStoreEntry.RefreshTime <= 0 && bundleStoreEntry.HasDateLimit && (player.UtcTimeStamp < bundleStoreEntry.StartTimeMilliseconds || player.UtcTimeStamp > bundleStoreEntry.EndTimeMilliseconds))
				{
					return false;
				}
				return true;
			}
			return false;
		}

		public long GetSecondsSinceLastPurchaseThatCostMoney()
		{
			PlayerModel player = base.manager.Player;
			if (player != null && BoughtBundlesLastPurchaseTime != null)
			{
				long num = 0L;
				foreach (KeyValuePair<string, long> item in BoughtBundlesLastPurchaseTime)
				{
					CustomBundleDefinition customBundleDefinition = player.gameEconomyData.GetCustomBundleDefinition(item.Key);
					if (item.Value > num && !string.IsNullOrEmpty(customBundleDefinition.IAPProduct))
					{
						num = item.Value;
					}
				}
				return (player.UtcTimeStamp - num) / 1000;
			}
			return -1L;
		}

		private bool Incentive(string identifier, CustomBundleDefinition customBundleDefinition, List<Dictionary<string, object>> RewardsMetrics)
		{
			int num = 0;
			foreach (IReward item in CustomRewards[identifier])
			{
				if (item == null)
				{
					num++;
					continue;
				}
				if (item.Type == RewardType.Equipment || item.Type == RewardType.RandomEquipment)
				{
					EquipmentItemModel equipmentItemModel = item.Give(base.manager, new object[1] { base.manager.Player.PlayerRandom }) as EquipmentItemModel;
					RewardEquipment rewardEquipment = item as RewardEquipment;
					if (equipmentItemModel != null)
					{
						base.manager.Metrics.AddFind().AddEquipment(equipmentItemModel, "Equipment", rewardEquipment?.Amount ?? 1).AddCustomBundle(customBundleDefinition)
							.Send();
						string text = "";
						text += equipmentItemModel.EquipmentDefinitionIdentifier;
						text += "(";
						text += ((rewardEquipment == null || rewardEquipment.Amount <= 0) ? new int?(1) : rewardEquipment?.Amount);
						text += ")";
						RewardsMetrics.Add(new Dictionary<string, object> {
						{
							customBundleDefinition.StorageID[num].ToString(),
							text
						} });
					}
				}
				else if (item.Type == RewardType.Outfit)
				{
					string text2 = item.Give(base.manager) as string;
					if (!string.IsNullOrEmpty(text2))
					{
						base.manager.Metrics.AddFind().AddOutfit(base.gameEconomyData.GetOutfitDefinition(text2)).AddCustomBundle(customBundleDefinition)
							.Send();
						string text3 = "";
						text3 += text2;
						text3 += "(";
						text3 += "1";
						text3 += ")";
						RewardsMetrics.Add(new Dictionary<string, object> {
						{
							customBundleDefinition.StorageID[num].ToString(),
							text3
						} });
					}
				}
				else if (item.Type == RewardType.HeroSkin)
				{
					string text4 = item.Give(base.manager) as string;
					if (!string.IsNullOrEmpty(text4))
					{
						RewardHeroSkin rewardHeroSkin = item as RewardHeroSkin;
						base.manager.Metrics.AddFind().AddHeroSkin(rewardHeroSkin).AddCustomBundle(customBundleDefinition)
							.Send();
						string text5 = "";
						text5 += text4;
						text5 += "(";
						text5 += "1";
						text5 += ")";
						RewardsMetrics.Add(new Dictionary<string, object> {
						{
							customBundleDefinition.StorageID[num].ToString(),
							text5
						} });
					}
				}
				else if (item.Type == RewardType.EquipToken)
				{
					if (item.Give(base.manager) is EquipTokenItemModel)
					{
						RewardEquipToken rewardEquipToken = item as RewardEquipToken;
						base.manager.Metrics.AddFind().AddEquipToken(rewardEquipToken).AddCustomBundle(customBundleDefinition)
							.Send();
						string text6 = "";
						text6 += rewardEquipToken.EquipTokenId;
						text6 += "(";
						text6 += rewardEquipToken.RewardAmount;
						text6 += ")";
						RewardsMetrics.Add(new Dictionary<string, object> {
						{
							customBundleDefinition.StorageID[num].ToString(),
							text6
						} });
					}
				}
				else if (item.Type == RewardType.Avatars)
				{
					string text7 = "";
					RewardAvatars rewardAvatars = item as RewardAvatars;
					rewardAvatars.Give(base.manager);
					if (rewardAvatars.Avatar > 0)
					{
						text7 += "avator";
						text7 += "(";
						text7 += rewardAvatars.Avatar;
						text7 += ")";
					}
					else if (rewardAvatars.Border > 0)
					{
						text7 += "borad";
						text7 += "(";
						text7 += rewardAvatars.Border;
						text7 += ")";
					}
					else if (rewardAvatars.Color > 0)
					{
						text7 += "color";
						text7 += "(";
						text7 += rewardAvatars.Color;
						text7 += ")";
					}
					RewardsMetrics.Add(new Dictionary<string, object> {
					{
						customBundleDefinition.StorageID[num].ToString(),
						text7
					} });
				}
				else
				{
					item.Give(base.manager);
					if (item is RewardCurrency)
					{
						RewardCurrency rewardCurrency = item as RewardCurrency;
						if (rewardCurrency.Amount > 0)
						{
							base.manager.Metrics.AddFind().AddResources(rewardCurrency.CurrencyType, rewardCurrency.AmountActuallyAdded, rewardCurrency.GetOverflowAmount()).AddCustomBundle(customBundleDefinition)
								.Send();
							string text8 = "";
							text8 += rewardCurrency.CurrencyType;
							text8 += "(";
							text8 += rewardCurrency.Amount;
							text8 += ")";
							RewardsMetrics.Add(new Dictionary<string, object> {
							{
								customBundleDefinition.StorageID[num].ToString(),
								text8
							} });
						}
					}
				}
				num++;
			}
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

		private void CheckForNewLimitedBundles()
		{
			List<CustomBundleDefinition> orderedAvailableBundlesNew = GetOrderedAvailableBundlesNew();
			for (int i = 0; i < orderedAvailableBundlesNew.Count; i++)
			{
				CustomBundleDefinition bundle = orderedAvailableBundlesNew[i];
				SetupNewLimitedBundle(bundle);
			}
		}

		public List<CustomBundleDefinition> GetOrderedAvailableBundlesNew()
		{
			if (base.manager != null && base.manager.Player != null)
			{
				PlayerModel player = base.manager.Player;
				List<CustomBundleDefinition> list = new List<CustomBundleDefinition>();
				List<CustomBundleDefinition> orderedCustomBundleDefinitions = player.gameEconomyData.GetOrderedCustomBundleDefinitions(player.UtcTimeStamp);
				for (int i = 0; i < orderedCustomBundleDefinitions.Count; i++)
				{
					CustomBundleDefinition customBundleDefinition = orderedCustomBundleDefinitions[i];
					if (CanBuyBundle(customBundleDefinition))
					{
						list.Add(customBundleDefinition);
					}
				}
				return list;
			}
			return null;
		}

		public List<CustomBundleDefinition> GetOrderedAvailableBundles()
		{
			if (base.manager != null && base.manager.Player != null)
			{
				PlayerModel player = base.manager.Player;
				List<CustomBundleDefinition> list = new List<CustomBundleDefinition>();
				List<CustomBundleDefinition> orderedCustomBundleDefinitions = player.gameEconomyData.GetOrderedCustomBundleDefinitions(player.UtcTimeStamp);
				for (int i = 0; i < orderedCustomBundleDefinitions.Count; i++)
				{
					CustomBundleDefinition customBundleDefinition = orderedCustomBundleDefinitions[i];
					if (CanBuyBundle(customBundleDefinition))
					{
						list.Add(customBundleDefinition);
					}
				}
				return list;
			}
			return null;
		}

		public bool CanBuyBundle(CustomBundleDefinition bundleStoreEntry)
		{
			if (bundleStoreEntry == null)
			{
				return false;
			}
			PlayerModel player = base.manager.Player;
			GameEconomyData gameEconomyData = base.manager.Player.gameEconomyData;
			if (player != null && gameEconomyData != null)
			{
				LimitedCustomBundleData initiatedLimitedBundle = GetInitiatedLimitedBundle(bundleStoreEntry.Identifier);
				if (initiatedLimitedBundle != null && !HasLimitedBundleDefinitionChanged(initiatedLimitedBundle) && !initiatedLimitedBundle.IsAvailable)
				{
					base.Debug.Log("Bundle can't be bought because it is during cooldown timer");
					return false;
				}
				if (bundleStoreEntry.CustomBundleType == CustomizedBundleType.Loop && !IsBundleAvailableForStore(bundleStoreEntry) && initiatedLimitedBundle != null)
				{
					initiatedLimitedBundle.IsCanBy = false;
				}
				return IsBundleAvailableForStore(bundleStoreEntry);
			}
			return false;
		}

		private void TickRegisteredLimitedBundles(long deltaTime)
		{
			List<LimitedCustomBundleData> list = null;
			for (int i = 0; i < InitiatedLimitedBundles.Count; i++)
			{
				LimitedCustomBundleData limitedCustomBundleData = InitiatedLimitedBundles[i];
				CustomBundleDefinition customBundleDefinition = base.manager.Player.gameEconomyData.GetCustomBundleDefinition(limitedCustomBundleData.Identifier);
				if ((customBundleDefinition == null || customBundleDefinition.CustomBundleType != CustomizedBundleType.Loop) && (customBundleDefinition == null || !IsBundleAvailableForStore(customBundleDefinition) || HasLimitedBundleDefinitionChanged(limitedCustomBundleData)))
				{
					if (list == null)
					{
						list = new List<LimitedCustomBundleData>();
					}
					list.Add(limitedCustomBundleData);
				}
				else if (customBundleDefinition.CustomBundleType == CustomizedBundleType.TimeBundle)
				{
					long num = customBundleDefinition.EndTimeMilliseconds - base.manager.Player.UtcTimeStamp;
					long timer = limitedCustomBundleData.Timer;
					limitedCustomBundleData.Timer -= deltaTime;
					if (num > 0)
					{
						limitedCustomBundleData.Timer = Math.Min(limitedCustomBundleData.Timer, num);
					}
					if (limitedCustomBundleData.Timer > 0)
					{
						continue;
					}
					limitedCustomBundleData.IsAvailable = !limitedCustomBundleData.IsAvailable;
					if (limitedCustomBundleData.IsAvailable)
					{
						if (customBundleDefinition.HasDateLimit)
						{
							long utcTimeStamp = base.manager.Player.UtcTimeStamp;
							long endTimeMilliseconds = customBundleDefinition.EndTimeMilliseconds;
							limitedCustomBundleData.Timer = Math.Max(0L, endTimeMilliseconds - utcTimeStamp);
						}
						else
						{
							limitedCustomBundleData.Timer = 0L;
						}
					}
					else
					{
						limitedCustomBundleData.Timer = limitedCustomBundleData.MinTimeFromLastCategoryBought * 1000;
						long num2 = deltaTime - timer;
						limitedCustomBundleData.Timer = Math.Max(0L, Math.Min(limitedCustomBundleData.Timer, limitedCustomBundleData.Timer - num2));
					}
				}
				else if (customBundleDefinition.CustomBundleType == CustomizedBundleType.Loop && !limitedCustomBundleData.IsCanBy)
				{
					limitedCustomBundleData.Timer -= deltaTime;
					limitedCustomBundleData.RefreshTime -= deltaTime;
					if (limitedCustomBundleData.RefreshTime < 0 && CustomBundleBoughtBundlesAmount.Count > 0 && CustomBundleBoughtBundlesAmount != null && CustomBundleBoughtBundlesAmount.ContainsKey(limitedCustomBundleData.Identifier) && !limitedCustomBundleData.IsCanBy && CustomBundleBoughtBundlesAmount[limitedCustomBundleData.Identifier] >= customBundleDefinition.MaxPurchases && CustomBundleBoughtBundlesAmount.ContainsKey(limitedCustomBundleData.Identifier))
					{
						limitedCustomBundleData.IsAvailable = true;
						limitedCustomBundleData.IsCanBy = true;
						CustomBundleBoughtBundlesAmount.Remove(limitedCustomBundleData.Identifier);
						limitedCustomBundleData.RefreshTime = 0L;
						limitedCustomBundleData.Timer = 0L;
					}
				}
			}
			if (list != null)
			{
				for (int j = 0; j < list.Count; j++)
				{
					_ = list[j]?.Identifier;
					InitiatedLimitedBundles.Remove(list[j]);
				}
			}
			ResetLimitedBundlesAmountCounter();
		}

		public override bool IsValid()
		{
			return true;
		}

		public bool IsCanPay(CustomBundleDefinition customBundleDefinition)
		{
			List<IReward> selectReward = GetSelectReward(customBundleDefinition.Identifier);
			if (selectReward == null || selectReward.Count <= 0)
			{
				return false;
			}
			int num = 0;
			for (int i = 0; i < selectReward.Count; i++)
			{
				if (selectReward[i] != null)
				{
					num++;
				}
			}
			return num == customBundleDefinition.StorageID.Count;
		}

		public List<IReward> GetSelectReward(string id)
		{
			if (CustomRewards == null)
			{
				return null;
			}
			if (!CustomRewards.TryGetValue(id, out var value))
			{
				return null;
			}
			return value;
		}

		public IReward GetSelectRewardByIndex(string id, int index)
		{
			if (CustomRewards == null)
			{
				return null;
			}
			if (CustomRewards.TryGetValue(id, out var value) && value != null && index >= 0 && index < value.Count)
			{
				return value[index];
			}
			return null;
		}

		public bool CheckTypeEqual(CustomBundleDefinition customBundleDefinition, IReward reward1, IReward reward2, bool isNeedExclusion)
		{
			if (isNeedExclusion && !customBundleDefinition.ExcludeSameItem)
			{
				return false;
			}
			if (reward1.Type == reward2.Type)
			{
				switch (reward1.Type)
				{
				case RewardType.Currency:
					if (reward1 is RewardCurrency rewardCurrency && reward2 is RewardCurrency rewardCurrency2 && rewardCurrency.CurrencyType == rewardCurrency2.CurrencyType)
					{
						return true;
					}
					break;
				case RewardType.Equipment:
					if (reward1 is RewardEquipment rewardEquipment && reward2 is RewardEquipment rewardEquipment2 && rewardEquipment.RarityLevel == rewardEquipment2.RarityLevel && rewardEquipment.EquipmentId.Equals(rewardEquipment2.EquipmentId))
					{
						return true;
					}
					break;
				case RewardType.Loot:
					return true;
				case RewardType.Outfit:
					return true;
				case RewardType.RandomEquipment:
					return true;
				case RewardType.SurvivorClass:
					if (reward1 is RewardSurvivorClass rewardSurvivorClass && reward2 is RewardSurvivorClass rewardSurvivorClass2 && rewardSurvivorClass.SurvivorClass == rewardSurvivorClass2.SurvivorClass)
					{
						return true;
					}
					break;
				case RewardType.SurvivorSlot:
					return true;
				case RewardType.UnlockBuilding:
					return true;
				case RewardType.TimedBonus:
					if (reward1 is RewardTimedBonus rewardTimedBonus && reward2 is RewardTimedBonus rewardTimedBonus2 && rewardTimedBonus.TimedBonusType == rewardTimedBonus2.TimedBonusType)
					{
						return true;
					}
					break;
				case RewardType.SurvivorToken:
					return true;
				case RewardType.TradeCrate:
					return true;
				case RewardType.TraitBonus:
					return true;
				case RewardType.GuildBattleVP:
					return true;
				case RewardType.MissingTokens:
					return true;
				case RewardType.BattlePassPremium:
					return true;
				case RewardType.HeroSkin:
					if (reward1 is RewardHeroSkin rewardHeroSkin && reward2 is RewardHeroSkin rewardHeroSkin2 && rewardHeroSkin.PreferredOrder != null && rewardHeroSkin2.PreferredOrder != null && rewardHeroSkin.PreferredOrder.Count > 0 && rewardHeroSkin2.PreferredOrder.Count > 0 && rewardHeroSkin.PreferredOrder[0].Equals(rewardHeroSkin2.PreferredOrder[0]))
					{
						return true;
					}
					break;
				case RewardType.RewardSkipChallange:
					return true;
				case RewardType.Avatars:
					if (reward1 is RewardAvatars rewardAvatars && reward2 is RewardAvatars rewardAvatars2)
					{
						if (rewardAvatars.Avatar > 0 && rewardAvatars2.Avatar > 0 && rewardAvatars.Avatar == rewardAvatars2.Avatar)
						{
							return true;
						}
						if (rewardAvatars.Border > 0 && rewardAvatars2.Border > 0 && rewardAvatars.Border == rewardAvatars2.Border)
						{
							return true;
						}
						if (rewardAvatars.Color > 0 && rewardAvatars2.Color > 0 && rewardAvatars.Color == rewardAvatars2.Border)
						{
							return true;
						}
					}
					break;
				case RewardType.EquipToken:
					if (reward1 is RewardEquipToken rewardEquipToken && reward2 is RewardEquipToken rewardEquipToken2 && rewardEquipToken.EquipTokenId.Equals(rewardEquipToken2.EquipTokenId))
					{
						return true;
					}
					break;
				case RewardType.SevenDayPremium:
					return true;
				case RewardType.ActiveFoundationPremium:
					return true;
				case RewardType.WeeklySubscription:
					return true;
				case RewardType.MonthlySubscription:
					return true;
				case RewardType.ThreeDayPremium:
					return true;
				}
			}
			return false;
		}
	}
}
