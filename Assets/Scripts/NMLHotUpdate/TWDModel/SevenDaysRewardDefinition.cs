using System;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class SevenDaysRewardDefinition
	{
		public int Id;

		public int PeriodId;

		public int Day;

		public string FreeReward;

		public string PremiumReward;

		[NonSerialized]
		[JsonIgnore]
		public Rewards FreeRewardEntries;

		[NonSerialized]
		[JsonIgnore]
		public Rewards PremiumRewardEntries;

		public bool IsApocalypseFreeReward;

		public bool IsApocalypsePremiumReward;
	}
}
