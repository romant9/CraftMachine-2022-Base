using System;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public struct Vector3Data
	{
		public float X;

		public float Y;

		public float Z;

		public static Vector3Data Zero = new Vector3Data(0f, 0f, 0f);

		private static float epsilon = 1E-05f;

		[JsonIgnore]
		public float Magnitude => (float)Math.Sqrt(X * X + Y * Y + Z * Z);

		[JsonIgnore]
		public float SqrMagnitude => X * X + Y * Y + Z * Z;

		public Vector3Data(float x, float y, float z)
		{
			this = default(Vector3Data);
			X = x;
			Y = y;
			Z = z;
		}

		public override string ToString()
		{
			return $"X: {X:0.000} Y: {Y:0.000} Z: {Z:0.000}";
		}

		public override bool Equals(object obj)
		{
			return (Vector3Data)obj == this;
		}

		public override int GetHashCode()
		{
			return X.GetHashCode() + Y.GetHashCode() + Z.GetHashCode();
		}

		public static float Distance(Vector3Data arg1, Vector3Data arg2)
		{
			return (arg2 - arg1).Magnitude;
		}

		public static float DistanceSquared(Vector3Data arg1, Vector3Data arg2)
		{
			return (arg2 - arg1).SqrMagnitude;
		}

		public static float Dot(Vector3Data arg1, Vector3Data arg2)
		{
			return arg1.X * arg2.X + arg1.Y * arg2.Y + arg1.Z * arg2.Z;
		}

		public static Vector3Data Cross(Vector3Data arg1, Vector3Data arg2)
		{
			return new Vector3Data(arg1.Y * arg2.Z - arg1.Z * arg2.Y, arg1.Z * arg2.X - arg1.X * arg2.Z, arg1.X * arg2.Y - arg1.Y * arg2.X);
		}

		public static Vector3Data Rcp(Vector3Data arg)
		{
			return new Vector3Data(1f / arg.X, 1f / arg.Y, 1f / arg.Z);
		}

		public static Vector3Data Abs(Vector3Data arg)
		{
			return new Vector3Data(Math.Abs(arg.X), Math.Abs(arg.Y), Math.Abs(arg.Z));
		}

		public static Vector3Data Normalize(Vector3Data arg)
		{
			return arg / arg.Magnitude;
		}

		public static Vector3Data operator -(Vector3Data arg1, Vector3Data arg2)
		{
			return new Vector3Data(arg1.X - arg2.X, arg1.Y - arg2.Y, arg1.Z - arg2.Z);
		}

		public static Vector3Data operator +(Vector3Data arg1, Vector3Data arg2)
		{
			return new Vector3Data(arg1.X + arg2.X, arg1.Y + arg2.Y, arg1.Z + arg2.Z);
		}

		public static bool operator !=(Vector3Data arg1, Vector3Data arg2)
		{
			if (!(Math.Abs(arg1.X - arg2.X) > epsilon) && !(Math.Abs(arg1.Y - arg2.Y) > epsilon))
			{
				return Math.Abs(arg1.Z - arg2.Z) > epsilon;
			}
			return true;
		}

		public static bool operator ==(Vector3Data arg1, Vector3Data arg2)
		{
			if (Math.Abs(arg1.X - arg2.X) < epsilon && Math.Abs(arg1.Y - arg2.Y) < epsilon)
			{
				return Math.Abs(arg1.Z - arg2.Z) < epsilon;
			}
			return false;
		}

		public static Vector3Data operator /(Vector3Data vec, float value)
		{
			return new Vector3Data(vec.X / value, vec.Y / value, vec.Z / value);
		}

		public static Vector3Data operator *(Vector3Data vec, float value)
		{
			return new Vector3Data(vec.X * value, vec.Y * value, vec.Z * value);
		}

		public static float operator *(Vector3Data a, Vector3Data b)
		{
			return Dot(a, b);
		}

		public static Vector3Data operator -(Vector3Data a)
		{
			return new Vector3Data(0f - a.X, 0f - a.Y, 0f - a.Z);
		}

		public static Vector3Data Min(Vector3Data a, Vector3Data b)
		{
			return new Vector3Data(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y), Math.Min(a.Z, b.Z));
		}

		public static Vector3Data Max(Vector3Data a, Vector3Data b)
		{
			return new Vector3Data(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y), Math.Max(a.Z, b.Z));
		}

		public static Vector3Data Clamp(Vector3Data a, Vector3Data min, Vector3Data max)
		{
			return Min(Max(a, min), max);
		}

		public static Vector3Data Snap(Vector3Data arg, float scale)
		{
			return new Vector3Data((float)Math.Round(arg.X / scale) * scale, (float)Math.Round(arg.Y / scale) * scale, (float)Math.Round(arg.Z / scale) * scale);
		}

		public static Vector3Data Lerp(Vector3Data from, Vector3Data to, float time)
		{
			return from + (to - from) * time;
		}

		public void SetLength(float length)
		{
			float magnitude = Magnitude;
			if (magnitude != 0f)
			{
				float num = length / magnitude;
				X *= num;
				Y *= num;
				Z *= num;
			}
		}
	}
}
