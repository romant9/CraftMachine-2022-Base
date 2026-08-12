using System.Collections.Generic;
using UnityEngine;

namespace Client.Tweener
{
	public class TweenObjects
	{
		public UITweener longestTween;

		private Transform target;

		private List<UITweener> currentTweens = new List<UITweener>();

		public TweenObjects(Transform target)
		{
			this.target = target;
		}

		public void Clear(EventDelegate callback = null)
		{
			if (longestTween != null && callback != null)
			{
				longestTween.RemoveOnFinished(callback);
			}
			if (currentTweens != null)
			{
				currentTweens.Clear();
			}
			longestTween = null;
			target = null;
		}

		public TweenObjects Add(UITweener tween)
		{
			currentTweens.Add(tween);
			if (tween.style == UITweener.Style.Loop || tween.style == UITweener.Style.PingPong)
			{
				return this;
			}
			if (longestTween == null || tween.duration + tween.delay > longestTween.duration + longestTween.delay)
			{
				longestTween = tween;
			}
			return this;
		}

		public TweenObjects Play()
		{
			for (int i = 0; i < currentTweens.Count; i++)
			{
				if (!(currentTweens[i] == null))
				{
					currentTweens[i].ResetToBeginning();
					currentTweens[i].PlayForward();
				}
			}
			return this;
		}

		public TweenObjects Stop()
		{
			for (int i = 0; i < currentTweens.Count; i++)
			{
				if (!(currentTweens[i] == null))
				{
					currentTweens[i].enabled = false;
				}
			}
			return this;
		}

		public float CalculateTotalDuration()
		{
			if (!(longestTween != null))
			{
				return 0f;
			}
			return longestTween.duration + longestTween.delay;
		}

		public static TweenObjects Wait(Transform target, float waitTime)
		{
			if (target == null || target.gameObject == null)
			{
				return null;
			}
			TweenWait tweenWait = Helpers.AddComponent<TweenWait>(target.gameObject);
			tweenWait.enabled = false;
			tweenWait.duration = 0f;
			tweenWait.delay = waitTime;
			TweenObjects tweenObjects = new TweenObjects(target);
			tweenObjects.Add(tweenWait);
			return tweenObjects;
		}

		public static TweenObjects Group(GameObject target, int groupId, bool recursive = true)
		{
			return Group(target.transform, groupId, recursive);
		}

		public static TweenObjects Group(Transform target, int groupId, bool recursive = true)
		{
			if (target == null)
			{
				return null;
			}
			UITweener[] array = (recursive ? target.GetComponentsInChildren<UITweener>(includeInactive: false) : target.GetComponents<UITweener>());
			if (array == null)
			{
				return null;
			}
			TweenObjects tweenObjects = new TweenObjects(target);
			foreach (UITweener uITweener in array)
			{
				if (!(uITweener == null) && uITweener.tweenGroup == groupId)
				{
					tweenObjects.Add(uITweener);
				}
			}
			if (tweenObjects.longestTween == null)
			{
				tweenObjects = null;
				Debug.LogWarning("Could not find Tweens with id: " + groupId);
			}
			return tweenObjects;
		}
	}
}
