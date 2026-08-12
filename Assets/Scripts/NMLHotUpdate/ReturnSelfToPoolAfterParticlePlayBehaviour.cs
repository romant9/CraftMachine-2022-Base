using UnityEngine;

public class ReturnSelfToPoolAfterParticlePlayBehaviour : MonoBehaviour
{
	[SerializeField]
	private ParticleSystem particleSystem;

	private void Update()
	{
		if (!particleSystem || !particleSystem.isPlaying)
		{
			SingularityMonoBehaviour<ObjectPoolManager>.Instance.ReturnObjectToPool(base.gameObject);
		}
	}
}
