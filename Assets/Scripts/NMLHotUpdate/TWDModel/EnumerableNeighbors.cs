using System.Collections;

namespace TWDModel
{
	public class EnumerableNeighbors : IEnumerable
	{
		private int width;

		private int height;

		private GridCoordinate source;

		public EnumerableNeighbors(int width, int height, GridCoordinate source)
		{
			this.width = width;
			this.height = height;
			this.source = source;
		}

		public IEnumerator GetEnumerator()
		{
			return new GridNeighborEnumerator(width, height, source);
		}
	}
}
