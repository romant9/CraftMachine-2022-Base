using System;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class CampaignRewardsDefinition
	{
		public int Id;

		public int Control;

		public string Reward;

		public bool Highlighted;

		[NonSerialized]
		[JsonIgnore]
		public Rewards RewardEntries;
	}
}
