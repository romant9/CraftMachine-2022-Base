using System.Collections;

namespace TWDModel
{
	public class GridCoordinateEnumerator : IEnumerator
	{
		public int width;

		public int height;

		private int currentX = -1;

		private int currentY;

		object IEnumerator.Current => Current;

		public GridCoordinate Current => new GridCoordinate(currentX, currentY);

		public GridCoordinateEnumerator(int width, int height)
		{
			this.width = width;
			this.height = height;
		}

		public bool MoveNext()
		{
			currentX++;
			if (currentX >= width)
			{
				currentX = 0;
				currentY++;
			}
			if (currentX < width)
			{
				return currentY < height;
			}
			return false;
		}

		public void Reset()
		{
			currentX = -1;
			currentY = 0;
		}
	}
}
