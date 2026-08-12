using UnityEngine;

public class TweenPrefab : UITweener
{
	[SerializeField]
	private GameObject prefab;

	[SerializeField]
	private bool addInUi;

	private bool hasSpawned;

	protected override void Start()
	{
		base.Start();
		hasSpawned = false;
	}

	protected override void OnUpdate(float factor, bool isFinished)
	{
		if (hasSpawned)
		{
			return;
		}
		hasSpawned = true;
		if (addInUi)
		{
			return;
		}
		Camera camera = null;
		Camera[] allCameras = Camera.allCameras;
		foreach (Camera camera2 in allCameras)
		{
			if ((camera == null || camera2.depth > camera.depth) && camera2.gameObject.layer != 5)
			{
				camera = camera2;
			}
		}
		GameObject obj = Helpers.InstantiateToParent(prefab, camera.gameObject.transform.parent.gameObject);
		Vector3 position = NGUITools.FindCameraForLayer(base.gameObject.layer).WorldToViewportPoint(base.transform.position);
		position = camera.ViewportToWorldPoint(position);
		Transform parent = obj.transform.parent;
		obj.transform.localPosition = parent.InverseTransformPoint(position);
		obj.transform.localPosition = position;
	}
}
