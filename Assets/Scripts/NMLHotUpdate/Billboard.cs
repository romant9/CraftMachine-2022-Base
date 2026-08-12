using UnityEngine;

public class Billboard : MonoBehaviour
{
	private void Update()
	{
		Vector3 forward = -Camera.main.transform.forward;
		base.transform.rotation = Quaternion.LookRotation(forward, Camera.main.transform.up);
	}
}
