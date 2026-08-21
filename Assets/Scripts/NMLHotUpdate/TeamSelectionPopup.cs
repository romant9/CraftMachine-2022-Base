using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BaseModel;
using Client.Camp;
using Client.Connectivity;
using TWDModel;
using UnityEngine;
using TwdCustomMod;
using NextGames.Sdk.AssetBundleManager;

public class TeamSelectionPopup : HUDElement, ISurvivorSlotProvider
{
	[SerializeField]
	private GameObject loadingContainer;

	public const string EventCloseSelectionPanel = "EventCloseSelectionPanel";

	public const string EventSurvivorReplaced = "EventSurvivorReplaced";

	public const string EventAnimationFinished = "EventAnimationFinished";

	[SerializeField]
	[Tooltip("Prefab of the panel to select the survivor.")]
	private GameObject teamSelectionSurvivorsListPanelPrefab;

	private TeamSelectionSurvivorsListPanel teamSelectionSurvivorsListPanel;

	[SerializeField]
	[Tooltip("The panel of the selected team.")]
	private TeamSelectionSelectedSurvivorPanel teamSelectionSelectedSurvivorPanel;

	[Header("Type of infos")]
	[SerializeField]
	private GameObject CombatInfo;

	[Header("WorldBoss PVE infos")]
	[SerializeField]
	private GameObject worldBossPVEPlayButton;

	[SerializeField]
	private UILabel worldBossPVEPlayButtonTxt;

	[SerializeField]
	private GameObject worldBossPVEPlayButtonDisabled;

	[SerializeField]
	private UILabel worldBossPVEPlayButtonDisabledTxt;

	[SerializeField]
	private GameObject worldBossPVEInfo;

	[SerializeField]
	private UILabel worldBossTitle;

	[SerializeField]
	private UILabel worldBossDifficultyDescription;

	[SerializeField]
	private UILabel worldBossDescription;

	[SerializeField]
	private GameObject worldBossPVPInfo;

	[SerializeField]
	private UILabel worldBossPVPEnemyName;

	[SerializeField]
	private GameObject CombatOutpostInfo;

	[SerializeField]
	private GameObject OutpostInfo;

	[SerializeField]
	private UILabel outpostMessage;

	[Header("Combat Info")]
	[SerializeField]
	private UILabel missionTypeLabel;

	[SerializeField]
	private UILabel missionNameLabel;

	[SerializeField]
	private UILabel missionLevelLabel;

	[SerializeField]
	private UILabel missioFlavorLabel;

	[SerializeField]
	private UILabel missionBriefingLabel;

	[SerializeField]
	private UILabel missionNoRegularSurvivorsLabel;

	[SerializeField]
	private GameObject startButton;

	[SerializeField]
	private GameObject payGold;

	[SerializeField]
	private PayButton challengeStartButton;

	[SerializeField]
	private UIButton challengeStartUIButton;

	[SerializeField]
	private GameObject startButtonLocked;

	[SerializeField]
	private GameObject starsContainer;

	[SerializeField]
	private UISprite[] stars;

	[SerializeField]
	private UISprite difficultyBg;

	[SerializeField]
	private Color[] difficultyColors;

	[SerializeField]
	private UISprite tankWalkers;

	[SerializeField]
	private UISprite armoredWalkers;

	[SerializeField]
	private UISprite normalWalkers;

	[SerializeField]
	private UISprite burningWalkers;

	[SerializeField]
	private UISprite explosiveWalkers;

	[SerializeField]
	private UISprite gooWalkers;

	[SerializeField]
	private UISprite spikedWalkers;

	[SerializeField]
	private UISprite metalheadWalkers;

	[SerializeField]
	private UISprite fastWalkers;

	[SerializeField]
	private UISprite explosiveBarrels;

	[SerializeField]
	private UISprite randomWalkers;

	[SerializeField]
	private UISprite raiders;

	[SerializeField]
	private UISprite commonwealthWalkers;

	[SerializeField]
	private UIGrid enemiesContainer;

	[SerializeField]
	private GameObject enemyTypesContainer;

	[Space(10f)]
	[SerializeField]
	[Tooltip("Container containing all missions locked stuff.")]
	private GameObject missionLockedContainer;

	[SerializeField]
	[Tooltip("Container containing all missions unlocked stuff.")]
	private GameObject missionUnlockedContainer;

	[SerializeField]
	[Tooltip("Label telling you that the mission is locked and you need a better survivor level.")]
	private UILabel missionLockedLabel;

	[SerializeField]
	private GameObject iconAlert;

	[SerializeField]
	private GameObject deadlyMissionContainer;

	[SerializeField]
	private GameObject outpostContainer;

	[SerializeField]
	private OutpostDetailsPanelMatchMaking outpostMatchInfoNormal;

	[SerializeField]
	private OutpostDetailsPanelMatchMaking outpostMatchInfoLimited;

	[Header("Survival mode specific")]
	[SerializeField]
	private GameObject survivalModeRestContainer;

	[SerializeField]
	private PayButton survivalModeRestButton;

	[SerializeField]
	private int survivalMillisecondsToAnimateRest = 3000;

	[Header("Special Team Label")]
	[SerializeField]
	private GameObject SpecialTeamLabel;

	[SerializeField]
	private GameObject RegularTitleLabel;

	[SerializeField]
	private GameObject closeButton;

	[SerializeField]
	private GameObject featuredHeroCallContainer;

	public MatchInfo OutpostMatchInfo;

	public string OutpostDefenderName;

	public string OutpostDefenderHashedId;

	[Header("Guild Battle Enemy Info")]
	[SerializeField]
	private GuildBattleMissionInfoEnemyPlayer guildBattleEnemyInfo;

	[SerializeField]
	private GameObject activeBonusesGameObject;

	[SerializeField]
	private GuildBattleActiveBonusList activeBonusesList;

	[Header("Guild Battle Rewards")]
	[SerializeField]
	private GvGRewardsUI guildBatleRewards;

	[SerializeField]
	private TeamSelectSupportsView teamSelectSupportsView;

	[SerializeField]
	private TeamPresetSelectionPanel teamPresetSelectionPanel;

	[SerializeField]
	private UITable playButtonTable;

	private int survivorToReplaceIndex;

	private bool survivorBeingAddedToTheTeam;

	[SerializeField]
	private GameObject[] gvgDefendersContainers;

	private IMapMissionModel _selectedMapMissionModel;

	private MapMissionModel _mapMissionModel;

	private GuildBattleMapMissionModel _guildBattleMapMissionModel;

	private Cashier startCashier;

	private Cashier survivalRestCashier;

	private bool animatingSurvivalRest;

	private float survivalRestAnimationTime;

	private List<SurvivorModel> survivorsAnimatingSurvivalRest = new List<SurvivorModel>();

	private List<SurvivorModel> survivorsAnimatedSurvivalRest = new List<SurvivorModel>();

	private bool loadingCombat;

	private readonly Dictionary<int, int> worldBossAttackReceiptCodes = new Dictionary<int, int>();

	private int lastCompletedWorldBossAttackSequenceId = -1;

	private SignalRClient worldBossAttackReceiptClient;

	private List<SurvivorModel> lastRestedSurvivorsList;

	private int shownGvGMissionCompletion;

	public SurvivorContainerModel.SurvivorType SurvivorType { get; set; }

	public string WorldBossCapturePoint { get; set; }

	public string WorldBossCell { get; set; }

	public WorldBossPVPItemItem.CellState WorldBossCellState { get; set; }

	public List<string> WorldBossOccupyingSurvivorIds { get; set; }

	public string WorldBossOccupyingPlayerName { get; set; }

	private IMapMissionModel selectedMapMissionModel
	{
		get
		{
			if (_selectedMapMissionModel == null && IsModelRequestedType<MapMissionModel>())
			{
				_selectedMapMissionModel = GetModel<MapMissionModel>();
			}
			else if (_selectedMapMissionModel == null && IsGroupModelRequestedType<GuildBattleMapMissionModel>())
			{
				_selectedMapMissionModel = GetGroupModel<GuildBattleMapMissionModel>();
			}
			return _selectedMapMissionModel;
		}
	}

	private MissionData missionData
	{
		get
		{
			if (mapMissionModel != null)
			{
				return mapMissionModel.MissionData;
			}
			return null;
		}
	}

	private MapMissionModel mapMissionModel
	{
		get
		{
			if (_mapMissionModel == null && IsModelRequestedType<MapMissionModel>())
			{
				_mapMissionModel = GetModel<MapMissionModel>();
			}
			return _mapMissionModel;
		}
	}

	public GuildBattleMapMissionModel guildBattleMapMissionModel
	{
		get
		{
			if (_guildBattleMapMissionModel == null && IsGroupModelRequestedType<GuildBattleMapMissionModel>())
			{
				_guildBattleMapMissionModel = GetGroupModel<GuildBattleMapMissionModel>();
			}
			return _guildBattleMapMissionModel;
		}
	}

	public SupportSelectionPanel SupportSelectionPanel => teamSelectionSurvivorsListPanel.GetComponent<SupportSelectionPanel>();

	public Transform SelectedSlotPosition => teamSelectionSelectedSurvivorPanel.GetSelectedCard();

	public Transform FirstSlotPosition => teamSelectionSelectedSurvivorPanel.GetSlotAt(0);

	private void Awake()
	{
		if (SignalRClient.Instance != null)
		{
			SignalRClient.Instance.OnWorldBossFullSnapshotMessage -= OnWorldBossFullSnapshotChanged;
			SignalRClient.Instance.OnWorldBossFullSnapshotMessage += OnWorldBossFullSnapshotChanged;
		}
	}

	private void OnDestroy()
	{
		CancelWorldBossAttackReceiptWait();
		if (SignalRClient.Instance != null)
		{
			SignalRClient.Instance.OnWorldBossFullSnapshotMessage -= OnWorldBossFullSnapshotChanged;
		}
	}

	private void OnEnable()
	{
		if (OfflineManager.IsLoadDataManager)
		{
			DebugTWD.LogMycode("if (OfflineManager.IsLoadDataManager)");

			var area = teamSelectionSelectedSurvivorPanel.transform.parent;
			var tween = area.gameObject.GetComponent<TweenPosition>();

			if (!tween)
			{
				var offset = new Vector3(25, 10, 0);
				RegularTitleLabel.transform.localPosition += offset;
				var areaPos = area.localPosition;

				area.gameObject.AddComponent<TweenPosition>().duration = .2f;
				tween = area.gameObject.GetComponent<TweenPosition>();
				tween.to = areaPos;
				tween.from = tween.to + new Vector3(85, 0, 0);
				TeamSelectionTweenFromPos = areaPos;
			}

			if (tween.transform.localPosition != TeamSelectionTweenFromPos)
			{
				tween.transform.localPosition += TeamSelectionTweenFromPos;
			}
			if (tween.value != tween.to) tween.PlayForward();
			TeamSelectionTween = tween;
		}

		if (SurvivorType == SurvivorContainerModel.SurvivorType.Outpost)
		{
			Helpers.GameObjectSetActive(closeButton, value: false);
		}
		else
		{
			Helpers.GameObjectSetActive(closeButton, value: true);
		}
		survivorToReplaceIndex = -1;
		survivorBeingAddedToTheTeam = false;
		UIEvent.OnUIEvent += OnUIEvent;
		SingularityMonoBehaviour<LocalizationManager>.Instance.OnLocalizationLanguageChanged += OnLocalizationLanguageChanged;
		GameManager.Instance.playerModel.Changed += OnPlayerChange;
		TimedBonusModel timedBonus = GameManager.Instance.playerModel.GetTimedBonus(TimedBonusType.UnlimitedGas);
		if (timedBonus != null)
		{
			timedBonus.Changed += OnUnlimitedGasModelChange;
		}
		int count = GameManager.Instance.playerModel.SurvivorContainer.Survivors.Count;
		for (int i = 0; i < count; i++)
		{
			GameManager.Instance.playerModel.SurvivorContainer.Survivors[i].Changed += OnSurvivorChanged;
		}
		loadingCombat = false;
		if (SurvivorType == SurvivorContainerModel.SurvivorType.CombatGuildBattle)
		{
			SubscribeForGuildModelEvents();
		}
	}

	private void SubscribeForGuildModelEvents()
	{
		if (guildBattleMapMissionModel != null)
		{
			guildBattleMapMissionModel.Changed -= OnMissionChange;
			guildBattleMapMissionModel.Changed += OnMissionChange;
		}
		GuildBattleMapModel currentMapModel = GuildWarHelper.GetCurrentMapModel();
		if (currentMapModel != null)
		{
			currentMapModel.Changed -= OnMapModelChange;
			currentMapModel.Changed += OnMapModelChange;
		}
	}

	private void OnDisable()
	{
		if (OfflineManager.IsLoadDataManager)
		{
			TeamSelectionTween = null;
		}
		CancelWorldBossAttackReceiptWait();
		UIEvent.OnUIEvent -= OnUIEvent;
		GameManager.Instance.playerModel.Changed -= OnPlayerChange;
		SingularityMonoBehaviour<LocalizationManager>.Instance.OnLocalizationLanguageChanged -= OnLocalizationLanguageChanged;
		TimedBonusModel timedBonus = GameManager.Instance.playerModel.GetTimedBonus(TimedBonusType.UnlimitedGas);
		if (timedBonus != null)
		{
			timedBonus.Changed -= OnUnlimitedGasModelChange;
		}
		int count = GameManager.Instance.playerModel.SurvivorContainer.Survivors.Count;
		for (int i = 0; i < count; i++)
		{
			GameManager.Instance.playerModel.SurvivorContainer.Survivors[i].Changed -= OnSurvivorChanged;
		}
		if (guildBattleMapMissionModel != null)
		{
			guildBattleMapMissionModel.Changed -= OnMissionChange;
		}
		GuildBattleMapModel currentMapModel = GuildWarHelper.GetCurrentMapModel();
		if (currentMapModel != null)
		{
			currentMapModel.Changed -= OnMapModelChange;
		}
		model = null;
		groupModelChild = null;
		_selectedMapMissionModel = null;
		_guildBattleMapMissionModel = null;
		_mapMissionModel = null;
		if (teamSelectionSelectedSurvivorPanel != null)
		{
			teamSelectionSelectedSurvivorPanel.ClearMissionData();
		}
	}

	private int GetMaxTeamSize(MissionData missionData)
	{
		int num = missionData?.MaxTeamSize ?? 3;
		if (SurvivorType == SurvivorContainerModel.SurvivorType.CombatSurvival)
		{
			int numSurvivorsAvailableForAction = GameManager.Instance.playerModel.SurvivorContainer.SurvivalCharacters.GetNumSurvivorsAvailableForAction();
			if (numSurvivorsAvailableForAction < num)
			{
				num = numSurvivorsAvailableForAction;
			}
		}
		return num;
	}

	private void UpdateSurvivalRestContainer()
	{
		if (!(survivalModeRestContainer != null))
		{
			return;
		}
		if (SurvivorType == SurvivorContainerModel.SurvivorType.CombatSurvival && CanRest())
		{
			survivalModeRestContainer.SetActive(value: true);
			if (survivalModeRestButton != null)
			{
				survivalModeRestButton.UpdateUI(survivalRestCashier);
			}
		}
		else
		{
			survivalModeRestContainer.SetActive(value: false);
		}
	}

	public override void Open()
	{
		animatingSurvivalRest = false;
		survivorsAnimatingSurvivalRest.Clear();
		survivorsAnimatedSurvivalRest.Clear();
		if ((bool)CampView.Instance && (bool)CampView.Instance.CampViewBuildings)
		{
			CampView.Instance.CampViewBuildings.UnselectBuilding();
		}
		SetCashier();
		teamSelectionSelectedSurvivorPanel.SurvivorType = SurvivorType;
		UpdateSurvivalRestContainer();
		lastRestedSurvivorsList = null;
		outpostContainer.SetActive(SurvivorType == SurvivorContainerModel.SurvivorType.Outpost);
		PlayerModel playerModel = GameManager.Instance.playerModel;
		if ((SurvivorType == SurvivorContainerModel.SurvivorType.Combat || SurvivorType == SurvivorContainerModel.SurvivorType.CombatSurvival || SurvivorType == SurvivorContainerModel.SurvivorType.CombatGuildBattle) && teamSelectionSelectedSurvivorPanel != null && mapMissionModel != null)
		{
			MissionData missionData = mapMissionModel.MissionData;
			List<SurvivorModel> combatSurvivors = playerModel.SurvivorContainer.CombatSurvivors;
			if ((missionData.ExtraData == null || !missionData.ExtraData.InUse || missionData.ExtraData.PlayableSurvivors == null || missionData.ExtraData.PlayableSurvivors.Count <= 0) && playerModel.SurvivorContainer.CanRestoreCombatTeam(SurvivorType))
			{
				Helpers.ExecuteCommand(new RestoreSavedCombatTeamCommand(SurvivorType));
			}
			bool disableOutpostHeroLimits = GameManager.Instance.gameEconomyData.ConfigData.DisableOutpostHeroLimits;
			if (SurvivorType != SurvivorContainerModel.SurvivorType.CombatSurvival)
			{
				for (int num = combatSurvivors.Count - 1; num >= 0; num--)
				{
					if ((!disableOutpostHeroLimits && playerModel.SurvivorContainer.OutpostDefendingSurvivors.Contains(combatSurvivors[num])) || !EndlessModeHelpers.IsSurvivorAvailableForCombat(combatSurvivors[num], mapMissionModel))
					{
						Helpers.ExecuteCommand(new RemoveSurvivorFromCombatTeamCommand(combatSurvivors[num])
						{
							SurvivorType = SurvivorType
						});
					}
				}
			}
			while (combatSurvivors.Count > GetMaxTeamSize(missionData))
			{
				Helpers.ExecuteCommand(new RemoveSurvivorFromCombatTeamCommand(combatSurvivors[combatSurvivors.Count - 1])
				{
					SurvivorType = SurvivorType
				});
			}
			teamSelectionSelectedSurvivorPanel.SetMissionData(missionData);
			if (missionData.HasCivilianActorIdContaining("daryl"))
			{
				for (int num2 = combatSurvivors.Count - 1; num2 >= 0; num2--)
				{
					if (combatSurvivors[num2].ActorDefinitionID.ToLowerInvariant().Contains("daryl"))
					{
						Helpers.ExecuteCommand(new RemoveSurvivorFromCombatTeamCommand(combatSurvivors[num2])
						{
							SurvivorType = SurvivorType
						});
					}
				}
			}
		}
		else if (playerModel.SurvivorContainer.CanRestoreCombatTeam(SurvivorContainerModel.SurvivorType.Combat))
		{
			Helpers.ExecuteCommand(new RestoreSavedCombatTeamCommand(SurvivorContainerModel.SurvivorType.Combat));
		}
		if (IsWorldBossSurvivorType())
		{
			RemoveUnavailableWorldBossPVEHeroes(playerModel);
		}
		if (teamSelectionSelectedSurvivorPanel != null)
		{
			teamSelectionSelectedSurvivorPanel.UpdateSlots();
		}
		if (guildBattleMapMissionModel != null)
		{
			shownGvGMissionCompletion = guildBattleMapMissionModel.CompletionAmount;
			DebugTWD.Log("GuildBattleMapMissionModel is: " + guildBattleMapMissionModel.Id, DebugType.Wars);
		}
		base.Open();
		Helpers.GameObjectSetActive(loadingContainer, value: false);
		if (teamSelectionSurvivorsListPanel == null)
		{
			teamSelectionSurvivorsListPanel = Helpers.InstantiateToParentAndLayer(teamSelectionSurvivorsListPanelPrefab, base.gameObject).GetComponent<TeamSelectionSurvivorsListPanel>();
			teamSelectionSurvivorsListPanel.SurvivorSlotProvider = this;
			teamSelectionSelectedSurvivorPanel.TeamSelectionSurvivorsList = teamSelectionSurvivorsListPanel;
			teamSelectionSelectedSurvivorPanel.TeamSelectionSurvivorsList.SetCurrentTeam(teamSelectionSelectedSurvivorPanel.GetCurrentTeam());
		}
		teamSelectionSurvivorsListPanel.SurvivorType = SurvivorType;
		teamSelectionSurvivorsListPanel.gameObject.SetActive(false);

		UpdateUI();
		GetComponent<WeeklyClassEventPanel>().Init(mapMissionModel, SurvivorType);
		var eventWeeklyChallengeActivityPanel = GetComponent<EventWeeklyChallengeActivityPanel>();
		if (eventWeeklyChallengeActivityPanel) eventWeeklyChallengeActivityPanel.Init(mapMissionModel);
		if (!OfflineManager.IsTutorialDisable) TutorialView.Instance.UpdateSuggestion();
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/view_change");
		teamSelectSupportsView.OnPopupActive(teamSelectionSelectedSurvivorPanel.GetCurrentTeamSize(), SurvivorType == SurvivorContainerModel.SurvivorType.Outpost || SurvivorType == SurvivorContainerModel.SurvivorType.GvGDefenders, mapMissionModel);
		teamPresetSelectionPanel.RefreshShowState(SurvivorType, mapMissionModel);
	}

	private void OnUnlimitedGasModelChange(ModelObject modelObject, string changed, object args)
	{
		SetCashier();
		UpdateUI();
	}

	private void SetCashier()
	{
		if (SurvivorType == SurvivorContainerModel.SurvivorType.Combat || SurvivorType == SurvivorContainerModel.SurvivorType.CombatSurvival || SurvivorType == SurvivorContainerModel.SurvivorType.CombatGuildBattle)
		{
			MapMissionModel obj = mapMissionModel;
			if (obj != null && obj.MissionSpawnPointGroup.Category == MapCategory.Endless && EndlessModeHelpers.IsEndlessExpertMode())
			{
				startCashier = selectedMapMissionModel.GetStartMissionExpertModeCashier(GameManager.Instance.modelManager);
			}
			else
			{
				startCashier = selectedMapMissionModel.GetStartMissionCashier(GameManager.Instance.modelManager);
			}
		}
		if (SurvivorType == SurvivorContainerModel.SurvivorType.CombatSurvival)
		{
			survivalRestCashier = GameManager.Instance.playerModel.SurvivorContainer.SurvivalCharacters.GetPurchaseRestCashier();
		}
		else
		{
			survivalRestCashier = null;
		}
	}

	private void ShowSelectSurvivorPanel()
	{
		teamSelectionSurvivorsListPanel.gameObject.SetActive(value: true);
		teamSelectionSurvivorsListPanel.OpenPanel(missionData, mapMissionModel);
	}

	private void OnSurvivorChanged(ModelObject modelObject, string changed, object args)
	{
		if (changed == "ActionFinishedEvent" || changed == "ActionStartEvent")
		{
			UpdateUI();
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/survivor_heal");
		}
	}

	private void OnPlayerChange(ModelObject m, string changed, object args)
	{
		if (changed == "currencyChangedEvent")
		{
			UpdateStartButton();
		}
	}

	private void OnUIEvent(string type, object parameter)
	{
		if (type == "OnNewSurvivorSelected" && !survivorBeingAddedToTheTeam && (TutorialView.Allowed("SurvivorCard") || TutorialView.Allowed("SurvivorCardReserve") || TutorialView.Allowed("HeroSlot") || TutorialView.Allowed("HeroSelect_Hero_Daryl")))
		{
			if (survivorToReplaceIndex == -1)
			{
				if (parameter is SurvivorModel)
				{
					SurvivorModel survivorModel = parameter as SurvivorModel;
					if (!OfflineManager.IsTutorialDisable)
					{
						if (!TutorialView.Instance.Model.StaticTutorialComplete && survivorModel.SurvivorClass == SurvivorClass.Bruiser)
						{
							HUDNotification.Error(LocalizationManager.GetText("Popup.UpgradeSurvivor.TutorialCantSwitchBruiser"));
							return;
						}
					}
					survivorToReplaceIndex = teamSelectionSelectedSurvivorPanel.GetSurvivorIndex(survivorModel);
					teamSelectionSurvivorsListPanel.IncludeTeamSurvivors = true;
				}
				else
				{
					survivorToReplaceIndex = (int)parameter;
					teamSelectionSurvivorsListPanel.IncludeTeamSurvivors = false;
				}
				if (survivorToReplaceIndex >= 0)
				{
					ShowSelectSurvivorPanel();
					teamSelectionSelectedSurvivorPanel.SurvivorSelected(survivorToReplaceIndex);
				}
				return;
			}
			SurvivorModel survivorModel2 = parameter as SurvivorModel;
			if (survivorModel2 == null)
			{
				return;
			}
			if (SurvivorType == SurvivorContainerModel.SurvivorType.Outpost)
			{
				if (survivorModel2.IsUpgrading())
				{
					HUDNotification.Error(LocalizationManager.GetText("Popup.TeamSelection.Outpost.CannotSelectUpgrading"));
					SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/invalid_action");
					return;
				}
				if (survivorModel2.InjuryType != InjuryType.None)
				{
					HUDNotification.Error(LocalizationManager.GetText("Popup.TeamSelection.Outpost.CannotSelectHealing"));
					SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/invalid_action");
					return;
				}
			}
			else if (SurvivorType != SurvivorContainerModel.SurvivorType.CombatSurvival)
			{
				if (SurvivorType == SurvivorContainerModel.SurvivorType.GvGDefenders)
				{
					GvgDefendersTeamSelection component = GetComponent<GvgDefendersTeamSelection>();
					if (component.HasSurvivorAsDefender(survivorModel2))
					{
						string text = LocalizationManager.GetText("Popup.TeamSelect.GvgDefenders.SurvivorInAnotherTeam.Description{Parameters}", survivorModel2.Name, component.GetTeamForSurvivorAsLetter(survivorModel2), teamSelectionSelectedSurvivorPanel.GetSurvivorAtIndex(survivorToReplaceIndex).Name, survivorModel2.Name, component.GetTeamForSurvivorAsLetter(survivorModel2));
						if (component.GetCurrentSelectedTeam().Contains(survivorModel2))
						{
							SwapSurvivorSlotsInTeam(survivorModel2, survivorToReplaceIndex);
							return;
						}
						ConfirmationPopup.ShowPopup(LocalizationManager.GetText("Popup.TeamSelect.GvgDefenders.SurvivorInAnotherTeam.Title"), text, LocalizationManager.GetText("Button.Continue"), delegate
						{
							ReplaceCurrentSurvivorWith(survivorModel2);
						}, LocalizationManager.GetText("Button.Cancel"), delegate
						{
						});
						return;
					}
				}
				else if (!GameManager.Instance.gameEconomyData.ConfigData.DisableOutpostHeroLimits && GameManager.Instance.playerModel.SurvivorContainer.OutpostDefendingSurvivors.Contains(survivorModel2))
				{
					HUDNotification.Error(LocalizationManager.GetText("Popup.TeamSelection.CannotSelectDefender"));
					SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/invalid_action");
					return;
				}
			}
			if (!GetSurvivorsForType(SurvivorType).Contains(survivorModel2))
			{
				ReplaceCurrentSurvivorWith(survivorModel2);
			}
			else
			{
				SwapSurvivorSlotsInTeam(survivorModel2, survivorToReplaceIndex);
			}
		}
		else if (type == "EventAnimationFinished")
		{
			survivorToReplaceIndex = -1;
			survivorBeingAddedToTheTeam = false;
			if (!OfflineManager.IsTutorialDisable) TutorialView.Instance.UpdateSuggestion();
			UpdateUI();
		}
		else if (type == "SurvivorCardEquipmentClicked" && TutorialView.Allowed("Equipment"))
		{
			EquipmentButton equipmentButton = parameter as EquipmentButton;
			if (equipmentButton.GetEquipment() != null)
			{
				SurvivorModel owningSurvivor = equipmentButton.GetOwningSurvivor();
				int survivorIndex = teamSelectionSelectedSurvivorPanel.GetSurvivorIndex(owningSurvivor);
				SurvivorCard survivorCard = null;
				survivorCard = ((survivorIndex <= -1) ? teamSelectionSurvivorsListPanel.GetCardFromSurvivor(owningSurvivor) : teamSelectionSelectedSurvivorPanel.GetSlotAt(survivorIndex).GetComponent<SurvivorCard>());
				if (base.gameObject != null && survivorCard != null && equipmentButton != null && base.gameObject.GetComponent<EquipmentSelectionContainerView>() != null)
				{
					base.gameObject.GetComponent<EquipmentSelectionContainerView>().OpenForSurvivorCard(survivorCard, equipmentButton);
				}
				UpdateUI();
			}
		}
		else if (type == "OnSurvivorRenamed")
		{
			if (teamSelectionSelectedSurvivorPanel != null)
			{
				teamSelectionSelectedSurvivorPanel.UpdateSlots();
			}
			if (teamSelectionSurvivorsListPanel != null)
			{
				teamSelectionSurvivorsListPanel.UpdateCards();
			}
		}
		else if (type == "ReloadSurvivorList")
		{
			UpdateUI();
		}
	}

	private void SwapSurvivorSlotsInTeam(SurvivorModel survivorModel, int survivorToReplaceIndex)
	{
		survivorBeingAddedToTheTeam = true;
		SurvivorModel survivorAtIndex = teamSelectionSelectedSurvivorPanel.GetSurvivorAtIndex(survivorToReplaceIndex);
		if (survivorAtIndex == survivorModel)
		{
			object parameter = new object[2] { survivorModel, survivorToReplaceIndex };
			UIEvent.Send("EventSurvivorReplaced", parameter);
			return;
		}
		int survivorIndex = teamSelectionSelectedSurvivorPanel.GetSurvivorIndex(survivorModel);
		if (SurvivorType != SurvivorContainerModel.SurvivorType.GvGDefenders)
		{
			if (Helpers.ExecuteCommand(new RemoveSurvivorFromCombatTeamCommand(survivorModel)
			{
				SurvivorType = SurvivorType
			}) == TWDModelResult.OK)
			{
				TWDModelResult tWDModelResult = TWDModelResult.OK;
				if (survivorAtIndex != null)
				{
					tWDModelResult = Helpers.ExecuteCommand(new RemoveSurvivorFromCombatTeamCommand(survivorAtIndex)
					{
						SurvivorType = SurvivorType
					});
				}
				if (tWDModelResult == TWDModelResult.OK)
				{
					Helpers.ExecuteCommand(new SetSurvivorToCombatTeamSlotCommand(survivorModel, survivorToReplaceIndex, survivorIndex)
					{
						SurvivorType = SurvivorType
					});
					if (survivorAtIndex != null)
					{
						Helpers.ExecuteCommand(new SetSurvivorToCombatTeamSlotCommand(survivorAtIndex, survivorIndex, survivorToReplaceIndex)
						{
							SurvivorType = SurvivorType
						});
					}
					object parameter2 = new object[2] { survivorModel, survivorToReplaceIndex };
					UIEvent.Send("EventSurvivorReplaced", parameter2);
					object parameter3 = new object[2] { survivorAtIndex, survivorIndex };
					UIEvent.Send("EventSurvivorReplaced", parameter3);
				}
			}
		}
		else
		{
			object parameter4 = new object[2] { survivorModel, survivorToReplaceIndex };
			UIEvent.Send("EventSurvivorReplaced", parameter4);
			object parameter5 = new object[2] { survivorAtIndex, survivorIndex };
			UIEvent.Send("EventSurvivorReplaced", parameter5);
		}
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/survivor_equip");
	}

	private void ReplaceCurrentSurvivorWith(SurvivorModel survivorModel)
	{
		survivorBeingAddedToTheTeam = true;
		if (SurvivorType != SurvivorContainerModel.SurvivorType.GvGDefenders)
		{
			SetSurvivorToCombat(survivorModel, teamSelectionSelectedSurvivorPanel.GetSurvivorAtIndex(survivorToReplaceIndex));
		}
		object parameter = new object[2] { survivorModel, survivorToReplaceIndex };
		UIEvent.Send("EventSurvivorReplaced", parameter);
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/survivor_equip");
	}

	public void OnEnemyInfoClick()
	{
		EnemyInfoPopup enemyInfoPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.EnemyInfoPopup) as EnemyInfoPopup;
		SurvivalMissionConfig survivalMissionConfig = null;
		MapMissionModel obj = mapMissionModel;
		bool isEndlessMode = obj != null && obj.MissionSpawnPointGroup.Category == MapCategory.Endless;
		if (selectedMapMissionModel.IsUsingSurvivalConfig())
		{
			survivalMissionConfig = selectedMapMissionModel.SolveSurvivalConfigForCurrentMission();
		}
		if (enemyInfoPopup != null)
		{
			enemyInfoPopup.SetContent(missionData, survivalMissionConfig, IsMissionUsingRandomWalkers(survivalMissionConfig), isEndlessMode);
			enemyInfoPopup.Open();
		}
	}

	private void UpdateStartButton()
	{
		if (startCashier == null)
		{
			Helpers.GameObjectSetActive(startButton, value: true);
			Helpers.GameObjectSetActive(challengeStartButton, value: false);
			return;
		}
		int num = 0;
		for (int i = 0; i < (int)CurrencyType.Count; i++)
		{
			CurrencyType currencyType = (CurrencyType)i;
			if (startCashier.GetTotalCost(currencyType) > 0)
			{
				num += startCashier.GetTotalCost(currencyType);
			}
		}
		if (GameManager.Instance.playerModel.IsTimedBonusActive(TimedBonusType.UnlimitedGas) && SurvivorType != SurvivorContainerModel.SurvivorType.CombatGuildBattle)
		{
			num = 0;
		}
		if (num > 0)
		{
			Helpers.GameObjectSetActive(startButton, value: false);
			Helpers.GameObjectSetActive(challengeStartButton, value: true);
			if (guildBattleMapMissionModel != null && !startCashier.CanAfford())
			{
				HelpersUI.SetButtonState(challengeStartUIButton, UIButtonColor.State.Disabled);
			}
			else
			{
				HelpersUI.SetButtonState(challengeStartUIButton, UIButtonColor.State.Normal);
			}
			challengeStartButton.UpdateUI(startCashier);
		}
		else
		{
			Helpers.GameObjectSetActive(startButton, value: true);
			Helpers.GameObjectSetActive(payGold, value: false);
			Helpers.GameObjectSetActive(challengeStartButton, value: false);
		}
	}

	public override void UpdateUI()
	{
		CombatInfo.SetActive(SurvivorType == SurvivorContainerModel.SurvivorType.Combat || SurvivorType == SurvivorContainerModel.SurvivorType.CombatSurvival || SurvivorType == SurvivorContainerModel.SurvivorType.CombatGuildBattle);
		CombatOutpostInfo.SetActive(SurvivorType == SurvivorContainerModel.SurvivorType.CombatOutpost);
		OutpostInfo.SetActive(SurvivorType == SurvivorContainerModel.SurvivorType.Outpost);
		worldBossPVEInfo.SetActive(SurvivorType == SurvivorContainerModel.SurvivorType.WorldBossPVE || SurvivorType == SurvivorContainerModel.SurvivorType.WorldBossPVP || SurvivorType == SurvivorContainerModel.SurvivorType.WorldBoss);
		if (outpostMessage != null)
		{
			bool disableOutpostHeroLimits = GameManager.Instance.gameEconomyData.ConfigData.DisableOutpostHeroLimits;
			outpostMessage.text = LocalizationManager.GetText(disableOutpostHeroLimits ? "Popup.TeamSelect.Defenders.Message.Free" : "Popup.TeamSelect.Defenders.Message");
		}
		if (SurvivorType == SurvivorContainerModel.SurvivorType.Combat || SurvivorType == SurvivorContainerModel.SurvivorType.CombatSurvival || SurvivorType == SurvivorContainerModel.SurvivorType.CombatGuildBattle)
		{
			UpdateUICombat();
		}
		else if (SurvivorType == SurvivorContainerModel.SurvivorType.WorldBossPVE || SurvivorType == SurvivorContainerModel.SurvivorType.WorldBossPVP || SurvivorType == SurvivorContainerModel.SurvivorType.WorldBoss)
		{
			UpdateWorldBossPVEPVP();
		}
		else if (SurvivorType == SurvivorContainerModel.SurvivorType.CombatOutpost || SurvivorType == SurvivorContainerModel.SurvivorType.Outpost)
		{
			Helpers.GameObjectSetActive(SpecialTeamLabel, value: false);
			Helpers.GameObjectSetActive(RegularTitleLabel, value: true);
			Helpers.GameObjectSetActive(missionNoRegularSurvivorsLabel.gameObject, value: false);
		}
		else if (SurvivorType == SurvivorContainerModel.SurvivorType.GvGDefenders)
		{
			Helpers.GameObjectSetActive(SpecialTeamLabel, value: false);
			Helpers.GameObjectSetActive(RegularTitleLabel, value: true);
			GetComponent<GvgDefendersTeamSelection>().UpdateTitle();
			deadlyMissionContainer.SetActive(value: false);
			Helpers.GameObjectSetActive(missionNoRegularSurvivorsLabel.gameObject, value: false);
		}
		else
		{
			deadlyMissionContainer.SetActive(value: false);
		}
		UpdateOutpostMatchInfo();
		UpdateGuildBattleUI();
		UpdateStartButton();
		UpdateGvgDefenders();
	}

	private void UpdateGvgDefenders()
	{
		GameObject[] array = gvgDefendersContainers;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(SurvivorContainerModel.SurvivorType.GvGDefenders == SurvivorType);
		}
	}

	private void RemoveUnavailableWorldBossPVEHeroes(PlayerModel player)
	{
		WorldBossModelManager worldBossModelManager = player?.WorldBossModelManager;
		if (worldBossModelManager == null)
		{
			return;
		}
		List<SurvivorModel> combatSurvivors = player.SurvivorContainer.CombatSurvivors;
		for (int num = combatSurvivors.Count - 1; num >= 0; num--)
		{
			SurvivorModel survivorModel = combatSurvivors[num];
			if (survivorModel != null && !string.IsNullOrEmpty(survivorModel.IdForAnalytics) && !worldBossModelManager.CanHeroBattle(survivorModel.IdForAnalytics))
			{
				Helpers.ExecuteCommand(new RemoveSurvivorFromCombatTeamCommand(survivorModel)
				{
					SurvivorType = SurvivorType
				});
			}
		}
	}

	private void OnWorldBossFullSnapshotChanged(string message, string type)
	{
		if (IsWorldBossSurvivorType() && IsPopupAlive())
		{
			GetWorldBossFullSnapshot();
		}
	}

	private bool IsPopupAlive()
	{
		if (this != null && base.gameObject != null)
		{
			return base.IsOpen;
		}
		return false;
	}

	private bool IsWorldBossSurvivorType()
	{
		if (SurvivorType != SurvivorContainerModel.SurvivorType.WorldBossPVE && SurvivorType != SurvivorContainerModel.SurvivorType.WorldBossPVP)
		{
			return SurvivorType == SurvivorContainerModel.SurvivorType.WorldBoss;
		}
		return true;
	}

	private void GetWorldBossFullSnapshot()
	{
		PlayerModel playerModel = GameManager.Instance?.playerModel;
		WorldBossModelManager worldBossModelManager = playerModel?.WorldBossModelManager;
		if (playerModel != null && worldBossModelManager != null && !(SignalRClient.Instance == null))
		{
			WorldBossGetSnapshotRequest value = ((!worldBossModelManager.IsOffSeason()) ? new WorldBossGetSnapshotRequest
			{
				GroupId = playerModel.GuildId,
				SeasonId = worldBossModelManager.GetCurrentSeasonId(),
				CycleId = worldBossModelManager.GetCurrentCycleId()
			} : new WorldBossGetSnapshotRequest
			{
				GroupId = playerModel.GuildId,
				SeasonId = worldBossModelManager.GetCurrentSeasonId(),
				CycleId = worldBossModelManager.GetNextCycleId()
			});
			string arg = GameManager.Instance.jsonSerializer.Serialize(value);
			SignalRClient.Instance.RequestCommand("WorldBossFullSnapshot", arg, OnWorldBossFullSnapshotAsync, waitForResponse: true);
		}
	}

	private void OnWorldBossFullSnapshotAsync(string responseJson)
	{
		if (SignalRClient.Instance.HasError || string.IsNullOrEmpty(responseJson))
		{
			SignalRClient.Instance.ClearError();
			return;
		}
		WorldBossGuildFullSnapshot worldBossGuildFullSnapshot = GameManager.Instance.jsonSerializer.Deserialize<WorldBossGuildFullSnapshot>(responseJson);
		if (worldBossGuildFullSnapshot != null)
		{
			GameManager.Instance.modelManager.SetWorldBossGuildFullSnapshot(worldBossGuildFullSnapshot);
		}
		if (IsWorldBossSurvivorType() && IsPopupAlive())
		{
			Helpers.GameObjectSetActive(loadingContainer, value: false);
			RefreshWorldBossTeamAfterSnapshot();
		}
	}

	private void RefreshWorldBossTeamAfterSnapshot()
	{
		if (!IsPopupAlive())
		{
			return;
		}
		PlayerModel playerModel = GameManager.Instance?.playerModel;
		if (playerModel == null)
		{
			return;
		}
		RemoveUnavailableWorldBossPVEHeroes(playerModel);
		if (teamSelectionSelectedSurvivorPanel != null)
		{
			teamSelectionSelectedSurvivorPanel.UpdateSlots();
		}
		if (teamSelectionSurvivorsListPanel != null)
		{
			if (teamSelectionSelectedSurvivorPanel != null)
			{
				teamSelectionSurvivorsListPanel.SetCurrentTeam(teamSelectionSelectedSurvivorPanel.GetCurrentTeam());
			}
			if (teamSelectionSurvivorsListPanel.gameObject.activeInHierarchy)
			{
				teamSelectionSurvivorsListPanel.UpdateCards();
			}
		}
		UpdateUI();
	}

	public void UpdateWorldBossPVEPVP()
	{
		WorldBossModelManager worldBossModelManager = GameManager.Instance.playerModel?.WorldBossModelManager;
		if (worldBossModelManager != null)
		{
			List<SurvivorModel> combatSurvivors = GameManager.Instance.playerModel.SurvivorContainer.CombatSurvivors;
			for (int i = 0; i < combatSurvivors.Count && i < 3; i++)
			{
				SurvivorModel survivorModel = combatSurvivors[i];
				if (survivorModel != null && !string.IsNullOrEmpty(survivorModel.IdForAnalytics))
				{
					worldBossModelManager.GetHeroCharges(survivorModel.IdForAnalytics);
				}
			}
		}
		if (SurvivorType == SurvivorContainerModel.SurvivorType.WorldBossPVE)
		{
			UpdateWorldBossPveInfoPanel(GetWorldBossBattlegroundDefinition());
		}
		else if (SurvivorType == SurvivorContainerModel.SurvivorType.WorldBoss)
		{
			UpdateWorldBossBossInfoPanel(GetWorldBossBattlegroundDefinition());
		}
		else if (SurvivorType == SurvivorContainerModel.SurvivorType.WorldBossPVP)
		{
			UpdateWorldBossPvpInfoPanel(GetWorldBossBattlegroundDefinition());
		}
		_ = missionData;
		_ = missionData;
		GameManager.Instance.playerModel.SurvivorContainer.NumberCombatSurvivorsHaveRequiredLevelForMission(selectedMapMissionModel.RequiredSurvivorLevel);
		bool hasInjuredSurvivorInCombatTeam = GameManager.Instance.playerModel.SurvivorContainer.HasInjuredSurvivorInCombatTeam;
		bool hasUpgradingSurvivorInCombatTeam = GameManager.Instance.playerModel.SurvivorContainer.HasUpgradingSurvivorInCombatTeam;
		bool flag = false;
		bool flag2 = false;
		if (guildBattleMapMissionModel != null)
		{
			flag2 = shownGvGMissionCompletion != guildBattleMapMissionModel.CompletionAmount;
		}
		bool flag3 = !hasInjuredSurvivorInCombatTeam && !hasUpgradingSurvivorInCombatTeam && !flag && !flag2;
		worldBossPVEPlayButton.SetActive(flag3);
		worldBossPVEPlayButtonDisabled.SetActive(!flag3);
		if (!flag3)
		{
			if (flag2)
			{
				worldBossPVEPlayButtonDisabledTxt.text = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Popup.TeamSelection.GvGAlreadyCompleted");
			}
			else if (flag)
			{
				worldBossPVEPlayButtonDisabledTxt.text = LocalizationManager.GetText("Popup.TeamSelection.SurvivalOutOfActionTeamMember");
			}
			else if (hasUpgradingSurvivorInCombatTeam)
			{
				worldBossPVEPlayButtonDisabledTxt.text = LocalizationManager.GetText("Popup.TeamSelection.UpgradingTeamMember");
			}
			else if (hasInjuredSurvivorInCombatTeam)
			{
				worldBossPVEPlayButtonDisabledTxt.text = LocalizationManager.GetText("Popup.TeamSelection.InjuredTeamMember");
			}
		}
		Helpers.GameObjectSetActive(missionNoRegularSurvivorsLabel.gameObject, value: false);
		TryStartHeroTraitTutorial(missionData);
	}

	private void UpdateWorldBossBossInfoPanel(WorldBossBattlegroundDefinition definition)
	{
		Helpers.GameObjectSetActive(worldBossPVPInfo, value: false);
		Helpers.GameObjectSetActive((worldBossTitle != null) ? worldBossTitle.gameObject : null, value: true);
		Helpers.GameObjectSetActive((worldBossDifficultyDescription != null) ? worldBossDifficultyDescription.gameObject : null, value: true);
		Helpers.GameObjectSetActive((worldBossDescription != null) ? worldBossDescription.gameObject : null, value: true);
		if (definition != null)
		{
			HelpersUI.SetContentToLabel(worldBossTitle, LocalizationManager.GetText(definition.BuildingName));
			string text = LocalizationManager.GetText("DetailMap.Popup.Distance.Difficulty");
			HelpersUI.SetContentToLabel(worldBossDifficultyDescription, text + " " + definition.EnemyLevel);
			HelpersUI.SetContentToLabel(worldBossDescription, LocalizationManager.GetText(definition.BuildingDoneDesc));
			HelpersUI.SetContentToLabel(worldBossPVEPlayButtonTxt, "开始任务");
			HelpersUI.SetContentToLabel(worldBossPVEPlayButtonDisabledTxt, "开始任务");
		}
	}

	private WorldBossBattlegroundDefinition GetWorldBossBattlegroundDefinition()
	{
		string text = WorldBossCapturePoint;
		if (string.IsNullOrEmpty(text) && SurvivorType == SurvivorContainerModel.SurvivorType.WorldBoss)
		{
			WorldBossMissionModel worldBossMissionModel = selectedMapMissionModel as WorldBossMissionModel;
			text = ((!string.IsNullOrEmpty(worldBossMissionModel?.CapturePoint)) ? worldBossMissionModel.CapturePoint : "BOSS");
		}
		if (string.IsNullOrEmpty(text))
		{
			return null;
		}
		int difficultyLevel = (GameManager.Instance?.playerModel?.WorldBossModelManager)?.GetCurrentBattleDifficulty() ?? 0;
		return GameManager.Instance?.gameEconomyData?.FindWorldBossBattlegroundDefinitionByCapturePoint(text, difficultyLevel);
	}

	private void UpdateWorldBossPveInfoPanel(WorldBossBattlegroundDefinition definition)
	{
		Helpers.GameObjectSetActive(worldBossPVPInfo, value: false);
		Helpers.GameObjectSetActive((worldBossTitle != null) ? worldBossTitle.gameObject : null, value: true);
		Helpers.GameObjectSetActive((worldBossDifficultyDescription != null) ? worldBossDifficultyDescription.gameObject : null, value: true);
		Helpers.GameObjectSetActive((worldBossDescription != null) ? worldBossDescription.gameObject : null, value: true);
		if (definition != null)
		{
			HelpersUI.SetContentToLabel(worldBossTitle, LocalizationManager.GetText(definition.BuildingName));
			string text = LocalizationManager.GetText("World.Boss.TeamSelection.Difficulty", definition.EnemyLevel);
			HelpersUI.SetContentToLabel(worldBossDifficultyDescription, text);
			HelpersUI.SetContentToLabel(worldBossDescription, LocalizationManager.GetText(definition.BuildingDoneDesc));
			HelpersUI.SetContentToLabel(worldBossPVEPlayButtonTxt, LocalizationManager.GetText("World.Boss.Occupy"));
			HelpersUI.SetContentToLabel(worldBossPVEPlayButtonDisabledTxt, LocalizationManager.GetText("World.Boss.Occupy"));
		}
	}

	private void UpdateWorldBossPvpInfoPanel(WorldBossBattlegroundDefinition definition)
	{
		switch (WorldBossCellState)
		{
		case WorldBossPVPItemItem.CellState.Uncross:
			Helpers.GameObjectSetActive(worldBossPVPInfo, value: false);
			Helpers.GameObjectSetActive((worldBossTitle != null) ? worldBossTitle.gameObject : null, value: true);
			Helpers.GameObjectSetActive((worldBossDifficultyDescription != null) ? worldBossDifficultyDescription.gameObject : null, value: true);
			Helpers.GameObjectSetActive((worldBossDescription != null) ? worldBossDescription.gameObject : null, value: true);
			if (definition != null)
			{
				HelpersUI.SetContentToLabel(worldBossTitle, LocalizationManager.GetText(definition.BuildingName));
				string text = LocalizationManager.GetText("World.Boss.TeamSelection.Difficulty", definition.EnemyLevel);
				HelpersUI.SetContentToLabel(worldBossDifficultyDescription, text);
			}
			HelpersUI.SetContentToLabel(worldBossDescription, LocalizationManager.GetText("World.Boss.PVPPVEenemy.desc"));
			HelpersUI.SetContentToLabel(worldBossPVEPlayButtonTxt, LocalizationManager.GetText("World.Boss.Occupy"));
			HelpersUI.SetContentToLabel(worldBossPVEPlayButtonDisabledTxt, LocalizationManager.GetText("World.Boss.Occupy"));
			break;
		case WorldBossPVPItemItem.CellState.Empty:
			Helpers.GameObjectSetActive(worldBossPVPInfo, value: false);
			Helpers.GameObjectSetActive((worldBossTitle != null) ? worldBossTitle.gameObject : null, value: true);
			Helpers.GameObjectSetActive((worldBossDifficultyDescription != null) ? worldBossDifficultyDescription.gameObject : null, value: false);
			Helpers.GameObjectSetActive((worldBossDescription != null) ? worldBossDescription.gameObject : null, value: true);
			if (definition != null)
			{
				HelpersUI.SetContentToLabel(worldBossTitle, LocalizationManager.GetText(definition.BuildingName));
			}
			HelpersUI.SetContentToLabel(worldBossDescription, LocalizationManager.GetText("World.Boss.NOONEHERE.desc"));
			HelpersUI.SetContentToLabel(worldBossPVEPlayButtonTxt, LocalizationManager.GetText("World.Boss.DeployTeam"));
			HelpersUI.SetContentToLabel(worldBossPVEPlayButtonDisabledTxt, LocalizationManager.GetText("World.Boss.DeployTeam"));
			break;
		case WorldBossPVPItemItem.CellState.GetOtherGroup:
			Helpers.GameObjectSetActive((worldBossTitle != null) ? worldBossTitle.gameObject : null, value: false);
			Helpers.GameObjectSetActive((worldBossDescription != null) ? worldBossDescription.gameObject : null, value: false);
			Helpers.GameObjectSetActive((worldBossDifficultyDescription != null) ? worldBossDifficultyDescription.gameObject : null, value: false);
			Helpers.GameObjectSetActive(worldBossPVPInfo, value: true);
			HelpersUI.SetContentToLabel(worldBossPVPEnemyName, WorldBossOccupyingPlayerName);
			UpdateWorldBossOccupyingClassIcons();
			HelpersUI.SetContentToLabel(worldBossPVEPlayButtonTxt, LocalizationManager.GetText("World.Boss.Occupy"));
			HelpersUI.SetContentToLabel(worldBossPVEPlayButtonDisabledTxt, LocalizationManager.GetText("World.Boss.Occupy"));
			break;
		case WorldBossPVPItemItem.CellState.Fight:
		case WorldBossPVPItemItem.CellState.FightHero:
			break;
		}
	}

	private void UpdateWorldBossOccupyingClassIcons()
	{
		if (worldBossPVPInfo == null)
		{
			return;
		}
		List<SurvivorMockData> occupyingSurvivorMocksForDisplay = GetOccupyingSurvivorMocksForDisplay();
		for (int i = 0; i < 3; i++)
		{
			Transform transform = worldBossPVPInfo.transform.Find("Survivor_Icon" + (i + 1));
			if (transform == null)
			{
				continue;
			}
			SurvivorMockData survivorMockData = ((occupyingSurvivorMocksForDisplay != null && i < occupyingSurvivorMocksForDisplay.Count) ? occupyingSurvivorMocksForDisplay[i] : null);
			string text = null;
			if (WorldBossOccupyingSurvivorIds != null && i < WorldBossOccupyingSurvivorIds.Count)
			{
				text = WorldBossOccupyingSurvivorIds[i];
			}
			bool flag = survivorMockData != null || !string.IsNullOrEmpty(text);
			Helpers.GameObjectSetActive(transform.gameObject, flag);
			if (!flag)
			{
				continue;
			}
			Transform transform2 = transform.Find("Class_Icon") ?? transform.Find("ClassIcon");
			UISprite uISprite = ((transform2 != null) ? transform2.GetComponent<UISprite>() : null);
			if (!(uISprite == null))
			{
				string text2 = ((survivorMockData != null) ? HelpersGfx.GetSurvivorClassIconName(survivorMockData.SurvivorClass.ToString(), survivorMockData.RarityLevel) : GetWorldBossOccupyingClassIconName(text));
				if (!string.IsNullOrEmpty(text2))
				{
					uISprite.spriteName = text2;
				}
			}
		}
	}

	private List<SurvivorMockData> GetOccupyingSurvivorMocksForDisplay()
	{
		List<SurvivorMockData> occupyingDefenderMocks = GetOccupyingDefenderMocks();
		if (WorldBossOccupyingSurvivorIds == null || WorldBossOccupyingSurvivorIds.Count == 0)
		{
			return occupyingDefenderMocks;
		}
		if (occupyingDefenderMocks == null || occupyingDefenderMocks.Count == 0)
		{
			return null;
		}
		List<SurvivorMockData> list = new List<SurvivorMockData>();
		for (int i = 0; i < WorldBossOccupyingSurvivorIds.Count; i++)
		{
			string text = WorldBossOccupyingSurvivorIds[i];
			if (string.IsNullOrEmpty(text))
			{
				continue;
			}
			SurvivorMockData item = null;
			for (int j = 0; j < occupyingDefenderMocks.Count; j++)
			{
				if (occupyingDefenderMocks[j] != null && occupyingDefenderMocks[j].AnalyticsId == text)
				{
					item = occupyingDefenderMocks[j];
					break;
				}
			}
			list.Add(item);
		}
		return list;
	}

	private List<SurvivorMockData> GetOccupyingDefenderMocks()
	{
		if (string.IsNullOrEmpty(WorldBossCapturePoint) || string.IsNullOrEmpty(WorldBossCell))
		{
			return null;
		}
		WorldBossGuildFullSnapshot worldBossGuildFullSnapshot = GameManager.Instance?.playerModel?.WorldBossModelManager?.WorldBossGuildFullSnapshot;
		if (worldBossGuildFullSnapshot?.CapturePoints == null || GameManager.Instance?.jsonSerializer == null)
		{
			return null;
		}
		string value = null;
		for (int i = 0; i < worldBossGuildFullSnapshot.CapturePoints.Count; i++)
		{
			WorldBossCapturePointSnapshot worldBossCapturePointSnapshot = worldBossGuildFullSnapshot.CapturePoints[i];
			if (worldBossCapturePointSnapshot == null || worldBossCapturePointSnapshot.CapturePoint != WorldBossCapturePoint)
			{
				continue;
			}
			if (worldBossCapturePointSnapshot.Defenders == null)
			{
				break;
			}
			for (int j = 0; j < worldBossCapturePointSnapshot.Defenders.Count; j++)
			{
				WorldBossCellDefenderSnapshot worldBossCellDefenderSnapshot = worldBossCapturePointSnapshot.Defenders[j];
				if (worldBossCellDefenderSnapshot != null && worldBossCellDefenderSnapshot.Cell == WorldBossCell)
				{
					value = worldBossCellDefenderSnapshot.DefenderInfo;
					break;
				}
			}
			break;
		}
		if (string.IsNullOrEmpty(value))
		{
			return null;
		}
		return GameManager.Instance.jsonSerializer.Deserialize<GuildBattleParticipantInfo>(value)?.SelectedSurvivors;
	}

	private string GetWorldBossOccupyingClassIconName(string survivorAnalyticsId)
	{
		if (string.IsNullOrEmpty(survivorAnalyticsId))
		{
			return null;
		}
		SurvivorModel survivorModel = FindSurvivorByAnalyticsId(survivorAnalyticsId);
		if (survivorModel != null)
		{
			return HelpersGfx.GetSurvivorClassIconName(survivorModel);
		}
		SurvivorMockData survivorMockData = FindOccupyingSurvivorMockData(survivorAnalyticsId);
		if (survivorMockData != null)
		{
			return HelpersGfx.GetSurvivorClassIconName(survivorMockData.SurvivorClass.ToString(), survivorMockData.RarityLevel);
		}
		ActorDefinition actorDefinition = GameManager.Instance?.gameEconomyData?.GetActorDefinition(survivorAnalyticsId);
		if (actorDefinition != null && !string.IsNullOrEmpty(actorDefinition.Class))
		{
			return HelpersGfx.GetSurvivorClassIconName(actorDefinition.Class, actorDefinition.RarityLevel);
		}
		return null;
	}

	private static SurvivorModel FindSurvivorByAnalyticsId(string analyticsId)
	{
		ModelList<SurvivorModel> modelList = GameManager.Instance?.playerModel?.SurvivorContainer?.Survivors;
		if (modelList == null)
		{
			return null;
		}
		for (int i = 0; i < modelList.Count; i++)
		{
			if (modelList[i] != null && modelList[i].IdForAnalytics == analyticsId)
			{
				return modelList[i];
			}
		}
		return null;
	}

	private SurvivorMockData FindOccupyingSurvivorMockData(string analyticsId)
	{
		if (string.IsNullOrEmpty(WorldBossCapturePoint) || string.IsNullOrEmpty(WorldBossCell))
		{
			return null;
		}
		WorldBossGuildFullSnapshot worldBossGuildFullSnapshot = GameManager.Instance?.playerModel?.WorldBossModelManager?.WorldBossGuildFullSnapshot;
		if (worldBossGuildFullSnapshot?.CapturePoints == null || GameManager.Instance?.jsonSerializer == null)
		{
			return null;
		}
		string value = null;
		for (int i = 0; i < worldBossGuildFullSnapshot.CapturePoints.Count; i++)
		{
			WorldBossCapturePointSnapshot worldBossCapturePointSnapshot = worldBossGuildFullSnapshot.CapturePoints[i];
			if (worldBossCapturePointSnapshot == null || worldBossCapturePointSnapshot.CapturePoint != WorldBossCapturePoint || worldBossCapturePointSnapshot.Defenders == null)
			{
				continue;
			}
			for (int j = 0; j < worldBossCapturePointSnapshot.Defenders.Count; j++)
			{
				WorldBossCellDefenderSnapshot worldBossCellDefenderSnapshot = worldBossCapturePointSnapshot.Defenders[j];
				if (worldBossCellDefenderSnapshot != null && worldBossCellDefenderSnapshot.Cell == WorldBossCell)
				{
					value = worldBossCellDefenderSnapshot.DefenderInfo;
					break;
				}
			}
			break;
		}
		if (string.IsNullOrEmpty(value))
		{
			return null;
		}
		GuildBattleParticipantInfo guildBattleParticipantInfo = GameManager.Instance.jsonSerializer.Deserialize<GuildBattleParticipantInfo>(value);
		if (guildBattleParticipantInfo?.SelectedSurvivors == null)
		{
			return null;
		}
		for (int k = 0; k < guildBattleParticipantInfo.SelectedSurvivors.Count; k++)
		{
			SurvivorMockData survivorMockData = guildBattleParticipantInfo.SelectedSurvivors[k];
			if (survivorMockData != null && survivorMockData.AnalyticsId == analyticsId)
			{
				return survivorMockData;
			}
		}
		return null;
	}

	public void UpdateUICombat()
	{
		SurvivalMissionConfig survivalMissionConfig = (selectedMapMissionModel.IsUsingSurvivalConfig() ? selectedMapMissionModel.SolveSurvivalConfigForCurrentMission() : null);
		bool flag = mapMissionModel?.IsFixedSurvivorSeasonMission ?? false;
		missionLevelLabel.text = LocalizationManager.GetText("Popup.TeamSelection.MissionLevel{Level}", selectedMapMissionModel.MissionLevel.ToString());
		MapMissionModel obj = mapMissionModel;
		bool flag2 = obj != null && obj.MissionSpawnPointGroup.Category == MapCategory.Endless;
		if (missionData != null)
		{
			missionTypeLabel.text = HelpersLocalization.GetMissionTypeName(missionData.MissionType);
		}
		if ((bool)missionNoRegularSurvivorsLabel)
		{
			Helpers.GameObjectSetActive(missionNoRegularSurvivorsLabel.gameObject, flag);
		}
		if (survivalMissionConfig != null)
		{
			if (guildBattleMapMissionModel != null && !guildBattleMapMissionModel.IsEnemyUnlocked())
			{
				missionNameLabel.text = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("MissionObjective." + survivalMissionConfig.ObjectiveType);
			}
			else
			{
				missionNameLabel.text = HelpersLocalization.GetSurvivalMissionName(survivalMissionConfig);
			}
			missionBriefingLabel.text = "";
		}
		else if (flag2)
		{
			if (EndlessModeHelpers.IsEndlessExpertMode())
			{
				missionNameLabel.text = EndlessModeHelpers.GetNormalCurrentEndlessModeMapName;
			}
			else
			{
				missionNameLabel.text = EndlessModeHelpers.GetExpertCurrentEndlessModeMapName;
			}
			missionBriefingLabel.text = LocalizationManager.GetText("Mission.EndlessMode.Briefing");
		}
		else if (missionData != null)
		{
			missionNameLabel.text = HelpersLocalization.GetMissionName(missionData.DisplayTextID);
			missionBriefingLabel.text = HelpersLocalization.GetMissionBriefing(missionData.DisplayTextID);
		}
		missioFlavorLabel.text = "";
		bool flag3 = GameManager.Instance.playerModel.SurvivorContainer.NumberCombatSurvivorsHaveRequiredLevelForMission(selectedMapMissionModel.RequiredSurvivorLevel) > 0;
		bool flag4 = GameManager.Instance.playerModel.SurvivorContainer.HasInjuredSurvivorInCombatTeam;
		bool hasUpgradingSurvivorInCombatTeam = GameManager.Instance.playerModel.SurvivorContainer.HasUpgradingSurvivorInCombatTeam;
		bool flag5 = false;
		if (survivalMissionConfig != null && survivalMissionConfig.MissionType == SurvivalMissionConfig.Type.Survival)
		{
			flag4 = false;
			flag5 = GameManager.Instance.playerModel.SurvivorContainer.SurvivalCharacters.HasOutOfActionSurvivorInCombatTeam;
		}
		bool flag6 = false;
		if (guildBattleMapMissionModel != null)
		{
			flag6 = shownGvGMissionCompletion != guildBattleMapMissionModel.CompletionAmount;
			if (flag6 && HelpersModel.IsUnlockAllSectors) flag6 = false;
		}
		bool flag7 = !flag4 && !hasUpgradingSurvivorInCombatTeam && !flag5 && !flag6;
		missionUnlockedContainer.SetActive(flag7);
		startButtonLocked.SetActive(!flag7);
		if (!flag7)
		{
			Helpers.GameObjectSetActive(iconAlert, value: true);
			if (flag6)
			{
				missionLockedLabel.text = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Popup.TeamSelection.GvGAlreadyCompleted");
			}
			else if (flag5)
			{
				missionLockedLabel.text = LocalizationManager.GetText("Popup.TeamSelection.SurvivalOutOfActionTeamMember");
			}
			else if (hasUpgradingSurvivorInCombatTeam)
			{
				missionLockedLabel.text = LocalizationManager.GetText("Popup.TeamSelection.UpgradingTeamMember");
			}
			else if (flag4)
			{
				missionLockedLabel.text = LocalizationManager.GetText("Popup.TeamSelection.InjuredTeamMember");
			}
		}
		else
		{
			if (flag2)
			{
				EndlessModeManagerModel endlessModeManager = GameManager.Instance.playerModel.EndlessModeManager;
				missionLockedLabel.text = LocalizationManager.GetText("Popup.TeamSelection.MissionLocked[MinSurvivorLevel]", EndlessModeHelpers.GetStartingDifficulty(endlessModeManager.EndlessModeGameModeType));
			}
			else
			{
				missionLockedLabel.text = LocalizationManager.GetText("Popup.TeamSelection.MissionLocked[MinSurvivorLevel]", selectedMapMissionModel.RequiredSurvivorLevel);
			}
			Helpers.GameObjectSetActive(iconAlert, !flag3);
			missionLockedContainer.SetActive(!flag);
		}
		SetEnemyInfo(missionData, survivalMissionConfig, flag2);
		difficultyBg.color = difficultyColors[(int)selectedMapMissionModel.MissionDifficulty];
		bool flag8 = mapMissionModel != null && mapMissionModel.Stars != null && (mapMissionModel.IsInWeeklyChallenge || mapMissionModel.IsInApocalyptiWeeklyChallenge);
		starsContainer.SetActive(flag8);
		featuredHeroCallContainer.SetActive(flag8 && !WeeklyChallengeHelper.HasUnLockedFeaturedHero() && WeeklyChallengeHelper.FeaturedStarHeroActive);
		if (flag8)
		{
			for (int i = 0; i < stars.Length; i++)
			{
				stars[i].spriteName = ((i >= mapMissionModel.Stars.NumberStars) ? "Ui_Icon_Reward_Star_Bg" : "Ui_Icon_Reward_Star");
			}
		}
		if (missionData?.ExtraData != null && missionData.ExtraData.InUse && missionData.ExtraData.PlayableSurvivors != null && missionData.ExtraData.PlayableSurvivors.Count > 0)
		{
			Helpers.GameObjectSetActive(SpecialTeamLabel, value: true);
			Helpers.GameObjectSetActive(RegularTitleLabel, value: false);
		}
		else
		{
			Helpers.GameObjectSetActive(SpecialTeamLabel, value: false);
			Helpers.GameObjectSetActive(RegularTitleLabel, value: true);
		}
		deadlyMissionContainer.SetActive(mapMissionModel != null && mapMissionModel.IsDeadly);
		if (!OfflineManager.IsTutorialDisable)
			TryStartHeroTraitTutorial(missionData);
	}

	private void TryStartHeroTraitTutorial(MissionData missionData)
	{
		if (missionData == null || !(missionData.DisplayTextID == "E02M03") || GameManager.Instance.playerModel.Tutorial.HasCompletedPart("HeroTrait") || !(TutorialView.Instance != null) || TutorialView.Instance.Running)
		{
			return;
		}
		CampModel camp = GameManager.Instance.playerModel.Camp;
		TrainingGroundBuildingModel trainingGroundBuildingModel = ((camp == null) ? null : (camp.GetBuilding("TrainingGround") as TrainingGroundBuildingModel));
		string heroId = SurvivorToken.GetHeroId(CurrencyType.DarylToken);
		if (trainingGroundBuildingModel != null && trainingGroundBuildingModel.UpgradingSurvivor != null && trainingGroundBuildingModel.UpgradingSurvivor.Definition != null && trainingGroundBuildingModel.UpgradingSurvivor.Definition.ID == heroId)
		{
			return;
		}
		SurvivorModel heroById = GameManager.Instance.playerModel.SurvivorContainer.GetHeroById(heroId);
		if (heroById == null || GameManager.Instance.playerModel.SurvivorContainer.CombatSurvivors == null)
		{
			return;
		}
		for (int i = 0; i < GameManager.Instance.playerModel.SurvivorContainer.CombatSurvivors.Count; i++)
		{
			if (GameManager.Instance.playerModel.SurvivorContainer.CombatSurvivors[i] == heroById)
			{
				return;
			}
		}
		TutorialView.Instance.StartPart("HeroTrait");
	}

	private void SetEnemyInfo(MissionData missionData, SurvivalMissionConfig survivalMissionConfig, bool isEndlessMission)
	{
		Helpers.GameObjectSetActive(normalWalkers, value: false);
		Helpers.GameObjectSetActive(armoredWalkers, value: false);
		Helpers.GameObjectSetActive(tankWalkers, value: false);
		Helpers.GameObjectSetActive(burningWalkers, value: false);
		Helpers.GameObjectSetActive(raiders, value: false);
		Helpers.GameObjectSetActive(explosiveWalkers, value: false);
		Helpers.GameObjectSetActive(gooWalkers, value: false);
		Helpers.GameObjectSetActive(spikedWalkers, value: false);
		Helpers.GameObjectSetActive(metalheadWalkers, value: false);
		Helpers.GameObjectSetActive(fastWalkers, value: false);
		Helpers.GameObjectSetActive(randomWalkers, value: false);
		Helpers.GameObjectSetActive(explosiveBarrels, value: false);
		Helpers.GameObjectSetActive(commonwealthWalkers, value: false);
		int numberOfTypes = 0;
		if (survivalMissionConfig != null)
		{
			SetSpecialTypesForSurvival(survivalMissionConfig, out numberOfTypes);
		}
		else if (isEndlessMission)
		{
			SetEndlessSpecialTypes(out numberOfTypes);
		}
		else
		{
			SetSpecialTypes(missionData, out numberOfTypes);
		}
		if (enemyTypesContainer != null)
		{
			enemyTypesContainer.SetActive(numberOfTypes != 0);
		}
		if (enemiesContainer != null)
		{
			enemiesContainer.repositionNow = true;
		}
	}

	private void SetSpecialTypesForSurvival(SurvivalMissionConfig conf, out int numberOfTypes)
	{
		int num = 0;
		if (conf != null)
		{
			if (conf.HasBurningTypes())
			{
				burningWalkers.gameObject.SetActive(value: true);
				num++;
			}
			if (conf.HasAnyRaiderType() && raiders != null)
			{
				raiders.gameObject.SetActive(value: true);
				num++;
			}
			if (conf.HasWalker(WalkerType.WalkerArmored) && armoredWalkers != null)
			{
				armoredWalkers.gameObject.SetActive(value: true);
				num++;
			}
			if (conf.HasWalker(WalkerType.WalkerTank) && tankWalkers != null)
			{
				tankWalkers.gameObject.SetActive(value: true);
				num++;
			}
			if (conf.HasWalker(WalkerType.WalkerExplosive) && explosiveWalkers != null)
			{
				explosiveWalkers.gameObject.SetActive(value: true);
				num++;
			}
			if (conf.HasWalker(WalkerType.WalkerGoo) && gooWalkers != null)
			{
				gooWalkers.gameObject.SetActive(value: true);
				num++;
			}
			if (conf.HasWalker(WalkerType.WalkerSpiked) && spikedWalkers != null)
			{
				spikedWalkers.gameObject.SetActive(value: true);
				num++;
			}
			if (conf.HasWalker(WalkerType.WalkerMetalhead) && metalheadWalkers != null)
			{
				metalheadWalkers.gameObject.SetActive(value: true);
				num++;
			}
			if (conf.HasWalker(WalkerType.WalkerFast) && fastWalkers != null)
			{
				fastWalkers.gameObject.SetActive(value: true);
				num++;
			}
			if (conf.HasWalker(WalkerType.ExplosiveBarrel) && explosiveBarrels != null)
			{
				explosiveBarrels.gameObject.SetActive(value: true);
				num++;
			}
			if (conf.HasWalker(WalkerType.WalkerCommonWealth) && commonwealthWalkers != null)
			{
				commonwealthWalkers.gameObject.SetActive(value: true);
				num++;
			}
			if (IsMissionUsingRandomWalkers(conf) && randomWalkers != null)
			{
				randomWalkers.gameObject.SetActive(value: true);
				num++;
			}
		}
		numberOfTypes = num;
	}

	private void SetSpecialTypes(MissionData missionData, out int numberOfTypes)
	{
		int num = 0;
		if (missionData != null)
		{
			if (missionData.HasEnemyTrait("Burning"))
			{
				burningWalkers.gameObject.SetActive(value: true);
				num++;
			}
			if (missionData.RaiderTypes != 0 && raiders != null)
			{
				Array values = Enum.GetValues(typeof(SurvivorClass));
				for (int i = 0; i < values.Length; i++)
				{
					if (i != 6)
					{
						SurvivorClass cls = (SurvivorClass)values.GetValue(i);
						if (missionData.HasRaider(cls))
						{
							raiders.gameObject.SetActive(value: true);
							num++;
							break;
						}
					}
				}
			}
			if (missionData.HasWalker(WalkerType.WalkerArmored) && armoredWalkers != null)
			{
				armoredWalkers.gameObject.SetActive(value: true);
				num++;
			}
			if (missionData.HasWalker(WalkerType.WalkerTank) && tankWalkers != null)
			{
				tankWalkers.gameObject.SetActive(value: true);
				num++;
			}
			if (missionData.HasWalker(WalkerType.WalkerExplosive) && explosiveWalkers != null)
			{
				explosiveWalkers.gameObject.SetActive(value: true);
				num++;
			}
			if (missionData.HasWalker(WalkerType.WalkerGoo) && gooWalkers != null)
			{
				gooWalkers.gameObject.SetActive(value: true);
				num++;
			}
			if (missionData.HasWalker(WalkerType.WalkerSpiked) && spikedWalkers != null)
			{
				spikedWalkers.gameObject.SetActive(value: true);
				num++;
			}
			if (missionData.HasWalker(WalkerType.WalkerMetalhead) && metalheadWalkers != null)
			{
				metalheadWalkers.gameObject.SetActive(value: true);
				num++;
			}
			if (missionData.HasWalker(WalkerType.WalkerFast) && fastWalkers != null)
			{
				fastWalkers.gameObject.SetActive(value: true);
				num++;
			}
			if (missionData.HasWalker(WalkerType.ExplosiveBarrel) && explosiveBarrels != null)
			{
				explosiveBarrels.gameObject.SetActive(value: true);
				num++;
			}
			if (missionData.HasWalker(WalkerType.WalkerCommonWealth) && commonwealthWalkers != null)
			{
				commonwealthWalkers.gameObject.SetActive(value: true);
				num++;
			}
			if (IsMissionUsingRandomWalkers() && randomWalkers != null)
			{
				randomWalkers.gameObject.SetActive(value: true);
				num++;
			}
		}
		numberOfTypes = num;
	}

	private void SetEndlessSpecialTypes(out int numberOfTypes)
	{
		List<WalkerType> list = ((!EndlessModeHelpers.IsEndlessExpertMode()) ? EndlessModeHelpers.GetEndlessBattleMissionWalkerTypes() : EndlessModeHelpers.GetEndlessExpertBattleMissionWalkerTypes());
		int num = 0;
		foreach (WalkerType item in list)
		{
			if (item == WalkerType.WalkerArmored || (item == WalkerType.WalkerArmored_Boss && armoredWalkers != null))
			{
				armoredWalkers.gameObject.SetActive(value: true);
				num++;
			}
			if (item == WalkerType.WalkerTank || (item == WalkerType.WalkerTank_Boss && tankWalkers != null))
			{
				tankWalkers.gameObject.SetActive(value: true);
				num++;
			}
			if (item == WalkerType.WalkerExplosive || (item == WalkerType.WalkerExplosive_Boss && explosiveWalkers != null))
			{
				explosiveWalkers.gameObject.SetActive(value: true);
				num++;
			}
			if (item == WalkerType.WalkerGoo || (item == WalkerType.WalkerGoo_Boss && gooWalkers != null))
			{
				gooWalkers.gameObject.SetActive(value: true);
				num++;
			}
			if (item == WalkerType.WalkerSpiked || (item == WalkerType.WalkerSpiked_Boss && spikedWalkers != null))
			{
				spikedWalkers.gameObject.SetActive(value: true);
				num++;
			}
			if (item == WalkerType.WalkerMetalhead || (item == WalkerType.WalkerMetalhead_Boss && metalheadWalkers != null))
			{
				metalheadWalkers.gameObject.SetActive(value: true);
				num++;
			}
			if (item == WalkerType.WalkerFast || (item == WalkerType.WalkerFast_Boss && fastWalkers != null))
			{
				fastWalkers.gameObject.SetActive(value: true);
				num++;
			}
			if (item == WalkerType.WalkerCommonWealth && commonwealthWalkers != null)
			{
				commonwealthWalkers.gameObject.SetActive(value: true);
				num++;
			}
		}
		numberOfTypes = list.Count;
	}

	private bool IsMissionUsingRandomWalkers(SurvivalMissionConfig conf = null)
	{
		WalkerRandomizerSwap walkerRandomizerSwap = GameManager.Instance.gameEconomyData.GetWalkerRandomizerSwap(GetMapCategory(conf), GetMissionLevel());
		WalkerRandomizerWeight walkerRandomizerWeight = GameManager.Instance.gameEconomyData.GetWalkerRandomizerWeight(GetMapCategory(conf), GetMissionLevel());
		if (walkerRandomizerSwap != null && 0 < walkerRandomizerSwap.MaxSwaps)
		{
			return walkerRandomizerWeight != null;
		}
		return false;
	}

	private MapCategory GetMapCategory(SurvivalMissionConfig survivalMissionConfig = null)
	{
		if (SurvivorType == SurvivorContainerModel.SurvivorType.CombatOutpost)
		{
			return MapCategory.Outpost;
		}
		if (survivalMissionConfig != null && survivalMissionConfig.MissionType == SurvivalMissionConfig.Type.GuildBattle)
		{
			return MapCategory.GuildBattle;
		}
		if (mapMissionModel != null && mapMissionModel.MissionSpawnPointGroup != null)
		{
			return mapMissionModel.MissionSpawnPointGroup.Category;
		}
		return MapCategory.Story;
	}

	private int GetMissionLevel()
	{
		if (SurvivorType == SurvivorContainerModel.SurvivorType.CombatOutpost || (SurvivorType == SurvivorContainerModel.SurvivorType.CombatGuildBattle && guildBattleMapMissionModel != null && guildBattleMapMissionModel.IsEnemyUnlocked()))
		{
			return GameManager.Instance.playerModel.Level;
		}
		if (mapMissionModel != null)
		{
			return mapMissionModel.MissionLevel;
		}
		if (guildBattleMapMissionModel != null)
		{
			return guildBattleMapMissionModel.MissionLevel;
		}
		return 1;
	}

	private void UpdateOutpostMatchInfo()
	{
		Helpers.GameObjectSetActive(outpostMatchInfoNormal, value: false);
		Helpers.GameObjectSetActive(outpostMatchInfoLimited, value: false);
		if (SurvivorType != SurvivorContainerModel.SurvivorType.CombatOutpost)
		{
			return;
		}
		if (GameManager.Instance.gameEconomyData.ConfigData.SkipOutpostMatchPreview)
		{
			if (outpostMatchInfoLimited != null)
			{
				outpostMatchInfoLimited.CurrentMatchInfo = OutpostMatchInfo;
				outpostMatchInfoLimited.CurrentMatchSurviviorName = OutpostDefenderName;
				outpostMatchInfoLimited.CurrentMatchPlayerHashedId = OutpostDefenderHashedId;
				outpostMatchInfoLimited.UpdateUI();
				Helpers.GameObjectSetActive(outpostMatchInfoLimited, value: true);
			}
		}
		else if (outpostMatchInfoNormal != null)
		{
			outpostMatchInfoNormal.CurrentMatchInfo = OutpostMatchInfo;
			outpostMatchInfoNormal.CurrentMatchSurviviorName = OutpostDefenderName;
			outpostMatchInfoNormal.CurrentMatchPlayerHashedId = OutpostDefenderHashedId;
			outpostMatchInfoNormal.UpdateUI();
			Helpers.GameObjectSetActive(outpostMatchInfoNormal, value: true);
		}
	}

	private void UpdateGuildBattleUI()
	{
		if (SurvivorType == SurvivorContainerModel.SurvivorType.CombatGuildBattle)
		{
			UpdateGuildBattleEnemyTeamInfo();
			bool flag = activeBonusesList != null && activeBonusesList.AreActiveBonusesAvailable();
			Helpers.GameObjectSetActive(activeBonusesGameObject, flag);
			if (flag)
			{
				activeBonusesList.UpdateActiveBonuses();
			}
			if (guildBatleRewards != null)
			{
				guildBatleRewards.SetupForMission(guildBattleMapMissionModel);
			}
		}
		else
		{
			Helpers.GameObjectSetActive(guildBattleEnemyInfo.gameObject, value: false);
			Helpers.GameObjectSetActive(activeBonusesGameObject, value: false);
			Helpers.GameObjectSetActive(guildBatleRewards, value: false);
		}
	}

	private void UpdateGuildBattleEnemyTeamInfo()
	{
		if (guildBattleMapMissionModel != null && guildBattleMapMissionModel.IsEnemyUnlocked())
		{
			guildBattleEnemyInfo.Model = guildBattleMapMissionModel;
			guildBattleEnemyInfo.UpdateUI();
			Helpers.GameObjectSetActive(guildBattleEnemyInfo.gameObject, value: true);
		}
		else
		{
			Helpers.GameObjectSetActive(guildBattleEnemyInfo.gameObject, value: false);
		}
	}

	private void SetSurvivorToCombat(SurvivorModel newSurvivor, SurvivorModel oldSurvivor)
	{
		if (newSurvivor != null && Helpers.ExecuteCommand(new SetSurvivorToCombatCommand(newSurvivor, oldSurvivor)
		{
			SurvivorType = SurvivorType
		}) == TWDModelResult.AlreadyMaxAmount)
		{
			HUDNotification.Error(LocalizationManager.GetText("Notification.CombatTeamFull"));
		}
	}

	public void OnClickRest()
	{
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_click");
		SurvivalRestRequest();
	}

	public async void OnClickGoToCombat()
	{
		if (OfflineManager.IsLoadDataManager)
		{
			DebugTWD.LogMycode("if (OfflineManager.IsLoadDataManager)");

			DebugTWD.Log("OnClick GoToCombat async", DebugType.OnClick);
			if (!OfflineManager.IsMissionModBuild)
			{
				MyTools.OpenAlert("!!!Прохождение миссий недоступно в версии \"Base\"!!!");
				return;
			}
			if (!DataManager.Instance.ProGuild)
			{
				MyTools.OpenAlert("Прохождение миссий доступно только для пользователей с Pro-допуском\nСвяжитесь с разработчиком для дополнительной информации!!!");
				return;
			}
		}

		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_click");
		if (!(await teamPresetSelectionPanel.Intercept()))
		{
			return;
		}
		List<SurvivorModel> combatSurvivors = GameManager.Instance.playerModel.SurvivorContainer.CombatSurvivors;
		int maxTeamSize = GetMaxTeamSize(missionData);
		if (combatSurvivors.Count < maxTeamSize)
		{
			AlertPopup confirmationPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.AlertPopup) as AlertPopup;
			confirmationPopup.SetContent(LocalizationManager.GetText("Popup.PartialCombatTeam.Title"), LocalizationManager.GetText("Popup.PartialTeam.Message"));
			confirmationPopup.SetOkButtonLabel(LocalizationManager.GetText("Button.Ok"));
			confirmationPopup.SetCallbacks(delegate
			{
				confirmationPopup.Close();
			});
			confirmationPopup.Open();
		}
		else if (SurvivorType == SurvivorContainerModel.SurvivorType.WorldBossPVE || SurvivorType == SurvivorContainerModel.SurvivorType.WorldBossPVP)
		{
			RequestWorldBossCellStatusBeforeCombat();
		}
		else
		{
			GoToCombat();
		}
	}

	private void RequestWorldBossCellStatusBeforeCombat()
	{
		string text = WorldBossCapturePoint;
		string text2 = WorldBossCell;
		if ((string.IsNullOrEmpty(text) || string.IsNullOrEmpty(text2)) && selectedMapMissionModel is WorldBossMissionModel worldBossMissionModel)
		{
			text = worldBossMissionModel.CapturePoint;
			text2 = worldBossMissionModel.Cell;
		}
		if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(text2))
		{
			BeginWorldBossCombatEnterCover();
			WorldBossCellStatusRequest value = new WorldBossCellStatusRequest
			{
				GroupId = GameManager.Instance.playerModel.GuildId,
				SeasonId = GameManager.Instance.playerModel.WorldBossModelManager.GetCurrentSeasonId(),
				CycleId = GameManager.Instance.playerModel.WorldBossModelManager.GetCurrentCycleId(),
				CapturePoint = text,
				Cell = text2
			};
			string arg = GameManager.Instance.jsonSerializer.Serialize(value);
			SignalRClient.Instance.RequestCommand("WorldBossCellStatus", arg, OnWorldBossCellStatusAsync, waitForResponse: true);
		}
	}

	private void OnWorldBossCellStatusAsync(string responseJson)
	{
		if (IsPopupAlive())
		{
			WorldBossCellStatusResult worldBossCellStatusResult = GameManager.Instance.jsonSerializer.Deserialize<WorldBossCellStatusResult>(responseJson);
			if (IsWorldBossCellReadyForCombat(worldBossCellStatusResult))
			{
				GoToCombat();
				return;
			}
			EndWorldBossCombatEnterCover();
			HUDNotification.Info(LocalizationManager.GetText((worldBossCellStatusResult != null && worldBossCellStatusResult.IsOccupied) ? "World.Boss.Occupied.Tips" : "World.Boss.AtWar.Tips"));
			RefreshWorldBossDetailSnapshot();
			Close();
		}
	}

	private void BeginWorldBossCombatEnterCover()
	{
		TransitionScreenHUD transitionScreenHUD = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.Transition) as TransitionScreenHUD;
		if (!(transitionScreenHUD == null) && !transitionScreenHUD.IsOpen)
		{
			transitionScreenHUD.AnimationInCallback = null;
			transitionScreenHUD.SceneToLoadAfterInAnimation = null;
			transitionScreenHUD.SceneToUnload = null;
			transitionScreenHUD.Open();
		}
	}

	private void EndWorldBossCombatEnterCover()
	{
		SingularityMonoBehaviour<HUDManager>.Instance.CloseIfExists(UIType.Transition);
	}

	private bool IsWorldBossCellReadyForCombat(WorldBossCellStatusResult response)
	{
		if (response == null || !response.Success || response.IsFighting)
		{
			return false;
		}
		if (response.IsOccupied)
		{
			if (SurvivorType == SurvivorContainerModel.SurvivorType.WorldBossPVE)
			{
				return false;
			}
			if (SurvivorType == SurvivorContainerModel.SurvivorType.WorldBossPVP && WorldBossCellState == WorldBossPVPItemItem.CellState.Empty)
			{
				return false;
			}
			if (response.OccupyingGroupId == GameManager.Instance.playerModel.GuildId)
			{
				return false;
			}
		}
		return true;
	}

	private void RefreshWorldBossDetailSnapshot()
	{
		if (SurvivorType == SurvivorContainerModel.SurvivorType.WorldBossPVE)
		{
			WorldBossPVEDetailBackPopup worldBossPVEDetailBackPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.WorldBossPVEDetailBackPopup) as WorldBossPVEDetailBackPopup;
			if (worldBossPVEDetailBackPopup != null)
			{
				worldBossPVEDetailBackPopup.GetWorldBossFullSnapshot();
			}
		}
		else if (SurvivorType == SurvivorContainerModel.SurvivorType.WorldBossPVP)
		{
			WorldBossPVPDetailPopup worldBossPVPDetailPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.WorldBossPVPDetailPopup) as WorldBossPVPDetailPopup;
			if (worldBossPVPDetailPopup != null)
			{
				worldBossPVPDetailPopup.GetWorldBossFullSnapshot();
			}
		}
	}

	public void CloseInvalidTeamPopUp()
	{
	}

	public bool AreMoreRestsAvailable()
	{
		if (GameManager.Instance.playerModel.WeeklySurvival != null)
		{
			return GameManager.Instance.playerModel.WeeklySurvival.IsRestAvailable;
		}
		return false;
	}

	public bool CanRest()
	{
		if (GameManager.Instance.playerModel.SurvivorContainer.SurvivalCharacters.CanAnySurvivorRest())
		{
			return AreMoreRestsAvailable();
		}
		return false;
	}

	public void SurvivalRestRequest()
	{
		if (CanRest() && survivalRestCashier != null)
		{
			DebugLog("OnClickBuyRest");
			lastRestedSurvivorsList = GameManager.Instance.playerModel.SurvivorContainer.SurvivalCharacters.GetSurvivorsForRest();
			ConsumeCurrencyCommandUtils.Execute(new BuySurvivalRestCommand
			{
				Cashier = survivalRestCashier
			}, OnSurvivalRestCallback);
		}
	}

	public void GoToCombat()
	{
		if (loadingCombat)
		{
			return;
		}

		if (OfflineManager.IsLoadDataManager && !OfflineManager.IsLoadFromResources)
		{
			var weapons = "weapons";
			bool isWeaponBundleEmpty = AssetBundleManager.Instance.IsAssetBundleEmpty(weapons);
			bool isWeaponBundleLoading = AssetBundleManager.Instance.IsAssetBundleDownloading(weapons);

			if (isWeaponBundleEmpty)
			{
				if (!isWeaponBundleLoading)
				{
					AssetBundleController.Instance.LoadAssetBundle(new List<string>() { weapons }, GoToCombat);
				}
				return;
			}
			DebugTWD.Log("AssetBundle: weapons is loaded", DebugType.OnClick);
		}

		if (SurvivorType == SurvivorContainerModel.SurvivorType.CombatOutpost)
		{
			ConsumeCurrencyCommandUtils.Execute(new PayForOutpostRaidCommand
			{
				Cashier = GameManager.Instance.playerModel.OutpostModel.GetRaidCashier()
			}, OnGoToOutpostCombatCallback);
		}
		else if (SurvivorType == SurvivorContainerModel.SurvivorType.WorldBossPVE || SurvivorType == SurvivorContainerModel.SurvivorType.WorldBossPVP || SurvivorType == SurvivorContainerModel.SurvivorType.WorldBoss)
		{
			StartWorldBossCombat();
		}
		else if (mapMissionModel != null)
		{
			AttackCommand attackCommand = new AttackCommand(mapMissionModel);
		MapMissionModel obj = mapMissionModel;
		if (obj != null && obj.MissionSpawnPointGroup.Category == MapCategory.Endless && EndlessModeHelpers.IsEndlessExpertMode())
		{
			attackCommand.Cashier = mapMissionModel.GetStartMissionExpertModeCashier();
		}
		else
		{
			attackCommand.Cashier = mapMissionModel.GetStartMissionCashier();
		}
		attackCommand.EndlessModeGameModeType = GameManager.Instance.playerModel.EndlessModeManager.EndlessModeGameModeType;
		ConsumeCurrencyCommandUtils.Execute(attackCommand, OnGoToCombatCallback);
		}
		else if (guildBattleMapMissionModel != null)
		{
			if (OfflineManager.IsLoadDataManager && OfflineManager.IsFreeAll)
			{
				OnGoToCombatGuildBattleCallback();
				return;
			}
			ConsumeCurrencyCommandUtils.Execute(new AttackGuildBattleMissionCommand
			{
				SectorId = guildBattleMapMissionModel.SectorIdOwner,
				UniqueMissionId = guildBattleMapMissionModel.Id,
				Cashier = guildBattleMapMissionModel.GetStartMissionCashier(GameManager.Instance.playerModel.manager)
			}, OnGoToCombatGuildBattleCallback);
		}
	}

	private void VisualizeSurvivalRest()
	{
		animatingSurvivalRest = true;
		survivalRestAnimationTime = 0f;
		survivorsAnimatingSurvivalRest.Clear();
		survivorsAnimatedSurvivalRest.Clear();
	}

	private void GoToOutpostCombat()
	{
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("map/start_mission");
		OutpostEditManager.StartOutpostAttack();
	}

	public override void OnBackButtonClicked()
	{
		if (survivorBeingAddedToTheTeam)
		{
			return;
		}
		if (survivorToReplaceIndex != -1)
		{
			if (teamSelectionSurvivorsListPanel != null && !TutorialView.Instance.IsWaitingForClick)
			{
				teamSelectionSurvivorsListPanel.ClosePanel();
			}
		}
		else
		{
			base.OnBackButtonClicked();
		}
	}

	public override async void OnClickClose()
	{
		if (!(await teamPresetSelectionPanel.Intercept()))
		{
			return;
		}
		if (SurvivorType == SurvivorContainerModel.SurvivorType.GvGDefenders && GetComponent<GvgDefendersTeamSelection>().GetUnsavedDefendersChanged().Any((bool x) => x))
		{
			ConfirmationPopup.ShowPopup(LocalizationManager.GetText("Popup.TeamSelect.GvgDefenders.RevertChanges.AlertTitle"), LocalizationManager.GetText("Popup.TeamSelect.GvgDefenders.RevertChanges.AlertText"), LocalizationManager.GetText("Popup.TeamSelect.GvgDefenders.RevertChanges.Confirm"), delegate
			{
				base.OnClickClose();
			}, LocalizationManager.GetText("Button.Cancel"), delegate
			{
			});
			return;
		}
		base.OnClickClose();
		if (OutpostMatchInfo != null && SurvivorType == SurvivorContainerModel.SurvivorType.CombatOutpost)
		{
			Helpers.ExecuteCommand(new SendSkipOutpostMatchMetricCommand(OutpostMatchInfo));
		}
	}

	public void OnClickFeatureHeroCall()
	{
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_click");
		NewPhonePopup.OpenRadiophoneFeaturePopup();
	}

	private void OnSurvivalRestCallback(TWDModelResult result)
	{
		if (result == TWDModelResult.OK)
		{
			UpdateSurvivalRestContainer();
			UpdateUI();
			if (teamSelectionSelectedSurvivorPanel != null)
			{
				teamSelectionSelectedSurvivorPanel.UpdateSlots();
			}
			VisualizeSurvivalRest();
		}
	}

	private void OnGoToOutpostCombatCallback(TWDModelResult result)
	{
		if (result == TWDModelResult.OK)
		{
			loadingCombat = true;
			GoToOutpostCombat();
		}
	}

	private void OnGoToCombatGuildBattleCallback(TWDModelResult result)
	{
		switch (result)
		{
		case TWDModelResult.OK:
		{
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("map/start_mission");
			EventManager.NotifyClick("StartMission");
			EventManager.NotifyEvent(EventManager.EventType.StartMission);
			MapMissionParameters missionInfo = guildBattleMapMissionModel.ToMissionParameters();
			DebugTWD.Log("Try LoadVisitModel PVE: " + missionInfo.MissionId, DebugType.Wars);
			GameManager.Instance.LoadVisitModel(VisitMode.PVE, missionInfo);
			SingularityMonoBehaviour<HUDManager>.Instance.CloseAllOpenPopupsAndDialogs();
			break;
		}
		case TWDModelResult.NotEnoughSurvivors:
			HUDNotification.Error(LocalizationManager.GetText("Notification.AttemptCombatWithoutSurvivors"));
			break;
		}
	}

	private void OnGoToCombatCallback(TWDModelResult result)
	{
		switch (result)
		{
		case TWDModelResult.OK:
		{
			loadingCombat = true;
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("map/start_mission");
			EventManager.NotifyClick("StartMission");
			EventManager.NotifyEvent(EventManager.EventType.StartMission);
			DebugTWD.Log("Try LoadVisitModel PVE: " + mapMissionModel.MissionId, DebugType.Wars);
			if (mapMissionModel.MissionSpawnPointGroup.Category == MapCategory.Story)
			{
				string missionNumber = $"Mission {mapMissionModel.MissionSpawnPointGroup.MissionSpawnPoints.IndexOf(mapMissionModel.MissionSpawnPoint) + 1}";
				string difficulty = ((mapMissionModel.MissionSpawnPointGroup.EpisodeDifficultyLevel == 2) ? "Hard" : ((mapMissionModel.MissionSpawnPointGroup.EpisodeDifficultyLevel == 3) ? "Nightmare" : "Normal"));
				SingularityMonoBehaviour<SDKManager>.Instance.PlayStoryMission(mapMissionModel.MissionSpawnPointGroup.DisplayName, missionNumber, difficulty);
			}
			MapMissionParameters missionInfo = mapMissionModel.ToMissionParameters();
			GameManager.Instance.LoadVisitModel(VisitMode.PVE, missionInfo);
			SingularityMonoBehaviour<HUDManager>.Instance.CloseAllOpenPopupsAndDialogs();
			break;
		}
		case TWDModelResult.NotEnoughSurvivors:
			HUDNotification.Error(LocalizationManager.GetText("Notification.AttemptCombatWithoutSurvivors"));
			break;
		}
	}

	private void UpdateSurvivalRestAnimations(float normalizedAnimationTime)
	{
		if (lastRestedSurvivorsList == null)
		{
			return;
		}
		for (int i = 0; i < lastRestedSurvivorsList.Count; i++)
		{
			SurvivorModel survivorModel = lastRestedSurvivorsList[i];
			if (!survivorsAnimatedSurvivalRest.Contains(survivorModel) && !survivorsAnimatingSurvivalRest.Contains(survivorModel))
			{
				survivorsAnimatingSurvivalRest.Add(survivorModel);
				teamSelectionSelectedSurvivorPanel.StartSurvivalRestAnimation(survivorModel);
			}
		}
		for (int j = 0; j < survivorsAnimatingSurvivalRest.Count; j++)
		{
			SurvivorModel survivorModel2 = survivorsAnimatingSurvivalRest[j];
			if (normalizedAnimationTime >= 1f)
			{
				if (teamSelectionSelectedSurvivorPanel != null)
				{
					teamSelectionSelectedSurvivorPanel.EndSurvivalRestAnimation(survivorModel2);
				}
				survivorsAnimatedSurvivalRest.Add(survivorModel2);
				survivorsAnimatingSurvivalRest.RemoveAt(j);
				j--;
			}
			else if (teamSelectionSelectedSurvivorPanel != null)
			{
				teamSelectionSelectedSurvivorPanel.UpdateSurvivalRestAnimation(survivorModel2, normalizedAnimationTime);
			}
		}
	}

	public override void Update()
	{
		base.Update();
		if (animatingSurvivalRest)
		{
			float num = Time.deltaTime;
			if (num > 0.2f)
			{
				num = 0.2f;
			}
			survivalRestAnimationTime += num;
			float normalizedAnimationTime = 1f;
			if (survivalMillisecondsToAnimateRest > 0)
			{
				normalizedAnimationTime = survivalRestAnimationTime / ((float)survivalMillisecondsToAnimateRest * 0.001f);
			}
			UpdateSurvivalRestAnimations(normalizedAnimationTime);
		}
	}

	private void OnMapModelChange(TWDGroupModelChild model, string changed, object args)
	{
		switch (changed)
		{
		case "GuildBattleMissionPvPEnemiesUpdated":
		case "GuildBattleNonPvpCompletionAdded":
		case "GuildBattlePvpCompletionAdded":
			if (args is GuildBattleMapMissionModel guildBattleMapMissionModel && this.guildBattleMapMissionModel != null && this.guildBattleMapMissionModel.Id == guildBattleMapMissionModel.Id)
			{
				UpdateUI();
			}
			break;
		}
	}

	private void OnMissionChange(TWDGroupModelChild model, string changed, object args)
	{
		if (changed == "GuildBattleMissionPvPEnemiesUpdated" && args is GuildBattleMapMissionModel guildBattleMapMissionModel && this.guildBattleMapMissionModel != null && this.guildBattleMapMissionModel.Id == guildBattleMapMissionModel.Id)
		{
			UpdateUI();
		}
	}

	public static List<SurvivorModel> GetSurvivorsForType(SurvivorContainerModel.SurvivorType survivorType)
	{
		switch (survivorType)
		{
		case SurvivorContainerModel.SurvivorType.Outpost:
			return GameManager.Instance.playerModel.SurvivorContainer.OutpostDefendingSurvivors;
		case SurvivorContainerModel.SurvivorType.GvGDefenders:
		{
			GvgDefendersTeamSelection gvgDefendersTeamSelection = UnityEngine.Object.FindObjectOfType<GvgDefendersTeamSelection>();
			if (gvgDefendersTeamSelection == null)
			{
				return GvgDefendersTeamSelection.GetDefaultGvgDefenders();
			}
			return gvgDefendersTeamSelection.GetCurrentSelectedTeam();
		}
		default:
			return GameManager.Instance.playerModel.SurvivorContainer.CombatSurvivors;
		}
	}

	private void OnLocalizationLanguageChanged(string newLanguage)
	{
		if ((bool)playButtonTable)
		{
			playButtonTable.repositionNow = true;
		}
	}

	private void StartWorldBossCombat()
	{
		PlayerModel playerModel = GameManager.Instance.playerModel;
		WorldBossModelManager worldBossModelManager = playerModel.WorldBossModelManager;
		List<string> worldBossParticipantSurvivorIds = GetWorldBossParticipantSurvivorIds(playerModel);
		if (SurvivorType == SurvivorContainerModel.SurvivorType.WorldBossPVE)
		{
			if (selectedMapMissionModel is WorldBossMissionModel worldBossMissionModel)
			{
				ExecuteWorldBossAttackAndEnterCombat(playerModel, worldBossModelManager, worldBossMissionModel.CapturePoint, worldBossMissionModel.Cell, worldBossParticipantSurvivorIds);
			}
		}
		else if (SurvivorType == SurvivorContainerModel.SurvivorType.WorldBossPVP)
		{
			if (!string.IsNullOrEmpty(WorldBossCapturePoint) && !string.IsNullOrEmpty(WorldBossCell))
			{
				if (WorldBossCellState == WorldBossPVPItemItem.CellState.Empty)
				{
					OccupyWorldBossEmptyCellCommand command = new OccupyWorldBossEmptyCellCommand(worldBossModelManager.GetCurrentSeasonId(), worldBossModelManager.GetCurrentCycleId(), WorldBossCapturePoint, WorldBossCell, worldBossParticipantSurvivorIds);
					StartCoroutine(WaitWorldBossOccupyReceiptAndClose(command));
				}
				else if (WorldBossCellState == WorldBossPVPItemItem.CellState.Uncross || WorldBossCellState == WorldBossPVPItemItem.CellState.GetOtherGroup)
				{
					ExecuteWorldBossAttackAndEnterCombat(playerModel, worldBossModelManager, WorldBossCapturePoint, WorldBossCell, worldBossParticipantSurvivorIds);
				}
			}
		}
		else if (SurvivorType == SurvivorContainerModel.SurvivorType.WorldBoss)
		{
			TWDModelResult result = Helpers.ExecuteCommand(new AttackWorldBossTankCommand(worldBossModelManager.GetCurrentSeasonId(), worldBossModelManager.GetCurrentCycleId(), worldBossParticipantSurvivorIds));
			OnGoToCombatWorldBossCallback(result);
		}
	}

	private static List<string> GetWorldBossParticipantSurvivorIds(PlayerModel player)
	{
		return (from survivor in player.SurvivorContainer.CombatSurvivors
			where survivor != null && !string.IsNullOrEmpty(survivor.IdForAnalytics)
			select survivor.IdForAnalytics).ToList();
	}

	private void ExecuteWorldBossAttackAndEnterCombat(PlayerModel player, WorldBossModelManager worldBossModelManager, string capturePoint, string cell, List<string> participantSurvivorIds)
	{
		AttackWorldBossCellCommand command = new AttackWorldBossCellCommand(worldBossModelManager.GetCurrentSeasonId(), worldBossModelManager.GetCurrentCycleId(), player.GuildId, capturePoint, cell, participantSurvivorIds);
		StartCoroutine(WaitWorldBossAttackReceiptAndEnterCombat(command));
	}

	private IEnumerator WaitWorldBossOccupyReceiptAndClose(OccupyWorldBossEmptyCellCommand command)
	{
		if (loadingCombat)
		{
			yield break;
		}
		SignalRClient signalRClient = SignalRClient.Instance;
		if (signalRClient == null)
		{
			yield break;
		}
		loadingCombat = true;
		CancelWorldBossAttackReceiptWait();
		worldBossAttackReceiptClient = signalRClient;
		worldBossAttackReceiptClient.OnCommandCompletedMessage += HandleWorldBossAttackCommandCompleted;
		if (Helpers.ExecuteCommand(command) != TWDModelResult.OK)
		{
			CancelWorldBossAttackReceiptWait();
			loadingCombat = false;
			EndWorldBossCombatEnterCover();
			HUDNotification.Info(LocalizationManager.GetText("World.Boss.Occupied.Tips"));
			RefreshWorldBossDetailSnapshot();
			Close();
			yield break;
		}
		int targetSequenceId = command.SequenceId;
		float waited = 0f;
		float timeout = GetWorldBossCommandTimeout(signalRClient);
		while (signalRClient != null && signalRClient.IsConnected && lastCompletedWorldBossAttackSequenceId < targetSequenceId && waited < timeout)
		{
			waited += Time.deltaTime;
			yield return null;
		}
		bool flag = lastCompletedWorldBossAttackSequenceId >= targetSequenceId;
		worldBossAttackReceiptCodes.TryGetValue(targetSequenceId, out var value);
		CancelWorldBossAttackReceiptWait();
		if (IsPopupAlive() && flag)
		{
			loadingCombat = false;
			EndWorldBossCombatEnterCover();
			switch (value)
			{
			case 70:
				Close();
				break;
			case 0:
				Close();
				break;
			}
		}
	}

	private IEnumerator WaitWorldBossAttackReceiptAndEnterCombat(AttackWorldBossCellCommand command)
	{
		if (loadingCombat)
		{
			yield break;
		}
		SignalRClient signalRClient = SignalRClient.Instance;
		if (signalRClient == null)
		{
			yield break;
		}
		loadingCombat = true;
		BeginWorldBossCombatEnterCover();
		CancelWorldBossAttackReceiptWait();
		worldBossAttackReceiptClient = signalRClient;
		worldBossAttackReceiptClient.OnCommandCompletedMessage += HandleWorldBossAttackCommandCompleted;
		TWDModelResult tWDModelResult = Helpers.ExecuteCommand(command);
		if (tWDModelResult != TWDModelResult.OK)
		{
			CancelWorldBossAttackReceiptWait();
			loadingCombat = false;
			EndWorldBossCombatEnterCover();
			OnGoToCombatWorldBossCallback(tWDModelResult);
			yield break;
		}
		int targetSequenceId = command.SequenceId;
		float waited = 0f;
		float timeout = GetWorldBossCommandTimeout(signalRClient);
		while (signalRClient != null && signalRClient.IsConnected && lastCompletedWorldBossAttackSequenceId < targetSequenceId && waited < timeout)
		{
			waited += Time.deltaTime;
			yield return null;
		}
		bool flag = lastCompletedWorldBossAttackSequenceId >= targetSequenceId;
		worldBossAttackReceiptCodes.TryGetValue(targetSequenceId, out var value);
		CancelWorldBossAttackReceiptWait();
		if (IsPopupAlive() && flag)
		{
			switch (value)
			{
			case 70:
				loadingCombat = false;
				EndWorldBossCombatEnterCover();
				Close();
				break;
			case 0:
				OnGoToCombatWorldBossCallback(TWDModelResult.OK);
				break;
			}
		}
	}

	private void HandleWorldBossAttackCommandCompleted(int code, int sequenceId)
	{
		lastCompletedWorldBossAttackSequenceId = Math.Max(lastCompletedWorldBossAttackSequenceId, sequenceId);
		worldBossAttackReceiptCodes[sequenceId] = code;
	}

	private void CancelWorldBossAttackReceiptWait()
	{
		if (worldBossAttackReceiptClient != null)
		{
			worldBossAttackReceiptClient.OnCommandCompletedMessage -= HandleWorldBossAttackCommandCompleted;
			worldBossAttackReceiptClient = null;
		}
		lastCompletedWorldBossAttackSequenceId = -1;
		worldBossAttackReceiptCodes.Clear();
	}

	private static float GetWorldBossCommandTimeout(SignalRClient signalRClient)
	{
		float result = signalRClient.CommandTimeout;
		ConfigData configData = GameManager.Instance?.gameEconomyData?.ConfigData;
		if (configData != null && configData.ReloadTimer > 0)
		{
			result = configData.ReloadTimer;
		}
		return result;
	}

	private void OnGoToCombatWorldBossCallback(TWDModelResult result)
	{
		switch (result)
		{
		case TWDModelResult.OK:
		{
			loadingCombat = true;
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("map/start_mission");
			EventManager.NotifyClick("StartMission");
			EventManager.NotifyEvent(EventManager.EventType.StartMission);
			WorldBossMissionModel worldBossMissionModel = (GameManager.Instance.playerModel.GetAttackTargetMissionModel() as WorldBossMissionModel) ?? (selectedMapMissionModel as WorldBossMissionModel);
			if (worldBossMissionModel == null || !worldBossMissionModel.HasValidMissionBinding())
			{
				loadingCombat = false;
				break;
			}
			MapMissionParameters missionInfo = worldBossMissionModel.ToMissionParameters();
			GameManager.Instance.LoadVisitModel(VisitMode.PVE, missionInfo);
			SingularityMonoBehaviour<HUDManager>.Instance.CloseAllOpenPopupsAndDialogs();
			break;
		}
		case TWDModelResult.NotEnoughSurvivors:
			HUDNotification.Error(LocalizationManager.GetText("Notification.AttemptCombatWithoutSurvivors"));
			break;
		}
	}

	public void OpenForWorldBoss(WorldBossMissionModel missionModel, SurvivorContainerModel.SurvivorType survivorType)
	{
		if (missionModel != null && (!base.gameObject.activeSelf || _selectedMapMissionModel != missionModel))
		{
			model = null;
			groupModelChild = null;
			_mapMissionModel = null;
			_guildBattleMapMissionModel = null;
			_selectedMapMissionModel = missionModel;
			startCashier = null;
			survivalRestCashier = null;
			SurvivorType = survivorType;
			Open();
		}
	}



	#region myparams
	private TweenPosition TeamSelectionTween;
	private Vector3 TeamSelectionTweenFromPos;
	#endregion

	#region mycode
	//public string missionID;

	[ContextMenu("Unlock GVGButton")]
	public void UnlockGVGButton()
	{
		if (challengeStartButton == null) return;

		var keysCount = GameManager.Instance.playerModel.GetCurrency(CurrencyType.GvGMissionKey).Value;

		if (keysCount < 1)
		{
			var challengeStartButtonUI = challengeStartButton.GetComponent<UIButton>();

			//PurchaseType.GvGMissionRetry, CurrencyType.GvGGas - красный газ
			//PurchaseType.RechargeCurrency,CurrencyType.ReplayToken - бензин
			//PurchaseType.GuildBattleAttackMission, CurrencyType.GvGMissionKey - мечи
			HelpersUI.SetButtonState(challengeStartButtonUI, UIButtonColor.State.Normal);

			CashierItem item = new CashierItem(PurchaseType.RechargeCurrency);
			item.SetCost(CurrencyType.GvGMissionKey, 1);

			int cost = UtilsMath.Clamp(1, 0, GameManager.Instance.playerModel.GetCurrency(CurrencyType.GvGMissionKey).Max);
			var cashier = Cashier.CreateOneItemCashier(GameManager.Instance.modelManager, PurchaseType.RechargeCurrency, CurrencyType.GvGMissionKey, cost);
			cashier.AddItem(item);
			challengeStartButton.UpdateUI(cashier);
		}
		StartBattle();
	}

	private void StartBattle()
	{
		var missionID = guildBattleMapMissionModel.Id;
		if (string.IsNullOrEmpty(missionID)) return;

		GuildBattleMapMissionModel gwMapMissionModel = GuildWarHelper.GetCurrentMapModel().GetMissionModel(missionID);

		if (gwMapMissionModel != null)
		{
			DebugTWD.Log("Execute PVE", DebugType.Wars);

			var result = ExecuteGoToCombatResult(gwMapMissionModel);
			if (result == TWDModelResult.OK)
			{
				DebugTWD.Log("ExecuteGoToCombatResult is OK", DebugType.Wars);
			}
			else
			{
				DebugTWD.Log("ExecuteGoToCombatResult is " + result.ToString(), DebugType.Wars);
			}
			OnGoToCombatGuildBattleCallbackMod(result, missionID);
		}
	}

	public TWDModelResult ExecuteGoToCombatResult(GuildBattleMapMissionModel gwMapMissionModel)
	{
		TWDModelManager tWDModelManager = GameManager.Instance.modelManager;
		PlayerModel player = tWDModelManager.Player;

		player.MapContainerModel.ClearAttackTargetMissionData();
		GuildModel guildModel = player.GuildModel;
		if (!player.IsGuildMember)
		{
			DebugTWD.Log("AttackGuildBattleMissionCommand: Player is not a Guild Member", DebugType.Wars);
			return TWDModelResult.Error;
		}

		if (guildModel.GuildWarModel == null)
		{
			DebugTWD.Log("AttackGuildBattleMissionCommand: GuildWarModel is null", DebugType.Wars);
			return TWDModelResult.Error;
		}

		if (guildModel.GuildWarModel.CurrentBattle == null)
		{
			DebugTWD.Log("AttackGuildBattleMissionCommand: CurrentBattle is null", DebugType.Wars);
			return TWDModelResult.Error;
		}

		if (guildModel.GuildWarModel.CurrentBattle.CurrentMapModel == null)
		{
			DebugTWD.Log("AttackGuildBattleMissionCommand: CurrentMapModel is null", DebugType.Wars);
			return TWDModelResult.Error;
		}

		GuildBattleModel currentBattle = player.GuildWarModel.CurrentBattle;
		var UniqueMissionId = gwMapMissionModel.Id;
		var SectorId = gwMapMissionModel.SectorIdOwner;
		GuildBattleMapSectorModel sectorModel = currentBattle.CurrentMapModel.GetSectorModel(SectorId);
		GuildBattleMapMissionModel missionModel = currentBattle.CurrentMapModel.GetMissionModel(UniqueMissionId);
		if (missionModel == null)
		{
			DebugTWD.Log("AttackGuildBattleMissionCommand: MapMission is null :" + UniqueMissionId + " " + SectorId, DebugType.Wars);
			return TWDModelResult.Error;
		}

		if (sectorModel == null)
		{
			DebugTWD.Log("AttackGuildBattleMissionCommand: MapSector is null :" + UniqueMissionId + " " + SectorId, DebugType.Wars);
			return TWDModelResult.Error;
		}

		TWDModelResult tWDModelResult = AttackMission(currentBattle.CurrentMapModel, missionModel, sectorModel, player.SurvivorContainer);

		if (tWDModelResult == TWDModelResult.OK)
		{
			DropEventDefinition.DropEventType eventType = DropEventDefinition.DropEventType.MissionScavenge;
			int missionLevel = missionModel.MissionLevel;
			DropEventDefinition.DropEventContext context = DropEventDefinition.DropEventContext.Deadly;
			LootEntryGenParams lootParams = new LootEntryGenParams
			{
				eventType = eventType,
				targetLevel = missionLevel,
				tag = DropEventDefinition.DropEventTag.None,
				context = context
			};
			if (tWDModelManager.Player.SurvivorContainer != null && tWDModelManager.Player.SurvivorContainer.CombatSurvivors != null && tWDModelManager.Player.SurvivorContainer.CombatSurvivors.Count > 0)
			{
				SurvivorModel firstSlotSurvivor = tWDModelManager.Player.SurvivorContainer.CombatSurvivors[0];
				AttackCommand.AddTraitModifiers(ref lootParams, tWDModelManager.Player, firstSlotSurvivor);
			}
		}

		tWDModelManager.Player.LastVisitDebugInfo = "";
		return tWDModelResult;
	}

	public TWDModelResult AttackMission(GuildBattleMapModel mapModel, GuildBattleMapMissionModel mapMissionModel, GuildBattleMapSectorModel sectorModel, SurvivorContainerModel container)
	{
		if (container.CombatSurvivors.Count < 1)
		{
			return TWDModelResult.NotEnoughSurvivors;
		}

		if (mapMissionModel != null)
		{
			var GuildBattleModel = GameManager.Instance.playerModel.GvGSeasonModelPlayer.GuildWarModelPlayer.GuildBattleModel;
			GuildBattleModel.AttackTargetMission.AttackMission(mapMissionModel);
			if (GuildBattleModel.AttackTargetMission.MissionModel == null)
			{
				return TWDModelResult.Error;
			}

			if (!GuildBattleModel.AttackTargetMission.MissionModel.SectorModelOwner.CanBeUnlocked(mapModel))
			{
				return TWDModelResult.Error;
			}

			GameManager.Instance.playerModel.ShouldConsumeMissionCurrency = true;
			GuildBattleModel.CurrentMissionRetriedAttempts = 0;
			GuildBattleModel.RetryMission = false;
			return TWDModelResult.OK;
		}

		return TWDModelResult.InvalidPosition;
	}

	private void OnGoToCombatGuildBattleCallbackMod(TWDModelResult result, string missionID)
	{
		switch (result)
		{
			case TWDModelResult.OK:
				{
					DebugTWD.Log("Try LoadVisitModel PVE", DebugType.Wars);

					EventManager.NotifyClick("StartMission");
					EventManager.NotifyEvent(EventManager.EventType.StartMission);
					var gwMapMissionModel = GuildWarHelper.GetCurrentMapModel().GetMissionModel(missionID);

					MapMissionParameters missionInfo = gwMapMissionModel.ToMissionParameters();
					GameManager.Instance.LoadVisitModel(VisitMode.PVE, missionInfo);
					SingularityMonoBehaviour<HUDManager>.Instance.CloseAllOpenPopupsAndDialogs();
					break;
				}
			case TWDModelResult.NotEnoughSurvivors:
				DebugTWD.Log("Error : NotEnoughSurvivors", DebugType.Wars);

				HUDNotification.Error(LocalizationManager.GetText("Notification.AttemptCombatWithoutSurvivors"));
				break;
		}
	}

	//Не помню для чего
	private void OnGoToCombatGuildBattleCallback()
	{
		if (!OfflineManager.IsMissionModBuild)
		{
			MyTools.OpenAlert("!!!Прохождение миссий недоступно в этой версии!!!");
			return;
		}

		GuildBattleModel currentBattle = GameManager.Instance.playerModel.GuildWarModel.CurrentBattle;
		GuildBattleMapSectorModel sectorModel = currentBattle.CurrentMapModel.GetSectorModel(guildBattleMapMissionModel.SectorIdOwner);
		GuildBattleMapMissionModel missionModel = currentBattle.CurrentMapModel.GetMissionModel(guildBattleMapMissionModel.Id);

		TWDModelResult result = GameManager.Instance.playerModel.GvGSeasonModelPlayer.GuildWarModelPlayer.GuildBattleModel.AttackMission(currentBattle.CurrentMapModel, missionModel, sectorModel, GameManager.Instance.playerModel.SurvivorContainer);
		if (result == TWDModelResult.OK)
		{
			DropEventDefinition.DropEventType eventType = DropEventDefinition.DropEventType.MissionScavenge;
			int missionLevel = missionModel.MissionLevel;
			DropEventDefinition.DropEventContext context = DropEventDefinition.DropEventContext.Deadly;
			LootEntryGenParams lootParams = new LootEntryGenParams
			{
				eventType = eventType,
				targetLevel = missionLevel,
				tag = DropEventDefinition.DropEventTag.None,
				context = context
			};
			if (GameManager.Instance.playerModel.SurvivorContainer != null && GameManager.Instance.playerModel.SurvivorContainer.CombatSurvivors != null && GameManager.Instance.playerModel.SurvivorContainer.CombatSurvivors.Count > 0)
			{
				SurvivorModel firstSlotSurvivor = GameManager.Instance.playerModel.SurvivorContainer.CombatSurvivors[0];
				AttackCommand.AddTraitModifiers(ref lootParams, GameManager.Instance.playerModel, firstSlotSurvivor);
			}

			GameManager.Instance.playerModel.LastVisitDebugInfo = "";
			GameManager.Instance.playerModel.MapContainerModel.ClearAttackTargetMissionData();

			EventManager.NotifyClick("StartMission");
			EventManager.NotifyEvent(EventManager.EventType.StartMission);
			MapMissionParameters missionInfo = guildBattleMapMissionModel.ToMissionParameters();
			DebugTWD.Log("Try LoadVisitModel PVE", DebugType.Wars);
			GameManager.Instance.LoadVisitModel(VisitMode.PVE, missionInfo);
			SingularityMonoBehaviour<HUDManager>.Instance.CloseAllOpenPopupsAndDialogs();
		}
		else
		{
			DebugTWD.LogError("OnGoToCombatGuildBattleCallback error");
		}
	}
	#endregion
}
