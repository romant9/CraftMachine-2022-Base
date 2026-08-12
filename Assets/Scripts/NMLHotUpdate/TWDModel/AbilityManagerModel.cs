using System;
using System.Collections.Generic;
using System.Linq;
using BaseModel;
using Newtonsoft.Json;

namespace TWDModel
{
	public class AbilityManagerModel : TWDModelObject, IModifierCollection
	{
		private ModifierCollection modifierCollection;

		private Dictionary<Faction, FactionLeaderModifiers> factionModifierCollections = new Dictionary<Faction, FactionLeaderModifiers>();

		private ModifierCollection survivorBuffsCollection;

		private ModifierCollection survivorGuildBattleBuffsCollection;

		private ModifierCollection featuredHeroBuffsCollection;

		[JsonIgnore]
		private List<AbilityAction> pendingAbilityActions;

		public AbilityModel AbilityUnderApplication { get; private set; }

		public ActorModel AbilityOwnerActor { get; private set; }

		public event AbilityPerformedHandler AbilityPerformed;

		public event AfterApplyEffectHandler AfterEffectApplied;

		public int GetCount()
		{
			return modifierCollection.GetCount();
		}

		public ModelModifier GetModifier(int index)
		{
			return modifierCollection.GetModifier(index);
		}

		public override bool IsValid()
		{
			return true;
		}

		public override void Initialize()
		{
			base.Initialize();
			modifierCollection = new ModifierCollection();
			modifierCollection.SetManager(base.manager);
			modifierCollection.Initialize();
			CreateFactionModifierCollections();
			survivorBuffsCollection = new ModifierCollection();
			survivorBuffsCollection.SetManager(base.manager);
			survivorBuffsCollection.Initialize();
			survivorGuildBattleBuffsCollection = new ModifierCollection();
			survivorGuildBattleBuffsCollection.SetManager(base.manager);
			survivorGuildBattleBuffsCollection.Initialize();
			featuredHeroBuffsCollection = new ModifierCollection();
			featuredHeroBuffsCollection.SetManager(base.manager);
			featuredHeroBuffsCollection.Initialize();
			pendingAbilityActions = new List<AbilityAction>();
		}

		public void RegisterModifier(ModelModifier modifier)
		{
			modifierCollection.RegisterModifier(modifier);
		}

		public void RemoveModifier(ModelModifier modifier)
		{
			modifierCollection.RemoveModifier(modifier);
		}

		public bool HasModifier(ModelModifier modifier)
		{
			return modifierCollection.HasModifier(modifier);
		}

		public void RegisterFactionModifier(ActorModel leader, Faction faction, ModelModifier modifier)
		{
			FactionLeaderModifiers value = null;
			if (factionModifierCollections.TryGetValue(faction, out value))
			{
				value.Leader = leader;
				value.RegisterModifier(modifier);
			}
		}

		public void RegisterFactionBuffs(ModelModifier modifier)
		{
			survivorBuffsCollection.RegisterModifier(modifier);
		}

		public void RegisterGuildBattleBuffs(ModelModifier modifier)
		{
			survivorGuildBattleBuffsCollection.RegisterModifier(modifier);
		}

		public void RegisterFeaturedHeroBuffs(ModelModifier modifier)
		{
			featuredHeroBuffsCollection.RegisterModifier(modifier);
		}

		public void RemoveFactionModifier(Faction faction, ModelModifier modifier)
		{
			FactionLeaderModifiers value = null;
			if (factionModifierCollections.TryGetValue(faction, out value))
			{
				value.RemoveModifier(modifier);
			}
		}

		public void RemoveAllFactionBuffs()
		{
			survivorBuffsCollection = new ModifierCollection();
			survivorBuffsCollection.SetManager(base.manager);
			survivorBuffsCollection.Initialize();
		}

		public void RemoveAllGuildBattleBuffs()
		{
			survivorGuildBattleBuffsCollection = new ModifierCollection();
			survivorGuildBattleBuffsCollection.SetManager(base.manager);
			survivorGuildBattleBuffsCollection.Initialize();
		}

		public void RemoveAllFeaturedHeroBuffs()
		{
			featuredHeroBuffsCollection = new ModifierCollection();
			featuredHeroBuffsCollection.SetManager(base.manager);
			featuredHeroBuffsCollection.Initialize();
		}

		public bool HasFactionModifier(Faction faction, ModelModifier modifier)
		{
			FactionLeaderModifiers value = null;
			if (factionModifierCollections.TryGetValue(faction, out value))
			{
				return value.HasModifier(modifier);
			}
			return false;
		}

		public bool HasLeaderTraitAlreadyRegistered(Faction faction)
		{
			FactionLeaderModifiers value = null;
			factionModifierCollections.TryGetValue(faction, out value);
			if (value != null)
			{
				return value.GetCount() > 0;
			}
			return false;
		}

		public void CreateFactionModifierCollections()
		{
			if (factionModifierCollections == null)
			{
				factionModifierCollections = new Dictionary<Faction, FactionLeaderModifiers>();
			}
			InitializeModifersForAllFactions(factionModifierCollections);
		}

		private void InitializeModifersForAllFactions(Dictionary<Faction, FactionLeaderModifiers> factionModifiers)
		{
			for (int i = 0; i < Enum.GetValues(typeof(Faction)).Length; i++)
			{
				FactionLeaderModifiers value = null;
				if (!factionModifierCollections.TryGetValue((Faction)i, out value))
				{
					value = new FactionLeaderModifiers();
					value.SetManager(base.manager);
					value.Initialize();
					factionModifierCollections.Add((Faction)i, value);
				}
			}
		}

		public void ClearFactionModifiers()
		{
			if (factionModifierCollections == null)
			{
				factionModifierCollections = new Dictionary<Faction, FactionLeaderModifiers>();
			}
			factionModifierCollections.Clear();
			InitializeModifersForAllFactions(factionModifierCollections);
		}

		public void ClearLeaderModifiersForFaction(Faction faction)
		{
			FactionLeaderModifiers factionLeaderModifiers = new FactionLeaderModifiers();
			factionLeaderModifiers.SetManager(base.manager);
			factionLeaderModifiers.Initialize();
			factionModifierCollections[faction] = factionLeaderModifiers;
		}

		public bool VisitParameterWithAbility(AbilityModel ability, string paramName, ref FixedPoint value, ActorModel actor = null)
		{
			bool flag = false;
			FixedPoint oldValue = value;
			if (ability != null && ability.Definition.Type != AbilityType.Passive)
			{
				oldValue = value;
				if (ability.Modifiers.VisitParameter(paramName, ref value, actor))
				{
					if (base.manager.CurrentCommandLogEntry != null)
					{
						base.manager.CurrentCommandLogEntry.ParameterModifiedAbilityActive(paramName, oldValue, value, actor, ability);
					}
					flag = true;
				}
			}
			if (!flag)
			{
				return VisitParameter(paramName, ref value, ref oldValue, actor);
			}
			return true;
		}

		private bool VisitParameter(string paramName, ref FixedPoint value, ref FixedPoint oldValue, ActorModel actor = null)
		{
			bool result = false;
			if (actor == null && base.manager.CombatModel != null)
			{
				actor = base.manager.CombatModel.ActiveActor;
			}
			if (actor != null)
			{
				List<AbilityModel> models = actor.Abilities.Models;
				for (int i = 0; i < models.Count; i++)
				{
					AbilityModel abilityModel = models[i];
					if (abilityModel.Definition.Type != AbilityType.Passive)
					{
						continue;
					}
					oldValue = value;
					if (abilityModel.Modifiers.VisitParameter(paramName, ref value, actor))
					{
						if (base.manager.CurrentCommandLogEntry != null)
						{
							base.manager.CurrentCommandLogEntry.ParameterModifiedAbilityPassive(paramName, oldValue, value, actor, abilityModel);
						}
						result = true;
					}
				}
				if (actor.Modifiers != null)
				{
					oldValue = value;
					if (actor.Modifiers.VisitParameter(paramName, ref value, actor))
					{
						if (base.manager.CurrentCommandLogEntry != null)
						{
							base.manager.CurrentCommandLogEntry.ParameterModifiedActorPassive(paramName, oldValue, value, actor);
						}
						result = true;
					}
				}
				FactionLeaderModifiers value2 = null;
				if (factionModifierCollections.TryGetValue(actor.Faction, out value2) && value2 != null && value2.Leader != actor)
				{
					oldValue = value;
					if (value2 != null && value2.VisitParameter(paramName, ref value, actor))
					{
						if (base.manager.CurrentCommandLogEntry != null)
						{
							base.manager.CurrentCommandLogEntry.ParameterModifiedFactionPassive(paramName, oldValue, value, actor);
						}
						result = true;
					}
				}
				if (survivorBuffsCollection != null)
				{
					oldValue = value;
					if (survivorBuffsCollection.VisitParameter(paramName, ref value, actor))
					{
						if (base.manager.CurrentCommandLogEntry != null)
						{
							base.manager.CurrentCommandLogEntry.ParameterModifiedFactionPassive(paramName, oldValue, value, actor);
						}
						result = true;
					}
				}
				if (survivorGuildBattleBuffsCollection != null)
				{
					oldValue = value;
					if (survivorGuildBattleBuffsCollection.VisitParameter(paramName, ref value, actor))
					{
						if (base.manager.CurrentCommandLogEntry != null)
						{
							base.manager.CurrentCommandLogEntry.ParameterModifiedFactionPassive(paramName, oldValue, value, actor);
						}
						result = true;
					}
				}
				if (featuredHeroBuffsCollection != null)
				{
					oldValue = value;
					if (featuredHeroBuffsCollection.VisitParameter(paramName, ref value, actor))
					{
						if (base.manager.CurrentCommandLogEntry != null)
						{
							base.manager.CurrentCommandLogEntry.ParameterModifiedFactionPassive(paramName, oldValue, value, actor);
						}
						result = true;
					}
				}
			}
			return result;
		}

		public bool VisitParameter(string paramName, ref FixedPoint value, ActorModel actor = null)
		{
			return VisitParameterWithAbility(AbilityUnderApplication, paramName, ref value, actor);
		}

		public void VisitActions(ModelAction action, ActorModel nullActor, List<ModelAction> addedActions)
		{
			if (AbilityUnderApplication != null && AbilityUnderApplication.Definition.Type != AbilityType.Passive)
			{
				AbilityUnderApplication.Modifiers.VisitActions(action, null, addedActions);
			}
			if (base.manager.CombatModel != null)
			{
				ActorModel activeActor = base.manager.CombatModel.ActiveActor;
				if (activeActor != null)
				{
					for (int i = 0; i < activeActor.Abilities.Count; i++)
					{
						AbilityModel abilityModel = activeActor.Abilities[i];
						if (abilityModel.Definition.Type == AbilityType.Passive)
						{
							abilityModel.Modifiers.VisitActions(action, activeActor, addedActions);
						}
					}
					if (activeActor.Modifiers != null)
					{
						activeActor.Modifiers.VisitActions(action, activeActor, addedActions);
					}
				}
				List<ActorModel> allActors = base.manager.CombatModel.GetAllActors();
				for (int j = 0; j < allActors.Count; j++)
				{
					ActorModel actorModel = allActors[j];
					if (actorModel == activeActor)
					{
						continue;
					}
					for (int k = 0; k < actorModel.Abilities.Count; k++)
					{
						AbilityModel abilityModel2 = actorModel.Abilities[k];
						if (abilityModel2.Definition.Type == AbilityType.Passive)
						{
							abilityModel2.Modifiers.VisitActions(action, actorModel, addedActions);
						}
					}
					if (actorModel.Modifiers != null)
					{
						actorModel.Modifiers.VisitActions(action, actorModel, addedActions);
					}
				}
			}
			modifierCollection.VisitActions(action, null, addedActions);
			action.Visited = true;
		}

		public AbilityResult PerformAbility(ActorModel sourceActor, AbilityModel ability, GridCoordinate targetCell, ActorModel targetActor = null, Dictionary<RollDiceType, PlayerRandomChanceResult> resolvedRolls = null, OOTType ootType = OOTType.None, bool isAssistAttack = false, bool isTriggerExtraAttackDamage = false)
		{
			AbilityResult abilityResult = AbilityResult.Success;
			CombatModel combat = base.manager.Player.Combat;
			ability.OnApply();
			if (combat != null && sourceActor.IsValid())
			{
				abilityResult = ability.CanAbilityBePerformedOnGridCell(combat, sourceActor, sourceActor.GridCoordinate, targetCell);
				if (abilityResult == AbilityResult.Success)
				{
					if (ability.MaxUses < 0 || ability.TotalUses < ability.MaxUses || ability.UsesThisTurn < ability.MaxUsesPerTurn)
					{
						AbilityModel abilityUnderApplication = AbilityUnderApplication;
						ActorModel abilityOwnerActor = AbilityOwnerActor;
						AbilityUnderApplication = ability;
						AbilityOwnerActor = sourceActor;
						bool flag = true;
						if (targetActor == null)
						{
							targetActor = combat.GetOccupier(targetCell);
							if (targetActor != null)
							{
								targetActor.HasHeadshotLTTriggered = false;
							}
						}
						sourceActor.VisitedExtraApChance = false;
						sourceActor.VisitedRedactChance = false;
						sourceActor.MainTargetCell = targetCell;
						List<ActorModel> listOfActorsToBeTargetted = combat.AbilityManager.GetListOfActorsToBeTargetted(ability, sourceActor, sourceActor.GridCoordinate, targetCell);
						sourceActor.NumberOfEnemiesAttacked = listOfActorsToBeTargetted.Count;
						for (int i = 0; i < ability.Effects.Count; i++)
						{
							AbilityEffect abilityEffect = ability.Effects[i];
							if (base.manager.CurrentCommandLogEntry != null)
							{
								base.manager.CurrentCommandLogEntry.StartEffect(abilityEffect.GetType().Name);
							}
							flag = abilityEffect.ApplyEffect(combat, sourceActor, targetCell, targetActor, resolvedRolls, ootType, isAssistAttack, isTriggerExtraAttackDamage);
							if (base.manager.CurrentCommandLogEntry != null)
							{
								base.manager.CurrentCommandLogEntry.EndEffect(flag);
							}
							if (!flag)
							{
								IModelDebug debug = base.Debug;
								string name = sourceActor.Name;
								GridCoordinate gridCoordinate = targetCell;
								debug.Log("PerformAbility failed -> Failed to apply effect for actor " + name + " at " + gridCoordinate.ToString());
								abilityResult = AbilityResult.FailedEffectFailed;
								break;
							}
						}
						sourceActor.NumberOfEnemiesAttacked = 0;
						AbilityUnderApplication = abilityUnderApplication;
						AbilityOwnerActor = abilityOwnerActor;
						if (abilityResult == AbilityResult.Success && ability.LinkedAbility != null)
						{
							abilityResult = PerformAbility(sourceActor, ability.LinkedAbility, targetCell, targetActor, null, ootType);
						}
						ability.UsesThisTurn++;
						if (ootType == OOTType.PassByAttack || ootType == OOTType.AutoAttack)
						{
							sourceActor.PassByAttackedOnMove = true;
							sourceActor.FightingFuryActivated = false;
						}
						else if (sourceActor.CanMoveWithoutAttacking)
						{
							sourceActor.PassByAttackedOnMove = false;
							if (!sourceActor.GainedAPFromAbilityExecution)
							{
								sourceActor.FightingFuryActivated = true;
							}
						}
					}
					else
					{
						abilityResult = AbilityResult.FailedOutOfUses;
					}
				}
				else
				{
					base.manager.Debug.LogError($"PerformAbility CanAbilityBePerformedOnGridCell failed: {abilityResult}");
				}
			}
			else
			{
				base.manager.Debug.LogError("PerformAbility failed: combatModel != null [" + ((combat != null) ? "TRUE" : "FALSE") + "] - sourceActor.IsValid [" + (sourceActor.IsValid() ? "TRUE" : "FALSE") + "]");
				abilityResult = AbilityResult.FailedNoValidSource;
			}
			ability.ExecutePostActions();
			if (abilityResult == AbilityResult.Success)
			{
				NotifyAfterEffectApplied();
				if (ability.Definition.NeedsReloading)
				{
					sourceActor.StartReloading();
				}
				ability.OnApplied();
				if (ability.Definition.IsFreeAction)
				{
					if (!sourceActor.IsInteractingWithGuts)
					{
						sourceActor.ClearInvisibility();
					}
					sourceActor.IsInteractingWithGuts = false;
					return abilityResult;
				}
				bool flag2 = false;
				bool flag3 = false;
				bool flag4 = sourceActor.IsDead || sourceActor.IsBleedingOut || sourceActor.IsStruggling || sourceActor.IsStunned || sourceActor.IsElectricShocked || sourceActor.IsQuantunCanNotMove;
				PlayerRandomChanceResult playerRandomChanceResult = PlayerRandomChanceResult.Failed;
				bool flag5 = false;
				string text = "";
				bool freeAttackUsedOnAbility = false;
				if (ootType != OOTType.PassByAttack && !flag4 && ootType != OOTType.AutoAttack)
				{
					FixedPoint fixedPoint = 0.0;
					FixedPoint value = 0.0;
					if (sourceActor.HasAnyLevelTrait("Equipment_Active_ExtraAP"))
					{
						flag2 = (sourceActor.HasGainedExtraAP = true);
						sourceActor.EnsureGainedExtraMoveAp = false;
						sourceActor.GainedAPFromAbilityExecution = true;
						freeAttackUsedOnAbility = true;
					}
					if (!sourceActor.HasGainedExtraAP && !sourceActor.MoveCompleted)
					{
						FixedPoint value2 = 0.0;
						if (VisitParameter("AbilityModifierRepulseGainAPChance", ref value2, sourceActor))
						{
							text = "Repulse";
							sourceActor.OneTurnCriticalHit = true;
							sourceActor.OneTurnStagger = true;
						}
						fixedPoint = 0.0;
						if (value2 != 0.0)
						{
							base.manager.CombatModel.AbilityManager.VisitParameter("ExtendProbability", ref fixedPoint, sourceActor);
						}
						playerRandomChanceResult = base.manager.Player.RollDice(RollDiceType.GainAP, value2, fixedPoint);
						flag2 = (sourceActor.HasGainedExtraAP = playerRandomChanceResult != PlayerRandomChanceResult.Failed);
						if (sourceActor.HasGainedExtraAP)
						{
							sourceActor.EnsureGainedExtraMoveAp = false;
						}
					}
					if (!sourceActor.HasGainedExtraAP && !sourceActor.MoveCompleted)
					{
						FixedPoint value3 = 0.0;
						if (VisitParameter("AbilityModifierAdvanceGainAPChance", ref value3, sourceActor))
						{
							text = "Equipment_Active_Advance";
							sourceActor.OneTurnCriticalHit = true;
						}
						fixedPoint = 0.0;
						if (value3 != 0.0)
						{
							base.manager.CombatModel.AbilityManager.VisitParameter("ExtendProbability", ref fixedPoint, sourceActor);
						}
						playerRandomChanceResult = base.manager.Player.RollDice(RollDiceType.GainAP, value3, fixedPoint);
						flag2 = (sourceActor.HasGainedExtraAP = playerRandomChanceResult != PlayerRandomChanceResult.Failed);
						if (sourceActor.HasGainedExtraAP)
						{
							sourceActor.EnsureGainedExtraMoveAp = false;
						}
					}
					if (!sourceActor.HasGainedExtraAP)
					{
						if (sourceActor.EnsureExtraAP)
						{
							flag2 = (sourceActor.HasGainedExtraAP = true);
							if (sourceActor.HasAnyLevelTrait("LeaderBuffJustice"))
							{
								text = "LeaderBuffJustice";
							}
						}
						else if (sourceActor.AttackKilledAnyEnemy)
						{
							if (VisitParameter("AbilityModifierIncreaseExtraAPChance", ref value, sourceActor))
							{
								text = "Inspiration";
							}
							if (VisitParameter("AbilityModifierPursuitAP", ref value, sourceActor))
							{
								text = "Pursuit";
								sourceActor.OneTurnCriticalHit = true;
							}
							if (sourceActor.SelectedEquipment.Definition.Category == EquipmentCategory.MeleeWeapon && !sourceActor.FollowUpAttackedOnTurn && VisitParameter("AbilityModifierIncreaseExtraAPChanceForMelee", ref value, sourceActor))
							{
								text = "LeaderBuffReduceThreatMelee";
							}
							fixedPoint = 0.0;
							if (value != 0.0)
							{
								base.manager.CombatModel.AbilityManager.VisitParameter("ExtendProbability", ref fixedPoint, sourceActor);
							}
							playerRandomChanceResult = base.manager.Player.RollDice(RollDiceType.GainAP, value, fixedPoint);
							flag2 = (sourceActor.HasGainedExtraAP = playerRandomChanceResult != PlayerRandomChanceResult.Failed);
							if (sourceActor.HasGainedExtraAP)
							{
								sourceActor.EnsureGainedExtraMoveAp = false;
							}
						}
					}
					if (sourceActor.AttackChainGainExtraActionPoint)
					{
						flag2 = (sourceActor.HasGainedExtraAP = true);
						sourceActor.EnsureGainedExtraMoveAp = false;
						text = "";
					}
				}
				if (!flag2)
				{
					if (ootType != OOTType.PassByAttack && ootType != OOTType.AutoAttack && ootType != OOTType.FreeShooting && base.manager.CombatModel.TurnManager.ActiveFaction == sourceActor.Faction)
					{
						FixedPoint value4 = 0.0;
						FixedPoint value5 = 0.0;
						bool num = VisitParameterWithAbility(ability, "AbilityModifierIncreaseMoveRangeForSecondMoveColdBlooded", ref value4, sourceActor);
						bool flag12 = VisitParameterWithAbility(ability, "AbilityModifierIncreaseMoveRangeForSecondMoveDeadlyTactics", ref value5, sourceActor);
						bool flag13 = num || flag12;
						FixedPoint value6 = 0L;
						FixedPoint value7 = 0L;
						bool flag14 = VisitParameterWithAbility(ability, "AbilityModifierIncreaseMoveRangeForSecondMove", ref value6, sourceActor);
						bool flag15 = VisitParameterWithAbility(ability, "AbilityModifierIncreaseMoveRangeForSecondMoveTacticalArmor", ref value7, sourceActor);
						if (!sourceActor.HasGainedExtraMoveAp && sourceActor.EnsureGainedExtraMoveAp && flag13)
						{
							text = sourceActor.ExtraMoveApNotificationKey;
							sourceActor.EndAbilityAction(allowSecondMove: true, Math.Max((int)value4, (int)value5), resetMoveCompleted: true);
							sourceActor.NotifyChange("ActorExtraMoveAction", new object[2] { text, false });
							sourceActor.HasGainedExtraAP = true;
							sourceActor.HasGainedExtraMoveAp = true;
							sourceActor.SecondMoveCompleted = false;
							flag3 = true;
						}
						else if (sourceActor.AttackHasNotKilledAllEnemies && !sourceActor.HasGainedExtraMoveAp)
						{
							FixedPoint value8 = 0.0;
							if (base.manager.CombatModel.AbilityManager.VisitParameter("LeaderBuffGoodEnough", ref value8, sourceActor))
							{
								sourceActor.HasGainedExtraMoveAp = true;
								sourceActor.EndAbilityAction(allowSecondMove: true, 0, resetMoveCompleted: true);
								sourceActor.NotifyChange("ActorExtraMoveAction", new object[2] { "LeaderBuffGoodEnough", false });
								sourceActor.SecondMoveCompleted = false;
								flag3 = true;
							}
						}
						if (!sourceActor.HasGainedExtraAP && !sourceActor.HasGainedExtraMoveAp)
						{
							foreach (object model in base.manager.CombatModel.Models)
							{
								if (!sourceActor.IsMeleeClass)
								{
									break;
								}
								if (model is PitfallArea pitfallArea && pitfallArea.Faction == sourceActor.Faction && (pitfallArea.IsNearAreaGrid(sourceActor.GridCoordinate) || pitfallArea.IsInArea(sourceActor.GridCoordinate)))
								{
									sourceActor.HasGainedExtraMoveAp = true;
									sourceActor.EndAbilityAction(allowSecondMove: true, 0, resetMoveCompleted: true);
									sourceActor.NotifyChange("ActorExtraMoveAction", new object[2] { "LeaderBuffUnleashedFighter", false });
									sourceActor.SecondMoveCompleted = false;
									flag3 = true;
									break;
								}
							}
						}
						if ((flag14 || flag15) && !sourceActor.HasGainedExtraMoveAp)
						{
							FixedPoint fixedPoint2 = FixedPoint.Max(value7, value6);
							flag5 = (int)fixedPoint2 > 0 && !flag4 && !sourceActor.FocusModeState;
							sourceActor.EndAbilityAction(flag5, (int)fixedPoint2);
							flag3 = flag5;
						}
					}
				}
				else
				{
					if (!string.IsNullOrEmpty(text))
					{
						sourceActor.NotifyChange("AbilityVisited", new object[2]
						{
							text,
							playerRandomChanceResult == PlayerRandomChanceResult.SuccessDueToExtension
						});
					}
					sourceActor.EnsureExtraAction(text, playerRandomChanceResult == PlayerRandomChanceResult.SuccessDueToExtension);
				}
				bool flag16 = ability is FiringSquadAbility || ootType == OOTType.PassByAttack || ootType == OOTType.AutoAttack || ootType == OOTType.FreeShooting;
				if (sourceActor.CanMoveWithoutAttacking && !flag16)
				{
					sourceActor.HandleAdditionalAttacks(flag2, freeAttackUsedOnAbility);
				}
				else if (!flag2 && !flag3 && !flag16)
				{
					sourceActor.EndAbilityAction();
				}
			}
			return abilityResult;
		}

		public List<ActorModel> GetListOfActorsToBeTargetted(AbilityModel ability, ActorModel source, GridCoordinate sourceCell, GridCoordinate targetCell)
		{
			List<ActorModel> list = new List<ActorModel>();
			CombatModel combatModel = base.manager.CombatModel;
			if (source.DeathsDoor_IsPursuitAttack && !ability.IsChargeAttack)
			{
				ActorModel occupier = combatModel.GetOccupier(targetCell);
				if (occupier != null && occupier.IsEnemy(source))
				{
					list.Add(occupier);
				}
				return list;
			}
			bool hasFriendlyFire = ability.Definition.HasFriendlyFire;
			bool canBeBlocked = ability.Definition.CanBeBlocked;
			if (AbilityRangeTridentSkill.ShouldApplySeparatedAttackLines(source, ability))
			{
				AppendTridentSeparatedLineTargets(list, combatModel, ability, source, sourceCell, targetCell, hasFriendlyFire, canBeBlocked);
			}
			else if (ability.Definition.AbilityTargetArea == AbilityTargetAreaType.Line)
			{
				List<ActorModel> actorsInLine = combatModel.GetActorsInLine(sourceCell, targetCell, source);
				for (int i = 0; i < actorsInLine.Count; i++)
				{
					ActorModel actorModel = actorsInLine[i];
					if (actorModel.IsEnemy(source) || (hasFriendlyFire && !actorModel.IsStruggling))
					{
						bool flag = false;
						if (ability.Definition.RequiresLineOfSight && ability.Definition.EffectSource == EffectSource.SourceActor && (!combatModel.IsGridCellVisible(sourceCell, actorModel.GridCoordinate) || (canBeBlocked && !combatModel.IsGridCellPenetrable(sourceCell, targetCell, actorModel.GridCoordinate))))
						{
							flag = true;
						}
						else if (ability.Definition.RequiresLineOfMovement && combatModel.IsGridLineMovementBlocked(sourceCell, actorModel.GridCoordinate))
						{
							flag = true;
						}
						if (!flag)
						{
							list.Add(actorModel);
						}
					}
				}
			}
			else if (ability.Definition.AbilityTargetArea == AbilityTargetAreaType.LineMax)
			{
				FixedVec3 position = combatModel.Grid.GetPosition(sourceCell);
				FixedVec3 fixedVec = FixedVec3.Normalize(combatModel.Grid.GetPosition(targetCell) - position);
				FixedPoint range = ability.Definition.AbilityRange;
				if (!ability.IsConsumableAbility)
				{
					CombatHelpers.CalculateRangeExtension(ref range, source, combatModel.AbilityManager);
				}
				FixedPoint fixedPoint = range * combatModel.Grid.CellSize.X;
				FixedVec3 position2 = position + fixedVec * fixedPoint;
				GridCoordinate coordinate = combatModel.Grid.GetCoordinate(position2);
				List<ActorModel> actorsInLine2 = combatModel.GetActorsInLine(sourceCell, coordinate, source);
				for (int j = 0; j < actorsInLine2.Count; j++)
				{
					ActorModel actorModel2 = actorsInLine2[j];
					if (actorModel2.IsEnemy(source) || (hasFriendlyFire && !actorModel2.IsStruggling))
					{
						bool flag2 = false;
						if (ability.Definition.RequiresLineOfSight && ability.Definition.EffectSource == EffectSource.SourceActor && (!combatModel.IsGridCellVisible(sourceCell, actorModel2.GridCoordinate) || (canBeBlocked && !combatModel.IsGridCellPenetrable(sourceCell, coordinate, actorModel2.GridCoordinate))))
						{
							flag2 = true;
						}
						else if (ability.Definition.RequiresLineOfMovement && combatModel.IsGridLineMovementBlocked(sourceCell, actorModel2.GridCoordinate))
						{
							flag2 = true;
						}
						if (!flag2)
						{
							list.Add(actorModel2);
						}
					}
				}
			}
			else if (ability.Definition.AbilityTargetArea == AbilityTargetAreaType.Circle)
			{
				List<ActorModel> actorsInRange = combatModel.GetActorsInRange(targetCell, (int)ability.Definition.AbilityTargetAreaRadius, ability.Definition.AbilityTargetDiagonal);
				for (int k = 0; k < actorsInRange.Count; k++)
				{
					ActorModel actorModel3 = actorsInRange[k];
					if (actorModel3.IsEnemy(source) || (hasFriendlyFire && !actorModel3.IsStruggling))
					{
						bool flag3 = false;
						if (ability.Definition.RequiresLineOfSight && ability.Definition.EffectSource == EffectSource.SourceActor && (!combatModel.IsGridCellVisible(sourceCell, actorModel3.GridCoordinate) || (canBeBlocked && !combatModel.IsGridCellPenetrable(sourceCell, actorModel3.GridCoordinate, actorModel3.GridCoordinate))))
						{
							flag3 = true;
						}
						else if (ability.Definition.RequiresLineOfMovement && combatModel.IsGridLineMovementBlocked(sourceCell, actorModel3.GridCoordinate))
						{
							flag3 = true;
						}
						if (!flag3)
						{
							list.Add(actorModel3);
						}
					}
				}
			}
			else if (ability.Definition.AbilityTargetArea == AbilityTargetAreaType.ConeRight || ability.Definition.AbilityTargetArea == AbilityTargetAreaType.ConeLeft)
			{
				List<GridCoordinate> list2 = combatModel.GetAbilityTargetsInRange(ability, source, sourceCell);
				FixedPoint value = ability.Definition.AbilityTargetAreaAngle;
				if (!ability.IsConsumableAbility)
				{
					VisitParameter("AbilityModifierIncreaseConeAngle", ref value, source);
					VisitParameter("AbilityModifierThreatArcUpgrade", ref value, source);
				}
				if (value > 0L)
				{
					FixedPoint angleOffset = ((ability.Definition.AbilityTargetArea == AbilityTargetAreaType.ConeRight) ? (value * 0.5) : (value * -0.5));
					list2 = FilterCoordinatesByAngle(list2, sourceCell, targetCell, value, angleOffset);
				}
				list2.Remove(targetCell);
				list2.Insert(0, targetCell);
				if (list2.Count > ability.Definition.MaxAffectedTargetsCount)
				{
					list2 = list2.GetRange(0, ability.Definition.MaxAffectedTargetsCount);
				}
				for (int l = 0; l < list2.Count; l++)
				{
					GridCoordinate coordinate2 = list2[l];
					ActorModel occupier2 = combatModel.GetOccupier(coordinate2);
					if (occupier2 != null && (occupier2.IsEnemy(source) || (hasFriendlyFire && !occupier2.IsStruggling)))
					{
						list.Add(occupier2);
					}
				}
			}
			else if (ability.Definition.MaxAffectedTargetsCount > 1)
			{
				List<GridCoordinate> list3 = combatModel.GetAbilityTargetsInRange(ability, source, sourceCell);
				FixedPoint value2 = ability.Definition.AbilityTargetAreaAngle;
				if (!ability.IsConsumableAbility)
				{
					VisitParameter("AbilityModifierIncreaseConeAngle", ref value2, source);
					VisitParameter("AbilityModifierThreatArcUpgrade", ref value2, source);
				}
				if (value2 > 0L && value2 < 360L)
				{
					list3 = FilterCoordinatesByAngle(list3, sourceCell, targetCell, value2, 0L);
				}
				list3.Remove(targetCell);
				list3.Insert(0, targetCell);
				if (list3.Count > ability.Definition.MaxAffectedTargetsCount)
				{
					list3 = list3.GetRange(0, ability.Definition.MaxAffectedTargetsCount);
				}
				for (int m = 0; m < list3.Count; m++)
				{
					GridCoordinate gridCoordinate = list3[m];
					ActorModel occupier3 = combatModel.GetOccupier(gridCoordinate);
					if (occupier3 != null && (occupier3.IsEnemy(source) || (hasFriendlyFire && !occupier3.IsStruggling)) && (!(ability.Definition.RequiresLineOfSight && ability.Definition.EffectSource == EffectSource.SourceActor && canBeBlocked) || combatModel.IsGridCellPenetrable(sourceCell, gridCoordinate, gridCoordinate)))
					{
						list.Add(occupier3);
					}
				}
			}
			else
			{
				ActorModel occupier4 = combatModel.GetOccupier(targetCell);
				if (occupier4 != null && occupier4.IsEnemy(source))
				{
					list.Add(occupier4);
				}
			}
			if (source.HasAnyLevelTrait("Equipment_Active_Recoil"))
			{
				List<ActorModel> oneGridEnemyActorModels = GetOneGridEnemyActorModels(source);
				if (oneGridEnemyActorModels != null && oneGridEnemyActorModels.Count > 0)
				{
					for (int n = 0; n < oneGridEnemyActorModels.Count; n++)
					{
						if (!list.Contains(oneGridEnemyActorModels[n]))
						{
							oneGridEnemyActorModels[n].IsRecoilEffected = true;
							list.Add(oneGridEnemyActorModels[n]);
						}
					}
				}
			}
			ActorModel occupier5 = combatModel.GetOccupier(targetCell);
			if (occupier5 != null && list.Contains(occupier5))
			{
				list.Remove(occupier5);
				list.Insert(0, occupier5);
			}
			return list;
		}

		public bool HasAnyValidActorToBeTargetted(AbilityModel ability, ActorModel source, GridCoordinate sourceCell, GridCoordinate targetCell, FixedPoint preComputedRange, bool requiresLineOfSight, bool requiresLineOfMovement)
		{
			CombatModel combatModel = base.manager.CombatModel;
			bool hasFriendlyFire = ability.Definition.HasFriendlyFire;
			bool canBeBlocked = ability.Definition.CanBeBlocked;
			if (AbilityRangeTridentSkill.ShouldApplySeparatedAttackLines(source, ability))
			{
				List<ActorModel> list = new List<ActorModel>();
				AppendTridentSeparatedLineTargets(list, combatModel, ability, source, sourceCell, targetCell, hasFriendlyFire, canBeBlocked);
				for (int i = 0; i < list.Count; i++)
				{
					if (IsActorVisibleForValidation(list[i], sourceCell, targetCell, requiresLineOfSight, requiresLineOfMovement, combatModel))
					{
						return true;
					}
				}
			}
			else if (ability.Definition.AbilityTargetArea == AbilityTargetAreaType.Line)
			{
				List<ActorModel> actorsInLine = combatModel.GetActorsInLine(sourceCell, targetCell, source);
				for (int j = 0; j < actorsInLine.Count; j++)
				{
					ActorModel actorModel = actorsInLine[j];
					if (actorModel.IsEnemy(source) || (hasFriendlyFire && !actorModel.IsStruggling))
					{
						bool flag = false;
						if (ability.Definition.RequiresLineOfSight && ability.Definition.EffectSource == EffectSource.SourceActor && (!combatModel.IsGridCellVisible(sourceCell, actorModel.GridCoordinate) || (canBeBlocked && !combatModel.IsGridCellPenetrable(sourceCell, targetCell, actorModel.GridCoordinate))))
						{
							flag = true;
						}
						else if (ability.Definition.RequiresLineOfMovement && combatModel.IsGridLineMovementBlocked(sourceCell, actorModel.GridCoordinate))
						{
							flag = true;
						}
						if (!flag && IsActorVisibleForValidation(actorModel, sourceCell, targetCell, requiresLineOfSight, requiresLineOfMovement, combatModel))
						{
							return true;
						}
					}
				}
			}
			else if (ability.Definition.AbilityTargetArea == AbilityTargetAreaType.LineMax)
			{
				FixedVec3 position = combatModel.Grid.GetPosition(sourceCell);
				FixedVec3 fixedVec = FixedVec3.Normalize(combatModel.Grid.GetPosition(targetCell) - position);
				FixedPoint fixedPoint = preComputedRange * combatModel.Grid.CellSize.X;
				FixedVec3 position2 = position + fixedVec * fixedPoint;
				GridCoordinate coordinate = combatModel.Grid.GetCoordinate(position2);
				List<ActorModel> actorsInLine2 = combatModel.GetActorsInLine(sourceCell, coordinate, source);
				for (int k = 0; k < actorsInLine2.Count; k++)
				{
					ActorModel actorModel2 = actorsInLine2[k];
					if (actorModel2.IsEnemy(source) || (hasFriendlyFire && !actorModel2.IsStruggling))
					{
						bool flag2 = false;
						if (ability.Definition.RequiresLineOfSight && ability.Definition.EffectSource == EffectSource.SourceActor && (!combatModel.IsGridCellVisible(sourceCell, actorModel2.GridCoordinate) || (canBeBlocked && !combatModel.IsGridCellPenetrable(sourceCell, coordinate, actorModel2.GridCoordinate))))
						{
							flag2 = true;
						}
						else if (ability.Definition.RequiresLineOfMovement && combatModel.IsGridLineMovementBlocked(sourceCell, actorModel2.GridCoordinate))
						{
							flag2 = true;
						}
						if (!flag2 && IsActorVisibleForValidation(actorModel2, sourceCell, targetCell, requiresLineOfSight, requiresLineOfMovement, combatModel))
						{
							return true;
						}
					}
				}
			}
			else if (ability.Definition.AbilityTargetArea == AbilityTargetAreaType.Circle)
			{
				List<ActorModel> actorsInRange = combatModel.GetActorsInRange(targetCell, (int)ability.Definition.AbilityTargetAreaRadius, ability.Definition.AbilityTargetDiagonal);
				for (int l = 0; l < actorsInRange.Count; l++)
				{
					ActorModel actorModel3 = actorsInRange[l];
					if (actorModel3.IsEnemy(source) || (hasFriendlyFire && !actorModel3.IsStruggling))
					{
						bool flag3 = false;
						if (ability.Definition.RequiresLineOfSight && ability.Definition.EffectSource == EffectSource.SourceActor && (!combatModel.IsGridCellVisible(sourceCell, actorModel3.GridCoordinate) || (canBeBlocked && !combatModel.IsGridCellPenetrable(sourceCell, actorModel3.GridCoordinate, actorModel3.GridCoordinate))))
						{
							flag3 = true;
						}
						else if (ability.Definition.RequiresLineOfMovement && combatModel.IsGridLineMovementBlocked(sourceCell, actorModel3.GridCoordinate))
						{
							flag3 = true;
						}
						if (!flag3 && IsActorVisibleForValidation(actorModel3, sourceCell, targetCell, requiresLineOfSight, requiresLineOfMovement, combatModel))
						{
							return true;
						}
					}
				}
			}
			else if (ability.Definition.AbilityTargetArea == AbilityTargetAreaType.ConeRight || ability.Definition.AbilityTargetArea == AbilityTargetAreaType.ConeLeft)
			{
				List<GridCoordinate> list2 = combatModel.GetAbilityTargetsInRange(ability, source, sourceCell, preComputedRange);
				FixedPoint value = ability.Definition.AbilityTargetAreaAngle;
				if (!ability.IsConsumableAbility)
				{
					VisitParameter("AbilityModifierIncreaseConeAngle", ref value, source);
					VisitParameter("AbilityModifierThreatArcUpgrade", ref value, source);
				}
				if (value > 0L)
				{
					FixedPoint angleOffset = ((ability.Definition.AbilityTargetArea == AbilityTargetAreaType.ConeRight) ? (value * 0.5) : (value * -0.5));
					list2 = FilterCoordinatesByAngle(list2, sourceCell, targetCell, value, angleOffset);
				}
				list2.Remove(targetCell);
				list2.Insert(0, targetCell);
				int num = list2.Count;
				if (num > ability.Definition.MaxAffectedTargetsCount)
				{
					num = ability.Definition.MaxAffectedTargetsCount;
				}
				for (int m = 0; m < num; m++)
				{
					ActorModel occupier = combatModel.GetOccupier(list2[m]);
					if (occupier != null && (occupier.IsEnemy(source) || (hasFriendlyFire && !occupier.IsStruggling)) && IsActorVisibleForValidation(occupier, sourceCell, targetCell, requiresLineOfSight, requiresLineOfMovement, combatModel))
					{
						return true;
					}
				}
			}
			else if (ability.Definition.MaxAffectedTargetsCount > 1)
			{
				List<GridCoordinate> list3 = combatModel.GetAbilityTargetsInRange(ability, source, sourceCell, preComputedRange);
				FixedPoint value2 = ability.Definition.AbilityTargetAreaAngle;
				if (!ability.IsConsumableAbility)
				{
					VisitParameter("AbilityModifierIncreaseConeAngle", ref value2, source);
					VisitParameter("AbilityModifierThreatArcUpgrade", ref value2, source);
				}
				if (value2 > 0L && value2 < 360L)
				{
					list3 = FilterCoordinatesByAngle(list3, sourceCell, targetCell, value2, 0L);
				}
				list3.Remove(targetCell);
				list3.Insert(0, targetCell);
				int num2 = list3.Count;
				if (num2 > ability.Definition.MaxAffectedTargetsCount)
				{
					num2 = ability.Definition.MaxAffectedTargetsCount;
				}
				for (int n = 0; n < num2; n++)
				{
					ActorModel occupier2 = combatModel.GetOccupier(list3[n]);
					if (occupier2 != null && (occupier2.IsEnemy(source) || (hasFriendlyFire && !occupier2.IsStruggling)) && (!(ability.Definition.RequiresLineOfSight && ability.Definition.EffectSource == EffectSource.SourceActor && canBeBlocked) || combatModel.IsGridCellPenetrable(sourceCell, list3[n], list3[n])) && IsActorVisibleForValidation(occupier2, sourceCell, targetCell, requiresLineOfSight, requiresLineOfMovement, combatModel))
					{
						return true;
					}
				}
			}
			else
			{
				ActorModel occupier3 = combatModel.GetOccupier(targetCell);
				if (occupier3 != null && occupier3.IsEnemy(source) && IsActorVisibleForValidation(occupier3, sourceCell, targetCell, requiresLineOfSight, requiresLineOfMovement, combatModel))
				{
					return true;
				}
			}
			if (source.HasAnyLevelTrait("Equipment_Active_Recoil"))
			{
				List<ActorModel> oneGridEnemyActorModels = GetOneGridEnemyActorModels(source);
				if (oneGridEnemyActorModels != null)
				{
					for (int num3 = 0; num3 < oneGridEnemyActorModels.Count; num3++)
					{
						if (IsActorVisibleForValidation(oneGridEnemyActorModels[num3], sourceCell, targetCell, requiresLineOfSight, requiresLineOfMovement, combatModel))
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		private static bool IsActorVisibleForValidation(ActorModel actor, GridCoordinate sourceCell, GridCoordinate targetCell, bool requiresLineOfSight, bool requiresLineOfMovement, CombatModel combatModel)
		{
			if (actor.IsVisibleToSurvivors && (!requiresLineOfSight || combatModel.IsGridCellVisible(sourceCell, actor.GridCoordinate)))
			{
				if (requiresLineOfMovement)
				{
					return combatModel.IsGridLineMovementBlocked(sourceCell, targetCell);
				}
				return true;
			}
			return false;
		}

		private static void AppendTridentSeparatedLineTargets(List<ActorModel> actorsThatAbilityWillTarget, CombatModel combatModel, AbilityModel ability, ActorModel source, GridCoordinate sourceCell, GridCoordinate aimCell, bool allowFriendlyFire, bool abilityCanBeBlocked)
		{
			AbilityRangeTridentSkill.ResolveSeparatedLineEndCells(combatModel, ability, source, sourceCell, aimCell, out var middleEnd, out var leftEnd, out var rightEnd);
			AppendActorsFromLineSegment(actorsThatAbilityWillTarget, combatModel, ability, source, sourceCell, middleEnd, allowFriendlyFire, abilityCanBeBlocked);
			AppendActorsFromLineSegment(actorsThatAbilityWillTarget, combatModel, ability, source, sourceCell, leftEnd, allowFriendlyFire, abilityCanBeBlocked);
			AppendActorsFromLineSegment(actorsThatAbilityWillTarget, combatModel, ability, source, sourceCell, rightEnd, allowFriendlyFire, abilityCanBeBlocked);
		}

		private static void AppendActorsFromLineSegment(List<ActorModel> actorsThatAbilityWillTarget, CombatModel combatModel, AbilityModel ability, ActorModel source, GridCoordinate sourceCell, GridCoordinate lineEndCell, bool allowFriendlyFire, bool abilityCanBeBlocked)
		{
			List<ActorModel> actorsInLine = combatModel.GetActorsInLine(sourceCell, lineEndCell, source);
			for (int i = 0; i < actorsInLine.Count; i++)
			{
				ActorModel actorModel = actorsInLine[i];
				if (actorModel.IsEnemy(source) || (allowFriendlyFire && !actorModel.IsStruggling))
				{
					bool flag = false;
					if (ability.Definition.RequiresLineOfSight && ability.Definition.EffectSource == EffectSource.SourceActor && (!combatModel.IsGridCellVisible(sourceCell, actorModel.GridCoordinate) || (abilityCanBeBlocked && !combatModel.IsGridCellPenetrable(sourceCell, lineEndCell, actorModel.GridCoordinate))))
					{
						flag = true;
					}
					else if (ability.Definition.RequiresLineOfMovement && combatModel.IsGridLineMovementBlocked(sourceCell, actorModel.GridCoordinate))
					{
						flag = true;
					}
					if (!flag && !actorsThatAbilityWillTarget.Contains(actorModel))
					{
						actorsThatAbilityWillTarget.Add(actorModel);
					}
				}
			}
		}

		public void NotifyAbilityPerformed(ActorModel actor)
		{
			this.AbilityPerformed?.Invoke(actor);
		}

		public void NotifyAfterEffectApplied()
		{
			this.AfterEffectApplied?.Invoke();
		}

		private List<GridCoordinate> FilterCoordinatesByAngle(List<GridCoordinate> coordinates, GridCoordinate source, GridCoordinate target, FixedPoint angle, FixedPoint angleOffset)
		{
			FixedPoint fixedPoint = source.X;
			FixedPoint fixedPoint2 = source.Y;
			FixedPoint fixedPoint3 = target.X;
			FixedPoint fixedPoint4 = target.Y;
			FixedVec2 fixedVec = FixedVec2.Normalize(new FixedVec2(fixedPoint3 - fixedPoint, fixedPoint4 - fixedPoint2));
			if (angleOffset != 0L)
			{
				FixedPoint radians = angleOffset * FixedPoint.PI / 180.0;
				FixedPoint fixedPoint5 = FixedPoint.Cos(radians);
				FixedPoint fixedPoint6 = FixedPoint.Sin(radians);
				FixedPoint x = fixedVec.X * fixedPoint5 - fixedVec.Y * fixedPoint6;
				FixedPoint y = fixedVec.X * fixedPoint6 + fixedVec.Y * fixedPoint5;
				fixedVec.X = x;
				fixedVec.Y = y;
			}
			FixedPoint fixedPoint7 = FixedPoint.Cos(angle * 0.5 * FixedPoint.PI / 180.0);
			FixedPoint fixedPoint8 = fixedPoint7 * fixedPoint7;
			List<GridCoordinate> list = new List<GridCoordinate>();
			for (int i = 0; i < coordinates.Count; i++)
			{
				GridCoordinate item = coordinates[i];
				FixedPoint fixedPoint9 = item.X - fixedPoint;
				FixedPoint fixedPoint10 = item.Y - fixedPoint2;
				FixedPoint fixedPoint11 = fixedPoint9 * fixedVec.X + fixedPoint10 * fixedVec.Y;
				if (fixedPoint7 >= 0.0)
				{
					if (fixedPoint11 >= 0.0)
					{
						FixedPoint fixedPoint12 = fixedPoint9 * fixedPoint9 + fixedPoint10 * fixedPoint10;
						if (fixedPoint12 > 0.0 && fixedPoint11 * fixedPoint11 >= fixedPoint8 * fixedPoint12)
						{
							list.Add(item);
						}
					}
				}
				else
				{
					FixedPoint fixedPoint13 = fixedPoint9 * fixedPoint9 + fixedPoint10 * fixedPoint10;
					if (fixedPoint13 > 0.0 && (fixedPoint11 >= 0.0 || fixedPoint11 * fixedPoint11 <= fixedPoint8 * fixedPoint13))
					{
						list.Add(item);
					}
				}
			}
			if (list.Count > 1)
			{
				Dictionary<GridCoordinate, FixedPoint> distCache = new Dictionary<GridCoordinate, FixedPoint>(list.Count);
				for (int j = 0; j < list.Count; j++)
				{
					GridCoordinate key = list[j];
					if (!distCache.ContainsKey(key))
					{
						FixedPoint fixedPoint14 = key.X - fixedPoint3;
						FixedPoint fixedPoint15 = key.Y - fixedPoint4;
						distCache[key] = fixedPoint14 * fixedPoint14 + fixedPoint15 * fixedPoint15;
					}
				}
				list.StableSort((GridCoordinate coordinate1, GridCoordinate coordinate2) => (distCache[coordinate1] >= distCache[coordinate2]) ? 1 : (-1));
			}
			return list;
		}

		public void ApplyGuildBattleBuffs(string traitId)
		{
			TraitDefinition traitDefinition = base.manager.GameEconomyData.GetTraitDefinition(traitId);
			if (traitDefinition != null && traitDefinition.HasTag("GuildBattleBuff"))
			{
				AbilityManagerModel abilityManager = base.manager.Player.AbilityManager;
				List<ModelModifier> list = new ActorTraitContainerModel().CreateTraitModifiers(traitDefinition, new FixedPoint(1.0), null);
				for (int i = 0; i < list.Count; i++)
				{
					ModelModifier modifier = list[i];
					abilityManager.RegisterGuildBattleBuffs(modifier);
				}
			}
		}

		public void CheckAndAddFeaturedHeroTraits()
		{
			FeaturedHeroDefinition activeFeaturedHero = base.manager.GameEconomyData.GetActiveFeaturedHero(base.manager.Player.UtcTimeStamp);
			if (activeFeaturedHero != null)
			{
				_ = base.manager.Player.AbilityManager;
				TraitDefinition traitDefinition = base.manager.GameEconomyData.GetTraitDefinition("FeaturedHeroBuff.Damage");
				if (traitDefinition != null)
				{
					activeFeaturedHero.UpdateTraitDefinitionWithValues(traitDefinition);
					ApplyFeaturedHeroBuff(traitDefinition);
				}
				traitDefinition = base.manager.GameEconomyData.GetTraitDefinition("FeaturedHeroBuff.Health");
				if (traitDefinition != null)
				{
					activeFeaturedHero.UpdateTraitDefinitionWithValues(traitDefinition);
					ApplyFeaturedHeroBuff(traitDefinition);
				}
				traitDefinition = base.manager.GameEconomyData.GetTraitDefinition("FeaturedHeroBuff.Rarity");
				if (traitDefinition != null)
				{
					activeFeaturedHero.UpdateTraitDefinitionWithValues(traitDefinition);
					ApplyFeaturedHeroBuff(traitDefinition);
				}
			}
		}

		public void ApplyFeaturedHeroBuff(TraitDefinition featuredHeroTrait)
		{
			List<ModelModifier> list = new ActorTraitContainerModel().CreateTraitModifiers(featuredHeroTrait, new FixedPoint(1.0), null);
			for (int i = 0; i < list.Count; i++)
			{
				ModelModifier modifier = list[i];
				RegisterFeaturedHeroBuffs(modifier);
			}
		}

		public void StoreAbilityAction(AbilityAction action)
		{
			if (pendingAbilityActions == null)
			{
				pendingAbilityActions = new List<AbilityAction>();
			}
			pendingAbilityActions.Add(action);
		}

		public AbilityAction GetPendingActionOfType<T>(ActorModel actor)
		{
			if (pendingAbilityActions == null)
			{
				return null;
			}
			return pendingAbilityActions.FirstOrDefault((AbilityAction x) => x is T && x.Actor == actor);
		}

		public void RemoveStoredAbilityActionsOfType<T>(ActorModel actor)
		{
			List<AbilityAction> list = new List<AbilityAction>();
			for (int i = 0; i < pendingAbilityActions.Count; i++)
			{
				if (pendingAbilityActions[i].Actor == actor && pendingAbilityActions[i] is T)
				{
					list.Add(pendingAbilityActions[i]);
				}
			}
			for (int j = 0; j < list.Count; j++)
			{
				pendingAbilityActions.Remove(list[j]);
			}
		}

		public List<ActorModel> GetOneGridEnemyActorModels(ActorModel source)
		{
			List<ActorModel> list = new List<ActorModel>();
			List<ActorModel> actorsInRange = base.manager.CombatModel.GetActorsInRange(source.GridCoordinate, 1);
			for (int i = 0; i < actorsInRange.Count; i++)
			{
				ActorModel actorModel = actorsInRange[i];
				if (actorModel.IsEnemy(source) && actorModel.IsWalker)
				{
					list.Add(actorModel);
				}
			}
			return list;
		}
	}
}
