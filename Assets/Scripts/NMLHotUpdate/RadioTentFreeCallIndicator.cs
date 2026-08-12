using UnityEngine;

public class RadioTentFreeCallIndicator : BuildingUpgradeInsideIndicator
{
	[SerializeField]
	private UILabel labelTime;

	[SerializeField]
	private GameObject callAvailableContainer;

	[SerializeField]
	private GameObject callComingSoonContainer;

	private int previousTimeSeconds = -1;

	private long time = 2147483647L;

	protected virtual void Start()
	{
		SetContainerVisibility();
	}

	protected virtual void LateUpdate()
	{
		if (time != int.MaxValue)
		{
			time -= (long)(Time.deltaTime * 1000f);
			int num = Helpers.ConvertToSecondsNoZero(time);
			if (num != previousTimeSeconds)
			{
				previousTimeSeconds = num;
				labelTime.text = Helpers.FormatTime(num * 1000);
			}
			if (time <= 0)
			{
				SetContainerVisibility();
			}
		}
	}

	private void FindShortestTime()
	{
		time = 2147483647L;
		PhoneCallModel phoneCall = GameManager.Instance.playerModel.PhoneCall;
		for (int i = 0; i < 3; i++)
		{
			if (phoneCall.MillisecondsTillFreeCall[i] > 0 && time > phoneCall.MillisecondsTillFreeCall[i])
			{
				time = phoneCall.MillisecondsTillFreeCall[i];
			}
		}
	}

	private void SetContainerVisibility()
	{
		if (GameManager.Instance.playerModel.PhoneCall.HasFreeCall())
		{
			callAvailableContainer.SetActive(value: true);
			callComingSoonContainer.SetActive(value: false);
			return;
		}
		if (GameManager.Instance.playerModel.PhoneCall.AnyFreeCallAvailable())
		{
			FindShortestTime();
			callComingSoonContainer.SetActive(value: true);
		}
		else
		{
			callComingSoonContainer.SetActive(value: false);
		}
		callAvailableContainer.SetActive(value: false);
	}
}
