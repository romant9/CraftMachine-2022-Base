using System;
using UnityEngine;

public class TwoFieldTimer : TimerComponent
{
	[SerializeField]
	private UILabel FirstTimerAmount;

	[SerializeField]
	private UILabel FirstTimerLabel;

	[SerializeField]
	private UILabel SecondTimerAmount;

	[SerializeField]
	private UILabel SecondTimerLabel;

	public override void Set(TimeSpan timeSpan)
	{
		if (timeSpan.TotalHours < 1.0)
		{
			FirstTimerAmount.text = timeSpan.Minutes.ToString();
			FirstTimerLabel.text = Localize("Text.General.Timer.Minutes");
			SecondTimerAmount.text = timeSpan.Seconds.ToString();
			SecondTimerLabel.text = Localize("Text.General.Timer.Seconds");
		}
		else if (timeSpan.TotalDays < 1.0)
		{
			FirstTimerAmount.text = timeSpan.Hours.ToString();
			FirstTimerLabel.text = Localize("Text.General.Timer.Hours");
			SecondTimerAmount.text = timeSpan.Minutes.ToString();
			SecondTimerLabel.text = Localize("Text.General.Timer.Minutes");
		}
		else
		{
			FirstTimerAmount.text = timeSpan.Days.ToString();
			FirstTimerLabel.text = Localize("Text.General.Timer.Days");
			SecondTimerAmount.text = timeSpan.Hours.ToString();
			SecondTimerLabel.text = Localize("Text.General.Timer.Hours");
		}
	}

	private static string Localize(string key)
	{
		return LocalizationManager.GetText(key);
	}
}
