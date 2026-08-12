using System;

namespace TWDModel
{
	[Serializable]
	public class OutpostRewardInfo
	{
		public int Level;

		public OutpostRewardLevelType LevelType;

		public int MinReward;

		public int MaxReward;
	}
}
