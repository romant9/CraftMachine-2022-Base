using UnityEngine;

public class CustomGizmo : MonoBehaviour
{
	public string gizmoName;

	public void OnDrawGizmos()
	{
		Gizmos.DrawIcon(base.transform.position, gizmoName);
	}
}
