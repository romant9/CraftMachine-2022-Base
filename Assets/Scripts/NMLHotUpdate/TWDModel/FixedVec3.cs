using System;
using System.Globalization;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public struct FixedVec3
	{
		public static FixedVec3 Zero = new FixedVec3(0.0, 0.0, 0.0);

		public FixedPoint X;

		public FixedPoint Y;

		public FixedPoint Z;

		[JsonIgnore]
		public FixedPoint Magnitude => FixedPoint.Sqrt(X * X + Y * Y + Z * Z);

		[JsonIgnore]
		public FixedPoint SqrMagnitude => X * X + Y * Y + Z * Z;

		public FixedVec3(FixedVec3 other)
		{
			X = other.X;
			Y = other.Y;
			Z = other.Z;
		}

		public FixedVec3(FixedPoint x, FixedPoint y, FixedPoint z)
		{
			X = x;
			Y = y;
			Z = z;
		}

		public override string ToString()
		{
			string text = ((double)X).ToString("F1", CultureInfo.InvariantCulture);
			string text2 = ((double)Y).ToString("F1", CultureInfo.InvariantCulture);
			string text3 = ((double)Z).ToString("F1", CultureInfo.InvariantCulture);
			return "{ X: " + text + " Y: " + text2 + " Z: " + text3 + " }";
		}

		public static FixedVec3 Negative(FixedVec3 a)
		{
			return new FixedVec3(-a.X, -a.Y, -a.Z);
		}

		public static bool operator ==(FixedVec3 a, FixedVec3 b)
		{
			if (a.X == b.X && a.Y == b.Y)
			{
				return a.Z == b.Z;
			}
			return false;
		}

		public static bool operator !=(FixedVec3 a, FixedVec3 b)
		{
			if (!(a.X != b.X) && !(a.Y != b.Y))
			{
				return a.Z != b.Z;
			}
			return true;
		}

		public override int GetHashCode()
		{
			return X.GetHashCode() + (Y.GetHashCode() << 16) + Z.GetHashCode();
		}

		public override bool Equals(object other)
		{
			if (other is FixedVec3)
			{
				return this == (FixedVec3)other;
			}
			return false;
		}

		public static FixedVec3 operator +(FixedVec3 a, FixedVec3 b)
		{
			return new FixedVec3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
		}

		public static FixedVec3 operator -(FixedVec3 a, FixedVec3 b)
		{
			return new FixedVec3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
		}

		public static FixedVec3 operator *(FixedVec3 vec, FixedPoint scalar)
		{
			return new FixedVec3(vec.X * scalar, vec.Y * scalar, vec.Z * scalar);
		}

		public static FixedPoint operator *(FixedVec3 a, FixedVec3 b)
		{
			return Dot(a, b);
		}

		public static FixedVec3 operator /(FixedVec3 vec, FixedPoint scalar)
		{
			return new FixedVec3(vec.X / scalar, vec.Y / scalar, vec.Z / scalar);
		}

		public static FixedPoint Distance(FixedVec3 a, FixedVec3 b)
		{
			return (b - a).Magnitude;
		}

		public static FixedPoint DistanceSquared(FixedVec3 a, FixedVec3 b)
		{
			return (b - a).SqrMagnitude;
		}

		public static FixedPoint Dot(FixedVec3 a, FixedVec3 b)
		{
			return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
		}

		public static FixedVec3 Cross(FixedVec3 arg1, FixedVec3 arg2)
		{
			return new FixedVec3(arg1.Y * arg2.Z - arg1.Z * arg2.Y, arg1.Z * arg2.X - arg1.X * arg2.Z, arg1.X * arg2.Y - arg1.Y * arg2.X);
		}

		public static FixedVec3 Normalize(FixedVec3 arg)
		{
			if (!(arg.Magnitude != 0L))
			{
				return new FixedVec3(0.0, 0.0, 0.0);
			}
			return arg / arg.Magnitude;
		}

		public static FixedVec3 Abs(FixedVec3 arg)
		{
			return new FixedVec3(FixedPoint.Abs(arg.X), FixedPoint.Abs(arg.Y), FixedPoint.Abs(arg.Z));
		}

		public static FixedPoint Angle(FixedVec3 a, FixedVec3 b)
		{
			FixedPoint fixedPoint = a.Magnitude * b.Magnitude;
			if (fixedPoint == 0L)
			{
				return 0L;
			}
			return FixedPoint.Acos(Dot(a, b) / fixedPoint);
		}

		public static FixedPoint AngleDegrees(FixedVec3 a, FixedVec3 b)
		{
			FixedPoint fixedPoint = a.Magnitude * b.Magnitude;
			if (fixedPoint == 0L)
			{
				return 0L;
			}
			if (Math.Abs((float)(fixedPoint - 1.0)) <= 0.001f)
			{
				fixedPoint = 1L;
			}
			return FixedPoint.Acos(Dot(a, b) / fixedPoint) * (180L / FixedPoint.PI);
		}
	}
}
