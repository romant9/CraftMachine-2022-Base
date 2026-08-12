using System;

namespace TWDModel
{
	[Serializable]
	public class EquipmentLevelDefinition
	{
		public int Level;

		public int WorkshopLevelRequired;

		public float ArmorBase;

		public float DamageBase;

		public int UpgradeCostSurvivalPointsBase;

		public int UpgradeTimeBase;

		public int UpgradeCostEquipmentUpgradeTokens;

		public int ScrapSurvivalPointsBase;
	}
}
