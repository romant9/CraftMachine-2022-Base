namespace MIConvexHull
{
	internal sealed class FaceConnector
	{
		public ConvexFaceInternal Face;

		public int EdgeIndex;

		public int[] Vertices;

		public uint HashCode;

		public FaceConnector Previous;

		public FaceConnector Next;

		public FaceConnector(int dimension)
		{
			Vertices = new int[dimension - 1];
		}

		public void Update(ConvexFaceInternal face, int edgeIndex, int dim)
		{
			Face = face;
			EdgeIndex = edgeIndex;
			uint num = 31u;
			VertexWrap[] vertices = face.Vertices;
			int i = 0;
			int num2 = 0;
			for (; i < dim; i++)
			{
				if (i != edgeIndex)
				{
					int index = vertices[i].Index;
					Vertices[num2++] = index;
					num += (uint)((int)(23 * num) + index);
				}
			}
			HashCode = num;
		}

		public static bool AreConnectable(FaceConnector a, FaceConnector b, int dim)
		{
			if (a.HashCode != b.HashCode)
			{
				return false;
			}
			int num = dim - 1;
			int[] vertices = a.Vertices;
			int[] vertices2 = b.Vertices;
			for (int i = 0; i < num; i++)
			{
				if (vertices[i] != vertices2[i])
				{
					return false;
				}
			}
			return true;
		}

		public static void Connect(FaceConnector a, FaceConnector b)
		{
			a.Face.AdjacentFaces[a.EdgeIndex] = b.Face;
			b.Face.AdjacentFaces[b.EdgeIndex] = a.Face;
		}
	}
}
