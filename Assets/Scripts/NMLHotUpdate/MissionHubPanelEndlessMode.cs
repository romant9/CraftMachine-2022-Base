using System.Collections.Generic;
using BaseModel;
using UnityEngine;

public class MissionHubPanelEndlessMode : MissionHubPanelBase
{
	private EndlessModePanelState currentEndlessModePanelState;

	[SerializeField]
	private List<GameObject> MissionPanelStateObjects;

	[Header("Timer")]
	[SerializeField]
	private UILabel cycleTimerLabel;

	[SerializeField]
	private UILabel nextCycleTimerLabel;

	private long cycleTimeLeft;

	private UILabel activeTimerLabel;

	[Header("Map")]
	[SerializeField]
	private UILabel endlessMapName;

	[SerializeField]
	private UILabel endlessExpertMapName;

	[SerializeField]
	private UILabel nextEndlessMapName;

	[SerializeField]
	private UILabel nextEndlessExpertMapName;

	[Header("General")]
	[SerializeField]
	private UIButton dialobButton;

	[SerializeField]
	private UILabel lockedLabel;

	[SerializeField]
	private GameObject unlockEffect;

	[SerializeField]
	private UILabel currentRewardTier;

	private float refreshRate = 1f;

	public override void UpdateUI()
	{
		base.UpdateUI();
		DeActivateAllContainers();
		CheckState();
		SetupOnClickDialogButton();
	}

	public override void Update()
	{
		base.Update();
		refreshRate += Time.deltaTime;
		if (refreshRate > 1f)
		{
			cycleTimeLeft -= 1000L;
			activeTimerLabel.text = FormatTimeLeft(cycleTimeLeft);
			refreshRate = 0f;
			if (cycleTimeLeft < 0)
			{
				DeActivateAllContainers();
				CheckState();
			}
		}
	}

	private void CheckState()
	{
		CheckHighLightState();
		currentEndlessModePanelState = EndlessModeHelpers.GetEndlessHubPanelState();
		cycleTimeLeft = EndlessModeHelpers.GetTimeLeftDependingState(currentEndlessModePanelState);
		switch (currentEndlessModePanelState)
		{
		case EndlessModePanelState.Active:
			OnActivePanelState();
			break;
		case EndlessModePanelState.Locked:
			OnLockedPanelState();
			break;
		case EndlessModePanelState.InActive:
			OnInActivePanelState();
			break;
		}
	}

	private void OnActivePanelState()
	{
		activeTimerLabel = cycleTimerLabel;
		Helpers.GameObjectSetActive(cycleTimerLabel.gameObject, value: true);
		Helpers.GameObjectSetActive(MissionPanelStateObjects[0], value: true);
		HelpersUI.SetContentToLabel(endlessMapName, EndlessModeHelpers.GetNormalCurrentEndlessModeMapName);
		HelpersUI.SetContentToLabel(endlessExpertMapName, EndlessModeHelpers.GetExpertCurrentEndlessModeMapName);
		currentRewardTier.text = "";
		EndlessModeHelpers.GetCurrentLeaderboardPosition(OnGetCurrentLeaderboardPositionHandler);
	}

	private void OnGetCurrentLeaderboardPositionHandler(LeaderboardPosition obj)
	{
		if (obj != null && !(this == null))
		{
			if (obj.Position <= 100)
			{
				currentRewardTier.text = $"Rank {obj.Position}";
				return;
			}
			int num = Mathf.Clamp(Mathf.CeilToInt((float)obj.Position / (float)obj.LeaderboardCount * 100f), 1, 100);
			currentRewardTier.text = $"Top {num}%";
		}
	}

	private void OnInActivePanelState()
	{
		activeTimerLabel = nextCycleTimerLabel;
		Helpers.GameObjectSetActive(cycleTimerLabel.gameObject, value: false);
		if (EndlessModeHelpers.GetNextEndlessModeCalendarDefinition() == null)
		{
			Helpers.GameObjectSetActive(MissionPanelStateObjects[0], value: true);
			HelpersUI.SetContentToLabel(endlessMapName, string.Empty);
		}
		else
		{
			Helpers.GameObjectSetActive(MissionPanelStateObjects[1], value: true);
			HelpersUI.SetContentToLabel(nextEndlessMapName, EndlessModeHelpers.GetNextNormalEndlessModeMapName);
			HelpersUI.SetContentToLabel(nextEndlessExpertMapName, EndlessModeHelpers.GetNextExpertEndlessModeMapName);
		}
		EndlessModeHelpers.CheckForUnclaimedRewards();
	}

	private void OnLockedPanelState()
	{
		activeTimerLabel = cycleTimerLabel;
		Helpers.GameObjectSetActive(cycleTimerLabel.gameObject, value: false);
		Helpers.GameObjectSetActive(MissionPanelStateObjects[2], value: true);
		HelpersUI.SetContentToLabel(lockedLabel, LocalizationManager.GetText("Popup.MissionHub.OutpostUnlockAtLevel{CouncilLevel}", GameManager.Instance.gameEconomyData.EndlessModeConfig.CouncilLockLevel));
	}

	private void SetupOnClickDialogButton()
	{
		EventDelegate.Set(dialobButton.onClick, OnClickDialogButton);
	}

	private void OnClickDialogButton()
	{
		if (EndlessModeHelpers.UnSeenEndlessPassTokens)
		{
			FeatureUIHighlights.MarkHighlightExpired(FeatureUIHighlights.FeaturesIds.EndlessModeUnlocked);
		}
		MissionHubNavigation.TryOpenEndlessMode();
		EventManager.NotifyClick("Endless");
	}

	private string FormatTimeLeft(long timeLeft)
	{
		string text = Helpers.FormatTime(timeLeft);
		switch (currentEndlessModePanelState)
		{
		case EndlessModePanelState.Locked:
		case EndlessModePanelState.Active:
			if (timeLeft <= 0)
			{
				return "";
			}
			return LocalizationManager.GetText("OutpostSeason.EndsIn{Time}", text);
		case EndlessModePanelState.InActive:
			if (timeLeft <= 0)
			{
				return "";
			}
			return LocalizationManager.GetText("OutpostSeason.StartsIn{Time}", text);
		default:
			return "";
		}
	}

	private void DeActivateAllContainers()
	{
		foreach (GameObject missionPanelStateObject in MissionPanelStateObjects)
		{
			missionPanelStateObject.SetActive(value: false);
		}
	}

	private void CheckHighLightState()
	{
		Helpers.GameObjectSetActive(unlockEffect, FeatureUIHighlights.IsActive(FeatureUIHighlights.FeaturesIds.EndlessModeUnlocked));
	}
}
