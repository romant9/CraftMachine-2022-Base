using System.Collections.Generic;

namespace TWDModel
{
	public class ChargeAttackWithFreeShootingTrait : ActionModifier
	{
		private readonly FixedPoint damageMultiplier;

		private readonly int maxTriggersPerTurn;

		private bool lastAbilityWasChargeAttack;

		private ActorModel lastChargeAttackActor;

		public ChargeAttackWithFreeShootingTrait(FixedPoint damageMultiplier, int maxTriggers)
		{
			this.damageMultiplier = damageMultiplier;
			maxTriggersPerTurn = maxTriggers;
		}

		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			CombatModel combatModel = actor?.manager?.CombatModel;
			if (combatModel == null)
			{
				return ActionListClearFlag.Keep;
			}
			if (action is ChangeTurnAction && combatModel.TurnManager.ActiveFaction == actor.Faction)
			{
				actor.ChargeAttackWithFreeShootingTriggeredCount = 0;
			}
			if (action is AbilityAction abilityAction && abilityAction.Actor.Faction == actor.Faction && abilityAction.Ability.IsChargeAttack && !abilityAction.Ability.IsConsumableAbility)
			{
				lastAbilityWasChargeAttack = true;
				lastChargeAttackActor = abilityAction.Actor;
			}
			if (action is PostAbilityExecuteAction postAbilityExecuteAction && postAbilityExecuteAction.DamagerActor.Faction == actor.Faction)
			{
				if (lastAbilityWasChargeAttack && lastChargeAttackActor == postAbilityExecuteAction.DamagerActor && actor.ChargeAttackWithFreeShootingTriggeredCount < maxTriggersPerTurn && combatModel.TurnManager.ActiveFaction == actor.Faction)
				{
					TryTriggerFreeShooting(actor, combatModel, addedActions);
				}
				if (lastChargeAttackActor == postAbilityExecuteAction.DamagerActor)
				{
					lastAbilityWasChargeAttack = false;
					lastChargeAttackActor = null;
				}
			}
			return ActionListClearFlag.Keep;
		}

		private void TryTriggerFreeShooting(ActorModel actor, CombatModel combatModel, List<ModelAction> addedActions)
		{
			if (actor.ChargeAttackWithFreeShootingTriggeredCount >= maxTriggersPerTurn)
			{
				return;
			}
			EquipmentItemModel weaponEquipment = actor.GetWeaponEquipment();
			AbilityModel abilityModel = weaponEquipment?.Ability;
			if (weaponEquipment == null || abilityModel == null)
			{
				return;
			}
			ActorModel actorModel = FindClosestAttackableEnemy(actor, combatModel, abilityModel);
			if (actorModel != null)
			{
				FixedPoint fixedPoint = actor.GetCitadel_PursuitDown_ParameterMultiplier();
				if (fixedPoint <= ActorTraitContainerModel.Citadel_PercentBase)
				{
					fixedPoint = ActorTraitContainerModel.Citadel_PercentBase;
				}
				if (base.manager.Player.RollDice(RollDiceType.Citadel, fixedPoint) != PlayerRandomChanceResult.Failed)
				{
					actor.ChargeAttackWithFreeShootingTriggeredCount++;
					ChargeAttackWithFreeShootingAction item = new ChargeAttackWithFreeShootingAction(actor, weaponEquipment.Ability, actorModel.GridCoordinate, actorModel, damageMultiplier);
					addedActions.Add(item);
				}
			}
		}

		private ActorModel FindClosestAttackableEnemy(ActorModel actor, CombatModel combatModel, AbilityModel weaponAbility)
		{
			ActorModel result = null;
			int num = int.MaxValue;
			foreach (ActorModel enemyFactionsActor in combatModel.GetEnemyFactionsActors(actor.Faction))
			{
				if (enemyFactionsActor != null && !enemyFactionsActor.IsDead && !enemyFactionsActor.IsEnvironmental && weaponAbility.CanAbilityBePerformedOnGridCell(combatModel, actor, actor.GridCoordinate, enemyFactionsActor.GridCoordinate) == AbilityResult.Success)
				{
					int num2 = actor.GridCoordinate.ChebyshevDistance(enemyFactionsActor.GridCoordinate);
					if (num2 < num)
					{
						num = num2;
						result = enemyFactionsActor;
					}
				}
			}
			return result;
		}
	}
}
