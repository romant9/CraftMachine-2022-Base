using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace TWDModel
{
	public class ThreeDayModel : TWDModelObject, IActivityManagerIntegrationInterface
	{
		public int Id { get; set; }

		public List<ThreeDayRewardStatus> RewardsStatus { get; set; }

		public string CurSpendtier { get; set; }

		public long LastUpdateRewardTime { get; set; }

		[JsonIgnore]
		public ThreeDayDefinition CurrentDefinition => base.manager.GameEconomyData.GetThreeDayDefinition(Id);

		public Dictionary<int, List<int>> NeedPopReward { get; set; }

		public Dictionary<int, string> NeedPopRewardSpandier { get; set; }

		public long NoPopTime { get; set; }

		[JsonIgnore]
		public bool HasBuy => LastUpdateRewardTime > 0;

		[JsonIgnore]
		public bool CanShowThreeDay
		{
			get
			{
				if (Id > 0 && !string.IsNullOrEmpty(CurSpendtier))
				{
					return base.manager.GameEconomyData.ConfigData.ThreeDaySwich;
				}
				return false;
			}
		}

		[JsonIgnore]
		public bool CanPopup
		{
			get
			{
				if (CanShowThreeDay)
				{
					return base.manager.Player.UtcTimeStamp > NoPopTime;
				}
				return false;
			}
		}

		[JsonIgnore]
		public List<Rewards> CurrentReward
		{
			get
			{
				if (CurSpendtier == CurrentDefinition.spendetier1)
				{
					return CurrentDefinition.RewardEntries1;
				}
				if (CurSpendtier == CurrentDefinition.spendetier2)
				{
					return CurrentDefinition.RewardEntries2;
				}
				return new List<Rewards>();
			}
		}

		public bool IsCanPopOpenStatus { get; set; }

		public void SetNoPopTime()
		{
			if (CurrentDefinition != null)
			{
				NoPopTime = base.manager.Player.UtcTimeStamp + CurrentDefinition.refresh;
			}
		}

		public void ClearRewardPop()
		{
			if (NeedPopReward != null)
			{
				NeedPopReward.Clear();
				NeedPopRewardSpandier.Clear();
			}
		}

		public List<IReward> GetNeedPopReward()
		{
			List<IReward> ret = new List<IReward>();
			if (NeedPopReward == null)
			{
				return ret;
			}
			foreach (KeyValuePair<int, List<int>> item in NeedPopReward)
			{
				ThreeDayDefinition definition = base.manager.GameEconomyData.GetThreeDayDefinition(item.Key);
				if (NeedPopRewardSpandier[item.Key] == definition.spendetier1)
				{
					item.Value.ForEach(delegate(int x)
					{
						ret.AddRange(definition.RewardEntries1[x].RewardsList);
					});
				}
				else if (NeedPopRewardSpandier[item.Key] == definition.spendetier2)
				{
					item.Value.ForEach(delegate(int x)
					{
						ret.AddRange(definition.RewardEntries2[x].RewardsList);
					});
				}
			}
			return ret;
		}

		public override bool IsValid()
		{
			return true;
		}

		public override void Initialize()
		{
			base.Initialize();
			Id = -1;
			RewardsStatus = new List<ThreeDayRewardStatus>();
			CurSpendtier = "";
			LastUpdateRewardTime = 0L;
		}

		public void RewardIndex(int index)
		{
			List<Rewards> currentReward = CurrentReward;
			if (currentReward.Count > index && RewardsStatus.Count > index)
			{
				currentReward[index].Give(base.manager);
				RewardsStatus[index] = ThreeDayRewardStatus.Rewarded;
				base.Debug.LogInfo($"ThreeDayModel Rewarded Id :{Id},CurSpendtier:{CurSpendtier},Index:{index}");
				return;
			}
			base.Debug.LogError($"ThreeDayModel RewardError Id :{Id},CurSpendtier:{CurSpendtier},Index:{index},RewardsCount:{currentReward.Count},StatusCount:{RewardsStatus.Count}");
		}

		public override void Tick(long deltaTime)
		{
			base.Tick(deltaTime);
			long utcTimeStamp = base.manager.Player.UtcTimeStamp;
			if (CurrentDefinition != null && CurrentDefinition.EndTimeMilliseconds > utcTimeStamp)
			{
				if (LastUpdateRewardTime > 0 && LastUpdateRewardTime + CurrentDefinition.refresh <= utcTimeStamp)
				{
					int num = RewardsStatus.IndexOf(ThreeDayRewardStatus.Lock);
					if (num >= 0)
					{
						RewardsStatus[num] = ThreeDayRewardStatus.Unlock;
					}
					LastUpdateRewardTime = utcTimeStamp;
					base.Debug.LogInfo($"ThreeDayModel Unlock Id :{Id},CurSpendtier:{CurSpendtier},now:{utcTimeStamp},Index:{num}");
				}
				return;
			}
			if (CurrentDefinition != null)
			{
				int num2 = RewardsStatus.IndexOf(ThreeDayRewardStatus.Unlock);
				if (num2 >= 0)
				{
					if (NeedPopReward == null)
					{
						NeedPopReward = new Dictionary<int, List<int>>();
						NeedPopRewardSpandier = new Dictionary<int, string>();
					}
					NeedPopReward[Id] = new List<int>();
					NeedPopRewardSpandier[Id] = CurSpendtier;
				}
				while (num2 >= 0)
				{
					RewardIndex(num2);
					NeedPopReward[Id].Add(num2);
					num2 = RewardsStatus.IndexOf(ThreeDayRewardStatus.Unlock);
				}
				base.Debug.LogInfo($"ThreeDayModel Ended Id :{Id},CurSpendtier:{CurSpendtier},now:{utcTimeStamp}");
				Id = -1;
				RewardsStatus.Clear();
				CurSpendtier = "";
				LastUpdateRewardTime = 0L;
			}
			ThreeDayDefinition curOpenedThreeDayDefinition = base.manager.GameEconomyData.GetCurOpenedThreeDayDefinition(utcTimeStamp);
			if (curOpenedThreeDayDefinition == null)
			{
				return;
			}
			Id = curOpenedThreeDayDefinition.Id;
			CurSpendtier = GetOpenedSpendertier(curOpenedThreeDayDefinition);
			IsCanPopOpenStatus = true;
			if (!string.IsNullOrEmpty(CurSpendtier))
			{
				if (CurSpendtier == CurrentDefinition.spendetier1)
				{
					RewardsStatus.AddRange(CurrentDefinition.RewardEntries1.Select((Rewards x) => ThreeDayRewardStatus.Lock));
				}
				else if (CurSpendtier == CurrentDefinition.spendetier2)
				{
					RewardsStatus.AddRange(CurrentDefinition.RewardEntries2.Select((Rewards x) => ThreeDayRewardStatus.Lock));
				}
			}
			base.Debug.LogInfo($"ThreeDayModel Started Id :{Id},CurSpendtier:{CurSpendtier},now:{utcTimeStamp}");
		}

		public bool OnBuyBundle()
		{
			if (CurrentDefinition != null && !string.IsNullOrEmpty(CurSpendtier))
			{
				LastUpdateRewardTime = base.manager.Player.UtcTimeStamp;
				RewardsStatus[0] = ThreeDayRewardStatus.Unlock;
				base.Debug.LogInfo($"ThreeDayModel OnBuyBundle Id :{Id},CurSpendtier:{CurSpendtier}");
				return true;
			}
			return false;
		}

		public string GetOpenedSpendertier(ThreeDayDefinition threeDayDefinition)
		{
			PlayerModel player = base.manager.Player;
			long secondsSinceLastPurchaseThatCostMoney = player.BundleManager.GetSecondsSinceLastPurchaseThatCostMoney();
			if (!string.IsNullOrEmpty(threeDayDefinition.spendetier1) && base.manager.GameEconomyData.IsInSpenderTier(player, threeDayDefinition.spendetier1, player.TotalUSDSpent, (int)player.LifeTimeInDays, player.GetTotalPurchases(), secondsSinceLastPurchaseThatCostMoney, player.CreationTimeStamp, player.CouncilLevel))
			{
				return threeDayDefinition.spendetier1;
			}
			if (!string.IsNullOrEmpty(threeDayDefinition.spendetier2) && base.manager.GameEconomyData.IsInSpenderTier(player, threeDayDefinition.spendetier2, player.TotalUSDSpent, (int)player.LifeTimeInDays, player.GetTotalPurchases(), secondsSinceLastPurchaseThatCostMoney, player.CreationTimeStamp, player.CouncilLevel))
			{
				return threeDayDefinition.spendetier2;
			}
			return "";
		}

		public string GetIntegrationEventId()
		{
			return "ThreeDayMission";
		}

		public bool CanShowInActivityList()
		{
			return CanShowThreeDay;
		}

		public bool AreThereAnyUnclaimedReward()
		{
			if (!CanShowThreeDay)
			{
				return false;
			}
			if (RewardsStatus == null || RewardsStatus.Count <= 0)
			{
				return false;
			}
			bool result = false;
			foreach (ThreeDayRewardStatus item in RewardsStatus)
			{
				if (item == ThreeDayRewardStatus.Unlock)
				{
					result = true;
				}
			}
			return result;
		}

		public bool AreThereCanCompleteTask()
		{
			return false;
		}

		public bool IsActivityOpen()
		{
			if (!IsCanPopOpenStatus)
			{
				return false;
			}
			return CanShowThreeDay;
		}
	}
}
