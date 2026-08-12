using System;

namespace TWDModel
{
	[Serializable]
	public class ActorLevelBalanceModifier
	{
		public Faction Attacker;

		public Faction Target;

		public int LevelDiff;

		public float BodyshotChance;

		public float CriticalChance;
	}
}
