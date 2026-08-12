using UnityEngine;

public class AnimationSetSpeed : MonoBehaviour
{
	private Animator anim;

	public float MinSpeed = 0.2f;

	public float MaxSpeed = 1.2f;

	private void Start()
	{
		anim = base.gameObject.GetComponent<Animator>();
		if (anim != null)
		{
			anim.speed = (MaxSpeed - MinSpeed) * Random.value + MinSpeed;
		}
	}
}
