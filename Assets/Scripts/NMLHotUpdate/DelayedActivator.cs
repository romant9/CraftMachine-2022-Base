using System;
using UnityEngine;

public class DelayedActivator : MonoBehaviour
{
	[SerializeField]
	private float delay;

	private IDisposable disposable;

	private void Start()
	{
		Helpers.GameObjectSetActive(base.gameObject, value: false);
		disposable = GameManager.Instance.TimingManager.Timer(TimeSpan.FromSeconds(delay), delegate
		{
			Helpers.GameObjectSetActive(base.gameObject, value: true);
		});
	}

	private void OnDestroy()
	{
		disposable?.Dispose();
	}
}
