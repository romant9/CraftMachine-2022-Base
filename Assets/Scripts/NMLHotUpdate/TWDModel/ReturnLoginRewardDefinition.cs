using System;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class ReturnLoginRewardDefinition
	{
		public int Id;

		public int ReturnLoginId;

		public int Day;

		public string Reward;

		[NonSerialized]
		[JsonIgnore]
		public Rewards RewardEntries;
	}
}
