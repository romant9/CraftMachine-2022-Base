using System;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class TD_Reward
	{
		public enum TD_RewardType
		{
			TD_Mission = 0,
			TD_Difficulty = 1
		}

		public int Identifier;

		public TD_RewardType RewardType;

		public int Level;

		public string Rewards;

		[NonSerialized]
		[JsonIgnore]
		public Rewards RewardEntries;
	}
}
