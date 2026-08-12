using System.Collections.Generic;

public class StorageRewardListPanel : ScrollableListPanel<MergeBundleData>
{
	public void Init(List<MergeBundleData> rewards)
	{
		SetCards(rewards);
	}
}
