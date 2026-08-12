using System.Collections.Generic;

namespace TWDModel
{
	public class RiotShieldHerdTrait : ActionModifier
	{
		private int gridDistance;

		public RiotShieldHerdTrait(int distance)
		{
			gridDistance = distance;
		}

		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			if (action is AbilityAction abilityAction && abilityAction.Actor == actor && abilityAction.Ability.IsChargeAttack)
			{
				CombatModel combatModel = actor.manager.CombatModel;
				foreach (ActorModel target in actor.GridCoordinate.GetEnemiesByDistance(abilityAction.TargetCell, combatModel, gridDistance))
				{
					if (!actor.IsDead && !actor.IsEnvironmental && actor.ExclusiveTimedEffect == null && !actor.IsImmuneToStun && !target.IsPitfalled)
					{
						int hpPriorToAttack = target.Hitpoints;
						addedActions.Add(new HerdAction(actor, target, 1, 0, null, () => hpPriorToAttack - target.Hitpoints));
					}
				}
			}
			return ActionListClearFlag.Keep;
		}
	}
}
