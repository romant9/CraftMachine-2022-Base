using System;
using System.Collections.Generic;

namespace TWDModel
{
	public class EquipmentActiveBloodMarkTrait : ActionModifier
	{
		private int durationTurns;

		public EquipmentActiveBloodMarkTrait()
		{
		}

		public EquipmentActiveBloodMarkTrait(int durationTurns)
		{
			this.durationTurns = durationTurns;
		}

		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			if (action == null || actor == null)
			{
				return ActionListClearFlag.Keep;
			}
			if (action is PostDamageAction postDamageAction)
			{
				TryApplyBloodMark(actor, postDamageAction);
			}
			return ActionListClearFlag.Keep;
		}

		private void TryApplyBloodMark(ActorModel source, PostDamageAction postDamageAction)
		{
			if (postDamageAction.DamagerActor == source && postDamageAction.IsMainTarget && postDamageAction.TargetActor != null && postDamageAction.DamageAction != null && !postDamageAction.DamageAction.IsPushDamage && postDamageAction.DamageAction.SourceSupport == null && !(postDamageAction.DamageAction is DamageConsumableAction) && !postDamageAction.IsTriggerExtraAttackDamage && postDamageAction.DamageAction.DamageType != DamageType.BloodMarkSettlement && postDamageAction.DamageAction.DamageType != DamageType.BloodMarkSplash)
			{
				ActorModel targetActor = postDamageAction.TargetActor;
				if (!targetActor.IsDead && !targetActor.IsEnvironmental && targetActor.Faction != Faction.Environmental && source.Faction != Faction.Environmental && !source.IsDead && targetActor.IsEnemy(source))
				{
					ApplyBloodMark(source, targetActor);
				}
			}
		}

		private void ApplyBloodMark(ActorModel source, ActorModel target)
		{
			EquipmentPassiveBloodMarkTrait equipmentPassiveBloodMarkTrait = EquipmentPassiveBloodMarkTrait.FindOnActor(source);
			if (equipmentPassiveBloodMarkTrait != null)
			{
				FixedPoint healthPercentageForTarget = equipmentPassiveBloodMarkTrait.GetHealthPercentageForTarget(target);
				int num = Math.Max(1, durationTurns);
				CombatModel combatModel = source.manager?.CombatModel;
				if (combatModel != null && source.Faction != combatModel.TurnManager.ActiveFaction)
				{
					num++;
				}
				BloodMarkTimedEffect bloodMarkTimedEffect = new BloodMarkTimedEffect(num, source, target, equipmentPassiveBloodMarkTrait.MoveDistanceCap, equipmentPassiveBloodMarkTrait.DamageCount, healthPercentageForTarget, equipmentPassiveBloodMarkTrait.Chance, equipmentPassiveBloodMarkTrait.DamagePercentage, equipmentPassiveBloodMarkTrait.Range, equipmentPassiveBloodMarkTrait.DamageLimit);
				bloodMarkTimedEffect.InstigatorFaction = target.Faction;
				target.StartTimedEffect(bloodMarkTimedEffect);
			}
		}
	}
}
