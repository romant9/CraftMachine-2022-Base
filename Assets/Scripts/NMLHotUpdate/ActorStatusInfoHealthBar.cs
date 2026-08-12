using TWDModel;

public class ActorStatusInfoHealthBar
{
	public TimedEffectType StatusType;

	public int TurnCount;

	public int MaxTurnCount;

	public ActorStatusInfoHealthBar(TimedEffectType type, int turnCount = -1, int maxDuration = -1)
	{
		StatusType = type;
		TurnCount = turnCount;
		MaxTurnCount = maxDuration;
	}
}
