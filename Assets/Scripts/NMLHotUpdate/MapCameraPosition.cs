using UnityEngine;

public class MapCameraPosition
{
	public Vector3 SavedCameraTarget { get; set; }

	public float SavedCameraDistance { get; set; }

	public MapCameraSaveReason MapCameraSaveReason { get; set; }
}
