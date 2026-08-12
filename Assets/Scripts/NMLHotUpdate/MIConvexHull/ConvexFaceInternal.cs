namespace MIConvexHull
{
	internal sealed class ConvexFaceInternal
	{
		public ConvexFaceInternal[] AdjacentFaces;

		public VertexBuffer VerticesBeyond;

		public VertexWrap FurthestVertex;

		public VertexWrap[] Vertices;

		public double[] Normal;

		public bool IsNormalFlipped;

		public double Offset;

		public int Tag;

		public ConvexFaceInternal Previous;

		public ConvexFaceInternal Next;

		public bool InList;

		public ConvexFaceInternal(int dimension, VertexBuffer beyondList)
		{
			AdjacentFaces = new ConvexFaceInternal[dimension];
			VerticesBeyond = beyondList;
			Normal = new double[dimension];
			Vertices = new VertexWrap[dimension];
		}
	}
}
