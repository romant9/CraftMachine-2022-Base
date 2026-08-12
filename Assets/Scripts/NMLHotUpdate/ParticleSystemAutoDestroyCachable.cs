using UnityEngine;

public class ParticleSystemAutoDestroyCachable : CacheableObject
{
	private ParticleSystem particleSystemComponent;

	[SerializeField]
	[Tooltip("Destroy game object if it is disable.")]
	private bool destroyOnDisable;

	public void Start()
	{
		particleSystemComponent = GetComponent<ParticleSystem>();
	}

	public void Update()
	{
		if (particleSystemComponent != null && !particleSystemComponent.IsAlive())
		{
			Helpers.DestroyOrCache(this);
		}
	}

	private void OnDisable()
	{
		if (destroyOnDisable)
		{
			Helpers.DestroyOrCache(this);
		}
	}
}
