using TWDModel;
using UnityEngine;

public class MissionHubPanelChallenge : MissionHubGameModePanel
{
	[Header("Challenge not active")]
	[SerializeField]
	private GameObject challengeNotActiveParent;

	[SerializeField]
	private UILabel nextChallengeTimerLabel;

	[SerializeField]
	private Material starHeroChallengeMaterial;

	[SerializeField]
	private UITexture challengeTexture;

	[SerializeField]
	public GameObject plightEffect;

	[SerializeField]
	public GameObject challengeRewardTipsContainer;

	public override void Start()
	{
		base.Start();
	}

	protected override void OpenDialog()
	{
		if (WeeklyChallengeHelper.IsLockedByCouncilLevelOrTutorial())
		{
			FeatureLockedPopup.Open(FeatureLockedPopup.FeatureType.Challenge, locked: true);
			return;
		}
		WeeklyChallengeDifficultyPopup weeklyChallengeDifficultyPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.WeeklyChallengeDifficulty) as WeeklyChallengeDifficultyPopup;
		if (weeklyChallengeDifficultyPopup != null)
		{
			weeklyChallengeDifficultyPopup.Open();
		}
	}

	public override void Update()
	{
		base.Update();
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
		if (WeeklyChallengeHelper.FeaturedStarHeroActive)
		{
			challengeTexture.material = starHeroChallengeMaterial;
		}
		if (WeeklyChallengeHelper.IsChallengeOngoing())
		{
			gameModeTimeLeft = WeeklyChallengeHelper.GetTimeLeftToCurrentChallengeEnd();
			timeLabelLocalisation = LocalizationManager.GetText("Popup.MissionHub.ChallengeEndsIn");
		}
		else if (WeeklyChallengeHelper.IsNextChallengePossible())
		{
			gameModeTimeLeft = WeeklyChallengeHelper.GetTimeLeftToNextChallenge();
			timeLabelLocalisation = LocalizationManager.GetText("Popup.MissionHub.ChallengeStartsIn");
		}
		else
		{
			timeLabelLocalisation = "";
		}
		MissionSpawnPointGroup missionSpawnPointGroup = null;
		IReward reward = null;
		CheckLockedState();
		bool flag = !GameManager.Instance.gameEconomyData.GetFeature("Social").Enabled;
		if (base.isLocked)
		{
			if (WeeklyChallengeHelper.IsLockedByCouncilLevel())
			{
				HelpersUI.SetContentToLabel(lockedLabel, LocalizationManager.GetText("Popup.MissionHub.ChallengesUnlockAtLevel{CouncilLevel}", GameManager.Instance.gameEconomyData.ConfigData.ChallengesUnlockAtCouncilLevel));
			}
			else if (flag)
			{
				HelpersUI.SetContentToLabel(lockedLabel, LocalizationManager.GetText("Popup.MissionHub.NotAvailable"));
			}
			else if (WeeklyChallengeHelper.IsLockedByTutorial())
			{
				HelpersUI.SetContentToLabel(lockedLabel, LocalizationManager.GetText("Popup.FeatureLocked.TutorialEndBlockingChallenges"));
			}
			else
			{
				Helpers.GameObjectSetActive(lockedLabel, value: false);
			}
		}
		else if (WeeklyChallengeHelper.IsChallengeOngoing())
		{
			WeeklyChallengeReward nextReward = WeeklyChallengeHelper.GetNextReward(personal: true);
			if (nextReward != null && nextReward.RewardEntries != null && nextReward.RewardEntries.Count > 0)
			{
				reward = nextReward.RewardEntries.RewardsList[0];
			}
			ShowChallengeUpcoming(value: false);
			missionSpawnPointGroup = WeeklyChallengeHelper.GetWeeklyChallengeModel().GetMissionSpawnPointGroup();
			if (progressBar != null)
			{
				progressBar.UpdateUI();
			}
		}
		else if (WeeklyChallengeHelper.IsNextChallengePossible())
		{
			ShowChallengeUpcoming(value: true);
			missionSpawnPointGroup = GameManager.Instance.gameEconomyData.MissionSpawnPointData.GetSpawnPointGroup(WeeklyChallengeHelper.GetWeeklyChallengeModel().NextWeeklyChallenge.DetailMapId);
		}
		if (missionSpawnPointGroup != null)
		{
			HelpersUI.SetContentToLabel(titleSubLabel, HelpersLocalization.GetEpisodeName(missionSpawnPointGroup));
		}
		else
		{
			HelpersUI.SetContentToLabel(titleSubLabel, "");
		}
		Helpers.GameObjectSetActive(unlockedEffect, !flag && FeatureUIHighlights.IsActive(FeatureUIHighlights.FeaturesIds.WeeklyChallengeUnlocked));
		WeeklyChallengeModel weeklyChallengeModel = WeeklyChallengeHelper.GetWeeklyChallengeModel();
		if (weeklyChallengeModel != null)
		{
			bool flag2 = weeklyChallengeModel.IsDebufCycles();
			Helpers.GameObjectSetActive(plightEffect, flag2 && GameManager.Instance.gameEconomyData.GetFeature("UIEventIconPlight").Enabled);
		}
		if (WeeklyChallengeHelper.IsChallengeOngoing())
		{
			GameManager.Instance.Blackboard.IsToggleOn("Toggle.ChallengeUnlockedSeen");
		}
		Helpers.GameObjectSetActive(challengeRewardTipsContainer, value: false);
		if (Helpers.IsChallengeRewardTipsOpen())
		{
			Helpers.GameObjectSetActive(rewardsPreviewIcon, value: false);
			Helpers.GameObjectSetActive(challengeRewardTipsContainer, value: true);
			return;
		}
		Helpers.GameObjectSetActive(challengeRewardTipsContainer, value: false);
		if (flag)
		{
			Helpers.GameObjectSetActive(rewardsPreviewIcon, value: false);
		}
		else
		{
			PreviewSingleReward(reward);
		}
	}

	public override void SetContentToTimerLabel(string value)
	{
		if (challengeNotActiveParent.activeSelf)
		{
			HelpersUI.SetContentToLabel(nextChallengeTimerLabel, value);
		}
		else
		{
			base.SetContentToTimerLabel(value);
		}
	}

	private void ShowChallengeUpcoming(bool value)
	{
		Helpers.GameObjectSetActive(challengeNotActiveParent, value);
		Helpers.GameObjectSetActive(nextChallengeTimerLabel, value);
		Helpers.GameObjectSetActive(timerLabel, !value);
		Helpers.GameObjectSetActive(locationTexture, !value);
		Helpers.GameObjectSetActive(progressBar, !value);
	}

	public override void CheckLockedState()
	{
		bool flag = GameManager.Instance.gameEconomyData.GetFeature("Social").Enabled;
		UpdateLockedState(!flag || WeeklyChallengeHelper.IsLockedByCouncilLevelOrTutorial());
	}

	protected override void ButtonMainClicked(UIButtonExtended button)
	{
		base.ButtonMainClicked(button);
		EventManager.NotifyClick("Challenge");
	}
}
