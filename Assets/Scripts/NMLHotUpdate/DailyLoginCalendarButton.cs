using TWDModel;
using UnityEngine;

public class DailyLoginCalendarButton : MonoBehaviour
{
	[SerializeField]
	private GameObject button;

	[SerializeField]
	private UILabel timeToNextReward;

	private static bool firstShow;

	private void Initialize()
	{
		DailyLoginCampaignModel dailyLoginCalendar = GameManager.Instance.playerModel.DailyLoginCalendar;
		GameEconomyData gameEconomyData = GameManager.Instance.gameEconomyData;
		if (gameEconomyData.GetFeature("DailyLoginCalendar").Enabled && !dailyLoginCalendar.IsInitialized && !dailyLoginCalendar.IsCompleted && (dailyLoginCalendar.GetCreationDate() != 0L || GameManager.Instance.playerModel.CouncilLevel <= gameEconomyData.ConfigData.DailyLoginCalendarMaxCouncilLevel))
		{
			if (OfflineManager.IsLoadDataManager || OfflineManager.IsFakeExecuteCommands)
			{
				GameManager.Instance.playerModel.DailyLoginCalendar?.InitializeCampaign();
			}
			else
			{
				Helpers.ExecuteCommand(new StartDailyLoginCampaignCommand());
			}
		}
	}

	private void OnEnable()
	{
		Initialize();
		UIEvent.OnUIEvent += OnEvent;
	}

	private void OnDisable()
	{
		UIEvent.OnUIEvent -= OnEvent;
	}

	private void OnEvent(string type, object parameter)
	{
		switch (type)
		{
		case "OnBuildingMoveCancelled":
		case "OnBuildingMoveEnded":
			UpdateButtonVisibility();
			break;
		case "OnBuildingConstructionStartPlacing":
		case "OnBuildingMoveStarted":
			button.SetActive(value: false);
			break;
		}
	}

	private void LateUpdate()
	{
		if (TutorialView.Instance.RunningButNotSuggesting)
		{
			button.SetActive(value: false);
			return;
		}
		UpdateButtonVisibility();
		if (button.activeSelf)
		{
			UpdateTimeUntilNextRewardUnlock();
			DailyLoginCampaignModel dailyLoginCalendar = GameManager.Instance.playerModel.DailyLoginCalendar;
			if (!TutorialView.Instance.Running && button.activeSelf && !firstShow && dailyLoginCalendar.CanClaimRewardForActiveDay())
			{
				firstShow = true;
				SpawnLoginCalendar();
			}
		}
	}

	private void UpdateButtonVisibility()
	{
		DailyLoginCampaignModel dailyLoginCalendar = GameManager.Instance.playerModel.DailyLoginCalendar;
		button.SetActive(dailyLoginCalendar.IsInitialized && !dailyLoginCalendar.IsCompleted);
	}

	public void SpawnLoginCalendar()
	{
		if (GameManager.Instance.playerModel.DailyLoginCalendar.IsInitialized)
		{
			LoginRewardPopup loginRewardPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.LoginRewardPopup) as LoginRewardPopup;
			if (loginRewardPopup != null)
			{
				loginRewardPopup.Open();
			}
		}
	}

	private void UpdateTimeUntilNextRewardUnlock()
	{
		DailyLoginCampaignModel dailyLoginCalendar = GameManager.Instance.playerModel.DailyLoginCalendar;
		if (dailyLoginCalendar.IsCompleted)
		{
			button.SetActive(value: false);
			base.gameObject.SetActive(value: false);
		}
		long num = dailyLoginCalendar.NextRewardTime - GameManager.Instance.playerModel.UtcTimeStamp;
		if (num >= 0)
		{
			if (num < 1000)
			{
				HelpersUI.SetContentToLabel(timeToNextReward, "0s");
			}
			else
			{
				HelpersUI.SetContentToLabel(timeToNextReward, Helpers.FormatTime(num));
			}
		}
		else
		{
			HelpersUI.SetContentToLabel(timeToNextReward, LocalizationManager.GetText("Popup.LoginReward.ClaimButton"));
		}
	}
}
