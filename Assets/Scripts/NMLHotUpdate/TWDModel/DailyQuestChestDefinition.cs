using System;

namespace TWDModel
{
	[Serializable]
	public class DailyQuestChestDefinition
	{
		public string Id;

		public DropEventDefinition.DropEventType EventType;

		public DropEventDefinition.DropEventContext DropContext;

		public DropEventDefinition.DropEventTag Tag;

		public int QuestPointsRequired;
	}
}
