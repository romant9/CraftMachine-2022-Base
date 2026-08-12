using System.Collections.Generic;
using Newtonsoft.Json;

namespace TWDModel
{
	public class ActiveFoundationRewardItemModel : TWDModelObject
	{
		public Rewards Rewards { get; private set; }

		[JsonIgnore]
		public EquipmentItemModel LastRewardedEquipment { get; set; }

		public ActiveFoundationRewardType RewardType { get; private set; }

		public ActiveFoundationRewardItemModel(ActiveFoundationRewardType rewardType)
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
				Rewards = new Rewards(reward, base.manager, base.manager.Player.Level, EquipmentSource.ActiveFoundation, base.manager.Player.PlayerRandom);
			}
		}

		public bool ClaimReward()
		{
			if (Rewards == null)
			{
				return false;
			}
			List<object> list = Rewards.Give(base.manager);
			for (int i = 0; i < Rewards.RewardsList.Count; i++)
			{
				base.manager.Metrics.ResourceChangeObtainReason = "ActiveFoundation";
				base.manager.Metrics.AddFind();
				IReward rewardAt = Rewards.GetRewardAt(i);
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
	}
}
