using System;
using System.Collections.Generic;

namespace TWDModel
{
	[Serializable]
	public class BuildingUpgradeLevel
	{
		public string BuildingType;

		public int Level;

		public int CostSupplies;

		public int CostInhabitants;

		public int CostDiamonds;

		public int AwardedXp;

		public int UpgradeTime;

		public int ProductionRate;

		public int ProductionCapacity;

		public int SuppliesCapacity;

		public int SPTraitCapacity;

		public int ReplayTokenCapacity;

		public int InhabitantsCapacity;

		public int SPCapacity;

		public int OutpostCapacity;

		public BuffEffectType BuffEffectType;

		public int DependencyLevelRequired;

		public int PlayerLevelRequired;

		public int MedicSlotsAmount;

		public int MedicInjuryTimeBonus;

		public int BadgeCreationCost;

		public int BadgeScrapXP;

		public int DestroyTime;

		public int DestroyCost;

		public List<long> FreeCallTimeSeconds;

		public long FreeCallTimeOnUpgrade;

		public List<int> FreeCallMaxStackable;

		public List<int> UpgradedCallChance;

		public BuildingUpgradeLevel()
		{
		}

		public BuildingUpgradeLevel(string typeName, int level, int upgradeTime)
		{
			BuildingType = typeName;
			Level = level;
			UpgradeTime = upgradeTime;
		}

		public int GetCapacity(CurrencyType currencyType)
		{
			return currencyType switch
			{
				CurrencyType.Supplies => SuppliesCapacity,
				CurrencyType.SPTraitsUpgradeToken => SPTraitCapacity,
				CurrencyType.SurvivalPoints => SPCapacity,
				CurrencyType.Inhabitants => InhabitantsCapacity,
				CurrencyType.ReplayToken => ReplayTokenCapacity,
				CurrencyType.Outpost => OutpostCapacity,
				_ => 0,
			};
		}
	}
}
