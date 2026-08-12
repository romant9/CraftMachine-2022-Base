using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace TWDModel
{
	public class ReturnThreeDayModel : TWDModelObject
	{
		public const string ReturnThreeDayChanged = "ReturnThreeDayChanged";

		public int Id { get; set; }

		public List<ReturnThreeDayRewardStatus> RewardsStatus { get; set; }

		public string CurSpendtier { get; set; }

		public long LastUpdateRewardTime { get; set; }

		[JsonIgnore]
		public ReturnThreeDayDefinition CurrentDefinition => base.manager?.GameEconomyData?.GetReturnThreeDayDefinition(Id);

		public Dictionary<int, List<int>> NeedPopReward { get; set; }

		public Dictionary<int, string> NeedPopRewardSpandier { get; set; }

		[JsonIgnore]
		public bool HasBuy => LastUpdateRewardTime > 0;

		[JsonIgnore]
		public bool IsActivityAvailable
		{
			get
			{
				if (Id > 0 && !string.IsNullOrEmpty(CurSpendtier))
				{
					TWDModelManager tWDModelManager = base.manager;
					if (tWDModelManager == null)
					{
						return false;
					}
					return tWDModelManager.Player?.ReturnActivityManager?.IsReturnActivityAvailable() == true;
				}
				return false;
			}
		}

		[JsonIgnore]
		public List<Rewards> CurrentReward
		{
			get
			{
				if (CurrentDefinition == null)
				{
					return new List<Rewards>();
				}
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

		[JsonIgnore]
		public bool HasRedDot
		{
			get
			{
				if (!IsActivityAvailable || RewardsStatus == null)
				{
					return false;
				}
				return RewardsStatus.Contains(ReturnThreeDayRewardStatus.Unlock);
			}
		}

		public void ClearRewardPop()
		{
			NeedPopReward.Clear();
			NeedPopRewardSpandier.Clear();
		}

		public List<IReward> GetNeedPopReward()
		{
			List<IReward> ret = new List<IReward>();
			if (base.manager?.GameEconomyData == null || NeedPopReward.Count == 0)
			{
				return ret;
			}
			foreach (KeyValuePair<int, List<int>> item in NeedPopReward)
			{
				ReturnThreeDayDefinition definition = base.manager.GameEconomyData.GetReturnThreeDayDefinition(item.Key);
				if (definition == null || item.Value == null || !NeedPopRewardSpandier.TryGetValue(item.Key, out var value))
				{
					continue;
				}
				if (value == definition.spendetier1)
				{
					item.Value.ForEach(delegate(int x)
					{
						if (x >= 0 && definition.RewardEntries1 != null && definition.RewardEntries1.Count > x)
						{
							ret.AddRange(definition.RewardEntries1[x].RewardsList);
						}
					});
				}
				else
				{
					if (!(value == definition.spendetier2))
					{
						continue;
					}
					item.Value.ForEach(delegate(int x)
					{
						if (x >= 0 && definition.RewardEntries2 != null && definition.RewardEntries2.Count > x)
						{
							ret.AddRange(definition.RewardEntries2[x].RewardsList);
						}
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
			CurSpendtier = "";
			LastUpdateRewardTime = 0L;
			EnsureRuntimeState();
		}

		public override void Start()
		{
			EnsureRuntimeState();
			if (CurSpendtier == null)
			{
				CurSpendtier = "";
			}
			base.Start();
		}

		public bool RewardIndex(int index)
		{
			if (base.manager == null || index < 0)
			{
				base.Debug.LogError($"ReturnThreeDayModel RewardError Id :{Id},CurSpendtier:{CurSpendtier},Index:{index}");
				return false;
			}
			List<Rewards> currentReward = CurrentReward;
			if (currentReward.Count > index && RewardsStatus.Count > index && RewardsStatus[index] == ReturnThreeDayRewardStatus.Unlock && currentReward[index] != null)
			{
				currentReward[index].Give(base.manager);
				RewardsStatus[index] = ReturnThreeDayRewardStatus.Rewarded;
				base.Debug.LogInfo($"ReturnThreeDayModel Rewarded Id :{Id},CurSpendtier:{CurSpendtier},Index:{index}");
				NotifyChange("ReturnThreeDayChanged");
				return true;
			}
			base.Debug.LogError($"ReturnThreeDayModel RewardError Id :{Id},CurSpendtier:{CurSpendtier},Index:{index},RewardsCount:{currentReward.Count},StatusCount:{RewardsStatus.Count}");
			return false;
		}

		public void OnActivityEnded()
		{
			if (Id == -1)
			{
				return;
			}
			long valueOrDefault = (base.manager?.Player?.UtcTimeStamp).GetValueOrDefault();
			if (CurrentDefinition != null && HasBuy)
			{
				int num = RewardsStatus.IndexOf(ReturnThreeDayRewardStatus.Unlock);
				if (num >= 0 && !NeedPopReward.ContainsKey(Id))
				{
					NeedPopReward[Id] = new List<int>();
					NeedPopRewardSpandier[Id] = CurSpendtier;
				}
				while (num >= 0 && RewardIndex(num))
				{
					NeedPopReward[Id].Add(num);
					num = RewardsStatus.IndexOf(ReturnThreeDayRewardStatus.Unlock);
				}
				base.manager?.Debug?.LogInfo($"ReturnThreeDayModel Ended (Activity Over) Id :{Id},CurSpendtier:{CurSpendtier},now:{valueOrDefault}");
			}
			Id = -1;
			RewardsStatus.Clear();
			CurSpendtier = "";
			LastUpdateRewardTime = 0L;
			NotifyChange("ReturnThreeDayChanged");
		}

		public override void Tick(long deltaTime)
		{
			base.Tick(deltaTime);
			if (base.manager?.Player == null)
			{
				return;
			}
			long utcTimeStamp = base.manager.Player.UtcTimeStamp;
			if (CurrentDefinition != null && LastUpdateRewardTime > 0 && LastUpdateRewardTime + CurrentDefinition.refresh <= utcTimeStamp)
			{
				int num = RewardsStatus.IndexOf(ReturnThreeDayRewardStatus.Lock);
				if (num >= 0)
				{
					RewardsStatus[num] = ReturnThreeDayRewardStatus.Unlock;
					NotifyChange("ReturnThreeDayChanged");
				}
				LastUpdateRewardTime = utcTimeStamp;
				base.manager?.Debug?.LogInfo($"ReturnThreeDayModel Unlock Id :{Id},CurSpendtier:{CurSpendtier},now:{utcTimeStamp},Index:{num}");
			}
		}

		public bool OnBuyBundle()
		{
			if (base.manager?.Player == null)
			{
				return false;
			}
			if (CurrentDefinition != null && !string.IsNullOrEmpty(CurSpendtier))
			{
				LastUpdateRewardTime = base.manager.Player.UtcTimeStamp;
				if (RewardsStatus.Count <= 0)
				{
					base.Debug.LogError($"ReturnThreeDayModel OnBuyBundle MissingRewards Id :{Id},CurSpendtier:{CurSpendtier}");
					return false;
				}
				if (RewardsStatus.Count > 0)
				{
					RewardsStatus[0] = ReturnThreeDayRewardStatus.Unlock;
				}
				base.Debug.LogInfo($"ReturnThreeDayModel OnBuyBundle Id :{Id},CurSpendtier:{CurSpendtier}");
				NotifyChange("ReturnThreeDayChanged");
				return true;
			}
			return false;
		}

		public string GetOpenedSpendertier(ReturnThreeDayDefinition threeDayDefinition)
		{
			if (threeDayDefinition == null || base.manager?.Player == null || base.manager.GameEconomyData == null)
			{
				return "";
			}
			PlayerModel player = base.manager.Player;
			long secondsSinceLastPurchase = player.BundleManager?.GetSecondsSinceLastPurchaseThatCostMoney() ?? 0;
			if (!string.IsNullOrEmpty(threeDayDefinition.spendetier1) && base.manager.GameEconomyData.IsInSpenderTier(player, threeDayDefinition.spendetier1, player.TotalUSDSpent, (int)player.LifeTimeInDays, player.GetTotalPurchases(), secondsSinceLastPurchase, player.CreationTimeStamp, player.CouncilLevel))
			{
				return threeDayDefinition.spendetier1;
			}
			if (!string.IsNullOrEmpty(threeDayDefinition.spendetier2) && base.manager.GameEconomyData.IsInSpenderTier(player, threeDayDefinition.spendetier2, player.TotalUSDSpent, (int)player.LifeTimeInDays, player.GetTotalPurchases(), secondsSinceLastPurchase, player.CreationTimeStamp, player.CouncilLevel))
			{
				return threeDayDefinition.spendetier2;
			}
			return "";
		}

		public void ResetForNewActivity(long currentTimestamp)
		{
			EnsureRuntimeState();
			Id = -1;
			RewardsStatus.Clear();
			CurSpendtier = "";
			LastUpdateRewardTime = 0L;
			ReturnThreeDayDefinition curOpenedReturnThreeDayDefinition = base.manager.GameEconomyData.GetCurOpenedReturnThreeDayDefinition(currentTimestamp);
			if (curOpenedReturnThreeDayDefinition != null)
			{
				Id = curOpenedReturnThreeDayDefinition.Id;
				CurSpendtier = GetOpenedSpendertier(curOpenedReturnThreeDayDefinition);
				if (!string.IsNullOrEmpty(CurSpendtier))
				{
					if (CurSpendtier == curOpenedReturnThreeDayDefinition.spendetier1)
					{
						RewardsStatus.AddRange(curOpenedReturnThreeDayDefinition.RewardEntries1.Select((Rewards x) => ReturnThreeDayRewardStatus.Lock));
					}
					else if (CurSpendtier == curOpenedReturnThreeDayDefinition.spendetier2)
					{
						RewardsStatus.AddRange(curOpenedReturnThreeDayDefinition.RewardEntries2.Select((Rewards x) => ReturnThreeDayRewardStatus.Lock));
					}
				}
				base.Debug.LogInfo($"ReturnThreeDayModel ResetForNewActivity Started Id :{Id},CurSpendtier:{CurSpendtier},now:{currentTimestamp}");
			}
			NotifyChange("ReturnThreeDayChanged");
		}

		private void EnsureRuntimeState()
		{
			if (RewardsStatus == null)
			{
				RewardsStatus = new List<ReturnThreeDayRewardStatus>();
			}
			if (NeedPopReward == null)
			{
				NeedPopReward = new Dictionary<int, List<int>>();
			}
			if (NeedPopRewardSpandier == null)
			{
				NeedPopRewardSpandier = new Dictionary<int, string>();
			}
		}
	}
}
