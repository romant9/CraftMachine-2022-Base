using System.Collections.Generic;

namespace TWDModel
{
	public class AILog
	{
		public static int logEntryId = -1;

		public List<ActorTurnEntry> TurnEntries;

		public ActorTurnEntry CurrentActorTurnLogEntry
		{
			get
			{
				return null;
			}
			private set
			{
			}
		}

		public void StartLogEntry(int turn, ActorModel actor)
		{
			if (CurrentActorTurnLogEntry == null)
			{
				CurrentActorTurnLogEntry = new ActorTurnEntry
				{
					Id = logEntryId++,
					Turn = turn,
					Actor = actor,
					BehaviorLog = new List<BehaviorLogEntry>(),
					VisibleEnemyLocations = new List<GridCoordinate>()
				};
			}
		}

		public void EndLogEntry()
		{
			if (CurrentActorTurnLogEntry != null)
			{
				if (TurnEntries == null)
				{
					TurnEntries = new List<ActorTurnEntry>();
				}
				TurnEntries.Add(CurrentActorTurnLogEntry);
				CurrentActorTurnLogEntry = null;
			}
		}
	}
}
