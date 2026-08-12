using System.Collections.Generic;

namespace TWDModel
{
	public class RiotShieldStunTrait : ActionModifier
	{
		private int turnCount;

		public RiotShieldStunTrait(int turnCount)
		{
			this.turnCount = turnCount;
		}

		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			DamageAction damageAction = action as DamageAction;
			if (damageAction != null && damageAction.DamagerActor == actor && damageAction.SourceSupport == null && !damageAction.BodyShot)
			{
				_ = actor.manager.CombatModel;
				addedActions.Add(new StunAction(actor, damageAction.TargetActor, turnCount, ignoreSourceBeingDead: false, null, () => damageAction.FinalDamage));
			}
			return ActionListClearFlag.Keep;
		}
	}
}
