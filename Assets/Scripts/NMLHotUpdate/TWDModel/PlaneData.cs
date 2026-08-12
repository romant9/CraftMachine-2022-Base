using System;

namespace TWDModel
{
	[Serializable]
	public struct PlaneData
	{
		public FixedPoint Distance;

		public FixedVec3 Normal;

		public PlaneData(FixedVec3 normal, FixedPoint distance)
		{
			Distance = distance;
			Normal = normal;
		}

		public IntersectionResult TestPoint(FixedVec3 position)
		{
			FixedPoint fixedPoint = FixedVec3.Dot(position, Normal) - Distance;
			if (fixedPoint > 0.0)
			{
				return IntersectionResult.Positive;
			}
			if (fixedPoint < 0.0)
			{
				return IntersectionResult.Negative;
			}
			return IntersectionResult.Intersect;
		}
	}
}
