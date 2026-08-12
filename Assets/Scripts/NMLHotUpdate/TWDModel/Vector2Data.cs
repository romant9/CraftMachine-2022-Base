using System;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public struct Vector2Data
	{
		public static Vector2Data Zero = new Vector2Data(0f, 0f);

		public float X;

		public float Y;

		[JsonIgnore]
		public float Angle
		{
			get
			{
				float val = Normalize(this) * new Vector2Data(1f, 0f);
				float num = (float)Math.Acos(Math.Max(-1f, Math.Min(1f, val)));
				if (!(Y < 0f))
				{
					return num;
				}
				return (float)(Math.PI * 2.0 - (double)num);
			}
		}

		[JsonIgnore]
		public float Magnitude => (float)Math.Sqrt(X * X + Y * Y);

		[JsonIgnore]
		public float SqrMagnitude => X * X + Y * Y;

		public Vector2Data(float x, float y)
		{
			X = x;
			Y = y;
		}

		public Vector2Data(Vector2Data other)
		{
			X = other.X;
			Y = other.Y;
		}

		public static Vector2Data FromAngle(Vector2Data start, float angle, float magnitude)
		{
			return new Vector2Data(start.X + (float)Math.Cos(angle) * magnitude, start.Y - (float)Math.Sin(angle) * magnitude);
		}

		public static Vector2Data FromAngle(float angle, float magnitude)
		{
			return new Vector2Data((float)Math.Cos(angle) * magnitude, (0f - (float)Math.Sin(angle)) * magnitude);
		}

		public bool IsZero()
		{
			if (X == 0f)
			{
				return Y == 0f;
			}
			return false;
		}

		public static float AngleTo(Vector2Data arg1, Vector2Data arg2)
		{
			return Normalize(arg2 - arg1).Angle;
		}

		public override bool Equals(object obj)
		{
			return (Vector2Data)obj == this;
		}

		public override int GetHashCode()
		{
			return X.GetHashCode() + Y.GetHashCode();
		}

		public static float Distance(Vector2Data arg1, Vector2Data arg2)
		{
			return (arg2 - arg1).Magnitude;
		}

		public static float DistanceSquared(Vector2Data arg1, Vector2Data arg2)
		{
			return (arg2 - arg1).SqrMagnitude;
		}

		public void SetZero()
		{
			X = 0f;
			Y = 0f;
		}

		public void Set(float inX, float inY)
		{
			X = inX;
			Y = inY;
		}

		public static float Dot(Vector2Data arg1, Vector2Data arg2)
		{
			return arg1.X * arg2.X + arg1.Y * arg2.Y;
		}

		public static Vector2Data Cross(Vector2Data arg)
		{
			return new Vector2Data(arg.Y, 0f - arg.X);
		}

		public static float Cross(Vector2Data arg1, Vector2Data arg2)
		{
			return arg1.X * arg2.Y - arg1.Y * arg2.X;
		}

		public static Vector2Data Normalize(Vector2Data arg)
		{
			float magnitude = arg.Magnitude;
			if (magnitude == 0f)
			{
				return new Vector2Data(0f, 0f);
			}
			return new Vector2Data(arg.X / magnitude, arg.Y / magnitude);
		}

		public static Vector2Data operator +(Vector2Data a, Vector2Data b)
		{
			return new Vector2Data(a.X + b.X, a.Y + b.Y);
		}

		public static Vector2Data operator -(Vector2Data a, Vector2Data b)
		{
			return new Vector2Data(a.X - b.X, a.Y - b.Y);
		}

		public static float operator *(Vector2Data a, Vector2Data b)
		{
			return Dot(a, b);
		}

		public static Vector2Data operator *(Vector2Data a, float scalar)
		{
			return new Vector2Data(a.X * scalar, a.Y * scalar);
		}

		public static Vector2Data operator /(Vector2Data a, float scalar)
		{
			if (scalar != 0f)
			{
				return new Vector2Data(a.X / scalar, a.Y / scalar);
			}
			return new Vector2Data(0f, 0f);
		}

		public static bool operator ==(Vector2Data a, Vector2Data b)
		{
			if (a.X == b.X)
			{
				return a.Y == b.Y;
			}
			return false;
		}

		public static bool operator !=(Vector2Data a, Vector2Data b)
		{
			return !(a == b);
		}

		public static Vector2Data operator -(Vector2Data a)
		{
			return new Vector2Data(0f - a.X, 0f - a.Y);
		}

		public static Vector2Data Min(Vector2Data a, Vector2Data b)
		{
			return new Vector2Data(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y));
		}

		public static Vector2Data Max(Vector2Data a, Vector2Data b)
		{
			return new Vector2Data(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y));
		}

		public static Vector2Data Clamp(Vector2Data a, Vector2Data min, Vector2Data max)
		{
			return Min(Max(a, min), max);
		}

		public void SetLength(float length)
		{
			float magnitude = Magnitude;
			if (magnitude != 0f)
			{
				X = X / magnitude * length;
				Y = Y / magnitude * length;
			}
		}

		public static Vector2Data Snap(Vector2Data arg, float scale)
		{
			return new Vector2Data((float)Math.Round(arg.X / scale) * scale, (float)Math.Round(arg.Y / scale) * scale);
		}

		public override string ToString()
		{
			return $"X: {X:0.000} Y: {Y:0.000}";
		}
	}
}
