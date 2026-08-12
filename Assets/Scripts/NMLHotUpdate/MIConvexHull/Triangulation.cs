using System.Collections.Generic;
using System.Linq;

namespace MIConvexHull
{
	public static class Triangulation
	{
		public static ITriangulation<TVertex, DefaultTriangulationCell<TVertex>> CreateDelaunay<TVertex>(IEnumerable<TVertex> data) where TVertex : IVertex
		{
			return DelaunayTriangulation<TVertex, DefaultTriangulationCell<TVertex>>.Create(data);
		}

		public static ITriangulation<DefaultVertex, DefaultTriangulationCell<DefaultVertex>> CreateDelaunay(IEnumerable<double[]> data)
		{
			return DelaunayTriangulation<DefaultVertex, DefaultTriangulationCell<DefaultVertex>>.Create(data.Select((double[] p) => new DefaultVertex
			{
				Position = p.ToArray()
			}));
		}

		public static ITriangulation<TVertex, TFace> CreateDelaunay<TVertex, TFace>(IEnumerable<TVertex> data) where TVertex : IVertex where TFace : TriangulationCell<TVertex, TFace>, new()
		{
			return DelaunayTriangulation<TVertex, TFace>.Create(data);
		}
	}
}
