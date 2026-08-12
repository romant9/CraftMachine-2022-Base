using System;

namespace TWDModel
{
	[Serializable]
	public enum AbilityTriggerType
	{
		Instant = 0,
		Targetted = 1,
		Grid = 2,
		WaitsTurn = 3,
		GridOrTarget = 4
	}
}
