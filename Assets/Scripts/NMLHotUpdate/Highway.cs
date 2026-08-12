using UnityEngine;

public class Highway : MonoBehaviour
{
	public float EnterAnimationDurationSeconds;

	public float ExitAnimationDurationSeconds;

	[SerializeField]
	private Animator enterCampAnimator;

	[SerializeField]
	private Animator exitCampAnimator;

	[SerializeField]
	private GameObject enterCampSpot;

	[SerializeField]
	private GameObject exitCampSpot;

	public GameObject ExitCampSpot => exitCampSpot;

	private void OnEnable()
	{
		enterCampAnimator.gameObject.SetActive(value: false);
	}

	public void PlayEnterCampAnimation()
	{
		enterCampAnimator.gameObject.SetActive(value: true);
		enterCampAnimator.SetTrigger("Start");
	}

	public void PlayExitCampAnimation()
	{
		exitCampAnimator.SetTrigger("Start");
	}
}
