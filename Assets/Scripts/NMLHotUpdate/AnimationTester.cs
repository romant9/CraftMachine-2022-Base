using UnityEngine;

public class AnimationTester : MonoBehaviour
{
	protected AnimatorOverrideController AnimatorOverrideController => GetComponent<Animator>().runtimeAnimatorController as AnimatorOverrideController;

	public bool IsValid
	{
		get
		{
			if (Animator != null)
			{
				return Animator.gameObject.activeInHierarchy;
			}
			return false;
		}
	}

	protected Animator Animator { get; set; }

	private void Start()
	{
	}

	private void Update()
	{
	}
}
