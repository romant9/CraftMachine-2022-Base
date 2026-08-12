using TWDModel;

public class RewardUnlockBuilding : IReward
{
	public string BuildingTypeName { get; set; }

	public RewardType Type => RewardType.UnlockBuilding;

	public object Give(TWDModelManager manager, object[] param = null)
	{
		return null;
	}
}
