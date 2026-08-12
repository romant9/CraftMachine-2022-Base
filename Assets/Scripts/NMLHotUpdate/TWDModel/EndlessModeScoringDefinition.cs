using System;

namespace TWDModel
{
	[Serializable]
	public class EndlessModeScoringDefinition
	{
		public string Enemy;

		public FixedPoint Score;

		public FixedPoint EnemyConstant;

		public FixedPoint MultiplierIncrease;
	}
}
