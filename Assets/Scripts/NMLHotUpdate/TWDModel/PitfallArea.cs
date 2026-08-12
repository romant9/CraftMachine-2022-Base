using System.Collections.Generic;

namespace TWDModel
{
	public class PitfallArea : CombatArea
	{
		public ActorModel Owner;

		public List<GridCoordinate> PitfallAreaGrids { get; private set; }

		public override CombatAreaType Type => CombatAreaType.Pitfall;

		public PitfallArea()
		{
		}

		public PitfallArea(FixedPoint xLength, FixedPoint yLength, FixedPoint radius, GridCoordinate gridCoordinate, Faction faction, int expiryTurn, ActorModel owner)
			: base(xLength, yLength, radius, gridCoordinate, faction, expiryTurn)
		{
			Owner = owner;
			addPitfallAreaGrids(xLength, yLength, gridCoordinate);
		}

		public PitfallArea(PitfallArea area)
			: base(area)
		{
			Owner = area.Owner;
			PitfallAreaGrids = new List<GridCoordinate>(area.PitfallAreaGrids);
		}

		private void addPitfallAreaGrids(FixedPoint xLength, FixedPoint yLength, GridCoordinate baseGridCoordinate)
		{
			PitfallAreaGrids = new List<GridCoordinate>();
			for (int i = baseGridCoordinate.X; i < baseGridCoordinate.X + xLength; i++)
			{
				int num = baseGridCoordinate.Y;
				while (num > baseGridCoordinate.Y - yLength)
				{
					PitfallAreaGrids.Add(new GridCoordinate(i, num));
					num--;
				}
			}
		}

		public override bool IsInArea(GridCoordinate otherCoord)
		{
			if (otherCoord.X >= Coordinate.X && otherCoord.X - Coordinate.X <= XLength - 1L && otherCoord.Y <= Coordinate.Y)
			{
				return Coordinate.Y - otherCoord.Y <= YLength - 1L;
			}
			return false;
		}

		public override bool IsNearAreaGrid(GridCoordinate otherCoord)
		{
			if (IsInArea(otherCoord))
			{
				return false;
			}
			if (otherCoord.X >= Coordinate.X - 1 && otherCoord.X <= Coordinate.X + XLength && otherCoord.Y >= Coordinate.Y - YLength)
			{
				return otherCoord.Y <= Coordinate.Y + 1;
			}
			return false;
		}

		public GridCoordinate GetMinDistanceAreaGrid(GridCoordinate baseGridCoordinate)
		{
			FixedPoint fixedPoint = 100L;
			GridCoordinate result = GridCoordinate.Invalid;
			foreach (GridCoordinate pitfallAreaGrid in PitfallAreaGrids)
			{
				FixedPoint fixedPoint2 = pitfallAreaGrid.DistanceTo(baseGridCoordinate);
				if (fixedPoint > fixedPoint2)
				{
					fixedPoint = fixedPoint2;
					result = pitfallAreaGrid;
				}
			}
			return result;
		}
	}
}
