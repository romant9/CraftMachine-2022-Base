using System.Collections.Generic;
using BaseModel;

namespace TWDModel
{
	public class ActorBackup : TWDModelObject
	{
		public Dictionary<Faction, HeirloomsHershelFetter> HeirloomsHershelFetterFloor;

		public FixedPoint ChargeLoadFloor;

		public int DodgeShotTurns;

		public int DodgeShotTimes;

		public AttackChainStaus AttackChainStaus;

		public int SurvivalDashFlagTurns;

		public int PastaTurns;

		public bool PastaCurrentTurn;

		public bool CapFirstAttack;

		public bool CapFirstHeal;

		public int UnluckyFlagTurns;

		public bool IsTriggerPassOW;

		public int GodWarTraitTurns;

		public int RaiderDashFlagTurns;

		public int DefendingHeartTraitCDTurns;

		public int DefendingHeartTraitEffectLeftTurns;

		public int DebuffStatusRemoveTurns;

		public int BlindLeftTurns;

		public FixedPoint BlindDecreaseRate = 0L;

		public bool dashTraitAttackFlag;

		public bool dashTraitValidFlag = true;

		public bool SupportTalent_NoMoveHitrateFlag;

		public bool SupportTalent_NoMoveCritRateFlag;

		public bool bloodFrenzyFlag;

		public int NextCanTriggerFirstAidTurn;

		public int NextReadyEquipmentPassiveRemoveNegativeEndTurn;

		public int NextReadyEquipmentPassiveRemoveNegativeStartTurn;

		public int NextReadyEquipmentPassiveRemoveNegativeOwnTurn;

		public int NextCanTriggerPassOW;

		public int FistSpikeTurns;

		public int KilledEnemyNum;

		public bool FocusModeState;

		public bool FocusModeStateChargeCD;

		public int FocusCoolOff;

		public bool ActorFactionChangedInCombat;

		public bool OverwatchedOnTurn;

		[IgnoreModelProperty]
		public ActorModel Actor { get; set; }

		public int Hitpoints { get; private set; }

		public bool OnRedHealthBar { get; set; }

		public UndyingState UndyingState { get; set; }

		public GridCoordinate GridCoordinate { get; set; }

		public int ShieldHitPoints { get; private set; }

		public int StrugglesLeft { get; set; }

		public TimeEffectBackup ExclusiveTimedEffect { get; set; }

		public ShieldTimedEffectBackup ShieldTimedEffect { get; set; }

		public TimeEffectBackup TauntTimedEffect { get; set; }

		public ScorchTimedEffectBackup ScorchTimedEffect { get; set; }

		public TimeEffectBackup PendingExclusiveTimedEffect { get; set; }

		public TimeEffectBackup ReloadingTimedEffect { get; set; }

		public AIAlertness Alertness { get; set; }

		public bool AIControlEnabled { get; set; }

		[IgnoreModelProperty]
		public AttributeModel AttributeModel { get; set; }

		[IgnoreModelProperty]
		public ActorDebuffParameterManager ActorDebuffParameterManager { get; set; }

		[IgnoreModelProperty]
		public CoexistTimedEffectsManager CoexistTimedEffectsManager { get; set; }

		[IgnoreModelProperty]
		public CommandSkillModelManager CommandSkillModelManager { get; set; }

		public List<BaseCommandSkill> CommandSkills { get; set; }

		[IgnoreModelProperty]
		public BaseCommandSkill ActorCommandSkill { get; set; }

		public Dictionary<string, GridCoordinate> AIGridCoordinates { get; set; }

		public Dictionary<string, AIEvent> AIEvents { get; set; }

		public bool HadActionPointsAtEndOfTurn { get; set; }

		public bool ChargeEnable { get; set; }

		public int ChargeLevel { get; set; }

		public int BaseRage { get; set; }

		public FixedPoint ChargeConvertDamageBonus { get; set; }

		public FixedPoint ChargeConvertCritDamageBonus { get; set; }

		public bool MoveCompleted { get; set; }

		public bool SecondMoveCompleted { get; set; }

		public bool AbilityCompleted { get; set; }

		public TurnState TurnState { get; set; }

		public bool UserCanControl { get; set; }

		public List<TraitEntry> Traits { get; set; }

		public List<string> CitadelLastTurnAddedTraits { get; set; }

		public int EquipmentActiveKingFactor { get; set; }

		public List<int> AsTargetAttackChainSlots { get; set; }

		public int SharpBladeLayers { get; private set; }

		public int MoveRangeConsumed { get; set; }

		public int BounsPhonePortraitTurn { get; set; }

		public int SurvivalGameLeftCD { get; set; }

		[IgnoreModelProperty]
		public ActorModel GuardActorModel { get; set; }

		public int SavedOnTurnIndex { get; set; }

		public int ParryRiposteIncreaseStorey { get; set; }

		public bool FollowThroughTriggeredInAttack { get; set; }

		public int MinHitpoints { get; set; }

		public int MaxShieldHitPoints { get; set; }

		public int AdditionalAttackCount { get; set; }

		public GridCoordinate MainTargetCell { get; set; }

		public Faction Faction { get; set; }

		public Faction OriginalFaction { get; set; }

		public bool KilledByLevelDifference { get; set; }

		public bool GainedChargePointOnMove { get; set; }

		public bool CanBenefitFromStaggerInstantly { get; set; }

		public bool CanReceiveChargePointFromStagger { get; set; }

		public ActorGender Gender { get; set; }

		[IgnoreModelProperty]
		public ActorModel LastHitAttacker { get; set; }

		[IgnoreModelProperty]
		public ExplosiveModel LastHitExplosive { get; set; }

		public OOTType LastOOT { get; set; }

		public bool HasPerformedOOT { get; set; }

		public bool BloodThirst { get; set; }

		public bool VisitedExtraApChance { get; set; }

		public bool EnsureExtraAP { get; set; }

		public bool EnsureGainedExtraMoveAp { get; set; }

		public bool HasGainedExtraMoveAp { get; set; }

		public bool HasGainedExtraAP { get; set; }

		public bool HasGainedExtraAPFromInteraction { get; set; }

		public bool TacticalResupplyMagazineNextDragLineCritPending { get; set; }

		public int KillsInTurn { get; set; }

		public int HitsInTurn { get; set; }

		public bool VisitedRedactChance { get; set; }

		public bool UsedToolThisTurn { get; set; }

		public bool UsedChargeAttackThisTurn { get; set; }

		public bool CanMoveWithoutAttacking { get; set; }

		public bool GainedAPFromPreviousAbilityExecution { get; set; }

		public bool GainedAPFromAbilityExecution { get; set; }

		public bool AdditionalAttackConsumed { get; set; }

		public bool FightingFuryActivated { get; set; }

		public int FightingFuryTargetCount { get; set; }

		public int CarolNotAttackAndNotAttackedTurns { get; set; }

		public bool IsAttackAndBeAttacked { get; set; }

		public int ExtraBurnLayer { get; set; }

		public int ExtraBurnTurn { get; set; }

		public bool HasHeadshotLTTriggered { get; set; }

		public bool AllowSecondMoveAfterAbility { get; set; }

		public int AdditionalMoveRange { get; set; }

		public bool RevengedOnTurn { get; set; }

		public int ShieldRevengedTimesOnTurn { get; set; }

		public bool AttackKilledAnyEnemy { get; set; }

		public bool AttackHasNotKilledAllEnemies { get; set; }

		public bool FollowUpAttackedOnTurn { get; set; }

		public bool PreAttackedOnTurn { get; set; }

		public bool PreAttackedOnRiposte { get; set; }

		public bool PassByAttackedOnMove { get; set; }

		public int GivenAdditionalAttacks { get; set; }

		public int ChargeAttackWithFreeShootingTriggeredCount { get; set; }

		public int FightBackTimesThisRound { get; set; }

		public bool freeAttackUsed { get; set; }

		public int BetterTogetherMultiplier { get; set; }

		public bool OneTurnCriticalHit { get; set; }

		public bool OneTurnStagger { get; set; }

		public int RandomStatusNumberOfAttack { get; set; }

		public FixedPoint DebuffRemoteRepulseWeakenAddChargePointPercentage { get; set; }

		public int DebuffRemoteRepulseWeakenAddChargePoints { get; set; }

		public FixedPoint DebuffKnockKnockMarkCount { get; set; }

		public FixedPoint OneTurnAttackedTimes { get; set; }

		public FixedPoint TornApartMarkCount { get; set; }

		[IgnoreModelProperty]
		public ActorModel DisorientLockActor { get; set; }

		public bool IsRecoilEffected { get; set; }

		public FixedPoint A_DamageMultiplier { get; set; }

		[IgnoreModelProperty]
		public ActorModel A_source { get; set; }

		public FixedPoint B_APChance { get; set; }

		[IgnoreModelProperty]
		public ActorModel B_source { get; set; }

		public int OverloadStatusLeftTurns { get; set; }

		public int OverloadStatusEXAttackTimesInTurn { get; set; }

		public int DeadlyFocusLeftCount_SourceSurvivor { get; set; }

		public int DeadlyFocusLeftCount_SourceRaider { get; set; }

		public int DeadlyFocus_EXDamageLayerCount { get; set; }

		public int ShadowedGuard_LeftCount { get; set; }

		public int ShadowedGuard_DelHP { get; set; }

		public int ShadowedGuard_Atk { get; set; }

		public FixedPoint ChargeNum { get; set; }

		public int VengefulChargeAPNum_Turns { get; set; }

		public int LeaderBuffShadowedVengefulChargeNums { get; set; }

		public int VengefulChargeNums { get; set; }

		public int CadenceAttackCount { get; set; }

		public bool CadenceReady { get; set; }

		public bool CadenceBoostingThisAttack { get; set; }

		public int DeathsDoor_DmgUpLayer { get; set; }

		public int DeathsDoor_DmgUpLeftTurns { get; set; }

		public int DeathsDoor_DmgUpLayerGainedThisAttack { get; set; }

		public override bool IsValid()
		{
			return true;
		}

		public void RecordStatus(ActorModel actor)
		{
			Actor = actor;
			Hitpoints = actor.Hitpoints;
			ShieldHitPoints = actor.ShieldHitPoints;
			GridCoordinate = actor.GridCoordinate;
			OnRedHealthBar = actor.OnRedHealthBar;
			Alertness = actor.AIDataModel.Alertness;
			ChargeLevel = actor.ChargeMeter.ChargeLevel;
			ChargeEnable = actor.ChargeMeter.ChargeEnabled;
			BaseRage = actor.BaseRage;
			ChargeConvertDamageBonus = actor.ChargeConvertDamageBonus;
			ChargeConvertCritDamageBonus = actor.ChargeConvertCritDamageBonus;
			StrugglesLeft = actor.StrugglesLeft;
			MoveCompleted = actor.MoveCompleted;
			SecondMoveCompleted = actor.SecondMoveCompleted;
			AbilityCompleted = actor.AbilityCompleted;
			TurnState = actor.TurnState;
			UserCanControl = actor.UserCanControl;
			AIControlEnabled = actor.AIController.Enabled;
			AttributeModel = actor.AttributeModel;
			HeirloomsHershelFetterFloor = actor.HeirloomsHershelFetterFloor;
			HadActionPointsAtEndOfTurn = actor.HadActionPointsAtEndOfTurn;
			AIGridCoordinates = actor.AIDataModel.GridCoordinates ?? new Dictionary<string, GridCoordinate>(actor.AIDataModel.GridCoordinates);
			AIEvents = actor.AIDataModel.Events ?? new Dictionary<string, AIEvent>(actor.AIDataModel.Events);
			SavedOnTurnIndex = actor.SavedOnTurnIndex;
			ParryRiposteIncreaseStorey = actor.ParryRiposteIncreaseStorey;
			FollowThroughTriggeredInAttack = actor.FollowThroughTriggeredInAttack;
			MinHitpoints = actor.MinHitpoints;
			MaxShieldHitPoints = actor.MaxShieldHitPoints;
			AdditionalAttackCount = actor.AdditionalAttackCount;
			MainTargetCell = actor.MainTargetCell;
			Faction = actor.Faction;
			OriginalFaction = actor.OriginalFaction;
			ActorFactionChangedInCombat = actor.ActorFactionChangedInCombat;
			KilledByLevelDifference = actor.KilledByLevelDifference;
			GainedChargePointOnMove = actor.GainedChargePointOnMove;
			CanBenefitFromStaggerInstantly = actor.CanBenefitFromStaggerInstantly;
			CanReceiveChargePointFromStagger = actor.CanReceiveChargePointFromStagger;
			Gender = actor.Gender;
			LastHitAttacker = actor.LastHitAttacker;
			LastHitExplosive = actor.LastHitExplosive;
			LastOOT = actor.LastOOT;
			HasPerformedOOT = actor.HasPerformedOOT;
			BloodThirst = actor.BloodThirst;
			VisitedExtraApChance = actor.VisitedExtraApChance;
			EnsureExtraAP = actor.EnsureExtraAP;
			EnsureGainedExtraMoveAp = actor.EnsureGainedExtraMoveAp;
			HasGainedExtraMoveAp = actor.HasGainedExtraMoveAp;
			HasGainedExtraAP = actor.HasGainedExtraAP;
			HasGainedExtraAPFromInteraction = actor.HasGainedExtraAPFromInteraction;
			TacticalResupplyMagazineNextDragLineCritPending = actor.TacticalResupplyMagazineNextDragLineCritPending;
			KillsInTurn = actor.KillsInTurn;
			HitsInTurn = actor.HitsInTurn;
			VisitedRedactChance = actor.VisitedRedactChance;
			UsedToolThisTurn = actor.UsedToolThisTurn;
			UsedChargeAttackThisTurn = actor.UsedChargeAttackThisTurn;
			CanMoveWithoutAttacking = actor.CanMoveWithoutAttacking;
			GainedAPFromPreviousAbilityExecution = actor.GainedAPFromPreviousAbilityExecution;
			GainedAPFromAbilityExecution = actor.GainedAPFromAbilityExecution;
			AdditionalAttackConsumed = actor.AdditionalAttackConsumed;
			FightingFuryActivated = actor.FightingFuryActivated;
			FightingFuryTargetCount = actor.FightingFuryTargetCount;
			CarolNotAttackAndNotAttackedTurns = actor.CarolNotAttackAndNotAttackedTurns;
			IsAttackAndBeAttacked = actor.IsAttackAndBeAttacked;
			ExtraBurnLayer = actor.ExtraBurnLayer;
			ExtraBurnTurn = actor.ExtraBurnTurn;
			HasHeadshotLTTriggered = actor.HasHeadshotLTTriggered;
			AllowSecondMoveAfterAbility = actor.AllowSecondMoveAfterAbility;
			AdditionalMoveRange = actor.AdditionalMoveRange;
			RevengedOnTurn = actor.RevengedOnTurn;
			ShieldRevengedTimesOnTurn = actor.ShieldRevengedTimesOnTurn;
			OverwatchedOnTurn = actor.OverwatchedOnTurn;
			AttackKilledAnyEnemy = actor.AttackKilledAnyEnemy;
			AttackHasNotKilledAllEnemies = actor.AttackHasNotKilledAllEnemies;
			FollowUpAttackedOnTurn = actor.FollowUpAttackedOnTurn;
			PreAttackedOnTurn = actor.PreAttackedOnTurn;
			PreAttackedOnRiposte = actor.PreAttackedOnRiposte;
			PassByAttackedOnMove = actor.PassByAttackedOnMove;
			GivenAdditionalAttacks = actor.GivenAdditionalAttacks;
			ChargeAttackWithFreeShootingTriggeredCount = actor.ChargeAttackWithFreeShootingTriggeredCount;
			FightBackTimesThisRound = actor.FightBackTimesThisRound;
			freeAttackUsed = actor.freeAttackUsed;
			BetterTogetherMultiplier = actor.BetterTogetherMultiplier;
			OneTurnCriticalHit = actor.OneTurnCriticalHit;
			OneTurnStagger = actor.OneTurnStagger;
			RandomStatusNumberOfAttack = actor.RandomStatusNumberOfAttack;
			DebuffRemoteRepulseWeakenAddChargePointPercentage = actor.DebuffRemoteRepulseWeakenAddChargePointPercentage;
			DebuffRemoteRepulseWeakenAddChargePoints = actor.DebuffRemoteRepulseWeakenAddChargePoints;
			DebuffKnockKnockMarkCount = actor.DebuffKnockKnockMarkCount;
			OneTurnAttackedTimes = actor.OneTurnAttackedTimes;
			TornApartMarkCount = actor.TornApartMarkCount;
			DisorientLockActor = actor.DisorientLockActor;
			IsRecoilEffected = actor.IsRecoilEffected;
			A_DamageMultiplier = actor.abTestParam.A_DamageMultiplier;
			A_source = actor.abTestParam.A_source;
			B_APChance = actor.abTestParam.B_APChance;
			B_source = actor.abTestParam.B_source;
			Traits = new List<TraitEntry>();
			foreach (TraitEntry trait in actor.TraitContainer.Traits)
			{
				Traits.Add(new TraitEntry(trait));
			}
			CitadelLastTurnAddedTraits = ((actor.CitadelLastTurnAddedTraits == null) ? new List<string>() : new List<string>(actor.CitadelLastTurnAddedTraits));
			EquipmentActiveKingFactor = actor.EquipmentActiveKingFactor;
			ChargeLoadFloor = actor.ChargeLoadFloor;
			DodgeShotTimes = actor.DodgeShotTimes;
			DodgeShotTurns = actor.DodgeShotTurns;
			AsTargetAttackChainSlots = actor.AsTargetAttackChainSlots;
			if (actor.AttackChainStaus != null)
			{
				AttackChainStaus = new AttackChainStaus();
				AttackChainStaus.RecordStatus(actor.AttackChainStaus);
			}
			if (actor.UndyingState != null)
			{
				UndyingState = new UndyingState();
				UndyingState.RecordStatus(actor.UndyingState);
			}
			NextCanTriggerFirstAidTurn = actor.NextCanTriggerFirstAidTurn;
			NextCanTriggerPassOW = actor.NextCanTriggerPassOW;
			FistSpikeTurns = actor.FistSpikeTurns;
			MoveRangeConsumed = actor.MoveRangeConsumed;
			KilledEnemyNum = actor.KilledEnemyNum;
			FocusModeState = actor.FocusModeState;
			FocusCoolOff = actor.FocusCoolOff;
			FocusModeStateChargeCD = actor.FocusModeStateChargeCD;
			BounsPhonePortraitTurn = actor.BounsPhonePortraitTurn;
			SurvivalDashFlagTurns = actor.SurvivalDashFlagTurns;
			PastaTurns = actor.PastaTurns;
			PastaCurrentTurn = actor.PastaCurrentTurn;
			CapFirstAttack = actor.CapFirstAttack;
			CapFirstHeal = actor.CapFirstHeal;
			UnluckyFlagTurns = actor.UnluckyFlagTurns;
			IsTriggerPassOW = actor.IsTriggerPassOW;
			RaiderDashFlagTurns = actor.RaiderDashFlagTurns;
			dashTraitAttackFlag = actor.dashTraitAttackFlag;
			dashTraitValidFlag = actor.dashTraitValidFlag;
			SurvivalGameLeftCD = actor.SurvivalGameLeftCD;
			bloodFrenzyFlag = actor.bloodFrenzyFlag;
			SharpBladeLayers = actor.SharpBladeLayers;
			DefendingHeartTraitCDTurns = actor.DefendingHeartTraitCDTurns;
			DefendingHeartTraitEffectLeftTurns = actor.DefendingHeartTraitEffectLeftTurns;
			DebuffStatusRemoveTurns = actor.DebuffStatusRemoveTurns;
			BlindLeftTurns = actor.BlindLeftTurns;
			BlindDecreaseRate = actor.BlindDecreaseRate;
			GodWarTraitTurns = actor.GodWarTraitTurns;
			SupportTalent_NoMoveHitrateFlag = actor.SupportTalent_NoMoveHitrateFlag;
			SupportTalent_NoMoveCritRateFlag = actor.SupportTalent_NoMoveCritRateFlag;
			DeadlyFocusLeftCount_SourceSurvivor = actor.DeadlyFocusLeftCount_SourceSurvivor;
			DeadlyFocusLeftCount_SourceRaider = actor.DeadlyFocusLeftCount_SourceRaider;
			DeadlyFocus_EXDamageLayerCount = actor.DeadlyFocus_EXDamageLayerCount;
			ShadowedGuard_LeftCount = actor.ShadowedGuard_LeftCount;
			ShadowedGuard_DelHP = actor.ShadowedGuard_DelHP;
			ShadowedGuard_Atk = actor.ShadowedGuard_Atk;
			ChargeNum = actor.ChargeNum;
			OverloadStatusLeftTurns = actor.OverloadStatusLeftTurns;
			OverloadStatusEXAttackTimesInTurn = actor.OverloadStatusEXAttackTimesInTurn;
			GuardActorModel = actor.GuardActorModel;
			VengefulChargeAPNum_Turns = actor.VengefulChargeAPNum_Turns;
			LeaderBuffShadowedVengefulChargeNums = actor.LeaderBuffShadowedVengefulChargeNums;
			LeaderBuffShadowedVengefulChargeNums = actor.LeaderBuffShadowedVengefulChargeNums;
			CadenceAttackCount = actor.CadenceAttackCount;
			CadenceReady = actor.CadenceReady;
			CadenceBoostingThisAttack = actor.CadenceBoostingThisAttack;
			DeathsDoor_DmgUpLayer = actor.DeathsDoor_DmgUpLayer;
			DeathsDoor_DmgUpLeftTurns = actor.DeathsDoor_DmgUpLeftTurns;
			DeathsDoor_DmgUpLayerGainedThisAttack = actor.DeathsDoor_DmgUpLayerGainedThisAttack;
			if (actor.ExclusiveTimedEffect != null)
			{
				ExclusiveTimedEffect = new TimeEffectBackup();
				ExclusiveTimedEffect.RecordStatus(actor.ExclusiveTimedEffect);
			}
			if (actor.ShieldTimedEffect != null)
			{
				ShieldTimedEffect = new ShieldTimedEffectBackup();
				ShieldTimedEffect.RecordStatus(actor.ShieldTimedEffect);
			}
			if (actor.TauntTimedEffect != null)
			{
				TauntTimedEffect = new TimeEffectBackup();
				TauntTimedEffect.RecordStatus(actor.TauntTimedEffect);
			}
			if (actor.ScorchTimedEffect != null)
			{
				ScorchTimedEffect = new ScorchTimedEffectBackup();
				ScorchTimedEffect.RecordStatus(actor.ScorchTimedEffect);
			}
			if (actor.PendingExclusiveTimedEffect != null)
			{
				PendingExclusiveTimedEffect = new TimeEffectBackup();
				PendingExclusiveTimedEffect.RecordStatus(actor.PendingExclusiveTimedEffect);
			}
			if (actor.SelectedEquipment?.ReloadingTimedEffect != null)
			{
				ReloadingTimedEffect = new TimeEffectBackup();
				ReloadingTimedEffect.RecordStatus(actor.SelectedEquipment.ReloadingTimedEffect);
			}
			if (actor.DebuffParameterManager != null)
			{
				ActorDebuffParameterManager = new ActorDebuffParameterManager(actor.DebuffParameterManager);
				ActorDebuffParameterManager.DebuffParameters = RecordDebuffParameters(actor.DebuffParameterManager.DebuffParameters);
			}
			if (actor.CommandSkillModelManager != null)
			{
				CommandSkillModelManager = new CommandSkillModelManager(actor.CommandSkillModelManager);
			}
			CommandSkills = RecordCommandSkills();
			ActorCommandSkill = RecordActorCommandSkill();
			if (actor.CoexistTimedEffectsManager != null)
			{
				CoexistTimedEffectsManager = new CoexistTimedEffectsManager(actor.CoexistTimedEffectsManager);
				CoexistTimedEffectsManager.CoexistTimedEffects = RecordCoexistTimedEffects(actor.CoexistTimedEffectsManager.CoexistTimedEffects);
			}
		}

		public void BackUp()
		{
			if (Actor.ModelId == 0)
			{
				Actor.SetManager(base.Manager);
				Actor.Start();
			}
			Actor.BackUpHitpoint(Hitpoints);
			Actor.ShieldHitPoints = ShieldHitPoints;
			Actor.GridCoordinate = GridCoordinate;
			Actor.OnRedHealthBar = OnRedHealthBar;
			Actor.AIDataModel.Alertness = Alertness;
			Actor.ChargeMeter.ChargeLevel = ChargeLevel;
			Actor.ChargeMeter.ChargeEnabled = ChargeEnable;
			Actor.BaseRage = BaseRage;
			Actor.ChargeConvertDamageBonus = ChargeConvertDamageBonus;
			Actor.ChargeConvertCritDamageBonus = ChargeConvertCritDamageBonus;
			Actor.StrugglesLeft = StrugglesLeft;
			Actor.MoveCompleted = MoveCompleted;
			Actor.SecondMoveCompleted = SecondMoveCompleted;
			Actor.AbilityCompleted = AbilityCompleted;
			Actor.TurnState = TurnState;
			Actor.SetUserCanControl(UserCanControl, "ActorBackup.BackUp");
			Actor.AIController.Enabled = AIControlEnabled;
			Actor.HadActionPointsAtEndOfTurn = HadActionPointsAtEndOfTurn;
			Actor.AIDataModel.GridCoordinates = AIGridCoordinates ?? new Dictionary<string, GridCoordinate>(AIGridCoordinates);
			Actor.AIDataModel.Events = AIEvents ?? new Dictionary<string, AIEvent>(AIEvents);
			Actor.SavedOnTurnIndex = SavedOnTurnIndex;
			Actor.ParryRiposteIncreaseStorey = ParryRiposteIncreaseStorey;
			Actor.FollowThroughTriggeredInAttack = FollowThroughTriggeredInAttack;
			Actor.MinHitpoints = MinHitpoints;
			Actor.MaxShieldHitPoints = MaxShieldHitPoints;
			Actor.AdditionalAttackCount = AdditionalAttackCount;
			Actor.MainTargetCell = MainTargetCell;
			Actor.Faction = Faction;
			Actor.OriginalFaction = OriginalFaction;
			Actor.ActorFactionChangedInCombat = ActorFactionChangedInCombat;
			Actor.KilledByLevelDifference = KilledByLevelDifference;
			Actor.GainedChargePointOnMove = GainedChargePointOnMove;
			Actor.CanBenefitFromStaggerInstantly = CanBenefitFromStaggerInstantly;
			Actor.CanReceiveChargePointFromStagger = CanReceiveChargePointFromStagger;
			Actor.Gender = Gender;
			Actor.LastHitAttacker = LastHitAttacker;
			Actor.LastHitExplosive = LastHitExplosive;
			Actor.LastOOT = LastOOT;
			Actor.HasPerformedOOT = HasPerformedOOT;
			Actor.BloodThirst = BloodThirst;
			Actor.VisitedExtraApChance = VisitedExtraApChance;
			Actor.EnsureExtraAP = EnsureExtraAP;
			Actor.EnsureGainedExtraMoveAp = EnsureGainedExtraMoveAp;
			Actor.HasGainedExtraMoveAp = HasGainedExtraMoveAp;
			Actor.HasGainedExtraAP = HasGainedExtraAP;
			Actor.HasGainedExtraAPFromInteraction = HasGainedExtraAPFromInteraction;
			Actor.TacticalResupplyMagazineNextDragLineCritPending = TacticalResupplyMagazineNextDragLineCritPending;
			Actor.KillsInTurn = KillsInTurn;
			Actor.HitsInTurn = HitsInTurn;
			Actor.VisitedRedactChance = VisitedRedactChance;
			Actor.UsedToolThisTurn = UsedToolThisTurn;
			Actor.UsedChargeAttackThisTurn = UsedChargeAttackThisTurn;
			Actor.CanMoveWithoutAttacking = CanMoveWithoutAttacking;
			Actor.GainedAPFromPreviousAbilityExecution = GainedAPFromPreviousAbilityExecution;
			Actor.GainedAPFromAbilityExecution = GainedAPFromAbilityExecution;
			Actor.AdditionalAttackConsumed = AdditionalAttackConsumed;
			Actor.FightingFuryActivated = FightingFuryActivated;
			Actor.FightingFuryTargetCount = FightingFuryTargetCount;
			Actor.CarolNotAttackAndNotAttackedTurns = CarolNotAttackAndNotAttackedTurns;
			Actor.IsAttackAndBeAttacked = IsAttackAndBeAttacked;
			Actor.ExtraBurnLayer = ExtraBurnLayer;
			Actor.ExtraBurnTurn = ExtraBurnTurn;
			Actor.HasHeadshotLTTriggered = HasHeadshotLTTriggered;
			Actor.AllowSecondMoveAfterAbility = AllowSecondMoveAfterAbility;
			Actor.AdditionalMoveRange = AdditionalMoveRange;
			Actor.RevengedOnTurn = RevengedOnTurn;
			Actor.ShieldRevengedTimesOnTurn = ShieldRevengedTimesOnTurn;
			Actor.OverwatchedOnTurn = OverwatchedOnTurn;
			Actor.AttackKilledAnyEnemy = AttackKilledAnyEnemy;
			Actor.AttackHasNotKilledAllEnemies = AttackHasNotKilledAllEnemies;
			Actor.FollowUpAttackedOnTurn = FollowUpAttackedOnTurn;
			Actor.PreAttackedOnTurn = PreAttackedOnTurn;
			Actor.PreAttackedOnRiposte = PreAttackedOnRiposte;
			Actor.PassByAttackedOnMove = PassByAttackedOnMove;
			Actor.GivenAdditionalAttacks = GivenAdditionalAttacks;
			Actor.ChargeAttackWithFreeShootingTriggeredCount = ChargeAttackWithFreeShootingTriggeredCount;
			Actor.FightBackTimesThisRound = FightBackTimesThisRound;
			Actor.freeAttackUsed = freeAttackUsed;
			Actor.BetterTogetherMultiplier = BetterTogetherMultiplier;
			Actor.OneTurnCriticalHit = OneTurnCriticalHit;
			Actor.OneTurnStagger = OneTurnStagger;
			Actor.RandomStatusNumberOfAttack = RandomStatusNumberOfAttack;
			Actor.DebuffRemoteRepulseWeakenAddChargePointPercentage = DebuffRemoteRepulseWeakenAddChargePointPercentage;
			Actor.DebuffRemoteRepulseWeakenAddChargePoints = DebuffRemoteRepulseWeakenAddChargePoints;
			Actor.DebuffKnockKnockMarkCount = DebuffKnockKnockMarkCount;
			Actor.OneTurnAttackedTimes = OneTurnAttackedTimes;
			Actor.TornApartMarkCount = TornApartMarkCount;
			Actor.DisorientLockActor = DisorientLockActor;
			Actor.IsRecoilEffected = IsRecoilEffected;
			Actor.abTestParam.A_DamageMultiplier = A_DamageMultiplier;
			Actor.abTestParam.A_source = A_source;
			Actor.abTestParam.B_APChance = B_APChance;
			Actor.abTestParam.B_source = B_source;
			if (!base.manager.Player.Combat.AllActors.Contains(Actor))
			{
				base.manager.Player.Combat.RegisterActor(Actor);
			}
			if (AttributeModel != null)
			{
				Actor.AttributeModel = AttributeModel;
			}
			else
			{
				AttributeModel = new AttributeModel();
				AttributeModel.SetManager(base.manager);
				AttributeModel.Initialize();
				AttributeModel.Start();
				Actor.AttributeModel = AttributeModel;
			}
			if (ActorDebuffParameterManager != null)
			{
				TurnManager turnManager = base.manager.CombatModel?.TurnManager;
				if (turnManager != null)
				{
					turnManager.FactionChanged -= Actor.DebuffParameterManager.RemoveExpiryParameterOnFactionChanged;
				}
				Actor.DebuffParameterManager = ActorDebuffParameterManager;
				Actor.DebuffParameterManager.SetManager(base.manager);
				if (turnManager != null)
				{
					turnManager.FactionChanged += Actor.DebuffParameterManager.RemoveExpiryParameterOnFactionChanged;
				}
			}
			else
			{
				ActorDebuffParameterManager = new ActorDebuffParameterManager();
				ActorDebuffParameterManager.SetManager(base.manager);
				ActorDebuffParameterManager.Initialize();
				ActorDebuffParameterManager.Start();
				Actor.DebuffParameterManager = ActorDebuffParameterManager;
			}
			Actor.DebuffParameterManager.DebuffParameters = BackupDebuffParameters();
			if (CommandSkillModelManager != null)
			{
				TurnManager turnManager2 = base.manager.CombatModel?.TurnManager;
				if (turnManager2 != null)
				{
					turnManager2.FactionChanged -= Actor.CommandSkillModelManager.OnFactionChanged;
				}
				Actor.CommandSkillModelManager = CommandSkillModelManager;
				Actor.CommandSkillModelManager.SetManager(base.manager);
				if (turnManager2 != null)
				{
					turnManager2.FactionChanged += Actor.CommandSkillModelManager.OnFactionChanged;
				}
			}
			else
			{
				CommandSkillModelManager = new CommandSkillModelManager();
				CommandSkillModelManager.SetOwnActorModel(Actor);
				CommandSkillModelManager.SetManager(base.manager);
				CommandSkillModelManager.Initialize();
				CommandSkillModelManager.Start();
				Actor.CommandSkillModelManager = CommandSkillModelManager;
			}
			Actor.CommandSkillModelManager.BackupCommandSkills(BackupCommandSkills());
			Actor.CommandSkillModelManager.BackActorCommandSkill(BackupActorCommandSkill());
			if (CoexistTimedEffectsManager != null)
			{
				TurnManager turnManager3 = base.manager.CombatModel?.TurnManager;
				if (turnManager3 != null)
				{
					turnManager3.FactionChanged -= Actor.CoexistTimedEffectsManager.OnFactionChanged;
				}
				Actor.CoexistTimedEffectsManager = CoexistTimedEffectsManager;
				Actor.CoexistTimedEffectsManager.SetManager(base.manager);
				if (turnManager3 != null)
				{
					turnManager3.FactionChanged += Actor.CoexistTimedEffectsManager.OnFactionChanged;
				}
			}
			else
			{
				CoexistTimedEffectsManager = new CoexistTimedEffectsManager();
				CoexistTimedEffectsManager.SetManager(base.manager);
				CoexistTimedEffectsManager.Initialize();
				CoexistTimedEffectsManager.Start();
				Actor.CoexistTimedEffectsManager = CoexistTimedEffectsManager;
			}
			Actor.CoexistTimedEffectsManager.CoexistTimedEffects = BackupCoexistTimedEffects();
			if (HeirloomsHershelFetterFloor == null || HeirloomsHershelFetterFloor.Count == 0)
			{
				Actor.HeirloomsHershelFetterFloor = new Dictionary<Faction, HeirloomsHershelFetter>();
			}
			else
			{
				Actor.HeirloomsHershelFetterFloor = HeirloomsHershelFetterFloor;
			}
			Actor.RemoveAllTraits();
			foreach (TraitEntry trait in Traits)
			{
				Actor.AddTraitByEntry(trait);
			}
			if (Actor.CitadelLastTurnAddedTraits == null)
			{
				Actor.CitadelLastTurnAddedTraits = new List<string>();
			}
			else
			{
				Actor.CitadelLastTurnAddedTraits.Clear();
			}
			if (CitadelLastTurnAddedTraits != null && CitadelLastTurnAddedTraits.Count > 0)
			{
				Actor.CitadelLastTurnAddedTraits.AddRange(CitadelLastTurnAddedTraits);
			}
			Actor.NotifyChange("ActorCitadelBeEffectedUpdate");
			Actor.EquipmentActiveKingFactor = EquipmentActiveKingFactor;
			Actor.ChargeLoadFloor = ChargeLoadFloor;
			Actor.DodgeShotTurns = DodgeShotTurns;
			Actor.UndyingState = UndyingState;
			Actor.DodgeShotTimes = DodgeShotTimes;
			Actor.NextCanTriggerFirstAidTurn = NextCanTriggerFirstAidTurn;
			Actor.NextReadyEquipmentPassiveRemoveNegativeEndTurn = NextReadyEquipmentPassiveRemoveNegativeEndTurn;
			Actor.NextReadyEquipmentPassiveRemoveNegativeStartTurn = NextReadyEquipmentPassiveRemoveNegativeStartTurn;
			Actor.NextReadyEquipmentPassiveRemoveNegativeOwnTurn = NextReadyEquipmentPassiveRemoveNegativeOwnTurn;
			Actor.NextCanTriggerPassOW = NextCanTriggerPassOW;
			Actor.FistSpikeTurns = FistSpikeTurns;
			Actor.MoveRangeConsumed = MoveRangeConsumed;
			Actor.KilledEnemyNum = KilledEnemyNum;
			Actor.FocusCoolOff = FocusCoolOff;
			Actor.FocusModeState = FocusModeState;
			Actor.FocusModeStateChargeCD = FocusModeStateChargeCD;
			Actor.BounsPhonePortraitTurn = BounsPhonePortraitTurn;
			Actor.SurvivalDashFlagTurns = SurvivalDashFlagTurns;
			Actor.PastaTurns = PastaTurns;
			Actor.PastaCurrentTurn = PastaCurrentTurn;
			Actor.CapFirstAttack = CapFirstAttack;
			Actor.CapFirstHeal = CapFirstHeal;
			Actor.UnluckyFlagTurns = UnluckyFlagTurns;
			Actor.IsTriggerPassOW = IsTriggerPassOW;
			Actor.RaiderDashFlagTurns = RaiderDashFlagTurns;
			Actor.dashTraitAttackFlag = dashTraitAttackFlag;
			Actor.dashTraitValidFlag = dashTraitValidFlag;
			Actor.bloodFrenzyFlag = bloodFrenzyFlag;
			Actor.SharpBladeLayers = SharpBladeLayers;
			Actor.DefendingHeartTraitCDTurns = DefendingHeartTraitCDTurns;
			Actor.DefendingHeartTraitEffectLeftTurns = DefendingHeartTraitEffectLeftTurns;
			Actor.DebuffStatusRemoveTurns = DebuffStatusRemoveTurns;
			Actor.BlindLeftTurns = BlindLeftTurns;
			Actor.BlindDecreaseRate = BlindDecreaseRate;
			Actor.GodWarTraitTurns = GodWarTraitTurns;
			Actor.SupportTalent_NoMoveHitrateFlag = SupportTalent_NoMoveHitrateFlag;
			Actor.SupportTalent_NoMoveCritRateFlag = SupportTalent_NoMoveCritRateFlag;
			Actor.OverloadStatusLeftTurns = OverloadStatusLeftTurns;
			Actor.OverloadStatusEXAttackTimesInTurn = OverloadStatusEXAttackTimesInTurn;
			Actor.DeadlyFocusLeftCount_SourceSurvivor = DeadlyFocusLeftCount_SourceSurvivor;
			Actor.DeadlyFocusLeftCount_SourceRaider = DeadlyFocusLeftCount_SourceRaider;
			Actor.DeadlyFocus_EXDamageLayerCount = DeadlyFocus_EXDamageLayerCount;
			Actor.SurvivalGameLeftCD = SurvivalGameLeftCD;
			Actor.GuardActorModel = GuardActorModel;
			Actor.ShadowedGuard_LeftCount = ShadowedGuard_LeftCount;
			Actor.ShadowedGuard_DelHP = ShadowedGuard_DelHP;
			Actor.ShadowedGuard_Atk = ShadowedGuard_Atk;
			Actor.ChargeNum = ChargeNum;
			Actor.VengefulChargeAPNum_Turns = VengefulChargeAPNum_Turns;
			Actor.LeaderBuffShadowedVengefulChargeNums = LeaderBuffShadowedVengefulChargeNums;
			Actor.LeaderBuffShadowedVengefulChargeNums = LeaderBuffShadowedVengefulChargeNums;
			Actor.CadenceAttackCount = CadenceAttackCount;
			Actor.CadenceReady = CadenceReady;
			Actor.CadenceBoostingThisAttack = CadenceBoostingThisAttack;
			Actor.DeathsDoor_DmgUpLayer = DeathsDoor_DmgUpLayer;
			Actor.DeathsDoor_DmgUpLeftTurns = DeathsDoor_DmgUpLeftTurns;
			Actor.DeathsDoor_DmgUpLayerGainedThisAttack = DeathsDoor_DmgUpLayerGainedThisAttack;
			if (ExclusiveTimedEffect != null)
			{
				ExclusiveTimedEffect.BackUp();
				Actor.ExclusiveTimedEffect = ExclusiveTimedEffect.Model;
			}
			else
			{
				Actor.ExclusiveTimedEffect = null;
			}
			if (ShieldTimedEffect != null)
			{
				ShieldTimedEffect.BackUp();
				Actor.ShieldTimedEffect = ShieldTimedEffect.Model as ShieldTimedEffect;
			}
			else
			{
				Actor.ShieldTimedEffect = null;
			}
			if (TauntTimedEffect != null)
			{
				TauntTimedEffect.BackUp();
				Actor.TauntTimedEffect = TauntTimedEffect.Model;
			}
			else
			{
				Actor.TauntTimedEffect = null;
			}
			if (ScorchTimedEffect != null)
			{
				ScorchTimedEffect.BackUp();
				Actor.ScorchTimedEffect = ScorchTimedEffect.Model as ScorchTimedEffect;
			}
			else
			{
				Actor.ScorchTimedEffect = null;
			}
			if (PendingExclusiveTimedEffect != null)
			{
				PendingExclusiveTimedEffect.BackUp();
				Actor.PendingExclusiveTimedEffect = PendingExclusiveTimedEffect.Model;
			}
			else
			{
				Actor.PendingExclusiveTimedEffect = null;
			}
			if (ReloadingTimedEffect != null && Actor.SelectedEquipment != null)
			{
				ReloadingTimedEffect.BackUp();
				Actor.SelectedEquipment.ReloadingTimedEffect = ReloadingTimedEffect.Model;
			}
			else if (Actor.SelectedEquipment != null)
			{
				Actor.SelectedEquipment.ReloadingTimedEffect = null;
			}
		}

		public List<DebuffParameterBase> RecordDebuffParameters(List<DebuffParameterBase> debuffParameters)
		{
			List<DebuffParameterBase> list = new List<DebuffParameterBase>();
			if (debuffParameters == null || debuffParameters.Count == 0)
			{
				return list;
			}
			foreach (DebuffParameterBase debuffParameter in debuffParameters)
			{
				if (debuffParameter is ElectronChargeDebuffParameter electronChargeDebuffParameter)
				{
					list.Add(new ElectronChargeDebuffParameter(electronChargeDebuffParameter));
				}
			}
			return list;
		}

		public List<DebuffParameterBase> BackupDebuffParameters()
		{
			List<DebuffParameterBase> list = new List<DebuffParameterBase>();
			if (ActorDebuffParameterManager.DebuffParameters == null)
			{
				return list;
			}
			foreach (DebuffParameterBase debuffParameter in ActorDebuffParameterManager.DebuffParameters)
			{
				list.Add(debuffParameter);
			}
			return list;
		}

		public List<BaseCommandSkill> RecordCommandSkills()
		{
			List<BaseCommandSkill> list = new List<BaseCommandSkill>();
			ModelList<BaseCommandSkill> commandSkills = Actor.CommandSkillModelManager.CommandSkills;
			if (commandSkills == null || commandSkills.Count == 0)
			{
				return list;
			}
			foreach (object item in commandSkills)
			{
				if (item is HealDamageSkill skill)
				{
					list.Add(new HealDamageSkill(skill));
				}
				if (item is HealMaxHealthSkill skill2)
				{
					list.Add(new HealMaxHealthSkill(skill2));
				}
				if (item is HealTargetHealthSkill skill3)
				{
					list.Add(new HealTargetHealthSkill(skill3));
				}
				if (item is HealTargetMaxHealthSkill skill4)
				{
					list.Add(new HealTargetMaxHealthSkill(skill4));
				}
				if (item is HealTargetLossHealthSkill skill5)
				{
					list.Add(new HealTargetLossHealthSkill(skill5));
				}
				if (item is AdrenalineSkill skill6)
				{
					list.Add(new AdrenalineSkill(skill6));
				}
				if (item is ShieldType1Skill shieldType1Skill)
				{
					list.Add(new ShieldType1Skill(shieldType1Skill));
				}
				if (item is IncreaseAttackSkill increaseAttackSkill)
				{
					list.Add(new IncreaseAttackSkill(increaseAttackSkill));
				}
				if (item is GodWarSkill skill7)
				{
					list.Add(new GodWarSkill(skill7));
				}
				if (item is EquipTauntSkill skill8)
				{
					list.Add(new EquipTauntSkill(skill8));
				}
				if (item is BerserkerSkill skill9)
				{
					list.Add(new BerserkerSkill(skill9));
				}
				if (item is GuardianVowSkill skill10)
				{
					list.Add(new GuardianVowSkill(skill10));
				}
				if (item is DelayedActionGrenadeSkill skill11)
				{
					list.Add(new DelayedActionGrenadeSkill(skill11));
				}
				if (item is AbilityRangeTridentSkill skill12)
				{
					list.Add(new AbilityRangeTridentSkill(skill12));
				}
				if (item is FortificationsSkill skill13)
				{
					list.Add(new FortificationsSkill(skill13));
				}
				if (item is FortificationsRemoveSkill skill14)
				{
					list.Add(new FortificationsRemoveSkill(skill14));
				}
			}
			return list;
		}

		public ModelList<BaseCommandSkill> BackupCommandSkills()
		{
			ModelList<BaseCommandSkill> modelList = new ModelList<BaseCommandSkill>();
			foreach (BaseCommandSkill commandSkill in CommandSkills)
			{
				commandSkill.SetOwnActor(Actor);
				commandSkill.SetManager(base.manager);
				commandSkill.Start();
				modelList.Add(commandSkill);
			}
			modelList.Initialize();
			modelList.SetManager(base.manager);
			return modelList;
		}

		public BaseCommandSkill RecordActorCommandSkill()
		{
			BaseCommandSkill baseCommandSkill = Actor.CommandSkillModelManager?.ActorCommandSkill;
			if (baseCommandSkill == null)
			{
				return null;
			}
			BaseCommandSkill result = null;
			if (baseCommandSkill is SurvivalGameSkill survivalGameSkill)
			{
				result = new SurvivalGameSkill(survivalGameSkill);
			}
			if (baseCommandSkill is ShadowedGuardSkill shadowedGuardSkill)
			{
				result = new ShadowedGuardSkill(shadowedGuardSkill);
			}
			return result;
		}

		public BaseCommandSkill BackupActorCommandSkill()
		{
			if (ActorCommandSkill != null)
			{
				ActorCommandSkill.SetOwnActor(Actor);
				ActorCommandSkill.SetManager(base.manager);
				ActorCommandSkill.Start();
			}
			return ActorCommandSkill;
		}

		public ModelList<CoexistTimedEffectAbstract> RecordCoexistTimedEffects(ModelList<CoexistTimedEffectAbstract> listEffects)
		{
			ModelList<CoexistTimedEffectAbstract> modelList = new ModelList<CoexistTimedEffectAbstract>();
			modelList.Initialize();
			modelList.SetManager(base.manager);
			if (listEffects == null || listEffects.Count == 0)
			{
				return modelList;
			}
			foreach (object listEffect in listEffects)
			{
				if (listEffect is QuantunTimedEffect quantunTimedEffect)
				{
					modelList.Add(new QuantunTimedEffect(quantunTimedEffect));
				}
				if (listEffect is MomentumTimedEffect momentumTimedEffect)
				{
					modelList.Add(new MomentumTimedEffect(momentumTimedEffect));
				}
				if (listEffect is SkillShieldType1TimedEffect skillShieldType1TimedEffect)
				{
					modelList.Add(new SkillShieldType1TimedEffect(skillShieldType1TimedEffect));
				}
				if (listEffect is SkillEquipTauntShieldTimedEffect skillEquipTauntShieldTimedEffect)
				{
					modelList.Add(new SkillEquipTauntShieldTimedEffect(skillEquipTauntShieldTimedEffect));
				}
				if (listEffect is SkillIncreaseAttackTimedEffect skillIncreaseAttackTimedEffect)
				{
					modelList.Add(new SkillIncreaseAttackTimedEffect(skillIncreaseAttackTimedEffect));
				}
				if (listEffect is UnluckyTimedEffect unluckyTimedEffect)
				{
					modelList.Add(new UnluckyTimedEffect(unluckyTimedEffect));
				}
				if (listEffect is ShieldBreakerTimedEffect shieldBreakerTimedEffect)
				{
					modelList.Add(new ShieldBreakerTimedEffect(shieldBreakerTimedEffect));
				}
				if (listEffect is DebuffDamagePerRoundTimedEffect debuffDamagePerRoundTimedEffect)
				{
					modelList.Add(new DebuffDamagePerRoundTimedEffect(debuffDamagePerRoundTimedEffect));
				}
				if (listEffect is DebuffReduceRecoveryTimedEffect debuffReduceRecoveryTimedEffect)
				{
					modelList.Add(new DebuffReduceRecoveryTimedEffect(debuffReduceRecoveryTimedEffect));
				}
				if (listEffect is BerserkRageTimedEffect berserkRageTimedEffect)
				{
					modelList.Add(new BerserkRageTimedEffect(berserkRageTimedEffect));
				}
				if (listEffect is BloodMarkTimedEffect other)
				{
					modelList.Add(new BloodMarkTimedEffect(other));
				}
				if (listEffect is FortificationsTimedEffect other2)
				{
					modelList.Add(new FortificationsTimedEffect(other2));
				}
			}
			return modelList;
		}

		public ModelList<CoexistTimedEffectAbstract> BackupCoexistTimedEffects()
		{
			ModelList<CoexistTimedEffectAbstract> modelList = new ModelList<CoexistTimedEffectAbstract>();
			modelList.Initialize();
			modelList.SetManager(base.manager);
			if (CoexistTimedEffectsManager.CoexistTimedEffects == null)
			{
				return modelList;
			}
			foreach (CoexistTimedEffectAbstract coexistTimedEffect in CoexistTimedEffectsManager.CoexistTimedEffects)
			{
				coexistTimedEffect.Initialize();
				coexistTimedEffect.SetManager(base.manager);
				coexistTimedEffect.Start();
				modelList.Add(coexistTimedEffect);
			}
			return modelList;
		}
	}
}
