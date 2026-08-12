using UnityEngine;

public class AnimationSound : MonoBehaviour
{
	public void PlaySound(string eventName)
	{
		if (SingularityMonoBehaviour<AudioManager>.Instance != null)
		{
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent(eventName);
		}
	}
}
