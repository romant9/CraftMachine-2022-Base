using System;
using System.Collections.Generic;

namespace TWDModel
{
	public class TWDObjectLocation
	{
		public List<GridCoordinate> Coordinates;

		public List<int> Edges;

		public int Width
		{
			get
			{
				if (Coordinates.Count == 0)
				{
					return 1;
				}
				int num = int.MaxValue;
				int num2 = int.MinValue;
				for (int i = 0; i < Coordinates.Count; i++)
				{
					num = Math.Min(num, Coordinates[i].X);
					num2 = Math.Max(num2, Coordinates[i].X);
				}
				return num2 - num + 1;
			}
		}

		public int Height
		{
			get
			{
				if (Coordinates.Count == 0)
				{
					return 1;
				}
				int num = int.MaxValue;
				int num2 = int.MinValue;
				for (int i = 0; i < Coordinates.Count; i++)
				{
					num = Math.Min(num, Coordinates[i].Y);
					num2 = Math.Max(num2, Coordinates[i].Y);
				}
				return num2 - num + 1;
			}
		}

		public bool IsEmpty
		{
			get
			{
				if (Coordinates.Count == 0)
				{
					return Edges.Count == 0;
				}
				return false;
			}
		}

		public GridCoordinate Coordinate
		{
			get
			{
				if (Coordinates.Count == 0)
				{
					return GridCoordinate.Invalid;
				}
				return Coordinates[0];
			}
		}

		public int Edge
		{
			get
			{
				if (Edges.Count == 0)
				{
					return -1;
				}
				return Edges[0];
			}
		}

		public bool Contains(GridCoordinate coordinate)
		{
			if (Coordinate.IsValid)
			{
				for (int i = 0; i < Coordinates.Count; i++)
				{
					if (Coordinates[i] == coordinate)
					{
						return true;
					}
				}
			}
			return false;
		}

		public TWDObjectLocation()
		{
			Coordinates = new List<GridCoordinate>();
			Edges = new List<int>();
		}

		public TWDObjectLocation(GridCoordinate c)
		{
			Coordinates = new List<GridCoordinate> { c };
			Edges = new List<int>();
		}

		public TWDObjectLocation(int edge)
		{
			Coordinates = new List<GridCoordinate>();
			Edges = new List<int> { edge };
		}

		public TWDObjectLocation(List<GridCoordinate> coordinates, List<int> edges)
		{
			Coordinates = new List<GridCoordinate>();
			Edges = new List<int>();
			if (coordinates != null)
			{
				Coordinates.AddRange(coordinates);
			}
			if (edges != null)
			{
				Edges.AddRange(edges);
			}
		}
	}
}
