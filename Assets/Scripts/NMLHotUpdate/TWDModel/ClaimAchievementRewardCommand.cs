using System;
using BaseModel;

namespace TWDModel
{
	public class ClaimAchievementRewardCommand : ModelCommand
	{
		public string AchievementID { get; private set; }

		public ClaimAchievementRewardCommand()
		{
		}

		public ClaimAchievementRewardCommand(string achievementID)
		{
			AchievementID = achievementID;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelResult result = TWDModelResult.Error;
			if (manager is TWDModelManager tWDModelManager && tWDModelManager.Player.AchievementManager != null)
			{
				tWDModelManager.Player.AchievementManager.CheckAchievements();
				AchievementDefinition achievementDefinition = tWDModelManager.Player.gameEconomyData.GetAchievementDefinition(AchievementID);
				Achievement achievement = ((achievementDefinition != null) ? tWDModelManager.Player.AchievementManager.GetAchievement(achievementDefinition) : null);
				if (achievement != null && !achievement.RewardClaimed && tWDModelManager.Player.AchievementManager.IsAchievementCompleted(achievementDefinition))
				{
					Rewards rewards = null;
					try
					{
						rewards = achievement.GetRewards();
					}
					catch (Exception)
					{
					}
					if (rewards != null)
					{
						tWDModelManager.Player.AchievementManager.SetAchievementClaimed(achievementDefinition);
						rewards.Give(tWDModelManager);
						SendCollectionAnalyticEventsForRewards(achievementDefinition, rewards, "Achievement", tWDModelManager);
						result = TWDModelResult.OK;
					}
				}
			}
			return new NGModelCommandRespond(this, result);
		}

		private void SendCollectionAnalyticEventsForRewards(AchievementDefinition achievementDefinition, Rewards rewards, string collectionType, TWDModelManager manager)
		{
			if (manager == null || manager.Player == null)
			{
				return;
			}
			manager.Metrics.Reset();
			for (int i = 0; i < rewards.RewardsList.Count; i++)
			{
				if (rewards.RewardsList[i] is RewardCurrency)
				{
					RewardCurrency rewardCurrency = rewards.RewardsList[i] as RewardCurrency;
					manager.Metrics.PushResource(rewardCurrency.CurrencyType, rewardCurrency.AmountActuallyAdded, rewardCurrency.GetOverflowAmount());
				}
			}
			manager.Metrics.AddFind().AddResources().AddAchivement(achievementDefinition)
				.Send();
			for (int j = 0; j < rewards.RewardsList.Count; j++)
			{
				if (rewards.RewardsList[j] is RewardEquipment rewardEquipment)
				{
					EquipmentItemModel equipment = manager.Player.Equipment.GenerateAndInitializeEquipmentFromDefinition(rewardEquipment.EquipmentId);
					manager.Metrics.AddFind().AddEquipment(equipment, "Equipment", rewardEquipment.Amount).AddAchivement(achievementDefinition)
						.Send();
				}
				else if (rewards.RewardsList[j] is RewardTimedBonus rewardTimedBonus)
				{
					manager.Metrics.AddFind().AddTimedBonus(rewardTimedBonus).AddAchivement(achievementDefinition)
						.Send();
				}
			}
		}
	}
}
