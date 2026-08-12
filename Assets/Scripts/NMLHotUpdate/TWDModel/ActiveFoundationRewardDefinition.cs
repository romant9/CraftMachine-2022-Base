using System;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class ActiveFoundationRewardDefinition
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

		public bool IsPremiumRewardSpecial;

		public bool IsApocalypseFreeReward;

		public bool IsApocalypsePremiumReward;
	}
}
