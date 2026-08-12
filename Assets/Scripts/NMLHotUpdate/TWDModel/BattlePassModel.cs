using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using BaseModel;
using Newtonsoft.Json;

namespace TWDModel
{
	public class BattlePassModel : TWDModelObject
	{
		[Serializable]
		public class TierClaimInfo
		{
			public bool[] FreeRewardsClaimed;

			public bool[] PremiumRewardsClaimed;

			public TierClaimInfo()
			{
			}

			public TierClaimInfo(int freeRewardCount, int premiumRewardCount)
			{
				FreeRewardsClaimed = new bool[freeRewardCount];
				PremiumRewardsClaimed = new bool[premiumRewardCount];
			}
		}

		private class ProcessedRewardDefinition
		{
			public int RequiredBC;

			public Rewards FreeRewards;

			public Rewards PremiumRewards;

			public bool IsPremiumRewardSpecial;

			public bool IsApocalypseFreeReward;

			public bool IsApocalypsePremiumReward;
		}

		public const string SeasonChanged = "SeasonChanged";

		public const string DailyCapRefreshed = "DailyCapRefreshed";

		public const string TierIncreased = "TierIncreased";

		public const string PremiumActivated = "PremiumActivated";

		public const string BonusChestClaimed = "BonusChestClaimed";

		private const long PremiumInfoPopupDailyResetOffsetMilliseconds = 28800000L;

		private BattlePassDataManager battlePassDataManager;

		private IList<ProcessedRewardDefinition> rewardDefinitions;

		public int EarnedFromKillsThisCycle { get; set; }

		public long KillCapExpiryDateMilliseconds { get; set; }

		public int CurrentSeasonId { get; set; }

		public bool PremiumActive { get; set; }

		public int GoldTierUnlockCount { get; set; }

		public TierClaimInfo[] TierClaimInfos { get; set; }

		public int ReachedTier { get; set; }

		public int ClaimedBonusChestCount { get; set; }

		public long LastPremiumInfoPopupViewedTimestamp { get; set; }

		private IBattlePassAnalyticsHandler BattlePassAnalytics => battlePassDataManager.CurrentAnalyticsHandler;

		[JsonIgnore]
		public int MaxDailyBCFromKills => battlePassDataManager.CurrentDataProvider.MaxDailyBCFromKills;

		[JsonIgnore]
		public CurrencyModel BattleCurrency => base.manager.Player.GetCurrency(CurrencyType.BattlePassPoints);

		private CurrencyModel PremiumFlagCurrency => base.manager.Player.GetCurrency(CurrencyType.BattlePassPremium);

		[JsonIgnore]
		public int NextTierBCPrice
		{
			get
			{
				if (!AtMaxTier)
				{
					if (rewardDefinitions == null)
					{
						return 0;
					}
					return rewardDefinitions[ReachedTier + 1].RequiredBC;
				}
				return BonusChestCost;
			}
		}

		[JsonIgnore]
		public int PreviousTierBCPrice
		{
			get
			{
				if (ReachedTier >= 0)
				{
					return rewardDefinitions[ReachedTier].RequiredBC;
				}
				return 0;
			}
		}

		[JsonIgnore]
		public long CurrentSeasonStartDate { get; private set; }

		[JsonIgnore]
		public long CurrentSeasonEndDate { get; private set; }

		[JsonIgnore]
		public long NextSeasonStartDate => battlePassDataManager.CurrentDataProvider?.NextSeasonStartDate ?? long.MaxValue;

		[JsonIgnore]
		public Rewards GrantedUnclaimedRewards { get; private set; }

		[JsonIgnore]
		public Rewards LastClaimedRewards { get; private set; }

		[JsonIgnore]
		public bool IsSeasonActive => CurrentSeasonId > 0;

		[JsonIgnore]
		public bool AtMaxTier => ReachedTier == (rewardDefinitions?.Count ?? 0) - 1;

		[JsonIgnore]
		public int BonusChestCost { get; private set; }

		[JsonIgnore]
		public Cashier CurrentTierUnlockCashier { get; private set; }

		[JsonIgnore]
		public long LastTierIncreaseTimestamp { get; private set; }

		[JsonIgnore]
		public int LastSpecialRewardTier { get; private set; }

		[JsonIgnore]
		private int NextTierGoldPrice
		{
			get
			{
				IBattlePassDataProvider currentDataProvider = battlePassDataManager.CurrentDataProvider;
				if (currentDataProvider == null)
				{
					return 0;
				}
				return currentDataProvider.TierUnlockGoldPrice[Math.Min(GoldTierUnlockCount, currentDataProvider.TierUnlockGoldPrice.Length - 1)];
			}
		}

		[JsonIgnore]
		public bool CanClaimTheBonusChest
		{
			get
			{
				if (BonusChestCost > 0 && AtMaxTier && BattleCurrency.Value >= BonusChestCost)
				{
					return PremiumActive;
				}
				return false;
			}
		}

		[JsonIgnore]
		public bool Unlocked => battlePassDataManager.CurrentDataProvider != null;

		[JsonIgnore]
		public bool PremiumFromSupport { get; private set; }

		[JsonIgnore]
		public bool IsBeginnerBattlePass => CurrentSeasonId == int.MaxValue;

		[JsonIgnore]
		public string TitleColor => battlePassDataManager.CurrentDataProvider.TitleColor;

		[JsonIgnore]
		public string BackgroundColor => battlePassDataManager.CurrentDataProvider.BackgroundColor;

		[JsonIgnore]
		public string BundleIdentifier => battlePassDataManager.CurrentDataProvider.BundleIdentifier;

		[JsonIgnore]
		public string PopupIcon => battlePassDataManager?.CurrentDataProvider?.PopupIcon ?? "";

		[JsonIgnore]
		public string[] PopupIcons
		{
			get
			{
				string popupIcon = PopupIcon;
				if (!string.IsNullOrEmpty(popupIcon))
				{
					return (from icon in popupIcon.Split(new char[1] { ';' }, StringSplitOptions.RemoveEmptyEntries)
						select icon.Trim() into icon
						where !string.IsNullOrEmpty(icon)
						select icon).ToArray();
				}
				return new string[0];
			}
		}

		[JsonIgnore]
		public DropEventDefinition.DropEventType BonusChestDropEventType { get; private set; }

		[JsonIgnore]
		public bool CanShowPremiumInfoPopup
		{
			get
			{
				if (base.manager?.Player != null && IsSeasonActive && !IsBeginnerBattlePass && !PremiumActive)
				{
					return !IsSamePremiumInfoPopupDay(LastPremiumInfoPopupViewedTimestamp, base.manager.Player.UtcTimeStamp);
				}
				return false;
			}
		}

		public int AttemptToEarnCurrencyThroughKill(int killCount)
		{
			RefreshActiveSeason();
			if (!IsSeasonActive)
			{
				return 0;
			}
			CheckAndUpdateCapExpiry();
			IBattlePassDataProvider currentDataProvider = battlePassDataManager.CurrentDataProvider;
			int num = Math.Min(killCount * currentDataProvider.BCPerKill, currentDataProvider.MaxDailyBCFromKills - EarnedFromKillsThisCycle);
			if (num > 0)
			{
				base.manager.Player.GetCurrency(CurrencyType.BattlePassPoints).Add(num);
				EarnedFromKillsThisCycle += num;
				return num;
			}
			return 0;
		}

		public void CheckAndUpdateCapExpiry()
		{
			if (!IsSeasonActive)
			{
				return;
			}
			long utcTimeStamp = base.manager.Player.UtcTimeStamp;
			if (KillCapExpiryDateMilliseconds <= utcTimeStamp)
			{
				IBattlePassDataProvider currentDataProvider = battlePassDataManager.CurrentDataProvider;
				EarnedFromKillsThisCycle = 0;
				DateTime dateTime = DateTime.Parse(currentDataProvider.CapRefreshUTC, CultureInfo.InvariantCulture);
				DateTime utcTime = base.manager.Player.UtcTime;
				DateTime dateTime2 = new DateTime(utcTime.Year, utcTime.Month, utcTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second);
				KillCapExpiryDateMilliseconds = dateTime2.TotalMilliseconds();
				if (KillCapExpiryDateMilliseconds <= utcTimeStamp)
				{
					KillCapExpiryDateMilliseconds += UtilsDateTime.DayInMilliseconds;
				}
				NotifyChange("DailyCapRefreshed");
				BattlePassAnalytics.DailyKillReset();
			}
		}

		public bool MarkPremiumInfoPopupViewed()
		{
			if (!CanShowPremiumInfoPopup)
			{
				return false;
			}
			LastPremiumInfoPopupViewedTimestamp = base.manager.Player.UtcTimeStamp;
			return true;
		}

		private bool IsSamePremiumInfoPopupDay(long previousTimestamp, long currentTimestamp)
		{
			if (previousTimestamp <= 0)
			{
				return false;
			}
			return GetPremiumInfoPopupDay(previousTimestamp) == GetPremiumInfoPopupDay(currentTimestamp);
		}

		private long GetPremiumInfoPopupDay(long timestamp)
		{
			return (timestamp - 28800000) / UtilsDateTime.DayInMilliseconds;
		}

		public void RefreshActiveSeason()
		{
			CurrentSeasonStartDate = 0L;
			CurrentSeasonEndDate = 0L;
			BattlePassSeason battlePassSeason = (battlePassDataManager?.CurrentDataProvider)?.GetCurrentSeason();
			if (battlePassSeason != null)
			{
				long startTimeUtc = battlePassSeason.StartTimeUtc;
				long endTimeUtc = battlePassSeason.EndTimeUtc;
				CurrentSeasonStartDate = startTimeUtc;
				CurrentSeasonEndDate = endTimeUtc;
			}
			if (CurrentSeasonId != (battlePassSeason?.Id ?? 0))
			{
				int currentSeasonId = CurrentSeasonId;
				CurrentSeasonId = battlePassSeason?.Id ?? 0;
				ResetSeason(currentSeasonId);
				BattlePassAnalytics.SeasonChange(currentSeasonId, CurrentSeasonId);
			}
		}

		private void ResetSeason(int oldSeasonId)
		{
			GiveUnclaimedRewards(oldSeasonId);
			EarnedFromKillsThisCycle = 0;
			ClaimedBonusChestCount = 0;
			BattleCurrency.SetValue(0);
			PremiumFlagCurrency.SetValue(0);
			PremiumActive = false;
			GoldTierUnlockCount = 0;
			ReachedTier = -1;
			RefreshRewards();
			TierClaimInfos = rewardDefinitions?.Select((ProcessedRewardDefinition definition) => new TierClaimInfo(definition.FreeRewards.Count, definition.PremiumRewards.Count)).ToArray();
			RegenerateCashier();
			NotifyChange("SeasonChanged");
		}

		private void RefreshRewards()
		{
			BonusChestCost = battlePassDataManager.CurrentDataProvider?.BonusChestCost ?? 0;
			BonusChestDropEventType = battlePassDataManager.CurrentDataProvider?.BonusChestDropType ?? DropEventDefinition.DropEventType.BattlePassCrate;
			rewardDefinitions = ((!IsSeasonActive) ? null : battlePassDataManager.GetRewards(CurrentSeasonId)?.Select((BattlePassRewardDefinition definition) => new ProcessedRewardDefinition
			{
				RequiredBC = definition.RequiredBC,
				FreeRewards = new Rewards(definition.FreeReward ?? ""),
				PremiumRewards = new Rewards(definition.PremiumReward ?? ""),
				IsPremiumRewardSpecial = definition.IsPremiumRewardSpecial,
				IsApocalypseFreeReward = definition.IsApocalypseFreeReward,
				IsApocalypsePremiumReward = definition.IsApocalypsePremiumReward
			}).ToArray());
			LastSpecialRewardTier = -1;
			if (rewardDefinitions == null)
			{
				return;
			}
			for (int num = rewardDefinitions.Count - 1; num >= 0; num--)
			{
				if (rewardDefinitions[num].IsPremiumRewardSpecial)
				{
					LastSpecialRewardTier = num;
					break;
				}
			}
		}

		private void GiveUnclaimedRewards(int oldSeasonId)
		{
			GrantedUnclaimedRewards = new Rewards();
			Rewards rewards = new Rewards();
			List<ClaimRewardAnalyticsEntry> list = new List<ClaimRewardAnalyticsEntry>();
			if (rewardDefinitions != null)
			{
				RefreshTier();
				int num = Math.Min(rewardDefinitions.Count, ReachedTier + 1);
				for (int i = 0; i < num; i++)
				{
					ProcessedRewardDefinition processedRewardDefinition = rewardDefinitions[i];
					for (int j = 0; j < processedRewardDefinition.FreeRewards.Count; j++)
					{
						if (!TierClaimInfos[i].FreeRewardsClaimed[j])
						{
							GrantedUnclaimedRewards.RewardsList.Add(processedRewardDefinition.FreeRewards.GetRewardAt(j));
							list.Add(new ClaimRewardAnalyticsEntry(i, j, isPremium: false, isAutoClaimed: true, oldSeasonId));
						}
					}
					if (!PremiumActive)
					{
						continue;
					}
					for (int k = 0; k < processedRewardDefinition.PremiumRewards.Count; k++)
					{
						if (!TierClaimInfos[i].PremiumRewardsClaimed[k])
						{
							GrantedUnclaimedRewards.RewardsList.Add(processedRewardDefinition.PremiumRewards.GetRewardAt(k));
							list.Add(new ClaimRewardAnalyticsEntry(i, k, isPremium: true, isAutoClaimed: true, oldSeasonId));
						}
					}
					for (int l = 0; l < 100; l++)
					{
						LootEntry lootEntry = ClaimBonusChest(oldSeasonId);
						if (lootEntry == null)
						{
							break;
						}
						if (lootEntry.RewardedCurrency != CurrencyType.None)
						{
							rewards.AddRewardCurrency(lootEntry.RewardedCurrency, lootEntry.RewardedAmount, isDiamondExchange: false, canOverflowMax: false);
						}
						else if (lootEntry.DropCurrencyType == DropCurrenciesProbabilitiesDefinition.DropCurrency.Consumable && lootEntry.RewardedEquipment != null)
						{
							rewards.AddEquipmentConsumableClass(lootEntry.RewardedEquipment.EquipmentDefinitionIdentifier, lootEntry.RewardedAmount);
						}
						else if (lootEntry.RewardedEquipment != null)
						{
							rewards.AddEquipmentClass(lootEntry.RewardedEquipment.EquipmentDefinitionIdentifier, lootEntry.RewardedEquipment.RarityLevel, lootEntry.RewardedEquipment.Level, 0);
						}
					}
				}
			}
			GrantedUnclaimedRewards.Give(base.manager);
			foreach (ClaimRewardAnalyticsEntry item in list)
			{
				BattlePassAnalytics.ClaimReward(item.Tier, item.Index, item.IsPremium, item.IsAutoClaimed, item.OverrideSeasonId);
			}
			GrantedUnclaimedRewards.RewardsList.AddRange(rewards.RewardsList);
		}

		public void RefreshTier()
		{
			while (!AtMaxTier && ReachedTier + 1 < rewardDefinitions.Count && BattleCurrency.Value >= NextTierBCPrice)
			{
				int nextTierBCPrice = NextTierBCPrice;
				ProcessedRewardDefinition processedRewardDefinition = rewardDefinitions[++ReachedTier];
				BattleCurrency.Subtract(processedRewardDefinition.RequiredBC);
				LastTierIncreaseTimestamp = base.manager.Player.UtcTimeStamp;
				NotifyChange("TierIncreased");
				BattlePassAnalytics.AdvanceTier(CurrencyType.BattlePassPoints, nextTierBCPrice);
			}
		}

		public IReward ClaimReward(int tier, bool premium, int index)
		{
			if (ReachedTier < tier || (!PremiumActive && premium))
			{
				return null;
			}
			bool[] array = (premium ? TierClaimInfos[tier].PremiumRewardsClaimed : TierClaimInfos[tier].FreeRewardsClaimed);
			if (array[index])
			{
				return null;
			}
			array[index] = true;
			IReward reward = GetReward(tier, premium, index);
			GiveReward(reward);
			LastClaimedRewards = new Rewards();
			LastClaimedRewards.RewardsList.Add(reward);
			BattlePassAnalytics.ClaimReward(tier, index, premium, auto: false);
			return reward;
		}

		public IReward GetReward(int tier, bool premium, int index)
		{
			if (rewardDefinitions == null)
			{
				return null;
			}
			return (premium ? rewardDefinitions[tier].PremiumRewards : rewardDefinitions[tier].FreeRewards).GetRewardAt(index);
		}

		public bool GetIsPremiumRewardSpecialForTier(int tier)
		{
			return rewardDefinitions[tier].IsPremiumRewardSpecial;
		}

		public bool IsClaimable(int tier, bool premium, int index)
		{
			if (!IsClaimed(tier, premium, index) && (!premium || PremiumActive))
			{
				return tier <= ReachedTier;
			}
			return false;
		}

		public bool IsClaimed(int tier, bool premium, int index)
		{
			return (premium ? TierClaimInfos[tier].PremiumRewardsClaimed : TierClaimInfos[tier].FreeRewardsClaimed)[index];
		}

		public bool BuyNextReward()
		{
			if (AtMaxTier || ReachedTier + 1 >= rewardDefinitions.Count)
			{
				return false;
			}
			int nextTierGoldPrice = NextTierGoldPrice;
			if (CurrentTierUnlockCashier.Pay() != TWDModelResult.OK)
			{
				return false;
			}
			ReachedTier++;
			GoldTierUnlockCount++;
			RegenerateCashier();
			LastTierIncreaseTimestamp = base.manager.Player.UtcTimeStamp;
			NotifyChange("TierIncreased");
			BattlePassAnalytics.AdvanceTier(CurrencyType.Diamonds, nextTierGoldPrice);
			return true;
		}

		public Rewards ActivatePremium(bool fromSupport = false)
		{
			if (PremiumActive)
			{
				return null;
			}
			PremiumActive = true;
			PremiumFlagCurrency.SetValue(1);
			base.manager.Player.GetCurrency(CurrencyType.FreeGuildGiftPerk).Add(1);
			Rewards rewards = new Rewards();
			List<ClaimRewardAnalyticsEntry> list = new List<ClaimRewardAnalyticsEntry>();
			Dictionary<CurrencyType, int> dictionary = new Dictionary<CurrencyType, int>
			{
				{
					CurrencyType.BuildingTokenBP,
					0
				},
				{
					CurrencyType.TrainingTokenBP,
					0
				},
				{
					CurrencyType.EquipmentTokenBP,
					0
				},
				{
					CurrencyType.HealingTokenBP,
					0
				},
				{
					CurrencyType.SuperBuildingTokenBP,
					0
				},
				{
					CurrencyType.SuperTrainingTokenBP,
					0
				},
				{
					CurrencyType.SuperEquipmentTokenBP,
					0
				},
				{
					CurrencyType.BuildingToken1min,
					0
				},
				{
					CurrencyType.BuildingToken5min,
					0
				},
				{
					CurrencyType.BuildingToken10min,
					0
				},
				{
					CurrencyType.BuildingToken30min,
					0
				},
				{
					CurrencyType.BuildingToken1h,
					0
				},
				{
					CurrencyType.BuildingToken6h,
					0
				},
				{
					CurrencyType.BuildingToken12h,
					0
				},
				{
					CurrencyType.BuildingToken24h,
					0
				},
				{
					CurrencyType.TrainingToken5min,
					0
				},
				{
					CurrencyType.TrainingToken20min,
					0
				},
				{
					CurrencyType.TrainingToken1h,
					0
				},
				{
					CurrencyType.TrainingToken3h,
					0
				},
				{
					CurrencyType.TrainingToken8h,
					0
				},
				{
					CurrencyType.TrainingToken16h,
					0
				},
				{
					CurrencyType.EquipmentToken1min,
					0
				},
				{
					CurrencyType.EquipmentToken10min,
					0
				},
				{
					CurrencyType.EquipmentToken20min,
					0
				},
				{
					CurrencyType.EquipmentToken1h,
					0
				},
				{
					CurrencyType.EquipmentToken3h,
					0
				},
				{
					CurrencyType.EquipmentToken7h,
					0
				},
				{
					CurrencyType.EquipmentToken14h,
					0
				},
				{
					CurrencyType.HealingToken1min,
					0
				},
				{
					CurrencyType.HealingToken5min,
					0
				},
				{
					CurrencyType.HealingToken10min,
					0
				},
				{
					CurrencyType.HealingToken1h,
					0
				},
				{
					CurrencyType.HealingToken2h,
					0
				},
				{
					CurrencyType.HealingToken4h,
					0
				}
			};
			for (int i = 0; i <= ReachedTier && i < rewardDefinitions.Count; i++)
			{
				bool flag = true;
				foreach (RewardCurrency allRewardCurrency in rewardDefinitions[i].PremiumRewards.GetAllRewardCurrencies())
				{
					if (base.manager.GameEconomyData.IsSpeedUpTokenCurrencyType(allRewardCurrency.CurrencyType))
					{
						int currencyAmount = base.manager.Player.GetCurrencyAmount(allRewardCurrency.CurrencyType);
						int max = base.manager.Player.GetCurrency(allRewardCurrency.CurrencyType).Max;
						if (currencyAmount + allRewardCurrency.Amount + dictionary[allRewardCurrency.CurrencyType] > max)
						{
							flag = false;
						}
						dictionary[allRewardCurrency.CurrencyType] += allRewardCurrency.Amount;
					}
				}
				if (flag)
				{
					rewards.Add(rewardDefinitions[i].PremiumRewards);
					for (int j = 0; j < TierClaimInfos[i].PremiumRewardsClaimed.Length; j++)
					{
						TierClaimInfos[i].PremiumRewardsClaimed[j] = true;
						list.Add(new ClaimRewardAnalyticsEntry(i, j, isPremium: true, isAutoClaimed: true));
						BattlePassAnalytics.ClaimReward(i, j, premium: true, auto: true);
					}
				}
			}
			if (rewards.Count > 0)
			{
				rewards.Give(base.manager);
			}
			foreach (ClaimRewardAnalyticsEntry item in list)
			{
				BattlePassAnalytics.ClaimReward(item.Tier, item.Index, item.IsPremium, item.IsAutoClaimed);
			}
			LastClaimedRewards = rewards;
			NotifyChange("PremiumActivated");
			BattlePassAnalytics.GainPremium(fromSupport);
			PremiumFromSupport = fromSupport;
			return rewards;
		}

		public Rewards GetAllReachedPremiumRewards()
		{
			Rewards rewards = new Rewards();
			for (int i = 0; i <= ReachedTier && i < rewardDefinitions.Count; i++)
			{
				rewards.Add(rewardDefinitions[i].PremiumRewards);
			}
			return rewards;
		}

		public Rewards GetAllReachedUnclaimedPremiumRewards()
		{
			Rewards rewards = new Rewards();
			for (int i = 0; i <= ReachedTier && i < rewardDefinitions.Count; i++)
			{
				for (int j = 0; j < TierClaimInfos[i].PremiumRewardsClaimed.Length; j++)
				{
					if (!TierClaimInfos[i].PremiumRewardsClaimed[j])
					{
						rewards.Add(rewardDefinitions[i].PremiumRewards);
					}
				}
			}
			return rewards;
		}

		public override bool IsValid()
		{
			return true;
		}

		private void GiveReward(IReward reward)
		{
			if (reward is RandomizedReward)
			{
				reward.Give(base.manager, new object[1] { base.manager.Player.PlayerRandom });
			}
			else
			{
				reward.Give(base.manager);
			}
		}

		public override void Start()
		{
			base.Start();
			base.manager.Player.Camp.GetBuilding("Council").Changed += CouncilOnChanged;
			RefreshRewards();
			RefreshActiveSeason();
			RegenerateCashier();
			CheckAndGrantPremium();
		}

		private void CouncilOnChanged(ModelObject model, string changed, object args)
		{
			if (changed == "level")
			{
				RefreshActiveSeason();
			}
		}

		public LootEntry ClaimBonusChest(int? overrideSeasonId = null)
		{
			if (!CanClaimTheBonusChest)
			{
				return null;
			}
			PlayerModel player = base.manager.Player;
			LootEntry lootEntry = player.LootManager.ShuffleOneLoot(new LootEntryGenParams
			{
				tag = DropEventDefinition.DropEventTag.BonusCrate,
				dropType = DropType.Gold,
				eventType = BonusChestDropEventType,
				targetLevel = player.Level,
				random = player.PlayerRandom
			});
			ClaimedBonusChestCount++;
			lootEntry.Type = LootEntryType.BattlePassBonusChest;
			player.LootManager.GiveLoot(lootEntry);
			BattleCurrency.Subtract(BonusChestCost);
			NotifyChange("BonusChestClaimed");
			BattlePassAnalytics.ClaimBonusChest(lootEntry, CurrencyType.BattlePassPoints, BonusChestCost, overrideSeasonId);
			return lootEntry;
		}

		private void RegenerateCashier()
		{
			CurrentTierUnlockCashier = Cashier.CreateOneItemCashier(base.manager, PurchaseType.BattlePassNextTier, CurrencyType.Diamonds, NextTierGoldPrice);
		}

		private void CheckAndGrantPremium()
		{
			if (PremiumFlagCurrency.Value > 0)
			{
				if (IsSeasonActive)
				{
					if (!PremiumActive)
					{
						ActivatePremium(fromSupport: true);
					}
				}
				else
				{
					PremiumFlagCurrency.SetValue(0);
				}
			}
			else
			{
				PremiumActive = false;
			}
		}

		public void FakeBattlePassSeasonEnd()
		{
			int currentSeasonId = CurrentSeasonId;
			CurrentSeasonId = -999;
			GiveUnclaimedRewards(currentSeasonId);
			NotifyChange("SeasonChanged");
		}

		public override void SetManager(ModelManager manager)
		{
			base.SetManager(manager);
			battlePassDataManager = new BattlePassDataManager((TWDModelManager)manager);
		}

		public bool CanShowApocalypseEffect(int tier, bool IsPremiumReward)
		{
			if (rewardDefinitions == null || tier >= rewardDefinitions.Count)
			{
				return false;
			}
			if (IsPremiumReward)
			{
				return rewardDefinitions[tier].IsApocalypsePremiumReward;
			}
			return rewardDefinitions[tier].IsApocalypseFreeReward;
		}
	}
}
