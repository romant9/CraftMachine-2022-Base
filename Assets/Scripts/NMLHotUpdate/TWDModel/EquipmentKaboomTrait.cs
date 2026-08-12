using System.Collections.Generic;

namespace TWDModel
{
	public class EquipmentKaboomTrait : ActionModifier
	{
		public FixedPoint Prob;

		public FixedPoint Duration;

		public FixedPoint TriggerCount;

		public FixedPoint DamageReduce;

		public FixedPoint DamageReflect;

		public FixedPoint ReflectMaxHpRatio;

		public EquipmentKaboomTrait(FixedPoint prob, FixedPoint duration, FixedPoint triggerCount, FixedPoint damageReduce, FixedPoint damageReflect, FixedPoint reflectMaxHpRatio)
		{
			Prob = prob;
			Duration = duration;
			TriggerCount = triggerCount;
			DamageReduce = damageReduce;
			DamageReflect = damageReflect;
			ReflectMaxHpRatio = reflectMaxHpRatio;
		}

		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			if (action == null || actor == null)
			{
				return ActionListClearFlag.Keep;
			}
			if (action is PostDamageAction postDamageAction && postDamageAction.DamagerActor == actor && postDamageAction.TargetActor != null && !postDamageAction.TargetActor.IsDead && postDamageAction.DamageAction != null && !postDamageAction.DamageAction.IsPushDamage && postDamageAction.DamageAction.SourceSupport == null && !(postDamageAction.DamageAction is DamageConsumableAction) && !postDamageAction.IsTriggerExtraAttackDamage)
			{
				PlayerModel playerModel = actor.manager?.Player;
				if (playerModel == null)
				{
					return ActionListClearFlag.Keep;
				}
				ActorModel targetActor = postDamageAction.TargetActor;
				EquipmentItemModel weaponEquipment = targetActor.GetWeaponEquipment();
				if (weaponEquipment != null && weaponEquipment.Definition != null && weaponEquipment.Definition.Category == EquipmentCategory.RangeWeapon)
				{
					FixedPoint successProbability = FixedPoint.Max(0L, FixedPoint.Min(1L, Prob));
					FixedPoint value = 0.0;
					if (actor.manager?.CombatModel?.AbilityManager != null)
					{
						actor.manager.CombatModel.AbilityManager.VisitParameter("ExtendProbability", ref value, actor);
					}
					if (playerModel.RollDice(RollDiceType.EquipmentKaboom, successProbability, value) != PlayerRandomChanceResult.Failed)
					{
						long num = (long)FixedPoint.Max(1L, Duration);
						List<int> list = new List<int> { 0, 1, 2, 3 };
						List<int> list2 = new List<int>
						{
							(int)FixedPoint.Max(0L, TriggerCount),
							(int)FixedPoint.Max(0L, DamageReduce),
							(int)FixedPoint.Max(0L, DamageReflect),
							(int)FixedPoint.Max(0L, ReflectMaxHpRatio)
						};
						targetActor.RemoveAnyLevelTrait("DebuffEquipmentKaboom");
						long duration = num;
						List<int> remodeIndex = list;
						List<int> remodeValue = list2;
						targetActor.AddTemporaryTrait("DebuffEquipmentKaboom", default(FixedPoint), null, duration, "", remodeIndex, remodeValue);
					}
				}
			}
			return ActionListClearFlag.Keep;
		}
	}
}
