using System;
using System.Collections.Generic;

namespace TWDModel
{
	[Serializable]
	public class EquipmentDefinition
	{
		public string ID;

		public EquipmentCategory Category;

		public string SubCategory;

		public EquipmentType Type;

		public SurvivorClass SurvivorClass;

		public bool CannotBeGivenAsLoot;

		public int MinTier;

		public int MaxTier;

		public string AbilityIdentifier;

		public string ChargeEquipmentIdentifier;

		public List<int> AvailableRarityLevels;

		public List<Faction> HolderFactions;

		public List<string> HolderActors;

		public int MaxAmountAtInventory;

		public int DamageMultiplier;

		public int DamageVariation;

		public int ArmorMultiplier;

		public List<string> ActiveTraits;

		public List<string> PassiveTraits;

		public List<string> TraitsOverride;

		public List<int> CommandSkills;

		public List<int> CommandSkillsBreakthroughLv;

		public bool SwitchRemoldMode;

		public int RemoldTraitsSlotCount;

		public List<string> ScrapSPTokenPackage;

		public List<string> SPTraitsRemoldType;

		public List<string> SPTraitsRemoldRandomPackage;

		public string AnimatorOverride;

		public string UseThisWeaponIcon;

		public string UseThisLocalizationName;

		public string UseThisLocalizationDescription;

		public string InfusedTrait;

		public bool UseSpecialMaterial;

		public string SpecialTrait;

		public string TD_Ability;

		public bool CanBeEquippedToFaction(Faction faction)
		{
			if (HolderFactions != null && HolderFactions.Count != 0)
			{
				return HolderFactions.Contains(faction);
			}
			return true;
		}

		public bool CanBeEquippedBySurvivorClass(SurvivorClass survivorClass)
		{
			if (CanBeEquippedToFaction(Faction.Survivor))
			{
				if (SurvivorClass != SurvivorClass.None)
				{
					return SurvivorClass == survivorClass;
				}
				return true;
			}
			return false;
		}
	}
}
