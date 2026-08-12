using System;
using UnityEngine;

public class AnimatedUIButtonExtended : UIButtonExtended
{
	private Animator currentAnimator;

	private int currentAnimatorState = -1;

	private OnClickCallback OnCompleteDelegate;

	protected override void OnEnable()
	{
		base.OnEnable();
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
			Debug.LogError("Could not find Animator in object: " + base.gameObject.name);
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
			Debug.LogError("Could not find Animator in object: " + base.gameObject.name);
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

	public void SetCompleteCallback(OnClickCallback callback)
	{
		RemoveOnCompleteCallback(callback);
		OnCompleteDelegate = (OnClickCallback)Delegate.Combine(OnCompleteDelegate, callback);
	}

	public void RemoveOnCompleteCallback(OnClickCallback callback)
	{
		OnCompleteDelegate = (OnClickCallback)Delegate.Remove(OnCompleteDelegate, callback);
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
