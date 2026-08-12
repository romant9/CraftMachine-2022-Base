using System.Collections.Generic;

namespace TWDModel
{
	public class AttackWithTriggerDotTrait : ActionModifier
	{
		public const int SortOrder = 8;

		private int activeAttackTriggerCount;

		private int chargeAttackTriggerCount;

		private FixedPoint damageIncreasePercentage;

		public AttackWithTriggerDotTrait(int activeAttackTriggerCount, int chargeAttackTriggerCount, FixedPoint damageIncreasePercentage)
		{
			this.activeAttackTriggerCount = activeAttackTriggerCount;
			this.chargeAttackTriggerCount = chargeAttackTriggerCount;
			this.damageIncreasePercentage = damageIncreasePercentage;
		}

		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			if (actor.IsDead || actor.AIController.IsActorIncapacitated || actor.ExclusiveTimedEffect != null)
			{
				return ActionListClearFlag.Keep;
			}
			if (action is PostDamageAction postDamageAction && postDamageAction.DamagerActor == actor && postDamageAction.TargetActor != null && !postDamageAction.TargetActor.IsDead)
			{
				DamageAction damageAction = postDamageAction.DamageAction;
				if (damageAction != null && !damageAction.IsPushDamage)
				{
					if (!HasDotEffect(postDamageAction.TargetActor))
					{
						return ActionListClearFlag.Keep;
					}
					if (actor.manager.CombatModel == null)
					{
						return ActionListClearFlag.Keep;
					}
					int num = 0;
					num = ((!IsChargeAttack(postDamageAction, actor)) ? activeAttackTriggerCount : chargeAttackTriggerCount);
					if (num > 0)
					{
						addedActions.Add(new TriggerDotAction(actor, postDamageAction.TargetActor, num, damageIncreasePercentage));
					}
				}
			}
			return ActionListClearFlag.Keep;
		}

		private bool HasDotEffect(ActorModel targetActor)
		{
			if (targetActor.IsBurning)
			{
				return true;
			}
			if (targetActor.IsBleeding)
			{
				return true;
			}
			if (targetActor.manager.CombatModel != null)
			{
				PoisonRelationsManager model = targetActor.manager.CombatModel.GetModel<PoisonRelationsManager>();
				if (model != null && model.ExistedPoisonRelations != null)
				{
					foreach (PoisonRelation existedPoisonRelation in model.ExistedPoisonRelations)
					{
						if (existedPoisonRelation.TargetActor == targetActor)
						{
							return true;
						}
					}
				}
			}
			if (targetActor.IsQuantuned)
			{
				return true;
			}
			return false;
		}

		private bool IsChargeAttack(PostDamageAction postDamageAction, ActorModel actor)
		{
			if (postDamageAction.IsChargeAttack)
			{
				return true;
			}
			if (actor.FocusModeState)
			{
				return true;
			}
			return false;
		}
	}
}
