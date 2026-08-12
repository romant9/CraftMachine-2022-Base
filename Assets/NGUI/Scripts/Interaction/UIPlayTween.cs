using System;
using System.Collections.Generic;
using AnimationOrTween;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("NGUI/Interaction/Play Tween")]
public class UIPlayTween : MonoBehaviour
{
	public static UIPlayTween current;

	public GameObject tweenTarget;

	public int tweenGroup;

	public Trigger trigger;

	public Direction playDirection = Direction.Forward;

	public bool resetOnPlay;

	public bool resetIfDisabled;

	[Tooltip("If true, Play Tween will reset all associated tweens to their starting state at the very start, before activation triggers")]
	public bool setState;

	[Tooltip("Starting factor to assume, 0 being the start and 1 being the end")]
	[Range(0f, 1f)]
	public float startState;

	public EnableCondition ifDisabledOnPlay;

	public DisableCondition disableWhenFinished;

	public bool includeChildren;

	public List<EventDelegate> onFinished = new List<EventDelegate>();

	[HideInInspector]
	[SerializeField]
	private GameObject eventReceiver;

	[HideInInspector]
	[SerializeField]
	private string callWhenFinished;

	private UITweener[] mTweens;

	private bool mStarted;

	private int mActive;

	private bool mActivated;

	private void Awake()
	{
		if (eventReceiver != null && EventDelegate.IsValid(onFinished))
		{
			eventReceiver = null;
			callWhenFinished = null;
		}
	}

	private void Start()
	{
		mStarted = true;
		if (tweenTarget == null)
		{
			tweenTarget = base.gameObject;
		}
		if (!setState || !Application.isPlaying)
		{
			return;
		}
		GameObject gameObject = ((tweenTarget == null) ? base.gameObject : tweenTarget);
		mTweens = (includeChildren ? gameObject.GetComponentsInChildren<UITweener>() : gameObject.GetComponents<UITweener>());
		if (mTweens.Length == 0)
		{
			if (disableWhenFinished != 0)
			{
				NGUITools.SetActive(tweenTarget, state: false);
			}
			return;
		}
		bool flag = true;
		if (playDirection == Direction.Reverse)
		{
			flag = !flag;
		}
		int i = 0;
		for (int num = mTweens.Length; i < num; i++)
		{
			UITweener uITweener = mTweens[i];
			if (uITweener.tweenGroup == tweenGroup)
			{
				uITweener.Play(flag ? (startState == 1f) : (startState != 1f));
				uITweener.Sample(flag ? startState : (1f - startState), isFinished: true);
				uITweener.enabled = false;
			}
		}
	}

	private void OnEnable()
	{
		if (mStarted)
		{
			OnHover(UICamera.IsHighlighted(base.gameObject));
		}
		if (UICamera.currentTouch != null)
		{
			if (trigger == Trigger.OnPress || trigger == Trigger.OnPressTrue)
			{
				mActivated = UICamera.currentTouch.pressed == base.gameObject;
			}
			if (trigger == Trigger.OnHover || trigger == Trigger.OnHoverTrue)
			{
				mActivated = UICamera.currentTouch.current == base.gameObject;
			}
		}
		UIToggle component = GetComponent<UIToggle>();
		if (component != null)
		{
			EventDelegate.Add(component.onChange, OnToggle);
		}
	}

	private void OnDisable()
	{
		UIToggle component = GetComponent<UIToggle>();
		if (component != null)
		{
			EventDelegate.Remove(component.onChange, OnToggle);
		}
	}

	private void OnDragOver()
	{
		if (trigger == Trigger.OnHover)
		{
			OnHover(isOver: true);
		}
	}

	private void OnHover(bool isOver)
	{
		if (!base.enabled || (trigger != Trigger.OnHover && !(trigger == Trigger.OnHoverTrue && isOver) && (trigger != Trigger.OnHoverFalse || isOver)) || isOver == mActivated)
		{
			return;
		}
		if (!isOver && UICamera.hoveredObject != null && UICamera.hoveredObject.transform.IsChildOf(base.transform))
		{
			UICamera.onHover = (UICamera.BoolDelegate)Delegate.Combine(UICamera.onHover, new UICamera.BoolDelegate(CustomHoverListener));
			isOver = true;
			if (mActivated)
			{
				return;
			}
		}
		mActivated = isOver && trigger == Trigger.OnHover;
		Play(isOver);
	}

	private void CustomHoverListener(GameObject go, bool isOver)
	{
		if ((bool)this)
		{
			GameObject gameObject = base.gameObject;
			if (!gameObject || !go || (!(go == gameObject) && !go.transform.IsChildOf(base.transform)))
			{
				OnHover(isOver: false);
				UICamera.onHover = (UICamera.BoolDelegate)Delegate.Remove(UICamera.onHover, new UICamera.BoolDelegate(CustomHoverListener));
			}
		}
	}

	private void OnDragOut()
	{
		if (base.enabled && mActivated)
		{
			mActivated = false;
			Play(forward: false);
		}
	}

	private void OnPress(bool isPressed)
	{
		if (base.enabled && (trigger == Trigger.OnPress || (trigger == Trigger.OnPressTrue && isPressed) || (trigger == Trigger.OnPressFalse && !isPressed)))
		{
			mActivated = isPressed && trigger == Trigger.OnPress;
			Play(isPressed);
		}
	}

	private void OnClick()
	{
		if (base.enabled && trigger == Trigger.OnClick)
		{
			Play(forward: true);
		}
	}

	private void OnDoubleClick()
	{
		if (base.enabled && trigger == Trigger.OnDoubleClick)
		{
			Play(forward: true);
		}
	}

	private void OnSelect(bool isSelected)
	{
		if (base.enabled && (trigger == Trigger.OnSelect || (trigger == Trigger.OnSelectTrue && isSelected) || (trigger == Trigger.OnSelectFalse && !isSelected)))
		{
			mActivated = isSelected && trigger == Trigger.OnSelect;
			Play(isSelected);
		}
	}

	private void OnToggle()
	{
		if (base.enabled && !(UIToggle.current == null) && (trigger == Trigger.OnActivate || (trigger == Trigger.OnActivateTrue && UIToggle.current.value) || (trigger == Trigger.OnActivateFalse && !UIToggle.current.value)))
		{
			Play(UIToggle.current.value);
		}
	}

	private void Update()
	{
		if (disableWhenFinished == DisableCondition.DoNotDisable || mTweens == null)
		{
			return;
		}
		bool flag = true;
		bool flag2 = true;
		int i = 0;
		for (int num = mTweens.Length; i < num; i++)
		{
			UITweener uITweener = mTweens[i];
			if (uITweener.tweenGroup == tweenGroup)
			{
				if (uITweener.enabled)
				{
					flag = false;
					break;
				}
				if (uITweener.direction != (Direction)disableWhenFinished)
				{
					flag2 = false;
				}
			}
		}
		if (flag)
		{
			if (flag2)
			{
				NGUITools.SetActive(tweenTarget, state: false);
			}
			mTweens = null;
		}
	}

	[ContextMenu("Stop")]
	public void Stop()
	{
		if (mTweens != null)
		{
			UITweener[] array = mTweens;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Finish();
			}
		}
	}

	[ContextMenu("Play")]
	public void Play()
	{
		Play(forward: true);
	}

	[ContextMenu("Play in reverse")]
	public void PlayReverse()
	{
		Play(forward: false);
	}

	public void Play(bool forward)
	{
		mActive = 0;
		GameObject gameObject = ((tweenTarget == null) ? base.gameObject : tweenTarget);
		if (!NGUITools.GetActive(gameObject))
		{
			if (ifDisabledOnPlay != EnableCondition.EnableThenPlay)
			{
				return;
			}
			NGUITools.SetActive(gameObject, state: true);
		}
		mTweens = (includeChildren ? gameObject.GetComponentsInChildren<UITweener>() : gameObject.GetComponents<UITweener>());
		if (mTweens.Length == 0)
		{
			if (disableWhenFinished != 0)
			{
				NGUITools.SetActive(tweenTarget, state: false);
			}
			return;
		}
		bool flag = false;
		if (playDirection == Direction.Reverse)
		{
			forward = !forward;
		}
		int i = 0;
		for (int num = mTweens.Length; i < num; i++)
		{
			UITweener uITweener = mTweens[i];
			if (uITweener.tweenGroup != tweenGroup)
			{
				continue;
			}
			if (!flag && !NGUITools.GetActive(gameObject))
			{
				flag = true;
				NGUITools.SetActive(gameObject, state: true);
			}
			mActive++;
			if (playDirection == Direction.Toggle)
			{
				EventDelegate.Add(uITweener.onFinished, OnFinished, oneShot: true);
				uITweener.Toggle();
				continue;
			}
			if (resetOnPlay || (resetIfDisabled && !uITweener.enabled))
			{
				uITweener.Play(forward);
				uITweener.ResetToBeginning();
			}
			EventDelegate.Add(uITweener.onFinished, OnFinished, oneShot: true);
			uITweener.Play(forward);
		}
	}

	private void OnFinished()
	{
		if (--mActive == 0 && current == null)
		{
			current = this;
			EventDelegate.Execute(onFinished);
			if (eventReceiver != null && !string.IsNullOrEmpty(callWhenFinished))
			{
				eventReceiver.SendMessage(callWhenFinished, SendMessageOptions.DontRequireReceiver);
			}
			eventReceiver = null;
			current = null;
		}
	}
}
