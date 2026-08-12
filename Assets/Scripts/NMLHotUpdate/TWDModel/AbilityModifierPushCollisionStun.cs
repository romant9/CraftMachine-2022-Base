using System.Collections.Generic;

namespace TWDModel
{
	public class AbilityModifierPushCollisionStun : ActionModifier
	{
		private int turns;

		private int chancePercentage;

		public AbilityModifierPushCollisionStun()
		{
		}

		public AbilityModifierPushCollisionStun(int turns, int chancePercentage)
		{
			this.turns = turns;
			this.chancePercentage = chancePercentage;
		}

		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			DamageAction damageAction = action as DamageAction;
			if (damageAction != null && damageAction.IsPushDamage && !damageAction.TargetActor.IsDead && !damageAction.TargetActor.IsEnvironmental && base.manager.Player.RollDice(RollDiceType.Stun, (FixedPoint)chancePercentage / (FixedPoint)100.0, 0.0) != PlayerRandomChanceResult.Failed)
			{
				addedActions.Add(new StunAction(damageAction.DamagerActor, damageAction.TargetActor, turns, ignoreSourceBeingDead: false, null, () => damageAction.FinalDamage));
			}
			return ActionListClearFlag.Keep;
		}
	}
}
