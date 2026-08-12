using System.Collections.Generic;
using System.Linq;
using BaseModel;
using Newtonsoft.Json;

namespace TWDModel
{
	public class ActiveFoundationPeriodModel : TWDModelObject
	{
		public int PeriodId { get; private set; }

		public int CurrentDay { get; private set; }

		public bool IsUnlockPremium { get; set; }

		public List<int> RemedyDays { get; private set; }

		public ModelList<ActiveFoundationDayItemModel> RewardDays { get; private set; }

		public bool HaveClaimedPremiumExtraRewards { get; private set; }

		public Rewards PremiumExtraRewards { get; private set; }

		[JsonIgnore]
		private bool _isInitialized { get; set; }

		[JsonIgnore]
		public EquipmentItemModel LastRewardedEquipment { get; set; }

		[JsonIgnore]
		private CurrencyModel PremiumFlagCurrency => base.manager.Player.GetCurrency(CurrencyType.ActiveFoundationPremium);

		[JsonIgnore]
		public bool CanRemedy
		{
			get
			{
				int remedyCapTimes = getRemedyCapTimes();
				if (RemedyDays == null)
				{
					return true;
				}
				return remedyCapTimes > RemedyDays.Count;
			}
		}

		[JsonIgnore]
		public string BundleIdentifier => Definition.BundleIdentifier;

		[JsonIgnore]
		public ActiveFoundationDefinition Definition => base.manager.GameEconomyData.GetActiveFoundationDefinition(PeriodId);

		[JsonIgnore]
		public long CurrentPeriodEndTimeUtc => GameEconomyData.ParseDateTime(Definition.EndTimeUtc).TotalMilliseconds();

		[JsonIgnore]
		public Rewards LastClaimedRewards { get; private set; }

		public override bool IsValid()
		{
			return true;
		}

		public ActiveFoundationPeriodModel(int periodId)
		{
			PeriodId = periodId;
		}

		public override void Start()
		{
			base.Start();
			NewRewardDaysOnCreate();
			UpdateRewardsOnStart();
		}

		private void NewRewardDaysOnCreate()
		{
			List<ActiveFoundationRewardDefinition> activeFoundationRewardDefinitionListByPeriod = base.manager.GameEconomyData.GetActiveFoundationRewardDefinitionListByPeriod(PeriodId);
			if (activeFoundationRewardDefinitionListByPeriod.Count <= 0 || base.manager.Player.ActiveFoundationManager.CurrentPeriodId != PeriodId || RewardDays != null)
			{
				return;
			}
			RewardDays = new ModelList<ActiveFoundationDayItemModel>();
			RewardDays.SetManager(base.manager);
			RewardDays.Initialize();
			foreach (ActiveFoundationRewardDefinition item in activeFoundationRewardDefinitionListByPeriod)
			{
				ActiveFoundationDayItemModel activeFoundationDayItemModel = new ActiveFoundationDayItemModel(item.PeriodId, item.Day);
				activeFoundationDayItemModel.SetManager(base.manager);
				activeFoundationDayItemModel.Initialize();
				activeFoundationDayItemModel.GenerateRewards(item);
				ChangeDayItemModelStatus(activeFoundationDayItemModel);
				activeFoundationDayItemModel.Start();
				RewardDays.Add(activeFoundationDayItemModel);
			}
			PremiumExtraRewards = new Rewards(Definition.PremiumExtraRewards, base.manager, base.manager.Player.Level, EquipmentSource.ActiveFoundation, base.manager.Player.PlayerRandom);
		}

		private void UpdateRewardsOnStart()
		{
			ModelList<ActiveFoundationDayItemModel> rewardDays = RewardDays;
			if (rewardDays != null && rewardDays.Count <= 0)
			{
				return;
			}
			List<ActiveFoundationRewardDefinition> activeFoundationRewardDefinitionListByPeriod = base.manager.GameEconomyData.GetActiveFoundationRewardDefinitionListByPeriod(PeriodId);
			if (activeFoundationRewardDefinitionListByPeriod.Count <= 0)
			{
				return;
			}
			foreach (ActiveFoundationDayItemModel rewardDay in RewardDays)
			{
				foreach (ActiveFoundationRewardDefinition item in activeFoundationRewardDefinitionListByPeriod)
				{
					if (rewardDay.Day == item.Day)
					{
						rewardDay.GenerateRewards(item);
					}
				}
			}
			PremiumExtraRewards = new Rewards(Definition.PremiumExtraRewards, base.manager, base.manager.Player.Level, EquipmentSource.ActiveFoundation, base.manager.Player.PlayerRandom);
		}

		public override void Initialize()
		{
			base.Initialize();
			init();
		}

		private void init()
		{
			if (_isInitialized)
			{
				return;
			}
			RemedyDays = new List<int>();
			List<ActiveFoundationRewardDefinition> activeFoundationRewardDefinitionListByPeriod = base.manager.GameEconomyData.GetActiveFoundationRewardDefinitionListByPeriod(PeriodId);
			if (activeFoundationRewardDefinitionListByPeriod.Count <= 0)
			{
				return;
			}
			if (RewardDays == null)
			{
				RewardDays = new ModelList<ActiveFoundationDayItemModel>();
				RewardDays.SetManager(base.manager);
				RewardDays.Initialize();
			}
			foreach (ActiveFoundationRewardDefinition item in activeFoundationRewardDefinitionListByPeriod)
			{
				ActiveFoundationDayItemModel activeFoundationDayItemModel = new ActiveFoundationDayItemModel(item.PeriodId, item.Day);
				activeFoundationDayItemModel.SetManager(base.manager);
				activeFoundationDayItemModel.Initialize();
				activeFoundationDayItemModel.GenerateRewards(item);
				ChangeDayItemModelStatus(activeFoundationDayItemModel);
				activeFoundationDayItemModel.Start();
				RewardDays.Add(activeFoundationDayItemModel);
			}
			_isInitialized = true;
		}

		public override void Tick(long deltaTime)
		{
			base.Tick(deltaTime);
			if (base.manager.Player.ActiveFoundationManager != null && base.manager.Player.ActiveFoundationManager.CurrentPeriodId == PeriodId)
			{
				if (!IsUnlockPremium && PremiumFlagCurrency.Value > 0)
				{
					TryUnlockPremium();
				}
				if (base.manager.Player.ActiveFoundationManager?.CanSignInTodayDay != CurrentDay)
				{
					TickUpdateTodayDayStatus();
					CurrentDay = base.manager.Player.ActiveFoundationManager.CanSignInTodayDay;
					NotifyChange("ActiveFoundationChangeToday");
				}
			}
		}

		private void TickUpdateTodayDayStatus()
		{
			if (base.manager.Player.ActiveFoundationManager == null || base.manager.Player.ActiveFoundationManager.CurrentPeriodId != PeriodId)
			{
				return;
			}
			foreach (ActiveFoundationDayItemModel rewardDay in RewardDays)
			{
				ChangeDayItemModelStatus(rewardDay);
			}
		}

		private void ChangeDayItemModelStatus(ActiveFoundationDayItemModel dayItemModel)
		{
			if (base.manager.Player.ActiveFoundationManager.CanSignInTodayDay > dayItemModel.Day)
			{
				dayItemModel.DayStatus = ActiveFoundationDayStatus.PastShouldRemedy;
			}
			else if (base.manager.Player.ActiveFoundationManager.CanSignInTodayDay == dayItemModel.Day)
			{
				dayItemModel.DayStatus = ActiveFoundationDayStatus.TodayCanClaim;
			}
			else
			{
				dayItemModel.DayStatus = ActiveFoundationDayStatus.FutureDay;
			}
		}

		public bool TryRemedy(int day)
		{
			ActiveFoundationDayItemModel activeFoundationDayItemModel = RewardDays.FirstOrDefault((ActiveFoundationDayItemModel x) => x.Day == day);
			if (activeFoundationDayItemModel == null)
			{
				base.Debug.LogError("Can't resign in a non-exist day reward.");
				return false;
			}
			int remedyCapTimes = getRemedyCapTimes();
			if (RemedyDays.Count >= remedyCapTimes)
			{
				base.Debug.LogError("Remedy limit.");
				return false;
			}
			Cashier remedyCashier = GetRemedyCashier();
			remedyCashier.UsedReason = "ActiveFoundation";
			if (remedyCashier.Pay() != TWDModelResult.OK)
			{
				return false;
			}
			activeFoundationDayItemModel.HaveRemedied = true;
			if (!RemedyDays.Contains(day))
			{
				RemedyDays.Add(day);
			}
			return true;
		}

		public bool TryClaimReward(int day)
		{
			ActiveFoundationDayItemModel activeFoundationDayItemModel = RewardDays.FirstOrDefault((ActiveFoundationDayItemModel x) => x.Day == day);
			if (activeFoundationDayItemModel == null)
			{
				base.Debug.LogError("Can't claim a non-exist day reward.");
				return false;
			}
			Rewards rewards = activeFoundationDayItemModel.TryClaimReward(IsUnlockPremium);
			if (rewards == null)
			{
				return false;
			}
			LastClaimedRewards = rewards;
			return true;
		}

		public bool TryClaimPremiumExtraReward()
		{
			if (!IsUnlockPremium)
			{
				base.Debug.LogError("Can't claim premium extra reward.");
				return false;
			}
			if (HaveClaimedPremiumExtraRewards)
			{
				base.Debug.LogError("Can't claim a claimed premium extra reward.");
				return false;
			}
			return ClaimPremiumExtraReward();
		}

		private bool ClaimPremiumExtraReward()
		{
			if (!IsUnlockPremium)
			{
				return false;
			}
			if (PremiumExtraRewards == null)
			{
				return false;
			}
			List<object> list = PremiumExtraRewards.Give(base.manager);
			HaveClaimedPremiumExtraRewards = true;
			for (int i = 0; i < PremiumExtraRewards.RewardsList.Count; i++)
			{
				base.manager.Metrics.ResourceChangeObtainReason = "ActiveFoundation";
				base.manager.Metrics.AddFind();
				IReward rewardAt = PremiumExtraRewards.GetRewardAt(i);
				if (list?[i] is EquipmentItemModel)
				{
					LastRewardedEquipment = (EquipmentItemModel)list[i];
					base.manager.Metrics.AddEquipment((EquipmentItemModel)list[i], "Equipment", (rewardAt as RewardEquipment)?.Amount ?? 1);
				}
				else if (rewardAt is RewardTimedBonus rewardTimedBonus)
				{
					base.manager.Metrics.AddTimedBonus(rewardTimedBonus);
				}
				else
				{
					base.manager.Metrics.AddReward(rewardAt);
				}
				base.manager.Metrics.AddActiveFoundation(base.manager.Player.ActiveFoundationManager.CurrentPeriodId, base.manager.Player.ActiveFoundationManager.CanSignInTodayDay).Send();
			}
			return true;
		}

		private int getRemedyCapTimes()
		{
			if (IsUnlockPremium)
			{
				return base.manager.GameEconomyData.ActiveFoundationConfig.RemedyCapPaid;
			}
			return base.manager.GameEconomyData.ActiveFoundationConfig.RemedyCapFree;
		}

		public bool GiveAllSignedInPremiumRewards()
		{
			if (RewardDays == null || RewardDays.Count == 0)
			{
				return true;
			}
			foreach (ActiveFoundationDayItemModel rewardDay in RewardDays)
			{
				if (rewardDay.HaveRemedied && !rewardDay.HaveClaimedFreeReward)
				{
					rewardDay.TryClaimPastFreeReward();
				}
				if (IsUnlockPremium && (rewardDay.HaveRemedied || rewardDay.HaveClaimedFreeReward) && !rewardDay.HaveClaimedPremiumReward)
				{
					rewardDay.TryClaimPastPremiumReward();
				}
			}
			if (IsUnlockPremium && !HaveClaimedPremiumExtraRewards)
			{
				return ClaimPremiumExtraReward();
			}
			return true;
		}

		public void TryRetrievePeriodUnclaimedRewards(ref List<IReward> outputActiveFoundationRewardList)
		{
			if (RewardDays == null || RewardDays.Count == 0)
			{
				return;
			}
			foreach (ActiveFoundationDayItemModel rewardDay in RewardDays)
			{
				if (rewardDay.HaveRemedied && !rewardDay.HaveClaimedFreeReward)
				{
					outputActiveFoundationRewardList.AddRange(rewardDay.FreeReward.Rewards.RewardsList);
				}
				if (IsUnlockPremium && (rewardDay.HaveRemedied || rewardDay.HaveClaimedFreeReward) && !rewardDay.HaveClaimedPremiumReward)
				{
					outputActiveFoundationRewardList.AddRange(rewardDay.PremiumReward.Rewards.RewardsList);
				}
			}
			if (IsUnlockPremium && !HaveClaimedPremiumExtraRewards)
			{
				outputActiveFoundationRewardList.AddRange(PremiumExtraRewards.RewardsList);
			}
		}

		public bool TryUnlockPremium()
		{
			IsUnlockPremium = true;
			NotifyChange("ActiveFoundationChangeToday");
			NotifyChange("UnlockedPremiumEvent");
			return true;
		}

		public Cashier GetRemedyCashier()
		{
			Cashier cashier = new Cashier(base.manager);
			CashierItem cashierItem = new CashierItem(PurchaseType.RemedyActiveFoundation);
			int remedyCostGold = base.gameEconomyData.ActiveFoundationConfig.RemedyCostGold;
			cashierItem.SetCost(CurrencyType.Diamonds, remedyCostGold);
			cashier.AddItem(cashierItem);
			return cashier;
		}

		public bool GetIsPremiumRewardSpecialForTier(int tier)
		{
			if (RewardDays == null || RewardDays.Count < tier)
			{
				return false;
			}
			return RewardDays[tier].RewardDefinition.IsPremiumRewardSpecial;
		}

		public IReward GetReward(int tier, bool premium, int index)
		{
			if (RewardDays == null || RewardDays.Count < tier || index < 0 || tier < 0)
			{
				return null;
			}
			if (premium)
			{
				return RewardDays[tier].PremiumReward.Rewards.RewardsList[index];
			}
			return RewardDays[tier].FreeReward.Rewards.RewardsList[index];
		}

		public List<IReward> GetClaimableRewards(int tier)
		{
			if (RewardDays == null || RewardDays.Count < tier)
			{
				return null;
			}
			ActiveFoundationDayItemModel activeFoundationDayItemModel = RewardDays[tier];
			ActiveFoundationRewardItemModel freeReward = activeFoundationDayItemModel.FreeReward;
			ActiveFoundationRewardItemModel premiumReward = activeFoundationDayItemModel.PremiumReward;
			List<IReward> list = new List<IReward>();
			if (!activeFoundationDayItemModel.HaveClaimedFreeReward)
			{
				list.AddRange(freeReward.Rewards.RewardsList);
			}
			if (IsUnlockPremium && !activeFoundationDayItemModel.HaveClaimedPremiumReward)
			{
				list.AddRange(premiumReward.Rewards.RewardsList);
			}
			return list;
		}

		public int GetRewardCount(int tier, bool premium)
		{
			if (RewardDays == null || RewardDays.Count < tier || tier < 0)
			{
				return 0;
			}
			if (premium)
			{
				return RewardDays[tier].PremiumReward.Rewards.RewardsList.Count;
			}
			return RewardDays[tier].FreeReward.Rewards.RewardsList.Count;
		}

		public ActiveFoundationRewardStatus GetRewardStatus(int tier, bool premium)
		{
			if (RewardDays == null || RewardDays.Count < tier || tier < 0)
			{
				return ActiveFoundationRewardStatus.Normal;
			}
			if (!premium)
			{
				return RewardDays[tier].FreeRewardStatus;
			}
			return RewardDays[tier].PremiumRewardStatus;
		}

		public bool IsClaimable(int tier, bool premium)
		{
			return GetRewardStatus(tier, premium) == ActiveFoundationRewardStatus.ReadyToBeClaim;
		}

		public bool IsRemedyable(int tier, bool premium)
		{
			return GetRewardStatus(tier, premium) == ActiveFoundationRewardStatus.ReadyToBeRemedy;
		}

		public bool CanShowApocalypseEffect(int tier, bool IsPremiumReward)
		{
			if (RewardDays == null || RewardDays.Count < tier || tier < 0)
			{
				return false;
			}
			if (IsPremiumReward)
			{
				return RewardDays[tier].RewardDefinition.IsApocalypsePremiumReward;
			}
			return RewardDays[tier].RewardDefinition.IsApocalypseFreeReward;
		}

		public bool IsHaveSomeRewardCanClaim()
		{
			if (RewardDays == null || RewardDays.Count < 0)
			{
				return false;
			}
			bool result = false;
			foreach (ActiveFoundationDayItemModel rewardDay in RewardDays)
			{
				if (rewardDay.DayStatus == ActiveFoundationDayStatus.FutureDay)
				{
					continue;
				}
				if (rewardDay.DayStatus == ActiveFoundationDayStatus.TodayCanClaim)
				{
					if (!rewardDay.HaveClaimedFreeReward)
					{
						result = true;
						break;
					}
					if (IsUnlockPremium && !rewardDay.HaveClaimedPremiumReward)
					{
						result = true;
						break;
					}
				}
				if (rewardDay.DayStatus == ActiveFoundationDayStatus.PastShouldRemedy)
				{
					if (!rewardDay.HaveClaimedFreeReward && rewardDay.HaveRemedied)
					{
						result = true;
						break;
					}
					if (IsUnlockPremium && !rewardDay.HaveClaimedPremiumReward && rewardDay.HaveRemedied)
					{
						result = true;
						break;
					}
				}
			}
			if (IsUnlockPremium && !HaveClaimedPremiumExtraRewards)
			{
				result = true;
			}
			return result;
		}
	}
}
