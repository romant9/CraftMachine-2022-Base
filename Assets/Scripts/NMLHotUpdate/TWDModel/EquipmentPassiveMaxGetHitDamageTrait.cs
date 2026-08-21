using System;
using System.Collections.Generic;

namespace TWDModel
{
	public class EquipmentPassiveMaxGetHitDamageTrait : ActionModifier
	{
		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			if (!(action is PreHPDeductionAction preHPDeductionAction) || actor == null || preHPDeductionAction.Target != actor)
			{
				return ActionListClearFlag.Keep;
			}
			if (preHPDeductionAction.Damage <= 0 || preHPDeductionAction.DamageType == DamageType.Heal || actor.ShieldHitPoints > 0 || IsExecutionDamage(preHPDeductionAction))
			{
				return ActionListClearFlag.Keep;
			}
			AbilityManagerModel abilityManagerModel = actor.manager?.CombatModel?.AbilityManager;
			if (abilityManagerModel == null)
			{
				return ActionListClearFlag.Keep;
			}
			FixedPoint value = 0.0;
			string paramName = ((preHPDeductionAction.Attacker?.Definition?.Class == "Boss") ? "AbilityModifierEquipmentPassiveMaxGetHitDamageBossCap" : "AbilityModifierEquipmentPassiveMaxGetHitDamageNormalCap");
			abilityManagerModel.VisitParameter(paramName, ref value, actor);
			if (value <= 0.0)
			{
				return ActionListClearFlag.Keep;
			}
			int num = Math.Max(1, (int)(actor.MaxHitPoints * value));
			if (preHPDeductionAction.Damage <= num)
			{
				return ActionListClearFlag.Keep;
			}
			preHPDeductionAction.Damage = num;
			actor.NotifyChange("AbilityVisited", new object[2] { "Equipment.Passive.MaxGetHitDamage", false });
			return ActionListClearFlag.Keep;
		}

		private static bool IsExecutionDamage(PreHPDeductionAction action)
		{
			if (action != null && action.Attacker != null && action.Target != null && action.Target.Iskill)
			{
				return action.Attacker.HasTraitsThatContains("NegativeFlagFatalFlag");
			}
			return false;
		}
	}
}
