using TWDModel;

public class RewardSurvivorSlot : IReward
{
	public int Amount { get; set; }

	public RewardType Type => RewardType.SurvivorSlot;

	public object Give(TWDModelManager manager, object[] param = null)
	{
		manager.Player.SurvivorContainer.SurvivorGiftSlotsCount += Amount;
		return null;
	}
}
