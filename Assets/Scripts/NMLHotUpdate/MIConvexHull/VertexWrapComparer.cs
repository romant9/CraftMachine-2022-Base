using System.Collections.Generic;

namespace MIConvexHull
{
	internal class VertexWrapComparer : IComparer<VertexWrap>
	{
		public static readonly VertexWrapComparer Instance = new VertexWrapComparer();

		public int Compare(VertexWrap x, VertexWrap y)
		{
			return x.Index.CompareTo(y.Index);
		}
	}
}
