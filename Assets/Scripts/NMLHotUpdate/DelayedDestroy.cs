using System.Collections;
using UnityEngine;

public class DelayedDestroy : MonoBehaviour
{
	[SerializeField]
	[Tooltip("The delay before the destruction.")]
	private float delay;

	[SerializeField]
	[Tooltip("Destroy game object if it is disable.")]
	private bool destroyOnDisable;

	private void Start()
	{
		StartCoroutine(WaitAndDestroy());
	}

	private IEnumerator WaitAndDestroy()
	{
		yield return new WaitForSeconds(delay);
		Object.Destroy(base.gameObject);
	}

	private void OnDisable()
	{
		if (destroyOnDisable)
		{
			Object.Destroy(base.gameObject);
		}
	}
}
