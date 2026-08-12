using System.Collections.Generic;

namespace TWDModel
{
	public class AssistAttackTrait : ActionModifier
	{
		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			CombatModel combatModel = actor.manager.CombatModel;
			if (action is AbilityAction abilityAction)
			{
				abilityAction.Actor.ActorsLastAbilityCell = abilityAction.TargetCell;
			}
			if (action is PostDamageAction postDamageAction)
			{
				if (postDamageAction.DamagerActor != null && postDamageAction.DamagerActor.dashTraitAttackFlag)
				{
					return ActionListClearFlag.Keep;
				}
				EquipmentItemModel weaponEquipment = actor.GetWeaponEquipment();
				if (postDamageAction.DamagerActor != null && postDamageAction.DamagerActor.Faction == actor.Faction && postDamageAction.DamagerActor != actor && !actor.IsInteractingWithObject && combatModel.CanAssistAttackTargetThisTurn(actor) && !weaponEquipment.NeedsReloading && !actor.IsInvisible)
				{
					EquipmentItemModel weaponEquipment2 = postDamageAction.DamagerActor.GetWeaponEquipment();
					GridCoordinate actorsLastAbilityCell = postDamageAction.DamagerActor.ActorsLastAbilityCell;
					List<ActorModel> listOfActorsToBeTargetted = combatModel.AbilityManager.GetListOfActorsToBeTargetted(weaponEquipment2.Ability, postDamageAction.DamagerActor, postDamageAction.DamagerActor.GridCoordinate, actorsLastAbilityCell);
					if (listOfActorsToBeTargetted.Count == 0)
					{
						return ActionListClearFlag.Keep;
					}
					ActorModel actorModel = CalculateApplyAssistAttackToTarget(actor, weaponEquipment.Ability, listOfActorsToBeTargetted);
					if (actorModel == null)
					{
						return ActionListClearFlag.Keep;
					}
					FixedPoint value = 0.0;
					if (combatModel.AbilityManager.VisitParameter("EquipmentActiveAssistAttackDamagePercent", ref value, actor))
					{
						addedActions.Add(new AssistAttackAction(actor, weaponEquipment.Ability, actorModel.GridCoordinate, actorModel, value, isTriggerExtraAttackDamage: true));
						combatModel.AddAssistAttackRecord(actor);
					}
				}
			}
			return ActionListClearFlag.Keep;
		}

		private ActorModel CalculateApplyAssistAttackToTarget(ActorModel actor, AbilityModel actorAbility, List<ActorModel> targets)
		{
			TWDModelManager tWDModelManager = actor.manager;
			CombatModel combatModel = tWDModelManager.CombatModel;
			List<ActorModel> list = new List<ActorModel>();
			foreach (ActorModel target in targets)
			{
				if (!target.IsDead && actorAbility.CanAbilityBePerformedOnGridCell(combatModel, actor, actor.GridCoordinate, target.GridCoordinate) == AbilityResult.Success)
				{
					list.Add(target);
				}
			}
			if (list.Count == 0)
			{
				return null;
			}
			FixedPoint value = 0.0;
			tWDModelManager.Player.AbilityManager.VisitParameter("EquipmentActiveAssistAttackPercent", ref value, actor);
			FixedPoint citadel_PursuitDown_ParameterMultiplier = actor.GetCitadel_PursuitDown_ParameterMultiplier();
			value *= citadel_PursuitDown_ParameterMultiplier;
			if (value <= ActorTraitContainerModel.Citadel_PercentBase)
			{
				value = ActorTraitContainerModel.Citadel_PercentBase;
			}
			FixedPoint value2 = 0.0;
			if (value != 0.0)
			{
				tWDModelManager.Player.AbilityManager.VisitParameter("ExtendProbability", ref value2, actor);
			}
			if (tWDModelManager.Player.RollDice(RollDiceType.AssistAttack, value, value2) == PlayerRandomChanceResult.Failed)
			{
				return null;
			}
			ActorModel actorModel = CaculateMinDistTargetFromSource(actor, list);
			if (actorModel == null)
			{
				return null;
			}
			return actorModel;
		}

		private ActorModel CaculateMinDistTargetFromSource(ActorModel source, List<ActorModel> targets)
		{
			FixedPoint fixedPoint = FixedPoint.MaxValue;
			ActorModel result = null;
			foreach (ActorModel target in targets)
			{
				if (!target.IsDead)
				{
					FixedPoint fixedPoint2 = source.GridCoordinate.DistanceTo(target.GridCoordinate);
					if (fixedPoint2 < fixedPoint)
					{
						result = target;
						fixedPoint = fixedPoint2;
					}
				}
			}
			return result;
		}
	}
}
