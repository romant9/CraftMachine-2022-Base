using System.Collections.Generic;
using Newtonsoft.Json;

namespace TWDModel
{
	public class ActiveFoundationDayItemModel : TWDModelObject
	{
		[JsonIgnore]
		public ActiveFoundationDayStatus DayStatus { get; set; }

		[JsonIgnore]
		public ActiveFoundationRewardStatus FreeRewardStatus
		{
			get
			{
				if (HaveClaimedFreeReward)
				{
					return ActiveFoundationRewardStatus.Claimed;
				}
				switch (DayStatus)
				{
				case ActiveFoundationDayStatus.PastShouldRemedy:
					if (!HaveRemedied)
					{
						return ActiveFoundationRewardStatus.ReadyToBeRemedy;
					}
					return ActiveFoundationRewardStatus.ReadyToBeClaim;
				case ActiveFoundationDayStatus.TodayCanClaim:
					return ActiveFoundationRewardStatus.ReadyToBeClaim;
				case ActiveFoundationDayStatus.FutureDay:
					return ActiveFoundationRewardStatus.Normal;
				default:
					return ActiveFoundationRewardStatus.Normal;
				}
			}
		}

		[JsonIgnore]
		public ActiveFoundationRewardStatus PremiumRewardStatus
		{
			get
			{
				ActiveFoundationPeriodModel currentPeriodModel = base.manager.Player.ActiveFoundationManager.CurrentPeriodModel;
				if (currentPeriodModel == null)
				{
					return ActiveFoundationRewardStatus.Normal;
				}
				if (currentPeriodModel.IsUnlockPremium)
				{
					if (HaveClaimedPremiumReward)
					{
						return ActiveFoundationRewardStatus.Claimed;
					}
					return FreeRewardStatus switch
					{
						ActiveFoundationRewardStatus.Normal => ActiveFoundationRewardStatus.Normal, 
						ActiveFoundationRewardStatus.ReadyToBeClaim => ActiveFoundationRewardStatus.ReadyToBeClaim, 
						ActiveFoundationRewardStatus.ReadyToBeRemedy => ActiveFoundationRewardStatus.Normal, 
						ActiveFoundationRewardStatus.Claimed => ActiveFoundationRewardStatus.ReadyToBeClaim, 
						_ => ActiveFoundationRewardStatus.Normal, 
					};
				}
				return ActiveFoundationRewardStatus.Lock;
			}
		}

		public bool HaveRemedied { get; set; }

		public bool HaveClaimedFreeReward { get; set; }

		public bool HaveClaimedPremiumReward { get; private set; }

		public int PeriodId { get; private set; }

		public int Day { get; private set; }

		public ActiveFoundationRewardItemModel FreeReward { get; private set; }

		public ActiveFoundationRewardItemModel PremiumReward { get; private set; }

		[JsonIgnore]
		public ActiveFoundationRewardDefinition RewardDefinition => base.manager.GameEconomyData.GetActiveFoundationRewardDefinitionByPeriodDay(PeriodId, Day);

		public ActiveFoundationDayItemModel(int periodId, int day)
		{
			PeriodId = periodId;
			Day = day;
		}

		public override bool IsValid()
		{
			return true;
		}

		public void GenerateRewards(ActiveFoundationRewardDefinition activeFoundationRewardDefinition)
		{
			if (!string.IsNullOrEmpty(activeFoundationRewardDefinition.FreeReward))
			{
				FreeReward = new ActiveFoundationRewardItemModel(ActiveFoundationRewardType.Free);
				FreeReward.SetManager(base.manager);
				FreeReward.Initialize();
				FreeReward.GenerateRewards(activeFoundationRewardDefinition.FreeReward);
			}
			if (!string.IsNullOrEmpty(activeFoundationRewardDefinition.PremiumReward))
			{
				PremiumReward = new ActiveFoundationRewardItemModel(ActiveFoundationRewardType.Premium);
				PremiumReward.SetManager(base.manager);
				PremiumReward.Initialize();
				PremiumReward.GenerateRewards(activeFoundationRewardDefinition.PremiumReward);
			}
		}

		public Rewards TryClaimReward(bool isUnlockPremium)
		{
			if (DayStatus != ActiveFoundationDayStatus.TodayCanClaim && (DayStatus != ActiveFoundationDayStatus.PastShouldRemedy || (!HaveRemedied && !HaveClaimedFreeReward)) && (DayStatus != ActiveFoundationDayStatus.FutureDay || Day != base.manager.Player.ActiveFoundationManager.CanSignInTodayDay))
			{
				base.Debug.LogError($"Trying to claim reward '{base.ModelId}' but can not claim, HaveRemedied: {HaveRemedied.ToString()}, DayStatus: {DayStatus.ToString()}.");
				return null;
			}
			ActiveFoundationRewardItemModel freeReward = FreeReward;
			ActiveFoundationRewardItemModel premiumReward = PremiumReward;
			if (freeReward == null || premiumReward == null)
			{
				base.Debug.LogError($"Trying to claim free or premium reward '{base.ModelId}' not in the reward list.");
				return null;
			}
			if (HaveClaimedFreeReward && HaveClaimedPremiumReward)
			{
				base.Debug.LogError($"Trying to claim free and premium reward '{base.ModelId}', but already claimed.");
				return null;
			}
			Rewards rewards = new Rewards();
			if (!HaveClaimedFreeReward)
			{
				rewards.RewardsList.AddRange(freeReward.Rewards.RewardsList);
			}
			if (isUnlockPremium && !HaveClaimedPremiumReward)
			{
				rewards.RewardsList.AddRange(premiumReward.Rewards.RewardsList);
			}
			if (rewards.Count == 0)
			{
				base.Debug.LogError($"Trying to claim free and premium reward '{base.ModelId}', but reward is empty.");
				return null;
			}
			ClaimTodayReward(rewards);
			if (!HaveClaimedFreeReward)
			{
				HaveClaimedFreeReward = true;
			}
			if (isUnlockPremium && !HaveClaimedPremiumReward)
			{
				HaveClaimedPremiumReward = true;
			}
			return rewards;
		}

		private bool ClaimTodayReward(Rewards waitClaimRewards)
		{
			if (waitClaimRewards == null)
			{
				return false;
			}
			List<object> list = waitClaimRewards.Give(base.manager);
			for (int i = 0; i < waitClaimRewards.RewardsList.Count; i++)
			{
				base.manager.Metrics.ResourceChangeObtainReason = "ActiveFoundation";
				base.manager.Metrics.AddFind();
				IReward rewardAt = waitClaimRewards.GetRewardAt(i);
				if (list?[i] is EquipmentItemModel)
				{
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

		public bool TryClaimPastFreeReward()
		{
			ActiveFoundationRewardItemModel freeReward = FreeReward;
			if (freeReward == null)
			{
				base.Debug.LogError($"Trying to claim past free reward '{base.ModelId}' not in the reward list.");
				return false;
			}
			if (HaveClaimedFreeReward)
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
			ActiveFoundationRewardItemModel premiumReward = PremiumReward;
			if (premiumReward == null)
			{
				base.Debug.LogError($"Trying to claim past premium reward '{base.ModelId}' not in the reward list.");
				return false;
			}
			if (HaveClaimedPremiumReward)
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
