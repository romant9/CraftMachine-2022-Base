using Client.Utils;
using TWDModel;
using UnityEngine;

public class StationaryBossTurretAim : MonoBehaviour
{
	[SerializeField]
	private Transform turretPivot;

	[SerializeField]
	[Tooltip("Degrees per second the turret rotates towards its target. 0 = snap instantly.")]
	private float turnSpeed = 180f;

	public bool IsAimedAt(ActorModel target, float maxAngleDegrees = 2f)
	{
		if (turretPivot == null || target == null || GridView.Instance == null)
		{
			return turretPivot == null;
		}
		Vector3 vector = GridView.Instance.GetPosition(target.GridCoordinate).ToVector3() - turretPivot.position;
		vector.y = 0f;
		if (vector.sqrMagnitude < 0.0001f)
		{
			return true;
		}
		Quaternion b = Quaternion.LookRotation(vector.normalized, Vector3.up);
		return Quaternion.Angle(turretPivot.rotation, b) <= maxAngleDegrees;
	}

	public void AimToward(ActorModel target, float deltaTime)
	{
		if (turretPivot == null || target == null || GridView.Instance == null)
		{
			return;
		}
		Vector3 vector = GridView.Instance.GetPosition(target.GridCoordinate).ToVector3() - turretPivot.position;
		vector.y = 0f;
		if (!(vector.sqrMagnitude < 0.0001f))
		{
			Quaternion quaternion = Quaternion.LookRotation(vector.normalized, Vector3.up);
			if (turnSpeed <= 0f)
			{
				turretPivot.rotation = quaternion;
			}
			else
			{
				turretPivot.rotation = Quaternion.RotateTowards(turretPivot.rotation, quaternion, turnSpeed * deltaTime);
			}
		}
	}
}
