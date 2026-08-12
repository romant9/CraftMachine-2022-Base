using System.Collections.Generic;
using UnityEngine;

public class CombatCameraData : ScriptableObject
{
	[Tooltip("How fast the camera goes when dragged")]
	public float CameraDragSpeed = 10f;//50f

	[Tooltip("How fast the camera goes when it moves to a target location")]
	public float CameraMoveSpeed = 10f;

	[Tooltip("The camera hard limit size in world units.")]
	public float CameraHardLimitSize = 5f;

	[Tooltip("Percentage of screen width to which the player must be on the limits in order to detect that camera needs to focus on it when clicked.")]
	public float CameraFocusTriggerRatio = 0.1f;

	[Tooltip("How quickly the camera stops after the player lifts his finger.")]
	public float Damping = 0.1f;

	[Tooltip("The maximum elevation angle variation used when dragging.")]
	public float ElevationAngleVariationSoft = 10f;//5f;

	[Tooltip("The maximum elevation angle variation used when dragging.")]
	public float ElevationAngleVariationHard = 12f;//6f;

	[Tooltip("The friction applied to movement during dragging beyond the soft limits.")]
	public float SoftLimitsFriction = 0.25f;

	[Range(20f, 85f)]
	[Tooltip("Camera field-of-view for combat. Limited to reasonable range.")]
	public float FieldOfView;

	[SerializeField]
	[Tooltip("Combat camera parameters for different device models.")]
	public List<CombatCameraProfile> CombatCameraProfiles;

	public static DeviceModelEnum CurrentDeviceModel
	{
		get
		{
			if (SystemInfo.deviceModel.ToLower().Contains("iphone"))
			{
				return DeviceModelEnum.iPhone;
			}
			if (SystemInfo.deviceModel.ToLower().Contains("ipad"))
			{
				return DeviceModelEnum.iPad;
			}
			return DeviceModelEnum.Other;
		}
	}

	public CombatCameraProfile GetCurrentProfile()
	{
		if (CombatCameraProfiles != null && CombatCameraProfiles.Count > 0)
		{
			DeviceModelEnum currentDeviceModel = CurrentDeviceModel;
			for (int i = 0; i < CombatCameraProfiles.Count; i++)
			{
				if (CombatCameraProfiles[i].DeviceModel == currentDeviceModel)
				{
					return CombatCameraProfiles[i];
				}
			}
			for (int j = 0; j < CombatCameraProfiles.Count; j++)
			{
				if (CombatCameraProfiles[j].DeviceModel == DeviceModelEnum.Other)
				{
					return CombatCameraProfiles[j];
				}
			}
			return CombatCameraProfiles[0];
		}
		return null;
	}
}
