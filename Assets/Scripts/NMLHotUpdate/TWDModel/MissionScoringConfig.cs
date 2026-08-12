using System;

namespace TWDModel
{
	[Serializable]
	public class MissionScoringConfig
	{
		public string Id;

		public int MaxValue;

		public int ScoreScale;

		public ScoringFunction Function;
	}
}
