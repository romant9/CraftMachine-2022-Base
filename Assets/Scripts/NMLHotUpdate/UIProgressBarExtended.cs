using Client.Tweener;
using UnityEngine;

public class UIProgressBarExtended : MonoBehaviourExtended
{
	[Header("Progress Bar")]
	[SerializeField]
	protected UIProgressBar progressBar;

	[SerializeField]
	protected UISprite progressBarSprite;

	[SerializeField]
	protected UILabel progressBarLabel;

	private Tweener tweener;

	public bool IsAnimating
	{
		get
		{
			if (tweener != null)
			{
				return tweener.animating;
			}
			return false;
		}
	}

	public Tweener CurrentTweener
	{
		get
		{
			return tweener;
		}
		set
		{
			tweener = value;
		}
	}

	public virtual void Update()
	{
		if (tweener != null && tweener.animating)
		{
			tweener.update();
			if (tweener != null)
			{
				SetProgress(tweener.progression.x);
			}
		}
	}

	public virtual void OnEnable()
	{
	}

	public virtual void OnDisable()
	{
	}

	public virtual void UpdateUI()
	{
	}

	public virtual void SetProgress(float progressValue)
	{
		if (progressBar != null)
		{
			progressBar.value = Mathf.Clamp01(progressValue);
		}
	}

	public virtual void SetTextToLabel(string value)
	{
		if (progressBarLabel != null)
		{
			progressBarLabel.text = value;
		}
	}

	public virtual float GetProgressValue()
	{
		if (progressBar != null)
		{
			return progressBar.value;
		}
		return 0f;
	}

	public virtual void TweenToProgress(float progressValue, float startValue = -1f, float duration = 1f, Easing.All easing = Easing.All.Linear)
	{
		if (IsNotNull(progressBar))
		{
			if (duration > 0f)
			{
				Vector4 vector = new Vector4
				{
					x = ((startValue > -1f) ? startValue : progressBar.value)
				};
				Vector4 to = new Vector4
				{
					x = progressValue
				};
				DebugLog("Staring Tween from: " + vector.x + " toVec: " + to.x + " duration: " + duration);
				tweener = new Tweener();
				tweener.easeFromTo(vector, to, duration, TweenerHelpers.getGetByEnum(easing), OnEasingComplete);
			}
			else
			{
				tweener = null;
				SetProgress(progressValue);
			}
		}
	}

	public override void Clear()
	{
		base.Clear();
		tweener = null;
	}

	protected virtual void OnEasingComplete()
	{
	}
}
