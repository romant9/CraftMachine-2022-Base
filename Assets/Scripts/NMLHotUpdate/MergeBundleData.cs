using TWDModel;

public class MergeBundleData
{
	public int CurrentSelectIndex;

	public IReward Reward;

	public CustomBundleDefinition CustomBundleDefinition;

	public MergeBundleData(int currentSelectIndex, IReward reward, CustomBundleDefinition customBundleDefinition)
	{
		CurrentSelectIndex = currentSelectIndex;
		Reward = reward;
		CustomBundleDefinition = customBundleDefinition;
	}
}
