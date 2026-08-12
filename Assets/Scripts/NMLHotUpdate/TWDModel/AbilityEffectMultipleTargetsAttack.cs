using System;
using System.Collections.Generic;

namespace TWDModel
{
	public class AbilityEffectMultipleTargetsAttack : AbilityEffect
	{
		private EquipmentCategory equipmentCategory;

		private Dictionary<EquipmentCategory, DamageType> categoryDamageMap = new Dictionary<EquipmentCategory, DamageType>();

		public AbilityEffectMultipleTargetsAttack(string inEquipmentCategory)
		{
			equipmentCategory = (EquipmentCategory)Enum.Parse(typeof(EquipmentCategory), inEquipmentCategory);
			categoryDamageMap[EquipmentCategory.MeleeWeapon] = DamageType.Melee;
			categoryDamageMap[EquipmentCategory.RangeWeapon] = DamageType.Ranged;
		}

		public override AbilityResult CanAbilityBePerformedOnGridCell(CombatModel combatModel, ActorModel sourceActor, GridCoordinate sourceCell, GridCoordinate targetCell, bool acceptInteractiveObjects = false)
		{
			AbilityResult result = base.CanAbilityBePerformedOnGridCell(combatModel, sourceActor, sourceCell, targetCell);
			if (combatModel.FindPath(sourceActor, sourceCell, targetCell).Count > sourceActor.MoveRange)
			{
				result = AbilityResult.FailedOutOfRange;
			}
			return result;
		}

		public override AbilityResult CanAbilityBePerformedOnGridCell(CombatModel combatModel, ActorModel sourceActor, GridCoordinate sourceCell, GridCoordinate targetCell, FixedPoint preComputedRange, bool acceptInteractiveObjects = false)
		{
			AbilityResult result = base.CanAbilityBePerformedOnGridCell(combatModel, sourceActor, sourceCell, targetCell, preComputedRange);
			if (combatModel.FindPath(sourceActor, sourceCell, targetCell).Count > sourceActor.MoveRange)
			{
				result = AbilityResult.FailedOutOfRange;
			}
			return result;
		}

		public override bool ApplyEffect(CombatModel combatModel, ActorModel source, GridCoordinate targetCell, ActorModel targetActor = null)
		{
			EquipmentItemModel equipmentOfCategory = source.GetEquipmentOfCategory(equipmentCategory);
			if (equipmentOfCategory != null && ownerAbility != null && ownerAbility.IsEquipmentAllowed(equipmentOfCategory.Definition.Type))
			{
				(combatModel.Manager as TWDModelManager).ExecuteAction(new MultipleTargetsAttackAction(source, targetCell, ownerAbility));
				DamageType damageType = categoryDamageMap[equipmentCategory];
				List<ActorModel> listOfActorsToBeTargetted = combatModel.AbilityManager.GetListOfActorsToBeTargetted(ownerAbility, source, source.GridCoordinate, targetCell);
				for (int i = 0; i < equipmentOfCategory.Ability.Modifiers.GetCount(); i++)
				{
					ownerAbility.Modifiers.RegisterModifier(equipmentOfCategory.Ability.Modifiers.GetModifier(i));
				}
				CombatHelpers.AttackTargets(combatModel, source, listOfActorsToBeTargetted, equipmentOfCategory.Ability, damageType);
				if (ownerAbility.IsChargeAttack)
				{
					CombatHelpers.CheckForLeaderBuffLeadByExample(combatModel, source);
				}
				CombatHelpers.CheckForExtraApMovement(source, listOfActorsToBeTargetted, combatModel);
				source.ClearPerAttackFlags();
				for (int j = 0; j < equipmentOfCategory.Ability.Modifiers.GetCount(); j++)
				{
					ModelModifier modifier = equipmentOfCategory.Ability.Modifiers.GetModifier(j);
					ownerAbility.Modifiers.RemoveModifier(modifier);
				}
			}
			return true;
		}
	}
}
