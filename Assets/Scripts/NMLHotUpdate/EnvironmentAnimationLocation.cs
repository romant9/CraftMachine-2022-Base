using System;
using UnityEngine;

[Serializable]
public class EnvironmentAnimationLocation
{
	public Vector3 Position;

	public Quaternion Rotation;

	public EnvironmentLocationType LocationType;

	public Vector3 GetWorldPosition(Transform transform)
	{
		return transform.TransformPoint(Position);
	}

	public Quaternion GetWorldRotation(Transform transform)
	{
		return transform.rotation * Rotation;
	}
}
