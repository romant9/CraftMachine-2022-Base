using System;
using System.Collections;
using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class DetailMapPopUp : HUDElement
{
	[Serializable]
	public struct SeasonColorSettings
	{
		public string SeasonID;

		public Color TitleColor;

		public Color TitleGradientTop;

		public Color TitleGradientBottom;
	}

	[SerializeField]
	private GameObject missionViewPrefab;

	[SerializeField]
	private GameObject missionViewContainer;

	[SerializeField]
	private GameObject survivalMissionViewContainerOuter;

	[SerializeField]
	private GameObject survivalMissionViewContainerScrollable;

	[SerializeField]
	private GameObject survivalMapSlotPositioningReference;

	[SerializeField]
	private UILabel survivalDifficultyLabel;

	[SerializeField]
	private GameObject survivalDifficultyNormalContainer;

	[SerializeField]
	private GameObject survivalDifficultyHardContainer;

	[SerializeField]
	private GameObject survivalDifficultyNightmareContainer;

	[SerializeField]
	private GameObject survivalDifficultyNormalTableBackground;

	[SerializeField]
	private GameObject survivalDifficultyHardTableBackground;

	[SerializeField]
	private GameObject survivalDifficultyNightmareTableBackground;

	[SerializeField]
	private GameObject scrollItemPrefab;

	[SerializeField]
	private UIScrollView scrollContainer;

	[SerializeField]
	private GameObject scrollBarContainer;

	[SerializeField]
	private LocalizationUIUpdater scrollBarLabelLocalizationUpdater;

	[SerializeField]
	private ChallengeInfoPanel challengeInfoPanel;

	[SerializeField]
	private SurvivalInfoPanel survivalInfoPanel;

	[SerializeField]
	private GameObject seasonContainer;

	[SerializeField]
	private GameObject storyContainer;

	[SerializeField]
	private GameObject scrollBarCenter;

	[SerializeField]
	private UIGrid gridContainer;

	[SerializeField]
	private DetailedMapInfo detailedMapInfo;

	[SerializeField]
	private MapVideoContent videoContainer;

	[SerializeField]
	private UITexture hero;

	[SerializeField]
	private UILabel seasonName;

	[SerializeField]
	private List<SeasonColorSettings> seasonColorSettings;

	[SerializeField]
	private UISeasonProggressBar seasonProgressBar;

	[SerializeField]
	private UISeasonRewardIcon seasonRewardIcon;

	[SerializeField]
	private UILabel seasonEpisodeName;

	[SerializeField]
	private LimitedSeasonTimeOfferButton bundleButton;

	[SerializeField]
	private UISprite scrollBG;

	[SerializeField]
	private Color scrollBGStory;

	[SerializeField]
	private Color scrollBGSeason;

	[SerializeField]
	public GameObject seasonRewardPosition;

	[SerializeField]
	public FakeTrainingGroundsHudButton fakeRadioButton;

	[SerializeField]
	public FakeTrainingGroundsHudButton fakeSurvivorButton;

	private MissionView currentMissionView;

	private List<DetailMapScrollItem> scrollItems;

	private long NextSeasonCheckTimeStamp;

	private long SeasonCheckIntervalMilli = 1000L;

	private MapCategory lastCategory = MapCategory.Grind;

	private string lastSubcategory;

	private MapMissionModel lastPlayedMission;

	private Coroutine activeOpenApocalypticAfterDelayCoroutine;

	public MapMissionGroupModel CurrentMap { get; protected set; }

	public string CurrentSeason { get; protected set; }

	public MapCategory MapCategory => lastCategory;

	public ChallengeInfoPanel GetChallengeInfoPanel()
	{
		return challengeInfoPanel;
	}

	public SurvivalInfoPanel GetSurvivalInfoPanel()
	{
		return survivalInfoPanel;
	}

	public void OnClickExit()
	{
		UITypeOpenOnClose = UIType.MissionHubPopup;
		Close();
		EventManager.NotifyClick("Hub");
	}

	public void OnClickSelectSeason()
	{
		UITypeOpenOnClose = UIType.SelectSeasonPopup;
		Close();
	}

	public override void OnClickClose()
	{
		UITypeOpenOnClose = UIType.MissionHubPopup;
		base.OnClickClose();
	}

	private void CreateScrollItems(MapCategory category, string subcategory = null)
	{
		if (lastCategory == category && lastSubcategory == subcategory)
		{
			return;
		}
		lastCategory = category;
		lastSubcategory = subcategory;
		ClearScrollItems();
		scrollContainer.ResetPosition();
		scrollItems = new List<DetailMapScrollItem>();
		int num = 1;
		if (category == MapCategory.Season && subcategory != null)
		{
			num = GameManager.Instance.gameEconomyData.GetSeasonDefinition(subcategory).FirstEpisodeNumber;
		}
		MapMissionGroupModel mapMissionGroupModel = null;
		MissionHighlight missionHighlight = null;
		bool flag = false;
		List<MissionSpawnPointGroup> availableMapsByCategory = GameManager.Instance.gameEconomyData.GetAvailableMapsByCategory(category, subcategory);
		int count = availableMapsByCategory.Count;
		for (int i = 0; i < availableMapsByCategory.Count; i++)
		{
			MapMissionGroupModel missionGroupModelForSpawnPointGroup = GameManager.Instance.playerModel.MapContainerModel.GetMissionGroupModelForSpawnPointGroup(availableMapsByCategory[i]);
			DetailMapScrollItem component = scrollContainer.gameObject.AddChild(scrollItemPrefab).GetComponent<DetailMapScrollItem>();
			mapMissionGroupModel = missionGroupModelForSpawnPointGroup.GetCurrentEpisodeDifficultyGroupModel();
			if (!flag && mapMissionGroupModel != null)
			{
				missionHighlight = mapMissionGroupModel.NextFeaturedData;
				flag = missionHighlight != null;
				component.SetItem(mapMissionGroupModel, num.ToString(), missionHighlight);
			}
			else
			{
				component.SetItem(mapMissionGroupModel, num.ToString());
			}
			num++;
			scrollItems.Add(component);
		}
		if (count >= num)
		{
			for (int j = num; j < count; j++)
			{
				DetailMapScrollItem component2 = scrollContainer.gameObject.AddChild(scrollItemPrefab).GetComponent<DetailMapScrollItem>();
				component2.SetItem(null, j.ToString());
				component2.UpdateUI();
				scrollItems.Add(component2);
			}
		}
		gridContainer.enabled = true;
		StartCoroutine(DelayedScrollPositionReset());
	}

	public IEnumerator DelayedScrollPositionReset()
	{
		yield return new WaitForSeconds(0.1f);
		scrollContainer.ResetPosition();
	}

	public void ClearScrollItems()
	{
		if (scrollItems != null)
		{
			for (int i = 0; i < scrollItems.Count; i++)
			{
				UnityEngine.Object.Destroy(scrollItems[i].gameObject);
			}
			scrollItems.Clear();
		}
	}

	public override void Open()
	{
		GameManager.Instance.ShowTipsDone = true;
		base.Open();
		EventManager.NotifyEvent(EventManager.EventType.MapDetailMapShown);
	}

	public override void Update()
	{
		base.Update();
		if (NextSeasonCheckTimeStamp < GameManager.Instance.playerModel.UtcTimeStamp)
		{
			CheckSeasonTrialsReset();
		}
	}

	public void CheckSeasonTrialsReset()
	{
		NextSeasonCheckTimeStamp = GameManager.Instance.playerModel.UtcTimeStamp + SeasonCheckIntervalMilli;
		if (GameManager.Instance.gameEconomyData.GetFeature("UpdateSeasonTrials").Enabled && GameManager.Instance.playerModel.MapContainerModel.DoesSeasonTrialsNeedUpdate())
		{
			Helpers.ExecuteCommand(new UpdateSeasonTrialsCommand());
			UIEvent.Send("OnSeasonTrialsUpdated");
		}
	}

	public static void ReloadChallengeMap()
	{
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
		}
	}

	public static void ReloadSurvivalMap()
	{
		DetailMapPopUp detailMapPopUp = SingularityMonoBehaviour<HUDManager>.Instance.GetNoCreation(UIType.DetailMapPopUp) as DetailMapPopUp;
		if (detailMapPopUp != null)
		{
			detailMapPopUp.LoadSurvivalMap();
		}
	}

	public void UpdateMissionInfo()
	{
		if (detailedMapInfo != null)
		{
			detailedMapInfo.SetCurrentMissionGroup(CurrentMap);
			detailedMapInfo.UpdateUI();
		}
	}

	public void LoadEpisode()
	{
		if (CurrentMap == null && GameManager.Instance.playerModel.SurvivorContainer.StoryTeller.CurrentQuest is MissionQuest)
		{
			MapContainerModel mapContainerModel = GameManager.Instance.playerModel.MapContainerModel;
			lastPlayedMission = mapContainerModel.LastPlayedMissionModel;
			if (lastPlayedMission != null && lastPlayedMission.MissionSpawnPointGroup.EpisodeDifficultyLevel > 1)
			{
				MapMissionGroupModel missionGroupModelForSpawnPointGroup = mapContainerModel.GetMissionGroupModelForSpawnPointGroup(lastPlayedMission.MissionSpawnPointGroupId);
				if (missionGroupModelForSpawnPointGroup.AreAllStoryMissionsCompleted())
				{
					CurrentMap = mapContainerModel.GetHarderVersion(missionGroupModelForSpawnPointGroup);
				}
				else
				{
					CurrentMap = GameManager.Instance.playerModel.MapContainerModel.GetMissionGroupModelForSpawnPointGroup(lastPlayedMission.MissionSpawnPointGroupId);
				}
			}
			else
			{
				CurrentMap = ((MissionQuest)GameManager.Instance.playerModel.SurvivorContainer.StoryTeller.CurrentQuest).GetUnlockedEpisode();
			}
		}
		if (CurrentMap == null)
		{
			for (int i = 0; i < GameManager.Instance.playerModel.MapContainerModel.MapMissionGroups.Count; i++)
			{
				MapMissionGroupModel mapMissionGroupModel = GameManager.Instance.playerModel.MapContainerModel.MapMissionGroups[i];
				if (mapMissionGroupModel.MissionSpawnPointGroup != null && mapMissionGroupModel.MissionSpawnPointGroup.Category == MapCategory.Story && mapMissionGroupModel.AreAllStoryMissionsCompleted() && mapMissionGroupModel.MissionSpawnPointGroup.EpisodeDifficultyLevel == 1)
				{
					CurrentMap = mapMissionGroupModel.GetCurrentEpisodeDifficultyGroupModel();
				}
			}
		}
		if (CurrentMap == null)
		{
			Debug.LogError("CurrentMap null story, invalid configuration or missing map");
			Close();
		}
		LoadMap();
	}

	public void LoadSeason(string seasonId)
	{
		MapMissionGroupModel seasonCurrentMapMissionGroup = GetSeasonCurrentMapMissionGroup(GameManager.Instance.gameEconomyData.GetSeasonDefinition(seasonId));
		LoadSeason(seasonCurrentMapMissionGroup);
	}

	public void LoadSeason(MapMissionGroupModel map)
	{
		if (map.MissionSpawnPointGroup == null || map.MissionSpawnPointGroup.Category != MapCategory.Season)
		{
			Debug.LogError("Invalid parameter!");
			return;
		}
		CurrentMap = map;
		CurrentSeason = map.MissionSpawnPointGroup.Subcategory;
		LoadMap();
	}

	public static MapMissionGroupModel GetSeasonCurrentMapMissionGroup(SeasonDefinition season)
	{
		MapMissionGroupModel mapMissionGroupModel = null;
		List<MissionSpawnPointGroup> list = null;
		if (season != null)
		{
			list = GameManager.Instance.gameEconomyData.GetAllMapsInSeason(season);
		}
		for (int i = 0; i < GameManager.Instance.playerModel.MapContainerModel.MapMissionGroups.Count; i++)
		{
			MapMissionGroupModel mapMissionGroupModel2 = GameManager.Instance.playerModel.MapContainerModel.MapMissionGroups[i];
			MissionSpawnPointGroup missionSpawnPointGroup = mapMissionGroupModel2.MissionSpawnPointGroup;
			if (missionSpawnPointGroup != null && missionSpawnPointGroup.Category == MapCategory.Season && (list == null || list.Contains(missionSpawnPointGroup)))
			{
				if (mapMissionGroupModel == null)
				{
					mapMissionGroupModel = mapMissionGroupModel2;
				}
				if (mapMissionGroupModel2.IsFeaturedData != null)
				{
					return mapMissionGroupModel2;
				}
			}
		}
		return mapMissionGroupModel;
	}

	public void LoadEpisode(int episodeId)
	{
		CurrentMap = GameManager.Instance.playerModel.MapContainerModel.GetMissionGroupModelForSpawnPointGroup(episodeId);
		LoadMap();
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
		if (CurrentMap == null)
		{
			return;
		}
		UpdateSelectedItem();
		UpdateMissionInfo();
		scrollBarContainer.SetActive(!CurrentMap.IsWeeklyChallenge && !CurrentMap.IsInApocalyptiWeeklyChallenge && !CurrentMap.IsWeeklySurvival);
		if (CurrentMap.MissionSpawnPointGroup.Category == MapCategory.Survival)
		{
			Helpers.GameObjectSetActive(survivalMissionViewContainerOuter, value: true);
			WeeklySurvivalModel weeklySurvival = GameManager.Instance.playerModel.WeeklySurvival;
			SurvivalDifficulty survivalDifficulty = weeklySurvival?.CurrentDifficulty ?? SurvivalDifficulty.Normal;
			Helpers.GameObjectSetActive(survivalDifficultyHardContainer, survivalDifficulty == SurvivalDifficulty.Hard);
			Helpers.GameObjectSetActive(survivalDifficultyNormalContainer, survivalDifficulty < SurvivalDifficulty.Hard);
			Helpers.GameObjectSetActive(survivalDifficultyNightmareContainer, survivalDifficulty == SurvivalDifficulty.Nightmare);
			if (weeklySurvival.Finished && weeklySurvival.CanRestartMapOrDoubleRewards())
			{
				Helpers.GameObjectSetActive(survivalDifficultyNormalTableBackground, value: false);
				Helpers.GameObjectSetActive(survivalDifficultyHardTableBackground, value: false);
				Helpers.GameObjectSetActive(survivalDifficultyNightmareTableBackground, value: false);
			}
			else
			{
				Helpers.GameObjectSetActive(survivalDifficultyNormalTableBackground, value: true);
				Helpers.GameObjectSetActive(survivalDifficultyHardTableBackground, value: true);
				Helpers.GameObjectSetActive(survivalDifficultyNightmareTableBackground, value: true);
			}
			if (survivalDifficulty == SurvivalDifficulty.None)
			{
				HelpersUI.SetContentToLabel(survivalDifficultyLabel, "");
			}
			else
			{
				HelpersUI.SetContentToLabel(survivalDifficultyLabel, LocalizationManager.GetText("Survival.Difficulty." + Enum.GetName(typeof(SurvivalDifficulty), survivalDifficulty)));
			}
		}
		else
		{
			Helpers.GameObjectSetActive(survivalMissionViewContainerOuter, value: false);
		}
		scrollBarLabelLocalizationUpdater.LocalizationKey = ((CurrentMap.MissionSpawnPointGroup.Category == MapCategory.Season) ? "DetailMap.Popup.Season7.Title2" : "DetailMap.Popup.Story.Title2");
		if (CurrentMap.IsWeeklyChallenge || CurrentMap.IsInApocalyptiWeeklyChallenge)
		{
			challengeInfoPanel.UpdateUI();
		}
		Helpers.GameObjectSetActive(challengeInfoPanel, CurrentMap.IsWeeklyChallenge || CurrentMap.IsInApocalyptiWeeklyChallenge);
		if (CurrentMap.IsWeeklySurvival)
		{
			survivalInfoPanel.UpdateUI();
		}
		Helpers.GameObjectSetActive(survivalInfoPanel, CurrentMap.IsWeeklySurvival);
		seasonContainer.SetActive(CurrentMap.MissionSpawnPointGroup.Category == MapCategory.Season);
		storyContainer.SetActive(CurrentMap.MissionSpawnPointGroup.Category != MapCategory.Season && !CurrentMap.IsWeeklySurvival);
		if (CurrentMap.MissionSpawnPointGroup.Category == MapCategory.Season)
		{
			if (CurrentMap.MissionSpawnPointGroup != null)
			{
				HelpersGfx.SetSeasonHeroMaterial(hero, CurrentMap.MissionSpawnPointGroup.MapId);
			}
			SeasonDefinition seasonDefinitionForMap = GameManager.Instance.gameEconomyData.GetSeasonDefinitionForMap(CurrentMap.MissionSpawnPointGroup);
			if (seasonDefinitionForMap != null)
			{
				HelpersUI.SetContentToLabel(seasonName, HelpersLocalization.GetSeasonTitle(seasonDefinitionForMap.Id));
				for (int i = 0; i < seasonColorSettings.Count; i++)
				{
					if (seasonColorSettings[i].SeasonID == seasonDefinitionForMap.Id)
					{
						seasonName.color = seasonColorSettings[i].TitleColor;
						seasonName.gradientTop = seasonColorSettings[i].TitleGradientTop;
						seasonName.gradientBottom = seasonColorSettings[i].TitleGradientBottom;
					}
				}
				if (seasonProgressBar != null)
				{
					seasonProgressBar.SetSeason(seasonDefinitionForMap);
				}
				if (seasonRewardIcon != null)
				{
					seasonRewardIcon.UpdateUI(seasonDefinitionForMap);
				}
				if (videoContainer != null)
				{
					videoContainer.SetSeasonVideo(seasonDefinitionForMap.SeasonVideoUrl);
				}
			}
			HelpersUI.SetContentToLabel(seasonEpisodeName, HelpersLocalization.GetSeasonEpisodeName(CurrentMap.MissionSpawnPointGroup.MapId));
		}
		if (scrollBG != null)
		{
			if (CurrentMap.MissionSpawnPointGroup.Category == MapCategory.Season)
			{
				scrollBG.color = scrollBGSeason;
			}
			else
			{
				scrollBG.color = scrollBGStory;
			}
		}
		if (videoContainer != null)
		{
			videoContainer.Init(CurrentMap.MissionSpawnPointGroup);
		}
		if (bundleButton != null)
		{
			bundleButton.SetFeaturedData(CurrentMap.IsFeaturedData);
		}
		TriggerStarTweens();
	}

	private void TriggerStarTweens()
	{
		if (lastPlayedMission == null || !lastPlayedMission.IsLastInGroup || scrollItems == null)
		{
			return;
		}
		for (int i = 0; i < scrollItems.Count; i++)
		{
			if (scrollItems[i].EpisodeModel.MissionSpawnPointGroupId == CurrentMap.MissionSpawnPointGroupId)
			{
				scrollItems[i].TriggerTween();
			}
		}
		lastPlayedMission = null;
	}

	public void ToggleFakePhone(bool enable, RewardCurrency currency = null)
	{
		if (enable && currency != null && fakeSurvivorButton != null)
		{
			fakeRadioButton.Init(currency);
		}
	}

	public void ToggleFakeSurvivor(bool enable, RewardCurrency currency = null)
	{
		if (enable && currency != null && fakeSurvivorButton != null)
		{
			fakeSurvivorButton.Init(currency);
		}
	}

	public int GetSelectedIndex()
	{
		if (scrollItems == null)
		{
			return 0;
		}
		for (int i = 0; i < scrollItems.Count; i++)
		{
			if (scrollItems[i].IsSelected)
			{
				return i;
			}
		}
		return 0;
	}

	public void UpdateSelectedItem()
	{
		if (scrollItems == null)
		{
			return;
		}
		for (int i = 0; i < scrollItems.Count; i++)
		{
			if (scrollItems[i].EpisodeModel == null)
			{
				continue;
			}
			bool flag = scrollItems[i].EpisodeModel.MissionSpawnPointGroupId == CurrentMap.MissionSpawnPointGroupId;
			TWDModelManager modelManager = GameManager.Instance.modelManager;
			if (CurrentMap != null && CurrentMap.MissionSpawnPointGroup != null && CurrentMap.MissionSpawnPointGroup.Category == MapCategory.Season)
			{
				string mapId = CurrentMap.MissionSpawnPointGroup.MapId;
				if (flag && !modelManager.Blackboard.IsToggleOn("Toggle.Episode." + mapId + ".Seen"))
				{
					Helpers.ExecuteCommand(new SeasonEpisodeSeenCommand(CurrentMap.MissionSpawnPointGroup.MapId));
				}
			}
			scrollItems[i].SetSelected(flag);
		}
	}

	public void LoadApocalypticChallengeMap()
	{
		WeeklyChallengeModel weeklyChallengeModel = WeeklyChallengeHelper.GetWeeklyChallengeModel();
		ApocalypseWeeklyChallengeModel weeklyApocalypticChallengeModel = WeeklyChallengeHelper.GetWeeklyApocalypticChallengeModel();
		int num = -1;
		if (weeklyChallengeModel.Finished)
		{
			WeeklyChallenge nextWeeklyChallenge = weeklyChallengeModel.NextWeeklyChallenge;
			if (nextWeeklyChallenge != null)
			{
				num = nextWeeklyChallenge.ApocalypticMapId;
			}
		}
		else if (weeklyApocalypticChallengeModel.CurrentDefinition != null)
		{
			num = weeklyApocalypticChallengeModel.GetWeeklyConfigApocalypticMapId();
		}
		if (num == -1)
		{
			OnClickClose();
			return;
		}
		CurrentMap = GameManager.Instance.playerModel.MapContainerModel.GetMissionGroupModelForSpawnPointGroup(num);
		LoadMap();
		LoadApocalyptic();
	}

	public void LoadChallengeMap()
	{
		WeeklyChallengeModel weeklyChallengeModel = WeeklyChallengeHelper.GetWeeklyChallengeModel();
		int num = -1;
		if (weeklyChallengeModel.Finished)
		{
			WeeklyChallenge nextWeeklyChallenge = weeklyChallengeModel.NextWeeklyChallenge;
			if (nextWeeklyChallenge != null)
			{
				num = nextWeeklyChallenge.DetailMapId;
			}
		}
		else
		{
			WeeklyChallenge currentDefinition = weeklyChallengeModel.CurrentDefinition;
			if (currentDefinition != null)
			{
				num = currentDefinition.DetailMapId;
			}
		}
		if (num == -1)
		{
			OnClickClose();
			return;
		}
		CurrentMap = GameManager.Instance.playerModel.MapContainerModel.GetMissionGroupModelForSpawnPointGroup(num);
		LoadMap();
	}

	public void LoadSurvivalMap()
	{
		WeeklySurvivalModel weeklySurvivalModel = WeeklySurvivalHelper.GetWeeklySurvivalModel();
		int num = -1;
		if (weeklySurvivalModel.Finished)
		{
			WeeklySurvival nextWeeklySurvival = weeklySurvivalModel.NextWeeklySurvival;
			if (nextWeeklySurvival != null)
			{
				num = nextWeeklySurvival.DetailMapId;
			}
		}
		else
		{
			WeeklySurvival currentDefinition = weeklySurvivalModel.CurrentDefinition;
			if (currentDefinition != null)
			{
				num = currentDefinition.DetailMapId;
			}
		}
		if (num == -1)
		{
			OnClickClose();
			return;
		}
		CurrentMap = GameManager.Instance.playerModel.MapContainerModel.GetMissionGroupModelForSpawnPointGroup(num);
		LoadMap();
	}

	public void CenterScrollableMapToNormalizedMapPosition(float normalizedPos)
	{
		if (survivalMissionViewContainerOuter == null || survivalMissionViewContainerScrollable == null)
		{
			return;
		}
		UIPanel component = survivalMissionViewContainerOuter.GetComponent<UIPanel>();
		if (component == null)
		{
			return;
		}
		UIScrollView component2 = survivalMissionViewContainerOuter.GetComponent<UIScrollView>();
		if (component2 == null)
		{
			return;
		}
		UIWidget component3 = survivalMissionViewContainerScrollable.GetComponent<UIWidget>();
		if (!(component3 == null))
		{
			float num = (component2.restrictToCustomBounds ? component2.customBoundsForRestrict : component3.CalculateBounds()).extents.x * 2f;
			float num2 = 0f;
			if (num > 0f)
			{
				num2 = component.width / num;
			}
			float value = 0.5f + (normalizedPos - 0.5f) * (1f + num2);
			value = UtilsMath.Clamp(value, 0f, 1f);
			component.ResetAndUpdateAnchors();
			component2.SetDragAmount(value, 0f, updateScrollbars: false);
			component2.RestrictWithinBounds(instant: true);
			component2.UpdateScrollbars();
		}
	}

	public void CenterScrollableMapOnCurrentMission()
	{
		if (GameManager.Instance != null && GameManager.Instance.playerModel != null && GameManager.Instance.playerModel.WeeklySurvival != null)
		{
			WeeklySurvivalModel weeklySurvival = GameManager.Instance.playerModel.WeeklySurvival;
			float scrollableMapMissionPosition = currentMissionView.GetScrollableMapMissionPosition(weeklySurvival.NextMissionOrderNumber);
			CenterScrollableMapToNormalizedMapPosition(scrollableMapMissionPosition);
		}
	}

	private void LoadMap()
	{
		if (CurrentMap != null)
		{
			MapCategory category = CurrentMap.MissionSpawnPointGroup.Category;
			string subcategory = ((category == MapCategory.Season) ? CurrentSeason : null);
			CreateScrollItems(category, subcategory);
			UpdateSelectedItem();
			if (currentMissionView != null)
			{
				UnityEngine.Object.Destroy(currentMissionView.gameObject);
			}
			GameObject parent = missionViewContainer;
			UIWidget scrollableMapSlotReference = null;
			if (category == MapCategory.Survival)
			{
				parent = survivalMissionViewContainerScrollable;
				scrollableMapSlotReference = survivalMapSlotPositioningReference.GetComponent<UITexture>();
			}
			currentMissionView = parent.AddChild(missionViewPrefab).GetComponent<MissionView>();
			lastPlayedMission = GameManager.Instance.playerModel.MapContainerModel.LastPlayedMissionModel;
			currentMissionView.LoadMap(CurrentMap, scrollableMapSlotReference);
			if (category == MapCategory.Survival)
			{
				CenterScrollableMapOnCurrentMission();
			}
			UpdateUI();
			ShowTutorialDetailMap(CurrentMap);
		}
	}

	public MissionView GetCurrentMissionView()
	{
		return currentMissionView;
	}

	private void ShowTutorialDetailMap(MapMissionGroupModel mapMissionGroupModel)
	{
		if (OfflineManager.IsLoadDataManager && !OfflineManager.IsUseServices) return;
		if (!TutorialView.Instance.Model.HasCompletedPart("HarderEpisodeDetail") && mapMissionGroupModel != null && mapMissionGroupModel.MissionSpawnPointGroup != null && mapMissionGroupModel.MissionSpawnPointGroup.EpisodeDifficultyLevel > 1 && !GameManager.Instance.playerModel.MapContainerModel.HasCompletedHarderEpisodeMission())
		{
			TutorialView.Instance.StartPart("HarderEpisodeDetail");
		}
	}

	public override void Close()
	{
		if (CurrentMap != null && (CurrentMap.IsWeeklyChallenge || CurrentMap.IsInApocalyptiWeeklyChallenge || CurrentMap.IsWeeklySurvival))
		{
			CurrentMap = null;
		}
		CampHUD.CurrencyHudSetActive(enable: true);
		base.Close();
	}

	private void LoadApocalyptic()
	{
		if (activeOpenApocalypticAfterDelayCoroutine != null)
		{
			StopCoroutine(activeOpenApocalypticAfterDelayCoroutine);
		}
		activeOpenApocalypticAfterDelayCoroutine = SingularityMonoBehaviour<HUDManager>.Instance.StartCoroutine(OpenApocalypticAfterDelay());
	}

	private IEnumerator OpenApocalypticAfterDelay()
	{
		while (SingularityMonoBehaviour<HUDManager>.Instance.IsActive(UIType.OpenLootInUi) || SingularityMonoBehaviour<HUDManager>.Instance.IsActive(UIType.OpenApocalypticLootInUi) || SingularityMonoBehaviour<HUDManager>.Instance.IsActive(UIType.WeeklyChallengeNextCycle) || SingularityMonoBehaviour<HUDManager>.Instance.IsActive(UIType.ApocalypticWeeklyChallengeStartSkipping) || GameManager.Instance.playerModel.WeeklyChallenge.CanCollectApocalypticRewards)
		{
			yield return null;
		}
		yield return null;
		OpenApocalypticLootInUi.TryOpenOnLootEnter();
		while (SingularityMonoBehaviour<HUDManager>.Instance.IsActive(UIType.OpenLootInUi) || SingularityMonoBehaviour<HUDManager>.Instance.IsActive(UIType.OpenApocalypticLootInUi))
		{
			yield return null;
		}
		PopupSurvivalApocalypticDifficulty.TryOpenOnChallengeEnter();
		activeOpenApocalypticAfterDelayCoroutine = null;
	}
}
