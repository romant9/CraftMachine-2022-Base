using System;
using UnityEngine;

[Serializable]
public class TargetCameraParams
{
	[Tooltip("Type for this configuration.")]
	public ActionCameraType actionCameraType;

	[Range(1f, 40f)]
	[Tooltip("Distance from the target along the camera forward axis.")]
	public float distance = 5f;

	[Range(0f, 5f)]
	[Tooltip("Offset along Y axis from the targets transform.")]
	public float heightOffset = 1.6f;

	[Range(-90f, 90f)]
	[Tooltip("Pitch around the target.")]
	public float pitch = 30f;

	[Range(0f, 360f)]
	[Tooltip("Yaw around the target.")]
	public float yaw = 40f;

	[Range(20f, 160f)]
	[Tooltip("FoV at target.")]
	public float fov = 40f;

	[Range(0f, 10f)]
	[Tooltip("Interpolation time from current camera to this specification.")]
	public float interpolationTime = 0.75f;

	[Range(-1f, 10f)]
	[Tooltip("Delay in seconds before proceeding, -1 will want until outside signal.")]
	public float endDelay = -1f;

	[Tooltip("Is start tween enabled, if not then camera cut will be used to go to target parameters.")]
	public bool startTweenEnabled;

	[Tooltip("Is end tween enabled, if not then camera cut will be used to get back to original parameters.")]
	public bool endTweenEnabled;

	[Tooltip("Curve for interpolation of values")]
	public AnimationCurve interpolationCurve;
}
