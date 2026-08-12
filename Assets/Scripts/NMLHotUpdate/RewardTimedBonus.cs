using TWDModel;

public class RewardTimedBonus : IReward
{
	public FixedPoint Duration { get; set; }

	public TimedBonusType TimedBonusType { get; set; }

	public RewardType Type => RewardType.TimedBonus;

	public object Give(TWDModelManager manager, object[] param = null)
	{
		if (manager != null && manager.Player != null)
		{
			manager.Player.AddTimedBonus(TimedBonusType, Duration);
		}
		return TimedBonusType;
	}
}
