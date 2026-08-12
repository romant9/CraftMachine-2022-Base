using UnityEngine;

public class ParticleSystemAutoDestroy : MonoBehaviour
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
		if (particleSystemComponent != null && !particleSystemComponent.IsAlive(withChildren: true))
		{
			Object.Destroy(base.gameObject);
			particleSystemComponent = null;
		}
	}

	private void OnDisable()
	{
		if (destroyOnDisable)
		{
			Object.Destroy(base.gameObject);
		}
	}
}
