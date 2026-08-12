using System;
using System.Collections.Generic;

namespace TWDModel
{
	[Serializable]
	public class ActivityDefinition
	{
		[GEDType(GEDSpecialType.AddAdvanceSecs)]
		public string StartTimeUtc;

		[GEDType(GEDSpecialType.AddAdvanceSecs)]
		public string EndTimeUtc;

		public ActivityType Event;

		public List<string> EventValue;

		public string SpenderTiers;

		public int SpenderTiersValue;

		public ActivityDefinition()
		{
			EventValue = new List<string>();
		}
	}
}
