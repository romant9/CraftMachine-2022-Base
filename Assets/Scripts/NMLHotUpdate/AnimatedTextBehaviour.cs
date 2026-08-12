using UnityEngine;

public class AnimatedTextBehaviour : MonoBehaviour
{
	[SerializeField]
	private UILabel label;

	[SerializeField]
	private int showTweenGroup;

	[SerializeField]
	private int hideTweenGroup;

	private bool shown;

	private void Awake()
	{
		if (!shown)
		{
			base.gameObject.SetActive(value: false);
		}
	}

	public void Show(string text)
	{
		label.text = text;
		if (!shown)
		{
			shown = true;
			base.gameObject.SetActive(value: true);
			TweenManager.RemoveCallback(base.gameObject, hideTweenGroup, HideCallback);
			TweenManager.FinishTweenGroup(base.gameObject, hideTweenGroup);
			TweenManager.PlayTweenGroup(base.gameObject, showTweenGroup);
		}
	}

	public void Hide()
	{
		if (shown)
		{
			TweenManager.FinishTweenGroup(base.gameObject, showTweenGroup);
			TweenManager.PlayTweenGroup(base.gameObject, hideTweenGroup, forward: true, HideCallback);
			shown = false;
		}
	}

	private void HideCallback()
	{
		base.gameObject.SetActive(value: false);
	}
}
