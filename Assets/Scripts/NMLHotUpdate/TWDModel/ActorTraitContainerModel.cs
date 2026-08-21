using System;
using System.Collections.Generic;

namespace TWDModel
{
	public class ActorTraitContainerModel : TWDModelObject
	{
		public const string InjuryLight = "InjuryLight";

		public const string InjuryMedium = "InjuryMedium";

		public const string InjuryHeavy = "InjuryHeavy";

		public const string Bitten = "Bitten";

		public const string BittenRaged = "BittenRaged";

		public const string Stiff = "Stiff";

		public const string Lucky = "Lucky";

		public const string EquipmentActiveStun = "Equipment_Active_Stun";

		public const string EquipmentActiveRiotShieldHerd = "Equipment_Active_RiotShield_Herd";

		public const string EquipmentActiveRiotShieldStun = "Equipment_Active_RiotShield_Stun";

		public const string EquipmentActiveEnsnare = "Ensnare";

		public const string EquipmentActiveFacehurt = "Facehurt";

		public const string EquipmentActiveExtraAP = "Equipment_Active_ExtraAP";

		public const string EquipmentActiveExtraDamageExecution = "Equipment_Active_ExtraDamageExecution";

		public const string EquipmentActiveCriticalPenetratesArmor = "Equipment_Active_CriticalPenetratesArmor";

		public const string EquipmentActiveCripple = "Equipment_Active_Cripple";

		public const string EquipmentActiveOverflow = "Overflow";

		public const string EquipmentActiveFollowThrough = "Equipment_Active_FollowThrough";

		public const string EquipmentActiveInterruptor = "Equipment_Active_Interruptor";

		public const string FieldMedic = "FieldMedic";

		public const string Soldier = "Soldier";

		public const string EquipmentSoldier = "Equipment.Soldier";

		public const string EquipmentBreakthrough = "Equipment.Breakthrough";

		public const string EquipmentKaboom = "Equipment.Kaboom";

		public const string BaseMeleeDodge = "BaseMeleeDodge";

		public const string BaseRangedDodge = "BaseRangedDodge";

		public const string BaseRetaliate = "BaseRetaliate";

		public const string BaseRevenge = "BaseRevenge";

		public const string Dodge = "Dodge";

		public const string EquipmentDodge = "Equipment.Dodge";

		public const string Jumpingshot = "Jumpingshot";

		public const string ResistJumpingshot = "ResistJumpingshot";

		public const string HealthBoost = "HealthBoost";

		public const string BoostTotalHealth = "BoostTotalHealth";

		public const string Strong = "Strong";

		public const string EquipmentStrong = "Equipment.Strong";

		public const string Weak = "Weak";

		public const string IronSkin = "IronSkin";

		public const string Wrestler = "Wrestler";

		public const string Farmer = "Farmer";

		public const string Gluttony = "Gluttony";

		public const string BloodThirst = "BloodThirst";

		public const string Trigger = "Trigger";

		public const string Overwatch = "Overwatch";

		public const string BodyShotBonus = "BodyShotBonus";

		public const string Lethal = "Lethal";

		public const string Accurate = "Accurate";

		public const string Destructive = "Destructive";

		public const string WideArc = "WideArc";

		public const string WideSpread = "WideSpread";

		public const string HighPowered = "HighPowered";

		public const string Concussion = "Concussion";

		public const string LargeCaliber = "LargeCaliber";

		public const string Inspiration = "Inspiration";

		public const string RetaliateMultiplier = "RetaliateMultiplier";

		public const string StruggleInvulnerable = "StruggleInvulnerable";

		public const string Interruptor = "Interruptor";

		public const string Protective = "Protective";

		public const string EquipmentProtective = "Equipment.Protective";

		public const string Charging = "Charging";

		public const string PowerStrike = "PowerStrike";

		public const string EquipmentPowerStrike = "Equipment.PowerStrike";

		public const string SureShot = "SureShot";

		public const string EquipmentSureShot = "Equipment.SureShot";

		public const string Bleeding = "Bleeding";

		public const string Burning = "Burning";

		public const string Skinned = "Skinned";

		public const string Equipment_Active_Skinned_1 = "Equipment_Active_Skinned_1";

		public const string Equipment_Active_Skinned_2 = "Equipment_Active_Skinned_2";

		public const string HelpHand = "HelpHand";

		public const string EquipmentHelpHand = "Equipment.HelpHand";

		public const string Ripped = "Ripped";

		public const string Explosive = "Explosive";

		public const string ExplosiveGoo = "ExplosiveGoo";

		public const string Impenetrable = "Impenetrable";

		public const string PushCollisionDamage = "PushCollisionDamage";

		public const string Silenced = "Silenced";

		public const string ThreatReduction = "ThreatReduction";

		public const string ThreatFree = "ThreatFree";

		public const string Bulletproof = "Bulletproof";

		public const string PointBlankShot = "PointBlankShot";

		public const string StunResistance = "StunResistance";

		public const string EquipmentStunResistance = "Equipment.StunResistance";

		public const string PassiveStunResistance = "PassiveAbilityAvoidStun";

		public const string BulletDodge = "BulletDodge";

		public const string EquipmentBulletDodge = "Equipment.BulletDodge";

		public const string DefensiveStance = "DefensiveStance";

		public const string EquipmentDefensiveStance = "Equipment.DefensiveStance";

		public const string SecondChance = "SecondChance";

		public const string Piercing = "Piercing";

		public const string Razor = "Razor";

		public const string Retaliate = "Retaliate";

		public const string EquipmentRetaliate = "Equipment.Retaliate";

		public const string FollowThrough = "FollowThrough";

		public const string EquipmentFollowThrough = "Equipment.FollowThrough";

		public const string CriticalAim = "CriticalAim";

		public const string EquipmentCriticalAim = "Equipment.CriticalAim";

		public const string Ruthless = "Ruthless";

		public const string Revenge = "Revenge";

		public const string EquipmentRevenge = "Equipment.Revenge";

		public const string ShieldRevenge = "ShieldRevenge";

		public const string EquipmentIncendiary = "Equipment.Incendiary";

		public const string EquipmentTactical = "Equipment.Tactical";

		public const string EquipmentArmorTactical = "Equipment.ArmorTactical";

		public const string PreventPush = "PreventPush";

		public const string PreventIncendiary = "PreventIncendiary";

		public const string EquipmentShield = "Equipment.Shield";

		public const string Punish = "Punish";

		public const string EquipmentPunish = "Equipment.Punish";

		public const string BasePunish = "BasePunish";

		public const string BaseBeatEmUp = "BaseBeatEmUp";

		public const string BaseRegalAuthority = "BaseRegalAuthority";

		public const string BoostFinalDamage = "BoostFinalDamage";

		public const string EquipmentSniperHarness = "Equipment.SniperHarness";

		public const string EquipmentTrainingGear = "Equipment.TrainingGear";

		public const string EquipmentHazardSuit = "Equipment.HazardSuit";

		public const string MeleeResistance = "MeleeResistance";

		public const string RangedResistance = "RangedResistance";

		public const string FireResistance = "FireResistance";

		public const string DistanceShield = "DistanceShield";

		public const string HealthThresholdedStatusResistance = "HealthThresholdedStatusResistance";

		public const string Whisperer = "Whisperer";

		public const string WhispererMelee = "Whisperer.Melee";

		public const string Gore = "Gore";

		public const string Crippling = "Crippling";

		public const string Perseverance = "Perseverance";

		public const string EquipmentPerseverance = "Equipment.Perseverance";

		public const string InfiniteRange = "InfiniteRange";

		public const string RangedDamageFalloff = "RangedDamageFalloff";

		public const string PrimedChance = "PrimedChance";

		public const string FirstStrike = "FirstStrike";

		public const string FirstStrikeAdditionalDamage = "FirstStrikeAdditionalDamage";

		public const string FirstStrikeDamageThreshold = "FirstStrikeDamageThreshold";

		public const string Stagger = "Stagger";

		public const string EquipmentStagger = "Equipment.Stagger";

		public const string EquipmentFollowStatusStagger = "Equipment.FollowStatus.Stagger";

		public const string StaggerActive = "StaggerActive";

		public const string StaggerChance = "StaggerChance";

		public const string StaggerActiveChargeChance = "StaggerActiveChargeChance";

		public const string Fortified = "Fortified";

		public const string FortifiedCriticalReduction = "FortifiedCriticalReduction";

		public const string FistSpike = "FistSpike";

		public const string Poison = "Poison";

		public const string Pestilence = "Pestilence";

		public const string PoisonBurst = "PoisonBurst";

		public const string RemoteRepulse = "RemoteRepulse";

		public const string RemoteWeakenActiveFlag = "RemoteWeakenActiveFlag";

		public const string Equipment_Passive_Range_Repulse_1 = "Equipment_Passive_Range_Repulse_1";

		public const string Equipment_Passive_Range_Repulse_2 = "Equipment_Passive_Range_Repulse_2";

		public const string ElectronCharge = "ElectronCharge";

		public const string ElectronShockAsElectronChargeLayer = "ElectronShockAsElectronChargeLayer";

		public const string Conductive = "Conductive";

		public const string AbilityModifierConductiveAdditionalDamagePercentage = "AbilityModifierConductiveAdditionalDamagePercentage";

		public const string CurrentSurge = "CurrentSurge";

		public const string VoltCharge = "VoltCharge";

		public const string AbilityModifierVoltChargeAdditionalDamagePercentage = "AbilityModifierVoltChargeAdditionalDamagePercentage";

		public const string VoltShock = "VoltShock";

		public const string Quantun = "Quantun";

		public const string SkillShieldType1 = "SkillShieldType1";

		public const string SkillEquipTauntShield = "SkillEquipTauntShield";

		public const string SkillIncreaseAttack = "SkillIncreaseAttack";

		public const string ResurgenceType1 = "ResurgenceType1";

		public const string ResurgenceType2 = "ResurgenceType2";

		public const string FirstAid = "FirstAid";

		public const string Momentum = "Momentum";

		public const string RandomStatus = "RandomStatus";

		public const string Fortuna_Heart = "Fortuna_Heart";

		public const string Fortuna_Club = "Fortuna_Club";

		public const string Fortuna_Spade = "Fortuna_Spade";

		public const string EquipmentActiveBloodMark = "Equipment.Active.BloodMark";

		public const string EquipmentActiveBloodMarkDesc = "EquipmentActiveBloodMarkDesc";

		public const string EquipmentPassiveBloodMark = "Equipment.Passive.BloodMark";

		public const string AbilityModifierEquipmentPassiveBloodMarkMoveDistance = "AbilityModifierEquipmentPassiveBloodMarkMoveDistance";

		public const string AbilityModifierEquipmentPassiveBloodMarkDamageCount = "AbilityModifierEquipmentPassiveBloodMarkDamageCount";

		public const string AbilityModifierEquipmentPassiveBloodMarkHealthPercentageNonBoss = "AbilityModifierEquipmentPassiveBloodMarkHealthPercentageNonBoss";

		public const string AbilityModifierEquipmentPassiveBloodMarkHealthPercentageBoss = "AbilityModifierEquipmentPassiveBloodMarkHealthPercentageBoss";

		public const string AbilityModifierEquipmentPassiveBloodMarkChance = "AbilityModifierEquipmentPassiveBloodMarkChance";

		public const string AbilityModifierEquipmentPassiveBloodMarkDamagePercentage = "AbilityModifierEquipmentPassiveBloodMarkDamagePercentage";

		public const string AbilityModifierEquipmentPassiveBloodMarkRange = "AbilityModifierEquipmentPassiveBloodMarkRange";

		public const string AbilityModifierEquipmentPassiveBloodMarkDamageLimit = "AbilityModifierEquipmentPassiveBloodMarkDamageLimit";

		public const string EquipmentPassiveRemoveNegative = "Equipment.Passive.RemoveNegative";

		public const string EquipmentPassivePreventControl = "Equipment.Passive.PreventControl";

		public const string AbilityModifierEquipmentPassivePreventControlChance = "AbilityModifierEquipmentPassivePreventControlChance";

		public const string EquipmentPassiveMaxGetHitDamage = "Equipment.Passive.MaxGetHitDamage";

		public const string AbilityModifierEquipmentPassiveMaxGetHitDamageNormalCap = "AbilityModifierEquipmentPassiveMaxGetHitDamageNormalCap";

		public const string AbilityModifierEquipmentPassiveMaxGetHitDamageBossCap = "AbilityModifierEquipmentPassiveMaxGetHitDamageBossCap";

		public const string EquipmentPassiveDamageAreaBlock = "Equipment.Passive.DamageAreaBlock";

		public const string AbilityModifierEquipmentPassiveDamageAreaBlockNormalRadiusReduction = "AbilityModifierEquipmentPassiveDamageAreaBlockNormalRadiusReduction";

		public const string AbilityModifierEquipmentPassiveDamageAreaBlockBossRadiusReduction = "AbilityModifierEquipmentPassiveDamageAreaBlockBossRadiusReduction";

		public const string AbilityModifierEquipmentPassiveDamageAreaBlockNormalMinimumRadius = "AbilityModifierEquipmentPassiveDamageAreaBlockNormalMinimumRadius";

		public const string AbilityModifierEquipmentPassiveDamageAreaBlockBossMinimumRadius = "AbilityModifierEquipmentPassiveDamageAreaBlockBossMinimumRadius";

		public const string EquipmentPassiveLineSeparatedPlus = "Equipment.Passive.LineSeparatedPlus";

		public const string AbilityModifierLineSeparatedMiddleRangePlus = "AbilityModifierLineSeparatedMiddleRangePlus";

		public const string AbilityModifierLineSeparatedSideRangePlus = "AbilityModifierLineSeparatedSideRangePlus";

		public const string RangeArmorDominance = "RangeArmorDominance";

		public const string AbilityModifierArmorAttackingMoreNFrames = "AbilityModifierArmorAttackingMoreNFrames";

		public const string AbilityModifierArmorIncreaseInDamage = "AbilityModifierArmorIncreaseInDamage";

		public const string AbilityModifierArmorIncreaseNFrame = "AbilityModifierArmorIncreaseNFrame";

		public const string AbilityModifierArmorDamageBoost = "AbilityModifierArmorDamageBoost";

		public const string AbilityModifierArmorDamageBoostLimit = "AbilityModifierArmorDamageBoostLimit";

		public const string RangeEquipmentDominance = "RangeEquipmentDominance";

		public const string AbilityModifierEquipmentAttackingMoreNFrames = "AbilityModifierEquipmentAttackingMoreNFrames";

		public const string AbilityModifierEquipmentIncreaseInDamage = "AbilityModifierEquipmentIncreaseInDamage";

		public const string AbilityModifierEquipmentIncreaseNFrame = "AbilityModifierEquipmentIncreaseNFrame";

		public const string AbilityModifierEquipmentDamageBoost = "AbilityModifierEquipmentDamageBoost";

		public const string AbilityModifierEquipmentDamageBoostLimit = "AbilityModifierEquipmentDamageBoostLimit";

		public const string RangeActorDominance = "RangeActorDominance";

		public const string AbilityModifierActorAttackingMoreNFrames = "AbilityModifierActorAttackingMoreNFrames";

		public const string AbilityModifierActorIncreaseInDamage = "AbilityModifierActorIncreaseInDamage";

		public const string AbilityModifierActorIncreaseNFrame = "AbilityModifierActorIncreaseNFrame";

		public const string AbilityModifierActorDamageBoost = "AbilityModifierActorDamageBoost";

		public const string AbilityModifierActorDamageBoostLimit = "AbilityModifierActorDamageBoostLimit";

		public const string AddDamageNormalAttack = "AddDamage.NormalAttack";

		public const string AbilityModifierAddDamageNormalAttackMinHPPercentage = "AbilityModifierAddDamageNormalAttackMinHPPercentage";

		public const string AbilityModifierAddDamageNormalAttackMaxHPPercentage = "AbilityModifierAddDamageNormalAttackMaxHPPercentage";

		public const string AbilityModifierAddDamageNormalAttackExtraDamagePercentage = "AbilityModifierAddDamageNormalAttackExtraDamagePercentage";

		public const string AddDamageAddAttack = "AddDamage.AddAttack";

		public const string AbilityModifierAddDamageAddAttackMinHPPercentage = "AbilityModifierAddDamageAddAttackMinHPPercentage";

		public const string AbilityModifierAddDamageAddAttackMaxHPPercentage = "AbilityModifierAddDamageAddAttackMaxHPPercentage";

		public const string AbilityModifierAddDamageAddAttackExtraDamagePercentage = "AbilityModifierAddDamageAddAttackExtraDamagePercentage";

		public const string AddDamageChargeAttack = "AddDamage.ChargeAttack";

		public const string AbilityModifierAddDamageChargeAttackMinHPPercentage = "AbilityModifierAddDamageChargeAttackMinHPPercentage";

		public const string AbilityModifierAddDamageChargeAttackMaxHPPercentage = "AbilityModifierAddDamageChargeAttackMaxHPPercentage";

		public const string AbilityModifierAddDamageChargeAttackExtraDamagePercentage = "AbilityModifierAddDamageChargeAttackExtraDamagePercentage";

		public const string FreeChargePoint = "FreeChargePoint";

		public const string FreeChargePointNonConsumeChargePointPercentage = "FreeChargePointNonConsumeChargePointPercentage";

		public const string BoostHitRate = "BoostHitRate";

		public const string AbilityModifierBoostHitRatePercentage = "AbilityModifierBoostHitRatePercentage";

		public const string IgnoreDefense = "IgnoreDefense";

		public const string AbilityModifierIgnoreDefensePercentage = "AbilityModifierIgnoreDefensePercentage";

		public const string LeaderBuffNoThreatRangedPercentageIncreaseChargePoint = "LeaderBuffNoThreatRangedPercentageIncreaseChargePoint";

		public const string LeaderBuffNoThreatRangedIncreaseChargePoint = "LeaderBuffNoThreatRangedIncreaseChargePoint";

		public const string LeaderBuffNoThreatRangedCriticalIncreaseDamage = "LeaderBuffNoThreatRangedCriticalIncreaseDamage";

		public const string FlatBaseDamage = "FlatBaseDamage";

		public const string FlatHealth = "FlatHealth";

		public const string FlatCritDamage = "FlatCritDamage";

		public const string FreeAttack = "FreeAttack";

		public const string EmptyTactical = "EmptyTactical";

		public const string TutorialSetDamage = "TutorialSetDamage";

		public const string TutorialUninterruptable = "TutorialUninterruptable";

		public const string TutorialInvulnerable = "TutorialInvulnerable";

		public const string FactionBuffTrait = "FactionBuffTrait";

		public const string LeaderBuffFinalDamage = "LeaderBuffFinalDamage";

		public const string LeaderBuffShooter = "LeaderBuffShooter";

		public const string LeaderBuffHunter = "LeaderBuffHunter";

		public const string LeaderBuffAssault = "LeaderBuffAssault";

		public const string LeaderBuffWarrior = "LeaderBuffWarrior";

		public const string LeaderBuffBruiser = "LeaderBuffBruiser";

		public const string LeaderBuffScout = "LeaderBuffScout";

		public const string LeaderBuffRanged = "LeaderBuffRanged";

		public const string LeaderBuffMelee = "LeaderBuffMelee";

		public const string LeaderBuffQuickLearner = "LeaderBuffQuickLearner";

		public const string LeaderBuffLooter = "LeaderBuffLooter";

		public const string LeaderBuffKiller = "LeaderBuffKiller";

		public const string LeaderBuffNoThreatRanged = "LeaderBuffNoThreatRanged";

		public const string LeaderBuffReduceThreatMelee = "LeaderBuffReduceThreatMelee";

		public const string LeaderBuffCoverDamageReduction = "LeaderBuffCoverDamageReduction";

		public const string LeaderBuffDontTouchMyAllies = "LeaderBuffDontTouchMyAllies";

		public const string LeaderBuffCriticalChance = "LeaderBuffCriticalChance";

		public const string LeaderBuffCriticalResistance = "LeaderBuffCriticalResistance";

		public const string LeaderBuffExtraChargePointAtAttackDmgTaken = "LeaderBuffExtraChargePointAtAttackDmgTaken";

		public const string LeaderBuffSecondChance = "LeaderBuffSecondChance";

		public const string LeaderBuffHealingCharge = "LeaderBuffHealingCharge";

		public const string LeaderBuffDeadlyTactics = "LeaderBuffDeadlyTactics";

		public const string LeaderBuffNeedOnlyOne = "LeaderBuffNeedOnlyOne";

		public const string LeaderBuffSurvivalInstinct = "LeaderBuffSurvivalInstinct";

		public const string LeaderBuffBringThemOn = "LeaderBuffBringThemOn";

		public const string LeaderBuffGoodOutOfBad = "LeaderBuffGoodOutOfBad";

		public const string LeaderBuffHunterDesperation = "LeaderBuffHunterDesperation";

		public const string LeaderBuffRegalAuthority = "LeaderBuffRegalAuthority";

		public const string LeaderBuffMulletTime = "LeaderBuffMulletTime";

		public const string LeaderBuffMysteriousWays = "LeaderBuffMysteriousWays";

		public const string LeaderBuffTeamwork = "LeaderBuffTeamwork";

		public const string LeaderBuffOnlyTheBest = "LeaderBuffOnlyTheBest";

		public const string LeaderBuffJackass = "LeaderBuffJackass";

		public const string LeaderBuffBodyguard = "LeaderBuffBodyguard";

		public const string LeaderBuffReadyForAction = "LeaderBuffReadyForAction";

		public const string LeaderBuffPerceptive = "LeaderBuffPerceptive";

		public const string LeaderBuffLeadByExample = "LeaderBuffLeadByExample";

		public const string LeaderBuffForestStalker = "LeaderBuffForestStalker";

		public const string LeaderBuffJustice = "LeaderBuffJustice";

		public const string LeaderBuffColdBlooded = "LeaderBuffColdBlooded";

		public const string LeaderBuffOneWithTheHerd = "LeaderBuffOneWithTheHerd";

		public const string LeaderBuffOneWithTheHerdStalker = "LeaderBuffOneWithTheHerdStalker";

		public const string LeaderBuffExplosiveBullets = "LeaderBuffExplosiveBullets";

		public const string LeaderBuffBeatEmUp = "LeaderBuffBeatEmUp";

		public const string LeaderBuffInspire = "LeaderBuffInspire";

		public const string LeaderBuffPrincess = "LeaderBuffPrincess";

		public const string LeaderBuffPrincessExtraDamage = "LeaderBuffPrincess.ExtraDamage";

		public const string LeaderBuffPrincessExtraChargePoints = "LeaderBuffPrincess.ExtraChargePoints";

		public const string LeaderBuffFiringSquad = "LeaderBuffFiringSquad";

		public const string LeaderBuffFiringSquadDamageMultiplier = "LeaderBuffFiringSquad";

		public const string LeaderBuffSurvivalGame = "LeaderBuffSurvivalGame";

		public const string BaseSurvivalGame = "BaseSurvivalGame";

		public const string LeaderBuffSurvivalGame_TraitDis = "LeaderBuffSurvivalGame_TraitDis";

		public const string LeaderBuffSurvivalGame_MaxTurns = "LeaderBuffSurvivalGame_MaxTurns";

		public const string LeaderBuffSurvivalGame_CDTurns = "LeaderBuffSurvivalGame_CDTurns";

		public const string LeaderBuffSurvivalGame_NoDeadLevel = "LeaderBuffSurvivalGame_NoDeadLevel";

		public const string LeaderBuffSurvivalGame_NoDeadMaxCount = "LeaderBuffSurvivalGame_NoDeadMaxCount";

		public const string LeaderBuffSurvivalGame_DmgUp = "LeaderBuffSurvivalGame_DmgUp";

		public const string LeaderBuffSurvivalGame_MoveDisDown = "LeaderBuffSurvivalGame_MoveDisDown";

		public const string LeaderBuffSurvivalGame_DmgUpEachEff = "LeaderBuffSurvivalGame_DmgUpEachEff";

		public const string LeaderBuffSurvivalGame_ChanceStun = "LeaderBuffSurvivalGame_ChanceStun";

		public const string LeaderBuffSurvivalGame_HealPer = "LeaderBuffSurvivalGame_HealPer";

		public const string LeaderBuffSurvivalGame_LuckyDis = "LeaderBuffSurvivalGame_LuckyDis";

		public const string LeaderBuffDeadlyFocus = "LeaderBuffDeadlyFocus";

		public const string BaseDeadlyFocus = "BaseDeadlyFocus";

		public const string LeaderBuffDeadlyFocus_BuffEnemyMaxCount = "LeaderBuffDeadlyFocus_BuffEnemyMaxCount";

		public const string LeaderBuffDeadlyFocus_BuffMaxTurns = "LeaderBuffDeadlyFocus_BuffMaxTurns";

		public const string LeaderBuffDeadlyFocus_PursuitChance = "LeaderBuffDeadlyFocus_PursuitChance";

		public const string LeaderBuffDeadlyFocus_PursuitDmgPer = "LeaderBuffDeadlyFocus_PursuitDmgPer";

		public const string LeaderBuffDeadlyFocus_ChargePursuitChance = "LeaderBuffDeadlyFocus_ChargePursuitChance";

		public const string LeaderBuffDeadlyFocus_DmgUpPerKill = "LeaderBuffDeadlyFocus_DmgUpPerKill";

		public const string LeaderBuffDeadlyFocus_DmgUpPerKill_Max = "LeaderBuffDeadlyFocus_DmgUpPerKill_Max";

		public const string LeaderBuffDeadlyFocus_LevelReq_KilledTransDis = "LeaderBuffDeadlyFocus_LevelReq_KilledTransDis";

		public const string LeaderBuffDeadlyFocus_KilledTransDis = "LeaderBuffDeadlyFocus_KilledTransDis";

		public const string LeaderBuffDeadlyFocus_LevelReq_ExApChance = "LeaderBuffDeadlyFocus_LevelReq_ExApChance";

		public const string LeaderBuffDeadlyFocus_ExApChance = "LeaderBuffDeadlyFocus_ExApChance";

		public const string LeaderBuffDeadlyFocus_LevelReq_ChargeBuff = "LeaderBuffDeadlyFocus_LevelReq_ChargeBuff";

		public const string LeaderBuffDeadlyFocus_LevelReq_ExDmgHitRate = "LeaderBuffDeadlyFocus_LevelReq_ExDmgHitRate";

		public const string LeaderBuffDeadlyFocus_ExDmgHitRate_ExDmg = "LeaderBuffDeadlyFocus_ExDmgHitRate_ExDmg";

		public const string LeaderBuffDeadlyFocus_ExDmgHitRate_HitRate = "LeaderBuffDeadlyFocus_ExDmgHitRate_HitRate";

		public const string LeaderBuffShadowedGuard = "LeaderBuffShadowedGuard";

		public const string BaseShadowedGuard = "BaseShadowedGuard";

		public const string ShadowedGuard_StateRef = "ShadowedGuard_StateRef";

		public const string LeaderBuffShadowedGuard_HpDmg = "LeaderBuffShadowedGuard_HpDmg";

		public const string LeaderBuffShadowedGuard_Charge_AtkNum = "LeaderBuffShadowedGuard_Charge_AtkNum";

		public const string LeaderBuffShadowedGuard_Charge_UnderAtkNum = "LeaderBuffShadowedGuard_Charge_UnderAtkNum";

		public const string LeaderBuffShadowedGuard_Charge_MaxNum = "LeaderBuffShadowedGuard_Charge_MaxNum";

		public const string LeaderBuffShadowedGuard_Hp_PerReduce = "LeaderBuffShadowedGuard_Hp_PerReduce";

		public const string LeaderBuffShadowedGuard_Hp_PerChange = "LeaderBuffShadowedGuard_Hp_PreChange";

		public const string LeaderBuffShadowedGuard_MaxTurns = "LeaderBuffShadowedGuard_MaxTurns";

		public const string LeaderBuffShadowedGuard_CDTurns = "LeaderBuffShadowedGuard_CDTurns";

		public const string LeaderBuffShadowedGuard_Level_Resist = "LeaderBuffShadowedGuard_Level_Resist";

		public const string LeaderBuffShadowedGuard_Level_Resist_Per = "LeaderBuffShadowedGuard_Level_Resist_Per";

		public const string LeaderBuffShadowedGuard_Level_Recover = "LeaderBuffShadowedGuard_Level_Recover";

		public const string LeaderBuffShadowedGuard_Level_Charge = "LeaderBuffShadowedGuard_Level_Charge";

		public const string LeaderBuffShadowedGuard_Add_Charge = "LeaderBuffShadowedGuard_Add_Charge";

		public const string FiringSquadDamageActive = "FiringSquadDamageActive";

		public const string FiringSquadDamageActiveMultiplier = "FiringSquadDamageActiveMultiplier";

		public const string FiringSquadLeader = "FiringSquadLeader";

		public const string FiringSquadMember = "FiringSquadMember";

		public const string LeaderBuffMarkEnemy = "LeaderBuffMarkEnemy";

		public const string LeaderBuffMarkEnemyExtraDamage = "LeaderBuffMarkEnemy.ExtraDamage";

		public const string LeaderBuffMarkEnemyDamageReduction = "LeaderBuffMarkEnemy.DamageReduction";

		public const string DebuffMarkEnemy = "DebuffMarkEnemy";

		public const string DebuffEquipmentKaboom = "DebuffEquipmentKaboom";

		public const string LeaderBuffGoodEnough = "LeaderBuffGoodEnough";

		public const string LeaderBuffGoodEnoughCrippleBase = "LeaderBuffGoodEnoughCrippleBase";

		public const string LeaderBuffGoodEnoughStaggerBase = "LeaderBuffGoodEnoughStaggerBase";

		public const string LeaderBuffGoodEnoughCrippleChance = "LeaderBuffGoodEnoughCrippleChance";

		public const string LeaderBuffGoodEnoughStaggerChance = "LeaderBuffGoodEnoughStaggerChance";

		public const string LeaderBuffGoodEnoughStaggerChargeChance = "LeaderBuffGoodEnoughStaggerChargeChance";

		public const string LeaderBuffEmitter = "LeaderBuffEmitter";

		public const string EmitterCreator = "EmitterCreator";

		public const string LeaderBuffEmitterDamageMultiplier = "LeaderBuffEmitterDamageMultiplier";

		public const string LeaderBuffEmitterDuration = "LeaderBuffEmitterDuration";

		public const string LeaderBuffEmitterRadius = "LeaderBuffEmitterRadius";

		public const string LeaderBuffEmitterMaxMergedRadius = "LeaderBuffEmitterMaxMergedRadius";

		public const string EmitterDamageActive = "EmitterDamageActive";

		public const string EmitterDamageActiveMultiplier = "EmitterDamageActiveMultiplier";

		public const string LeaderBuffHeadshot = "LeaderBuffHeadshot";

		public const string LeaderBuffHeadshotCurrentHealthDamageChance = "LeaderBuffHeadshotCurrentHealthDamageChance";

		public const string LeaderBuffHeadshotCurrentHealthDamageMultiplierRanged = "LeaderBuffHeadshotCurrentHealthDamageMultiplierRanged";

		public const string LeaderBuffHeadshotCurrentHealthDamageMultiplierMelee = "LeaderBuffHeadshotCurrentHealthDamageMultiplierMelee";

		public const string LeaderBuffHeadshotStatusAvoidChance = "LeaderBuffHeadshotStatusAvoidChance";

		public const string BaseHeadshot = "BaseHeadshot";

		public const string LeaderBuffClosingTime = "LeaderBuffClosingTime";

		public const string LeaderBuffClosingTimeRange = "LeaderBuffClosingTimeRange";

		public const string LeaderBuffClosingTimeMainTargetDamageChance = "LeaderBuffClosingTimeMainTargetDamageChance";

		public const string LeaderBuffClosingTimeSecondaryTargetDamageChance = "LeaderBuffClosingTimeSecondaryTargetDamageChance";

		public const string BaseClosingTime = "BaseClosingTime";

		public const string LeaderBuffRedact = "LeaderBuffRedact";

		public const string LeaderBuffRedactStunChance = "LeaderBuffRedactStunChance";

		public const string LeaderBuffRedactChance = "LeaderBuffRedactChance";

		public const string LeaderBuffRedactMaxLayers = "LeaderBuffRedactMaxLayers";

		public const string LeaderBuffRedactIncreaseHumanDamage = "LeaderBuffRedactIncreaseHumanDamage";

		public const string LeaderBuffRedactReduceWalkerHpChance = "LeaderBuffRedactReduceWalkerHpChance";

		public const string LeaderBuffRedactReduceWalkerHpRatio = "LeaderBuffRedactReduceWalkerHpRatio";

		public const string LeaderBuffProtect = "LeaderBuffProtect";

		public const string LeaderBuffProtectDamageChance = "LeaderBuffProtectDamageChance";

		public const string LeaderBuffProtectTauntTurns = "LeaderBuffProtectTauntTurns";

		public const string LeaderBuffProtectChargeDamageChance = "LeaderBuffProtectChargeDamageChance";

		public const string LeaderBuffProtectChargeTauntTurns = "LeaderBuffProtectChargeTauntTurns";

		public const string LeaderBuffProtectShieldChance = "LeaderBuffProtectShieldChance";

		public const string LeaderBuffProtectShieldTurns = "LeaderBuffProtectShieldTurns";

		public const string LeaderBuffProtectLeaderShieldChance = "LeaderBuffProtectLeaderShieldChance";

		public const string LeaderBuffProtectLeaderShieldTurns = "LeaderBuffProtectLeaderShieldTurns";

		public const string LeaderBuffCoupDeGrace = "LeaderBuffCoupDeGrace";

		public const string LeaderBuffCoupDeGraceFollowUpProbability = "LeaderBuffCoupDeGraceFollowUpProbability";

		public const string LeaderBuffCoupDeGraceFollowUpDamage = "LeaderBuffCoupDeGraceFollowUpDamage";

		public const string LeaderBuffCoupDeGraceChargeProbability = "LeaderBuffCoupDeGraceChargeProbability";

		public const string BaseCoupDeGrace = "BaseCoupDeGrace";

		public const string CoupDeGraceActive = "CoupDeGraceActive";

		public const string LeaderBuffMadeToSuffer = "LeaderBuffMadeToSuffer";

		public const string SufferCreator = "SufferCreator";

		public const string SufferActive = "SufferActive";

		public const string LeaderBuffMadeToSufferDotRatio = "LeaderBuffMadeToSufferDotRatio";

		public const string LeaderBuffMadeToSufferMaxAreasLeader = "LeaderBuffMadeToSufferMaxAreasLeader";

		public const string LeaderBuffMadeToSufferMaxAreasNonLeader = "LeaderBuffMadeToSufferMaxAreasNonLeader";

		public const string LeaderBuffMadeToSufferMaxAreasDuration = "LeaderBuffMadeToSufferMaxAreasDuration";

		public const string LeaderBuffMadeToSufferRadius = "LeaderBuffMadeToSufferRadius";

		public const string LeaderBuffUnleashedFighter = "LeaderBuffUnleashedFighter";

		public const string BaseUnleashedFighter = "BaseUnleashedFighter";

		public const string UnleashedActive = "UnleashedActive";

		public const string LeaderBuffUnleashedFighterAreaGridLength = "LeaderBuffUnleashedFighterAreaGridLength";

		public const string LeaderBuffUnleashedFighterAreasDurationLeader = "LeaderBuffUnleashedFighterAreasDurationLeader";

		public const string LeaderBuffUnleashedFighterExtraDamageLeader = "LeaderBuffUnleashedFighterExtraDamageLeader";

		public const string LeaderBuffUnleashedFighterCoolingPeriodLeader = "LeaderBuffUnleashedFighterCoolingPeriodLeader";

		public const string LeaderBuffUnleashedFighterCoolingPeriodShare = "LeaderBuffUnleashedFighterCoolingPeriodShare";

		public const string LeaderBuffUnleashedMaxAreas = "LeaderBuffUnleashedMaxAreas";

		public const string LeaderBuffUnleashedFighterRemoteAreaGridLength = "LeaderBuffUnleashedFighterRemoteAreaGridLength";

		public const string LeaderBuffFightingFury = "LeaderBuffFightingFury";

		public const string LeaderBuffFightingFuryMaxAddAttacks = "LeaderBuffFightingFuryMaxAddAttacks";

		public const string LeaderBuffFightingFuryMaxAddAttacksLeader = "LeaderBuffFightingFuryMaxAddAttacksLeader";

		public const string LeaderBuffFightingFuryDamageModifier = "LeaderBuffFightingFuryDamageModifier";

		public const string BaseFightingFury = "BaseFightingFury";

		public const string FightingFury = "FightingFury";

		public const string LeaderBuffBetterTogether = "LeaderBuffBetterTogether";

		public const string LeaderBuffBetterTogetherExtraChargePointChance = "LeaderBuffBetterTogetherExtraChargePointChance";

		public const string LeaderBuffBetterTogetherAdditionalDamageModifier = "LeaderBuffBetterTogetherAdditionalDamageModifier";

		public const string LeaderBuffBetterTogetherSurvivorDistance = "LeaderBuffBetterTogetherSurvivorDistance";

		public const string BaseBetterTogether = "BaseBetterTogether";

		public const string FeaturedHeroBuffBase = "FeaturedHeroBuff";

		public const string FeaturedHeroBuffDamage = "FeaturedHeroBuff.Damage";

		public const string FeaturedHeroBuffHealth = "FeaturedHeroBuff.Health";

		public const string FeaturedHeroBuffRarity = "FeaturedHeroBuff.Rarity";

		public const string OccupationTag = "Occupation";

		public const string ResourceProdTag = "ResourceProd";

		public const string AbilityTag = "Ability";

		public const string BoosterTag = "Booster";

		public const string BonusTag = "Bonus";

		public const string GuildBattleBuff = "GuildBattleBuff";

		public const string PersonalityTag = "Personality";

		public const string PersonalityFlipFlopFetish = "FlipflopFetish";

		public const string PvP_HumanVsHumanDamageResistance = "PvP_HumanVsHumanDamageResistance";

		public const string PVP_SurvivorVsRaiderDamageResistance = "PVP_SurvivorVsRaiderDamageMultiplier";

		public const string PVP_RaiderVsSurvivorDamageResistance = "PVP_RaiderVsSurvivorDamageMultiplier";

		public const string PVP_RaiderVsSurvivorDamageModifierTrait = "PVP_RaiderVsSurvivorDamageModifierTrait";

		public const string InjuryTag = "Injury";

		public const string AbilityModifierDamage = "Damage";

		public const string AbilityModifierHealth = "Health";

		public const string AbilityModifierPercentageIncreaseBaseDamage = "PercentageIncreaseBaseDamage";

		public const string AbilityModifierPercentageIncreaseHealing = "PercentageIncreaseHealing";

		public const string AbilityModifierPercentageIncreaseRangeDamage = "PercentageIncreaseRangeDamage";

		public const string AbilityModifierPercentageNewIncreaseRangeDamage = "PercentageNewIncreaseRangeDamage";

		public const string AbilityModifierPercentageIncreaseRangeDamageInCover = "PercentageIncreaseRangeDamageInCover";

		public const string AbilityModifierPercentageIncreaseMeleeDamage = "PercentageIncreaseMeleeDamage";

		public const string AbilityModifierEquipPercentageIncreaseMeleeDamage = "AbilityModifierEquipPercentageIncreaseMeleeDamage";

		public const string AbilityModifierPercentageIncreaseOverwatchDamage = "PercentageIncreaseOverwatchDamage";

		public const string AbilityModifierPercentageIncreaseNewOverwatchDamage = "PercentageIncreaseNewOverwatchDamage";

		public const string AbilityModifierPercentageIncreaseResistance = "AbilityModifierPercentageIncreaseResistance";

		public const string AbilityModifierPercentageIncreaseResistanceMelee = "AbilityModifierPercentageIncreaseResistanceMelee";

		public const string AbilityModifierPercentageIncreaseResistanceMeleeArmor = "AbilityModifierPercentageIncreaseResistanceMeleeArmor";

		public const string AbilityModifierPercentageIncreaseResistanceRanged = "AbilityModifierPercentageIncreaseResistanceRanged";

		public const string AbilityModifierPercentageDecreaseResistance = "AbilityModifierPercentageDecreaseResistance";

		public const string AbilityModifierPercentageIncreaseResistanceHumanVsHuman = "AbilityModifierPercentageIncreaseResistanceHumanVsHuman";

		public const string AbilityModifierPercentageIncreaseResistanceOverwatch = "AbilityModifierPercentageIncreaseResistanceOverwatch";

		public const string AbilityModifierPercentageIncreaseNewResistanceOverwatch = "AbilityModifierPercentageIncreaseNewResistanceOverwatch";

		public const string AbilityModifierPercentageIncreaseTargetDamageNextToAlly = "AbilityModifierPercentageIncreaseTargetDamageNextToAlly";

		public const string AbilityModifierPercentageReduceJumpingshotDamage = "AbilityModifierPercentageReduceJumpingshotDamage";

		public const string AbilityModifierPercentageIncreaseResistanceCriticalDamageFromHumans = "AbilityModifierPercentageIncreaseResistanceCriticalDamageFromHumans";

		public const string AbilityModifierPercentageIncreaseCriticalChance = "AbilityModifierPercentageIncreaseCriticalChance";

		public const string AbilityModifierIncreaseCriticalChanceResistance = "AbilityModifierIncreaseCriticalChanceResistance";

		public const string AbilityModifierCarolCriticalChance = "AbilityModifierCarolCriticalChance";

		public const string AbilityModifierCarolCriticalDamage = "AbilityModifierCarolCriticalDamage";

		public const string AbilityModifierCarolNoAttackTurn = "AbilityModifierCarolNoAttackTurn";

		public const string AbilityModifierCarolCannotAttackedChance = "AbilityModifierCarolCannotAttackedChance";

		public const string AbilityModifierCarolAddFirstSpikeChance = "AbilityModifierCarolAddFirstSpikeChance";

		public const string AbilityModifierCarolFirstSpikeGrid = "AbilityModifierCarolFirstSpikeGrid";

		public const string AbilityModifierCarolFirstSpikeTurn = "AbilityModifierCarolFirstSpikeTurn";

		public const string AbilityModifierPercentageIncreaseRangedCriticalChance = "AbilityModifierPercentageIncreaseRangedCriticalChance";

		public const string AbilityModifierPercentageIncreaseAvoidDeathChance = "AbilityModifierPercentageIncreaseAvoidDeathChance";

		public const string AbilityModifierPercentageIncreaseCriticalChanceNoMove = "PercentageIncreaseCriticalChanceNoMove";

		public const string AbilityModifierPercentageIncreaseResistanceSurvivorVsRaider = "AbilityModifierPercentageIncreaseResistanceSurvivorVsRaider";

		public const string AbilityModifierPercentageIncreaseResistanceRaiderVsSurvivor = "AbilityModifierPercentageIncreaseResistanceRaiderVsSurvivor";

		public const string AbilityModifierPercentageMultiplyFinalDamageIncrementer = "AbilityModifierPercentageMultiplyFinalDamageIncrementer";

		public const string AbilityModifierPercentageMultiplyFinalDamageIncrementerBadges = "AbilityModifierPercentageMultiplyFinalDamageIncrementerBadges";

		public const string AbilityModifierPercentageMultiplyFinalDamage = "AbilityModifierPercentageMultiplyFinalDamage";

		public const string AbilityModifierPercentageMultiplyFinalDamageShooter = "AbilityModifierPercentageMultiplyFinalDamageShooter";

		public const string AbilityModifierPercentageMultiplyFinalDamageHunter = "AbilityModifierPercentageMultiplyFinalDamageHunter";

		public const string AbilityModifierPercentageMultiplyFinalDamageAssault = "AbilityModifierPercentageMultiplyFinalDamageAssault";

		public const string AbilityModifierPercentageMultiplyFinalDamageWarrior = "AbilityModifierPercentageMultiplyFinalDamageWarrior";

		public const string AbilityModifierPercentageMultiplyFinalDamageBruiser = "AbilityModifierPercentageMultiplyFinalDamageBruiser";

		public const string AbilityModifierPercentageMultiplyFinalDamageScout = "AbilityModifierPercentageMultiplyFinalDamageScout";

		public const string AbilityModifierPercentageMultiplyFinalDamageRanged = "AbilityModifierPercentageMultiplyFinalDamageRanged";

		public const string AbilityModifierPercentageMultiplyFinalDamageMelee = "AbilityModifierPercentageMultiplyFinalDamageMelee";

		public const string AbilityModifierPercentageMultiplyFinalDamageNoMove = "AbilityModifierPercentageMultiplyFinalDamageNoMove";

		public const string AbilityModifierPercentageMultiplyFinalNewDamageNoMove = "AbilityModifierPercentageMultiplyFinalNewDamageNoMove";

		public const string AbilityModifierPercentageMultiplyFinalDamageVsHumans = "AbilityModifierPercentageMultiplyFinalDamageVsHumans";

		public const string AbilityModifierPercentageMultiplyHealthShooter = "AbilityModifierPercentageMultiplyHealthShooter";

		public const string AbilityModifierPercentageMultiplyHealthHunter = "AbilityModifierPercentageMultiplyHealthHunter";

		public const string AbilityModifierPercentageMultiplyHealthAssault = "AbilityModifierPercentageMultiplyHealthAssault";

		public const string AbilityModifierPercentageMultiplyHealthWarrior = "AbilityModifierPercentageMultiplyHealthWarrior";

		public const string AbilityModifierPercentageMultiplyHealthBruiser = "AbilityModifierPercentageMultiplyHealthBruiser";

		public const string AbilityModifierPercentageMultiplyHealthScout = "AbilityModifierPercentageMultiplyHealthScout";

		public const string AbilityModifierPercentageMultiplyHealthRanged = "AbilityModifierPercentageMultiplyHealthRanged";

		public const string AbilityModifierPercentageMultiplyHealthMelee = "AbilityModifierPercentageMultiplyHealthMelee";

		public const string AbilityModifierPercentageMultiplyHealthAll = "AbilityModifierPercentageMultiplyHealthAll";

		public const string AbilityModifierPercentageMultiplyFinalDamageFeaturedHero = "AbilityModifierPercentageMultiplyFinalDamageFeaturedHero";

		public const string AbilityModifierPercentageMultiplyHealthFeaturedHero = "AbilityModifierPercentageMultiplyHealthFeaturedHero";

		public const string AbilityModifierRarityModifierFeaturedHero = "AbilityModifierRarityModifierFeaturedHero";

		public const string RangedDamageFalloffRange = "RangedDamageFalloffRange";

		public const string RangedDamageFalloffMultiplier = "RangedDamageFalloffMultiplier";

		public const string HelpHandGuardianshipProbability = "HelpHandGuardianshipProbability";

		public const string HelpHandNumberOfGuardianGrids = "HelpHandNumberOfGuardianGrids";

		public const string HelpHandGuardianDamageValues = "HelpHandGuardianDamageValues";

		public const string AbilityModifierPercentageMultiplyKillSP = "AbilityModifierPercentageMultiplyKillSP";

		public const string AbilityModifierIncreaseScorchChance = "AbilityModifierIncreaseScorchChance";

		public const string AbilityModifierIncreaseScorchTurns = "AbilityModifierIncreaseScorchTurns";

		public const string AbilityModifierIncreaseExtraScorchDamageChance = "AbilityModifierIncreaseExtraScorchDamageChance";

		public const string AbilityModifierIncreaseScorchLayers = "AbilityModifierIncreaseScorchLayers";

		public const string AbilityModifierPercentageMultiplyKillSupplies = "AbilityModifierPercentageMultiplyKillSupplies";

		public const string AbilityModifierAddMeleeDamage = "AddMeleeDamage";

		public const string AbilityModifierAddRangedDamage = "AddRangedDamage";

		public const string AbilityModifierPercentageIncreaseCampProduction = "AbilityModifierPercentageIncreaseCampProduction";

		public const string AbilityModifierExtendProbability = "ExtendProbability";

		public const string AbilityModifierBloodThirst = "AbilityModifierBloodThirst";

		public const string AbilityModifierIncreaseConeAngle = "AbilityModifierIncreaseConeAngle";

		public const string AbilityModifierIncreaseBulletWidth = "AbilityModifierIncreaseBulletWidth";

		public const string AbilityModifierIncreaseRange = "AbilityModifierIncreaseRange";

		public const string AbilityModifierIncreaseChanceStunTurns = "AbilityModifierIncreaseChanceStunTurns";

		public const string AbilityModifierIncreaseStruggleTurns = "AbilityModifierIncreaseStruggleTurns";

		public const string AbilityModifierIncreaseExtraAPChance = "AbilityModifierIncreaseExtraAPChance";

		public const string AbilityModifierIncreaseExtraAPChanceForMelee = "AbilityModifierIncreaseExtraAPChanceForMelee";

		public const string AbilityModifierIncreaseExtraAPChanceSpecialEnemies = "AbilityModifierIncreaseExtraAPChanceSpecialEnemies";

		public const string AbilityModifierIncreaseInterruptChance = "AbilityModifierIncreaseInterruptChance";

		public const string AbilityModifierIncreaseExtraChargePointChance = "AbilityModifierIncreaseExtraChargePointChance";

		public const string AbilityModifierIncreaseExtraChargePointAtAttackDmgChance = "AbilityModifierIncreaseExtraChargePointAtAttackDmgChance";

		public const string AbilityModifierIncreaseChargeAbilityDamage = "AbilityModifierIncreaseChargeAbilityDamage";

		public const string AbilitySilencedWeaponChance = "AbilitySilencedWeaponChance";

		public const string AbilityModifierDamageScaleOnTargetHealth = "AbilityModifierDamageScaleOnTargetHealth";

		public const string AbilityModifierScaleDamageByMaxSurvivorLevel = "AbilityModifierScaleDamageByMaxSurvivorLevel";

		public const string AbilityThreatReductionChance = "AbilityThreatReductionChance";

		public const string AbilityThreatFreeChance = "AbilityThreatFreeChance";

		public const string AbilityModifierIncreaseMoveRangeForSecondMove = "AbilityModifierIncreaseMoveRangeForSecondMove";

		public const string AbilityModifierIncreaseMoveRangeForSecondMoveTacticalArmor = "AbilityModifierIncreaseMoveRangeForSecondMoveTacticalArmor";

		public const string AbilityModifierIncreaseShieldHitPointsPercentage = "AbilityModifierIncreaseShieldHitPointsPercentage";

		public const string AbilityModifierExtraMoveChance = "AbilityModifierExtraMoveChance";

		public const string AbilityModifierIncreaseMoveRangeForSecondMoveColdBlooded = "AbilityModifierIncreaseMoveRangeForSecondMoveColdBlooded";

		public const string AbilityModifierIncreaseExtraChargePointChanceAtThreatWave = "AbilityModifierIncreaseExtraChargePointChanceAtThreatWave";

		public const string AbilityModifierIncreaseExtraChargePointChanceAfterBodyShot = "AbilityModifierIncreaseExtraChargePointChanceAfterBodyShot";

		public const string AbilityModifieAddChargePointAtStart = "AbilityModifieAddChargePointAtStart";

		public const string AbilityModifierChangeNotTriggerOverwatch = "AbilityModifierChangeNotTriggerOverwatch";

		public const string LeaderBuffGainExtraChargePointAtTaunt = "LeaderBuffModifierGainChargePointAtTaunt";

		public const string LeaderBuffGainExtraChargePointAtTauntIncreaseChance = "LeaderBuffGainExtraChargePointAtTauntIncreaseChance";

		public const string LeaderBuffBeatEmUpPunishMultiplier = "LeaderBuffBeatEmUpPunishMultiplier";

		public const string LeaderBuffInspireDamageIncreasePerKillPercentage = "LeaderBuffInspireDamageIncreasePerKillPercentage";

		public const string LeaderBuffInspireMaxDamageIncreasePerKillPercentage = "LeaderBuffInspireMaxDamageIncreasePerKillPercentage";

		public const string LeaderBuffInspireIncreaseExtraChargePointChance = "LeaderBuffInspireIncreaseExtraChargePointChance";

		public const string LeaderBuffInspireMaxExtraChargePointChance = "LeaderBuffInspireMaxExtraChargePointChance";

		public const string LeaderBuffPercentageIncreasePreEmptiveStrikeDamage = "LeaderBuffPercentageIncreasePreEmptiveStrikeDamage";

		public const string InspirePerKillIncreaseDamageModifierTrait = "InspirePerKillIncreaseDamageModifierTrait";

		public const string InspirePerKillIncreaseExtraChargePointChanceModifierTrait = "InspirePerKillIncreaseExtraChargePointChanceModifierTrait";

		public const string BlindModifierTrait = "ModifierBlinTrait";

		public const string AbilityModifierIncreaseNoThreatChanceRanged = "AbilityModifierIncreaseNoThreatChanceRanged";

		public const string AbilityModifierIncreaseReduceThreatChanceMelee = "AbilityModifierIncreaseReduceThreatChanceMelee";

		public const string AbilityModifierIncreaseCoverDamageReduction = "AbilityModifierIncreaseCoverDamageReduction";

		public const string AbilityModifierDecreaseBodyshotChance = "AbilityModifierDecreaseBodyshotChance";

		public const string AbilityModifierDecreaseBodyshotChanceColdBlooded = "AbilityModifierDecreaseBodyshotChanceColdBlooded";

		public const string AbilityModifierDecreaseBodyshotMeleeChance = "AbilityModifierDecreaseBodyshotMeleeChance";

		public const string AbilityModifierIncreaseKillOnStruggle = "AbilityModifierKillOnStruggle";

		public const string AbilityModifierGiveDamageOnStruggle = "AbilityModifierGiveDamageOnStruggle";

		public const string AbilityModifierGiveDamageOnStruggleVariance = "AbilityModifierGiveDamageOnStruggleVariance";

		public const string AbilityModifierGiveDamageOnStruggleRoundModifier = "AbilityModifierGiveDamageOnStruggleRoundModifier";

		public const string AbilityModifierIncreaseDamageOnSpecial = "AbilityModifierIncreaseDamageOnSpecial";

		public const string AbilityModifierIncreaseRetaliateDamage = "AbilityModifierIncreaseRetaliateDamage";

		public const string AbilityModifierIncreaseEquipmentRetaliateDamage = "AbilityModifierIncreaseEquipmentRetaliateDamage";

		public const string AbilityModifierIncreaseCriticalResistance = "AbilityModifierIncreaseCriticalResistance";

		public const string AbilityModifierIncreaseSecondChanceChance = "AbilityModifierIncreaseSecondChanceChance";

		public const string AbilityModifierIncreaseMeleeDodgeChance = "AbilityModifierIncreaseMeleeDodgeChance";

		public const string AbilityModifierIncreaseRangedDodgeChance = "AbilityModifierIncreaseRangedDodgeChance";

		public const string AbilityModifierIncreaseRangedEquipmentBulletDodgeChance = "AbilityModifierIncreaseRangedEquipmentBulletDodgeChance";

		public const string AbilityModifierIncreaseFollowThroughChance = "AbilityModifierIncreaseFollowThroughChance";

		public const string AbilityModifierIncreaseEquipFollowThroughChance = "AbilityModifierIncreaseEquipFollowThroughChance";

		public const string AbilityModifierIncreaseCriticalAimChance = "AbilityModifierIncreaseCriticalAimChance";

		public const string AbilityModifierIncreaseEquipCriticalAimChance = "AbilityModifierIncreaseEquipCriticalAimChance";

		public const string AbilityModifierIncreaseCriticalAimChanceCriticalHit = "AbilityModifierIncreaseCriticalAimChanceCriticalHit";

		public const string AbilityModifierIncreaseEquipCriticalAimChanceCriticalHit = "AbilityModifierIncreaseEquipCriticalAimChanceCriticalHit";

		public const string AbilityModifierPercentageIncreaseSameTargetDamage = "AbilityModifierPercentageIncreaseSameTargetDamage";

		public const string AbilityModifierIncreaseHigherLevelEquipmentDropChance = "AbilityModifierIncreaseHigherLevelEquipmentDropChance";

		public const string AbilityModifierIncreaseAmountSuppliesDropChance = "AbilityModifierIncreaseAmountSuppliesDropChance";

		public const string AbilityModifierIncreaseAmountXpDropChance = "AbilityModifierIncreaseAmountXpDropChance";

		public const string AbilityModifierPercentageIncreaseTargetHigherLevelDamage = "AbilityModifierPercentageIncreaseTargetHigherLevelDamage";

		public const string AbilityModifierPercentageIncreaseTargetHigherLevelCritChance = "AbilityModifierPercentageIncreaseTargetHigherLevelCritChance";

		public const string AbilityModifierIncreaseRevengeDamage = "AbilityModifierIncreaseRevengeDamage";

		public const string AbilityModifierIncreaseEquipRevengeDamage = "AbilityModifierIncreaseEquipRevengeDamage";

		public const string AbilityModifierIncreaseChanceToSetTargetOnFire = "AbilityModifierIncreaseChanceToSetTargetOnFire";

		public const string AbilityModifierIncreasePunishDamage = "AbilityModifierIncreasePunishDamage";

		public const string AbilityModifierIncreaseNewPunishDamage = "AbilityModifierIncreaseNewPunishDamage";

		public const string AbilityModifierIncreaseChanceForBodyguard = "AbilityModifierIncreaseChanceForBodyguard";

		public const string AbilityModifierIncreaseDamageReductionIfJerryOrNextToJerry = "AbilityModifierIncreaseDamageReductionIfJerryOrNextToJerry";

		public const string AbilityModifierDecreaseBurningDamage = "AbilityModifierDecreaseBurningDamage";

		public const string AbilityModifierIncreaseDoubleTapChance = "AbilityModifierIncreaseDoubleTapChance";

		public const string AbilityModifierPercentageMaxReduceOnCritical = "AbilityModifierPercentageMaxReduceOnCritical";

		public const string AbilityModifierExtraAttackDamageModifier = "AbilityModifierExtraAttackDamageModifier";

		public const string AbilityModifierNewExtraAttackDamageModifier = "AbilityModifierNewExtraAttackDamageModifier";

		public const string AbilityModifierIncreaseCriticalAimStunTurnsModifier = "AbilityModifierIncreaseCriticalAimStunTurnsModifier";

		public const string AbilityModifierIncreaseEquipCriticalAimStunTurnsModifier = "AbilityModifierIncreaseEquipCriticalAimStunTurnsModifier";

		public const string AbilityModifierExplosiveBulletDamageScaleOnTargetHealth = "AbilityModifierExplosiveBulletDamageScaleOnTargetHealth";

		public const string AbilityModifierExplosiveBulletStunChance = "AbilityModifierExplosiveBulletStunChance";

		public const string AbilityModifierLeaderBuffInspireDamageIncrease = "AbilityModifierLeaderBuffInspireDamageIncrease";

		public const string AbilityModifierLeaderBuffInspireExtraChargePointChance = "AbilityModifierLeaderBuffInspireExtraChargePointChance";

		public const string AbilityModifierIncreaseMoveRangeForSecondMoveDeadlyTactics = "AbilityModifierIncreaseMoveRangeForSecondMoveDeadlyTactics";

		public const string AbilityModifierIncreaseBaseDamageFlat = "AbilityModifierIncreaseBaseDamageFlat";

		public const string AbilityModifierIncreaseBaseHealthFlat = "AbilityModifierIncreaseBaseHealthFlat";

		public const string AbilityModifierIncreaseCritDamageFlat = "AbilityModifierIncreaseCritDamageFlat";

		public const string DamageBonus = "GuildBattleBuff.Damage";

		public const string DamageReductionBonus = "GuildBattleBuff.DamageReduction";

		public const string CriticalChanceBonus = "GuildBattleBuff.CriticalChance";

		public const string BodyShotReductionBonus = "GuildBattleBuff.Piercing";

		public const string DodgeChanceBonus = "GuildBattleBuff.Dodge";

		public const string FullChargeChanceBonus = "GuildBattleBuff.FullCharge";

		public const string GuildBattleAbilityModifierDamage = "GuildBattleAbilityModifierDamage";

		public const string GuildBattleAbilityModifierDamageReduction = "GuildBattleAbilityModifierDamageReduction";

		public const string GuildBattleAbilityModifierCriticalChance = "GuildBattleAbilityModifierCriticalChance";

		public const string GuildBattleAbilityModifierBodyShotReduction = "GuildBattleAbilityModifierBodyShotReduction";

		public const string GuildBattleAbilityModifierDodgeChance = "GuildBattleAbilityModifierDodgeChance";

		public const string GuildBattleAbilityModifierFullChargeChance = "GuildBattleAbilityModifierFullChargeChance";

		public const string CommonwealthArmorTrait = "CommonwealthArmorTrait";

		public const string CommonwealthArmorActive = "CommonwealthArmorActive";

		public const string CommonwealthArmorExtraChargeChance = "CommonwealthArmorExtraChargeChance";

		public const string PastaSupportTrait = "PastaSupportTrait";

		public const string PastaSupportActive = "PastaSupportActive";

		public const string TemFullyStateTrait = "TemFullyStateTrait";

		public const string CarolsCookiesTrait = "CarolsCookiesTrait";

		public const string CarolsCookiesActive = "CarolsCookiesActive";

		public const string CarolsCookiesRegularDamageMultiplier = "CarolsCookiesRegularDamageMultiplier";

		public const string CarolsCookiesChargeDamageMultiplier = "CarolsCookiesChargeDamageMultiplier";

		public const string WalkerMikeActive = "WalkerMikeActive";

		public const string LeaderBuffKnockKnock = "LeaderBuffKnockKnock";

		public const string LeaderBuffKnockKnockTargetMaxNum = "LeaderBuffKnockKnockTargetMaxNum";

		public const string LeaderBuffKnockKnockMarkMaxNum = "LeaderBuffKnockKnockMarkMaxNum";

		public const string LeaderBuffKnockKnockOneMarkDamageMultiplier = "LeaderBuffKnockKnockOneMarkDamageMultiplier";

		public const string LeaderBuffKnockKnockExtraChargePointChance = "LeaderBuffKnockKnockExtraChargePointChance";

		public const string LeaderBuffKnockKnockExtraChargePointConfig = "LeaderBuffKnockKnockExtraChargePointConfig";

		public const string BaseKnockKnock = "BaseKnockKnock";

		public const string DebuffKnockKnockMarkEnemy = "DebuffKnockKnockMarkEnemy";

		public const string FortunaMain = "Fortuna_Main";

		public const string Equipment_Passive_Fortuna_Spade = "Equipment_Passive_Fortuna_Spade";

		public const string AbilityModifierEquipmentPassiveFortunaSpade = "AbilityModifierEquipmentPassiveFortunaSpade";

		public const string Equipment_Passive_Fortuna_Club = "Equipment_Passive_Fortuna_Club";

		public const string AbilityModifierEquipmentPassiveFortunaClub = "AbilityModifierEquipmentPassiveFortunaClub";

		public const string Equipment_Passive_Fortuna_Heart = "Equipment_Passive_Fortuna_Heart";

		public const string AbilityModifierEquipmentPassiveFortunaHeart = "AbilityModifierEquipmentPassiveFortunaHeart";

		public const string Equipment_Passive_TornApart = "Equipment_Passive_TornApart";

		public const string Equipment_Passive_TornApartMarkMaxNum = "Equipment_Passive_TornApartMarkMaxNum";

		public const string Equipment_Passive_TornDamageMultiplier = "Equipment_Passive_TornDamageMultiplier";

		public const string Equipment_Passive_TornExtraDamageMultiplier = "Equipment_Passive_TornExtraDamageMultiplier";

		public const string Equipment_Passive_FreeOW = "Equipment_Passive_FreeOW";

		public const string Equipment_Passive_FreeOWChanceNotToRaider = "Equipment_Passive_FreeOWChanceNotToRaider";

		public const string Equipment_Passive_SawAxe = "Equipment_Passive_SawAxe";

		public const string Equipment_Passive_SawAxe_CriticalChance = "Equipment_Passive_SawAxe_CriticalChance";

		public const string Equipment_Passive_SawAxe_CriticalMultiplier = "Equipment_Passive_SawAxe_CriticalMultiplier";

		public const string Equipment_Passive_SawAxe_ExtraDmgCount = "Equipment_Passive_SawAxe_ExtraDmgCount";

		public const string Equipment_Passive_SawAxe_ExtraDmgChance = "Equipment_Passive_SawAxe_ExtraDmgChance";

		public const string Equipment_Passive_SawAxe_ExtraDmgMultiplier = "Equipment_Passive_SawAxe_ExtraDmgMultiplier";

		public const string Equipment_Passive_SawAxe_MaxExtraDmgMultiplier = "Equipment_Passive_SawAxe_MaxExtraDmgMultiplier";

		public const string Equipment_Passive_ShotGun = "Equipment_Passive_ShotGun";

		public const string Equipment_Passive_ShotGun_Param0 = "Equipment_Passive_ShotGun_Param0";

		public const string Equipment_Passive_ShotGun_Param1 = "Equipment_Passive_ShotGun_Param1";

		public const string Equipment_Passive_ShotGun_Param2 = "Equipment_Passive_ShotGun_Param2";

		public const string Equipment_Passive_ShotGun_Param3 = "Equipment_Passive_ShotGun_Param3";

		public const string Equipment_Passive_ShotGun_Param4 = "Equipment_Passive_ShotGun_Param4";

		public const string HealthRealdmg = "Equipment_Active_HealthRealdmg";

		public const string AbilityModifierHealthRealdmg = "AbilityModifierHealthRealdmg";

		public const string AbilityModifierHealthRealdmg_Param0 = "AbilityModifierHealthRealdmg_Param0";

		public const string Pursuit = "Pursuit";

		public const string AbilityModifierPursuitAP = "AbilityModifierPursuitAP";

		public const string AbilityModifierPursuitCH = "AbilityModifierPursuitCH";

		public const string DodgeShot = "DodgeShot";

		public const string DodgedShotInjurerFlag = "DodgedShotInjurerFlag";

		public const string EquipmentActiveSpecialStun = "SpecialStun";

		public const string EquipmentActiveSpecialStunActiveFlag = "Special_Stun_Active_Flag";

		public const string SpecialStunTargetActiveFlag = "SpecialStunTargetActiveFlag";

		public const string Vigilance = "Vigilance";

		public const string AbilityModifierVigilanceDamageMultiplier = "AbilityModifierVigilanceDamageMultiplier";

		public const string ArcUpgrade = "ArcUpgrade";

		public const string AbilityModifierThreatArcUpgrade = "AbilityModifierThreatArcUpgrade";

		public const string Repulse = "Repulse";

		public const string AbilityModifierRepulseGainAPChance = "AbilityModifierRepulseGainAPChance";

		public const string AbilityModifierRepulseStaggerChance = "AbilityModifierRepulseStaggerChance";

		public const string AbilityModifierRepulseCriticalHitChance = "AbilityModifierRepulseCriticalHitChance";

		public const string BossHitPointDMG = "Boss.HitPointDMG";

		public const string AbilityModifierBossHitPointDMGAttackCount = "AbilityModifierBossHitPointDMGAttackCount";

		public const string AbilityModifierBossHitPointDMGAddAdditionalDamage = "Boss.AbilityModifierBossHitPointDMGAddAdditionalDamage";

		private static readonly string BossHitPointDMG_ToLower = "Boss.HitPointDMG".ToLower();

		public const string EquipmentActiveAdvance = "Equipment_Active_Advance";

		public const string AbilityModifierAdvanceGainAPChance = "AbilityModifierAdvanceGainAPChance";

		public const string AbilityModifierAdvanceCriticalHitChance = "AbilityModifierAdvanceCriticalHitChance";

		public const string EquipmentActiveLight = "Equipment_Active_Light";

		public const string AbilityModifierLightMovementSpeedIsIncreasedBySpaces = "AbilityModifierLightMovementSpeedIsIncreasedBySpaces";

		public const string AbilityModifierLightChanceNotToBeOverwatchedByWalkers = "AbilityModifierLightChanceNotToBeOverwatchedByWalkers";

		public const string AbilityModifierLightChanceNotToBeHumanEnemies = "AbilityModifierLightChanceNotToBeHumanEnemies";

		public const string MultiAttacks = "MultiAttacks";

		public const string MultiAttackExtraDamageActive = "MultiAttackExtraDamageActive";

		public const string AbilityModifierMultiAttackExtraDamageMultiplier = "AbilityModifierMultiAttackExtraDamageMultiplier";

		public const string MultiAttackDoubleShot = "MultiAttackDoubleShot";

		public const string MultiAttackTripleShot = "MultiAttackTripleShot";

		public const string EquipmentActiveShieldBreakerStrikeType1 = "Equipment_Active_ShieldBreakerStrikeType1";

		public const string AbilityModifierShieldBreakerStrikeType1Parameter0 = "AbilityModifierShieldBreakerStrikeType1Parameter0";

		public const string AbilityModifierShieldBreakerStrikeType1Parameter1 = "AbilityModifierShieldBreakerStrikeType1Parameter1";

		public const string EquipmentActiveShieldBreakerStrikeType2 = "Equipment_Active_ShieldBreakerStrikeType2";

		public const string FreeRun = "FreeRun";

		public const string EquipmentActiveGroupdmgboost = "Equipment_Active_Groupdmgboost";

		public const string AbilityModifierGroupdmgboostNumberofEnemiesAttacked = "AbilityModifierGroupdmgboostNumberofEnemiesAttacked";

		public const string AbilityModifierGroupdmgboostprobability = "AbilityModifierGroupdmgboostprobability";

		public const string AbilityModifierGroupdmgboostAdditionalweapondamage = "AbilityModifierGroupdmgboostAdditionalweapondamage";

		public const string EquipmentActiveSkinned = "Equipment_Active_Skinned";

		public const string EquipmentFollowStatusSkinned = "Equipment.FollowStatus.Skinned";

		public const string SkinnedDebuffMarkReduceAttackPowerPercent = "SkinnedDebuffMarkReduceAttackPowerPercent";

		public const string EquipmentActiveAssistAttack = "Equipment_Active_AssistAttack";

		public const string EquipmentActiveAssistAttackActive = "EquipmentActiveAssistAttackActive";

		public const string EquipmentActiveAssistAttackPercent = "EquipmentActiveAssistAttackPercent";

		public const string EquipmentActiveAssistAttackDamagePercent = "EquipmentActiveAssistAttackDamagePercent";

		public const string EquipmentActiveAssistAttackActiveMultiplier = "EquipmentActiveAssistAttackActiveMultiplier";

		public const string EquipmentActiveChargeLoad = "Equipment_Active_ChargeLoad";

		public const string EquipmentActiveChargeLoadBumpPercent = "EquipmentActiveChargeLoadBumpPercent";

		public const string EquipmentActiveChargeLoadBumpDmgRatio = "EquipmentActiveChargeLoadBumpDmgRatio";

		public const string EquipmentActiveChargeLoadBumpMaxFloor = "EquipmentActiveChargeLoadBumpMaxFloor";

		public const string EquipmentActiveRipped = "Equipment_Active_Ripped";

		public const string AbilityModifierRippedAdditionalDmgPercent = "AbilityModifierRippedAdditionalDmgPercent";

		public const string AbilityModifierRippedAdditionalDmgRatio = "AbilityModifierRippedAdditionalDmgRatio";

		public const string AbilityModifierRippedAdditionalDmgMaxRatio = "AbilityModifierRippedAdditionalDmgMaxRatio";

		public const string Riposte = "Riposte";

		public const string AbilityModifierRippedAdditionalPRIncreaseDmg = "AbilityModifierRippedAdditionalPRIncreaseDmg";

		public const string AbilityModifierRippedAdditionalPRMaxStorey = "AbilityModifierRippedAdditionalPRMaxStorey";

		public const string EquipmentPassiveShield = "Equipment_Passive_Shield";

		public const string TrapFlame = "TrapFlame";

		public const string Asthenia = "Asthenia";

		public const string AbilityModifierDamagerActorUpDamagePercentage = "AbilityModifierDamagerActorUpDamagePercentage";

		public const string AbilityModifierDamagerActorDamageReducePercentage = "AbilityModifierDamagerActorDamageReducePercentage";

		public const string GrenadeFragmentDamage = "GrenadeFragmentDamage";

		public const string AbilityModifierPreventPushPercentage = "AbilityModifierPreventPushPercentage";

		public const string AbilityModifierPreventIncendiaryPercentage = "AbilityModifierPreventIncendiaryPercentage";

		public const string EquipmentPassiveHelpHand = "Equipment_Passive_HelpHand";

		public const string BaseEquipmentApocalypticDMG = "Equipment_Apocalyptic_DMG";

		public const string BaseEquipmentApocalypticBS = "Equipment_Apocalyptic_BS";

		public const string BaseEquipmentApocalypticDEF = "Equipment_Apocalyptic_DEF";

		public const string EquipmentActiveHPNailgun = "Equipment_Active_HPNailgun";

		public const string AbilityModifierAttackDamageEnhancement = "AbilityModifierAttackDamageEnhancement";

		public const string AbilityModifierExtrAtorsoAttackDamageBoost = "AbilityModifierExtrAtorsoAttackDamageBoost";

		public const string EquipmentApocalypticDMGScout = "Equipment_Apocalyptic_DMG_Scout";

		public const string AbilityModifierDMGScoutAttackingAHighRanking = "AbilityModifierDMGScoutAttackingAHighRanking";

		public const string AbilityModifierDMGScoutLevelDifference = "AbilityModifierDMGScoutLevelDifference";

		public const string AbilityModifierDMGScoutIncreaseDamage = "AbilityModifierDMGScoutIncreaseDamage";

		public const string AbilityModifierDMGScoutMaximumLiftingValue = "AbilityModifierDMGScoutMaximumLiftingValue";

		public const string AbilityModifierDMGScoutMaxLeveLimitValue = "AbilityModifierDMGScoutMaxLeveLimitValue";

		public const string EquipmentApocalypticDMGBruiser = "Equipment_Apocalyptic_DMG_Bruiser";

		public const string AbilityModifierDMGBruiserAttackingAHighRanking = "AbilityModifierDMGBruiserAttackingAHighRanking";

		public const string AbilityModifierDMGBruiserLevelDifference = "AbilityModifierDMGBruiserLevelDifference";

		public const string AbilityModifierDMGBruiserIncreaseDamage = "AbilityModifierDMGBruiserIncreaseDamage";

		public const string AbilityModifierDMGBruiserMaximumLiftingValue = "AbilityModifierDMGBruiserMaximumLiftingValue";

		public const string AbilityModifierDMGBruiserMaxLeveLimitValue = "AbilityModifierDMGBruiserMaxLeveLimitValue";

		public const string EquipmentApocalypticDMGWarrior = "Equipment_Apocalyptic_DMG_Warrior";

		public const string AbilityModifierDMGWarriorAttackingAHighRanking = "AbilityModifierDMGWarriorAttackingAHighRanking";

		public const string AbilityModifierDMGWarriorLevelDifference = "AbilityModifierDMGWarriorLevelDifference";

		public const string AbilityModifierDMGWarriorIncreaseDamage = "AbilityModifierDMGWarriorIncreaseDamage";

		public const string AbilityModifierDMGWarriorMaximumLiftingValue = "AbilityModifierDMGWarriorMaximumLiftingValue";

		public const string AbilityModifierDMGWarriorMaxLeveLimitValue = "AbilityModifierDMGWarriorMaxLeveLimitValue";

		public const string EquipmentApocalypticDMGShooter = "Equipment_Apocalyptic_DMG_Shooter";

		public const string AbilityModifierDMGShooterAttackingAHighRanking = "AbilityModifierDMGShooterAttackingAHighRanking";

		public const string AbilityModifierDMGShooterLevelDifference = "AbilityModifierDMGShooterLevelDifference";

		public const string AbilityModifierDMGShooterIncreaseDamage = "AbilityModifierDMGShooterIncreaseDamage";

		public const string AbilityModifierDMGShooterMaximumLiftingValue = "AbilityModifierDMGShooterMaximumLiftingValue";

		public const string AbilityModifierDMGShooterMaxLeveLimitValue = "AbilityModifierDMGShooterMaxLeveLimitValue";

		public const string EquipmentApocalypticDMGHunter = "Equipment_Apocalyptic_DMG_Hunter";

		public const string AbilityModifierDMGHunterAttackingAHighRanking = "AbilityModifierDMGHunterAttackingAHighRanking";

		public const string AbilityModifierDMGHunterLevelDifference = "AbilityModifierDMGHunterLevelDifference";

		public const string AbilityModifierDMGHunterIncreaseDamage = "AbilityModifierDMGHunterIncreaseDamage";

		public const string AbilityModifierDMGHunterMaximumLiftingValue = "AbilityModifierDMGHunterMaximumLiftingValue";

		public const string AbilityModifierDMGHunterMaxLeveLimitValue = "AbilityModifierDMGHunterMaxLeveLimitValue";

		public const string EquipmentApocalypticDMGAssault = "Equipment_Apocalyptic_DMG_Assault";

		public const string AbilityModifierDMGAssaultAttackingAHighRanking = "AbilityModifierDMGAssaultAttackingAHighRanking";

		public const string AbilityModifierDMGAssaultLevelDifference = "AbilityModifierDMGAssaultLevelDifference";

		public const string AbilityModifierDMGAssaultIncreaseDamage = "AbilityModifierDMGAssaultIncreaseDamage";

		public const string AbilityModifierDMGAssaultMaximumLiftingValue = "AbilityModifierDMGAssaultMaximumLiftingValue";

		public const string AbilityModifierDMGAssaultMaxLeveLimitValue = "AbilityModifierDMGAssaultMaxLeveLimitValue";

		public const string EquipmentActiveFocusMode = "Equipment_Active_FocusMode";

		public const string FocusMode = "FocusMode";

		public const string AbilityModifierFocusModeAttackDistance = "AbilityModifierFocusModeAttackDistance";

		public const string AbilityModifierFocusModeAttackWidth = "AbilityModifierFocusModeAttackWidth";

		public const string AbilityModifierFocusModeDamageIncrease = "AbilityModifierFocusModeDamageIncrease";

		public const string AbilityModifierLimitNumberChargeAttack = "AbilityModifierLimitNumberChargeAttack";

		public const string AbilityModifierLimitNumberTurns = "AbilityModifierLimitNumberTurns";

		public const string EquipmentActiveBreakthrough = "Equipment_Active_Breakthrough";

		public const string AbilityModifierLBreakthroughMath = "AbilityModifierLBreakthroughMath";

		public const string AbilityModifierLEquipBreakthroughMath = "AbilityModifierLEquipBreakthroughMath";

		public const string EquipmentApocalypticBSScout = "Equipment_Apocalyptic_BS_Scout";

		public const string AbilityModifierBSScoutAttackingAHighRanking = "AbilityModifierBSScoutAttackingAHighRanking";

		public const string AbilityModifierBSScoutLevelDifference = "AbilityModifierBSScoutLevelDifference";

		public const string AbilityModifierBSScoutProbabilityReduction = "AbilityModifierBSScoutProbabilityReduction";

		public const string AbilityModifierBSScoutMaximumLiftingValue = "AbilityModifierBSScoutMaximumLiftingValue";

		public const string EquipmentApocalypticBSBruiser = "Equipment_Apocalyptic_BS_Bruiser";

		public const string AbilityModifierBSBruiserAttackingAHighRanking = "AbilityModifierBSBruiserAttackingAHighRanking";

		public const string AbilityModifierBSBruiserLevelDifference = "AbilityModifierBSBruiserLevelDifference";

		public const string AbilityModifierBSBruiserProbabilityReduction = "AbilityModifierBSBruiserProbabilityReduction";

		public const string AbilityModifierBSBruiserMaximumLiftingValue = "AbilityModifierBSBruiserMaximumLiftingValue";

		public const string EquipmentApocalypticBSWarrior = "Equipment_Apocalyptic_BS_Warrior";

		public const string AbilityModifierBSWarriorAttackingAHighRanking = "AbilityModifierBSWarriorAttackingAHighRanking";

		public const string AbilityModifierBSWarriorLevelDifference = "AbilityModifierBSWarriorLevelDifference";

		public const string AbilityModifierBSWarriorProbabilityReduction = "AbilityModifierBSWarriorProbabilityReduction";

		public const string AbilityModifierBSWarriorMaximumLiftingValue = "AbilityModifierBSWarriorMaximumLiftingValue";

		public const string EquipmentApocalypticBSShooter = "Equipment_Apocalyptic_BS_Shooter";

		public const string AbilityModifierBSShooterAttackingAHighRanking = "AbilityModifierBSShooterAttackingAHighRanking";

		public const string AbilityModifierBSShooterLevelDifference = "AbilityModifierBSShooterLevelDifference";

		public const string AbilityModifierBSShooterProbabilityReduction = "AbilityModifierBSShooterProbabilityReduction";

		public const string AbilityModifierBSShooterMaximumLiftingValue = "AbilityModifierBSShooterMaximumLiftingValue";

		public const string EquipmentApocalypticBSHunter = "Equipment_Apocalyptic_BS_Hunter";

		public const string AbilityModifierBSHunterAttackingAHighRanking = "AbilityModifierBSHunterAttackingAHighRanking";

		public const string AbilityModifierBSHunterLevelDifference = "AbilityModifierBSHunterLevelDifference";

		public const string AbilityModifierBSHunterProbabilityReduction = "AbilityModifierBSHunterProbabilityReduction";

		public const string AbilityModifierBSHunterMaximumLiftingValue = "AbilityModifierBSHunterMaximumLiftingValue";

		public const string EquipmentApocalypticBSAssault = "Equipment_Apocalyptic_BS_Assault";

		public const string AbilityModifierBSAssaultAttackingAHighRanking = "AbilityModifierBSAssaultAttackingAHighRanking";

		public const string AbilityModifierBSAssaultLevelDifference = "AbilityModifierBSAssaultLevelDifference";

		public const string AbilityModifierBSAssaultProbabilityReduction = "AbilityModifierBSAssaultProbabilityReduction";

		public const string AbilityModifierBSAssaultMaximumLiftingValue = "AbilityModifierBSAssaultMaximumLiftingValue";

		public const string EquipmentApocalypticDEFScout = "Equipment_Apocalyptic_DEF_Scout";

		public const string AbilityModifierDEFScoutAttackedByHighLevel = "AbilityModifierDEFScoutAttackedByHighLevel";

		public const string AbilityModifierDEFScoutLevelDifference = "AbilityModifierDEFScoutLevelDifference";

		public const string AbilityModifierDEFScoutDamageReduction = "AbilityModifierDEFScoutDamageReduction";

		public const string AbilityModifierDEFScoutMaximumLiftingValue = "AbilityModifierDEFScoutMaximumLiftingValue";

		public const string AbilityModifierDEFScoutMaxLeveLimitValue = "AbilityModifierDEFScoutMaxLeveLimitValue";

		public const string EquipmentApocalypticDEFBruiser = "Equipment_Apocalyptic_DEF_Bruiser";

		public const string AbilityModifierDEFBruiserAttackedByHighLevel = "AbilityModifierDEFBruiserAttackedByHighLevel";

		public const string AbilityModifierDEFBruiserLevelDifference = "AbilityModifierDEFBruiserLevelDifference";

		public const string AbilityModifierDEFBruiserDamageReduction = "AbilityModifierDEFBruiserDamageReduction";

		public const string AbilityModifierDEFBruiserMaximumLiftingValue = "AbilityModifierDEFBruiserMaximumLiftingValue";

		public const string AbilityModifierDEFBruiserMaxLeveLimitValue = "AbilityModifierDEFBruiserMaxLeveLimitValue";

		public const string EquipmentApocalypticDEFWarrior = "Equipment_Apocalyptic_DEF_Warrior";

		public const string AbilityModifierDEFWarriorAttackedByHighLevel = "AbilityModifierDEFWarriorAttackedByHighLevel";

		public const string AbilityModifierDEFWarriorLevelDifference = "AbilityModifierDEFWarriorLevelDifference";

		public const string AbilityModifierDEFWarriorDamageReduction = "AbilityModifierDEFWarriorDamageReduction";

		public const string AbilityModifierDEFWarriorMaximumLiftingValue = "AbilityModifierDEFWarriorMaximumLiftingValue";

		public const string AbilityModifierDEFWarriorMaxLeveLimitValue = "AbilityModifierDEFWarriorMaxLeveLimitValue";

		public const string EquipmentApocalypticDEFShooter = "Equipment_Apocalyptic_DEF_Shooter";

		public const string AbilityModifierDEFShooterAttackedByHighLevel = "AbilityModifierDEFShooterAttackedByHighLevel";

		public const string AbilityModifierDEFShooterLevelDifference = "AbilityModifierDEFShooterLevelDifference";

		public const string AbilityModifierDEFShooterDamageReduction = "AbilityModifierDEFShooterDamageReduction";

		public const string AbilityModifierDEFShooterMaximumLiftingValue = "AbilityModifierDEFShooterMaximumLiftingValue";

		public const string AbilityModifierDEFShooterMaxLeveLimitValue = "AbilityModifierDEFShooterMaxLeveLimitValue";

		public const string EquipmentApocalypticDEFHunter = "Equipment_Apocalyptic_DEF_Hunter";

		public const string AbilityModifierDEFHunterAttackedByHighLevel = "AbilityModifierDEFHunterAttackedByHighLevel";

		public const string AbilityModifierDEFHunterLevelDifference = "AbilityModifierDEFHunterLevelDifference";

		public const string AbilityModifierDEFHunterDamageReduction = "AbilityModifierDEFHunterDamageReduction";

		public const string AbilityModifierDEFHunterMaximumLiftingValue = "AbilityModifierDEFHunterMaximumLiftingValue";

		public const string AbilityModifierDEFHunterMaxLeveLimitValue = "AbilityModifierDEFHunterMaxLeveLimitValue";

		public const string EquipmentApocalypticDEFAssault = "Equipment_Apocalyptic_DEF_Assault";

		public const string AbilityModifierDEFAssaultAttackedByHighLevel = "AbilityModifierDEFAssaultAttackedByHighLevel";

		public const string AbilityModifierDEFAssaultLevelDifference = "AbilityModifierDEFAssaultLevelDifference";

		public const string AbilityModifierDEFAssaultDamageReduction = "AbilityModifierDEFAssaultDamageReduction";

		public const string AbilityModifierDEFAssaultMaximumLiftingValue = "AbilityModifierDEFAssaultMaximumLiftingValue";

		public const string AbilityModifierDEFAssaultMaxLeveLimitValue = "AbilityModifierDEFAssaultMaxLeveLimitValue";

		public const string EquipmentActiveKing = "Equipment_Active_King";

		public const string AbilityModifierEquipmentActiveKingSuperpositionNumber = "AbilityModifierEquipmentActiveKingSuperpositionNumber";

		public const string AbilityModifierEquipmentActiveKingMaxSuperpositionNumber = "AbilityModifierEquipmentActiveKingMaxSuperpositionNumber";

		public const string EquipmentActiveSuppress1 = "Equipment_Active_Suppress_1";

		public const string AbilityModifierEquipmentActiveSuppress1CheckNumber = "AbilityModifierEquipmentActiveSuppress1CheckNumber";

		public const string AbilityModifierEquipmentActiveSuppress1BloodRestriction = "AbilityModifierEquipmentActiveSuppress1BloodRestriction";

		public const string AbilityModifierEquipmentActiveSuppress1DamageBonus = "AbilityModifierEquipmentActiveSuppress1DamageBonus";

		public const string EquipmentActiveSuppress2 = "Equipment_Active_Suppress_2";

		public const string AbilityModifierEquipmentActiveSuppress2CheckNumber = "AbilityModifierEquipmentActiveSuppress2CheckNumber";

		public const string AbilityModifierEquipmentActiveSuppress2BloodRestriction = "AbilityModifierEquipmentActiveSuppress2BloodRestriction";

		public const string AbilityModifierEquipmentActiveSuppress2DamageBonus = "AbilityModifierEquipmentActiveSuppress2DamageBonus";

		public const string EquipmentActiveDisoriented = "Equipment_Active_Disoriented";

		public const string DisorientLeastSpaces = "DisorientLeastSpaces";

		public const string NegativeFatal = "NegativeFatal";

		public const string NegativeFlagFatalFlag = "NegativeFlagFatalFlag";

		public const string EquipmentActiveRecoil = "Equipment_Active_Recoil";

		public const string AbilityModifierRecoilDamageReduce = "AbilityModifierRecoilDamageReduce";

		public const string AbilityModifierRecoilNormalStunChance = "AbilityModifierRecoilNormalStunChance";

		public const string AbilityModifierRecoilCircleStunChance = "AbilityModifierRecoilCircleStunChance";

		public const string HealthDmg = "Healthdmg";

		public const string AbilityModifierExtraHealthDmgMultiplier = "AbilityModifierExtraHealthDmgMultiplier";

		public const string LeaderBuffABTester = "LeaderBuffABTester";

		public const string LeaderBuffABTesterAMaxNum = "LeaderBuffABTesterAMaxNum";

		public const string LeaderBuffABTesterADamageMultiplier = "LeaderBuffABTesterADamageMultiplier";

		public const string LeaderBuffABTesterBMarkChance = "LeaderBuffABTesterBMarkChance";

		public const string LeaderBuffABTesterBAPChance = "LeaderBuffABTesterBAPChance";

		public const string BaseABTester = "BaseABTester";

		public const string ABTesterA2Active = "ABTesterA2Active";

		public const string AttackChain = "AttackChain";

		public const string HeirloomsBracelets = "Heirlooms_Daryl_Bracelets";

		public const string BraceletsGainChargePointChanceForAll = "BraceletsGainChargePointChanceForAll";

		public const string BounsPhonePortrait = "Heirlooms_Rick_PhonePortrait";

		public const string BounsPhonePortraitAfterKilledTimes = "BounsPhonePortraitAfterKilledTimes";

		public const string BounsPhonePortraitTargetHitPointsBelowPercent = "BonusPhonePortraitTargetHitPointsBelowPercent";

		public const string BounsPhonePortraitKilledTargetPercentage = "BounsPhonePortraitKilledTargetPercentage";

		public const string BounsPhonePortraitOnceKilledMaxTarget = "BounsPhonePortraitOnceKilledMaxTarget";

		public const string HeirloomsMaggiePocketWatch = "Heirlooms_Maggie_PocketWatch";

		public const string HeirloomsMaggiePocketWatchBurnChance = "HeirloomsMaggiePocketWatchBurnChance";

		public const string HeirloomsMaggiePocketWatchScorchChance = "HeirloomsMaggiePocketWatchScorchChance";

		public const string HeirloomsMaggiePocketWatchScorchTurns = "HeirloomsMaggiePocketWatchScorchTurns";

		public const string Heirlooms_RiotGearGlenn_Fetter = "Heirlooms_RiotGearGlenn_Fetter";

		public const string Heirlooms_RiotGearGlenn_Fetter_BurnDmg = "Heirlooms_RiotGearGlenn_Fetter_BurnDmg";

		public const string Heirlooms_RiotGearGlenn_Fetter_AtkChance = "Heirlooms_RiotGearGlenn_Fetter_AtkChance";

		public const string Heirlooms_RiotGearGlenn_Fetter_AtkTimes = "Heirlooms_RiotGearGlenn_Fetter_AtkTimes";

		public const string Heirlooms_RiotGearGlenn_Fetter_AtkChanceStun = "Heirlooms_RiotGearGlenn_Fetter_AtkChanceStun";

		public const string Heirlooms_RiotGearGlenn_Fetter_ChargeChance = "Heirlooms_RiotGearGlenn_Fetter_ChargeChance";

		public const string Heirlooms_RiotGearGlenn_Fetter_ChargeTimes = "Heirlooms_RiotGearGlenn_Fetter_ChargeTimes";

		public const string Heirlooms_RiotGearGlenn_Fetter_ChargeChanceStun = "Heirlooms_RiotGearGlenn_Fetter_ChargeChanceStun";

		public const string Heirlooms_Hershel_Fetter = "Heirlooms_Hershel_Fetter";

		public const string HealthBoostBouns = "HealthBoostBouns";

		public const string AbilityModifierHealthBoostBounsHealth = "AbilityModifierHealthBoostBounsHealth";

		public const string FlameDMGReduceBouns = "FlameDMGReduceBouns";

		public const string FlameDMGReduceBouns_ReduceBurn = "FlameDMGReduceBouns_ReduceBurn";

		public const string LeaderBuffOverload = "LeaderBuffOverload";

		public const string BaseOverload = "BaseOverload";

		public const string LeaderBuffOverload_ChargePointNum = "LeaderBuffOverload_ChargePointNum";

		public const string LeaderBuffOverload_ChargePointLimitNum = "LeaderBuffOverload_ChargePointLimitNum";

		public const string LeaderBuffOverload_ChargePointDmgPer = "LeaderBuffOverload_ChargePointDmgPer";

		public const string LeaderBuffOverload_ContinueTurnNum = "LeaderBuffOverload_ContinueTurnNum";

		public const string LeaderBuffOverload_FullChargeEXDmgPer = "LeaderBuffOverload_FullChargeEXDmgPer";

		public const string LeaderBuffOverload_FullChargeEXTurnLimitNum = "LeaderBuffOverload_FullChargeEXTurnLimitNum";

		public const string LeaderBuffOverload_AddDmgPer = "LeaderBuffOverload_AddDmgPer";

		public const string LeaderBuffOverload_LifeDmgPer = "LeaderBuffOverload_LifeDmgPer";

		public const string BaseLeaderBuffOverload_ChargePointNum = "BaseLeaderBuffOverload_ChargePointNum";

		public const string BaseLeaderBuffOverload_ChargePointLimitNum = "BaseLeaderBuffOverload_ChargePointLimitNum";

		public const string BaseLeaderBuffOverload_ChargePointDmgPer = "BaseLeaderBuffOverload_ChargePointDmgPer";

		public const string BaseLeaderBuffOverload_ContinueTurnNum = "BaseLeaderBuffOverload_ContinueTurnNum";

		public const string BaseLeaderBuffOverload_FullChargeEXDmgPer = "BaseLeaderBuffOverload_FullChargeEXDmgPer";

		public const string BaseLeaderBuffOverload_FullChargeEXTurnLimitNum = "BaseLeaderBuffOverload_FullChargeEXTurnLimitNum";

		public const string BaseLeaderBuffOverload_AddDmgPer = "BaseLeaderBuffOverload_AddDmgPer";

		public const string BaseLeaderBuffOverload_LifeDmgPer = "BaseLeaderBuffOverload_LifeDmgPer";

		public const string OverloadEXDamageActive = "OverloadDamageActive";

		public const string OverloadEXDamageActiveMultiplier = "OverloadDamageActiveActiveMultiplier";

		public const string DeadlyFocusEXDamageActive = "DeadlyFocusEXDamageActive";

		public const string DeadlyFocusEXDamageActiveMultiplier = "DeadlyFocusEXDamageActiveMultiplier";

		public const string Equipment_Passive_Detonation = "Equipment_Passive_Detonation";

		public const string Equipment_Passive_Detonation_Dmg = "Equipment_Passive_Detonation_Dmg";

		public const string Equipment_Passive_DetonationProbility = "Equipment_Passive_DetonationProbility";

		public const string Equipment_Passive_Detonation1 = "Equipment_Passive_Detonation_1";

		public const string Equipment_Passive_Detonation_Dmg1 = "Equipment_Passive_Detonation_Dmg_1";

		public const string StrengthenDefenseFunc1 = "StrengthenDefenseFunc1";

		public const string StrengthenDefenseFunc1Param1 = "StrengthenDefenseFunc1Param1";

		public const string StrengthenDefenseFunc1Param2 = "StrengthenDefenseFunc1Param2";

		public const string StrengthenDefenseFunc1Param3 = "StrengthenDefenseFunc1Param3";

		public const string StrengthenDefenseFunc2 = "StrengthenDefenseFunc2";

		public const string StrengthenDefenseFunc2Param1 = "StrengthenDefenseFunc2Param1";

		public const string StrengthenDefenseFunc2Param2 = "StrengthenDefenseFunc2Param2";

		public const string StrengthenDefenseFunc2Param3 = "StrengthenDefenseFunc2Param3";

		public const string StrengthenDefenseFunc2Param4 = "StrengthenDefenseFunc2Param4";

		public const string StrengthenDefenseFunc3 = "StrengthenDefenseFunc3";

		public const string StrengthenDefenseFunc3Param1 = "StrengthenDefenseFunc3Param1";

		public const string StrengthenDefenseFunc3Param2 = "StrengthenDefenseFunc3Param2";

		public const string StrengthenDefenseFunc3Param3 = "StrengthenDefenseFunc3Param3";

		public const string Equipment_Passive_ScoutDMGBoost = "Equipment_Passive_ScoutDMGBoost";

		public const string Equipment_Passive_ScoutDMGBoost_Dmg = "Equipment_Passive_ScoutDMGBoost_Dmg";

		public const string Equipment_Passive_BruiserDMGBoost = "Equipment_Passive_BruiserDMGBoost";

		public const string Equipment_Passive_BruiserDMGBoost_Dmg = "Equipment_Passive_BruiserDMGBoost_Dmg";

		public const string Equipment_Passive_WarriorDMGBoost = "Equipment_Passive_WarriorDMGBoost";

		public const string Equipment_Passive_WarriorDMGBoost_Dmg = "Equipment_Passive_WarriorDMGBoost_Dmg";

		public const string Equipment_Passive_ShooterDMGBoost = "Equipment_Passive_ShooterDMGBoost";

		public const string Equipment_Passive_ShooterDMGBoost_Dmg = "Equipment_Passive_ShooterDMGBoost_Dmg";

		public const string Equipment_Passive_HunterDMGBoost = "Equipment_Passive_HunterDMGBoost";

		public const string Equipment_Passive_HunterDMGBoost_Dmg = "Equipment_Passive_HunterDMGBoost_Dmg";

		public const string Equipment_Passive_AssaultDMGBoost = "Equipment_Passive_AssaultDMGBoost";

		public const string Equipment_Passive_AssaultDMGBoost_Dmg = "Equipment_Passive_AssaultDMGBoost_Dmg";

		public const string Equipment_Passive_Flame = "Equipment_Passive_Flame";

		public const string Equipment_Passive_FlamePercentage = "Equipment_Passive_FlamePercentage";

		public const string Equipment_Passive_DefendingHeart = "Equipment_Passive_DefendingHeart";

		public const string Equipment_Passive_DefendingHeartPercentage = "Equipment_Passive_DefendingHeartPercentage";

		public const string Equipment_Passive_DefendingHeartTurns = "Equipment_Passive_DefendingHeartTurns";

		public const string Equipment_Passive_DefendingHeartCD = "Equipment_Passive_DefendingHeartCD";

		public const string GodWarBless = "GodWarBless";

		public const string GodWarBless_DmgPercentage = "GodWarBless_DmgPercentage";

		public const string Equipment_Passive_HPPercentDmg = "Equipment_Passive_HPPercentDmg";

		public const string Equipment_Passive_HPPercentDmg_Per = "Equipment_Passive_HPPercentDmg_Per";

		public const string Equipment_Active_BloodFrenzy = "Equipment_Active_BloodFrenzy";

		public const string Equipment_Active_BloodFrenzy_Hp = "Equipment_Active_BloodFrenzy_Hp";

		public const string Equipment_Active_BloodFrenzy_Dmg = "Equipment_Active_BloodFrenzy_Dmg";

		public const string Equipment_Passive_PassOW = "Equipment_Passive_PassOW";

		public const string Equipment_Passive_Dash = "Equipment_Passive_Dash";

		public const string Equipment_Passive_Backstep = "Equipment_Passive_Backstep";

		public const string ChargeAttackWithFreeShooting = "ChargeAttackWithFreeShooting";

		public const string EquipmentPassiveFightBack = "Equipment.Passive.FightBack";

		public const string Equipment_Passive_Rage = "Equipment_Passive_Rage";

		public const string Equipment_Passive_RageParam0 = "Equipment_Passive_RageParam0";

		public const string Equipment_Passive_RageParam1 = "Equipment_Passive_RageParam1";

		public const string Equipment_Passive_RageParam2 = "Equipment_Passive_RageParam2";

		public const string Equipment_Passive_RageParam3 = "Equipment_Passive_RageParam3";

		public const string Equipment_Passive_RageParam4 = "Equipment_Passive_RageParam4";

		public const string Equipment_Passive_RageParam5 = "Equipment_Passive_RageParam5";

		public const string Equipment_Passive_RageParam6 = "Equipment_Passive_RageParam6";

		public const string Equipment_Passive_RageParam7 = "Equipment_Passive_RageParam7";

		public const string Equipment_Passive_RageParam8 = "Equipment_Passive_RageParam8";

		public const string Equipment_Passive_RageParam9 = "Equipment_Passive_RageParam9";

		public const string Equipment_Passive_RageParam10 = "Equipment_Passive_RageParam10";

		public const string Equipment_Passive_RageParam11 = "Equipment_Passive_RageParam11";

		public const string Equipment_Passive_RageParam12 = "Equipment_Passive_RageParam12";

		public const string AttackWithTriggerDot = "AttackWithTriggerDot";

		public const string Cadence = "Cadence";

		public const string FireSpread = "FireSpread";

		public const string LeaderBuffNoExceptions = "LeaderBuffNoExceptions";

		public const string BaseNoExceptions = "BaseNoExceptions";

		public const string LeaderBuffNoExceptions_SetFireChance = "LeaderBuffNoExceptions_SetFireChance";

		public const string LeaderBuffNoExceptions_FlameTriggerRange = "LeaderBuffNoExceptions_FlameTriggerRange";

		public const string LeaderBuffNoExceptions_MaxEnemy = "LeaderBuffNoExceptions_MaxEnemy";

		public const string LeaderBuffNoExceptions_BurnDamageRatio = "LeaderBuffNoExceptions_BurnDamageRatio";

		public const string LeaderBuffNoExceptions_MaxTriggerCount = "LeaderBuffNoExceptions_MaxTriggerCount";

		public const string LeaderBuffNoExceptions_LeaderMaxTriggerCount = "LeaderBuffNoExceptions_LeaderMaxTriggerCount";

		public const string LeaderBuffNoExceptions_BurnLayerChance = "LeaderBuffNoExceptions_BurnLayerChance";

		public const string LeaderBuffNoExceptions_BurnLayerTurn = "LeaderBuffNoExceptions_BurnLayerTurn";

		public const string LeaderBuffNoExceptions_ChargePointChance = "LeaderBuffNoExceptions_ChargePointChance";

		public const string LeaderBuffNoExceptions_MaxBurnLayer = "LeaderBuffNoExceptions_MaxBurnLayer";

		public const string SurvivalManualStorySkill_A = "SurvivalManualStorySkill_A";

		public const string SurvivalManualDecreaseBodyshotChance = "SurvivalManualDecreaseBodyshotChance";

		public const string SurvivalManualMaxDecreaseBodyshotChance = "SurvivalManualMaxDecreaseBodyshotChance";

		public const string SurvivalManualStorySkill_B = "SurvivalManualStorySkill_B";

		public const string SurvivalManualStorySkill_C = "SurvivalManualStorySkill_C";

		public const string SurvivalManualIncreaseDmg = "SurvivalManualIncreaseDmg";

		public const string SurvivalManualStorySkill_D = "SurvivalManualStorySkill_D";

		public const string SurvivalManualKillIncreaseDmg = "SurvivalManualKillIncreaseDmg";

		public const string SurvivalManualKillMaxIncreaseDmg = "SurvivalManualKillMaxIncreaseDmg";

		public const string SurvivalManualStorySkill_E = "SurvivalManualStorySkill_E";

		public const string SurvivalManualStorySkill_EParm1 = "SurvivalManualStorySkill_EParm1";

		public const string SurvivalManualStorySkill_EParm2 = "SurvivalManualStorySkill_EParm2";

		public const string SurvivalManualStorySkill_EParm3 = "SurvivalManualStorySkill_EParm3";

		public const string SurvivalManualStorySkill_F = "SurvivalManualStorySkill_F";

		public const string SurvivalManualStorySkill_FParm1 = "SurvivalManualStorySkill_FParm1";

		public const string SurvivalManualStorySkill_FParm2 = "SurvivalManualStorySkill_FParm2";

		public const string SurvivalManualStorySkill_FParm3 = "SurvivalManualStorySkill_FParm3";

		public const string SurvivalManualKillIncreaseDmgTrait = "SurvivalManualKillIncreaseDmgTrait";

		public const string SurvivalManualCurKillIncreaseDmg = "SurvivalManualCurKillIncreaseDmg";

		public const string SurvivalManualStorySkill_G = "SurvivalManualStorySkill_G";

		public const string SurvivalManualStorySkill_GParm1 = "SurvivalManualStorySkill_GParm1";

		public const string SurvivalManualStorySkill_GParm2 = "SurvivalManualStorySkill_GParm2";

		public const string SurvivalManualStorySkill_GParm3 = "SurvivalManualStorySkill_GParm3";

		public const string SurvivalManualStorySkill_H = "SurvivalManualStorySkill_H";

		public const string SurvivalManualStorySkill_HParm1 = "SurvivalManualStorySkill_HParm1";

		public const string SurvivalManualStorySkill_HParm2 = "SurvivalManualStorySkill_HParm2";

		public const string SurvivalManualStorySkill_HParm3 = "SurvivalManualStorySkill_HParm3";

		public const string SurvivalManualStorySkill_I = "SurvivalManualStorySkill_I";

		public const string SurvivalManualStorySkill_IParm1 = "SurvivalManualStorySkill_IParm1";

		public const string SurvivalManualStorySkill_IParm2 = "SurvivalManualStorySkill_IParm2";

		public const string SurvivalManualStorySkill_IParm3 = "SurvivalManualStorySkill_IParm3";

		public const string SupportTalent_MoveHitrate = "SupportTalent_MoveHitrate";

		public const string SupportTalent_MoveHitrateParm1 = "SupportTalent_MoveHitrateParm1";

		public const string SupportTalent_MoveHitrateParm2 = "SupportTalent_MoveHitrateParm2";

		public const string SupportTalent_MoveCritRate = "SupportTalent_MoveCritRate";

		public const string SupportTalent_MoveCritRateParm1 = "SupportTalent_MoveCritRateParm1";

		public const string SupportTalent_MoveCritRateParm2 = "SupportTalent_MoveCritRateParm2";

		public const string SupportTalent_NoMoveHitrate = "SupportTalent_NoMoveHitrate";

		public const string SupportTalent_NoMoveHitrateParm1 = "SupportTalent_NoMoveHitrateParm1";

		public const string SupportTalent_NoMoveCritRate = "SupportTalent_NoMoveCritRate";

		public const string SupportTalent_NoMoveCritRateParm1 = "SupportTalent_NoMoveCritRateParm1";

		public const string SupportTalent_CritRate = "SupportTalent_CritRate";

		public const string SupportTalent_CritRateParm1 = "SupportTalent_CritRateParm1";

		public const string SupportTalent_CritDmg = "SupportTalent_CritDmg";

		public const string SupportTalent_CritDmgParm1 = "SupportTalent_CritDmgParm1";

		public const string SupportTalent_CritRateRef = "SupportTalent_RefCritRate";

		public const string SupportTalent_CritRateRefParm1 = "SupportTalent_CritRateRefParm1";

		public const string SupportTalent_CritDmgRef = "SupportTalent_RefCritDmg";

		public const string SupportTalent_CritDmgRefParm1 = "SupportTalent_CritDmgRefParm1";

		public const string SupportTalent_BodyshootRate = "SupportTalent_BodyshootRate";

		public const string SupportTalent_BodyshootRateParm1 = "SupportTalent_BodyshootRateParm1";

		public const string SupportTalent_BodyshootDmg = "SupportTalent_BodyshootDmg";

		public const string SupportTalent_BodyshootDmgParm1 = "SupportTalent_BodyshootDmgParm1";

		public const string SupportTalent_BodyshootRateRef = "SupportTalent_RefBodyshootRate";

		public const string SupportTalent_BodyshootRateRefParm1 = "SupportTalent_BodyshootRateRefParm1";

		public const string SupportTalent_BodyshootDmgRef = "SupportTalent_RefBodyshootDmg";

		public const string SupportTalent_BodyshootDmgRefParm1 = "SupportTalent_BodyshootDmgRefParm1";

		public const string SupportTalent_Guard = "SupportTalent_Guard";

		public const string SupportTalent_GuardParm1 = "SupportTalent_GuardParm1";

		public const string SupportTalent_GuardParm2 = "SupportTalent_GuardParm2";

		public const string SupportTalent_GuardParm3 = "SupportTalent_GuardParm3";

		public const string SupportTalent_Cure = "SupportTalent_Cure";

		public const string SupportTalent_CureParm1 = "SupportTalent_CureParm1";

		public const string SupportTalent_CureParm2 = "SupportTalent_CureParm2";

		public const string SupportTalent_Lowerlucky = "SupportTalent_Lowerlucky";

		public const string SupportTalent_LowerluckyParm1 = "SupportTalent_LowerluckyParm1";

		public const string SupportTalent_LowerluckyParm2 = "SupportTalent_LowerluckyParm2";

		public const string SupportTalent_LowerluckyParm3 = "SupportTalent_LowerluckyParm3";

		public const string SupportTalent_LowerluckyParm4 = "SupportTalent_LowerluckyParm4";

		public const string SupportTalent_resistDebuff = "SupportTalent_resistDebuff";

		public const string SupportTalent_resistDebuffParm1 = "SupportTalent_resistDebuffParm1";

		public const string ResistNegativeEffects = "ResistNegativeEffects";

		public const string ResistNegativeEffectsParm1 = "ResistNegativeEffectsParm1";

		public const string SupportTalent_DodgeRange = "SupportTalent_DodgeRange";

		public const string SupportTalent_DodgeRangeParm1 = "SupportTalent_DodgeRangeParm1";

		public const string SupportTalent_DodgeMelee = "SupportTalent_DodgeMelee";

		public const string SupportTalent_DodgeMeleeParm1 = "SupportTalent_DodgeMeleeParm1";

		public const string SupportTalent_HitrateRange = "SupportTalent_HitrateRange";

		public const string SupportTalent_HitrateRangeParm1 = "SupportTalent_HitrateRangeParm1";

		public const string SupportTalent_HitrateMelee = "SupportTalent_HitrateMelee";

		public const string SupportTalent_HitrateMeleeParm1 = "SupportTalent_HitrateMeleeParm1";

		public const string FollowAttackWithSplashDamage = "FollowAttackWithSplashDamage";

		public const string FollowAttackWithSplashDamageParam1 = "FollowAttackWithSplashDamageParam1";

		public const string FollowAttackWithSplashDamageParam2 = "FollowAttackWithSplashDamageParam2";

		public const string FollowAttackWithSplashDamageParam3 = "FollowAttackWithSplashDamageParam3";

		public const string FollowAttackWithSplashDamageParam4 = "FollowAttackWithSplashDamageParam4";

		public const string Blind = "Blind";

		public const string BlindParam1 = "BlindParam1";

		public const string BlindParam2 = "BlindParam2";

		public const string VengefulCharge = "VengefulCharge";

		public const string Equipment_VengefulCharge = "Equipment.VengefulCharge";

		public const string Equipment_VengefulCharge_MarkNum = "Equipment_VengefulCharge_MarkNum";

		public const string Equipment_VengefulCharge_APNum = "Equipment_VengefulCharge_APNum";

		public const string Equipment_VengefulCharge_APNum_Max = "Equipment_VengefulCharge_APNum_Max";

		public const string Equipment_VengefulCharge_MarkNum_Max = "Equipment_VengefulCharge_MarkNum_Max";

		public const string Equipment_VengefulCharge_MarkNumShadowedGuard = "Equipment_VengefulCharge_MarkNumShadowedGuard";

		public const string Equipment_VengefulCharge_PerMarkDmg = "Equipment_VengefulCharge_PerMarkDmg";

		public const string TacticalResupply = "TacticalResupply";

		public const string Equipment_LastStand = "Equipment.LastStand";

		public const string Equipment_LastStand_HPLowerMultiplier = "Equipment_LastStand_HPLowerMultiplier";

		public const string Equipment_LastStand_DmgMultiplier = "Equipment_LastStand_DmgMultiplier";

		public const string Defense = "Equipment.Defense";

		public const string Equipment_Defense_Melee = "Equipment_DefDefense_Melee";

		public const string Equipment_Defense_Melee_Percent = "Equipment_Defense_Melee_Percent";

		public const string Equipment_Defense_Range = "Equipment_DefDefense_Range";

		public const string Equipment_Defense_Range_Percent = "Equipment_Defense_Range_Percent";

		public const string LeaderBuffCitadel = "LeaderBuffCitadel";

		public const string BaseCitadel = "BaseCitadel";

		public static readonly FixedPoint Citadel_PercentBase = 0.10000000149011612;

		public const string LeaderBuffCitadel_Range = "LeaderBuffCitadel_Range";

		public const string LeaderBuffCitadel_TargetFaction = "LeaderBuffCitadel_TargetFaction";

		public const string LeaderBuffCitadel_DownOverWatchPercent = "LeaderBuffCitadel_DownOverWatchPercent";

		public const string Citadel_PursuitDown = "Citadel_PursuitDown";

		public const string Citadel_PursuitDown_LowerMultiplier = "Citadel_PursuitDown_LowerMultiplier";

		public const string Citadel_RangeDown = "Citadel_RangeDown";

		public const string Citadel_RangeDown_RangeMultiplier = "Citadel_RangeDown_RangeMultiplier";

		public const string Citadel_RangeDown_MinRangeMultiplier = "Citadel_RangeDown_MinRangeMultiplier";

		public const string Citadel_MoveDown = "Citadel_MoveDown";

		public const string Citadel_MoveDownNum = "Citadel_MoveDownNum";

		public const string LeaderBuffDeathsDoor = "LeaderBuffDeathsDoor";

		public const string BaseDeathsDoor = "BaseDeathsDoor";

		public const string LeaderBuffDeathsDoor_DmgUpPerLayer = "LeaderBuffDeathsDoor_DmgUpPerLayer";

		public const string LeaderBuffDeathsDoor_MaxLayer = "LeaderBuffDeathsDoor_MaxLayer";

		public const string LeaderBuffDeathsDoor_DmgUpDuration = "LeaderBuffDeathsDoor_DmgUpDuration";

		public const string LeaderBuffDeathsDoor_PursuitChance = "LeaderBuffDeathsDoor_PursuitChance";

		public const string LeaderBuffDeathsDoor_PursuitDmgUp = "LeaderBuffDeathsDoor_PursuitDmgUp";

		public const string LeaderBuffDeathsDoor_MaxPursuitCount = "LeaderBuffDeathsDoor_MaxPursuitCount";

		public const string LeaderBuffDeathsDoor_UnlockLevel = "LeaderBuffDeathsDoor_UnlockLevel";

		public const string LeaderBuffDeathsDoor_MaxDmgUp = "LeaderBuffDeathsDoor_MaxDmgUp";

		private static readonly string Stiff_ToLower = "Stiff".ToLower();

		private static readonly string Lucky_ToLower = "Lucky".ToLower();

		private static readonly string SupportTalent_Lowerlucky_ToLower = "SupportTalent_Lowerlucky".ToLower();

		private static readonly string FieldMedic_ToLower = "FieldMedic".ToLower();

		private static readonly string Soldier_ToLower = "Soldier".ToLower();

		private static readonly string EquipmentSoldier_ToLower = "Equipment.Soldier".ToLower();

		private static readonly string GuildBattleBuff_ToLower = "GuildBattleBuff".ToLower();

		private static readonly string Dodge_ToLower = "Dodge".ToLower();

		private static readonly string EquipmentDodge_ToLower = "Equipment.Dodge".ToLower();

		private static readonly string DodgedShotInjurerFlag_ToLower = "DodgedShotInjurerFlag".ToLower();

		private static readonly string DodgeShot_ToLower = "DodgeShot".ToLower();

		private static readonly string BulletDodge_ToLower = "BulletDodge".ToLower();

		private static readonly string EquipmentBulletDodge_ToLower = "Equipment.BulletDodge".ToLower();

		private static readonly string BaseMeleeDodge_ToLower = "BaseMeleeDodge".ToLower();

		private static readonly string BaseRangedDodge_ToLower = "BaseRangedDodge".ToLower();

		private static readonly string Jumpingshot_ToLower = "Jumpingshot".ToLower();

		private static readonly string ResistJumpingshot_ToLower = "ResistJumpingshot".ToLower();

		private static readonly string Strong_ToLower = "Strong".ToLower();

		private static readonly string EquipmentStrong_ToLower = "Equipment.Strong".ToLower();

		private static readonly string Weak_ToLower = "Weak".ToLower();

		private static readonly string Lethal_ToLower = "Lethal".ToLower();

		private static readonly string FortunaMain_ToLower = "Fortuna_Main".ToLower();

		private static readonly string Equipment_Passive_Fortuna_Spade_ToLower = "Equipment_Passive_Fortuna_Spade".ToLower();

		private static readonly string Equipment_Passive_Fortuna_Club_ToLower = "Equipment_Passive_Fortuna_Club".ToLower();

		private static readonly string Equipment_Passive_Fortuna_Heart_ToLower = "Equipment_Passive_Fortuna_Heart".ToLower();

		private static readonly string IronSkin_ToLower = "IronSkin".ToLower();

		private static readonly string DefensiveStance_ToLower = "DefensiveStance".ToLower();

		private static readonly string EquipmentDefensiveStance_ToLower = "Equipment.DefensiveStance".ToLower();

		private static readonly string Wrestler_ToLower = "Wrestler".ToLower();

		private static readonly string RetaliateMultiplier_ToLower = "RetaliateMultiplier".ToLower();

		private static readonly string PowerStrike_ToLower = "PowerStrike".ToLower();

		private static readonly string EquipmentPowerStrike_ToLower = "Equipment.PowerStrike".ToLower();

		private static readonly string SureShot_ToLower = "SureShot".ToLower();

		private static readonly string EquipmentSureShot_ToLower = "Equipment.SureShot".ToLower();

		private static readonly string LeaderBuffFinalDamage_ToLower = "LeaderBuffFinalDamage".ToLower();

		private static readonly string BoostFinalDamage_ToLower = "BoostFinalDamage".ToLower();

		private static readonly string FeaturedHeroBuffRarity_ToLower = "FeaturedHeroBuff.Rarity".ToLower();

		private static readonly string FeaturedHeroBuffHealth_ToLower = "FeaturedHeroBuff.Health".ToLower();

		private static readonly string FeaturedHeroBuffDamage_ToLower = "FeaturedHeroBuff.Damage".ToLower();

		private static readonly string LeaderBuffShooter_ToLower = "LeaderBuffShooter".ToLower();

		private static readonly string LeaderBuffHunter_ToLower = "LeaderBuffHunter".ToLower();

		private static readonly string LeaderBuffAssault_ToLower = "LeaderBuffAssault".ToLower();

		private static readonly string LeaderBuffWarrior_ToLower = "LeaderBuffWarrior".ToLower();

		private static readonly string LeaderBuffBruiser_ToLower = "LeaderBuffBruiser".ToLower();

		private static readonly string LeaderBuffScout_ToLower = "LeaderBuffScout".ToLower();

		private static readonly string LeaderBuffRanged_ToLower = "LeaderBuffRanged".ToLower();

		private static readonly string LeaderBuffMelee_ToLower = "LeaderBuffMelee".ToLower();

		private static readonly string LeaderBuffQuickLearner_ToLower = "LeaderBuffQuickLearner".ToLower();

		private static readonly string HeirloomsMaggiePocketWatch_ToLower = "Heirlooms_Maggie_PocketWatch".ToLower();

		private static readonly string LeaderBuffLooter_ToLower = "LeaderBuffLooter".ToLower();

		private static readonly string LeaderBuffKiller_ToLower = "LeaderBuffKiller".ToLower();

		private static readonly string LeaderBuffBodyguard_ToLower = "LeaderBuffBodyguard".ToLower();

		private static readonly string LeaderBuffPerceptive_ToLower = "LeaderBuffPerceptive".ToLower();

		private static readonly string LeaderBuffLeadByExample_ToLower = "LeaderBuffLeadByExample".ToLower();

		private static readonly string SurvivalManualStorySkill_B_ToLower = "SurvivalManualStorySkill_B".ToLower();

		private static readonly string SurvivalManualStorySkill_C_ToLower = "SurvivalManualStorySkill_C".ToLower();

		private static readonly string SurvivalManualStorySkill_D_ToLower = "SurvivalManualStorySkill_D".ToLower();

		private static readonly string SurvivalManualKillIncreaseDmgTrait_ToLower = "SurvivalManualKillIncreaseDmgTrait".ToLower();

		private static readonly string SurvivalManualStorySkill_E_ToLower = "SurvivalManualStorySkill_E".ToLower();

		private static readonly string SurvivalManualStorySkill_F_ToLower = "SurvivalManualStorySkill_F".ToLower();

		private static readonly string SurvivalManualStorySkill_G_ToLower = "SurvivalManualStorySkill_G".ToLower();

		private static readonly string SurvivalManualStorySkill_H_ToLower = "SurvivalManualStorySkill_H".ToLower();

		private static readonly string SurvivalManualStorySkill_I_ToLower = "SurvivalManualStorySkill_I".ToLower();

		private static readonly string SupportTalent_MoveHitrate_ToLower = "SupportTalent_MoveHitrate".ToLower();

		private static readonly string SupportTalent_MoveCritRate_ToLower = "SupportTalent_MoveCritRate".ToLower();

		private static readonly string SupportTalent_NoMoveHitrate_ToLower = "SupportTalent_NoMoveHitrate".ToLower();

		private static readonly string SupportTalent_NoMoveCritRate_ToLower = "SupportTalent_NoMoveCritRate".ToLower();

		private static readonly string SupportTalent_CritRate_ToLower = "SupportTalent_CritRate".ToLower();

		private static readonly string SupportTalent_CritDmg_ToLower = "SupportTalent_CritDmg".ToLower();

		private static readonly string SupportTalent_CritRateRef_ToLower = "SupportTalent_RefCritRate".ToLower();

		private static readonly string SupportTalent_CritDmgRef_ToLower = "SupportTalent_RefCritDmg".ToLower();

		private static readonly string SupportTalent_BodyshootRate_ToLower = "SupportTalent_BodyshootRate".ToLower();

		private static readonly string SupportTalent_BodyshootDmg_ToLower = "SupportTalent_BodyshootDmg".ToLower();

		private static readonly string SupportTalent_BodyshootRateRef_ToLower = "SupportTalent_RefBodyshootRate".ToLower();

		private static readonly string SupportTalent_BodyshootDmgRef_ToLower = "SupportTalent_RefBodyshootDmg".ToLower();

		private static readonly string SupportTalent_Guard_ToLower = "SupportTalent_Guard".ToLower();

		private static readonly string SupportTalent_Cure_ToLower = "SupportTalent_Cure".ToLower();

		private static readonly string SupportTalent_resistDebuff_ToLower = "SupportTalent_resistDebuff".ToLower();

		private static readonly string SupportTalent_DodgeRange_ToLower = "SupportTalent_DodgeRange".ToLower();

		private static readonly string SupportTalent_DodgeMelee_ToLower = "SupportTalent_DodgeMelee".ToLower();

		private static readonly string SupportTalent_HitrateRange_ToLower = "SupportTalent_HitrateRange".ToLower();

		private static readonly string SupportTalent_HitrateMelee_ToLower = "SupportTalent_HitrateMelee".ToLower();

		private static readonly string ResistNegativeEffects_ToLower = "ResistNegativeEffects".ToLower();

		private static readonly string FollowAttackWithSplashDamage_ToLower = "FollowAttackWithSplashDamage".ToLower();

		private static readonly string Blind_ToLower = "Blind".ToLower();

		private static readonly string BlindModifierTrait_ToLower = "ModifierBlinTrait".ToLower();

		private static readonly string Farmer_ToLower = "Farmer".ToLower();

		private static readonly string Gluttony_ToLower = "Gluttony".ToLower();

		private static readonly string BloodThirst_ToLower = "BloodThirst".ToLower();

		private static readonly string BittenRaged_ToLower = "BittenRaged".ToLower();

		private static readonly string EquipmentHazardSuit_ToLower = "Equipment.HazardSuit".ToLower();

		private static readonly string HelpHand_ToLower = "HelpHand".ToLower();

		private static readonly string EquipmentHelpHand_ToLower = "Equipment.HelpHand".ToLower();

		private static readonly string EquipmentPassiveHelpHand_ToLower = "Equipment_Passive_HelpHand".ToLower();

		private static readonly string MeleeResistance_ToLower = "MeleeResistance".ToLower();

		private static readonly string RangedResistance_ToLower = "RangedResistance".ToLower();

		private static readonly string FireResistance_ToLower = "FireResistance".ToLower();

		private static readonly string DistanceShield_ToLower = "DistanceShield".ToLower();

		private static readonly string BodyShotBonus_ToLower = "BodyShotBonus".ToLower();

		private static readonly string EquipmentActiveStun_ToLower = "Equipment_Active_Stun".ToLower();

		private static readonly string EquipmentActiveRiotShieldHerd_ToLower = "Equipment_Active_RiotShield_Herd".ToLower();

		private static readonly string EquipmentActiveRiotShieldStun_ToLower = "Equipment_Active_RiotShield_Stun".ToLower();

		private static readonly string EquipmentActiveEnsnare_ToLower = "Ensnare".ToLower();

		private static readonly string EquipmentActiveFacehurt_ToLower = "Facehurt".ToLower();

		private static readonly string HeirloomsBracelets_ToLower = "Heirlooms_Daryl_Bracelets".ToLower();

		private static readonly string Crippling_ToLower = "Crippling".ToLower();

		private static readonly string LeaderBuffGoodEnoughCrippleBase_ToLower = "LeaderBuffGoodEnoughCrippleBase".ToLower();

		private static readonly string FistSpike_ToLower = "FistSpike".ToLower();

		private static readonly string Poison_ToLower = "Poison".ToLower();

		private static readonly string PoisonBurst_ToLower = "PoisonBurst".ToLower();

		private static readonly string Pestilence_ToLower = "Pestilence".ToLower();

		private static readonly string HealthDmg_ToLower = "Healthdmg".ToLower();

		private static readonly string Perseverance_ToLower = "Perseverance".ToLower();

		private static readonly string EquipmentPerseverance_ToLower = "Equipment.Perseverance".ToLower();

		private static readonly string EquipmentActiveCripple_ToLower = "Equipment_Active_Cripple".ToLower();

		private static readonly string EquipmentActiveOverflow_ToLower = "Overflow".ToLower();

		private static readonly string EquipmentActiveSpecialStun_ToLower = "SpecialStun".ToLower();

		private static readonly string EquipmentActiveSpecialStunActiveFlag_ToLower = "Special_Stun_Active_Flag".ToLower();

		private static readonly string SpecialStunTargetActiveFlag_ToLower = "SpecialStunTargetActiveFlag".ToLower();

		private static readonly string Vigilance_ToLower = "Vigilance".ToLower();

		private static readonly string ArcUpgrade_ToLower = "ArcUpgrade".ToLower();

		private static readonly string Accurate_ToLower = "Accurate".ToLower();

		private static readonly string Destructive_ToLower = "Destructive".ToLower();

		private static readonly string WideArc_ToLower = "WideArc".ToLower();

		private static readonly string WideSpread_ToLower = "WideSpread".ToLower();

		private static readonly string HighPowered_ToLower = "HighPowered".ToLower();

		private static readonly string LargeCaliber_ToLower = "LargeCaliber".ToLower();

		private static readonly string Concussion_ToLower = "Concussion".ToLower();

		private static readonly string Inspiration_ToLower = "Inspiration".ToLower();

		private static readonly string TutorialSetDamage_ToLower = "TutorialSetDamage".ToLower();

		private static readonly string FollowThrough_ToLower = "FollowThrough".ToLower();

		private static readonly string EquipmentFollowThrough_ToLower = "Equipment.FollowThrough".ToLower();

		private static readonly string EquipmentActiveFollowThrough_ToLower = "Equipment_Active_FollowThrough".ToLower();

		private static readonly string CriticalAim_ToLower = "CriticalAim".ToLower();

		private static readonly string EquipmentCriticalAim_ToLower = "Equipment.CriticalAim".ToLower();

		private static readonly string Stagger_ToLower = "Stagger".ToLower();

		private static readonly string EquipmentStagger_ToLower = "Equipment.Stagger".ToLower();

		private static readonly string EquipmentFollowStatusStagger_ToLower = "Equipment.FollowStatus.Stagger".ToLower();

		private static readonly string StaggerActive_ToLower = "StaggerActive".ToLower();

		private static readonly string RemoteRepulse_ToLower = "RemoteRepulse".ToLower();

		private static readonly string Equipment_Passive_Range_Repulse_1_ToLower = "Equipment_Passive_Range_Repulse_1".ToLower();

		private static readonly string Equipment_Passive_Range_Repulse_2_ToLower = "Equipment_Passive_Range_Repulse_2".ToLower();

		private static readonly string ElectronCharge_ToLower = "ElectronCharge".ToLower();

		private static readonly string Conductive_ToLower = "Conductive".ToLower();

		private static readonly string CurrentSurge_ToLower = "CurrentSurge".ToLower();

		private static readonly string VoltCharge_ToLower = "VoltCharge".ToLower();

		private static readonly string VoltShock_ToLower = "VoltShock".ToLower();

		private static readonly string Quantun_ToLower = "Quantun".ToLower();

		private static readonly string ResurgenceType1_ToLower = "ResurgenceType1".ToLower();

		private static readonly string ResurgenceType2_ToLower = "ResurgenceType2".ToLower();

		private static readonly string FirstAid_ToLower = "FirstAid".ToLower();

		private static readonly string RandomStatus_ToLower = "RandomStatus".ToLower();

		private static readonly string Skinned_ToLower = "Skinned".ToLower();

		private static readonly string AddDamageAddAttack_ToLower = "AddDamage.AddAttack".ToLower();

		private static readonly string RangeArmorDominance_ToLower = "RangeArmorDominance".ToLower();

		private static readonly string EquipmentActiveBloodMark_ToLower = "Equipment.Active.BloodMark".ToLower();

		private static readonly string EquipmentPassiveBloodMark_ToLower = "Equipment.Passive.BloodMark".ToLower();

		private static readonly string EquipmentPassiveRemoveNegative_ToLower = "Equipment.Passive.RemoveNegative".ToLower();

		private static readonly string EquipmentPassivePreventControl_ToLower = "Equipment.Passive.PreventControl".ToLower();

		private static readonly string EquipmentPassiveMaxGetHitDamage_ToLower = "Equipment.Passive.MaxGetHitDamage".ToLower();

		private static readonly string EquipmentPassiveDamageAreaBlock_ToLower = "Equipment.Passive.DamageAreaBlock".ToLower();

		private static readonly string EquipmentPassiveLineSeparatedPlus_ToLower = "Equipment.Passive.LineSeparatedPlus".ToLower();

		private static readonly string RangeEquipmentDominance_ToLower = "RangeEquipmentDominance".ToLower();

		private static readonly string RangeActorDominance_ToLower = "RangeActorDominance".ToLower();

		private static readonly string AddDamageChargeAttack_ToLower = "AddDamage.ChargeAttack".ToLower();

		private static readonly string FreeChargePoint_ToLower = "FreeChargePoint".ToLower();

		private static readonly string BoostHitRate_ToLower = "BoostHitRate".ToLower();

		private static readonly string IgnoreDefense_ToLower = "IgnoreDefense".ToLower();

		private static readonly string LeaderBuffGoodEnough_ToLower = "LeaderBuffGoodEnough".ToLower();

		private static readonly string HealthBoost_ToLower = "HealthBoost".ToLower();

		private static readonly string EquipmentActiveLight_ToLower = "Equipment_Active_Light".ToLower();

		private static readonly string MultiAttacks_ToLower = "MultiAttacks".ToLower();

		private static readonly string MultiAttackExtraDamageActive_ToLower = "MultiAttackExtraDamageActive".ToLower();

		private static readonly string EquipmentActiveShieldBreakerStrikeType1_ToLower = "Equipment_Active_ShieldBreakerStrikeType1".ToLower();

		private static readonly string EquipmentActiveShieldBreakerStrikeType2_ToLower = "Equipment_Active_ShieldBreakerStrikeType2".ToLower();

		private static readonly string FreeRun_ToLower = "FreeRun".ToLower();

		private static readonly string EquipmentActiveGroupdmgboost_ToLower = "Equipment_Active_Groupdmgboost".ToLower();

		private static readonly string EquipmentApocalypticDMGScout_ToLower = "Equipment_Apocalyptic_DMG_Scout".ToLower();

		private static readonly string EquipmentActiveHPNailgun_ToLower = "Equipment_Active_HPNailgun".ToLower();

		private static readonly string EquipmentApocalypticDMGBruiser_ToLower = "Equipment_Apocalyptic_DMG_Bruiser".ToLower();

		private static readonly string EquipmentApocalypticDMGWarrior_ToLower = "Equipment_Apocalyptic_DMG_Warrior".ToLower();

		private static readonly string EquipmentApocalypticDMGShooter_ToLower = "Equipment_Apocalyptic_DMG_Shooter".ToLower();

		private static readonly string EquipmentApocalypticDMGHunter_ToLower = "Equipment_Apocalyptic_DMG_Hunter".ToLower();

		private static readonly string EquipmentApocalypticDMGAssault_ToLower = "Equipment_Apocalyptic_DMG_Assault".ToLower();

		private static readonly string EquipmentApocalypticBSScout_ToLower = "Equipment_Apocalyptic_BS_Scout".ToLower();

		private static readonly string EquipmentApocalypticBSBruiser_ToLower = "Equipment_Apocalyptic_BS_Bruiser".ToLower();

		private static readonly string EquipmentApocalypticBSWarrior_ToLower = "Equipment_Apocalyptic_BS_Warrior".ToLower();

		private static readonly string EquipmentApocalypticBSShooter_ToLower = "Equipment_Apocalyptic_BS_Shooter".ToLower();

		private static readonly string EquipmentApocalypticBSHunter_ToLower = "Equipment_Apocalyptic_BS_Hunter".ToLower();

		private static readonly string EquipmentApocalypticBSAssault_ToLower = "Equipment_Apocalyptic_BS_Assault".ToLower();

		private static readonly string EquipmentApocalypticDEFScout_ToLower = "Equipment_Apocalyptic_DEF_Scout".ToLower();

		private static readonly string EquipmentApocalypticDEFBruiser_ToLower = "Equipment_Apocalyptic_DEF_Bruiser".ToLower();

		private static readonly string EquipmentApocalypticDEFWarrior_ToLower = "Equipment_Apocalyptic_DEF_Warrior".ToLower();

		private static readonly string EquipmentApocalypticDEFShooter_ToLower = "Equipment_Apocalyptic_DEF_Shooter".ToLower();

		private static readonly string EquipmentApocalypticDEFHunter_ToLower = "Equipment_Apocalyptic_DEF_Hunter".ToLower();

		private static readonly string EquipmentApocalypticDEFAssault_ToLower = "Equipment_Apocalyptic_DEF_Assault".ToLower();

		private static readonly string EquipmentActiveKing_ToLower = "Equipment_Active_King".ToLower();

		private static readonly string EquipmentActiveSuppress1_ToLower = "Equipment_Active_Suppress_1".ToLower();

		private static readonly string EquipmentActiveSuppress2_ToLower = "Equipment_Active_Suppress_2".ToLower();

		private static readonly string EquipmentActiveInterruptor_ToLower = "Equipment_Active_Interruptor".ToLower();

		private static readonly string Interruptor_ToLower = "Interruptor".ToLower();

		private static readonly string Protective_ToLower = "Protective".ToLower();

		private static readonly string EquipmentActiveFocusMode_ToLower = "Equipment_Active_FocusMode".ToLower();

		private static readonly string EquipmentActiveBreakthrough_ToLower = "Equipment_Active_Breakthrough".ToLower();

		private static readonly string EquipmentBreakthrough_ToLower = "Equipment.Breakthrough".ToLower();

		private static readonly string EquipmentProtective_ToLower = "Equipment.Protective".ToLower();

		private static readonly string Charging_ToLower = "Charging".ToLower();

		private static readonly string Ruthless_ToLower = "Ruthless".ToLower();

		private static readonly string Burning_ToLower = "Burning".ToLower();

		private static readonly string PvP_HumanVsHumanDamageResistance_ToLower = "PvP_HumanVsHumanDamageResistance".ToLower();

		private static readonly string PVP_SurvivorVsRaiderDamageResistance_ToLower = "PVP_SurvivorVsRaiderDamageMultiplier".ToLower();

		private static readonly string PVP_RaiderVsSurvivorDamageResistance_ToLower = "PVP_RaiderVsSurvivorDamageMultiplier".ToLower();

		private static readonly string Silenced_ToLower = "Silenced".ToLower();

		private static readonly string LeaderBuffNoThreatRanged_ToLower = "LeaderBuffNoThreatRanged".ToLower();

		private static readonly string ThreatReduction_ToLower = "ThreatReduction".ToLower();

		private static readonly string ThreatFree_ToLower = "ThreatFree".ToLower();

		private static readonly string LeaderBuffReduceThreatMelee_ToLower = "LeaderBuffReduceThreatMelee".ToLower();

		private static readonly string LeaderBuffDontTouchMyAllies_ToLower = "LeaderBuffDontTouchMyAllies".ToLower();

		private static readonly string LeaderBuffCriticalChance_ToLower = "LeaderBuffCriticalChance".ToLower();

		private static readonly string LeaderBuffCriticalResistance_ToLower = "LeaderBuffCriticalResistance".ToLower();

		private static readonly string LeaderBuffExtraChargePointAtAttackDmgTaken_ToLower = "LeaderBuffExtraChargePointAtAttackDmgTaken".ToLower();

		private static readonly string LeaderBuffDeadlyTactics_ToLower = "LeaderBuffDeadlyTactics".ToLower();

		private static readonly string Piercing_ToLower = "Piercing".ToLower();

		private static readonly string Razor_ToLower = "Razor".ToLower();

		private static readonly string EquipmentIncendiary_ToLower = "Equipment.Incendiary".ToLower();

		private static readonly string LeaderBuffSecondChance_ToLower = "LeaderBuffSecondChance".ToLower();

		private static readonly string SecondChance_ToLower = "SecondChance".ToLower();

		private static readonly string LeaderBuffHealingCharge_ToLower = "LeaderBuffHealingCharge".ToLower();

		private static readonly string LeaderBuffSurvivalInstinct_ToLower = "LeaderBuffSurvivalInstinct".ToLower();

		private static readonly string Bulletproof_ToLower = "Bulletproof".ToLower();

		private static readonly string PointBlankShot_ToLower = "PointBlankShot".ToLower();

		private static readonly string BoostTotalHealth_ToLower = "BoostTotalHealth".ToLower();

		private static readonly string LeaderBuffBringThemOn_ToLower = "LeaderBuffBringThemOn".ToLower();

		private static readonly string LeaderBuffGoodOutOfBad_ToLower = "LeaderBuffGoodOutOfBad".ToLower();

		private static readonly string LeaderBuffHunterDesperation_ToLower = "LeaderBuffHunterDesperation".ToLower();

		private static readonly string Retaliate_ToLower = "Retaliate".ToLower();

		private static readonly string EquipmentRetaliate_ToLower = "Equipment.Retaliate".ToLower();

		private static readonly string LeaderBuffRegalAuthority_ToLower = "LeaderBuffRegalAuthority".ToLower();

		private static readonly string BaseRegalAuthority_ToLower = "BaseRegalAuthority".ToLower();

		private static readonly string LeaderBuffMulletTime_ToLower = "LeaderBuffMulletTime".ToLower();

		private static readonly string LeaderBuffMysteriousWays_ToLower = "LeaderBuffMysteriousWays".ToLower();

		private static readonly string LeaderBuffTeamwork_ToLower = "LeaderBuffTeamwork".ToLower();

		private static readonly string LeaderBuffOnlyTheBest_ToLower = "LeaderBuffOnlyTheBest".ToLower();

		private static readonly string LeaderBuffJackass_ToLower = "LeaderBuffJackass".ToLower();

		private static readonly string EquipmentStunResistance_ToLower = "Equipment.StunResistance".ToLower();

		private static readonly string LeaderBuffReadyForAction_ToLower = "LeaderBuffReadyForAction".ToLower();

		private static readonly string LeaderBuffForestStalker_ToLower = "LeaderBuffForestStalker".ToLower();

		private static readonly string LeaderBuffJustice_ToLower = "LeaderBuffJustice".ToLower();

		private static readonly string LeaderBuffColdBlooded_ToLower = "LeaderBuffColdBlooded".ToLower();

		private static readonly string LeaderBuffOneWithTheHerdStalker_ToLower = "LeaderBuffOneWithTheHerdStalker".ToLower();

		private static readonly string LeaderBuffOneWithTheHerd_ToLower = "LeaderBuffOneWithTheHerd".ToLower();

		private static readonly string LeaderBuffExplosiveBullets_ToLower = "LeaderBuffExplosiveBullets".ToLower();

		private static readonly string LeaderBuffBeatEmUp_ToLower = "LeaderBuffBeatEmUp".ToLower();

		private static readonly string BaseBeatEmUp_ToLower = "BaseBeatEmUp".ToLower();

		private static readonly string LeaderBuffFightingFury_ToLower = "LeaderBuffFightingFury".ToLower();

		private static readonly string BaseFightingFury_ToLower = "BaseFightingFury".ToLower();

		private static readonly string FightingFury_ToLower = "FightingFury".ToLower();

		private static readonly string LeaderBuffBetterTogether_ToLower = "LeaderBuffBetterTogether".ToLower();

		private static readonly string BaseBetterTogether_ToLower = "BaseBetterTogether".ToLower();

		private static readonly string LeaderBuffInspire_ToLower = "LeaderBuffInspire".ToLower();

		private static readonly string LeaderBuffPrincess_ToLower = "LeaderBuffPrincess".ToLower();

		private static readonly string LeaderBuffMarkEnemy_ToLower = "LeaderBuffMarkEnemy".ToLower();

		private static readonly string DebuffMarkEnemy_ToLower = "DebuffMarkEnemy".ToLower();

		private static readonly string EquipmentActiveRipped_ToLower = "Equipment_Active_Ripped".ToLower();

		private static readonly string EquipmentActiveAssistAttack_ToLower = "Equipment_Active_AssistAttack".ToLower();

		private static readonly string EquipmentActiveAssistAttackActive_ToLower = "EquipmentActiveAssistAttackActive".ToLower();

		private static readonly string EquipmentActiveChargeLoad_ToLower = "Equipment_Active_ChargeLoad".ToLower();

		private static readonly string LeaderBuffFiringSquad_ToLower = "LeaderBuffFiringSquad".ToLower();

		private static readonly string FiringSquadMember_ToLower = "FiringSquadMember".ToLower();

		private static readonly string FiringSquadLeader_ToLower = "FiringSquadLeader".ToLower();

		private static readonly string FiringSquadDamageActive_ToLower = "FiringSquadDamageActive".ToLower();

		private static readonly string LeaderBuffEmitter_ToLower = "LeaderBuffEmitter".ToLower();

		private static readonly string EmitterCreator_ToLower = "EmitterCreator".ToLower();

		private static readonly string EmitterDamageActive_ToLower = "EmitterDamageActive".ToLower();

		private static readonly string LeaderBuffHeadshot_ToLower = "LeaderBuffHeadshot".ToLower();

		private static readonly string BaseHeadshot_ToLower = "BaseHeadshot".ToLower();

		private static readonly string LeaderBuffCoupDeGrace_ToLower = "LeaderBuffCoupDeGrace".ToLower();

		private static readonly string BaseCoupDeGrace_ToLower = "BaseCoupDeGrace".ToLower();

		private static readonly string CoupDeGraceActive_ToLower = "CoupDeGraceActive".ToLower();

		private static readonly string LeaderBuffMadeToSuffer_ToLower = "LeaderBuffMadeToSuffer".ToLower();

		private static readonly string SufferCreator_ToLower = "SufferCreator".ToLower();

		private static readonly string LeaderBuffUnleashedFighter_ToLower = "LeaderBuffUnleashedFighter".ToLower();

		private static readonly string BaseUnleashedFighter_ToLower = "BaseUnleashedFighter".ToLower();

		private static readonly string UnleashedActive_ToLower = "UnleashedActive".ToLower();

		private static readonly string TrapFlame_ToLower = "TrapFlame".ToLower();

		private static readonly string AttackChain_ToLower = "AttackChain".ToLower();

		private static readonly string Asthenia_ToLower = "Asthenia".ToLower();

		private static readonly string GrenadeFragmentDamage_ToLower = "GrenadeFragmentDamage".ToLower();

		private static readonly string InspirePerKillIncreaseDamageModifierTrait_ToLower = "InspirePerKillIncreaseDamageModifierTrait".ToLower();

		private static readonly string InspirePerKillIncreaseExtraChargePointChanceModifierTrait_ToLower = "InspirePerKillIncreaseExtraChargePointChanceModifierTrait".ToLower();

		private static readonly string EquipmentRevenge_ToLower = "Equipment.Revenge".ToLower();

		private static readonly string ShieldRevenge_ToLower = "ShieldRevenge".ToLower();

		private static readonly string EquipmentPunish_ToLower = "Equipment.Punish".ToLower();

		private static readonly string FlatBaseDamage_ToLower = "FlatBaseDamage".ToLower();

		private static readonly string FlatHealth_ToLower = "FlatHealth".ToLower();

		private static readonly string FlatCritDamage_ToLower = "FlatCritDamage".ToLower();

		private static readonly string DamageReductionBonus_ToLower = "GuildBattleBuff.DamageReduction".ToLower();

		private static readonly string DamageBonus_ToLower = "GuildBattleBuff.Damage".ToLower();

		private static readonly string BodyShotReductionBonus_ToLower = "GuildBattleBuff.Piercing".ToLower();

		private static readonly string CriticalChanceBonus_ToLower = "GuildBattleBuff.CriticalChance".ToLower();

		private static readonly string FullChargeChanceBonus_ToLower = "GuildBattleBuff.FullCharge".ToLower();

		private static readonly string DodgeChanceBonus_ToLower = "GuildBattleBuff.Dodge".ToLower();

		private static readonly string EquipmentTactical_ToLower = "Equipment.Tactical".ToLower();

		private static readonly string EquipmentArmorTactical_ToLower = "Equipment.ArmorTactical".ToLower();

		private static readonly string PreventPush_ToLower = "PreventPush".ToLower();

		private static readonly string PreventIncendiary_ToLower = "PreventIncendiary".ToLower();

		private static readonly string EquipmentShield_ToLower = "Equipment.Shield".ToLower();

		private static readonly string EquipmentPassiveShield_ToLower = "Equipment_Passive_Shield".ToLower();

		private static readonly string CommonwealthArmorTrait_ToLower = "CommonwealthArmorTrait".ToLower();

		private static readonly string CommonwealthArmorActive_ToLower = "CommonwealthArmorActive".ToLower();

		private static readonly string PastaSupportTrait_ToLower = "PastaSupportTrait".ToLower();

		private static readonly string PastaSupportActive_ToLower = "PastaSupportActive".ToLower();

		private static readonly string TemFullyStateTrait_ToLower = "TemFullyStateTrait".ToLower();

		private static readonly string Riposte_ToLower = "Riposte".ToLower();

		private static readonly string CommonwealthArmorExtraChargeChance_ToLower = "CommonwealthArmorExtraChargeChance".ToLower();

		private static readonly string StruggleInvulnerable_ToLower = "StruggleInvulnerable".ToLower();

		private static readonly string TutorialInvulnerable_ToLower = "TutorialInvulnerable".ToLower();

		private static readonly string TutorialUninterruptable_ToLower = "TutorialUninterruptable".ToLower();

		private static readonly string EquipmentActiveExtraAP_ToLower = "Equipment_Active_ExtraAP".ToLower();

		private static readonly string Explosive_ToLower = "Explosive".ToLower();

		private static readonly string EmptyTactical_ToLower = "EmptyTactical".ToLower();

		private static readonly string Bleeding_ToLower = "Bleeding".ToLower();

		private static readonly string Impenetrable_ToLower = "Impenetrable".ToLower();

		private static readonly string PushCollisionDamage_ToLower = "PushCollisionDamage".ToLower();

		private static readonly string Gore_ToLower = "Gore".ToLower();

		private static readonly string InfiniteRange_ToLower = "InfiniteRange".ToLower();

		private static readonly string RangedDamageFalloff_ToLower = "RangedDamageFalloff".ToLower();

		private static readonly string CarolsCookiesTrait_ToLower = "CarolsCookiesTrait".ToLower();

		private static readonly string CarolsCookiesActive_ToLower = "CarolsCookiesActive".ToLower();

		private static readonly string PrimedChance_ToLower = "PrimedChance".ToLower();

		private static readonly string WalkerMikeActive_ToLower = "WalkerMikeActive".ToLower();

		private static readonly string HealthThresholdedStatusResistance_ToLower = "HealthThresholdedStatusResistance".ToLower();

		private static readonly string FirstStrike_ToLower = "FirstStrike".ToLower();

		private static readonly string Fortified_ToLower = "Fortified".ToLower();

		private static readonly string LeaderBuffKnockKnock_ToLower = "LeaderBuffKnockKnock".ToLower();

		private static readonly string Equipment_Passive_TornApart_ToLower = "Equipment_Passive_TornApart".ToLower();

		private static readonly string Equipment_Passive_FreeOW_ToLower = "Equipment_Passive_FreeOW".ToLower();

		private static readonly string Equipment_Passive_SawAxe_ToLower = "Equipment_Passive_SawAxe".ToLower();

		private static readonly string BaseKnockKnock_ToLower = "BaseKnockKnock".ToLower();

		private static readonly string LeaderBuffRedact_ToLower = "LeaderBuffRedact".ToLower();

		private static readonly string LeaderBuffProtect_ToLower = "LeaderBuffProtect".ToLower();

		private static readonly string LeaderBuffClosingTime_ToLower = "LeaderBuffClosingTime".ToLower();

		private static readonly string BaseClosingTime_ToLower = "BaseClosingTime".ToLower();

		private static readonly string Pursuit_ToLower = "Pursuit".ToLower();

		private static readonly string EquipmentActiveAdvance_ToLower = "Equipment_Active_Advance".ToLower();

		private static readonly string Repulse_ToLower = "Repulse".ToLower();

		private static readonly string NegativeFatal_ToLower = "NegativeFatal".ToLower();

		private static readonly string EquipmentActiveDisoriented_ToLower = "Equipment_Active_Disoriented".ToLower();

		private static readonly string EquipmentActiveRecoil_ToLower = "Equipment_Active_Recoil".ToLower();

		private static readonly string LeaderBuffABTester_ToLower = "LeaderBuffABTester".ToLower();

		private static readonly string BaseABTester_ToLower = "BaseABTester".ToLower();

		private static readonly string BounsPhonePortrait_ToLower = "Heirlooms_Rick_PhonePortrait".ToLower();

		private static readonly string Heirlooms_RiotGearGlenn_Fetter_ToLower = "Heirlooms_RiotGearGlenn_Fetter".ToLower();

		private static readonly string Heirlooms_Hershel_Fetter_ToLower = "Heirlooms_Hershel_Fetter".ToLower();

		private static readonly string HealthBoostBouns_ToLower = "HealthBoostBouns".ToLower();

		private static readonly string FlameDMGReduceBouns_ToLower = "FlameDMGReduceBouns".ToLower();

		private static readonly string LeaderBuffNoExceptions_ToLower = "LeaderBuffNoExceptions".ToLower();

		private static readonly string BaseNoExceptions_ToLower = "BaseNoExceptions".ToLower();

		private static readonly string LeaderBuffOverload_ToLower = "LeaderBuffOverload".ToLower();

		private static readonly string BaseOverload_ToLower = "BaseOverload".ToLower();

		private static readonly string OverloadEXDamageActive_ToLower = "OverloadDamageActive".ToLower();

		private static readonly string Equipment_Passive_Detonation1_ToLower = "Equipment_Passive_Detonation_1".ToLower();

		private static readonly string Equipment_Passive_Detonation_ToLower = "Equipment_Passive_Detonation".ToLower();

		private static readonly string StrengthenDefenseFunc1_ToLower = "StrengthenDefenseFunc1".ToLower();

		private static readonly string StrengthenDefenseFunc2_ToLower = "StrengthenDefenseFunc2".ToLower();

		private static readonly string StrengthenDefenseFunc3_ToLower = "StrengthenDefenseFunc3".ToLower();

		private static readonly string Equipment_Passive_Flame_ToLower = "Equipment_Passive_Flame".ToLower();

		private static readonly string Equipment_Passive_DefendingHeart_ToLower = "Equipment_Passive_DefendingHeart".ToLower();

		private static readonly string GodWarBless_ToLower = "GodWarBless".ToLower();

		private static readonly string Equipment_Passive_Dash_ToLower = "Equipment_Passive_Dash".ToLower();

		private static readonly string ChargeAttackWithFreeShooting_ToLower = "ChargeAttackWithFreeShooting".ToLower();

		private static readonly string EquipmentPassiveFightBack_ToLower = "Equipment.Passive.FightBack".ToLower();

		private static readonly string Equipment_Passive_Rage_ToLower = "Equipment_Passive_Rage".ToLower();

		private static readonly string Equipment_Passive_PassOW_ToLower = "Equipment_Passive_PassOW".ToLower();

		private static readonly string Equipment_Passive_ScoutDMGBoost_ToLower = "Equipment_Passive_ScoutDMGBoost".ToLower();

		private static readonly string Equipment_Passive_BruiserDMGBoost_ToLower = "Equipment_Passive_BruiserDMGBoost".ToLower();

		private static readonly string Equipment_Passive_WarriorDMGBoost_ToLower = "Equipment_Passive_WarriorDMGBoost".ToLower();

		private static readonly string Equipment_Passive_ShooterDMGBoost_ToLower = "Equipment_Passive_ShooterDMGBoost".ToLower();

		private static readonly string Equipment_Passive_HunterDMGBoost_ToLower = "Equipment_Passive_HunterDMGBoost".ToLower();

		private static readonly string Equipment_Passive_AssaultDMGBoost_ToLower = "Equipment_Passive_AssaultDMGBoost".ToLower();

		private static readonly string AttackWithTriggerDot_ToLower = "AttackWithTriggerDot".ToLower();

		private static readonly string Equipment_Passive_HPPercentDmg_ToLower = "Equipment_Passive_HPPercentDmg".ToLower();

		private static readonly string Equipment_Active_BloodFrenzy_ToLower = "Equipment_Active_BloodFrenzy".ToLower();

		private static readonly string LeaderBuffSurvivalGame_ToLower = "LeaderBuffSurvivalGame".ToLower();

		private static readonly string DeadlyFocusEXDamageActive_ToLower = "DeadlyFocusEXDamageActive".ToLower();

		private static readonly string BaseDeadlyFocus_ToLower = "BaseDeadlyFocus".ToLower();

		private static readonly string LeaderBuffDeadlyFocus_ToLower = "LeaderBuffDeadlyFocus".ToLower();

		private static readonly string SurvivalManualStorySkill_A_ToLower = "SurvivalManualStorySkill_A".ToLower();

		private static readonly string LeaderBuffShadowedGuard_ToLower = "LeaderBuffShadowedGuard".ToLower();

		private static readonly string ShadowedGuard_StateRef_ToLower = "ShadowedGuard_StateRef".ToLower();

		private static readonly string Equipment_VengefulCharge_ToLower = "Equipment.VengefulCharge".ToLower();

		private static readonly string Equipment_LastStand_ToLower = "Equipment.LastStand".ToLower();

		private static readonly string Defense_ToLower = "Equipment.Defense".ToLower();

		private static readonly string StunResistance_ToLower = "StunResistance".ToLower();

		private static readonly string Revenge_ToLower = "Revenge".ToLower();

		private static readonly string Punish_ToLower = "Punish".ToLower();

		private static readonly string EquipmentSniperHarness_ToLower = "Equipment.SniperHarness".ToLower();

		private static readonly string EquipmentTrainingGear_ToLower = "Equipment.TrainingGear".ToLower();

		private static readonly string EquipmentActiveExtraDamageExecution_ToLower = "Equipment_Active_ExtraDamageExecution".ToLower();

		private static readonly string EquipmentActiveCriticalPenetratesArmor_ToLower = "Equipment_Active_CriticalPenetratesArmor".ToLower();

		private static readonly string EquipmentActiveSkinned_ToLower = "Equipment_Active_Skinned".ToLower();

		private static readonly string EquipmentFollowStatusSkinned_ToLower = "Equipment.FollowStatus.Skinned".ToLower();

		private static readonly string Equipment_Passive_Backstep_ToLower = "Equipment_Passive_Backstep".ToLower();

		private static readonly string Equipment_Passive_ShotGun_ToLower = "Equipment_Passive_ShotGun".ToLower();

		private static readonly string HealthRealdmg_ToLower = "Equipment_Active_HealthRealdmg".ToLower();

		private static readonly string AddDamageNormalAttack_ToLower = "AddDamage.NormalAttack".ToLower();

		private static readonly string EquipmentKaboom_ToLower = "Equipment.Kaboom".ToLower();

		private static readonly string DebuffEquipmentKaboom_ToLower = "DebuffEquipmentKaboom".ToLower();

		private static readonly string LeaderBuffDeathsDoor_ToLower = "LeaderBuffDeathsDoor".ToLower();

		private static readonly string BaseDeathsDoor_ToLower = "BaseDeathsDoor".ToLower();

		public const string Undying = "Undying";

		private static readonly ICollection<string> DeprecatedTraitsLower = new string[2]
		{
			"LeaderBuffNeedOnlyOne".ToLower(),
			"PVP_RaiderVsSurvivorDamageModifierTrait".ToLower()
		};

		private Dictionary<string, TraitEntry> traitsByIdentifier = new Dictionary<string, TraitEntry>();

		private Dictionary<string, List<TraitEntry>> traitsByBaseIdentifier = new Dictionary<string, List<TraitEntry>>(StringComparer.OrdinalIgnoreCase);

		private static readonly string[] DefaultResistDebuffRemovableNegatives = new string[7] { "Stun", "Bleeding", "Root", "Cripple", "Burning", "StaggerActive", "ElectricShock" };

		public List<TraitEntry> Traits { get; private set; }

		public ActorTraitContainerModel()
		{
			Traits = new List<TraitEntry>();
			traitsByIdentifier = new Dictionary<string, TraitEntry>();
			traitsByBaseIdentifier = new Dictionary<string, List<TraitEntry>>(StringComparer.OrdinalIgnoreCase);
		}

		public override void Start()
		{
			base.Start();
			RebuildDictionaries();
		}

		private void RebuildDictionaries()
		{
			traitsByIdentifier.Clear();
			traitsByBaseIdentifier.Clear();
			for (int i = 0; i < Traits.Count; i++)
			{
				TraitEntry traitEntry = Traits[i];
				if (!traitsByIdentifier.ContainsKey(traitEntry.TraitIdentifier))
				{
					traitsByIdentifier[traitEntry.TraitIdentifier] = traitEntry;
				}
				string key = UpgradeTraitsData.StripTraitLevelIdentifier(traitEntry.TraitIdentifier);
				if (!traitsByBaseIdentifier.TryGetValue(key, out var value))
				{
					value = new List<TraitEntry>();
					traitsByBaseIdentifier[key] = value;
				}
				value.Add(traitEntry);
			}
		}

		public override bool IsValid()
		{
			return true;
		}

		public TraitEntry GetTrait(string traitIdentifier)
		{
			string text = traitIdentifier.ToLower();
			for (int i = 0; i < Traits.Count; i++)
			{
				if (Traits[i].TraitIdentifier.ToLower() == text)
				{
					return Traits[i];
				}
			}
			return null;
		}

		public TraitEntry GetTraitAnyLevel(string traitIdentifier)
		{
			if (traitsByBaseIdentifier.TryGetValue(traitIdentifier, out var value) && value.Count > 0)
			{
				return value[0];
			}
			return null;
		}

		public void ReplaceTraits(List<TraitEntry> newTraits)
		{
			Traits = newTraits;
			RebuildDictionaries();
		}

		public TraitEntry AddTrait(string traitIdentifier, FixedPoint additivePercentageMultiplier, bool isTemporary = false, long duration = 0L, string tag = "", List<int> RemodeIndex = null, List<int> RemodeValue = null)
		{
			if (base.manager.GameEconomyData.GetTraitDefinition(traitIdentifier) != null)
			{
				TraitEntry traitEntry = new TraitEntry(traitIdentifier, duration, 1.0 + additivePercentageMultiplier / 100.0);
				traitEntry.IsTemporary = isTemporary;
				traitEntry.Tag = tag;
				traitEntry.RemodeParamIndex = RemodeIndex;
				traitEntry.RemodeValues = RemodeValue;
				Traits.Add(traitEntry);
				if (!traitsByIdentifier.ContainsKey(traitIdentifier))
				{
					traitsByIdentifier[traitIdentifier] = traitEntry;
				}
				string key = UpgradeTraitsData.StripTraitLevelIdentifier(traitIdentifier);
				if (!traitsByBaseIdentifier.TryGetValue(key, out var value))
				{
					value = new List<TraitEntry>();
					traitsByBaseIdentifier[key] = value;
				}
				value.Add(traitEntry);
				return traitEntry;
			}
			base.manager.Debug.LogError("Failed to create trait '" + traitIdentifier + "' could not find trait definition for it!");
			return null;
		}

		public TraitEntry AddTraitByEntry(TraitEntry entry)
		{
			if (base.manager.GameEconomyData.GetTraitDefinition(entry.TraitIdentifier) != null)
			{
				TraitEntry traitEntry = new TraitEntry(entry);
				Traits.Add(traitEntry);
				if (!traitsByIdentifier.ContainsKey(entry.TraitIdentifier))
				{
					traitsByIdentifier[entry.TraitIdentifier] = traitEntry;
				}
				string key = UpgradeTraitsData.StripTraitLevelIdentifier(entry.TraitIdentifier);
				if (!traitsByBaseIdentifier.TryGetValue(key, out var value))
				{
					value = new List<TraitEntry>();
					traitsByBaseIdentifier[key] = value;
				}
				value.Add(traitEntry);
				return traitEntry;
			}
			base.manager.Debug.LogError("Failed to create trait '" + entry.TraitIdentifier + "' could not find trait definition for it!");
			return null;
		}

		public void RemoveTrait(string traitIdentifier)
		{
			for (int num = Traits.Count - 1; num >= 0; num--)
			{
				if (Traits[num].TraitIdentifier == traitIdentifier)
				{
					Traits.RemoveAt(num);
				}
			}
			traitsByIdentifier.Remove(traitIdentifier);
			string key = UpgradeTraitsData.StripTraitLevelIdentifier(traitIdentifier);
			if (!traitsByBaseIdentifier.TryGetValue(key, out var value))
			{
				return;
			}
			for (int num2 = value.Count - 1; num2 >= 0; num2--)
			{
				if (value[num2].TraitIdentifier == traitIdentifier)
				{
					value.RemoveAt(num2);
				}
			}
			if (value.Count == 0)
			{
				traitsByBaseIdentifier.Remove(key);
			}
		}

		private List<string> GetSupportTalentResistDebuffRemoveNegativeList()
		{
			GameEconomyData gameEconomyData = ((base.manager != null) ? base.manager.GameEconomyData : null);
			if (gameEconomyData != null && gameEconomyData.TraitDefinitions != null)
			{
				for (int i = 0; i < gameEconomyData.TraitDefinitions.Length; i++)
				{
					TraitDefinition traitDefinition = gameEconomyData.TraitDefinitions[i];
					if (traitDefinition != null && !string.IsNullOrEmpty(traitDefinition.Identifier) && traitDefinition.Identifier.IndexOf("SupportTalent_resistDebuff", StringComparison.OrdinalIgnoreCase) >= 0)
					{
						if (traitDefinition.EffectIndex == null || traitDefinition.EffectIndex.Count <= 0)
						{
							break;
						}
						return new List<string>(traitDefinition.EffectIndex);
					}
				}
			}
			return new List<string>(DefaultResistDebuffRemovableNegatives);
		}

		private void CreateAndAddIncrementerModifier(List<ModelModifier> modifiers, FixedPoint[] parameters, string[] parameterNames, string displayName)
		{
			for (int i = 0; i < ((parameters != null) ? parameters.Length : 0); i++)
			{
				FixedPoint inIncrement = parameters[i];
				AbilityModifierIncrementer item = new AbilityModifierIncrementer(parameterNames[i], inIncrement);
				modifiers.Add(item);
			}
		}

		private void CreateAndAddIncrementerClassModifier(List<ModelModifier> modifiers, FixedPoint parameter, SurvivorClass survivorClass, string parameterName, string displayName)
		{
			AbilityModifierClassIncrementer item = new AbilityModifierClassIncrementer(parameterName, parameter, survivorClass);
			modifiers.Add(item);
		}

		private void CreateAndAddIncrementerHeroModifier(List<ModelModifier> modifiers, string actorDefinition, FixedPoint parameter, string parameterName, string displayName, bool includeAltVersion = false)
		{
			AbilityModifierHeroIncrementer item = new AbilityModifierHeroIncrementer(parameterName, parameter, actorDefinition, includeAltVersion);
			modifiers.Add(item);
		}

		public List<ModelModifier> CreateTraitModifiers(TraitDefinition traitDefinition, FixedPoint constructionParametersMultiplier, FixedPoint? chance)
		{
			List<ModelModifier> list = new List<ModelModifier>();
			string traitIdentifier = traitDefinition.Identifier.Replace("_Debug", "");
			traitIdentifier = UpgradeTraitsData.StripTraitLevelIdentifier(traitIdentifier);
			traitIdentifier = traitIdentifier.ToLower();
			if (traitIdentifier == Stiff_ToLower)
			{
				CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) }, new string[1] { "Damage" }, traitDefinition.DisplayName);
			}
			else if (traitIdentifier.Contains(SupportTalent_Lowerlucky_ToLower))
			{
				CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) * constructionParametersMultiplier }, new string[1] { "SupportTalent_LowerluckyParm1" }, traitDefinition.DisplayName);
				CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(2) * constructionParametersMultiplier }, new string[1] { "SupportTalent_LowerluckyParm3" }, traitDefinition.DisplayName);
				CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(3) * constructionParametersMultiplier }, new string[1] { "SupportTalent_LowerluckyParm4" }, traitDefinition.DisplayName);
				LowerUnluckyTrait item = new LowerUnluckyTrait(traitDefinition.GetParameter<string>(1));
				list.Add(item);
			}
			else if (traitIdentifier.Contains(Lucky_ToLower))
			{
				CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "ExtendProbability" }, traitDefinition.DisplayName);
			}
			else if (traitIdentifier == FieldMedic_ToLower)
			{
				CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 }, new string[1] { "PercentageIncreaseHealing" }, traitDefinition.DisplayName);
			}
			else if (traitIdentifier.Contains(Soldier_ToLower))
			{
				CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "PercentageIncreaseRangeDamage" }, traitDefinition.DisplayName);
			}
			else if (traitIdentifier.Contains(EquipmentSoldier_ToLower))
			{
				CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "PercentageNewIncreaseRangeDamage" }, traitDefinition.DisplayName);
			}
			else if (!traitIdentifier.Contains(GuildBattleBuff_ToLower) && (traitIdentifier == Dodge_ToLower || traitIdentifier == EquipmentDodge_ToLower))
			{
				CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierIncreaseMeleeDodgeChance" }, traitDefinition.DisplayName);
			}
			else if (traitIdentifier == DodgedShotInjurerFlag_ToLower)
			{
				AbilityModifierDodgedShotInjurer item2 = new AbilityModifierDodgedShotInjurer(constructionParametersMultiplier - 1.0);
				list.Add(item2);
			}
			else if (traitIdentifier.Contains(DodgeShot_ToLower))
			{
				AbilityModifierDodgeShot item3 = new AbilityModifierDodgeShot(traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier, traitDefinition.GetParameter<FixedPoint>(1) * constructionParametersMultiplier, traitDefinition.GetParameter<FixedPoint>(2) * constructionParametersMultiplier, traitDefinition.GetParameter<FixedPoint>(3) * constructionParametersMultiplier);
				list.Add(item3);
			}
			else if (traitIdentifier == BulletDodge_ToLower)
			{
				CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierIncreaseRangedDodgeChance" }, traitDefinition.DisplayName);
			}
			else if (traitIdentifier.Contains(EquipmentBulletDodge_ToLower))
			{
				CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierIncreaseRangedEquipmentBulletDodgeChance" }, traitDefinition.DisplayName);
			}
			else if (traitIdentifier == BaseMeleeDodge_ToLower || traitIdentifier == BaseRangedDodge_ToLower)
			{
				AbilityModifierDodge item4 = new AbilityModifierDodge((traitIdentifier == BaseMeleeDodge_ToLower) ? DamageType.Melee : DamageType.Ranged);
				list.Add(item4);
			}
			else if (traitIdentifier.Contains(ResistJumpingshot_ToLower))
			{
				CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierPercentageReduceJumpingshotDamage" }, traitDefinition.DisplayName);
			}
			else if (traitIdentifier.Contains(Jumpingshot_ToLower))
			{
				AbilityModifierJumpingshot item5 = new AbilityModifierJumpingshot(traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier, traitDefinition.GetParameter<FixedPoint>(1) / 100.0 * constructionParametersMultiplier, traitDefinition.GetParameter<FixedPoint>(2), traitDefinition.GetParameter<FixedPoint>(3) / 100.0 * constructionParametersMultiplier);
				list.Add(item5);
			}
			else if (traitIdentifier.Contains(Strong_ToLower) || traitIdentifier == Weak_ToLower)
			{
				CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "PercentageIncreaseMeleeDamage" }, traitDefinition.DisplayName);
			}
			else if (traitIdentifier.Contains(EquipmentStrong_ToLower) || traitIdentifier == Weak_ToLower)
			{
				CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierEquipPercentageIncreaseMeleeDamage" }, traitDefinition.DisplayName);
			}
			else if (traitIdentifier.Contains(Lethal_ToLower))
			{
				CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierPercentageMultiplyFinalDamageIncrementer" }, traitDefinition.DisplayName);
			}
			else if (traitIdentifier.Contains(FortunaMain_ToLower))
			{
				List<int> randomTalentIds = new List<int>
				{
					traitDefinition.GetParameter<int>(1),
					traitDefinition.GetParameter<int>(2),
					traitDefinition.GetParameter<int>(3)
				};
				FortunaMainTrait item6 = new FortunaMainTrait(traitDefinition.GetParameter<int>(0), randomTalentIds);
				list.Add(item6);
			}
			else if (traitIdentifier.Contains(Equipment_Passive_Fortuna_Spade_ToLower))
			{
				CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierEquipmentPassiveFortunaSpade" }, traitDefinition.DisplayName);
			}
			else if (traitIdentifier.Contains(Equipment_Passive_Fortuna_Club_ToLower))
			{
				CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) }, new string[1] { "AbilityModifierEquipmentPassiveFortunaClub" }, traitDefinition.DisplayName);
			}
			else if (traitIdentifier.Contains(Equipment_Passive_Fortuna_Heart_ToLower))
			{
				CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) }, new string[1] { "AbilityModifierEquipmentPassiveFortunaHeart" }, traitDefinition.DisplayName);
			}
			else if (traitIdentifier.Contains(IronSkin_ToLower))
			{
				CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierPercentageIncreaseResistance" }, traitDefinition.DisplayName);
			}
			else if (traitIdentifier.Contains(DefensiveStance_ToLower))
			{
				CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierPercentageIncreaseResistanceOverwatch" }, traitDefinition.DisplayName);
			}
			else if (traitIdentifier.Contains(EquipmentDefensiveStance_ToLower))
			{
				CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierPercentageIncreaseNewResistanceOverwatch" }, traitDefinition.DisplayName);
			}
			else if (traitIdentifier.Contains(Wrestler_ToLower))
			{
				CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierGiveDamageOnStruggle" }, traitDefinition.DisplayName);
				CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(1) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierGiveDamageOnStruggleVariance" }, traitDefinition.DisplayName);
				CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(2) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierGiveDamageOnStruggleRoundModifier" }, traitDefinition.DisplayName);
			}
			else if (traitIdentifier == RetaliateMultiplier_ToLower)
			{
				CreateAndAddIncrementerModifier(list, new FixedPoint[1] { -1.0 + (constructionParametersMultiplier - 1.0) }, new string[1] { "AbilityModifierPercentageMultiplyFinalDamage" }, traitDefinition.DisplayName);
			}
			else if (traitIdentifier.Contains(PowerStrike_ToLower))
			{
				FixedPoint fixedPoint = traitDefinition.GetParameter<FixedPoint>(0) / 100.0;
				CreateAndAddIncrementerModifier(list, new FixedPoint[1] { fixedPoint }, new string[1] { "AbilityModifierPercentageMultiplyFinalDamageNoMove" }, traitDefinition.DisplayName);
				CreateAndAddIncrementerModifier(list, new FixedPoint[1] { fixedPoint }, new string[1] { "PercentageIncreaseCriticalChanceNoMove" }, traitDefinition.DisplayName);
			}
			else if (traitIdentifier.Contains(EquipmentPowerStrike_ToLower))
			{
				FixedPoint fixedPoint2 = traitDefinition.GetParameter<FixedPoint>(0) / 100.0;
				CreateAndAddIncrementerModifier(list, new FixedPoint[1] { fixedPoint2 }, new string[1] { "AbilityModifierPercentageMultiplyFinalNewDamageNoMove" }, traitDefinition.DisplayName);
				CreateAndAddIncrementerModifier(list, new FixedPoint[1] { fixedPoint2 }, new string[1] { "PercentageIncreaseCriticalChanceNoMove" }, traitDefinition.DisplayName);
			}
			else if (traitIdentifier.Contains(SureShot_ToLower))
			{
				CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 }, new string[1] { "PercentageIncreaseCriticalChanceNoMove" }, traitDefinition.DisplayName);
			}
			else if (traitIdentifier.Contains(EquipmentSureShot_ToLower))
			{
				CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 }, new string[1] { "PercentageIncreaseCriticalChanceNoMove" }, traitDefinition.DisplayName);
			}
			else if (traitIdentifier.Contains(LeaderBuffFinalDamage_ToLower))
			{
				CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 }, new string[1] { "AbilityModifierPercentageMultiplyFinalDamageIncrementer" }, traitDefinition.DisplayName);
			}
			else if (traitIdentifier.Contains(BoostFinalDamage_ToLower))
			{
				CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 }, new string[1] { "AbilityModifierPercentageMultiplyFinalDamageIncrementerBadges" }, traitDefinition.DisplayName);
			}
			else if (traitIdentifier.Contains(FeaturedHeroBuffRarity_ToLower))
			{
				CreateAndAddIncrementerHeroModifier(list, traitDefinition.GetParameter<string>(0), traitDefinition.GetParameter<FixedPoint>(1), "AbilityModifierRarityModifierFeaturedHero", traitDefinition.DisplayName);
			}
			else if (traitIdentifier.Contains(FeaturedHeroBuffHealth_ToLower))
			{
				CreateAndAddIncrementerHeroModifier(list, traitDefinition.GetParameter<string>(0), traitDefinition.GetParameter<FixedPoint>(1) / 100.0, "AbilityModifierPercentageMultiplyHealthFeaturedHero", traitDefinition.DisplayName);
			}
			else if (traitIdentifier.Contains(FeaturedHeroBuffDamage_ToLower))
			{
				CreateAndAddIncrementerHeroModifier(list, traitDefinition.GetParameter<string>(0), traitDefinition.GetParameter<FixedPoint>(1) / 100.0, "AbilityModifierPercentageMultiplyFinalDamageFeaturedHero", traitDefinition.DisplayName);
			}
			else if (traitIdentifier.Contains(LeaderBuffShooter_ToLower))
			{
				CreateAndAddIncrementerModifier(list, new FixedPoint[2]
				{
					traitDefinition.GetParameter<FixedPoint>(0) / 100.0,
					traitDefinition.GetParameter<FixedPoint>(1) / 100.0
				}, new string[2] { "AbilityModifierPercentageMultiplyFinalDamageShooter", "AbilityModifierPercentageMultiplyHealthShooter" }, traitDefinition.DisplayName);
			}
			else if (traitIdentifier == LeaderBuffHunter_ToLower)
			{
				CreateAndAddIncrementerModifier(list, new FixedPoint[2]
				{
					traitDefinition.GetParameter<FixedPoint>(0) / 100.0,
					traitDefinition.GetParameter<FixedPoint>(1) / 100.0
				}, new string[2] { "AbilityModifierPercentageMultiplyFinalDamageHunter", "AbilityModifierPercentageMultiplyHealthHunter" }, traitDefinition.DisplayName);
			}
			else if (traitIdentifier.Contains(LeaderBuffAssault_ToLower))
			{
				CreateAndAddIncrementerModifier(list, new FixedPoint[2]
				{
					traitDefinition.GetParameter<FixedPoint>(0) / 100.0,
					traitDefinition.GetParameter<FixedPoint>(1) / 100.0
				}, new string[2] { "AbilityModifierPercentageMultiplyFinalDamageAssault", "AbilityModifierPercentageMultiplyHealthAssault" }, traitDefinition.DisplayName);
			}
			else if (traitIdentifier.Contains(LeaderBuffWarrior_ToLower))
			{
				CreateAndAddIncrementerModifier(list, new FixedPoint[2]
				{
					traitDefinition.GetParameter<FixedPoint>(0) / 100.0,
					traitDefinition.GetParameter<FixedPoint>(1) / 100.0
				}, new string[2] { "AbilityModifierPercentageMultiplyFinalDamageWarrior", "AbilityModifierPercentageMultiplyHealthWarrior" }, traitDefinition.DisplayName);
			}
			else if (traitIdentifier.Contains(LeaderBuffBruiser_ToLower))
			{
				CreateAndAddIncrementerModifier(list, new FixedPoint[2]
				{
					traitDefinition.GetParameter<FixedPoint>(0) / 100.0,
					traitDefinition.GetParameter<FixedPoint>(1) / 100.0
				}, new string[2] { "AbilityModifierPercentageMultiplyFinalDamageBruiser", "AbilityModifierPercentageMultiplyHealthBruiser" }, traitDefinition.DisplayName);
			}
			else if (traitIdentifier.Contains(LeaderBuffScout_ToLower))
			{
				CreateAndAddIncrementerModifier(list, new FixedPoint[2]
				{
					traitDefinition.GetParameter<FixedPoint>(0) / 100.0,
					traitDefinition.GetParameter<FixedPoint>(1) / 100.0
				}, new string[2] { "AbilityModifierPercentageMultiplyFinalDamageScout", "AbilityModifierPercentageMultiplyHealthScout" }, traitDefinition.DisplayName);
			}
			else if (traitIdentifier.Contains(LeaderBuffRanged_ToLower))
			{
				CreateAndAddIncrementerModifier(list, new FixedPoint[2]
				{
					traitDefinition.GetParameter<FixedPoint>(0) / 100.0,
					traitDefinition.GetParameter<FixedPoint>(1) / 100.0
				}, new string[2] { "AbilityModifierPercentageMultiplyFinalDamageRanged", "AbilityModifierPercentageMultiplyHealthRanged" }, traitDefinition.DisplayName);
			}
			else if (traitIdentifier.Contains(LeaderBuffMelee_ToLower))
			{
				CreateAndAddIncrementerModifier(list, new FixedPoint[2]
				{
					traitDefinition.GetParameter<FixedPoint>(0) / 100.0,
					traitDefinition.GetParameter<FixedPoint>(1) / 100.0
				}, new string[2] { "AbilityModifierPercentageMultiplyFinalDamageMelee", "AbilityModifierPercentageMultiplyHealthMelee" }, traitDefinition.DisplayName);
			}
			else if (traitIdentifier.Contains(LeaderBuffQuickLearner_ToLower))
			{
				CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 }, new string[1] { "AbilityModifierPercentageMultiplyKillSP" }, traitDefinition.DisplayName);
				CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(1) / 100.0 }, new string[1] { "AbilityModifierIncreaseScorchChance" }, traitDefinition.DisplayName);
				CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(2) }, new string[1] { "AbilityModifierIncreaseScorchTurns" }, traitDefinition.DisplayName);
				CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(3) }, new string[1] { "AbilityModifierIncreaseExtraScorchDamageChance" }, traitDefinition.DisplayName);
				CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(4) }, new string[1] { "AbilityModifierIncreaseScorchLayers" }, traitDefinition.DisplayName);
			}
			else if (traitIdentifier.Contains(HeirloomsMaggiePocketWatch_ToLower))
			{
				CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 }, new string[1] { "HeirloomsMaggiePocketWatchBurnChance" }, traitDefinition.DisplayName);
				CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(1) / 100.0 }, new string[1] { "HeirloomsMaggiePocketWatchScorchChance" }, traitDefinition.DisplayName);
				CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(2) }, new string[1] { "HeirloomsMaggiePocketWatchScorchTurns" }, traitDefinition.DisplayName);
			}
			else if (traitIdentifier.Contains(LeaderBuffLooter_ToLower))
			{
				CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 }, new string[1] { "AbilityModifierPercentageMultiplyKillSupplies" }, traitDefinition.DisplayName);
				CreateAndAddIncrementerModifier(list, new FixedPoint[1], new string[1] { "LeaderBuffLooter" }, traitDefinition.DisplayName);
			}
			else if (traitIdentifier.Contains(LeaderBuffKiller_ToLower))
			{
				CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 }, new string[1] { "AbilityModifierPercentageMultiplyFinalDamageVsHumans" }, traitDefinition.DisplayName);
			}
			else if (traitIdentifier.Contains(LeaderBuffBodyguard_ToLower))
			{
				CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierIncreaseChanceForBodyguard" }, traitDefinition.DisplayName);
				CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(1) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierIncreaseDamageReductionIfJerryOrNextToJerry" }, traitDefinition.DisplayName);
			}
			else if (traitIdentifier.Contains(LeaderBuffPerceptive_ToLower))
			{
				CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierIncreaseRevengeDamage" }, traitDefinition.DisplayName);
				CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierIncreasePunishDamage" }, traitDefinition.DisplayName);
				CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierIncreaseRetaliateDamage" }, traitDefinition.DisplayName);
				CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "LeaderBuffBeatEmUpPunishMultiplier" }, traitDefinition.DisplayName);
				CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) * constructionParametersMultiplier }, new string[1] { "PercentageIncreaseOverwatchDamage" }, traitDefinition.DisplayName);
			}
			else if (traitIdentifier.Contains(LeaderBuffLeadByExample_ToLower))
			{
				CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierIncreaseChargeAbilityDamage" }, traitDefinition.DisplayName);
				CreateAndAddIncrementerModifier(list, new FixedPoint[1], new string[1] { "LeaderBuffLeadByExample" }, traitDefinition.DisplayName);
			}
			else if (traitIdentifier.Contains(SurvivalManualStorySkill_B_ToLower))
			{
				CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierIncreaseChargeAbilityDamage" }, traitDefinition.DisplayName);
			}
			else if (traitIdentifier.Contains(SurvivalManualStorySkill_C_ToLower))
			{
				CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "SurvivalManualIncreaseDmg" }, traitDefinition.DisplayName);
			}
			else if (traitIdentifier.Contains(SurvivalManualStorySkill_D_ToLower))
			{
				CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "SurvivalManualKillIncreaseDmg" }, traitDefinition.DisplayName);
				CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(1) / 100.0 * constructionParametersMultiplier }, new string[1] { "SurvivalManualKillMaxIncreaseDmg" }, traitDefinition.DisplayName);
			}
			else if (traitIdentifier.Contains(SurvivalManualKillIncreaseDmgTrait_ToLower))
			{
				CreateAndAddIncrementerModifier(list, new FixedPoint[1] { constructionParametersMultiplier }, new string[1] { "SurvivalManualCurKillIncreaseDmg" }, traitDefinition.DisplayName);
			}
			else if (traitIdentifier.Contains(SurvivalManualStorySkill_E_ToLower))
			{
				CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "SurvivalManualStorySkill_EParm1" }, traitDefinition.DisplayName);
				CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(1) / 100.0 * constructionParametersMultiplier }, new string[1] { "SurvivalManualStorySkill_EParm2" }, traitDefinition.DisplayName);
				CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(2) / 100.0 * constructionParametersMultiplier }, new string[1] { "SurvivalManualStorySkill_EParm3" }, traitDefinition.DisplayName);
			}
			else if (traitIdentifier.Contains(SurvivalManualStorySkill_F_ToLower))
			{
				CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) * constructionParametersMultiplier }, new string[1] { "SurvivalManualStorySkill_FParm1" }, traitDefinition.DisplayName);
				CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(1) * constructionParametersMultiplier }, new string[1] { "SurvivalManualStorySkill_FParm2" }, traitDefinition.DisplayName);
				CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(2) / 100.0 * constructionParametersMultiplier }, new string[1] { "SurvivalManualStorySkill_FParm3" }, traitDefinition.DisplayName);
			}
			else if (traitIdentifier.Contains(SurvivalManualStorySkill_G_ToLower))
			{
				CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "SurvivalManualStorySkill_GParm1" }, traitDefinition.DisplayName);
				CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(1) / 100.0 * constructionParametersMultiplier }, new string[1] { "SurvivalManualStorySkill_GParm2" }, traitDefinition.DisplayName);
				CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(2) / 100.0 * constructionParametersMultiplier }, new string[1] { "SurvivalManualStorySkill_GParm3" }, traitDefinition.DisplayName);
			}
			else if (traitIdentifier.Contains(SurvivalManualStorySkill_H_ToLower))
			{
				CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "SurvivalManualStorySkill_HParm1" }, traitDefinition.DisplayName);
				CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(1) * constructionParametersMultiplier }, new string[1] { "SurvivalManualStorySkill_HParm2" }, traitDefinition.DisplayName);
				CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(2) / 100.0 * constructionParametersMultiplier }, new string[1] { "SurvivalManualStorySkill_HParm3" }, traitDefinition.DisplayName);
			}
			else if (traitIdentifier.Contains(SurvivalManualStorySkill_I_ToLower))
			{
				CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "SurvivalManualStorySkill_IParm1" }, traitDefinition.DisplayName);
				CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(1) * constructionParametersMultiplier }, new string[1] { "SurvivalManualStorySkill_IParm2" }, traitDefinition.DisplayName);
				CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(2) / 100.0 * constructionParametersMultiplier }, new string[1] { "SurvivalManualStorySkill_IParm3" }, traitDefinition.DisplayName);
			}
			else if (traitIdentifier.Contains(SupportTalent_MoveHitrate_ToLower))
			{
				CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "SupportTalent_MoveHitrateParm1" }, traitDefinition.DisplayName);
				CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(1) * constructionParametersMultiplier }, new string[1] { "SupportTalent_MoveHitrateParm2" }, traitDefinition.DisplayName);
			}
			else if (traitIdentifier.Contains(SupportTalent_MoveCritRate_ToLower))
			{
				CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "SupportTalent_MoveCritRateParm1" }, traitDefinition.DisplayName);
				CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(1) * constructionParametersMultiplier }, new string[1] { "SupportTalent_MoveCritRateParm2" }, traitDefinition.DisplayName);
			}
			else if (traitIdentifier.Contains(SupportTalent_NoMoveHitrate_ToLower))
			{
				CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "SupportTalent_NoMoveHitrateParm1" }, traitDefinition.DisplayName);
			}
			else if (traitIdentifier.Contains(SupportTalent_NoMoveCritRate_ToLower))
			{
				CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "SupportTalent_NoMoveCritRateParm1" }, traitDefinition.DisplayName);
			}
			else if (traitIdentifier.Contains(SupportTalent_CritRate_ToLower))
			{
				CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "SupportTalent_CritRateParm1" }, traitDefinition.DisplayName);
			}
			else if (traitIdentifier.Contains(SupportTalent_CritDmg_ToLower))
			{
				CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "SupportTalent_CritDmgParm1" }, traitDefinition.DisplayName);
			}
			else if (traitIdentifier.Contains(SupportTalent_CritRateRef_ToLower))
			{
				CreateAndAddIncrementerModifier(list, new FixedPoint[1] { -traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "SupportTalent_CritRateRefParm1" }, traitDefinition.DisplayName);
			}
			else if (traitIdentifier.Contains(SupportTalent_CritDmgRef_ToLower))
			{
				CreateAndAddIncrementerModifier(list, new FixedPoint[1] { -traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "SupportTalent_CritDmgRefParm1" }, traitDefinition.DisplayName);
			}
			else if (traitIdentifier.Contains(SupportTalent_BodyshootRate_ToLower))
			{
				CreateAndAddIncrementerModifier(list, new FixedPoint[1] { -traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "SupportTalent_BodyshootRateParm1" }, traitDefinition.DisplayName);
			}
			else if (traitIdentifier.Contains(SupportTalent_BodyshootDmg_ToLower))
			{
				CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "SupportTalent_BodyshootDmgParm1" }, traitDefinition.DisplayName);
			}
			else if (traitIdentifier.Contains(SupportTalent_BodyshootRateRef_ToLower))
			{
				CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "SupportTalent_BodyshootRateRefParm1" }, traitDefinition.DisplayName);
			}
			else if (traitIdentifier.Contains(SupportTalent_BodyshootDmgRef_ToLower))
			{
				CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "SupportTalent_BodyshootDmgRefParm1" }, traitDefinition.DisplayName);
			}
			else if (traitIdentifier.Contains(SupportTalent_Guard_ToLower))
			{
				CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) * constructionParametersMultiplier }, new string[1] { "SupportTalent_GuardParm1" }, traitDefinition.DisplayName);
				CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(1) / 100.0 * constructionParametersMultiplier }, new string[1] { "SupportTalent_GuardParm2" }, traitDefinition.DisplayName);
				CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(2) / 100.0 * constructionParametersMultiplier }, new string[1] { "SupportTalent_GuardParm3" }, traitDefinition.DisplayName);
				GuardFriendsTrait item7 = new GuardFriendsTrait();
				list.Add(item7);
			}
			else if (traitIdentifier.Contains(SupportTalent_Cure_ToLower))
			{
				CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) * constructionParametersMultiplier }, new string[1] { "SupportTalent_CureParm1" }, traitDefinition.DisplayName);
				CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(1) / 100.0 * constructionParametersMultiplier }, new string[1] { "SupportTalent_CureParm2" }, traitDefinition.DisplayName);
				CureTrait item8 = new CureTrait();
				list.Add(item8);
			}
			else if (traitIdentifier.Contains(SupportTalent_resistDebuff_ToLower))
			{
				CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "SupportTalent_resistDebuffParm1" }, traitDefinition.DisplayName);
				ResistDebuffTrait item9 = new ResistDebuffTrait();
				list.Add(item9);
			}
			else if (traitIdentifier.Contains(ResistNegativeEffects_ToLower))
			{
				CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "ResistNegativeEffectsParm1" }, traitDefinition.DisplayName);
				ResistNegativeEffectsTrait item10 = new ResistNegativeEffectsTrait(traitDefinition.EffectIndex);
				list.Add(item10);
			}
			else if (traitIdentifier.Contains(SupportTalent_DodgeRange_ToLower))
			{
				CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "SupportTalent_DodgeRangeParm1" }, traitDefinition.DisplayName);
			}
			else if (traitIdentifier.Contains(SupportTalent_DodgeMelee_ToLower))
			{
				CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "SupportTalent_DodgeMeleeParm1" }, traitDefinition.DisplayName);
			}
			else if (traitIdentifier.Contains(SupportTalent_HitrateRange_ToLower))
			{
				CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "SupportTalent_HitrateRangeParm1" }, traitDefinition.DisplayName);
			}
			else if (traitIdentifier.Contains(SupportTalent_HitrateMelee_ToLower))
			{
				CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "SupportTalent_HitrateMeleeParm1" }, traitDefinition.DisplayName);
			}
			else if (traitIdentifier.Contains(FollowAttackWithSplashDamage_ToLower))
			{
				CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "FollowAttackWithSplashDamageParam1" }, traitDefinition.DisplayName);
				CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(1) / 100.0 * constructionParametersMultiplier }, new string[1] { "FollowAttackWithSplashDamageParam2" }, traitDefinition.DisplayName);
				CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(2) * constructionParametersMultiplier }, new string[1] { "FollowAttackWithSplashDamageParam3" }, traitDefinition.DisplayName);
				CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(3) * constructionParametersMultiplier }, new string[1] { "FollowAttackWithSplashDamageParam4" }, traitDefinition.DisplayName);
				FollowAttackWithSplashDamageTrait item11 = new FollowAttackWithSplashDamageTrait();
				list.Add(item11);
			}
			else if (traitIdentifier.Contains(Blind_ToLower) && traitIdentifier != BlindModifierTrait_ToLower)
			{
				CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "BlindParam1" }, traitDefinition.DisplayName);
				CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(1) * constructionParametersMultiplier }, new string[1] { "BlindParam2" }, traitDefinition.DisplayName);
			}
			else if (traitIdentifier == Farmer_ToLower || traitIdentifier == Gluttony_ToLower)
			{
				CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 }, new string[1] { "AbilityModifierPercentageIncreaseCampProduction" + CurrencyType.Supplies }, traitDefinition.DisplayName);
			}
			else if (traitIdentifier == BloodThirst_ToLower)
			{
				CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 }, new string[1] { "AbilityModifierBloodThirst" }, traitDefinition.DisplayName);
			}
			else if (traitIdentifier == BittenRaged_ToLower)
			{
				CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 }, new string[1] { "PercentageIncreaseMeleeDamage" }, traitDefinition.DisplayName);
			}
			else if (traitIdentifier == EquipmentHazardSuit_ToLower)
			{
				CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierDecreaseBurningDamage" }, traitDefinition.DisplayName);
				CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(1) / 100.0 * constructionParametersMultiplier }, new string[1] { AbilityModifierIncreaseBodyShot.FetchIncreaseBodyShotChance }, traitDefinition.DisplayName);
				CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(1) / 100.0 * constructionParametersMultiplier }, new string[1] { AbilityModifierIncreaseMeleeBodyShot.FetchIncreaseMeleeBodyShotChance }, traitDefinition.DisplayName);
			}
			else if (traitIdentifier.Contains(HelpHand_ToLower) || traitIdentifier.Contains(EquipmentHelpHand_ToLower) || traitIdentifier.Contains(EquipmentPassiveHelpHand_ToLower))
			{
				CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "HelpHandGuardianshipProbability" }, traitDefinition.DisplayName);
				CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(1) }, new string[1] { "HelpHandNumberOfGuardianGrids" }, traitDefinition.DisplayName);
				CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(2) / 100.0 * constructionParametersMultiplier }, new string[1] { "HelpHandGuardianDamageValues" }, traitDefinition.DisplayName);
				HelpHandTrait item12 = new HelpHandTrait();
				list.Add(item12);
			}
			else if (traitIdentifier.Contains(MeleeResistance_ToLower))
			{
				CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierPercentageIncreaseResistanceMelee" }, traitDefinition.DisplayName);
			}
			else if (traitIdentifier.Contains(RangedResistance_ToLower))
			{
				CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierPercentageIncreaseResistanceRanged" }, traitDefinition.DisplayName);
			}
			else if (traitIdentifier.Contains(FireResistance_ToLower))
			{
				CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierDecreaseBurningDamage" }, traitDefinition.DisplayName);
			}
			else if (traitIdentifier.Contains(DistanceShield_ToLower))
			{
				FixedPoint resistanceHitPointsPercent = traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier;
				FixedPoint parameter = traitDefinition.GetParameter<FixedPoint>(1);
				list.Add(new DistanceShieldResistanceTrait(resistanceHitPointsPercent, parameter));
			}
			else if (traitIdentifier.Contains(BodyShotBonus_ToLower))
			{
				FixedPoint fixedPoint3 = traitDefinition.GetParameter<FixedPoint>(0);
				if (chance.HasValue)
				{
					fixedPoint3 = chance.Value;
				}
				FixedPoint parameter2 = traitDefinition.GetParameter<FixedPoint>(1);
				AbilityModifierIncreaseBodyShot item13 = new AbilityModifierIncreaseBodyShot(fixedPoint3 / 100L, parameter2 / 100L);
				list.Add(item13);
			}
			else if (traitIdentifier.Contains(EquipmentActiveStun_ToLower))
			{
				int parameter3 = traitDefinition.GetParameter<int>(0);
				FixedPoint fixedPoint4 = traitDefinition.GetParameter<FixedPoint>(1);
				if (chance.HasValue)
				{
					fixedPoint4 = chance.Value;
				}
				StunTrait item14 = new StunTrait(parameter3, (int)(fixedPoint4 * constructionParametersMultiplier));
				list.Add(item14);
			}
			else if (traitIdentifier.Contains(EquipmentActiveRiotShieldHerd_ToLower))
			{
				RiotShieldHerdTrait item15 = new RiotShieldHerdTrait((int)traitDefinition.GetParameter<FixedPoint>(0));
				list.Add(item15);
			}
			else if (traitIdentifier.Contains(EquipmentActiveRiotShieldStun_ToLower))
			{
				RiotShieldStunTrait item16 = new RiotShieldStunTrait((int)traitDefinition.GetParameter<FixedPoint>(0));
				list.Add(item16);
			}
			else if (traitIdentifier.Contains(EquipmentActiveEnsnare_ToLower))
			{
				int parameter4 = traitDefinition.GetParameter<int>(0);
				FixedPoint meleePercentage = traitDefinition.GetParameter<FixedPoint>(1) / 100.0 * constructionParametersMultiplier;
				FixedPoint rangedPercentage = traitDefinition.GetParameter<FixedPoint>(2) / 100.0 * constructionParametersMultiplier;
				EnsnareTrait item17 = new EnsnareTrait(parameter4, meleePercentage, rangedPercentage);
				list.Add(item17);
			}
			else if (traitIdentifier.Contains(EquipmentActiveFacehurt_ToLower))
			{
				int parameter5 = traitDefinition.GetParameter<int>(0);
				FixedPoint rootChance = traitDefinition.GetParameter<FixedPoint>(1) / 100.0 * constructionParametersMultiplier;
				FacehurtTrait item18 = new FacehurtTrait(parameter5, rootChance);
				list.Add(item18);
			}
			else if (traitIdentifier.Contains(HeirloomsBracelets_ToLower))
			{
				CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "BraceletsGainChargePointChanceForAll" }, traitDefinition.DisplayName);
				BraceletsTrait item19 = new BraceletsTrait(traitDefinition.GetParameter<int>(1));
				list.Add(item19);
			}
			else if (traitIdentifier.Contains(Crippling_ToLower) || traitIdentifier.Contains(LeaderBuffGoodEnoughCrippleBase_ToLower))
			{
				int parameter6 = traitDefinition.GetParameter<int>(0);
				FixedPoint chancePercentage = traitDefinition.GetParameter<FixedPoint>(1) / 100.0 * constructionParametersMultiplier;
				if (chance.HasValue)
				{
					chancePercentage = chance.Value;
				}
				bool workOnlyOnBodyShots = traitIdentifier.Contains(Crippling_ToLower);
				CrippleTrait item20 = new CrippleTrait(parameter6, chancePercentage, workOnlyOnBodyShots);
				list.Add(item20);
			}
			else if (traitIdentifier.Contains(FistSpike_ToLower))
			{
				FistSpikeTrait item21 = new FistSpikeTrait(traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier, traitDefinition.GetParameter<int>(1));
				list.Add(item21);
			}
			else if (traitIdentifier.Contains(Poison_ToLower) && !traitIdentifier.Contains(PoisonBurst_ToLower))
			{
				PoisonTrait item22 = new PoisonTrait(traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier, traitDefinition.GetParameter<int>(1), traitDefinition.GetParameter<FixedPoint>(2) / 100.0 * constructionParametersMultiplier, traitDefinition.GetParameter<int>(3));
				list.Add(item22);
			}
			else if (traitIdentifier.Contains(Pestilence_ToLower))
			{
				PestilenceTrait item23 = new PestilenceTrait(traitDefinition.GetParameter<FixedPoint>(0), traitDefinition.GetParameter<int>(1));
				list.Add(item23);
			}
			else if (traitIdentifier.Contains(PoisonBurst_ToLower))
			{
				PoisonBurstTrait item24 = new PoisonBurstTrait(traitDefinition.GetParameter<int>(0), traitDefinition.GetParameter<FixedPoint>(1) / 100.0 * constructionParametersMultiplier);
				list.Add(item24);
			}
			else if (traitIdentifier.Contains(HealthDmg_ToLower))
			{
				CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierExtraHealthDmgMultiplier" }, traitDefinition.DisplayName);
			}
			else if (traitIdentifier.Contains(Perseverance_ToLower))
			{
				FixedPoint fixedPoint5 = traitDefinition.GetParameter<FixedPoint>(0);
				if (chance.HasValue)
				{
					fixedPoint5 = chance.Value;
				}
				PerseveranceTrait item25 = new PerseveranceTrait((int)(fixedPoint5 * constructionParametersMultiplier));
				list.Add(item25);
			}
			else if (traitIdentifier.Contains(EquipmentPerseverance_ToLower))
			{
				FixedPoint fixedPoint6 = traitDefinition.GetParameter<FixedPoint>(0);
				if (chance.HasValue)
				{
					fixedPoint6 = chance.Value;
				}
				PerseveranceTrait item26 = new PerseveranceTrait((int)(fixedPoint6 * constructionParametersMultiplier));
				list.Add(item26);
			}
			else if (traitIdentifier.Contains(EquipmentActiveCripple_ToLower))
			{
				int parameter7 = traitDefinition.GetParameter<int>(0);
				FixedPoint fixedPoint7 = traitDefinition.GetParameter<FixedPoint>(1);
				if (chance.HasValue)
				{
					fixedPoint7 = chance.Value;
				}
				CrippleTrait item27 = new CrippleTrait(parameter7, (int)(fixedPoint7 * constructionParametersMultiplier), workOnlyOnBodyShots: false);
				list.Add(item27);
			}
			else if (traitIdentifier.Contains(EquipmentActiveOverflow_ToLower))
			{
				FixedPoint chance2 = traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier;
				FixedPoint overflowHealthChance = traitDefinition.GetParameter<FixedPoint>(1) / 100.0 * constructionParametersMultiplier;
				AbilityModifierOverflow item28 = new AbilityModifierOverflow(chance2, overflowHealthChance);
				list.Add(item28);
			}
			else if (traitIdentifier.Contains(EquipmentActiveSpecialStun_ToLower))
			{
				FixedPoint makeStunPercentage = traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier;
				FixedPoint makeStunMaxPercentage = traitDefinition.GetParameter<FixedPoint>(1) / 100.0 * constructionParametersMultiplier;
				SpecialStunTrait item29 = new SpecialStunTrait(makeStunPercentage, makeStunMaxPercentage);
				list.Add(item29);
			}
			else if (traitIdentifier.Contains(EquipmentActiveSpecialStunActiveFlag_ToLower))
			{
				SpecialStunActiveTrait item30 = new SpecialStunActiveTrait();
				list.Add(item30);
			}
			else if (!traitIdentifier.Contains(SpecialStunTargetActiveFlag_ToLower))
			{
				if (traitIdentifier.Contains(Vigilance_ToLower))
				{
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierVigilanceDamageMultiplier" }, traitDefinition.DisplayName);
				}
				else if (traitIdentifier.Contains(ArcUpgrade_ToLower))
				{
					AbilityModifierArcUpgrade item31 = new AbilityModifierArcUpgrade("AbilityModifierThreatArcUpgrade", traitDefinition.GetParameter<FixedPoint>(0) * constructionParametersMultiplier);
					list.Add(item31);
				}
				else if (traitIdentifier.Contains(Accurate_ToLower))
				{
					FixedPoint fixedPoint8 = traitDefinition.GetParameter<FixedPoint>(0);
					if (chance.HasValue)
					{
						fixedPoint8 = chance.Value;
					}
					AbilityModifierIncreaseCriticalChance item32 = new AbilityModifierIncreaseCriticalChance(fixedPoint8 / 100.0 * constructionParametersMultiplier);
					list.Add(item32);
				}
				else if (traitIdentifier.Contains(Destructive_ToLower))
				{
					AbilityModifierIncreaseCriticalMultiplier item33 = new AbilityModifierIncreaseCriticalMultiplier(traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier);
					list.Add(item33);
				}
				else if (traitIdentifier.Contains(WideArc_ToLower) || traitIdentifier.Contains(WideSpread_ToLower))
				{
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) * constructionParametersMultiplier }, new string[1] { "AbilityModifierIncreaseConeAngle" }, traitDefinition.DisplayName);
				}
				else if (traitIdentifier.Contains(HighPowered_ToLower))
				{
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) * constructionParametersMultiplier }, new string[1] { "AbilityModifierIncreaseRange" }, traitDefinition.DisplayName);
				}
				else if (traitIdentifier.Contains(LargeCaliber_ToLower))
				{
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierIncreaseBulletWidth" }, traitDefinition.DisplayName);
				}
				else if (traitIdentifier.Contains(Concussion_ToLower))
				{
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierIncreaseChanceStunTurns" }, traitDefinition.DisplayName);
				}
				else if (traitIdentifier.Contains(Inspiration_ToLower))
				{
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierIncreaseExtraAPChance" }, traitDefinition.DisplayName);
				}
				else if (traitIdentifier.Contains(TutorialSetDamage_ToLower))
				{
					AbilityModifierTutorialSetDamage item34 = new AbilityModifierTutorialSetDamage((int)((constructionParametersMultiplier - 1.0) * 100L));
					list.Add(item34);
				}
				else if (traitIdentifier == FollowThrough_ToLower)
				{
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierIncreaseFollowThroughChance" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(1) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierExtraAttackDamageModifier" }, traitDefinition.DisplayName);
				}
				else if (traitIdentifier.Contains(EquipmentFollowThrough_ToLower))
				{
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierIncreaseEquipFollowThroughChance" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(1) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierNewExtraAttackDamageModifier" }, traitDefinition.DisplayName);
				}
				else if (traitIdentifier.Contains(EquipmentActiveFollowThrough_ToLower))
				{
					list.Add(new FollowThroughEquipmentTrait(traitDefinition.GetParameter<FixedPoint>(0) * 0.009999999776482582 * constructionParametersMultiplier));
				}
				else if (traitIdentifier == CriticalAim_ToLower)
				{
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierIncreaseCriticalAimChance" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(1) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierIncreaseCriticalAimChanceCriticalHit" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<int>(2) * constructionParametersMultiplier }, new string[1] { "AbilityModifierIncreaseCriticalAimStunTurnsModifier" }, traitDefinition.DisplayName);
				}
				else if (traitIdentifier.Contains(EquipmentCriticalAim_ToLower))
				{
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierIncreaseEquipCriticalAimChance" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(1) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierIncreaseEquipCriticalAimChanceCriticalHit" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<int>(2) * constructionParametersMultiplier }, new string[1] { "AbilityModifierIncreaseEquipCriticalAimStunTurnsModifier" }, traitDefinition.DisplayName);
				}
				else if (traitIdentifier == Stagger_ToLower)
				{
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(1) / 100.0 * constructionParametersMultiplier }, new string[1] { "StaggerChance" }, traitDefinition.DisplayName);
					int parameter8 = traitDefinition.GetParameter<int>(0);
					FixedPoint fixedPoint9 = traitDefinition.GetParameter<FixedPoint>(2) / 100.0 * constructionParametersMultiplier;
					StaggerTrait item35 = new StaggerTrait(parameter8, fixedPoint9);
					list.Add(item35);
					list.Add(new AbilityModifierMultiplier("StaggerActiveChargeChance", fixedPoint9));
				}
				else if (traitIdentifier.Contains(EquipmentStagger_ToLower) || traitIdentifier.Contains(EquipmentFollowStatusStagger_ToLower))
				{
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(1) / 100.0 * constructionParametersMultiplier }, new string[1] { "StaggerChance" }, traitDefinition.DisplayName);
					int parameter9 = traitDefinition.GetParameter<int>(0);
					FixedPoint fixedPoint10 = traitDefinition.GetParameter<FixedPoint>(2) / 100.0 * constructionParametersMultiplier;
					StaggerTrait item36 = new StaggerTrait(parameter9, fixedPoint10);
					list.Add(item36);
					list.Add(new AbilityModifierMultiplier("StaggerActiveChargeChance", fixedPoint10));
				}
				else if (traitIdentifier == StaggerActive_ToLower)
				{
					list.Add(new AbilityModifierMultiplier("StaggerActiveChargeChance", constructionParametersMultiplier * 100L));
				}
				else if (traitIdentifier.Contains(RemoteRepulse_ToLower))
				{
					RemoteRepulseTrait item37 = new RemoteRepulseTrait(traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier, traitDefinition.GetParameter<int>(1), traitDefinition.GetParameter<FixedPoint>(2) / 100.0 * constructionParametersMultiplier, traitDefinition.GetParameter<int>(3));
					list.Add(item37);
				}
				else if (traitIdentifier.Contains(Equipment_Passive_Range_Repulse_1_ToLower))
				{
					RangeRepulse1Trait item38 = new RangeRepulse1Trait(traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier, traitDefinition.GetParameter<int>(1));
					list.Add(item38);
				}
				else if (traitIdentifier.Contains(Equipment_Passive_Range_Repulse_2_ToLower))
				{
					RangeRepulse2Trait item39 = new RangeRepulse2Trait(traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier, traitDefinition.GetParameter<int>(1));
					list.Add(item39);
				}
				else if (traitIdentifier.Contains(ElectronCharge_ToLower))
				{
					ElectronChargeTrait item40 = new ElectronChargeTrait(traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier, traitDefinition.GetParameter<int>(1), traitDefinition.GetParameter<FixedPoint>(2) / 100.0 * constructionParametersMultiplier, traitDefinition.GetParameter<int>(3), traitDefinition.GetParameter<int>(4), traitDefinition.GetParameter<int>(5));
					list.Add(item40);
				}
				else if (traitIdentifier.Contains(Conductive_ToLower))
				{
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierConductiveAdditionalDamagePercentage" }, traitDefinition.DisplayName);
				}
				else if (traitIdentifier.Contains(CurrentSurge_ToLower))
				{
					CurrentSurgeTrait item41 = new CurrentSurgeTrait(traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier);
					list.Add(item41);
				}
				else if (traitIdentifier.Contains(VoltCharge_ToLower))
				{
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierVoltChargeAdditionalDamagePercentage" }, traitDefinition.DisplayName);
				}
				else if (traitIdentifier.Contains(VoltShock_ToLower))
				{
					VoltShockTrait item42 = new VoltShockTrait(traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier, traitDefinition.GetParameter<int>(1), traitDefinition.GetParameter<int>(2));
					list.Add(item42);
				}
				else if (traitIdentifier.Contains(Quantun_ToLower))
				{
					QuantunTrait item43 = new QuantunTrait(traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier, traitDefinition.GetParameter<int>(1), traitDefinition.GetParameter<FixedPoint>(2) / 100.0 * constructionParametersMultiplier, traitDefinition.GetParameter<FixedPoint>(3) / 100.0 * constructionParametersMultiplier, traitDefinition.GetParameter<int>(4), traitDefinition.GetParameter<FixedPoint>(5) / 100.0 * constructionParametersMultiplier);
					list.Add(item43);
				}
				else if (traitIdentifier.Contains(ResurgenceType1_ToLower))
				{
					ResurgenceType1Trait item44 = new ResurgenceType1Trait(traitDefinition.GetParameter<int>(0));
					list.Add(item44);
				}
				else if (traitIdentifier.Contains(ResurgenceType2_ToLower))
				{
					ResurgenceType2Trait item45 = new ResurgenceType2Trait(traitDefinition.GetParameter<int>(0));
					list.Add(item45);
				}
				else if (traitIdentifier.Contains(FirstAid_ToLower))
				{
					FirstAidTrait item46 = new FirstAidTrait(traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier, traitDefinition.GetParameter<int>(1));
					list.Add(item46);
				}
				else if (traitIdentifier.Contains(RandomStatus_ToLower) && !traitIdentifier.Contains(Skinned_ToLower) && !traitIdentifier.Contains(Poison_ToLower) && !traitIdentifier.Contains(Stagger_ToLower))
				{
					List<KeyValuePair<string, FixedPoint>> triggerWeightList = new List<KeyValuePair<string, FixedPoint>>
					{
						new KeyValuePair<string, FixedPoint>("Equipment.FollowStatus.Skinned", traitDefinition.GetParameter<FixedPoint>(2)),
						new KeyValuePair<string, FixedPoint>("Equipment.FollowStatus.Stagger", traitDefinition.GetParameter<FixedPoint>(3)),
						new KeyValuePair<string, FixedPoint>("Equipment.FollowStatus.Poison", traitDefinition.GetParameter<FixedPoint>(4))
					};
					RandomStatusTrait item47 = new RandomStatusTrait(traitDefinition.GetParameter<int>(0), traitDefinition.GetParameter<FixedPoint>(1) / 100.0 * constructionParametersMultiplier, triggerWeightList);
					list.Add(item47);
				}
				else if (traitIdentifier.Contains(AddDamageNormalAttack_ToLower))
				{
					AddDamageNormalAttackTrait item48 = new AddDamageNormalAttackTrait(traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier, traitDefinition.GetParameter<FixedPoint>(1) / 100.0 * constructionParametersMultiplier, traitDefinition.GetParameter<FixedPoint>(2) / 100.0 * constructionParametersMultiplier);
					list.Add(item48);
				}
				else if (traitIdentifier.Contains(AddDamageAddAttack_ToLower))
				{
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierAddDamageAddAttackMinHPPercentage" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(1) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierAddDamageAddAttackMaxHPPercentage" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(2) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierAddDamageAddAttackExtraDamagePercentage" }, traitDefinition.DisplayName);
				}
				else if (traitIdentifier.Contains(RangeArmorDominance_ToLower))
				{
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) }, new string[1] { "AbilityModifierArmorAttackingMoreNFrames" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(2) }, new string[1] { "AbilityModifierArmorIncreaseNFrame" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(4) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierArmorDamageBoostLimit" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[2]
					{
						traitDefinition.GetParameter<FixedPoint>(1) / 100.0 * constructionParametersMultiplier,
						traitDefinition.GetParameter<FixedPoint>(3) / 100.0 * constructionParametersMultiplier
					}, new string[2] { "AbilityModifierArmorIncreaseInDamage", "AbilityModifierArmorDamageBoost" }, traitDefinition.DisplayName);
				}
				else if (traitIdentifier.Contains(EquipmentActiveBloodMark_ToLower))
				{
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) }, new string[1] { "EquipmentActiveBloodMarkDesc" }, traitDefinition.DisplayName);
					list.Add(new EquipmentActiveBloodMarkTrait(traitDefinition.GetParameter<int>(0)));
				}
				else if (traitIdentifier.Contains(EquipmentPassiveBloodMark_ToLower))
				{
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) }, new string[1] { "AbilityModifierEquipmentPassiveBloodMarkMoveDistance" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(1) }, new string[1] { "AbilityModifierEquipmentPassiveBloodMarkDamageCount" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(2) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierEquipmentPassiveBloodMarkHealthPercentageNonBoss" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(3) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierEquipmentPassiveBloodMarkHealthPercentageBoss" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(4) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierEquipmentPassiveBloodMarkChance" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(5) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierEquipmentPassiveBloodMarkDamagePercentage" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(6) }, new string[1] { "AbilityModifierEquipmentPassiveBloodMarkRange" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(7) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierEquipmentPassiveBloodMarkDamageLimit" }, traitDefinition.DisplayName);
					list.Add(new EquipmentPassiveBloodMarkTrait(traitDefinition.GetParameter<int>(0), traitDefinition.GetParameter<int>(1), traitDefinition.GetParameter<FixedPoint>(2) / 100.0 * constructionParametersMultiplier, traitDefinition.GetParameter<FixedPoint>(3) / 100.0 * constructionParametersMultiplier, traitDefinition.GetParameter<FixedPoint>(4) / 100.0 * constructionParametersMultiplier, traitDefinition.GetParameter<FixedPoint>(5) / 100.0 * constructionParametersMultiplier, traitDefinition.GetParameter<int>(6), traitDefinition.GetParameter<FixedPoint>(7) / 100.0 * constructionParametersMultiplier));
				}
				else if (traitIdentifier.Contains(EquipmentPassiveRemoveNegative_ToLower))
				{
					list.Add(new EquipmentPassiveRemoveNegativeTrait(traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier, traitDefinition.GetParameter<int>(1), traitDefinition.GetParameter<int>(2), traitDefinition.GetParameter<FixedPoint>(3) / 100.0 * constructionParametersMultiplier, traitDefinition.GetParameter<int>(4), traitDefinition.GetParameter<int>(5), traitDefinition.GetParameter<FixedPoint>(6) / 100.0 * constructionParametersMultiplier, traitDefinition.GetParameter<int>(7), traitDefinition.GetParameter<int>(8), traitDefinition.EffectIndex));
				}
				else if (traitIdentifier.Contains(EquipmentPassivePreventControl_ToLower))
				{
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierEquipmentPassivePreventControlChance" }, traitDefinition.DisplayName);
					list.Add(new EquipmentPassivePreventControlTrait());
				}
				else if (traitIdentifier.Contains(EquipmentPassiveMaxGetHitDamage_ToLower))
				{
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierPercentageIncreaseResistance" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(1) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierEquipmentPassiveMaxGetHitDamageNormalCap" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(2) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierEquipmentPassiveMaxGetHitDamageBossCap" }, traitDefinition.DisplayName);
					list.Add(new EquipmentPassiveMaxGetHitDamageTrait());
				}
				else if (traitIdentifier.Contains(EquipmentPassiveDamageAreaBlock_ToLower))
				{
					CreateAndAddIncrementerModifier(list, new FixedPoint[4]
					{
						traitDefinition.GetParameter<FixedPoint>(0) * constructionParametersMultiplier,
						traitDefinition.GetParameter<FixedPoint>(1) * constructionParametersMultiplier,
						traitDefinition.GetParameter<FixedPoint>(2) * constructionParametersMultiplier,
						traitDefinition.GetParameter<FixedPoint>(3) * constructionParametersMultiplier
					}, new string[4] { "AbilityModifierEquipmentPassiveDamageAreaBlockNormalRadiusReduction", "AbilityModifierEquipmentPassiveDamageAreaBlockBossRadiusReduction", "AbilityModifierEquipmentPassiveDamageAreaBlockNormalMinimumRadius", "AbilityModifierEquipmentPassiveDamageAreaBlockBossMinimumRadius" }, traitDefinition.DisplayName);
				}
				else if (traitIdentifier.Contains(EquipmentPassiveLineSeparatedPlus_ToLower))
				{
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) * constructionParametersMultiplier }, new string[1] { "AbilityModifierLineSeparatedMiddleRangePlus" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(1) * constructionParametersMultiplier }, new string[1] { "AbilityModifierLineSeparatedSideRangePlus" }, traitDefinition.DisplayName);
				}
				else if (traitIdentifier.Contains(RangeActorDominance_ToLower))
				{
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) }, new string[1] { "AbilityModifierActorAttackingMoreNFrames" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(2) }, new string[1] { "AbilityModifierActorIncreaseNFrame" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(4) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierActorDamageBoostLimit" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[2]
					{
						traitDefinition.GetParameter<FixedPoint>(1) / 100.0 * constructionParametersMultiplier,
						traitDefinition.GetParameter<FixedPoint>(3) / 100.0 * constructionParametersMultiplier
					}, new string[2] { "AbilityModifierActorIncreaseInDamage", "AbilityModifierActorDamageBoost" }, traitDefinition.DisplayName);
				}
				else if (traitIdentifier.Contains(AddDamageChargeAttack_ToLower))
				{
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierAddDamageChargeAttackMinHPPercentage" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(1) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierAddDamageChargeAttackMaxHPPercentage" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(2) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierAddDamageChargeAttackExtraDamagePercentage" }, traitDefinition.DisplayName);
				}
				else if (traitIdentifier.Contains(FreeChargePoint_ToLower))
				{
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "FreeChargePointNonConsumeChargePointPercentage" }, traitDefinition.DisplayName);
				}
				else if (traitIdentifier.Contains(BoostHitRate_ToLower))
				{
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierBoostHitRatePercentage" }, traitDefinition.DisplayName);
				}
				else if (traitIdentifier.Contains(IgnoreDefense_ToLower))
				{
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierIgnoreDefensePercentage" }, traitDefinition.DisplayName);
				}
				else if (traitIdentifier == LeaderBuffGoodEnough_ToLower)
				{
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { constructionParametersMultiplier }, new string[1] { "LeaderBuffGoodEnough" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) * constructionParametersMultiplier }, new string[1] { "LeaderBuffGoodEnoughCrippleChance" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(1) / 100.0 * constructionParametersMultiplier }, new string[1] { "LeaderBuffGoodEnoughStaggerChance" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(2) / 100.0 * constructionParametersMultiplier }, new string[1] { "LeaderBuffGoodEnoughStaggerChargeChance" }, traitDefinition.DisplayName);
					int parameter10 = traitDefinition.GetParameter<int>(3);
					FixedPoint addChargeChance = traitDefinition.GetParameter<FixedPoint>(2) / 100.0 * constructionParametersMultiplier;
					StaggerTrait item49 = new StaggerTrait(parameter10, addChargeChance);
					list.Add(item49);
				}
				else if (traitIdentifier.Contains(HealthBoost_ToLower))
				{
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "Health" }, traitDefinition.DisplayName);
				}
				else if (traitIdentifier.Contains(EquipmentActiveLight_ToLower))
				{
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) }, new string[1] { "AbilityModifierLightMovementSpeedIsIncreasedBySpaces" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(1) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierLightChanceNotToBeOverwatchedByWalkers" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(2) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierLightChanceNotToBeHumanEnemies" }, traitDefinition.DisplayName);
				}
				else if (traitIdentifier.Contains(MultiAttacks_ToLower))
				{
					MultiAttacksTrait item50 = new MultiAttacksTrait(traitDefinition.GetParameter<int>(0), traitDefinition.GetParameter<int>(1), traitDefinition.GetParameter<int>(2), traitDefinition.GetParameter<FixedPoint>(3) * constructionParametersMultiplier, traitDefinition.GetParameter<FixedPoint>(4) * constructionParametersMultiplier);
					list.Add(item50);
				}
				else if (traitIdentifier.Contains(MultiAttackExtraDamageActive_ToLower))
				{
					list.Add(new AbilityModifierMultiplier("AbilityModifierMultiAttackExtraDamageMultiplier", constructionParametersMultiplier));
				}
				else if (traitIdentifier.Contains(EquipmentActiveShieldBreakerStrikeType1_ToLower))
				{
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierShieldBreakerStrikeType1Parameter0" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(1) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierShieldBreakerStrikeType1Parameter1" }, traitDefinition.DisplayName);
					ShieldBreakerStrikeType1Trait item51 = new ShieldBreakerStrikeType1Trait();
					list.Add(item51);
				}
				else if (traitIdentifier.Contains(EquipmentActiveShieldBreakerStrikeType2_ToLower))
				{
					ShieldBreakerStrikeTrait item52 = new ShieldBreakerStrikeTrait(traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier, traitDefinition.GetParameter<int>(1));
					list.Add(item52);
				}
				else if (traitIdentifier.Contains(FreeRun_ToLower))
				{
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) }, new string[1] { "AbilityModifierLightMovementSpeedIsIncreasedBySpaces" }, traitDefinition.DisplayName);
				}
				else if (traitIdentifier.Contains(EquipmentActiveGroupdmgboost_ToLower))
				{
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) }, new string[1] { "AbilityModifierGroupdmgboostNumberofEnemiesAttacked" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(1) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierGroupdmgboostprobability" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(2) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierGroupdmgboostAdditionalweapondamage" }, traitDefinition.DisplayName);
				}
				else if (traitIdentifier.Contains(EquipmentApocalypticDMGScout_ToLower))
				{
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) }, new string[1] { "AbilityModifierDMGScoutAttackingAHighRanking" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(1) }, new string[1] { "AbilityModifierDMGScoutLevelDifference" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(2) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierDMGScoutIncreaseDamage" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(3) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierDMGScoutMaximumLiftingValue" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(4) }, new string[1] { "AbilityModifierDMGScoutMaxLeveLimitValue" }, traitDefinition.DisplayName);
				}
				else if (traitIdentifier.Contains(EquipmentActiveHPNailgun_ToLower))
				{
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierAttackDamageEnhancement" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(1) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierExtrAtorsoAttackDamageBoost" }, traitDefinition.DisplayName);
				}
				else if (traitIdentifier.Contains(EquipmentApocalypticDMGBruiser_ToLower))
				{
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) }, new string[1] { "AbilityModifierDMGBruiserAttackingAHighRanking" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(1) }, new string[1] { "AbilityModifierDMGBruiserLevelDifference" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(2) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierDMGBruiserIncreaseDamage" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(3) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierDMGBruiserMaximumLiftingValue" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(4) }, new string[1] { "AbilityModifierDMGBruiserMaxLeveLimitValue" }, traitDefinition.DisplayName);
				}
				else if (traitIdentifier.Contains(EquipmentApocalypticDMGWarrior_ToLower))
				{
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) }, new string[1] { "AbilityModifierDMGWarriorAttackingAHighRanking" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(1) }, new string[1] { "AbilityModifierDMGWarriorLevelDifference" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(2) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierDMGWarriorIncreaseDamage" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(3) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierDMGWarriorMaximumLiftingValue" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(4) }, new string[1] { "AbilityModifierDMGWarriorMaxLeveLimitValue" }, traitDefinition.DisplayName);
				}
				else if (traitIdentifier.Contains(EquipmentApocalypticDMGShooter_ToLower))
				{
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) }, new string[1] { "AbilityModifierDMGShooterAttackingAHighRanking" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(1) }, new string[1] { "AbilityModifierDMGShooterLevelDifference" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(2) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierDMGShooterIncreaseDamage" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(3) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierDMGShooterMaximumLiftingValue" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(4) }, new string[1] { "AbilityModifierDMGShooterMaxLeveLimitValue" }, traitDefinition.DisplayName);
				}
				else if (traitIdentifier.Contains(EquipmentApocalypticDMGHunter_ToLower))
				{
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) }, new string[1] { "AbilityModifierDMGHunterAttackingAHighRanking" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(1) }, new string[1] { "AbilityModifierDMGHunterLevelDifference" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(2) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierDMGHunterIncreaseDamage" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(3) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierDMGHunterMaximumLiftingValue" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(4) }, new string[1] { "AbilityModifierDMGHunterMaxLeveLimitValue" }, traitDefinition.DisplayName);
				}
				else if (traitIdentifier.Contains(EquipmentApocalypticDMGAssault_ToLower))
				{
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) }, new string[1] { "AbilityModifierDMGAssaultAttackingAHighRanking" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(1) }, new string[1] { "AbilityModifierDMGAssaultLevelDifference" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(2) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierDMGAssaultIncreaseDamage" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(3) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierDMGAssaultMaximumLiftingValue" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(4) }, new string[1] { "AbilityModifierDMGAssaultMaxLeveLimitValue" }, traitDefinition.DisplayName);
				}
				else if (traitIdentifier.Contains(EquipmentApocalypticBSScout_ToLower))
				{
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) }, new string[1] { "AbilityModifierBSScoutAttackingAHighRanking" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(1) }, new string[1] { "AbilityModifierBSScoutLevelDifference" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(2) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierBSScoutProbabilityReduction" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(3) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierBSScoutMaximumLiftingValue" }, traitDefinition.DisplayName);
				}
				else if (traitIdentifier.Contains(EquipmentApocalypticBSBruiser_ToLower))
				{
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) }, new string[1] { "AbilityModifierBSBruiserAttackingAHighRanking" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(1) }, new string[1] { "AbilityModifierBSBruiserLevelDifference" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(2) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierBSBruiserProbabilityReduction" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(3) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierBSBruiserMaximumLiftingValue" }, traitDefinition.DisplayName);
				}
				else if (traitIdentifier.Contains(EquipmentApocalypticBSWarrior_ToLower))
				{
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) }, new string[1] { "AbilityModifierBSWarriorAttackingAHighRanking" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(1) }, new string[1] { "AbilityModifierBSWarriorLevelDifference" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(2) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierBSWarriorProbabilityReduction" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(3) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierBSWarriorMaximumLiftingValue" }, traitDefinition.DisplayName);
				}
				else if (traitIdentifier.Contains(EquipmentApocalypticBSShooter_ToLower))
				{
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) }, new string[1] { "AbilityModifierBSShooterAttackingAHighRanking" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(1) }, new string[1] { "AbilityModifierBSShooterLevelDifference" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(2) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierBSShooterProbabilityReduction" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(3) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierBSShooterMaximumLiftingValue" }, traitDefinition.DisplayName);
				}
				else if (traitIdentifier.Contains(EquipmentApocalypticBSHunter_ToLower))
				{
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) }, new string[1] { "AbilityModifierBSHunterAttackingAHighRanking" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(1) }, new string[1] { "AbilityModifierBSHunterLevelDifference" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(2) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierBSHunterProbabilityReduction" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(3) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierBSHunterMaximumLiftingValue" }, traitDefinition.DisplayName);
				}
				else if (traitIdentifier.Contains(EquipmentApocalypticBSAssault_ToLower))
				{
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) }, new string[1] { "AbilityModifierBSAssaultAttackingAHighRanking" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(1) }, new string[1] { "AbilityModifierBSAssaultLevelDifference" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(2) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierBSAssaultProbabilityReduction" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(3) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierBSAssaultMaximumLiftingValue" }, traitDefinition.DisplayName);
				}
				else if (traitIdentifier.Contains(EquipmentApocalypticDEFScout_ToLower))
				{
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) }, new string[1] { "AbilityModifierDEFScoutAttackedByHighLevel" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(1) }, new string[1] { "AbilityModifierDEFScoutLevelDifference" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(2) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierDEFScoutDamageReduction" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(3) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierDEFScoutMaximumLiftingValue" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(4) }, new string[1] { "AbilityModifierDEFScoutMaxLeveLimitValue" }, traitDefinition.DisplayName);
				}
				else if (traitIdentifier.Contains(EquipmentApocalypticDEFBruiser_ToLower))
				{
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) }, new string[1] { "AbilityModifierDEFBruiserAttackedByHighLevel" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(1) }, new string[1] { "AbilityModifierDEFBruiserLevelDifference" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(2) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierDEFBruiserDamageReduction" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(3) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierDEFBruiserMaximumLiftingValue" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(4) }, new string[1] { "AbilityModifierDEFBruiserMaxLeveLimitValue" }, traitDefinition.DisplayName);
				}
				else if (traitIdentifier.Contains(EquipmentApocalypticDEFWarrior_ToLower))
				{
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) }, new string[1] { "AbilityModifierDEFWarriorAttackedByHighLevel" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(1) }, new string[1] { "AbilityModifierDEFWarriorLevelDifference" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(2) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierDEFWarriorDamageReduction" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(3) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierDEFWarriorMaximumLiftingValue" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(4) }, new string[1] { "AbilityModifierDEFWarriorMaxLeveLimitValue" }, traitDefinition.DisplayName);
				}
				else if (traitIdentifier.Contains(EquipmentApocalypticDEFShooter_ToLower))
				{
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) }, new string[1] { "AbilityModifierDEFShooterAttackedByHighLevel" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(1) }, new string[1] { "AbilityModifierDEFShooterLevelDifference" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(2) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierDEFShooterDamageReduction" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(3) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierDEFShooterMaximumLiftingValue" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(4) }, new string[1] { "AbilityModifierDEFShooterMaxLeveLimitValue" }, traitDefinition.DisplayName);
				}
				else if (traitIdentifier.Contains(EquipmentApocalypticDEFHunter_ToLower))
				{
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) }, new string[1] { "AbilityModifierDEFHunterAttackedByHighLevel" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(1) }, new string[1] { "AbilityModifierDEFHunterLevelDifference" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(2) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierDEFHunterDamageReduction" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(3) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierDEFHunterMaximumLiftingValue" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(4) }, new string[1] { "AbilityModifierDEFHunterMaxLeveLimitValue" }, traitDefinition.DisplayName);
				}
				else if (traitIdentifier.Contains(EquipmentApocalypticDEFAssault_ToLower))
				{
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) }, new string[1] { "AbilityModifierDEFAssaultAttackedByHighLevel" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(1) }, new string[1] { "AbilityModifierDEFAssaultLevelDifference" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(2) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierDEFAssaultDamageReduction" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(3) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierDEFAssaultMaximumLiftingValue" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(4) }, new string[1] { "AbilityModifierDEFAssaultMaxLeveLimitValue" }, traitDefinition.DisplayName);
				}
				else if (traitIdentifier.Contains(EquipmentActiveKing_ToLower))
				{
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierEquipmentActiveKingSuperpositionNumber" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(1) }, new string[1] { "AbilityModifierEquipmentActiveKingMaxSuperpositionNumber" }, traitDefinition.DisplayName);
				}
				else if (traitIdentifier.Contains(EquipmentActiveSuppress1_ToLower))
				{
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) }, new string[1] { "AbilityModifierEquipmentActiveSuppress1CheckNumber" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(1) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierEquipmentActiveSuppress1BloodRestriction" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(2) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierEquipmentActiveSuppress1DamageBonus" }, traitDefinition.DisplayName);
				}
				else if (traitIdentifier.Contains(EquipmentActiveSuppress2_ToLower))
				{
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) }, new string[1] { "AbilityModifierEquipmentActiveSuppress2CheckNumber" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(1) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierEquipmentActiveSuppress2BloodRestriction" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(2) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierEquipmentActiveSuppress2DamageBonus" }, traitDefinition.DisplayName);
				}
				else if (traitIdentifier.Contains(EquipmentActiveInterruptor_ToLower))
				{
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) * 0.009999999776482582 * constructionParametersMultiplier }, new string[1] { "Equipment_Active_Interruptor" }, traitDefinition.DisplayName);
				}
				else if (traitIdentifier.Contains(Interruptor_ToLower))
				{
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierIncreaseInterruptChance" }, traitDefinition.DisplayName);
				}
				else if (traitIdentifier.Contains(Protective_ToLower))
				{
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) * constructionParametersMultiplier }, new string[1] { "PercentageIncreaseOverwatchDamage" }, traitDefinition.DisplayName);
				}
				else if (traitIdentifier.Contains(EquipmentActiveFocusMode_ToLower))
				{
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<int>(0) * constructionParametersMultiplier }, new string[1] { "AbilityModifierFocusModeAttackDistance" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(1) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierFocusModeAttackWidth" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(3) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierFocusModeDamageIncrease" }, traitDefinition.DisplayName);
					FixedPoint focusModeCoolOff = traitDefinition.GetParameter<int>(2) * constructionParametersMultiplier;
					FocusModeTrait item53 = new FocusModeTrait(traitDefinition.GetParameter<int>(4) * constructionParametersMultiplier, focusModeCoolOff);
					list.Add(item53);
				}
				else if (traitIdentifier.Contains(EquipmentActiveBreakthrough_ToLower))
				{
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierLBreakthroughMath" }, traitDefinition.DisplayName);
				}
				else if (traitIdentifier.Contains(EquipmentBreakthrough_ToLower))
				{
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierLEquipBreakthroughMath" }, traitDefinition.DisplayName);
				}
				else if (traitIdentifier.Contains(EquipmentKaboom_ToLower))
				{
					EquipmentKaboomTrait item54 = new EquipmentKaboomTrait(traitDefinition.GetParameter<FixedPoint>(0) * 0.009999999776482582 * constructionParametersMultiplier, traitDefinition.GetParameter<FixedPoint>(1) * constructionParametersMultiplier, traitDefinition.GetParameter<FixedPoint>(2) * constructionParametersMultiplier, traitDefinition.GetParameter<FixedPoint>(3) * constructionParametersMultiplier, traitDefinition.GetParameter<FixedPoint>(4) * constructionParametersMultiplier, traitDefinition.GetParameter<FixedPoint>(5) * constructionParametersMultiplier);
					list.Add(item54);
				}
				else if (traitIdentifier.Contains(EquipmentProtective_ToLower))
				{
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) * constructionParametersMultiplier }, new string[1] { "PercentageIncreaseNewOverwatchDamage" }, traitDefinition.DisplayName);
				}
				else if (traitIdentifier.Contains(Charging_ToLower))
				{
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierIncreaseExtraChargePointChance" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(1) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierIncreaseChargeAbilityDamage" }, traitDefinition.DisplayName);
				}
				else if (traitIdentifier.Contains(Ruthless_ToLower))
				{
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierIncreaseChargeAbilityDamage" }, traitDefinition.DisplayName);
				}
				else if (traitIdentifier.Contains(Burning_ToLower))
				{
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<int>(2) * constructionParametersMultiplier }, new string[1] { "AbilityModifierIncreaseStruggleTurns" }, traitDefinition.DisplayName);
				}
				else if (traitIdentifier.Contains(PvP_HumanVsHumanDamageResistance_ToLower))
				{
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierPercentageIncreaseResistanceHumanVsHuman" }, traitDefinition.DisplayName);
				}
				else if (traitIdentifier.Contains(PVP_SurvivorVsRaiderDamageResistance_ToLower))
				{
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierPercentageIncreaseResistanceSurvivorVsRaider" }, traitDefinition.DisplayName);
				}
				else if (traitIdentifier.Contains(PVP_RaiderVsSurvivorDamageResistance_ToLower))
				{
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierPercentageIncreaseResistanceRaiderVsSurvivor" }, traitDefinition.DisplayName);
				}
				else if (traitIdentifier.Contains(Silenced_ToLower))
				{
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilitySilencedWeaponChance" }, traitDefinition.DisplayName);
				}
				else if (traitIdentifier.Contains(LeaderBuffNoThreatRanged_ToLower))
				{
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierIncreaseNoThreatChanceRanged" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(1) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierPercentageIncreaseRangedCriticalChance" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[3]
					{
						traitDefinition.GetParameter<FixedPoint>(2) / 100.0 * constructionParametersMultiplier,
						traitDefinition.GetParameter<FixedPoint>(3),
						traitDefinition.GetParameter<FixedPoint>(4) / 100.0 * constructionParametersMultiplier
					}, new string[3] { "LeaderBuffNoThreatRangedPercentageIncreaseChargePoint", "LeaderBuffNoThreatRangedIncreaseChargePoint", "LeaderBuffNoThreatRangedCriticalIncreaseDamage" }, traitDefinition.DisplayName);
				}
				else if (traitIdentifier.Contains(ThreatReduction_ToLower))
				{
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityThreatReductionChance" }, traitDefinition.DisplayName);
				}
				else if (traitIdentifier.Contains(ThreatFree_ToLower))
				{
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityThreatFreeChance" }, traitDefinition.DisplayName);
				}
				else if (traitIdentifier.Contains(LeaderBuffReduceThreatMelee_ToLower))
				{
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierIncreaseReduceThreatChanceMelee" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(1) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierIncreaseExtraAPChanceForMelee" }, traitDefinition.DisplayName);
				}
				else if (traitIdentifier.Contains(LeaderBuffDontTouchMyAllies_ToLower))
				{
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierPercentageIncreaseTargetDamageNextToAlly" }, traitDefinition.DisplayName);
				}
				else if (traitIdentifier.Contains(LeaderBuffCriticalChance_ToLower))
				{
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 }, new string[1] { "AbilityModifierPercentageIncreaseCriticalChance" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(1) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierCarolCriticalChance" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(2) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierCarolCriticalDamage" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<int>(3) * constructionParametersMultiplier }, new string[1] { "AbilityModifierCarolNoAttackTurn" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(4) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierCarolCannotAttackedChance" }, traitDefinition.DisplayName);
					FistSpikeRangeTrait item55 = new FistSpikeRangeTrait(traitDefinition.GetParameter<int>(6), traitDefinition.GetParameter<FixedPoint>(5) / 100.0, traitDefinition.GetParameter<int>(7));
					list.Add(item55);
				}
				else if (traitIdentifier.Contains(LeaderBuffCriticalResistance_ToLower))
				{
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierPercentageIncreaseResistanceCriticalDamageFromHumans" }, traitDefinition.DisplayName);
				}
				else if (traitIdentifier.Contains(LeaderBuffExtraChargePointAtAttackDmgTaken_ToLower))
				{
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierIncreaseExtraChargePointAtAttackDmgChance" }, traitDefinition.DisplayName);
				}
				else if (traitIdentifier == LeaderBuffDeadlyTactics_ToLower)
				{
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "LeaderBuffDeadlyTactics" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(1) }, new string[1] { "AbilityModifierIncreaseMoveRangeForSecondMoveDeadlyTactics" }, traitDefinition.DisplayName);
				}
				else if (!traitIdentifier.Contains(GuildBattleBuff_ToLower) && traitIdentifier.Contains(Piercing_ToLower))
				{
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { -traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierDecreaseBodyshotChance" }, traitDefinition.DisplayName);
				}
				else if (traitIdentifier.Contains(Razor_ToLower))
				{
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { -traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierDecreaseBodyshotMeleeChance" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { -traitDefinition.GetParameter<FixedPoint>(1) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierPercentageDecreaseResistance" }, traitDefinition.DisplayName);
				}
				else if (traitIdentifier.Contains(EquipmentIncendiary_ToLower))
				{
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierIncreaseChanceToSetTargetOnFire" }, traitDefinition.DisplayName);
				}
				else if (traitIdentifier == LeaderBuffSecondChance_ToLower)
				{
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierIncreaseSecondChanceChance" }, traitDefinition.DisplayName);
				}
				else if (traitIdentifier == SecondChance_ToLower)
				{
					AbilityModifierSecondChance item56 = new AbilityModifierSecondChance();
					list.Add(item56);
				}
				else if (traitIdentifier.Contains(LeaderBuffHealingCharge_ToLower))
				{
					FixedPoint fixedPoint11 = traitDefinition.GetParameter<FixedPoint>(0);
					if (chance.HasValue)
					{
						fixedPoint11 = chance.Value;
					}
					FixedPoint parameter11 = traitDefinition.GetParameter<FixedPoint>(1);
					AbilityModifierIncreaseHealingAtChargeUsage item57 = new AbilityModifierIncreaseHealingAtChargeUsage(fixedPoint11 / 100L, parameter11 / 100L);
					list.Add(item57);
				}
				else if (traitIdentifier.Contains(LeaderBuffSurvivalInstinct_ToLower))
				{
					FixedPoint parameter12 = traitDefinition.GetParameter<FixedPoint>(0);
					AbilityModifierSurvivalInstinct item58 = new AbilityModifierSurvivalInstinct(percentLessDamageTakenIn: traitDefinition.GetParameter<FixedPoint>(1) / 100L, percentMoreDamageDoneIn: parameter12 / 100L);
					list.Add(item58);
				}
				else if (traitIdentifier.Contains(Bulletproof_ToLower))
				{
					FixedPoint fixedPoint12 = traitDefinition.GetParameter<FixedPoint>(0);
					if (chance.HasValue)
					{
						fixedPoint12 = chance.Value;
					}
					FixedPoint parameter13 = traitDefinition.GetParameter<FixedPoint>(1);
					AbilityModifierIncreaseBodyShot item59 = new AbilityModifierIncreaseBodyShot(fixedPoint12 / 100L, parameter13 / 100L);
					list.Add(item59);
				}
				else if (traitIdentifier.Contains(PointBlankShot_ToLower))
				{
					AbilityModifierPointBlankShot item60 = new AbilityModifierPointBlankShot(traitDefinition.GetParameter<FixedPoint>(1) / 100L);
					list.Add(item60);
				}
				else if (traitIdentifier.Contains(BoostTotalHealth_ToLower))
				{
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierPercentageMultiplyHealthAll" }, traitDefinition.DisplayName);
				}
				else if (traitIdentifier.Contains(LeaderBuffBringThemOn_ToLower))
				{
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierIncreaseExtraChargePointChanceAtThreatWave" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(1) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierPercentageMultiplyHealthAll" }, traitDefinition.DisplayName);
				}
				else if (traitIdentifier.Contains(LeaderBuffGoodOutOfBad_ToLower))
				{
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierIncreaseExtraChargePointChanceAfterBodyShot" }, traitDefinition.DisplayName);
				}
				else if (traitIdentifier == LeaderBuffHunterDesperation_ToLower)
				{
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierIncreaseDamageOnSpecial" }, traitDefinition.DisplayName);
				}
				else if (traitIdentifier == Retaliate_ToLower)
				{
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierIncreaseRetaliateDamage" }, traitDefinition.DisplayName);
				}
				else if (traitIdentifier.Contains(EquipmentRetaliate_ToLower))
				{
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierIncreaseEquipmentRetaliateDamage" }, traitDefinition.DisplayName);
				}
				else if (traitIdentifier == LeaderBuffRegalAuthority_ToLower)
				{
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "LeaderBuffPercentageIncreasePreEmptiveStrikeDamage" }, traitDefinition.DisplayName);
				}
				else if (traitIdentifier == BaseRegalAuthority_ToLower)
				{
					PreEmptiveStrikeTrait item61 = new PreEmptiveStrikeTrait();
					list.Add(item61);
				}
				else if (traitIdentifier == LeaderBuffMulletTime_ToLower)
				{
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { -traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierIncreaseCriticalChanceResistance" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(1) / 100.0 * constructionParametersMultiplier }, new string[1] { AbilityModifierIncreaseStunAvoidChance.StunAvoidChance }, traitDefinition.DisplayName);
				}
				else if (traitIdentifier == LeaderBuffMysteriousWays_ToLower)
				{
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierIncreaseMeleeDodgeChance" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(1) / 100.0 * constructionParametersMultiplier }, new string[1] { "ExtendProbability" }, traitDefinition.DisplayName);
				}
				else if (traitIdentifier == LeaderBuffTeamwork_ToLower)
				{
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierPercentageIncreaseSameTargetDamage" }, traitDefinition.DisplayName);
				}
				else if (traitIdentifier == LeaderBuffOnlyTheBest_ToLower)
				{
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierIncreaseHigherLevelEquipmentDropChance" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierIncreaseAmountSuppliesDropChance" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierIncreaseAmountXpDropChance" }, traitDefinition.DisplayName);
				}
				else if (traitIdentifier == LeaderBuffJackass_ToLower)
				{
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierPercentageIncreaseTargetHigherLevelDamage" }, traitDefinition.DisplayName);
				}
				else if (traitIdentifier == EquipmentStunResistance_ToLower)
				{
					FixedPoint fixedPoint13 = traitDefinition.GetParameter<FixedPoint>(0);
					if (chance.HasValue)
					{
						fixedPoint13 = chance.Value;
					}
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { fixedPoint13 / 100.0 * constructionParametersMultiplier }, new string[1] { AbilityModifierIncreaseStunAvoidChance.StunAvoidChance }, traitDefinition.DisplayName);
				}
				else if (traitIdentifier.Contains(LeaderBuffReadyForAction_ToLower))
				{
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifieAddChargePointAtStart" }, traitDefinition.DisplayName);
				}
				else if (traitIdentifier.Contains(LeaderBuffForestStalker_ToLower))
				{
					list.Add(new LeaderBuffForestStalkerTrait(traitDefinition.GetParameter<FixedPoint>(0)));
					if (constructionParametersMultiplier > 0.0)
					{
						CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(1) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierChangeNotTriggerOverwatch" }, traitDefinition.DisplayName);
					}
				}
				else if (traitIdentifier.Contains(LeaderBuffJustice_ToLower))
				{
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierIncreaseExtraAPChanceSpecialEnemies" }, traitDefinition.DisplayName);
				}
				else if (traitIdentifier.Contains(LeaderBuffColdBlooded_ToLower))
				{
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { -traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierDecreaseBodyshotChanceColdBlooded" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(1) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierPercentageIncreaseTargetHigherLevelCritChance" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(2) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierExtraMoveChance" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(3) }, new string[1] { "AbilityModifierIncreaseMoveRangeForSecondMoveColdBlooded" }, traitDefinition.DisplayName);
				}
				else if (traitIdentifier.Contains(LeaderBuffOneWithTheHerdStalker_ToLower))
				{
					FixedPoint parameter14 = traitDefinition.GetParameter<FixedPoint>(0);
					list.Add(new LeaderBuffOneWithTheHerdStalkerTrait(parameter14));
				}
				else if (traitIdentifier.Contains(LeaderBuffOneWithTheHerd_ToLower))
				{
					list.Add(new LeaderBuffOneWithTheHerdTrait());
					list.Add(new AbilityModifierGainChargePointWithHerd());
					if (constructionParametersMultiplier > 0.0)
					{
						CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "LeaderBuffModifierGainChargePointAtTaunt" }, traitDefinition.DisplayName);
						CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(1) / 100.0 * constructionParametersMultiplier }, new string[1] { "LeaderBuffGainExtraChargePointAtTauntIncreaseChance" }, traitDefinition.DisplayName);
					}
				}
				else if (traitIdentifier.Contains(LeaderBuffExplosiveBullets_ToLower))
				{
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierExplosiveBulletDamageScaleOnTargetHealth" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(1) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierExplosiveBulletStunChance" }, traitDefinition.DisplayName);
				}
				else if (traitIdentifier.Contains(LeaderBuffBeatEmUp_ToLower))
				{
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "LeaderBuffBeatEmUpPunishMultiplier" }, traitDefinition.DisplayName);
				}
				else if (traitIdentifier.Contains(BaseBeatEmUp_ToLower))
				{
					BeatEmUpTrait item62 = new BeatEmUpTrait();
					list.Add(item62);
				}
				else if (traitIdentifier.Contains(LeaderBuffFightingFury_ToLower))
				{
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) }, new string[1] { "LeaderBuffFightingFuryMaxAddAttacks" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(1) }, new string[1] { "LeaderBuffFightingFuryMaxAddAttacksLeader" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(2) / 100.0 * constructionParametersMultiplier }, new string[1] { "LeaderBuffFightingFuryDamageModifier" }, traitDefinition.DisplayName);
				}
				else if (traitIdentifier.Contains(BaseFightingFury_ToLower) || traitIdentifier.Contains(FightingFury_ToLower))
				{
					list.Add(new FightingFuryTrait());
				}
				else if (traitIdentifier.Contains(LeaderBuffBetterTogether_ToLower))
				{
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) }, new string[1] { "LeaderBuffBetterTogetherSurvivorDistance" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(1) * 0.009999999776482582 * constructionParametersMultiplier }, new string[1] { "LeaderBuffBetterTogetherExtraChargePointChance" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(2) * 0.009999999776482582 * constructionParametersMultiplier }, new string[1] { "LeaderBuffBetterTogetherAdditionalDamageModifier" }, traitDefinition.DisplayName);
				}
				else if (traitIdentifier.Contains(BaseBetterTogether_ToLower))
				{
					BetterTogetherTrait item63 = new BetterTogetherTrait();
					list.Add(item63);
				}
				else if (traitIdentifier.Contains(LeaderBuffInspire_ToLower))
				{
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "LeaderBuffInspireDamageIncreasePerKillPercentage" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(1) / 100.0 * constructionParametersMultiplier }, new string[1] { "LeaderBuffInspireIncreaseExtraChargePointChance" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(2) / 100.0 * constructionParametersMultiplier }, new string[1] { "LeaderBuffInspireMaxDamageIncreasePerKillPercentage" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(3) / 100.0 * constructionParametersMultiplier }, new string[1] { "LeaderBuffInspireMaxExtraChargePointChance" }, traitDefinition.DisplayName);
				}
				else if (traitIdentifier.Contains(LeaderBuffPrincess_ToLower))
				{
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "LeaderBuffPrincess.ExtraDamage" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(1) / 100.0 * constructionParametersMultiplier }, new string[1] { "LeaderBuffPrincess.ExtraChargePoints" }, traitDefinition.DisplayName);
				}
				else if (traitIdentifier.Contains(LeaderBuffMarkEnemy_ToLower))
				{
					MarkEnemyTrait item64 = new MarkEnemyTrait(traitDefinition.GetParameter<int>(0), traitDefinition.GetParameter<int>(1));
					list.Add(item64);
				}
				else if (traitIdentifier.Contains(DebuffMarkEnemy_ToLower))
				{
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "LeaderBuffMarkEnemy.ExtraDamage" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { -traitDefinition.GetParameter<FixedPoint>(1) / 100.0 * constructionParametersMultiplier }, new string[1] { "LeaderBuffMarkEnemy.DamageReduction" }, traitDefinition.DisplayName);
				}
				else if (traitIdentifier.Contains(DebuffEquipmentKaboom_ToLower))
				{
					DebuffEquipmentKaboomTrait item65 = new DebuffEquipmentKaboomTrait(traitDefinition.GetParameter<FixedPoint>(0), traitDefinition.GetParameter<FixedPoint>(1), traitDefinition.GetParameter<FixedPoint>(2), traitDefinition.GetParameter<FixedPoint>(3));
					list.Add(item65);
				}
				else if (traitIdentifier.Contains(EquipmentActiveSkinned_ToLower) || traitIdentifier.Contains(EquipmentFollowStatusSkinned_ToLower))
				{
					SkinnedTrait item66 = new SkinnedTrait(traitDefinition.GetParameter<FixedPoint>(0) / 100.0, traitDefinition.GetParameter<int>(1));
					list.Add(item66);
				}
				else if (traitIdentifier.Contains(Skinned_ToLower))
				{
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { -traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "SkinnedDebuffMarkReduceAttackPowerPercent" }, traitDefinition.DisplayName);
				}
				else if (traitIdentifier.Contains(EquipmentActiveRipped_ToLower))
				{
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierRippedAdditionalDmgPercent" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(1) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierRippedAdditionalDmgRatio" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(2) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierRippedAdditionalDmgMaxRatio" }, traitDefinition.DisplayName);
					RippedTrait item67 = new RippedTrait(traitDefinition.GetParameter<FixedPoint>(3) / 100.0, traitDefinition.GetParameter<int>(4));
					list.Add(item67);
				}
				else if (traitIdentifier.Contains(EquipmentActiveAssistAttack_ToLower))
				{
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "EquipmentActiveAssistAttackPercent" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(1) * constructionParametersMultiplier }, new string[1] { "EquipmentActiveAssistAttackDamagePercent" }, traitDefinition.DisplayName);
					list.Add(new AssistAttackTrait());
				}
				else if (traitIdentifier.Contains(EquipmentActiveAssistAttackActive_ToLower))
				{
					list.Add(new AbilityModifierMultiplier("EquipmentActiveAssistAttackActiveMultiplier", constructionParametersMultiplier));
				}
				else if (traitIdentifier.Contains(EquipmentActiveChargeLoad_ToLower))
				{
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "EquipmentActiveChargeLoadBumpPercent" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(1) / 100.0 * constructionParametersMultiplier }, new string[1] { "EquipmentActiveChargeLoadBumpDmgRatio" }, traitDefinition.DisplayName);
					CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(2) * constructionParametersMultiplier }, new string[1] { "EquipmentActiveChargeLoadBumpMaxFloor" }, traitDefinition.DisplayName);
				}
				else if (traitIdentifier.Contains(LeaderBuffFiringSquad_ToLower))
				{
					list.Add(new AbilityModifierMultiplier("LeaderBuffFiringSquad", traitDefinition.GetParameter<FixedPoint>(0) * constructionParametersMultiplier));
				}
				else if (traitIdentifier.Contains(FiringSquadMember_ToLower))
				{
					list.Add(new FiringSquadMemberTrait());
				}
				else if (!traitIdentifier.Contains(FiringSquadLeader_ToLower))
				{
					if (traitIdentifier.Contains(FiringSquadDamageActive_ToLower))
					{
						list.Add(new AbilityModifierMultiplier("FiringSquadDamageActiveMultiplier", constructionParametersMultiplier));
					}
					else if (traitIdentifier.Contains(LeaderBuffEmitter_ToLower))
					{
						list.Add(new AbilityModifierMultiplier("LeaderBuffEmitterDamageMultiplier", traitDefinition.GetParameter<FixedPoint>(0) * constructionParametersMultiplier));
						CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(1) }, new string[1] { "LeaderBuffEmitterDuration" }, traitDefinition.DisplayName);
						CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(2) }, new string[1] { "LeaderBuffEmitterRadius" }, traitDefinition.DisplayName);
						CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(3) }, new string[1] { "LeaderBuffEmitterMaxMergedRadius" }, traitDefinition.DisplayName);
					}
					else if (traitIdentifier.Contains(EmitterCreator_ToLower))
					{
						list.Add(new EmitterCreatorTrait());
					}
					else if (traitIdentifier.Contains(EmitterDamageActive_ToLower))
					{
						list.Add(new AbilityModifierMultiplier("EmitterDamageActiveMultiplier", constructionParametersMultiplier));
					}
					else if (traitIdentifier.Contains(LeaderBuffHeadshot_ToLower))
					{
						CreateAndAddIncrementerModifier(list, new FixedPoint[2]
						{
							traitDefinition.GetParameter<FixedPoint>(0) * 0.009999999776482582 * constructionParametersMultiplier,
							traitDefinition.GetParameter<FixedPoint>(2) * 0.009999999776482582 * constructionParametersMultiplier
						}, new string[2] { "LeaderBuffHeadshotCurrentHealthDamageChance", "LeaderBuffHeadshotStatusAvoidChance" }, traitDefinition.DisplayName);
						list.Add(new AbilityModifierMultiplier("LeaderBuffHeadshotCurrentHealthDamageMultiplierRanged", traitDefinition.GetParameter<FixedPoint>(1) * 0.009999999776482582 * constructionParametersMultiplier));
						list.Add(new AbilityModifierMultiplier("LeaderBuffHeadshotCurrentHealthDamageMultiplierMelee", traitDefinition.GetParameter<FixedPoint>(3) * 0.009999999776482582 * constructionParametersMultiplier));
					}
					else if (traitIdentifier.Contains(BaseHeadshot_ToLower))
					{
						list.Add(new HeadshotTrait());
					}
					else if (traitIdentifier.Contains(LeaderBuffCoupDeGrace_ToLower))
					{
						CreateAndAddIncrementerModifier(list, new FixedPoint[2]
						{
							traitDefinition.GetParameter<FixedPoint>(0) * 0.009999999776482582 * constructionParametersMultiplier,
							traitDefinition.GetParameter<FixedPoint>(2) * 0.009999999776482582 * constructionParametersMultiplier
						}, new string[2] { "LeaderBuffCoupDeGraceFollowUpProbability", "LeaderBuffCoupDeGraceChargeProbability" }, traitDefinition.DisplayName);
						list.Add(new AbilityModifierMultiplier("LeaderBuffCoupDeGraceFollowUpDamage", traitDefinition.GetParameter<FixedPoint>(1) * 0.009999999776482582 * constructionParametersMultiplier));
					}
					else if (traitIdentifier.Contains(BaseCoupDeGrace_ToLower))
					{
						list.Add(new CoupDeGraceTrait());
					}
					else if (!traitIdentifier.Contains(CoupDeGraceActive_ToLower))
					{
						if (traitIdentifier.Contains(LeaderBuffMadeToSuffer_ToLower))
						{
							CreateAndAddIncrementerModifier(list, new FixedPoint[5]
							{
								traitDefinition.GetParameter<FixedPoint>(0) * 0.009999999776482582 * constructionParametersMultiplier,
								traitDefinition.GetParameter<FixedPoint>(1),
								traitDefinition.GetParameter<FixedPoint>(2),
								traitDefinition.GetParameter<FixedPoint>(3),
								traitDefinition.GetParameter<FixedPoint>(4)
							}, new string[5] { "LeaderBuffMadeToSufferDotRatio", "LeaderBuffMadeToSufferMaxAreasLeader", "LeaderBuffMadeToSufferMaxAreasNonLeader", "LeaderBuffMadeToSufferMaxAreasDuration", "LeaderBuffMadeToSufferRadius" }, traitDefinition.DisplayName);
						}
						else if (traitIdentifier.Contains(SufferCreator_ToLower))
						{
							list.Add(new SufferCreatorTrait());
						}
						else if (!traitIdentifier.Contains("SufferActive"))
						{
							if (traitIdentifier.Contains(LeaderBuffUnleashedFighter_ToLower))
							{
								CreateAndAddIncrementerModifier(list, new FixedPoint[7]
								{
									traitDefinition.GetParameter<FixedPoint>(0),
									traitDefinition.GetParameter<FixedPoint>(1),
									traitDefinition.GetParameter<FixedPoint>(2) * 0.009999999776482582 * constructionParametersMultiplier,
									traitDefinition.GetParameter<FixedPoint>(3),
									traitDefinition.GetParameter<FixedPoint>(4),
									traitDefinition.GetParameter<FixedPoint>(5),
									1L
								}, new string[7] { "LeaderBuffUnleashedFighterAreaGridLength", "LeaderBuffUnleashedFighterAreasDurationLeader", "LeaderBuffUnleashedFighterExtraDamageLeader", "LeaderBuffUnleashedFighterCoolingPeriodLeader", "LeaderBuffUnleashedFighterCoolingPeriodShare", "LeaderBuffUnleashedMaxAreas", "LeaderBuffUnleashedFighterRemoteAreaGridLength" }, traitDefinition.DisplayName);
							}
							else if (traitIdentifier.Contains(BaseUnleashedFighter_ToLower))
							{
								list.Add(new UnleashedCreatorTrait());
							}
							else if (traitIdentifier.Contains(UnleashedActive_ToLower))
							{
								list.Add(new UnleashedActiveTrait());
							}
							else if (traitIdentifier.Contains(TrapFlame_ToLower))
							{
								list.Add(new TrapFlameTrait(traitDefinition.GetParameter<FixedPoint>(0) * 0.009999999776482582 * constructionParametersMultiplier, traitDefinition.GetParameter<FixedPoint>(1), traitDefinition.GetParameter<FixedPoint>(2), traitDefinition.GetParameter<FixedPoint>(3), traitDefinition.GetParameter<FixedPoint>(4) * 0.009999999776482582 * constructionParametersMultiplier));
							}
							else if (traitIdentifier.Contains(AttackChain_ToLower))
							{
								AttackChainTrait item68 = new AttackChainTrait(traitDefinition.GetParameter<int>(0), traitDefinition.GetParameter<int>(1), traitDefinition.GetParameter<FixedPoint>(2) * 0.009999999776482582 * constructionParametersMultiplier, traitDefinition.GetParameter<FixedPoint>(3) * 0.009999999776482582 * constructionParametersMultiplier);
								list.Add(item68);
							}
							else if (traitIdentifier.Contains(Asthenia_ToLower))
							{
								CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierDamagerActorUpDamagePercentage" }, traitDefinition.DisplayName);
								CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(1) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierDamagerActorDamageReducePercentage" }, traitDefinition.DisplayName);
								AstheniaTrait item69 = new AstheniaTrait(traitDefinition.GetParameter<FixedPoint>(2) / 100.0 * constructionParametersMultiplier, traitDefinition.GetParameter<FixedPoint>(3) / 100.0 * constructionParametersMultiplier, traitDefinition.GetParameter<FixedPoint>(4) / 100.0 * constructionParametersMultiplier, traitDefinition.GetParameter<int>(5));
								list.Add(item69);
							}
							else if (traitIdentifier.Contains(GrenadeFragmentDamage_ToLower))
							{
								GrenadeFragmentDamageTrait item70 = new GrenadeFragmentDamageTrait(traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier, traitDefinition.GetParameter<FixedPoint>(1) / 100.0 * constructionParametersMultiplier, traitDefinition.GetParameter<FixedPoint>(2) / 100.0 * constructionParametersMultiplier);
								list.Add(item70);
							}
							else if (traitIdentifier.Contains(InspirePerKillIncreaseDamageModifierTrait_ToLower))
							{
								CreateAndAddIncrementerModifier(list, new FixedPoint[1] { constructionParametersMultiplier }, new string[1] { "AbilityModifierLeaderBuffInspireDamageIncrease" }, traitDefinition.DisplayName);
							}
							else if (traitIdentifier.Contains(InspirePerKillIncreaseExtraChargePointChanceModifierTrait_ToLower))
							{
								CreateAndAddIncrementerModifier(list, new FixedPoint[1] { constructionParametersMultiplier }, new string[1] { "AbilityModifierLeaderBuffInspireExtraChargePointChance" }, traitDefinition.DisplayName);
							}
							else if (traitIdentifier == StunResistance_ToLower)
							{
								AbilityModifierAvoidStun item71 = new AbilityModifierAvoidStun();
								list.Add(item71);
							}
							else if (traitIdentifier == Revenge_ToLower)
							{
								CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierIncreaseRevengeDamage" }, traitDefinition.DisplayName);
							}
							else if (traitIdentifier.Contains(EquipmentRevenge_ToLower))
							{
								CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierIncreaseEquipRevengeDamage" }, traitDefinition.DisplayName);
							}
							else if (traitIdentifier.Contains(ShieldRevenge_ToLower))
							{
								ShieldRevengeTrait item72 = new ShieldRevengeTrait(traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier, traitDefinition.GetParameter<FixedPoint>(1) / 100.0 * constructionParametersMultiplier, traitDefinition.GetParameter<int>(2));
								list.Add(item72);
							}
							else if (traitIdentifier == Punish_ToLower)
							{
								CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierIncreasePunishDamage" }, traitDefinition.DisplayName);
							}
							else if (traitIdentifier.Contains(EquipmentPunish_ToLower))
							{
								CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierIncreaseNewPunishDamage" }, traitDefinition.DisplayName);
							}
							else if (traitIdentifier.Contains(FlatBaseDamage_ToLower))
							{
								CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) * constructionParametersMultiplier }, new string[1] { "AbilityModifierIncreaseBaseDamageFlat" }, traitDefinition.DisplayName);
							}
							else if (traitIdentifier.Contains(FlatHealth_ToLower))
							{
								CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) * constructionParametersMultiplier }, new string[1] { "AbilityModifierIncreaseBaseHealthFlat" }, traitDefinition.DisplayName);
							}
							else if (traitIdentifier.Contains(FlatCritDamage_ToLower))
							{
								CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) * constructionParametersMultiplier }, new string[1] { "AbilityModifierIncreaseCritDamageFlat" }, traitDefinition.DisplayName);
							}
							else if (traitIdentifier == EquipmentSniperHarness_ToLower)
							{
								CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "PercentageIncreaseRangeDamageInCover" }, traitDefinition.DisplayName);
								CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(1) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierIncreaseCoverDamageReduction" }, traitDefinition.DisplayName);
							}
							else if (traitIdentifier == EquipmentTrainingGear_ToLower)
							{
								CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 }, new string[1] { "AbilityModifierPercentageMultiplyKillSP" }, traitDefinition.DisplayName);
							}
							else if (traitIdentifier.Contains(DamageReductionBonus_ToLower))
							{
								CreateAndAddIncrementerClassModifier(list, traitDefinition.GetParameter<FixedPoint>(0) / 100.0, traitDefinition.GetParameter<SurvivorClass>(1), "GuildBattleAbilityModifierDamageReduction", traitDefinition.DisplayName);
							}
							else if (traitIdentifier.Contains(DamageBonus_ToLower))
							{
								CreateAndAddIncrementerClassModifier(list, traitDefinition.GetParameter<FixedPoint>(0) / 100.0, traitDefinition.GetParameter<SurvivorClass>(1), "GuildBattleAbilityModifierDamage", traitDefinition.DisplayName);
							}
							else if (traitIdentifier.Contains(BodyShotReductionBonus_ToLower))
							{
								CreateAndAddIncrementerClassModifier(list, -traitDefinition.GetParameter<FixedPoint>(0) / 100.0, traitDefinition.GetParameter<SurvivorClass>(1), "GuildBattleAbilityModifierBodyShotReduction", traitDefinition.DisplayName);
							}
							else if (traitIdentifier.Contains(CriticalChanceBonus_ToLower))
							{
								CreateAndAddIncrementerClassModifier(list, traitDefinition.GetParameter<FixedPoint>(0) / 100.0, traitDefinition.GetParameter<SurvivorClass>(1), "GuildBattleAbilityModifierCriticalChance", traitDefinition.DisplayName);
							}
							else if (traitIdentifier.Contains(FullChargeChanceBonus_ToLower))
							{
								CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 }, new string[1] { "GuildBattleAbilityModifierFullChargeChance" }, traitDefinition.DisplayName);
							}
							else if (traitIdentifier.Contains(DodgeChanceBonus_ToLower))
							{
								CreateAndAddIncrementerClassModifier(list, traitDefinition.GetParameter<FixedPoint>(0) / 100.0, traitDefinition.GetParameter<SurvivorClass>(1), "GuildBattleAbilityModifierDodgeChance", traitDefinition.DisplayName);
							}
							else if (traitIdentifier.Contains(EquipmentActiveExtraDamageExecution_ToLower))
							{
								CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) }, new string[1] { "Equipment_Active_ExtraDamageExecution" }, traitDefinition.DisplayName);
							}
							else if (traitIdentifier.Contains(EquipmentActiveCriticalPenetratesArmor_ToLower))
							{
								list.Add(new AbilityModifierMultiplier("AbilityModifierPercentageMaxReduceOnCritical", traitDefinition.GetParameter<FixedPoint>(0) * 0.009999999776482582 * constructionParametersMultiplier));
							}
							else if (traitIdentifier.Contains("TacticalResupply".ToLower()))
							{
								TacticalResupplyTrait item73 = new TacticalResupplyTrait(traitDefinition.GetParameter<FixedPoint>(0) * 0.009999999776482582 * constructionParametersMultiplier, traitDefinition.GetParameter<int>(1), traitDefinition.GetParameter<int>(2), traitDefinition.GetParameter<int>(3), traitDefinition.GetParameter<int>(4));
								list.Add(item73);
							}
							else if (traitIdentifier.Contains(EquipmentTactical_ToLower))
							{
								CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(1) }, new string[1] { "AbilityModifierIncreaseMoveRangeForSecondMove" }, traitDefinition.DisplayName);
								CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierPercentageIncreaseResistanceMelee" }, traitDefinition.DisplayName);
							}
							else if (traitIdentifier.Contains(EquipmentArmorTactical_ToLower))
							{
								CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(1) }, new string[1] { "AbilityModifierIncreaseMoveRangeForSecondMoveTacticalArmor" }, traitDefinition.DisplayName);
								CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierPercentageIncreaseResistanceMeleeArmor" }, traitDefinition.DisplayName);
							}
							else if (traitIdentifier.Contains(PreventPush_ToLower))
							{
								CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 }, new string[1] { "AbilityModifierPreventPushPercentage" }, traitDefinition.DisplayName);
							}
							else if (traitIdentifier.Contains(PreventIncendiary_ToLower))
							{
								CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 }, new string[1] { "AbilityModifierPreventIncendiaryPercentage" }, traitDefinition.DisplayName);
							}
							else if (traitIdentifier.Contains(EquipmentShield_ToLower) || traitIdentifier.Contains(EquipmentPassiveShield_ToLower))
							{
								CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierIncreaseShieldHitPointsPercentage" }, traitDefinition.DisplayName);
							}
							else if (traitIdentifier.Contains(CommonwealthArmorTrait_ToLower))
							{
								list.Add(new CommonwealthArmorTrait());
							}
							else if (!traitIdentifier.Contains(CommonwealthArmorActive_ToLower))
							{
								if (traitIdentifier.Contains(PastaSupportTrait_ToLower))
								{
									list.Add(new PastaSupportTrait());
								}
								else if (!traitIdentifier.Contains(PastaSupportActive_ToLower))
								{
									if (traitIdentifier.Contains(TemFullyStateTrait_ToLower))
									{
										list.Add(new FullyStateTrait());
									}
									else if (traitIdentifier.Contains(Riposte_ToLower))
									{
										CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(4) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierRippedAdditionalPRIncreaseDmg" }, traitDefinition.DisplayName);
										CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<int>(5) }, new string[1] { "AbilityModifierRippedAdditionalPRMaxStorey" }, traitDefinition.DisplayName);
										RiposteTrait item74 = new RiposteTrait(traitDefinition.GetParameter<FixedPoint>(0) * constructionParametersMultiplier, traitDefinition.GetParameter<int>(1), traitDefinition.GetParameter<int>(2), traitDefinition.GetParameter<int>(3), traitDefinition.GetParameter<FixedPoint>(4) / 100.0 * constructionParametersMultiplier, traitDefinition.GetParameter<int>(5), traitDefinition.GetParameter<FixedPoint>(6) / 100.0 * constructionParametersMultiplier, traitDefinition.GetParameter<FixedPoint>(7) / 100.0 * constructionParametersMultiplier, traitDefinition.GetParameter<FixedPoint>(8) / 100.0 * constructionParametersMultiplier, traitDefinition.GetParameter<int>(9), traitDefinition.GetParameter<FixedPoint>(10) / 100.0 * constructionParametersMultiplier);
										list.Add(item74);
									}
									else if (traitIdentifier.Contains(CommonwealthArmorExtraChargeChance_ToLower))
									{
										CreateAndAddIncrementerModifier(list, new FixedPoint[1] { chance ?? ((FixedPoint)0.0) }, new string[1] { "CommonwealthArmorExtraChargeChance" }, traitDefinition.DisplayName);
									}
									else if (!traitIdentifier.Contains(StruggleInvulnerable_ToLower) && !traitIdentifier.Contains(TutorialInvulnerable_ToLower) && !traitIdentifier.Contains(TutorialUninterruptable_ToLower) && !traitIdentifier.Contains(EquipmentActiveExtraAP_ToLower) && !traitIdentifier.Contains(Explosive_ToLower) && !traitIdentifier.Contains(EmptyTactical_ToLower) && !(traitIdentifier == Bleeding_ToLower) && !(traitIdentifier == Impenetrable_ToLower) && !(traitIdentifier == PushCollisionDamage_ToLower) && !traitIdentifier.Contains(Gore_ToLower))
									{
										if (traitIdentifier.Contains(InfiniteRange_ToLower))
										{
											CreateAndAddIncrementerModifier(list, new FixedPoint[1] { 50L }, new string[1] { "AbilityModifierIncreaseRange" }, traitDefinition.DisplayName);
										}
										else if (traitIdentifier.Contains(RangedDamageFalloff_ToLower))
										{
											CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) }, new string[1] { "RangedDamageFalloffRange" }, traitDefinition.DisplayName);
											list.Add(new AbilityModifierMultiplier("RangedDamageFalloffMultiplier", traitDefinition.GetParameter<FixedPoint>(1) * 0.009999999776482582 * constructionParametersMultiplier));
										}
										else if (traitIdentifier.Contains(CarolsCookiesTrait_ToLower))
										{
											list.Add(new CarolsCookiesTrait());
										}
										else if (!traitIdentifier.Contains(CarolsCookiesActive_ToLower))
										{
											if (traitIdentifier.Contains(PrimedChance_ToLower))
											{
												CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) * 0.009999999776482582 * constructionParametersMultiplier }, new string[1] { "PrimedChance" }, traitDefinition.DisplayName);
											}
											else if (!traitIdentifier.Contains(WalkerMikeActive_ToLower))
											{
												if (traitIdentifier.Contains(HealthThresholdedStatusResistance_ToLower))
												{
													int parameterCount = traitDefinition.GetParameterCount();
													KeyValuePair<FixedPoint, FixedPoint>[] array = new KeyValuePair<FixedPoint, FixedPoint>[parameterCount];
													for (int i = 0; i < parameterCount; i++)
													{
														string[] array2 = traitDefinition.GetParameter<string>(i).Split('-');
														FixedPoint key = new FixedPoint(int.Parse(array2[0])) * 0.009999999776482582;
														FixedPoint value = new FixedPoint(int.Parse(array2[1])) * 0.009999999776482582;
														array[i] = new KeyValuePair<FixedPoint, FixedPoint>(key, value);
													}
													list.Add(new HealthThresholdedStatusResistanceTrait(array));
												}
												else if (traitIdentifier.Contains(FirstStrike_ToLower))
												{
													CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) * 0.009999999776482582 * constructionParametersMultiplier }, new string[1] { "FirstStrikeAdditionalDamage" }, traitDefinition.DisplayName);
													CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(1) * 0.009999999776482582 * constructionParametersMultiplier }, new string[1] { "FirstStrikeDamageThreshold" }, traitDefinition.DisplayName);
												}
												else if (traitIdentifier.Contains(Fortified_ToLower))
												{
													CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) * -0.009999999776482582 * constructionParametersMultiplier }, new string[1] { "FortifiedCriticalReduction" }, traitDefinition.DisplayName);
												}
												else if (traitIdentifier.Contains(LeaderBuffKnockKnock_ToLower))
												{
													CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) }, new string[1] { "LeaderBuffKnockKnockTargetMaxNum" }, traitDefinition.DisplayName);
													CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(1) }, new string[1] { "LeaderBuffKnockKnockMarkMaxNum" }, traitDefinition.DisplayName);
													CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(2) * 0.009999999776482582 }, new string[1] { "LeaderBuffKnockKnockOneMarkDamageMultiplier" }, traitDefinition.DisplayName);
													CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(3) }, new string[1] { "LeaderBuffKnockKnockExtraChargePointConfig" }, traitDefinition.DisplayName);
													CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(4) * 0.009999999776482582 * constructionParametersMultiplier }, new string[1] { "LeaderBuffKnockKnockExtraChargePointChance" }, traitDefinition.DisplayName);
												}
												else if (traitIdentifier.Contains(Equipment_Passive_TornApart_ToLower))
												{
													CreateAndAddIncrementerModifier(list, new FixedPoint[2]
													{
														traitDefinition.GetParameter<FixedPoint>(0) * 0.009999999776482582 * constructionParametersMultiplier,
														traitDefinition.GetParameter<FixedPoint>(1) * 0.009999999776482582 * constructionParametersMultiplier
													}, new string[2] { "Equipment_Passive_TornDamageMultiplier", "Equipment_Passive_TornExtraDamageMultiplier" }, traitDefinition.DisplayName);
													TornApartTrait item75 = new TornApartTrait(traitDefinition.GetParameter<int>(2));
													list.Add(item75);
												}
												else if (traitIdentifier.Contains(Equipment_Passive_FreeOW_ToLower))
												{
													CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) * 0.009999999776482582 * constructionParametersMultiplier }, new string[1] { "Equipment_Passive_FreeOWChanceNotToRaider" }, traitDefinition.DisplayName);
												}
												else if (traitIdentifier.Contains(Equipment_Passive_SawAxe_ToLower))
												{
													CreateAndAddIncrementerModifier(list, new FixedPoint[6]
													{
														traitDefinition.GetParameter<FixedPoint>(0) * 0.009999999776482582 * constructionParametersMultiplier,
														traitDefinition.GetParameter<FixedPoint>(1) * 0.009999999776482582 * constructionParametersMultiplier,
														traitDefinition.GetParameter<FixedPoint>(2),
														traitDefinition.GetParameter<FixedPoint>(3) * 0.009999999776482582 * constructionParametersMultiplier,
														traitDefinition.GetParameter<FixedPoint>(4) * 0.009999999776482582 * constructionParametersMultiplier,
														traitDefinition.GetParameter<FixedPoint>(5) * 0.009999999776482582 * constructionParametersMultiplier
													}, new string[6] { "Equipment_Passive_SawAxe_CriticalChance", "Equipment_Passive_SawAxe_CriticalMultiplier", "Equipment_Passive_SawAxe_ExtraDmgCount", "Equipment_Passive_SawAxe_ExtraDmgChance", "Equipment_Passive_SawAxe_ExtraDmgMultiplier", "Equipment_Passive_SawAxe_MaxExtraDmgMultiplier" }, traitDefinition.DisplayName);
												}
												else if (traitIdentifier.Contains(Equipment_Passive_ShotGun_ToLower))
												{
													CreateAndAddIncrementerModifier(list, new FixedPoint[5]
													{
														traitDefinition.GetParameter<int>(0),
														traitDefinition.GetParameter<int>(1),
														traitDefinition.GetParameter<FixedPoint>(2) * 0.009999999776482582 * constructionParametersMultiplier,
														traitDefinition.GetParameter<FixedPoint>(3) * 0.009999999776482582 * constructionParametersMultiplier,
														traitDefinition.GetParameter<FixedPoint>(4) * 0.009999999776482582 * constructionParametersMultiplier
													}, new string[5] { "Equipment_Passive_ShotGun_Param0", "Equipment_Passive_ShotGun_Param1", "Equipment_Passive_ShotGun_Param2", "Equipment_Passive_ShotGun_Param3", "Equipment_Passive_ShotGun_Param4" }, traitDefinition.DisplayName);
												}
												else if (traitIdentifier.Contains(HealthRealdmg_ToLower))
												{
													CreateAndAddIncrementerModifier(list, new FixedPoint[1] { (traitDefinition.GetParameter<FixedPoint>(0) / 100.0 - 1.0) * constructionParametersMultiplier }, new string[1] { "AbilityModifierHealthRealdmg" }, traitDefinition.DisplayName);
													CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(1) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierHealthRealdmg_Param0" }, traitDefinition.DisplayName);
												}
												else if (traitIdentifier.Contains(BaseKnockKnock_ToLower))
												{
													KnockKnockTrait item76 = new KnockKnockTrait();
													list.Add(item76);
												}
												else if (traitIdentifier.Contains(LeaderBuffRedact_ToLower))
												{
													CreateAndAddIncrementerModifier(list, new FixedPoint[6]
													{
														traitDefinition.GetParameter<FixedPoint>(0) * 0.009999999776482582 * constructionParametersMultiplier,
														traitDefinition.GetParameter<FixedPoint>(1) * 0.009999999776482582 * constructionParametersMultiplier,
														traitDefinition.GetParameter<FixedPoint>(2),
														traitDefinition.GetParameter<FixedPoint>(3),
														traitDefinition.GetParameter<FixedPoint>(4) * 0.009999999776482582 * constructionParametersMultiplier,
														traitDefinition.GetParameter<FixedPoint>(5)
													}, new string[6] { "LeaderBuffRedactStunChance", "LeaderBuffRedactChance", "LeaderBuffRedactMaxLayers", "LeaderBuffRedactIncreaseHumanDamage", "LeaderBuffRedactReduceWalkerHpChance", "LeaderBuffRedactReduceWalkerHpRatio" }, traitDefinition.DisplayName);
													RedactTrait item77 = new RedactTrait();
													list.Add(item77);
												}
												else if (traitIdentifier.Contains(LeaderBuffProtect_ToLower))
												{
													list.Add(new AbilityModifierMultiplier("LeaderBuffProtectDamageChance", traitDefinition.GetParameter<FixedPoint>(0) * 0.009999999776482582 * constructionParametersMultiplier));
													list.Add(new AbilityModifierMultiplier("LeaderBuffProtectChargeDamageChance", traitDefinition.GetParameter<FixedPoint>(2) * 0.009999999776482582 * constructionParametersMultiplier));
													list.Add(new AbilityModifierMultiplier("LeaderBuffProtectShieldChance", traitDefinition.GetParameter<FixedPoint>(4) * 0.009999999776482582 * constructionParametersMultiplier));
													list.Add(new AbilityModifierMultiplier("LeaderBuffProtectLeaderShieldChance", traitDefinition.GetParameter<FixedPoint>(6) * 0.009999999776482582 * constructionParametersMultiplier));
													int parameter15 = traitDefinition.GetParameter<int>(1);
													int parameter16 = traitDefinition.GetParameter<int>(3);
													int parameter17 = traitDefinition.GetParameter<int>(5);
													int parameter18 = traitDefinition.GetParameter<int>(7);
													TauntTrait item78 = new TauntTrait(parameter15, parameter16, parameter17, parameter18);
													list.Add(item78);
												}
												else if (traitIdentifier.Contains(LeaderBuffClosingTime_ToLower))
												{
													CreateAndAddIncrementerModifier(list, new FixedPoint[2]
													{
														traitDefinition.GetParameter<FixedPoint>(0),
														traitDefinition.GetParameter<FixedPoint>(2) * 0.009999999776482582 * constructionParametersMultiplier
													}, new string[2] { "LeaderBuffClosingTimeRange", "LeaderBuffClosingTimeSecondaryTargetDamageChance" }, traitDefinition.DisplayName);
													list.Add(new AbilityModifierMultiplier("LeaderBuffClosingTimeMainTargetDamageChance", traitDefinition.GetParameter<FixedPoint>(1) * 0.009999999776482582 * constructionParametersMultiplier));
												}
												else if (traitIdentifier.Contains(BaseClosingTime_ToLower))
												{
													list.Add(new ClosingTimeTrait());
												}
												else if (traitIdentifier.Contains("Citadel_PursuitDown".ToLower()))
												{
													CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) * 0.009999999776482582 * constructionParametersMultiplier }, new string[1] { "Citadel_PursuitDown_LowerMultiplier" }, traitDefinition.DisplayName);
												}
												else if (traitIdentifier.Contains(Pursuit_ToLower))
												{
													CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierPursuitAP" }, traitDefinition.DisplayName);
													CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(1) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierPursuitCH" }, traitDefinition.DisplayName);
												}
												else if (traitIdentifier.Contains(EquipmentActiveAdvance_ToLower))
												{
													CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierAdvanceGainAPChance" }, traitDefinition.DisplayName);
													CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(1) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierAdvanceCriticalHitChance" }, traitDefinition.DisplayName);
												}
												else if (traitIdentifier.Contains(Repulse_ToLower))
												{
													CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierRepulseGainAPChance" }, traitDefinition.DisplayName);
													CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(1) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierRepulseCriticalHitChance" }, traitDefinition.DisplayName);
													CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(2) / 100.0 * constructionParametersMultiplier }, new string[1] { "AbilityModifierRepulseStaggerChance" }, traitDefinition.DisplayName);
													int parameter19 = traitDefinition.GetParameter<int>(3);
													FixedPoint addChargeChance2 = traitDefinition.GetParameter<FixedPoint>(4) / 100.0 * constructionParametersMultiplier;
													StaggerTrait item79 = new StaggerTrait(parameter19, addChargeChance2);
													list.Add(item79);
												}
												else if (traitIdentifier.Contains(BossHitPointDMG_ToLower))
												{
													CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) * constructionParametersMultiplier }, new string[1] { "AbilityModifierBossHitPointDMGAttackCount" }, traitDefinition.DisplayName);
													CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(1) / 100.0 * constructionParametersMultiplier }, new string[1] { "Boss.AbilityModifierBossHitPointDMGAddAdditionalDamage" }, traitDefinition.DisplayName);
												}
												else if (traitIdentifier.Contains(NegativeFatal_ToLower))
												{
													List<FixedPoint> list2 = new List<FixedPoint>();
													for (int j = 0; j < traitDefinition.ConstructionParameters.Count; j++)
													{
														FixedPoint fixedPoint14 = traitDefinition.GetParameter<FixedPoint>(j) / 100.0 * constructionParametersMultiplier;
														if (fixedPoint14 < 0L)
														{
															base.Debug.LogWarning("Could not NegativeFatal trait modifier for " + traitDefinition.GetTraitClassName());
														}
														list2.Add(fixedPoint14);
													}
													NegativeFatalTrait item80 = new NegativeFatalTrait(list2, traitDefinition.EffectIndex);
													list.Add(item80);
												}
												else if (traitIdentifier.Contains(EquipmentActiveDisoriented_ToLower))
												{
													FixedPoint fixedPoint15 = traitDefinition.GetParameter<FixedPoint>(0);
													int parameter20 = traitDefinition.GetParameter<int>(1);
													int parameter21 = traitDefinition.GetParameter<int>(2);
													CreateAndAddIncrementerModifier(list, new FixedPoint[1] { parameter21 }, new string[1] { "DisorientLeastSpaces" }, traitDefinition.DisplayName);
													if (chance.HasValue)
													{
														fixedPoint15 = chance.Value;
													}
													DisorientTrait item81 = new DisorientTrait(parameter20, (int)(fixedPoint15 * constructionParametersMultiplier));
													list.Add(item81);
												}
												else if (traitIdentifier.Contains(EquipmentActiveRecoil_ToLower))
												{
													list.Add(new RecoilTrait(traitDefinition.GetParameter<FixedPoint>(0) * 0.009999999776482582 * constructionParametersMultiplier, traitDefinition.GetParameter<FixedPoint>(1) * 0.009999999776482582 * constructionParametersMultiplier, traitDefinition.GetParameter<FixedPoint>(2) * 0.009999999776482582 * constructionParametersMultiplier));
													CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) * 0.009999999776482582 * constructionParametersMultiplier }, new string[1] { "AbilityModifierRecoilDamageReduce" }, traitDefinition.DisplayName);
													CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(1) * 0.009999999776482582 * constructionParametersMultiplier }, new string[1] { "AbilityModifierRecoilNormalStunChance" }, traitDefinition.DisplayName);
													CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(2) * 0.009999999776482582 * constructionParametersMultiplier }, new string[1] { "AbilityModifierRecoilCircleStunChance" }, traitDefinition.DisplayName);
												}
												else if (traitIdentifier.Contains(LeaderBuffABTester_ToLower))
												{
													CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) * constructionParametersMultiplier }, new string[1] { "LeaderBuffABTesterAMaxNum" }, traitDefinition.DisplayName);
													CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(1) * 0.009999999776482582 * constructionParametersMultiplier }, new string[1] { "LeaderBuffABTesterADamageMultiplier" }, traitDefinition.DisplayName);
													CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(2) * 0.009999999776482582 * constructionParametersMultiplier }, new string[1] { "LeaderBuffABTesterBMarkChance" }, traitDefinition.DisplayName);
													CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(3) * 0.009999999776482582 * constructionParametersMultiplier }, new string[1] { "LeaderBuffABTesterBAPChance" }, traitDefinition.DisplayName);
												}
												else if (traitIdentifier.Contains(BaseABTester_ToLower))
												{
													BaseABTester item82 = new BaseABTester();
													list.Add(item82);
												}
												else if (traitIdentifier.Contains(BounsPhonePortrait_ToLower))
												{
													CreateAndAddIncrementerModifier(list, new FixedPoint[4]
													{
														traitDefinition.GetParameter<FixedPoint>(0),
														traitDefinition.GetParameter<FixedPoint>(1) * 0.009999999776482582 * constructionParametersMultiplier,
														traitDefinition.GetParameter<FixedPoint>(2) * 0.009999999776482582 * constructionParametersMultiplier,
														traitDefinition.GetParameter<FixedPoint>(3)
													}, new string[4] { "BounsPhonePortraitAfterKilledTimes", "BonusPhonePortraitTargetHitPointsBelowPercent", "BounsPhonePortraitKilledTargetPercentage", "BounsPhonePortraitOnceKilledMaxTarget" }, traitDefinition.DisplayName);
												}
												else if (traitIdentifier.Contains(Heirlooms_RiotGearGlenn_Fetter_ToLower))
												{
													CreateAndAddIncrementerModifier(list, new FixedPoint[7]
													{
														traitDefinition.GetParameter<FixedPoint>(0) * 0.009999999776482582 * constructionParametersMultiplier,
														traitDefinition.GetParameter<FixedPoint>(1) * 0.009999999776482582 * constructionParametersMultiplier,
														traitDefinition.GetParameter<FixedPoint>(2),
														traitDefinition.GetParameter<FixedPoint>(3) * 0.009999999776482582 * constructionParametersMultiplier,
														traitDefinition.GetParameter<FixedPoint>(4) * 0.009999999776482582 * constructionParametersMultiplier,
														traitDefinition.GetParameter<FixedPoint>(5),
														traitDefinition.GetParameter<FixedPoint>(6) * 0.009999999776482582 * constructionParametersMultiplier
													}, new string[7] { "Heirlooms_RiotGearGlenn_Fetter_BurnDmg", "Heirlooms_RiotGearGlenn_Fetter_AtkChance", "Heirlooms_RiotGearGlenn_Fetter_AtkTimes", "Heirlooms_RiotGearGlenn_Fetter_AtkChanceStun", "Heirlooms_RiotGearGlenn_Fetter_ChargeChance", "Heirlooms_RiotGearGlenn_Fetter_ChargeTimes", "Heirlooms_RiotGearGlenn_Fetter_ChargeChanceStun" }, traitDefinition.DisplayName);
												}
												else if (traitIdentifier.Contains(Heirlooms_Hershel_Fetter_ToLower))
												{
													list.Add(new HeirloomsHershelFetterTrait(traitDefinition.GetParameter<FixedPoint>(0) * 0.009999999776482582 * constructionParametersMultiplier, traitDefinition.GetParameter<FixedPoint>(1), traitDefinition.GetParameter<FixedPoint>(2), traitDefinition.GetParameter<FixedPoint>(3), traitDefinition.GetParameter<FixedPoint>(4) * 0.009999999776482582 * constructionParametersMultiplier, traitDefinition.GetParameter<FixedPoint>(5) * 0.009999999776482582 * constructionParametersMultiplier, traitDefinition.GetParameter<FixedPoint>(6) * 0.009999999776482582 * constructionParametersMultiplier, traitDefinition.GetParameter<FixedPoint>(7), traitDefinition.GetParameter<FixedPoint>(8)));
												}
												else if (traitIdentifier.Contains(HealthBoostBouns_ToLower))
												{
													CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) * 0.009999999776482582 * constructionParametersMultiplier }, new string[1] { "AbilityModifierHealthBoostBounsHealth" }, traitDefinition.DisplayName);
												}
												else if (traitIdentifier.Contains(FlameDMGReduceBouns_ToLower))
												{
													CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) * 0.009999999776482582 * constructionParametersMultiplier }, new string[1] { "FlameDMGReduceBouns_ReduceBurn" }, traitDefinition.DisplayName);
												}
												else if (traitIdentifier.Contains(LeaderBuffNoExceptions_ToLower))
												{
													CreateAndAddIncrementerModifier(list, new FixedPoint[10]
													{
														traitDefinition.GetParameter<FixedPoint>(0) * 0.009999999776482582 * constructionParametersMultiplier,
														traitDefinition.GetParameter<FixedPoint>(1),
														traitDefinition.GetParameter<FixedPoint>(2),
														traitDefinition.GetParameter<FixedPoint>(3) * 0.009999999776482582 * constructionParametersMultiplier,
														traitDefinition.GetParameter<FixedPoint>(4),
														traitDefinition.GetParameter<FixedPoint>(5),
														traitDefinition.GetParameter<FixedPoint>(6) * 0.009999999776482582 * constructionParametersMultiplier,
														traitDefinition.GetParameter<FixedPoint>(7),
														traitDefinition.GetParameter<FixedPoint>(8) * 0.009999999776482582 * constructionParametersMultiplier,
														traitDefinition.GetParameter<FixedPoint>(9)
													}, new string[10] { "LeaderBuffNoExceptions_SetFireChance", "LeaderBuffNoExceptions_FlameTriggerRange", "LeaderBuffNoExceptions_MaxEnemy", "LeaderBuffNoExceptions_BurnDamageRatio", "LeaderBuffNoExceptions_MaxTriggerCount", "LeaderBuffNoExceptions_LeaderMaxTriggerCount", "LeaderBuffNoExceptions_BurnLayerChance", "LeaderBuffNoExceptions_BurnLayerTurn", "LeaderBuffNoExceptions_ChargePointChance", "LeaderBuffNoExceptions_MaxBurnLayer" }, traitDefinition.DisplayName);
												}
												else if (traitIdentifier.Contains(BaseNoExceptions_ToLower))
												{
													list.Add(new FlameTriggerTrait());
												}
												else if (traitIdentifier.Contains(LeaderBuffOverload_ToLower))
												{
													CreateAndAddIncrementerModifier(list, new FixedPoint[16]
													{
														traitDefinition.GetParameter<FixedPoint>(0),
														traitDefinition.GetParameter<FixedPoint>(1),
														traitDefinition.GetParameter<FixedPoint>(2) * 0.009999999776482582 * constructionParametersMultiplier,
														traitDefinition.GetParameter<FixedPoint>(3),
														traitDefinition.GetParameter<FixedPoint>(4) * 0.009999999776482582 * constructionParametersMultiplier,
														traitDefinition.GetParameter<FixedPoint>(5),
														traitDefinition.GetParameter<FixedPoint>(6) * 0.009999999776482582 * constructionParametersMultiplier,
														traitDefinition.GetParameter<FixedPoint>(7) * 0.009999999776482582 * constructionParametersMultiplier,
														traitDefinition.GetParameter<FixedPoint>(8),
														traitDefinition.GetParameter<FixedPoint>(9),
														traitDefinition.GetParameter<FixedPoint>(10) * 0.009999999776482582 * constructionParametersMultiplier,
														traitDefinition.GetParameter<FixedPoint>(11),
														traitDefinition.GetParameter<FixedPoint>(12) * 0.009999999776482582 * constructionParametersMultiplier,
														traitDefinition.GetParameter<FixedPoint>(13),
														traitDefinition.GetParameter<FixedPoint>(14) * 0.009999999776482582 * constructionParametersMultiplier,
														traitDefinition.GetParameter<FixedPoint>(15) * 0.009999999776482582 * constructionParametersMultiplier
													}, new string[16]
													{
														"LeaderBuffOverload_ChargePointNum", "LeaderBuffOverload_ChargePointLimitNum", "LeaderBuffOverload_ChargePointDmgPer", "LeaderBuffOverload_ContinueTurnNum", "LeaderBuffOverload_FullChargeEXDmgPer", "LeaderBuffOverload_FullChargeEXTurnLimitNum", "LeaderBuffOverload_AddDmgPer", "LeaderBuffOverload_LifeDmgPer", "BaseLeaderBuffOverload_ChargePointNum", "BaseLeaderBuffOverload_ChargePointLimitNum",
														"BaseLeaderBuffOverload_ChargePointDmgPer", "BaseLeaderBuffOverload_ContinueTurnNum", "BaseLeaderBuffOverload_FullChargeEXDmgPer", "BaseLeaderBuffOverload_FullChargeEXTurnLimitNum", "BaseLeaderBuffOverload_AddDmgPer", "BaseLeaderBuffOverload_LifeDmgPer"
													}, traitDefinition.DisplayName);
												}
												else if (traitIdentifier.Contains(BaseOverload_ToLower))
												{
													list.Add(new BaseOverloadTrait());
												}
												else if (traitIdentifier.Contains(OverloadEXDamageActive_ToLower))
												{
													list.Add(new AbilityModifierMultiplier("OverloadDamageActiveActiveMultiplier", constructionParametersMultiplier));
												}
												else if (traitIdentifier.Contains(Equipment_Passive_Detonation1_ToLower))
												{
													CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) * 0.009999999776482582 * constructionParametersMultiplier }, new string[1] { "Equipment_Passive_Detonation_Dmg_1" }, traitDefinition.DisplayName);
												}
												else if (traitIdentifier.Contains(Equipment_Passive_Detonation_ToLower))
												{
													CreateAndAddIncrementerModifier(list, new FixedPoint[2]
													{
														traitDefinition.GetParameter<FixedPoint>(0) * 0.009999999776482582 * constructionParametersMultiplier,
														traitDefinition.GetParameter<FixedPoint>(1) * 0.009999999776482582 * constructionParametersMultiplier
													}, new string[2] { "Equipment_Passive_Detonation_Dmg", "Equipment_Passive_DetonationProbility" }, traitDefinition.DisplayName);
												}
												else if (traitIdentifier.Contains(StrengthenDefenseFunc1_ToLower))
												{
													CreateAndAddIncrementerModifier(list, new FixedPoint[3]
													{
														traitDefinition.GetParameter<FixedPoint>(0) * 0.009999999776482582 * constructionParametersMultiplier,
														traitDefinition.GetParameter<FixedPoint>(1) * constructionParametersMultiplier,
														traitDefinition.GetParameter<FixedPoint>(2) * 0.009999999776482582 * constructionParametersMultiplier
													}, new string[3] { "StrengthenDefenseFunc1Param1", "StrengthenDefenseFunc1Param2", "StrengthenDefenseFunc1Param3" }, traitDefinition.DisplayName);
												}
												else if (traitIdentifier.Contains(StrengthenDefenseFunc2_ToLower))
												{
													CreateAndAddIncrementerModifier(list, new FixedPoint[4]
													{
														traitDefinition.GetParameter<FixedPoint>(0) * 0.009999999776482582 * constructionParametersMultiplier,
														traitDefinition.GetParameter<FixedPoint>(1) * constructionParametersMultiplier,
														traitDefinition.GetParameter<FixedPoint>(2) * 0.009999999776482582 * constructionParametersMultiplier,
														traitDefinition.GetParameter<FixedPoint>(3) * 0.009999999776482582 * constructionParametersMultiplier
													}, new string[4] { "StrengthenDefenseFunc2Param1", "StrengthenDefenseFunc2Param2", "StrengthenDefenseFunc2Param3", "StrengthenDefenseFunc2Param4" }, traitDefinition.DisplayName);
												}
												else if (traitIdentifier.Contains(StrengthenDefenseFunc3_ToLower))
												{
													CreateAndAddIncrementerModifier(list, new FixedPoint[3]
													{
														traitDefinition.GetParameter<FixedPoint>(0) * 0.009999999776482582 * constructionParametersMultiplier,
														traitDefinition.GetParameter<FixedPoint>(1) * constructionParametersMultiplier,
														traitDefinition.GetParameter<FixedPoint>(2) * 0.009999999776482582 * constructionParametersMultiplier
													}, new string[3] { "StrengthenDefenseFunc3Param1", "StrengthenDefenseFunc3Param2", "StrengthenDefenseFunc3Param3" }, traitDefinition.DisplayName);
													StrengthenDefenseFunc3Trait item83 = new StrengthenDefenseFunc3Trait();
													list.Add(item83);
												}
												else if (traitIdentifier.Contains(Equipment_Passive_Flame_ToLower))
												{
													FlameTrait item84 = new FlameTrait(traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier);
													list.Add(item84);
												}
												else if (traitIdentifier.Contains(Equipment_Passive_DefendingHeart_ToLower))
												{
													CreateAndAddIncrementerModifier(list, new FixedPoint[3]
													{
														traitDefinition.GetParameter<FixedPoint>(0) * 0.009999999776482582 * constructionParametersMultiplier,
														traitDefinition.GetParameter<int>(1),
														traitDefinition.GetParameter<int>(2)
													}, new string[3] { "Equipment_Passive_DefendingHeartPercentage", "Equipment_Passive_DefendingHeartTurns", "Equipment_Passive_DefendingHeartCD" }, traitDefinition.DisplayName);
												}
												else if (traitIdentifier.Contains(GodWarBless_ToLower))
												{
													CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) * 0.009999999776482582 * constructionParametersMultiplier }, new string[1] { "GodWarBless_DmgPercentage" }, traitDefinition.DisplayName);
													GodWarTrait item85 = new GodWarTrait(traitDefinition.GetParameter<int>(1), traitDefinition.GetParameter<FixedPoint>(2) * 0.009999999776482582 * constructionParametersMultiplier);
													list.Add(item85);
												}
												else if (traitIdentifier.Contains(Equipment_Passive_Dash_ToLower))
												{
													DashTrait item86 = new DashTrait(traitDefinition.GetParameter<int>(0), traitDefinition.GetParameter<int>(1));
													list.Add(item86);
												}
												else if (traitIdentifier.Contains(Equipment_Passive_Backstep_ToLower))
												{
													BackstepTrait item87 = new BackstepTrait(traitDefinition.GetParameter<int>(0));
													list.Add(item87);
												}
												else if (traitIdentifier.Contains(ChargeAttackWithFreeShooting_ToLower))
												{
													ChargeAttackWithFreeShootingTrait item88 = new ChargeAttackWithFreeShootingTrait(traitDefinition.GetParameter<FixedPoint>(0) * 0.009999999776482582 * constructionParametersMultiplier, traitDefinition.GetParameter<int>(1));
													list.Add(item88);
												}
												else if (traitIdentifier.Contains(EquipmentPassiveFightBack_ToLower))
												{
													list.Add(new EquipmentPassiveFightBackTrait(traitDefinition.GetParameter<FixedPoint>(0) * 0.009999999776482582 * constructionParametersMultiplier, traitDefinition.GetParameter<FixedPoint>(1) * 0.009999999776482582 * constructionParametersMultiplier, traitDefinition.GetParameter<int>(2)));
												}
												else if (traitIdentifier.Contains(Equipment_Passive_Rage_ToLower))
												{
													CreateAndAddIncrementerModifier(list, new FixedPoint[13]
													{
														traitDefinition.GetParameter<FixedPoint>(0),
														traitDefinition.GetParameter<FixedPoint>(1),
														traitDefinition.GetParameter<FixedPoint>(2),
														traitDefinition.GetParameter<FixedPoint>(3) * 0.009999999776482582 * constructionParametersMultiplier,
														traitDefinition.GetParameter<FixedPoint>(4) * 0.009999999776482582 * constructionParametersMultiplier,
														traitDefinition.GetParameter<FixedPoint>(5),
														traitDefinition.GetParameter<FixedPoint>(6) * 0.009999999776482582 * constructionParametersMultiplier,
														traitDefinition.GetParameter<FixedPoint>(7) * 0.009999999776482582 * constructionParametersMultiplier,
														traitDefinition.GetParameter<FixedPoint>(8),
														traitDefinition.GetParameter<FixedPoint>(9) * 0.009999999776482582 * constructionParametersMultiplier,
														traitDefinition.GetParameter<FixedPoint>(10) * 0.009999999776482582 * constructionParametersMultiplier,
														traitDefinition.GetParameter<FixedPoint>(11) * 0.009999999776482582 * constructionParametersMultiplier,
														traitDefinition.GetParameter<FixedPoint>(12) * 0.009999999776482582 * constructionParametersMultiplier
													}, new string[13]
													{
														"Equipment_Passive_RageParam0", "Equipment_Passive_RageParam1", "Equipment_Passive_RageParam2", "Equipment_Passive_RageParam3", "Equipment_Passive_RageParam4", "Equipment_Passive_RageParam5", "Equipment_Passive_RageParam6", "Equipment_Passive_RageParam7", "Equipment_Passive_RageParam8", "Equipment_Passive_RageParam9",
														"Equipment_Passive_RageParam10", "Equipment_Passive_RageParam11", "Equipment_Passive_RageParam12"
													}, traitDefinition.DisplayName);
												}
												else if (traitIdentifier.Contains(Equipment_Passive_PassOW_ToLower))
												{
													PassOWTrait item89 = new PassOWTrait();
													list.Add(item89);
												}
												else if (traitIdentifier.Contains(Equipment_Passive_ScoutDMGBoost_ToLower))
												{
													CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) * 0.009999999776482582 * constructionParametersMultiplier }, new string[1] { "Equipment_Passive_ScoutDMGBoost_Dmg" }, traitDefinition.DisplayName);
												}
												else if (traitIdentifier.Contains(Equipment_Passive_BruiserDMGBoost_ToLower))
												{
													CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) * 0.009999999776482582 * constructionParametersMultiplier }, new string[1] { "Equipment_Passive_BruiserDMGBoost_Dmg" }, traitDefinition.DisplayName);
												}
												else if (traitIdentifier.Contains(Equipment_Passive_WarriorDMGBoost_ToLower))
												{
													CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) * 0.009999999776482582 * constructionParametersMultiplier }, new string[1] { "Equipment_Passive_WarriorDMGBoost_Dmg" }, traitDefinition.DisplayName);
												}
												else if (traitIdentifier.Contains(Equipment_Passive_ShooterDMGBoost_ToLower))
												{
													CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) * 0.009999999776482582 * constructionParametersMultiplier }, new string[1] { "Equipment_Passive_ShooterDMGBoost_Dmg" }, traitDefinition.DisplayName);
												}
												else if (traitIdentifier.Contains(Equipment_Passive_HunterDMGBoost_ToLower))
												{
													CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) * 0.009999999776482582 * constructionParametersMultiplier }, new string[1] { "Equipment_Passive_HunterDMGBoost_Dmg" }, traitDefinition.DisplayName);
												}
												else if (traitIdentifier.Contains(Equipment_Passive_AssaultDMGBoost_ToLower))
												{
													CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) * 0.009999999776482582 * constructionParametersMultiplier }, new string[1] { "Equipment_Passive_AssaultDMGBoost_Dmg" }, traitDefinition.DisplayName);
												}
												else if (traitIdentifier.Contains(AttackWithTriggerDot_ToLower))
												{
													AttackWithTriggerDotTrait item90 = new AttackWithTriggerDotTrait(traitDefinition.GetParameter<int>(0), traitDefinition.GetParameter<int>(1), traitDefinition.GetParameter<FixedPoint>(2) * 0.009999999776482582 * constructionParametersMultiplier);
													list.Add(item90);
												}
												else if (traitIdentifier.Contains("Cadence".ToLower()))
												{
													CadenceTrait item91 = new CadenceTrait(traitDefinition.GetParameter<int>(0), traitDefinition.GetParameter<int>(1), traitDefinition.GetParameter<int>(2), traitDefinition.GetParameter<int>(3), traitDefinition.GetParameter<FixedPoint>(4) * 0.009999999776482582 * constructionParametersMultiplier);
													list.Add(item91);
												}
												else if (traitIdentifier.Contains("FireSpread".ToLower()))
												{
													FireSpreadTrait item92 = new FireSpreadTrait(traitDefinition.GetParameter<FixedPoint>(0) * 0.009999999776482582 * constructionParametersMultiplier, traitDefinition.GetParameter<FixedPoint>(1) * 0.009999999776482582 * constructionParametersMultiplier, traitDefinition.GetParameter<int>(2), traitDefinition.GetParameter<int>(3), traitDefinition.GetParameter<int>(4), traitDefinition.GetParameter<int>(5));
													list.Add(item92);
												}
												else if (traitIdentifier.Contains(Equipment_Passive_HPPercentDmg_ToLower))
												{
													CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) * 0.009999999776482582 * constructionParametersMultiplier }, new string[1] { "Equipment_Passive_HPPercentDmg_Per" }, traitDefinition.DisplayName);
												}
												else if (traitIdentifier.Contains(Equipment_Active_BloodFrenzy_ToLower))
												{
													CreateAndAddIncrementerModifier(list, new FixedPoint[2]
													{
														traitDefinition.GetParameter<FixedPoint>(0) * 0.009999999776482582 * constructionParametersMultiplier,
														traitDefinition.GetParameter<FixedPoint>(1) * 0.009999999776482582 * constructionParametersMultiplier
													}, new string[2] { "Equipment_Active_BloodFrenzy_Hp", "Equipment_Active_BloodFrenzy_Dmg" }, traitDefinition.DisplayName);
													list.Add(new BloodFrenzyTrait());
												}
												else if (traitIdentifier.Contains(LeaderBuffSurvivalGame_ToLower))
												{
													CreateAndAddIncrementerModifier(list, new FixedPoint[11]
													{
														traitDefinition.GetParameter<FixedPoint>(0),
														traitDefinition.GetParameter<FixedPoint>(1),
														traitDefinition.GetParameter<FixedPoint>(2),
														traitDefinition.GetParameter<FixedPoint>(3),
														traitDefinition.GetParameter<FixedPoint>(4),
														traitDefinition.GetParameter<FixedPoint>(5) * 0.009999999776482582 * constructionParametersMultiplier,
														traitDefinition.GetParameter<FixedPoint>(6),
														traitDefinition.GetParameter<FixedPoint>(7) * 0.009999999776482582 * constructionParametersMultiplier,
														traitDefinition.GetParameter<FixedPoint>(8) * 0.009999999776482582 * constructionParametersMultiplier,
														traitDefinition.GetParameter<FixedPoint>(9) * 0.009999999776482582 * constructionParametersMultiplier,
														traitDefinition.GetParameter<FixedPoint>(12)
													}, new string[11]
													{
														"LeaderBuffSurvivalGame_TraitDis", "LeaderBuffSurvivalGame_MaxTurns", "LeaderBuffSurvivalGame_CDTurns", "LeaderBuffSurvivalGame_NoDeadLevel", "LeaderBuffSurvivalGame_NoDeadMaxCount", "LeaderBuffSurvivalGame_DmgUp", "LeaderBuffSurvivalGame_MoveDisDown", "LeaderBuffSurvivalGame_DmgUpEachEff", "LeaderBuffSurvivalGame_ChanceStun", "LeaderBuffSurvivalGame_HealPer",
														"LeaderBuffSurvivalGame_LuckyDis"
													}, traitDefinition.DisplayName);
												}
												else if (traitIdentifier.Contains(DeadlyFocusEXDamageActive_ToLower))
												{
													list.Add(new AbilityModifierMultiplier("DeadlyFocusEXDamageActiveMultiplier", constructionParametersMultiplier));
												}
												else if (traitIdentifier.Contains(BaseDeadlyFocus_ToLower))
												{
													list.Add(new BaseDeadlyFocusTrait());
												}
												else if (traitIdentifier.Contains(LeaderBuffDeadlyFocus_ToLower))
												{
													CreateAndAddIncrementerModifier(list, new FixedPoint[15]
													{
														traitDefinition.GetParameter<FixedPoint>(0),
														traitDefinition.GetParameter<FixedPoint>(1),
														traitDefinition.GetParameter<FixedPoint>(2) * 0.009999999776482582 * constructionParametersMultiplier,
														traitDefinition.GetParameter<FixedPoint>(3) * 0.009999999776482582 * constructionParametersMultiplier,
														traitDefinition.GetParameter<FixedPoint>(4) * 0.009999999776482582 * constructionParametersMultiplier,
														traitDefinition.GetParameter<FixedPoint>(5) * 0.009999999776482582 * constructionParametersMultiplier,
														traitDefinition.GetParameter<FixedPoint>(6),
														traitDefinition.GetParameter<FixedPoint>(7),
														traitDefinition.GetParameter<FixedPoint>(8),
														traitDefinition.GetParameter<FixedPoint>(9),
														traitDefinition.GetParameter<FixedPoint>(10) * 0.009999999776482582 * constructionParametersMultiplier,
														traitDefinition.GetParameter<FixedPoint>(11),
														traitDefinition.GetParameter<FixedPoint>(12),
														traitDefinition.GetParameter<FixedPoint>(13) * 0.009999999776482582 * constructionParametersMultiplier,
														traitDefinition.GetParameter<FixedPoint>(14) * 0.009999999776482582 * constructionParametersMultiplier
													}, new string[15]
													{
														"LeaderBuffDeadlyFocus_BuffEnemyMaxCount", "LeaderBuffDeadlyFocus_BuffMaxTurns", "LeaderBuffDeadlyFocus_PursuitChance", "LeaderBuffDeadlyFocus_PursuitDmgPer", "LeaderBuffDeadlyFocus_ChargePursuitChance", "LeaderBuffDeadlyFocus_DmgUpPerKill", "LeaderBuffDeadlyFocus_DmgUpPerKill_Max", "LeaderBuffDeadlyFocus_LevelReq_KilledTransDis", "LeaderBuffDeadlyFocus_KilledTransDis", "LeaderBuffDeadlyFocus_LevelReq_ExApChance",
														"LeaderBuffDeadlyFocus_ExApChance", "LeaderBuffDeadlyFocus_LevelReq_ChargeBuff", "LeaderBuffDeadlyFocus_LevelReq_ExDmgHitRate", "LeaderBuffDeadlyFocus_ExDmgHitRate_ExDmg", "LeaderBuffDeadlyFocus_ExDmgHitRate_HitRate"
													}, traitDefinition.DisplayName);
												}
												else if (traitIdentifier.Contains(SurvivalManualStorySkill_A_ToLower))
												{
													CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) / 100.0 * constructionParametersMultiplier }, new string[1] { "SurvivalManualDecreaseBodyshotChance" }, traitDefinition.DisplayName);
													CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(1) / 100.0 * constructionParametersMultiplier }, new string[1] { "SurvivalManualMaxDecreaseBodyshotChance" }, traitDefinition.DisplayName);
												}
												else if (traitIdentifier.Contains(LeaderBuffShadowedGuard_ToLower))
												{
													CreateAndAddIncrementerModifier(list, new FixedPoint[13]
													{
														traitDefinition.GetParameter<FixedPoint>(0) * 0.009999999776482582 * constructionParametersMultiplier,
														traitDefinition.GetParameter<FixedPoint>(1),
														traitDefinition.GetParameter<FixedPoint>(2),
														traitDefinition.GetParameter<FixedPoint>(3),
														traitDefinition.GetParameter<FixedPoint>(4) * 0.009999999776482582 * constructionParametersMultiplier,
														traitDefinition.GetParameter<FixedPoint>(5) * 0.009999999776482582 * constructionParametersMultiplier,
														traitDefinition.GetParameter<FixedPoint>(6),
														traitDefinition.GetParameter<FixedPoint>(7),
														traitDefinition.GetParameter<FixedPoint>(8),
														traitDefinition.GetParameter<FixedPoint>(9) * 0.009999999776482582 * constructionParametersMultiplier,
														traitDefinition.GetParameter<FixedPoint>(10),
														traitDefinition.GetParameter<FixedPoint>(11),
														traitDefinition.GetParameter<FixedPoint>(12)
													}, new string[13]
													{
														"LeaderBuffShadowedGuard_HpDmg", "LeaderBuffShadowedGuard_Charge_AtkNum", "LeaderBuffShadowedGuard_Charge_UnderAtkNum", "LeaderBuffShadowedGuard_Charge_MaxNum", "LeaderBuffShadowedGuard_Hp_PerReduce", "LeaderBuffShadowedGuard_Hp_PreChange", "LeaderBuffShadowedGuard_MaxTurns", "LeaderBuffShadowedGuard_CDTurns", "LeaderBuffShadowedGuard_Level_Resist", "LeaderBuffShadowedGuard_Level_Resist_Per",
														"LeaderBuffShadowedGuard_Level_Recover", "LeaderBuffShadowedGuard_Level_Charge", "LeaderBuffShadowedGuard_Add_Charge"
													}, traitDefinition.DisplayName);
													list.Add(new ShadowedGuardAddChargeTrait(traitDefinition.GetParameter<FixedPoint>(1), traitDefinition.GetParameter<FixedPoint>(2)));
												}
												else if (traitIdentifier.Contains(ShadowedGuard_StateRef_ToLower))
												{
													list.Add(new ShadowedGuardTrait((float)traitDefinition.GetParameter<int>(0) * 0.01f * constructionParametersMultiplier));
												}
												else if (traitIdentifier.Contains(Equipment_VengefulCharge_ToLower))
												{
													CreateAndAddIncrementerModifier(list, new FixedPoint[6]
													{
														traitDefinition.GetParameter<FixedPoint>(0),
														traitDefinition.GetParameter<FixedPoint>(1),
														traitDefinition.GetParameter<FixedPoint>(2),
														traitDefinition.GetParameter<FixedPoint>(3),
														traitDefinition.GetParameter<FixedPoint>(4),
														traitDefinition.GetParameter<FixedPoint>(5) * 0.009999999776482582 * constructionParametersMultiplier
													}, new string[6] { "Equipment_VengefulCharge_MarkNum", "Equipment_VengefulCharge_APNum", "Equipment_VengefulCharge_APNum_Max", "Equipment_VengefulCharge_MarkNum_Max", "Equipment_VengefulCharge_MarkNumShadowedGuard", "Equipment_VengefulCharge_PerMarkDmg" }, traitDefinition.DisplayName);
												}
												else if (traitIdentifier.Contains(Equipment_LastStand_ToLower))
												{
													CreateAndAddIncrementerModifier(list, new FixedPoint[2]
													{
														traitDefinition.GetParameter<FixedPoint>(0) * 0.009999999776482582 * constructionParametersMultiplier,
														traitDefinition.GetParameter<FixedPoint>(1) * 0.009999999776482582 * constructionParametersMultiplier
													}, new string[2] { "Equipment_LastStand_HPLowerMultiplier", "Equipment_LastStand_DmgMultiplier" }, traitDefinition.DisplayName);
												}
												else if (traitIdentifier.Contains(Defense_ToLower))
												{
													CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) }, new string[1] { "Equipment_DefDefense_Melee" }, traitDefinition.DisplayName);
													CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(1) / 100.0 * constructionParametersMultiplier }, new string[1] { "Equipment_Defense_Melee_Percent" }, traitDefinition.DisplayName);
													CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(2) }, new string[1] { "Equipment_DefDefense_Range" }, traitDefinition.DisplayName);
													CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(3) / 100.0 * constructionParametersMultiplier }, new string[1] { "Equipment_Defense_Range_Percent" }, traitDefinition.DisplayName);
												}
												else if (traitIdentifier.Contains("LeaderBuffCitadel".ToLower()))
												{
													CreateAndAddIncrementerModifier(list, new FixedPoint[3]
													{
														traitDefinition.GetParameter<FixedPoint>(0),
														traitDefinition.GetParameter<FixedPoint>(1),
														traitDefinition.GetParameter<FixedPoint>(3) * 0.009999999776482582 * constructionParametersMultiplier
													}, new string[3] { "LeaderBuffCitadel_Range", "LeaderBuffCitadel_TargetFaction", "LeaderBuffCitadel_DownOverWatchPercent" }, traitDefinition.DisplayName);
												}
												else if (traitIdentifier.Contains("BaseCitadel".ToLower()))
												{
													list.Add(new BaseCitadelTrait());
												}
												else if (traitIdentifier.Contains("Citadel_MoveDown".ToLower()))
												{
													CreateAndAddIncrementerModifier(list, new FixedPoint[1] { traitDefinition.GetParameter<FixedPoint>(0) }, new string[1] { "Citadel_MoveDownNum" }, traitDefinition.DisplayName);
												}
												else if (traitIdentifier.Contains("Citadel_RangeDown".ToLower()))
												{
													CreateAndAddIncrementerModifier(list, new FixedPoint[2]
													{
														traitDefinition.GetParameter<FixedPoint>(0),
														traitDefinition.GetParameter<FixedPoint>(1)
													}, new string[2] { "Citadel_RangeDown_RangeMultiplier", "Citadel_RangeDown_MinRangeMultiplier" }, traitDefinition.DisplayName);
												}
												else if (traitIdentifier.Contains("Undying".ToLower()))
												{
													int parameter22 = traitDefinition.GetParameter<int>(0);
													int parameter23 = traitDefinition.GetParameter<int>(1);
													int parameter24 = traitDefinition.GetParameter<int>(2);
													FixedPoint healPercentage = traitDefinition.GetParameter<FixedPoint>(3) / 100.0 * constructionParametersMultiplier;
													int parameter25 = traitDefinition.GetParameter<int>(4);
													int parameter26 = traitDefinition.GetParameter<int>(5);
													list.Add(new UndyingTrait(parameter22, parameter23, parameter24, healPercentage, parameter25, parameter26));
												}
												else if (traitIdentifier.Contains(LeaderBuffDeathsDoor_ToLower))
												{
													CreateAndAddIncrementerModifier(list, new FixedPoint[8]
													{
														traitDefinition.GetParameter<FixedPoint>(0) * 0.009999999776482582 * constructionParametersMultiplier,
														traitDefinition.GetParameter<FixedPoint>(1),
														traitDefinition.GetParameter<FixedPoint>(2),
														traitDefinition.GetParameter<FixedPoint>(3) * 0.009999999776482582 * constructionParametersMultiplier,
														traitDefinition.GetParameter<FixedPoint>(4) * 0.009999999776482582 * constructionParametersMultiplier,
														traitDefinition.GetParameter<FixedPoint>(5),
														traitDefinition.GetParameter<FixedPoint>(6),
														traitDefinition.GetParameter<FixedPoint>(7) * 0.009999999776482582 * constructionParametersMultiplier
													}, new string[8] { "LeaderBuffDeathsDoor_DmgUpPerLayer", "LeaderBuffDeathsDoor_MaxLayer", "LeaderBuffDeathsDoor_DmgUpDuration", "LeaderBuffDeathsDoor_PursuitChance", "LeaderBuffDeathsDoor_PursuitDmgUp", "LeaderBuffDeathsDoor_MaxPursuitCount", "LeaderBuffDeathsDoor_UnlockLevel", "LeaderBuffDeathsDoor_MaxDmgUp" }, traitDefinition.DisplayName);
												}
												else if (traitIdentifier.Contains(BaseDeathsDoor_ToLower))
												{
													list.Add(new BaseDeathsDoorTrait());
												}
												else if (!IsDeprecated(traitIdentifier))
												{
													Type type = ReflectionUtils.FindDerivedTypeStartingWith(typeof(ModelModifier), traitDefinition.GetTraitClassName());
													List<string> inConstructorParams = (chance.HasValue ? new List<string> { chance.ToString() } : traitDefinition.ConstructionParameters);
													ModelModifier modelModifier = ((type != null) ? (ReflectionUtils.Instantiate(type, inConstructorParams) as ModelModifier) : null);
													if (modelModifier != null)
													{
														list.Add(modelModifier);
													}
													else if (base.manager != null)
													{
														base.Debug.LogWarning("Could not instantiate trait modifier for " + traitDefinition.GetTraitClassName());
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
			return list;
		}

		public TraitAbilityModel CreateTraitAbilityModel(TraitDefinition traitDefinition, List<ModelModifier> modifiers)
		{
			TraitAbilityModel traitAbilityModel = null;
			if (modifiers != null && modifiers.Count > 0)
			{
				AbilityDefinition abilityDefinition = new AbilityDefinition();
				abilityDefinition.Type = AbilityType.Passive;
				abilityDefinition.DisplayName = "TraitPassiveAbility";
				abilityDefinition.Hidden = true;
				traitAbilityModel = new TraitAbilityModel(traitDefinition.Identifier);
				traitAbilityModel.SetDefinition(abilityDefinition);
				traitAbilityModel.SetManager(base.manager);
				traitAbilityModel.Initialize();
				traitAbilityModel.Start();
				foreach (ModelModifier modifier in modifiers)
				{
					traitAbilityModel.Modifiers.RegisterModifier(modifier);
					modifier.SetManager(base.manager);
					modifier.Initialize();
					modifier.Start();
				}
			}
			return traitAbilityModel;
		}

		public AbilityModel InstantiateTraitAbility(TraitDefinition traitDefinition, FixedPoint constructionParametersMultiplier)
		{
			return InstantiateTraitAbility(traitDefinition, constructionParametersMultiplier, null);
		}

		public AbilityModel InstantiateTraitAbility(TraitDefinition traitDefinition, FixedPoint constructionParametersMultiplier, FixedPoint? chance)
		{
			List<ModelModifier> modifiers = CreateTraitModifiers(traitDefinition, constructionParametersMultiplier, chance);
			return CreateTraitAbilityModel(traitDefinition, modifiers);
		}

		public override void Tick(long deltaTime)
		{
			if (base.manager.CombatModel != null)
			{
				return;
			}
			List<TraitEntry> list = new List<TraitEntry>();
			foreach (TraitEntry trait in Traits)
			{
				if (trait.TraitDuration > 0)
				{
					trait.TraitDuration -= deltaTime;
					if (trait.TraitDuration <= 0)
					{
						list.Add(trait);
					}
				}
			}
			foreach (TraitEntry item in list)
			{
				RemoveTrait(item.TraitIdentifier);
			}
		}

		public string GetTraitNames()
		{
			if (Traits.Count == 0)
			{
				return "";
			}
			string text = "(";
			foreach (TraitEntry trait in Traits)
			{
				text = text + trait.TraitIdentifier + ", ";
			}
			return text.Substring(0, text.Length - 2) + ")";
		}

		public static bool IsDeprecated(string traitIdentifier)
		{
			string text = traitIdentifier.ToLower();
			foreach (string item in DeprecatedTraitsLower)
			{
				if (text.Contains(item))
				{
					return true;
				}
			}
			return false;
		}
	}
}
