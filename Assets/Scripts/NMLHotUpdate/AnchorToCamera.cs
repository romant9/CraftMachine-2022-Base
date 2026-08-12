using UnityEngine;

[ExecuteInEditMode]
public class AnchorToCamera : MonoBehaviour
{
	private void Start()
	{
		SetAnchor();
	}

	private void SetAnchor()
	{
		UIWidget component = GetComponent<UIWidget>();
		if (component != null)
		{
			UICamera uICamera = Object.FindObjectOfType<UICamera>();
			if (uICamera != null)
			{
				component.SetAnchor(uICamera.gameObject, 0, 0, 0, 0);
				component.ResetAndUpdateAnchors();
			}
		}
	}

	private void Update()
	{
		if (Application.isEditor && !Application.isPlaying)
		{
			SetAnchor();
		}
	}
}
