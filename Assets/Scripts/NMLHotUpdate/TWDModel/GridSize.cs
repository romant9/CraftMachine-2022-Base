using System;

namespace TWDModel
{
	[Serializable]
	public class GridSize
	{
		public int X;

		public int Y;

		public GridSize()
		{
		}

		public GridSize(int x, int y)
		{
			X = x;
			Y = y;
		}
	}
}
