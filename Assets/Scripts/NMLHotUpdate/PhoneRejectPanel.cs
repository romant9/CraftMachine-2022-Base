using System.Collections;
using UnityEngine;

public class PhoneRejectPanel : MonoBehaviourExtended
{
	[SerializeField]
	private UIButtonExtended Button;

	[SerializeField]
	private UISprite Sprite;

	[SerializeField]
	private UILabel Label;

	[SerializeField]
	private float showSoundDelay;

	[SerializeField]
	private string showSoundEvent;

	public void SetClickCallback(UIButtonExtended.OnClickCallback callback)
	{
		if (Button != null)
		{
			Button.SetClickCallback(callback);
		}
	}

	public void SetRejectAmount(string content)
	{
		if (Label != null)
		{
			Label.text = content;
		}
	}

	public void Show(bool skipTween = false)
	{
		TweenManager.PlayTweenGroup(base.gameObject, 5, forward: true, null, skipTween);
	}

	protected IEnumerator PlaySound()
	{
		yield return new WaitForSeconds(showSoundDelay);
		if (SingularityMonoBehaviour<AudioManager>.Instance != null)
		{
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent(showSoundEvent);
		}
	}

	public void Hide()
	{
		base.gameObject.SetActive(value: false);
	}

	public override void Clear()
	{
		base.Clear();
		if ((bool)Button)
		{
			Button.Clear();
		}
	}
}
