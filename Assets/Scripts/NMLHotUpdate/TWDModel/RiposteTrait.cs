using System.Collections.Generic;

namespace TWDModel
{
	public class RiposteTrait : ActionModifier
	{
		private bool isChargeAttack;

		public FixedPoint IncreaseStorey;

		public int ProtectCheckered;

		public int ProtectSpan;

		public int MomentumStorey;

		public FixedPoint IncreaseDmg;

		public FixedPoint MaxStorey;

		public FixedPoint TempMultiplier;

		public FixedPoint AddDamagePercentageBase;

		public FixedPoint ReduceEnemyDodgePercentageBase;

		public FixedPoint ReduceEnemyDamageReductionBase;

		public int MaxLayer;

		public RiposteTrait()
		{
		}

		public RiposteTrait(FixedPoint increaseStorey, int protectCheckered, int protectSpan, int momentumStorey, FixedPoint increaseDmg, FixedPoint maxStorey, FixedPoint addDamagePercentageBase, FixedPoint reduceEnemyDodgePercentageBase, FixedPoint reduceEnemyDamageReductionBase, int maxLayer, FixedPoint tempMultiplier)
		{
			IncreaseStorey = increaseStorey;
			ProtectCheckered = protectCheckered;
			ProtectSpan = protectSpan;
			MomentumStorey = momentumStorey;
			IncreaseDmg = increaseDmg;
			MaxStorey = maxStorey;
			TempMultiplier = tempMultiplier;
			AddDamagePercentageBase = addDamagePercentageBase;
			ReduceEnemyDodgePercentageBase = reduceEnemyDodgePercentageBase;
			ReduceEnemyDamageReductionBase = reduceEnemyDamageReductionBase;
			MaxLayer = maxLayer;
		}

		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			if (!(action is StunAction stunAction))
			{
				if (action is ElectricShockAction electricShockAction && actor == electricShockAction.TargetActor && actor.ParryRiposteIncreaseStorey > 0)
				{
					electricShockAction.Avoided = true;
				}
			}
			else if (actor == stunAction.TargetActor && stunAction.CanNotAvoidStunType == CanNotAvoidStunType.None && actor.ParryRiposteIncreaseStorey > 0)
			{
				stunAction.Avoided = true;
			}
			if (action is AbilityAction abilityAction && actor != null && abilityAction.Actor == actor)
			{
				if (abilityAction.Ability.IsChargeAttack && !abilityAction.Ability.IsConsumableAbility)
				{
					isChargeAttack = abilityAction.Ability.IsChargeAttack;
				}
				if (abilityAction.OOTType == OOTType.Retaliation)
				{
					MomentumAction item = new MomentumAction(actor, MomentumStorey, AddDamagePercentageBase, ReduceEnemyDodgePercentageBase, ReduceEnemyDamageReductionBase, MaxLayer);
					addedActions.Add(item);
				}
			}
			if (action is AbilityBeforeRemoveActiveTraitAction abilityBeforeRemoveActiveTraitAction)
			{
				CombatModel combatModel = actor.manager.CombatModel;
				if (combatModel != null && !combatModel.MissionCompleted && abilityBeforeRemoveActiveTraitAction.AbilityAction.Ability.IsChargeAttack && abilityBeforeRemoveActiveTraitAction.Source == actor)
				{
					actor.UpdateParryRiposteIncreaseStorey((int)IncreaseStorey);
					actor.NotifyChange("UpParryRiposteFloor");
					addTaunt(actor, addedActions, combatModel);
				}
			}
			if (action is PreChangeTurnAction preChangeTurnAction && actor != null && preChangeTurnAction.CurrentActiveFaction == actor.Faction)
			{
				CombatModel combatModel2 = actor.manager.CombatModel;
				if (combatModel2 != null && !combatModel2.MissionCompleted)
				{
					actor.UpdateParryRiposteIncreaseStorey((int)IncreaseStorey);
					actor.NotifyChange("UpParryRiposteFloor");
					addTaunt(actor, addedActions, combatModel2);
				}
			}
			if (action is PreAttackAction { EffectiveAttack: not false } preAttackAction && !actor.NotTriggeredRiposted && (!actor.CanPerformOOT || !actor.PreAttackedOnRiposte))
			{
				CombatModel combatModel3 = actor.manager.CombatModel;
				if (actor.SelectedAbility.CanAbilityBePerformedOnGridCell(base.manager.CombatModel, actor, actor.GridCoordinate, preAttackAction.DamagerActor.GridCoordinate) != AbilityResult.Success)
				{
					return ActionListClearFlag.Keep;
				}
				if (preAttackAction.TargetActor == actor && combatModel3 != null && !combatModel3.MissionCompleted && combatModel3.TurnManager.ActiveFaction != actor.Faction && actor.ParryRiposteIncreaseStorey > 0)
				{
					EquipmentItemModel weaponEquipment = actor.GetWeaponEquipment();
					if (weaponEquipment != null && preAttackAction.DamagerActor != null)
					{
						weaponEquipment.AddTemporaryTrait("RetaliateMultiplier", TraitExpirationType.Activation, TempMultiplier * 100L);
						AbilityAction action2 = new AbilityAction(actor, weaponEquipment.Ability, preAttackAction.DamagerActor.GridCoordinate, preAttackAction.DamagerActor, OOTType.ParryRiposteRetaliation, skipActiveWeaponTraits: false, isAssistAttack: false, isTriggerExtraAttackDamage: true);
						if (base.manager.ExecuteAction(action2))
						{
							MomentumAction item2 = new MomentumAction(preAttackAction.TargetActor, MomentumStorey, AddDamagePercentageBase, ReduceEnemyDodgePercentageBase, ReduceEnemyDamageReductionBase, MaxLayer);
							addedActions.Add(item2);
							actor.UpdateParryRiposteIncreaseStorey(-1);
							actor.NotifyChange("UpParryRiposteFloor");
							actor.NotifyChange("AbilityVisited", new object[2] { "Riposte", false });
							actor.PreAttackedOnRiposte = true;
							if (actor.SelectedAbility.PushEffect != null && actor.SelectedAbility.PushEffect.FindFurthestPushCoordinateByCoordinates(combatModel3, actor.GridCoordinate, preAttackAction.DamagerActor.GridCoordinate).ChebyshevDistance(actor.GridCoordinate) - 1 > 1)
							{
								preAttackAction.DamagerActor.EndAction();
							}
						}
					}
				}
			}
			if (action is PostAbilityExecuteAction postAbilityExecuteAction && actor.PreAttackedOnRiposte)
			{
				CombatModel combatModel4 = actor.manager.CombatModel;
				if (postAbilityExecuteAction.TargetActor == actor && combatModel4 != null && !combatModel4.MissionCompleted && combatModel4.TurnManager.ActiveFaction != actor.Faction)
				{
					actor.PreAttackedOnRiposte = false;
				}
			}
			return ActionListClearFlag.Keep;
		}

		private void addTaunt(ActorModel actor, List<ModelAction> addedActions, CombatModel combatModel)
		{
			actor.GridCoordinate.GetEnemiesByDistanceAndFaction(actor.GridCoordinate, combatModel, ProtectCheckered, actor.Faction).ForEach(delegate(ActorModel x)
			{
				if (x != null && !x.IsDead && actor.GridCoordinate.DistanceTo(x.GridCoordinate) <= ProtectCheckered)
				{
					addedActions.Add(new TauntAction(actor, x, ProtectSpan));
				}
			});
		}
	}
}
