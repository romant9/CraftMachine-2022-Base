using UnityEngine;

public class DestroyOnRequest : MonoBehaviour
{
	public void RequestDestroy()
	{
		Object.Destroy(base.gameObject);
	}
}
