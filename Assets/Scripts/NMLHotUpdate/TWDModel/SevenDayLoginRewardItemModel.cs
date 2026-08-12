using System.Collections.Generic;
using Newtonsoft.Json;

namespace TWDModel
{
	public class SevenDayLoginRewardItemModel : TWDModelObject
	{
		public Rewards Rewards { get; private set; }

		[JsonIgnore]
		public EquipmentItemModel LastRewardedEquipment { get; set; }

		public SevenDayLoginRewardType RewardType { get; private set; }

		public bool Claimed { get; private set; }

		[JsonIgnore]
		public IReward Reward
		{
			get
			{
				if (Rewards?.RewardsList != null && Rewards.RewardsList.Count > 0)
				{
					return Rewards.RewardsList[0];
				}
				return null;
			}
		}

		public SevenDayLoginRewardItemModel(SevenDayLoginRewardType rewardType)
		{
			RewardType = rewardType;
		}

		public override bool IsValid()
		{
			return true;
		}

		public void GenerateRewards(string reward)
		{
			if (!string.IsNullOrEmpty(reward))
			{
				Rewards = new Rewards(reward, base.manager, base.manager.Player.Level, EquipmentSource.SevenDayLogin, base.manager.Player.PlayerRandom);
			}
		}

		public bool ClaimReward()
		{
			if (Rewards == null)
			{
				return false;
			}
			List<object> list = Rewards.Give(base.manager);
			Claimed = true;
			if (list != null && list.Count > 0 && list[0] is EquipmentItemModel)
			{
				LastRewardedEquipment = (EquipmentItemModel)list[0];
			}
			for (int i = 0; i < Rewards.RewardsList.Count; i++)
			{
				base.manager.Metrics.AddFind();
				IReward rewardAt = Rewards.GetRewardAt(i);
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
				base.manager.Metrics.AddSevenDay(base.manager.Player.SevenDayLoginManager.CurrentPeriodId, base.manager.Player.SevenDayLoginManager.CanSignInTodayDay).Send();
			}
			return true;
		}
	}
}
