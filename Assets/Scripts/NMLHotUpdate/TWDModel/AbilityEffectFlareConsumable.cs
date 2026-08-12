using System.Collections.Generic;

namespace TWDModel
{
	public class AbilityEffectFlareConsumable : AbilityEffect
	{
		private string objectIdentifier;

		private int lureDuration;

		private int lureLifetime;

		public AbilityEffectFlareConsumable()
		{
		}

		public AbilityEffectFlareConsumable(string identifier, int duration, int lifetime)
		{
			objectIdentifier = identifier;
			lureDuration = duration;
			lureLifetime = lifetime;
		}

		public override AbilityResult CanAbilityBePerformedOnGridCell(CombatModel combatModel, ActorModel sourceActor, GridCoordinate sourceCell, GridCoordinate targetCell, bool acceptInteractiveObjects = false)
		{
			ActorModel occupier = combatModel.GetOccupier(targetCell);
			if (!combatModel.IsBlocked(targetCell) && (occupier == null || occupier.IsEnemy(sourceActor)))
			{
				bool abilityTargetDiagonal = ownerAbility.Definition.AbilityTargetDiagonal;
				FixedPoint fixedPoint = (ownerAbility.Definition.AbilityRange + (abilityTargetDiagonal ? 0.42f : 0f)) * combatModel.Grid.CellSize.X;
				FixedPoint fixedPoint2 = fixedPoint * fixedPoint;
				FixedVec3 position = combatModel.Grid.GetPosition(sourceCell);
				FixedVec3 position2 = combatModel.Grid.GetPosition(targetCell);
				if ((position - position2).SqrMagnitude >= fixedPoint2)
				{
					return AbilityResult.FailedOutOfRange;
				}
				return AbilityResult.Success;
			}
			return AbilityResult.FailedNoValidTarget;
		}

		public override AbilityResult CanAbilityBePerformedOnGridCell(CombatModel combatModel, ActorModel sourceActor, GridCoordinate sourceCell, GridCoordinate targetCell, FixedPoint preComputedRange, bool acceptInteractiveObjects = false)
		{
			return CanAbilityBePerformedOnGridCell(combatModel, sourceActor, sourceCell, targetCell, acceptInteractiveObjects);
		}

		public override bool ApplyEffect(CombatModel combatModel, ActorModel source, GridCoordinate targetCell, ActorModel targetActor = null, Dictionary<RollDiceType, PlayerRandomChanceResult> resolvedRolls = null, OOTType ootType = OOTType.None, bool isAssistAttack = false, bool isTriggerExtraAttackDamage = false)
		{
			return (combatModel.Manager as TWDModelManager).ExecuteAction(new ThrowableAction(source, targetCell, objectIdentifier, lureLifetime, lureDuration));
		}
	}
}
