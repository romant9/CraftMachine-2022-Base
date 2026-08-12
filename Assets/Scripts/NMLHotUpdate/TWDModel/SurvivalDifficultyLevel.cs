using System;

namespace TWDModel
{
	[Serializable]
	public class SurvivalDifficultyLevel
	{
		public FixedPoint UserLevelFactor;

		public int MissionLevelNormal;

		public int MissionLevelHard;

		public int MissionLevelNightmare;

		public FixedPoint MissionLevelUsageFactor;
	}
}
