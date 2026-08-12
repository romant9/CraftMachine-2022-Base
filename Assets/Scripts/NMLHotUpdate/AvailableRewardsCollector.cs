using System.Collections.Generic;

public interface AvailableRewardsCollector
{
	List<IReward> GetRewards();

	object CreateParameterObject();
}
