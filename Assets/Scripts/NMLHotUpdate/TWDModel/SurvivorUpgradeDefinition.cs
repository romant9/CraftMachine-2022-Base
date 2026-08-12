using System;

namespace TWDModel
{
	[Serializable]
	public class SurvivorUpgradeDefinition
	{
		public SurvivorClass SurvivorClass;

		public int Level;

		public int TrainingGroundLevel;

		public float DamageBase;

		public float HealthBase;

		public float MovementBase;

		public int DemoteSPBase;

		public int DemoteSPRefund;
	}
}
