using System;
using System.Collections.Generic;
using UnityEngine;

public class UIButtonExtended : UIButton
{
	public delegate void OnClickCallback(UIButtonExtended button);

	public const float PressAndHoldThresholdDuration = 0.5f;

	public string id = "";

	private OnClickCallback OnClickDelegate;

	private OnClickCallback OnPressedDelegate;

	private OnClickCallback OnPressedAndHoldDelegate;

	private OnClickCallback OnDragOverDelegate;

	private bool isVisuallyDisabled;

	private string normalSpriteName = "";

	private string disabledSpriteName = "";

	private Color defaultNormalColor = Color.white;

	private Color defaultDisabledColor = Color.grey;

	private float presStartedTime = -1f;

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
					normalSpriteName = base.normalSprite;
					disabledSpriteName = disabledSprite;
					base.normalSprite = disabledSprite;
					defaultNormalColor = base.defaultColor;
					defaultDisabledColor = disabledColor;
					base.defaultColor = disabledColor;
				}
				else
				{
					base.normalSprite = normalSpriteName;
					disabledSprite = disabledSpriteName;
					base.defaultColor = defaultNormalColor;
					disabledColor = defaultDisabledColor;
				}
			}
		}
	}

	public void SetClickCallback(OnClickCallback callback)
	{
		RemoveClickCallback(callback);
		OnClickDelegate = (OnClickCallback)Delegate.Combine(OnClickDelegate, callback);
	}

	public void RemoveClickCallback(OnClickCallback callback)
	{
		OnClickDelegate = (OnClickCallback)Delegate.Remove(OnClickDelegate, callback);
	}

	public void SetOnPressCallback(OnClickCallback callback)
	{
		RemoveOnPressCallback(callback);
		OnPressedDelegate = (OnClickCallback)Delegate.Combine(OnPressedDelegate, callback);
	}

	public void RemoveOnPressCallback(OnClickCallback callback)
	{
		OnPressedDelegate = (OnClickCallback)Delegate.Remove(OnPressedDelegate, callback);
	}

	public void SetOnPressAndHoldCallback(OnClickCallback callback)
	{
		RemoveOnPressCallback(callback);
		OnPressedAndHoldDelegate = (OnClickCallback)Delegate.Combine(OnPressedAndHoldDelegate, callback);
	}

	public void RemoveOnPressAndHoldCCallback(OnClickCallback callback)
	{
		OnPressedAndHoldDelegate = (OnClickCallback)Delegate.Remove(OnPressedAndHoldDelegate, callback);
	}

	public void SetOnDragOverCallback(OnClickCallback callback)
	{
		RemoveOnDragOverCallback(callback);
		OnDragOverDelegate = (OnClickCallback)Delegate.Combine(OnDragOverDelegate, callback);
	}

	public void RemoveOnDragOverCallback(OnClickCallback callback)
	{
		OnDragOverDelegate = (OnClickCallback)Delegate.Remove(OnDragOverDelegate, callback);
	}

	public virtual void Clear()
	{
		OnClickDelegate = null;
		OnPressedAndHoldDelegate = null;
		OnPressedDelegate = null;
		OnDragOverDelegate = null;
	}

	protected override void OnClick()
	{
		base.OnClick();
		if (OnClickDelegate != null)
		{
			OnClickDelegate(this);
		}
	}

	protected override void OnPress(bool isPressed)
	{
		base.OnPress(isPressed);
		if (OnPressedDelegate != null)
		{
			OnPressedDelegate(this);
		}
		if (isPressed && OnPressedAndHoldDelegate != null)
		{
			StartPressDurationTracking();
		}
		else
		{
			StopPressDurationTracking();
		}
	}

	protected virtual void OnPressAndHold()
	{
		StopPressDurationTracking();
		if (OnPressedAndHoldDelegate == null)
		{
			return;
		}
		OnPressedAndHoldDelegate(this);
		List<UICamera.MouseOrTouch> activeTouches = UICamera.activeTouches;
		for (int i = 0; i < activeTouches.Count; i++)
		{
			if (activeTouches[i] != null)
			{
				activeTouches[i].current = null;
				activeTouches[i].pressed = null;
				activeTouches[i].dragged = null;
			}
		}
	}

	protected override void OnDragOver()
	{
		base.OnDragOver();
		if (OnDragOverDelegate != null)
		{
			OnDragOverDelegate(this);
		}
		StopPressDurationTracking();
	}

	private void Update()
	{
		if (presStartedTime > -1f)
		{
			presStartedTime += Time.deltaTime;
		}
		if (presStartedTime >= 0.5f)
		{
			OnPressAndHold();
		}
	}

	private void StartPressDurationTracking()
	{
		presStartedTime = 0f;
	}

	private void StopPressDurationTracking()
	{
		presStartedTime = -1f;
	}
}
