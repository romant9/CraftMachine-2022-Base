using System;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class NewbieStageReward
	{
		public int PointNeeded;

		public string StageReward;

		[NonSerialized]
		[JsonIgnore]
		public Rewards RewardEntries;

		public void CalcReward()
		{
			RewardEntries = new Rewards(StageReward);
		}
	}
}
