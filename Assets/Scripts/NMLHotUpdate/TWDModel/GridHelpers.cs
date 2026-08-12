using System.Collections.Generic;

namespace TWDModel
{
	public class GridHelpers
	{
		public static GridPath GetPathTowardsTarget(ActorModel mover, CombatModel combatModel, GridCoordinate targetCoordinate, int maxPathLength = 100)
		{
			GridCoordinate gridCoordinate = mover.GridCoordinate;
			GridPath gridPath = GridPath.Create();
			GridModel grid = combatModel.Grid;
			GridCoordinate gridCoordinate2 = targetCoordinate;
			if (combatModel.GetOccupier(targetCoordinate) != null)
			{
				GridField<FixedPoint> gridField = DistanceField.CreateDistanceField(combatModel, mover.GridCoordinate, new DistanceFieldOptions(1f, mover, mover));
				FixedPoint fixedPoint = FixedPoint.MaxValue;
				for (int i = 0; i < 8; i++)
				{
					GridCoordinate coordinateNeighbor = GridModel.GetCoordinateNeighbor(targetCoordinate, i, grid.Width, grid.Height);
					if (!grid.IsCoordinateValid(coordinateNeighbor))
					{
						continue;
					}
					bool flag = combatModel.GetOccupier(coordinateNeighbor) != null;
					bool num = combatModel.IsBlocked(coordinateNeighbor);
					bool flag2 = combatModel.CanTraverse(null, targetCoordinate, coordinateNeighbor);
					if (!num && !flag && flag2)
					{
						FixedPoint fixedPoint2 = gridField[coordinateNeighbor];
						if (fixedPoint2 < fixedPoint)
						{
							gridCoordinate2 = coordinateNeighbor;
							fixedPoint = fixedPoint2;
						}
					}
				}
			}
			GridField<FixedPoint> gridField2 = DistanceField.CreateDistanceField(combatModel, gridCoordinate2, new DistanceFieldOptions(1f, mover, mover));
			GridField<bool> gridField3 = new GridField<bool>(combatModel.Grid.Width, combatModel.Grid.Height, defaultValue: false);
			Queue<GridCoordinate> queue = new Queue<GridCoordinate>();
			queue.Enqueue(gridCoordinate);
			gridField3[gridCoordinate] = true;
			gridPath.AddNode(gridCoordinate);
			FixedPoint fixedPoint3 = ((combatModel.GetOccupier(gridCoordinate2) != null && combatModel.IsBlocked(gridCoordinate2)) ? 1.5f : 0f);
			while (queue.Count > 0)
			{
				GridCoordinate gridCoordinate3 = queue.Dequeue();
				bool flag3 = gridCoordinate3 != gridCoordinate && (combatModel.IsBlocked(gridCoordinate3) || combatModel.GetOccupier(gridCoordinate3) != null);
				GridCoordinate gridCoordinate4 = GridCoordinate.Invalid;
				FixedPoint fixedPoint4 = FixedPoint.MinValue;
				bool flag4 = gridPath.Length >= maxPathLength - 1;
				foreach (GridCoordinate item in grid.Neighbors(gridCoordinate3))
				{
					bool flag5 = combatModel.GetOccupier(item) != null && flag4;
					if (!gridField3[item] && combatModel.CanTraverse(mover, gridCoordinate3, item) && !combatModel.IsBlocked(item) && !flag5)
					{
						FixedPoint fixedPoint5 = gridField2[gridCoordinate3] - gridField2[item] + ((combatModel.GetOccupier(item) == null) ? 0.1f : 0f);
						if ((fixedPoint5 > 0L && fixedPoint5 > fixedPoint4) || (flag3 && !gridCoordinate4.IsValid))
						{
							fixedPoint4 = fixedPoint5;
							gridCoordinate4 = item;
						}
						gridField3[item] = true;
					}
				}
				if (gridCoordinate4.IsValid)
				{
					gridPath.AddNode(gridCoordinate4);
					if ((combatModel.GetOccupier(gridCoordinate4) == null && gridField2[gridCoordinate4] <= fixedPoint3) || gridPath.Length >= maxPathLength)
					{
						break;
					}
					queue.Enqueue(gridCoordinate4);
				}
			}
			while (gridPath.Length > 0 && combatModel.GetOccupier(gridPath.End) != null)
			{
				gridPath.RemoveLast();
			}
			return gridPath;
		}
	}
}
