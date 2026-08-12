using System;
using UnityEngine;

public class UILabelColor : UILabel
{
	public Color hover = new Color(0.88235295f, 40f / 51f, 0.5882353f, 1f);

	public Color pressed = new Color(61f / 85f, 0.6392157f, 41f / 85f, 1f);

	public Color disabledColor = Color.grey;

	public float duration = 0.2f;

	[NonSerialized]
	protected UIButtonColor.State currentState;

	[NonSerialized]
	protected Color mDefaultColor;

	[NonSerialized]
	protected bool mInitDone;

	private bool isVisuallyDisabled;

	private Color defaultNormalColor = Color.white;

	private Color defaultDisabledColor = Color.grey;

	public UIButtonColor.State state
	{
		get
		{
			return currentState;
		}
		set
		{
			SetState(value, instant: false);
		}
	}

	public Color defaultColor
	{
		get
		{
			if (!mInitDone)
			{
				OnInitLabel();
			}
			return mDefaultColor;
		}
		set
		{
			if (!mInitDone)
			{
				OnInitLabel();
			}
			mDefaultColor = value;
			UIButtonColor.State state = currentState;
			currentState = UIButtonColor.State.Disabled;
			SetState(state, instant: false);
		}
	}

	public virtual bool IsVisuallyDisabled
	{
		get
		{
			return isVisuallyDisabled;
		}
		set
		{
			if (value != isVisuallyDisabled)
			{
				isVisuallyDisabled = value;
				if (isVisuallyDisabled)
				{
					defaultNormalColor = defaultColor;
					defaultDisabledColor = disabledColor;
					defaultColor = disabledColor;
				}
				else
				{
					defaultColor = defaultNormalColor;
					disabledColor = defaultDisabledColor;
				}
			}
		}
	}

	protected void OnInitLabel()
	{
		mInitDone = true;
		mDefaultColor = base.color;
	}

	public virtual void SetState(UIButtonColor.State state, bool instant)
	{
		if (!mInitDone)
		{
			OnInitLabel();
		}
		if (currentState != state)
		{
			currentState = state;
			UpdateColor(instant);
		}
	}

	public void UpdateColor(bool instant)
	{
		if (base.gameObject != null)
		{
			TweenColor tweenColor = currentState switch
			{
				UIButtonColor.State.Hover => TweenColor.Begin(base.gameObject, duration, hover), 
				UIButtonColor.State.Pressed => TweenColor.Begin(base.gameObject, duration, pressed), 
				UIButtonColor.State.Disabled => TweenColor.Begin(base.gameObject, duration, disabledColor), 
				_ => TweenColor.Begin(base.gameObject, duration, defaultColor), 
			};
			if (instant && tweenColor != null)
			{
				tweenColor.value = tweenColor.to;
				tweenColor.enabled = false;
			}
		}
	}
}
