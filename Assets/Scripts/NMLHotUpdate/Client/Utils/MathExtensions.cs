using TWDModel;
using UnityEngine;

namespace Client.Utils
{
	public static class MathExtensions
	{
		public static Vector2 ToVector2(this FixedVec2 inVector)
		{
			return new Vector2((float)inVector.X, (float)inVector.Y);
		}

		public static Vector3 ToVector3(this FixedVec3 inVector)
		{
			return new Vector3((float)inVector.X, (float)inVector.Y, (float)inVector.Z);
		}

		public static Quaternion ToQuaternion(this QuaternionData inQuaternion)
		{
			return new Quaternion((float)inQuaternion.X, (float)inQuaternion.Y, (float)inQuaternion.Z, (float)inQuaternion.W);
		}

		public static Matrix4x4 ToMatrix4x4(this Matrix44Data inMatrix)
		{
			return new Matrix4x4
			{
				m00 = inMatrix.M00,
				m10 = inMatrix.M10,
				m20 = inMatrix.M20,
				m30 = inMatrix.M30,
				m01 = inMatrix.M01,
				m11 = inMatrix.M11,
				m21 = inMatrix.M21,
				m31 = inMatrix.M31,
				m02 = inMatrix.M02,
				m12 = inMatrix.M12,
				m22 = inMatrix.M22,
				m32 = inMatrix.M32,
				m03 = inMatrix.M03,
				m13 = inMatrix.M13,
				m23 = inMatrix.M23,
				m33 = inMatrix.M33
			};
		}

		public static FixedVec2 ToFixedVec2(this Vector2 inVector)
		{
			return new FixedVec2(inVector.x, inVector.y);
		}

		public static FixedVec3 ToFixedVec3(this Vector3 inVector)
		{
			return new FixedVec3(inVector.x, inVector.y, inVector.z);
		}

		public static Matrix44Data ToMatrix44Data(this Matrix4x4 inMatrix)
		{
			return new Matrix44Data
			{
				M00 = inMatrix.m00,
				M10 = inMatrix.m10,
				M20 = inMatrix.m20,
				M30 = inMatrix.m30,
				M01 = inMatrix.m01,
				M11 = inMatrix.m11,
				M21 = inMatrix.m21,
				M31 = inMatrix.m31,
				M02 = inMatrix.m02,
				M12 = inMatrix.m12,
				M22 = inMatrix.m22,
				M32 = inMatrix.m32,
				M03 = inMatrix.m03,
				M13 = inMatrix.m13,
				M23 = inMatrix.m23,
				M33 = inMatrix.m33
			};
		}

		public static QuaternionData ToQuaternionData(this Quaternion inQuaternion)
		{
			return new QuaternionData(inQuaternion.x, inQuaternion.y, inQuaternion.z, inQuaternion.w);
		}

		public static float SignedAngle(this Vector3 a, Vector3 b, Vector3 up)
		{
			return Vector3.Angle(a, b) * Mathf.Sign(Vector3.Dot(a, Vector3.Cross(b, up)));
		}

		public static float LinearInterpolation(float currentTime, float startValue, float changeOfValue, float durationTime)
		{
			return changeOfValue * currentTime / durationTime + startValue;
		}

		public static float EaseQuadraticIn(float currentTime, float startValue, float changeOfValue, float durationTime)
		{
			return changeOfValue * (currentTime /= durationTime) * currentTime + startValue;
		}

		public static float EaseQuadraticInOut(float currentTime, float startValue, float changeOfValue, float durationTime)
		{
			if ((currentTime /= durationTime / 2f) < 1f)
			{
				return changeOfValue / 2f * currentTime * currentTime + startValue;
			}
			return (0f - changeOfValue) / 2f * ((currentTime -= 1f) * (currentTime - 2f) - 1f) + startValue;
		}

		public static float EaseQuadraticOut(float currentTime, float startValue, float changeOfValue, float durationTime)
		{
			return (0f - changeOfValue) * (currentTime /= durationTime) * (currentTime - 2f) + startValue;
		}

		public static float EaseCubicIn(float currentTime, float startValue, float changeOfValue, float durationTime)
		{
			return changeOfValue * (currentTime /= durationTime) * currentTime * currentTime + startValue;
		}

		public static float EaseCubicInOut(float currentTime, float startValue, float changeOfValue, float durationTime)
		{
			if ((currentTime /= durationTime / 2f) < 1f)
			{
				return changeOfValue / 2f * currentTime * currentTime * currentTime + startValue;
			}
			return changeOfValue / 2f * ((currentTime -= 2f) * currentTime * currentTime + 2f) + startValue;
		}

		public static float EaseCubicOut(float currentTime, float startValue, float changeOfValue, float durationTime)
		{
			return changeOfValue * ((currentTime = currentTime / durationTime - 1f) * currentTime * currentTime + 1f) + startValue;
		}

		public static FixedPoint UIRounding(this FixedPoint value)
		{
			return value + 0.0001;
		}
	}
}
