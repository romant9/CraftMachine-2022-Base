using System;

[Serializable]
public enum ActionCameraState
{
	Idle = 0,
	InterpolatingToTarget = 1,
	AtTarget = 2,
	InterpolatingToOriginal = 3
}
