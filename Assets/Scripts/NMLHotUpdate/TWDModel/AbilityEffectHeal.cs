using System.Collections.Generic;

namespace TWDModel
{
	public class AbilityEffectHeal : AbilityEffect
	{
		public bool InMoveRange { get; private set; }

		public override bool ApplyEffect(CombatModel combatModel, ActorModel source, GridCoordinate targetCell, ActorModel targetActor = null)
		{
			bool flag = true;
			if (!combatModel.Grid.AreNeighbors(source.GridCoordinate, targetCell) && InMoveRange)
			{
				GridPath pathTowardsTarget = GridHelpers.GetPathTowardsTarget(source, combatModel, targetCell, source.MoveRange);
				flag = (combatModel.Manager as TWDModelManager).ExecuteAction(new MoveAction(source, pathTowardsTarget));
			}
			if (flag && ownerAbility.CanAbilityBePerformedOnGridCell(combatModel, source, source.GridCoordinate, targetCell) == AbilityResult.Success)
			{
				List<ActorModel> listOfActorsToBeTargetted = combatModel.AbilityManager.GetListOfActorsToBeTargetted(ownerAbility, source, source.GridCoordinate, targetCell);
				if (listOfActorsToBeTargetted.Count > 0)
				{
					flag = CombatHelpers.AttackTarget(combatModel, source, listOfActorsToBeTargetted[0], ownerAbility, DamageType.Heal, ignoreRandomHitChance: true);
					listOfActorsToBeTargetted.RemoveAt(0);
					if (flag)
					{
						CombatHelpers.AttackTargets(combatModel, source, listOfActorsToBeTargetted, ownerAbility, DamageType.Heal);
						if (ownerAbility.IsChargeAttack)
						{
							CombatHelpers.CheckForLeaderBuffLeadByExample(combatModel, source);
						}
						source.ClearPerAttackFlags();
					}
				}
				else
				{
					flag = true;
				}
			}
			return flag;
		}

		public override AbilityResult CanAbilityBePerformedOnGridCell(CombatModel combatModel, ActorModel sourceActor, GridCoordinate sourceCell, GridCoordinate targetCell, bool acceptInteractiveObjects = false)
		{
			AbilityResult result = base.CanAbilityBePerformedOnGridCell(combatModel, sourceActor, sourceCell, targetCell);
			ActorModel occupier = combatModel.GetOccupier(targetCell);
			GridCoordinate gridCoordinate = sourceActor.GridCoordinate;
			GridCoordinate other = new GridCoordinate(gridCoordinate.X + sourceActor.MoveRange, gridCoordinate.Y);
			InMoveRange = gridCoordinate.SquaredDistanceTo(targetCell) <= gridCoordinate.SquaredDistanceTo(other);
			if (!InMoveRange)
			{
				result = AbilityResult.FailedOutOfRange;
			}
			else if (occupier == null || occupier.IsStruggling)
			{
				result = AbilityResult.FailedNoValidTarget;
			}
			return result;
		}

		public override AbilityResult CanAbilityBePerformedOnGridCell(CombatModel combatModel, ActorModel sourceActor, GridCoordinate sourceCell, GridCoordinate targetCell, FixedPoint preComputedRange, bool acceptInteractiveObjects = false)
		{
			AbilityResult result = base.CanAbilityBePerformedOnGridCell(combatModel, sourceActor, sourceCell, targetCell, preComputedRange);
			ActorModel occupier = combatModel.GetOccupier(targetCell);
			GridCoordinate gridCoordinate = sourceActor.GridCoordinate;
			GridCoordinate other = new GridCoordinate(gridCoordinate.X + sourceActor.MoveRange, gridCoordinate.Y);
			InMoveRange = gridCoordinate.SquaredDistanceTo(targetCell) <= gridCoordinate.SquaredDistanceTo(other);
			if (!InMoveRange)
			{
				result = AbilityResult.FailedOutOfRange;
			}
			else if (occupier == null || occupier.IsStruggling)
			{
				result = AbilityResult.FailedNoValidTarget;
			}
			return result;
		}
	}
}
