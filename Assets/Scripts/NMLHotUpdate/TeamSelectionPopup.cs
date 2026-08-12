using BaseModel;
using Client.Camp;
using NextGames.Sdk.AssetBundleManager;
using System;
using System.Collections.Generic;
using System.Linq;
using TwdCustomMod;
using TWDModel;
using UnityEngine;

public class TeamSelectionPopup : HUDElement, ISurvivorSlotProvider
{
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

	[SerializeField]
	private GameObject CombatOutpostInfo;

	[SerializeField]
	private GameObject OutpostInfo;

	[SerializeField]
	private UILabel outpostMessage;

	[SerializeField]
	[Header("Combat Info")]
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

	[Tooltip("Container containing all missions locked stuff.")]
	[SerializeField]
	[Space(10f)]
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

	[SerializeField]
	[Header("Survival mode specific")]
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

	[SerializeField]
	[Header("Guild Battle Enemy Info")]
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

	private List<SurvivorModel> lastRestedSurvivorsList;

	private int shownGvGMissionCompletion;

	public SurvivorContainerModel.SurvivorType SurvivorType { get; set; }

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
		if (outpostMessage != null)
		{
			bool disableOutpostHeroLimits = GameManager.Instance.gameEconomyData.ConfigData.DisableOutpostHeroLimits;
			outpostMessage.text = LocalizationManager.GetText(disableOutpostHeroLimits ? "Popup.TeamSelect.Defenders.Message.Free" : "Popup.TeamSelect.Defenders.Message");
		}
		if (SurvivorType == SurvivorContainerModel.SurvivorType.Combat || SurvivorType == SurvivorContainerModel.SurvivorType.CombatSurvival || SurvivorType == SurvivorContainerModel.SurvivorType.CombatGuildBattle)
		{
			UpdateUICombat();
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

	public void UpdateUICombat()
	{
		SurvivalMissionConfig survivalMissionConfig = null;
		survivalMissionConfig = selectedMapMissionModel.SolveSurvivalConfigForCurrentMission();
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
		else
		{
			GoToCombat();
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
