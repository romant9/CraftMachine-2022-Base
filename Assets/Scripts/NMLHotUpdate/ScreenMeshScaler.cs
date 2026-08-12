using UnityEngine;

public class ScreenMeshScaler : MonoBehaviour
{
	[SerializeField]
	private Camera cam;

	[SerializeField]
	private MeshRenderer meshRenderer;

	private Vector3 initialScale;

	private float meshAspectRatio;

	private void Awake()
	{
		initialScale = meshRenderer.transform.localScale;
		meshAspectRatio = initialScale.x / initialScale.y;
	}

	private void Update()
	{
		Refresh();
	}

	private void Refresh()
	{
		float num = (float)Screen.width / (float)Screen.height;
		meshRenderer.transform.localScale = ((num > meshAspectRatio) ? (num / meshAspectRatio) : 1f) * initialScale;
	}
}
