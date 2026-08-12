using System;
using System.Collections.Generic;

namespace TWDModel
{
	public class DebuffEquipmentKaboomTrait : ActionModifier
	{
		public FixedPoint TriggerCount;

		public FixedPoint DamageReduce;

		public FixedPoint DamageReflect;

		public FixedPoint ReflectMaxHpRatio;

		public override string ToString()
		{
			return $"DebuffEquipmentKaboomTrait(TriggerCount: {TriggerCount}, DamageReduce: {DamageReduce}%, DamageReflect: {DamageReflect}%, ReflectMaxHpRatio: {ReflectMaxHpRatio}%)";
		}

		public DebuffEquipmentKaboomTrait(FixedPoint triggerCount, FixedPoint damageReduce, FixedPoint damageReflect, FixedPoint reflectMaxHpRatio)
		{
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
			if (actor.IsDead)
			{
				return ActionListClearFlag.Keep;
			}
			if (action is PreDealDamageAction { DamageAction: not null } preDealDamageAction && preDealDamageAction.DamageAction.DamagerActor == actor && !preDealDamageAction.DamageAction.IsTriggerExtraAttackDamage && !preDealDamageAction.DamageAction.IsPushDamage && preDealDamageAction.DamageAction.SourceSupport == null && !(preDealDamageAction.DamageAction is DamageConsumableAction))
			{
				DamageAction damageAction = preDealDamageAction.DamageAction;
				FixedPoint fixedPoint = FixedPoint.Max(0L, FixedPoint.Min(100L, DamageReduce));
				FixedPoint fixedPoint2 = 1L - fixedPoint / 100L;
				int val = (int)(damageAction.BaseDamage * fixedPoint2);
				int val2 = (int)(damageAction.AdditionalCriticalDamage * fixedPoint2);
				val = Math.Max(0, val);
				val2 = Math.Max(0, val2);
				damageAction.UpBaseDamage(val);
				damageAction.UpAdditionalCriticalDamage(val2);
			}
			if (action is PostDamageAction postDamageAction && postDamageAction.DamagerActor == actor && postDamageAction.DamageAction != null && postDamageAction.DamageAction.IsMainTarget && !postDamageAction.IsTriggerExtraAttackDamage && !postDamageAction.DamageAction.IsPushDamage && postDamageAction.DamageAction.SourceSupport == null && !(postDamageAction.DamageAction is DamageConsumableAction))
			{
				FixedPoint fixedPoint3 = postDamageAction.DamageAction.FinalDamage;
				if (fixedPoint3 > 0L)
				{
					actor.NotifyChange("AbilityVisited", new object[2] { "DebuffEquipmentKaboom", false });
					FixedPoint fixedPoint4 = FixedPoint.Max(0L, DamageReflect);
					FixedPoint fixedPoint5 = FixedPoint.Max(0L, ReflectMaxHpRatio);
					FixedPoint fixedPoint6 = fixedPoint3 * fixedPoint4 / 100L;
					FixedPoint fixedPoint7 = actor.MaxHitPoints * fixedPoint5 / 100L;
					if (fixedPoint6 > fixedPoint7)
					{
						fixedPoint6 = fixedPoint7;
					}
					if (fixedPoint6 > 0L)
					{
						int damage = (int)FixedPoint.Ceiling(fixedPoint6);
						addedActions?.Add(new EquipmentKaboomReflectDamageAction(actor, damage));
					}
				}
				TriggerCount -= (FixedPoint)1L;
				TraitEntry traitEntry = actor.TraitContainer?.GetTraitAnyLevel("DebuffEquipmentKaboom");
				if (traitEntry != null)
				{
					if (traitEntry.RemodeValues != null && traitEntry.RemodeValues.Count > 0)
					{
						traitEntry.RemodeValues[0] = (int)TriggerCount;
					}
					if (TriggerCount <= 0L)
					{
						actor.RemoveAnyLevelTrait("DebuffEquipmentKaboom");
					}
				}
			}
			return ActionListClearFlag.Keep;
		}
	}
}
