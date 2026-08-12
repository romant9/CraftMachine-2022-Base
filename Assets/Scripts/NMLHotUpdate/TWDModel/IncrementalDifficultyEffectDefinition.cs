using System;

namespace TWDModel
{
	[Serializable]
	public class IncrementalDifficultyEffectDefinition
	{
		public IncrementalDifficultyMissionType MissionType;

		public int Increment;

		public IncrementalDifficultyEffect Effect;

		public int Parameter;
	}
}
