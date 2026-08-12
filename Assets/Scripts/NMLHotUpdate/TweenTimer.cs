using UnityEngine;

public class TweenTimer : UITweener
{
	public long FromTimeInSeconds;

	public long ToTimeInSeconds;

	private UILabel timerLabel;

	protected override void OnUpdate(float factor, bool isFinished)
	{
		if (timerLabel == null)
		{
			timerLabel = base.gameObject.GetComponent<UILabel>();
		}
		timerLabel.text = Helpers.FormatTimeNoZero((long)((float)FromTimeInSeconds * (1f - factor) + (float)ToTimeInSeconds * factor) * 1000);
	}

	public static TweenTimer Begin(GameObject go, float duration, long fromTimeSeconds, long toTimeSeconds)
	{
		TweenTimer tweenTimer = UITweener.Begin<TweenTimer>(go, duration);
		tweenTimer.FromTimeInSeconds = fromTimeSeconds;
		tweenTimer.ToTimeInSeconds = toTimeSeconds;
		if (duration <= 0f)
		{
			tweenTimer.Sample(1f, isFinished: true);
			tweenTimer.enabled = false;
		}
		return tweenTimer;
	}
}
