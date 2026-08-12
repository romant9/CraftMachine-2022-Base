using System.Collections;

namespace TWDModel
{
	public class GridNeighborEnumerator : IEnumerator
	{
		private int width;

		private int height;

		private GridCoordinate source;

		private GridCoordinate currentNeighbor = GridCoordinate.Invalid;

		private int currentNeighborIndex = -1;

		object IEnumerator.Current => Current;

		public GridCoordinate Current => currentNeighbor;

		public GridNeighborEnumerator(int width, int height, GridCoordinate source)
		{
			this.width = width;
			this.height = height;
			this.source = source;
		}

		public bool MoveNext()
		{
			int num;
			int num2;
			do
			{
				currentNeighborIndex++;
				if (currentNeighborIndex > 7)
				{
					return false;
				}
				currentNeighbor = GridCoordinate.Invalid;
				num = source.X + GridModel.NeighborOffsetX[currentNeighborIndex];
				num2 = source.Y + GridModel.NeighborOffsetY[currentNeighborIndex];
			}
			while (num < 0 || num >= width || num2 < 0 || num2 >= height);
			currentNeighbor.Set(num, num2);
			return true;
		}

		public void Reset()
		{
			currentNeighborIndex = -1;
			currentNeighbor = GridCoordinate.Invalid;
		}
	}
}
