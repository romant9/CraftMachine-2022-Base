using Newtonsoft.Json;

namespace TWDModel
{
	public class SevenDayLoginDayItemModel : TWDModelObject
	{
		[JsonIgnore]
		public SevenDayLoginDayStatus DayStatus { get; set; }

		[JsonIgnore]
		public SevenDayLoginRewardStatus FreeRewardStatus
		{
			get
			{
				if (HaveClaimedFreeReward)
				{
					return SevenDayLoginRewardStatus.Claimed;
				}
				switch (DayStatus)
				{
				case SevenDayLoginDayStatus.PastShouldRemedy:
					if (!HaveRemedied)
					{
						return SevenDayLoginRewardStatus.ReadyToBeRemedy;
					}
					return SevenDayLoginRewardStatus.ReadyToBeClaim;
				case SevenDayLoginDayStatus.TodayCanClaim:
					return SevenDayLoginRewardStatus.ReadyToBeClaim;
				case SevenDayLoginDayStatus.FutureDay:
					return SevenDayLoginRewardStatus.Normal;
				default:
					return SevenDayLoginRewardStatus.Normal;
				}
			}
		}

		[JsonIgnore]
		public SevenDayLoginRewardStatus PremiumRewardStatus
		{
			get
			{
				SevenDayLoginPeriodModel currentPeriodModel = base.manager.Player.SevenDayLoginManager.CurrentPeriodModel;
				if (currentPeriodModel == null)
				{
					return SevenDayLoginRewardStatus.Normal;
				}
				if (currentPeriodModel.IsUnlockPremium)
				{
					if (HaveClaimedPremiumReward)
					{
						return SevenDayLoginRewardStatus.Claimed;
					}
					return FreeRewardStatus switch
					{
						SevenDayLoginRewardStatus.Normal => SevenDayLoginRewardStatus.Normal, 
						SevenDayLoginRewardStatus.ReadyToBeClaim => SevenDayLoginRewardStatus.ReadyToBeClaim, 
						SevenDayLoginRewardStatus.ReadyToBeRemedy => SevenDayLoginRewardStatus.Normal, 
						SevenDayLoginRewardStatus.Claimed => SevenDayLoginRewardStatus.ReadyToBeClaim, 
						_ => SevenDayLoginRewardStatus.Normal, 
					};
				}
				return SevenDayLoginRewardStatus.Lock;
			}
		}

		public bool HaveRemedied { get; set; }

		public bool HaveClaimedFreeReward { get; set; }

		public bool HaveClaimedPremiumReward { get; private set; }

		public int PeriodId { get; private set; }

		public int Day { get; private set; }

		public SevenDayLoginRewardItemModel FreeReward { get; private set; }

		public SevenDayLoginRewardItemModel PremiumReward { get; private set; }

		[JsonIgnore]
		public SevenDaysRewardDefinition RewardDefinition => base.manager.GameEconomyData.GetSevenDaysRewardDefinitionByPeriodDay(PeriodId, Day);

		public SevenDayLoginDayItemModel(int periodId, int day)
		{
			PeriodId = periodId;
			Day = day;
		}

		public override void Tick(long deltaTime)
		{
			base.Tick(deltaTime);
		}

		public override void Initialize()
		{
			base.Initialize();
		}

		public override bool IsValid()
		{
			return true;
		}

		public void GenerateRewards(SevenDaysRewardDefinition sevenDaysRewardDefinition)
		{
			if (!string.IsNullOrEmpty(sevenDaysRewardDefinition.FreeReward))
			{
				FreeReward = new SevenDayLoginRewardItemModel(SevenDayLoginRewardType.Free);
				FreeReward.SetManager(base.manager);
				FreeReward.Initialize();
				FreeReward.GenerateRewards(sevenDaysRewardDefinition.FreeReward);
			}
			if (!string.IsNullOrEmpty(sevenDaysRewardDefinition.PremiumReward))
			{
				PremiumReward = new SevenDayLoginRewardItemModel(SevenDayLoginRewardType.Premium);
				PremiumReward.SetManager(base.manager);
				PremiumReward.Initialize();
				PremiumReward.GenerateRewards(sevenDaysRewardDefinition.PremiumReward);
			}
		}

		public bool TryClaimFreeReward()
		{
			if (DayStatus != SevenDayLoginDayStatus.TodayCanClaim && (DayStatus != SevenDayLoginDayStatus.PastShouldRemedy || !HaveRemedied) && (DayStatus != SevenDayLoginDayStatus.FutureDay || Day != base.manager.Player.SevenDayLoginManager.CanSignInTodayDay))
			{
				base.Debug.LogError($"Trying to claim free reward '{base.ModelId}' but can not claim, HaveRemedied: {HaveRemedied.ToString()}, DayStatus: {DayStatus.ToString()}.");
				return false;
			}
			SevenDayLoginRewardItemModel freeReward = FreeReward;
			if (freeReward == null)
			{
				base.Debug.LogError($"Trying to claim free reward '{base.ModelId}' not in the reward list.");
				return false;
			}
			if (freeReward.Claimed)
			{
				base.Debug.LogError($"Trying to claim free reward '{base.ModelId}' but already claimed.");
				return true;
			}
			if (!freeReward.ClaimReward())
			{
				base.Debug.LogError($"Failed claiming free reward '{base.ModelId}'.");
				return false;
			}
			HaveClaimedFreeReward = true;
			return true;
		}

		public bool TryClaimPremiumReward()
		{
			if (DayStatus != SevenDayLoginDayStatus.TodayCanClaim && (DayStatus != SevenDayLoginDayStatus.PastShouldRemedy || (!HaveRemedied && !HaveClaimedFreeReward)) && (DayStatus != SevenDayLoginDayStatus.FutureDay || Day != base.manager.Player.SevenDayLoginManager.CanSignInTodayDay))
			{
				base.Debug.LogError($"Trying to claim reward '{base.ModelId}' but can not claim, HaveRemedied: {HaveRemedied.ToString()}, DayStatus: {DayStatus.ToString()}.");
				return false;
			}
			SevenDayLoginRewardItemModel premiumReward = PremiumReward;
			if (premiumReward == null)
			{
				base.Debug.LogError($"Trying to claim premium reward '{base.ModelId}' not in the reward list.");
				return false;
			}
			if (premiumReward.Claimed)
			{
				base.Debug.LogError($"Trying to claim premium reward '{base.ModelId}' but already claimed.");
				return true;
			}
			if (!premiumReward.ClaimReward())
			{
				base.Debug.LogError($"Failed claiming premium reward '{base.ModelId}'.");
				return false;
			}
			HaveClaimedPremiumReward = true;
			return true;
		}

		public bool TryClaimPastFreeReward()
		{
			SevenDayLoginRewardItemModel freeReward = FreeReward;
			if (freeReward == null)
			{
				base.Debug.LogError($"Trying to claim past free reward '{base.ModelId}' not in the reward list.");
				return false;
			}
			if (freeReward.Claimed)
			{
				base.Debug.LogError($"Trying to claim past free reward '{base.ModelId}' but already claimed.");
				return true;
			}
			if (!freeReward.ClaimReward())
			{
				base.Debug.LogError($"Failed claiming past free reward '{base.ModelId}'.");
				return false;
			}
			HaveClaimedFreeReward = true;
			return true;
		}

		public bool TryClaimPastPremiumReward()
		{
			SevenDayLoginRewardItemModel premiumReward = PremiumReward;
			if (premiumReward == null)
			{
				base.Debug.LogError($"Trying to claim past premium reward '{base.ModelId}' not in the reward list.");
				return false;
			}
			if (premiumReward.Claimed)
			{
				base.Debug.LogError($"Trying to claim past premium reward '{base.ModelId}' but already claimed.");
				return true;
			}
			if (!premiumReward.ClaimReward())
			{
				base.Debug.LogError($"Failed claiming past premium reward '{base.ModelId}'.");
				return false;
			}
			HaveClaimedPremiumReward = true;
			return true;
		}
	}
}
