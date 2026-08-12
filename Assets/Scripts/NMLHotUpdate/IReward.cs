using TWDModel;

public interface IReward
{
	RewardType Type { get; }

	object Give(TWDModelManager manager, object[] param = null);
}
