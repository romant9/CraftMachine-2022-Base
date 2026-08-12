using TWDModel;

public class RewardSurvivorClass : IReward
{
	public SurvivorClass SurvivorClass { get; set; }

	public RewardType Type => RewardType.SurvivorClass;

	public object Give(TWDModelManager manager, object[] param = null)
	{
		manager.Player.SurvivorContainer.UnlockSurvivorClass(SurvivorClass);
		return SurvivorClass;
	}
}
