using System;
using System.Collections.Generic;

namespace TWDModel
{
	[Serializable]
	public class EquipmentModifierDefinition
	{
		public string Identifier;

		public string DisplayName;

		public EquipmentModifierType Type;

		public int RarityLevel;

		public int UpgradeLevel;

		public float DamageMultiplier;

		public float CriticalChanceAdditive;

		public float CriticalMultiplier;

		public float RangeAdditive;

		public float AmmoAdditive;

		public float NoiseMultiplier;

		public float CooldownAdditive;

		public float SpreadAdditive;

		public float PenetrationAdditive;

		public float BurstAdditive;

		public float StunAdditive;

		public float ScrapValueMultiplier;

		public float ArmorMultiplier;

		public float ResourceCostMultiplier;

		public string ExtraModifierType;

		public List<string> ExtraModifierParameters;

		public string LinkedAbilityIdentifier;

		public List<AbilityModifierDefinition> GetExtraModifiers()
		{
			List<AbilityModifierDefinition> list = new List<AbilityModifierDefinition>();
			if (!string.IsNullOrEmpty(ExtraModifierType))
			{
				list.Add(new AbilityModifierDefinition(ExtraModifierType, ExtraModifierParameters));
			}
			return list;
		}
	}
}
