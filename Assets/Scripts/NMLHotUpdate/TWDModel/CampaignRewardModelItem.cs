using System.Collections.Generic;
using Newtonsoft.Json;

namespace TWDModel
{
	public class CampaignRewardModelItem : TWDModelObject, CampaignRewardItem
	{
		[JsonIgnore]
		public Rewards Rewards { get; private set; }

		[JsonIgnore]
		public EquipmentItemModel LastRewardedEquipment { get; set; }

		[JsonIgnore]
		public EquipTokenItemModel LastRewardedEquipmentToken { get; set; }

		public bool Claimed { get; private set; }

		[JsonIgnore]
		public bool Claimable => !Claimed;

		public int Control { get; set; }

		[JsonIgnore]
		public IReward Reward
		{
			get
			{
				if (Rewards != null && Rewards.RewardsList != null && Rewards.RewardsList.Count > 0)
				{
					return Rewards.RewardsList[0];
				}
				return null;
			}
		}

		[JsonIgnore]
		public CampaignRewardsDefinition RewardsDefinition { get; private set; }

		public override bool IsValid()
		{
			return true;
		}

		public void GenerateRewards(CampaignRewardsDefinition definition)
		{
			if (definition != null)
			{
				RewardsDefinition = definition;
				Rewards = new Rewards(definition.Reward, base.manager, base.manager.Player.Level, EquipmentSource.Campaign, base.manager.Player.PlayerRandom);
			}
		}

		public void ClaimReward()
		{
			if (Rewards == null)
			{
				return;
			}
			List<object> list = Rewards.Give(base.manager);
			Claimed = true;
			if (list != null && list.Count > 0 && list[0] is EquipmentItemModel)
			{
				LastRewardedEquipment = (EquipmentItemModel)list[0];
			}
			if (list != null && list.Count > 0 && list[0] is EquipTokenItemModel)
			{
				LastRewardedEquipmentToken = (EquipTokenItemModel)list[0];
			}
			if (RewardsDefinition == null)
			{
				return;
			}
			for (int i = 0; i < Rewards.RewardsList.Count; i++)
			{
				base.manager.Metrics.AddFind();
				IReward rewardAt = Rewards.GetRewardAt(i);
				if (list[i] is EquipmentItemModel)
				{
					base.manager.Metrics.AddEquipment((EquipmentItemModel)list[i], "Equipment", (rewardAt as RewardEquipment)?.Amount ?? 1);
				}
				else
				{
					base.manager.Metrics.AddReward(rewardAt);
				}
				base.manager.Metrics.AddCampaign(RewardsDefinition.Id.ToString(), RewardsDefinition.Control.ToString()).Send();
			}
		}
	}
}
