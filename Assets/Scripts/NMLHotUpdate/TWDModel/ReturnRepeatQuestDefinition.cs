using System;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class ReturnRepeatQuestDefinition
	{
		public int Id;

		public string DisplayDescription;

		public ReturnQuestType QuestType;

		public string Params;

		public string Reward;

		public int Time;

		public int CouncilLevelMin;

		public string DeepLink;

		[NonSerialized]
		[JsonIgnore]
		public Rewards RewardEntries;
	}
}
