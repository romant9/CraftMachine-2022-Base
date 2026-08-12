using System;
using UnityEngine;

public class TweenProgressBar : UITweener
{
	[Range(0f, 1f)]
	public float From = 1f;

	[Range(0f, 1f)]
	public float To = 1f;

	[Range(-1f, 1f)]
	public float MinValue = -1f;

	[Range(-1f, 1f)]
	public float MaxValue = -1f;

	private UIProgressBar progressBar;

	public float value
	{
		get
		{
			return progressBar.value;
		}
		set
		{
			progressBar.value = value;
		}
	}

	protected override void OnUpdate(float factor, bool isFinished)
	{
		InitializeIfNecessary();
		float val = From * (1f - factor) + To * factor;
		if (MinValue != -1f)
		{
			val = Math.Max(val, MinValue);
		}
		if (MaxValue != -1f)
		{
			val = Math.Min(val, MaxValue);
		}
		progressBar.value = val;
	}

	private void InitializeIfNecessary()
	{
		if (progressBar == null)
		{
			progressBar = base.gameObject.GetComponent<UIProgressBar>();
		}
	}

	public static TweenProgressBar Begin(GameObject go, float duration, float valueTo)
	{
		TweenProgressBar tweenProgressBar = UITweener.Begin<TweenProgressBar>(go, duration);
		tweenProgressBar.InitializeIfNecessary();
		tweenProgressBar.From = tweenProgressBar.value;
		tweenProgressBar.To = valueTo;
		if (duration <= 0f)
		{
			tweenProgressBar.Sample(1f, isFinished: true);
			tweenProgressBar.enabled = false;
		}
		return tweenProgressBar;
	}
}
