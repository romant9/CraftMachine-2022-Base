using UnityEngine;

public class InhabitantLegacyAnimationController : MonoBehaviour
{
	[SerializeField]
	private Animation legacyAnimationController;

	private const string WALK_ANIMATION_NAME = "Inhabitant_Walk";

	private const string IDLE_ANIMATION_NAME = "Inhabitant_Idle";

	private const string PICKLOCK_ANIMATION_NAME = "Inhabitant_PickLock";

	public float LastDeltaMovementMagnitude;

	public void StartMove()
	{
		legacyAnimationController.CrossFade("Inhabitant_Walk");
	}

	public void StopMove()
	{
		legacyAnimationController.CrossFade("Inhabitant_Idle");
	}

	public void StartCustomAnimation()
	{
		legacyAnimationController.CrossFade("Inhabitant_PickLock");
	}

	public void StopCustomAnimation()
	{
		StopMove();
	}
}
