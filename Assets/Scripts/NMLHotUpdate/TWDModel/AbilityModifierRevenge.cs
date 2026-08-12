using System;
using System.Collections.Generic;

namespace TWDModel
{
	public class AbilityModifierRevenge : ActionModifier
	{
		private FixedPoint multiplier = 1.0;

		public const int SortOrder = 6;

		public AbilityModifierRevenge()
		{
		}

		public AbilityModifierRevenge(FixedPoint damageMultiplier)
		{
			multiplier = damageMultiplier;
		}

		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			if (!actor.CanPerformOOT || actor.dashTraitAttackFlag)
			{
				return ActionListClearFlag.Keep;
			}
			if (action is PostDamageAction postDamageAction)
			{
				ActorModel damagerActor = postDamageAction.DamagerActor;
				if (damagerActor != null && damagerActor.HasTrait("CoupDeGraceActive"))
				{
					return ActionListClearFlag.Keep;
				}
				CombatModel combatModel = actor.manager.CombatModel;
				if (postDamageAction.TargetActor != actor && combatModel != null && !combatModel.MissionCompleted && postDamageAction.TargetActor != null && postDamageAction.DamagerActor != null && postDamageAction.DamagerActor != actor && !actor.IsEnemy(postDamageAction.TargetActor))
				{
					if (postDamageAction.TargetActor.Faction == Faction.Lure)
					{
						Type type = postDamageAction.TargetActor.GetType();
						if (type == typeof(RaiderModel) && actor.Faction == Faction.Survivor)
						{
							return ActionListClearFlag.Keep;
						}
						if (type == typeof(SurvivorModel) && actor.Faction == Faction.Raider)
						{
							return ActionListClearFlag.Keep;
						}
					}
					EquipmentItemModel weaponEquipment = actor.GetWeaponEquipment();
					if (weaponEquipment != null && postDamageAction.DamagerActor != null && weaponEquipment.Definition != null && weaponEquipment.Definition.Category == EquipmentCategory.RangeWeapon && !actor.RevengedOnTurn && IsInRangeAndHasLineOfSight(actor, postDamageAction.DamagerActor.GridCoordinate, actor.SelectedAbility))
					{
						FixedPoint value = multiplier;
						combatModel.AbilityManager.VisitParameter("AbilityModifierIncreaseRevengeDamage", ref value, actor);
						combatModel.AbilityManager.VisitParameter("AbilityModifierIncreaseEquipRevengeDamage", ref value, actor);
						RevengeAction action2 = new RevengeAction(actor, postDamageAction.TargetActor, weaponEquipment.Ability, postDamageAction.DamagerActor.GridCoordinate, value, postDamageAction.DamagerActor, OOTType.Revenge, isTriggerExtraAttackDamage: true);
						combatModel.AbilityManager.StoreAbilityAction(action2);
						if (!base.manager.GameEconomyData.GetFeature("RevengeRetaliateFix").Enabled)
						{
							actor.RevengedOnTurn = true;
						}
					}
				}
			}
			if (action is PostAbilityExecuteAction)
			{
				CombatModel combatModel2 = actor.manager.CombatModel;
				if (combatModel2 != null && combatModel2.AbilityManager != null)
				{
					AbilityAction pendingActionOfType = combatModel2.AbilityManager.GetPendingActionOfType<RevengeAction>(actor);
					if (pendingActionOfType != null)
					{
						addedActions.Add(pendingActionOfType);
						combatModel2.AbilityManager.RemoveStoredAbilityActionsOfType<RevengeAction>(actor);
					}
				}
			}
			return ActionListClearFlag.Keep;
		}

		private bool IsInRangeAndHasLineOfSight(ActorModel actor, GridCoordinate targetCoordinate, AbilityModel selectedAbility)
		{
			if (actor != null && selectedAbility != null)
			{
				FixedPoint range = selectedAbility.Definition.AbilityRange;
				EquipmentItemModel weaponEquipment = actor.GetWeaponEquipment();
				if (actor.manager.CombatModel != null)
				{
					GridModel grid = actor.manager.CombatModel.Grid;
					if (!selectedAbility.IsConsumableAbility)
					{
						CombatHelpers.CalculateRangeExtension(ref range, actor, actor.manager.CombatModel.AbilityManager);
					}
					FixedPoint fixedPoint = (range + (selectedAbility.Definition.AbilityTargetDiagonal ? 0.42f : 0f)) * grid.CellSize.X;
					FixedPoint fixedPoint2 = fixedPoint * fixedPoint;
					FixedVec3 position = grid.GetPosition(actor.GridCoordinate);
					FixedVec3 position2 = grid.GetPosition(targetCoordinate);
					if ((position - position2).SqrMagnitude < fixedPoint2)
					{
						if (weaponEquipment.Ability.Definition.RequiresLineOfSight && !weaponEquipment.Ability.Definition.HasFriendlyFire)
						{
							return actor.manager.CombatModel.IsGridCellVisible(actor.GridCoordinate, targetCoordinate);
						}
						return false;
					}
				}
			}
			return false;
		}
	}
}
