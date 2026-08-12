namespace TWDModel
{
	public class QuestUtils
	{
		public static RewardSurvivorClass GetRewardSurvivorClassForEpisode(TWDModelManager manager, MapMissionGroupModel mapMissionGroupModel)
		{
			QuestDefinition[] questDefinitions = manager.GameEconomyData.QuestDefinitions;
			RewardSurvivorClass rewardSurvivorClass = null;
			for (int i = 0; i < questDefinitions.Length; i++)
			{
				if (rewardSurvivorClass != null)
				{
					break;
				}
				QuestDefinition questDefinition = questDefinitions[i];
				if (questDefinition.ClassName == "MissionQuest" && questDefinition.GetUnlockedEpisode(manager) == mapMissionGroupModel && questDefinition.Rewards.Contains("Class"))
				{
					rewardSurvivorClass = questDefinition.GetRewards().GetSurvivorClassReward();
				}
			}
			return rewardSurvivorClass;
		}

		public static QuestDefinition GetNextUnlockSurvivorClassQuest(TWDModelManager manager)
		{
			QuestDefinition[] questDefinitions = manager.GameEconomyData.QuestDefinitions;
			foreach (QuestDefinition questDefinition in questDefinitions)
			{
				if (questDefinition.ClassName == "MissionQuest" && questDefinition.Rewards.Contains("Class"))
				{
					RewardSurvivorClass survivorClassReward = questDefinition.GetRewards().GetSurvivorClassReward();
					if (!manager.Player.SurvivorContainer.IsSurvivorClassUnlocked(survivorClassReward.SurvivorClass))
					{
						return questDefinition;
					}
				}
			}
			return null;
		}

		public static QuestDefinition GetUnlockSurvivorClassQuest(TWDModelManager manager, SurvivorClass survivorClass)
		{
			QuestDefinition[] questDefinitions = manager.GameEconomyData.QuestDefinitions;
			foreach (QuestDefinition questDefinition in questDefinitions)
			{
				if (questDefinition.ClassName == "MissionQuest" && questDefinition.Rewards.Contains("Class"))
				{
					RewardSurvivorClass survivorClassReward = questDefinition.GetRewards().GetSurvivorClassReward();
					if (survivorClassReward != null && survivorClassReward.SurvivorClass == survivorClass)
					{
						return questDefinition;
					}
				}
			}
			return null;
		}
	}
}
