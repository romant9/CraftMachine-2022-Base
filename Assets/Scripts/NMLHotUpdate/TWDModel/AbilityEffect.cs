using System.Collections.Generic;

namespace TWDModel
{
	public class AbilityEffect
	{
		protected AbilityModel ownerAbility;

		public void SetOwnerAbility(AbilityModel ability)
		{
			ownerAbility = ability;
		}

		public virtual bool ApplyEffect(CombatModel combatModel, ActorModel source, GridCoordinate targetCell, ActorModel targetActor = null)
		{
			return false;
		}

		public virtual bool ApplyEffect(CombatModel combatModel, ActorModel source, GridCoordinate targetCell, ActorModel targetActor = null, Dictionary<RollDiceType, PlayerRandomChanceResult> resolvedRolls = null, OOTType ootType = OOTType.None, bool isAssistAttack = false, bool isTriggerExtraAttackDamage = false)
		{
			return false;
		}

		protected virtual void VisitModifierBeforeApplyingEffect(CombatModel combatModel, ref FixedPoint value, ActorModel sourceActor)
		{
		}

		public virtual AbilityResult CanAbilityBePerformedOnGridCell(CombatModel combatModel, ActorModel sourceActor, GridCoordinate sourceCell, GridCoordinate targetCell, bool acceptInteractiveObjects = false)
		{
			return ValidateAbilityWithTargetFactions(combatModel, sourceActor, sourceCell, targetCell, acceptInteractiveObjects);
		}

		public virtual AbilityResult CanAbilityBePerformedOnGridCell(CombatModel combatModel, ActorModel sourceActor, GridCoordinate sourceCell, GridCoordinate targetCell, FixedPoint preComputedRange, bool acceptInteractiveObjects = false)
		{
			return ValidateAbilityWithTargetFactions(combatModel, sourceActor, sourceCell, targetCell, preComputedRange, acceptInteractiveObjects);
		}

		protected AbilityResult ValidateAbilityWithTargetFactions(CombatModel combatModel, ActorModel sourceActor, GridCoordinate sourceCell, GridCoordinate targetCell, bool acceptInteractiveObjects = false)
		{
			if (ownerAbility.Definition.TriggerType == AbilityTriggerType.Grid)
			{
				if (!combatModel.IsBlocked(targetCell) && combatModel.GetOccupier(targetCell) == null)
				{
					return combatModel.IsAbilityTargetValid(ownerAbility, sourceActor, sourceCell, targetCell, acceptInteractiveObjects);
				}
				return AbilityResult.FailedNoValidTarget;
			}
			if (ownerAbility.Definition.TriggerType == AbilityTriggerType.GridOrTarget)
			{
				if (!combatModel.IsBlocked(targetCell))
				{
					return combatModel.IsAbilityTargetValid(ownerAbility, sourceActor, sourceCell, targetCell, acceptInteractiveObjects);
				}
				return AbilityResult.FailedNoValidTarget;
			}
			if (ownerAbility.Definition.TargetAreaAtSource)
			{
				return AbilityResult.Success;
			}
			if (combatModel.GetOccupier(targetCell) == null && (combatModel.GetInteractiveObject(targetCell) == null || !acceptInteractiveObjects))
			{
				return AbilityResult.FailedNoValidTarget;
			}
			return combatModel.IsAbilityTargetValid(ownerAbility, sourceActor, sourceCell, targetCell, acceptInteractiveObjects);
		}

		protected AbilityResult ValidateAbilityWithTargetFactions(CombatModel combatModel, ActorModel sourceActor, GridCoordinate sourceCell, GridCoordinate targetCell, FixedPoint preComputedRange, bool acceptInteractiveObjects = false)
		{
			if (ownerAbility.Definition.TriggerType == AbilityTriggerType.Grid)
			{
				if (!combatModel.IsBlocked(targetCell) && combatModel.GetOccupier(targetCell) == null)
				{
					return combatModel.IsAbilityTargetValid(ownerAbility, sourceActor, sourceCell, targetCell, preComputedRange, acceptInteractiveObjects);
				}
				return AbilityResult.FailedNoValidTarget;
			}
			if (ownerAbility.Definition.TriggerType == AbilityTriggerType.GridOrTarget)
			{
				if (!combatModel.IsBlocked(targetCell))
				{
					return combatModel.IsAbilityTargetValid(ownerAbility, sourceActor, sourceCell, targetCell, preComputedRange, acceptInteractiveObjects);
				}
				return AbilityResult.FailedNoValidTarget;
			}
			if (ownerAbility.Definition.TargetAreaAtSource)
			{
				return AbilityResult.Success;
			}
			if (combatModel.GetOccupier(targetCell) == null && (combatModel.GetInteractiveObject(targetCell) == null || !acceptInteractiveObjects))
			{
				return AbilityResult.FailedNoValidTarget;
			}
			return combatModel.IsAbilityTargetValid(ownerAbility, sourceActor, sourceCell, targetCell, preComputedRange, acceptInteractiveObjects);
		}
	}
}
