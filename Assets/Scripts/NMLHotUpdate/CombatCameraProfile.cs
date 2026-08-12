using System;
using UnityEngine;

[Serializable]
public class CombatCameraProfile
{
	[Tooltip("Device model, parsed from string reported by Unity.")]
	public DeviceModelEnum DeviceModel = DeviceModelEnum.Other;

	[Range(0f, 0.45f)]
	[Tooltip("Top margin in screen-space percentages.")]
	public float TopMargin = 0.45f;//0.25f

	[Range(0f, 0.45f)]
	[Tooltip("Bottom margin in screen-space percentages.")]
	public float BottomMargin = 0.45f;//0.1f;

	[Range(0f, 0.45f)]
	[Tooltip("Left and right side margins in screen-space percentages.")]
	public float SideMargin = 0.3f;//0.2f;

	[Range(20f, 90f)]
	[Tooltip("Elevation angle, 20 = first-person, 90 = top-down.")]
	public float ElevationAngle = 58f;
}
