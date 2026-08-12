using System.Collections.Generic;
using TWDModel;

public class CurrencyRewardSolver : RewardsSolver<CurrencyType>
{
	public virtual bool HasRewardsOfType(CurrencyType type, List<IReward> rewards)
	{
		for (int i = 0; i < rewards.Count; i++)
		{
			if (rewards[i] is RewardCurrency && ((RewardCurrency)rewards[i]).CurrencyType == type)
			{
				return true;
			}
		}
		return false;
	}
}
