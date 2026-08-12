using System.Collections.Generic;

namespace TWDModel
{
	public class BaseOverloadTrait : ActionModifier
	{
		private bool isChargeAttack;

		private ActorModel attackMainTarget;

		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			if (!actor.IsMeleeClass)
			{
				return ActionListClearFlag.Keep;
			}
			if (action is AbilityAction abilityAction)
			{
				if (abilityAction.Actor == actor && !abilityAction.Ability.IsConsumableAbility)
				{
					attackMainTarget = base.manager.CombatModel.Occupiers[abilityAction.TargetCell];
					isChargeAttack = abilityAction.Ability.IsChargeAttack;
				}
				CombatModel combatModel = actor.manager.CombatModel;
				ActorModel actorModel = combatModel.Occupiers[abilityAction.TargetCell];
				EquipmentItemModel weaponEquipment = actor.GetWeaponEquipment();
				if (abilityAction.Actor != actor && !abilityAction.Ability.IsConsumableAbility && actorModel != null && !actorModel.IsDead && !weaponEquipment.NeedsReloading && !actor.IsInvisible)
				{
					FixedPoint fixedPoint = actor.GetCitadel_PursuitDown_ParameterMultiplier();
					if (fixedPoint <= ActorTraitContainerModel.Citadel_PercentBase)
					{
						fixedPoint = ActorTraitContainerModel.Citadel_PercentBase;
					}
					if (base.manager.Player.RollDice(RollDiceType.Citadel, fixedPoint) == PlayerRandomChanceResult.Failed)
					{
						return ActionListClearFlag.Keep;
					}
					if (weaponEquipment.Ability.CanAbilityBePerformedOnGridCell(combatModel, actor, actor.GridCoordinate, actorModel.GridCoordinate) == AbilityResult.Success && actor.ChargeMeter.LastChargeConsume == actor.ChargeMeter.MaxLevel && actor.OverloadStatusLeftTurns > 0 && actor.OverloadStatusEXAttackTimesInTurn < actor.Overload_FullChargeEXTurnLimitNum())
					{
						actor.OverloadStatusEXAttackTimesInTurn++;
						FixedPoint fixedPoint2 = 0.0;
						fixedPoint2 = actor.Overload_FullChargeEXDmgPer();
						addedActions.Add(new OverloadAction(actor, weaponEquipment.Ability, actorModel.GridCoordinate, actorModel, fixedPoint2, isTriggerExtraAttackDamage: true));
					}
				}
			}
			if (action is PostDamageAction postDamageAction && postDamageAction.DamagerActor == actor && attackMainTarget != null && !attackMainTarget.IsEnvironmental && isChargeAttack)
			{
				actor.OverloadStatusLeftTurns = actor.Overload_ContinueTurnNum();
				actor.NotifyChange("OverLoadEvent");
				actor.NotifyChange("AbilityVisited", new object[2] { "Overload", false });
			}
			return ActionListClearFlag.Keep;
		}
	}
}
