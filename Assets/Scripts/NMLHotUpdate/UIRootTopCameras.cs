using UnityEngine;

public class UIRootTopCameras : MonoBehaviour
{
	private void Awake()
	{
		var roots = FindObjectsOfType<UIRootTopCameras>();
		if (roots != null && roots.Length > 1)
		{
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		Object.DontDestroyOnLoad(base.gameObject);
	}
}
