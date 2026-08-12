using System;
using System.Collections.Generic;
using TWDModel;

public class WeeklyChallengeRewardsCollector : AvailableRewardsCollector
{
	private WeeklyChallengeModel challengeModel;

	public WeeklyChallengeRewardsCollector(WeeklyChallengeModel challengeModel)
	{
		this.challengeModel = challengeModel;
	}

	public List<IReward> GetRewards()
	{
		List<IReward> list = new List<IReward>();
		Dictionary<WeeklyChallengeReward.ChallengeRewardType, int> dictionary = (Dictionary<WeeklyChallengeReward.ChallengeRewardType, int>)CreateParameterObject();
		if (challengeModel != null)
		{
			bool flag = false;
			flag = (challengeModel.CurrentDefinition != null && !challengeModel.Finished) || challengeModel.gameEconomyData.GetWeeklyChallengePlayableWhen(challengeModel.manager.Player.UtcTimeStamp, (long)new TimeSpan(0, 1, 0).TotalMilliseconds) != null;
			GameEconomyData gameEconomyData = challengeModel.gameEconomyData;
			int num = 0;
			while (flag && num < gameEconomyData.WeeklyChallengeRewards.Length)
			{
				WeeklyChallengeReward weeklyChallengeReward = gameEconomyData.WeeklyChallengeRewards[num];
				int value = 0;
				dictionary.TryGetValue(weeklyChallengeReward.RewardType, out value);
				if (weeklyChallengeReward.Control > value && weeklyChallengeReward.RewardEntries != null && weeklyChallengeReward.RewardEntries.Count > 0)
				{
					list.AddRange(weeklyChallengeReward.RewardEntries.RewardsList);
				}
				num++;
			}
		}
		return list;
	}

	public object CreateParameterObject()
	{
		return new Dictionary<WeeklyChallengeReward.ChallengeRewardType, int>
		{
			{
				WeeklyChallengeReward.ChallengeRewardType.PersonalStars,
				challengeModel.NumberStars
			},
			{
				WeeklyChallengeReward.ChallengeRewardType.GuildStars,
				challengeModel.LastNumberOfGuildStars
			}
		};
	}
}
