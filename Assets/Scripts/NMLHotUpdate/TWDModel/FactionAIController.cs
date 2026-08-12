using System;
using System.Collections.Generic;

namespace TWDModel
{
	public class FactionAIController
	{
		private CombatModel combatModel;

		public Faction Faction { get; private set; }

		public List<ActorModel> VisibleEnemies { get; private set; }

		public FactionAIController()
		{
			VisibleEnemies = new List<ActorModel>();
		}

		public FactionAIController(Faction faction, CombatModel combat)
		{
			Faction = faction;
			combatModel = combat;
			VisibleEnemies = new List<ActorModel>();
		}

		public void SetCombatModel(CombatModel combat)
		{
			combatModel = combat;
		}

		public List<ActorModel> GetVisibleEnemiesWhoCanTarget(ActorModel actor)
		{
			List<ActorModel> list = new List<ActorModel>();
			for (int i = 0; i < VisibleEnemies.Count; i++)
			{
				ActorModel actorModel = VisibleEnemies[i];
				if (actorModel.SelectedAbility != null && actorModel.SelectedAbility.CanAbilityBePerformedOnGridCell(combatModel, actorModel, actorModel.GridCoordinate, actor.GridCoordinate) == AbilityResult.Success)
				{
					list.Add(actorModel);
				}
			}
			return list;
		}

		public GridCoordinate GetRetreatCoordinate(ActorModel actor)
		{
			GridCoordinate result = GridCoordinate.Invalid;
			GridModel grid = combatModel.Grid;
			List<ActorModel> visibleEnemiesWhoCanTarget = GetVisibleEnemiesWhoCanTarget(actor);
			List<GridCoordinate> list = new List<GridCoordinate>();
			for (int i = 0; i < visibleEnemiesWhoCanTarget.Count; i++)
			{
				list.Add(visibleEnemiesWhoCanTarget[i].GridCoordinate);
			}
			GridField<FixedPoint> gridField = DistanceField.CreateDistanceField(combatModel, list, new DistanceFieldOptions(1.5f, actor, actor));
			FixedPoint fixedPoint = FixedPoint.MinValue;
			for (int j = 0; j < grid.NumCells; j++)
			{
				int num = 0;
				GridCoordinate coordinate = grid.GetCoordinate(j);
				if (!(gridField[coordinate] > fixedPoint))
				{
					continue;
				}
				for (int k = 0; k < VisibleEnemies.Count; k++)
				{
					ActorModel actorModel = VisibleEnemies[k];
					if (actorModel.SelectedAbility.CanAbilityBePerformedOnGridCell(combatModel, actorModel, actorModel.GridCoordinate, coordinate) == AbilityResult.Success)
					{
						num++;
					}
					if (num < visibleEnemiesWhoCanTarget.Count)
					{
						fixedPoint = gridField[coordinate];
						result = coordinate;
					}
				}
			}
			return result;
		}

		public TacticalMoveTargetInfo GetTacticalMoveTarget(ActorModel actor, ActorModel currentTarget)
		{
			GridCoordinate coordinate = GridCoordinate.Invalid;
			GridModel grid = combatModel.Grid;
			SurvivorClass survivorClass = (SurvivorClass)Enum.Parse(typeof(SurvivorClass), actor.Definition.Class);
			AIMoveBehaviorData aIMoveBehaviorData = combatModel.manager.GameEconomyData.GetAIMoveBehaviorData(actor.Faction, actor.AIController.AIDataModel.Mode, survivorClass);
			List<GridCoordinate> list = new List<GridCoordinate>();
			for (int i = 0; i < VisibleEnemies.Count; i++)
			{
				list.Add(VisibleEnemies[i].GridCoordinate);
			}
			GridField<FixedPoint> gridField = new GridField<FixedPoint>(grid.Width, grid.Height, 0.0);
			GridField<FixedPoint> gridField2 = new GridField<FixedPoint>(grid.Width, grid.Height, 0.0);
			GridField<FixedPoint> gridField3 = new GridField<FixedPoint>(grid.Width, grid.Height, 0.0);
			GridField<FixedPoint> gridField4 = new GridField<FixedPoint>(grid.Width, grid.Height, 0.0);
			GridField<FixedPoint> gridField5 = DistanceField.CreateDistanceField(combatModel, actor.GridCoordinate, new DistanceFieldOptions(1.5f, actor, actor));
			GridField<FixedPoint> gridField6 = ((currentTarget != null) ? DistanceField.CreateDistanceField(combatModel, list, new DistanceFieldOptions(1.5f, actor, actor)) : new GridField<FixedPoint>(grid.Width, grid.Height, 0.0));
			GridField<FixedPoint> gridField7 = null;
			GridField<FixedPoint> gridField8 = null;
			GridField<FixedPoint> gridField9 = null;
			GridField<FixedPoint> gridField10 = null;
			GridField<bool> gridField11 = null;
			if (combatModel.AILog != null)
			{
				gridField7 = new GridField<FixedPoint>(grid.Width, grid.Height, 0.0);
				gridField8 = new GridField<FixedPoint>(grid.Width, grid.Height, 0.0);
				gridField9 = new GridField<FixedPoint>(grid.Width, grid.Height, 0.0);
				gridField10 = new GridField<FixedPoint>(grid.Width, grid.Height, 0.0);
				gridField11 = new GridField<bool>(grid.Width, grid.Height, defaultValue: false);
			}
			FixedPoint currentTargetMultiplier = 1.0;
			FixedPoint fixedPoint = aIMoveBehaviorData?.ActorHitEnemiestMultiplier ?? ((FixedPoint)1.0);
			FixedPoint fixedPoint2 = aIMoveBehaviorData?.EnemiesTargetActorMultiplier ?? ((FixedPoint)1.0);
			FixedPoint fixedPoint3 = aIMoveBehaviorData?.ExploreMultiplier ?? ((FixedPoint)1.0);
			FixedPoint fixedPoint4 = aIMoveBehaviorData?.DistanceMultiplier ?? ((FixedPoint)1.0);
			FixedPoint fixedPoint5 = aIMoveBehaviorData?.DistanceToTargetMultiplier ?? ((FixedPoint)0.0);
			FixedPoint fixedPoint6 = aIMoveBehaviorData?.CurrentTargetMultiplier ?? ((FixedPoint)1.0);
			FixedPoint fixedPoint7 = aIMoveBehaviorData?.CoverBaseValue ?? ((FixedPoint)0.0);
			FixedPoint fixedPoint8 = aIMoveBehaviorData?.CoverMultiplier ?? ((FixedPoint)1.0);
			FixedPoint fixedPoint9 = 1.0;
			GridField<CellValidity> gridField12 = new GridField<CellValidity>(grid.Width, grid.Height, new CellValidity(CellStatus.Invalid, null, null));
			CombatHelpers.GetValidMoveTargets(combatModel, actor, actor.GridCoordinate, actor.MoveRange, gridField12, null);
			int coordinateOffset = grid.GetCoordinateOffset(actor.GridCoordinate);
			gridField12[coordinateOffset] = new CellValidity(CellStatus.Valid, null, null);
			List<ActorModel> factionActors = combatModel.GetFactionActors(Faction);
			List<GridCoordinate> list2 = new List<GridCoordinate>();
			for (int j = 0; j < grid.NumCells; j++)
			{
				GridCoordinate coordinate2 = grid.GetCoordinate(j);
				for (int k = 0; k < factionActors.Count; k++)
				{
					ActorModel actorModel = factionActors[k];
					if (!list2.Contains(coordinate2) && combatModel.IsGridCellVisible(actorModel.GridCoordinate, coordinate2))
					{
						list2.Add(coordinate2);
						break;
					}
				}
			}
			for (int l = 0; l < gridField12.Length; l++)
			{
				GridCoordinate coordinate3 = grid.GetCoordinate(l);
				if (!gridField12[l].Valid || (combatModel.GetOccupier(coordinate3) != null && combatModel.GetOccupier(coordinate3) != actor))
				{
					continue;
				}
				int num = 0;
				int num2 = 0;
				int num3 = 0;
				fixedPoint9 = 1.0;
				List<GridCoordinate> list3 = new List<GridCoordinate>();
				bool flag = false;
				for (int m = 0; m < grid.NumCells; m++)
				{
					GridCoordinate coordinate4 = grid.GetCoordinate(m);
					if (!list3.Contains(coordinate4))
					{
						if (combatModel.IsGridCellVisible(coordinate4, coordinate3))
						{
							list3.Add(coordinate4);
						}
						else
						{
							for (int n = 0; n < factionActors.Count; n++)
							{
								ActorModel actorModel2 = factionActors[n];
								if (actorModel2 != actor && combatModel.IsGridCellVisible(coordinate4, actorModel2.GridCoordinate))
								{
									list3.Add(coordinate4);
									break;
								}
							}
						}
					}
					ActorModel occupier = combatModel.GetOccupier(coordinate4);
					if (occupier == null || !VisibleEnemies.Contains(occupier))
					{
						continue;
					}
					if (combatModel.IsGridCellVisible(occupier.GridCoordinate, coordinate3))
					{
						num2++;
					}
					if (combatModel.IsInCover(coordinate3, occupier.GridCoordinate))
					{
						EquipmentItemModel weaponEquipment = occupier.GetWeaponEquipment();
						if (weaponEquipment != null && weaponEquipment.Definition.Category == EquipmentCategory.RangeWeapon)
						{
							num3++;
						}
						fixedPoint9 += fixedPoint8;
					}
					else
					{
						fixedPoint9 -= fixedPoint8;
					}
					if (actor.SelectedAbility.CanAbilityBeTargetedOnGridCell(combatModel, actor, coordinate3, occupier.GridCoordinate))
					{
						List<ActorModel> listOfActorsToBeTargetted = combatModel.AbilityManager.GetListOfActorsToBeTargetted(actor.SelectedAbility, actor, coordinate3, occupier.GridCoordinate);
						num = Math.Max(num, listOfActorsToBeTargetted.Count);
						if (!flag)
						{
							flag = occupier == currentTarget;
						}
					}
				}
				FixedPoint fixedPoint10 = num * fixedPoint;
				fixedPoint10 = (flag ? (fixedPoint10 * fixedPoint6) : fixedPoint10);
				FixedPoint value = num2 * fixedPoint2;
				FixedPoint value2 = (list3.Count - list2.Count) * fixedPoint3;
				fixedPoint9 = Math.Max(0f, (float)fixedPoint9);
				FixedPoint value3 = ((combatModel.GetCoveredDirections(coordinate3) != 0 && num3 > 0) ? (fixedPoint7 * fixedPoint9) : ((FixedPoint)0.0));
				gridField[l] = fixedPoint10;
				gridField2[l] = value;
				gridField3[l] = value2;
				gridField4[l] = value3;
				if (combatModel.AILog != null)
				{
					gridField7[l] = num;
					gridField8[l] = num2;
					gridField9[l] = list3.Count - list2.Count;
					gridField10[l] = fixedPoint9;
					gridField11[l] = combatModel.GetCoveredDirections(coordinate3) != 0 && num3 > 0;
					currentTargetMultiplier = (flag ? fixedPoint6 : ((FixedPoint)1.0));
				}
			}
			GridField<FixedPoint> gridField13 = null;
			if (combatModel.AILog != null)
			{
				gridField13 = new GridField<FixedPoint>(grid.Width, grid.Height, 0.0);
			}
			FixedPoint fixedPoint11 = FixedPoint.MinValue;
			for (int num4 = 0; num4 < grid.NumCells; num4++)
			{
				GridCoordinate coordinate5 = grid.GetCoordinate(num4);
				FixedPoint fixedPoint12 = gridField5[coordinate5] * fixedPoint4;
				FixedPoint fixedPoint13 = gridField6[coordinate5] * fixedPoint5;
				FixedPoint fixedPoint14 = (gridField12[coordinate5].Valid ? (gridField[coordinate5] + gridField2[coordinate5] + gridField3[coordinate5] + fixedPoint12 + fixedPoint13 + gridField4[coordinate5]) : FixedPoint.MinValue);
				if (fixedPoint14 > fixedPoint11)
				{
					fixedPoint11 = fixedPoint14;
					coordinate = coordinate5;
				}
				if (gridField13 != null)
				{
					gridField13[coordinate5] = fixedPoint14;
				}
			}
			if (combatModel.AILog != null)
			{
				combatModel.AILog.CurrentActorTurnLogEntry.SetAttackField(gridField7, fixedPoint, currentTargetMultiplier);
				combatModel.AILog.CurrentActorTurnLogEntry.SetDefenceField(gridField8, fixedPoint2);
				combatModel.AILog.CurrentActorTurnLogEntry.SetExploreField(gridField9, fixedPoint3);
				combatModel.AILog.CurrentActorTurnLogEntry.SetDistanceField(gridField5, fixedPoint4);
				combatModel.AILog.CurrentActorTurnLogEntry.SetDistanceToTargetField(gridField6, fixedPoint5);
				combatModel.AILog.CurrentActorTurnLogEntry.SetMovementField(gridField13);
				combatModel.AILog.CurrentActorTurnLogEntry.SetCoverField(gridField11, gridField10, fixedPoint7, fixedPoint9);
				combatModel.AILog.CurrentActorTurnLogEntry.SetVisibleEnemyLocations(VisibleEnemies);
			}
			return new TacticalMoveTargetInfo
			{
				Coordinate = coordinate,
				Value = fixedPoint11
			};
		}

		public void UpdateSituation()
		{
			VisibleEnemies.Clear();
			GridModel grid = combatModel.Grid;
			List<ActorModel> factionActors = combatModel.GetFactionActors(Faction);
			for (int i = 0; i < grid.NumCells; i++)
			{
				GridCoordinate coordinate = grid.GetCoordinate(i);
				for (int j = 0; j < factionActors.Count; j++)
				{
					ActorModel actorModel = factionActors[j];
					ActorModel occupier = combatModel.GetOccupier(coordinate);
					if (!combatModel.IsGridCellVisible(coordinate, actorModel.GridCoordinate) || occupier == null)
					{
						continue;
					}
					if (combatModel.HasPvPRules)
					{
						if (occupier.Faction == Faction.Survivor && !VisibleEnemies.Contains(occupier))
						{
							VisibleEnemies.Add(occupier);
						}
					}
					else if (occupier.IsEnemy(actorModel) && !VisibleEnemies.Contains(occupier))
					{
						VisibleEnemies.Add(occupier);
					}
					break;
				}
			}
		}
	}
}
