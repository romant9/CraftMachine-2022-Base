using System.Collections.Generic;

namespace TWDModel
{
	public class MultiAttacksTrait : ActionModifier
	{
		private int _parameter0Weight;

		private int _parameter1Weight;

		private int _parameter2Weight;

		private FixedPoint _parameter3Weight;

		private FixedPoint _parameter4Weight;

		private bool IsTriggered;

		public MultiAttacksTrait(int parameter0Weight, int parameter1Weight, int parameter2Weight, FixedPoint parameter3Weight, FixedPoint parameter4Weight)
		{
			_parameter0Weight = parameter0Weight;
			_parameter1Weight = parameter1Weight;
			_parameter2Weight = parameter2Weight;
			_parameter3Weight = parameter3Weight;
			_parameter4Weight = parameter4Weight;
		}

		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			CombatModel combatModel = actor?.manager?.CombatModel;
			if (combatModel == null)
			{
				return ActionListClearFlag.Keep;
			}
			if (action is PostDamageAction { DamagerActor: not null } postDamageAction && actor == postDamageAction.DamagerActor && postDamageAction.TargetActor.Faction != Faction.Environmental && postDamageAction.DamagerActor.Faction != Faction.Environmental && postDamageAction.DamageAction.SourceSupport == null && !(postDamageAction.DamageAction is DamageConsumableAction) && postDamageAction.IsMainTarget && !IsTriggered && !postDamageAction.TargetActor.IsDead && !postDamageAction.DamagerActor.IsMoving && !postDamageAction.IsTriggerExtraAttackDamage)
			{
				EquipmentItemModel weaponEquipment = actor.GetWeaponEquipment();
				if (weaponEquipment == null || weaponEquipment.Ability == null || actor.SelectedAbility == null)
				{
					return ActionListClearFlag.Keep;
				}
				if (actor.SelectedAbility.CanAbilityBePerformedOnGridCell(combatModel, actor, actor.GridCoordinate, postDamageAction.TargetActor.GridCoordinate) != AbilityResult.Success)
				{
					return ActionListClearFlag.Keep;
				}
				FixedPoint[] weights = new FixedPoint[3] { _parameter0Weight, _parameter1Weight, _parameter2Weight };
				int num = actor.manager.Player.PlayerRandom.WeightedRandom(weights);
				if (num > 0)
				{
					string text = "MultiAttackDoubleShot";
					addedActions.Add(new MultiAttackAction(actor, weaponEquipment.Ability, postDamageAction.TargetActor.GridCoordinate, postDamageAction.TargetActor, _parameter3Weight));
					if (num > 1)
					{
						addedActions.Add(new MultiAttackAction(actor, weaponEquipment.Ability, postDamageAction.TargetActor.GridCoordinate, postDamageAction.TargetActor, _parameter4Weight));
						text = "MultiAttackTripleShot";
					}
					actor.NotifyChange("AbilityVisited", new object[2] { text, false });
				}
				IsTriggered = true;
			}
			return ActionListClearFlag.Keep;
		}
	}
}
