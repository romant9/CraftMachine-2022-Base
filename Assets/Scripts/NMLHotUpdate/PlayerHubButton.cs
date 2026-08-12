using System;
using TWDModel;
using UnityEngine;

public class PlayerHubButton : MonoBehaviour
{
	[SerializeField]
	private GameObject timerContainer;

	[SerializeField]
	private UILabel timerLabel;

	private PlayerHubNewsItem newsItemShown;

	private long timeUntilNotValid;

	private const string CLICK_COUNT_KEY = "ClickCount";

	private const string LAST_UTC_TIMESTAMP_KEY = "LastUTCTimestamp";

	private int currentPopupCount;

	private long lastUTCTimestamp;

	private static bool firstShow;

	private void OnEnable()
	{
		ShowTimer();
		LoadData();
	}

	private void Update()
	{
		if (newsItemShown != null)
		{
			long num = (long)(Time.deltaTime * 1000f);
			if (timeUntilNotValid > 0 && timeUntilNotValid - num <= 0)
			{
				ShowTimer();
			}
			timeUntilNotValid = Math.Max(timeUntilNotValid - num, 0L);
			if (timerLabel != null)
			{
				timerLabel.text = Helpers.FormatTimeNoZero(timeUntilNotValid);
			}
		}
	}

	private void LateUpdate()
	{
		if (firstShow || !base.gameObject.activeSelf || GameManager.Instance.playerModel.Tutorial == null || !GameManager.Instance.playerModel.Tutorial.StaticTutorialComplete || TutorialView.Instance.Running || currentPopupCount >= GameManager.Instance.gameEconomyData.ConfigData.ForcedDisplayForcedDisplayLimit)
		{
			return;
		}
		ActiveInformationDefinition[] activeInformationDefinitions = GameManager.Instance.gameEconomyData.ActiveInformationDefinitions;
		foreach (ActiveInformationDefinition activeInformationDefinition in activeInformationDefinitions)
		{
			long utcTimeStamp = GameManager.Instance.playerModel.UtcTimeStamp;
			if (utcTimeStamp >= activeInformationDefinition.ShowTimeMilliseconds && utcTimeStamp <= activeInformationDefinition.EndTimeMilliseconds && activeInformationDefinition.ForcedDisplay == 1)
			{
				firstShow = true;
				CampHUD campHUD = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CampCampMapHud) as CampHUD;
				if (campHUD != null)
				{
					campHUD.OnClickPlayerHub();
				}
				currentPopupCount++;
				SaveData(utcTimeStamp);
				break;
			}
		}
	}

	private void ShowTimer()
	{
		PlayerHubNewsItem playerHubNewsItem = null;
		int num = int.MaxValue;
		for (int i = 0; i < GameManager.Instance.PlayerHubManager.News.Count; i++)
		{
			PlayerHubNewsItem playerHubNewsItem2 = GameManager.Instance.PlayerHubManager.News[i];
			if (playerHubNewsItem2.ShowCounter && playerHubNewsItem2.NavigationLink != "OPEN_BUNDLE" && playerHubNewsItem2.NavigationLink != "OPEN_CHALLENGE" && playerHubNewsItem2.OrderNumber < num)
			{
				timeUntilNotValid = Math.Max((long)(playerHubNewsItem2.EndUnixTime.FromUnixTimeSeconds() - DateTime.UtcNow).TotalMilliseconds, 0L);
				if (timeUntilNotValid > 0)
				{
					num = playerHubNewsItem2.OrderNumber;
					playerHubNewsItem = playerHubNewsItem2;
				}
			}
		}
		timerContainer.SetActive(playerHubNewsItem != null);
		newsItemShown = playerHubNewsItem;
		if (playerHubNewsItem != null)
		{
			timeUntilNotValid = Math.Max((long)(playerHubNewsItem.EndUnixTime.FromUnixTimeSeconds() - DateTime.UtcNow).TotalMilliseconds, 0L);
		}
	}

	private void LoadData()
	{
		currentPopupCount = PlayerPrefs.GetInt("ClickCount", 0);
		long.TryParse(PlayerPrefs.GetString("LastUTCTimestamp", "0"), out lastUTCTimestamp);
		if (HasPassedOneCalendarDay())
		{
			currentPopupCount = 0;
			SaveData(GameManager.Instance.playerModel.UtcTimeStamp);
		}
	}

	private void SaveData(long timestamp)
	{
		PlayerPrefs.SetInt("ClickCount", currentPopupCount);
		PlayerPrefs.SetString("LastUTCTimestamp", timestamp.ToString());
		PlayerPrefs.Save();
		lastUTCTimestamp = timestamp;
	}

	private bool HasPassedOneCalendarDay()
	{
		DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeMilliseconds(lastUTCTimestamp);
		return DateTimeOffset.FromUnixTimeMilliseconds(GameManager.Instance.playerModel.UtcTimeStamp).Date > dateTimeOffset.Date;
	}
}
