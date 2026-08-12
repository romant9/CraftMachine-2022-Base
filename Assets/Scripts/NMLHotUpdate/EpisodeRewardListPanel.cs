using System.Collections.Generic;

public class EpisodeRewardListPanel : ScrollableListPanel<IReward>
{
	public void Init(List<IReward> data)
	{
		SetCards(data);
	}
}
