using UnityEngine;

public abstract class SingularityMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
	private static T instance;

	public static T Instance => instance;

	protected void Awake()
	{
		if (instance != null)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		instance = this as T;
		if (base.transform == base.transform.root)
		{
			Object.DontDestroyOnLoad(base.gameObject);
		}
		AwakeInternal();
	}

	protected virtual void AwakeInternal()
	{
	}
}
