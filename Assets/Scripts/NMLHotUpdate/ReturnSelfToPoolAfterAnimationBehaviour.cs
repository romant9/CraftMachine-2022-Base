using UnityEngine;

public class ReturnSelfToPoolAfterAnimationBehaviour : MonoBehaviour
{
	[SerializeField]
	private Animator animator;

	private void Update()
	{
		if (!animator || animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.999f)
		{
			SingularityMonoBehaviour<ObjectPoolManager>.Instance.ReturnObjectToPool(base.gameObject);
		}
	}
}
