using System;

namespace TWDModel
{
	[Serializable]
	public class EquipTokenDefinition
	{
		public string EquipTokenId;

		public int Sort;

		public SurvivorClass SurvivorClass;

		public EquipmentCategory Category;

		public int Star;

		public string EquipmentBreakthroughsType;

		public int TokensToUnlock;

		public string RelateEquipId;

		public string UseThisLocalizationName;

		public string UseThisWeaponIcon;

		public int ApocalypticEquipToken;
	}
}
