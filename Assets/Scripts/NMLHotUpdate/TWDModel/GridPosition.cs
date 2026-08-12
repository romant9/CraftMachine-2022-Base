namespace TWDModel
{
	public class GridPosition
	{
		public FixedPoint X;

		public FixedPoint Y;

		public GridPosition()
		{
		}

		public GridPosition(FixedPoint x, FixedPoint y)
		{
			X = x;
			Y = y;
		}

		public GridPosition(GridPosition other)
		{
			X = other.X;
			Y = other.Y;
		}

		public override string ToString()
		{
			string[] obj = new string[5] { "[x ", null, null, null, null };
			FixedPoint x = X;
			obj[1] = x.ToString();
			obj[2] = ", y ";
			x = Y;
			obj[3] = x.ToString();
			obj[4] = "]";
			return string.Concat(obj);
		}

		public static bool operator ==(GridPosition a, GridPosition b)
		{
			if ((object)a == null && (object)b == null)
			{
				return true;
			}
			if ((object)a == null || (object)b == null)
			{
				return false;
			}
			if (a.X == b.X)
			{
				return a.Y == b.Y;
			}
			return false;
		}

		public static bool operator !=(GridPosition a, GridPosition b)
		{
			return !(a == b);
		}

		public override bool Equals(object obj)
		{
			GridPosition gridPosition = obj as GridPosition;
			if (gridPosition == null)
			{
				return false;
			}
			return this == gridPosition;
		}

		public override int GetHashCode()
		{
			return (int)X ^ (int)Y;
		}
	}
}
