using System;

namespace TWDModel
{
	[Serializable]
	public class ConditionBundleConfig
	{
		public int MaxDailyPopLimit;

		[GEDType(GEDSpecialType.TimeMilliseconds)]
		public long RecencyBaseValue;

		public int FrequencyBaseValue;

		public int MonetaryBaseValue;

		[GEDType(GEDSpecialType.TimeMilliseconds)]
		public long MonetaryTimePeriod;

		[GEDType(GEDSpecialType.TimeMilliseconds)]
		public long FrequencyTimePeriod;

		public int BundleBaseChance;

		public int BundleAccumulateChance;

		public int UnlockCouncilLevel;

		public int GoldBaseValue;

		public int GoldPoolWeight;

		public int ResourceBaseValue;

		public int ResourcePoolWeight;

		public int XPBaseValue;

		public int XPPoolWeight;

		public int RadioBaseValue;

		public int RadioPoolWeight;
	}
}
