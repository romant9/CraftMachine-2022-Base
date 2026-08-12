using System.Collections.Generic;

namespace TWDModel
{
	public class FollowAttackWithSplashDamageTrait : ActionModifier
	{
		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			if (action is PostDamageAction postDamageAction && actor != null && postDamageAction.DamagerActor == actor && base.manager.CombatModel.AbilityManager.AbilityUnderApplication != null && postDamageAction.DamageAction.IsTriggerExtraAttackDamage && postDamageAction.DamageAction.IsMainTarget)
			{
				DamageAction damageAction = postDamageAction.DamageAction;
				CombatModel combatModel = base.manager.CombatModel;
				ActorModel damagerActor = damageAction.DamagerActor;
				ActorModel targetActor = damageAction.TargetActor;
				if (combatModel != null && damagerActor != null && targetActor != null)
				{
					FixedPoint value = 0.0;
					combatModel.AbilityManager.VisitParameter("FollowAttackWithSplashDamageParam2", ref value, damagerActor);
					CombatHelpers.SplashDamage(damageAction, value);
				}
			}
			return ActionListClearFlag.Keep;
		}
	}
}
