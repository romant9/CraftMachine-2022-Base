using System.Collections.Generic;

namespace TWDModel
{
	public class ShieldBreakerStrikeType1Trait : ActionModifier
	{
		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			if (action is PostDamageAction { DamagerActor: not null } postDamageAction && actor == postDamageAction.DamagerActor && postDamageAction.TargetActor.Faction != Faction.Environmental && postDamageAction.DamagerActor.Faction != Faction.Environmental && !postDamageAction.TargetActor.IsDead && postDamageAction.DamageAction.SourceSupport == null && !(postDamageAction.DamageAction is DamageConsumableAction))
			{
				postDamageAction.TargetActor.NotifyChange("ActorHealthChanged");
			}
			return ActionListClearFlag.Keep;
		}
	}
}
