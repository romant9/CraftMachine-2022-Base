using UnityEngine;

public class OffsetTowardsCamera : MonoBehaviour
{
	[SerializeField]
	private float offset;

	private new Transform camera;

	private Transform trans;

	private Transform parent;

	private void Start()
	{
		camera = Camera.main.transform;
		trans = base.transform;
		parent = trans.parent;
	}

	private void Update()
	{
		trans.position = Vector3.MoveTowards(parent.position, camera.position, offset);
	}
}
