using System.Collections.Generic;

namespace TWDModel
{
	public class CadenceTrait : ActionModifier
	{
		private readonly int activeAttackCount;

		private readonly int chargeAttackCount;

		private readonly int extraAttackCount;

		private readonly int threshold;

		private readonly FixedPoint damageBoostPercentage;

		public CadenceTrait(int activeAttackCount, int chargeAttackCount, int extraAttackCount, int threshold, FixedPoint damageBoostPercentage)
		{
			this.activeAttackCount = activeAttackCount;
			this.chargeAttackCount = chargeAttackCount;
			this.extraAttackCount = extraAttackCount;
			this.threshold = threshold;
			this.damageBoostPercentage = damageBoostPercentage;
		}

		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			if (actor.IsDead)
			{
				return ActionListClearFlag.Keep;
			}
			if (action is PreDealDamageAction { DamageAction: var damageAction } preDealDamageAction)
			{
				if (damageAction.DamagerActor != actor)
				{
					return ActionListClearFlag.Keep;
				}
				if (damageAction.IsPushDamage || damageAction.SourceSupport != null || damageAction is DamageConsumableAction)
				{
					return ActionListClearFlag.Keep;
				}
				if (!damageAction.IsMainTarget)
				{
					if (actor.CadenceBoostingThisAttack)
					{
						ApplyDamageBoost(preDealDamageAction);
					}
					return ActionListClearFlag.Keep;
				}
				actor.CadenceBoostingThisAttack = false;
				if (actor.CadenceReady)
				{
					ApplyDamageBoost(preDealDamageAction);
					actor.CadenceAttackCount = 0;
					actor.CadenceReady = false;
					actor.CadenceBoostingThisAttack = true;
					actor.NotifyChange("AbilityVisited", new object[2] { "Cadence", false });
				}
				else
				{
					int countIncrement = GetCountIncrement(damageAction);
					if (countIncrement > 0)
					{
						actor.CadenceAttackCount += countIncrement;
						if (actor.CadenceAttackCount >= threshold)
						{
							actor.CadenceReady = true;
						}
					}
				}
			}
			return ActionListClearFlag.Keep;
		}

		private int GetCountIncrement(DamageAction damageAction)
		{
			if (damageAction.IsChargeAttack)
			{
				return chargeAttackCount;
			}
			if (damageAction.IsTriggerExtraAttackDamage)
			{
				return extraAttackCount;
			}
			return activeAttackCount;
		}

		private void ApplyDamageBoost(PreDealDamageAction predmgAction)
		{
			DamageAction damageAction = predmgAction.DamageAction;
			int newDamage = (int)(damageAction.BaseDamage * (1L + damageBoostPercentage));
			int newAdditionalCriticalDamage = (int)(damageAction.AdditionalCriticalDamage * (1L + damageBoostPercentage));
			damageAction.UpBaseDamage(newDamage);
			damageAction.UpAdditionalCriticalDamage(newAdditionalCriticalDamage);
		}
	}
}
