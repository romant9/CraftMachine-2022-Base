namespace TWDModel
{
	public class AIEvent
	{
		public int StartTurn;

		public int DurationInTurns;

		public AIEvent()
		{
			StartTurn = 0;
			DurationInTurns = -1;
		}

		public AIEvent(int currentTurn, int durationInTurns)
		{
			StartTurn = currentTurn;
			DurationInTurns = durationInTurns;
		}

		public bool IsValid(int currentTurn)
		{
			if (StartTurn <= currentTurn)
			{
				if (DurationInTurns >= 0)
				{
					return currentTurn < StartTurn + DurationInTurns;
				}
				return true;
			}
			return false;
		}
	}
}
