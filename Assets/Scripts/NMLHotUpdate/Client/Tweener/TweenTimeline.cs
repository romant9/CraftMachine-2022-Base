using System;
using System.Collections.Generic;
using UnityEngine;

namespace Client.Tweener
{
	public class TweenTimeline
	{
		private List<TweenObjects> tweenObjectsList;

		private List<TweenObjects> tweenObjectsQueuedList;

		private List<TweenTimeline> nestedTweenTimelinesList;

		private int currentIndex;

		private EventDelegate queuedCompleteCallback;

		private EventDelegate nonQueuedCompleteCallback;

		private Callback completeCallback;

		private bool queuedComplete = true;

		private bool nonQueuedComplete = true;

		private bool nestedTimelineComplete = true;

		public TweenTimeline()
		{
			tweenObjectsList = new List<TweenObjects>();
			tweenObjectsQueuedList = new List<TweenObjects>();
			nestedTweenTimelinesList = new List<TweenTimeline>();
			currentIndex = 0;
			queuedCompleteCallback = new EventDelegate(QueuedCompleted);
			nonQueuedCompleteCallback = new EventDelegate(NonQueuedCompleted);
		}

		public void Clear()
		{
			completeCallback = null;
			for (int i = 0; i < ((nestedTweenTimelinesList != null) ? nestedTweenTimelinesList.Count : 0); i++)
			{
				if (nestedTweenTimelinesList[i] != null)
				{
					nestedTweenTimelinesList[i].Clear();
				}
			}
			nestedTweenTimelinesList?.Clear();
			for (int j = 0; j < ((tweenObjectsQueuedList != null) ? tweenObjectsQueuedList.Count : 0); j++)
			{
				if (tweenObjectsQueuedList[j] != null)
				{
					tweenObjectsQueuedList[j].Clear(queuedCompleteCallback);
				}
			}
			tweenObjectsQueuedList?.Clear();
			for (int k = 0; k < ((tweenObjectsList != null) ? tweenObjectsList.Count : 0); k++)
			{
				if (tweenObjectsList[k] != null)
				{
					tweenObjectsList[k].Clear(nonQueuedCompleteCallback);
				}
			}
			tweenObjectsList?.Clear();
			queuedComplete = true;
			nonQueuedComplete = true;
			nestedTimelineComplete = true;
			currentIndex = 0;
		}

		public void OnComplete(Callback callback)
		{
			completeCallback = (Callback)Delegate.Remove(completeCallback, callback);
			completeCallback = (Callback)Delegate.Combine(completeCallback, callback);
		}

		public void RemoveCallback(Callback callback)
		{
			completeCallback = (Callback)Delegate.Remove(completeCallback, callback);
		}

		public TweenTimeline Add(TweenTimeline timeline)
		{
			if (timeline == this)
			{
				Debug.LogError("Cannot nest itself!!");
				return this;
			}
			nestedTweenTimelinesList.Add(timeline);
			return this;
		}

		public TweenTimeline Add(TweenObjects tween)
		{
			tweenObjectsList.Add(tween);
			return this;
		}

		public TweenTimeline Queue(TweenObjects tween)
		{
			if (tween == null)
			{
				return this;
			}
			tweenObjectsQueuedList.Add(tween);
			return this;
		}

		public TweenTimeline Play()
		{
			queuedComplete = true;
			nonQueuedComplete = true;
			nestedTimelineComplete = true;
			TweenTimeline tweenTimeline = null;
			for (int i = 0; i < nestedTweenTimelinesList.Count; i++)
			{
				if (nestedTweenTimelinesList[i] != null)
				{
					if (tweenTimeline == null || IsLonger(nestedTweenTimelinesList[i], tweenTimeline) == 1)
					{
						tweenTimeline?.RemoveCallback(NestedTimelinesCompleted);
						tweenTimeline = nestedTweenTimelinesList[i];
						tweenTimeline.OnComplete(NestedTimelinesCompleted);
					}
					nestedTimelineComplete = false;
					nestedTweenTimelinesList[i].Play();
				}
			}
			TweenObjects tweenObjects = null;
			for (int j = 0; j < tweenObjectsList.Count; j++)
			{
				if (tweenObjectsList[j] != null)
				{
					if (tweenObjects == null || IsLonger(tweenObjectsList[j], tweenObjects) == 1)
					{
						tweenObjects = tweenObjectsList[j];
					}
					nonQueuedComplete = false;
					tweenObjectsList[j].Play();
				}
			}
			tweenObjects?.longestTween.SetOnFinished(nonQueuedCompleteCallback);
			PlayQueued();
			return this;
		}

		public void Stop()
		{
			for (int i = 0; i < nestedTweenTimelinesList.Count; i++)
			{
				if (nestedTweenTimelinesList[i] != null)
				{
					nestedTweenTimelinesList[i].Stop();
				}
			}
			for (int j = 0; j < tweenObjectsList.Count; j++)
			{
				if (tweenObjectsList[j] != null)
				{
					tweenObjectsList[j].Stop();
				}
			}
			for (int k = 0; k < tweenObjectsQueuedList.Count; k++)
			{
				if (tweenObjectsQueuedList[k] != null)
				{
					tweenObjectsQueuedList[k].Stop();
				}
			}
		}

		private void PlayQueued()
		{
			if (currentIndex >= tweenObjectsQueuedList.Count)
			{
				queuedComplete = true;
				CheckComplete();
				return;
			}
			queuedComplete = false;
			tweenObjectsQueuedList[currentIndex].longestTween.RemoveOnFinished(queuedCompleteCallback);
			tweenObjectsQueuedList[currentIndex].longestTween.AddOnFinished(queuedCompleteCallback);
			tweenObjectsQueuedList[currentIndex].Play();
		}

		private void QueuedCompleted()
		{
			currentIndex++;
			PlayQueued();
		}

		private void NonQueuedCompleted()
		{
			nonQueuedComplete = true;
			CheckComplete();
		}

		private void NestedTimelinesCompleted()
		{
			nestedTimelineComplete = true;
			CheckComplete();
		}

		private void CheckComplete()
		{
			if (nonQueuedComplete && queuedComplete && nestedTimelineComplete)
			{
				CallComplete();
			}
		}

		private void CallComplete()
		{
			completeCallback?.Invoke();
		}

		public float CalculateTotalDuration()
		{
			return Mathf.Max(CalculateQueuedDuration(), CalculateNonQueuedDuration());
		}

		private float CalculateQueuedDuration()
		{
			float num = 0f;
			for (int i = 0; i < tweenObjectsQueuedList.Count; i++)
			{
				if (tweenObjectsQueuedList[i] != null)
				{
					num += tweenObjectsQueuedList[i].CalculateTotalDuration();
				}
			}
			return num;
		}

		private float CalculateNonQueuedDuration()
		{
			TweenObjects tweenObjects = null;
			for (int i = 0; i < tweenObjectsList.Count; i++)
			{
				if (tweenObjectsList[i] != null && (tweenObjects == null || IsLonger(tweenObjectsList[i], tweenObjects) == 1))
				{
					tweenObjects = tweenObjectsList[i];
				}
			}
			return tweenObjects?.CalculateTotalDuration() ?? 0f;
		}

		private static int IsLonger(TweenObjects tweenA, TweenObjects tweenB)
		{
			if (tweenA == null || tweenB == null)
			{
				return 0;
			}
			float num = tweenA.CalculateTotalDuration();
			float num2 = tweenB.CalculateTotalDuration();
			if (!(num > num2))
			{
				return -1;
			}
			return 1;
		}

		private static int IsLonger(TweenTimeline tweenA, TweenTimeline tweenB)
		{
			if (tweenA == null || tweenB == null)
			{
				return 0;
			}
			float num = tweenA.CalculateTotalDuration();
			float num2 = tweenB.CalculateTotalDuration();
			if (!(num > num2))
			{
				return -1;
			}
			return 1;
		}
	}
}
