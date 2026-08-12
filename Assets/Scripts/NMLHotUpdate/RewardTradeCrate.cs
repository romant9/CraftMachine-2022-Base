using TWDModel;

public class RewardTradeCrate : IReward
{
	public string TradeCrateId { get; set; }

	public RewardType Type => RewardType.TradeCrate;

	public object Give(TWDModelManager manager, object[] param = null)
	{
		return manager.Player.LootManager.AddTradeCrateLoot(TradeCrateId);
	}
}
