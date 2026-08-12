using UnityEngine;

public static class QuaternionHelpers
{
	public static Quaternion AddQuaternions(Quaternion first, Quaternion second)
	{
		first.w += second.w;
		first.x += second.x;
		first.y += second.y;
		first.z += second.z;
		return first;
	}

	public static Quaternion ScaleQuaternion(Quaternion rotation, float multiplier)
	{
		rotation.w *= multiplier;
		rotation.x *= multiplier;
		rotation.y *= multiplier;
		rotation.z *= multiplier;
		return rotation;
	}

	public static float QuaternionMagnitude(Quaternion rotation)
	{
		return Mathf.Sqrt(Quaternion.Dot(rotation, rotation));
	}

	public static Quaternion NormalizeQuaternion(Quaternion rotation)
	{
		float num = QuaternionMagnitude(rotation);
		if (num > 0f)
		{
			return ScaleQuaternion(rotation, 1f / num);
		}
		Debug.LogWarning("Cannot normalize a quaternion with zero magnitude.");
		return Quaternion.identity;
	}
}
