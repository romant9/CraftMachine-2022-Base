using System;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class DailyLoginRewardsDefinition
	{
		public int Id;

		public string Reward;

		[NonSerialized]
		[JsonIgnore]
		public Rewards RewardEntries;
	}
}
