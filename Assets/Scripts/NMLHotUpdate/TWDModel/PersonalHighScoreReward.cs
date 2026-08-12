using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class PersonalHighScoreReward
	{
		[GEDListFromColumns]
		public List<string> CompletionRatio;

		[NonSerialized]
		[JsonIgnore]
		public List<Rewards> RewardEntries;
	}
}
