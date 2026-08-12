using System.Collections.Generic;

namespace TWDModel
{
	public class StrengthenDefenseFunc3Trait : ActionModifier
	{
		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			if (!(action is PreDealDamageAction { DamageAction: not null } preDealDamageAction) || preDealDamageAction.DamageAction.TargetActor != actor || !actor.HasTraitsThatContains("StrengthenDefenseFunc3"))
			{
				return ActionListClearFlag.Keep;
			}
			DamageType damageType = preDealDamageAction.DamageAction.DamageType;
			if (damageType != DamageType.Base && damageType != DamageType.Melee && damageType != DamageType.Ranged && damageType != DamageType.HelpHand && damageType != DamageType.BloodMarkSplash && damageType != DamageType.BloodMarkSettlement)
			{
				return ActionListClearFlag.Keep;
			}
			FixedPoint value = 0.0;
			FixedPoint value2 = 0.0;
			FixedPoint value3 = 0.0;
			actor?.manager?.CombatModel?.AbilityManager?.VisitParameter("StrengthenDefenseFunc3Param1", ref value, actor);
			actor?.manager?.CombatModel?.AbilityManager?.VisitParameter("StrengthenDefenseFunc3Param2", ref value2, actor);
			actor?.manager?.CombatModel?.AbilityManager?.VisitParameter("StrengthenDefenseFunc3Param3", ref value3, actor);
			int num = preDealDamageAction.DamageAction.BaseDamage + preDealDamageAction.DamageAction.AdditionalCriticalDamage;
			if (num + preDealDamageAction.DamageAction.ModifyDamage < actor.Hitpoints || num <= 0)
			{
				return ActionListClearFlag.Keep;
			}
			int num2 = (int)(num * value + value2 + actor.MaxHitPoints * value3);
			if (num - num2 <= 0)
			{
				preDealDamageAction.DamageAction.UpAdditionalCriticalDamage(0);
				preDealDamageAction.DamageAction.UpBaseDamage(1);
			}
			else if (preDealDamageAction.DamageAction.AdditionalCriticalDamage > num2)
			{
				int newAdditionalCriticalDamage = preDealDamageAction.DamageAction.AdditionalCriticalDamage - num2;
				preDealDamageAction.DamageAction.UpAdditionalCriticalDamage(newAdditionalCriticalDamage);
			}
			else
			{
				int num3 = num2 - preDealDamageAction.DamageAction.AdditionalCriticalDamage;
				preDealDamageAction.DamageAction.UpAdditionalCriticalDamage(0);
				if (preDealDamageAction.DamageAction.BaseDamage > num3)
				{
					int newDamage = preDealDamageAction.DamageAction.BaseDamage - num3;
					preDealDamageAction.DamageAction.UpBaseDamage(newDamage);
				}
				else
				{
					preDealDamageAction.DamageAction.UpBaseDamage(1);
				}
			}
			return ActionListClearFlag.Keep;
		}
	}
}
