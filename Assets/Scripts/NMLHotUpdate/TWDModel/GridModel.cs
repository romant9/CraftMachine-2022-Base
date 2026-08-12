using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TWDModel
{
	public class GridModel : TWDModelObject
	{
		public static int[] NeighborOffsetX = new int[8] { 0, 1, 1, 1, 0, -1, -1, -1 };

		public static int[] NeighborOffsetY = new int[8] { -1, -1, 0, 1, 1, 1, 0, -1 };

		public int Width { get; private set; }

		public int Height { get; private set; }

		public FixedVec2 CellSize { get; private set; }

		public FixedVec3 Position { get; private set; }

		[JsonIgnore]
		public EnumerableGrid Coordinates => new EnumerableGrid(Width, Height);

		public int NumCells => Width * Height;

		public GridModel()
		{
			Position = new FixedVec3(0.0, 0.0, 0.0);
			CellSize = new FixedVec2(1.0, 1.0);
			Width = 4;
			Height = 4;
		}

		public GridModel(GridModel copyFrom)
		{
			Width = copyFrom.Width;
			Height = copyFrom.Height;
			CellSize = copyFrom.CellSize;
			Position = copyFrom.Position;
		}

		public override void Initialize()
		{
			base.Initialize();
		}

		public EnumerableNeighbors Neighbors(GridCoordinate source)
		{
			return new EnumerableNeighbors(Width, Height, source);
		}

		public void SetPosition(FixedVec3 inPosition)
		{
			Position = inPosition;
		}

		public void SetCellSize(FixedVec2 inCellSize)
		{
			CellSize = inCellSize;
		}

		public FixedVec2 GetCellSize()
		{
			return CellSize;
		}

		public void SetWidth(int inWidth)
		{
			Width = inWidth;
		}

		public void SetHeight(int inHeight)
		{
			Height = inHeight;
		}

		public override bool IsValid()
		{
			return NumCells > 0;
		}

		public static GridCoordinate GetCoordinateNeighbor(GridCoordinate coordinate, int index, int inWidth, int inHeight)
		{
			GridCoordinate invalid = GridCoordinate.Invalid;
			int num = coordinate.X + NeighborOffsetX[index];
			int num2 = coordinate.Y + NeighborOffsetY[index];
			if (num >= 0 && num < inWidth && num2 >= 0 && num2 < inHeight)
			{
				invalid.Set(num, num2);
			}
			return invalid;
		}

		public static int GetCoordinateNeighborIndex(GridCoordinate coordinate1, GridCoordinate coordinate2, int inWidth, int inHeight)
		{
			int num = coordinate2.X - coordinate1.X;
			int num2 = coordinate2.Y - coordinate1.Y;
			for (int i = 0; i < 8; i++)
			{
				if (NeighborOffsetX[i] == num && NeighborOffsetY[i] == num2)
				{
					return i;
				}
			}
			return -1;
		}

		public GridCoordinate GetCoordinateNeighbor(GridCoordinate coordinate, int index)
		{
			return GetCoordinateNeighbor(coordinate, index, Width, Height);
		}

		public int GetCoordinateNeighborIndex(GridCoordinate coordinate1, GridCoordinate coordinate2)
		{
			return GetCoordinateNeighborIndex(coordinate1, coordinate2, Width, Height);
		}

		public int GetCoordinateOffset(GridCoordinate coordinate)
		{
			return GetCoordinateOffset(coordinate, Width);
		}

		public static int GetCoordinateOffset(GridCoordinate coordinate, int width)
		{
			return coordinate.Y * width + coordinate.X;
		}

		public bool IsCoordinateValid(GridCoordinate coordinate)
		{
			if (coordinate.X >= 0 && coordinate.X < Width && coordinate.Y >= 0)
			{
				return coordinate.Y < Height;
			}
			return false;
		}

		private GridCoordinate CreateCoordinate(int offset)
		{
			return CreateCoordinate(offset, Width);
		}

		private static GridCoordinate CreateCoordinate(int offset, int width)
		{
			if (width <= 0)
			{
				return GridCoordinate.Invalid;
			}
			return new GridCoordinate(offset % width, offset / width);
		}

		public bool AreNeighbors(GridCoordinate from, GridCoordinate to)
		{
			if (Math.Abs(from.Y - to.Y) <= 1)
			{
				return Math.Abs(from.X - to.X) <= 1;
			}
			return false;
		}

		public GridCoordinate GetCoordinate(FixedVec3 position)
		{
			FixedVec3 fixedVec = position - Position;
			FixedVec2 fixedVec2 = new FixedVec2(fixedVec.X / CellSize.X, -fixedVec.Z / CellSize.Y);
			return new GridCoordinate((int)FixedPoint.Round(fixedVec2.X - 0.5), (int)FixedPoint.Round(fixedVec2.Y - 0.5));
		}

		public GridCoordinate GetCoordinate(int offset)
		{
			if (offset < 0 || offset >= NumCells)
			{
				return GridCoordinate.Invalid;
			}
			return new GridCoordinate(offset % Width, offset / Width);
		}

		public FixedVec3 GetPosition(GridCoordinate coordinate)
		{
			FixedPoint x = CellSize.X;
			FixedPoint y = CellSize.Y;
			return new FixedVec3(coordinate.X * x + x * 0.5, 0.0, -coordinate.Y * y - y * 0.5) + Position;
		}

		public FixedPoint GetAngle(ActorModel source, ActorModel target)
		{
			FixedVec3 forwardDirection = target.ForwardDirection;
			FixedVec3 position = GetPosition(source.GridCoordinate);
			FixedVec3 position2 = GetPosition(target.GridCoordinate);
			FixedVec3 b = FixedVec3.Normalize(position - position2);
			return FixedVec3.AngleDegrees(forwardDirection, b);
		}

		public void GetCellBounds(GridCoordinate coordinate, out FixedVec3 minPosition, out FixedVec3 maxPosition)
		{
			FixedPoint x = CellSize.X;
			FixedPoint y = CellSize.Y;
			minPosition = new FixedVec3(coordinate.X * x, 0.0, -(coordinate.Y + 1) * y) + Position;
			maxPosition = new FixedVec3((coordinate.X + 1) * x, 0.0, -coordinate.Y * y) + Position;
		}

		public static int GetEdgeID(int gridCoordinateOffsetA, int gridCoordinateOffsetB)
		{
			return (gridCoordinateOffsetA << 16) + gridCoordinateOffsetB;
		}

		public static void GetCoordinatesFromEdge(int edgeId, out GridCoordinate a, out GridCoordinate b, int width)
		{
			a = CreateCoordinate((edgeId >> 16) & 0xFFFF, width);
			b = CreateCoordinate(edgeId & 0xFFFF, width);
		}

		public void GetCoordinatesFromEdge(int edgeId, out GridCoordinate a, out GridCoordinate b)
		{
			a = CreateCoordinate((edgeId >> 16) & 0xFFFF);
			b = CreateCoordinate(edgeId & 0xFFFF);
		}

		public static List<GridCoordinate> GetLineCoordinates(GridCoordinate from, GridCoordinate to)
		{
			List<GridCoordinate> list = new List<GridCoordinate>();
			int num = from.X;
			int num2 = from.Y;
			int x = to.X;
			int y = to.Y;
			int num3 = Math.Abs(x - num);
			int num4 = ((num < x) ? 1 : (-1));
			int num5 = Math.Abs(y - num2);
			int num6 = ((num2 < y) ? 1 : (-1));
			int num7 = ((num3 > num5) ? num3 : (-num5)) / 2;
			while (true)
			{
				list.Add(new GridCoordinate(num, num2));
				if (num == x && num2 == y)
				{
					break;
				}
				int num8 = num7;
				if (num8 > -num3)
				{
					num7 -= num5;
					num += num4;
				}
				if (num8 < num5)
				{
					num7 += num3;
					num2 += num6;
				}
			}
			return list;
		}
	}
}
