using System;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class ReturnExchangeStoreDefinition
	{
		public int Id;

		public ReturnExchangeStoreType Type;

		public string DisplayDescription;

		public string Cost;

		public string Reward;

		public int Limit;

		public int CouncilLevelMin;

		public int CouncilLevelMax;

		[NonSerialized]
		[JsonIgnore]
		public Rewards CostRewardEntries;

		[NonSerialized]
		[JsonIgnore]
		public Rewards RewardEntries;
	}
}
