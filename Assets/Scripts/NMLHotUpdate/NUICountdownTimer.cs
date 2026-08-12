using UnityEngine;

public class NUICountdownTimer : MonoBehaviourExtended
{
	[SerializeField]
	private UILabel labelSeconds;

	[SerializeField]
	private UILabel labelMinutes;

	[SerializeField]
	private UILabel labelHours;

	[SerializeField]
	private UILabel labelDays;

	private string[] timeStringArray = new string[4] { "00", "00", "00", "00" };

	private void Awake()
	{
		DebugIdString = "MissionHubSeasonTimer";
	}

	public virtual void Start()
	{
	}

	public void SetCurrentMilliseconds(long milliSeconds)
	{
		if (timeStringArray.Length >= 4)
		{
			Helpers.FormatTimeSpecialTimer(milliSeconds, out timeStringArray[0], out timeStringArray[1], out timeStringArray[2], out timeStringArray[3]);
			HelpersUI.SetContentToLabel(labelSeconds, timeStringArray[0]);
			HelpersUI.SetContentToLabel(labelMinutes, timeStringArray[1]);
			HelpersUI.SetContentToLabel(labelHours, timeStringArray[2]);
			HelpersUI.SetContentToLabel(labelDays, timeStringArray[3]);
		}
	}
}
