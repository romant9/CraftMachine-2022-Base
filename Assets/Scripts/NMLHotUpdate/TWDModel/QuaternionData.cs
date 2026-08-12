using System;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public struct QuaternionData
	{
		public static QuaternionData Zero = new QuaternionData(0.0, 0.0, 0.0, 1.0);

		public FixedPoint X;

		public FixedPoint Y;

		public FixedPoint Z;

		public FixedPoint W;

		[JsonIgnore]
		public FixedPoint Magnitude => FixedPoint.Sqrt(X * X + Y * Y + Z * Z + W * W);

		[JsonIgnore]
		public FixedPoint SqrMagnitude => X * X + Y * Y + Z * Z + W * W;

		public QuaternionData(FixedPoint x, FixedPoint y, FixedPoint z, FixedPoint w)
		{
			X = x;
			Y = y;
			Z = z;
			W = w;
		}

		public QuaternionData(FixedVec3 axis, FixedPoint angle)
		{
			FixedPoint w = FixedPoint.Cos(angle / 2.0);
			FixedPoint fixedPoint = FixedPoint.Sin(angle / 2.0);
			W = w;
			X = fixedPoint * axis.X;
			Y = fixedPoint * axis.Y;
			Z = fixedPoint * axis.Z;
		}

		public static QuaternionData Normalize(QuaternionData arg)
		{
			FixedPoint magnitude = arg.Magnitude;
			if (magnitude != 0.0)
			{
				return new QuaternionData(arg.X / magnitude, arg.Y / magnitude, arg.Z / magnitude, arg.W / magnitude);
			}
			return Zero;
		}

		public static bool operator ==(QuaternionData a, QuaternionData b)
		{
			if (a.X == b.X && a.Y == b.Y && a.Z == b.Z)
			{
				return a.W == b.W;
			}
			return false;
		}

		public static bool operator !=(QuaternionData a, QuaternionData b)
		{
			return !(a == b);
		}

		public static QuaternionData operator *(QuaternionData a, FixedPoint b)
		{
			return new QuaternionData(a.X * b, a.Y * b, a.Z * b, a.W * b);
		}

		public static QuaternionData operator /(QuaternionData a, FixedPoint b)
		{
			return new QuaternionData(a.X / b, a.Y / b, a.Z / b, a.W / b);
		}

		public static QuaternionData operator *(QuaternionData a, QuaternionData b)
		{
			return new QuaternionData
			{
				X = a.W * b.X + a.X * b.W + a.Y * b.Z - a.Z * b.Y,
				Y = a.W * b.Y - a.X * b.Z + a.Y * b.W + a.Z * b.X,
				Z = a.W * b.Z + a.X * b.Y - a.Y * b.X + a.Z * b.W,
				W = a.W * b.W - a.X * b.X - a.Y * b.Y - a.Z * b.Z
			};
		}

		public QuaternionData Conjugate()
		{
			return new QuaternionData(-X, -Y, -Z, W);
		}

		public QuaternionData Inverse()
		{
			FixedPoint sqrMagnitude = SqrMagnitude;
			if (sqrMagnitude == 0.0)
			{
				return default(QuaternionData);
			}
			return Conjugate() / sqrMagnitude;
		}

		public override bool Equals(object obj)
		{
			return (QuaternionData)obj == this;
		}

		public override int GetHashCode()
		{
			return X.GetHashCode() + Y.GetHashCode() + Z.GetHashCode() + W.GetHashCode();
		}

		public override string ToString()
		{
			return $"X: {(float)X:0.000} Y: {(float)Y:0.000} Z: {(float)Z:0.000} W: {(float)W:0.000}";
		}
	}
}
