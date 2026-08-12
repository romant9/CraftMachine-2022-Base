using TWDModel;
using TWDModel.ContentTypes;
using UnityEngine;

public class EndlessMissionHubPopup : HUDElement
{
	[Header("Timer")]
	[SerializeField]
	private UILabel cycleTimerLabel;

	private long cycleTimeLeft;

	[Header("Map")]
	[SerializeField]
	private UILabel endlessMapName;

	[Header("My Scores")]
	[SerializeField]
	private GameObject attemptScoreContainer;

	[SerializeField]
	private GameObject noScoresContainer;

	[SerializeField]
	private UILabel totalScoreLabel;

	[SerializeField]
	private UIButtonExtended myScoreButtonExtended;

	[SerializeField]
	private UIButton totalScoreToolTipButton;

	[SerializeField]
	private GameObject totalScoreToolTipTarget;

	[SerializeField]
	private GameObject totalScoreContainer;

	[SerializeField]
	private EndlessModeAttemptScoreList endlessModeAttemptScoreList;

	[Header("General")]
	[SerializeField]
	private UIButtonExtended backButton;

	[SerializeField]
	private UIButtonWithLabelAndIcon playButton;

	[SerializeField]
	private UIButtonWithLabelAndIcon ScanButtonExtended;

	[SerializeField]
	private UIButtonExtended ScanInfoButton;

	[SerializeField]
	private UIButtonExtended infoButton;

	[SerializeField]
	private UIButtonExtended campButton;

	[SerializeField]
	private UIButtonExtended leaderboardButton;

	[SerializeField]
	private UIButtonExtended switchButton;

	[Header("Gold Attempt")]
	[SerializeField]
	private GameObject goldAttemptContainer;

	[SerializeField]
	private UILabel goldAttemptLabel;

	private UIButton goldAttemptToolTipButton;

	[SerializeField]
	private GameObject goldAttemptTooltipTarget;

	[SerializeField]
	private GameObject scanGoldAttemptContainer;

	[SerializeField]
	private UILabel scanGoldAttemptLabel;

	public override void Open()
	{
		base.Open();
		UpdateUI();
	}

	public override void Update()
	{
		base.Update();
		cycleTimeLeft -= (long)(Time.deltaTime * 1000f);
		cycleTimerLabel.text = FormatTimeLeft(cycleTimeLeft);
		if (cycleTimeLeft <= 0)
		{
			GoToMissionHub();
		}
	}

	private void SetupOwnAttemptScores()
	{
		if (EndlessModeHelpers.GetCurrentExpertAttemptCount() == 0)
		{
			Helpers.GameObjectSetActive(attemptScoreContainer, value: false);
			Helpers.GameObjectSetActive(noScoresContainer, value: true);
			Helpers.GameObjectSetActive(totalScoreContainer, value: false);
			Helpers.GameObjectSetActive(myScoreButtonExtended.gameObject, value: false);
		}
		else
		{
			Helpers.GameObjectSetActive(attemptScoreContainer, value: true);
			Helpers.GameObjectSetActive(noScoresContainer, value: false);
			Helpers.GameObjectSetActive(totalScoreContainer, value: true);
			Helpers.GameObjectSetActive(myScoreButtonExtended.gameObject, value: true);
		}
	}

	private void SetupOnClickMyScoresButton()
	{
		myScoreButtonExtended.SetOnPressCallback(OnClickMyScores);
	}

	private void OnClickMyScores(UIButtonExtended uiButtonExtended)
	{
		EndlessModeAttemptScorePopup endlessModeAttemptScorePopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.EndlessModeAttemptScorePopup) as EndlessModeAttemptScorePopup;
		if (endlessModeAttemptScorePopup != null)
		{
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_click");
			endlessModeAttemptScorePopup.OpenWithIndex(0, EndlessModeGameModeType.Expert);
		}
	}

	private void SetupOnClickTotalScoreToolTip()
	{
		EventDelegate.Set(totalScoreToolTipButton.onClick, OnClickTotalScoreToolTip);
	}

	private void OnClickTotalScoreToolTip()
	{
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_click");
		int attemptsToSumForFinalScoreExpert = EndlessModeHelpers.EndlessModeConfig.AttemptsToSumForFinalScoreExpert;
		TooltipManager.OpenTextBoxWithText(totalScoreToolTipTarget, LocalizationManager.GetText("Endless.Hub.TotalScore.Tooltip{Parameter}", attemptsToSumForFinalScoreExpert));
	}

	public void OnClickProfessionButton()
	{
		PopupProfessionTip popupProfessionTip = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.EndlessProfessionTipPopup) as PopupProfessionTip;
		if (popupProfessionTip != null)
		{
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_click");
			popupProfessionTip.Open();
			popupProfessionTip.SetTipContent(EndlessModeGameModeType.Expert);
		}
	}

	private void SetupOnClickBackButton()
	{
		backButton.SetOnPressCallback(OnClickBackButton);
	}

	private void OnClickBackButton(UIButtonExtended uiButtonExtended)
	{
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_dialog_exit");
		GoToMissionHub();
	}

	private void GoToMissionHub()
	{
		SingularityMonoBehaviour<HUDManager>.Instance.CloseIfExists(UIType.MapTeamSelection);
		UITypeOpenOnClose = UIType.MissionHubPopup;
		base.OnClickClose();
	}

	private string FormatTimeLeft(long timeLeft)
	{
		string text = Helpers.FormatTime(timeLeft);
		if (timeLeft <= 0)
		{
			return "";
		}
		return LocalizationManager.GetText("OutpostSeason.EndsIn{Time}", text);
	}

	private void SetupOnClickPlayButton()
	{
		Cashier startMissionExpertModeCashier = EndlessModeHelpers.GetCurrentMissionModel(EndlessModeGameModeType.Expert).GetStartMissionExpertModeCashier();
		if (EndlessModeHelpers.EndlessModeConfig.DailyGoldExpertAttemptCount > 0)
		{
			Helpers.GameObjectSetActive(goldAttemptContainer, !startMissionExpertModeCashier.CanAfford());
			HelpersUI.SetContentToLabel(goldAttemptLabel, LocalizationManager.GetText("Endless.Hub.PlayButton.GoldAttemptsLeft{Parameters}", EndlessModeHelpers.GetExpertCurrentGoldAttemptCount, EndlessModeHelpers.EndlessModeConfig.DailyGoldExpertAttemptCount), !startMissionExpertModeCashier.CanAfford());
			Helpers.GameObjectSetActive(scanGoldAttemptContainer, !startMissionExpertModeCashier.CanAfford());
			HelpersUI.SetContentToLabel(scanGoldAttemptLabel, LocalizationManager.GetText("Endless.Hub.PlayButton.GoldAttemptsLeft{Parameters}", EndlessModeHelpers.GetExpertCurrentGoldAttemptCount, EndlessModeHelpers.EndlessModeConfig.DailyGoldExpertAttemptCount), !startMissionExpertModeCashier.CanAfford());
		}
		string spriteName = ((!startMissionExpertModeCashier.CanAfford()) ? "Ui_Icon_Resource_Gold" : "Ui_Icon_EndlessExpertPassToken");
		string content = ((!startMissionExpertModeCashier.CanAfford()) ? EndlessModeHelpers.GetExpertEndlessTokenPriceInGold.ToString() : EndlessModeHelpers.EndlessModeConfig.MissionBaseCost.ToString());
		playButton.SetContentToIconOne(spriteName);
		playButton.SetContentToColorLabelOne(LocalizationManager.GetText("Endless.Hub.PlayButton"), EndlessModeHelpers.CanAttemptExpertMode() ? Color.white : Color.red);
		playButton.SetContentToColorLabelTwo(content, EndlessModeHelpers.CanAttemptExpertMode() ? Color.white : Color.red);
		EventDelegate.Set(playButton.onClick, delegate
		{
			OnClickPlayButton(EndlessModeHelpers.CanAttemptExpertMode());
		});
	}

	private void OnClickPlayButton(bool canAttempt)
	{
		if (canAttempt)
		{
			if (Helpers.ExecuteCommand(new ChangeEndlessModeGameDifficultyCommand(EndlessModeGameModeType.Expert)) == TWDModelResult.OK)
			{
				SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_click");
				TeamSelectionPopup teamSelectionPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.MapTeamSelection) as TeamSelectionPopup;
				MapMissionModel currentMissionModel = EndlessModeHelpers.GetCurrentMissionModel(EndlessModeGameModeType.Expert);
				if (teamSelectionPopup != null && currentMissionModel != null)
				{
					teamSelectionPopup.SurvivorType = SurvivorContainerModel.SurvivorType.Combat;
					teamSelectionPopup.SetUITypeOpenOnClose(UIType.EndlessMissionHubPopup);
					teamSelectionPopup.OpenForModel(currentMissionModel);
					EventManager.NotifyClick("SelectTeam");
					Close();
				}
			}
		}
		else
		{
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_click");
			if (EndlessModeHelpers.UsedAllGoldExpertAttempts)
			{
				TooltipManager.OpenTextBoxWithText(goldAttemptTooltipTarget, LocalizationManager.GetText("Endless.Hub.GoldAttemptToolTip{0}", EndlessModeHelpers.EndlessModeConfig.DailyGoldExpertAttemptCount));
			}
			else
			{
				MiniShopPopup.OpenWithTotalRequiredCurrencyAmount(CurrencyType.Diamonds, EndlessModeHelpers.GetExpertEndlessTokenPriceInGold);
			}
		}
	}

	private void SetupOnClickInfoButton()
	{
		EventDelegate.Set(infoButton.onClick, OnClickInfoButton);
	}

	private void OnClickInfoButton()
	{
		PopupQuickTip popupQuickTip = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.EndlessModeMissionHubInfoPopup) as PopupQuickTip;
		if (popupQuickTip != null)
		{
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_click");
			popupQuickTip.Open();
		}
	}

	private void SetupOnClickCampButton()
	{
		EventDelegate.Set(campButton.onClick, OnClickCampButton);
	}

	private void SetupOnClickSwitchButton()
	{
		EventDelegate.Set(switchButton.onClick, OnClickSwitchButton);
	}

	private void OnClickCampButton()
	{
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_dialog_exit");
		CampManager.Instance.GoToCamp();
		EventManager.NotifyClick("Camp");
	}

	private void OnClickSwitchButton()
	{
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_dialog_exit");
		base.OnClickClose();
		EndlessNormalMissionHubPopup endlessNormalMissionHubPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.EndlessNormalMissionHubPopup) as EndlessNormalMissionHubPopup;
		if (endlessNormalMissionHubPopup != null)
		{
			SingularityMonoBehaviour<HUDManager>.Instance.CloseAllOpenPopupsAndDialogs();
			endlessNormalMissionHubPopup.Open();
		}
	}

	private void SetupOnClickLeaderboardButton()
	{
		EventDelegate.Set(leaderboardButton.onClick, OnClickLeaderboardButton);
	}

	private void OnClickLeaderboardButton()
	{
		HUDElement hUDElement = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.EndlessModeHighScorePopup);
		if (hUDElement != null)
		{
			hUDElement.Open();
		}
	}

	private void TryToScrollLatestAttemptScore()
	{
		int latestExpertEndlessModeAttemptIndex = EndlessModeHelpers.GetLatestExpertEndlessModeAttemptIndex();
		if (latestExpertEndlessModeAttemptIndex > -1)
		{
			endlessModeAttemptScoreList.CenterToSelectedAttemptEntry(latestExpertEndlessModeAttemptIndex);
		}
	}

	private void SetupOnClickScanButton()
	{
		ScanButtonExtended.SetContentToColorLabelOne(LocalizationManager.GetText("AutoClear.Info.Intro.Title"), EndlessModeHelpers.CanAttemptExpertMode() ? Color.white : Color.red);
		if (EndlessModeHelpers.GetOrderedExpertAttemptDataByScore().Count == 0)
		{
			ScanButtonExtended.isEnabled = false;
			return;
		}
		EventDelegate.Set(ScanButtonExtended.onClick, delegate
		{
			OnClickScan();
		});
	}

	private void OnClickScan()
	{
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_click");
		Cashier startMissionExpertModeCashier = EndlessModeHelpers.GetCurrentMissionModel(EndlessModeGameModeType.Expert).GetStartMissionExpertModeCashier();
		if (startMissionExpertModeCashier.CanAfford())
		{
			ConfirmationPopup obj = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.ConfirmationPopup) as ConfirmationPopup;
			obj.SetContent(LocalizationManager.GetText("AutoClear.Info.Intro.Title"), LocalizationManager.GetText("Popup.AutoClear.Reconfirm"));
			obj.SetCurrencies(startMissionExpertModeCashier);
			obj.SetCallbacks(delegate
			{
				GoScanCommand();
			});
			obj.SetOkButtonLabel(LocalizationManager.GetText("Button.Ok"));
			obj.SetCancelButtonLabel(LocalizationManager.GetText("Button.Cancel"));
			obj.Open();
		}
		else if (EndlessModeHelpers.CanAttemptExpertMode())
		{
			ConsumeCurrencyCommandUtils.Execute(null, startMissionExpertModeCashier, delegate(TWDModelResult result)
			{
				if (result == TWDModelResult.OK)
				{
					GoScanCommand();
				}
			});
		}
		else if (EndlessModeHelpers.UsedAllGoldExpertAttempts)
		{
			TooltipManager.OpenTextBoxWithText(goldAttemptTooltipTarget, LocalizationManager.GetText("Endless.Hub.GoldAttemptToolTip{0}", EndlessModeHelpers.EndlessModeConfig.DailyGoldExpertAttemptCount));
		}
		else
		{
			MiniShopPopup.OpenWithTotalRequiredCurrencyAmount(CurrencyType.Diamonds, EndlessModeHelpers.GetExpertEndlessTokenPriceInGold);
		}
	}

	private void GoScanCommand()
	{
		ScanEndlessExpertCommand scanEndlessExpertCommand = new ScanEndlessExpertCommand();
		if (Helpers.ExecuteCommand(scanEndlessExpertCommand) == TWDModelResult.OK)
		{
			IAPConfirmPopupNew iAPConfirmPopupNew = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.IAPConfirmPopupNew) as IAPConfirmPopupNew;
			if (iAPConfirmPopupNew != null)
			{
				iAPConfirmPopupNew.OpenForRewards(scanEndlessExpertCommand.Rewards.RewardsList);
				iAPConfirmPopupNew.SetContent(LocalizationManager.GetText("Popup.IAPConfirm.Title.GenericReward"), null);
			}
			UpdateUI();
			UIEvent.Send("EndlessScanEvent");
		}
	}

	public override void UpdateUI()
	{
		cycleTimeLeft = EndlessModeHelpers.GetCurrentCycleTimeLeft;
		HelpersUI.SetContentToLabel(endlessMapName, EndlessModeHelpers.GetExpertCurrentEndlessModeMapName);
		HelpersUI.SetContentToLabel(totalScoreLabel, EndlessModeHelpers.GetFormattedOverAllAttemptsScoreExpert());
		SetupOwnAttemptScores();
		SetupOnClickMyScoresButton();
		SetupOnClickTotalScoreToolTip();
		SetupOnClickBackButton();
		SetupOnClickPlayButton();
		SetupOnClickInfoButton();
		SetupOnClickCampButton();
		SetupOnClickSwitchButton();
		SetupOnClickLeaderboardButton();
		SetupOnClickScanButton();
		SetupOnClickScanInfoButton();
		EndlessModeHelpers.CheckForUnclaimedRewards();
		SingularityMonoBehaviour<AudioManager>.Instance.RequestMusicStateChange(MusicState.EndlessMenus);
		if (!BlackboardUISeenToggle.TryToOpen(UIType.EndlessModeIntroductionPopup, "ToggleEndlessModeIntroductionPopup"))
		{
			if (EndlessModePostMissionTutorial.CanStartTutorial())
			{
				new EndlessModePostMissionTutorial();
			}
			TryToScrollLatestAttemptScore();
			if (endlessModeAttemptScoreList.gameObject.activeInHierarchy)
			{
				endlessModeAttemptScoreList.UpdateUI(EndlessModeGameModeType.Expert);
			}
		}
	}

	private void SetupOnClickScanInfoButton()
	{
		EventDelegate.Set(ScanInfoButton.onClick, OnClickScanInfoButton);
	}

	private void OnClickScanInfoButton()
	{
		PopupScanQuickTip popupScanQuickTip = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.EndlessModeMissionHubScanInfoPopup) as PopupScanQuickTip;
		if (popupScanQuickTip != null)
		{
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_click");
			popupScanQuickTip.Open();
		}
	}
}
