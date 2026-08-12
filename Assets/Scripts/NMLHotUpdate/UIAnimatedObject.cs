using System;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class UIAnimatedObject : MonoBehaviourExtended
{
	public delegate void CompleteCallback(UIAnimatedObject target);

	private Animator currentAnimator;

	private int currentAnimatorState = -1;

	private CompleteCallback OnCompleteDelegate;

	private void Awake()
	{
		DebugIdString = "UIAnimatedObject";
	}

	protected virtual void OnEnable()
	{
		UpdateAnimator();
	}

	public void SetBoolToAnimation(int hash, bool value)
	{
		UpdateAnimator();
		if (currentAnimator != null)
		{
			currentAnimator.enabled = true;
			currentAnimator.SetBool(hash, value);
			currentAnimatorState = hash;
		}
		else
		{
			DebugLogError("Could not find Animator in object: " + base.gameObject.name);
		}
	}

	public void SetTriggerToAnimation(int hash)
	{
		UpdateAnimator();
		if (currentAnimator != null)
		{
			currentAnimator.enabled = true;
			currentAnimator.SetTrigger(hash);
			currentAnimatorState = hash;
		}
		else
		{
			DebugLogError("Could not find Animator in object: " + base.gameObject.name);
		}
	}

	public int LastSetState()
	{
		return currentAnimatorState;
	}

	public void OnAnimationCompletedHandler()
	{
		if (OnCompleteDelegate != null)
		{
			OnCompleteDelegate(this);
		}
	}

	public void SetCompleteCallback(CompleteCallback callback)
	{
		OnCompleteDelegate = (CompleteCallback)Delegate.Combine(OnCompleteDelegate, callback);
	}

	public void RemoveOnCompleteCallback(CompleteCallback callback)
	{
		OnCompleteDelegate = (CompleteCallback)Delegate.Remove(OnCompleteDelegate, callback);
	}

	public override void Clear()
	{
		base.Clear();
		OnCompleteDelegate = null;
	}

	private void UpdateAnimator()
	{
		if (currentAnimator == null)
		{
			Animator[] componentsInChildren = base.gameObject.GetComponentsInChildren<Animator>(includeInactive: true);
			if (componentsInChildren.Length != 0)
			{
				currentAnimator = componentsInChildren[0];
			}
		}
	}
}
