using Client.Tweener;
using UnityEngine;

public class ProgressBarWaypoint
{
	public string id = "-";

	public Easing.All Easing = Client.Tweener.Easing.All.Linear;

	public bool CurrentAsStartValue = true;

	public WaypointIconBase ActivateObject;

	private float durationInternal;

	private float toInternal;

	private float fromInternal;

	private float delayInternal;

	public float duration
	{
		get
		{
			return durationInternal;
		}
		set
		{
			durationInternal = Mathf.Clamp(value, 0f, float.MaxValue);
		}
	}

	public float from
	{
		get
		{
			if (CurrentAsStartValue)
			{
				return -1f;
			}
			return fromInternal;
		}
		set
		{
			fromInternal = Mathf.Clamp01(value);
		}
	}

	public float to
	{
		get
		{
			return toInternal;
		}
		set
		{
			toInternal = Mathf.Clamp01(value);
		}
	}

	public float completionDelay
	{
		get
		{
			return delayInternal;
		}
		set
		{
			delayInternal = Mathf.Clamp(value, 0f, float.MaxValue);
		}
	}

	public float TotalDuration => duration + completionDelay;

	public void Complete()
	{
		if (ActivateObject != null)
		{
			ActivateObject.CompleteTrigger();
		}
	}
}
