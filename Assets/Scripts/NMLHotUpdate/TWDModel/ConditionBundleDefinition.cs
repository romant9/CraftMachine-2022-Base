using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class ConditionBundleDefinition
	{
		public string BundleIdentifier;

		public RFMEvent Condition;

		public List<string> Params;

		public RFMLevel RecencyLevel;

		public RFMLevel FrequencyLevel;

		public RFMLevel MonetaryLevel;

		public string BundleStatusH;

		[GEDType(GEDSpecialType.TimeMilliseconds)]
		public long TimeLimit;

		public int DailyPopLimit;

		public int Priority;

		public string ConditionTitleLocalisation;

		[JsonIgnore]
		public int RMFValue => (int)((uint)(RFMLevel.L | RecencyLevel) | (uint)((int)FrequencyLevel << 1)) | ((int)MonetaryLevel << 2);
	}
}
