using System;
using UnityEngine;

public class MapCamTarget : MonoBehaviour
{
	[SerializeField]
	[Tooltip("Camera Object")]
	private GameObject cam;

	[SerializeField]
	[Tooltip("Object Offset From Camera")]
	private Vector3 offset;

	private void Start()
	{
	}

	private void Update()
	{
		Vector3 position = cam.transform.position;
		float num = Mathf.Tan(cam.transform.localEulerAngles.x * (MathF.PI / 180f));
		base.transform.position = new Vector3(position.x + offset.x, 0f, position.z + position.y / num + offset.z);
	}
}
