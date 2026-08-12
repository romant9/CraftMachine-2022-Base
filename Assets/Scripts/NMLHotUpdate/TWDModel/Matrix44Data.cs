using System;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public struct Matrix44Data
	{
		public static Matrix44Data Identity = new Matrix44Data
		{
			M00 = 1f,
			M01 = 0f,
			M02 = 0f,
			M03 = 0f,
			M10 = 0f,
			M11 = 1f,
			M12 = 0f,
			M13 = 0f,
			M20 = 0f,
			M21 = 0f,
			M22 = 1f,
			M23 = 0f,
			M30 = 0f,
			M31 = 0f,
			M32 = 0f,
			M33 = 1f
		};

		public float M00;

		public float M10;

		public float M20;

		public float M30;

		public float M01;

		public float M11;

		public float M21;

		public float M31;

		public float M02;

		public float M12;

		public float M22;

		public float M32;

		public float M03;

		public float M13;

		public float M23;

		public float M33;

		[JsonIgnore]
		public bool IsIdentity => this == Identity;

		[JsonIgnore]
		public FixedVec3 Translation
		{
			get
			{
				return new FixedVec3(M03, M13, M23);
			}
			set
			{
				M03 = (float)value.X;
				M13 = (float)value.X;
				M23 = (float)value.X;
			}
		}

		public Matrix44Data(Matrix44Data other)
		{
			M00 = other.M00;
			M01 = other.M01;
			M02 = other.M02;
			M03 = other.M03;
			M10 = other.M10;
			M11 = other.M11;
			M12 = other.M12;
			M13 = other.M13;
			M20 = other.M20;
			M21 = other.M21;
			M22 = other.M22;
			M23 = other.M23;
			M30 = other.M30;
			M31 = other.M31;
			M32 = other.M32;
			M33 = other.M33;
		}

		public static Matrix44Data FromQuaternion(QuaternionData q)
		{
			QuaternionData quaternionData = QuaternionData.Normalize(q);
			float num = (float)quaternionData.X;
			float num2 = (float)quaternionData.Y;
			float num3 = (float)quaternionData.Z;
			float num4 = (float)quaternionData.W;
			return new Matrix44Data
			{
				M00 = 1f - 2f * num2 * num2 - 2f * num3 * num3,
				M10 = 2f * num * num2 - 2f * num4 * num3,
				M20 = 2f * num * num3 + 2f * num4 * num2,
				M30 = 0f,
				M01 = 2f * num * num2 + 2f * num4 * num3,
				M11 = 1f - 2f * num * num - 2f * num3 * num3,
				M21 = 2f * num2 * num3 - 2f * num4 * num,
				M31 = 0f,
				M02 = 2f * num * num3 - 2f * num4 * num2,
				M12 = 2f * num2 * num3 + 2f * num4 * num,
				M22 = 1f - 2f * num * num - 2f * num2 * num2,
				M32 = 0f,
				M03 = 0f,
				M13 = 0f,
				M23 = 0f,
				M33 = 1f
			};
		}

		public static bool operator ==(Matrix44Data a, Matrix44Data b)
		{
			if (a.M00 != b.M00)
			{
				return false;
			}
			if (a.M10 != b.M10)
			{
				return false;
			}
			if (a.M20 != b.M20)
			{
				return false;
			}
			if (a.M30 != b.M30)
			{
				return false;
			}
			if (a.M01 != b.M01)
			{
				return false;
			}
			if (a.M11 != b.M11)
			{
				return false;
			}
			if (a.M21 != b.M21)
			{
				return false;
			}
			if (a.M31 != b.M31)
			{
				return false;
			}
			if (a.M02 != b.M02)
			{
				return false;
			}
			if (a.M12 != b.M12)
			{
				return false;
			}
			if (a.M22 != b.M22)
			{
				return false;
			}
			if (a.M32 != b.M32)
			{
				return false;
			}
			if (a.M03 != b.M03)
			{
				return false;
			}
			if (a.M13 != b.M13)
			{
				return false;
			}
			if (a.M23 != b.M23)
			{
				return false;
			}
			if (a.M33 != b.M33)
			{
				return false;
			}
			return true;
		}

		public static bool operator !=(Matrix44Data a, Matrix44Data b)
		{
			return !(a == b);
		}

		public override bool Equals(object obj)
		{
			return (Matrix44Data)obj == this;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public void Scale(FixedVec3 scale)
		{
			M00 *= (float)scale.X;
			M01 *= (float)scale.X;
			M02 *= (float)scale.X;
			M03 *= (float)scale.X;
			M10 *= (float)scale.Y;
			M11 *= (float)scale.Y;
			M12 *= (float)scale.Y;
			M13 *= (float)scale.Y;
			M20 *= (float)scale.Z;
			M21 *= (float)scale.Z;
			M22 *= (float)scale.Z;
			M23 *= (float)scale.Z;
		}

		public static Matrix44Data operator *(Matrix44Data a, Matrix44Data b)
		{
			Matrix44Data identity = Identity;
			identity.M00 = b.M00 * a.M00 + b.M10 * a.M01 + b.M20 * a.M02 + b.M30 * a.M03;
			identity.M01 = b.M01 * a.M00 + b.M11 * a.M01 + b.M21 * a.M02 + b.M31 * a.M03;
			identity.M02 = b.M02 * a.M00 + b.M12 * a.M01 + b.M22 * a.M02 + b.M32 * a.M03;
			identity.M03 = b.M03 * a.M00 + b.M13 * a.M01 + b.M23 * a.M02 + b.M33 * a.M03;
			identity.M10 = b.M00 * a.M10 + b.M10 * a.M11 + b.M20 * a.M12 + b.M30 * a.M13;
			identity.M11 = b.M01 * a.M10 + b.M11 * a.M11 + b.M21 * a.M12 + b.M31 * a.M13;
			identity.M12 = b.M02 * a.M10 + b.M12 * a.M11 + b.M22 * a.M12 + b.M32 * a.M13;
			identity.M13 = b.M03 * a.M10 + b.M13 * a.M11 + b.M23 * a.M12 + b.M33 * a.M13;
			identity.M20 = b.M00 * a.M20 + b.M10 * a.M21 + b.M20 * a.M22 + b.M30 * a.M23;
			identity.M21 = b.M01 * a.M20 + b.M11 * a.M21 + b.M21 * a.M22 + b.M31 * a.M23;
			identity.M22 = b.M02 * a.M20 + b.M12 * a.M21 + b.M22 * a.M22 + b.M32 * a.M23;
			identity.M23 = b.M03 * a.M20 + b.M13 * a.M21 + b.M23 * a.M22 + b.M33 * a.M23;
			identity.M30 = b.M00 * a.M30 + b.M10 * a.M31 + b.M20 * a.M32 + b.M30 * a.M33;
			identity.M31 = b.M01 * a.M30 + b.M11 * a.M31 + b.M21 * a.M32 + b.M31 * a.M33;
			identity.M32 = b.M02 * a.M30 + b.M12 * a.M31 + b.M22 * a.M32 + b.M32 * a.M33;
			identity.M33 = b.M03 * a.M30 + b.M13 * a.M31 + b.M23 * a.M32 + b.M33 * a.M33;
			return identity;
		}

		public static Matrix44Data Invert(Matrix44Data arg)
		{
			float num = arg.M00 * arg.M11 - arg.M01 * arg.M10;
			float num2 = arg.M00 * arg.M21 - arg.M01 * arg.M20;
			float num3 = arg.M00 * arg.M31 - arg.M01 * arg.M30;
			float num4 = arg.M10 * arg.M21 - arg.M11 * arg.M20;
			float num5 = arg.M10 * arg.M31 - arg.M11 * arg.M30;
			float num6 = arg.M20 * arg.M31 - arg.M21 * arg.M30;
			float num7 = arg.M22 * arg.M33 - arg.M23 * arg.M32;
			float num8 = arg.M12 * arg.M33 - arg.M13 * arg.M32;
			float num9 = arg.M12 * arg.M23 - arg.M13 * arg.M22;
			float num10 = arg.M02 * arg.M33 - arg.M03 * arg.M32;
			float num11 = arg.M02 * arg.M23 - arg.M03 * arg.M22;
			float num12 = arg.M02 * arg.M13 - arg.M03 * arg.M12;
			float num13 = num * num7 - num2 * num8 + num3 * num9 + num4 * num10 - num5 * num11 + num6 * num12;
			if (num13 == 0f)
			{
				return new Matrix44Data(Identity);
			}
			float num14 = 1f / num13;
			Matrix44Data result = new Matrix44Data(Identity);
			result.M00 = (arg.M11 * num7 - arg.M21 * num8 + arg.M31 * num9) * num14;
			result.M10 = ((0f - arg.M10) * num7 + arg.M20 * num8 - arg.M30 * num9) * num14;
			result.M20 = (arg.M13 * num6 - arg.M23 * num5 + arg.M33 * num4) * num14;
			result.M30 = ((0f - arg.M12) * num6 + arg.M22 * num5 - arg.M32 * num4) * num14;
			result.M01 = ((0f - arg.M01) * num7 + arg.M21 * num10 - arg.M31 * num11) * num14;
			result.M11 = (arg.M00 * num7 - arg.M20 * num10 + arg.M30 * num11) * num14;
			result.M21 = ((0f - arg.M03) * num6 + arg.M23 * num3 - arg.M33 * num2) * num14;
			result.M31 = (arg.M02 * num6 - arg.M22 * num3 + arg.M32 * num2) * num14;
			result.M02 = (arg.M01 * num8 - arg.M11 * num10 + arg.M31 * num12) * num14;
			result.M12 = ((0f - arg.M00) * num8 + arg.M10 * num10 - arg.M30 * num12) * num14;
			result.M22 = (arg.M03 * num5 - arg.M13 * num3 + arg.M33 * num) * num14;
			result.M32 = ((0f - arg.M02) * num5 + arg.M12 * num3 - arg.M32 * num) * num14;
			result.M03 = ((0f - arg.M01) * num9 + arg.M11 * num11 - arg.M21 * num12) * num14;
			result.M13 = (arg.M00 * num9 - arg.M10 * num11 + arg.M20 * num12) * num14;
			result.M23 = ((0f - arg.M03) * num4 + arg.M13 * num2 - arg.M23 * num) * num14;
			result.M33 = (arg.M02 * num4 - arg.M12 * num2 + arg.M22 * num) * num14;
			return result;
		}

		public static FixedVec3 operator *(Matrix44Data a, FixedVec3 b)
		{
			return new FixedVec3(a.M00 * b.X + a.M01 * b.Y + a.M02 * b.Z + a.M03, a.M10 * b.X + a.M11 * b.Y + a.M12 * b.Z + a.M13, a.M20 * b.X + a.M22 * b.Y + a.M22 * b.Z + a.M23);
		}

		public static FixedVec3 Rotate(Matrix44Data a, FixedVec3 b)
		{
			return new FixedVec3(a.M00 * b.X + a.M01 * b.Y + a.M02 * b.Z, a.M10 * b.X + a.M11 * b.Y + a.M12 * b.Z, a.M20 * b.X + a.M22 * b.Y + a.M22 * b.Z);
		}

		public override string ToString()
		{
			return $"{M00:0.000} {M10:0.000} {M20:0.000} {M30:0.000}\n" + $"{M01:0.000} {M11:0.000} {M21:0.000} {M31:0.000}\n" + $"{M02:0.000} {M12:0.000} {M22:0.000} {M32:0.000}\n" + $"{M03:0.000} {M13:0.000} {M23:0.000} {M33:0.000}";
		}
	}
}
