using System.Collections.Generic;

namespace TWDModel
{
	public class FightingFuryTrait : ActionModifier
	{
		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			if (!base.manager.Player.Tutorial.StaticTutorialComplete)
			{
				return ActionListClearFlag.Keep;
			}
			CombatModel combatModel = actor.manager.CombatModel;
			if (action is ChangeTurnAction)
			{
				actor.FightingFuryActivated = false;
				actor.AdditionalAttackCount = 0;
			}
			if (action is AbilityAction abilityAction)
			{
				if (abilityAction.Actor == actor && actor is SurvivorModel { IsMeleeClass: not false })
				{
					_ = abilityAction.OOTType;
					_ = 4;
					return ActionListClearFlag.Keep;
				}
			}
			else if (action is PreAttackAction preAttackAction && preAttackAction.DamagerActor == actor && actor is SurvivorModel { IsMeleeClass: not false })
			{
				if (actor != null && combatModel != null)
				{
					StoreAdjacentTargetCount(combatModel, actor, preAttackAction.TargetActor, addTargetToCount: false);
				}
			}
			else if (action is PostDamageAction postDamageAction && actor == postDamageAction.DamagerActor && actor is SurvivorModel { IsMeleeClass: not false } && actor != null && combatModel != null)
			{
				if (actor.FightingFuryTargetCount <= 0)
				{
					StoreAdjacentTargetCount(combatModel, actor, postDamageAction.TargetActor, addTargetToCount: true);
				}
				if (actor.FightingFuryTargetCount <= 0)
				{
					return ActionListClearFlag.Keep;
				}
				FixedPoint value = 0L;
				if (actor.HasAnyLevelTrait("BaseFightingFury"))
				{
					combatModel.AbilityManager.VisitParameter("LeaderBuffFightingFuryMaxAddAttacksLeader", ref value, actor);
				}
				else if (actor.HasAnyLevelTrait("FightingFury"))
				{
					combatModel.AbilityManager.VisitParameter("LeaderBuffFightingFuryMaxAddAttacks", ref value, actor);
				}
				int num = (int)((value >= actor.FightingFuryTargetCount) ? ((FixedPoint)actor.FightingFuryTargetCount) : value);
				if (!actor.FightingFuryActivated && num > 0 && !actor.AdditionalAttackConsumed)
				{
					num = (actor.GivenAdditionalAttacks = (actor.AdditionalAttackCount = num + 1));
					actor.CanMoveWithoutAttacking = true;
					actor.GetWeaponEquipment().Ability.MaxUsesPerTurn = num;
					actor.FightingFuryActivated = true;
				}
			}
			return ActionListClearFlag.Keep;
		}

		private void StoreAdjacentTargetCount(CombatModel combatModel, ActorModel sourceActor, ActorModel targetActor, bool addTargetToCount)
		{
			int num = 0;
			if (sourceActor == null || combatModel == null)
			{
				return;
			}
			foreach (GridCoordinate item in combatModel.Grid.Neighbors(sourceActor.GridCoordinate))
			{
				ActorModel occupier = combatModel.GetOccupier(item);
				if (occupier != null && sourceActor.IsEnemy(occupier) && !occupier.Definition.IsEnvironmental)
				{
					num++;
				}
				else if (addTargetToCount && occupier == null && targetActor != null && targetActor.GridCoordinate == item)
				{
					num++;
				}
			}
			sourceActor.FightingFuryTargetCount = num;
		}
	}
}
