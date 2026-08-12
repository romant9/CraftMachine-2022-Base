using System;

namespace TWDModel
{
	[Serializable]
	public class MissionFlavorData
	{
		public string Name;

		public int MinPlayerLevel;

		public float Probability;

		public float BossProbability;

		public float WalkerTypeNormal;

		public float WalkerTypeTank;

		public float WalkerTypeArmored;

		public float WalkerTypeExplosive;

		public float WalkerTypeSlim;

		public float WalkerBehaviorNormal;

		public float WalkerBehaviorDormant;

		public float WalkerBehaviorRoaming;

		public float WalkerBehaviorHoming;

		public int LootTableModifier;

		public int LevelModifier;

		public float InitialWalkerAmount;

		public int InitialWalkerClustering;

		public int InitialThreat;

		public string Description;
	}
}
