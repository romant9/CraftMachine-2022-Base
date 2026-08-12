namespace TWDModel
{
	public class GeometryMath
	{
		public static FixedPoint LineLineDistance(FixedVec3 startA, FixedVec3 endA, FixedVec3 startB, FixedVec3 endB, bool isSegmentA, bool isSegmentB, out FixedPoint t, out FixedPoint s)
		{
			FixedVec3 fixedVec = endA - startA;
			FixedVec3 fixedVec2 = endB - startB;
			FixedVec3 fixedVec3 = startA - startB;
			FixedPoint fixedPoint = fixedVec * fixedVec;
			FixedPoint fixedPoint2 = fixedVec * fixedVec2;
			FixedPoint fixedPoint3 = fixedVec2 * fixedVec2;
			FixedPoint fixedPoint4 = fixedVec * fixedVec3;
			FixedPoint fixedPoint5 = fixedVec2 * fixedVec3;
			FixedPoint fixedPoint6 = fixedPoint * fixedPoint3 - fixedPoint2 * fixedPoint2;
			if (fixedPoint6 == 0.0)
			{
				t = 0.0;
				s = ((fixedPoint2 > fixedPoint3) ? (fixedPoint4 / fixedPoint2) : (fixedPoint5 / fixedPoint3));
			}
			else
			{
				t = (fixedPoint2 * fixedPoint5 - fixedPoint3 * fixedPoint4) / fixedPoint6;
				s = (fixedPoint * fixedPoint5 - fixedPoint2 * fixedPoint4) / fixedPoint6;
			}
			if (isSegmentA)
			{
				t = FixedPoint.Clamp(t, 0.0, fixedVec.Magnitude);
			}
			if (isSegmentB)
			{
				s = FixedPoint.Clamp(s, 0.0, fixedVec3.Magnitude);
			}
			FixedVec3 fixedVec4 = startA + fixedVec * t;
			return (startB + fixedVec2 * s - fixedVec4).Magnitude;
		}

		public static bool Intersect(FixedVec2 startA, FixedVec2 directionA, FixedVec2 startB, FixedVec2 directionB, out FixedVec2 intersection)
		{
			intersection = default(FixedVec2);
			FixedVec2 arg = startB - startA;
			FixedPoint fixedPoint = FixedVec2.Cross(directionA, directionB);
			if (fixedPoint != 0.0)
			{
				intersection = startA + directionA * (FixedVec2.Cross(arg, directionB) / fixedPoint);
				return true;
			}
			return false;
		}

		public static bool Intersect(FixedVec3 lineStart, FixedVec3 lineEnd, FixedVec3 a, FixedVec3 b, FixedVec3 c)
		{
			if (Intersect(lineStart, lineEnd - lineStart, a, b, c, out var intersection))
			{
				return FixedVec3.DistanceSquared(lineStart, intersection) <= FixedVec3.DistanceSquared(lineStart, lineEnd);
			}
			return false;
		}

		public static bool Intersect(FixedVec3 rayStart, FixedVec3 rayDir, FixedVec3 a, FixedVec3 b, FixedVec3 c, out FixedVec3 intersection)
		{
			intersection = new FixedVec3(0.0, 0.0, 0.0);
			FixedVec3 fixedVec = b - a;
			FixedVec3 fixedVec2 = c - a;
			FixedVec3 fixedVec3 = FixedVec3.Cross(rayDir, fixedVec2);
			FixedPoint fixedPoint = fixedVec * fixedVec3;
			if (fixedPoint == 0.0)
			{
				return false;
			}
			FixedVec3 fixedVec4 = rayStart - a;
			FixedPoint fixedPoint2 = fixedVec4 * fixedVec3 / fixedPoint;
			if (fixedPoint2 < 0.0 || fixedPoint2 > 1.0)
			{
				return false;
			}
			FixedVec3 fixedVec5 = FixedVec3.Cross(fixedVec4, fixedVec);
			FixedPoint fixedPoint3 = rayDir * fixedVec5 / fixedPoint;
			if (fixedPoint3 < 0.0 || fixedPoint2 + fixedPoint3 > 1.0)
			{
				return false;
			}
			FixedPoint fixedPoint4 = fixedVec2 * fixedVec5 / fixedPoint;
			if (fixedPoint4 < 0.0)
			{
				return false;
			}
			intersection = rayStart + rayDir * fixedPoint4;
			return true;
		}

		public static bool Contains(FixedVec3 point, Matrix44Data obbTransform, FixedVec3 obbCenter, FixedVec3 obbHalfSize)
		{
			FixedVec3 fixedVec = FixedVec3.Abs(Matrix44Data.Invert(obbTransform) * point - obbCenter);
			if (fixedVec.X <= obbHalfSize.X && fixedVec.Y <= obbHalfSize.Y)
			{
				return fixedVec.Z <= obbHalfSize.Z;
			}
			return false;
		}

		public static bool IntersectOBB(FixedVec3 rayStart, FixedVec3 rayDir, Matrix44Data obbTransform, FixedVec3 obbCenter, FixedVec3 obbHalfSize, out FixedVec3 collisionPoint)
		{
			Matrix44Data matrix44Data = Matrix44Data.Invert(obbTransform);
			FixedVec3 fixedVec = matrix44Data * rayStart;
			FixedVec3 fixedVec2 = Matrix44Data.Rotate(matrix44Data, rayDir);
			FixedVec3 min = obbCenter - obbHalfSize;
			FixedVec3 max = obbCenter + obbHalfSize;
			FixedPoint tmin = 0.0;
			FixedPoint tmax = 0.0;
			if (IntersectAABB(fixedVec, fixedVec2, min, max, out tmin, out tmax))
			{
				FixedVec3 fixedVec3 = fixedVec + fixedVec2 * tmin;
				collisionPoint = obbTransform * fixedVec3;
				return true;
			}
			collisionPoint = default(FixedVec3);
			return false;
		}

		public static bool IntersectAABB(FixedVec3 rayStart, FixedVec3 rayDir, FixedVec3 min, FixedVec3 max, out FixedPoint tmin, out FixedPoint tmax)
		{
			tmin = (tmax = 0.0);
			FixedVec3 fixedVec = new FixedVec3(1.0 / rayDir.X, 1.0 / rayDir.Y, 1.0 / rayDir.Z);
			FixedPoint fixedPoint = (((fixedVec.X < 0.0) ? max.X : min.X) - rayStart.X) * fixedVec.X;
			FixedPoint fixedPoint2 = (((fixedVec.X < 0.0) ? min.X : max.X) - rayStart.X) * fixedVec.X;
			FixedPoint fixedPoint3 = (((fixedVec.Y < 0.0) ? max.Y : min.Y) - rayStart.Y) * fixedVec.Y;
			FixedPoint fixedPoint4 = (((fixedVec.Y < 0.0) ? min.Y : max.Y) - rayStart.Y) * fixedVec.Y;
			if (fixedPoint > fixedPoint4 || fixedPoint3 > fixedPoint2)
			{
				return false;
			}
			if (fixedPoint3 > fixedPoint)
			{
				fixedPoint = fixedPoint3;
			}
			if (fixedPoint4 < fixedPoint2)
			{
				fixedPoint2 = fixedPoint4;
			}
			FixedPoint fixedPoint5 = (((fixedVec.Z < 0.0) ? max.Z : min.Z) - rayStart.Z) * fixedVec.Z;
			FixedPoint fixedPoint6 = (((fixedVec.Z < 0.0) ? min.Z : max.Z) - rayStart.Z) * fixedVec.Z;
			if (fixedPoint > fixedPoint6 || fixedPoint5 > fixedPoint2)
			{
				return false;
			}
			if (fixedPoint5 > fixedPoint)
			{
				fixedPoint = fixedPoint5;
			}
			if (fixedPoint6 < fixedPoint2)
			{
				fixedPoint2 = fixedPoint6;
			}
			if (fixedPoint < 0.0 && fixedPoint2 < 0.0)
			{
				return false;
			}
			tmin = fixedPoint;
			tmax = fixedPoint2;
			return true;
		}

		public static bool GetCircumcircle(FixedVec2 a, FixedVec2 b, FixedVec2 c, out FixedVec2 center, out FixedPoint radius)
		{
			FixedVec2 arg = b - a;
			FixedVec2 arg2 = c - b;
			FixedVec2 fixedVec = a - c;
			if (Intersect((a + b) * 0.5, startB: (b + c) * 0.5, directionA: FixedVec2.Cross(arg), directionB: FixedVec2.Cross(arg2), intersection: out center))
			{
				FixedPoint magnitude = arg.Magnitude;
				FixedPoint magnitude2 = arg2.Magnitude;
				FixedPoint magnitude3 = fixedVec.Magnitude;
				FixedPoint fixedPoint = FixedPoint.Sqrt((magnitude + magnitude2 + magnitude3) * (magnitude2 + magnitude3 - magnitude) * (magnitude + magnitude3 - magnitude2) * (magnitude + magnitude2 - magnitude3));
				if (fixedPoint > 0.0)
				{
					radius = magnitude * magnitude2 * magnitude3 / fixedPoint;
					return true;
				}
			}
			radius = 0.0;
			return false;
		}

		public static bool PointInCircumcircle(FixedVec2 point, FixedVec2 a, FixedVec2 b, FixedVec2 c)
		{
			if (!GetCircumcircle(a, b, c, out var center, out var radius))
			{
				return true;
			}
			return FixedVec2.Distance(point, center) <= radius;
		}

		public static bool GetBarycentricCoordinates(FixedVec2 point, FixedVec2 a, FixedVec2 b, FixedVec2 c, out FixedPoint t, out FixedPoint u, out FixedPoint v)
		{
			FixedPoint fixedPoint = (b.Y - c.Y) * (a.X - c.X) + (c.X - b.X) * (a.Y - c.Y);
			if (fixedPoint == 0.0)
			{
				t = (u = (v = 0.0));
				return false;
			}
			t = ((b.Y - c.Y) * (point.X - c.X) + (c.X - b.X) * (point.Y - c.Y)) / fixedPoint;
			u = ((c.Y - a.Y) * (point.X - c.X) + (a.X - c.X) * (point.Y - c.Y)) / fixedPoint;
			v = 1.0 - t - u;
			return true;
		}

		public static FixedVec3 GetTriangleNormal(FixedVec3 a, FixedVec3 b, FixedVec3 c)
		{
			return FixedVec3.Normalize(FixedVec3.Cross(b - a, c - a));
		}
	}
}
