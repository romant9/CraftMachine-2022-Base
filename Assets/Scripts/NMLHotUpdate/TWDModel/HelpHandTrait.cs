using System.Collections.Generic;

namespace TWDModel
{
	public class HelpHandTrait : ActionModifier
	{
		private FixedPoint multiplier = 1.0;

		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			if (action is PreAttackAction { TargetActor: not null } preAttackAction && preAttackAction.TargetActor != actor && preAttackAction.TargetActor.HelpreHandActorModel == null)
			{
				ActorModel helpHandActor = CombatHelpers.getHelpHandActor(base.manager.CombatModel, preAttackAction.TargetActor);
				if (helpHandActor != null)
				{
					preAttackAction.TargetActor.HelpreHandActorModel = helpHandActor;
				}
			}
			if (action is PreDealDamageAction preDealDamageAction && !(preDealDamageAction.DamageAction is DamageConsumableAction) && preDealDamageAction.DamageAction != null && (preDealDamageAction.DamageAction.DamagerActor != null || preDealDamageAction.DamageAction.IsEquipmentKaboomReflect) && preDealDamageAction.DamageAction.DamageType != DamageType.HelpHand && preDealDamageAction.DamageAction.TargetActor != null && preDealDamageAction.DamageAction.TargetActor.Faction == actor.Faction && preDealDamageAction.DamageAction.IsMainTarget && preDealDamageAction.DamageAction.TargetActor.HelpreHandActorModel == actor && !preDealDamageAction.DamageAction.Dodged && preDealDamageAction.DamageAction.DamageType != DamageType.Fire && preDealDamageAction.DamageAction.DamageType != DamageType.Heal && preDealDamageAction.DamageAction.DamageType != DamageType.Bleeding && (preDealDamageAction.DamageAction.DamagerActor == null || !preDealDamageAction.DamageAction.DamagerActor.HasTrait("NegativeFlagFatalFlag")))
			{
				int[] array = CombatHelpers.HelpHandDamageCalculation(base.manager.CombatModel, preDealDamageAction, preDealDamageAction.DamageAction.TargetActor, preDealDamageAction.DamageAction.TargetActor.HelpreHandActorModel, preDealDamageAction.DamageAction.DamageType, preDealDamageAction.DamageAction.BaseDamage, preDealDamageAction.DamageAction.AdditionalCriticalDamage);
				int damage = array[2] + array[3];
				CombatHelpers.ExecuteDamage(base.manager.CombatModel, null, actor, damage, 0, DamageType.HelpHand, PlayerRandomChanceResult.Failed, PlayerRandomChanceResult.Failed);
				preDealDamageAction.DamageAction.UpBaseDamage(array[0]);
				preDealDamageAction.DamageAction.UpAdditionalCriticalDamage(array[1]);
				actor.NotifyChange("AbilityVisited", new object[2] { "HelpHand", false });
				preDealDamageAction.DamageAction.TargetActor.NotifyChange("AbilityVisited", new object[2] { "HelpHand", false });
			}
			if (action is PostDamageAction { DamageAction: not null } postDamageAction && postDamageAction.DamageAction.TargetActor != null && postDamageAction.DamageAction.TargetActor.HelpreHandActorModel != null)
			{
				postDamageAction.DamageAction.TargetActor.HelpreHandActorModel = null;
			}
			return ActionListClearFlag.Keep;
		}
	}
}
