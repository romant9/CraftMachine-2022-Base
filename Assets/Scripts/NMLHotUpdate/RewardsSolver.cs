using System.Collections.Generic;

public interface RewardsSolver<T>
{
	bool HasRewardsOfType(T type, List<IReward> rewards);
}
