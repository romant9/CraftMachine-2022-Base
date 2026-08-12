using System;
using System.Collections.Generic;

namespace TWDModel
{
	public class ShieldRevengeTrait : ActionModifier
	{
		private FixedPoint Parameter0;

		private FixedPoint Parameter1;

		private int Parameter2;

		public const int SortOrder = 7;

		public ShieldRevengeTrait(FixedPoint parameter0, FixedPoint parameter1, int parameter2)
		{
			Parameter0 = parameter0;
			Parameter1 = parameter1;
			Parameter2 = parameter2;
		}

		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			if (!actor.CanPerformOOT || actor.dashTraitAttackFlag)
			{
				return ActionListClearFlag.Keep;
			}
			if (actor.ShieldRevengedTimesOnTurn >= Parameter2)
			{
				return ActionListClearFlag.Keep;
			}
			CombatModel combatModel = base.manager.CombatModel;
			if (combatModel == null)
			{
				return ActionListClearFlag.Keep;
			}
			if (action is DamageAction { DamagerActor: not null } damageAction && actor.IsEnemy(damageAction.DamagerActor) && damageAction.TargetActor != null && damageAction.TargetActor.ShieldHitPoints > 0 && actor != damageAction.TargetActor && actor.Faction == damageAction.TargetActor.Faction && damageAction.TargetActor.Faction != Faction.Environmental && damageAction.DamagerActor.Faction != Faction.Environmental && damageAction.DamagerActor.Faction != damageAction.TargetActor.Faction && !(damageAction is DamageConsumableAction))
			{
				EquipmentItemModel weaponEquipment = actor.GetWeaponEquipment();
				if (weaponEquipment == null)
				{
					return ActionListClearFlag.Keep;
				}
				FixedPoint value = 0.0;
				base.manager.CombatModel.AbilityManager.VisitParameter("ExtendProbability", ref value, actor);
				FixedPoint citadel_PursuitDown_ParameterMultiplier = damageAction.DamagerActor.GetCitadel_PursuitDown_ParameterMultiplier();
				Parameter0 *= citadel_PursuitDown_ParameterMultiplier;
				if (Parameter0 <= ActorTraitContainerModel.Citadel_PercentBase)
				{
					Parameter0 = ActorTraitContainerModel.Citadel_PercentBase;
				}
				if (base.manager.Player.RollDice(RollDiceType.ShieldRevenge, Parameter0, value) == PlayerRandomChanceResult.Failed)
				{
					return ActionListClearFlag.Keep;
				}
				if (actor.SelectedAbility.CanAbilityBePerformedOnGridCell(combatModel, actor, actor.GridCoordinate, damageAction.DamagerActor.GridCoordinate) == AbilityResult.Success)
				{
					AbilityAction action2 = new ShieldRevengeAction(actor, weaponEquipment.Ability, damageAction.DamagerActor.GridCoordinate, Parameter1, Parameter2, damageAction.DamagerActor, OOTType.Revenge, isTriggerExtraAttackDamage: true);
					combatModel.AbilityManager.StoreAbilityAction(action2);
				}
			}
			if (action is PostAbilityExecuteAction && combatModel != null && combatModel.AbilityManager != null)
			{
				AbilityAction pendingActionOfType = combatModel.AbilityManager.GetPendingActionOfType<ShieldRevengeAction>(actor);
				if (pendingActionOfType != null)
				{
					if (pendingActionOfType.TargetActor != null && pendingActionOfType.TargetActor.IsDead)
					{
						actor.ShieldRevengedTimesOnTurn = Math.Max(0, actor.ShieldRevengedTimesOnTurn - 1);
						combatModel.AbilityManager.RemoveStoredAbilityActionsOfType<ShieldRevengeAction>(actor);
						return ActionListClearFlag.Keep;
					}
					addedActions.Add(pendingActionOfType);
					actor.NotifyChange("AbilityVisited", new object[2] { "ShieldRevenge", false });
					combatModel.AbilityManager.RemoveStoredAbilityActionsOfType<ShieldRevengeAction>(actor);
				}
			}
			return ActionListClearFlag.Keep;
		}
	}
}
