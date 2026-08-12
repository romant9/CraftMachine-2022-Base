using System;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class ReturnDailyQuestDefinition
	{
		public int Id;

		public int Group;

		public string DisplayDescription;

		public ReturnQuestType QuestType;

		public string Params;

		public string Reward;

		public int CouncilLevelMin;

		public int CouncilLevelMax;

		public string DeepLink;

		[NonSerialized]
		[JsonIgnore]
		public Rewards RewardEntries;
	}
}
