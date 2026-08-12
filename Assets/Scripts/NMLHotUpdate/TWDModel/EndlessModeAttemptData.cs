using System;
using System.Collections.Generic;
using TWDModel.ContentTypes;

namespace TWDModel
{
	[Serializable]
	public class EndlessModeAttemptData : IComparable<EndlessModeAttemptData>
	{
		public FixedPoint MaxMultiplier { get; set; }

		public long TimeStamp { get; set; }

		public int WalkersKilled { get; set; }

		public int WaveCount { get; set; }

		public long Score { get; set; }

		public EndlessModeGameModeType GameModeType { get; set; }

		public List<SurvivorMockData> SurvivorMockData { get; set; }

		public List<SurvivorSupportData> SurvivorSupportData { get; set; }

		public bool Expired { get; set; }

		public bool IsScan { get; set; }

		public int CompareTo(EndlessModeAttemptData other)
		{
			return Score.CompareTo(other.Score);
		}
	}
}
