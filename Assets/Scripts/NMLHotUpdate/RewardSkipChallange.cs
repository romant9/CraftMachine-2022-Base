using TWDModel;

public class RewardSkipChallange : IReward
{
	public int Amount { get; set; }

	public RewardType Type => RewardType.RewardSkipChallange;

	public object Give(TWDModelManager manager, object[] param = null)
	{
		if (manager.Player.WeeklyChallenge != null)
		{
			manager.Player.WeeklyChallenge.PendingSkipTokens += Amount;
		}
		return Amount;
	}
}
