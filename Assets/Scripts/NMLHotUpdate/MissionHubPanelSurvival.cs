using TWDModel;
using UnityEngine;

public class MissionHubPanelSurvival : MissionHubGameModePanel
{
	[SerializeField]
	private GameObject survivorCountContainer;

	[SerializeField]
	private UILabel survivorsLeftLabel;

	[SerializeField]
	private GameObject survivalRewardPreviewContainer;

	[SerializeField]
	private GameObject selectDifficultyLabelContainer;

	[Header("Survival not active")]
	[SerializeField]
	private GameObject survivalNotActiveParent;

	[SerializeField]
	private UILabel nextSurvivalTimerLabel;

	[SerializeField]
	private UISprite[] rewardPreviewIcons;

	public override void Start()
	{
		base.Start();
	}

	protected override void OpenDialog()
	{
		MissionHubNavigation.TryOpenSurvivalMap();
	}

	public override void Update()
	{
		base.Update();
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
		bool value = false;
		bool value2 = false;
		bool value3 = false;
		if (WeeklySurvivalHelper.IsSurvivalOngoing())
		{
			gameModeTimeLeft = WeeklySurvivalHelper.GetTimeLeftToCurrentSurvivalEnd();
			timeLabelLocalisation = LocalizationManager.GetText("Popup.MissionHub.SurvivalEndsIn");
			SurvivalCharacterContainerModel survivalCharacters = GameManager.Instance.playerModel.SurvivorContainer.SurvivalCharacters;
			HelpersUI.SetContentToLabel(survivorsLeftLabel, survivalCharacters.GetNumSurvivorsAvailableForAction() + " / " + survivalCharacters.SurvivalModeSurvivors.Count);
			string iconsStrings = "";
			WeeklySurvivalModel weeklySurvival = GameManager.Instance.playerModel.WeeklySurvival;
			if (weeklySurvival != null)
			{
				bool flag = true;
				if (weeklySurvival.CurrentDifficulty == SurvivalDifficulty.Nightmare)
				{
					iconsStrings = GameManager.Instance.gameEconomyData.ConfigData.SurvivalNightmareRewardPreviewIcons;
				}
				else if (weeklySurvival.CurrentDifficulty == SurvivalDifficulty.Hard)
				{
					iconsStrings = GameManager.Instance.gameEconomyData.ConfigData.SurvivalHardRewardPreviewIcons;
				}
				else if (weeklySurvival.CurrentDifficulty == SurvivalDifficulty.Normal)
				{
					iconsStrings = GameManager.Instance.gameEconomyData.ConfigData.SurvivalNormalRewardPreviewIcons;
				}
				else
				{
					flag = false;
				}
				value = flag;
				value2 = flag;
				value3 = !flag;
			}
			UISpriteIconHelper.SetIcons(rewardPreviewIcons, iconsStrings);
		}
		else if (WeeklySurvivalHelper.IsNextSurvivalPossible())
		{
			gameModeTimeLeft = WeeklySurvivalHelper.GetTimeLeftToNextSurvival();
			timeLabelLocalisation = LocalizationManager.GetText("Popup.MissionHub.SurvivalStartsIn");
			UISpriteIconHelper.SetIcons(rewardPreviewIcons, "");
		}
		else
		{
			timeLabelLocalisation = "";
			UISpriteIconHelper.SetIcons(rewardPreviewIcons, "");
		}
		Helpers.GameObjectSetActive(survivorCountContainer, value);
		Helpers.GameObjectSetActive(survivalRewardPreviewContainer, value2);
		Helpers.GameObjectSetActive(selectDifficultyLabelContainer, value3);
		MissionSpawnPointGroup missionSpawnPointGroup = null;
		CheckLockedState();
		if (base.isLocked)
		{
			if (WeeklySurvivalHelper.IsLockedByCouncilLevel())
			{
				HelpersUI.SetContentToLabel(lockedLabel, LocalizationManager.GetText("Popup.MissionHub.SurvivalUnlockAtLevel{CouncilLevel}", GameManager.Instance.gameEconomyData.ConfigData.SurvivalUnlockAtCouncilLevel));
			}
			else if (WeeklySurvivalHelper.IsLockedByTutorial())
			{
				HelpersUI.SetContentToLabel(lockedLabel, LocalizationManager.GetText("Popup.MissionHub.SurvivalUnlockAfterTutorial"));
			}
			else
			{
				Helpers.GameObjectSetActive(lockedLabel, value: false);
			}
		}
		else if (WeeklySurvivalHelper.IsSurvivalOngoing())
		{
			ShowSurvivalUpcoming(value: false);
			missionSpawnPointGroup = WeeklySurvivalHelper.GetWeeklySurvivalModel().GetMissionSpawnPointGroup();
			if (progressBar != null)
			{
				progressBar.UpdateUI();
			}
		}
		else if (WeeklySurvivalHelper.IsNextSurvivalPossible())
		{
			ShowSurvivalUpcoming(value: true);
			missionSpawnPointGroup = GameManager.Instance.gameEconomyData.MissionSpawnPointData.GetSpawnPointGroup(WeeklySurvivalHelper.GetWeeklySurvivalModel().NextWeeklySurvival.DetailMapId);
		}
		if (missionSpawnPointGroup != null)
		{
			HelpersUI.SetContentToLabel(titleSubLabel, "");
		}
		else
		{
			HelpersUI.SetContentToLabel(titleSubLabel, "");
		}
		Helpers.GameObjectSetActive(unlockedEffect, FeatureUIHighlights.IsActive(FeatureUIHighlights.FeaturesIds.WeeklySurvivalUnlocked));
		if (WeeklySurvivalHelper.IsSurvivalOngoing())
		{
			GameManager.Instance.Blackboard.IsToggleOn("Toggle.SurvivalUnlockedSeen");
		}
	}

	public override void SetContentToTimerLabel(string value)
	{
		if (survivalNotActiveParent.activeSelf)
		{
			HelpersUI.SetContentToLabel(nextSurvivalTimerLabel, value);
		}
		else
		{
			base.SetContentToTimerLabel(value);
		}
	}

	public override void CheckLockedState()
	{
		UpdateLockedState(WeeklySurvivalHelper.IsLockedByCouncilLevelOrTutorial());
	}

	private void ShowSurvivalUpcoming(bool value)
	{
		Helpers.GameObjectSetActive(survivalNotActiveParent, value);
		Helpers.GameObjectSetActive(nextSurvivalTimerLabel, value);
		Helpers.GameObjectSetActive(timerLabel, !value);
		Helpers.GameObjectSetActive(locationTexture, !value);
		Helpers.GameObjectSetActive(progressBar, !value);
	}

	protected override void ButtonMainClicked(UIButtonExtended button)
	{
		base.ButtonMainClicked(button);
		EventManager.NotifyClick("Survival");
	}
}
