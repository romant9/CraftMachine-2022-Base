using System.Collections.Generic;
using Newtonsoft.Json;

namespace TWDModel
{
	public class TankActorModel : RaiderModel
	{
		[JsonIgnore]
		private ActorFootprint _footprint;

		public FacingDirection Facing { get; set; }

		[JsonIgnore]
		public ActorFootprint Footprint => _footprint;

		[JsonIgnore]
		public override bool IsMultiCell
		{
			get
			{
				EnsureFootprint();
				return _footprint != null;
			}
		}

		[JsonIgnore]
		public override bool IsImpenetrable => true;

		[JsonIgnore]
		public override bool UsesScreenTopHealthBar => true;

		private void EnsureFootprint()
		{
			if (_footprint == null && base.manager != null && !string.IsNullOrEmpty(base.ActorDefinitionID))
			{
				ActorDefinition actorDefinition = base.Definition;
				if (actorDefinition != null && actorDefinition.FootprintWidth > 0)
				{
					_footprint = ActorFootprint.CreateFromActorDefinition(actorDefinition);
				}
			}
		}

		public override List<GridCoordinate> GetOccupiedCells()
		{
			EnsureFootprint();
			if (_footprint == null)
			{
				return base.GetOccupiedCells();
			}
			return _footprint.GetOccupiedCells(base.GridCoordinate, Facing);
		}

		public override List<GridCoordinate> GetOccupiedCellsAt(GridCoordinate anchor)
		{
			EnsureFootprint();
			if (_footprint == null)
			{
				return base.GetOccupiedCellsAt(anchor);
			}
			return _footprint.GetOccupiedCells(anchor, Facing);
		}

		public void SetFootprint(ActorFootprint footprint)
		{
			_footprint = footprint;
		}

		public override GridCoordinate GetAttackOriginCell()
		{
			EnsureFootprint();
			if (_footprint == null)
			{
				return base.GetAttackOriginCell();
			}
			return GetFrontCenterCell();
		}

		public override GridCoordinate GetClosestOccupiedCell(GridCoordinate from)
		{
			List<GridCoordinate> occupiedCells = GetOccupiedCells();
			if (occupiedCells == null || occupiedCells.Count == 0)
			{
				return base.GetClosestOccupiedCell(from);
			}
			GridCoordinate result = occupiedCells[0];
			int num = result.SquaredDistanceTo(from);
			for (int i = 1; i < occupiedCells.Count; i++)
			{
				int num2 = occupiedCells[i].SquaredDistanceTo(from);
				if (num2 < num)
				{
					num = num2;
					result = occupiedCells[i];
				}
			}
			return result;
		}

		public TankPartRole? GetPartRoleAt(GridCoordinate worldCell)
		{
			EnsureFootprint();
			if (_footprint == null)
			{
				return null;
			}
			GridCoordinate gridCoordinate = ActorFootprint.UnrotateOffset(worldCell - base.GridCoordinate, Facing) + _footprint.AnchorOffset;
			bool flag = false;
			int num = int.MaxValue;
			int num2 = int.MinValue;
			List<GridCoordinate> baseOffsets = _footprint.BaseOffsets;
			for (int i = 0; i < baseOffsets.Count; i++)
			{
				GridCoordinate gridCoordinate2 = baseOffsets[i];
				if (gridCoordinate2.Y < num)
				{
					num = gridCoordinate2.Y;
				}
				if (gridCoordinate2.Y > num2)
				{
					num2 = gridCoordinate2.Y;
				}
				if (gridCoordinate2 == gridCoordinate)
				{
					flag = true;
				}
			}
			if (!flag)
			{
				return null;
			}
			if (gridCoordinate.Y == num)
			{
				return TankPartRole.Front;
			}
			if (gridCoordinate.Y == num2)
			{
				return TankPartRole.Rear;
			}
			return TankPartRole.Body;
		}

		public bool IsAdjacentTo(GridCoordinate cell)
		{
			GridModel gridModel = ((base.manager != null && base.manager.CombatModel != null) ? base.manager.CombatModel.Grid : null);
			if (gridModel == null)
			{
				return false;
			}
			List<GridCoordinate> occupiedCells = GetOccupiedCells();
			for (int i = 0; i < occupiedCells.Count; i++)
			{
				if (gridModel.AreNeighbors(occupiedCells[i], cell))
				{
					return true;
				}
			}
			return false;
		}

		public bool CanRotateTo(FacingDirection newFacing, CombatModel combatModel)
		{
			EnsureFootprint();
			if (_footprint == null || combatModel == null)
			{
				return false;
			}
			List<GridCoordinate> occupiedCells = _footprint.GetOccupiedCells(base.GridCoordinate, newFacing);
			for (int i = 0; i < occupiedCells.Count; i++)
			{
				if (!combatModel.Grid.IsCoordinateValid(occupiedCells[i]))
				{
					return false;
				}
				if (combatModel.IsBlocked(occupiedCells[i]))
				{
					return false;
				}
				ActorModel occupier = combatModel.GetOccupier(occupiedCells[i]);
				if (occupier != null && occupier != this)
				{
					return false;
				}
			}
			return true;
		}

		private GridCoordinate GetFootprintLocalCell(int localX, int localY)
		{
			EnsureFootprint();
			if (_footprint == null)
			{
				return base.GridCoordinate;
			}
			GridCoordinate offset = new GridCoordinate(localX, localY) - _footprint.AnchorOffset;
			return base.GridCoordinate + ActorFootprint.RotateOffset(offset, Facing);
		}

		public GridCoordinate GetFrontCenterCell()
		{
			return GetFootprintLocalCell(1, 0);
		}

		public GridCoordinate GetRearCenterCell()
		{
			return GetFootprintLocalCell(1, 4);
		}

		public GridCoordinate GetVisualCenterCell()
		{
			return GetFootprintLocalCell(1, 2);
		}
	}
}
