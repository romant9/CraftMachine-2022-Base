using System.Collections;

namespace TWDModel
{
	public class EnumerableGrid : IEnumerable
	{
		private int width;

		private int height;

		public EnumerableGrid(int width, int height)
		{
			this.width = width;
			this.height = height;
		}

		public IEnumerator GetEnumerator()
		{
			return new GridCoordinateEnumerator(width, height);
		}
	}
}
