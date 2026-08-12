using System.Collections.Generic;
using TWD.Externals;
using TWDModel;
using UnityEngine;

public class ChallengeInfoPanel : MonoBehaviourExtended
{
	[SerializeField]
	private GameObject TitlesAndTextParent;

	[SerializeField]
	private UIChallengeProgressBar playerProgressBar;

	[SerializeField]
	private UIChallengeProgressBar guildProgressBar;

	[SerializeField]
	private UIPlayerNameLabel playerName;

	[SerializeField]
	private UIPlayerGuildNameLabel guildName;

	[SerializeField]
	private GameObject challengeCountdownParent;

	[SerializeField]
	private NUICountdownTimer challengeCountdownTimer;

	[SerializeField]
	private GameObject noGuildParent;

	[SerializeField]
	private GameObject collectSkipTokensAnimationPrefab;

	[Header("Progress Content")]
	[SerializeField]
	private GameObject progressContentParent;

	[SerializeField]
	private UIButtonToggleSet toggleSet;

	[SerializeField]
	private GameObject personalProgressParent;

	[SerializeField]
	private GameObject guildProgressParent;

	[SerializeField]
	private UIRewardsProgressBar playerRewardsProgressBar;

	[SerializeField]
	private UIRewardsProgressBar guildRewardsProgressBar;

	[SerializeField]
	private UILabel playerStarCountLabel;

	[SerializeField]
	private UILabel guildStarCountLabel;

	[Header("Cycle Parts")]
	[SerializeField]
	private GameObject cycleParent;

	[SerializeField]
	private UIButtonWithLabelAndIcon cycleButton;

	[SerializeField]
	private UIButtonWithLabelAndIcon plightButton;

	[SerializeField]
	private GameObject apocalypticEffect;

	[SerializeField]
	private UIButtonWithLabelAndIcon apocalypticInfoButton;

	[SerializeField]
	private GameObject apocalypticBG;

	[SerializeField]
	private Color cycleButtonColorComplete;

	[SerializeField]
	private Color cycleButtonColorNormal;

	[SerializeField]
	private GameObject cycleCompleteEffect;

	[SerializeField]
	private GameObject cycleTimerContainer;

	[SerializeField]
	private UILabel cycleTimerLabel;

	[SerializeField]
	private UILabel roundCounterLabel;

	private int roundCounterLabelCached = -1;

	private int apRoundCounterLabelCached = -1;

	[Header("Title Parts")]
	[SerializeField]
	private GameObject titleParent;

	[SerializeField]
	[Tooltip("Contains Challenge difficulty and other elements. Realigns content when there is no challenge available.")]
	private UIGrid challengeGrid;

	[SerializeField]
	private UIButtonWithLabelAndIcon challengeNameButton;

	[SerializeField]
	private UIChallengeDifficultyProgressBar difficultyProgressBar;

	[SerializeField]
	private GameObject missionCostContainer;

	[SerializeField]
	private UISprite missionCostSprite;

	[SerializeField]
	private UILabel missionCostLabel;

	[SerializeField]
	private UILabel challengeTimeLeftLabel;

	[SerializeField]
	private UISprite skipTokenCountIcon;

	[SerializeField]
	private UILabel skipTokenCountLabel;

	private int skipTokenCountLabelCached = -1;

	private int apSkipTokenCountLabelCached = -1;

	[SerializeField]
	private GameObject challengeCompletedMessage;

	[SerializeField]
	private GameObject apocalypticChallengeButton;

	[SerializeField]
	private WeeklyChallenge nextChallenge;

	private long challengeTime;

	[SerializeField]
	private PlightListPanel plightListPanel;

	[SerializeField]
	private GameObject weeklyChallengeRewardIcon;

	[SerializeField]
	private UIButtonExtended weekIconButton;

	[SerializeField]
	private UISprite starSprite;

	[SerializeField]
	public GameObject weeklyChallengeActivityObj;

	[SerializeField]
	private UITexture weeklyChallengeTexture;

	[SerializeField]
	public UISprite weeklyChallengeActivityIcon;

	[SerializeField]
	public UILabel weeklyChallengeActivityLabel;

	private void Awake()
	{
		DebugIdString = "ChallengeInfoPanel";
	}

	private void OnEnable()
	{
		if (toggleSet != null)
		{
			toggleSet.SetInitialToggle(0);
		}
		UpdateUI();
		Helpers.GameObjectSetActive(challengeCountdownTimer, value: false);
		AddListeners();
		SingularityMonoBehaviour<LocalizationManager>.Instance.OnLocalizationLanguageChanged += OnLocalizationLanguageChanged;
	}

	private void OnDisable()
	{
		RemoveListeners();
		SingularityMonoBehaviour<LocalizationManager>.Instance.OnLocalizationLanguageChanged -= OnLocalizationLanguageChanged;
	}

	private void Update()
	{
		if (challengeCountdownTimer != null && nextChallenge != null && nextChallenge.Identifier > 0)
		{
			challengeTime = nextChallenge.StartTimeMilliseconds - GameManager.Instance.playerModel.UtcTimeStamp;
			challengeCountdownTimer.SetCurrentMilliseconds(challengeTime);
			Helpers.GameObjectSetActive(challengeCountdownTimer, value: true);
			if (challengeTime < 0)
			{
				nextChallenge = null;
				DetailMapPopUp.ReloadChallengeMap();
			}
		}
		UpdateTimeleftUI();
		WeeklyChallengeModel weeklyChallengeModel = WeeklyChallengeHelper.GetWeeklyChallengeModel();
		bool flag = weeklyChallengeModel.IsNewCycleLockedByTimer();
		if (cycleTimerContainer != null)
		{
			cycleTimerContainer.SetActive(flag);
		}
		if (cycleTimerLabel != null && weeklyChallengeModel != null && flag)
		{
			HelpersUI.SetContentToLabel(cycleTimerLabel, WeeklyChallengeHelper.GetFormatedTimeLeftToUnlockNextCycle());
		}
		UpdateSkipTokenInfo();
		UpdateRoundInfo();
	}

	public void PlaySkipTokenCollectionAnimation(int skipTokenCount, Transform from, GameObject parent)
	{
		int num = (PlatformInfo.HasFlag(PlatformFlag.SlowGPU) ? 4 : 8);
		for (int i = 0; i < skipTokenCount && i < num; i++)
		{
			CollectAnimation component = Helpers.InstantiateToParent(collectSkipTokensAnimationPrefab, parent).GetComponent<CollectAnimation>();
			component.transform.position = from.position;
			component.StartAnimation(skipTokenCount, skipTokenCountIcon.transform);
		}
	}

	private void UpdateSkipTokenInfo()
	{
		if (WeeklyChallengeHelper.IsNormalChallenge)
		{
			WeeklyChallengeModel weeklyChallengeModel = WeeklyChallengeHelper.GetWeeklyChallengeModel();
			if (weeklyChallengeModel != null && weeklyChallengeModel.PendingSkipTokens != skipTokenCountLabelCached)
			{
				skipTokenCountLabelCached = weeklyChallengeModel.PendingSkipTokens;
				HelpersUI.SetContentToLabel(skipTokenCountLabel, skipTokenCountLabelCached.ToString());
			}
		}
		else
		{
			ApocalypseWeeklyChallengeModel weeklyApocalypticChallengeModel = WeeklyChallengeHelper.GetWeeklyApocalypticChallengeModel();
			if (weeklyApocalypticChallengeModel != null && weeklyApocalypticChallengeModel.PendingSkipTokens != apSkipTokenCountLabelCached)
			{
				apSkipTokenCountLabelCached = weeklyApocalypticChallengeModel.PendingSkipTokens;
				HelpersUI.SetContentToLabel(skipTokenCountLabel, apSkipTokenCountLabelCached.ToString());
			}
		}
	}

	public void OnClickSkipToken()
	{
		if (WeeklyChallengeHelper.IsNormalChallenge)
		{
			WeeklyChallengeModel weeklyChallengeModel = WeeklyChallengeHelper.GetWeeklyChallengeModel();
			if (weeklyChallengeModel != null && weeklyChallengeModel.CurrentDefinition != null)
			{
				TooltipManager.OpenTextBoxWithText(skipTokenCountIcon.gameObject, LocalizationManager.GetText("Map.WeeklyChallenge.RoundPassInfo{RoundsToGetPass}{RoundsLeftToGetPass}", weeklyChallengeModel.GetCurrentCycleRoundsToSkipToken(), weeklyChallengeModel.CalculateRoundsToNextSkipToken()));
			}
			else
			{
				TooltipManager.OpenTextBoxWithText(skipTokenCountIcon.gameObject, LocalizationManager.GetText("Map.WeeklyChallenge.RoundPassInfoGeneral"));
			}
		}
		else
		{
			ApocalypseWeeklyChallengeModel weeklyApocalypticChallengeModel = WeeklyChallengeHelper.GetWeeklyApocalypticChallengeModel();
			if (weeklyApocalypticChallengeModel != null && weeklyApocalypticChallengeModel.CurrentDefinition != null)
			{
				TooltipManager.OpenTextBoxWithText(skipTokenCountIcon.gameObject, LocalizationManager.GetText("Map.ApocalypticWeeklyChallenge.RoundPassInfo"));
			}
			else
			{
				TooltipManager.OpenTextBoxWithText(skipTokenCountIcon.gameObject, LocalizationManager.GetText("Map.WeeklyChallenge.RoundPassInfoGeneral"));
			}
		}
	}

	private void OnLocalizationLanguageChanged(string newLanguage)
	{
		roundCounterLabelCached = -1;
		apRoundCounterLabelCached = -1;
		UpdateRoundInfo();
	}

	private void UpdateRoundInfo()
	{
		if (WeeklyChallengeHelper.IsNormalChallenge)
		{
			WeeklyChallengeModel weeklyChallengeModel = WeeklyChallengeHelper.GetWeeklyChallengeModel();
			if (weeklyChallengeModel != null && weeklyChallengeModel.CurrentCycle != roundCounterLabelCached)
			{
				roundCounterLabelCached = weeklyChallengeModel.CurrentCycle;
				HelpersUI.SetContentToLabel(content: (!GameManager.Instance.gameEconomyData.GetFeature("UseChallengeRoundCap").Enabled) ? LocalizationManager.GetText("Map.WeeklyChallenge.Round{Number}", roundCounterLabelCached + 1) : ((!weeklyChallengeModel.HasCompletedMaxCycles()) ? LocalizationManager.GetText("Map.WeeklyChallenge.Round{Number}", $"{roundCounterLabelCached + 1} / {GameManager.Instance.gameEconomyData.ConfigData.ChallengeRoundCap}") : LocalizationManager.GetText("Map.WeeklyChallenge.Round{Number}", $"{GameManager.Instance.gameEconomyData.ConfigData.ChallengeRoundCap} / {GameManager.Instance.gameEconomyData.ConfigData.ChallengeRoundCap}")), label: roundCounterLabel);
			}
		}
		else
		{
			ApocalypseWeeklyChallengeModel weeklyApocalypticChallengeModel = WeeklyChallengeHelper.GetWeeklyApocalypticChallengeModel();
			if (weeklyApocalypticChallengeModel != null && weeklyApocalypticChallengeModel.CurrentCycle != apRoundCounterLabelCached)
			{
				apRoundCounterLabelCached = weeklyApocalypticChallengeModel.CurrentCycle;
				HelpersUI.SetContentToLabel(content: (!GameManager.Instance.gameEconomyData.GetFeature("UseChallengeRoundCap").Enabled) ? LocalizationManager.GetText("Map.WeeklyChallenge.Round{Number}", apRoundCounterLabelCached) : ((!weeklyApocalypticChallengeModel.HasCompleteMaxRound()) ? LocalizationManager.GetText("Map.WeeklyChallenge.Round{Number}", $"{apRoundCounterLabelCached} / {GameManager.Instance.gameEconomyData.ConfigData.ChallengeApocalypticModeMaxRound}") : LocalizationManager.GetText("Map.WeeklyChallenge.Round{Number}", $"{GameManager.Instance.gameEconomyData.ConfigData.ChallengeApocalypticModeMaxRound} / {GameManager.Instance.gameEconomyData.ConfigData.ChallengeApocalypticModeMaxRound}")), label: roundCounterLabel);
			}
		}
	}

	public virtual void UpdateUI()
	{
		bool isNormalChallenge = WeeklyChallengeHelper.IsNormalChallenge;
		WeeklyChallengeModel weeklyChallengeModel = WeeklyChallengeHelper.GetWeeklyChallengeModel();
		ApocalypseWeeklyChallengeModel weeklyApocalypticChallengeModel = WeeklyChallengeHelper.GetWeeklyApocalypticChallengeModel();
		Helpers.GameObjectSetActive(plightButton, value: false);
		Helpers.GameObjectSetActive(apocalypticInfoButton, value: false);
		Helpers.GameObjectSetActive(apocalypticBG, value: false);
		Helpers.GameObjectSetActive(apocalypticEffect, value: false);
		Helpers.GameObjectSetActive(weeklyChallengeRewardIcon, value: false);
		if (weeklyChallengeModel != null && weeklyApocalypticChallengeModel != null && WeeklyChallengeHelper.IsChallengeOngoing())
		{
			if (playerProgressBar != null)
			{
				playerProgressBar.UpdateUI();
			}
			if (guildProgressBar != null)
			{
				guildProgressBar.UpdateUI();
			}
			if (playerName != null)
			{
				playerName.UpdateUI();
			}
			if (guildName != null)
			{
				guildName.UpdateUI();
			}
			int missionCount = 0;
			int completedCount = 0;
			WeeklyChallengeHelper.CalculateTotalMissions(out completedCount, out missionCount);
			bool flag = completedCount > 0 && completedCount >= missionCount;
			if (cycleButton != null)
			{
				cycleButton.SetContentToLabelOne(completedCount + "/" + missionCount);
				cycleButton.defaultColor = (flag ? cycleButtonColorComplete : cycleButtonColorNormal);
			}
			bool flag2 = weeklyChallengeModel.IsDebufCycles();
			if (!isNormalChallenge)
			{
				Helpers.GameObjectSetActive(apocalypticEffect, value: true);
				Helpers.GameObjectSetActive(apocalypticInfoButton, value: true);
				ApocalypseWeeklyChallengeModel weeklyApocalypticChallengeModel2 = WeeklyChallengeHelper.GetWeeklyApocalypticChallengeModel();
				if (weeklyApocalypticChallengeModel2 != null && weeklyApocalypticChallengeModel2.CurrentCycle > 50)
				{
					Helpers.GameObjectSetActive(apocalypticBG, value: true);
				}
				Helpers.GameObjectSetActive(weeklyChallengeRewardIcon, weeklyApocalypticChallengeModel.IsShowApocalypticMode90RoundRewards);
			}
			else if (flag2 && plightListPanel != null)
			{
				Helpers.GameObjectSetActive(plightButton, value: true);
				plightListPanel.Init(weeklyChallengeModel.GetChallengeDebuffs());
			}
			HelpersUI.SetSprite(skipTokenCountIcon, isNormalChallenge ? "Ui_Icon_Round_Pass" : "Ui_Icon_Round_Pass_Apocalyptic");
			HelpersUI.SetSprite(starSprite, isNormalChallenge ? "Ui_Mission_Star_Large" : "Ui_Mission_Star_Large_Apocalyptic");
			Helpers.GameObjectSetActive(cycleCompleteEffect, flag);
			Helpers.GameObjectSetActive(challengeCountdownParent, value: false);
			Helpers.GameObjectSetActive(cycleParent, isNormalChallenge ? (!weeklyChallengeModel.HasCompletedMaxCycles()) : (!weeklyApocalypticChallengeModel.HasCompleteMaxRound()));
			Helpers.GameObjectSetActive(progressContentParent, value: true);
			Helpers.GameObjectSetActive(noGuildParent, !GameManager.Instance.playerModel.IsGuildMember && isNormalChallenge);
			Helpers.GameObjectSetActive(challengeCompletedMessage, isNormalChallenge ? weeklyChallengeModel.HasCompletedMaxCycles() : weeklyApocalypticChallengeModel.HasCompleteMaxRound());
			Helpers.GameObjectSetActive(apocalypticChallengeButton, isNormalChallenge && WeeklyChallengeHelper.IsApocalypticUnlocked);
			bool flag3 = GameManager.Instance.playerModel.WeeklyChallengeClassTeamActivity.IsActive && !isNormalChallenge;
			Helpers.GameObjectSetActive(weeklyChallengeActivityObj, flag3);
			if (flag3)
			{
				ClassTeamDefinition currentDefinition = GameManager.Instance.playerModel.WeeklyChallengeClassTeamActivity.CurrentDefinition;
				weeklyChallengeActivityIcon.spriteName = HelpersGfx.GetCurrencyIconName(SurvivorToken.GetClassAsCurrency(GameManager.Instance.playerModel.WeeklyChallengeClassTeamActivity.CurrentDefinition.GetClasses()[0]));
				Object obj = UnityUtils.LoadFromAssetBundle(currentDefinition.Pic_Banner, "itemgraphics");
				if (obj != null)
				{
					weeklyChallengeTexture.mainTexture = (Texture)obj;
				}
				string survivorClassName = HelpersLocalization.GetSurvivorClassName(currentDefinition.GetClasses()[0]);
				HelpersUI.SetContentToLabel(weeklyChallengeActivityLabel, LocalizationManager.GetText("WeeklyChallengeClassTeamChallenge.BannerDesc", survivorClassName));
			}
		}
		else
		{
			nextChallenge = WeeklyChallengeHelper.GetNextChallenge();
			Helpers.GameObjectSetActive(playerProgressBar, value: false);
			Helpers.GameObjectSetActive(guildProgressBar, value: false);
			Helpers.GameObjectSetActive(playerName, value: false);
			Helpers.GameObjectSetActive(guildName, value: false);
			Helpers.GameObjectSetActive(noGuildParent, value: false);
			Helpers.GameObjectSetActive(challengeCountdownParent, value: true);
			Helpers.GameObjectSetActive(cycleParent, value: false);
			Helpers.GameObjectSetActive(progressContentParent, value: false);
			Helpers.GameObjectSetActive(challengeCompletedMessage, value: false);
			Helpers.GameObjectSetActive(apocalypticChallengeButton, value: false);
			Helpers.GameObjectSetActive(weeklyChallengeActivityObj, value: false);
		}
		UpdateProgressBars();
		UpdateTitleParts();
		UpdateTimeleftUI();
		CampHUD campHUD = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CampCampMapHud) as CampHUD;
		if (campHUD != null)
		{
			campHUD.ShowChallengeLock(!WeeklyChallengeHelper.IsApocalypticUnlocked);
		}
	}

	public void UpdateProgressBars()
	{
		if (!(toggleSet != null))
		{
			return;
		}
		if (WeeklyChallengeHelper.IsNormalChallenge)
		{
			bool flag = WeeklyChallengeHelper.IsChallengeOngoing();
			Helpers.GameObjectSetActive(personalProgressParent, flag);
			Helpers.GameObjectSetActive(guildProgressParent, flag && GameManager.Instance.playerModel.IsGuildMember);
			if (WeeklyChallengeHelper.GetWeeklyChallengeModel() != null)
			{
				if (flag)
				{
					HelpersUI.SetContentToLabel(playerStarCountLabel, WeeklyChallengeHelper.GetWeeklyChallengeModel().NumberStars.ToString());
					HelpersUI.SetContentToLabel(guildStarCountLabel, WeeklyChallengeHelper.GetWeeklyChallengeModel().NumberStarsGuild.ToString());
				}
				if (playerRewardsProgressBar != null && personalProgressParent != null && personalProgressParent.activeInHierarchy)
				{
					playerRewardsProgressBar.ShowProgressFromLastSeenToCurrent(personal: true, 2f);
					WeeklyChallengeHelper.MarkPersonalStarsAsSeen();
				}
				if (guildRewardsProgressBar != null && guildProgressParent != null && guildProgressParent.activeInHierarchy)
				{
					guildRewardsProgressBar.ShowProgressFromLastSeenToCurrent(personal: false, 2f);
					WeeklyChallengeHelper.MarkGuildStarsAsSeen();
				}
			}
			return;
		}
		bool flag2 = WeeklyChallengeHelper.IsChallengeOngoing();
		Helpers.GameObjectSetActive(personalProgressParent, flag2);
		Helpers.GameObjectSetActive(guildProgressParent, value: false);
		if (WeeklyChallengeHelper.GetWeeklyApocalypticChallengeModel() != null)
		{
			if (flag2)
			{
				HelpersUI.SetContentToLabel(playerStarCountLabel, WeeklyChallengeHelper.GetWeeklyApocalypticChallengeModel().NumberStars.ToString());
			}
			if (playerRewardsProgressBar != null && personalProgressParent != null && personalProgressParent.activeInHierarchy)
			{
				playerRewardsProgressBar.ShowApocalypticProgressFromLastSeenToCurrent(personal: true, 2f);
				WeeklyChallengeHelper.MarkPersonalStarsAsSeen();
			}
		}
	}

	public void UpdateTitleParts()
	{
		bool flag = WeeklyChallengeHelper.IsChallengeOngoing();
		Helpers.GameObjectSetActive(titleParent, value: true);
		Helpers.GameObjectSetActive(challengeNameButton, WeeklyChallengeHelper.IsNormalChallenge);
		if (challengeNameButton != null)
		{
			if (flag)
			{
				challengeNameButton.SetContentToLabelOne(WeeklyChallengeHelper.GetCurrentChallengeName());
			}
			else
			{
				challengeNameButton.SetContentToLabelOne(WeeklyChallengeHelper.GetNextChallengeName());
			}
		}
		if (flag)
		{
			difficultyProgressBar.UpdateUIAfterSeconds(0.5f);
			Helpers.GameObjectSetActive(difficultyProgressBar, value: true);
			Helpers.GameObjectSetActive(missionCostContainer, value: true);
			HelpersUI.SetContentToLabel(missionCostLabel, WeeklyChallengeHelper.GetGasCost().ToString());
			HelpersUI.SetSprite(missionCostSprite, HelpersGfx.GetCurrencyIconName(CurrencyType.ReplayToken));
		}
		else
		{
			Helpers.GameObjectSetActive(missionCostContainer, value: false);
			Helpers.GameObjectSetActive(difficultyProgressBar, value: false);
		}
		challengeGrid.Reposition();
	}

	public void AddListeners()
	{
		if (toggleSet != null)
		{
			toggleSet.SetChangeCallback(ToggleChangeCallback);
		}
		if (cycleButton != null)
		{
			cycleButton.SetClickCallback(OnClickCycleButton);
		}
		if (plightButton != null)
		{
			plightButton.SetClickCallback(OnClickPlightButton);
		}
		if (apocalypticInfoButton != null)
		{
			apocalypticInfoButton.SetClickCallback(OnClickApocalypticInfoButton);
		}
		if (weekIconButton != null)
		{
			weekIconButton.SetClickCallback(OnClickIcon);
		}
		if ((bool)challengeNameButton)
		{
			challengeNameButton.SetClickCallback(OnClickChallengeName);
		}
		UIEvent.OnUIEvent += OnUIEvent;
	}

	public void RemoveListeners()
	{
		if (toggleSet != null)
		{
			toggleSet.Clear();
		}
		if (cycleButton != null)
		{
			cycleButton.Clear();
		}
		if (plightButton != null)
		{
			plightButton.Clear();
		}
		if (apocalypticInfoButton != null)
		{
			apocalypticInfoButton.Clear();
		}
		if (weekIconButton != null)
		{
			weekIconButton.Clear();
		}
		if ((bool)challengeNameButton)
		{
			challengeNameButton.Clear();
		}
		UIEvent.OnUIEvent -= OnUIEvent;
	}

	private void UpdateTimeleftUI()
	{
		if (WeeklyChallengeHelper.IsChallengeOngoing())
		{
			string text = LocalizationManager.GetText("Map.WeeklyChallenge.EndsIn{Time}", WeeklyChallengeHelper.GetFormatedTimeLeftToCurrentChallengeEnd());
			HelpersUI.SetContentToLabel(challengeTimeLeftLabel, text);
		}
		else
		{
			Helpers.GameObjectSetActive(challengeTimeLeftLabel, value: false);
		}
	}

	private void ToggleChangeCallback(UIButtonExtended clickedToggle)
	{
	}

	private void OnClickGuild()
	{
		CampHUD.OpenGuildOrChallenge(UIType.SocialPopupGuild);
	}

	private void OnClickCycleButton(UIButtonExtended button)
	{
		SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.WeeklyChallengeNextCycle).Open();
	}

	private void OnClickPlightButton(UIButtonExtended button)
	{
		SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.PlightIntroductionPopup).Open();
	}

	private void OnClickApocalypticInfoButton(UIButtonExtended button)
	{
		SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.ConsumablesPlightCombatPopup).Open();
	}

	private void OnClickApocalypticButton(UIButtonExtended button)
	{
		SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.ConsumablesPlightCombatPopup).Open();
	}

	private void OnClickChallengeName(UIButtonExtended button)
	{
		if (WeeklyChallengeHelper.IsNormalChallenge)
		{
			WeeklyChallengeModel weeklyChallengeModel = WeeklyChallengeHelper.GetWeeklyChallengeModel();
			if (weeklyChallengeModel != null && !weeklyChallengeModel.Finished)
			{
				WeeklyChallengeStartOngoingPopup.TryOpenFromClick();
			}
			else
			{
				WeeklyChallengeEndPopup.TryOpenWithWeeklyModel(weeklyChallengeModel);
			}
		}
	}

	private void OnUIEvent(string type, object parameter = null)
	{
		if (type == "OnPopUpClose" && SingularityMonoBehaviour<HUDManager>.Instance.GetNoCreation(UIType.DetailMapPopUp).IsOpen && !SingularityMonoBehaviour<HUDManager>.Instance.GetNoCreation(UIType.DetailMapPopUp).IsClosing)
		{
			UpdateUI();
		}
		else if (type == "SocialGuildPlayerChanged" || type == "SocialMembershipAccepted")
		{
			UpdateUI();
		}
	}

	public void OnClickEnterApocalypticChallengeButton()
	{
		if (!GameManager.Instance.gameEconomyData.ConfigData.ApocalypticChallengeSwitch)
		{
			HUDNotification.Info(LocalizationManager.GetText("Tips.ChallengeMode.SwitchOff"));
			return;
		}
		MissionHubNavigation.TryOpenApocalypticChallengeMap();
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_click");
	}

	private void OnClickIcon(UIButtonExtended button)
	{
		if (button != null && button.gameObject != null)
		{
			TooltipManager.OpenTextBoxWithText(weeklyChallengeRewardIcon, LocalizationManager.GetText("Popup.ApocalypticWeeklyChallenge.NormalReward.Info"));
		}
	}

	public void TipsButtonClicked()
	{
		WeeklyChallengeActivityInfo weeklyChallengeActivityInfo = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.WeeklyChallengeActivityInfo) as WeeklyChallengeActivityInfo;
		if (weeklyChallengeActivityInfo != null)
		{
			weeklyChallengeActivityInfo.Open();
		}
	}

	public void JumpButtonClicked()
	{
		DeepLinkNavigation.HandleDeepLink("CAMP");
		ActivityPopup activityPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.ActivityPopup) as ActivityPopup;
		if (activityPopup != null)
		{
			activityPopup.Open();
			UIEvent.Send("ActivityClickEvent", GameManager.Instance.playerModel.WeeklyChallengeClassTeamActivity);
			List<IActivityManagerIntegrationInterface> list = GameManager.Instance.playerModel?.ActivityIntegrationManager?.GetIntegrationActivityList();
			if (list != null)
			{
				int index = list.IndexOf(GameManager.Instance.playerModel.WeeklyChallengeClassTeamActivity);
				activityPopup.ScrollToIndex(index);
			}
		}
	}
}
