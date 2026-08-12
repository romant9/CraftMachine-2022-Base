using System.Collections;
using UnityEngine;

public class SoundToggle : MonoBehaviour
{
	public string soundEvent;

	public static string DefaultSound = "global/ui_toggle";

	protected UIToggle toggleComponent;

	private EventDelegate playSoundDelegate;

	public void OnEnable()
	{
		StartCoroutine("DelayedDelegate");
	}

	protected IEnumerator DelayedDelegate()
	{
		yield return new WaitForEndOfFrame();
		toggleComponent = base.gameObject.GetComponent<UIToggle>();
		if (toggleComponent != null)
		{
			playSoundDelegate = new EventDelegate(PlaySound);
			toggleComponent.onChange.Insert(0, playSoundDelegate);
		}
	}

	public void OnDisable()
	{
		if (toggleComponent != null && playSoundDelegate != null)
		{
			toggleComponent.onChange.Remove(playSoundDelegate);
		}
	}

	public void PlaySound()
	{
		if (string.IsNullOrEmpty(soundEvent))
		{
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent(DefaultSound);
		}
		else
		{
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent(soundEvent);
		}
	}
}
