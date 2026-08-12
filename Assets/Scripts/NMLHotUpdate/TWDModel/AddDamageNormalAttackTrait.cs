using System.Collections.Generic;

namespace TWDModel
{
	public class AddDamageNormalAttackTrait : ActionModifier
	{
		private readonly FixedPoint minTargetHpPercentage;

		private readonly FixedPoint maxTargetHpPercentage;

		private readonly FixedPoint extraDamagePercentage;

		private bool shouldApplyForCurrentAbilityAction;

		public AddDamageNormalAttackTrait(FixedPoint minTargetHpPercentage, FixedPoint maxTargetHpPercentage, FixedPoint extraDamagePercentage)
		{
			this.minTargetHpPercentage = minTargetHpPercentage;
			this.maxTargetHpPercentage = maxTargetHpPercentage;
			this.extraDamagePercentage = extraDamagePercentage;
		}

		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			if (action == null || actor == null)
			{
				return ActionListClearFlag.Keep;
			}
			if (action is AbilityAction abilityAction && abilityAction.Actor == actor)
			{
				if (abilityAction.IsFromAbilityCommand && !abilityAction.IsTriggerExtraAttackDamage && abilityAction.Ability != null && !abilityAction.Ability.IsChargeAttack && !abilityAction.Ability.IsConsumableAbility)
				{
					shouldApplyForCurrentAbilityAction = true;
				}
				return ActionListClearFlag.Keep;
			}
			if (action is PreDealDamageAction preDealDamageAction && shouldApplyForCurrentAbilityAction)
			{
				TryApplyDamageBonus(preDealDamageAction, actor);
				return ActionListClearFlag.Keep;
			}
			if (action is AbilityBeforeRemoveActiveTraitAction abilityBeforeRemoveActiveTraitAction && abilityBeforeRemoveActiveTraitAction.Source == actor && abilityBeforeRemoveActiveTraitAction.AbilityAction != null && abilityBeforeRemoveActiveTraitAction.AbilityAction.IsFromAbilityCommand && !abilityBeforeRemoveActiveTraitAction.AbilityAction.IsTriggerExtraAttackDamage && abilityBeforeRemoveActiveTraitAction.AbilityAction.Ability != null && !abilityBeforeRemoveActiveTraitAction.AbilityAction.Ability.IsChargeAttack && !abilityBeforeRemoveActiveTraitAction.AbilityAction.Ability.IsConsumableAbility)
			{
				shouldApplyForCurrentAbilityAction = false;
			}
			return ActionListClearFlag.Keep;
		}

		private void TryApplyDamageBonus(PreDealDamageAction preDealDamageAction, ActorModel actor)
		{
			if (preDealDamageAction == null || actor == null)
			{
				return;
			}
			DamageAction damageAction = preDealDamageAction.DamageAction;
			if (damageAction != null && damageAction.DamagerActor == actor && damageAction.TargetActor != null && !damageAction.IsTriggerExtraAttackDamage && !damageAction.IsChargeAttack && !damageAction.IsPushDamage && damageAction.SourceSupport == null && !(damageAction is DamageConsumableAction) && damageAction.TargetActor.MaxHitPoints > 0)
			{
				FixedPoint fixedPoint = FixedPoint.Min((FixedPoint)damageAction.TargetActor.Hitpoints / (FixedPoint)damageAction.TargetActor.MaxHitPoints, 1.0);
				if (!(fixedPoint < minTargetHpPercentage) && !(fixedPoint > maxTargetHpPercentage))
				{
					damageAction.UpBaseDamage((int)((1L + extraDamagePercentage) * damageAction.BaseDamage));
					damageAction.UpAdditionalCriticalDamage((int)((1L + extraDamagePercentage) * damageAction.AdditionalCriticalDamage));
				}
			}
		}
	}
}
