public struct ActorStatusInfo
{
	public ActorStatusType StatusType;

	public int TurnCount;

	public ActorStatusInfo(ActorStatusType type, int turnCount = -1)
	{
		StatusType = type;
		TurnCount = turnCount;
	}
}
