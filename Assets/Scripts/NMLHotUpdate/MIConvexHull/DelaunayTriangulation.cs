using System;
using System.Collections.Generic;
using System.Linq;

namespace MIConvexHull
{
	public class DelaunayTriangulation<TVertex, TCell> : ITriangulation<TVertex, TCell> where TVertex : IVertex where TCell : TriangulationCell<TVertex, TCell>, new()
	{
		public IEnumerable<TCell> Cells { get; private set; }

		public static DelaunayTriangulation<TVertex, TCell> Create(IEnumerable<TVertex> data)
		{
			if (data == null)
			{
				throw new ArgumentException("data can't be null.");
			}
			if (!(data is IList<TVertex>))
			{
				data = data.ToArray();
			}
			if (data.Count() == 0)
			{
				return new DelaunayTriangulation<TVertex, TCell>
				{
					Cells = Enumerable.Empty<TCell>()
				};
			}
			int num = data.First().Position.Length;
			foreach (TVertex datum in data)
			{
				TVertex current = datum;
				double num2 = MathHelper.LengthSquared(current.Position);
				double[] array = current.Position;
				Array.Resize(ref array, num + 1);
				double[] position = array;
				current.Position = position;
				current.Position[num] = num2;
			}
			List<ConvexFaceInternal> convexFacesInternal = ConvexHullInternal.GetConvexFacesInternal<TVertex, TCell>(data);
			foreach (TVertex datum2 in data)
			{
				TVertex current2 = datum2;
				double[] array2 = current2.Position;
				Array.Resize(ref array2, num);
				double[] position2 = array2;
				current2.Position = position2;
			}
			for (int num3 = convexFacesInternal.Count - 1; num3 >= 0; num3--)
			{
				ConvexFaceInternal convexFaceInternal = convexFacesInternal[num3];
				if (convexFaceInternal.Normal[num] >= 0.0)
				{
					for (int i = 0; i < convexFaceInternal.AdjacentFaces.Length; i++)
					{
						ConvexFaceInternal convexFaceInternal2 = convexFaceInternal.AdjacentFaces[i];
						if (convexFaceInternal2 == null)
						{
							continue;
						}
						for (int j = 0; j < convexFaceInternal2.AdjacentFaces.Length; j++)
						{
							if (convexFaceInternal2.AdjacentFaces[j] == convexFaceInternal)
							{
								convexFaceInternal2.AdjacentFaces[j] = null;
							}
						}
					}
					int index = convexFacesInternal.Count - 1;
					convexFacesInternal[num3] = convexFacesInternal[index];
					convexFacesInternal.RemoveAt(index);
				}
			}
			int count = convexFacesInternal.Count;
			TCell[] array3 = new TCell[count];
			for (int k = 0; k < count; k++)
			{
				ConvexFaceInternal convexFaceInternal3 = convexFacesInternal[k];
				TVertex[] array4 = new TVertex[num + 1];
				for (int l = 0; l <= num; l++)
				{
					array4[l] = (TVertex)convexFaceInternal3.Vertices[l].Vertex;
				}
				array3[k] = new TCell
				{
					Vertices = array4,
					Adjacency = new TCell[num + 1]
				};
				convexFaceInternal3.Tag = k;
			}
			for (int m = 0; m < count; m++)
			{
				ConvexFaceInternal convexFaceInternal4 = convexFacesInternal[m];
				TCell val = array3[m];
				for (int n = 0; n <= num; n++)
				{
					if (convexFaceInternal4.AdjacentFaces[n] != null)
					{
						val.Adjacency[n] = array3[convexFaceInternal4.AdjacentFaces[n].Tag];
					}
				}
			}
			return new DelaunayTriangulation<TVertex, TCell>
			{
				Cells = array3
			};
		}

		private DelaunayTriangulation()
		{
		}
	}
}
