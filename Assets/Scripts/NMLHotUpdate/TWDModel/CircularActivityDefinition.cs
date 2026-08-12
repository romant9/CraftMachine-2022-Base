using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class CircularActivityDefinition
	{
		[GEDType(GEDSpecialType.AddAdvanceSecs)]
		public string StartTimeUtc;

		public int DurationTime;

		public int IntervalTime;

		public ActivityType Event;

		public List<string> EventValue;

		[JsonIgnore]
		public int CircularDays => DurationTime + IntervalTime;

		public CircularActivityDefinition()
		{
			EventValue = new List<string>();
		}
	}
}
