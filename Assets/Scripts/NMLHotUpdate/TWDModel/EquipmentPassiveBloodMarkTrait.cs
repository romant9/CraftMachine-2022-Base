using System;
using System.Collections.Generic;

namespace TWDModel
{
	public class EquipmentPassiveBloodMarkTrait : ActionModifier
	{
		private int moveDistanceCap;

		private int damageCount;

		private FixedPoint healthPercentageNonBoss;

		private FixedPoint healthPercentageBoss;

		private FixedPoint chance;

		private FixedPoint damagePercentage;

		private int range;

		private FixedPoint damageLimit;

		public int MoveDistanceCap => moveDistanceCap;

		public int DamageCount => damageCount;

		public FixedPoint Chance => chance;

		public FixedPoint DamagePercentage => damagePercentage;

		public int Range => range;

		public FixedPoint DamageLimit => damageLimit;

		public EquipmentPassiveBloodMarkTrait()
		{
		}

		public EquipmentPassiveBloodMarkTrait(int moveDistanceCap, int damageCount, FixedPoint healthPercentageNonBoss, FixedPoint healthPercentageBoss, FixedPoint chance, FixedPoint damagePercentage, int range, FixedPoint damageLimit)
		{
			this.moveDistanceCap = moveDistanceCap;
			this.damageCount = damageCount;
			this.healthPercentageNonBoss = healthPercentageNonBoss;
			this.healthPercentageBoss = healthPercentageBoss;
			this.chance = chance;
			this.damagePercentage = damagePercentage;
			this.range = range;
			this.damageLimit = damageLimit;
		}

		public FixedPoint GetHealthPercentageForTarget(ActorModel target)
		{
			if (target != null && target.Definition != null && target.Definition.Class == "boss")
			{
				return healthPercentageBoss;
			}
			return healthPercentageNonBoss;
		}

		public static EquipmentPassiveBloodMarkTrait FindOnActor(ActorModel actor)
		{
			if (actor == null)
			{
				return null;
			}
			EquipmentPassiveBloodMarkTrait equipmentPassiveBloodMarkTrait = FindInModifierCollection(actor.Modifiers);
			if (equipmentPassiveBloodMarkTrait != null)
			{
				return equipmentPassiveBloodMarkTrait;
			}
			if (actor.Abilities != null)
			{
				for (int i = 0; i < actor.Abilities.Count; i++)
				{
					AbilityModel abilityModel = actor.Abilities[i];
					if (abilityModel != null && abilityModel.Modifiers != null)
					{
						equipmentPassiveBloodMarkTrait = FindInModifierCollection(abilityModel.Modifiers);
						if (equipmentPassiveBloodMarkTrait != null)
						{
							return equipmentPassiveBloodMarkTrait;
						}
					}
				}
			}
			EquipmentItemModel weaponEquipment = actor.GetWeaponEquipment();
			if (weaponEquipment != null && weaponEquipment.Ability != null && weaponEquipment.Ability.Modifiers != null)
			{
				equipmentPassiveBloodMarkTrait = FindInModifierCollection(weaponEquipment.Ability.Modifiers);
				if (equipmentPassiveBloodMarkTrait != null)
				{
					return equipmentPassiveBloodMarkTrait;
				}
			}
			EquipmentItemModel selectedEquipment = actor.SelectedEquipment;
			if (selectedEquipment != null && selectedEquipment != weaponEquipment && selectedEquipment.Ability != null && selectedEquipment.Ability.Modifiers != null)
			{
				equipmentPassiveBloodMarkTrait = FindInModifierCollection(selectedEquipment.Ability.Modifiers);
			}
			return equipmentPassiveBloodMarkTrait;
		}

		private static EquipmentPassiveBloodMarkTrait FindInModifierCollection(IModifierCollection collection)
		{
			if (collection == null)
			{
				return null;
			}
			int count = collection.GetCount();
			for (int i = 0; i < count; i++)
			{
				if (collection.GetModifier(i) is EquipmentPassiveBloodMarkTrait result)
				{
					return result;
				}
			}
			return null;
		}

		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			if (action == null || actor == null)
			{
				return ActionListClearFlag.Keep;
			}
			if (action is DamageActionExecuteFinishedAction damageActionExecuteFinishedAction)
			{
				ActorModel actorModel = ((damageActionExecuteFinishedAction.DamageAction != null) ? damageActionExecuteFinishedAction.DamageAction.TargetActor : damageActionExecuteFinishedAction.TargetActor);
				if (actorModel?.BloodMarkTimedEffect != null)
				{
					TryAccumulateOnMarkedTarget(actor, actorModel, damageActionExecuteFinishedAction.DamageAction, addedActions);
				}
			}
			if (action is PreChangeTurnAction preChangeTurnAction && preChangeTurnAction.CurrentActiveFaction == actor.Faction)
			{
				ForEachHandledMarkedActor(actor, delegate(ActorModel marked)
				{
					TrySettle(marked, addedActions);
				});
			}
			return ActionListClearFlag.Keep;
		}

		private void TryAccumulateOnMarkedTarget(ActorModel handler, ActorModel marked, DamageAction damageAction, List<ModelAction> addedActions)
		{
			BloodMarkTimedEffect bloodMarkTimedEffect = marked?.BloodMarkTimedEffect;
			if (bloodMarkTimedEffect == null || !IsMarkHandler(handler, marked, bloodMarkTimedEffect) || damageAction == null || damageAction.DamageType == DamageType.BloodMarkSettlement || damageAction.DamageType == DamageType.Heal || damageAction.DodgedShot)
			{
				return;
			}
			int num = damageAction.BaseDamage + damageAction.AdditionalCriticalDamage;
			if (num > 0)
			{
				bloodMarkTimedEffect.AccumulatedDamage += num;
				if (marked.IsDead)
				{
					TrySettle(marked, addedActions);
				}
			}
		}

		private static void TrySettle(ActorModel marked, List<ModelAction> addedActions)
		{
			BloodMarkTimedEffect bloodMarkTimedEffect = marked?.BloodMarkTimedEffect;
			if (bloodMarkTimedEffect == null)
			{
				return;
			}
			if (bloodMarkTimedEffect.AccumulatedDamage <= 0 || bloodMarkTimedEffect.DamageCount <= 0)
			{
				bloodMarkTimedEffect.AccumulatedDamage = 0;
				return;
			}
			int accumulatedDamage = bloodMarkTimedEffect.AccumulatedDamage;
			bloodMarkTimedEffect.AccumulatedDamage = 0;
			bloodMarkTimedEffect.SkipAccumulateOnce = false;
			FixedPoint fixedPoint = FixedPoint.Max(0L, bloodMarkTimedEffect.HealthPercentage);
			int num = (int)(marked.MaxHitPoints * fixedPoint);
			if (num <= 0)
			{
				bloodMarkTimedEffect.LastSettledDamage = 0;
				return;
			}
			int num2 = (bloodMarkTimedEffect.LastSettledDamage = Math.Min(accumulatedDamage * bloodMarkTimedEffect.DamageCount, num));
			if (num2 > 0)
			{
				ActorModel actorModel = bloodMarkTimedEffect.Instigator;
				if (actorModel == null || actorModel.IsDead || actorModel.Faction != bloodMarkTimedEffect.MarkFaction)
				{
					actorModel = ResolveMarkSource(marked, bloodMarkTimedEffect.MarkFaction);
				}
				if (!marked.IsDead)
				{
					EnqueueSettleDamage(marked, accumulatedDamage, num2, bloodMarkTimedEffect.DamageCount, addedActions, actorModel);
				}
				TrySplash(marked, bloodMarkTimedEffect, num2, addedActions, actorModel);
			}
		}

		private static void EnqueueSettleDamage(ActorModel marked, int accumulated, int totalSettleDamage, int damageCount, List<ModelAction> addedActions, ActorModel markSource)
		{
			int num = totalSettleDamage;
			int num2 = Math.Max(1, damageCount);
			for (int i = 0; i < num2; i++)
			{
				if (num <= 0)
				{
					break;
				}
				int num3 = Math.Min(accumulated, num);
				if (i == num2 - 1)
				{
					num3 = num;
				}
				if (num3 > 0)
				{
					num -= num3;
					if (addedActions != null)
					{
						addedActions.Add(new EquipmentBloodMarkSettleAction(marked, num3, markSource));
					}
					else if (marked.manager?.CombatModel != null)
					{
						CombatHelpers.ExecuteDamage(marked.manager.CombatModel, markSource, marked, num3, 0, DamageType.BloodMarkSettlement, PlayerRandomChanceResult.Failed, PlayerRandomChanceResult.Failed, null, dealDamagePostAbility: false, null, noChargeGain: true, null, isMainTarget: false, isTriggerExtraAttackDamage: false, OOTType.None, isChargeAttack: false, isEquipmentKaboomReflect: false, applyIncomingDamageMitigation: true);
					}
				}
			}
		}

		private static void TrySplash(ActorModel markedActor, BloodMarkTimedEffect effect, int settleDamage, List<ModelAction> addedActions, ActorModel markSource)
		{
			if (settleDamage <= 0 || effect.Range <= 0 || effect.Chance <= 0L || effect.DamagePercentage <= 0L)
			{
				return;
			}
			PlayerModel playerModel = markedActor.manager?.Player;
			CombatModel combatModel = markedActor.manager?.CombatModel;
			if (playerModel == null || combatModel == null || combatModel.MissionCompleted || playerModel.RollDice(RollDiceType.EquipmentBloodMarkSplash, effect.Chance, 0L) == PlayerRandomChanceResult.Failed)
			{
				return;
			}
			Faction markFaction = effect.MarkFaction;
			if (markFaction == Faction.Any)
			{
				return;
			}
			int num = (int)(settleDamage * effect.DamagePercentage);
			int num2 = (int)(markedActor.MaxHitPoints * FixedPoint.Max(0L, effect.DamageLimit));
			if (num2 > 0)
			{
				num = Math.Min(num, num2);
			}
			if (num <= 0)
			{
				return;
			}
			List<ActorModel> splashTargets = GetSplashTargets(markedActor, combatModel, markFaction, effect.Range);
			if (splashTargets.Count == 0)
			{
				return;
			}
			if (markSource == null)
			{
				markSource = ResolveMarkSource(markedActor, markFaction);
			}
			foreach (ActorModel item in splashTargets)
			{
				if (addedActions != null)
				{
					addedActions.Add(new EquipmentBloodMarkSplashAction(item, num, markSource));
				}
				else
				{
					CombatHelpers.ExecuteDamage(combatModel, markSource, item, num, 0, DamageType.BloodMarkSplash, PlayerRandomChanceResult.Failed, PlayerRandomChanceResult.Failed, null, dealDamagePostAbility: false, null, noChargeGain: true, null, isMainTarget: false, isTriggerExtraAttackDamage: false, OOTType.None, isChargeAttack: false, isEquipmentKaboomReflect: false, applyIncomingDamageMitigation: true);
				}
			}
		}

		private static List<ActorModel> GetSplashTargets(ActorModel markedActor, CombatModel combatModel, Faction markFaction, int range)
		{
			List<ActorModel> list = new List<ActorModel>();
			List<ActorModel> actorsInRange = combatModel.GetActorsInRange(markedActor.GridCoordinate, range);
			if (actorsInRange == null)
			{
				return list;
			}
			List<ActorModel> enemyFactionsActors = combatModel.GetEnemyFactionsActors(markFaction);
			for (int i = 0; i < actorsInRange.Count; i++)
			{
				ActorModel actorModel = actorsInRange[i];
				if (actorModel != null && actorModel != markedActor && !actorModel.IsDead && !actorModel.IsEnvironmental && actorModel.Faction != Faction.Environmental && enemyFactionsActors != null && enemyFactionsActors.Contains(actorModel))
				{
					list.Add(actorModel);
				}
			}
			return list;
		}

		private static bool IsMarkHandler(ActorModel handler, ActorModel marked, BloodMarkTimedEffect effect)
		{
			if (handler == null || marked == null || effect == null)
			{
				return false;
			}
			if (effect.Instigator != null && !effect.Instigator.IsDead && effect.Instigator.Faction == effect.MarkFaction)
			{
				return handler == effect.Instigator;
			}
			return handler == ResolveMarkSource(marked, effect.MarkFaction);
		}

		private static ActorModel ResolveMarkSource(ActorModel marked, Faction markFaction)
		{
			CombatModel combatModel = marked?.manager?.CombatModel;
			if (combatModel == null || markFaction == Faction.Any)
			{
				return null;
			}
			List<ActorModel> factionActors = combatModel.GetFactionActors(markFaction);
			if (factionActors == null)
			{
				return null;
			}
			ActorModel actorModel = null;
			for (int i = 0; i < factionActors.Count; i++)
			{
				ActorModel actorModel2 = factionActors[i];
				if (actorModel2 != null && actorModel2 != marked && !actorModel2.IsDead)
				{
					if (actorModel2.HasAnyLevelTrait("Equipment.Passive.BloodMark") || actorModel2.HasTraitsThatContains("Equipment.Passive.BloodMark"))
					{
						return actorModel2;
					}
					if (actorModel == null)
					{
						actorModel = actorModel2;
					}
				}
			}
			return actorModel;
		}

		private static void ForEachHandledMarkedActor(ActorModel handler, Action<ActorModel> body)
		{
			CombatModel combatModel = handler?.manager?.CombatModel;
			if (combatModel == null || body == null)
			{
				return;
			}
			List<ActorModel> allActors = combatModel.GetAllActors();
			if (allActors == null)
			{
				return;
			}
			for (int i = 0; i < allActors.Count; i++)
			{
				ActorModel actorModel = allActors[i];
				BloodMarkTimedEffect bloodMarkTimedEffect = actorModel?.BloodMarkTimedEffect;
				if (bloodMarkTimedEffect != null && IsMarkHandler(handler, actorModel, bloodMarkTimedEffect))
				{
					body(actorModel);
				}
			}
		}
	}
}
