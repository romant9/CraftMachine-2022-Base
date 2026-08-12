using System.Collections.Generic;

namespace TWDModel
{
	public class TrapFlameTrait : ActionModifier
	{
		private FixedPoint ReleaseTrapFlamePercentage;

		private FixedPoint TrapFlameTargetRange;

		private FixedPoint TrapFlameRandomGridCoordinateCount;

		private FixedPoint TrapFlameTurns;

		private FixedPoint InTrapFlameInjuryHPPercentage;

		public TrapFlameTrait(FixedPoint releaseTrapFlamePercentage, FixedPoint trapFlameTargetRange, FixedPoint trapFlameRandomGridCoordinateCount, FixedPoint trapFlameTurns, FixedPoint inTrapFlameInjuryHpPercentage)
		{
			ReleaseTrapFlamePercentage = releaseTrapFlamePercentage;
			TrapFlameTargetRange = trapFlameTargetRange;
			TrapFlameRandomGridCoordinateCount = trapFlameRandomGridCoordinateCount;
			TrapFlameTurns = trapFlameTurns;
			InTrapFlameInjuryHPPercentage = inTrapFlameInjuryHpPercentage;
		}

		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			if (action is AbilityBeforeRemoveActiveTraitAction { Source: not null } abilityBeforeRemoveActiveTraitAction && abilityBeforeRemoveActiveTraitAction.Source == actor)
			{
				if (abilityBeforeRemoveActiveTraitAction.TargetCell == GridCoordinate.Invalid)
				{
					return ActionListClearFlag.Keep;
				}
				if (abilityBeforeRemoveActiveTraitAction.AbilityAction == null)
				{
					return ActionListClearFlag.Keep;
				}
				if (abilityBeforeRemoveActiveTraitAction.AbilityAction.Ability == null)
				{
					return ActionListClearFlag.Keep;
				}
				if (abilityBeforeRemoveActiveTraitAction.AbilityAction.Ability.IsConsumableAbility)
				{
					return ActionListClearFlag.Keep;
				}
				FixedPoint value = 0.0;
				base.manager.CombatModel.AbilityManager.VisitParameter("ExtendProbability", ref value, actor);
				if (base.manager.Player.RollDice(RollDiceType.TrapFlame, ReleaseTrapFlamePercentage, value) == PlayerRandomChanceResult.Failed)
				{
					return ActionListClearFlag.Keep;
				}
				CreateTrapFlameArea(actor, abilityBeforeRemoveActiveTraitAction.AbilityAction);
			}
			return ActionListClearFlag.Keep;
		}

		private void CreateTrapFlameArea(ActorModel actor, AbilityAction abilityAction)
		{
			CombatModel combatModel = actor.manager.CombatModel;
			TrapFlameAreaManager trapFlameAreaManager = combatModel.GetModel<TrapFlameAreaManager>();
			if (trapFlameAreaManager == null)
			{
				trapFlameAreaManager = new TrapFlameAreaManager();
				trapFlameAreaManager.SetManager(actor.manager);
				combatModel.AddModel(trapFlameAreaManager);
			}
			List<GridCoordinate> radiusNeighborCoordinates = GetRadiusNeighborCoordinates(combatModel, abilityAction.TargetCell);
			List<GridCoordinate> list = new List<GridCoordinate>();
			list.Add(abilityAction.TargetCell);
			list.AddRange(radiusNeighborCoordinates);
			List<TrapFlameArea> trapFlameAreasFromGridCoordinates = TrapFlameAreaManager.GetTrapFlameAreasFromGridCoordinates(actor, abilityAction.TargetCell, combatModel.TurnManager.TurnCount + (int)TrapFlameTurns, list, InTrapFlameInjuryHPPercentage);
			trapFlameAreaManager.UpdateWhenNewAreaGenerated(trapFlameAreasFromGridCoordinates);
		}

		private List<GridCoordinate> GetRadiusNeighborCoordinates(CombatModel combatModel, GridCoordinate coordinate)
		{
			List<GridCoordinate> list = new List<GridCoordinate>();
			foreach (GridCoordinate coordinate2 in combatModel.Grid.Coordinates)
			{
				if (coordinate2.CheckGridInWidthAndHeightRange(coordinate, (int)TrapFlameTargetRange) && !combatModel.IsBlocked(coordinate2) && !(coordinate2 == coordinate))
				{
					list.Add(coordinate2);
				}
			}
			if (list.Count <= TrapFlameRandomGridCoordinateCount)
			{
				return list;
			}
			List<GridCoordinate> list2 = new List<GridCoordinate>();
			for (int i = 0; i < (int)TrapFlameRandomGridCoordinateCount; i++)
			{
				GridCoordinate randomElement = combatModel.manager.Player.PlayerRandom.GetRandomElement(list, remove: true);
				list2.Add(randomElement);
			}
			return list2;
		}
	}
}
