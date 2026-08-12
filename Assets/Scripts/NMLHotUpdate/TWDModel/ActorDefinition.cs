using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class ActorDefinition
	{
		public string ID;

		public string Name;

		public string FullName;

		public int InitialLevelOffset;

		public int RarityLevel;

		public ActorGender Gender;

		public string History;

		public string Class;

		public bool IsAltHero;

		public bool IsSpecial;

		public bool IsEnvironmental;

		public string VisualAsset;

		public string OutfitDefinitionID;

		public bool IncludedInGacha;

		public bool IncludedInTokenPool;

		public Faction Faction;

		public int InitialActivationRange;

		public int InitialHealth;

		public int InitialMovementSpeed;

		public int InitialStruggleTurns;

		public int DamageMultiplier;

		public int HealthMultiplier;

		public int CommandSkill;

		public int InitialEquipmentRarityLevel;

		public List<EquipmentSetupData> InitialEquipmentsData;

		public List<string> InitialAbilities;

		public List<string> InitialTraits;

		public List<string> UpgradeTraits;

		public List<string> PvPTraits;

		public List<string> AttributeDefinition;

		public bool ShouldDestroyViewOnDeath;

		public int TokensToUnlock;

		public CurrencyType TraitUpgradeCurrency;

		public string UnlockDate;

		[GEDType(GEDSpecialType.TimeSeconds)]
		public long MinTimeToShowCountdown;

		public string AltOf;

		[GEDType(GEDSpecialType.TimeSeconds)]
		public int RespawnCD;

		public int ResourceCostBase;

		public int Defense;

		public int AttackSpeed;

		public int MovementSpeed;

		public int TD_ActorInitialHealth;

		public int TD_ActorHealthMultiplier;

		public int TD_ActorInitialDamage;

		public int TD_ActorDamageMultiplier;

		public List<string> TD_Equipment;

		public List<string> TD_PassiveSkill;

		public string TD_ActiveSkill;

		public List<string> TD_UltSkill;

		public bool TD_Available;

		public const string ActorDefinitionIdPrefix = "Default";

		public string Image;

		public string NormalHead;

		private long unlockTime;

		[JsonIgnore]
		public long UnlockTimeMilliseconds => unlockTime;

		[JsonIgnore]
		public bool HasUnlockDate => UnlockTimeMilliseconds > 0;

		[JsonIgnore]
		public bool IsNotBasicWalker
		{
			get
			{
				if ((Faction != Faction.Walker || !IsSpecial) && Faction != Faction.Raider)
				{
					return Faction == Faction.Any;
				}
				return true;
			}
		}

		public void SetUnlockTime(DateTime origin)
		{
			unlockTime = (long)(GameEconomyData.ParseDateTime(UnlockDate) - origin).TotalSeconds * 1000;
		}

		public bool IsAvailableToUnlock(long currentUTCTime)
		{
			if (UnlockTimeMilliseconds > 0)
			{
				return currentUTCTime > UnlockTimeMilliseconds;
			}
			return true;
		}

		public string GetNonAlternativeHeroDefinition()
		{
			string text = ID;
			if (!string.IsNullOrEmpty(AltOf))
			{
				text = AltOf;
			}
			else if (IsAltHero)
			{
				text = text.Replace(Class, "");
				text = text.Replace("Alt", "");
			}
			return text;
		}
	}
}
