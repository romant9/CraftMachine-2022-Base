using TWDModel;

public class RewardGuildBattleVP : IReward
{
	public int Amount { get; set; }

	public RewardType Type => RewardType.GuildBattleVP;

	public object Give(TWDModelManager manager, object[] param = null)
	{
		return null;
	}
}
