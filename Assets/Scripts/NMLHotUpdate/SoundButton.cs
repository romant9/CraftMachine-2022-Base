using UnityEngine;

public class SoundButton : MonoBehaviour
{
	public string soundEvent;

	public static string DefaultSound = "global/ui_click";

	protected UIButton buttonComponent;

	private EventDelegate playSoundDelegate;

	public void OnEnable()
	{
		buttonComponent = base.gameObject.GetComponent<UIButton>();
		if (buttonComponent != null)
		{
			playSoundDelegate = new EventDelegate(PlaySound);
			buttonComponent.onClick.Insert(0, playSoundDelegate);
		}
	}

	public void OnDisable()
	{
		if (buttonComponent != null && playSoundDelegate != null)
		{
			buttonComponent.onClick.Remove(playSoundDelegate);
		}
	}

	public void PlaySound()
	{
		if (!(SingularityMonoBehaviour<AudioManager>.Instance == null))
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
}
