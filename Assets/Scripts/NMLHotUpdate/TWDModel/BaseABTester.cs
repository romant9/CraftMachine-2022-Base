using System.Collections.Generic;

namespace TWDModel
{
	public class BaseABTester : ActionModifier
	{
		private ActorModel attackMainTarget;

		private bool isChargeAttack;

		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			if (actor.Faction != base.manager.CombatModel.TurnManager.ActiveFaction)
			{
				return ActionListClearFlag.Keep;
			}
			PostDamageAction postDamageAction = action as PostDamageAction;
			if (postDamageAction != null)
			{
				if (postDamageAction.DamagerActor == actor && attackMainTarget != null && !attackMainTarget.IsEnvironmental && actor.IsRangedClass)
				{
					if (!isChargeAttack)
					{
						FixedPoint value = 0.0;
						base.manager.CombatModel.AbilityManager.VisitParameter("LeaderBuffABTesterBMarkChance", ref value, actor);
						if (base.manager.Player.RollDice(RollDiceType.ABTesterTrait1, value) != PlayerRandomChanceResult.Failed)
						{
							postDamageAction.TargetActor.StartABTesterB(actor);
						}
					}
					else
					{
						if (postDamageAction.TargetActor != attackMainTarget)
						{
							return ActionListClearFlag.Keep;
						}
						if (!attackMainTarget.IsWalker && attackMainTarget.Faction != Faction.Raider)
						{
							return ActionListClearFlag.Keep;
						}
						addedActions.Add(new ABTesterAction(postDamageAction.DamagerActor, postDamageAction.TargetActor, 1, ignoreSourceBeingDead: false, null, () => postDamageAction.DamageAction.FinalDamage));
					}
				}
			}
			else if (action is AbilityAction abilityAction && abilityAction.Actor == actor && !abilityAction.Ability.IsConsumableAbility)
			{
				attackMainTarget = base.manager.CombatModel.Occupiers[abilityAction.TargetCell];
				isChargeAttack = abilityAction.Ability.IsChargeAttack;
			}
			return ActionListClearFlag.Keep;
		}
	}
}
