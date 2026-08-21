using System.Collections;
using System.Collections.Generic;
using BaseModel;
using BaseModel.ContentTypes;
using TWDModel;
using UnityEngine;

public class CampManager : MonoBehaviour
{
	private static int IAPRetryDelay = 2;

	private static int IAPREtryAmount = 6;

	private static CampManager instance = null;

	private IEnumerator waitForIAPsLoadedCoroutine;

	private IEnumerator delayedTriggerShowBundlesCoroutine;

	public static CampManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = Object.FindObjectOfType<CampManager>();
			}
			return instance;
		}
	}

	public static bool IsInstanceNull => instance == null;

	public CampBackground CampBackground { get; private set; }

	public static void Toggle(bool visibility)
	{
		if (Instance != null && visibility && CampView.Instance != null)
		{
			CampView.Instance.CampViewBuildings.UpdateBuildingIndicators();
			CampView.Instance.CampViewActors.UpdateQuestIndicators();
			BuildingMenu buildingMenu = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CampBuildingMenu, null, createIfNotExist: false) as BuildingMenu;
			if (buildingMenu != null)
			{
				buildingMenu.UpdateFollowTarget();
			}
		}
	}

	private void Awake()
	{
		if (OfflineManager.IsLoadDataManager)
		{
			DebugTWD.LogMycode("if (IsLoadDataManager) return");
			return;
		}
		GameManager.Instance.SetState(GameState.Camp);
		GameManager.Instance.OnLoadCompleted += OnLoadCompleted;
	}

	private void OnEnable()
	{
		UIEvent.OnUIEvent += OnEvent;
	}

	private void OnDisable()
	{
		UIEvent.OnUIEvent -= OnEvent;
	}

	private void OnEvent(string type, object parameter)
	{
		if (type == "OnTutorialDialogClicked")
		{
			CheckForPendingIAPVisualizations();
		}
	}

	private void OnLoadCompleted()
	{
		CampBackground = Object.FindObjectOfType<CampBackground>();
		if (GameManager.Instance.ForceGoThatDetailMap != null && GameManager.Instance.ForceGoThatDetailMap.AttackTargetId == GameManager.Instance.gameEconomyData.ConfigData.OutpostTutorialSpawnPointGroupId)
		{
			GameManager.Instance.ForceGoThatDetailMap = null;
		}
		bool forceOpenWorldBoss = GameManager.Instance.ForceOpenWorldBoss;
		bool flag = forceOpenWorldBoss && ShouldAutoOpenWorldBossUiAfterCombat(requireMatchingAttackTarget: false);
		bool show = GameManager.Instance.ForceGoThatDetailMap == null && !flag;
		ShowCamp(show);
		if (GameManager.Instance.playerModel.PendingVideoAdReward && GameManager.Instance.playerModel.IsVideoAdRewardAvailable(AdUsage.CinemaReward) && SingularityMonoBehaviour<HUDManager>.Instance.GetNoCreation(UIType.OpenLootInUi) == null)
		{
			SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.AdPopupView).Open();
		}
		if (GameManager.Instance.ForceGoThatDetailMap != null && !GameManager.Instance.ForceGoThatDetailMap.IsDisabledOnGED)
		{
			ShowDetailMap(GameManager.Instance.ForceGoThatDetailMap);
			GameManager.Instance.ForceGoThatDetailMap = null;
		}
		else if (forceOpenWorldBoss)
		{
			GameManager.Instance.ForceOpenWorldBoss = false;
			if (flag)
			{
				MissionHubNavigation.OpenWorldBoss();
				if (CampView.Instance != null && CampView.Instance.Hud != null)
				{
					CampView.Instance.Hud.ShowGenericElement(show: true);
					CampView.Instance.Hud.UpdateGenericElementsAfterChange();
				}
			}
		}
		GameManager.Instance.playerModel.CampMover.Changed += OnCampMoveModelChange;
		GameManager.Instance.playerModel.Changed += OnPlayerModelChange;
		if (GameManager.Instance.playerModel.BundleManager != null)
		{
			GameManager.Instance.playerModel.BundleManager.Changed += OnBundleManagerChanged;
		}
		if (GameManager.Instance.playerModel.WeeklyChallengeClassTeamActivity != null)
		{
			GameManager.Instance.playerModel.WeeklyChallengeClassTeamActivity.Changed += OnWeeklyChallengeActivityManagerChanged;
		}
		TutorialView.Instance.StartNextTutorial();
		for (int i = 0; i < GameManager.Instance.gameEconomyData.SeasonDefinitions.Length; i++)
		{
			SeasonDefinition seasonDefinition = GameManager.Instance.gameEconomyData.SeasonDefinitions[i];
			CurrencyType rewardCurrency = seasonDefinition.RewardCurrency;
			if (rewardCurrency == CurrencyType.None)
			{
				continue;
			}
			string heroId = SurvivorToken.GetHeroId(rewardCurrency);
			ActorDefinition actorDefinition = GameManager.Instance.gameEconomyData.GetActorDefinition(heroId);
			bool flag2 = GameManager.Instance.playerModel.SurvivorContainer.HasHero(heroId);
			if (actorDefinition != null && GameManager.Instance.playerModel.GetCurrency(rewardCurrency).Value >= actorDefinition.TokensToUnlock && !flag2 && (TutorialView.Instance == null || !TutorialView.Instance.Running))
			{
				int counter = GameManager.Instance.Blackboard.GetCounter(BlackboardModel.GetPromptedUnlocksPerActorKey(actorDefinition.ID));
				int maxPromptedUnlocksPerActor = GameManager.Instance.gameEconomyData.ConfigData.MaxPromptedUnlocksPerActor;
				if (counter < maxPromptedUnlocksPerActor)
				{
					Helpers.ExecuteCommand(new SeasonRewardPromptSeenCommand(actorDefinition));
					(SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.QuestPopup) as QuestPopup).OpenForSeasonReward(seasonDefinition);
					break;
				}
			}
		}
	}

	public void ShowDetailMap(IAttackTargetModel attackedMapMissionGroupModel)
	{
		MapMissionGroupModel mapMissionGroupModel = attackedMapMissionGroupModel as MapMissionGroupModel;
		if (mapMissionGroupModel != null)
		{
			if (!mapMissionGroupModel.HasBeenViewed)
			{
				Helpers.ExecuteCommand(new MarkObjectViewedCommand(mapMissionGroupModel));
			}
			GameManager gameManager = GameManager.Instance;
			if (gameManager.gameEconomyData.IsEpisodeWeeklyChallenge(mapMissionGroupModel.MissionSpawnPointGroupId))
			{
				gameManager.StartNextChallenge();
				if (gameManager.playerModel != null && gameManager.playerModel.WeeklyChallenge != null)
				{
					mapMissionGroupModel = gameManager.playerModel.WeeklyChallenge.GetMapMissionGroupModel();
				}
			}
			if (gameManager.gameEconomyData.IsEpisodeApocalypticWeeklyChallenge(mapMissionGroupModel.MissionSpawnPointGroupId))
			{
				gameManager.StartNextChallenge();
				if (gameManager.playerModel != null && gameManager.playerModel.ApocalypseWeeklyChallenge != null)
				{
					mapMissionGroupModel = gameManager.playerModel.ApocalypseWeeklyChallenge.GetMapMissionGroupModel();
				}
			}
			TutorialView.Instance.UpdateSuggestion();
			GoToMap(mapMissionGroupModel);
		}
		else
		{
			TutorialView.Instance.UpdateSuggestion();
			GoToGuildBattleMap(attackedMapMissionGroupModel);
		}
	}

	private void OnDestroy()
	{
		if (OfflineManager.IsLoadDataManager || GameManager.Instance && GameManager.Instance.playerModel == null)
		{
			DebugTWD.LogMycode("if (GameManager.Instance && GameManager.Instance.playerModel == null) return");
		}
		else
		{
			GameManager.Instance.playerModel.CampMover.Changed -= OnCampMoveModelChange;
			GameManager.Instance.playerModel.Changed -= OnPlayerModelChange;
			if (GameManager.Instance.playerModel.BundleManager != null)
			{
				GameManager.Instance.playerModel.BundleManager.Changed -= OnBundleManagerChanged;
			}
		if (GameManager.Instance.playerModel.WeeklyChallengeClassTeamActivity != null)
		{
			GameManager.Instance.playerModel.WeeklyChallengeClassTeamActivity.Changed -= OnWeeklyChallengeActivityManagerChanged;
		}
			if (!OfflineManager.IsLoadDataManager) GameManager.Instance.OnLoadCompleted -= OnLoadCompleted;
			instance = null;
		}
	}

	public void GoToCamp()
	{
		SingularityMonoBehaviour<HUDManager>.Instance.CloseAllOpenPopupsAndDialogs();
		ShowCamp(show: true);
		ShowMap(show: false);
		SingularityMonoBehaviour<HUDManager>.Instance.CloseIfExists(UIType.Transition);
		TutorialView.Instance.UpdateSuggestion();
	}

	public void GoToGuildBattleMap(IAttackTargetModel model = null)
	{
		if (!GameManager.Instance.gameEconomyData.GetFeature("Social").Enabled)
		{
			AlertPopup.ShowPopup(LocalizationManager.GetText("Popup.Alert.NotAvailableTitle"), LocalizationManager.GetText("Popup.Alert.NotAvailableMessage"), LocalizationManager.GetText("Button.Ok"));
		}
		else
		{
			if (!GameManager.Instance.playerModel.IsGuildMember)
			{
				return;
			}
			if (GuildWarHelper.IsLockedByCouncilLevelOrTutorial())
			{
				FeatureLockedPopup.Open(FeatureLockedPopup.FeatureType.GuildBattle, locked: true);
				return;
			}
			if (CampView.Instance != null)
			{
				CampView.Instance.CancelBuildingPlacement();
			}
			SingularityMonoBehaviour<HUDManager>.Instance.CloseAllOpenPopupsAndDialogs(new List<UIType>
			{
				UIType.GuildBattleMapPopup,
				UIType.GvGStartBattleFlowPopup,
				UIType.GvGHubPopup
			});
			if (!SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.GuildBattleMapPopup).IsOpen)
			{
				GuildBattleMapPopup obj = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.GuildBattleMapPopup) as GuildBattleMapPopup;
				obj.MapMissionModel = model as GuildBattleMapMissionModel;
				obj.Open();
				GameManager.Instance.Blackboard.IsToggleOn("Toggle.GuildBattleUnlockedSeen");
			}
		}
	}

	public void GoToMap(MapMissionGroupModel detailMapMissionGroupModel = null)
	{
		if (CampView.Instance != null)
		{
			CampView.Instance.CancelBuildingPlacement();
		}
		SingularityMonoBehaviour<HUDManager>.Instance.CloseAllOpenPopupsAndDialogs();
		MapCategory mapCategory = detailMapMissionGroupModel?.MissionSpawnPointGroup.Category ?? MapCategory.None;
		Feature feature = GameManager.Instance.gameEconomyData.GetFeature("Social");
		switch (mapCategory)
		{
		case MapCategory.Story:
		{
			DetailMapPopUp obj3 = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.DetailMapPopUp) as DetailMapPopUp;
			obj3.Open();
			obj3.LoadEpisode();
			break;
		}
		case MapCategory.Challenge:
			if (feature.Enabled)
			{
				DetailMapPopUp obj = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.DetailMapPopUp) as DetailMapPopUp;
				obj.Open();
				obj.LoadChallengeMap();
				WeeklyChallengeModel weeklyChallengeModel = WeeklyChallengeHelper.GetWeeklyChallengeModel();
				if (weeklyChallengeModel != null && weeklyChallengeModel.Finished)
				{
					WeeklyChallengeEndPopup.TryOpenWithWeeklyModel(weeklyChallengeModel);
				}
				FeatureUIHighlights.MarkHighlightExpired(FeatureUIHighlights.FeaturesIds.WeeklyChallengeUnlocked);
			}
			else if (feature.ShowPopup)
			{
				OptionalUpdatePopup.OpenFeatureLockedContent();
				return;
			}
			break;
		case MapCategory.ApocalypticChallenge:
			if (feature.Enabled)
			{
				DetailMapPopUp obj2 = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.DetailMapPopUp) as DetailMapPopUp;
				obj2.Open();
				obj2.LoadApocalypticChallengeMap();
				FeatureUIHighlights.MarkHighlightExpired(FeatureUIHighlights.FeaturesIds.WeeklyChallengeUnlocked);
			}
			else if (feature.ShowPopup)
			{
				OptionalUpdatePopup.OpenFeatureLockedContent();
				return;
			}
			break;
		case MapCategory.Survival:
		{
			DetailMapPopUp obj4 = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.DetailMapPopUp) as DetailMapPopUp;
			obj4.Open();
			obj4.LoadSurvivalMap();
			FeatureUIHighlights.MarkHighlightExpired(FeatureUIHighlights.FeaturesIds.WeeklySurvivalUnlocked);
			break;
		}
		case MapCategory.Season:
			if (detailMapMissionGroupModel.MissionSpawnPointGroup != null)
			{
				DetailMapPopUp obj5 = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.DetailMapPopUp) as DetailMapPopUp;
				obj5.Open();
				obj5.LoadSeason(detailMapMissionGroupModel);
				FeatureUIHighlights.MarkHighlightExpired(FeatureUIHighlights.FeaturesIds.SeasonModeUnlocked);
			}
			break;
		case MapCategory.Grind:
			SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.ScavengePopup).Open();
			FeatureUIHighlights.MarkHighlightExpired(FeatureUIHighlights.FeaturesIds.ScavengeModeUnlocked);
			break;
		case MapCategory.Endless:
			if (EndlessModeHelpers.IsEndlessModeActive())
			{
				if (EndlessModeHelpers.IsEndlessExpertMode())
				{
					SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.EndlessMissionHubPopup).Open();
				}
				else
				{
					SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.EndlessNormalMissionHubPopup).Open();
				}
			}
			else
			{
				MissionHubPopup.OpenPopup();
			}
			break;
		}
		if (mapCategory != MapCategory.Challenge || (mapCategory == MapCategory.Challenge && feature.Enabled) || (mapCategory == MapCategory.ApocalypticChallenge && feature.Enabled))
		{
			ShowCamp(show: false);
			CampView.Instance.Hud.ShowGenericElement(show: true);
			CampView.Instance.Hud.UpdateGenericElementsAfterChange();
		}
	}

	public void GoToSeasonMap(string seasonId)
	{
		if (CampView.Instance != null)
		{
			CampView.Instance.CancelBuildingPlacement();
		}
		SingularityMonoBehaviour<HUDManager>.Instance.CloseAllOpenPopupsAndDialogs();
		ShowCamp(show: false);
		DetailMapPopUp obj = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.DetailMapPopUp, OfflineManager.IsLoadDataManager ? HUDManager.Instance.UIContainerTopCameras : null) as DetailMapPopUp;
		obj.Open();
		obj.LoadSeason(seasonId);
	}

	public void FullscreenPopupShowCamp(bool show, DisableCampMode mode = DisableCampMode.All)
	{
		switch (mode)
		{
		case DisableCampMode.All:
			if (CampView.Instance != null)
			{
				CampView.Instance.CancelBuildingPlacement();
			}
			ShowCamp(show);
			break;
		case DisableCampMode.CameraAndUI:
			if (!CampView.Instance.IsShown && show)
			{
				ShowCamp(show: true);
			}
			CampView.Instance.SetEnabledCameraAndUI(show);
			break;
		}
	}

	public void ReturnFromMap()
	{
		ShowCamp(show: true);
	}

	private void ShowCamp(bool show)
	{
		if (CampView.Instance == null)
		{
			return;
		}
		CampView.Instance.SetEnabled(show);
		if (CampBackground != null)
		{
			CampBackground.SetEnabled(show);
		}
		StartCoroutine(DelayCampDefenseShowing(show));
		if (show)
		{
			CheckBundleTrigger();
			if (PortraitManager.Instance != null)
			{
				PortraitManager.Instance.RemoveUnusedPortraits();
			}
			EventManager.NotifyEvent(EventManager.EventType.ShowCamp);
			CheckForPendingIAPVisualizations();
			SingularityMonoBehaviour<AudioManager>.Instance.RequestMusicStateChange(MusicState.Camp);
			TryCollectUnclaimedActiveFoundationRewards();
			TryCollectUnclaimedThreeDayRewards();
			TryCompleteReturnPrivilegeTask();
		}
	}

	private void TryCompleteReturnPrivilegeTask()
	{
		ReturnPrivilegeModel returnPrivilegeModel = GameManager.Instance?.playerModel?.ReturnActivityManager?.ReturnPrivilege;
		if (returnPrivilegeModel != null && returnPrivilegeModel.IsCurrentTaskClaimable && returnPrivilegeModel.IsTaskProgressCompleted)
		{
			Helpers.ExecuteCommandDelayed(new CompleteReturnPrivilegeTaskCommand());
		}
	}

	private void CheckForPendingIAPVisualizations()
	{
		if ((!(TutorialView.Instance != null) || !TutorialView.Instance.Running || !TutorialView.Instance.IsWaitingForClick || TutorialView.Instance.IsSuggesting) && GameManager.Instance != null && GameManager.Instance.playerModel != null && GameManager.Instance.playerModel.BundleManager != null && !string.IsNullOrEmpty(GameManager.Instance.playerModel.BundleManager.PendingViewBundleContentDefinition) && !string.IsNullOrEmpty(GameManager.Instance.playerModel.BundleManager.PendingViewBundleStoreDefinition))
		{
			if ((GameManager.Instance.playerModel.BundleManager.PendingViewBundleContentDefinition == BundleManagerModel.FAKE_SUPPORT_BUNDLE_FOR_REWARDS || GameManager.Instance.playerModel.BundleManager.PendingViewBundleStoreDefinition == BundleManagerModel.FAKE_SUPPORT_BUNDLE_FOR_REWARDS) && !string.IsNullOrEmpty(GameManager.Instance.playerModel.BundleManager.PendingViewRewardsGivenBySupport))
			{
				GameManager.Instance.playerModel.gameEconomyData.CreateTemporaryRewardBundleDefinitions(BundleManagerModel.FAKE_SUPPORT_BUNDLE_FOR_REWARDS, GameManager.Instance.playerModel.BundleManager.PendingViewRewardsGivenBySupport);
			}
			BundleStoreDefinition bundleStoreDefinition = GameManager.Instance.playerModel.gameEconomyData.GetBundleStoreDefinition(GameManager.Instance.playerModel.BundleManager.PendingViewBundleStoreDefinition);
			BundleContentDefinition bundleContentDefinition = GameManager.Instance.playerModel.gameEconomyData.GetBundleContentDefinition(GameManager.Instance.playerModel.BundleManager.PendingViewBundleStoreDefinition);
			if (bundleStoreDefinition != null && bundleContentDefinition != null)
			{
				IAPConfirmPopupNew.OpenWithBundleContent(bundleStoreDefinition, bundleContentDefinition, GameManager.Instance.playerModel.BundleManager.PendingViewBundleWasGivenBySupport);
			}
		}
		TryCollectUnclaimedPastCampaignRewards();
		TryCollectUnclaimedPastSevenDayRewards();
	}

	private void TryCollectUnclaimedPastSevenDayRewards()
	{
		if ((TutorialView.Instance != null && TutorialView.Instance.Running && TutorialView.Instance.IsWaitingForClick && !TutorialView.Instance.IsSuggesting) || GameManager.Instance == null || GameManager.Instance.playerModel == null || SingularityMonoBehaviour<HUDManager>.Instance.IsOpen(UIType.IAPConfirmPopupNew))
		{
			return;
		}
		List<IReward> outputSevenDayLoginRewardList = null;
		if (GameManager.Instance.playerModel.SevenDayLoginManager.TryRetrieveUnclaimedRewards(ref outputSevenDayLoginRewardList) && Helpers.ExecuteCommand(new SevenDayLoginGivePastRewardCommand()) == TWDModelResult.OK && outputSevenDayLoginRewardList != null && outputSevenDayLoginRewardList.Count > 0)
		{
			IAPConfirmPopupNew iAPConfirmPopupNew = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.IAPConfirmPopupNew) as IAPConfirmPopupNew;
			if (iAPConfirmPopupNew != null)
			{
				iAPConfirmPopupNew.OpenForRewards(outputSevenDayLoginRewardList);
			}
		}
	}

	private void TryCollectUnclaimedPastCampaignRewards()
	{
		if ((TutorialView.Instance != null && TutorialView.Instance.Running && TutorialView.Instance.IsWaitingForClick && !TutorialView.Instance.IsSuggesting) || GameManager.Instance == null || GameManager.Instance.playerModel == null || GameManager.Instance.playerModel.CampaignModel == null || !GameManager.Instance.playerModel.CampaignModel.ContainsPastCampaignRewards() || GameManager.Instance.playerModel.BundleManager.PendingViewEquipments.Count > 0 || !string.IsNullOrEmpty(GameManager.Instance.playerModel.BundleManager.PendingViewBundleContentDefinition) || SingularityMonoBehaviour<HUDManager>.Instance.IsOpen(UIType.IAPConfirmPopupNew))
		{
			return;
		}
		string output = null;
		if (GameManager.Instance.playerModel.CampaignModel.TryRetrieveUnclaimedRewardsString(ref output) && Helpers.ExecuteCommand(new ClaimAllPastCampaignRewardsCommand()) == TWDModelResult.OK)
		{
			string text = "CAMPAIGN_COLLECT_REWARDS";
			GameManager.Instance.playerModel.gameEconomyData.CreateTemporaryRewardBundleDefinitions(text, output);
			BundleStoreDefinition bundleStoreDefinition = GameManager.Instance.playerModel.gameEconomyData.GetBundleStoreDefinition(text);
			BundleContentDefinition bundleContentDefinition = GameManager.Instance.playerModel.gameEconomyData.GetBundleContentDefinition(text);
			if (bundleStoreDefinition != null && bundleContentDefinition != null)
			{
				ModelList<EquipmentItemModel> equipmentList = GameManager.Instance.playerModel.BundleManager.PendingViewEquipments;
				GameManager.Instance.playerModel.CampaignModel.TryRetrieveClaimedUnclaimedEquipment(ref equipmentList);
				ModelList<EquipTokenItemModel> equipTokenItemList = GameManager.Instance.playerModel.BundleManager.PendingViewEquipTokens;
				GameManager.Instance.playerModel.CampaignModel.TryRetrieveClaimedUnclaimedEquipToken(ref equipTokenItemList);
				GameManager.Instance.playerModel.BundleManager.SetPendingViewDefinitionId(text);
				IAPConfirmPopupNew.OpenWithBundleContent(bundleStoreDefinition, bundleContentDefinition, givenBySupport: false, "Popup.IAPConfirm.ReceivedFromCampaign");
				GameManager.Instance.playerModel.BundleManager.SetPendingViewDefinitionId("");
				GameManager.Instance.playerModel.BundleManager.PendingViewEquipments.Clear();
				GameManager.Instance.playerModel.BundleManager.PendingViewEquipTokens.Clear();
				GameManager.Instance.playerModel.gameEconomyData.RemoveTemporaryRewardBundleDefinitions(text);
			}
		}
		List<IReward> outputSevenDayLoginRewardList = null;
		if (GameManager.Instance.playerModel.SevenDayLoginManager.TryRetrieveUnclaimedRewards(ref outputSevenDayLoginRewardList) && Helpers.ExecuteCommand(new SevenDayLoginGivePastRewardCommand()) == TWDModelResult.OK && outputSevenDayLoginRewardList != null && outputSevenDayLoginRewardList.Count > 0)
		{
			IAPConfirmPopupNew iAPConfirmPopupNew = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.IAPConfirmPopupNew) as IAPConfirmPopupNew;
			if (iAPConfirmPopupNew != null)
			{
				iAPConfirmPopupNew.OpenForRewards(outputSevenDayLoginRewardList);
			}
		}
	}

	private IEnumerator DelayCampDefenseShowing(bool show)
	{
		yield return null;
		if (CampDefenseView.Instance != null)
		{
			CampDefenseView.Instance.SetEnabled(show);
		}
		if (show && CampView.Instance != null)
		{
			CampView.Instance.CampViewBuildings.UpdateBuildingIndicators();
			CampView.Instance.CampViewActors.UpdateQuestIndicators();
		}
	}

	private void ShowMap(bool show, MapMissionGroupModel detailMapMissionGroupModel = null)
	{
		if (show)
		{
			DetailMapPopUp detailMapPopUp = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.DetailMapPopUp) as DetailMapPopUp;
			detailMapPopUp.Open();
			if (detailMapMissionGroupModel == null)
			{
				detailMapPopUp.LoadEpisode();
			}
			else if (detailMapMissionGroupModel.MissionSpawnPointGroup.Category == MapCategory.Season)
			{
				detailMapPopUp.LoadSeason(detailMapMissionGroupModel);
			}
			else
			{
				detailMapPopUp.LoadEpisode(detailMapMissionGroupModel.MissionSpawnPointGroupId);
			}
		}
	}

	private void OnCampMoveModelChange(ModelObject m, string changed, object args)
	{
		if (changed == "campMoved")
		{
			CampView.Instance.UpdateGridSpaceTranslation();
		}
	}

	private IEnumerator DelayedTriggerShowBundles()
	{
		yield return null;
		Helpers.StartCoroutine(this, WaitForIAPsLoaded(IAPRetryDelay, IAPREtryAmount), ref waitForIAPsLoadedCoroutine);
	}

	public void OnPlayerModelChange(ModelObject m, string changed, object args)
	{
		if (changed == "iapOfferAvailableEvent")
		{
			Helpers.StartCoroutine(this, DelayedTriggerShowBundles(), ref delayedTriggerShowBundlesCoroutine);
		}
		else if (changed == "iapOfferExpiredEvent" && GameManager.Instance != null && GameManager.Instance.playerModel != null && SingularityMonoBehaviour<HUDManager>.Instance != null)
		{
			BundleCardPopup bundleCardPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.BundleCardPopup, null, createIfNotExist: false) as BundleCardPopup;
			if (bundleCardPopup != null && bundleCardPopup.IsOpen && bundleCardPopup.bundleData != null && bundleCardPopup.bundleData.bundleContentDefinition != null && bundleCardPopup.bundleData.bundleContentDefinition.Category != BundleContentDefinition.CategoryGoldPack)
			{
				bundleCardPopup.Close();
				SingularityMonoBehaviour<HUDManager>.Instance.CloseIfExists(UIType.CampBuildMenu);
			}
		}
	}

	public void OnBundleManagerChanged(ModelObject m, string changed, object args)
	{
		if (changed == "LimitedBundleAvailableEvent")
		{
			Helpers.StartCoroutine(this, DelayedTriggerShowBundles(), ref delayedTriggerShowBundlesCoroutine);
		}
		else
		{
			if (!(changed == "LimitedBundleExpiredEvent") || !(GameManager.Instance != null) || GameManager.Instance.playerModel == null || !(SingularityMonoBehaviour<HUDManager>.Instance != null))
			{
				return;
			}
			BundleCardPopup bundleCardPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.BundleCardPopup, null, createIfNotExist: false) as BundleCardPopup;
			if (!(bundleCardPopup != null) || !bundleCardPopup.IsOpen || !string.IsNullOrEmpty(GameManager.Instance.playerModel.BundleManager.PendingViewBundleContentDefinition))
			{
				return;
			}
			BundleContentDefinition bundleContentDefinition = null;
			if (bundleCardPopup.bundleData != null)
			{
				bundleContentDefinition = bundleCardPopup.bundleData.bundleContentDefinition;
			}
			if (bundleContentDefinition != null)
			{
				string text = args as string;
				if (bundleContentDefinition.Category != BundleContentDefinition.CategoryGoldPack && !string.IsNullOrEmpty(text) && bundleContentDefinition.Identifier == text)
				{
					bundleCardPopup.Close();
					SingularityMonoBehaviour<HUDManager>.Instance.CloseIfExists(UIType.CampBuildMenu);
				}
			}
		}
	}

	public void OnWeeklyChallengeActivityManagerChanged(ModelObject m, string changed, object args)
	{
		if (changed == "ClassTeamCloseExchangeRewards")
		{
			Rewards lastCloseExchangeRewards = GameManager.Instance.playerModel.WeeklyChallengeClassTeamActivity.Shop.LastCloseExchangeRewards;
			IAPConfirmPopupNew iAPConfirmPopupNew = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.IAPConfirmPopupNew) as IAPConfirmPopupNew;
			if (iAPConfirmPopupNew != null && lastCloseExchangeRewards != null)
			{
				iAPConfirmPopupNew.OpenForRewards(lastCloseExchangeRewards.RewardsList);
			}
		}
	}
	public void CheckBundleTrigger()
	{
		if (GameManager.Instance.BundleCheckDone)
		{
			return;
		}
		GameManager.Instance.BundleCheckDone = true;
		if (!OfflineManager.IsTutorialDisable && TutorialView.Instance.Running)
		{
			return;
		}

		if (GameManager.Instance.IAPManager.IsInitialized() || Application.isEditor)
		{
			BundleCardPopup.TryOpenSuitableBundle(saveTimestamp: true, addTimer: true);
			return;
		}
		bool flag = false;
		Feature feature = GameManager.Instance.gameEconomyData.GetFeature("GiftOfferOldPromoLogic");
		if (feature != null && feature.Enabled)
		{
			BundleStoreDefinition bundleStoreDefinitionToShowInPromo = GameManager.Instance.playerModel.BundleManager.GetBundleStoreDefinitionToShowInPromo(-1.0);
			if (bundleStoreDefinitionToShowInPromo != null)
			{
				BundleContentDefinition bundleContentDefinition = GameManager.Instance.playerModel.gameEconomyData.GetBundleContentDefinition(bundleStoreDefinitionToShowInPromo.BundleIdentifier);
				if (bundleContentDefinition != null && string.IsNullOrEmpty(bundleContentDefinition.IAPProduct))
				{
					flag = true;
				}
			}
		}
		if (flag)
		{
			BundleCardPopup.TryOpenSuitableBundle();
		}
		else
		{
			Helpers.StartCoroutine(this, WaitForIAPsLoaded(IAPRetryDelay, IAPREtryAmount), ref waitForIAPsLoadedCoroutine);
		}
	}

	private IEnumerator WaitForIAPsLoaded(float delayTime, int retries)
	{
		for (int i = 1; i <= retries; i++)
		{
			if (!GameManager.Instance.IAPManager.IsInitialized())
			{
				Debug.LogWarning("BundleTrigger: IAPs Not Loaded. Retry: " + i + " starts in " + delayTime + "s");
				yield return new WaitForSeconds(delayTime);
			}
		}
		if (GameManager.Instance.IAPManager.IsInitialized())
		{
			BundleCardPopup.TryOpenSuitableBundle(saveTimestamp: true, addTimer: true);
		}
		else
		{
			AnalyticsManager.instance.CreateEvent("BundlePopup_IAPError").AddProperty("TotalRetries", retries).Send();
			Debug.LogWarning("BundleTrigger: Bundle Failed To Open IAPs Not Loaded. Retries Total: " + retries + " with delay: " + delayTime);
		}
		waitForIAPsLoadedCoroutine = null;
	}

	private void TryCollectUnclaimedActiveFoundationRewards()
	{
		if ((TutorialView.Instance != null && TutorialView.Instance.Running && TutorialView.Instance.IsWaitingForClick && !TutorialView.Instance.IsSuggesting) || GameManager.Instance == null || GameManager.Instance.playerModel == null || SingularityMonoBehaviour<HUDManager>.Instance.IsOpen(UIType.IAPConfirmPopupNew))
		{
			return;
		}
		List<IReward> outputActiveFoundationRewardList = null;
		if (GameManager.Instance.playerModel.ActiveFoundationManager.TryRetrieveUnclaimedRewards(ref outputActiveFoundationRewardList) && Helpers.ExecuteCommand(new ActiveFoundationGivePastRewardCommand()) == TWDModelResult.OK && outputActiveFoundationRewardList != null && outputActiveFoundationRewardList.Count > 0)
		{
			IAPConfirmPopupNew iAPConfirmPopupNew = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.IAPConfirmPopupNew) as IAPConfirmPopupNew;
			if (iAPConfirmPopupNew != null)
			{
				iAPConfirmPopupNew.OpenForRewards(outputActiveFoundationRewardList);
			}
		}
	}

	private void TryCollectUnclaimedThreeDayRewards()
	{
		if ((TutorialView.Instance != null && TutorialView.Instance.Running && TutorialView.Instance.IsWaitingForClick && !TutorialView.Instance.IsSuggesting) || GameManager.Instance == null || GameManager.Instance.playerModel == null || SingularityMonoBehaviour<HUDManager>.Instance.IsOpen(UIType.IAPConfirmPopupNew))
		{
			return;
		}
		List<IReward> needPopReward = GameManager.Instance.playerModel.ThreeDayModel.GetNeedPopReward();
		if (needPopReward != null && needPopReward.Count > 0 && Helpers.ExecuteCommand(new ThreeDayRewardPopCommand()) == TWDModelResult.OK && needPopReward != null && needPopReward.Count > 0)
		{
			IAPConfirmPopupNew iAPConfirmPopupNew = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.IAPConfirmPopupNew) as IAPConfirmPopupNew;
			if (iAPConfirmPopupNew != null)
			{
				iAPConfirmPopupNew.OpenForRewards(needPopReward);
			}
		}
	}

	public static bool ShouldAutoOpenWorldBossUiAfterCombat(bool requireMatchingAttackTarget = true)
	{
		WorldBossModelManager worldBossModelManager = ((GameManager.Instance != null) ? GameManager.Instance.playerModel : null)?.WorldBossModelManager;
		if (worldBossModelManager == null)
		{
			return false;
		}
		if (worldBossModelManager.GetUnlockState() != WorldBossUnlockState.Unlocked)
		{
			return false;
		}
		if (!worldBossModelManager.IsCycleOpen())
		{
			return false;
		}
		if (requireMatchingAttackTarget)
		{
			WorldBossAttackTargetData attackTarget = worldBossModelManager.AttackTarget;
			if (attackTarget == null || !attackTarget.IsActive)
			{
				return false;
			}
			if (attackTarget.SeasonId != worldBossModelManager.GetCurrentSeasonId() || attackTarget.CycleId != worldBossModelManager.GetCurrentCycleId())
			{
				return false;
			}
		}
		return true;
	}
}
