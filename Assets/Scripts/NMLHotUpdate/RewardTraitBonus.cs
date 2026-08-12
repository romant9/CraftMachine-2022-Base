using TWDModel;

public class RewardTraitBonus : IReward
{
	public string TraitId { get; set; }

	public RewardType Type => RewardType.TraitBonus;

	public object Give(TWDModelManager manager, object[] param = null)
	{
		return manager.Player.LootManager.AddTraitBonusReward(TraitId);
	}
}
