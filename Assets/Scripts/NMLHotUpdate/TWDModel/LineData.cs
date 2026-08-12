using System;

namespace TWDModel
{
	[Serializable]
	public struct LineData
	{
		public FixedVec3 End0;

		public FixedVec3 End1;

		public LineData(FixedVec3 end0, FixedVec3 end1)
		{
			End0 = end0;
			End1 = end1;
		}

		private static bool InternalComputeLineIntersection(LineData line0, LineData line1, out FixedVec3 intersection)
		{
			FixedVec3 end = line0.End0;
			FixedVec3 end2 = line1.End0;
			FixedVec3 fixedVec = line0.End1 - line0.End0;
			FixedVec3 arg = line1.End1 - line1.End0;
			intersection = FixedVec3.Zero;
			FixedVec3 fixedVec2 = end2 - end;
			FixedVec3 b = FixedVec3.Cross(fixedVec, arg);
			FixedVec3 a = FixedVec3.Cross(fixedVec2, arg);
			FixedPoint fixedPoint = FixedVec3.Dot(fixedVec2, b);
			if (fixedPoint >= 9.999999747378752E-06 || fixedPoint <= -9.999999747378752E-06)
			{
				return false;
			}
			FixedPoint fixedPoint2 = FixedVec3.Dot(a, b) / b.SqrMagnitude;
			if (fixedPoint2 >= 0.0 && fixedPoint2 <= 1.0)
			{
				intersection = end + fixedVec * fixedPoint2;
				return true;
			}
			return false;
		}

		public static bool CalculateIntersection(LineData line0, LineData line1, out FixedVec3 intersection)
		{
			if (InternalComputeLineIntersection(line0, line1, out intersection))
			{
				return InternalComputeLineIntersection(line1, line0, out intersection);
			}
			return false;
		}

		public FixedPoint CalculateDistanceTo(FixedVec3 point)
		{
			FixedVec3 fixedVec = End1 - End0;
			FixedPoint magnitude = fixedVec.Magnitude;
			fixedVec /= magnitude;
			FixedVec3 fixedVec2 = point - End0;
			FixedPoint fixedPoint = FixedVec3.Dot(fixedVec, fixedVec2);
			if (fixedPoint < 0.0)
			{
				return (point - End0).Magnitude;
			}
			if (fixedPoint > magnitude)
			{
				return (point - End1).Magnitude;
			}
			return (fixedVec2 - fixedVec * fixedPoint).Magnitude;
		}
	}
}
