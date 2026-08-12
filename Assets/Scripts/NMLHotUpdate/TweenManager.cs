using System;
using Client.Tweener;
using UnityEngine;

public class TweenManager
{
	public const int TweenGroupOpen = 1;

	public const int TweenGroupClose = 2;

	public static float PlayTweenGroup(GameObject objectRoot, int group, bool forward = true, EventDelegate.Callback callback = null, bool resetToEnd = false, float overrideDelay = -1f)
	{
		EventDelegate eventDelegate = null;
		if (callback != null)
		{
			eventDelegate = new EventDelegate(callback);
		}
		UITweener[] componentsInChildren = objectRoot.GetComponentsInChildren<UITweener>(includeInactive: false);
		UITweener uITweener = null;
		UITweener[] array = componentsInChildren;
		foreach (UITweener uITweener2 in array)
		{
			if (uITweener2.tweenGroup == group)
			{
				if (overrideDelay > -1f)
				{
					uITweener2.delay = overrideDelay;
				}
				if (uITweener == null || uITweener2.delay + uITweener2.duration > uITweener.delay + uITweener.duration)
				{
					uITweener = uITweener2;
				}
				if (uITweener2 is TweenScale && uITweener2.delay != 0f)
				{
					uITweener2.transform.localScale = Vector3.zero;
				}
				uITweener2.Play(forward);
				if (resetToEnd)
				{
					uITweener2.ResetToEnd();
				}
				else
				{
					uITweener2.ResetToBeginning();
				}
			}
		}
		if (eventDelegate != null)
		{
			if (uITweener != null)
			{
				uITweener.SetOnFinished(eventDelegate);
			}
			else
			{
				eventDelegate.Execute();
			}
		}
		if (uITweener != null && !resetToEnd)
		{
			return uITweener.duration;
		}
		return 0f;
	}

	public static void UpdateCallback(GameObject objectRoot, int group, EventDelegate.Callback callback)
	{
		EventDelegate onFinished = new EventDelegate(callback);
		UITweener[] componentsInChildren = objectRoot.GetComponentsInChildren<UITweener>(includeInactive: false);
		UITweener uITweener = null;
		UITweener[] array = componentsInChildren;
		foreach (UITweener uITweener2 in array)
		{
			if (uITweener2.tweenGroup == group && (uITweener == null || uITweener2.delay + uITweener2.duration > uITweener.delay + uITweener.duration))
			{
				uITweener = uITweener2;
			}
		}
		if (uITweener != null)
		{
			uITweener.SetOnFinished(onFinished);
		}
	}

	public static void FinishTweenGroup(GameObject objectRoot, int group, bool includeInactive = false)
	{
		if (!(objectRoot != null))
		{
			return;
		}
		UITweener[] componentsInChildren = objectRoot.GetComponentsInChildren<UITweener>(includeInactive);
		foreach (UITweener uITweener in componentsInChildren)
		{
			if (uITweener.tweenGroup == group && uITweener.tweenFactor != 1f)
			{
				uITweener.ResetToEnd();
			}
		}
	}

	public static void StopTweenGroup(GameObject objectRoot, int group, bool includeInactive = false)
	{
		if (!(objectRoot != null))
		{
			return;
		}
		UITweener[] componentsInChildren = objectRoot.GetComponentsInChildren<UITweener>(includeInactive);
		foreach (UITweener uITweener in componentsInChildren)
		{
			if (uITweener.tweenGroup == group)
			{
				uITweener.enabled = false;
			}
		}
	}

	public static void ResetToBeginningTweenGroup(GameObject objectRoot, int group)
	{
		if (!(objectRoot != null))
		{
			return;
		}
		UITweener[] componentsInChildren = objectRoot.GetComponentsInChildren<UITweener>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (componentsInChildren[i] != null && componentsInChildren[i].tweenGroup == group)
			{
				componentsInChildren[i].ResetToBeginning();
			}
		}
	}

	public static void RemoveCallback(GameObject objectRoot, int group, EventDelegate.Callback callback)
	{
		EventDelegate del = new EventDelegate(callback);
		UITweener[] componentsInChildren = objectRoot.GetComponentsInChildren<UITweener>();
		foreach (UITweener uITweener in componentsInChildren)
		{
			if (uITweener.tweenGroup == group)
			{
				uITweener.RemoveOnFinished(del);
			}
		}
	}

	public static void PlayTweenAnchors(GameObject obj, bool forward = true, string id = "", bool includeChildren = true)
	{
		if (obj == null)
		{
			return;
		}
		TweenAnchors[] array;
		if (includeChildren)
		{
			TweenAnchors[] components = obj.GetComponents<TweenAnchors>();
			TweenAnchors[] componentsInChildren = obj.GetComponentsInChildren<TweenAnchors>();
			array = new TweenAnchors[components.Length + componentsInChildren.Length];
			Array.Copy(components, array, components.Length);
			Array.Copy(componentsInChildren, 0, array, components.Length, componentsInChildren.Length);
		}
		else
		{
			array = obj.GetComponents<TweenAnchors>();
		}
		for (int i = 0; i < ((array != null) ? array.Length : 0); i++)
		{
			if (!(array[i] == null) || !(array[i].id != id))
			{
				if (forward)
				{
					array[i].PlayForward();
				}
				else
				{
					array[i].PlayBackwards();
				}
			}
		}
	}
}
