using System;
using TWDModel;
using TWDModel.ContentTypes;
using UnityEngine;

public class EndlessNormalMissionHubPopup : HUDElement
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

	[SerializeField]
	private EndlessModeNormalRewardList endlessModeNormalRewardList;

	[SerializeField]
	private UILabel rewardCountLabel;

	[SerializeField]
	private GameObject totalScoreContent;

	[SerializeField]
	private GameObject maxScoreGameObject;

	[SerializeField]
	private UIScrollView rankingCardsScrollView;

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
	private UIButtonExtended switchButton;

	[SerializeField]
	private GameObject lockedSwitchGameObject;

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

	private float verticalPadding = 66.5f;

	public override void Open()
	{
		base.Open();
		UpdateUI();
		int index = EndlessModeHelpers.GetClaimedNormalProgressRewardIndex.Count;
		GameManager.Instance.TimingManager.Timer(TimeSpan.FromSeconds(0.10000000149011612), delegate
		{
			ScrollTo(index);
		});
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
		if (EndlessModeHelpers.GetCurrentNormalAttemptCount() == 0)
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
			endlessModeAttemptScorePopup.OpenWithIndex(0, EndlessModeGameModeType.Normal);
		}
	}

	private void SetupOnClickTotalScoreToolTip()
	{
		EventDelegate.Set(totalScoreToolTipButton.onClick, OnClickTotalScoreToolTip);
	}

	private void OnClickTotalScoreToolTip()
	{
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_click");
		int attemptsToSumForFinalScoreNormal = EndlessModeHelpers.EndlessModeConfig.AttemptsToSumForFinalScoreNormal;
		TooltipManager.OpenTextBoxWithText(totalScoreToolTipTarget, LocalizationManager.GetText("Endless.Hub.TotalScore.Tooltip{Parameter}", attemptsToSumForFinalScoreNormal));
	}

	public void OnClickProfessionButton()
	{
		PopupProfessionTip popupProfessionTip = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.EndlessProfessionTipPopup) as PopupProfessionTip;
		if (popupProfessionTip != null)
		{
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_click");
			popupProfessionTip.Open();
			popupProfessionTip.SetTipContent(EndlessModeGameModeType.Normal);
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
		Cashier startMissionCashier = EndlessModeHelpers.GetCurrentMissionModel(EndlessModeGameModeType.Normal).GetStartMissionCashier();
		if (!EndlessModeHelpers.EndlessModeConfig.MaxoutRetryPass && EndlessModeHelpers.GetAttemptsScoreNormal() >= EndlessModeHelpers.GetMaxEndlessNormalModeScore())
		{
			playButton.SetContentToIconOne("");
			playButton.SetContentToLabelOne("");
			playButton.SetContentToLabelTwo("");
			playButton.SetContentToLabelThree(LocalizationManager.GetText("Endless.Hub.PlayButton"));
			Helpers.GameObjectSetActive(goldAttemptContainer, value: false);
			Helpers.GameObjectSetActive(scanGoldAttemptContainer, value: false);
		}
		else
		{
			playButton.SetContentToLabelThree("");
			if (EndlessModeHelpers.EndlessModeConfig.DailyGoldAttemptCount > 0)
			{
				Helpers.GameObjectSetActive(goldAttemptContainer, !startMissionCashier.CanAfford());
				HelpersUI.SetContentToLabel(goldAttemptLabel, LocalizationManager.GetText("Endless.Hub.PlayButton.GoldAttemptsLeft{Parameters}", EndlessModeHelpers.GetCurrentGoldAttemptCount, EndlessModeHelpers.EndlessModeConfig.DailyGoldAttemptCount), !startMissionCashier.CanAfford());
				Helpers.GameObjectSetActive(scanGoldAttemptContainer, !startMissionCashier.CanAfford());
				HelpersUI.SetContentToLabel(scanGoldAttemptLabel, LocalizationManager.GetText("Endless.Hub.PlayButton.GoldAttemptsLeft{Parameters}", EndlessModeHelpers.GetCurrentGoldAttemptCount, EndlessModeHelpers.EndlessModeConfig.DailyGoldAttemptCount), !startMissionCashier.CanAfford());
			}
			string spriteName = ((!startMissionCashier.CanAfford()) ? "Ui_Icon_Resource_Gold" : "Ui_Icon_EndlessPassToken");
			string content = ((!startMissionCashier.CanAfford()) ? EndlessModeHelpers.GetEndlessTokenPriceInGold.ToString() : EndlessModeHelpers.EndlessModeConfig.MissionBaseCost.ToString());
			playButton.SetContentToIconOne(spriteName);
			playButton.SetContentToColorLabelOne(LocalizationManager.GetText("Endless.Hub.PlayButton"), EndlessModeHelpers.CanAttemptNormalMode() ? Color.white : Color.red);
			playButton.SetContentToColorLabelTwo(content, EndlessModeHelpers.CanAttemptNormalMode() ? Color.white : Color.red);
		}
		if (EndlessModeHelpers.GetAttemptsScoreNormal() >= EndlessModeHelpers.GetMaxEndlessNormalModeScore())
		{
			ScanButtonExtended.isEnabled = false;
			if (!EndlessModeHelpers.EndlessModeConfig.MaxoutRetry)
			{
				playButton.isEnabled = false;
			}
			else
			{
				playButton.isEnabled = true;
			}
		}
		EventDelegate.Set(playButton.onClick, delegate
		{
			OnClickPlayButton(EndlessModeHelpers.CanAttemptNormalMode());
		});
	}

	private void OnClickPlayButton(bool canAttempt)
	{
		if (canAttempt)
		{
			if (Helpers.ExecuteCommand(new ChangeEndlessModeGameDifficultyCommand(EndlessModeGameModeType.Normal)) == TWDModelResult.OK)
			{
				SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_click");
				TeamSelectionPopup teamSelectionPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.MapTeamSelection) as TeamSelectionPopup;
				MapMissionModel currentMissionModel = EndlessModeHelpers.GetCurrentMissionModel(EndlessModeGameModeType.Normal);
				if (teamSelectionPopup != null && currentMissionModel != null)
				{
					teamSelectionPopup.SurvivorType = SurvivorContainerModel.SurvivorType.Combat;
					teamSelectionPopup.SetUITypeOpenOnClose(UIType.EndlessNormalMissionHubPopup);
					teamSelectionPopup.OpenForModel(currentMissionModel);
					EventManager.NotifyClick("SelectTeam");
					Close();
				}
			}
		}
		else
		{
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_click");
			if (EndlessModeHelpers.UsedAllGoldAttempts)
			{
				TooltipManager.OpenTextBoxWithText(goldAttemptTooltipTarget, LocalizationManager.GetText("Endless.Hub.GoldAttemptToolTip{0}", EndlessModeHelpers.EndlessModeConfig.DailyGoldAttemptCount));
			}
			else
			{
				MiniShopPopup.OpenWithTotalRequiredCurrencyAmount(CurrencyType.Diamonds, EndlessModeHelpers.GetEndlessTokenPriceInGold);
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

	private void OnClickCampButton()
	{
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_dialog_exit");
		CampManager.Instance.GoToCamp();
		EventManager.NotifyClick("Camp");
	}

	private void SetupOnClickSwitchButton()
	{
		if (!EndlessModeHelpers.IsExpertMdeLockedByCouncilLevel && EndlessModeHelpers.HasGeneratedExpertModeActors)
		{
			EventDelegate.Set(switchButton.onClick, OnClickSwitchButton);
		}
	}

	private void OnClickSwitchButton()
	{
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_dialog_exit");
		base.OnClickClose();
		EndlessMissionHubPopup endlessMissionHubPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.EndlessMissionHubPopup) as EndlessMissionHubPopup;
		if (endlessMissionHubPopup != null)
		{
			SingularityMonoBehaviour<HUDManager>.Instance.CloseAllOpenPopupsAndDialogs();
			endlessMissionHubPopup.Open();
		}
	}

	private void TryToScrollLatestAttemptScore()
	{
		int latestNormalEndlessModeAttemptIndex = EndlessModeHelpers.GetLatestNormalEndlessModeAttemptIndex();
		if (latestNormalEndlessModeAttemptIndex > -1)
		{
			endlessModeAttemptScoreList.CenterToSelectedAttemptEntry(latestNormalEndlessModeAttemptIndex);
		}
	}

	private void SetupOnClickScanButton()
	{
		ScanButtonExtended.SetContentToColorLabelOne(LocalizationManager.GetText("AutoClear.Info.Intro.Title"), EndlessModeHelpers.CanAttemptNormalMode() ? Color.white : Color.red);
		if (EndlessModeHelpers.GetOrderedNormalAttemptDataByScore().Count == 0)
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
		Cashier startMissionCashier = EndlessModeHelpers.GetCurrentMissionModel(EndlessModeGameModeType.Normal).GetStartMissionCashier();
		if (startMissionCashier.CanAfford())
		{
			ConfirmationPopup obj = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.ConfirmationPopup) as ConfirmationPopup;
			obj.SetContent(LocalizationManager.GetText("AutoClear.Info.Intro.Title"), LocalizationManager.GetText("Popup.AutoClear.Reconfirm"));
			obj.SetCurrencies(startMissionCashier);
			obj.SetCallbacks(delegate
			{
				GoScanCommand();
			});
			obj.SetOkButtonLabel(LocalizationManager.GetText("Button.Ok"));
			obj.SetCancelButtonLabel(LocalizationManager.GetText("Button.Cancel"));
			obj.Open();
		}
		else if (EndlessModeHelpers.CanAttemptNormalMode())
		{
			ConsumeCurrencyCommandUtils.Execute(null, startMissionCashier, delegate(TWDModelResult result)
			{
				if (result == TWDModelResult.OK)
				{
					GoScanCommand();
				}
			});
		}
		else if (EndlessModeHelpers.UsedAllGoldAttempts)
		{
			TooltipManager.OpenTextBoxWithText(goldAttemptTooltipTarget, LocalizationManager.GetText("Endless.Hub.GoldAttemptToolTip{0}", EndlessModeHelpers.EndlessModeConfig.DailyGoldAttemptCount));
		}
		else
		{
			MiniShopPopup.OpenWithTotalRequiredCurrencyAmount(CurrencyType.Diamonds, EndlessModeHelpers.GetEndlessTokenPriceInGold);
		}
	}

	private void GoScanCommand()
	{
		ScanEndlessNormalCommand scanEndlessNormalCommand = new ScanEndlessNormalCommand();
		if (Helpers.ExecuteCommand(scanEndlessNormalCommand) == TWDModelResult.OK)
		{
			IAPConfirmPopupNew iAPConfirmPopupNew = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.IAPConfirmPopupNew) as IAPConfirmPopupNew;
			if (iAPConfirmPopupNew != null)
			{
				iAPConfirmPopupNew.OpenForRewards(scanEndlessNormalCommand.Rewards.RewardsList);
				iAPConfirmPopupNew.SetContent(LocalizationManager.GetText("Popup.IAPConfirm.Title.GenericReward"), null);
			}
			UpdateUI();
		}
	}

	public override void UpdateUI()
	{
		cycleTimeLeft = EndlessModeHelpers.GetCurrentCycleTimeLeft;
		Helpers.GameObjectSetActive(lockedSwitchGameObject, EndlessModeHelpers.IsExpertMdeLockedByCouncilLevel || !EndlessModeHelpers.HasGeneratedExpertModeActors);
		HelpersUI.SetContentToLabel(endlessMapName, EndlessModeHelpers.GetNormalCurrentEndlessModeMapName);
		if (EndlessModeHelpers.GetAttemptsScoreNormal() < EndlessModeHelpers.GetMaxEndlessNormalModeScore())
		{
			Helpers.GameObjectSetActive(totalScoreContent, value: true);
			Helpers.GameObjectSetActive(maxScoreGameObject, value: false);
			HelpersUI.SetContentToLabel(totalScoreLabel, EndlessModeHelpers.GetFormattedOverAllAttemptsScoreNormal());
		}
		else
		{
			Helpers.GameObjectSetActive(totalScoreContent, value: false);
			Helpers.GameObjectSetActive(maxScoreGameObject, value: true);
		}
		HelpersUI.SetContentToLabel(rewardCountLabel, LocalizationManager.GetText("SurvivalMode_Selection_Normal_ScoreProgress_Title") + Math.Min(EndlessModeHelpers.GetAttemptsScoreNormal(), EndlessModeHelpers.GetMaxEndlessNormalModeScore()) + "/" + EndlessModeHelpers.GetMaxEndlessNormalModeScore());
		SetupOwnAttemptScores();
		SetupOnClickMyScoresButton();
		SetupOnClickTotalScoreToolTip();
		SetupOnClickBackButton();
		SetupOnClickPlayButton();
		SetupOnClickInfoButton();
		SetupOnClickCampButton();
		SetupOnClickSwitchButton();
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
			endlessModeAttemptScoreList.UpdateUI(EndlessModeGameModeType.Normal);
			endlessModeNormalRewardList.UpdateUI();
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

	public void OnClickRewardButton()
	{
		ClaimNormalProgressRewardCommand claimNormalProgressRewardCommand = new ClaimNormalProgressRewardCommand();
		if (Helpers.ExecuteCommand(claimNormalProgressRewardCommand) == TWDModelResult.OK)
		{
			IAPConfirmPopupNew iAPConfirmPopupNew = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.IAPConfirmPopupNew) as IAPConfirmPopupNew;
			if (iAPConfirmPopupNew != null)
			{
				iAPConfirmPopupNew.OpenForRewards(claimNormalProgressRewardCommand.Rewards.RewardsList);
				iAPConfirmPopupNew.SetContent(LocalizationManager.GetText("Popup.IAPConfirm.Title.GenericReward"), null);
			}
			UpdateUI();
		}
	}

	private void ScrollTo(int index)
	{
		rankingCardsScrollView.ResetPosition();
		float y = CalculateTierScroll(index);
		rankingCardsScrollView.MoveRelative(new Vector3(0f, y));
		rankingCardsScrollView.RestrictWithinBounds(instant: true);
	}

	private float CalculateTierScroll(int index)
	{
		return (float)index * verticalPadding;
	}
}
