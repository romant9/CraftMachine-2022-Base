using UnityEngine;

public class MultiParticleSystemAutoDestroy : MonoBehaviour
{
	private ParticleSystem[] particleSystemComponents;

	[SerializeField]
	[Tooltip("Destroy game object if it is disable.")]
	private bool destroyOnDisable;

	public void Start()
	{
		particleSystemComponents = GetComponentsInChildren<ParticleSystem>();
	}

	public void Update()
	{
		if (particleSystemComponents == null)
		{
			return;
		}
		bool flag = true;
		ParticleSystem[] array = particleSystemComponents;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].IsAlive())
			{
				flag = false;
				break;
			}
		}
		if (flag)
		{
			particleSystemComponents = null;
			Object.Destroy(base.gameObject);
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
