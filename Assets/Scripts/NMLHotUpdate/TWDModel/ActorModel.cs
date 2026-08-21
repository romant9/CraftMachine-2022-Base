using System;
using System.Collections.Generic;
using System.Linq;
using BaseModel;
using Newtonsoft.Json;

namespace TWDModel
{
	public class ActorModel : TWDModelObject
	{
		public class ABtestParam
		{
			public FixedPoint A_DamageMultiplier = 0.0;

			public ActorModel A_source;

			public FixedPoint B_APChance = 0.0;

			public ActorModel B_source;
		}

		public AttackChainStaus AttackChainStaus;

		public const string PassiveTraitTag = "EquipmentPassive";

		public static readonly bool ACTION_RANGE = true;

		private static readonly string ClassAssault = SurvivorClass.Assault.ToString();

		private static readonly string ClassHunter = SurvivorClass.Hunter.ToString();

		private static readonly string ClassShooter = SurvivorClass.Shooter.ToString();

		private static readonly string ClassBruiser = SurvivorClass.Bruiser.ToString();

		private static readonly string ClassScout = SurvivorClass.Scout.ToString();

		private static readonly string ClassWarrior = SurvivorClass.Warrior.ToString();

		public const string ClassBoss = "Boss";

		[JsonIgnore]
		private bool definitionInvalid = true;

		public bool dashTraitAttackFlag;

		public bool dashTraitValidFlag = true;

		public bool bloodFrenzyFlag;

		public bool SupportTalent_NoMoveHitrateFlag;

		public bool SupportTalent_NoMoveCritRateFlag;

		[JsonIgnore]
		private ActorDefinition definition;

		private string actorDefinitionID;

		public bool Iskill;

		[JsonIgnore]
		private bool _isRandomActiveLightState;

		[JsonIgnore]
		private bool _equipmentActiveLightState;

		[JsonIgnore]
		private bool _equipmentFreeOWState;

		public Dictionary<Faction, HeirloomsHershelFetter> HeirloomsHershelFetterFloor;

		public bool ActorFactionChangedInCombat;

		[JsonIgnore]
		private int moveRange;

		public bool IsAttackAndBeAttacked;

		private bool timedEffectEnding;

		public bool FocusModeState;

		public bool FocusModeStateChargeCD;

		public int FistSpikeTurns;

		public int DodgeShotTurns;

		public int DodgeShotTimes;

		public bool IsMoving;

		public int NextCanTriggerFirstAidTurn;

		public int NextReadyEquipmentPassiveRemoveNegativeEndTurn;

		public int NextReadyEquipmentPassiveRemoveNegativeStartTurn;

		public int NextReadyEquipmentPassiveRemoveNegativeOwnTurn;

		public int NextCanTriggerPassOW;

		public int UnluckyFlagTurns;

		public int SurvivalDashFlagTurns;

		public int PastaTurns;

		public bool PastaCurrentTurn;

		public bool CapFirstAttack;

		public bool CapFirstHeal;

		public int GodWarTraitTurns;

		public bool IsTriggerPassOW;

		public int BlindLeftTurns;

		public FixedPoint BlindDecreaseRate = 0L;

		public int RaiderDashFlagTurns;

		public int DefendingHeartTraitCDTurns;

		public int DefendingHeartTraitEffectLeftTurns;

		public int DebuffStatusRemoveTurns;

		[JsonIgnore]
		public FixedVec3 ForwardDirection;

		public int CadenceAttackCount;

		public bool CadenceReady;

		public bool CadenceBoostingThisAttack;

		private bool bloodThirst;

		private bool _onRedHealthBar;

		public const string ActorUndyingUpdateEvent = "actorUndyingUpdateEvent";

		public const string ActorKilledEvent = "actorKilledEvent";

		public const string ActorElectronChargeUpdateEvent = "ActorElectronChargeUpdateEvent";

		public const string ActorQuantunUpdate = "ActorQuantunUpdate";

		public const string ActorShieldBreakerUpdate = "ActorShieldBreakerUpdate";

		public const string ActorUnluckyUpdate = "ActorUnluckyUpdate";

		public const string Unlucky = "Unlucky";

		public const string UpdateBloodMarkEvent = "UpdateBloodMarkEvent";

		public const string EquipmentPassiveRemoveNegativeVisited = "EquipmentPassiveRemoveNegativeVisited";

		public const string ActorMomentumUpdate = "ActorMomentumUpdate";

		public const string ActorDebuffDamagePerRoundUpdate = "ActorDebuffDamagePerRoundUpdate";

		public const string ActorDebuffDamagePerRoundGetDamage = "ActorDebuffDamagePerRoundGetDamage";

		public const string ActorDebuffReduceRecoveryUpdate = "ActorDebuffReduceRecoveryUpdate";

		public const string ReduceRecovery = "ReduceRecovery";

		public const string ActorElectricShockedEvent = "ActorElectricShockedEvent";

		public const string ActorElectricSurgedEvent = "ActorElectricSurgedEvent";

		public const string ActorStunnedEvent = "actorStunnedEvent";

		public const string ActorQuantunCanNotMoveEvent = "ActorQuantunCanNotMoveEvent";

		public const string ActorRootedEvent = "actorRootedEvent";

		public const string ActorRageUpdateEvent = "ActorRageUpdateEvent";

		public const string actorFocusedEvent = "actorFocusedEvent";

		public const string ActorStaggeredEvent = "actorStaggeredEvent";

		public const string ActorCrippledEvent = "actorCrippledEvent";

		public const string ActorTauntedEvent = "actorTauntedEvent";

		public const string ActorSetInvisibleEvent = "actorSetInvisibleEvent";

		public const string ActorTimedEffectStart = "actorTimedEffectStart";

		public const string ActorTimedEffectEnd = "actorTimedEffectEnd";

		public const string ActorUserCanControlChanged = "actorUserCanControlChanged";

		public const string ActorTimedEffectUpdated = "actorTimedEffectUpdated";

		public const string ActorStruggleSaved = "actorStruggleSaved";

		public const string ActorBleedingOutSaved = "actorBleedingOutSaved";

		public const string ActorStruggleFinished = "actorStruggleFinished";

		public const string ActorLostTrait = "actorLostTrait";

		public const string ActorInteractionInterrupting = "actorInteractionInterrupting";

		public const string ActorInteractionCompleting = "actorInteractionCompleting";

		public const string ActorMoveCompleted = "actorMoveCompleted";

		public const string ActorSecondMoveCompleted = "actorSecondMoveCompleted";

		public const string ActorAbilityCompleted = "actorAbilityCompleted";

		public const string ActorAdditionalAttackChecked = "actorAdditionalAttackChecked";

		public const string ActorUsedFreeAttack = "actorUsedFreeAttack";

		public const string ActorTurnCompleted = "actorTurnCompleted";

		public const string ActorExtraAbilityAction = "actorExtraAbilityAction";

		public const string ActorExtraMoveAction = "ActorExtraMoveAction";

		public const string ActorCleaved = "actorCleaved";

		public const string ActorAttackChainUpdate = "ActorAttackChainUpdate";

		public const string ActorNewTurn = "actorNewTurn";

		public const string bloodFrenzyFlagUpdate = "bloodFrenzyFlagUpdate";

		public const string ActorTurnToTarget = "actorTurnToTarget";

		public const string ActorCreateThreat = "actorCreateThreat";

		public const string ActorCriticalAim = "actorCriticalAim";

		public const string ActorRedact = "ActorRedact";

		public const string ActorThreatReduction = "actorThreatReduction";

		public const string ActorSilenced = "ActorSilenced";

		public const string ActorAIAlertnessStateChanged = "actorAIAlertnessStateChanged";

		public const string ActorReceivedSP = "actorReceivedSP";

		public const string ActorTraitGained = "actorTraitGained";

		public const string ActorBePoisoned = "ActorBePoisoned";

		public const string ActorPoisonUpdate = "ActorPoisonUpdate";

		public const string ActorBeRemoteWeakened = "ActorBeRemoteWeakened";

		public const string ActorBeAsthenia = "ActorBeAsthenia";

		public const string ActorAstheniaUpdate = "ActorAstheniaUpdate";

		public const string ActorHeirloomsHershelFetterUpdate = "ActorHeirloomsHershelFetterUpdate";

		public const string ActorHeirloomsHershelFetterMessage = "ActorHeirloomsHershelFetterMessage";

		public const string ActorPassiveFlameMessage = "ActorPassiveFlameMessage";

		public const string ActorBeGrenadeFragmentDamaged = "ActorBeGrenadeFragmentDamaged";

		public const string ActorGrenadeFragmentDamageUpdate = "ActorGrenadeFragmentDamageUpdate";

		public const string ActorExploded = "actorExploded";

		public const string ActorFreeAttackFailed = "freeAttackFailed";

		public const string ActorHealthChanged = "ActorHealthChanged";

		public const string ActorReceivedChargePoint = "ActorReceivedChargePoint";

		public const string ActorReloadingStarted = "ActorReloadingStarted";

		public const string ActorReloadingFinished = "ActorReloadingFinished";

		public const string ToggleEquippedEquipments = "ToggleEquippedEquipments";

		public const string UnEquipConsumable = "UnEquipConsumable";

		public const string AbilityVisited = "AbilityVisited";

		public const string GodWarSkillChange = "GodWarSkillChange";

		public const string FortunaMainTraitChange = "FortunaMainTraitChange";

		public const string WeaponAbilityVisited = "WeaponAbilityVisited";

		public const string CriticalHitAvoided = "CriticalHitAvoided";

		public const string BetterTogetherCountChanged = "BetterTogetherCountChanged";

		public const string ActorDisorientedEvent = "ActorDisorientedEvent";

		public const string UpdateTurnFactionEvent = "UpdateTurnFactionEvent";

		public const string UpdateEffectDurationEvent = "UpdateEffectDurationEvent";

		public const string TurnCountChangedEvent = "TurnCountChangedEvent";

		public const string UpdateSurvivalGameEvent = "UpdateSurvivalGameEvent";

		public const string EquipmentActiveChargeLoadEvent = "EquipmentActiveChargeLoadEvent";

		public const string ShieldChanged = "ShieldChanged";

		public const string SkillIncreaseAttackChanged = "SkillIncreaseAttackChanged";

		public const string HelpHandDamageChanged = "HelpHandDamageChanged";

		public const string CommonDamageChanged = "CommnDamageChanged";

		public const string JoinFocusMode = "JoinFocusMode";

		public const string AbortFocusMode = "AbortFocusMode";

		public const string HideFocusModeBTN = "HideFocusModeBTN";

		public const string ShowsFocusModeBTN = "ShowsFocusModeBTN";

		public const string RefreshFistSpikeTurns = "RefreshFistSpikeTurns";

		public const string RefreshDodgeShot = "RefreshDodgeShot";

		public const string SurvivalDashFlagUpdate = "SurvivalDashFlagUpdate";

		public const string RaiderDashFlagUpdate = "RaiderDashFlagUpdate";

		public const string FlameTrigger = "FlameTrigger";

		public const string OverLoadEvent = "OverLoadEvent";

		public const string PerlieFlameTrigger = "PerlieFlameTrigger";

		public const string UpParryRiposteFloor = "UpParryRiposteFloor";

		private int _additionalMoveRange;

		public bool CanMoveWithoutAttacking;

		public bool FightingFuryActivated;

		public bool AdditionalAttackConsumed;

		public bool GainedAPFromPreviousAbilityExecution;

		public bool GainedAPFromAbilityExecution;

		public bool freeAttackUsed;

		public int BetterTogetherMultiplier;

		private int _randomStatusNumberOfAttack;

		public FixedPoint DebuffKnockKnockMarkCount = 0L;

		public FixedPoint DebuffKnockKnockMarkMaxConfig = 0L;

		public FixedPoint OneTurnAttackedTimes = 0L;

		public const string KnockKnockMarkUpdateEvent = "KnockKnockMarkUpdateEvent";

		public const string PhonePortraitUpdateEvent = "PhonePortraitUpdateEvent";

		public FixedPoint TornApartMarkCount = 0L;

		public const string TornApartUpdateEvent = "TornApartUpdateEvent";

		public bool OneTurnCriticalHit;

		public bool OneTurnStagger;

		private bool isRecoilEffected;

		public int KilledEnemyNum;

		public FixedPoint ChargeLoadFloor;

		public ABtestParam abTestParam = new ABtestParam();

		public const string ABtestBUpdateEvent = "ABtestBUpdateEvent";

		public int FocusCoolOff;

		private int overloadStatusLeftTurns;

		private int overloadStatusEXAttackTimesInTurn;

		public const string UpdateDeadlyFocus = "UpdateDeadlyFocus";

		public int DeadlyFocusLeftCount_SourceSurvivor;

		public int DeadlyFocusLeftCount_SourceRaider;

		public int DeadlyFocus_EXDamageLayerCount;

		public const string ActorChargeNumUpdateEvent = "ActorChargeNumUpdateEvent";

		public FixedPoint chargeNumVal;

		public const string UpdateShadowedGuardEvent = "UpdateShadowedGuardEvent";

		public int ShadowedGuard_DelHP;

		public int ShadowedGuard_Atk;

		public const string UpdateGuardianVowEvent = "UpdateGuardianVowEvent";

		public static readonly string[] RemovableNegativeEffectNames = new string[23]
		{
			"StaggerActive", "Skinned", "Stun", "Burning", "Disoriented", "DisorientedLock", "Root", "Cripple", "DebuffMarkEnemy", "BaseKnockKnock",
			"Scorch", "Bleeding", "FistSpike", "Taunted", "DodgedShotInjurerFlag", "ABTesterAed", "Herd", "Asthenia", "Poison", "RemoteWeakened",
			"ElectricShock", "ElectronCharge", "BloodMark"
		};

		public const string ActorVengefulChargeUpdateEvent = "ActorVengefulChargeUpdateEvent";

		public const string ActorCitadelLeaderBuffUpdate = "ActorCitadelLeaderBuffUpdate";

		public const string ActorCitadelBeEffectedUpdate = "ActorCitadelBeEffectedUpdate";

		public List<string> CitadelLastTurnAddedTraits = new List<string>();

		public List<string> FortunaRandomTraitIds;

		public const string UpdateDeathsDoor = "UpdateDeathsDoor";

		public const string DeathsDoorBlockSecondChance = "DeathsDoorBlockSecondChance";

		public UndyingState UndyingState;

		[JsonIgnore]
		public bool IsAttackChainStatus => AttackChainStaus?.IsAttackChain ?? false;

		public List<int> AsTargetAttackChainSlots { get; set; }

		[JsonIgnore]
		public bool AttackChainGainExtraActionPoint { get; set; }

		public ActorAttributeContainerModel ActorAttributeContainer { get; set; }

		public Dictionary<AttributeType, FixedPoint> CombatAttributeSnapshots { get; set; }

		public CoexistTimedEffectsManager CoexistTimedEffectsManager { get; set; }

		[JsonIgnore]
		public bool IsQuantuned => CoexistTimedEffectsManager?.GetCoexistTimedEffect(CoexistTimedEffectType.Quantun) != null;

		[JsonIgnore]
		public int QuantunTurns
		{
			get
			{
				CoexistTimedEffectAbstract coexistTimedEffectAbstract = CoexistTimedEffectsManager?.GetCoexistTimedEffect(CoexistTimedEffectType.Quantun);
				if (coexistTimedEffectAbstract != null)
				{
					return coexistTimedEffectAbstract.Duration - coexistTimedEffectAbstract.Counter;
				}
				return 0;
			}
		}

		[JsonIgnore]
		public int QuantunLevel => (CoexistTimedEffectsManager?.GetCoexistTimedEffect<QuantunTimedEffect>(CoexistTimedEffectType.Quantun))?.CurrentLayer ?? 0;

		[JsonIgnore]
		public MomentumTimedEffect MomentumTimedEffect => CoexistTimedEffectsManager?.GetCoexistTimedEffect<MomentumTimedEffect>(CoexistTimedEffectType.Momentum);

		[JsonIgnore]
		public UnluckyTimedEffect UnluckyTimedEffect => CoexistTimedEffectsManager?.GetCoexistTimedEffect<UnluckyTimedEffect>(CoexistTimedEffectType.Unlucky);

		[JsonIgnore]
		public SkillShieldType1TimedEffect SkillShieldType1TimedEffect => CoexistTimedEffectsManager?.GetCoexistTimedEffect<SkillShieldType1TimedEffect>(CoexistTimedEffectType.SkillShieldType1);

		[JsonIgnore]
		public SkillEquipTauntShieldTimedEffect SkillEquipTauntShieldTimedEffect => CoexistTimedEffectsManager?.GetCoexistTimedEffect<SkillEquipTauntShieldTimedEffect>(CoexistTimedEffectType.SkillEquipTauntShield);

		[JsonIgnore]
		public SkillIncreaseAttackTimedEffect SkillIncreaseAttackTimedEffect => CoexistTimedEffectsManager?.GetCoexistTimedEffect<SkillIncreaseAttackTimedEffect>(CoexistTimedEffectType.SkillIncreaseAttack);

		[JsonIgnore]
		public ShieldBreakerTimedEffect ShieldBreakerTimedEffect => CoexistTimedEffectsManager?.GetCoexistTimedEffect<ShieldBreakerTimedEffect>(CoexistTimedEffectType.ShieldBreaker);

		[JsonIgnore]
		public DebuffDamagePerRoundTimedEffect DebuffDamagePerRoundTimedEffect => CoexistTimedEffectsManager?.GetCoexistTimedEffect<DebuffDamagePerRoundTimedEffect>(CoexistTimedEffectType.DebuffDamagePerRound);

		[JsonIgnore]
		public DebuffReduceRecoveryTimedEffect DebuffReduceRecoveryTimedEffect => CoexistTimedEffectsManager?.GetCoexistTimedEffect<DebuffReduceRecoveryTimedEffect>(CoexistTimedEffectType.DebuffReduceRecovery);

		[JsonIgnore]
		public BerserkRageTimedEffect BerserkRageTimedEffect => CoexistTimedEffectsManager?.GetCoexistTimedEffect<BerserkRageTimedEffect>(CoexistTimedEffectType.BerserkRage);

		[JsonIgnore]
		public BloodMarkTimedEffect BloodMarkTimedEffect => CoexistTimedEffectsManager?.GetCoexistTimedEffect<BloodMarkTimedEffect>(CoexistTimedEffectType.BloodMark);

		[JsonIgnore]
		public bool HasBloodMark => BloodMarkTimedEffect != null;

		[JsonIgnore]
		public FortificationsTimedEffect FortificationsTimedEffect => CoexistTimedEffectsManager?.GetCoexistTimedEffect<FortificationsTimedEffect>(CoexistTimedEffectType.Fortifications);

		[JsonIgnore]
		public bool IsInFortifications => FortificationsTimedEffect != null;

		[JsonIgnore]
		public int FortificationsLeftTurns => FortificationsTimedEffect?.LeftTurns ?? 0;

		[JsonIgnore]
		public ModifierCollection Modifiers { get; private set; }

		public int ChargeAttackWithFreeShootingTriggeredCount { get; set; }

		public int FightBackTimesThisRound { get; set; }

		public int SharpBladeLayers { get; set; }

		public List<string> SPAddPassiveAllSlotsTraits { get; set; }

		[JsonIgnore]
		public ActorDefinition Definition
		{
			get
			{
				if (definitionInvalid)
				{
					definition = base.manager.GameEconomyData.GetActorDefinition(ActorDefinitionID);
					definitionInvalid = false;
				}
				return definition;
			}
		}

		public string ActorDefinitionID
		{
			get
			{
				return actorDefinitionID;
			}
			set
			{
				if (value != actorDefinitionID)
				{
					definitionInvalid = true;
					actorDefinitionID = value;
				}
			}
		}

		public GridCoordinate GridCoordinate { get; set; }

		[JsonIgnore]
		public virtual bool IsMultiCell => false;

		[JsonIgnore]
		public virtual bool IsImpenetrable
		{
			get
			{
				if (base.manager != null)
				{
					return GetTraitWithTag("Impenetrable") != null;
				}
				return false;
			}
		}

		[JsonIgnore]
		public bool HasDamageAreaBlock
		{
			get
			{
				if (base.manager != null && TraitContainer != null)
				{
					return HasAnyLevelTrait("Equipment.Passive.DamageAreaBlock");
				}
				return false;
			}
		}

		[JsonIgnore]
		public virtual bool UsesScreenTopHealthBar => false;

		public bool UseSpawnRotationOverride { get; set; }

		public float SpawnRotationY { get; set; }

		public GridCoordinate ActorsLastAbilityCell { get; set; }

		[JsonIgnore]
		public bool IsWalker
		{
			get
			{
				if (Faction != Faction.Walker)
				{
					return Faction == Faction.Dormant;
				}
				return true;
			}
		}

		[JsonIgnore]
		public bool IsHumanNPC
		{
			get
			{
				if (Faction != Faction.Civilian)
				{
					if (OfflineManager.IsLoadDataManager && StartGWBattle.Instance && StartGWBattle.Instance.IsAIForSurvivors)
						return Faction == Faction.Raider || Faction == Faction.Survivor;
					else
						return Faction == Faction.Raider;
				}
				return true;
			}
		}

		[JsonIgnore]
		public bool IsEnvironmental => Faction == Faction.Environmental;

		[JsonIgnore]
		public bool IsHuman
		{
			get
			{
				if (Faction != Faction.Civilian && Faction != Faction.Raider)
				{
					return Faction == Faction.Survivor;
				}
				return true;
			}
		}

		[JsonIgnore]
		public bool IsFriendlyHuman
		{
			get
			{
				if (Faction != Faction.Civilian)
				{
					return Faction == Faction.Survivor;
				}
				return true;
			}
		}

		[JsonIgnore]
		public bool IsEnemyNPC
		{
			get
			{
				if (Faction != Faction.Walker && Faction != Faction.Raider)
				{
					return Faction == Faction.Environmental;
				}
				return true;
			}
		}

		[JsonIgnore]
		public bool IsRaider => Faction == Faction.Raider;

		[JsonIgnore]
		public bool IsFlare
		{
			get
			{
				if (Faction == Faction.Lure && base.manager.CombatModel != null)
				{
					return base.manager.CombatModel.Perceptors.Contains(this);
				}
				return false;
			}
		}

		[JsonIgnore]
		public bool IsBossWalker
		{
			get
			{
				if (Faction == Faction.Walker)
				{
					return definition.ID.Contains("Boss");
				}
				return false;
			}
		}

		public bool IsBoss { get; set; }

		[JsonIgnore]
		public bool IsNormalWalker
		{
			get
			{
				if (Faction == Faction.Walker && definition.ID.Contains("Normal"))
				{
					return !definition.ID.Contains("Boss");
				}
				return false;
			}
		}

		public bool IsExploding { get; set; }

		public bool HasBeenSaved { get; set; }

		public int SavedOnTurnIndex { get; set; }

		public bool RevengedOnTurn { get; set; }

		public int ShieldRevengedTimesOnTurn { get; set; }

		public bool FollowThroughTriggeredInAttack { get; set; }

		[JsonIgnore]
		public bool AttackKilledAnyEnemy { get; set; }

		[JsonIgnore]
		public bool AttackHasNotKilledAllEnemies { get; set; }

		[JsonIgnore]
		public bool FollowUpAttackedOnTurn { get; set; }

		public bool OverwatchedOnTurn { get; set; }

		public bool PreAttackedOnTurn { get; set; }

		public bool PreAttackedOnRiposte { get; set; }

		public bool HasHeadshotLTTriggered { get; set; }

		[JsonIgnore]
		public bool PassByAttackedOnMove { get; set; }

		public bool IsHeirloomsHershelFetter
		{
			get
			{
				if (HeirloomsHershelFetterFloor != null && HeirloomsHershelFetterFloor.Count != 0)
				{
					return HeirloomsHershelFetterFloor.ContainsKey(Faction);
				}
				return false;
			}
		}

		public int NumberChargePointAtStart { get; set; }

		public FixedPoint DebuffRemoteRepulseWeakenAddChargePointPercentage { get; set; }

		public int DebuffRemoteRepulseWeakenAddChargePoints { get; set; }

		public int ParryRiposteIncreaseStorey { get; set; }

		[JsonIgnore]
		public bool ThisAbilityActionAttackUseAsthenia { get; set; }

		[JsonIgnore]
		public bool IsInteractingWithGuts { get; set; }

		[JsonIgnore]
		public int SurvivalManualStorySkill_DLayerCount
		{
			get
			{
				FixedPoint value = 0.0;
				FixedPoint value2 = 0.0;
				if (base.manager.Player.AbilityManager.VisitParameter("SurvivalManualKillIncreaseDmg", ref value, this))
				{
					base.manager.Player.AbilityManager.VisitParameter("SurvivalManualCurKillIncreaseDmg", ref value2, this);
				}
				if (value == 0L)
				{
					return 0;
				}
				return (int)(value2 / value);
			}
		}

		public TurnState TurnState { get; set; }

		public AIDataModel AIDataModel { get; set; }

		[JsonIgnore]
		public AIController AIController { get; protected set; }

		[IgnoreModelProperty]
		public ActorModel HelpreHandActorModel { get; set; }

		[IgnoreModelProperty]
		public ActorModel GuardActorModel { get; set; }

		public AttributeModel AttributeModel { get; set; }

		public ActorDebuffParameterManager DebuffParameterManager { get; set; }

		public CommandSkillModelManager CommandSkillModelManager { get; set; }

		[JsonIgnore]
		public bool IsAIControlled
		{
			get
			{
				if (AIController != null)
				{
					return AIController.HasControl;
				}
				return false;
			}
		}

		public Faction Faction { get; set; }

		public Faction OriginalFaction { get; set; }

		public ChargeMeterModel ChargeMeter { get; set; }

		[JsonIgnore]
		public bool IsDead
		{
			get
			{
				if (Hitpoints > 0)
				{
					return Faction == Faction.Lure;
				}
				return true;
			}
		}

		public bool UseModularCharacter { get; set; }

		public bool UserCanControl { get; protected set; }

		public string UserCanControlFalseReason { get; set; } = "";

		public int MoveRange
		{
			get
			{
				FixedPoint value = 0.0;
				if (base.manager != null && HasTraitsThatContains("Equipment_Passive_Fortuna_Heart"))
				{
					base.manager.Player?.AbilityManager?.VisitParameter("AbilityModifierEquipmentPassiveFortunaHeart", ref value, this);
				}
				int num = moveRange - MoveRangeConsumed - GetSurvivalGameMoveDown() + (int)value;
				bool flag = false;
				FixedPoint fixedPoint = 0L;
				if (base.manager != null && (Faction == Faction.Walker || Faction == Faction.Raider))
				{
					IChallengeDebuffProvider challengeDebuffProvider = MapMissionDebuffHelper.CanUseDebuffMission(base.manager);
					if (challengeDebuffProvider != null)
					{
						List<DifficultyIncrementalDebuff> challengeDebuffs = challengeDebuffProvider.GetChallengeDebuffs();
						if (ChallengeDebufHelps.GetDebufConfig(challengeDebuffs, ChallengeDebuffType.WalkerMoveLess) != null)
						{
							fixedPoint = (int)ChallengeDebufHelps.GetDebufTotalFirstParam(challengeDebuffs, ChallengeDebuffType.WalkerMoveLess);
							flag = true;
							if (moveRange >= (int)fixedPoint && num <= (int)fixedPoint)
							{
								num = (int)fixedPoint;
							}
							if (moveRange < (int)fixedPoint && num < (int)fixedPoint)
							{
								num = moveRange;
							}
						}
					}
				}
				if (base.manager != null && IsRangedClass)
				{
					FixedPoint citadel_MoveDown_Num = GetCitadel_MoveDown_Num();
					if (citadel_MoveDown_Num > 0L)
					{
						num = Math.Max(1, num - (int)citadel_MoveDown_Num);
					}
				}
				if (IsCrippled && !flag)
				{
					num = Math.Min(1, num);
				}
				if (TryGetBloodMarkMoveDistanceCap(out var moveDistanceCap) && moveDistanceCap > 0)
				{
					num = Math.Min(num, moveDistanceCap);
				}
				return num;
			}
			protected set
			{
				moveRange = value;
			}
		}

		public int MoveRangeConsumed { get; set; }

		public int ActivationRange { get; protected set; }

		public int StrugglesLeft { get; set; }

		public bool TurnConsumedByEatingLure { get; set; }

		public TimedEffect ExclusiveTimedEffect { get; set; }

		public ScorchTimedEffect ScorchTimedEffect { get; set; }

		public TimedEffect TauntTimedEffect { get; set; }

		public ShieldTimedEffect ShieldTimedEffect { get; set; }

		public int CarolNotAttackAndNotAttackedTurns { get; set; }

		public int ExtraBurnLayer { get; set; }

		public int ExtraBurnTurn { get; set; }

		public TimedEffect PendingExclusiveTimedEffect { get; set; }

		public CombatEndCondition CombatEndCondition { get; set; }

		public int BounsPhonePortraitTurn { get; set; }

		[JsonIgnore]
		public bool IsStruggling
		{
			get
			{
				if (ExclusiveTimedEffect != null && ExclusiveTimedEffect.Duration > 0)
				{
					return ExclusiveTimedEffect.Type == TimedEffectType.Struggle;
				}
				return false;
			}
		}

		[JsonIgnore]
		public bool IsBleedingOut
		{
			get
			{
				if (ExclusiveTimedEffect != null && ExclusiveTimedEffect.Duration > 0)
				{
					return ExclusiveTimedEffect.Type == TimedEffectType.BleedOut;
				}
				return false;
			}
		}

		[JsonIgnore]
		public bool IsStunned
		{
			get
			{
				if (ExclusiveTimedEffect != null && ExclusiveTimedEffect.Duration > 0)
				{
					return ExclusiveTimedEffect.Type == TimedEffectType.Stun;
				}
				return false;
			}
		}

		[JsonIgnore]
		public bool NotTriggeredRiposted
		{
			get
			{
				if (!IsStunned && !IsElectricShocked)
				{
					return IsFistSpike;
				}
				return true;
			}
		}

		[JsonIgnore]
		public bool IsSneak
		{
			get
			{
				FixedPoint value = 0.0;
				base.manager.Player.AbilityManager.VisitParameter("AbilityModifierCarolNoAttackTurn", ref value, this);
				bool num = HasAnyLevelTrait("LeaderBuffCriticalChance");
				bool flag = CarolNotAttackAndNotAttackedTurns >= value;
				if (num && flag)
				{
					return true;
				}
				return false;
			}
		}

		[JsonIgnore]
		public bool IsEatingLure
		{
			get
			{
				if (ExclusiveTimedEffect != null && ExclusiveTimedEffect.Duration > 0)
				{
					return ExclusiveTimedEffect.Type == TimedEffectType.EatingLure;
				}
				return false;
			}
		}

		[JsonIgnore]
		public bool IsInteractingWithObject
		{
			get
			{
				if (ExclusiveTimedEffect != null && ExclusiveTimedEffect.Duration > 0)
				{
					return ExclusiveTimedEffect.Type == TimedEffectType.InteractingWithObject;
				}
				return false;
			}
		}

		[JsonIgnore]
		public bool IsBurning => HasTrait("Burning");

		[JsonIgnore]
		public bool IsSkinned => HasTrait("Skinned");

		[JsonIgnore]
		public bool IsElectricShocked
		{
			get
			{
				if (ExclusiveTimedEffect != null && ExclusiveTimedEffect.Duration > 0)
				{
					return ExclusiveTimedEffect.Type == TimedEffectType.ElectricShock;
				}
				return false;
			}
		}

		[JsonIgnore]
		public bool IsScorching
		{
			get
			{
				if (ScorchTimedEffect != null)
				{
					if (ScorchTimedEffect.Duration - ScorchTimedEffect.Counter > 0)
					{
						return ScorchTimedEffect.Type == TimedEffectType.Scorch;
					}
					return false;
				}
				return false;
			}
		}

		[JsonIgnore]
		public bool IsQuantunCanNotMove
		{
			get
			{
				if (ExclusiveTimedEffect != null && ExclusiveTimedEffect.Duration > 0)
				{
					return ExclusiveTimedEffect.Type == TimedEffectType.QuantunCanNotMove;
				}
				return false;
			}
		}

		[JsonIgnore]
		public bool IsBleeding => HasTrait("Bleeding");

		[JsonIgnore]
		public bool IsStaggered => HasTrait("StaggerActive");

		[JsonIgnore]
		public bool IsRemoteWeakened => HasTrait("RemoteWeakenActiveFlag");

		[JsonIgnore]
		public bool HasPrincessStatusEffect
		{
			get
			{
				if (!IsRooted && !IsPitfalled && !IsStunned && !IsEatingLure && !IsHerded && !IsBurning && !IsBleeding && !IsCrippled && !IsStaggered && !IsDisoriented && !IsABTesterA2ed && !IsABTesterAed && !IsSkinned && !IsRemoteWeakened && GetAstheniaLeftTurns() <= 0 && !IsElectricShocked)
				{
					return IsQuantuned;
				}
				return true;
			}
		}

		[JsonIgnore]
		public bool IsChallengeApocalypseEffectDmgIncStatus
		{
			get
			{
				if (!IsStunned && !IsRooted && !IsCrippled)
				{
					return IsDisoriented;
				}
				return true;
			}
		}

		[JsonIgnore]
		public bool IsReloading
		{
			get
			{
				if (SelectedEquipment != null)
				{
					return SelectedEquipment.IsReloading;
				}
				return false;
			}
		}

		[JsonIgnore]
		public bool IsFistSpike => FistSpikeTurns > 0;

		[JsonIgnore]
		public bool IsDodgeShot
		{
			get
			{
				if (DodgeShotTurns > 0 && DodgeShotTimes > 0)
				{
					return true;
				}
				return false;
			}
		}

		[JsonIgnore]
		public bool IsRooted
		{
			get
			{
				if (ExclusiveTimedEffect != null && ExclusiveTimedEffect.Duration > 0)
				{
					return ExclusiveTimedEffect.Type == TimedEffectType.Root;
				}
				return false;
			}
		}

		[JsonIgnore]
		public bool IsSurvivalDashFlag
		{
			get
			{
				if (SurvivalDashFlagTurns > 0)
				{
					return true;
				}
				return false;
			}
		}

		[JsonIgnore]
		public bool IsRaiderDashFlag
		{
			get
			{
				if (RaiderDashFlagTurns > 0)
				{
					return true;
				}
				return false;
			}
		}

		[JsonIgnore]
		public int Hit
		{
			get
			{
				int num = base.manager.GameEconomyData.ConfigData.InitialHit;
				foreach (EquipmentItemModel equipmentItem in EquipmentItems)
				{
					num += equipmentItem.Hit;
				}
				return num;
			}
		}

		[JsonIgnore]
		public bool IsPitfalled
		{
			get
			{
				if (ExclusiveTimedEffect != null && ExclusiveTimedEffect.Duration > 0)
				{
					return ExclusiveTimedEffect.Type == TimedEffectType.Pitfall;
				}
				return false;
			}
		}

		[JsonIgnore]
		public bool IsCrippled
		{
			get
			{
				if (ExclusiveTimedEffect != null && ExclusiveTimedEffect.Duration > 0)
				{
					return ExclusiveTimedEffect.Type == TimedEffectType.Crippled;
				}
				return false;
			}
		}

		[JsonIgnore]
		public bool IsHerded
		{
			get
			{
				if (ExclusiveTimedEffect != null && ExclusiveTimedEffect.Duration > 0)
				{
					return ExclusiveTimedEffect.Type == TimedEffectType.Herd;
				}
				return false;
			}
		}

		[JsonIgnore]
		public bool IsABTesterAed
		{
			get
			{
				if (ExclusiveTimedEffect != null && ExclusiveTimedEffect.Duration > 0)
				{
					return ExclusiveTimedEffect.Type == TimedEffectType.ABTesterA;
				}
				return false;
			}
		}

		[JsonIgnore]
		public bool IsDisoriented
		{
			get
			{
				if (ExclusiveTimedEffect != null && ExclusiveTimedEffect.Duration > 0)
				{
					return ExclusiveTimedEffect.Type == TimedEffectType.Disorient;
				}
				return false;
			}
		}

		[JsonIgnore]
		public bool IsTaunted
		{
			get
			{
				if (TauntTimedEffect != null)
				{
					if (TauntTimedEffect.Duration - TauntTimedEffect.Counter > 0)
					{
						return TauntTimedEffect.Type == TimedEffectType.Taunt;
					}
					return false;
				}
				return false;
			}
		}

		[JsonIgnore]
		public bool IsProtectorDarylShielded
		{
			get
			{
				if (ShieldTimedEffect != null)
				{
					if (ShieldTimedEffect.Duration - ShieldTimedEffect.Counter > 0)
					{
						return ShieldTimedEffect.Type == TimedEffectType.Shield;
					}
					return false;
				}
				return false;
			}
		}

		[JsonIgnore]
		public bool IsDisorientedLock
		{
			get
			{
				if (ExclusiveTimedEffect != null && ExclusiveTimedEffect.Duration > 0)
				{
					return ExclusiveTimedEffect.Type == TimedEffectType.DisorientLock;
				}
				return false;
			}
		}

		[JsonIgnore]
		public bool IsLured
		{
			get
			{
				if (ExclusiveTimedEffect != null && ExclusiveTimedEffect.Duration > 0)
				{
					return ExclusiveTimedEffect.Type == TimedEffectType.Lure;
				}
				return false;
			}
		}

		[JsonIgnore]
		public bool IsEatingLured
		{
			get
			{
				if (ExclusiveTimedEffect != null && ExclusiveTimedEffect.Duration > 0)
				{
					return ExclusiveTimedEffect.Type == TimedEffectType.EatingLure;
				}
				return false;
			}
		}

		[JsonIgnore]
		public bool IsInvisible => HasTrait("Gore");

		[JsonIgnore]
		public bool IsCamouflaged => HasTrait("WalkerMikeActive");

		[JsonIgnore]
		public virtual bool IsRangedClass
		{
			get
			{
				if (!(Definition.Class == ClassAssault) && !(Definition.Class == ClassHunter))
				{
					return Definition.Class == ClassShooter;
				}
				return true;
			}
		}

		[JsonIgnore]
		public virtual bool IsMeleeClass
		{
			get
			{
				if (!(Definition.Class == ClassBruiser) && !(Definition.Class == ClassScout))
				{
					return Definition.Class == ClassWarrior;
				}
				return true;
			}
		}

		[JsonIgnore]
		public bool IsBossClass => Definition.Class == "Boss";

		public bool IsVisibleToSurvivors { get; set; }

		public bool HasGainedExtraAP { get; set; }

		public bool HasGainedExtraMoveAp { get; set; }

		public bool HasUsedExtraAP { get; set; }

		public bool VisitedExtraApChance { get; set; }

		public bool VisitedRedactChance { get; set; }

		public bool EnsureExtraAP { get; set; }

		public bool EnsureGainedExtraMoveAp { get; set; }

		public string ExtraMoveApNotificationKey { get; set; }

		public bool GainedChargePointOnMove { get; set; }

		public bool HasGainedExtraAPFromInteraction { get; set; }

		public bool TacticalResupplyMagazineNextDragLineCritPending { get; set; }

		public bool HasPerformedOOT { get; set; }

		public OOTType LastOOT { get; set; }

		public bool IsTurnConsumedOutOfFaction { get; set; }

		public bool CanBenefitFromStaggerInstantly { get; set; }

		public bool CanReceiveChargePointFromStagger { get; set; }

		public int DamageCount { get; set; }

		[JsonIgnore]
		public bool CanPerformOOT
		{
			get
			{
				if (!IsDead && Faction != Faction.Dormant && Faction != Faction.Lure && AIController.CanPerformOOT && !AIController.IsActorIncapacitated && !HasPerformedOOT && (ExclusiveTimedEffect == null || ExclusiveTimedEffect.Type == TimedEffectType.Root) && (SelectedEquipment == null || !SelectedEquipment.NeedsReloading) && base.manager.CombatModel.TurnManager.ActiveFaction != Faction && !IsInvisible)
				{
					if (SelectedEquipment != null)
					{
						return !SelectedEquipment.LimitOOT;
					}
					return false;
				}
				return false;
			}
		}

		[JsonIgnore]
		public bool CanReceiveOOT
		{
			get
			{
				if (!HasTrait("TutorialUninterruptable"))
				{
					return !IsCamouflaged;
				}
				return false;
			}
		}

		[JsonIgnore]
		public bool CanStoreLeftOverAP
		{
			get
			{
				if (!IsDead && !IsWalker && Faction != Faction.Lure && !AIController.IsActorIncapacitated && AIController.AIDataModel.Alertness > AIAlertness.Wandering && (ExclusiveTimedEffect == null || ExclusiveTimedEffect.Type == TimedEffectType.Root) && !AbilityCompleted)
				{
					return !SecondMoveCompleted;
				}
				return false;
			}
		}

		[JsonIgnore]
		public bool BloodThirst
		{
			get
			{
				return bloodThirst;
			}
			set
			{
				if (HasTrait("BloodThirst"))
				{
					bloodThirst = value;
				}
			}
		}

		public int KillsInTurn { get; set; }

		public bool KilledByLevelDifference { get; set; }

		public int HitsInTurn { get; set; }

		[IgnoreModelProperty]
		public ActorModel LastHitAttacker { get; set; }

		[IgnoreModelProperty]
		public ExplosiveModel LastHitExplosive { get; set; }

		[IgnoreModelProperty]
		public ProducerModel Producer { get; set; }

		public int MaxShieldHitPoints { get; set; }

		public int ShieldHitPoints { get; set; }

		public int MaxHitPoints { get; set; }

		public bool IsSegmentedHP { get; private set; }

		public int SegmentedHPCount { get; private set; }

		public int SegmentedHPMax { get; private set; }

		public int GuildBossDefense { get; protected set; }

		[JsonIgnore]
		public int TotalMaxHitPoints
		{
			get
			{
				if (!IsSegmentedHP)
				{
					return MaxHitPoints;
				}
				return MaxHitPoints * SegmentedHPMax;
			}
		}

		public int Hitpoints { get; private set; }

		public int MinHitpoints { get; set; }

		public string CharacterPrefab { get; set; }

		public string OutfitDefinitionID { get; set; }

		public int Level { get; set; }

		public int ActorTag { get; set; }

		public ActorGender Gender { get; set; }

		public MissionFailCondition MissionFailCondition { get; set; }

		[JsonIgnore]
		public virtual string Name
		{
			get
			{
				if (base.manager == null || base.manager.GameEconomyData == null)
				{
					return "unknown";
				}
				return Definition.Name;
			}
		}

		[JsonIgnore]
		public virtual string FullName
		{
			get
			{
				if (base.manager == null || base.manager.GameEconomyData == null)
				{
					return "";
				}
				return Definition.FullName;
			}
		}

		[JsonIgnore]
		public string DebugInfo => Name + " [ModelID = " + base.ModelId + "]";

		[JsonIgnore]
		public ModelList<AbilityModel> Abilities { get; private set; }

		[JsonIgnore]
		public AbilityModel SelectedAbility
		{
			get
			{
				if (SelectedEquipment != null)
				{
					return SelectedEquipment.Ability;
				}
				return null;
			}
		}

		[JsonIgnore]
		public ModelList<CombatArea> FallInCombatAreas { get; set; }

		[JsonIgnore]
		public PitfallArea InWorkingPitfallArea { get; set; }

		public bool CivilianCanStruggle { get; set; }

		[IgnoreModelProperty]
		public EquipmentItemModel SelectedEquipment { get; private set; }

		[IgnoreModelProperty]
		public ModelList<EquipmentItemModel> EquipmentItems { get; set; }

		public ActorTraitContainerModel TraitContainer { get; private set; }

		public List<ModelModifier> LeaderTraitModifiers { get; private set; }

		public bool OnRedHealthBar
		{
			get
			{
				return _onRedHealthBar;
			}
			set
			{
				_onRedHealthBar = value && !(this is TankActorModel);
			}
		}

		public int NumberOfEnemiesAttacked { get; set; }

		[JsonIgnore]
		public bool CanCollectProduction
		{
			get
			{
				if (Producer == null)
				{
					return false;
				}
				return Producer.HasEnoughToCollect;
			}
		}

		[JsonIgnore]
		public bool IsImmuneToFistSpike
		{
			get
			{
				if (ParryRiposteIncreaseStorey > 0)
				{
					return true;
				}
				return false;
			}
		}

		[JsonIgnore]
		public bool IsImmuneToCurrentSurge
		{
			get
			{
				if (ParryRiposteIncreaseStorey > 0)
				{
					return true;
				}
				return false;
			}
		}

		[JsonIgnore]
		public bool IsImmuneToStun
		{
			get
			{
				if (ParryRiposteIncreaseStorey > 0)
				{
					return true;
				}
				foreach (AbilityModel ability in Abilities)
				{
					if (ability.DefinitionID == "PassiveAbilityAvoidStun")
					{
						return true;
					}
				}
				return false;
			}
		}

		[JsonIgnore]
		public bool TurnComplete
		{
			get
			{
				if (TurnState != TurnState.Completed && (!AbilityCompleted || AllowSecondMoveAfterAbility) && !SecondMoveCompleted)
				{
					return IsDead;
				}
				return true;
			}
		}

		public bool HadActionPointsAtEndOfTurn { get; set; }

		public bool MoveCompleted { get; set; }

		public bool SecondMoveCompleted { get; set; }

		public bool AbilityCompleted { get; set; }

		public bool AllowSecondMoveAfterAbility { get; set; }

		public int AdditionalMoveRange
		{
			get
			{
				return _additionalMoveRange;
			}
			set
			{
				if (!FocusModeState)
				{
					_additionalMoveRange = value;
				}
			}
		}

		public int GivenAdditionalAttacks { get; set; }

		public int AdditionalAttackCount { get; set; }

		public int FightingFuryTargetCount { get; set; }

		public bool UsedToolThisTurn { get; set; }

		public bool UsedChargeAttackThisTurn { get; set; }

		public int EquipmentActiveKingFactor { get; set; }

		public GridCoordinate MainTargetCell { get; set; }

		public int RandomStatusNumberOfAttack
		{
			get
			{
				return _randomStatusNumberOfAttack;
			}
			set
			{
				_randomStatusNumberOfAttack = value;
			}
		}

		public string RandomStatusTraitIdentifier { get; set; }

		[IgnoreModelProperty]
		public ActorModel DisorientLockActor { get; set; }

		public bool IsRecoilEffected
		{
			get
			{
				return isRecoilEffected;
			}
			set
			{
				isRecoilEffected = value;
			}
		}

		[JsonIgnore]
		public bool IsABTesterA2ed => HasTrait("ABTesterA2Active");

		public int OverloadStatusLeftTurns
		{
			get
			{
				return overloadStatusLeftTurns;
			}
			set
			{
				overloadStatusLeftTurns = value;
			}
		}

		public int OverloadStatusEXAttackTimesInTurn
		{
			get
			{
				return overloadStatusEXAttackTimesInTurn;
			}
			set
			{
				overloadStatusEXAttackTimesInTurn = value;
			}
		}

		public int SurvivalGameLeftCD { get; set; }

		public FixedPoint DeadlyFocus_TotalEXDamageMultiplier
		{
			get
			{
				if (base.manager == null || base.manager.CombatModel == null)
				{
					return 0.0;
				}
				ActorModel leaderBuffDeadlyFocusMan = CombatHelpers.GetLeaderBuffDeadlyFocusMan(base.manager.CombatModel, Faction);
				if (leaderBuffDeadlyFocusMan == null)
				{
					return 0.0;
				}
				FixedPoint value = 0.0;
				base.manager.Player.AbilityManager.VisitParameter("LeaderBuffDeadlyFocus_DmgUpPerKill", ref value, leaderBuffDeadlyFocusMan);
				FixedPoint value2 = 0.0;
				base.manager.Player.AbilityManager.VisitParameter("LeaderBuffDeadlyFocus_DmgUpPerKill_Max", ref value2, leaderBuffDeadlyFocusMan);
				if (DeadlyFocus_EXDamageLayerCount > value2)
				{
					DeadlyFocus_EXDamageLayerCount = (int)value2;
				}
				return value * DeadlyFocus_EXDamageLayerCount;
			}
		}

		public FixedPoint ChargeNum
		{
			get
			{
				return chargeNumVal;
			}
			set
			{
				FixedPoint fixedPoint = value;
				if (fixedPoint < 0L)
				{
					fixedPoint = 0L;
				}
				if (chargeNumVal != fixedPoint)
				{
					chargeNumVal = fixedPoint;
				}
			}
		}

		public int BaseRage { get; set; }

		public FixedPoint ChargeConvertDamageBonus { get; set; }

		public FixedPoint ChargeConvertCritDamageBonus { get; set; }

		[JsonIgnore]
		public int TotalRage
		{
			get
			{
				int num = BaseRage;
				if (BerserkRageTimedEffect != null)
				{
					num += BerserkRageTimedEffect.Layer;
				}
				return num;
			}
		}

		public int ShadowedGuard_LeftCount { get; set; }

		[JsonIgnore]
		public GuardianVowBinding GuardianVowBindingAsGuardian => base.manager?.CombatModel?.GetGuardianVowBindingByGuardian(this);

		[JsonIgnore]
		public GuardianVowBinding GuardianVowBindingAsSovereign => base.manager?.CombatModel?.GetGuardianVowBindingBySovereign(this);

		[JsonIgnore]
		public ActorModel GuardianVowSovereign
		{
			get
			{
				GuardianVowBinding guardianVowBindingAsGuardian = GuardianVowBindingAsGuardian;
				if (guardianVowBindingAsGuardian == null)
				{
					return null;
				}
				return base.manager.CombatModel.GetActorByActorDefinitionID(guardianVowBindingAsGuardian.SovereignActorDefinitionID);
			}
		}

		[JsonIgnore]
		public ActorModel GuardianVowGuardian
		{
			get
			{
				GuardianVowBinding guardianVowBindingAsSovereign = GuardianVowBindingAsSovereign;
				if (guardianVowBindingAsSovereign == null)
				{
					return null;
				}
				return base.manager.CombatModel.GetActorByActorDefinitionID(guardianVowBindingAsSovereign.GuardianActorDefinitionID);
			}
		}

		[JsonIgnore]
		public int GuardianVow_LeftTurns => GuardianVowBindingAsGuardian?.LeftTurns ?? 0;

		[JsonIgnore]
		public int GuardianVow_PursuitTriggeredCount => GuardianVowBindingAsGuardian?.PursuitTriggeredCount ?? 0;

		[JsonIgnore]
		public int GuardianVow_ChargeAttackMaxTimes => GuardianVowBindingAsSovereign?.ChargeAttackMaxTimes ?? 0;

		[JsonIgnore]
		public bool IsGuardianVowActive
		{
			get
			{
				GuardianVowBinding guardianVowBindingAsGuardian = GuardianVowBindingAsGuardian;
				if (guardianVowBindingAsGuardian == null)
				{
					return false;
				}
				if (guardianVowBindingAsGuardian.LeftTurns <= 0)
				{
					return false;
				}
				if (IsDead)
				{
					return false;
				}
				ActorModel actorByActorDefinitionID = base.manager.CombatModel.GetActorByActorDefinitionID(guardianVowBindingAsGuardian.SovereignActorDefinitionID);
				if (actorByActorDefinitionID == null || actorByActorDefinitionID.IsDead)
				{
					return false;
				}
				return true;
			}
		}

		public int VengefulChargeAPNum_Turns { get; set; }

		public int VengefulChargeNums { get; set; }

		public int LeaderBuffShadowedVengefulChargeNums { get; set; }

		[JsonIgnore]
		public int TotalVengefulChargeNums => VengefulChargeNums + LeaderBuffShadowedVengefulChargeNums;

		[JsonIgnore]
		public bool IsCitadelLeaderBuff => HasAnyLevelTrait("LeaderBuffCitadel");

		[JsonIgnore]
		public bool IsCitadelBeEffected
		{
			get
			{
				if (CitadelLastTurnAddedTraits == null || CitadelLastTurnAddedTraits.Count == 0)
				{
					return false;
				}
				for (int i = 0; i < CitadelLastTurnAddedTraits.Count; i++)
				{
					string traitIdentifier = CitadelLastTurnAddedTraits[i];
					if (HasTrait(traitIdentifier) || HasAnyLevelTrait(traitIdentifier))
					{
						return true;
					}
				}
				return false;
			}
		}

		public int DeathsDoor_DmgUpLayer { get; set; }

		public int DeathsDoor_DmgUpLeftTurns { get; set; }

		public int DeathsDoor_DmgUpLayerGainedThisAttack { get; set; }

		public int DeathsDoor_PursuitCount { get; set; }

		public bool DeathsDoor_IsPursuitAttack { get; set; }

		public bool DeathsBlockSecondChance { get; set; }

		public List<string> SupportTalentAssembledTraitRecords { get; set; }

		public FixedPoint GetSnapshotCombatAttributeValueByAttributeType(AttributeType attributeType)
		{
			if (CombatAttributeSnapshots != null && CombatAttributeSnapshots.TryGetValue(attributeType, out var value))
			{
				return value;
			}
			return 0.0;
		}

		public bool IsMomentum()
		{
			return MomentumTimedEffect != null;
		}

		public bool IsShieldBreaker()
		{
			return ShieldBreakerTimedEffect != null;
		}

		public bool IsDebuffDamagePerRound()
		{
			return DebuffDamagePerRoundTimedEffect != null;
		}

		public bool IsDebuffReduceRecovery()
		{
			return DebuffReduceRecoveryTimedEffect != null;
		}

		public bool IsBerserkRaged()
		{
			return BerserkRageTimedEffect != null;
		}

		public void StartFortifications(int turns, int sourceSkillID, List<string> grantedTraitIds)
		{
			if (turns > 0)
			{
				StartTimedEffect(new FortificationsTimedEffect(turns, this, sourceSkillID, grantedTraitIds));
			}
		}

		public void EndFortifications(bool interrupted, int cooldownOverride = -1)
		{
			FortificationsTimedEffect fortificationsTimedEffect = FortificationsTimedEffect;
			if (fortificationsTimedEffect != null)
			{
				if (interrupted)
				{
					fortificationsTimedEffect.MarkInterrupted();
				}
				if (cooldownOverride >= 0)
				{
					fortificationsTimedEffect.SetCooldownOverride(cooldownOverride);
				}
				CoexistTimedEffectsManager?.RemoveCoexistTimedEffectByCoexistTimedEffectTypeList(new List<CoexistTimedEffectType> { CoexistTimedEffectType.Fortifications });
			}
		}

		public virtual List<GridCoordinate> GetOccupiedCells()
		{
			return new List<GridCoordinate> { GridCoordinate };
		}

		public virtual List<GridCoordinate> GetOccupiedCellsAt(GridCoordinate anchor)
		{
			return new List<GridCoordinate> { anchor };
		}

		public virtual GridCoordinate GetAttackOriginCell()
		{
			return GridCoordinate;
		}

		public virtual GridCoordinate GetClosestOccupiedCell(GridCoordinate from)
		{
			return GridCoordinate;
		}

		public void SetIsRandomActiveLightState(bool state)
		{
			_isRandomActiveLightState = state;
		}

		public bool GetIsRandomActiveLightState()
		{
			return _isRandomActiveLightState;
		}

		public void SetActiveLightState(bool state)
		{
			_equipmentActiveLightState = state;
		}

		public bool GetActiveLightState()
		{
			return _equipmentActiveLightState;
		}

		public void ResetActiveLight()
		{
			_isRandomActiveLightState = false;
			_equipmentActiveLightState = false;
		}

		public void SetFreeOWState(bool state)
		{
			_equipmentFreeOWState = state;
		}

		public bool GetFreeOWState()
		{
			return _equipmentFreeOWState;
		}

		public void ResetFreeOW()
		{
			_equipmentFreeOWState = false;
		}

		public static SurvivorClass ParseSurvivorClassOrNone(string className)
		{
			if (!Enum.TryParse<SurvivorClass>(className, out var result))
			{
				return SurvivorClass.None;
			}
			return result;
		}

		public SurvivorClass GetSurvivorClassOrNone()
		{
			return ParseSurvivorClassOrNone(Definition.Class);
		}

		public void ChangeShieldHitPoints(int val)
		{
			int shieldHitPoints = ShieldHitPoints;
			MaxShieldHitPoints += val;
			if (val > 0)
			{
				ShieldHitPoints += val;
			}
			else
			{
				int num = ShieldHitPoints + val;
				if (num <= 0)
				{
					num = 0;
				}
				ShieldHitPoints = Math.Min(num, MaxShieldHitPoints);
			}
			NotifyChange("ShieldChanged");
			if (shieldHitPoints > ShieldHitPoints)
			{
				NotifyReducedShieldHitPoints();
			}
		}

		private void NotifyReducedShieldHitPoints()
		{
			VengefulCharge_dmg();
		}

		public void RestoreSegmentedHpState(bool isSegmentedHP, int segmentedHPMax, int segmentedHPCount, int guildBossDefense)
		{
			IsSegmentedHP = isSegmentedHP;
			SegmentedHPMax = segmentedHPMax;
			SegmentedHPCount = segmentedHPCount;
			GuildBossDefense = guildBossDefense;
		}

		public void ChangeHitpoints(int val)
		{
			int val2 = Hitpoints + val;
			SetHitpoints(val2);
		}

		public int UpdateParryRiposteIncreaseStorey(int num)
		{
			ParryRiposteIncreaseStorey = Math.Max(0, ParryRiposteIncreaseStorey + num);
			FixedPoint value = 0.0;
			if (HasTraitsThatContains("Riposte"))
			{
				base.manager.Player.AbilityManager.VisitParameter("AbilityModifierRippedAdditionalPRMaxStorey", ref value, this);
			}
			if (ParryRiposteIncreaseStorey > value)
			{
				ParryRiposteIncreaseStorey = (int)value;
			}
			return ParryRiposteIncreaseStorey;
		}

		public void ResetParryRiposteIncreaseStorey()
		{
			ParryRiposteIncreaseStorey = 0;
		}

		public void SetHitpoints(int val, DefenseSystemType useDefenseSystemType = DefenseSystemType.None, bool IsDealShield = true, ChangeHitPointSource source = ChangeHitPointSource.None)
		{
			int num = val - Hitpoints;
			if (IsDealShield && num < 0 && ShieldHitPoints > 0)
			{
				int shieldHitPoints = ShieldHitPoints;
				ShieldHitPoints = Math.Max(0, ShieldHitPoints + num);
				if (shieldHitPoints > ShieldHitPoints)
				{
					NotifyReducedShieldHitPoints();
				}
				NotifyChange("ShieldChanged");
				if (ShieldHitPoints <= 0 && source == ChangeHitPointSource.DealDamage && HasAnyLevelTrait("Equipment_Passive_DefendingHeart") && DefendingHeartTraitCDTurns <= 0)
				{
					FixedPoint value = 0L;
					FixedPoint value2 = 0L;
					base.manager.Player.AbilityManager.VisitParameter("Equipment_Passive_DefendingHeartCD", ref value, this);
					base.manager.Player.AbilityManager.VisitParameter("Equipment_Passive_DefendingHeartTurns", ref value2, this);
					DefendingHeartTraitCDTurns = (int)value;
					DefendingHeartTraitEffectLeftTurns = (int)value2;
				}
			}
			else
			{
				Hitpoints = val;
			}
		}

		public void HealUpHitpoint(int hp)
		{
			Hitpoints = hp;
		}

		public void BackUpHitpoint(int hp)
		{
			Hitpoints = hp;
		}

		public ActorModel()
		{
			EquipmentItems = new ModelList<EquipmentItemModel>();
			Abilities = new ModelList<AbilityModel>();
			FallInCombatAreas = new ModelList<CombatArea>();
			UserCanControl = true;
			CivilianCanStruggle = false;
		}

		public override void Initialize()
		{
			base.Initialize();
			UserCanControl = true;
			AIDataModel = new AIDataModel();
			AIDataModel.SetManager(base.manager);
			AIDataModel.Initialize();
			Abilities.SetManager(base.manager);
			TraitContainer = new ActorTraitContainerModel();
			TraitContainer.SetManager(base.manager);
			TraitContainer.Initialize();
			ChargeMeter = new ChargeMeterModel(this);
			ChargeMeter.SetManager(base.manager);
			ChargeMeter.Initialize();
			DebuffParameterManager = new ActorDebuffParameterManager();
			DebuffParameterManager.SetManager(base.manager);
			DebuffParameterManager.Initialize();
			CoexistTimedEffectsManager = new CoexistTimedEffectsManager();
			CoexistTimedEffectsManager.SetManager(base.manager);
			CoexistTimedEffectsManager.Initialize();
			ActorAttributeContainer = new ActorAttributeContainerModel();
			ActorAttributeContainer.SetManager(base.manager);
			ActorAttributeContainer.Initialize();
			CommandSkillModelManager = new CommandSkillModelManager();
			CommandSkillModelManager.SetOwnActorModel(this);
			CommandSkillModelManager.SetManager(base.manager);
			CommandSkillModelManager.Initialize();
			AsTargetAttackChainSlots = new List<int>();
			AttackChainStaus = null;
			FortunaRandomTraitIds = new List<string>();
			CombatAttributeSnapshots = new Dictionary<AttributeType, FixedPoint>();
			SupportTalentAssembledTraitRecords = new List<string>();
			LeaderTraitModifiers = new List<ModelModifier>();
			SPAddPassiveAllSlotsTraits = new List<string>();
			UndyingState = new UndyingState();
			InitActor(TurnState.Idle);
		}

		protected void InitActor(TurnState initialState)
		{
			TurnState = initialState;
			IsVisibleToSurvivors = true;
			BloodThirst = false;
			ResetActionPoints();
			InitializeAttributes();
		}

		public void SetUserCanControl(bool value, string reason = null)
		{
			UserCanControl = value;
			if (!value)
			{
				UserCanControlFalseReason = reason ?? "";
			}
			NotifyChange("actorUserCanControlChanged");
		}

		public virtual void CombatCleanup()
		{
			AIDataModel.Clear();
			ChargeMeter.Reset();
			ResetAllRageStates();
			ExclusiveTimedEffect = null;
			PendingExclusiveTimedEffect = null;
			ScorchTimedEffect = null;
			TauntTimedEffect = null;
			CarolNotAttackAndNotAttackedTurns = 0;
			ExtraBurnLayer = 0;
			ExtraBurnTurn = 0;
			IsAttackAndBeAttacked = false;
			FinishShieldTimedEffect();
			CombatCleanSkillShieldType1TimedEffect();
			CombatCleanSkillEquipTauntTimedEffect();
			RemoveTrait("Burning");
			RemoveTrait("StaggerActive");
			RemoveTrait("ABTesterA2Active");
			RemoveTrait("Skinned");
			RemoveTrait("RemoteWeakenActiveFlag");
			LastHitAttacker = null;
			LastHitExplosive = null;
			UserCanControl = true;
			timedEffectEnding = false;
			HadActionPointsAtEndOfTurn = false;
			HasGainedExtraAP = false;
			HasGainedExtraMoveAp = false;
			VisitedExtraApChance = false;
			VisitedRedactChance = false;
			EnsureExtraAP = false;
			EnsureGainedExtraMoveAp = false;
			HasGainedExtraAPFromInteraction = false;
			IsExploding = false;
			HasBeenSaved = false;
			SavedOnTurnIndex = -1;
			RevengedOnTurn = false;
			ShieldRevengedTimesOnTurn = 0;
			OverwatchedOnTurn = false;
			PreAttackedOnTurn = false;
			ChargeAttackWithFreeShootingTriggeredCount = 0;
			FightBackTimesThisRound = 0;
			PreAttackedOnRiposte = false;
			PassByAttackedOnMove = false;
			AttackKilledAnyEnemy = false;
			AttackHasNotKilledAllEnemies = false;
			FollowUpAttackedOnTurn = false;
			OnRedHealthBar = false;
			NumberChargePointAtStart = 0;
			DebuffRemoteRepulseWeakenAddChargePointPercentage = 0.0;
			DebuffRemoteRepulseWeakenAddChargePoints = 0;
			UsedToolThisTurn = false;
			UsedChargeAttackThisTurn = false;
			_isRandomActiveLightState = false;
			_equipmentActiveLightState = false;
			_equipmentFreeOWState = false;
			ActorsLastAbilityCell = GridCoordinate.Invalid;
			TurnState = TurnState.Idle;
			ResetActionPoints();
			ResetFocusMode();
			FistSpikeTurns = 0;
			DodgeShotTurns = 0;
			DodgeShotTimes = 0;
			IsMoving = false;
			AsTargetAttackChainSlots = new List<int>();
			AttackChainStaus = null;
			UndyingState = new UndyingState();
			if (FortunaRandomTraitIds != null && FortunaRandomTraitIds.Count > 0)
			{
				foreach (string fortunaRandomTraitId in FortunaRandomTraitIds)
				{
					RemoveTrait(fortunaRandomTraitId);
				}
				FortunaRandomTraitIds = new List<string>();
			}
			CombatAttributeSnapshots = new Dictionary<AttributeType, FixedPoint>();
			if (SupportTalentAssembledTraitRecords != null && SupportTalentAssembledTraitRecords.Count > 0)
			{
				foreach (string supportTalentAssembledTraitRecord in SupportTalentAssembledTraitRecords)
				{
					RemoveTrait(supportTalentAssembledTraitRecord);
				}
				SupportTalentAssembledTraitRecords = new List<string>();
			}
			ActorAttributeContainer?.SetSupportModel(null);
			ResetParryRiposteIncreaseStorey();
			NextCanTriggerFirstAidTurn = 0;
			NextReadyEquipmentPassiveRemoveNegativeEndTurn = 0;
			NextReadyEquipmentPassiveRemoveNegativeStartTurn = 0;
			NextReadyEquipmentPassiveRemoveNegativeOwnTurn = 0;
			NextCanTriggerPassOW = 0;
			SurvivalDashFlagTurns = 0;
			PastaTurns = 0;
			PastaCurrentTurn = false;
			CapFirstAttack = false;
			CapFirstHeal = false;
			UnluckyFlagTurns = 0;
			IsTriggerPassOW = false;
			RaiderDashFlagTurns = 0;
			DefendingHeartTraitCDTurns = 0;
			DefendingHeartTraitEffectLeftTurns = 0;
			DebuffStatusRemoveTurns = 0;
			GodWarTraitTurns = 0;
			SupportTalent_NoMoveHitrateFlag = false;
			SupportTalent_NoMoveCritRateFlag = false;
			BlindLeftTurns = 0;
			BlindDecreaseRate = 0L;
			ShadowedGuard_DelHP = 0;
			ShadowedGuard_Atk = 0;
			ShadowedGuard_LeftCount = 0;
			ResetGuardianVowState();
			ChargeNum = 0L;
			ClearRandomStatusNumberOfAttacks();
			DebuffParameterManager?.ClearDebuffParameters();
			CoexistTimedEffectsManager?.ClearCoexistTimedEffects();
			CommandSkillModelManager?.ClearCommandSkills();
			SelectedEquipment = null;
			if (base.manager != null && Definition != null && Definition.PvPTraits != null)
			{
				foreach (string pvPTrait in Definition.PvPTraits)
				{
					RemoveTrait(pvPTrait);
				}
			}
			RemoveTemporaryTraits();
			UpdateModelObjects();
			CleanLeaderBuffDeathsDoor();
		}

		public override void Start()
		{
			foreach (EquipmentItemModel model in EquipmentItems.Models)
			{
				model.Owner = this;
			}
			if ((IsHumanNPC || Faction == Faction.Lure) && EquipmentItems != null && EquipmentItems.Models != null)
			{
				foreach (EquipmentItemModel model2 in EquipmentItems.Models)
				{
					model2.SetManager(base.manager);
					model2.Start();
				}
			}
			base.Start();
			int count = EquipmentItems.Count;
			for (int i = 0; i < count; i++)
			{
				EquipmentItemModel equipmentItemModel = EquipmentItems[i];
				if (equipmentItemModel.manager == null)
				{
					throw new Exception("No manager on item " + equipmentItemModel.EquipmentDefinitionIdentifier + " " + equipmentItemModel.ModelId + " on " + Definition.Name + " faction " + Faction.ToString() + " " + Name);
				}
			}
			Modifiers = new ModifierCollection();
			Modifiers.SetManager(base.manager);
			Modifiers.Initialize();
			Modifiers.Start();
			if (AttributeModel == null)
			{
				AttributeModel = new AttributeModel();
				AttributeModel.SetManager(base.manager);
				AttributeModel.Initialize();
				AttributeModel.Start();
			}
			if (DebuffParameterManager == null)
			{
				DebuffParameterManager = new ActorDebuffParameterManager();
				DebuffParameterManager.SetManager(base.manager);
				DebuffParameterManager.Initialize();
				DebuffParameterManager.Start();
			}
			if (ActorAttributeContainer == null)
			{
				ActorAttributeContainer = new ActorAttributeContainerModel();
				ActorAttributeContainer.SetManager(base.manager);
				ActorAttributeContainer.Initialize();
				ActorAttributeContainer.Start();
			}
			if (CoexistTimedEffectsManager == null)
			{
				CoexistTimedEffectsManager = new CoexistTimedEffectsManager();
				CoexistTimedEffectsManager.SetManager(base.manager);
				CoexistTimedEffectsManager.Initialize();
				CoexistTimedEffectsManager.Start();
			}
			if (CommandSkillModelManager == null)
			{
				CommandSkillModelManager = new CommandSkillModelManager();
				CommandSkillModelManager.SetOwnActorModel(this);
				CommandSkillModelManager.SetManager(base.manager);
				CommandSkillModelManager.Initialize();
				CommandSkillModelManager.Start();
			}
			else
			{
				CommandSkillModelManager.SetOwnActorModel(this);
			}
			if (AsTargetAttackChainSlots == null)
			{
				AsTargetAttackChainSlots = new List<int>();
			}
			if (FortunaRandomTraitIds == null)
			{
				FortunaRandomTraitIds = new List<string>();
			}
			if (CombatAttributeSnapshots == null)
			{
				CombatAttributeSnapshots = new Dictionary<AttributeType, FixedPoint>();
			}
			if (SupportTalentAssembledTraitRecords == null)
			{
				SupportTalentAssembledTraitRecords = new List<string>();
			}
			if (UndyingState == null)
			{
				UndyingState = new UndyingState();
			}
			UpdateModelObjects();
			CreateAbilities();
			SetupTraits();
			AIController = CreateAIController();
			timedEffectEnding = false;
			if (!IsDead)
			{
				ConfigureBaseAttributes();
			}
		}

		public void SetProducerFromTrait(TraitDefinition traitDefinition = null)
		{
			if (traitDefinition == null)
			{
				traitDefinition = GetTraitWithTag("ResourceProd");
			}
			if (traitDefinition == null || !traitDefinition.HasTag("ResourceProd"))
			{
				return;
			}
			CurrencyType parameter = traitDefinition.GetParameter<CurrencyType>(0);
			int parameter2 = traitDefinition.GetParameter<int>(1);
			int parameter3 = traitDefinition.GetParameter<int>(2);
			if (Producer == null)
			{
				Producer = new ProducerModel(parameter);
				Producer.SetManager(base.manager);
				Producer.Initialize();
				if (base.manager.IsStarted)
				{
					Producer.Start();
				}
			}
			Producer.SetRate(parameter2);
			Producer.SetCapacity(parameter3);
			UpdateModelObjects();
		}

		public TWDModelResult CollectProduction()
		{
			if (Producer != null && CanCollectProduction && Producer.Collect() > 0)
			{
				NotifyChange("collected");
				return TWDModelResult.OK;
			}
			return TWDModelResult.Error;
		}

		public EquipmentItemModel[] GetEquipment()
		{
			return EquipmentItems.Models.ToArray();
		}

		public EquipmentItemModel GetEquipmentOfCategory(EquipmentCategory equipmentCategory)
		{
			int count = EquipmentItems.Count;
			for (int i = 0; i < count; i++)
			{
				EquipmentItemModel equipmentItemModel = EquipmentItems[i];
				if (equipmentItemModel.Definition.Category == equipmentCategory)
				{
					return equipmentItemModel;
				}
			}
			return null;
		}

		public EquipmentItemModel GetEquipmentWithAbility(AbilityModel ability)
		{
			int count = EquipmentItems.Count;
			for (int i = 0; i < count; i++)
			{
				EquipmentItemModel equipmentItemModel = EquipmentItems[i];
				if (equipmentItemModel.Ability == ability)
				{
					return equipmentItemModel;
				}
			}
			return null;
		}

		public AbilityModel GetSpecialAbilityForEquipment(EquipmentItemModel equipment)
		{
			EquipmentType type = equipment.Definition.Type;
			int count = Abilities.Count;
			for (int i = 0; i < count; i++)
			{
				AbilityModel abilityModel = Abilities[i];
				if (abilityModel.Definition.Type == AbilityType.Active && abilityModel.IsEquipmentAllowed(type))
				{
					return abilityModel;
				}
			}
			return null;
		}

		public EquipmentItemModel GetWeaponEquipment()
		{
			if (EquipmentItems != null)
			{
				for (int i = 0; i < EquipmentItems.Count; i++)
				{
					EquipmentItemModel equipmentItemModel = EquipmentItems[i];
					if (equipmentItemModel.IsWeaponEquipment)
					{
						return equipmentItemModel;
					}
				}
			}
			return null;
		}

		public EquipmentItemModel GetEquipmentByType(bool isWeapon)
		{
			if (EquipmentItems != null)
			{
				foreach (EquipmentItemModel equipmentItem in EquipmentItems)
				{
					if (equipmentItem.IsWeaponEquipment == isWeapon)
					{
						return equipmentItem;
					}
				}
			}
			return null;
		}

		public EquipmentItemModel GetConsumableEquipment()
		{
			if (EquipmentItems != null)
			{
				for (int i = 0; i < EquipmentItems.Count; i++)
				{
					EquipmentItemModel equipmentItemModel = EquipmentItems[i];
					if (equipmentItemModel.Definition.Category == EquipmentCategory.Utility)
					{
						return equipmentItemModel;
					}
				}
			}
			return null;
		}

		public EquipmentItemModel GetChargeEquipment()
		{
			return GetWeaponEquipment()?.ChargeEquipment;
		}

		public TWDModelResult Equip(EquipmentItemModel equipmentItem, bool forceEquip = false, bool preview = false)
		{
			ActorModel owner = equipmentItem.Owner;
			if (!forceEquip && !CanEquip(equipmentItem))
			{
				return TWDModelResult.Error;
			}
			if (equipmentItem.Owner != null)
			{
				if (equipmentItem.IsConsumable)
				{
					equipmentItem.Owner.UnequipConsumableEquipment();
				}
				else
				{
					EquipmentItemModel equipmentByType = GetEquipmentByType(equipmentItem.IsWeaponEquipment);
					if (owner != null && equipmentByType != null && owner.CanEquip(equipmentByType))
					{
						equipmentItem.Owner.Unequip(equipmentItem);
					}
				}
			}
			if (EquipmentItems.Count > 0)
			{
				List<EquipmentItemModel> list = new List<EquipmentItemModel>();
				for (int i = 0; i < EquipmentItems.Count; i++)
				{
					EquipmentItemModel equipmentItemModel = EquipmentItems[i];
					if (equipmentItem.IsWeaponEquipment)
					{
						if (equipmentItemModel.IsWeaponEquipment)
						{
							list.Add(equipmentItemModel);
						}
					}
					else if (equipmentItemModel.Definition.Category == equipmentItem.Definition.Category)
					{
						list.Add(equipmentItemModel);
					}
				}
				for (int j = 0; j < list.Count; j++)
				{
					EquipmentItemModel equipmentItem2 = list[j];
					Unequip(equipmentItem2, forceEquip);
					owner?.Equip(equipmentItem2);
				}
			}
			if (!preview)
			{
				List<string> equipmentPassiveTraits = equipmentItem.GetEquipmentPassiveTraits();
				if (equipmentPassiveTraits != null && equipmentPassiveTraits.Count > 0)
				{
					for (int k = 0; k < equipmentPassiveTraits.Count; k++)
					{
						string traitIdentifier = equipmentPassiveTraits[k];
						AddTrait(traitIdentifier);
					}
				}
				List<TraitDefinition> passiveTraits = equipmentItem.GetPassiveTraits();
				if (passiveTraits != null && passiveTraits.Count > 0)
				{
					if (SPAddPassiveAllSlotsTraits == null)
					{
						SPAddPassiveAllSlotsTraits = new List<string>();
					}
					for (int l = 0; l < passiveTraits.Count; l++)
					{
						TraitDefinition traitDefinition = passiveTraits[l];
						AddTrait(traitDefinition.Identifier);
						SPAddPassiveAllSlotsTraits.Add(traitDefinition.Identifier);
					}
				}
				List<UpgradeTraitsData> availableTraits = equipmentItem.GetAvailableTraits();
				for (int m = 0; m < availableTraits.Count; m++)
				{
					UpgradeTraitsData upgradeTraitsData = availableTraits[m];
					TraitDefinition traitDefinition2 = base.manager.GameEconomyData.GetTraitDefinition(upgradeTraitsData.Identifier);
					if (traitDefinition2 != null && traitDefinition2.HasTag("EquipmentPassive"))
					{
						if (upgradeTraitsData.RemodeValues != null && upgradeTraitsData.ThisRemodeParamIndex.TryGetValue(upgradeTraitsData.Identifier, out var value))
						{
							string identifier = upgradeTraitsData.Identifier;
							FixedPoint constructionMultiplier = upgradeTraitsData.ConstructionMultiplier;
							List<int> remodeValues = upgradeTraitsData.RemodeValues;
							List<int> remodeIndex = value;
							AddTrait(identifier, constructionMultiplier, doNotInstantiateTrait: false, null, "", remodeIndex, remodeValues);
						}
						else
						{
							AddTrait(upgradeTraitsData.Identifier, upgradeTraitsData.ConstructionMultiplier);
						}
					}
				}
				if (equipmentItem.Definition.Category == EquipmentCategory.Utility)
				{
					equipmentItem.Level = base.manager.Player.SurvivorContainer.GetHighestLevelSurvivor();
					equipmentItem.RefreshModifiers();
				}
			}
			EquipmentItems.Add(equipmentItem);
			equipmentItem.Owner = this;
			return TWDModelResult.OK;
		}

		public void MigrateToNewArmor(EquipmentItemModel equipment)
		{
			foreach (EquipmentItemModel model in EquipmentItems.Models)
			{
				if (model.Owner == this)
				{
					model.Owner = null;
				}
			}
			ModelList<EquipmentItemModel> modelList = new ModelList<EquipmentItemModel>();
			foreach (EquipmentItemModel model2 in EquipmentItems.Models)
			{
				if (model2.Definition != null)
				{
					modelList.Add(model2);
					model2.Owner = this;
				}
			}
			modelList.Add(equipment);
			equipment.Owner = this;
			EquipmentItems = modelList;
			UpdateModelObjects();
		}

		public virtual bool CanEquip(EquipmentItemModel equipment)
		{
			bool num = CanEquipDisregardingLevel(equipment);
			bool flag = equipment.StartingLevel <= Level;
			if (num)
			{
				if (!flag)
				{
					return equipment.Definition.Category == EquipmentCategory.Utility;
				}
				return true;
			}
			return false;
		}

		public bool EquipWeaponEquipment()
		{
			EquipmentItemModel weaponEquipment = GetWeaponEquipment();
			if (weaponEquipment != null)
			{
				SelectedEquipment = weaponEquipment;
				return true;
			}
			return false;
		}

		public bool EquipChargeEquipment()
		{
			EquipmentItemModel chargeEquipment = GetChargeEquipment();
			if (chargeEquipment != null && ChargeMeter.ChargeAvailable)
			{
				if (SelectedEquipment.EquipmentBreakthrough != null)
				{
					if (chargeEquipment.EquipmentBreakthrough == null)
					{
						chargeEquipment.EquipmentBreakthrough = new EquipmentBreakthroughModel();
						chargeEquipment.EquipmentBreakthrough.SetManager(base.manager);
						chargeEquipment.EquipmentBreakthrough.Level = SelectedEquipment.EquipmentBreakthrough.Level;
						chargeEquipment.EquipmentBreakthrough.UnlockedRandomTrait = SelectedEquipment.EquipmentBreakthrough.UnlockedRandomTrait;
						chargeEquipment.EquipmentBreakthrough.Start();
					}
					else
					{
						chargeEquipment.EquipmentBreakthrough.Level = SelectedEquipment.EquipmentBreakthrough.Level;
						chargeEquipment.EquipmentBreakthrough.UnlockedRandomTrait = SelectedEquipment.EquipmentBreakthrough.UnlockedRandomTrait;
					}
				}
				else
				{
					chargeEquipment.EquipmentBreakthrough = SelectedEquipment.EquipmentBreakthrough;
				}
				SelectedEquipment = chargeEquipment;
				SelectedEquipment.RefreshModifiers();
				return true;
			}
			return false;
		}

		public bool EquipConsumableEquipment()
		{
			EquipmentItemModel consumableEquipment = GetConsumableEquipment();
			if (consumableEquipment != null)
			{
				SelectedEquipment = consumableEquipment;
				NotifyChange("ToggleEquippedEquipments");
				if (SelectedAbility.Definition.TriggerType == AbilityTriggerType.Instant)
				{
					ClearInvisibility();
					AbilityCommand.PerformActions(base.manager, this, SelectedAbility, GridCoordinate, ignoreAPRestrictions: true);
				}
				return true;
			}
			return false;
		}

		public EquipmentItemModel UnequipConsumableEquipment(bool consumableUsed = false)
		{
			EquipmentItemModel consumableEquipment = GetConsumableEquipment();
			if (consumableEquipment != null)
			{
				Unequip(consumableEquipment);
				NotifyChange("ToggleEquippedEquipments", consumableUsed);
				NotifyChange("UnEquipConsumable");
			}
			return consumableEquipment;
		}

		public bool CanEquipDisregardingLevel(EquipmentItemModel equipment)
		{
			bool result = equipment.Definition.CanBeEquippedToFaction(Faction);
			if (equipment.Definition.HolderActors != null && equipment.Definition.HolderActors.Count > 0)
			{
				result = false;
				foreach (string holderActor in equipment.Definition.HolderActors)
				{
					if (holderActor == Definition.ID)
					{
						result = true;
						break;
					}
				}
			}
			return result;
		}

		public bool CheckEquipmentLevel(EquipmentItemModel equipment)
		{
			return equipment.Level <= Level;
		}

		public TWDModelResult Unequip(EquipmentItemModel equipmentItem, bool forceUnequip = false)
		{
			if (!forceUnequip && equipmentItem.Owner != this)
			{
				return TWDModelResult.Error;
			}
			if (EquipmentItems.Count > 0)
			{
				List<EquipmentItemModel> list = new List<EquipmentItemModel>();
				for (int i = 0; i < EquipmentItems.Count; i++)
				{
					EquipmentItemModel equipmentItemModel = EquipmentItems[i];
					if (equipmentItemModel.Definition.Category == equipmentItem.Definition.Category)
					{
						list.Add(equipmentItemModel);
					}
				}
				for (int j = 0; j < list.Count; j++)
				{
					EquipmentItemModel equipmentItemModel2 = list[j];
					List<string> equipmentPassiveTraits = equipmentItemModel2.GetEquipmentPassiveTraits();
					if (equipmentPassiveTraits != null && equipmentPassiveTraits.Count > 0)
					{
						for (int k = 0; k < equipmentPassiveTraits.Count; k++)
						{
							string traitIdentifier = equipmentPassiveTraits[k];
							RemoveTrait(traitIdentifier);
						}
					}
					List<TraitDefinition> passiveTraits = equipmentItemModel2.GetPassiveTraits(isRemove: true);
					if (passiveTraits != null && passiveTraits.Count > 0)
					{
						foreach (TraitDefinition item in passiveTraits)
						{
							if (item != null && !string.IsNullOrEmpty(item.Identifier))
							{
								RemoveTrait(item.Identifier);
							}
						}
						if (SPAddPassiveAllSlotsTraits != null && SPAddPassiveAllSlotsTraits.Count > 0)
						{
							foreach (string sPAddPassiveAllSlotsTrait in SPAddPassiveAllSlotsTraits)
							{
								RemoveTrait(sPAddPassiveAllSlotsTrait);
							}
							SPAddPassiveAllSlotsTraits.Clear();
						}
					}
					foreach (TraitDefinition item2 in passiveTraits)
					{
						if (item2 != null && !string.IsNullOrEmpty(item2.Identifier))
						{
							RemoveTrait(item2.Identifier);
						}
					}
					List<UpgradeTraitsData> availableTraits = equipmentItemModel2.GetAvailableTraits();
					for (int l = 0; l < availableTraits.Count; l++)
					{
						UpgradeTraitsData upgradeTraitsData = availableTraits[l];
						TraitDefinition traitDefinition = base.manager.GameEconomyData.GetTraitDefinition(upgradeTraitsData.Identifier);
						if (traitDefinition != null && traitDefinition.HasTag("EquipmentPassive"))
						{
							RemoveTrait(upgradeTraitsData.Identifier);
						}
					}
					EquipmentItems.Remove(equipmentItemModel2);
					if (equipmentItemModel2.Owner == this)
					{
						equipmentItemModel2.Owner = null;
					}
				}
			}
			return TWDModelResult.OK;
		}

		public void UnequipAll()
		{
			foreach (EquipmentItemModel model in EquipmentItems.Models)
			{
				if (model.Owner == this)
				{
					model.Owner = null;
				}
			}
			EquipmentItems.Clear();
		}

		public bool IsEquipped(EquipmentItemModel equipment)
		{
			int count = EquipmentItems.Count;
			for (int i = 0; i < count; i++)
			{
				if (EquipmentItems[i] == equipment)
				{
					return true;
				}
			}
			return false;
		}

		public void AddPhonePortraitModel()
		{
			BounsPhonePortraitTurn = base.manager.CombatModel.TurnManager.TurnCount;
		}

		public bool IsTakePhonePortraitEffectThisTurn()
		{
			return BounsPhonePortraitTurn == base.manager.CombatModel.TurnManager.TurnCount;
		}

		public void SetFaction(Faction faction)
		{
			Faction = faction;
			OriginalFaction = faction;
		}

		public void SetHitPoints(int currentHitPoints, int maxHitPoints, bool setConfig = false)
		{
			if (setConfig)
			{
				HealUpHitpoint(currentHitPoints);
			}
			else
			{
				SetHitpoints(currentHitPoints, DefenseSystemType.Shield);
			}
			MaxHitPoints = maxHitPoints;
			NotifyChange("ActorHealthChanged");
		}

		public virtual void SetupForCombat(CombatModel combatModel)
		{
			ExclusiveTimedEffect = null;
			PendingExclusiveTimedEffect = null;
			ScorchTimedEffect = null;
			TauntTimedEffect = null;
			CarolNotAttackAndNotAttackedTurns = 0;
			ExtraBurnLayer = 0;
			ExtraBurnTurn = 0;
			FinishShieldTimedEffect();
			CombatCleanSkillShieldType1TimedEffect();
			CombatCleanSkillEquipTauntTimedEffect();
			RemoveTrait("Burning");
			RemoveTrait("StaggerActive");
			RemoveTrait("ABTesterA2Active");
			RemoveTrait("Skinned");
			RemoveTrait("RemoteWeakenActiveFlag");
			CombatEndCondition = CombatEndCondition.None;
			TurnState = TurnState.Idle;
			DebuffRemoteRepulseWeakenAddChargePointPercentage = 0.0;
			DebuffRemoteRepulseWeakenAddChargePoints = 0;
			ResetActionPoints();
			UserCanControl = true;
			UserCanControlFalseReason = "";
			IsExploding = false;
			HasGainedExtraMoveAp = false;
			HasGainedExtraAP = false;
			VisitedExtraApChance = false;
			VisitedRedactChance = false;
			EnsureExtraAP = false;
			AttackKilledAnyEnemy = false;
			AttackHasNotKilledAllEnemies = false;
			FollowUpAttackedOnTurn = false;
			EnsureGainedExtraMoveAp = false;
			HasGainedExtraAPFromInteraction = false;
			CanMoveWithoutAttacking = false;
			GainedAPFromPreviousAbilityExecution = false;
			GainedAPFromAbilityExecution = false;
			AdditionalAttackConsumed = false;
			GivenAdditionalAttacks = 0;
			FightingFuryTargetCount = 0;
			FightingFuryActivated = false;
			HasHeadshotLTTriggered = false;
			freeAttackUsed = false;
			BetterTogetherMultiplier = 0;
			DamageCount = 0;
			CadenceAttackCount = 0;
			CadenceReady = false;
			CadenceBoostingThisAttack = false;
			UndyingState = new UndyingState();
			ActorFactionChangedInCombat = false;
			OneTurnAttackedTimes = 0L;
			DebuffKnockKnockMarkCount = 0L;
			TornApartMarkCount = 0L;
			OneTurnCriticalHit = false;
			OneTurnStagger = false;
			KilledEnemyNum = 0;
			ChargeAttackWithFreeShootingTriggeredCount = 0;
			FightBackTimesThisRound = 0;
			ChargeLoadFloor = 0.0;
			BounsPhonePortraitTurn = -1;
			IsMoving = false;
			CombatAttributeSnapshots = new Dictionary<AttributeType, FixedPoint>();
			ActorAttributeContainer.SetSupportModel(null);
			SupportTalentAssembledTraitRecords = new List<string>();
			ResetABtestParam();
			DebuffParameterManager.ClearDebuffParameters();
			CoexistTimedEffectsManager.ClearCoexistTimedEffects();
			CommandSkillModelManager.ClearCommandSkills();
			ResetOverloadParam();
			HelpreHandActorModel = null;
			GuardActorModel = null;
			ClearRandomStatusNumberOfAttacks();
			RandomStatusTraitIdentifier = null;
			ResetAttributeGreene();
			ResetParryRiposteIncreaseStorey();
			SurvivalGameLeftCD = 0;
			DeadlyFocusLeftCount_SourceSurvivor = 0;
			DeadlyFocusLeftCount_SourceRaider = 0;
			DeadlyFocus_EXDamageLayerCount = 0;
			ShadowedGuard_LeftCount = 0;
			ShadowedGuard_DelHP = 0;
			ShadowedGuard_Atk = 0;
			ResetGuardianVowState();
			ChargeNum = 0L;
			OnRedHealthBar = false;
			SavedOnTurnIndex = -1;
			if (!started)
			{
				base.manager.Debug.LogError("SetupForCombat() -> Actor " + Name + " has not been started! SetupForCombat will fail!");
			}
			InitializeAttributes();
			if (IsWalker && combatModel.SpawnModifiers != null)
			{
				MoveRange += combatModel.SpawnModifiers.WalkerMoveRange;
			}
			if (combatModel.HasPvPRules && Definition.PvPTraits != null)
			{
				for (int i = 0; i < Definition.PvPTraits.Count; i++)
				{
					string traitIdentifier = Definition.PvPTraits[i];
					AddTrait(traitIdentifier);
				}
			}
			ConfigureBaseAttributes();
			for (int j = 0; j < Abilities.Count; j++)
			{
				Abilities[j].SetupForCombat();
			}
			for (int k = 0; k < EquipmentItems.Count; k++)
			{
				EquipmentItems[k].SetupForCombat();
			}
			RemoveTutorialTraits();
			RemoveTemporaryTraits();
			SetupTraits();
			SelectedEquipment = GetWeaponEquipment();
			if (SelectedEquipment != null)
			{
				SelectedEquipment.ResetReloading();
			}
			if (Faction == Faction.Survivor)
			{
				SetBuffTraits();
				MinHitpoints = MaxHitPoints;
			}
			SetUpShieldHitPoints();
			ChargeMeter.Reset();
			ResetAllRageStates();
			DebuffParameterManager.SetupForCombat();
			CoexistTimedEffectsManager.SetupForCombat();
			CommandSkillModelManager.SetupForCombat();
			IsAttackAndBeAttacked = false;
			UpdateModelObjects();
			bloodFrenzyFlag = false;
			SharpBladeLayers = 0;
			SupportTalent_NoMoveHitrateFlag = false;
			SupportTalent_NoMoveCritRateFlag = false;
			IChallengeDebuffProvider challengeDebuffProvider = MapMissionDebuffHelper.CanUseDebuffMission(combatModel.manager);
			if (challengeDebuffProvider != null)
			{
				int debuffStatusRemoveTurns = 0;
				List<DifficultyIncrementalDebuff> challengeDebuffs = challengeDebuffProvider.GetChallengeDebuffs();
				if (IsWalker && ChallengeDebufHelps.GetDebufConfig(challengeDebuffs, ChallengeDebuffType.DebuffStatusRemove) != null)
				{
					debuffStatusRemoveTurns = (int)ChallengeDebufHelps.GetDebufTotalFirstParam(challengeDebuffs, ChallengeDebuffType.DebuffStatusRemove);
				}
				if (IsRaider && ChallengeDebufHelps.GetDebufConfig(challengeDebuffs, ChallengeDebuffType.DebuffStatusRemoveRaider) != null)
				{
					debuffStatusRemoveTurns = (int)ChallengeDebufHelps.GetDebufTotalFirstParam(challengeDebuffs, ChallengeDebuffType.DebuffStatusRemoveRaider);
				}
				DebuffStatusRemoveTurns = debuffStatusRemoveTurns;
			}
			ClearTotalVengefulChargeNums();
			CleanAllCitadelTraits();
			CleanLeaderBuffDeathsDoor();
		}

		public void ResetAttributeGreene()
		{
			List<string> attributeDefinition = definition.AttributeDefinition;
			if (attributeDefinition == null || attributeDefinition.Count <= 0)
			{
				return;
			}
			foreach (string item in attributeDefinition)
			{
				string[] array = item.Split(':');
				if (array.Length == 2)
				{
					AttributeModel.UpdateResetAttributeModelValue(array[0], (FixedPoint)array[1]);
				}
			}
		}

		public void SetAbilities(ModelList<AbilityModel> abilities)
		{
			Abilities = abilities;
		}

		public void AddTrait(string traitIdentifier, FixedPoint multiplier = default(FixedPoint), bool doNotInstantiateTrait = false, FixedPoint? chance = null, string tag = "", List<int> RemodeIndex = null, List<int> RemodeValue = null)
		{
			RemoveTrait(traitIdentifier);
			if (HasTrait(traitIdentifier))
			{
				return;
			}
			TraitDefinition traitDefinition = base.manager.GameEconomyData.GetTraitDefinition(traitIdentifier);
			if (traitDefinition != null)
			{
				TraitEntry traitEntry = TraitContainer.AddTrait(traitIdentifier, multiplier, isTemporary: false, 0L, "", RemodeIndex, RemodeValue);
				if (traitEntry != null && !doNotInstantiateTrait)
				{
					RegisterTrait(traitEntry, chance);
					NotifyChange("actorTraitGained", traitEntry.RemodeTraitDefinition(traitDefinition));
				}
			}
		}

		public void AddTemporaryTrait(string traitIdentifier, FixedPoint multiplier = default(FixedPoint), FixedPoint? chance = null, long duration = 0L, string tag = "", List<int> RemodeIndex = null, List<int> RemodeValue = null)
		{
			RemoveTrait(traitIdentifier);
			if (HasTrait(traitIdentifier))
			{
				return;
			}
			TraitDefinition traitDefinition = base.manager.GameEconomyData.GetTraitDefinition(traitIdentifier);
			if (traitDefinition != null)
			{
				TraitEntry traitEntry = TraitContainer.AddTrait(traitIdentifier, multiplier, isTemporary: true, duration, tag, RemodeIndex, RemodeValue);
				if (traitEntry != null)
				{
					RegisterTrait(traitEntry, chance);
					NotifyChange("actorTraitGained", traitEntry.RemodeTraitDefinition(traitDefinition));
				}
			}
		}

		public void AddTraitByEntry(TraitEntry entry)
		{
			RemoveTrait(entry.TraitIdentifier);
			if (HasTrait(entry.TraitIdentifier))
			{
				return;
			}
			TraitDefinition traitDefinition = base.manager.GameEconomyData.GetTraitDefinition(entry.TraitIdentifier);
			if (traitDefinition != null)
			{
				TraitEntry traitEntry = TraitContainer.AddTraitByEntry(entry);
				if (traitEntry != null)
				{
					RegisterTrait(traitEntry);
					NotifyChange("actorTraitGained", traitEntry.RemodeTraitDefinition(traitDefinition));
				}
			}
		}

		protected void AddMockTrait(string traitIdentifier)
		{
			RemoveTrait(traitIdentifier);
			if (!HasTrait(traitIdentifier) && base.manager.GameEconomyData.GetTraitDefinition(traitIdentifier) != null)
			{
				TraitContainer.AddTrait(traitIdentifier, default(FixedPoint), isTemporary: false, 0L);
			}
		}

		public void RegisterLeaderTraits()
		{
			List<TraitEntry> traits = GetTraits();
			for (int i = 0; i < traits.Count; i++)
			{
				RegisterLeaderTrait(traits[i]);
			}
		}

		public void UnregisterLeaderTraits()
		{
			if (LeaderTraitModifiers == null)
			{
				return;
			}
			if (base.manager != null && base.manager.Player != null)
			{
				AbilityManagerModel abilityManager = base.manager.Player.AbilityManager;
				if (abilityManager != null)
				{
					for (int i = 0; i < LeaderTraitModifiers.Count; i++)
					{
						ModelModifier modifier = LeaderTraitModifiers[i];
						if (abilityManager.HasFactionModifier(Faction, modifier))
						{
							abilityManager.RemoveFactionModifier(Faction, modifier);
						}
					}
				}
			}
			LeaderTraitModifiers.Clear();
		}

		public void RemoveAllTraits()
		{
			List<TraitEntry> list = new List<TraitEntry>(TraitContainer.Traits);
			for (int i = 0; i < list.Count; i++)
			{
				RemoveTrait(list[i].TraitIdentifier);
			}
		}

		public void RemoveTutorialTraits()
		{
			List<string> list = new List<string>();
			list.Add("TutorialSetDamage");
			list.Add("TutorialUninterruptable");
			list.Add("TutorialInvulnerable");
			GameEconomyData gameEconomyData = base.manager.GameEconomyData;
			List<TraitEntry> traits = GetTraits();
			for (int i = 0; i < traits.Count; i++)
			{
				string traitIdentifier = traits[i].TraitIdentifier;
				TraitDefinition traitDefinition = gameEconomyData.GetTraitDefinition(traitIdentifier);
				if (traitDefinition != null && traitDefinition.HasTag("Tutorial") && !list.Contains(traitIdentifier) && HasTrait(traitIdentifier))
				{
					list.Add(traitIdentifier);
				}
			}
			for (int j = 0; j < list.Count; j++)
			{
				RemoveTrait(list[j]);
			}
		}

		public void RemoveTrait(string traitIdentifier)
		{
			if (base.manager == null)
			{
				return;
			}
			TraitEntry trait = TraitContainer.GetTrait(traitIdentifier);
			if (trait != null)
			{
				TraitDefinition traitDefinition = base.manager.GameEconomyData.GetTraitDefinition(traitIdentifier);
				if (traitDefinition != null)
				{
					UnregisterTrait(trait);
					TraitContainer.RemoveTrait(traitIdentifier);
					NotifyChange("actorLostTrait", trait.RemodeTraitDefinition(traitDefinition));
				}
			}
		}

		public void RemoveAnyLevelTrait(string traitIdentifier)
		{
			if (base.manager == null)
			{
				return;
			}
			TraitEntry traitAnyLevel = TraitContainer.GetTraitAnyLevel(traitIdentifier);
			if (traitAnyLevel != null)
			{
				TraitDefinition traitDefinition = base.manager.GameEconomyData.GetTraitDefinition(traitAnyLevel.TraitIdentifier);
				if (traitDefinition != null)
				{
					UnregisterTrait(traitAnyLevel);
					TraitContainer.RemoveTrait(traitAnyLevel.TraitIdentifier);
					NotifyChange("actorLostTrait", traitAnyLevel.RemodeTraitDefinition(traitDefinition));
				}
			}
		}

		public List<TraitEntry> GetTraits()
		{
			return TraitContainer.Traits;
		}

		public void ReplaceTraits(List<TraitEntry> newTraits)
		{
			TraitContainer.ReplaceTraits(newTraits);
		}

		public bool HasAnyLevelTrait(string traitIdentifier)
		{
			return TraitContainer.GetTraitAnyLevel(traitIdentifier) != null;
		}

		public bool HasTrait(string traitIdentifier)
		{
			return TraitContainer.GetTrait(traitIdentifier) != null;
		}

		public void SetUpShieldHitPoints()
		{
			MaxShieldHitPoints = 0;
			ShieldHitPoints = 0;
			if (HasAnyLevelTrait("Equipment.Shield") || HasAnyLevelTrait("Equipment_Passive_Shield"))
			{
				FixedPoint value = 0.0;
				base.manager.Player.AbilityManager.VisitParameter("AbilityModifierIncreaseShieldHitPointsPercentage", ref value, this);
				if (!(value <= 0L))
				{
					int shieldHitPoints = (MaxShieldHitPoints = (int)(Hitpoints * value));
					ShieldHitPoints = shieldHitPoints;
				}
			}
		}

		public TraitDefinition GetTraitWithTag(string tag)
		{
			GameEconomyData gameEconomyData = base.manager.GameEconomyData;
			List<TraitEntry> traits = GetTraits();
			for (int i = 0; i < traits.Count; i++)
			{
				TraitEntry traitEntry = traits[i];
				TraitDefinition traitDefinition = traitEntry.RemodeTraitDefinition(gameEconomyData.GetTraitDefinition(traitEntry.TraitIdentifier));
				if (traitDefinition != null && traitDefinition.HasTag(tag))
				{
					return traitDefinition;
				}
			}
			return null;
		}

		public List<TraitDefinition> GetTraitsWithTag(string tag)
		{
			List<TraitDefinition> list = new List<TraitDefinition>();
			GameEconomyData gameEconomyData = base.manager.GameEconomyData;
			List<TraitEntry> traits = GetTraits();
			for (int i = 0; i < traits.Count; i++)
			{
				TraitEntry traitEntry = traits[i];
				TraitDefinition traitDefinition = traitEntry.RemodeTraitDefinition(gameEconomyData.GetTraitDefinition(traitEntry.TraitIdentifier));
				if (traitDefinition != null && traitDefinition.HasTag(tag))
				{
					list.Add(traitDefinition);
				}
			}
			return list;
		}

		public TraitEntry GetTraitWithTraitIdentifier(string identifier)
		{
			for (int i = 0; i < TraitContainer.Traits.Count; i++)
			{
				if (TraitContainer.Traits[i].TraitIdentifier.ToLower().Contains(identifier.ToLower()))
				{
					return TraitContainer.Traits[i];
				}
			}
			return null;
		}

		public List<TraitEntry> GetTraitsThatContain(string traitIdentifier)
		{
			List<TraitEntry> list = new List<TraitEntry>();
			string value = traitIdentifier.ToLower();
			List<TraitEntry> traits = GetTraits();
			for (int i = 0; i < traits.Count; i++)
			{
				if (traits[i].TraitIdentifier.ToLower().Contains(value))
				{
					list.Add(traits[i]);
				}
			}
			return list;
		}

		public TraitDefinition GetActiveWeaponTraitByIdentifier(string traitName)
		{
			GameEconomyData gameEconomyData = base.manager.GameEconomyData;
			int num = 0;
			while (GetWeaponEquipment().GetAvailableTraits() != null && num < GetWeaponEquipment().GetAvailableTraits().Count)
			{
				if (UpgradeTraitsData.StripEquipmentLabel(UpgradeTraitsData.StripTraitLevelIdentifier(GetWeaponEquipment().GetAvailableTraits()[num].Identifier)) == traitName)
				{
					return gameEconomyData.GetTraitDefinition(GetWeaponEquipment().GetAvailableTraits()[num].Identifier);
				}
				num++;
			}
			return null;
		}

		public int GetEquipmentBreakThroughTraitParam(string traitName, int paramIndex)
		{
			if (SelectedEquipment != null && SelectedEquipment.UpgradeTraits != null)
			{
				foreach (UpgradeTraitsData upgradeTrait in SelectedEquipment.UpgradeTraits)
				{
					if (upgradeTrait == null || !upgradeTrait.RemodelEd || !upgradeTrait.Identifier.Contains(traitName) || !upgradeTrait.ThisRemodeValues.TryGetValue(upgradeTrait.Identifier, out var value) || !upgradeTrait.ThisRemodeParamIndex.TryGetValue(upgradeTrait.Identifier, out var value2))
					{
						continue;
					}
					for (int i = 0; i < value.Count && i < value2.Count; i++)
					{
						if (value2[i] == paramIndex)
						{
							return value[i];
						}
					}
				}
			}
			return GetEquipmentTrait(traitName, getActiveTraits: true)?.GetParameter<int>(paramIndex) ?? (-1);
		}

		public TraitDefinition GetEquipmentTrait(string traitName, bool getActiveTraits = false)
		{
			GameEconomyData gameEconomyData = base.manager.GameEconomyData;
			List<string> equipmentActiveTraits = SelectedEquipment.GetEquipmentActiveTraits();
			if (SelectedEquipment.Definition.Category == EquipmentCategory.Utility)
			{
				int num = 0;
				while (SelectedEquipment.Definition.TraitsOverride != null && num < SelectedEquipment.Definition.TraitsOverride.Count)
				{
					if (UpgradeTraitsData.StripEquipmentLabel(UpgradeTraitsData.StripTraitLevelIdentifier(SelectedEquipment.Definition.TraitsOverride[num])) == traitName)
					{
						return gameEconomyData.GetTraitDefinition(equipmentActiveTraits[num]);
					}
					num++;
				}
			}
			else
			{
				List<TraitEntry> traits = GetTraits();
				for (int i = 0; i < traits.Count; i++)
				{
					if (UpgradeTraitsData.StripEquipmentLabel(UpgradeTraitsData.StripTraitLevelIdentifier(traits[i].TraitIdentifier)) == traitName)
					{
						return traits[i].RemodeTraitDefinition(gameEconomyData.GetTraitDefinition(traits[i].TraitIdentifier));
					}
				}
			}
			if (getActiveTraits)
			{
				int num2 = 0;
				while (SelectedEquipment.Definition.ActiveTraits != null && num2 < SelectedEquipment.Definition.ActiveTraits.Count)
				{
					if (UpgradeTraitsData.StripEquipmentLabel(UpgradeTraitsData.StripTraitLevelIdentifier(equipmentActiveTraits[num2])) == traitName)
					{
						return gameEconomyData.GetTraitDefinition(equipmentActiveTraits[num2]);
					}
					num2++;
				}
			}
			return null;
		}

		public bool HasTraitsThatContains(string identifier)
		{
			return GetTraitsThatContain(identifier).Count > 0;
		}

		protected virtual void SetupTraits()
		{
			List<TraitEntry> traits = GetTraits();
			for (int i = 0; i < traits.Count; i++)
			{
				TraitEntry traitEntry = traits[i];
				RegisterTrait(traitEntry);
			}
			if (Definition.InitialTraits != null)
			{
				for (int j = 0; j < Definition.InitialTraits.Count; j++)
				{
					string traitIdentifier = Definition.InitialTraits[j];
					AddTrait(traitIdentifier);
				}
			}
			AddTrait("Trigger");
		}

		public virtual void SetupMockTraits()
		{
			if (Definition.InitialTraits != null)
			{
				for (int i = 0; i < Definition.InitialTraits.Count; i++)
				{
					string traitIdentifier = Definition.InitialTraits[i];
					AddMockTrait(traitIdentifier);
				}
			}
			AddMockTrait("Trigger");
		}

		public virtual void SetupBadgeBonuses()
		{
		}

		private void SetBuffTraits()
		{
			foreach (BuffEffectType value in Enum.GetValues(typeof(BuffEffectType)))
			{
				RemoveTrait("Buff" + value);
			}
			int count = base.manager.Player.Camp.Buildings.Count;
			for (int i = 0; i < count; i++)
			{
				BuildingUpgradeLevel currentUpgradeLevel = base.manager.Player.Camp.Buildings[i].GetCurrentUpgradeLevel();
				if (currentUpgradeLevel != null)
				{
					BuffEffectType buffEffectType = currentUpgradeLevel.BuffEffectType;
					if (buffEffectType != BuffEffectType.None)
					{
						AddTrait("Buff" + buffEffectType);
					}
				}
			}
		}

		private TraitAbilityModel GetTraitAbilityModel(TraitDefinition traitDefinition)
		{
			int count = Abilities.Count;
			for (int i = 0; i < count; i++)
			{
				if (Abilities[i] is TraitAbilityModel traitAbilityModel && traitAbilityModel.TraitDefinitionIdentifier == traitDefinition.Identifier)
				{
					return traitAbilityModel;
				}
			}
			return null;
		}

		private AbilityModel GetAbilityModel(string abilityID)
		{
			foreach (AbilityModel ability in Abilities)
			{
				if (ability != null && ability.DefinitionID == abilityID)
				{
					return ability;
				}
			}
			return null;
		}

		private void RegisterTraitAbility(TraitEntry traitEntry, FixedPoint? chance)
		{
			TraitDefinition traitDefinition = traitEntry.RemodeTraitDefinition(base.manager.GameEconomyData.GetTraitDefinition(traitEntry.TraitIdentifier));
			if (traitDefinition == null || !HasTrait(traitEntry.TraitIdentifier))
			{
				return;
			}
			TraitAbilityModel traitAbilityModel = GetTraitAbilityModel(traitDefinition);
			if (traitAbilityModel == null && TraitContainer.InstantiateTraitAbility(traitDefinition, traitEntry.ConstructionParametersMultiplier, chance) is TraitAbilityModel model)
			{
				Abilities.Add(model);
			}
			if (traitDefinition.DependsOnTraits == null || traitDefinition.DependsOnTraits.Count <= 0)
			{
				return;
			}
			for (int i = 0; i < traitDefinition.DependsOnTraits.Count; i++)
			{
				string text = traitDefinition.DependsOnTraits[i];
				if (string.IsNullOrEmpty(text))
				{
					continue;
				}
				TraitDefinition traitDefinition2 = base.manager.GameEconomyData.GetTraitDefinition(text);
				if (traitDefinition2 != null)
				{
					TraitAbilityModel traitAbilityModel2 = GetTraitAbilityModel(traitDefinition2);
					if (traitAbilityModel2 == null && TraitContainer.InstantiateTraitAbility(traitDefinition2, traitEntry.ConstructionParametersMultiplier, 0.0) is TraitAbilityModel model2)
					{
						Abilities.Add(model2);
					}
				}
			}
		}

		private void UnregisterTraitAbility(TraitEntry traitEntry)
		{
			TraitDefinition traitDefinition = traitEntry.RemodeTraitDefinition(base.manager.GameEconomyData.GetTraitDefinition(traitEntry.TraitIdentifier));
			if (traitDefinition != null)
			{
				TraitAbilityModel traitAbilityModel = GetTraitAbilityModel(traitDefinition);
				if (traitAbilityModel != null)
				{
					Abilities.Remove(traitAbilityModel);
				}
			}
		}

		protected void UnregisterTraitAbilityDependencies(string identifier)
		{
			TraitDefinition traitDefinition = base.manager.GameEconomyData.GetTraitDefinition(identifier);
			if (traitDefinition.DependsOnTraits == null || traitDefinition.DependsOnTraits.Count <= 0)
			{
				return;
			}
			for (int i = 0; i < traitDefinition.DependsOnTraits.Count; i++)
			{
				string text = traitDefinition.DependsOnTraits[i];
				if (string.IsNullOrEmpty(text))
				{
					continue;
				}
				TraitDefinition traitDefinition2 = base.manager.GameEconomyData.GetTraitDefinition(text);
				if (traitDefinition2 != null)
				{
					TraitAbilityModel traitAbilityModel = GetTraitAbilityModel(traitDefinition2);
					if (traitAbilityModel != null)
					{
						Abilities.Remove(traitAbilityModel);
					}
				}
			}
		}

		private void RegisterLeaderTrait(TraitEntry traitEntry)
		{
			TraitDefinition traitDefinition = traitEntry.RemodeTraitDefinition(base.manager.GameEconomyData.GetTraitDefinition(traitEntry.TraitIdentifier));
			if (traitDefinition == null || !HasTrait(traitEntry.TraitIdentifier) || !traitDefinition.HasTag("FactionBuffTrait"))
			{
				return;
			}
			AbilityManagerModel abilityManager = base.manager.Player.AbilityManager;
			base.Debug.Log("registering leader trait for=" + Name + " faction=" + Faction);
			if (abilityManager.HasLeaderTraitAlreadyRegistered(Faction))
			{
				base.Debug.Log("leader traits exist already for faction=" + Faction);
				return;
			}
			List<ModelModifier> list = TraitContainer.CreateTraitModifiers(traitDefinition, traitEntry.ConstructionParametersMultiplier, null);
			for (int i = 0; i < list.Count; i++)
			{
				ModelModifier modifier = list[i];
				if (!abilityManager.HasFactionModifier(Faction, modifier))
				{
					abilityManager.RegisterFactionModifier(this, Faction, modifier);
				}
			}
			if (LeaderTraitModifiers == null)
			{
				LeaderTraitModifiers = new List<ModelModifier>();
			}
			else
			{
				LeaderTraitModifiers.Clear();
			}
			LeaderTraitModifiers.AddRange(list);
		}

		private void RegisterTrait(TraitEntry traitEntry)
		{
			RegisterTrait(traitEntry, null);
		}

		private void RegisterTrait(TraitEntry traitEntry, FixedPoint? chance)
		{
			TraitDefinition traitDefinition = traitEntry.RemodeTraitDefinition(base.manager.GameEconomyData.GetTraitDefinition(traitEntry.TraitIdentifier));
			if (traitDefinition == null)
			{
				if (traitEntry.TraitIdentifier != null && !traitEntry.TraitIdentifier.StartsWith("PVP"))
				{
					base.manager.Debug.LogWarning("AddTrait: Trait definition not found for trait " + traitEntry.TraitIdentifier + " for Actor [" + ToString() + "]");
				}
			}
			else
			{
				RegisterTraitAbility(traitEntry, chance);
				SetProducerFromTrait(traitDefinition);
			}
		}

		private void UnregisterTrait(TraitEntry traitEntry)
		{
			TraitDefinition traitDefinition = traitEntry.RemodeTraitDefinition(base.manager.GameEconomyData.GetTraitDefinition(traitEntry.TraitIdentifier));
			if (traitDefinition == null)
			{
				if (traitEntry.TraitIdentifier != null && !traitEntry.TraitIdentifier.StartsWith("PVP"))
				{
					base.manager.Debug.LogWarning("RemoveTrait: Trait definition not found for trait " + traitEntry.TraitIdentifier + " for Actor [" + ToString() + "]");
				}
			}
			else
			{
				UnregisterTraitAbility(traitEntry);
				if (traitDefinition.HasTag("ResourceProd") && Producer != null)
				{
					Producer = null;
					UpdateModelObjects();
				}
			}
		}

		public void IncreaseAttackCount()
		{
			_randomStatusNumberOfAttack++;
		}

		public void NewTurn()
		{
			if (IsTurnConsumedOutOfFaction)
			{
				IsTurnConsumedOutOfFaction = false;
				TurnState = TurnState.Completed;
				return;
			}
			ClearOOTPerformed();
			ClearBloodThirst();
			ClearExtraAP();
			if (KillsInTurn > 0 && HasAnyLevelTrait("Equipment_Active_King"))
			{
				EquipmentActiveKingFactor++;
			}
			ClearKillsInTurnCount();
			ClearHitsInTurn();
			VisitedRedactChance = false;
			UsedToolThisTurn = false;
			UsedChargeAttackThisTurn = false;
			HadActionPointsAtEndOfTurn = false;
			CanMoveWithoutAttacking = false;
			GainedAPFromPreviousAbilityExecution = false;
			GainedAPFromAbilityExecution = false;
			AdditionalAttackConsumed = false;
			FightingFuryActivated = false;
			MoveRangeConsumed = 0;
			FightingFuryTargetCount = 0;
			HasHeadshotLTTriggered = false;
			if ((ExclusiveTimedEffect != null && ExclusiveTimedEffect.Type != TimedEffectType.Disorient && ExclusiveTimedEffect.Type != TimedEffectType.ABTesterA && ExclusiveTimedEffect.Type != TimedEffectType.Root && ExclusiveTimedEffect.Type != TimedEffectType.Pitfall && ExclusiveTimedEffect.Type != TimedEffectType.Herd && ExclusiveTimedEffect.Type != TimedEffectType.Crippled) || TurnConsumedByEatingLure)
			{
				TurnConsumedByEatingLure = false;
				EndAction();
			}
			else
			{
				TurnState = TurnState.Idle;
				ResetActionPoints();
			}
			AllowSecondMoveAfterAbility = false;
			AdditionalMoveRange = 0;
			RevengedOnTurn = false;
			ShieldRevengedTimesOnTurn = 0;
			OverwatchedOnTurn = false;
			AttackKilledAnyEnemy = false;
			AttackHasNotKilledAllEnemies = false;
			FollowUpAttackedOnTurn = false;
			PreAttackedOnTurn = false;
			PreAttackedOnRiposte = false;
			PassByAttackedOnMove = false;
			GivenAdditionalAttacks = 0;
			freeAttackUsed = false;
			BetterTogetherMultiplier = 0;
			OneTurnCriticalHit = false;
			OneTurnStagger = false;
			DamageCount = 0;
			bloodFrenzyFlag = false;
			NotifyChange("bloodFrenzyFlagUpdate");
			DealBurningDamage();
			DealBleedingDamage();
			TraitEntry trait = TraitContainer.GetTrait("Gore");
			if (trait != null)
			{
				trait.TraitDuration--;
				if (trait.TraitDuration == 0L)
				{
					ClearInvisibility();
				}
			}
			TraitEntry trait2 = TraitContainer.GetTrait("WalkerMikeActive");
			if (trait2 != null)
			{
				trait2.TraitDuration--;
				if (trait2.TraitDuration <= 0)
				{
					RemoveTrait("WalkerMikeActive");
				}
			}
			if (IsReloading)
			{
				UpdateReloading();
			}
			if (!IsAttackAndBeAttacked)
			{
				CarolNotAttackAndNotAttackedTurns++;
			}
			IsAttackAndBeAttacked = false;
			ExtraBurnTurn = UtilsMath.Clamp(ExtraBurnTurn - 1, 0, ExtraBurnTurn - 1);
			if (ExtraBurnTurn <= 0)
			{
				ExtraBurnLayer = 0;
			}
			if (!UserCanControl)
			{
				EndAction();
			}
			SetSurvivalGameAI();
			SetShadowedGuardAI();
			VengefulChargeAPNum_Turns = 0;
			CleanAllCitadelTraits();
			NotifyChange("actorNewTurn");
		}

		public void ClearPerAttackFlags()
		{
			FollowThroughTriggeredInAttack = false;
		}

		public void EndAction()
		{
			TacticalResupplyMagazineNextDragLineCritPending = false;
			if (CanStoreLeftOverAP)
			{
				HadActionPointsAtEndOfTurn = true;
			}
			TurnState = TurnState.Completed;
			AbilityCompleted = true;
			MoveCompleted = true;
			SecondMoveCompleted = true;
			NotifyChange("actorTurnCompleted");
		}

		public void EndMovement()
		{
			if (!GainedAPFromPreviousAbilityExecution && !GetIsUsingExtraAPChargeEquipment() && !freeAttackUsed)
			{
				AdditionalAttackCount = 0;
			}
			AllowSecondMoveAfterAbility = false;
			if (MoveCompleted)
			{
				SecondMoveCompleted = true;
				NotifyChange("actorSecondMoveCompleted");
			}
			else
			{
				MoveCompleted = true;
				if (IsInvisible)
				{
					SecondMoveCompleted = true;
					NotifyChange("actorSecondMoveCompleted");
					return;
				}
				NotifyChange("actorMoveCompleted");
			}
			FightingFuryActivated = false;
		}

		public void UpdateEffectDuration(Faction previousFaction)
		{
			TraitEntry trait = TraitContainer.GetTrait("StaggerActive");
			if (trait != null && previousFaction == Faction)
			{
				trait.TraitDuration--;
				if (trait.TraitDuration == 0L)
				{
					ClearStaggered();
				}
			}
			TraitEntry trait2 = TraitContainer.GetTrait("Burning");
			if (trait2 != null && trait2.TraitDuration > 0 && previousFaction == Faction)
			{
				trait2.TraitDuration--;
				if (trait2.TraitDuration == 0L)
				{
					RemoveTrait("Burning");
				}
			}
			TraitEntry trait3 = TraitContainer.GetTrait("Skinned");
			if (trait3 != null && trait3.TraitDuration > 0 && previousFaction == Faction)
			{
				trait3.TraitDuration--;
				if (trait3.TraitDuration == 0L)
				{
					RemoveTrait("Skinned");
				}
			}
			TraitEntry trait4 = TraitContainer.GetTrait("ABTesterA2Active");
			if (trait4 != null && previousFaction == Faction)
			{
				trait4.TraitDuration--;
				if (trait4.TraitDuration == 0L)
				{
					RemoveTrait("ABTesterA2Active");
				}
			}
			TraitEntry trait5 = TraitContainer.GetTrait("RemoteWeakenActiveFlag");
			if (trait5 != null && previousFaction == Faction)
			{
				trait5.TraitDuration--;
				if (trait5.TraitDuration == 0L)
				{
					DebuffRemoteRepulseWeakenAddChargePointPercentage = 0.0;
					DebuffRemoteRepulseWeakenAddChargePoints = 0;
					RemoveTrait("RemoteWeakenActiveFlag");
				}
			}
			TraitEntry trait6 = TraitContainer.GetTrait("DebuffEquipmentKaboom");
			if (trait6 != null && trait6.TraitDuration > 0 && previousFaction == Faction)
			{
				trait6.TraitDuration--;
				if (trait6.TraitDuration == 0L)
				{
					RemoveTrait("DebuffEquipmentKaboom");
				}
			}
			NotifyChange("UpdateEffectDurationEvent");
		}

		public void EndAbilityAction(bool allowSecondMove = false, int extraMoveRange = 0, bool resetMoveCompleted = false, bool clearInvisibility = true)
		{
			AbilityCompleted = true;
			if (resetMoveCompleted)
			{
				MoveCompleted = false;
			}
			if (!MoveCompleted)
			{
				AllowSecondMoveAfterAbility = allowSecondMove;
				if (allowSecondMove)
				{
					AdditionalMoveRange = extraMoveRange;
				}
			}
			if (clearInvisibility)
			{
				ClearInvisibility();
			}
			NotifyChange("actorAbilityCompleted");
		}

		public void EnsureExtraAction(string notificationKey, bool dueToLuck)
		{
			if (AbilityCompleted || SecondMoveCompleted)
			{
				TurnState = TurnState.Idle;
				AbilityCompleted = false;
				SecondMoveCompleted = false;
				EnsureExtraAP = false;
			}
			ClearInvisibility();
			NotifyChange("actorExtraAbilityAction", new object[2] { notificationKey, dueToLuck });
		}

		public void HandleAdditionalAttacks(bool gainedAPFromAbility, bool freeAttackUsedOnAbility)
		{
			if (!CanMoveWithoutAttacking || IsStunned || IsElectricShocked || IsReloading || IsStruggling || IsQuantunCanNotMove)
			{
				EndAbilityAction();
				return;
			}
			if (PassByAttackedOnMove && AdditionalAttackConsumed && !GainedAPFromPreviousAbilityExecution)
			{
				AbilityCompleted = true;
				ClearInvisibility();
				EndMovement();
				CanMoveWithoutAttacking = false;
				return;
			}
			if (PassByAttackedOnMove)
			{
				AbilityCompleted = false;
				NotifyChange("actorAdditionalAttackChecked");
				return;
			}
			if (AdditionalAttackCount > 0)
			{
				TurnState = TurnState.Idle;
				AbilityCompleted = false;
				MoveCompleted = true;
				SecondMoveCompleted = false;
				EnsureExtraAP = false;
				string text = "LeaderBuffFightingFury";
				if (GivenAdditionalAttacks == AdditionalAttackCount)
				{
					NotifyChange("AbilityVisited", new object[2] { text, false });
				}
				if (gainedAPFromAbility && GainedAPFromAbilityExecution && GivenAdditionalAttacks == AdditionalAttackCount)
				{
					freeAttackUsed = true;
					AdditionalAttackConsumed = true;
					NotifyChange("actorExtraAbilityAction", new object[1] { text });
				}
				else
				{
					freeAttackUsed = false;
				}
				if (!GainedAPFromPreviousAbilityExecution && AdditionalAttackCount > 0)
				{
					NotifyChange("actorExtraAbilityAction", new object[1] { text });
					AdditionalAttackCount--;
					AdditionalAttackConsumed = true;
				}
				else
				{
					GainedAPFromPreviousAbilityExecution = false;
				}
				ClearInvisibility();
			}
			if (gainedAPFromAbility)
			{
				GainedAPFromPreviousAbilityExecution = true;
				if (GainedAPFromAbilityExecution)
				{
					GainedAPFromPreviousAbilityExecution = false;
					GainedAPFromAbilityExecution = false;
					NotifyChange("actorUsedFreeAttack");
				}
				NotifyChange("actorAdditionalAttackChecked");
				return;
			}
			if (CanMoveWithoutAttacking && AdditionalAttackCount <= 0)
			{
				AbilityCompleted = true;
				ClearInvisibility();
				if (!AllowSecondMoveAfterAbility)
				{
					EndMovement();
				}
				else
				{
					NotifyChange("actorExtraAbilityAction");
					MoveCompleted = false;
					EndAbilityAction(allowSecondMove: true);
				}
				CanMoveWithoutAttacking = false;
			}
			NotifyChange("actorAdditionalAttackChecked");
		}

		public bool ClearModelObjectReferences()
		{
			bool result = false;
			if (LastHitAttacker != null)
			{
				LastHitAttacker = null;
				result = true;
			}
			if (LastHitExplosive != null)
			{
				LastHitExplosive = null;
				result = true;
			}
			if (SelectedEquipment != null)
			{
				SelectedEquipment = null;
				result = true;
			}
			if (Producer != null)
			{
				Producer = null;
				result = true;
			}
			if (EquipmentItems.Count > 0)
			{
				foreach (EquipmentItemModel model in EquipmentItems.Models)
				{
					if (model.Owner == this)
					{
						model.Owner = null;
					}
				}
				EquipmentItems.Clear();
				result = true;
			}
			return result;
		}

		private void ClearOOTPerformed()
		{
			LastOOT = OOTType.None;
			HasPerformedOOT = false;
		}

		private void ClearBloodThirst()
		{
			if (BloodThirst)
			{
				BloodThirst = false;
			}
		}

		private void ClearExtraAP()
		{
			VisitedExtraApChance = false;
			EnsureExtraAP = false;
			EnsureGainedExtraMoveAp = false;
			HasGainedExtraMoveAp = false;
			HasGainedExtraAP = false;
			HasGainedExtraAPFromInteraction = false;
			TacticalResupplyMagazineNextDragLineCritPending = false;
		}

		private void ClearKillsInTurnCount()
		{
			KillsInTurn = 0;
		}

		public void ClearEquipmentActiveKingFactor()
		{
			EquipmentActiveKingFactor = 0;
		}

		public void ClearRandomStatusNumberOfAttacks()
		{
			RandomStatusNumberOfAttack = 0;
		}

		private void ClearHitsInTurn()
		{
			HitsInTurn = 0;
		}

		public void ClearInvisibility()
		{
			if (IsInvisible)
			{
				RemoveTrait("Gore");
			}
		}

		public void ClearCarolAttackTurn()
		{
			CarolNotAttackAndNotAttackedTurns = 0;
			IsAttackAndBeAttacked = true;
		}

		public void ClearStaggered()
		{
			if (IsStaggered)
			{
				RemoveTrait("StaggerActive");
			}
		}

		public void SetOOTPerformed(OOTType OOT)
		{
			LastOOT = OOT;
			HasPerformedOOT = true;
		}

		public void DealDamage(int damage, ActorModel attacker, DamageType damageType, ActorModel originalDamageInstigator = null, bool preHPDeductionResolved = false)
		{
			if (IsDead || Faction == Faction.Lure)
			{
				return;
			}
			if (!preHPDeductionResolved)
			{
				PreHPDeductionAction preHPDeductionAction = new PreHPDeductionAction(this, attacker, damage, damageType);
				base.manager?.ExecuteAction(preHPDeductionAction);
				if (preHPDeductionAction.Avoided)
				{
					return;
				}
				damage = Math.Max(0, preHPDeductionAction.Damage);
			}
			int hitpoints = Hitpoints;
			if (originalDamageInstigator != null && originalDamageInstigator.Faction == Faction.Survivor)
			{
				attacker = originalDamageInstigator;
			}
			if (Faction == Faction.Survivor)
			{
				if (this is SurvivorModel survivorModel)
				{
					survivorModel.Statistics.IncreaseHitsTakenInCombat();
					survivorModel.Statistics.IncreaseTotalDamageTakenInCombat(damage);
				}
			}
			else if (attacker != null && attacker.Faction == Faction.Survivor && attacker is SurvivorModel survivorModel2)
			{
				survivorModel2.Statistics.IncreaseHitsInflictedInMission();
				survivorModel2.Statistics.IncreaseTotalDamageInflictedInCombat(damage);
			}
			if (GetType() == typeof(SurvivorModel))
			{
				int val = Hitpoints - damage;
				if (ShieldHitPoints <= 0)
				{
					val = Math.Max(val, 0);
					MinHitpoints = Math.Min(val2: Math.Min(val, MaxHitPoints), val1: MinHitpoints);
				}
			}
			bool isDealShield = true;
			if (ShieldHitPoints > 0 && attacker != null && base.manager.Player.Combat != null && attacker.HasTraitsThatContains("Equipment_Active_ShieldBreakerStrikeType1"))
			{
				FixedPoint value = 0.0;
				base.manager.Player.Combat.AbilityManager.VisitParameter("AbilityModifierShieldBreakerStrikeType1Parameter0", ref value, attacker);
				if (value > 0.0)
				{
					FixedPoint value2 = 0.0;
					base.manager.Player.Combat.AbilityManager.VisitParameter("ExtendProbability", ref value2, attacker);
					if (base.manager.Player.Combat.AbilityManager.manager.Player.RollDice(RollDiceType.IgnoreShield, value, value2) != PlayerRandomChanceResult.Failed)
					{
						isDealShield = false;
					}
				}
			}
			if (damageType == DamageType.DebuffDamagePerRound || damageType == DamageType.ShadowedGuard)
			{
				isDealShield = false;
			}
			if (HasTrait("TutorialInvulnerable"))
			{
				int struggleBaseThreshold = base.manager.GameEconomyData.ConfigData.StruggleBaseThreshold;
				if (Hitpoints <= struggleBaseThreshold)
				{
					SetHitpoints(struggleBaseThreshold + 1, DefenseSystemType.Shield, isDealShield, ChangeHitPointSource.DealDamage);
				}
			}
			SetHitpoints(Hitpoints - damage, DefenseSystemType.Shield, isDealShield, ChangeHitPointSource.DealDamage);
			int num = ((IsSegmentedHP && Hitpoints < 0) ? (-Hitpoints) : 0);
			if (Hitpoints < 0)
			{
				SetHitpoints(0, DefenseSystemType.None, isDealShield, ChangeHitPointSource.DealDamage);
			}
			if (Hitpoints > MaxHitPoints)
			{
				SetHitpoints(MaxHitPoints, DefenseSystemType.None, isDealShield, ChangeHitPointSource.DealDamage);
			}
			if (Hitpoints == 0 && IsSegmentedHP && SegmentedHPCount > 1)
			{
				int num2 = (SegmentedHPCount - 1) * MaxHitPoints - num;
				if (num2 > 0 && MaxHitPoints > 0)
				{
					SegmentedHPCount = (num2 + MaxHitPoints - 1) / MaxHitPoints;
					int val2 = num2 - (SegmentedHPCount - 1) * MaxHitPoints;
					SetHitpoints(val2, DefenseSystemType.None, IsDealShield: false);
					NotifyChange("ActorHealthChanged");
				}
				else
				{
					SegmentedHPCount = 1;
				}
			}
			LastHitAttacker = attacker;
			if (damageType != DamageType.Explosion)
			{
				LastHitExplosive = null;
			}
			if (Hitpoints > 0 || damageType != DamageType.Explosion)
			{
				AIController.ReceiveDamage(attacker, damageType);
			}
			NotifyChange("damageDealt");
			if (attacker != null)
			{
				attacker.HitsInTurn++;
			}
			if (Hitpoints == 0 && Faction == Faction.Walker && (Definition.InitialTraits.Contains("Walker.Whisperer") || Definition.InitialTraits.Contains("Walker.Whisperer.Melee")))
			{
				ShieldHitPoints = 0;
				SetHitpoints(1);
			}
			bool flag = this is TankActorModel && IsSegmentedHP && SegmentedHPCount > 1;
			if (Hitpoints == 0 && !flag)
			{
				if (IsSegmentedHP)
				{
					SegmentedHPCount = 0;
				}
				ActorModel actorModel = ExclusiveTimedEffect?.Instigator;
				FinishTimedEffect(interrupted: true);
				if (actorModel != null && actorModel != attacker && actorModel.ExclusiveTimedEffect != null && actorModel.ExclusiveTimedEffect.Target == this)
				{
					actorModel.FinishTimedEffect(interrupted: true);
				}
				if (attacker != null)
				{
					if (damageType == DamageType.Melee)
					{
						attacker.BloodThirst = true;
					}
					attacker.KillsInTurn++;
				}
				if (Faction == Faction.Survivor)
				{
					CombatModel combat = base.manager.Player.Combat;
					if (this is SurvivorModel survivorModel3 && combat != null && combat.IsEndlessBattleMission)
					{
						survivorModel3.SurvivedUntilWave = base.manager.CombatModel.EndlessModeCombatModel.GetCurrentOverAllWaveIndex;
					}
				}
				NotifyChange("actorKilledEvent", attacker);
			}
			if (attacker != null)
			{
				TraitEntry traitAnyLevel = attacker.TraitContainer.GetTraitAnyLevel("SurvivalManualStorySkill_F");
				if (traitAnyLevel != null && UpgradeTraitsData.GetTraitLevelIdentifier(traitAnyLevel.TraitIdentifier) > 0)
				{
					FixedPoint value3 = 0.0;
					base.manager.Player.AbilityManager.VisitParameter("SurvivalManualStorySkill_FParm1", ref value3, attacker);
					attacker.SharpBladeLayers += (int)value3;
					FixedPoint value4 = 0.0;
					base.manager.Player.AbilityManager.VisitParameter("SurvivalManualStorySkill_FParm2", ref value4, attacker);
					if (attacker.SharpBladeLayers >= (int)value4)
					{
						attacker.SharpBladeLayers = (int)value4;
					}
					attacker.NotifyChange("SurvivalManualStorySkill_F");
				}
			}
			if (hitpoints > Hitpoints)
			{
				NotifyReducedHitPoints();
			}
		}

		private void NotifyReducedHitPoints()
		{
			VengefulCharge_dmg();
		}

		public void Heal(int amountHealed)
		{
			if (Faction == Faction.Survivor || Faction == Faction.Raider)
			{
				SurvivorModel survivorModel = this as SurvivorModel;
				int num = 0;
				num = ((Hitpoints + amountHealed > MaxHitPoints) ? (MaxHitPoints - Hitpoints) : amountHealed);
				survivorModel?.Statistics.IncreaseTotalHealingReceivedInCombat(num);
				SetHitPoints(Hitpoints + num, MaxHitPoints);
			}
		}

		public void Explode(string traitId, string arg = null)
		{
			TraitDefinition traitDefinition = base.manager.GameEconomyData.GetTraitDefinition(traitId);
			if (traitDefinition == null)
			{
				return;
			}
			IsExploding = true;
			NotifyChange("actorExploded", arg);
			WalkerExplosionDefinition walkerExplosionDefinition = base.manager.GameEconomyData.GetWalkerExplosionDefinition(traitId);
			if (walkerExplosionDefinition != null)
			{
				FixedPoint parameter = walkerExplosionDefinition.GetParameter<FixedPoint>(1);
				int parameter2 = walkerExplosionDefinition.GetParameter<int>(3);
				base.manager.ExecuteAction(new NoiseAction(this, GridCoordinate, (int)parameter * 2, parameter2));
				base.manager.ExecuteAction(new ThreatAction(this, parameter2));
				RemoveTrait(traitDefinition.Identifier);
				if (GetWeaponEquipment() != null)
				{
					FixedPoint fixedPoint = CalculateExplosionDamage(walkerExplosionDefinition);
					List<ActorModel> actorsInRange = base.manager.CombatModel.GetActorsInRange(GridCoordinate, (int)parameter);
					for (int i = 0; i < actorsInRange.Count; i++)
					{
						ActorModel actorModel = actorsInRange[i];
						if (actorModel == this)
						{
							CombatHelpers.ExecuteDamage(base.manager.CombatModel, this, actorModel, actorModel.MaxHitPoints, 0, DamageType.Explode, PlayerRandomChanceResult.Failed, PlayerRandomChanceResult.Failed);
						}
						else
						{
							CombatHelpers.ExecuteDamage(base.manager.CombatModel, this, actorModel, (int)fixedPoint, 0, DamageType.Explosion, PlayerRandomChanceResult.Failed, PlayerRandomChanceResult.Failed);
						}
						if (ReflectionUtils.Instantiate(ReflectionUtils.FindDerivedType(typeof(BaseExplosionBehavior), walkerExplosionDefinition.EffectClass), null) is ExplosionBehavior explosionBehavior)
						{
							explosionBehavior.Execute(base.manager, walkerExplosionDefinition, this, actorModel);
						}
					}
				}
			}
			IsExploding = false;
		}

		public int CalculateExplosionDamage(WalkerExplosionDefinition explosionDefinition)
		{
			if (explosionDefinition == null)
			{
				return 0;
			}
			return (int)FixedPoint.Round(explosionDefinition.GetParameter<FixedPoint>(0) * (GetWeaponEquipment()?.Damage ?? 0) / 100.0);
		}

		public void DealExplosionDamage(int damage, ExplosiveModel explosive, ActorModel instigator)
		{
			LastHitExplosive = explosive;
			CombatHelpers.ExecuteDamage(base.manager.CombatModel, instigator, this, damage, 0, DamageType.Explosion, PlayerRandomChanceResult.Failed, PlayerRandomChanceResult.Failed);
		}

		public void Kill()
		{
			ShieldHitPoints = 0;
			SetHitpoints(0);
			if (IsSegmentedHP)
			{
				SegmentedHPCount = 0;
			}
			FinishTimedEffect(interrupted: true);
			EndFortifications(interrupted: true);
			NotifyChange("actorKilledEvent");
		}

		public void FinishScorchTimedEffect()
		{
			if (ScorchTimedEffect != null)
			{
				ScorchTimedEffect = null;
				UpdateModelObjects();
			}
		}

		public void FinishTauntTimedEffect()
		{
			if (TauntTimedEffect != null)
			{
				TauntTimedEffect = null;
				UpdateModelObjects();
			}
		}

		public void FinishShieldTimedEffect()
		{
			if (ShieldTimedEffect == null)
			{
				NotifyChange("ShieldChanged");
				return;
			}
			ChangeShieldHitPoints(-ShieldTimedEffect.Shield);
			ShieldTimedEffect = null;
			UpdateModelObjects();
		}

		public void CombatCleanSkillShieldType1TimedEffect()
		{
			SkillShieldType1TimedEffect?.PostFinishTimedEffect();
		}

		public void CombatCleanSkillEquipTauntTimedEffect()
		{
			SkillEquipTauntShieldTimedEffect?.PostFinishTimedEffect();
		}

		public void CombatCleanQuanTunTimedEffect()
		{
			if (CoexistTimedEffectsManager?.GetCoexistTimedEffect<QuantunTimedEffect>(CoexistTimedEffectType.Quantun) != null)
			{
				List<CoexistTimedEffectType> list = new List<CoexistTimedEffectType>();
				list.Add(CoexistTimedEffectType.Quantun);
				CoexistTimedEffectsManager?.RemoveCoexistTimedEffectByCoexistTimedEffectTypeList(list);
			}
		}

		public void FinishTimedEffect(bool interrupted)
		{
			if (ExclusiveTimedEffect == null)
			{
				return;
			}
			if (timedEffectEnding)
			{
				base.Debug.Log("ActorModel::FinishTimedEffect -> Already in FinishTimedEffect for this actor, cannot re-enter. Actor = " + this);
				return;
			}
			timedEffectEnding = true;
			ActorModel actorModel = null;
			TimedEffect exclusiveTimedEffect = ExclusiveTimedEffect;
			if (exclusiveTimedEffect.Type == TimedEffectType.Struggle)
			{
				TimedEffect exclusiveTimedEffect2 = ExclusiveTimedEffect;
				ActorModel actorModel2 = ((exclusiveTimedEffect2 != null) ? (exclusiveTimedEffect2.Target as ActorModel) : null);
				if (actorModel2 != null && !actorModel2.IsDead)
				{
					actorModel2.FinishTimedEffect(interrupted);
					actorModel2.NotifyStruggleFinished();
					if (interrupted)
					{
						actorModel2.NotifyChange("actorStruggleSaved");
					}
				}
				RemoveTrait("StruggleInvulnerable");
				if (!interrupted)
				{
					actorModel = actorModel2;
				}
			}
			else if (exclusiveTimedEffect.Type == TimedEffectType.BleedOut)
			{
				RemoveTrait("StruggleInvulnerable");
				if (base.manager.CombatModel.HasPvPRules)
				{
					NotifyBleedingOutFinished();
				}
				else if (!interrupted)
				{
					actorModel = this;
				}
			}
			else if (exclusiveTimedEffect.Type == TimedEffectType.InteractingWithObject)
			{
				if (exclusiveTimedEffect.Target is InteractiveObjectModel interactiveObjectModel)
				{
					if (interrupted)
					{
						NotifyChange("actorInteractionInterrupting", interactiveObjectModel);
						interactiveObjectModel.CancelInteraction(this);
					}
					else
					{
						NotifyChange("actorInteractionCompleting", interactiveObjectModel);
						interactiveObjectModel.CompleteInteraction(this);
					}
				}
			}
			else if (exclusiveTimedEffect.Type == TimedEffectType.Lure)
			{
				OnLureEffectEnd();
				if (IsFlare)
				{
					base.manager.CombatModel.Perceptors.Models.Remove(this);
					base.manager.CombatModel.UpdateAllActorsVisibility();
					base.manager.CombatModel.UpdateObjectsVisibility();
				}
			}
			else if (exclusiveTimedEffect.Type == TimedEffectType.EatingLure)
			{
				TurnConsumedByEatingLure = true;
			}
			else if (exclusiveTimedEffect.Type == TimedEffectType.ElectricShock)
			{
				DebuffParameterManager?.RemoveParametersByParameterKey("ElectronShockAsElectronChargeLayer");
			}
			ExclusiveTimedEffect = null;
			NotifyChange("actorTimedEffectEnd", new object[2] { exclusiveTimedEffect, interrupted });
			if (PendingExclusiveTimedEffect != null)
			{
				ExclusiveTimedEffect = PendingExclusiveTimedEffect;
				PendingExclusiveTimedEffect = null;
				NotifyChange("actorTimedEffectStart", ExclusiveTimedEffect);
			}
			timedEffectEnding = false;
			if (actorModel != null)
			{
				CombatHelpers.ExecuteDamage(base.manager.CombatModel, this, actorModel, actorModel.Hitpoints, 0, DamageType.Struggle, PlayerRandomChanceResult.Failed, PlayerRandomChanceResult.Failed);
			}
			UpdateModelObjects();
		}

		public void StartScorchTimedEffect(ScorchTimedEffect timedEffect, ActorModel instigator)
		{
			FixedPoint value = 0.0;
			base.manager.Player.AbilityManager.VisitParameter("AbilityModifierIncreaseExtraScorchDamageChance", ref value, instigator);
			FixedPoint value2 = 0.0;
			base.manager.Player.AbilityManager.VisitParameter("AbilityModifierIncreaseScorchLayers", ref value2, instigator);
			if (ScorchTimedEffect != null)
			{
				int num = ScorchTimedEffect.Layers + 1;
				if (num <= value2)
				{
					ScorchTimedEffect.Layers = num;
					ScorchTimedEffect.DamageChance += (int)value;
				}
			}
			else
			{
				ScorchTimedEffect = timedEffect;
				ScorchTimedEffect.DamageChance = (int)value;
				ScorchTimedEffect.MaxLayers = (int)value2;
			}
		}

		public void StartTauntTimedEffect(TimedEffect timedEffect)
		{
			if (timedEffect == null)
			{
				return;
			}
			if (TauntTimedEffect != null)
			{
				int num = TauntTimedEffect.Duration - TauntTimedEffect.Counter;
				if (timedEffect.Duration - timedEffect.Counter > num)
				{
					TauntTimedEffect = timedEffect;
				}
			}
			else
			{
				TauntTimedEffect = timedEffect;
			}
		}

		public void StartShieldTimedEffect(ShieldTimedEffect timedEffect, int shield)
		{
			if (ShieldTimedEffect != null && timedEffect != null)
			{
				FinishShieldTimedEffect();
			}
			if (timedEffect != null)
			{
				ShieldTimedEffect = timedEffect;
				ShieldTimedEffect.Shield = shield;
				ChangeShieldHitPoints(shield);
			}
		}

		public void StartTimedEffect(TimedEffect timedEffect)
		{
			switch (timedEffect.ExistType)
			{
			case TimedEffectExistType.Replace:
				if (ExclusiveTimedEffect != null && ExclusiveTimedEffect.Type == timedEffect.Type && ExclusiveTimedEffect.Duration > timedEffect.Duration)
				{
					break;
				}
				if (ExclusiveTimedEffect != null || timedEffectEnding)
				{
					PendingExclusiveTimedEffect = timedEffect;
					if (!timedEffectEnding)
					{
						FinishTimedEffect(interrupted: true);
					}
				}
				else
				{
					ExclusiveTimedEffect = timedEffect;
					NotifyChange("actorTimedEffectStart", timedEffect);
				}
				break;
			case TimedEffectExistType.Coexist:
				CoexistTimedEffectsManager.StartTimedEffect(timedEffect as CoexistTimedEffectAbstract);
				break;
			}
		}

		public void StartBleedOut(ActorModel instigator, bool giveFullHealth = false)
		{
			if (!(!HasTrait("Bleeding") || giveFullHealth))
			{
				return;
			}
			if (giveFullHealth && !(this is TankActorModel))
			{
				SetHitPoints(MaxHitPoints, MaxHitPoints);
				OnRedHealthBar = true;
				EndFortifications(interrupted: true);
			}
			if (instigator != null)
			{
				bool flag = (instigator.Faction == Faction.Raider || instigator.Faction == Faction.Survivor) && instigator.HasAnyLevelTrait("DebuffMarkEnemy");
				if (!HasTrait("Bleeding") && !flag)
				{
					AddTemporaryTrait("Bleeding", default(FixedPoint), null, 0L);
				}
			}
		}

		public void StartBurningOut(bool onRedHealthBar, int burnTurns = 0)
		{
			if (HasTrait("Burning"))
			{
				return;
			}
			if (onRedHealthBar && !(this is TankActorModel))
			{
				SetHitPoints(MaxHitPoints, MaxHitPoints);
				OnRedHealthBar = true;
				EndFortifications(interrupted: true);
			}
			int num = burnTurns;
			IChallengeDebuffProvider challengeDebuffProvider = MapMissionDebuffHelper.CanUseDebuffMission(base.manager);
			if (challengeDebuffProvider != null)
			{
				FixedPoint debufMinFirstParam = ChallengeDebufHelps.GetDebufMinFirstParam(challengeDebuffProvider.GetChallengeDebuffs(), ChallengeDebuffType.DebuffFireLast);
				if (debufMinFirstParam != FixedPoint.MaxValue)
				{
					num = (int)debufMinFirstParam;
				}
			}
			AddTemporaryTrait("Burning", default(FixedPoint), null, num);
		}

		public void StartSkinned(int turns)
		{
			if (!HasTrait("Skinned"))
			{
				AddTemporaryTrait("Skinned", default(FixedPoint), null, turns);
			}
		}

		public void StartStagger(int turns, ActorModel target, FixedPoint chance)
		{
			if (target != null)
			{
				int num = ((target.Faction == base.manager.CombatModel.TurnManager.ActiveFaction) ? 1 : 0);
				target.AddTemporaryTrait("StaggerActive", chance - 100L, null, turns + num);
			}
		}

		public void StartRemoteWeaken(int turns, ActorModel target)
		{
			if (target != null)
			{
				int num = ((target.Faction == base.manager.CombatModel.TurnManager.ActiveFaction) ? 1 : 0);
				target.AddTemporaryTrait("RemoteWeakenActiveFlag", default(FixedPoint), null, turns + num);
			}
		}

		public void StartStruggle(ActorModel target, int turns)
		{
			EndFortifications(interrupted: true);
			target?.EndFortifications(interrupted: true);
			int num = ((Faction != base.manager.CombatModel.TurnManager.ActiveFaction) ? 1 : 0);
			StartTimedEffect(new TimedEffect(TimedEffectType.Struggle, turns + num, 0, this, target));
			if (target != null && !(target is TankActorModel))
			{
				if (target.Faction == Faction.Survivor && !target.HasTrait("StruggleInvulnerable") && !base.manager.CombatModel.HasPvPRules)
				{
					target.AddTemporaryTrait("StruggleInvulnerable", default(FixedPoint), null, 0L);
				}
				target.SetHitPoints(target.MaxHitPoints, target.MaxHitPoints);
				target.OnRedHealthBar = true;
				target.StartTimedEffect(new TimedEffect(TimedEffectType.Struggle, turns + num, 0, this, null));
				target.EndAction();
			}
		}

		public void NotifyStruggleFinished()
		{
			TurnState = TurnState.Idle;
			ResetActionPoints();
			NotifyChange("actorStruggleFinished");
		}

		public void NotifyBleedingOutFinished()
		{
			TurnState = TurnState.Idle;
			ResetActionPoints();
			NotifyChange("actorBleedingOutSaved");
		}

		public void Stun(int turns, ActorModel instigator)
		{
			int num = ((instigator.Faction != base.manager.CombatModel.TurnManager.ActiveFaction) ? 1 : 0);
			StartTimedEffect(new TimedEffect(TimedEffectType.Stun, turns + num, 0, instigator.Faction));
			EndAction();
			NotifyChange("actorStunnedEvent");
		}

		public void StartQuantun(int turns, ActorModel instigator, FixedPoint baseDamagePercentage, FixedPoint additionalDamagePercentage, int maxLayer, FixedPoint canNotActionPercentage)
		{
			int num = ((instigator.Faction != base.manager.CombatModel.TurnManager.ActiveFaction) ? 1 : 0);
			StartTimedEffect(new QuantunTimedEffect(turns + num, 0, instigator, this, baseDamagePercentage, additionalDamagePercentage, maxLayer, canNotActionPercentage));
			EndAction();
			NotifyChange("ActorQuantunUpdate");
		}

		public void StartShieldBreaker(int turns, ActorModel instigator)
		{
			int num = ((instigator.Faction != base.manager.CombatModel.TurnManager.ActiveFaction) ? 1 : 0);
			StartTimedEffect(new ShieldBreakerTimedEffect(turns + num, 0, instigator, this));
			EndAction();
			NotifyChange("ActorShieldBreakerUpdate");
		}

		public void StartQuantunCanNotMove(int turns)
		{
			StartTimedEffect(new TimedEffect(TimedEffectType.QuantunCanNotMove, turns, 0, Faction));
			EndAction();
			NotifyChange("ActorQuantunCanNotMoveEvent");
		}

		public void StartMomentum(ActorModel instigator, int addLayer, FixedPoint addDamagePercentageBase, FixedPoint reduceEnemyDodgePercentageBase, FixedPoint reduceEnemyDamageReductionBase, int maxLayer)
		{
			StartTimedEffect(new MomentumTimedEffect(-1, 0, instigator, this, addLayer, addDamagePercentageBase, reduceEnemyDodgePercentageBase, reduceEnemyDamageReductionBase, maxLayer));
			EndAction();
			NotifyChange("ActorMomentumUpdate");
		}

		public void StartDebuffDamagePerRound(int param0, int param1, int param2, FixedPoint param3)
		{
			StartTimedEffect(new DebuffDamagePerRoundTimedEffect(-1, 0, this, this, param0, param1, param2, param3));
			NotifyChange("ActorDebuffDamagePerRoundUpdate");
		}

		public void StartDebuffReduceRecovery(int param0, int param1)
		{
			StartTimedEffect(new DebuffReduceRecoveryTimedEffect(-1, 0, this, this, param0, param1));
			NotifyChange("ActorDebuffReduceRecoveryUpdate");
		}

		public void StartElectricShock(int turns, ActorModel instigator, int asElectronChargeLayer)
		{
			int num = ((instigator.Faction != base.manager.CombatModel.TurnManager.ActiveFaction) ? 1 : 0);
			StartTimedEffect(new TimedEffect(TimedEffectType.ElectricShock, turns + num, 0, instigator.Faction));
			EndAction();
			ElectronChargeDebuffParameter newDebuffParameter = new ElectronChargeDebuffParameter("ElectronShockAsElectronChargeLayer", asElectronChargeLayer, turns + num);
			DebuffParameterManager.RemoveAllAndAddNewParameterByParameterKey(newDebuffParameter);
			NotifyChange("ActorElectricShockedEvent");
		}

		public void StartUnLucky(int turns, ActorModel instigator, string traitId)
		{
			int num = ((instigator.Faction != base.manager.CombatModel.TurnManager.ActiveFaction) ? 1 : 0);
			StartTimedEffect(new UnluckyTimedEffect(turns + num, 0, instigator, this, traitId));
			EndAction();
		}

		public void Scorch(int turns, ActorModel instigator)
		{
			int num = 0;
			StartScorchTimedEffect(new ScorchTimedEffect(TimedEffectType.Scorch, turns + num, 0, instigator.Faction), instigator);
			EndAction();
		}

		public void StartBerserkRage(int turns, ActorModel instigator, int layer, int baseRageLayer, FixedPoint additionDamageMultiplier)
		{
			StartTimedEffect(new BerserkRageTimedEffect(turns, 0, instigator, this, layer, baseRageLayer, additionDamageMultiplier));
			NotifyChange("ActorRageUpdateEvent");
		}

		public void Root(int turns, ActorModel instigator)
		{
			int num = ((instigator.Faction != base.manager.CombatModel.TurnManager.ActiveFaction) ? 1 : 0);
			StartTimedEffect(new TimedEffect(TimedEffectType.Root, turns + num, 0, instigator, null));
			if (base.manager.CombatModel.TurnManager.ActiveFaction == Faction)
			{
				EndAction();
			}
			NotifyChange("actorRootedEvent");
		}

		public void Pitfall(int turns, ActorModel instigator)
		{
			int num = ((instigator.Faction != base.manager.CombatModel.TurnManager.ActiveFaction) ? 1 : 0);
			StartTimedEffect(new TimedEffect(TimedEffectType.Pitfall, turns + num, 0, instigator, null));
			if (base.manager.CombatModel.TurnManager.ActiveFaction == Faction)
			{
				EndAction();
			}
			NotifyChange("actorRootedEvent");
		}

		public void Cripple(int turns, ActorModel instigator)
		{
			int num = ((instigator.Faction != base.manager.CombatModel.TurnManager.ActiveFaction) ? 1 : 0);
			StartTimedEffect(new TimedEffect(TimedEffectType.Crippled, turns + num, 0, instigator, null));
			if (base.manager.CombatModel.TurnManager.ActiveFaction == Faction)
			{
				EndAction();
			}
			NotifyChange("actorCrippledEvent");
		}

		public void Herd(int turns, ActorModel instigator)
		{
			int num = ((instigator.Faction != base.manager.CombatModel.TurnManager.ActiveFaction) ? 1 : 0);
			StartTimedEffect(new TimedEffect(TimedEffectType.Herd, turns + num, 0, instigator, null));
			EndAction();
			NotifyChange("actorTauntedEvent");
		}

		public void SetInvisible(int turns, ActorModel instigator)
		{
			int num = ((instigator.Faction != base.manager.CombatModel.TurnManager.ActiveFaction) ? 1 : 0);
			TraitEntry trait = TraitContainer.GetTrait("Gore");
			if (trait == null || trait.TraitDuration < turns + num)
			{
				long duration = turns + num;
				AddTemporaryTrait("Gore", default(FixedPoint), null, duration);
			}
			AdditionalMoveRange = 0;
			NotifyChange("actorSetInvisibleEvent");
		}

		public void StartEatLure(ActorModel lure, int turns)
		{
			if (Faction == Faction.Walker)
			{
				int num = ((Faction != base.manager.CombatModel.TurnManager.ActiveFaction) ? 1 : 0);
				StartTimedEffect(new TimedEffect(TimedEffectType.EatingLure, turns + num, 0, this, null, lure.GridCoordinate));
			}
		}

		public bool IsEnemy(ActorModel actor)
		{
			bool result = false;
			switch (Faction)
			{
			case Faction.Survivor:
				result = actor.Faction == Faction.Walker || actor.Faction == Faction.Raider || actor.Faction == Faction.Dormant || actor.Faction == Faction.Environmental;
				break;
			case Faction.Environmental:
				result = actor.Faction == Faction.Survivor || actor.Faction == Faction.Raider || actor.Faction == Faction.Civilian || actor.Faction == Faction.Lure || actor.Faction == Faction.Walker;
				break;
			case Faction.Walker:
				result = (((!actor.IsWalker || !actor.IsDisoriented) && !IsDisoriented) ? ((!actor.IsABTesterAed && !IsABTesterAed) ? (actor.Faction == Faction.Survivor || actor.Faction == Faction.Raider || actor.Faction == Faction.Civilian || actor.Faction == Faction.Lure) : (actor.Faction == Faction.Walker || actor.Faction == Faction.Survivor || actor.Faction == Faction.Raider || actor.Faction == Faction.Civilian || actor.Faction == Faction.Lure)) : (actor.Faction == Faction.Walker || actor.Faction == Faction.Survivor || actor.Faction == Faction.Raider || actor.Faction == Faction.Civilian || actor.Faction == Faction.Lure));
				break;
			case Faction.Dormant:
				result = actor.Faction == Faction.Survivor || actor.Faction == Faction.Raider || actor.Faction == Faction.Civilian || actor.Faction == Faction.Lure;
				break;
			case Faction.Raider:
				result = ((!actor.IsABTesterAed && !IsABTesterAed) ? (actor.Faction == Faction.Survivor || actor.Faction == Faction.Walker || actor.Faction == Faction.Civilian || actor.Faction == Faction.Dormant || actor.Faction == Faction.Environmental) : (actor.Faction == Faction.Survivor || actor.Faction == Faction.Raider || actor.Faction == Faction.Walker || actor.Faction == Faction.Civilian || actor.Faction == Faction.Dormant || actor.Faction == Faction.Environmental));
				break;
			case Faction.Civilian:
				result = actor.Faction == Faction.Walker || actor.Faction == Faction.Raider || actor.Faction == Faction.Dormant;
				break;
			case Faction.Lure:
				result = actor.Faction == Faction.Walker || actor.Faction == Faction.Dormant;
				break;
			}
			return result;
		}

		public override void Tick(long deltaTime)
		{
			base.Tick(deltaTime);
			if (Producer != null)
			{
				Producer.Tick(deltaTime);
			}
		}

		public override bool IsValid()
		{
			return true;
		}

		public static ActorModel Create(Faction faction)
		{
			switch (faction)
			{
			case Faction.Survivor:
				return new SurvivorModel(1, 0);
			case Faction.Walker:
			case Faction.Dormant:
			case Faction.Environmental:
				return new WalkerModel();
			case Faction.Civilian:
				return new CivilianModel();
			case Faction.Raider:
				return new RaiderModel();
			case Faction.Lure:
				return new ActorModel();
			default:
				return new ActorModel();
			}
		}

		protected AIController CreateAIController()
		{
			if (Definition.IsEnvironmental)
			{
				return new EnvironmentalActorController(this);
			}
			return Faction switch
			{
				Faction.Survivor => new SurvivorController(this),
				Faction.Walker => new WalkerController(this),
				Faction.Dormant => new DormantController(this),
				Faction.Civilian => new CivilianController(this),
				Faction.Raider => new RaiderController(this),
				_ => new DefaultController(this),
			};
		}

		public void IncrementStunnedTimedEffects()
		{
			if (ExclusiveTimedEffect != null && ExclusiveTimedEffect.Type == TimedEffectType.Stun)
			{
				ExclusiveTimedEffect.Counter++;
				if (ExclusiveTimedEffect.Counter >= ExclusiveTimedEffect.Duration)
				{
					FinishTimedEffect(interrupted: false);
				}
				else
				{
					NotifyChange("actorTimedEffectUpdated", ExclusiveTimedEffect);
				}
			}
		}

		public void IncrementTimedEffects(Faction faction)
		{
			if (ScorchTimedEffect != null && ((ScorchTimedEffect.InstigatorFaction == faction && ScorchTimedEffect.Type != TimedEffectType.Lure) || (ScorchTimedEffect.Type == TimedEffectType.Lure && faction == Faction.Walker)))
			{
				ScorchTimedEffect.Counter++;
				if (ScorchTimedEffect.Counter >= ScorchTimedEffect.Duration)
				{
					FinishScorchTimedEffect();
				}
			}
			if (TauntTimedEffect != null && ((TauntTimedEffect.InstigatorFaction == faction && TauntTimedEffect.Type != TimedEffectType.Lure) || (TauntTimedEffect.Type == TimedEffectType.Lure && faction == Faction.Walker)))
			{
				TauntTimedEffect.Counter++;
				if (TauntTimedEffect.Counter >= TauntTimedEffect.Duration)
				{
					FinishTauntTimedEffect();
				}
			}
			if (ShieldTimedEffect != null && ((ShieldTimedEffect.InstigatorFaction == faction && ShieldTimedEffect.Type != TimedEffectType.Lure) || (ShieldTimedEffect.Type == TimedEffectType.Lure && faction == Faction.Walker)))
			{
				ShieldTimedEffect.Counter++;
				if (ShieldTimedEffect.Counter >= ShieldTimedEffect.Duration)
				{
					FinishShieldTimedEffect();
				}
			}
			CoexistTimedEffectsManager?.IncrementTimedEffects(faction);
			if (ExclusiveTimedEffect == null || ((ExclusiveTimedEffect.InstigatorFaction != faction || ExclusiveTimedEffect.Type == TimedEffectType.Lure) && (ExclusiveTimedEffect.Type != TimedEffectType.Lure || faction != Faction.Walker)))
			{
				return;
			}
			ExclusiveTimedEffect.Counter++;
			if (ExclusiveTimedEffect.Counter >= ExclusiveTimedEffect.Duration)
			{
				FinishTimedEffect(interrupted: false);
				return;
			}
			if (ExclusiveTimedEffect.Type == TimedEffectType.InteractingWithObject && ExclusiveTimedEffect.Target is InteractiveObjectModel interactiveObjectModel)
			{
				interactiveObjectModel.InteractStep(this, ExclusiveTimedEffect.Counter, ExclusiveTimedEffect.Duration);
			}
			NotifyChange("actorTimedEffectUpdated", ExclusiveTimedEffect);
		}

		public virtual bool CheckTimedEffectsEndByTraits(Faction activeFaction)
		{
			if (ExclusiveTimedEffect != null && ExclusiveTimedEffect.InstigatorFaction == Faction.Walker && ExclusiveTimedEffect.Type == TimedEffectType.Struggle)
			{
				FixedPoint value = 0.0;
				PlayerRandomChanceResult playerRandomChanceResult = PlayerRandomChanceResult.Failed;
				if (base.manager.Player.AbilityManager.VisitParameter("AbilityModifierKillOnStruggle", ref value, ExclusiveTimedEffect.Instigator))
				{
					playerRandomChanceResult = base.manager.Player.RollDice(RollDiceType.Damage, value);
				}
				if (playerRandomChanceResult != PlayerRandomChanceResult.Failed)
				{
					ActorModel instigator = ExclusiveTimedEffect.Instigator;
					ActorModel actorModel = ExclusiveTimedEffect.Target as ActorModel;
					if (instigator != null && actorModel != null)
					{
						CombatHelpers.ExecuteDamage(base.manager.CombatModel, instigator, actorModel, actorModel.Hitpoints, 0, DamageType.Melee, PlayerRandomChanceResult.Failed, PlayerRandomChanceResult.Failed);
						return true;
					}
				}
			}
			return false;
		}

		public void ChangeFaction(Faction faction)
		{
			AIAlertness alertness = AIDataModel.Alertness;
			if (faction == Faction.Tutorial)
			{
				Abilities.Clear();
			}
			AIDataModel.Alertness = alertness;
			OriginalFaction = Faction;
			ActorFactionChangedInCombat = true;
			Faction = faction;
			AIController = CreateAIController();
		}

		protected virtual void CreateAbilities()
		{
			ModelList<AbilityModel> modelList = new ModelList<AbilityModel>();
			modelList.SetManager(base.manager);
			if (Definition != null && Definition.InitialAbilities != null)
			{
				for (int i = 0; i < Definition.InitialAbilities.Count; i++)
				{
					string identifier = Definition.InitialAbilities[i];
					modelList.Add(CreateAbility(identifier, 1));
				}
			}
			List<TraitDefinition> traitsWithTag = GetTraitsWithTag("Ability");
			for (int j = 0; j < traitsWithTag.Count; j++)
			{
				string parameter = traitsWithTag[j].GetParameter<string>(0);
				AbilityModel abilityModel = CreateAbility(parameter, -1);
				abilityModel.Initialize();
				modelList.Add(abilityModel);
			}
			SetAbilities(modelList);
			Abilities.SetManager(base.manager);
			Abilities.Start();
		}

		private AbilityModel CreateAbility(string identifier, int maxUses)
		{
			AbilityModel abilityModel = new AbilityModel();
			abilityModel.SetManager(base.manager);
			abilityModel.DefinitionID = identifier;
			abilityModel.TotalUses = 0;
			abilityModel.MaxUses = maxUses;
			return abilityModel;
		}

		private void InitializeAttributes()
		{
			int num = 0;
			if (Definition != null)
			{
				num = Definition.InitialHealth;
				ActivationRange = Definition.InitialActivationRange;
				MoveRange = Definition.InitialMovementSpeed;
				StrugglesLeft = 1;
			}
			ActorLevelDefinition actorLevelDefinition = GetActorLevelDefinition(ActorDefinitionID, Level);
			if (actorLevelDefinition != null && this is TankActorModel)
			{
				num = actorLevelDefinition.GuildBossHP;
				if (!IsSegmentedHP)
				{
					IsSegmentedHP = actorLevelDefinition.SegmentedHP;
					if (IsSegmentedHP && actorLevelDefinition.HPBarQuantity > 0)
					{
						SegmentedHPMax = actorLevelDefinition.HPBarQuantity;
						SegmentedHPCount = SegmentedHPMax;
					}
				}
			}
			SetHitPoints(num, num);
		}

		private void ResetActionPoints()
		{
			MoveCompleted = false;
			SecondMoveCompleted = false;
			AbilityCompleted = false;
			NotifyChange("RefreshCommandSkill");
		}

		public void ResetActionPointsForExternal(bool resetTurnState = true)
		{
			if (!IsInteractingWithObject && resetTurnState)
			{
				TurnState = TurnState.Idle;
			}
			ResetActionPoints();
			NotifyChange("actorExtraAbilityAction");
		}

		public virtual void ConfigureBaseAttributes()
		{
		}

		public void UseInteractiveObject(InteractiveObjectModel target)
		{
			if (target.TurnsToComplete > 1 && target.InteractBy != InteractBy.Shoot)
			{
				StartTimedEffect(new TimedEffect(TimedEffectType.InteractingWithObject, target.TurnsToComplete, 0, this, target));
				target.InteractStep(this, ExclusiveTimedEffect.Counter, ExclusiveTimedEffect.Duration);
			}
			else
			{
				target.InteractStep(this, 0, 1);
				target.CompleteInteraction(this);
			}
		}

		public void StartReloading()
		{
			SelectedEquipment.StartReloading(this);
		}

		public void UpdateReloading()
		{
			SelectedEquipment.UpdateReloading(this);
		}

		public virtual int[] GetSPGain(ActorModel attacker, bool shouldCap = false)
		{
			FixedPoint fixedPoint = 0.0;
			FixedPoint fixedPoint2 = 0.0;
			ActorLevelDefinition actorLevelDefinition = GetActorLevelDefinition(ActorDefinitionID, Level);
			if (actorLevelDefinition != null)
			{
				fixedPoint = ((!base.manager.Player.ActivityManager.IsActivityOpen(ActivityType.DoubleXPFromKills)) ? ((FixedPoint)actorLevelDefinition.SPGain) : ((FixedPoint)actorLevelDefinition.EventSPGain));
				if (base.manager != null && base.manager.Player != null && base.manager.Player.Combat != null)
				{
					fixedPoint *= base.manager.Player.Combat.SpMultiplier;
				}
				if (attacker != null)
				{
					FixedPoint value = 1.0;
					base.manager.Player.AbilityManager.VisitParameter("AbilityModifierPercentageMultiplyKillSP", ref value, attacker);
					fixedPoint2 = fixedPoint * value - fixedPoint;
				}
				if (shouldCap && base.gameEconomyData != null && base.gameEconomyData.ConfigData != null)
				{
					fixedPoint = FixedPoint.Min(fixedPoint, new FixedPoint(base.gameEconomyData.ConfigData.MissionKillAfterMaxGivenXP));
					fixedPoint2 = 0.0;
				}
			}
			return new int[2]
			{
				(int)Math.Round((float)fixedPoint),
				(int)Math.Round((float)fixedPoint2)
			};
		}

		protected virtual ActorLevelDefinition GetActorLevelDefinition(string definitionId, int level)
		{
			return base.manager.GameEconomyData.GetActorLevelDefinition(definitionId, level);
		}

		public int GetSuppliesGain(ActorModel attacker, int spGain)
		{
			FixedPoint fixedPoint = 0.0;
			spGain = (int)(spGain * base.manager.Player.GetCurrency(CurrencyType.SurvivalPoints).AddMultiplier);
			if (spGain != 0 && attacker != null)
			{
				FixedPoint value = 0.0;
				base.manager.Player.AbilityManager.VisitParameter("AbilityModifierPercentageMultiplyKillSupplies", ref value, attacker);
				fixedPoint = spGain * value;
			}
			return (int)Math.Round((float)fixedPoint);
		}

		public void GiveSP(int amount)
		{
			CurrencyModel currency = base.manager.Player.GetCurrency(CurrencyType.SurvivalPoints);
			if (currency != null)
			{
				currency.Add(amount);
				NotifyChange("actorReceivedSP", amount);
			}
		}

		public void AddChargePoints(int amount)
		{
			if (ChargeMeter.ChargeLevel < ChargeMeter.MaxLevel)
			{
				ChargeMeter.ChangeChargeLevel(amount);
				NotifyChange("ActorReceivedChargePoint", amount);
				NotifyChange("EquipmentActiveChargeLoadEvent");
			}
			OnChargePointGainedForRage();
		}

		public void RemoveTemporaryTraits()
		{
			List<string> list = new List<string>();
			for (int i = 0; i < ((TraitContainer.Traits != null) ? TraitContainer.Traits.Count : 0); i++)
			{
				TraitEntry traitEntry = TraitContainer.Traits[i];
				if (traitEntry.IsTemporary)
				{
					list.Add(traitEntry.TraitIdentifier);
				}
			}
			for (int j = 0; j < list.Count; j++)
			{
				RemoveTrait(list[j]);
			}
		}

		public void CheckForBonusActionPoint()
		{
			if (IsInteractingWithObject || HasGainedExtraAPFromInteraction || IsDead || AIController.IsActorIncapacitated || IsInvisible)
			{
				return;
			}
			if (base.manager.Player.Combat != null)
			{
				FixedPoint value = 0.0;
				if (base.manager.Player.AbilityManager.VisitParameter("LeaderBuffLooter", ref value, this))
				{
					EnsureExtraAction("ActorNotification.LeaderBuffLooter", dueToLuck: false);
					HasGainedExtraAPFromInteraction = true;
					NotifyChange("AbilityVisited", new object[2] { "LeaderBuffLooter", false });
				}
			}
			if (!HasGainedExtraAP && AdditionalAttackCount > 0)
			{
				AdditionalAttackCount = 0;
			}
		}

		public bool GetIsUsingAdditionalAttacks()
		{
			bool isUsingExtraAPChargeEquipment = GetIsUsingExtraAPChargeEquipment();
			if (CanMoveWithoutAttacking && !GainedAPFromPreviousAbilityExecution && AdditionalAttackCount > 0 && !isUsingExtraAPChargeEquipment && !PassByAttackedOnMove)
			{
				return !freeAttackUsed;
			}
			return false;
		}

		public bool GetIsUsingExtraAPChargeEquipment()
		{
			bool result = false;
			if (SelectedEquipment != null && SelectedEquipment.EquipmentDefinitionIdentifier != null && base.manager.GameEconomyData.GetEquipmentDefinition(SelectedEquipment.EquipmentDefinitionIdentifier).ActiveTraits != null)
			{
				result = SelectedEquipment.IsChargeEquipment && base.manager.GameEconomyData.GetEquipmentDefinition(SelectedEquipment.EquipmentDefinitionIdentifier).ActiveTraits.Any((string t) => t.Contains("Equipment_Active_ExtraAP"));
			}
			return result;
		}

		public override string ToString()
		{
			ActorModel actorModel = ((AIDataModel != null) ? AIDataModel.GetCurrentTarget() : null);
			GridCoordinate gridCoordinate = ((AIDataModel != null) ? AIDataModel.GetGridCoordinate(AIDataModel.MoveToCoordinate) : GridCoordinate.Invalid);
			return string.Concat(string.Concat(string.Concat(string.Concat(string.Concat(Name + (GridCoordinate.IsValid ? (" @ " + GridCoordinate.ToString()) : ""), " [", AIDataModel.Alertness.ToString(), "]"), " Targeting { ", (actorModel != null) ? (actorModel.Name + " " + actorModel.GridCoordinate.ToString()) : "null", " }"), " MoveToTarget @ ", gridCoordinate.IsValid ? gridCoordinate.ToString() : "Invalid"), " Faction = ", Faction.ToString()), " IsDead = ", IsDead.ToString());
		}

		private void OnLureEffectEnd()
		{
			ShieldHitPoints = 0;
			SetHitpoints(0);
			NotifyChange("actorKilledEvent");
		}

		public void BuffKnockKnockChargePoint(ActorModel target)
		{
			if (TraitContainer.GetTraitAnyLevel("BaseKnockKnock") == null)
			{
				return;
			}
			CombatModel combat = base.manager.Player.Combat;
			if (combat != null && !combat.MissionCompleted)
			{
				FixedPoint value = 0.0;
				combat.AbilityManager.VisitParameter("LeaderBuffKnockKnockExtraChargePointConfig", ref value, this);
				if (IsEnemy(target) && !target.IsEnvironmental && !target.IsDead && target.OneTurnAttackedTimes >= value)
				{
					LeaderBuffKnockKnocRollChargePoint();
				}
			}
		}

		private void LeaderBuffKnockKnocRollChargePoint()
		{
			CombatModel combat = base.manager.Player.Combat;
			if (combat != null && !combat.MissionCompleted)
			{
				FixedPoint value = 0.0;
				combat.AbilityManager.VisitParameter("LeaderBuffKnockKnockExtraChargePointChance", ref value, this);
				FixedPoint value2 = 0.0;
				combat.AbilityManager.VisitParameter("ExtendProbability", ref value2, this);
				PlayerRandomChanceResult playerRandomChanceResult = combat.AbilityManager.manager.Player.RollDice(RollDiceType.FollowThrough, value, value2);
				if (playerRandomChanceResult != PlayerRandomChanceResult.Failed && ChargeMeter.ChargeLevel < ChargeMeter.MaxLevel)
				{
					AddChargePoints(1);
					NotifyChange("AbilityVisited", new object[2]
					{
						"LeaderBuffKnockKnock",
						playerRandomChanceResult == PlayerRandomChanceResult.SuccessDueToExtension
					});
				}
			}
		}

		public void UpdateTurnFaction(Faction previousFaction, Faction activeFaction)
		{
			if (activeFaction == Faction.Survivor)
			{
				DebuffKnockKnockMarkCount = 0L;
				OneTurnAttackedTimes = 0L;
				base.manager.Player.Combat.ClearDisorientedModel();
				DisorientLockActor = null;
				IsRecoilEffected = false;
				ResetABtestParam();
			}
			NotifyChange("UpdateTurnFactionEvent");
		}

		public void Taunt(int turns, ActorModel source)
		{
			int num = ((source.Faction != base.manager.CombatModel.TurnManager.ActiveFaction) ? 1 : 0);
			AIController.ClearFollowTarget();
			AIDataModel.SetCurrentTarget(source);
			AIDataModel.Alertness = AIAlertness.Homing;
			StartTauntTimedEffect(new TimedEffect(TimedEffectType.Taunt, turns + num, 0, source));
		}

		public void Shield(int turns, int shield, ActorModel source)
		{
			int num = 0;
			StartShieldTimedEffect(new ShieldTimedEffect(TimedEffectType.Shield, turns + num, 0, source.Faction), shield);
		}

		public void StartSkillShieldType1(int turns, int shield, ActorModel source)
		{
			StartTimedEffect(new SkillShieldType1TimedEffect(turns, 0, source, this, shield));
		}

		public void StartSkillEquipTauntShieldTaunt(int turns, int shield)
		{
			StartTimedEffect(new SkillEquipTauntShieldTimedEffect(turns, 0, this, this, shield));
		}

		public void StartSkillIncreaseAttack(FixedPoint normalAttackMultiplier, FixedPoint chargeAttackMultiplier, int turns, ActorModel source)
		{
			StartTimedEffect(new SkillIncreaseAttackTimedEffect(normalAttackMultiplier, chargeAttackMultiplier, turns, 0, source, this));
		}

		public void Disorient(int turns, ActorModel instigator)
		{
			base.manager.Player.Combat.AddDisorientedModel(this);
			int num = ((instigator.Faction != base.manager.CombatModel.TurnManager.ActiveFaction) ? 1 : 0);
			AIController.ClearFollowTarget();
			ActorModel disorientAttackTarget = AIBehaviorHelpers.GetDisorientAttackTarget(this, base.manager.Player.Combat);
			AIDataModel.SetCurrentTarget(disorientAttackTarget);
			AIDataModel.Alertness = AIAlertness.Homing;
			if (disorientAttackTarget != null && !EquipmentPassivePreventControlTrait.TryResistEffect(disorientAttackTarget, "DisorientedLock", RollDiceType.Disorient))
			{
				DisorientLockActor = disorientAttackTarget;
				disorientAttackTarget.StartTimedEffect(new TimedEffect(TimedEffectType.DisorientLock, turns + num, 0, instigator.Faction));
			}
			StartTimedEffect(new TimedEffect(TimedEffectType.Disorient, turns + num, 0, instigator.Faction));
			NotifyChange("ActorDisorientedEvent");
		}

		private void ResetABtestParam()
		{
			abTestParam = new ABtestParam();
		}

		public FixedPoint GetABtestDamageMultiplier()
		{
			return abTestParam.A_DamageMultiplier;
		}

		public void StartABTesterA(ActorModel sourceActor, int turns)
		{
			if (sourceActor != null)
			{
				abTestParam.A_DamageMultiplier = 0.0;
				abTestParam.A_source = sourceActor;
				base.manager.Player.AbilityManager.VisitParameter("LeaderBuffABTesterADamageMultiplier", ref abTestParam.A_DamageMultiplier, sourceActor);
				RemoveTrait("ABTesterA2Active");
				StartTimedEffect(new TimedEffect(TimedEffectType.ABTesterA, turns, 0, sourceActor, this));
				if (base.manager.CombatModel.TurnManager.ActiveFaction == Faction)
				{
					EndAction();
				}
				sourceActor.NotifyChange("AbilityVisited", new object[2] { "LeaderBuffABTester", false });
			}
		}

		public void StartABTesterB(ActorModel sourceActor)
		{
			if (sourceActor != null)
			{
				abTestParam.B_APChance = 0.0;
				base.manager.Player.AbilityManager.VisitParameter("LeaderBuffABTesterBAPChance", ref abTestParam.B_APChance, sourceActor);
				abTestParam.B_source = sourceActor;
				NotifyChange("ABtestBUpdateEvent", true);
				NotifyChange("AbilityVisited", new object[2] { "LeaderBuffABTester", false });
			}
		}

		public ABtestParam GetABtestParam()
		{
			return abTestParam;
		}

		public void StartABTesterA2(int turns, ActorModel target)
		{
			if (target != null)
			{
				int num = ((target.Faction == base.manager.CombatModel.TurnManager.ActiveFaction) ? 1 : 0);
				target.AddTemporaryTrait("ABTesterA2Active", default(FixedPoint), null, turns + num);
			}
		}

		public void OnTurnCountChanged()
		{
			FightBackTimesThisRound = 0;
			if (FocusCoolOff > 0)
			{
				FocusCoolOff--;
				if (FocusCoolOff <= 0)
				{
					FocusModeStateChargeCD = false;
					NotifyChange("ShowsFocusModeBTN");
				}
			}
			HershelGreeneModifiedAttributeValues();
			if (FistSpikeTurns > 0)
			{
				FistSpikeTurns--;
				NotifyChange("RefreshFistSpikeTurns");
			}
			if (DodgeShotTurns > 0)
			{
				DodgeShotTurns--;
				NotifyChange("RefreshDodgeShot");
			}
			else
			{
				DodgeShotTimes = 0;
			}
			if (SurvivalDashFlagTurns > 0)
			{
				SurvivalDashFlagTurns--;
				NotifyChange("SurvivalDashFlagUpdate");
			}
			else
			{
				SurvivalDashFlagTurns = 0;
			}
			if (PastaTurns > 0)
			{
				PastaTurns--;
			}
			else
			{
				PastaTurns = 0;
			}
			if (PastaCurrentTurn)
			{
				PastaCurrentTurn = false;
			}
			if (CapFirstHeal && base.manager != null && base.manager.Player != null)
			{
				CapFirstHeal = false;
				FixedPoint fixedPoint = FixedPoint.Min((FixedPoint)Hitpoints / (FixedPoint)MaxHitPoints, 1.0);
				SupportModel supportModel = base.manager.Player.GetSupportModel("Cap");
				if (supportModel != null && supportModel.Unlocked && fixedPoint <= supportModel.GetParameter(3) * 0.01)
				{
					int currentHitPoints = (int)(MaxHitPoints * supportModel.GetParameter(4) * 0.01);
					SetHitPoints(currentHitPoints, MaxHitPoints);
				}
			}
			if (UnluckyFlagTurns > 0)
			{
				UnluckyFlagTurns--;
				string traitIdentifier = "LowerUnlucky";
				RemoveAnyLevelTrait(traitIdentifier);
				if (UnluckyFlagTurns <= 0)
				{
					NotifyChange("SupportTalent_Lowerlucky");
				}
			}
			else
			{
				UnluckyFlagTurns = 0;
			}
			if (GodWarTraitTurns > 0)
			{
				GodWarTraitTurns--;
				if (GodWarTraitTurns <= 0)
				{
					List<TraitEntry> traitsThatContain = GetTraitsThatContain("GodWarBless");
					for (int i = 0; i < traitsThatContain.Count; i++)
					{
						RemoveTrait(traitsThatContain[i].TraitIdentifier);
					}
					NotifyChange("GodWarSkillChange");
				}
			}
			else
			{
				GodWarTraitTurns = 0;
			}
			if (BlindLeftTurns > 0)
			{
				BlindLeftTurns--;
				if (BlindLeftTurns <= 0)
				{
					NotifyChange("Blind");
				}
			}
			else
			{
				BlindLeftTurns = 0;
			}
			if (RaiderDashFlagTurns > 0)
			{
				RaiderDashFlagTurns--;
				NotifyChange("RaiderDashFlagUpdate");
			}
			else
			{
				RaiderDashFlagTurns = 0;
			}
			if (DebuffStatusRemoveTurns > 0)
			{
				DebuffStatusRemoveTurns--;
				if (DebuffStatusRemoveTurns == 0)
				{
					RemoveAllNegativeEffects(base.manager.CombatModel);
					IChallengeDebuffProvider challengeDebuffProvider = MapMissionDebuffHelper.CanUseDebuffMission(base.manager);
					if (challengeDebuffProvider != null)
					{
						int debuffStatusRemoveTurns = 0;
						List<DifficultyIncrementalDebuff> challengeDebuffs = challengeDebuffProvider.GetChallengeDebuffs();
						if (IsWalker)
						{
							if (ChallengeDebufHelps.GetDebufConfig(challengeDebuffs, ChallengeDebuffType.DebuffStatusRemove) != null)
							{
								debuffStatusRemoveTurns = (int)ChallengeDebufHelps.GetDebufTotalFirstParam(challengeDebuffs, ChallengeDebuffType.DebuffStatusRemove);
							}
						}
						else if (IsRaider && ChallengeDebufHelps.GetDebufConfig(challengeDebuffs, ChallengeDebuffType.DebuffStatusRemoveRaider) != null)
						{
							debuffStatusRemoveTurns = (int)ChallengeDebufHelps.GetDebufTotalFirstParam(challengeDebuffs, ChallengeDebuffType.DebuffStatusRemoveRaider);
						}
						DebuffStatusRemoveTurns = debuffStatusRemoveTurns;
					}
				}
			}
			else
			{
				DebuffStatusRemoveTurns = 0;
			}
			if (DefendingHeartTraitCDTurns > 0)
			{
				DefendingHeartTraitCDTurns--;
			}
			else
			{
				DefendingHeartTraitCDTurns = 0;
			}
			if (DefendingHeartTraitEffectLeftTurns > 0)
			{
				DefendingHeartTraitEffectLeftTurns--;
			}
			else
			{
				DefendingHeartTraitEffectLeftTurns = 0;
			}
			if (!IsDodgeShot && HasTrait("DodgedShotInjurerFlag"))
			{
				RemoveTrait("DodgedShotInjurerFlag");
			}
			if (OverloadStatusLeftTurns > 0)
			{
				OverloadStatusLeftTurns--;
			}
			overloadStatusEXAttackTimesInTurn = 0;
			if (SurvivalGameLeftCD > 0)
			{
				SurvivalGameLeftCD--;
			}
			if (DeadlyFocusLeftCount_SourceSurvivor > 0)
			{
				DeadlyFocusLeftCount_SourceSurvivor--;
			}
			if (DeadlyFocusLeftCount_SourceRaider > 0)
			{
				DeadlyFocusLeftCount_SourceRaider--;
			}
			if (ShadowedGuard_LeftCount > 0)
			{
				ShadowedGuard_LeftCount--;
				if (ShadowedGuard_LeftCount <= 0)
				{
					ShadowedGuardHealHp();
					ShadowedGuard_DelHP = 0;
					ShadowedGuard_Atk = 0;
					RemoveTrait("ShadowedGuard_StateRef");
					NotifyChange("UpdateShadowedGuardEvent");
				}
			}
			else
			{
				ShadowedGuard_LeftCount = 0;
			}
			OnTurnCountChanged_LeaderBuffDeathsDoor();
			NotifyChange("TurnCountChangedEvent");
		}

		public void HershelGreeneModifiedAttributeValues()
		{
			if (HeirloomsHershelFetterFloor == null)
			{
				return;
			}
			foreach (KeyValuePair<Faction, HeirloomsHershelFetter> item in HeirloomsHershelFetterFloor.ToList())
			{
				Faction key = item.Key;
				HeirloomsHershelFetter value = item.Value;
				if (value.Roundm > 0L)
				{
					value.Roundm -= (FixedPoint)1L;
					if (value.Roundm <= 0L)
					{
						AttributeModel.UpdateAttributeModelValueTotalizationNew("burn_be_dmg_ratio", value.BurnBeDmgRatio);
						value.BurnBeDmgRatio = 0L;
						AttributeModel.UpdateAttributeModelValueTotalizationNew("burn_ref_ratio", value.BurnRefRatio);
						value.BurnBeDmgRatio = 0L;
						AttributeModel.UpdateAttributeModelValueTotalizationNew("burn_be_ratio", value.BurnBeRatio);
						value.BurnBeDmgRatio = 0L;
						HeirloomsHershelFetterFloor.Remove(key);
						ResetAttributeGreene();
					}
				}
			}
		}

		public void ResetFocusMode()
		{
			FocusModeStateChargeCD = false;
			FocusModeState = false;
		}

		public FixedPoint GetDealBurningDamage(int burnLayer)
		{
			TraitDefinition traitDefinition = base.manager.GameEconomyData.GetTraitDefinition("Burning");
			if (traitDefinition != null)
			{
				FixedPoint parameter = traitDefinition.GetParameter<FixedPoint>(1);
				if (parameter > 0L)
				{
					FixedPoint value = 0.0;
					burnLayer = UtilsMath.Clamp(burnLayer, 0, burnLayer);
					FixedPoint fixedPoint = FixedPoint.Round(parameter * (burnLayer + 1) * ((float)MaxHitPoints / 100f)) * (1L + AttributeModel.GetAttributeModelValue("burn_be_dmg_ratio"));
					bool flag = base.manager.Player.AbilityManager.VisitParameter("AbilityModifierDecreaseBurningDamage", ref value, this);
					if (base.manager.Player.AbilityManager.VisitParameter("FlameDMGReduceBouns_ReduceBurn", ref value, this))
					{
						flag = true;
					}
					if (flag)
					{
						fixedPoint = fixedPoint - fixedPoint * value - fixedPoint * AttributeModel.GetAttributeModelValue("burn_ref_ratio");
						return FixedPoint.Max(0.0, fixedPoint);
					}
					fixedPoint -= fixedPoint * AttributeModel.GetAttributeModelValue("burn_ref_ratio");
					return FixedPoint.Max(0.0, fixedPoint);
				}
			}
			return 0L;
		}

		public void DealBurningDamage(FixedPoint damageBoostPercentage = default(FixedPoint))
		{
			if (base.manager.GameEconomyData.GetTraitDefinition("Burning") == null || !HasTrait("Burning"))
			{
				return;
			}
			int burnLayer = ((ExtraBurnTurn > 0) ? ExtraBurnLayer : 0);
			FixedPoint dealBurningDamage = GetDealBurningDamage(burnLayer);
			if (!(dealBurningDamage > 0L))
			{
				return;
			}
			dealBurningDamage *= 1.0 + damageBoostPercentage;
			if (IsScorching && ScorchTimedEffect.DamageChance > 0)
			{
				FixedPoint fixedPoint = FixedPoint.Round((float)ScorchTimedEffect.DamageChance * ((float)MaxHitPoints / 100f));
				dealBurningDamage += fixedPoint;
			}
			IChallengeDebuffProvider challengeDebuffProvider = MapMissionDebuffHelper.CanUseDebuffMission(base.manager);
			if (challengeDebuffProvider != null)
			{
				List<DifficultyIncrementalDebuff> challengeDebuffs = challengeDebuffProvider.GetChallengeDebuffs();
				if (IsWalker)
				{
					int num = (int)ChallengeDebufHelps.GetDebufTotalFirstParam(challengeDebuffs, ChallengeDebuffType.DebuffBurningDmgReduction);
					dealBurningDamage -= (FixedPoint)(int)(dealBurningDamage * num / 100L);
				}
				else if (IsRaider)
				{
					int num2 = (int)ChallengeDebufHelps.GetDebufTotalFirstParam(challengeDebuffs, ChallengeDebuffType.DebuffBurningDmgReductionRaider);
					dealBurningDamage -= (FixedPoint)(int)(dealBurningDamage * num2 / 100L);
				}
			}
			CombatHelpers.ExecuteDamage(base.manager.CombatModel, null, this, (int)dealBurningDamage, 0, DamageType.Fire, PlayerRandomChanceResult.Failed, PlayerRandomChanceResult.Failed);
		}

		public void DealBleedingDamage()
		{
			TraitDefinition traitDefinition = base.manager.GameEconomyData.GetTraitDefinition("Bleeding");
			if (traitDefinition != null && HasTrait("Bleeding"))
			{
				FixedPoint parameter = traitDefinition.GetParameter<FixedPoint>(0);
				if (parameter > 0L)
				{
					FixedPoint fixedPoint = FixedPoint.Round(parameter * ((float)MaxHitPoints / 100f));
					CombatHelpers.ExecuteDamage(base.manager.CombatModel, null, this, (int)fixedPoint, 0, DamageType.Bleeding, PlayerRandomChanceResult.Failed, PlayerRandomChanceResult.Failed);
				}
			}
		}

		public void FlameBurningDamage(FixedPoint dmg)
		{
			if (base.manager.GameEconomyData.GetTraitDefinition("Burning") != null && HasTrait("Burning") && dmg > 0L)
			{
				CombatHelpers.ExecuteDamage(base.manager.CombatModel, null, this, (int)dmg, 0, DamageType.Fire, PlayerRandomChanceResult.Failed, PlayerRandomChanceResult.Failed);
			}
		}

		public void FlameTriggerDamage(FixedPoint dmg)
		{
			if (dmg > 0L)
			{
				FixedPoint value = 0.0;
				bool flag = base.manager.Player.AbilityManager.VisitParameter("AbilityModifierDecreaseBurningDamage", ref value, this);
				if (base.manager.Player.AbilityManager.VisitParameter("FlameDMGReduceBouns_ReduceBurn", ref value, this))
				{
					flag = true;
				}
				if (flag)
				{
					dmg = dmg - dmg * value - dmg * AttributeModel.GetAttributeModelValue("burn_ref_ratio");
					dmg = FixedPoint.Max(0.0, dmg);
				}
				else
				{
					dmg -= dmg * AttributeModel.GetAttributeModelValue("burn_ref_ratio");
					dmg = FixedPoint.Max(0.0, dmg);
				}
				if (!(dmg <= 0L))
				{
					CombatHelpers.ExecuteDamage(base.manager.CombatModel, null, this, (int)dmg, 0, DamageType.Fire, PlayerRandomChanceResult.Failed, PlayerRandomChanceResult.Failed);
				}
			}
		}

		public List<PerPoisonStatus> GetBePoisonedLayerList()
		{
			List<PerPoisonStatus> list = new List<PerPoisonStatus>();
			CombatModel combat = base.manager.Player.Combat;
			if (combat == null)
			{
				return list;
			}
			PoisonRelationsManager model = combat.GetModel<PoisonRelationsManager>();
			if (model == null || model.ExistedPoisonRelations == null || model.ExistedPoisonRelations.Count == 0)
			{
				return list;
			}
			foreach (PoisonRelation existedPoisonRelation in model.ExistedPoisonRelations)
			{
				if (existedPoisonRelation.TargetActor == this)
				{
					list.Add(new PerPoisonStatus
					{
						LeftTurns = existedPoisonRelation.LeftTurns,
						LayerCount = existedPoisonRelation.CurrentLayerCount
					});
				}
			}
			return list;
		}

		public bool IsBePoisoned()
		{
			PoisonRelationsManager model = base.manager.Player.Combat.GetModel<PoisonRelationsManager>();
			if (model == null || model.ExistedPoisonRelations == null || model.ExistedPoisonRelations.Count == 0)
			{
				return false;
			}
			foreach (PoisonRelation existedPoisonRelation in model.ExistedPoisonRelations)
			{
				if (existedPoisonRelation.TargetActor == this)
				{
					return true;
				}
			}
			return false;
		}

		public int GetAstheniaLeftTurns()
		{
			AstheniaRelationsManager model = base.manager.Player.Combat.GetModel<AstheniaRelationsManager>();
			if (model == null || model.ExistedAstheniaRelations == null || model.ExistedAstheniaRelations.Count == 0)
			{
				return 0;
			}
			foreach (AstheniaRelation existedAstheniaRelation in model.ExistedAstheniaRelations)
			{
				if (existedAstheniaRelation.TargetActor == this)
				{
					return existedAstheniaRelation.LeftTurns;
				}
			}
			return 0;
		}

		public List<Faction> GetBeGrenadeFragmentDamagedList()
		{
			List<Faction> list = new List<Faction>();
			CombatModel combat = base.manager.Player.Combat;
			if (combat == null)
			{
				return list;
			}
			GrenadeFragmentDamageRelationsManager model = combat.GetModel<GrenadeFragmentDamageRelationsManager>();
			if (model == null || model.ExistedGrenadeFragmentDamageRelationRelations == null || model.ExistedGrenadeFragmentDamageRelationRelations.Count == 0)
			{
				return list;
			}
			foreach (GrenadeFragmentDamageRelation existedGrenadeFragmentDamageRelationRelation in model.ExistedGrenadeFragmentDamageRelationRelations)
			{
				if (existedGrenadeFragmentDamageRelationRelation.TargetActor == this)
				{
					list.Add(existedGrenadeFragmentDamageRelationRelation.FoundingFaction);
				}
			}
			return list;
		}

		public bool BeGrenadeFragmentDamagedByFaction(Faction faction)
		{
			GrenadeFragmentDamageRelationsManager model = base.manager.Player.Combat.GetModel<GrenadeFragmentDamageRelationsManager>();
			if (model == null || model.ExistedGrenadeFragmentDamageRelationRelations == null || model.ExistedGrenadeFragmentDamageRelationRelations.Count == 0)
			{
				return false;
			}
			return model.ExistedGrenadeFragmentDamageRelationRelations.Exists((GrenadeFragmentDamageRelation x) => x.FoundingFaction == faction && x.TargetActor == this);
		}

		public List<PerElectronChargeStatus> GetBeElectronChargeList()
		{
			List<PerElectronChargeStatus> list = new List<PerElectronChargeStatus>();
			CombatModel combat = base.manager.Player.Combat;
			if (combat == null)
			{
				return list;
			}
			ElectronChargeRelationManager model = combat.GetModel<ElectronChargeRelationManager>();
			if (model == null || model.ExistedElectronChargeRelations == null || model.ExistedElectronChargeRelations.Count == 0)
			{
				return list;
			}
			foreach (ElectronChargeRelation existedElectronChargeRelation in model.ExistedElectronChargeRelations)
			{
				if (existedElectronChargeRelation.TargetActor == this)
				{
					list.Add(new PerElectronChargeStatus
					{
						LeftTurns = existedElectronChargeRelation.LeftTurns,
						LayerCount = existedElectronChargeRelation.CurrentLayer,
						Faction = existedElectronChargeRelation.FoundingFaction
					});
				}
			}
			return list;
		}

		public int GetElectronChargeLayerByFaction(Faction faction)
		{
			ElectronChargeRelationManager model = base.manager.Player.Combat.GetModel<ElectronChargeRelationManager>();
			if (model == null || model.ExistedElectronChargeRelations == null || model.ExistedElectronChargeRelations.Count == 0)
			{
				return 0;
			}
			return model.ExistedElectronChargeRelations.Find((ElectronChargeRelation x) => x.FoundingFaction == faction && x.TargetActor == this)?.CurrentLayer ?? 0;
		}

		public long GetRemoteWeakenLeftTurns()
		{
			if (!IsRemoteWeakened)
			{
				return 0L;
			}
			return TraitContainer.GetTrait("RemoteWeakenActiveFlag").TraitDuration;
		}

		private void ResetOverloadParam()
		{
			overloadStatusLeftTurns = 0;
			overloadStatusEXAttackTimesInTurn = 0;
		}

		public bool IsHaveOverloadTrait()
		{
			if (HasAnyLevelTrait("BaseOverload"))
			{
				return true;
			}
			return false;
		}

		public int Overload_ChargePointNum()
		{
			FixedPoint value = 0.0;
			if (HasAnyLevelTrait("LeaderBuffOverload"))
			{
				base.manager.Player.AbilityManager.VisitParameter("LeaderBuffOverload_ChargePointNum", ref value, this);
			}
			else if (HasAnyLevelTrait("BaseOverload"))
			{
				base.manager.Player.AbilityManager.VisitParameter("BaseLeaderBuffOverload_ChargePointNum", ref value, this);
			}
			return (int)value;
		}

		public FixedPoint Overload_ChargePointDmgPer()
		{
			FixedPoint value = 0.0;
			if (HasAnyLevelTrait("LeaderBuffOverload"))
			{
				base.manager.Player.AbilityManager.VisitParameter("LeaderBuffOverload_ChargePointDmgPer", ref value, this);
			}
			else if (HasAnyLevelTrait("BaseOverload"))
			{
				base.manager.Player.AbilityManager.VisitParameter("BaseLeaderBuffOverload_ChargePointDmgPer", ref value, this);
			}
			return value;
		}

		public int Overload_ContinueTurnNum()
		{
			FixedPoint value = 0.0;
			if (HasAnyLevelTrait("LeaderBuffOverload"))
			{
				base.manager.Player.AbilityManager.VisitParameter("LeaderBuffOverload_ContinueTurnNum", ref value, this);
			}
			else if (HasAnyLevelTrait("BaseOverload"))
			{
				base.manager.Player.AbilityManager.VisitParameter("BaseLeaderBuffOverload_ContinueTurnNum", ref value, this);
			}
			return (int)value;
		}

		public FixedPoint Overload_FullChargeEXDmgPer()
		{
			FixedPoint value = 0.0;
			if (HasAnyLevelTrait("LeaderBuffOverload"))
			{
				base.manager.Player.AbilityManager.VisitParameter("LeaderBuffOverload_FullChargeEXDmgPer", ref value, this);
			}
			else if (HasAnyLevelTrait("BaseOverload"))
			{
				base.manager.Player.AbilityManager.VisitParameter("BaseLeaderBuffOverload_FullChargeEXDmgPer", ref value, this);
			}
			return value;
		}

		public int Overload_FullChargeEXTurnLimitNum()
		{
			FixedPoint value = 0.0;
			if (HasAnyLevelTrait("LeaderBuffOverload"))
			{
				base.manager.Player.AbilityManager.VisitParameter("LeaderBuffOverload_FullChargeEXTurnLimitNum", ref value, this);
			}
			else if (HasAnyLevelTrait("BaseOverload"))
			{
				base.manager.Player.AbilityManager.VisitParameter("BaseLeaderBuffOverload_FullChargeEXTurnLimitNum", ref value, this);
			}
			return (int)value;
		}

		public FixedPoint Overload_AddDmgPer()
		{
			FixedPoint value = 0.0;
			if (HasAnyLevelTrait("LeaderBuffOverload"))
			{
				base.manager.Player.AbilityManager.VisitParameter("LeaderBuffOverload_AddDmgPer", ref value, this);
			}
			else if (HasAnyLevelTrait("BaseOverload"))
			{
				base.manager.Player.AbilityManager.VisitParameter("BaseLeaderBuffOverload_AddDmgPer", ref value, this);
			}
			return value;
		}

		public FixedPoint Overload_LifeDmgPer()
		{
			FixedPoint value = 0.0;
			if (HasAnyLevelTrait("LeaderBuffOverload"))
			{
				base.manager.Player.AbilityManager.VisitParameter("LeaderBuffOverload_LifeDmgPer", ref value, this);
			}
			else if (HasAnyLevelTrait("BaseOverload"))
			{
				base.manager.Player.AbilityManager.VisitParameter("BaseLeaderBuffOverload_LifeDmgPer", ref value, this);
			}
			return value;
		}

		public bool IsRiposte()
		{
			if (ParryRiposteIncreaseStorey > 0)
			{
				return HasTraitsThatContains("Riposte");
			}
			return false;
		}

		public List<string> GetEffectShowBuffs()
		{
			List<string> list = new List<string>();
			if (IsRiposte())
			{
				list.Add("Riposte");
			}
			if (IsMomentum())
			{
				list.Add("Momentum");
			}
			if (SkillIncreaseAttackTimedEffect != null)
			{
				list.Add("SkillIncreaseAttack");
			}
			if (HasTraitsThatContains("GodWarBless"))
			{
				list.Add("GodWarSkill");
			}
			if (HasTrait("Equipment_Passive_Fortuna_Spade"))
			{
				list.Add("Fortuna_Spade");
			}
			if (HasAnyLevelTrait("Equipment_Passive_Fortuna_Club"))
			{
				list.Add("Fortuna_Club");
			}
			if (HasAnyLevelTrait("Equipment_Passive_Fortuna_Heart"))
			{
				list.Add("Fortuna_Heart");
			}
			list.RemoveAll((string t) => base.manager.GameEconomyData.GetTraitDefinition(t) == null);
			list.Sort((string x, string y) => base.manager.GameEconomyData.GetTraitDefinition(x).BuffPriority.CompareTo(base.manager.GameEconomyData.GetTraitDefinition(y).BuffPriority));
			return list;
		}

		public int GetEffectShowBuffLeftCount(string identifier)
		{
			return -1;
		}

		public int GetEffectShowBuffLevelCount(string identifier)
		{
			int num = -1;
			if (!(identifier == "Riposte"))
			{
				if (identifier == "Momentum")
				{
					return MomentumTimedEffect?.CurrentLayer ?? 0;
				}
				return -1;
			}
			return ParryRiposteIncreaseStorey;
		}

		public int GetSurvivalGameMoveDown()
		{
			FixedPoint value = 0.0;
			if (IsSurvivalGameEnemy())
			{
				SurvivalGameModel enemy_SurvivalGameModel = GetEnemy_SurvivalGameModel();
				base.manager.CombatModel.AbilityManager.VisitParameter("LeaderBuffSurvivalGame_MoveDisDown", ref value, enemy_SurvivalGameModel.LeaderActor);
			}
			return (int)value;
		}

		public bool IsSurvivalGameNoDead()
		{
			SurvivalGameModel leader_SurvivalGameModel = GetLeader_SurvivalGameModel();
			if (leader_SurvivalGameModel != null && leader_SurvivalGameModel.GetLeftNoDeadCount() > 0)
			{
				return true;
			}
			return false;
		}

		public void SurvivalGameNoDeadReduce()
		{
			SurvivalGameModel leader_SurvivalGameModel = GetLeader_SurvivalGameModel();
			if (leader_SurvivalGameModel != null && leader_SurvivalGameModel.GetLeftNoDeadCount() > 0)
			{
				leader_SurvivalGameModel.ReduceLeftNoDeadCount();
			}
		}

		public bool IsSurvivalGameEnemy()
		{
			SurvivalGameModel enemy_SurvivalGameModel = GetEnemy_SurvivalGameModel();
			if (enemy_SurvivalGameModel != null && enemy_SurvivalGameModel.LeftCount > 0)
			{
				return true;
			}
			return false;
		}

		public bool IsSurvivalGameLeader()
		{
			SurvivalGameModel leader_SurvivalGameModel = GetLeader_SurvivalGameModel();
			if (leader_SurvivalGameModel != null && leader_SurvivalGameModel.LeftCount > 0)
			{
				return true;
			}
			return false;
		}

		public bool IsSurvivalGameLeadFlag()
		{
			SurvivalGameModel leaderFaction_SurvivalGameModel = GetLeaderFaction_SurvivalGameModel();
			if (leaderFaction_SurvivalGameModel != null && leaderFaction_SurvivalGameModel.LeftCount > 0)
			{
				return true;
			}
			return false;
		}

		public SurvivalGameModel GetLeader_SurvivalGameModel()
		{
			if (this == null || base.manager == null)
			{
				return null;
			}
			CombatModel combatModel = base.manager.CombatModel;
			if (combatModel == null)
			{
				return null;
			}
			SurvivalGameModel survivalGameModel = combatModel.SurvivalGameModelList.Find((SurvivalGameModel t) => t.LeaderActor == this);
			if (survivalGameModel != null)
			{
				return survivalGameModel;
			}
			return null;
		}

		public SurvivalGameModel GetEnemy_SurvivalGameModel()
		{
			if (this == null || base.manager == null)
			{
				return null;
			}
			CombatModel combatModel = base.manager.CombatModel;
			if (combatModel == null)
			{
				return null;
			}
			SurvivalGameModel survivalGameModel = combatModel.SurvivalGameModelList.Find((SurvivalGameModel t) => t.EnemyActor == this);
			if (survivalGameModel != null)
			{
				return survivalGameModel;
			}
			return null;
		}

		public SurvivalGameModel GetLeaderFaction_SurvivalGameModel()
		{
			if (this == null || base.manager == null)
			{
				return null;
			}
			CombatModel combatModel = base.manager.CombatModel;
			if (combatModel == null)
			{
				return null;
			}
			SurvivalGameModel survivalGameModel = combatModel.SurvivalGameModelList.Find((SurvivalGameModel t) => t.LeaderActor.Faction == Faction);
			if (survivalGameModel != null && survivalGameModel.LeftCount > 0)
			{
				return survivalGameModel;
			}
			return null;
		}

		public int GetNegativeEffCount(List<string> effectIndex)
		{
			int num = 0;
			if (this == null || base.manager == null)
			{
				return num;
			}
			CombatModel combatModel = base.manager.CombatModel;
			if (combatModel == null)
			{
				return num;
			}
			if (effectIndex == null || effectIndex.Count <= 0)
			{
				return num;
			}
			for (int i = 0; i < effectIndex.Count; i++)
			{
				if (effectIndex[i] == "Stun" && IsStunned)
				{
					num++;
				}
				if (effectIndex[i] == "ABTesterAed" && IsABTesterAed)
				{
					num++;
				}
				if (effectIndex[i] == "Pitfall")
				{
					foreach (PitfallArea item in combatModel.Models.OfType<PitfallArea>().ToList())
					{
						if (Faction != item.Faction && item.IsInArea(GridCoordinate))
						{
							num++;
							break;
						}
					}
				}
				if (effectIndex[i] == "Burning" && IsBurning)
				{
					num++;
				}
				if (effectIndex[i] == "Disoriented" && IsDisoriented)
				{
					num++;
				}
				if (effectIndex[i] == "DisorientedLock" && IsDisorientedLock)
				{
					num++;
				}
				if (effectIndex[i] == "Cripple" && IsCrippled)
				{
					num++;
				}
				if (effectIndex[i] == "Root" && IsRooted)
				{
					num++;
				}
				if (effectIndex[i] == "StaggerActive" && IsStaggered)
				{
					num++;
				}
				if (effectIndex[i] == "Skinned" && IsSkinned)
				{
					num++;
				}
				if (effectIndex[i] == "Ripped" && IsRooted)
				{
					num++;
				}
				if (effectIndex[i] == "DebuffMarkEnemy" && HasAnyLevelTrait("DebuffMarkEnemy"))
				{
					num++;
				}
				if (effectIndex[i] == "BaseKnockKnock" && DebuffKnockKnockMarkCount > 0L)
				{
					num++;
				}
				if (effectIndex[i] == "Scorch" && IsScorching)
				{
					num++;
				}
				if (effectIndex[i] == "Bleeding" && IsBleeding)
				{
					num++;
				}
				if (effectIndex[i] == "FistSpike" && IsFistSpike)
				{
					num++;
				}
				if (effectIndex[i] == "Taunted" && IsTaunted)
				{
					num++;
				}
				if (effectIndex[i] == "DodgedShotInjurerFlag" && IsDodgeShot)
				{
					num++;
				}
				if (effectIndex[i] == "SufferActive" && HasAnyLevelTrait("LeaderBuffMadeToSuffer"))
				{
					num++;
				}
				if (effectIndex[i] == "Herd" && IsHerded)
				{
					num++;
				}
				if (effectIndex[i] == "SufferActive" && HasTrait("SufferActive"))
				{
					num++;
				}
				if (effectIndex[i] == "RemoteWeakened" && IsRemoteWeakened)
				{
					num++;
				}
				if (effectIndex[i] == "Asthenia" && GetAstheniaLeftTurns() > 0)
				{
					num++;
				}
				if (effectIndex[i] == "ElectricShock" && IsElectricShocked)
				{
					num++;
				}
				if (effectIndex[i] == "Quantun" && IsQuantuned)
				{
					num++;
				}
			}
			return num;
		}

		public List<string> GetSurvivalSkillList()
		{
			List<string> list = new List<string>();
			if (IsRiposte())
			{
				list.Add("Riposte");
			}
			if (IsMomentum())
			{
				list.Add("Momentum");
			}
			if (SkillIncreaseAttackTimedEffect != null)
			{
				list.Add("SkillIncreaseAttack");
			}
			list.RemoveAll((string t) => base.manager.GameEconomyData.GetTraitDefinition(t) == null);
			list.Sort((string x, string y) => base.manager.GameEconomyData.GetTraitDefinition(x).BuffPriority.CompareTo(base.manager.GameEconomyData.GetTraitDefinition(y).BuffPriority));
			return list;
		}

		public void SetSurvivalGameAI()
		{
			if (base.manager == null)
			{
				return;
			}
			CombatModel combatModel = base.manager.CombatModel;
			if (combatModel == null || Faction != Faction.Raider || (ExclusiveTimedEffect != null && ExclusiveTimedEffect.Type == TimedEffectType.Stun) || !combatModel.IsGridCellVisibleByAnySurvivor(GridCoordinate) || SurvivalGameLeftCD > 0 || TraitContainer.GetTraitAnyLevel("LeaderBuffSurvivalGame") == null)
			{
				return;
			}
			BaseCommandSkill actorCommandSkill = CommandSkillModelManager.ActorCommandSkill;
			if (actorCommandSkill == null)
			{
				return;
			}
			List<ActorModel> enemiesByDistanceAndFaction = GridCoordinate.GetEnemiesByDistanceAndFaction(GridCoordinate, combatModel, actorCommandSkill.Definition.Range, Faction);
			enemiesByDistanceAndFaction.RemoveAll((ActorModel t) => t.Faction != Faction.Walker && t.Faction != Faction.Survivor && t.Faction != Faction.Raider);
			enemiesByDistanceAndFaction.RemoveAll((ActorModel t) => !combatModel.IsGridCellVisibleByAnySurvivor(t.GridCoordinate));
			if (enemiesByDistanceAndFaction == null || enemiesByDistanceAndFaction.Count <= 0)
			{
				return;
			}
			ActorModel actorModel = enemiesByDistanceAndFaction.Find((ActorModel t) => t.Faction == Faction.Survivor);
			if (actorModel != null)
			{
				combatModel.SetNewSurvivalGame(this, actorModel);
				return;
			}
			actorModel = enemiesByDistanceAndFaction.Find((ActorModel t) => t.Faction == Faction.Walker);
			if (actorModel != null)
			{
				combatModel.SetNewSurvivalGame(this, actorModel);
			}
		}

		public void AddDeadlyFocus_TotalEXDamageMultiplier()
		{
			ActorModel leaderBuffDeadlyFocusMan = CombatHelpers.GetLeaderBuffDeadlyFocusMan(base.manager.CombatModel, Faction);
			if (leaderBuffDeadlyFocusMan == null)
			{
				DeadlyFocus_EXDamageLayerCount = 0;
				return;
			}
			DeadlyFocus_EXDamageLayerCount++;
			FixedPoint value = 0.0;
			base.manager.Player.AbilityManager.VisitParameter("LeaderBuffDeadlyFocus_DmgUpPerKill_Max", ref value, leaderBuffDeadlyFocusMan);
			if (DeadlyFocus_EXDamageLayerCount > value)
			{
				DeadlyFocus_EXDamageLayerCount = (int)value;
			}
			NotifyChange("AbilityVisited", new object[2] { "LeaderBuffDeadlyFocus", false });
			NotifyChange("UpdateDeadlyFocus");
		}

		public void SetDeadlyFocusAI()
		{
			if (base.manager == null)
			{
				return;
			}
			CombatModel combatModel = base.manager.CombatModel;
			if (combatModel == null)
			{
				return;
			}
			AbilityManagerModel abilityManager = base.manager.Player.AbilityManager;
			if (abilityManager == null || (Faction != Faction.Raider && Faction != Faction.Survivor) || (ExclusiveTimedEffect != null && ExclusiveTimedEffect.Type == TimedEffectType.Stun) || !combatModel.IsGridCellVisibleByAnySurvivor(GridCoordinate) || !HasAnyLevelTrait("BaseDeadlyFocus"))
			{
				return;
			}
			ActorModel leaderBuffDeadlyFocusMan = CombatHelpers.GetLeaderBuffDeadlyFocusMan(base.manager.CombatModel, Faction);
			if (leaderBuffDeadlyFocusMan == null)
			{
				return;
			}
			FixedPoint value = 0.0;
			abilityManager.VisitParameter("LeaderBuffDeadlyFocus_BuffEnemyMaxCount", ref value, leaderBuffDeadlyFocusMan);
			FixedPoint maxTurns = 0.0;
			abilityManager.VisitParameter("LeaderBuffDeadlyFocus_BuffMaxTurns", ref maxTurns, leaderBuffDeadlyFocusMan);
			List<ActorModel> enemyFactionsActors = combatModel.GetEnemyFactionsActors(Faction);
			enemyFactionsActors.Remove(this);
			enemyFactionsActors.RemoveAll((ActorModel t) => t.Faction != Faction.Walker && t.Faction != Faction.Survivor && t.Faction != Faction.Raider);
			enemyFactionsActors.RemoveAll((ActorModel t) => !combatModel.IsGridCellVisibleByAnySurvivor(t.GridCoordinate));
			if (enemyFactionsActors == null || enemyFactionsActors.Count <= 0)
			{
				return;
			}
			List<ActorModel> list = new List<ActorModel>();
			switch (Faction)
			{
			case Faction.Survivor:
				list = (from t in enemyFactionsActors
					orderby t.GridCoordinate.SquaredDistanceTo(GridCoordinate), t.DeadlyFocusLeftCount_SourceSurvivor
					select t).ToList();
				break;
			case Faction.Raider:
				list = (from t in enemyFactionsActors
					orderby t.GridCoordinate.SquaredDistanceTo(GridCoordinate), t.DeadlyFocusLeftCount_SourceRaider
					select t).ToList();
				break;
			}
			List<ActorModel> list2 = new List<ActorModel>();
			for (int num = 0; num < list.Count && !(num >= value); num++)
			{
				list2.Add(list[num]);
			}
			switch (Faction)
			{
			case Faction.Survivor:
				list2.ForEach(delegate(ActorModel t)
				{
					t.DeadlyFocusLeftCount_SourceSurvivor = (int)maxTurns;
					t.NotifyChange("AbilityVisited", new object[2] { "LeaderBuffDeadlyFocus", false });
					t.NotifyChange("UpdateDeadlyFocus");
				});
				break;
			case Faction.Raider:
				list2.ForEach(delegate(ActorModel t)
				{
					t.DeadlyFocusLeftCount_SourceRaider = (int)maxTurns;
					t.NotifyChange("AbilityVisited", new object[2] { "LeaderBuffDeadlyFocus", false });
					t.NotifyChange("UpdateDeadlyFocus");
				});
				break;
			}
		}

		public void OnEnemyKilledForRage()
		{
			if (base.manager == null || base.manager.Player == null || base.manager.Player.AbilityManager == null || !HasAnyLevelTrait("Equipment_Passive_Rage"))
			{
				return;
			}
			FixedPoint value = 0.0;
			if (!base.manager.Player.AbilityManager.VisitParameter("Equipment_Passive_RageParam0", ref value, this))
			{
				return;
			}
			int num = (int)(1L * value);
			if (num > 0)
			{
				int maxBaseRageFromTrait = GetMaxBaseRageFromTrait();
				int baseRage = BaseRage;
				BaseRage = Math.Min(BaseRage + num, maxBaseRageFromTrait);
				if (baseRage != BaseRage)
				{
					NotifyChange("ActorRageUpdateEvent");
				}
			}
		}

		public void OnChargedAttackCompletedForRage()
		{
			if (base.manager == null || base.manager.Player == null || base.manager.Player.AbilityManager == null || !HasAnyLevelTrait("Equipment_Passive_Rage"))
			{
				return;
			}
			FixedPoint value = 0.0;
			if (!base.manager.Player.AbilityManager.VisitParameter("Equipment_Passive_RageParam1", ref value, this))
			{
				return;
			}
			int num = (int)value;
			if (num > 0)
			{
				int maxBaseRageFromTrait = GetMaxBaseRageFromTrait();
				int baseRage = BaseRage;
				BaseRage = Math.Min(BaseRage + num, maxBaseRageFromTrait);
				if (baseRage != BaseRage)
				{
					NotifyChange("ActorRageUpdateEvent");
				}
			}
		}

		private int GetMaxBaseRageFromTrait()
		{
			if (base.manager == null || base.manager.Player == null || base.manager.Player.AbilityManager == null)
			{
				return 999;
			}
			int result = 999;
			FixedPoint value = 0.0;
			if (base.manager.Player.AbilityManager.VisitParameter("Equipment_Passive_RageParam2", ref value, this))
			{
				result = (int)value;
				result = Math.Max(1, result);
			}
			return result;
		}

		private bool IsInBloodthirstState()
		{
			if (base.manager == null || base.manager.Player == null || base.manager.Player.AbilityManager == null)
			{
				return false;
			}
			if (!HasAnyLevelTrait("Equipment_Passive_Rage"))
			{
				return false;
			}
			FixedPoint value = 0.0;
			if (base.manager.Player.AbilityManager.VisitParameter("Equipment_Passive_RageParam5", ref value, this))
			{
				return TotalRage >= (int)value;
			}
			return false;
		}

		public FixedPoint GetBloodthirstExtraDamage(FixedPoint enemyHealthPercent)
		{
			if (base.manager == null || base.manager.Player == null || base.manager.Player.AbilityManager == null)
			{
				return 0.0;
			}
			if (!IsInBloodthirstState())
			{
				return 0.0;
			}
			FixedPoint value = 0.0;
			FixedPoint value2 = 0.0;
			if (base.manager.Player.AbilityManager.VisitParameter("Equipment_Passive_RageParam6", ref value, this) && base.manager.Player.AbilityManager.VisitParameter("Equipment_Passive_RageParam7", ref value2, this))
			{
				if (value <= 0.0 || value2 <= 0.0)
				{
					return 0.0;
				}
				FixedPoint fixedPoint = 1L - enemyHealthPercent;
				if (fixedPoint <= 0.0)
				{
					return 0.0;
				}
				int num = (int)(fixedPoint / value);
				if (num <= 0)
				{
					return 0.0;
				}
				return num * value2;
			}
			return 0.0;
		}

		public bool IsInChargeConvertState()
		{
			if (base.manager == null || base.manager.Player == null || base.manager.Player.AbilityManager == null)
			{
				return false;
			}
			if (!HasAnyLevelTrait("Equipment_Passive_Rage"))
			{
				return false;
			}
			FixedPoint value = 0.0;
			if (base.manager.Player.AbilityManager.VisitParameter("Equipment_Passive_RageParam8", ref value, this))
			{
				return TotalRage >= (int)value;
			}
			return false;
		}

		public void OnChargePointGainedForRage()
		{
			if (base.manager != null && base.manager.Player != null && base.manager.Player.AbilityManager != null && IsInChargeConvertState())
			{
				FixedPoint value = 0.0;
				FixedPoint value2 = 0.0;
				FixedPoint value3 = 0.0;
				FixedPoint value4 = 0.0;
				if (base.manager.Player.AbilityManager.VisitParameter("Equipment_Passive_RageParam9", ref value, this) && base.manager.Player.AbilityManager.VisitParameter("Equipment_Passive_RageParam10", ref value3, this) && base.manager.Player.AbilityManager.VisitParameter("Equipment_Passive_RageParam11", ref value2, this) && base.manager.Player.AbilityManager.VisitParameter("Equipment_Passive_RageParam12", ref value4, this))
				{
					ChargeConvertDamageBonus = FixedPoint.Min(ChargeConvertDamageBonus + value, value3);
					ChargeConvertCritDamageBonus = FixedPoint.Min(ChargeConvertCritDamageBonus + value2, value4);
				}
			}
		}

		public void ResetBaseRage()
		{
			BaseRage = 0;
		}

		public void ResetAllRageStates()
		{
			BaseRage = 0;
			ChargeConvertDamageBonus = 0.0;
			ChargeConvertCritDamageBonus = 0.0;
		}

		public void ShadowedGuardHealHp()
		{
			if (base.manager.Player.AbilityManager != null && ShadowedGuard_DelHP > 0)
			{
				base.manager.ExecuteAction(new HealAction(this, this, ShadowedGuard_DelHP));
				NotifyChange("AbilityVisited", new object[2] { "LeaderBuffShadowedGuard", false });
			}
		}

		public void ShadowedGuardAddChargeNum()
		{
			AbilityManagerModel abilityManager = base.manager.Player.AbilityManager;
			if (abilityManager == null)
			{
				return;
			}
			int leaderBuffShadowedGuardLevel = CombatHelpers.GetLeaderBuffShadowedGuardLevel(base.manager.CombatModel, Faction);
			FixedPoint value = 0.0;
			ActorModel leaderBuffShadowedGuardMan = CombatHelpers.GetLeaderBuffShadowedGuardMan(base.manager.CombatModel, Faction);
			if (leaderBuffShadowedGuardMan != null)
			{
				abilityManager.VisitParameter("LeaderBuffShadowedGuard_Level_Charge", ref value, leaderBuffShadowedGuardMan);
				if (leaderBuffShadowedGuardLevel + 1 >= (int)value)
				{
					FixedPoint value2 = 0.0;
					abilityManager.VisitParameter("LeaderBuffShadowedGuard_Add_Charge", ref value2, leaderBuffShadowedGuardMan);
					ChargeNum += value2;
					NotifyChange("AbilityVisited", new object[2] { "LeaderBuffShadowedGuard", false });
				}
			}
		}

		public void SetShadowedGuardAI()
		{
			if (base.manager == null)
			{
				return;
			}
			CombatModel combatModel = base.manager.CombatModel;
			if (combatModel == null || Faction != Faction.Raider || (ExclusiveTimedEffect != null && ExclusiveTimedEffect.Type == TimedEffectType.Stun))
			{
				return;
			}
			ShadowedGuardSkill shadowedGuardSkill = CommandSkillModelManager?.GetActorCommandSkill<ShadowedGuardSkill>(CommandSkillType.CommandSkillShadowedGuard);
			if ((shadowedGuardSkill != null && shadowedGuardSkill.LeftCooldownTurns > 0) || TraitContainer.GetTraitAnyLevel("LeaderBuffShadowedGuard") == null || CommandSkillModelManager.ActorCommandSkill == null)
			{
				return;
			}
			FixedPoint value = 0L;
			combatModel.manager.Player.AbilityManager.VisitParameter("LeaderBuffShadowedGuard_Charge_MaxNum", ref value, this);
			if (value > ChargeNum || ShadowedGuard_LeftCount > 0 || !(this is SurvivorModel survivorModel))
			{
				return;
			}
			if (survivorModel.IsLeader)
			{
				foreach (ActorModel raider in combatModel.Raiders)
				{
					if (!raider.IsDead)
					{
						base.manager.CombatModel.AddShadowedGuard(this, raider);
						base.manager.CombatModel.AddShadowedGuardRefTrait(this, raider);
						raider.NotifyChange("UpdateShadowedGuardEvent");
					}
				}
			}
			else
			{
				base.manager.CombatModel.AddShadowedGuard(this, this);
				base.manager.CombatModel.AddShadowedGuardRefTrait(this, this);
				NotifyChange("UpdateShadowedGuardEvent");
			}
			ChargeNum = 0L;
			base.manager.CombatModel.NotifyChange("UpdateSurvivalGameEvent");
		}

		public void ResetGuardianVowState()
		{
			base.manager?.CombatModel?.ClearGuardianVowBindingsByActor(this);
		}

		public void RemoveAllNegativeEffects(CombatModel combat)
		{
			string[] removableNegativeEffectNames = RemovableNegativeEffectNames;
			foreach (string negativeEffect in removableNegativeEffectNames)
			{
				TryRemoveNegativeEffectByName(combat, negativeEffect);
			}
			if (HasTrait("Equipment_Active_Skinned_1") || HasTrait("Equipment_Active_Skinned_2"))
			{
				RemoveTrait("Equipment_Active_Skinned_1");
				RemoveTrait("Equipment_Active_Skinned_2");
			}
		}

		public bool HasRemovableNegativeEffect(string negativeEffect)
		{
			if (string.IsNullOrEmpty(negativeEffect))
			{
				return false;
			}
			switch (negativeEffect.Trim())
			{
			case "StaggerActive":
				return IsStaggered;
			case "Skinned":
				return IsSkinned;
			case "Stun":
				return IsStunned;
			case "Burning":
				return IsBurning;
			case "Disoriented":
				return IsDisoriented;
			case "DisorientedLock":
				return IsDisorientedLock;
			case "Root":
				return IsRooted;
			case "Cripple":
				return IsCrippled;
			case "DebuffMarkEnemy":
			{
				if (!HasAnyLevelTrait("DebuffMarkEnemy"))
				{
					return false;
				}
				TraitEntry traitAnyLevel = TraitContainer.GetTraitAnyLevel("DebuffMarkEnemy");
				if (traitAnyLevel != null)
				{
					return traitAnyLevel.Tag != Faction.ToString();
				}
				return false;
			}
			case "BaseKnockKnock":
				return DebuffKnockKnockMarkCount > 0L;
			case "Scorch":
				return IsScorching;
			case "Bleeding":
				return IsBleeding;
			case "FistSpike":
				return IsFistSpike;
			case "Taunted":
				return IsTaunted;
			case "DodgedShotInjurerFlag":
				return IsDodgeShot;
			case "ABTesterAed":
				return IsABTesterAed;
			case "Herd":
				return IsHerded;
			case "Asthenia":
				return GetAstheniaLeftTurns() > 0;
			case "Poison":
				return GetBePoisonedLayerList().Count > 0;
			case "RemoteWeakened":
				return IsRemoteWeakened;
			case "ElectricShock":
				return IsElectricShocked;
			case "ElectronCharge":
				return GetBeElectronChargeList().Count > 0;
			case "BloodMark":
				return HasBloodMark;
			default:
				return false;
			}
		}

		public bool TryRemoveNegativeEffectByName(CombatModel combat, string negativeEffect)
		{
			if (string.IsNullOrEmpty(negativeEffect) || !HasRemovableNegativeEffect(negativeEffect))
			{
				return false;
			}
			if (!ApplyNegativeEffectRemoval(combat, negativeEffect))
			{
				return false;
			}
			return !HasRemovableNegativeEffect(negativeEffect);
		}

		private bool ApplyNegativeEffectRemoval(CombatModel combat, string negativeEffect)
		{
			switch (negativeEffect.Trim())
			{
			case "StaggerActive":
				ClearStaggered();
				return true;
			case "Skinned":
				RemoveTrait("Skinned");
				return true;
			case "Stun":
				FinishTimedEffect(interrupted: true);
				ResetActionPointsForExternal();
				return true;
			case "Burning":
				RemoveTrait("Burning");
				return true;
			case "Disoriented":
			case "DisorientedLock":
			case "Cripple":
			case "ABTesterAed":
			case "Herd":
				FinishTimedEffect(interrupted: true);
				return true;
			case "Root":
				FinishTimedEffect(interrupted: true);
				ResetActionPointsForExternal();
				return true;
			case "DebuffMarkEnemy":
				RemoveAnyLevelTrait("DebuffMarkEnemy");
				return true;
			case "BaseKnockKnock":
				DebuffKnockKnockMarkCount = 0L;
				DebuffKnockKnockMarkMaxConfig = 0L;
				NotifyChange("KnockKnockMarkUpdateEvent", new object[2] { "LeaderBuffKnockKnock", false });
				return true;
			case "Scorch":
				FinishScorchTimedEffect();
				return true;
			case "Bleeding":
				RemoveTrait("Bleeding");
				return true;
			case "FistSpike":
				FistSpikeTurns = 0;
				NotifyChange("RefreshFistSpikeTurns");
				return true;
			case "Taunted":
				FinishTauntTimedEffect();
				return true;
			case "DodgedShotInjurerFlag":
				DodgeShotTimes = 0;
				DodgeShotTurns = 0;
				NotifyChange("RefreshDodgeShot");
				return true;
			case "Asthenia":
				return TryRemoveAstheniaRelations(combat);
			case "Poison":
				return TryRemovePoisonRelations(combat);
			case "RemoteWeakened":
				DebuffRemoteRepulseWeakenAddChargePointPercentage = 0.0;
				DebuffRemoteRepulseWeakenAddChargePoints = 0;
				RemoveTrait("RemoteWeakenActiveFlag");
				NotifyChange("HealTargetRemoveRemoteWeakened");
				return true;
			case "ElectricShock":
			{
				FinishTimedEffect(interrupted: true);
				if (DebuffParameterManager != null && DebuffParameterManager.TryGetParameterValueByParameterKey<ElectronChargeDebuffParameter>("ElectronShockAsElectronChargeLayer", out var value) && value != null && value != null)
				{
					DebuffParameterManager.RemoveParametersByParameterKey(value.ParameterKey);
				}
				ResetActionPointsForExternal();
				return true;
			}
			case "ElectronCharge":
				return TryRemoveElectronChargeRelations(combat);
			case "BloodMark":
				FinishBloodMarkTimedEffect();
				return true;
			default:
				return false;
			}
		}

		public void FinishBloodMarkTimedEffect()
		{
			if (BloodMarkTimedEffect != null)
			{
				List<CoexistTimedEffectType> list = new List<CoexistTimedEffectType>();
				list.Add(CoexistTimedEffectType.BloodMark);
				CoexistTimedEffectsManager?.RemoveCoexistTimedEffectByCoexistTimedEffectTypeList(list);
			}
		}

		private bool TryRemoveAstheniaRelations(CombatModel combat)
		{
			if (combat == null)
			{
				return false;
			}
			AstheniaRelationsManager model = combat.GetModel<AstheniaRelationsManager>();
			if (model == null)
			{
				return false;
			}
			List<AstheniaRelation> list = new List<AstheniaRelation>();
			foreach (AstheniaRelation existedAstheniaRelation in model.ExistedAstheniaRelations)
			{
				if (existedAstheniaRelation.TargetActor == this)
				{
					list.Add(existedAstheniaRelation);
				}
			}
			foreach (AstheniaRelation item in list)
			{
				model.RemoveRelationForExternal(item);
			}
			return list.Count > 0;
		}

		private bool TryRemovePoisonRelations(CombatModel combat)
		{
			if (combat == null)
			{
				return false;
			}
			PoisonRelationsManager model = combat.GetModel<PoisonRelationsManager>();
			if (model == null)
			{
				return false;
			}
			List<PoisonRelation> list = new List<PoisonRelation>();
			foreach (PoisonRelation existedPoisonRelation in model.ExistedPoisonRelations)
			{
				if (existedPoisonRelation.TargetActor == this)
				{
					list.Add(existedPoisonRelation);
				}
			}
			foreach (PoisonRelation item in list)
			{
				model.RemoveRelationForExternal(item);
			}
			return list.Count > 0;
		}

		private bool TryRemoveElectronChargeRelations(CombatModel combat)
		{
			if (combat == null)
			{
				return false;
			}
			ElectronChargeRelationManager model = combat.GetModel<ElectronChargeRelationManager>();
			if (model == null)
			{
				return false;
			}
			List<ElectronChargeRelation> list = new List<ElectronChargeRelation>();
			foreach (ElectronChargeRelation existedElectronChargeRelation in model.ExistedElectronChargeRelations)
			{
				if (existedElectronChargeRelation.TargetActor == this)
				{
					list.Add(existedElectronChargeRelation);
				}
			}
			foreach (ElectronChargeRelation item in list)
			{
				model.RemoveRelationForExternal(item);
			}
			return list.Count > 0;
		}

		public void ClearTotalVengefulChargeNums()
		{
			VengefulChargeNums = 0;
			LeaderBuffShadowedVengefulChargeNums = 0;
			NotifyChange("ActorVengefulChargeUpdateEvent");
		}

		public void VengefulCharge_dmg()
		{
			if (HasAnyLevelTrait("Equipment.VengefulCharge"))
			{
				AddVengefulChargeNums();
				AddVengefulChargeAPNum();
			}
		}

		private void AddVengefulChargeAPNum()
		{
			if (!HasAnyLevelTrait("Equipment.VengefulCharge"))
			{
				return;
			}
			FixedPoint value = 0.0;
			base.manager.Player.AbilityManager.VisitParameter("Equipment_VengefulCharge_APNum", ref value, this);
			if (!(value <= 0L))
			{
				FixedPoint value2 = 0.0;
				base.manager.Player.AbilityManager.VisitParameter("Equipment_VengefulCharge_APNum_Max", ref value2, this);
				int vengefulChargeAPNum_Turns = VengefulChargeAPNum_Turns;
				VengefulChargeAPNum_Turns += (int)value;
				if (VengefulChargeAPNum_Turns >= value2)
				{
					VengefulChargeAPNum_Turns = (int)value2;
				}
				if (VengefulChargeAPNum_Turns > vengefulChargeAPNum_Turns)
				{
					AddChargePoints(VengefulChargeAPNum_Turns - vengefulChargeAPNum_Turns);
				}
			}
		}

		private void AddVengefulChargeNums()
		{
			if (!HasAnyLevelTrait("Equipment.VengefulCharge"))
			{
				return;
			}
			FixedPoint value = 0.0;
			base.manager.Player.AbilityManager.VisitParameter("Equipment_VengefulCharge_MarkNum", ref value, this);
			if (!(value <= 0L))
			{
				FixedPoint value2 = 0.0;
				base.manager.Player.AbilityManager.VisitParameter("Equipment_VengefulCharge_MarkNum_Max", ref value2, this);
				VengefulChargeNums += (int)value;
				if (VengefulChargeNums >= value2)
				{
					VengefulChargeNums = (int)value2;
				}
				NotifyChange("AbilityVisited", new object[2] { "VengefulCharge", false });
				NotifyChange("ActorVengefulChargeUpdateEvent");
			}
		}

		public void VengefulCharge_LeaderBuffShadowedGuard()
		{
			if (HasTraitsThatContains("Equipment.VengefulCharge"))
			{
				FixedPoint value = 0.0;
				base.manager.Player.AbilityManager.VisitParameter("Equipment_VengefulCharge_MarkNumShadowedGuard", ref value, this);
				if (!(value <= 0L))
				{
					LeaderBuffShadowedVengefulChargeNums = (int)value;
					NotifyChange("AbilityVisited", new object[2] { "VengefulChargeLydiaMark", false });
				}
			}
		}

		public void AddCitadelTrait(string traitId)
		{
			if (!CitadelLastTurnAddedTraits.Contains(traitId))
			{
				CitadelLastTurnAddedTraits.Add(traitId);
			}
		}

		public FixedPoint GetCitadel_PursuitDown_ParameterMultiplier()
		{
			if (!HasAnyLevelTrait("Citadel_PursuitDown"))
			{
				return 1.0;
			}
			FixedPoint value = 0.0;
			base.manager.Player.AbilityManager.VisitParameter("Citadel_PursuitDown_LowerMultiplier", ref value, this);
			if (value <= 0L)
			{
				return 1.0;
			}
			return 1L - value;
		}

		public void CleanAllCitadelTraits()
		{
			if (CitadelLastTurnAddedTraits == null)
			{
				return;
			}
			foreach (string citadelLastTurnAddedTrait in CitadelLastTurnAddedTraits)
			{
				RemoveTrait(citadelLastTurnAddedTrait);
			}
			CitadelLastTurnAddedTraits.Clear();
			NotifyChange("ActorCitadelBeEffectedUpdate");
		}

		public void ExecuteCitadelTrait()
		{
			if (!HasAnyLevelTrait("LeaderBuffCitadel") || IsDead)
			{
				return;
			}
			TraitEntry traitAnyLevel = TraitContainer.GetTraitAnyLevel("LeaderBuffCitadel");
			if (traitAnyLevel == null)
			{
				return;
			}
			NotifyChange("ActorCitadelLeaderBuffUpdate");
			List<string> list = base.manager.GameEconomyData.GetTraitDefinition(traitAnyLevel.TraitIdentifier).GetParameter<string>(2).Split('|')
				.ToList();
			foreach (ActorModel citadelTargetActor in GetCitadelTargetActors())
			{
				if (citadelTargetActor.IsFactionLeaderHasAnyLevelTrait("LeaderBuffCitadel") && IsEnemy(citadelTargetActor))
				{
					continue;
				}
				foreach (string item in list)
				{
					citadelTargetActor.AddTrait(item);
					citadelTargetActor.AddCitadelTrait(item);
					citadelTargetActor.NotifyChange("ActorCitadelBeEffectedUpdate");
				}
			}
		}

		public List<ActorModel> GetCitadelTargetActors()
		{
			if (!HasAnyLevelTrait("LeaderBuffCitadel"))
			{
				return new List<ActorModel>();
			}
			FixedPoint value = 0.0;
			base.manager.Player.AbilityManager.VisitParameter("LeaderBuffCitadel_Range", ref value, this);
			List<ActorModel> list = new List<ActorModel>();
			if (value == 0L)
			{
				list = new List<ActorModel>(base.manager.CombatModel.GetAllActors());
			}
			if (value > 0L)
			{
				list = new List<ActorModel>(base.manager.CombatModel.GetActorsInRange(GridCoordinate, (int)value));
			}
			if (value < 0L)
			{
				List<ActorModel> allActors = base.manager.CombatModel.GetAllActors();
				int range = Math.Abs((int)value);
				List<ActorModel> excep = base.manager.CombatModel.GetActorsInRange(GridCoordinate, range);
				list = allActors.FindAll((ActorModel t) => !excep.Contains(t));
			}
			list.RemoveAll((ActorModel t) => t.Faction != Faction.Walker && t.Faction != Faction.Survivor && t.Faction != Faction.Raider);
			FixedPoint value2 = 0.0;
			base.manager.Player.AbilityManager.VisitParameter("LeaderBuffCitadel_TargetFaction", ref value2, this);
			if (!(value2 == 0L))
			{
				if (value2 > 0L)
				{
					list = list.FindAll((ActorModel t) => !IsEnemy(t));
				}
				else if (value2 < 0L)
				{
					list = list.FindAll((ActorModel t) => IsEnemy(t));
				}
			}
			return list;
		}

		public bool IsFactionHasAnyLevelTrait(string traitId)
		{
			bool result = false;
			switch (Faction)
			{
			case Faction.Survivor:
			{
				List<ActorModel> models2 = base.manager.CombatModel.Survivors.Models;
				if (models2 == null || models2.Count <= 0)
				{
					break;
				}
				for (int j = 0; j < models2.Count; j++)
				{
					if (models2[j] is SurvivorModel survivorModel && survivorModel.HasAnyLevelTrait(traitId))
					{
						result = true;
					}
				}
				break;
			}
			case Faction.Raider:
			{
				List<ActorModel> models = base.manager.CombatModel.Raiders.Models;
				if (models == null || models.Count <= 0)
				{
					break;
				}
				for (int i = 0; i < models.Count; i++)
				{
					if (models[i] is RaiderModel raiderModel && raiderModel.HasAnyLevelTrait(traitId))
					{
						result = true;
					}
				}
				break;
			}
			}
			return result;
		}

		public bool IsFactionLeaderHasAnyLevelTrait(string traitId)
		{
			bool result = false;
			switch (Faction)
			{
			case Faction.Survivor:
			{
				List<ActorModel> models2 = base.manager.CombatModel.Survivors.Models;
				if (models2 != null && models2.Count > 0 && models2[0] is SurvivorModel { IsLeader: not false } survivorModel2 && survivorModel2.HasAnyLevelTrait(traitId))
				{
					result = true;
				}
				break;
			}
			case Faction.Raider:
			{
				List<ActorModel> models = base.manager.CombatModel.Raiders.Models;
				if (models == null)
				{
					break;
				}
				for (int i = 0; i < models.Count; i++)
				{
					if (models[i] is SurvivorModel { IsLeader: not false } survivorModel && survivorModel.HasAnyLevelTrait(traitId))
					{
						result = true;
						break;
					}
				}
				break;
			}
			}
			return result;
		}

		public ActorModel GetLeaderInFaction()
		{
			List<ActorModel> models;
			switch (Faction)
			{
			case Faction.Survivor:
				models = base.manager.CombatModel.Survivors.Models;
				break;
			case Faction.Raider:
				models = base.manager.CombatModel.Raiders.Models;
				break;
			default:
				return null;
			}
			if (models == null)
			{
				return null;
			}
			for (int i = 0; i < models.Count; i++)
			{
				if (models[i] is SurvivorModel { IsLeader: not false } survivorModel)
				{
					return survivorModel;
				}
			}
			return null;
		}

		public FixedPoint GetCitadel_RangeDown_Range(FixedPoint range)
		{
			if (base.manager == null)
			{
				return range;
			}
			TraitEntry traitAnyLevel = TraitContainer.GetTraitAnyLevel("Citadel_RangeDown");
			if (traitAnyLevel == null)
			{
				return range;
			}
			TraitDefinition traitDefinition = base.manager.GameEconomyData.GetTraitDefinition(traitAnyLevel.TraitIdentifier);
			if (traitDefinition == null)
			{
				return range;
			}
			FixedPoint parameter = traitDefinition.GetParameter<FixedPoint>(0);
			if (parameter <= 0L)
			{
				return range;
			}
			FixedPoint parameter2 = traitDefinition.GetParameter<FixedPoint>(1);
			FixedPoint fixedPoint = range;
			range -= parameter;
			if (range < parameter2)
			{
				range = parameter2;
			}
			if (range > fixedPoint)
			{
				range = fixedPoint;
			}
			return range;
		}

		public FixedPoint GetCitadel_MoveDown_Num()
		{
			if (!HasAnyLevelTrait("Citadel_MoveDown"))
			{
				return 0.0;
			}
			FixedPoint value = 0.0;
			base.manager.Player.AbilityManager.VisitParameter("Citadel_MoveDownNum", ref value, this);
			if (value <= 0L)
			{
				return 0.0;
			}
			return value;
		}

		public bool TryGetBloodMarkMoveDistanceCap(out int moveDistanceCap)
		{
			moveDistanceCap = 0;
			BloodMarkTimedEffect bloodMarkTimedEffect = BloodMarkTimedEffect;
			if (bloodMarkTimedEffect == null || bloodMarkTimedEffect.MoveDistanceCap <= 0)
			{
				return false;
			}
			moveDistanceCap = bloodMarkTimedEffect.MoveDistanceCap;
			return true;
		}

		private void CleanLeaderBuffDeathsDoor()
		{
			DeathsDoor_DmgUpLayer = 0;
			DeathsDoor_DmgUpLeftTurns = 0;
			DeathsDoor_DmgUpLayerGainedThisAttack = 0;
			DeathsDoor_PursuitCount = 0;
			DeathsDoor_IsPursuitAttack = false;
			DeathsBlockSecondChance = false;
		}

		private void OnTurnCountChanged_LeaderBuffDeathsDoor()
		{
			if (DeathsDoor_DmgUpLeftTurns > 0)
			{
				DeathsDoor_DmgUpLeftTurns--;
				if (DeathsDoor_DmgUpLeftTurns <= 0)
				{
					DeathsDoor_DmgUpLayer = 0;
				}
			}
			DeathsDoor_PursuitCount = 0;
			DeathsDoor_IsPursuitAttack = false;
			DeathsBlockSecondChance = false;
			DeathsDoor_DmgUpLayerGainedThisAttack = 0;
		}

		public void UnityOutputCurrentTraits(string stage)
		{
		}

		public void GrantUndying()
		{
			UndyingState.IsUndying = true;
			NotifyChange("actorUndyingUpdateEvent");
		}

		public bool HasUndyingState()
		{
			return UndyingState.IsUndying;
		}

		public int RemainingNumOfUndyingTimes()
		{
			if (UndyingState.MaxTotalGrants - UndyingState.TotalGrantedCount <= 0)
			{
				return 0;
			}
			return UndyingState.MaxTotalGrants - UndyingState.TotalGrantedCount;
		}

		public int TurnsUntilNextUndying()
		{
			return UndyingState.TurnsUntilNextGrant;
		}


		#region mycode
		public void SetModifiers(ModifierCollection modifiers)
		{
			Modifiers = modifiers;
		}
		#endregion
	}
}
