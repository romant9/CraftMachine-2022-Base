using System;
using System.Collections.Generic;
using TWDModel;

public class WeeklySurvivalRewardsCollector : AvailableRewardsCollector
{
	private WeeklySurvivalModel survivalModel;

	public WeeklySurvivalRewardsCollector(WeeklySurvivalModel survivalModel)
	{
		this.survivalModel = survivalModel;
	}

	public List<IReward> GetRewards()
	{
		List<IReward> list = new List<IReward>();
		Dictionary<WeeklySurvivalReward.SurvivalRewardType, int> dictionary = (Dictionary<WeeklySurvivalReward.SurvivalRewardType, int>)CreateParameterObject();
		if (survivalModel != null)
		{
			GameEconomyData gameEconomyData = survivalModel.gameEconomyData;
			for (int i = 0; i < ((gameEconomyData.WeeklySurvivalRewards != null) ? gameEconomyData.WeeklySurvivalRewards.Length : 0); i++)
			{
				WeeklySurvivalReward weeklySurvivalReward = gameEconomyData.WeeklySurvivalRewards[i];
				int value = 0;
				dictionary.TryGetValue(weeklySurvivalReward.RewardType, out value);
				string text = "";
				if (survivalModel.CurrentDefinition != null && !survivalModel.Finished)
				{
					text = survivalModel.CurrentDefinition.RewardSetName;
				}
				else
				{
					WeeklySurvival survivalPlayableWhen = survivalModel.gameEconomyData.GetSurvivalPlayableWhen(survivalModel.manager.Player.UtcTimeStamp, (long)new TimeSpan(0, 1, 0).TotalMilliseconds);
					if (survivalPlayableWhen != null)
					{
						text = survivalPlayableWhen.RewardSetName;
					}
				}
				if (weeklySurvivalReward.Control <= value || !(weeklySurvivalReward.SetName == text) || weeklySurvivalReward.RewardType != WeeklySurvivalReward.SurvivalRewardType.MissionCompletions)
				{
					continue;
				}
				for (int j = 0; j < weeklySurvivalReward.RewardEntries.Length; j++)
				{
					Rewards rewards = weeklySurvivalReward.RewardEntries[j];
					if (rewards != null && rewards.RewardsList != null && rewards.RewardsList.Count > 0)
					{
						list.AddRange(rewards.RewardsList);
					}
				}
			}
		}
		return list;
	}

	public object CreateParameterObject()
	{
		return new Dictionary<WeeklySurvivalReward.SurvivalRewardType, int> { 
		{
			WeeklySurvivalReward.SurvivalRewardType.MissionCompletions,
			survivalModel.NumberCompleted
		} };
	}
}
