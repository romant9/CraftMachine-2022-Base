using System;
using BaseModel;

namespace TWDModel
{
	public class ClaimDailyQuestRewardCommand : ModelCommand
	{
		public string AchievementID { get; private set; }

		public ClaimDailyQuestRewardCommand()
		{
		}

		public ClaimDailyQuestRewardCommand(string achievementID)
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
				Achievement achievement = ((achievementDefinition != null) ? tWDModelManager.Player.AchievementManager.GetDailyQuest(achievementDefinition) : null);
				if (achievement != null)
				{
					Rewards rewards = null;
					try
					{
						rewards = achievement.GetRewards();
					}
					catch (Exception)
					{
					}
					if (rewards != null && tWDModelManager.Player.AchievementManager.CompleteDailyQuest(AchievementID))
					{
						rewards.Give(tWDModelManager);
						SendCollectionAnalyticEventsForRewards(achievementDefinition, rewards, "Daily_Quest", tWDModelManager);
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
			manager.Metrics.AddFind().AddResources().AddDailyQuest(achievementDefinition)
				.Send();
		}
	}
}
