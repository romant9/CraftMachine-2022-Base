using System.Collections;
using UnityEngine;

public class TweenSound : MonoBehaviour
{
	public float delay;

	public string soundEventName = "global/ui_click";

	[SerializeField]
	private bool shouldTrackTweener;

	public void OnEnable()
	{
		if (shouldTrackTweener)
		{
			GetTweenDelay();
		}
		StartCoroutine(DelaySound(delay));
	}

	protected IEnumerator DelaySound(float soundDelay)
	{
		yield return new WaitForSeconds(soundDelay);
		if (SingularityMonoBehaviour<AudioManager>.Instance != null)
		{
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent(soundEventName);
		}
	}

	private void GetTweenDelay()
	{
		UITweener component = GetComponent<UITweener>();
		if (component != null)
		{
			delay = component.delay;
		}
	}
}
