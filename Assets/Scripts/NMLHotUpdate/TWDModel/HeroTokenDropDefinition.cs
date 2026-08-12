using System;

namespace TWDModel
{
	[Serializable]
	public class HeroTokenDropDefinition
	{
		public int ControlLevelMin;

		public int ControlLevelMax;

		public DropEventDefinition.DropEventType EventType;

		public DropType DropType;

		public DropEventDefinition.DropEventTag Tag;

		public string BucketId;
	}
}
