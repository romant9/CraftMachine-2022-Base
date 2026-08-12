using System.Collections.Generic;
using TWDModel;

public class DailyQuestsRewardsCollector : AvailableRewardsCollector
{
	private DailyQuestModel dailyQuestModel;

	public DailyQuestsRewardsCollector(DailyQuestModel dailyQuestModel)
	{
		this.dailyQuestModel = dailyQuestModel;
	}

	public List<IReward> GetRewards()
	{
		List<IReward> list = new List<IReward>();
		if (dailyQuestModel != null && dailyQuestModel.ActiveQuests != null && dailyQuestModel.ActiveQuests.Count > 0)
		{
			for (int i = 0; i < dailyQuestModel.ActiveQuests.Count; i++)
			{
				DailyQuestItemModel dailyQuestItemModel = dailyQuestModel.ActiveQuests[i];
				if (!dailyQuestItemModel.Claimed && dailyQuestItemModel.Rewards != null && dailyQuestItemModel.Rewards.RewardsList != null)
				{
					list.AddRange(dailyQuestItemModel.Rewards.RewardsList);
				}
			}
		}
		return list;
	}

	public object CreateParameterObject()
	{
		return null;
	}
}
