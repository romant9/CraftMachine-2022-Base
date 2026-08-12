using System;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class WeeklySurvivalReward
	{
		public enum SurvivalRewardType
		{
			None = 0,
			MissionCompletions = 1,
			FullCompletion = 2
		}

		public SurvivalRewardType RewardType;

		public string SetName;

		public int Control;

		public string RewardsNormal;

		public string RewardsHard;

		public string RewardsNightmare;

		[NonSerialized]
		[JsonIgnore]
		public Rewards[] RewardEntries;
	}
}
