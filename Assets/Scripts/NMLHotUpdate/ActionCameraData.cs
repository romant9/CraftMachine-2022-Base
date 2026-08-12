using System.Collections.Generic;
using UnityEngine;

public class ActionCameraData : ScriptableObject
{
	[SerializeField]
	[Tooltip("Cooldown time in seconds for the action camera. When action camera triggers it cannot trigger again until cooldown is over.")]
	public float CooldownTime;

	[SerializeField]
	[Tooltip("Target camera parameters")]
	public List<TargetCameraParams> TargetCameraParams;
}
