using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class AbilityDefinition
	{
		public string Identifier;

		public AbilityType Type;

		public string DisplayName;

		public int DurationTurns;

		public int NoiseRange;

		public int ThreatValue;

		public int ChargePointsPerKill;

		public int ChargePointCost;

		public bool RequiresLineOfSight;

		public bool RequiresLineOfMovement;

		public AbilityTargetType TargetType;

		public AbilityTriggerType TriggerType;

		public int MaxAffectedTargetsCount;

		public FixedPoint SecondaryTargetsHitChance;

		public FixedPoint BodyShotChance;

		public bool IsPerformedAfterPlayerMove;

		public AbilityFireMode FireMode;

		public int ShotCount;

		public FixedPoint AbilityRange;

		public AbilityTargetAreaType AbilityTargetArea;

		public FixedPoint AbilityTargetAreaRadius;

		public FixedPoint AbilityTargetAreaAngle;

		public bool AbilityTargetDiagonal;

		public bool TargetAreaAtSource;

		public EffectSource EffectSource;

		public bool CanBeBlocked;

		public string DMGRangeDisplayImage;

		public List<AbilityEffectDefinition> EffectDefinitions;

		public List<EquipmentType> AllowedEquipmentTypes;

		public List<AbilityModifierDefinition> Modifiers;

		public string LinkedAbilityIdentifier;

		public int TurnsToReload;

		public bool HasFriendlyFire;

		public string SpecialDescriptionKey;

		public bool IsFreeAction;

		public bool IsAttack;

		public int InitialCooldown;

		public int CooldownAfterUse;

		[NonSerialized]
		[JsonIgnore]
		public bool Hidden;

		public bool LimitOOT;

		[JsonIgnore]
		public bool NeedsReloading => TurnsToReload > 0;
	}
}
