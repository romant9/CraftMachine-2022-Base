using Newtonsoft.Json;

namespace TWDModel
{
	public class ReturnLoginDayItemModel : TWDModelObject
	{
		public int Day { get; private set; }

		public int RewardDefinitionId { get; private set; }

		public bool HaveClaimed { get; set; }

		[JsonIgnore]
		public ReturnLoginRewardDefinition RewardDefinition => base.manager?.GameEconomyData?.GetReturnLoginRewardDefinition(RewardDefinitionId);

		[JsonIgnore]
		public Rewards RewardEntries => RewardDefinition?.RewardEntries;

		[JsonIgnore]
		public ReturnLoginRewardStatus RewardStatus
		{
			get
			{
				if (HaveClaimed)
				{
					return ReturnLoginRewardStatus.Claimed;
				}
				ReturnActivityManager returnActivityManager = base.manager?.Player?.ReturnActivityManager;
				ReturnLoginModel returnLoginModel = returnActivityManager?.ReturnLogin;
				if (returnActivityManager == null || returnLoginModel == null)
				{
					return ReturnLoginRewardStatus.Locked;
				}
				if (!returnActivityManager.IsReturnActivityAvailable())
				{
					return ReturnLoginRewardStatus.Expired;
				}
				if (returnLoginModel.AccumulatedLoginDays < Day)
				{
					return ReturnLoginRewardStatus.Locked;
				}
				return ReturnLoginRewardStatus.ReadyToClaim;
			}
		}

		public override bool IsValid()
		{
			return true;
		}

		public ReturnLoginDayItemModel(int day, int rewardDefinitionId)
		{
			Day = day;
			RewardDefinitionId = rewardDefinitionId;
		}

		public void UpdateDefinition(int rewardDefinitionId)
		{
			RewardDefinitionId = rewardDefinitionId;
		}

		public bool TryClaimReward()
		{
			if (HaveClaimed)
			{
				return false;
			}
			ReturnActivityManager returnActivityManager = base.manager?.Player?.ReturnActivityManager;
			ReturnLoginModel returnLoginModel = returnActivityManager?.ReturnLogin;
			if (returnActivityManager == null || returnLoginModel == null)
			{
				return false;
			}
			if (!returnActivityManager.IsReturnActivityAvailable())
			{
				return false;
			}
			if (returnLoginModel.AccumulatedLoginDays < Day || RewardEntries == null)
			{
				return false;
			}
			RewardEntries.Give(base.manager);
			HaveClaimed = true;
			return true;
		}
	}
}
