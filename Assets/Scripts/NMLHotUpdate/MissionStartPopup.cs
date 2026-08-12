using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using BaseModel;
using TWDModel;
using UnityEngine;

public class MissionStartPopup : HUDElement
{
	[Header("Mission Info")]
	[SerializeField]
	private GameObject titleContainer;

	[SerializeField]
	private UILabel missionTitle;

	[SerializeField]
	private UILabel missionLabelName;

	[Header("Portrait Info")]
	[SerializeField]
	private UISprite portraitSprite;

	[SerializeField]
	private UITexture portraitTexture;

	[SerializeField]
	private UILabel portraitLabel;

	[Header("Mission Reward ")]
	[SerializeField]
	private GameObject rewardParent;

	[SerializeField]
	private UILabel rewardLabel;

	[SerializeField]
	private UISprite rewardIcon;

	[SerializeField]
	private UILabel rewardLabelAmount;

	[SerializeField]
	private GameObject missionRewardEquipment;

	[SerializeField]
	private GameObject missionRewardCurrency;

	[SerializeField]
	private EpisodeRewardListPanel episodeRewardListPanel;

	[SerializeField]
	private SpeedUpTitle speedUpTitle;

	[SerializeField]
	private GameObject episodeScrollBar;

	[SerializeField]
	private UILabel missionRewardCurrencyLabel;

	[SerializeField]
	private UISprite missionRewardCurrencyIcon;

	[SerializeField]
	private UILabel missionRewardCurrencyLabel1_2;

	[SerializeField]
	private UISprite missionRewardCurrencyIcon1_2;

	[SerializeField]
	private UILabel missionRewardCurrencyLabel2_2;

	[SerializeField]
	private UISprite missionRewardCurrencyIcon2_2;

	[SerializeField]
	private UITable[] rewardTables;

	[SerializeField]
	private GameObject missionRewardDoubleXpContainer;

	[SerializeField]
	private GameObject survivalMissionDoubleRewardBoosterButton;

	[SerializeField]
	private GameObject survivalMissionDoubleRewardContainer;

	[SerializeField]
	private GameObject missionRewardSurvivorContainer;

	[SerializeField]
	private UISprite missionRewardSurvivorClassIcon;

	[SerializeField]
	private UITexture consumableRewardTexture;

	[SerializeField]
	private UILabel consumableRewardAmount;

	[SerializeField]
	private GameObject missionRewardSpecificEquipmentContainer;

	[SerializeField]
	private GameObject missionRewardSpecificEquipmentIconContainer;

	[SerializeField]
	private GameObject missionRewardConsumableEquipmentContainer;

	[SerializeField]
	private GameObject missionRewardNoneContainer;

	[SerializeField]
	private GameObject equipmentCardPrefab;

	[SerializeField]
	private GameObject randomEquipmentCardPrefab;

	[SerializeField]
	private GameObject challengeRewardContainer;

	[SerializeField]
	private GameObject seasonFeatureRewardContainer;

	[SerializeField]
	private UILabel seasonFeatureRewardDescription;

	[SerializeField]
	private UILabel seasonFeatureRewardProgress;

	[SerializeField]
	private UILabel seasonFeatureRewardTime;

	[SerializeField]
	private UISprite seasonFeatureRewardIcon;

	[SerializeField]
	private AnimateNumberFromTo mainCurrencyDoubleRewardAnimator;

	[SerializeField]
	private AnimateNumberFromTo secondaryCurrencyDoubleRewardAnimator;

	[SerializeField]
	private UILabel[] challengeRewardLabels;

	[SerializeField]
	private GameObject[] challengeRewardStars;

	[Header("Select Team")]
	[SerializeField]
	private GameObject messageParent;

	[SerializeField]
	private UILabel labelMessage;

	[SerializeField]
	private UIButton selectTeamButton;

	[SerializeField]
	private UILabel selectTeamButtonLabel;

	[SerializeField]
	private GameObject missionLockedInfo;

	[SerializeField]
	private GameObject selectTeamContainer;

	[SerializeField]
	private GameObject notEnoughSurvivorsContainer;

	[SerializeField]
	private GameObject challengeExtraStarContainer;

	[SerializeField]
	private GameObject challengeFeaturedHeroCallButton;

	[SerializeField]
	private GameObject challengeStarObj;

	[SerializeField]
	private GameObject apocalypticChallengeStarObj;

	private MapMissionModel currentMissionModel;

	private Rewards rewards;

	private RewardCurrency currencyMissionReward;

	private RewardCurrency currencyMissionReward2;

	private RewardEquipment equipmentMissionReward;

	private RewardRandomEquipment randomEquipmentMissionReward;

	private long seasonFeatureRemaining;

	private string portraitActorId;

	public override void Open()
	{
		base.Open();
		TweenManager.PlayTweenGroup(base.gameObject, 1, forward: true, AnimationOver);
		TutorialView.Instance.UpdateSuggestion();
	}

	private void AnimationOver()
	{
	}

	private void OnEnable()
	{
		UIEvent.OnUIEvent += OnUIEvent;
		foreach (SurvivorModel survivor in GameManager.Instance.playerModel.SurvivorContainer.Survivors)
		{
			survivor.Changed += OnSurvivorChanged;
		}
	}

	private void OnDisable()
	{
		UIEvent.OnUIEvent -= OnUIEvent;
		foreach (SurvivorModel survivor in GameManager.Instance.playerModel.SurvivorContainer.Survivors)
		{
			survivor.Changed -= OnSurvivorChanged;
		}
	}

	private string GetFormattedCurrencyReward(CurrencyType type, int amount)
	{
		if (ComponentHelper.IsComponentCurrency(type))
		{
			return HelpersLocalization.GetComponentRewardName(type, amount);
		}
		return amount.ToString();
	}

	public override void UpdateUI()
	{
		if (currentMissionModel == null)
		{
			return;
		}
		MapMissionModel mapMissionModel = currentMissionModel;
		if (mapMissionModel == null)
		{
			return;
		}
		MissionData missionData = mapMissionModel.MissionData;
		int num = (GameManager.Instance.playerModel.Blackboard.IsToggleOn("Toggle.OutpostGiftSurvivorsGiven") ? 6 : 3);
		num += mapMissionModel.MissionData.MaxTeamSize;
		bool flag = !mapMissionModel.IsDeadly || GameManager.Instance.playerModel.SurvivorContainer.Survivors.Count >= num;
		bool flag2 = mapMissionModel.IsInWeeklyChallenge || mapMissionModel.IsInApocalyptiWeeklyChallenge;
		bool isInWeeklySurvival = mapMissionModel.IsInWeeklySurvival;
		bool isGrindMission = mapMissionModel.IsGrindMission;
		bool isStoryMission = mapMissionModel.IsStoryMission;
		selectTeamButton.enabled = flag;
		if (isInWeeklySurvival)
		{
			selectTeamContainer.SetActive(!mapMissionModel.IsLocked);
			if (missionLockedInfo != null)
			{
				missionLockedInfo.SetActive(mapMissionModel.IsLocked);
			}
		}
		else
		{
			selectTeamContainer.SetActive(value: true);
		}
		if (notEnoughSurvivorsContainer != null)
		{
			notEnoughSurvivorsContainer.SetActive(!flag);
		}
		if (isInWeeklySurvival)
		{
			if (missionTitle != null)
			{
				missionTitle.text = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Map.Episode.Title.TheDistance");
			}
			if (missionLabelName != null)
			{
				SurvivalMissionConfig survivalConfig = mapMissionModel.SolveSurvivalConfigForCurrentMission();
				missionLabelName.text = HelpersLocalization.GetSurvivalMissionName(survivalConfig);
			}
		}
		else
		{
			if (missionTitle != null && !flag2 && !isGrindMission)
			{
				missionTitle.text = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Mission." + missionData.DisplayTextID + ".Mission");
			}
			if (missionLabelName != null)
			{
				missionLabelName.text = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Mission." + missionData.DisplayTextID + ".Title");
			}
		}
		if (labelMessage != null && messageParent != null)
		{
			labelMessage.text = LocalizationManager.GetText("Popup.TeamSelection.MissionLocked[MinSurvivorLevel]", mapMissionModel.RequiredSurvivorLevel);
			setActive(messageParent, labelMessage.text != "" && (mapMissionModel.MissionSpawnPointGroup.Category != MapCategory.Season || mapMissionModel.IsLastInGroup));
		}
		if (rewardIcon != null && rewardLabel != null && rewardLabelAmount != null)
		{
			setActive(rewardParent, setActive: false);
		}
		else
		{
			setActive(rewardParent, setActive: false);
		}
		if (selectTeamButtonLabel != null)
		{
			if (missionData.ExtraData != null && missionData.ExtraData.InUse && missionData.ExtraData.PlayableSurvivors != null && missionData.ExtraData.PlayableSurvivors.Count > 0)
			{
				selectTeamButtonLabel.text = LocalizationManager.GetText("Popup.PartialCombatTeam.Button.StartAttack");
			}
			else
			{
				selectTeamButtonLabel.text = LocalizationManager.GetText("Popup.TeamSelection.TeamSelect");
			}
		}
		if (isInWeeklySurvival)
		{
			SurvivalMissionConfig survivalMissionConfig = mapMissionModel.SolveSurvivalConfigForCurrentMission();
			if (survivalMissionConfig != null && !string.IsNullOrEmpty(survivalMissionConfig.BriefingDisplayLocale))
			{
				SurvivalSavedMissionModel saveData = null;
				if (GameManager.Instance.playerModel.WeeklySurvival != null && GameManager.Instance.playerModel.WeeklySurvival.NextMissionOrderNumber == mapMissionModel.SolveOrderNumberInGroup())
				{
					saveData = GameManager.Instance.playerModel.SavedSurvivalMissionData;
				}
				string survivalMissionBriefing = HelpersLocalization.GetSurvivalMissionBriefing(survivalMissionConfig, saveData);
				setQuestPortraitAndTitle("Map.Episode.Briefing.Survival." + survivalMissionConfig.BriefingDisplayLocale, missionData.DisplayTextID, survivalMissionBriefing);
			}
			else
			{
				setQuestPortraitAndTitle("Map.Episode.Briefing.Survival.Unknown", missionData.DisplayTextID);
			}
		}
		else
		{
			setQuestPortraitAndTitle("Mission." + missionData.DisplayTextID + ".Briefing", missionData.DisplayTextID);
		}
		if (missionRewardEquipment != null)
		{
			missionRewardEquipment.SetActive(flag && mapMissionModel.LootTag == DropEventDefinition.DropEventTag.PreferEquipment);
		}
		titleContainer.SetActive(!flag2 && !isGrindMission);
		challengeRewardContainer.SetActive(flag2);
		if (flag2)
		{
			SetChallengeStarts(missionData);
		}
		Helpers.GameObjectSetActive(challengeExtraStarContainer, flag2 && WeeklyChallengeHelper.FeaturedStarHeroActive);
		Helpers.GameObjectSetActive(challengeStarObj, mapMissionModel.IsInWeeklyChallenge && WeeklyChallengeHelper.FeaturedStarHeroActive);
		Helpers.GameObjectSetActive(apocalypticChallengeStarObj, mapMissionModel.IsInApocalyptiWeeklyChallenge && WeeklyChallengeHelper.FeaturedStarHeroActive);
		Helpers.GameObjectSetActive(challengeFeaturedHeroCallButton, flag2 && WeeklyChallengeHelper.FeaturedStarHeroActive && !WeeklyChallengeHelper.HasUnLockedFeaturedHero());
		Helpers.GameObjectSetActive(survivalMissionDoubleRewardBoosterButton, currentMissionModel.IsInWeeklySurvival && GameManager.Instance.playerModel.WeeklySurvival.CanRestartMapOrDoubleRewards());
		rewardLabel.enabled = !flag2;
		if (missionRewardCurrency != null)
		{
			bool flag3 = mapMissionModel.LootTag == DropEventDefinition.DropEventTag.PreferSP || mapMissionModel.LootTag == DropEventDefinition.DropEventTag.PreferSupplies;
			bool flag4 = currencyMissionReward != null;
			bool flag5 = currencyMissionReward2 != null;
			missionRewardCurrency.SetActive(flag && (flag3 || flag4));
			if (flag3)
			{
				if (missionRewardCurrencyLabel != null)
				{
					missionRewardCurrencyLabel.enabled = true;
					DropCurrenciesStaticDefinition dropCurrencyStaticDefinition = mapMissionModel.manager.GameEconomyData.GetDropCurrencyStaticDefinition(mapMissionModel.LootTag, mapMissionModel.MissionLevel);
					dropCurrencyStaticDefinition = GameManager.Instance.playerModel.ActivityManager.ModifyActivityDefinition(dropCurrencyStaticDefinition);
					int minSupplies = dropCurrencyStaticDefinition.MinSupplies;
					int minSurvivalPoints = dropCurrencyStaticDefinition.MinSurvivalPoints;
					missionRewardCurrencyLabel.text = (minSupplies + minSurvivalPoints).ToString();
				}
				if (missionRewardCurrencyIcon != null)
				{
					missionRewardCurrencyIcon.enabled = true;
					missionRewardCurrencyIcon.spriteName = HelpersGfx.GetSpriteNameForLootType(mapMissionModel.LootTag);
				}
				if (missionRewardCurrencyLabel1_2 != null)
				{
					missionRewardCurrencyLabel1_2.enabled = false;
				}
				if (missionRewardCurrencyIcon1_2 != null)
				{
					missionRewardCurrencyIcon1_2.enabled = false;
				}
				if (missionRewardCurrencyLabel2_2 != null)
				{
					missionRewardCurrencyLabel2_2.enabled = false;
				}
				if (missionRewardCurrencyIcon2_2 != null)
				{
					missionRewardCurrencyIcon2_2.enabled = false;
				}
			}
			else if (flag4 && flag5)
			{
				if (missionRewardCurrencyLabel != null)
				{
					missionRewardCurrencyLabel.enabled = false;
				}
				if (missionRewardCurrencyIcon != null)
				{
					missionRewardCurrencyIcon.enabled = false;
				}
				if (missionRewardCurrencyLabel1_2 != null)
				{
					missionRewardCurrencyLabel1_2.enabled = true;
					missionRewardCurrencyLabel1_2.text = GetFormattedCurrencyReward(currencyMissionReward.CurrencyType, currencyMissionReward.Amount);
				}
				if (missionRewardCurrencyIcon1_2 != null)
				{
					missionRewardCurrencyIcon1_2.enabled = true;
					missionRewardCurrencyIcon1_2.spriteName = HelpersGfx.GetCurrencyIconName(currencyMissionReward.CurrencyType, GameManager.Instance.playerModel);
				}
				if (missionRewardCurrencyLabel2_2 != null)
				{
					missionRewardCurrencyLabel2_2.enabled = true;
					missionRewardCurrencyLabel2_2.text = GetFormattedCurrencyReward(currencyMissionReward2.CurrencyType, currencyMissionReward2.Amount);
				}
				if (missionRewardCurrencyIcon2_2 != null)
				{
					missionRewardCurrencyIcon2_2.enabled = true;
					missionRewardCurrencyIcon2_2.spriteName = HelpersGfx.GetCurrencyIconName(currencyMissionReward2.CurrencyType, GameManager.Instance.playerModel);
				}
			}
			else if (flag4)
			{
				if (missionRewardCurrencyLabel != null)
				{
					missionRewardCurrencyLabel.enabled = true;
					missionRewardCurrencyLabel.text = GetFormattedCurrencyReward(currencyMissionReward.CurrencyType, currencyMissionReward.Amount);
				}
				if (missionRewardCurrencyIcon != null)
				{
					missionRewardCurrencyIcon.enabled = true;
					missionRewardCurrencyIcon.spriteName = HelpersGfx.GetCurrencyIconName(currencyMissionReward.CurrencyType);
					if (speedUpTitle != null)
					{
						speedUpTitle.UpdateUI(currencyMissionReward.CurrencyType);
					}
				}
				if (missionRewardCurrencyLabel1_2 != null)
				{
					missionRewardCurrencyLabel1_2.enabled = false;
				}
				if (missionRewardCurrencyIcon1_2 != null)
				{
					missionRewardCurrencyIcon1_2.enabled = false;
				}
				if (missionRewardCurrencyLabel2_2 != null)
				{
					missionRewardCurrencyLabel2_2.enabled = false;
				}
				if (missionRewardCurrencyIcon2_2 != null)
				{
					missionRewardCurrencyIcon2_2.enabled = false;
				}
			}
			if (isInWeeklySurvival && GameManager.Instance.playerModel.WeeklySurvival.DoubleRewardsEnabled)
			{
				bool flag6 = GameManager.Instance.playerModel.IsTimedBonusActive(TimedBonusType.DoubleXp);
				int num2;
				if (flag6)
				{
					RewardCurrency rewardCurrency = currencyMissionReward;
					if (rewardCurrency == null || rewardCurrency.CurrencyType != CurrencyType.SurvivalPoints)
					{
						RewardCurrency rewardCurrency2 = currencyMissionReward2;
						num2 = ((rewardCurrency2 != null && rewardCurrency2.CurrencyType == CurrencyType.SurvivalPoints) ? 1 : 0);
					}
					else
					{
						num2 = 1;
					}
				}
				else
				{
					num2 = 0;
				}
				bool flag7 = (byte)num2 != 0;
				missionRewardDoubleXpContainer.SetActive(flag7);
				float num3 = 0f;
				if (flag7)
				{
					UITweener[] componentsInChildren = missionRewardDoubleXpContainer.GetComponentsInChildren<UITweener>(includeInactive: false);
					foreach (UITweener uITweener in componentsInChildren)
					{
						if (uITweener.duration + uITweener.delay > num3)
						{
							num3 = uITweener.duration + uITweener.delay;
						}
					}
					componentsInChildren = survivalMissionDoubleRewardContainer.GetComponentsInChildren<UITweener>(includeInactive: false);
					for (int i = 0; i < componentsInChildren.Length; i++)
					{
						componentsInChildren[i].delay += num3;
					}
				}
				survivalMissionDoubleRewardContainer.gameObject.SetActive(value: true);
				if (flag4)
				{
					if (flag5)
					{
						mainCurrencyDoubleRewardAnimator.SetLabel(missionRewardCurrencyLabel1_2);
						secondaryCurrencyDoubleRewardAnimator.SetLabel(missionRewardCurrencyLabel2_2);
						secondaryCurrencyDoubleRewardAnimator.SetIgnoreTimeScale(ignoreTimeScale: true);
						SetUpCurrencyAnimations(secondaryCurrencyDoubleRewardAnimator, currencyMissionReward2, flag6, num3);
					}
					mainCurrencyDoubleRewardAnimator.SetIgnoreTimeScale(ignoreTimeScale: true);
					SetUpCurrencyAnimations(mainCurrencyDoubleRewardAnimator, currencyMissionReward, flag6, num3);
				}
			}
			else if (missionRewardDoubleXpContainer != null)
			{
				bool flag8 = (mapMissionModel.LootTag == DropEventDefinition.DropEventTag.PreferSP && GameManager.Instance.playerModel.IsTimedBonusActive(TimedBonusType.DoubleXp)) || (flag4 && currencyMissionReward.CurrencyType == CurrencyType.SurvivalPoints && GameManager.Instance.playerModel.IsTimedBonusActive(TimedBonusType.DoubleXp) && mapMissionModel.LootTag != DropEventDefinition.DropEventTag.PreferSupplies) || (flag5 && currencyMissionReward2.CurrencyType == CurrencyType.SurvivalPoints && GameManager.Instance.playerModel.IsTimedBonusActive(TimedBonusType.DoubleXp) && mapMissionModel.LootTag != DropEventDefinition.DropEventTag.PreferSupplies);
				missionRewardDoubleXpContainer.SetActive(flag8);
				if (flag8 && mainCurrencyDoubleRewardAnimator != null)
				{
					if (flag5)
					{
						if (currencyMissionReward.CurrencyType == CurrencyType.SurvivalPoints)
						{
							mainCurrencyDoubleRewardAnimator.SetLabel(missionRewardCurrencyLabel1_2);
						}
						else
						{
							mainCurrencyDoubleRewardAnimator.SetLabel(missionRewardCurrencyLabel2_2);
						}
					}
					if (flag4 || flag5)
					{
						if (currencyMissionReward.CurrencyType == CurrencyType.SurvivalPoints)
						{
							mainCurrencyDoubleRewardAnimator.Animate(currencyMissionReward.Amount, currencyMissionReward.Amount * 2);
						}
						else
						{
							mainCurrencyDoubleRewardAnimator.Animate(currencyMissionReward2.Amount, currencyMissionReward2.Amount * 2);
						}
					}
					else
					{
						DropCurrenciesStaticDefinition dropCurrencyStaticDefinition2 = mapMissionModel.manager.GameEconomyData.GetDropCurrencyStaticDefinition(mapMissionModel.LootTag, mapMissionModel.MissionLevel);
						dropCurrencyStaticDefinition2 = GameManager.Instance.playerModel.ActivityManager.ModifyActivityDefinition(dropCurrencyStaticDefinition2);
						mainCurrencyDoubleRewardAnimator.Animate(dropCurrencyStaticDefinition2.MinSurvivalPoints, dropCurrencyStaticDefinition2.MinSurvivalPoints * 2);
					}
				}
			}
			if (missionRewardSurvivorContainer != null)
			{
				bool flag9 = missionData.MissionType == MissionType.Rescue;
				missionRewardSurvivorContainer.SetActive(flag9);
				if (flag9 && missionRewardSurvivorClassIcon != null && missionData.GetRewardedSurvivorType() != SurvivorClass.None)
				{
					missionRewardSurvivorClassIcon.gameObject.SetActive(value: true);
					missionRewardSurvivorClassIcon.spriteName = HelpersGfx.GetSurvivorClassIconName(missionData.GetRewardedSurvivorType().ToString(), missionData.GetRewardedSurvivorRarityLevel());
				}
			}
			if (missionRewardSpecificEquipmentContainer != null)
			{
				missionRewardSpecificEquipmentContainer.SetActive(equipmentMissionReward != null || randomEquipmentMissionReward != null);
				if (equipmentMissionReward != null)
				{
					if (equipmentMissionReward.IsConsumableReward(GameManager.Instance.modelManager))
					{
						Helpers.GameObjectSetActive(missionRewardConsumableEquipmentContainer, value: true);
						missionRewardSpecificEquipmentIconContainer.RemoveAllChildren();
						consumableRewardTexture.mainTexture = HelpersGfx.GetTextureForEquipmentReward(equipmentMissionReward);
						consumableRewardAmount.text = equipmentMissionReward.Amount.ToString();
						if (isInWeeklySurvival && GameManager.Instance.playerModel.WeeklySurvival.DoubleRewardsEnabled)
						{
							mainCurrencyDoubleRewardAnimator.SetLabel(consumableRewardAmount);
							mainCurrencyDoubleRewardAnimator.Animate(equipmentMissionReward.Amount, equipmentMissionReward.Amount * 2);
						}
						UIButtonExtended component = missionRewardConsumableEquipmentContainer.GetComponent<UIButtonExtended>();
						component.Clear();
						component.SetClickCallback(delegate
						{
							TooltipManager.OpenTextBoxWithText(missionRewardSpecificEquipmentIconContainer, HelpersLocalization.GetShopTooltipForIReward(equipmentMissionReward));
						});
					}
					else
					{
						Helpers.GameObjectSetActive(missionRewardConsumableEquipmentContainer, value: false);
						missionRewardSpecificEquipmentIconContainer.RemoveAllChildren();
						Helpers.InstantiateToParentAndLayer(equipmentCardPrefab, missionRewardSpecificEquipmentIconContainer).GetComponent<EquipmentButton>().Setup(equipmentMissionReward, allowClick: true, traitsUnknown: true);
					}
				}
				else if (randomEquipmentMissionReward != null)
				{
					Helpers.GameObjectSetActive(missionRewardConsumableEquipmentContainer, value: false);
					missionRewardSpecificEquipmentIconContainer.RemoveAllChildren();
					Helpers.InstantiateToParentAndLayer(randomEquipmentCardPrefab, missionRewardSpecificEquipmentIconContainer).GetComponent<EquipmentRandomButton>().Setup(randomEquipmentMissionReward);
				}
			}
			if (missionRewardNoneContainer != null)
			{
				missionRewardNoneContainer.SetActive(mapMissionModel.MissionSpawnPointGroup.Category == MapCategory.Season && mapMissionModel.GetStoryMissionRewards() == null);
			}
			if (seasonFeatureRewardContainer != null)
			{
				MissionHighlight isFeaturedData = GameManager.Instance.playerModel.MapContainerModel.GetMissionGroupModelForSpawnPointGroup(mapMissionModel.MissionSpawnPointGroupId).IsFeaturedData;
				seasonFeatureRewardContainer.SetActive(mapMissionModel.MissionSpawnPointGroup.Category == MapCategory.Season && mapMissionModel.IsLastInGroup && isFeaturedData != null);
				if (isFeaturedData != null && seasonFeatureRewardIcon != null && seasonFeatureRewardDescription != null && mapMissionModel.IsLastInGroup)
				{
					List<RewardCurrency> allRewardCurrencies = isFeaturedData.CompletionRewards.GetAllRewardCurrencies();
					if (allRewardCurrencies != null && allRewardCurrencies.Count > 0)
					{
						string heroId = SurvivorToken.GetHeroId(allRewardCurrencies[0].CurrencyType);
						ActorDefinition actorDefinition = ((heroId != "") ? GameManager.Instance.playerModel.gameEconomyData.GetActorDefinition(heroId) : null);
						string text = ((actorDefinition != null) ? actorDefinition.Name : "");
						HelpersUI.SetContentToLabel(seasonFeatureRewardDescription, LocalizationManager.GetText("SeasonSevenTrial.Reward.Amount{actorName}{amount}", allRewardCurrencies[0].Amount, text));
						seasonFeatureRewardProgress.text = LocalizationManager.GetText("Popup.TeamSelection.Trial");
						seasonFeatureRemaining = isFeaturedData.EndTimeMilliseconds - GameManager.Instance.playerModel.UtcTimeStamp;
						seasonFeatureRewardIcon.spriteName = HelpersGfx.GetCurrencyIconName(allRewardCurrencies[0].CurrencyType);
					}
				}
			}
			StartCoroutine(DelayedRewardTablesReposition());
		}
		if (episodeRewardListPanel != null)
		{
			episodeRewardListPanel.ClearCards();
		}
		Helpers.GameObjectSetActive(episodeScrollBar, value: false);
		if (!(missionRewardCurrency != null && isStoryMission) || rewards == null)
		{
			return;
		}
		List<IReward> rewardsOfType = rewards.GetRewardsOfType(RewardType.Currency);
		if (episodeRewardListPanel != null && rewardsOfType != null && rewardsOfType.Count > 1)
		{
			episodeRewardListPanel.Init(rewardsOfType);
			Helpers.GameObjectSetActive(episodeScrollBar, value: true);
			if (missionRewardCurrencyLabel != null)
			{
				missionRewardCurrencyLabel.enabled = false;
			}
			if (missionRewardCurrencyIcon != null)
			{
				missionRewardCurrencyIcon.enabled = false;
			}
			if (missionRewardCurrencyLabel1_2 != null)
			{
				missionRewardCurrencyLabel1_2.enabled = false;
			}
			if (missionRewardCurrencyIcon1_2 != null)
			{
				missionRewardCurrencyIcon1_2.enabled = false;
			}
			if (missionRewardCurrencyLabel2_2 != null)
			{
				missionRewardCurrencyLabel2_2.enabled = false;
			}
			if (missionRewardCurrencyIcon2_2 != null)
			{
				missionRewardCurrencyIcon2_2.enabled = false;
			}
		}
	}

	public override void Update()
	{
		base.Update();
		if (seasonFeatureRemaining == -1)
		{
			return;
		}
		seasonFeatureRemaining -= (long)(Time.deltaTime * 1000f);
		seasonFeatureRewardTime.text = LocalizationManager.GetText("Popup.TeamSelection.Season.Time{time}", Helpers.FormatTime(seasonFeatureRemaining));
		if (seasonFeatureRemaining < 0)
		{
			seasonFeatureRemaining = -1L;
			if (seasonFeatureRewardContainer != null)
			{
				seasonFeatureRewardContainer.SetActive(value: false);
			}
		}
	}

	public void SetMission(MapMissionModel mapMissionModel)
	{
		seasonFeatureRemaining = -1L;
		if (mapMissionModel != null)
		{
			currentMissionModel = mapMissionModel;
			SetMissionReward();
			UpdateUI();
		}
	}

	private void SetMissionReward()
	{
		this.rewards = null;
		currencyMissionReward = null;
		currencyMissionReward2 = null;
		equipmentMissionReward = null;
		if (currentMissionModel.IsInWeeklySurvival)
		{
			WeeklySurvivalModel weeklySurvival = currentMissionModel.manager.Player.WeeklySurvival;
			if (weeklySurvival != null)
			{
				int num = currentMissionModel.SolveOrderNumberInGroup();
				List<WeeklySurvivalReward> personalRewardsBetween = weeklySurvival.GetPersonalRewardsBetween(num, num + 1);
				if (personalRewardsBetween != null && personalRewardsBetween.Count > 0)
				{
					Rewards rewards = new Rewards();
					for (int i = 0; i < personalRewardsBetween.Count; i++)
					{
						if (personalRewardsBetween[i].RewardEntries != null && personalRewardsBetween[i].RewardEntries.Length > (int)weeklySurvival.CurrentDifficulty && personalRewardsBetween[i].RewardEntries[(int)weeklySurvival.CurrentDifficulty] != null)
						{
							rewards.RewardsList.AddRange(personalRewardsBetween[i].RewardEntries[(int)weeklySurvival.CurrentDifficulty].RewardsList);
						}
					}
					this.rewards = rewards;
				}
			}
		}
		else
		{
			this.rewards = currentMissionModel.GetStoryMissionRewards();
		}
		if (this.rewards != null)
		{
			List<IReward> rewardsOfType = this.rewards.GetRewardsOfType(RewardType.Currency);
			if (rewardsOfType != null && rewardsOfType.Count > 0)
			{
				currencyMissionReward = rewardsOfType[0] as RewardCurrency;
			}
			if (rewardsOfType != null && rewardsOfType.Count > 1)
			{
				currencyMissionReward2 = rewardsOfType[1] as RewardCurrency;
			}
			List<IReward> rewardsOfType2 = this.rewards.GetRewardsOfType(RewardType.Equipment);
			if (rewardsOfType2 != null && rewardsOfType2.Count > 0)
			{
				equipmentMissionReward = rewardsOfType2[0] as RewardEquipment;
			}
			List<IReward> rewardsOfType3 = this.rewards.GetRewardsOfType(RewardType.RandomEquipment);
			if (rewardsOfType3 != null && rewardsOfType3.Count > 0)
			{
				randomEquipmentMissionReward = rewardsOfType3[0] as RewardRandomEquipment;
			}
		}
	}

	public void OnCloseClicked()
	{
		TweenManager.PlayTweenGroup(base.gameObject, 2, forward: true, AnimationOver);
		Close();
	}

	public override void Close()
	{
		EventManager.NotifyClick("Close");
		EventManager.NotifyClick("Back");
		base.Close();
	}

	public void onClickedSelectTeam()
	{
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_click");
		TeamSelectionPopup teamSelectionPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.MapTeamSelection) as TeamSelectionPopup;
		teamSelectionPopup.SurvivorType = SurvivorContainerModel.SurvivorType.Combat;
		if (currentMissionModel.IsInWeeklySurvival)
		{
			teamSelectionPopup.SurvivorType = SurvivorContainerModel.SurvivorType.CombatSurvival;
		}
		teamSelectionPopup.OpenForModel(currentMissionModel);
		EventManager.NotifyClick("SelectTeam");
	}

	public void OnClickFeatureHeroCall()
	{
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_click");
		NewPhonePopup.OpenRadiophoneFeaturePopup();
	}

	private void SetChallengeStarts(MissionData missionData)
	{
		if (challengeRewardStars == null || challengeRewardStars.Length != 3)
		{
			return;
		}
		for (int i = 0; i < 2; i++)
		{
			challengeRewardStars[i].SetActive(value: false);
		}
		MissionStarCondition[] conditions = missionData.MissionStarConditions.Conditions;
		if (challengeRewardLabels != null && challengeRewardLabels.Length == 3 && conditions != null)
		{
			for (int j = 0; j < missionData.MissionStarConditions.Conditions.Length && j < 3; j++)
			{
				challengeRewardLabels[j].text = LocalizationManager.GetText("Map.Star.Condition." + conditions[j].Type.ToString() + "{Parameter}", conditions[j].Parameter);
				challengeRewardStars[j].SetActive(value: true);
			}
		}
	}

	private void setQuestPortraitAndTitle(string localisationKey, string missionId, string localizedTextOptional = null)
	{
		if (!(localisationKey != "") || !(portraitSprite != null) || !(portraitLabel != null))
		{
			return;
		}
		portraitSprite.gameObject.SetActive(value: false);
		portraitTexture.gameObject.SetActive(value: false);
		string text = ((localizedTextOptional != null) ? localizedTextOptional : LocalizationManager.GetText(localisationKey));
		Match match = new Regex("<<(.*)>>").Match(text);
		Match match2 = new Regex(">>(.*)<<").Match(text);
		if (match.Success)
		{
			portraitSprite.spriteName = "Portrait_" + match.Groups[1].ToString();
			portraitLabel.text = text.Replace(match.Groups[0].ToString(), "");
			portraitSprite.gameObject.SetActive(value: true);
		}
		else if (match2.Success)
		{
			string text2 = match2.Groups[1].ToString();
			if (text2.ToLower() == "override")
			{
				text2 = HelpersLocalization.GetOverrideActor(missionId);
			}
			portraitLabel.text = text.Replace(match2.Groups[0].ToString(), "");
			ActorDefinition actorDefinition = GameManager.Instance.playerModel.gameEconomyData.GetActorDefinition(text2);
			if (actorDefinition == null)
			{
				Debug.LogWarning("ActorDefinition not found from localization key " + localisationKey + " language " + SingularityMonoBehaviour<LocalizationManager>.Instance.CurrentLanguage);
			}
			else
			{
				if (!(PortraitManager.Instance != null))
				{
					return;
				}
				Texture portrait = PortraitManager.Instance.GetPortrait(PortraitRenderSource.fromActorDefinition(actorDefinition));
				if (portrait == null)
				{
					ModularCharacter modularCharacter = ActorView.GetPrefabForActor(actorDefinition.ID, actorDefinition.VisualAsset);
					if (modularCharacter == null)
					{
						modularCharacter = ActorView.SelectRandomPrefabForActorDefinition(actorDefinition.ID, actorDefinition.Gender);
					}
					portraitActorId = actorDefinition.ID;
					PortraitManager.Instance.CreatePortrait(PortraitRenderSource.fromActorDefinition(actorDefinition), modularCharacter, OnMissingPortraitRendered);
				}
				else
				{
					portraitTexture.mainTexture = portrait;
					portraitTexture.gameObject.SetActive(value: true);
				}
			}
		}
		else
		{
			portraitLabel.text = text;
			portraitSprite.spriteName = "Portrait_Daryl";
			portraitSprite.gameObject.SetActive(value: true);
		}
	}

	private void OnMissingPortraitRendered(IPortraitRenderSource info)
	{
		if (portraitTexture != null && info != null && info.ActorDefinitionId == portraitActorId)
		{
			portraitTexture.mainTexture = PortraitManager.Instance.GetPortrait(info);
			portraitTexture.gameObject.SetActive(value: true);
		}
	}

	private void OnSurvivorChanged(ModelObject modelObject, string changed, object args)
	{
		if (changed == "ActionFinishedEvent")
		{
			UpdateUI();
		}
	}

	private static void setActive(GameObject obj, bool setActive)
	{
		if (obj != null)
		{
			if (setActive && !obj.activeSelf)
			{
				obj.SetActive(value: true);
			}
			else if (!setActive && obj.activeSelf)
			{
				obj.SetActive(value: false);
			}
		}
	}

	private void OnUIEvent(string type, object parameter)
	{
		if (type == "SurvivalDoubleRewardsEnabled")
		{
			UpdateUI();
		}
		if (type == "OnPopUpOpen" && parameter is WeeklyChallengeCyclePopup)
		{
			Close();
		}
	}

	private void SetUpCurrencyAnimations(AnimateNumberFromTo currencyAnimator, RewardCurrency rewardCurrency, bool doubleXpBoosterActive, float tweenDelay)
	{
		bool isComponentCurrency = ComponentHelper.IsComponentCurrency(rewardCurrency.CurrencyType);
		bool flag = rewardCurrency.CurrencyType != CurrencyType.SurvivalPoints;
		bool num = rewardCurrency.CurrencyType == CurrencyType.SurvivalPoints && doubleXpBoosterActive;
		AnimateCurrencies(currencyAnimator, rewardCurrency.Amount, rewardCurrency.Amount * 2, flag ? tweenDelay : 0f, isComponentCurrency, rewardCurrency.CurrencyType);
		if (num)
		{
			AnimateCurrencies(currencyAnimator, rewardCurrency.Amount * 2, rewardCurrency.Amount * 4, tweenDelay, isComponentCurrency: false);
		}
	}

	private void AnimateCurrencies(AnimateNumberFromTo currencyAnimator, int from, int to, float startDelay, bool isComponentCurrency, CurrencyType component = CurrencyType.None)
	{
		currencyAnimator.AddDelayToStart(startDelay);
		if (isComponentCurrency)
		{
			currencyAnimator.AnimateComponentCurrency(from, to, component);
		}
		else
		{
			currencyAnimator.Animate(from, to);
		}
	}

	private IEnumerator DelayedRewardTablesReposition()
	{
		yield return new WaitForEndOfFrame();
		if (rewardTables != null && rewardTables.Length != 0)
		{
			for (int i = 0; i < rewardTables.Length; i++)
			{
				rewardTables[i].Reposition();
			}
		}
	}
}
