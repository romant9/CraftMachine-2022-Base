using UnityEngine;

public class UITweenCameraData
{
	public Vector3 position;

	public float cameraSize;

	public bool instant;

	public UITweenCameraData(Vector3 position, float cameraSize = -1f, bool instant = false)
	{
		this.position = position;
		this.instant = instant;
		this.cameraSize = cameraSize;
	}

	public UITweenCameraData(Vector3 position, bool instant = false)
	{
		this.position = position;
		this.instant = instant;
	}
}
