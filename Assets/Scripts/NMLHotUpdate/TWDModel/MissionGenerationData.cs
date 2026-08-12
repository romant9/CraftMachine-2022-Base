using System;

namespace TWDModel
{
	[Serializable]
	public class MissionGenerationData
	{
		public int MissionLevel;

		public int MaxTotalWalkers;

		public int BossCount;

		public int BossLevelOffset;

		public int MinWalkerLevel;

		public int MaxWalkerLevel;

		public int CuringTimeMinor;

		public int CuringTimeMajor;

		public int CuringTimeCritical;
	}
}
