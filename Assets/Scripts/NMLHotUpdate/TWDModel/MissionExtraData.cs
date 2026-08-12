using System;
using System.Collections.Generic;

namespace TWDModel
{
	[Serializable]
	public class MissionExtraData
	{
		public int MaxTeamSize;

		public SurvivorClass RewardedSurvivorType;

		public int RewardedSurvivorRarityLevel;

		public List<string> EnemyAdditionalTraits;

		public List<string> CivilianActorIds;

		public List<PlayableSurvivor> PlayableSurvivors;

		public bool InUse;
	}
}
