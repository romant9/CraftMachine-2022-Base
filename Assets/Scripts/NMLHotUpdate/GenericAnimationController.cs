using UnityEngine;

public class GenericAnimationController : MonoBehaviour
{
	private Animator anim;

	public float MinSpeed = 0.2f;

	public float MaxSpeed = 1.2f;

	public float MinOffset = 0.6f;

	public float MaxOffset = 0.6f;

	public bool PlaySound;

	private void Start()
	{
		anim = base.gameObject.GetComponent<Animator>();
		if (anim != null)
		{
			anim.speed = (MaxSpeed - MinSpeed) * Random.value + MinSpeed;
			anim.Update((MaxOffset - MinOffset) * Random.value + MinOffset);
		}
	}

	private void OnPlaySound(string soundEventName)
	{
		if (!(GameManager.Instance == null) && GameManager.Instance.playerModel != null && GameManager.Instance.playerModel.Combat != null && SingularityMonoBehaviour<AudioManager>.Instance != null && PlaySound)
		{
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent(soundEventName, base.gameObject);
		}
	}
}
