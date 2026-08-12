using System.Collections.Generic;
using System.Linq;
using BaseModel;
using Newtonsoft.Json;

namespace TWDModel
{
	public class SevenDayLoginPeriodModel : TWDModelObject, IActivityManagerIntegrationInterface
	{
		private int CurrentDay = -1;

		public int PeriodId { get; private set; }

		public bool IsUnlockPremium { get; set; }

		public List<int> RemedyDays { get; private set; }

		public ModelList<SevenDayLoginDayItemModel> RewardDays { get; private set; }

		public bool IsCanPopOpenStatus { get; set; }

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
		private bool isInitialized { get; set; }

		[JsonIgnore]
		private CurrencyModel PremiumFlagCurrency => base.manager.Player.GetCurrency(CurrencyType.SevenDayPremium);

		public override bool IsValid()
		{
			return true;
		}

		public SevenDayLoginPeriodModel(int periodId)
		{
			PeriodId = periodId;
		}

		public override void Start()
		{
			base.Start();
			fixNullRewardDays();
			updateNewRewards();
		}

		private void fixNullRewardDays()
		{
			List<SevenDaysRewardDefinition> sevenDaysRewardDefinitionListByPeriod = base.manager.GameEconomyData.GetSevenDaysRewardDefinitionListByPeriod(PeriodId);
			if (sevenDaysRewardDefinitionListByPeriod.Count != 7 || base.manager.Player.SevenDayLoginManager.CurrentPeriodId != PeriodId || RewardDays != null)
			{
				return;
			}
			RewardDays = new ModelList<SevenDayLoginDayItemModel>();
			RewardDays.SetManager(base.manager);
			RewardDays.Initialize();
			foreach (SevenDaysRewardDefinition item in sevenDaysRewardDefinitionListByPeriod)
			{
				SevenDayLoginDayItemModel sevenDayLoginDayItemModel = new SevenDayLoginDayItemModel(item.PeriodId, item.Day);
				sevenDayLoginDayItemModel.SetManager(base.manager);
				sevenDayLoginDayItemModel.Initialize();
				sevenDayLoginDayItemModel.GenerateRewards(item);
				changeDayItemModelStatus(sevenDayLoginDayItemModel);
				sevenDayLoginDayItemModel.Start();
				RewardDays.Add(sevenDayLoginDayItemModel);
			}
		}

		private void updateNewRewards()
		{
			ModelList<SevenDayLoginDayItemModel> rewardDays = RewardDays;
			if (rewardDays == null || rewardDays.Count != 7)
			{
				return;
			}
			List<SevenDaysRewardDefinition> sevenDaysRewardDefinitionListByPeriod = base.manager.GameEconomyData.GetSevenDaysRewardDefinitionListByPeriod(PeriodId);
			if (sevenDaysRewardDefinitionListByPeriod.Count != 7)
			{
				return;
			}
			foreach (SevenDayLoginDayItemModel rewardDay in RewardDays)
			{
				foreach (SevenDaysRewardDefinition item in sevenDaysRewardDefinitionListByPeriod)
				{
					if (rewardDay.Day == item.Day)
					{
						rewardDay.GenerateRewards(item);
					}
				}
			}
		}

		public override void Initialize()
		{
			base.Initialize();
			init();
		}

		private void init()
		{
			if (isInitialized)
			{
				return;
			}
			RemedyDays = new List<int>();
			List<SevenDaysRewardDefinition> sevenDaysRewardDefinitionListByPeriod = base.manager.GameEconomyData.GetSevenDaysRewardDefinitionListByPeriod(PeriodId);
			if (sevenDaysRewardDefinitionListByPeriod.Count != 7)
			{
				return;
			}
			if (RewardDays == null)
			{
				RewardDays = new ModelList<SevenDayLoginDayItemModel>();
				RewardDays.SetManager(base.manager);
				RewardDays.Initialize();
			}
			foreach (SevenDaysRewardDefinition item in sevenDaysRewardDefinitionListByPeriod)
			{
				SevenDayLoginDayItemModel sevenDayLoginDayItemModel = new SevenDayLoginDayItemModel(item.PeriodId, item.Day);
				sevenDayLoginDayItemModel.SetManager(base.manager);
				sevenDayLoginDayItemModel.Initialize();
				sevenDayLoginDayItemModel.GenerateRewards(item);
				changeDayItemModelStatus(sevenDayLoginDayItemModel);
				sevenDayLoginDayItemModel.Start();
				RewardDays.Add(sevenDayLoginDayItemModel);
			}
			isInitialized = true;
		}

		public override void Tick(long deltaTime)
		{
			base.Tick(deltaTime);
			if (base.manager.Player.SevenDayLoginManager != null && base.manager.Player.SevenDayLoginManager.CurrentPeriodId == PeriodId)
			{
				if (!IsUnlockPremium && PremiumFlagCurrency.Value > 0)
				{
					TryUnlockPremium();
				}
				if (base.manager.Player.SevenDayLoginManager?.CanSignInTodayDay != CurrentDay)
				{
					tickUpdateTodayDayStatus();
					CurrentDay = base.manager.Player.SevenDayLoginManager.CanSignInTodayDay;
					NotifyChange("SevenDayLoginChangeToday");
				}
			}
		}

		private void tickUpdateTodayDayStatus()
		{
			if (base.manager.Player.SevenDayLoginManager == null || base.manager.Player.SevenDayLoginManager.CurrentPeriodId != PeriodId)
			{
				return;
			}
			foreach (SevenDayLoginDayItemModel rewardDay in RewardDays)
			{
				changeDayItemModelStatus(rewardDay);
			}
		}

		public void changeDayItemModelStatus(SevenDayLoginDayItemModel dayItemModel)
		{
			if (base.manager.Player.SevenDayLoginManager.CanSignInTodayDay > dayItemModel.Day)
			{
				dayItemModel.DayStatus = SevenDayLoginDayStatus.PastShouldRemedy;
			}
			else if (base.manager.Player.SevenDayLoginManager.CanSignInTodayDay == dayItemModel.Day)
			{
				dayItemModel.DayStatus = SevenDayLoginDayStatus.TodayCanClaim;
			}
			else
			{
				dayItemModel.DayStatus = SevenDayLoginDayStatus.FutureDay;
			}
		}

		public bool TryRemedy(int day)
		{
			SevenDayLoginDayItemModel sevenDayLoginDayItemModel = RewardDays.FirstOrDefault((SevenDayLoginDayItemModel x) => x.Day == day);
			if (sevenDayLoginDayItemModel == null)
			{
				base.Debug.LogError("Can't resign in a non-exist day reward.");
				return false;
			}
			if (sevenDayLoginDayItemModel.DayStatus != SevenDayLoginDayStatus.PastShouldRemedy)
			{
				base.Debug.LogError("Day model status incorrect.");
				return false;
			}
			int remedyCapTimes = getRemedyCapTimes();
			if (RemedyDays.Count >= remedyCapTimes)
			{
				base.Debug.LogError("Remedy limit.");
				return false;
			}
			if (Cashier.CreateOneItemCashier(base.manager, PurchaseType.SevenDayLogin, CurrencyType.Diamonds, base.manager.GameEconomyData.SevenDayConfig.RemedyCostGold).Pay() != TWDModelResult.OK)
			{
				return false;
			}
			sevenDayLoginDayItemModel.HaveRemedied = true;
			if (!RemedyDays.Contains(day))
			{
				RemedyDays.Add(day);
			}
			return true;
		}

		public bool TryClaimReward(int day, SevenDayLoginRewardType rewardType)
		{
			if (!IsUnlockPremium && rewardType == SevenDayLoginRewardType.Premium)
			{
				base.Debug.LogError("Can't claim premium reward.");
				return false;
			}
			SevenDayLoginDayItemModel sevenDayLoginDayItemModel = RewardDays.FirstOrDefault((SevenDayLoginDayItemModel x) => x.Day == day);
			if (sevenDayLoginDayItemModel == null)
			{
				base.Debug.LogError("Can't claim a non-exist day reward.");
				return false;
			}
			bool result = false;
			switch (rewardType)
			{
			case SevenDayLoginRewardType.Free:
				result = sevenDayLoginDayItemModel.TryClaimFreeReward();
				break;
			case SevenDayLoginRewardType.Premium:
				result = sevenDayLoginDayItemModel.TryClaimPremiumReward();
				break;
			}
			return result;
		}

		private int getRemedyCapTimes()
		{
			if (IsUnlockPremium)
			{
				return base.manager.GameEconomyData.SevenDayConfig.RemedyCapPaid;
			}
			return base.manager.GameEconomyData.SevenDayConfig.RemedyCapFree;
		}

		public bool GiveAllSignedInPremiumRewards()
		{
			if (RewardDays == null || RewardDays.Count == 0)
			{
				return true;
			}
			foreach (SevenDayLoginDayItemModel rewardDay in RewardDays)
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
			return true;
		}

		public void TryRetrievePeriodUnclaimedRewards(ref List<IReward> outputSevenDayLoginRewardList)
		{
			if (RewardDays == null || RewardDays.Count == 0)
			{
				return;
			}
			foreach (SevenDayLoginDayItemModel rewardDay in RewardDays)
			{
				if (rewardDay.HaveRemedied && !rewardDay.HaveClaimedFreeReward)
				{
					outputSevenDayLoginRewardList.Add(rewardDay.FreeReward.Reward);
				}
				if (IsUnlockPremium && (rewardDay.HaveRemedied || rewardDay.HaveClaimedFreeReward) && !rewardDay.HaveClaimedPremiumReward)
				{
					outputSevenDayLoginRewardList.Add(rewardDay.PremiumReward.Reward);
				}
			}
		}

		public bool TryUnlockPremium()
		{
			IsUnlockPremium = true;
			return true;
		}

		public Cashier GetRemedyCashier()
		{
			Cashier cashier = new Cashier(base.manager);
			CashierItem cashierItem = new CashierItem(PurchaseType.SevenDayLogin);
			int remedyCostGold = base.gameEconomyData.SevenDayConfig.RemedyCostGold;
			cashierItem.SetCost(CurrencyType.Diamonds, remedyCostGold);
			cashier.AddItem(cashierItem);
			return cashier;
		}

		public bool AreThereAnyUnClaimedRewards()
		{
			foreach (SevenDayLoginDayItemModel rewardDay in RewardDays)
			{
				if (rewardDay.FreeRewardStatus == SevenDayLoginRewardStatus.ReadyToBeClaim)
				{
					return true;
				}
				if (rewardDay.PremiumRewardStatus == SevenDayLoginRewardStatus.ReadyToBeClaim)
				{
					return true;
				}
			}
			return false;
		}

		public string GetIntegrationEventId()
		{
			return "SevenDayLogin";
		}

		public bool CanShowInActivityList()
		{
			if (base.manager == null || base.manager.Player == null)
			{
				return false;
			}
			return base.manager.Player.SevenDayLoginManager?.CurrentPeriodId == PeriodId;
		}

		public bool AreThereAnyUnclaimedReward()
		{
			return AreThereAnyUnClaimedRewards();
		}

		public bool AreThereCanCompleteTask()
		{
			return false;
		}

		public bool IsActivityOpen()
		{
			return IsCanPopOpenStatus;
		}
	}
}
