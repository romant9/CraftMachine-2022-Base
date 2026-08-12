using System.Collections.Generic;

namespace TWDModel
{
	public class DistanceField
	{
		private static FixedPoint Sqrt2 = 1.4142135381698608;

		private static FixedPoint[] DistanceToNeighborArray = new FixedPoint[8] { 1L, Sqrt2, 1L, Sqrt2, 1L, Sqrt2, 1L, Sqrt2 };

		public static FixedPoint OccupiedMultiplier = 1.100000023841858;

		public static FixedPoint DistanceNotSet = 1000.0;

		public static bool UniformDistance = false;

		private static OpenList _cachedOpenList;

		private static GridField<bool> _cachedVisited;

		private static int _cachedWidth;

		private static int _cachedHeight;

		public static GridField<FixedPoint> CreateDistanceField(CombatModel combatModel, GridCoordinate startLocation, DistanceFieldOptions options)
		{
			List<GridCoordinate> list = new List<GridCoordinate>();
			list.Add(startLocation);
			return CreateDistanceField(combatModel, list, options);
		}

		public static FixedPoint DistanceToNeighbor(int neighborIndex)
		{
			if (!UniformDistance)
			{
				return DistanceToNeighborArray[neighborIndex];
			}
			return 1.0;
		}

		public static GridField<FixedPoint> CreateDistanceField(CombatModel combatModel, List<GridCoordinate> startLocations, DistanceFieldOptions options)
		{
			GridModel grid = combatModel.Grid;
			int width = combatModel.manager.GridModel.Width;
			int height = combatModel.manager.GridModel.Height;
			GridField<FixedPoint> gridField = new GridField<FixedPoint>(width, height, DistanceNotSet);
			if (_cachedVisited == null || _cachedWidth != width || _cachedHeight != height)
			{
				_cachedVisited = new GridField<bool>(width, height, defaultValue: false);
				_cachedWidth = width;
				_cachedHeight = height;
			}
			else
			{
				_cachedVisited.Clear();
			}
			GridField<bool> cachedVisited = _cachedVisited;
			if (_cachedOpenList == null)
			{
				_cachedOpenList = new OpenList();
			}
			else
			{
				_cachedOpenList.Clear();
			}
			OpenList cachedOpenList = _cachedOpenList;
			FixedPoint maxDistance = options.MaxDistance;
			FixedPoint occupancyCostMultiplier = options.OccupancyCostMultiplier;
			ActorModel ignoreActorOccupancy = options.IgnoreActorOccupancy;
			ActorModel ignoreFactionOccupancy = options.IgnoreFactionOccupancy;
			int count = startLocations.Count;
			for (int i = 0; i < count; i++)
			{
				GridCoordinate coordinate = startLocations[i];
				if (!grid.IsCoordinateValid(coordinate))
				{
					continue;
				}
				gridField[coordinate] = 0L;
				cachedVisited[coordinate] = true;
				for (int j = 0; j < 8; j++)
				{
					GridCoordinate coordinateNeighbor = grid.GetCoordinateNeighbor(coordinate, j);
					if (!coordinateNeighbor.IsValid || cachedVisited[coordinateNeighbor] || combatModel.BlockedCache[coordinateNeighbor] || (combatModel.TraversableCache[coordinate] & (1 << j)) == 0)
					{
						continue;
					}
					ActorModel occupier = combatModel.GetOccupier(coordinateNeighbor);
					if (occupier == null || ignoreFactionOccupancy == null || !occupier.IsEnemy(ignoreFactionOccupancy))
					{
						FixedPoint fixedPoint = ((occupier != null && occupier != ignoreActorOccupancy) ? occupancyCostMultiplier : ((FixedPoint)1L));
						FixedPoint fixedPoint2 = (UniformDistance ? ((FixedPoint)1L) : DistanceToNeighborArray[j]) * fixedPoint;
						if (fixedPoint2 < maxDistance)
						{
							cachedOpenList.Enqueue(coordinateNeighbor, fixedPoint2);
						}
					}
				}
			}
			while (cachedOpenList.Count > 0)
			{
				OpenListEntry openListEntry = cachedOpenList.Dequeue();
				GridCoordinate coordinate2 = openListEntry.coordinate;
				if (cachedVisited[coordinate2])
				{
					continue;
				}
				cachedVisited[coordinate2] = true;
				FixedPoint fixedPoint3 = (gridField[coordinate2] = openListEntry.distance);
				for (int k = 0; k < 8; k++)
				{
					GridCoordinate coordinateNeighbor2 = grid.GetCoordinateNeighbor(coordinate2, k);
					if (!coordinateNeighbor2.IsValid || cachedVisited[coordinateNeighbor2] || combatModel.BlockedCache[coordinateNeighbor2] || (combatModel.TraversableCache[coordinate2] & (1 << k)) == 0)
					{
						continue;
					}
					ActorModel occupier2 = combatModel.GetOccupier(coordinateNeighbor2);
					if (occupier2 == null || ignoreFactionOccupancy == null || !occupier2.IsEnemy(ignoreFactionOccupancy))
					{
						FixedPoint fixedPoint4 = ((occupier2 != null && occupier2 != ignoreActorOccupancy) ? occupancyCostMultiplier : ((FixedPoint)1L));
						FixedPoint fixedPoint5 = (UniformDistance ? ((FixedPoint)1L) : DistanceToNeighborArray[k]) * fixedPoint4;
						FixedPoint fixedPoint6 = fixedPoint3 + fixedPoint5;
						if (fixedPoint6 < maxDistance)
						{
							cachedOpenList.Enqueue(coordinateNeighbor2, fixedPoint6);
						}
					}
				}
			}
			return gridField;
		}

		public static FixedPoint GetDistance(GridModel grid, GridField<FixedPoint> distanceField, GridCoordinate coordinate)
		{
			if (coordinate.IsValid && distanceField[coordinate] == 0.0)
			{
				return 0L;
			}
			FixedPoint fixedPoint = FixedPoint.MaxValue;
			int num = -1;
			for (int i = 0; i < 8; i++)
			{
				GridCoordinate coordinateNeighbor = grid.GetCoordinateNeighbor(coordinate, i);
				if (grid.IsCoordinateValid(coordinateNeighbor))
				{
					FixedPoint fixedPoint2 = distanceField[coordinateNeighbor];
					if (fixedPoint2 < fixedPoint)
					{
						fixedPoint = fixedPoint2;
						num = i;
					}
				}
			}
			if (num < 0)
			{
				return FixedPoint.MaxValue;
			}
			return fixedPoint + DistanceToNeighbor(num);
		}
	}
}
