using System;

namespace TWDModel
{
	[Serializable]
	public class WeeklyClassEvent
	{
		public enum AffectType
		{
			Damage = 0,
			Defense = 1,
			Xp = 2
		}

		public MapCategory MissionCategory;

		public AffectType Affects;

		public SurvivorClass SurvivorClass;

		public FixedPoint Multiplier;
	}
}
