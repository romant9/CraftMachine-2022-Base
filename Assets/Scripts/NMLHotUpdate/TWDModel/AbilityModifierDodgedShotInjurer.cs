using System.Collections.Generic;

namespace TWDModel
{
	public class AbilityModifierDodgedShotInjurer : ActionModifier
	{
		private FixedPoint InjureDodgedChance;

		public AbilityModifierDodgedShotInjurer(FixedPoint injureDodgedChance)
		{
			InjureDodgedChance = injureDodgedChance;
		}

		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			if (action is DamageAction damageAction && !(action is DamageConsumableAction) && damageAction.DamagerActor == actor && damageAction.DamagerActor != damageAction.TargetActor && damageAction.DamagerActor.IsDodgeShot && base.manager.Player.RollDice(RollDiceType.Dodge, InjureDodgedChance, 0L) != PlayerRandomChanceResult.Failed)
			{
				damageAction.Dodged = true;
				damageAction.DodgedShot = true;
			}
			if (action is PostAbilityExecuteAction postAbilityExecuteAction && postAbilityExecuteAction.DamagerActor == actor && postAbilityExecuteAction.DamagerActor != postAbilityExecuteAction.TargetActor && postAbilityExecuteAction.DamagerActor.IsDodgeShot)
			{
				postAbilityExecuteAction.DamagerActor.DodgeShotTimes--;
				if (!postAbilityExecuteAction.DamagerActor.IsDodgeShot)
				{
					postAbilityExecuteAction.DamagerActor.RemoveTrait("DodgedShotInjurerFlag");
				}
			}
			return ActionListClearFlag.Keep;
		}
	}
}
