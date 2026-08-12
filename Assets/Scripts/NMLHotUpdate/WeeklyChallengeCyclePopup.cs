using System.Collections;
using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class WeeklyChallengeCyclePopup : HUDElement
{
	[Header("State Containers")]
	[SerializeField]
	private GameObject incompleteCycleContainer;

	[SerializeField]
	private GameObject completeCycleContainer;

	[SerializeField]
	private GameObject rewardContainer;

	[SerializeField]
	private GameObject masterMissionContainer;

	[Header("Shared Content")]
	[SerializeField]
	private UILabel titleLabel;

	[SerializeField]
	private GameObject cycleTimerContainer;

	[SerializeField]
	private UILabel cycleTimerLabel;

	[Header("Rewards Content")]
	[SerializeField]
	private GameObject starMultiplierIcon;

	[SerializeField]
	private UILabel rewardStarsLabel;

	[SerializeField]
	private UILabel rewardLabel;

	[SerializeField]
	private UISprite rewardIcon;

	[SerializeField]
	private UISprite skipTokenRewardIcon;

	[SerializeField]
	private UILabel skipTokenLabel;

	[SerializeField]
	private GameObject[] rewardMultiplierItems;

	[Header("Incomplete Round Screen")]
	[SerializeField]
	private UILabel completedMissionsLabel;

	[SerializeField]
	private UILabel messageLabel;

	[Header("Round Complete Screen")]
	[SerializeField]
	private UILabel collectedStarsLabel;

	[SerializeField]
	private UIButton nextCycleButton;

	[SerializeField]
	private GameObject finalRoundCompletedText;

	[SerializeField]
	private LocalizationUIUpdater nextRoundButtonText;

	[SerializeField]
	private LocalizationUIUpdater nextRoundMessageLabel;

	[Header("Next Round Screen")]
	[SerializeField]
	private GameObject newParent;

	[SerializeField]
	private UIChallengeDifficultyProgressBar difficultyProgress;

	[Header("Master Mission")]
	[SerializeField]
	private UILabel missionsSkippedLabel;

	[SerializeField]
	private UILabel starsGainedLabel;

	[SerializeField]
	private UILabel extraStarsGainedLabel;

	[SerializeField]
	private UILabel totalStarsGainedLabel;

	[SerializeField]
	private GameObject starBonus;

	[SerializeField]
	private GameObject[] masterMissionChallengeStars;

	[SerializeField]
	private GameObject[] masterMissionChallengeStarsContainers;

	[SerializeField]
	private GameObject featuredHeroStarLabelContainer;

	private bool nextCyclePossible;

	private bool isLockedByTimer;

	private bool firstOpenAfterNewCycle;

	private bool masterMissionCompleted;

	private Coroutine startNextCycleRef;

	public override void Open()
	{
		base.Open();
		firstOpenAfterNewCycle = false;
		masterMissionCompleted = WeeklyChallengeHelper.WasLastCompletedMissionTheMasterMission();
		UpdateUI();
	}

	private void OnEnable()
	{
		UIEvent.OnUIEvent += OnUIEvent;
	}

	private void OnDisable()
	{
		UIEvent.OnUIEvent -= OnUIEvent;
	}

	public override void OnClickClose()
	{
		base.OnClickClose();
		if (newParent != null && newParent.activeSelf)
		{
			TriggerNewCycleAnimations();
		}
	}

	public void OnClickSkipToken()
	{
		if (WeeklyChallengeHelper.IsNormalChallenge)
		{
			WeeklyChallengeModel weeklyChallengeModel = WeeklyChallengeHelper.GetWeeklyChallengeModel();
			if (weeklyChallengeModel != null && weeklyChallengeModel.CurrentDefinition != null)
			{
				TooltipManager.OpenTextBoxWithText(skipTokenRewardIcon.gameObject, LocalizationManager.GetText("Map.WeeklyChallenge.RoundPassInfo{RoundsToGetPass}{RoundsLeftToGetPass}", weeklyChallengeModel.GetCurrentCycleRoundsToSkipToken(), weeklyChallengeModel.CalculateRoundsToNextSkipToken()));
			}
			else
			{
				TooltipManager.OpenTextBoxWithText(skipTokenRewardIcon.gameObject, LocalizationManager.GetText("Map.WeeklyChallenge.RoundPassInfoGeneral"));
			}
		}
		else
		{
			ApocalypseWeeklyChallengeModel weeklyApocalypticChallengeModel = WeeklyChallengeHelper.GetWeeklyApocalypticChallengeModel();
			if (weeklyApocalypticChallengeModel != null && weeklyApocalypticChallengeModel.CurrentDefinition != null)
			{
				TooltipManager.OpenTextBoxWithText(skipTokenRewardIcon.gameObject, LocalizationManager.GetText("Map.ApocalypticWeeklyChallenge.RoundPassInfo"));
			}
			else
			{
				TooltipManager.OpenTextBoxWithText(skipTokenRewardIcon.gameObject, LocalizationManager.GetText("Map.WeeklyChallenge.RoundPassInfoGeneral"));
			}
		}
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
		WeeklyChallengeModel weeklyChallengeModel = WeeklyChallengeHelper.GetWeeklyChallengeModel();
		ApocalypseWeeklyChallengeModel weeklyApocalypticChallengeModel = WeeklyChallengeHelper.GetWeeklyApocalypticChallengeModel();
		bool flag = WeeklyChallengeHelper.IsCurrentRoundFinal();
		nextCyclePossible = WeeklyChallengeHelper.CanAccessNextCycle();
		if (weeklyChallengeModel != null)
		{
			isLockedByTimer = weeklyChallengeModel.IsNewCycleLockedByTimer();
		}
		Helpers.GameObjectSetActive(newParent, value: false);
		Helpers.GameObjectSetActive(completeCycleContainer, nextCyclePossible && !masterMissionCompleted);
		Helpers.GameObjectSetActive(masterMissionContainer, masterMissionCompleted);
		nextCycleButton.isEnabled = nextCyclePossible;
		Helpers.GameObjectSetActive(incompleteCycleContainer, !nextCyclePossible);
		Helpers.GameObjectSetActive(messageLabel, !WeeklyChallengeHelper.HasCompletedMissions());
		Helpers.GameObjectSetActive(rewardContainer, !masterMissionCompleted);
		Helpers.GameObjectSetActive(finalRoundCompletedText, flag);
		WeeklyChallengeReward currentCycleCompletionReward = WeeklyChallengeHelper.GetCurrentCycleCompletionReward();
		if (nextCyclePossible && !masterMissionCompleted)
		{
			TweenManager.PlayTweenGroup(completeCycleContainer, 20);
		}
		if (rewardStarsLabel != null)
		{
			if (currentCycleCompletionReward != null)
			{
				if (WeeklyChallengeHelper.IsNormalChallenge)
				{
					rewardStarsLabel.text = weeklyChallengeModel?.DetermineFinalRewardStarAmount(weeklyChallengeModel.GetBonusStarsAtCurrentCycleCompletion()).ToString();
				}
				else
				{
					rewardStarsLabel.text = weeklyApocalypticChallengeModel.GetApocalypticRoundStars.ToString() ?? "";
				}
			}
			rewardStarsLabel.gameObject.SetActive(currentCycleCompletionReward != null);
		}
		if (currentCycleCompletionReward?.RewardEntries?.RewardsList != null && currentCycleCompletionReward.RewardEntries.RewardsList.Count > 0 && rewardIcon != null && rewardLabel != null)
		{
			IReward reward = currentCycleCompletionReward.RewardEntries.RewardsList[0];
			if (!(reward is RewardCurrency rewardCurrency))
			{
				if (!(reward is RewardSkipChallange rewardSkipChallange))
				{
					if (reward is RewardEquipment rewardEquipment && rewardEquipment.IsConsumableReward(GameManager.Instance.modelManager))
					{
						EquipmentModel.ConsumableType consumableType = ConsumableUtils.IdToConsumableType(rewardEquipment.EquipmentDefinition(GameManager.Instance.modelManager).ID);
						rewardIcon.spriteName = HelpersGfx.GetConsumableIconName(consumableType);
						rewardLabel.text = rewardEquipment.Amount.ToString();
					}
					else
					{
						rewardIcon.gameObject.SetActive(value: false);
						rewardLabel.gameObject.SetActive(value: false);
					}
				}
				else
				{
					HelpersGfx.GetIconNameForIReward(rewardSkipChallange, out var spriteName, null, null, null, GameManager.Instance.playerModel);
					rewardIcon.spriteName = spriteName;
					rewardLabel.text = (WeeklyChallengeHelper.IsNormalChallenge ? weeklyChallengeModel.DetermineFinalRewardCurrencyAmount(rewardSkipChallange).ToString() : rewardSkipChallange.Amount.ToString());
				}
			}
			else
			{
				rewardIcon.spriteName = HelpersGfx.GetCurrencyIconName(rewardCurrency.CurrencyType);
				rewardLabel.text = (WeeklyChallengeHelper.IsNormalChallenge ? weeklyChallengeModel.DetermineFinalRewardCurrencyAmount(rewardCurrency).ToString() : rewardCurrency.Amount.ToString());
			}
		}
		if (titleLabel != null)
		{
			if (masterMissionCompleted)
			{
				titleLabel.text = LocalizationManager.GetText("Popup.WeeklyChallengeCycle.RoundSkipCompleted.Title");
			}
			else if (nextCyclePossible)
			{
				if (flag)
				{
					nextRoundButtonText.LocalizationKey = "Popup.WeeklyChallengeCycle.Button.FinishChallenge";
					nextRoundButtonText.UpdateContent();
					nextRoundMessageLabel.LocalizationKey = "Popup.WeeklyChallengeCycle.ButtonInfo.FinalRound";
					nextRoundMessageLabel.UpdateContent();
					titleLabel.text = LocalizationManager.GetText("Popup.WeeklyChallengeCycle.Title.FinalRoundCompleted");
				}
				else
				{
					titleLabel.text = LocalizationManager.GetText("Popup.WeeklyChallengeCycle.Title.RoundCompleted");
				}
			}
			else if (isLockedByTimer && WeeklyChallengeHelper.HasCompletedMissions())
			{
				titleLabel.text = LocalizationManager.GetText("Popup.Quest.MissionsCompleted");
			}
			else
			{
				titleLabel.text = LocalizationManager.GetText("Popup.WeeklyChallengeCycle.Title.RoundNotCompleted");
			}
		}
		if (completedMissionsLabel != null && incompleteCycleContainer.activeInHierarchy)
		{
			int completedCount = 0;
			int missionCount = 0;
			WeeklyChallengeHelper.CalculateTotalMissions(out completedCount, out missionCount);
			completedMissionsLabel.text = completedCount + "/" + missionCount;
		}
		if (collectedStarsLabel != null && completeCycleContainer.activeInHierarchy)
		{
			collectedStarsLabel.text = GetFormattedChallengeTotalStarsGainedAmount();
		}
		int num = (WeeklyChallengeHelper.IsNormalChallenge ? weeklyChallengeModel.DetermineReceivedSkipTokenCount(weeklyChallengeModel.CurrentCycle) : 0);
		HelpersUI.SetContentToLabel(skipTokenLabel, num.ToString(), num > 0);
		Helpers.GameObjectSetActive(skipTokenRewardIcon.gameObject, num > 0);
		if (rewardMultiplierItems != null)
		{
			GameObject[] array = rewardMultiplierItems;
			for (int i = 0; i < array.Length; i++)
			{
				Helpers.GameObjectSetActive(array[i], weeklyChallengeModel.DoubleRewardsActive && WeeklyChallengeHelper.IsNormalChallenge);
			}
		}
		if (!masterMissionCompleted && weeklyChallengeModel.DoubleRewardsActive)
		{
			starMultiplierIcon.SetActive(WeeklyChallengeHelper.IsNormalChallenge);
		}
		if (!masterMissionCompleted)
		{
			return;
		}
		MapMissionGroupModel mapMissionGroupModel = (WeeklyChallengeHelper.IsNormalChallenge ? weeklyChallengeModel.GetMapMissionGroupModel() : weeklyApocalypticChallengeModel.GetMapMissionGroupModel());
		if (mapMissionGroupModel != null)
		{
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			int num5 = (WeeklyChallengeHelper.FeaturedStarHeroActive ? masterMissionChallengeStars.Length : 3);
			for (int j = 0; j < mapMissionGroupModel.Missions.Count; j++)
			{
				if (mapMissionGroupModel.Missions[j].IsMasterMission)
				{
					for (int k = 0; k < num5; k++)
					{
						if (masterMissionChallengeStars[k] != null && masterMissionChallengeStarsContainers[k] != null && k < mapMissionGroupModel.Missions[j].Stars.NumberStars)
						{
							masterMissionChallengeStarsContainers[k].SetActive(value: true);
							masterMissionChallengeStars[k].SetActive(value: true);
						}
					}
					continue;
				}
				if (mapMissionGroupModel.Missions[j].CompletedFromMasterMission)
				{
					num2++;
				}
				if (mapMissionGroupModel.Missions[j].FeaturedHeroExtraChallengeStarFromMasterMission)
				{
					num4 += ((!weeklyChallengeModel.DoubleRewardsActive || !WeeklyChallengeHelper.IsNormalChallenge) ? 1 : 2);
				}
				num3 += mapMissionGroupModel.Missions[j].StarsFromMasterMission * ((!weeklyChallengeModel.DoubleRewardsActive || !WeeklyChallengeHelper.IsNormalChallenge) ? 1 : 2);
				mapMissionGroupModel.Missions[j].CompletedFromMasterMission = false;
				mapMissionGroupModel.Missions[j].FeaturedHeroExtraChallengeStarFromMasterMission = false;
				mapMissionGroupModel.Missions[j].StarsFromMasterMission = 0;
			}
			HelpersUI.SetContentToLabel(missionsSkippedLabel, num2.ToString());
			HelpersUI.SetContentToLabel(starsGainedLabel, num3.ToString());
			HelpersUI.SetContentToLabel(totalStarsGainedLabel, GetFormattedChallengeTotalStarsGainedAmount());
			Helpers.GameObjectSetActive(featuredHeroStarLabelContainer, WeeklyChallengeHelper.FeaturedStarHeroActive);
			if (WeeklyChallengeHelper.FeaturedStarHeroActive)
			{
				HelpersUI.SetContentToLabel(extraStarsGainedLabel, num4.ToString());
			}
		}
		Helpers.GameObjectSetActive(starBonus, weeklyChallengeModel.DoubleRewardsActive && WeeklyChallengeHelper.IsNormalChallenge);
		masterMissionCompleted = false;
	}

	public override void Update()
	{
		base.Update();
		WeeklyChallengeModel weeklyChallengeModel = WeeklyChallengeHelper.GetWeeklyChallengeModel();
		bool flag = weeklyChallengeModel.IsNewCycleLockedByTimer();
		if (cycleTimerContainer != null)
		{
			cycleTimerContainer.SetActive(flag && !firstOpenAfterNewCycle);
		}
		if (cycleTimerLabel != null && weeklyChallengeModel != null && flag && !firstOpenAfterNewCycle)
		{
			string text = LocalizationManager.GetText("Popup.WeeklyChallenge.CycleTimer.Title{FormattedTime}", WeeklyChallengeHelper.GetFormatedTimeLeftToUnlockNextCycle());
			HelpersUI.SetContentToLabel(cycleTimerLabel, text);
		}
	}

	public void OnClickNextCycle()
	{
		if (nextCyclePossible)
		{
			nextCycleButton.isEnabled = false;
			WeeklyChallengeModel weeklyChallengeModel = WeeklyChallengeHelper.GetWeeklyChallengeModel();
			int num = (WeeklyChallengeHelper.IsNormalChallenge ? weeklyChallengeModel.DetermineReceivedSkipTokenCount(weeklyChallengeModel.CurrentCycle) : 0);
			if (num > 0)
			{
				(SingularityMonoBehaviour<HUDManager>.Instance.GetNoCreation(UIType.DetailMapPopUp) as DetailMapPopUp).GetChallengeInfoPanel().PlaySkipTokenCollectionAnimation(num, skipTokenRewardIcon.transform, base.gameObject);
				if (startNextCycleRef == null)
				{
					startNextCycleRef = StartCoroutine(DelayedCycleStart(1f));
				}
			}
			else if (startNextCycleRef == null)
			{
				startNextCycleRef = StartCoroutine(DelayedCycleStart(0f));
			}
		}
		else
		{
			HUDNotification.Info(LocalizationManager.GetText("First complete all missions with at least 1 star"));
		}
	}

	private IEnumerator DelayedCycleStart(float waitSeconds)
	{
		if (waitSeconds > 0f)
		{
			yield return new WaitForSeconds(waitSeconds);
		}
		if (WeeklyChallengeHelper.IsNormalChallenge)
		{
			Helpers.ExecuteCommand(new StartChallengeCycleCommand());
		}
		else
		{
			Helpers.ExecuteCommand(new StartApocalypseChallengeCycleCommand(isNextCircle: true));
		}
		if (WeeklyChallengeHelper.HasCompletedTheFinalRound())
		{
			OnClickOk();
			yield break;
		}
		UpdateUI();
		firstOpenAfterNewCycle = true;
		Helpers.GameObjectSetActive(completeCycleContainer, value: false);
		Helpers.GameObjectSetActive(incompleteCycleContainer, value: false);
		Helpers.GameObjectSetActive(rewardContainer, value: false);
		Helpers.GameObjectSetActive(newParent, value: true);
		if (difficultyProgress != null)
		{
			difficultyProgress.UpdateUIAfterSeconds(0.3f);
		}
		if (titleLabel != null)
		{
			titleLabel.text = LocalizationManager.GetText("Popup.WeeklyChallengeCycle.Title.NewDifficultyUnlocked");
		}
		if (completedMissionsLabel != null)
		{
			completedMissionsLabel.text = "";
		}
		startNextCycleRef = null;
	}

	public void OnClickOk()
	{
		Close();
		TriggerNewCycleAnimations();
	}

	private void TriggerNewCycleAnimations()
	{
		DetailMapPopUp.ReloadChallengeMap();
		DetailMapPopUp detailMapPopUp = SingularityMonoBehaviour<HUDManager>.Instance.GetNoCreation(UIType.DetailMapPopUp) as DetailMapPopUp;
		if (detailMapPopUp != null)
		{
			if (WeeklyChallengeHelper.IsNormalChallenge)
			{
				detailMapPopUp.LoadChallengeMap();
			}
			else
			{
				detailMapPopUp.LoadApocalypticChallengeMap();
			}
			List<MissionIcon> missionIcons = detailMapPopUp.GetCurrentMissionView().GetMissionIcons();
			for (int i = 0; i < (missionIcons?.Count ?? 0); i++)
			{
				missionIcons[i].PlayNewDifficultyAnimation();
			}
		}
	}

	private string GetFormattedChallengeTotalStarsGainedAmount()
	{
		WeeklyChallengeHelper.CalculateChallengeStars(out var collectedStars, out var maxStars);
		if (WeeklyChallengeHelper.IsNormalChallenge ? (WeeklyChallengeHelper.GetWeeklyChallengeModel().ActiveSkipTokens > 0) : (WeeklyChallengeHelper.GetWeeklyApocalypticChallengeModel().ActiveSkipTokens > 0))
		{
			collectedStars *= 2;
			maxStars *= 2;
		}
		return collectedStars + " / " + maxStars;
	}

	private void OnUIEvent(string type, object parameter)
	{
		if (type == "OnPopUpOpen" && parameter is MissionStartPopup)
		{
			Close();
		}
	}

	public void OnclickRewardIcon()
	{
		WeeklyChallengeReward currentCycleCompletionReward = WeeklyChallengeHelper.GetCurrentCycleCompletionReward();
		if (currentCycleCompletionReward?.RewardEntries?.RewardsList == null || currentCycleCompletionReward.RewardEntries.RewardsList.Count <= 0)
		{
			return;
		}
		RewardCurrency rewardCurrency = null;
		if (currentCycleCompletionReward.RewardEntries.RewardsList[0] is RewardCurrency)
		{
			rewardCurrency = currentCycleCompletionReward.RewardEntries.RewardsList[0] as RewardCurrency;
		}
		if (rewardCurrency != null && GameManager.Instance.gameEconomyData.IsSpeedUpTokenCurrencyType(rewardCurrency.CurrencyType))
		{
			PlayerModel playerModel = GameManager.Instance.playerModel;
			int currencyAmount = playerModel.GetCurrencyAmount(rewardCurrency.CurrencyType);
			int max = playerModel.GetCurrency(rewardCurrency.CurrencyType).Max;
			int amount = currencyAmount + rewardCurrency.Amount - max;
			int num = GameManager.Instance.modelManager.GameEconomyData.CurrencyToDiamonds(rewardCurrency.CurrencyType, amount, GameManager.Instance.modelManager.Player);
			if (num > 0)
			{
				TooltipManager.OpenForChallengeReward_sp_cy(rewardIcon.gameObject, rewardCurrency, num);
			}
		}
	}
}
