using System.Collections.Generic;

namespace TWDModel
{
	public class AbilityModifierPush : ActionModifier
	{
		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			if (action is DamageAction { BodyShot: false } damageAction && !damageAction.TargetActor.IsDead && !damageAction.TargetActor.IsEnvironmental)
			{
				AbilityModel abilityUnderApplication = base.manager.Player.AbilityManager.AbilityUnderApplication;
				ActorModel abilityOwnerActor = base.manager.Player.AbilityManager.AbilityOwnerActor;
				bool flag = abilityOwnerActor == null || abilityOwnerActor == damageAction.DamagerActor;
				if (abilityUnderApplication != null && abilityUnderApplication.PushEffect != null && flag && abilityUnderApplication.PushEffect.Add(damageAction))
				{
					abilityUnderApplication.PostExecuteActions.Add(damageAction);
				}
			}
			return ActionListClearFlag.Keep;
		}
	}
}
