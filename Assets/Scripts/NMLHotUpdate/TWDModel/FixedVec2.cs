using System;
using System.Globalization;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public struct FixedVec2
	{
		public FixedPoint X;

		public FixedPoint Y;

		[JsonIgnore]
		public FixedPoint Magnitude => FixedPoint.Sqrt(X * X + Y * Y);

		[JsonIgnore]
		public FixedPoint SqrMagnitude => X * X + Y * Y;

		public FixedVec2(FixedVec2 other)
		{
			X = other.X;
			Y = other.Y;
		}

		public FixedVec2(FixedPoint x, FixedPoint y)
		{
			X = x;
			Y = y;
		}

		public static bool operator ==(FixedVec2 a, FixedVec2 b)
		{
			if (a.X == b.X)
			{
				return a.Y == b.Y;
			}
			return false;
		}

		public static bool operator !=(FixedVec2 a, FixedVec2 b)
		{
			if (!(a.X != b.X))
			{
				return a.Y != b.Y;
			}
			return true;
		}

		public override int GetHashCode()
		{
			return X.GetHashCode() + (Y.GetHashCode() << 16);
		}

		public override bool Equals(object other)
		{
			if (other is FixedVec2)
			{
				return this == (FixedVec2)other;
			}
			return false;
		}

		public override string ToString()
		{
			string text = ((double)X).ToString("F1", CultureInfo.InvariantCulture);
			string text2 = ((double)Y).ToString("F1", CultureInfo.InvariantCulture);
			return "{ X: " + text + " Y: " + text2 + " }";
		}

		public static FixedVec2 operator +(FixedVec2 a, FixedVec2 b)
		{
			return new FixedVec2(a.X + b.X, a.Y + b.Y);
		}

		public static FixedVec2 operator -(FixedVec2 a, FixedVec2 b)
		{
			return new FixedVec2(a.X - b.X, a.Y - b.Y);
		}

		public static FixedVec2 operator *(FixedVec2 vec, FixedPoint scalar)
		{
			return new FixedVec2(vec.X * scalar, vec.Y * scalar);
		}

		public static FixedVec2 operator /(FixedVec2 vec, FixedPoint scalar)
		{
			return new FixedVec2(vec.X / scalar, vec.Y / scalar);
		}

		public static FixedVec2 Cross(FixedVec2 arg)
		{
			return new FixedVec2(arg.Y, -arg.X);
		}

		public static FixedPoint Cross(FixedVec2 arg1, FixedVec2 arg2)
		{
			return arg1.X * arg2.Y - arg1.Y * arg2.X;
		}

		public static FixedVec2 Normalize(FixedVec2 arg)
		{
			FixedPoint magnitude = arg.Magnitude;
			if (magnitude == 0.0)
			{
				return new FixedVec2(0.0, 0.0);
			}
			return new FixedVec2(arg.X / magnitude, arg.Y / magnitude);
		}

		public static FixedPoint Distance(FixedVec2 a, FixedVec2 b)
		{
			return (b - a).Magnitude;
		}

		public static FixedPoint Dot(FixedVec2 a, FixedVec2 b)
		{
			return a.X * b.X + a.Y * b.Y;
		}

		public static FixedPoint Angle(FixedVec2 a, FixedVec2 b)
		{
			FixedPoint fixedPoint = a.Magnitude * b.Magnitude;
			if (fixedPoint == 0L)
			{
				return 0L;
			}
			return FixedPoint.Acos(Dot(a, b) / fixedPoint);
		}
	}
}
