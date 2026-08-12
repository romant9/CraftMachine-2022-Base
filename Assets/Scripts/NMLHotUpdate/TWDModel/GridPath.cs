using System;
using System.Collections.Generic;

namespace TWDModel
{
	[Serializable]
	public class GridPath
	{
		public int Length
		{
			get
			{
				if (Path == null)
				{
					return 0;
				}
				return Path.Count;
			}
		}

		public FixedPoint MoveDistance
		{
			get
			{
				FixedPoint result = 0.0;
				if (Path != null)
				{
					for (int i = 0; i < Path.Count - 1; i++)
					{
						result += Path[i].DistanceTo(Path[i + 1]);
					}
				}
				return result;
			}
		}

		public bool IsValid
		{
			get
			{
				if (Path != null)
				{
					return Path.Count > 1;
				}
				return false;
			}
		}

		public GridCoordinate Start
		{
			get
			{
				if (Path == null || Path.Count <= 0)
				{
					return GridCoordinate.Invalid;
				}
				return Path[0];
			}
		}

		public GridCoordinate End
		{
			get
			{
				if (Path == null || Path.Count <= 0)
				{
					return GridCoordinate.Invalid;
				}
				return Path[Path.Count - 1];
			}
		}

		public List<GridCoordinate> EndPair
		{
			get
			{
				if (HasTargetCoordinate && Path != null && Path.Count > 0)
				{
					return new List<GridCoordinate>
					{
						Path[Path.Count - 1],
						TargetCoordinate
					};
				}
				if (Path != null && Path.Count > 1)
				{
					return new List<GridCoordinate>
					{
						Path[Path.Count - 2],
						Path[Path.Count - 1]
					};
				}
				return null;
			}
		}

		public GridCoordinate TargetCoordinate { get; set; }

		public bool HasTargetCoordinate => TargetCoordinate.IsValid;

		public bool IsDanger { get; set; }

		public GridCoordinate this[int index]
		{
			get
			{
				if (Path == null)
				{
					return GridCoordinate.Invalid;
				}
				return Path[index];
			}
		}

		public int Count
		{
			get
			{
				if (Path == null)
				{
					return 0;
				}
				return Path.Count;
			}
		}

		public List<GridCoordinate> Path { get; set; }

		public static GridPath Create()
		{
			return new GridPath
			{
				TargetCoordinate = GridCoordinate.Invalid
			};
		}

		public static GridPath Create(List<GridCoordinate> inPath)
		{
			GridPath gridPath = Create();
			gridPath.Path = inPath;
			return gridPath;
		}

		public static GridPath Create(GridPath gridPath)
		{
			GridPath gridPath2 = Create();
			if (gridPath.Path != null)
			{
				gridPath2.Path = new List<GridCoordinate>();
				if (gridPath.Count > 0)
				{
					gridPath2.Path.AddRange(gridPath.Path);
				}
			}
			gridPath2.TargetCoordinate = gridPath.TargetCoordinate;
			gridPath2.IsDanger = gridPath.IsDanger;
			return gridPath2;
		}

		public FixedPoint GetMoveDistanceAt(int index)
		{
			FixedPoint result = 0.0;
			for (int i = 1; i < index && i < Path.Count; i++)
			{
				result += Path[i - 1].DistanceTo(Path[i]);
			}
			return result;
		}

		public void RemoveLast()
		{
			if (Path.Count > 0)
			{
				Path.RemoveAt(Path.Count - 1);
			}
		}

		public void AddNode(GridCoordinate coordinate)
		{
			if (Path == null)
			{
				Path = new List<GridCoordinate>();
			}
			Path.Add(coordinate);
		}

		public bool StartsFrom(GridCoordinate coordinate)
		{
			if (Path != null && Path.Count > 0)
			{
				return Path[0] == coordinate;
			}
			return false;
		}

		public bool EndsAt(GridCoordinate coordinate)
		{
			if (Path != null && Path.Count > 0)
			{
				return Path[Path.Count - 1] == coordinate;
			}
			return false;
		}

		public bool Contains(GridCoordinate coordinate)
		{
			for (int i = 0; i < Path.Count; i++)
			{
				if (Path[i] == coordinate)
				{
					return true;
				}
			}
			return false;
		}

		public void Append(GridPath path)
		{
			for (int i = 0; i < path.Length; i++)
			{
				if (!Contains(path[i]))
				{
					AddNode(path[i]);
				}
			}
		}

		public void ClipTo(GridCoordinate coordinate)
		{
			int num = -1;
			for (int i = 0; i < Path.Count; i++)
			{
				if (Path[i] == coordinate)
				{
					num = i;
					break;
				}
			}
			if (num >= 0)
			{
				Path.RemoveRange(num + 1, Path.Count - (num + 1));
			}
		}

		public void ClipFromStartUntil(GridCoordinate coordinate)
		{
			int num = -1;
			for (int i = 0; i < Path.Count; i++)
			{
				if (Path[i] == coordinate)
				{
					num = i;
					break;
				}
			}
			if (num >= 0)
			{
				Path.RemoveRange(0, num);
			}
		}

		public void Invalidate()
		{
			Path = null;
			TargetCoordinate = GridCoordinate.Invalid;
			IsDanger = false;
		}

		public void Clear()
		{
			if (Path == null)
			{
				Path = new List<GridCoordinate>();
			}
			Path.Clear();
			TargetCoordinate = GridCoordinate.Invalid;
			IsDanger = false;
		}

		public void ClearTargetCoordinate()
		{
			TargetCoordinate = GridCoordinate.Invalid;
		}

		public override string ToString()
		{
			if (Count <= 0)
			{
				return "Path is empty";
			}
			string text = "Path = { ";
			for (int i = 0; i < Path.Count; i++)
			{
				text = text + Path[i].ToString() + ", ";
			}
			return text + " }, Target = " + TargetCoordinate.ToString();
		}
	}
}
