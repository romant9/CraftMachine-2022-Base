using BaseModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TwdCustomMod;
using TWDModel;
using UnityEngine;

public class SurvivorInfoPopup : HUDElement
{
	[Header("Content and Panels")]
	[SerializeField]
	[Tooltip("Survivior Name Prefab")]
	private GameObject surviviorNamePrefab;

	[SerializeField]
	[Tooltip("Survivior Statistics")]
	private SurvivorStatisticsPanel surviviorStatistics;

	[Tooltip("Survivior Traits")]
	public SurvivorTraitsList survivorTraitsList;

	[SerializeField]
	private SurvivorBadgesPanel survivorBadgesPanel;

	[SerializeField]
	private SurvivorSurvivalManualPanel survivalManualPanel;

	[Tooltip("Right side panel containing traits and badges")]
	public SurvivorInfoRightSidePanel survivorRightSidePanel;

	[SerializeField]
	[Tooltip("Survivior Outfits View")]
	private SurvivorOutfitsView survivorOutfitsView;

	[SerializeField]
	[Tooltip("Hero Skins View")]
	private HeroSkinsView heroSkinsView;

	[Tooltip("Upgrade View")]
	[SerializeField]
	private SurvivorUpgradePanel upgradePanel;

	[Tooltip("Share Content")]
	[SerializeField]
	private GameObject sharePanel;

	[SerializeField]
	private UITexture shareBadge;

	[SerializeField]
	private GameObject badgeNotificationContainer;

	[SerializeField]
	private UILabel badgeNotificationCount;

	[Header("Survivor Upgrade Panels")]
	[SerializeField]
	private SurvivorUpgradeView levelUpPanel;

	[SerializeField]
	private SurvivorUpgradeView promotedPanel;

	[SerializeField]
	private SurvivorUpgradeView promotedPanelNoTrait;

	[SerializeField]
	private SurvivorUpgradeView traitUpgradePanel;

	[Header("Other Content")]
	[SerializeField]
	private GameObject SurvivalManualNotice;

	[SerializeField]
	private SurvivorTraitRerollView traitRerollPanel;

	[SerializeField]
	private SurvivorRarityAndClassPanel rarityAndClass;

	[SerializeField]
	private SurvivorInfoUnlockView unlockView;

	[SerializeField]
	private GameObject trainingLockedParent;

	[SerializeField]
	private UILabel trainingLockedUILabel;

	[SerializeField]
	private UILabel survivorDescriptionLabel;

	[SerializeField]
	private GameObject hideForOutfitPreviewContainer;

	[Header("Top Buttons")]
	[SerializeField]
	private UIButton closeButton;

	[SerializeField]
	private GameObject closeButtonPanel;

	[Header("Bottom Buttons")]
	[SerializeField]
	private UIButtonExtended openTrainButton;

	[SerializeField]
	private UIButtonExtended speedUpButton;

	[SerializeField]
	private PayButton speedUpPayButton;

	[SerializeField]
	private UIButtonExtended trainButton;

	[SerializeField]
	private UIButtonExtended trainInstanButton;

	[SerializeField]
	private UIButtonExtended trainInstantWithTokensButton;

	[SerializeField]
	private GameObject trainInstantContainer;

	[SerializeField]
	private UIButtonExtended cancelButton;

	public UIButtonWithLabelAndIcon promoteButton;

	public UIButtonWithLabelAndIcon upgradeButton;

	[SerializeField]
	private UILabel maxUpgradesLabel;

	[SerializeField]
	private UIButtonExtended retireButton;

	[SerializeField]
	private UIButtonExtended outfitButton;

	public UIButtonExtended shareButton;

	[SerializeField]
	private UIButtonExtended shareUnlockButton;

	[SerializeField]
	private UIButtonExtended skipUnlockButton;

	[SerializeField]
	private UIButtonExtended genericOkButton;

	[SerializeField]
	private UIButtonExtended infoButton;

	[SerializeField]
	private UIButtonWithLabel previewHeroStatsButton;

	[SerializeField]
	private UIButtonWithLabel previewHeroStatsReturnButton;

	[SerializeField]
	private UIButtonWithLabelAndIcon unlockHeroUiButton;

	[SerializeField]
	private UIButtonWithLabelAndIcon moreTokensUiButton;

	[SerializeField]
	private UIButton favoriteToggleButton;

	[SerializeField]
	private GameObject survivorHealEffect;

	[Header("Accept From Mission Content")]
	[SerializeField]
	private SurvivorAcceptPanel acceptFromMissionParent;

	[SerializeField]
	private UIButtonExtended acceptButton;

	[SerializeField]
	private UIButtonExtended featureHeroInfoButton;

	[SerializeField]
	private UIButtonExtended rejectButton;

	[SerializeField]
	private UIButtonExtended manageButton;

	[SerializeField]
	private UIButtonExtended nextSurvivorButton;

	[SerializeField]
	private UIButtonExtended previousSurvivorButton;

	[SerializeField]
	private UIButtonExtended bounsButton;

	[Header("Current State")]
	public SurvivorInfoStateBase.States currentStateMachineState;

	private GameObject survivorName;

	private SurvivorNamePanel survivorNamePanel;

	private UIStateMachine stateMachine;

	private SurvivorInfoStateBase.States StateAfterTraitUpgrade;

	private string upgradeAudioEventName = "";

	private MedicTentModel medicTentModelCached;

	private bool acceptOrRejectBlock;

	private bool shareRewardGiven;

	private BadgeModel currentEquipBadgeModel;

	private ISurvivorFilterList currentSurvivorFilterList;

	public bool disableClose { get; set; }

	public SurvivorModel survivorModel { get; private set; }

	public static SurvivorModel survivorModelFromMission { get; private set; }

	public static bool AllowWeapons { get; set; }

	private TutorialModel tutorialModel => GameManager.Instance.playerModel.Tutorial;

	private MedicTentModel medicTentModel
	{
		get
		{
			if (medicTentModelCached == null)
			{
				medicTentModelCached = GameManager.Instance.playerModel.Camp.GetBuilding("MedicTent") as MedicTentModel;
			}
			return medicTentModelCached;
		}
	}

	public void OpenForModel(ModelObject model, ISurvivorFilterList currentSurvivorFilterList = null)
	{
		UIEvent.Send("OnSurvivorInfoOpen");
		this.currentSurvivorFilterList = currentSurvivorFilterList;
		acceptOrRejectBlock = false;
		shareRewardGiven = false;
		AllowWeapons = true;
		bool isLimited = currentStateMachineState == SurvivorInfoStateBase.States.SurvivorOverviewLimited;
		surviviorStatistics.IsLimited = isLimited;
		rarityAndClass.IsLimited = isLimited;

		if (OfflineManager.IsLoadDataManager)
		{
			SurvivorManagementPopUp survivorManagementPopUp = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CampTrainingGrounds, null, createIfNotExist: false) as SurvivorManagementPopUp;
			if (survivorManagementPopUp != null && survivorManagementPopUp.IsOpen)
			{
				survivorManagementPopUp.SetSurvivorListPanelVisibility(visibility: false);
			}
			if (RebuildPortraitButton != null && !OfflineManager.IsUsePortraitManager)
			{
				RebuildPortraitButton.gameObject.SetActive(false);
			}
		}

		AssetBundlePrepare(model);
	}

	private async void AssetBundlePrepare(ModelObject model)
	{
		while (SingularityMonoBehaviour<AssetBundleController>.Instance.LoadingAssetBundles)
		{
			await Task.Yield();
		}
		base.OpenForModel(model);
		survivorModel = model as SurvivorModel;
		UIEvent.Send("OnSurvivorInfoOpen", survivorModel);
		UIEvent.OnUIEvent -= OnUIEvent;
		UIEvent.OnUIEvent += OnUIEvent;
		if (medicTentModel != null)
		{
			medicTentModel.Changed -= OnMedicTentChanged;
			medicTentModel.Changed += OnMedicTentChanged;
		}
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("camp/open_trainingground");
		if (OfflineManager.IsLoadDataManager)
		{
			survivorNamePanel = surviviorNamePrefab.GetComponent<SurvivorNamePanel>();
		}
		else
		{
			if (survivorName == null && surviviorNamePrefab != null)
			{
				survivorName = base.gameObject.AddChild(surviviorNamePrefab);
				NGUITools.SetLayer(survivorName, base.gameObject.layer);
				survivorNamePanel = survivorName.GetComponentInChildren<SurvivorNamePanel>();
			}
		}

		SetupButtons();
		InitStateMachine();
		if (survivorModel.PendingReroll)
		{
			currentStateMachineState = SurvivorInfoStateBase.States.SurvivorTraitRerolled;
		}
		else if (currentStateMachineState == SurvivorInfoStateBase.States.SurvivorTraitRerolled)
		{
			currentStateMachineState = SurvivorInfoStateBase.States.SurvivorOverview;
		}
		SetState(currentStateMachineState);
		UpdateUI();
		TutorialView.Instance.UpdateSuggestion();
	}

	public static List<SurvivorModel> GetSurvivorsFromCards(List<UIListCard<SurvivorModel>> cards)
	{
		List<SurvivorModel> list = new List<SurvivorModel>();
		for (int i = 0; i < (cards?.Count ?? 0); i++)
		{
			if (cards[i] != null && cards[i].Item != null)
			{
				list.Add(cards[i].Item);
			}
		}
		return list;
	}

	public static List<string> GetSurvivorsFromCards(List<SurvivorCardHeroLocked> cards)
	{
		List<string> list = new List<string>();
		for (int i = 0; i < (cards?.Count ?? 0); i++)
		{
			if (cards[i] != null && cards[i].ActorDefinition != null)
			{
				list.Add(cards[i].ActorDefinition.ID);
			}
		}
		return list;
	}

	public bool CanSwitchSurvivor()
	{
		if (currentSurvivorFilterList != null)
		{
			return currentSurvivorFilterList.CanSwitchSurvivor();
		}
		return false;
	}

	private void OnNextSurvivorClicked(UIButton button)
	{
		SurvivorModel survivorModel = null;
		if (currentSurvivorFilterList != null)
		{
			survivorModel = currentSurvivorFilterList.GetNextSurvivor(model as SurvivorModel);
		}
		if (survivorModel != null)
		{
			OpenForModel(survivorModel, currentSurvivorFilterList);
		}
	}

	private void OnPreviousSurvivorClicked(UIButton button)
	{
		SurvivorModel survivorModel = null;
		if (currentSurvivorFilterList != null)
		{
			survivorModel = currentSurvivorFilterList.GetPreviousSurvivor(model as SurvivorModel);
		}
		if (survivorModel != null)
		{
			OpenForModel(survivorModel, currentSurvivorFilterList);
		}
	}

	private void OnBounsButtonClicked(UIButton button)
	{
		HUDElement hUDElement = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.BounsPopup);
		if ((bool)hUDElement)
		{
			hUDElement.OpenForModel(survivorModel);
		}
	}

	private void OnMaxPreviewClicked(UIButton button)
	{
		if (!(model is SurvivorModel survivorModel))
		{
			return;
		}
		if (survivorModel.SurvivorRarityLevel <= survivorModel.StartingRarityLevel)
		{
			int heroPreviewSurvivorRarityLevel = GameManager.Instance.gameEconomyData.ConfigData.HeroPreviewSurvivorRarityLevel;
			if (heroPreviewSurvivorRarityLevel > 0)
			{
				survivorModel.SurvivorRarityLevel = heroPreviewSurvivorRarityLevel;
			}
			int heroPreviewSurvivorLevel = GameManager.Instance.gameEconomyData.ConfigData.HeroPreviewSurvivorLevel;
			if (heroPreviewSurvivorLevel > 0)
			{
				survivorModel.Level = heroPreviewSurvivorLevel;
			}
		}
		else
		{
			survivorModel.SurvivorRarityLevel = survivorModel.StartingRarityLevel;
			survivorModel.Level = survivorModel.StartingLevel;
		}
		survivorModel.InitUpgradeTraits();
		survivorModel.SetupMockTraits();
		OpenForModel(survivorModel, currentSurvivorFilterList);
	}

	private void OnHeroUnlockClicked(UIButton button)
	{
		if (survivorModel != null && !HeroUnlockHelper.UnlockHero(survivorModel.Definition))
		{
			MissingTokensPopup missingTokensPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.MissingTokensPopup) as MissingTokensPopup;
			if (missingTokensPopup != null && TutorialView.Instance != null && !TutorialView.Instance.Running)
			{
				missingTokensPopup.IsHeroLocked = true;
				missingTokensPopup.IsHeroContent = true;
				missingTokensPopup.Open();
			}
		}
	}

	private void OnMoreTokensClicked(UIButton button)
	{
		MissingTokensPopup.OpenForRadioForMissingTokens();
	}

	public override void OpenWithStateData(object data)
	{
		SurvivorInfoPopupStateData survivorInfoPopupStateData = data as SurvivorInfoPopupStateData;
		if (data != null && survivorInfoPopupStateData.model != null && survivorInfoPopupStateData.stateMachineHistory != null)
		{
			SurvivorInfoPopup survivorInfoPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CampSurvivorInfoPopup) as SurvivorInfoPopup;
			if (survivorInfoPopup != null)
			{
				survivorInfoPopup.currentStateMachineState = survivorInfoPopupStateData.state;
				survivorInfoPopup.OpenForModel(survivorInfoPopupStateData.model, survivorInfoPopupStateData.currentFilter);
				survivorInfoPopup.stateMachine.SetHistory(survivorInfoPopupStateData.stateMachineHistory);
			}
		}
	}

	public static void HandleSurvivorUpgradeViewed(SurvivorModel model)
	{
		if (!(CampView.Instance != null))
		{
			return;
		}
		TrainingGroundView trainingGroundView = CampView.Instance.CampViewBuildings.FindBuildingViewOfType<TrainingGroundView>() as TrainingGroundView;
		if (trainingGroundView != null && trainingGroundView.Model != null && trainingGroundView.Model is ModelUpgraderBuildingModel && trainingGroundView.Model is ModelUpgraderBuildingModel { UpgradedUnseenModel: not null } modelUpgraderBuildingModel && modelUpgraderBuildingModel.UpgradedUnseenModel == model)
		{
			Helpers.ExecuteCommand(new UpgradedModelViewedCommand(modelUpgraderBuildingModel));
			if (trainingGroundView.GetDoneIndicator() != null)
			{
				trainingGroundView.GetDoneIndicator().Destroy();
			}
		}
	}

	public override void UpdateUI()
	{
		if (stateMachine != null)
		{
			stateMachine.UpdateUI();
		}
		if (survivorModel.IsUpgrading())
		{
			survivorModel.Changed -= OnSurvivorModelChanged;
			survivorModel.Changed += OnSurvivorModelChanged;
		}
		int equippableBadgeSlotCount = survivorModel.BadgeContainer.GetEquippableBadgeSlotCount();
		Helpers.GameObjectSetActive(badgeNotificationContainer, equippableBadgeSlotCount > 0);
		HelpersUI.SetContentToLabel(badgeNotificationCount, equippableBadgeSlotCount.ToString());
		Helpers.GameObjectSetActive(SurvivalManualNotice, Helpers.IsRedSurvivalManual_Hero(survivorModel));
	}

	public void SetAllowRename(bool allow)
	{
		if (survivorNamePanel != null)
		{
			survivorNamePanel.EnableNameInput(allow);
		}
	}

	public void UpgradeSurvivorTraitCallback(TWDModelResult result)
	{
		if (result != TWDModelResult.Cancelled)
		{
			if (StateAfterTraitUpgrade != SurvivorInfoStateBase.States.None)
			{
				SetState(StateAfterTraitUpgrade);
			}
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent(upgradeAudioEventName);
			UIEvent.Send("OnSurvivorPromote", survivorModel);
			EventManager.NotifyClick("Promote");
		}
		StateAfterTraitUpgrade = SurvivorInfoStateBase.States.None;
	}

	public void ShowBadges()
	{
		if (survivorRightSidePanel.GetSelectedIndex() == 0 || survivorRightSidePanel.GetSelectedIndex() == 2)
		{
			SetState(SurvivorInfoStateBase.States.SurvivosBadgesOverview);
			currentStateMachineState = SurvivorInfoStateBase.States.SurvivosBadgesOverview;
		}
	}

	public override void OnClickClose()
	{
		if (disableClose)
		{
			return;
		}
		TooltipManager.HideAll();
		if (OfflineManager.IsLoadDataManager)
		{
			DebugTWD.LogMycode("if (OfflineManager.IsLoadDataManager)");
			var tween = CraftSettings.Instance.TeamSelectionTween;
			if (tween && tween.value == tween.from)
			{
				tween.PlayForward();
			}
		}
		if (survivorRightSidePanel.GetSelectedIndex() == 1)
		{
			survivorRightSidePanel.SetSelectedIndex(0);
			OnToggleSetChange(null);
		}
		else if (!(stateMachine != null) || stateMachine.currentState == null || stateMachine.currentState.AllowExit())
		{
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_dialog_exit");
			if (GetCurrentState() == SurvivorInfoStateBase.States.SurvivorTrainDone)
			{
				HandleSurvivorUpgradeViewed(survivorModel);
			}
			UIEvent.Send("SurvivorListRefreshed");
			reactivateTrainingGroundPopup();
			base.OnClickClose();
		}
	}

	public override void Close()
	{
		disableClose = false;
		EventManager.NotifyClick("Close");
		EventManager.NotifyClick("Back");
		Clear();
		UIEvent.Send("OnSurvivorInfoClosed");
		reactivateTrainingGroundPopup();
		base.Close();
	}

	private void OnDestroy()
	{
		reactivateTrainingGroundPopup();
		Clear();
	}

	private void Clear()
	{
		if ((bool)SingularityMonoBehaviour<FullscreenActorOverlay>.Instance)
		{
			SingularityMonoBehaviour<FullscreenActorOverlay>.Instance.close();
		}
		if (survivorModel != null)
		{
			survivorModel.Changed -= OnSurvivorModelChanged;
		}
		if (medicTentModelCached != null)
		{
			medicTentModelCached.Changed -= OnMedicTentChanged;
		}
		if (stateMachine != null)
		{
			stateMachine.Clear();
		}
		if (currentSurvivorFilterList != null)
		{
			currentSurvivorFilterList.Clear();
		}
		UIEvent.OnUIEvent -= OnUIEvent;
		currentEquipBadgeModel = null;
	}

	private void OnSurvivorModelChanged(ModelObject model, string changed, object args)
	{
		if (survivorModel != null && args != null && args == survivorModel && changed == "ActionFinishedEvent")
		{
			SetState(SurvivorInfoStateBase.States.SurvivorTrainDone);
			return;
		}
		switch (changed)
		{
		case "UpgradingItemReady":
			UpdateUI();
			break;
		case "NewItemStartedUpgrading":
			UpdateUI();
			break;
		case "UpgradingItemCancelled":
			UpdateUI();
			break;
		}
	}

	private void OnMedicTentChanged(ModelObject model, string changed, object args)
	{
		if (survivorModel != null && args != null && args == survivorModel && changed == "ActionFinishedEvent")
		{
			SetState(GetCurrentState(), forceUpdate: true);
		}
	}

	private void SetupButtons()
	{
		if (tutorialModel != null && TutorialView.Instance.Running)
		{
			Helpers.GameObjectSetActive(trainInstantContainer, tutorialModel.GetCurrentStepDefinition.Id >= 6);
		}
		if (openTrainButton != null && survivorModel != null)
		{
			openTrainButton.SetClickCallback(OnOpenTrainClicked);
		}
		if (speedUpButton != null && survivorModel != null)
		{
			speedUpButton.SetClickCallback(OnSpeedUpClicked);
		}
		if (trainButton != null)
		{
			trainButton.SetClickCallback(OnTrainClicked);
		}
		if (trainInstanButton != null)
		{
			trainInstanButton.SetClickCallback(OnTrainInstantClicked);
		}
		if (trainInstantWithTokensButton != null)
		{
			trainInstantWithTokensButton.SetClickCallback(OnTrainInstantWithTokensClicked);
		}
		if (cancelButton != null)
		{
			cancelButton.SetClickCallback(OnCancelClicked);
		}
		if (retireButton != null)
		{
			retireButton.SetClickCallback(OnRetireClicked);
		}
		if (outfitButton != null)
		{
			outfitButton.SetClickCallback(OnOutfitClicked);
		}
		if (shareButton != null)
		{
			shareButton.SetClickCallback(OnShareClicked);
		}
		if (infoButton != null)
		{
			infoButton.SetClickCallback(OnBadgeInfoClicked);
		}
		if (shareUnlockButton != null)
		{
			shareUnlockButton.SetClickCallback(OnShareUnlockClicked);
		}
		if (skipUnlockButton != null)
		{
			skipUnlockButton.SetClickCallback(OnSkipUnlockClicked);
		}
		if (promoteButton != null)
		{
			promoteButton.SetClickCallback(OnUpgradeClicked);
		}
		if (upgradeButton != null)
		{
			upgradeButton.SetClickCallback(OnUpgradeClicked);
		}
		if (acceptButton != null)
		{
			acceptButton.SetClickCallback(OnAcceptFromMissionClicked);
		}
		if (featureHeroInfoButton != null)
		{
			featureHeroInfoButton.SetClickCallback(OnClickFeatureInfoButton);
		}
		if (rejectButton != null)
		{
			rejectButton.SetClickCallback(OnRejectFromMissionClicked);
		}
		if (manageButton != null)
		{
			manageButton.SetClickCallback(OnManageClicked);
		}
		if (genericOkButton != null)
		{
			genericOkButton.SetClickCallback(OnGenericOkClicked);
		}
		if (nextSurvivorButton != null)
		{
			nextSurvivorButton.SetClickCallback(OnNextSurvivorClicked);
		}
		if (previousSurvivorButton != null)
		{
			previousSurvivorButton.SetClickCallback(OnPreviousSurvivorClicked);
		}
		if (bounsButton != null)
		{
			bounsButton.SetClickCallback(OnBounsButtonClicked);
		}
		if (previewHeroStatsButton != null)
		{
			previewHeroStatsButton.SetClickCallback(OnMaxPreviewClicked);
		}
		if (previewHeroStatsReturnButton != null)
		{
			previewHeroStatsReturnButton.SetClickCallback(OnMaxPreviewClicked);
		}
		if (unlockHeroUiButton != null)
		{
			unlockHeroUiButton.SetClickCallback(OnHeroUnlockClicked);
		}
		if (moreTokensUiButton != null)
		{
			moreTokensUiButton.SetClickCallback(OnMoreTokensClicked);
		}
		if (survivorRightSidePanel != null)
		{
			survivorRightSidePanel.SetChangeCallback(OnToggleSetChange, onlyFromClicks: true);
		}
		if (survivorBadgesPanel != null)
		{
			BuildingModel building = GameManager.Instance.playerModel.Camp.GetBuilding("Residence");
			bool flag = building != null;
			bool flag2 = false;
			if (building != null)
			{
				flag2 = building.IsUpgrading;
			}
			if (survivorBadgesPanel.craftBadgesButton != null && Helpers.GameObjectSetActive(survivorBadgesPanel.craftBadgesButton, flag && !flag2))
			{
				survivorBadgesPanel.craftBadgesButton.SetClickCallback(OnClickedCraftBadges);
			}
		}
	}

	private void InitStateMachine()
	{
		if (stateMachine == null)
		{
			stateMachine = UIStateMachine.AddTo(base.gameObject);
		}
		if (stateMachine != null)
		{
			stateMachine.Clear();
			stateMachine.AddState(new SurvivorInfoStateOverview());
			stateMachine.AddState(new SurvivorInfoStateOverviewLimited());
			stateMachine.AddState(new SurvivorInfoStateTrainPreview());
			stateMachine.AddState(new SurvivorInfoStateTrainDone());
			stateMachine.AddState(new SurvivorInfoStateUpgradeDone());
			stateMachine.AddState(new SurvivorInfoStatePromoteDone());
			stateMachine.AddState(new SurvivorInfoStateHeroUnlock());
			stateMachine.AddState(new SurvivorInfoStateOutfits());
			stateMachine.AddState(new SurvivorInfoStateHeroSkins());
			stateMachine.AddState(new SurvivorInfoStateShare());
			stateMachine.AddState(new SurvivorInfoStateMissionAccept());
			stateMachine.AddState(new SurvivorInfoStateReject());
			stateMachine.AddState(new SurvivorInfoBadgesOverview());
			stateMachine.AddState(new SurvivorInfoStateHeroPreview());
			stateMachine.AddState(new SurvivorInfoTraitRerolledState());
			stateMachine.AddState(new SurvivorInfoSurvivalManualOverview());
			stateMachine.SetDefaultState(2);
			PassReferencesToStates();
		}
	}

	private void PassReferencesToStates()
	{
		if (!(stateMachine != null))
		{
			return;
		}
		for (int i = 0; i < stateMachine.StatesList.Count; i++)
		{
			if (stateMachine.StatesList[i] != null && stateMachine.StatesList[i] is SurvivorInfoStateBase)
			{
				SurvivorInfoStateBase obj = stateMachine.StatesList[i] as SurvivorInfoStateBase;
				obj.SurvivorNamePanel = survivorNamePanel;
				obj.SurvivorTraitsList = survivorTraitsList;
				obj.SurvivorStatistics = surviviorStatistics;
				obj.SurvivalManualPanel = survivalManualPanel;
				obj.SurvivorBadgesPanel = survivorBadgesPanel;
				obj.SurvivorRightSidePanel = survivorRightSidePanel;
				obj.LevelUpPanel = levelUpPanel;
				obj.PromotedPanel = promotedPanel;
				obj.PromotedPanelNoTrait = promotedPanelNoTrait;
				obj.TraitUpgradePanel = traitUpgradePanel;
				obj.SurvivorOutfitsView = survivorOutfitsView;
				obj.HeroSkinsView = heroSkinsView;
				obj.UpgradePanel = upgradePanel;
				obj.SharePanel = sharePanel;
				obj.RarityAndClass = rarityAndClass;
				obj.UnlockView = unlockView;
				obj.TrainingLockedParent = trainingLockedParent;
				obj.TrainingLockedUILabel = trainingLockedUILabel;
				obj.OpenTrainButton = openTrainButton;
				obj.SpeedUpButton = speedUpButton;
				obj.SpeedUpPayButton = speedUpPayButton;
				obj.TrainButton = trainButton;
				obj.TrainInstanButton = trainInstanButton;
				obj.TrainInstantWithTokensButton = trainInstantWithTokensButton;
				obj.CancelButton = cancelButton;
				obj.UpgradeButton = upgradeButton;
				obj.MaxUpgradesLabel = maxUpgradesLabel;
				obj.PromoteButton = promoteButton;
				obj.RetireButton = retireButton;
				obj.OutfitButton = outfitButton;
				obj.ShareButton = shareButton;
				obj.FavoriteButton = favoriteToggleButton;
				obj.CloseButton = closeButton;
				obj.GenericOkButton = genericOkButton;
				obj.NextSurvivorButton = nextSurvivorButton;
				obj.PreviousSurvivorButton = previousSurvivorButton;
				obj.BounsButton = bounsButton;
				obj.PreviewMaxStatsButton = previewHeroStatsButton;
				obj.PreviewMaxStatsReturnButton = previewHeroStatsReturnButton;
				obj.UnlockUiButton = unlockHeroUiButton;
				obj.MoreTokensUiButton = moreTokensUiButton;
				obj.SurvivorDescriptionLabel = survivorDescriptionLabel;
				obj.AcceptFromMissionParent = acceptFromMissionParent;
				obj.AcceptButton = acceptButton;
				obj.RejectButton = rejectButton;
				obj.ManageButton = manageButton;
				obj.MedicTent = medicTentModel;
				obj.SurvivorModel = survivorModel;
				obj.TraitRerollPanel = traitRerollPanel;
				obj.FeatureInfoButton = featureHeroInfoButton;
				obj.AllReferencesGiven();
			}
		}
	}

	private void SetState(SurvivorInfoStateBase.States state, bool forceUpdate = false)
	{
		if (stateMachine != null)
		{
			stateMachine.TrySwitchToState((int)state, forceUpdate);
		}
	}

	private SurvivorInfoStateBase.States GetCurrentState()
	{
		if (stateMachine != null)
		{
			return (SurvivorInfoStateBase.States)stateMachine.currentStateId;
		}
		return SurvivorInfoStateBase.States.None;
	}

	private bool SwitchToPreviousState(bool allowDefaultState = true)
	{
		if (stateMachine != null)
		{
			return stateMachine.SwitchToPreviousState(allowDefaultState);
		}
		return false;
	}

	private void OnUIEvent(string type, object parameter)
	{
		if (OfflineManager.IsLoadDataManager && type == "OnNewSurvivorSelected")
		{
			DebugTWD.LogMycode("if (OfflineManager.IsLoadDataManager && type == \"OnNewSurvivorSelected\")");
			DebugTWD.Log("OnNewSurvivorSelected", DebugType.OnClick);
			UpdateUI();
		}
		else if (AllowWeapons && type == "SurvivorCardEquipmentClicked")
		{
			EquipmentButton equipmentButton = parameter as EquipmentButton;
			if (equipmentButton.GetEquipment() != null)
			{
				if (OfflineManager.IsLoadDataManager)
				{
					DebugTWD.LogMycode("if (OfflineManager.IsLoadDataManager)");
					DebugTWD.Log("SurvivorCardEquipmentClicked infoPopup");
					StartCoroutine(WaitForUpdateUI());
				}
				base.gameObject.GetComponent<EquipmentSelectionContainerView>().OpenForSurvivorCard(null, equipmentButton);
			}
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/equipment_click");
		}
		else if (AllowWeapons && type == "OnNewEquipmentSelected")
		{
			if (OfflineManager.IsLoadDataManager)
			{
				DebugTWD.LogMycode("if (OfflineManager.IsLoadDataManager)");
				DebugTWD.Log("OnNewEquipmentSelected");
				UpdateUI();
			}
			else
			{
				EquipmentButton equipmentButton2 = parameter as EquipmentButton;
				if (equipmentButton2 != null && equipmentButton2.GetEquipment().IsWeaponEquipment)
				{
					SingularityMonoBehaviour<FullscreenActorOverlay>.Instance.RequestSwitchEquipment(equipmentButton2.GetEquipment());
					UpdateUI();
				}
				if (equipmentButton2 != null && HelpersGfx.IsApocalypticRarity(equipmentButton2.GetEquipment().RarityLevel))
				{
					SingularityMonoBehaviour<FullscreenActorOverlay>.Instance.RequestShowUpgradeAnim();
				}
			}
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/survivor_equip");
		}
		else if (type == "OnSharePreviewClosing")
		{
			if (GetCurrentState() == SurvivorInfoStateBase.States.SurvivorShare)
			{
				SwitchToPreviousState();
			}
		}
		else if (type == "OnClickBadgeIconEquip" && parameter != null && parameter is BadgeInfo)
		{
			TryEquipBadge(parameter as BadgeInfo);
		}
		else if (type == "OnClickBadgeIconRemove" && parameter != null && parameter is int)
		{
			TryRemoveBadgeAtIndex((int)parameter, OnBadgeRemoved);
		}
		else if (type == "SurvivorTraitRerolled" && parameter != null && parameter is string)
		{
			OnTraitRerollButtonClicked((string)parameter);
		}
		else if (type == "OnHeroSkinViewClosed")
		{
			if (stateMachine.GetHistory().Count() > 0)
			{
				SwitchToPreviousState();
			}
			else
			{
				SetState(SurvivorInfoStateBase.States.SurvivorOverview);
				OnClickClose();
			}
			Helpers.GameObjectSetActive(closeButtonPanel, value: true);
		}
		else if (type == "BounsUpgrade")
		{
			SetState(SurvivorInfoStateBase.States.SurvivorOverview, forceUpdate: true);
		}
	}

	private void TryEquipBadge(BadgeInfo badgeInfo)
	{
		if (badgeInfo != null && badgeInfo.Model != null && !badgeInfo.MaxSimilarBadgesReached)
		{
			BadgeModel badge = survivorModel.BadgeContainer.GetBadge(badgeInfo.Model.SlotIndex);
			currentEquipBadgeModel = badgeInfo.Model;
			TooltipManager.HideAll();
			if (badge == null)
			{
				if (OfflineManager.IsLoadDataManager)
				{
					DebugTWD.LogMycode("if (OfflineManager.IsLoadDataManager)");
					OnConfirmEquipSelected();
				}
				else
				{
					ConfirmationPopup obj = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.ConfirmationPopup) as ConfirmationPopup;
					obj.SetContent(LocalizationManager.GetText("Popup.Confirmation.EquipBadge.Title"), LocalizationManager.GetText("Popup.Confirmation.EquipBadge.Description"));
					obj.SetOkButtonLabel(LocalizationManager.GetText("Button.Equip"));
					obj.SetCancelButtonLabel(LocalizationManager.GetText("Button.Cancel"));
					obj.SetCallbacks(OnConfirmEquipSelected);
					obj.Open();
				}
			}
			else
			{
				if (OfflineManager.IsLoadDataManager)
				{
					DebugTWD.LogMycode("if (OfflineManager.IsLoadDataManager)");
					OnReplaceBadgeAndKeepPrevious();
				}
				else
				{
					BadgeReplacePopup obj2 = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.BadgeReplacePopup) as BadgeReplacePopup;
					obj2.SetCallbacks(OnReplaceBadgeAndScrapPrevious, OnReplaceBadgeAndKeepPrevious);
					obj2.Open();
				}
			}
		}
	}

	private void OnReplaceBadgeAndScrapPrevious()
	{
		Cashier scrapCashier = survivorModel.BadgeContainer.GetBadge(currentEquipBadgeModel.SlotIndex).GetScrapCashier();
		ConfirmationPopup obj = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.ConfirmationPopup) as ConfirmationPopup;
		obj.SetContent(LocalizationManager.GetText("Popup.ScrapConfirmationList.Title"), LocalizationManager.GetText("Popup.ScrapConfirmationList.Message"));
		obj.SetCurrencies(scrapCashier);
		obj.SetCallbacks(OnScrapConfirmed, OnScrapCanceled);
		obj.SetOkButtonLabel(LocalizationManager.GetText("Button.Ok"));
		obj.SetCancelButtonLabel(LocalizationManager.GetText("Button.Cancel"));
		obj.Open();
	}

	private void OnScrapConfirmed()
	{
		SingularityMonoBehaviour<HUDManager>.Instance.CloseIfExists(UIType.BadgeReplacePopup);
		EquipBadge(savePreviousBadge: false);
	}

	private void OnScrapCanceled()
	{
	}

	private void OnReplaceBadgeAndKeepPrevious()
	{
		if (currentEquipBadgeModel != null)
		{
			TryRemoveWithoutConfirmation(currentEquipBadgeModel.SlotIndex, OnEquipAfterReclaim);
		}
	}

	private void TryRemoveBadgeAtIndex(int index, ConsumeCurrencyCommandUtils.ConfirmationCallback callback)
	{
		if (OfflineManager.IsFakeExecuteCommands)
		{
			DebugTWD.LogMycode("if (OfflineManager.IsFakeExecuteCommands)");
			BadgeModel badgeWithSlotIndex = survivorModel.GetBadgeWithSlotIndex(index);
			if (badgeWithSlotIndex != null && model != null)
			{
				if (survivorModel.Modifiers == null)
				{
					survivorModel.SetModifiers(new ModifierCollection());
					survivorModel.Modifiers.SetManager(GameManager.Instance.modelManager);
					survivorModel.Modifiers.Initialize();
				}
				survivorModel.ReclaimBadge(badgeWithSlotIndex, pay: false, returnBadgeInventory: true);
			}
			UIEvent.Send("OnBadgeUnequipped");
			TooltipManager.HideAll();
			UpdateUI();
		}
		else
		{
			ConsumeCurrencyCommandUtils.Execute(new ReclaimBadgeCommand(survivorModel, index)
			{
				Cashier = survivorModel.GetBadgeReclaimCashier()
			}, callback);
		}
	}

	private void TryRemoveWithoutConfirmation(int index, ConsumeCurrencyCommandUtils.ConfirmationCallback callback)
	{
		if (OfflineManager.IsFakeExecuteCommands)
		{
			DebugTWD.LogMycode("if (OfflineManager.IsFakeExecuteCommands)");
			BadgeModel badgeWithSlotIndex = survivorModel.GetBadgeWithSlotIndex(index);
			TWDModelResult result = TWDModelResult.Error;
			if (badgeWithSlotIndex != null && model != null)
			{
				if (survivorModel.Modifiers == null)
				{
					survivorModel.SetModifiers(new ModifierCollection());
					survivorModel.Modifiers.SetManager(DataManager.Instance.ModelManager);
					survivorModel.Modifiers.Initialize();
				}
				result = survivorModel.ReclaimBadge(badgeWithSlotIndex, pay: false, returnBadgeInventory: true);
			}
			OnEquipAfterReclaim(result);
		}
		else
		{
			ReclaimBadgeCommand reclaimBadgeCommand = new ReclaimBadgeCommand(survivorModel, index);
			reclaimBadgeCommand.Cashier = survivorModel.GetBadgeReclaimCashier();
			callback(Helpers.ExecuteCommand(reclaimBadgeCommand));
		}
	}

	private void OnEquipAfterReclaim(TWDModelResult result)
	{
		if (result == TWDModelResult.OK)
		{
			EquipBadge(savePreviousBadge: false);
		}
	}

	private void OnBadgeRemoved(TWDModelResult result)
	{
		UIEvent.Send("OnBadgeUnequipped");
		TooltipManager.HideAll();
		UpdateUI();
	}

	private void EquipBadge(bool savePreviousBadge)
	{
		if (OfflineManager.IsFakeExecuteCommands)
		{
			DebugTWD.LogMycode("if (OfflineManager.IsFakeExecuteCommands)");
			TWDModelResult tWDModelResult = survivorModel.EquipBadge(currentEquipBadgeModel, savePreviousBadge);
			OnEquipCommandExecuted(tWDModelResult);
		}
		else
		{
			EquipBadgeCommand equipBadgeCommand = new EquipBadgeCommand(survivorModel, currentEquipBadgeModel);
			if (savePreviousBadge)
			{
				equipBadgeCommand.SaveExisting = true;
				equipBadgeCommand.Cashier = survivorModel.GetBadgeReclaimCashier();
			}
			ConsumeCurrencyCommandUtils.Execute(equipBadgeCommand, OnEquipCommandExecuted);
		}
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/survivor_equip");
	}

	private void OnEquipCommandExecuted(TWDModelResult result)
	{
		SingularityMonoBehaviour<HUDManager>.Instance.CloseIfExists(UIType.BadgeReplacePopup);
		UpdateUI();
		if (result == TWDModelResult.OK)
		{
			UIEvent.Send("OnBadgeEquipped", currentEquipBadgeModel);
			currentEquipBadgeModel = null;
		}
	}

	private void OnConfirmEquipSelected()
	{
		if (currentEquipBadgeModel != null && survivorModel != null)
		{
			EquipBadge(savePreviousBadge: false);
		}
	}

	private void OnOpenTrainClicked(UIButtonExtended button)
	{
		SetState(SurvivorInfoStateBase.States.SurvivorTrainPreview);
		TutorialView.Instance.UpdateSuggestion();
	}

	private void OnSpeedUpClicked(UIButtonExtended button)
	{
		if (survivorModel.IsUpgrading())
		{
			ConsumeCurrencyCommandUtils.Execute(new SpeedUpUpgradeSurvivorCommand(survivorModel)
			{
				Cashier = survivorModel.TimedActionModel.GetSpeedUpCashier()
			});
		}
		else
		{
			ConsumeCurrencyCommandUtils.Execute(new SpeedUpCuringSurvivorCommand(survivorModel)
			{
				Cashier = medicTentModel.GetFinishOneCashier(survivorModel)
			}, OnSurvivorHealCallBack);
		}
		UpdateUI();
	}

	private void OnUpgradeClicked(UIButtonExtended button)
	{
		if (survivorModel != null)
		{
			if (!survivorModel.GetUpgradeTraitCashier().CanAfford())
			{
				MissingTokensPopup missingTokensPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.MissingTokensPopup) as MissingTokensPopup;
				if (missingTokensPopup != null && TutorialView.Instance != null && !TutorialView.Instance.Running)
				{
					missingTokensPopup.IsHeroContent = survivorModel.IsHero;
					missingTokensPopup.Open();
				}
				return;
			}
			UpgradeSurvivorTraitCommand command = new UpgradeSurvivorTraitCommand(survivorModel);
			command.Cashier = survivorModel.GetUpgradeTraitCashier();
			ConfirmationPopup confirmationPopup = (ConfirmationPopup)SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.ConfirmationPopup);
			confirmationPopup.Open();
			if (survivorModel.CanUpgradeSurvivorRarity())
			{
				upgradeAudioEventName = "global/survivor_upgrade_rarity";
				confirmationPopup.SetContent(LocalizationManager.GetText("Popup.SurvivorInfo.Button.PromoteSurvivor"), "");
				confirmationPopup.SetCurrencies(command.Cashier);
				confirmationPopup.SetCallbacks(delegate
				{
					ConsumeCurrencyCommandUtils.Execute(command, UpgradeSurvivorTraitCallback);
				}, delegate
				{
				});
			}
			else
			{
				upgradeAudioEventName = "global/survivor_upgrade_trait";
				confirmationPopup.SetContent(LocalizationManager.GetText("Popup.SurvivorInfo.Button.Upgrade"), "");
				confirmationPopup.SetCurrencies(command.Cashier);
				confirmationPopup.SetCallbacks(delegate
				{
					ConsumeCurrencyCommandUtils.Execute(command, UpgradeSurvivorTraitCallback);
				}, delegate
				{
				});
			}
			if (survivorModel.CanUpgradeSurvivorRarity())
			{
				StateAfterTraitUpgrade = SurvivorInfoStateBase.States.SurvivorPromoteDone;
			}
			else
			{
				StateAfterTraitUpgrade = SurvivorInfoStateBase.States.SurvivorUpgradeDone;
			}
		}
		else
		{
			Debug.LogError("Data null or Cannot upgrade survivor!");
		}
	}

	private void OnTrainClicked(UIButtonExtended button)
	{
		if (survivorModel != null && survivorModel.CanUpgrade)
		{
			ConsumeCurrencyCommandUtils.Execute(new UpgradeSurvivorCommand(survivorModel)
			{
				Instant = false,
				Cashier = survivorModel.GetUpgradeCashier(instantUpgrade: false)
			});
			EventManager.NotifyClick("Buy");
		}
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/survivor_accept");
		UIEvent.Send("OnSurvivorUpgradeStarted", survivorModel);
		Close();
	}

	private void OnTrainInstantClicked(UIButtonExtended button)
	{
		if (survivorModel != null && survivorModel.CanUpgrade)
		{
			ConsumeCurrencyCommandUtils.Execute(new UpgradeSurvivorCommand(survivorModel)
			{
				Instant = true,
				Cashier = survivorModel.GetUpgradeCashier(instantUpgrade: true, addInitialSurvivorPoints: true)
			}, InstantUpgradeCallback);
		}
	}

	private void OnTrainInstantWithTokensClicked(UIButtonExtended button)
	{
		if (survivorModel != null && survivorModel.CanUpgrade)
		{
			UpgradeSurvivorCommand upgradeSurvivorCommand = new UpgradeSurvivorCommand(survivorModel);
			upgradeSurvivorCommand.Instant = true;
			upgradeSurvivorCommand.Cashier = survivorModel.GetUpgradeCashier(instantUpgrade: true, addInitialSurvivorPoints: false, useTokens: true);
			if (upgradeSurvivorCommand.Cashier.CanAfford())
			{
				ConsumeCurrencyCommandUtils.Execute(upgradeSurvivorCommand, InstantUpgradeCallback);
			}
			else
			{
				TooltipManager.OpenTextBoxWithText(button.gameObject, LocalizationManager.GetText("Tooltip.BattlePass.SpeedupToken.Missing"));
			}
		}
	}

	//Reroll Trait
	public void OnTraitRerollButtonClicked(string traitIdentifier)
	{
		if (OfflineManager.IsLoadDataManager)
		{
			TWDModelResult tWDModelResult = TWDModelResult.Error;
			TraitDefinition traitDefinition = DataManager.Instance.GameData.GetTraitDefinition(traitIdentifier);
			if (survivorModel != null && survivorModel.HasUpgradeTrait(traitIdentifier) && traitDefinition != null && !traitDefinition.HasTag("FactionBuffTrait") && !traitDefinition.Identifier.Equals("Overwatch", StringComparison.Ordinal))
			{
				bool isFree = DataManager.Instance.SurvivorManagementPopUp.IsTraitRerollFree || OfflineManager.IsFreeAll;

				if (isFree)
				{
					tWDModelResult = survivorModel.RerollTrait(traitIdentifier) ? TWDModelResult.OK : TWDModelResult.Error;
				}
				else
				{
					Cashier traitRerollCashier = survivorModel.GetTraitRerollCashier(traitIdentifier);
					if (traitRerollCashier != null && traitRerollCashier.CanAfford())
					{
						tWDModelResult = traitRerollCashier.Pay(survivorModel);
						if (tWDModelResult == TWDModelResult.OK)
						{
							tWDModelResult = survivorModel.RerollTrait(traitIdentifier) ? TWDModelResult.OK : TWDModelResult.Error;
						}
					}
				}
			}
			if (tWDModelResult == TWDModelResult.OK)
			{
				SetState(SurvivorInfoStateBase.States.SurvivorTraitRerolled);
				UpdateUI();
				DebugTWD.Log("Update Traits Everywere");
			}
		}
		else
		{
			if (survivorModel.CanRerollTrait)
			{
				GameManager.Instance.CheckConnectionReachability(showPopup: true, "RerollSurvivorTraitCommand");
				if (Helpers.ExecuteCommand(new RerollSurvivorTraitCommand(survivorModel, traitIdentifier)) == TWDModelResult.OK)
				{
					SetState(SurvivorInfoStateBase.States.SurvivorTraitRerolled);
					UpdateUI();
				}
			}
		}
	}

	private void InstantUpgradeCallback(TWDModelResult result)
	{
		if (result == TWDModelResult.OK)
		{
			Helpers.ExecuteCommand(new UpgradedModelViewedCommand(survivorModel.manager.Player.Camp.GetBuilding("TrainingGround") as TrainingGroundBuildingModel));
			SetState(SurvivorInfoStateBase.States.SurvivorTrainDone);
			UIEvent.Send("OnSurvivorInstantUpgraded", survivorModel);
		}
	}

	private void OnCancelClicked(UIButtonExtended button)
	{
		SwitchToPreviousState();
	}

	private void OnOutfitClicked(UIButtonExtended button)
	{
		SetOutfitRightSideActive(active: true);
		Helpers.GameObjectSetActive(closeButtonPanel, value: false);
		if (survivorModel.IsHero)
		{
			SetState(SurvivorInfoStateBase.States.SurvivorHeroSkins);
		}
		else
		{
			SetState(SurvivorInfoStateBase.States.SurvivorOutfits);
		}
	}

	private void OnShareClicked(UIButtonExtended button)
	{
		OpenSharePrompt();
	}

	private void OpenSharePrompt()
	{
		ShareState(SurvivorInfoStateBase.States.SurvivorShare, "Survivor", shareButton, ShowSurvivorUIForScreenshot, null);
	}

	private void OnBadgeInfoClicked(UIButtonExtended button)
	{
		SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.BadgeInfoPopup).Open();
	}

	private void OnShareUnlockClicked(UIButtonExtended button)
	{
		GiveShareReward();
		ShareState(SurvivorInfoStateBase.States.None, "HeroUnlock", null, ShowHeroUnlockUIForScreenshot, ShareUnlockCompleteCallback);
	}

	private void OnSkipUnlockClicked(UIButtonExtended button)
	{
		SetState(SurvivorInfoStateBase.States.SurvivorOverview);
	}

	private void ShareState(SurvivorInfoStateBase.States state, string shareType, UIButtonExtended buttonShare, Action<bool> showUiCallback, Callback completedCallback)
	{
		if (GetComponent<ScreenshotShare>() != null && base.gameObject.activeSelf)
		{
			if (state != SurvivorInfoStateBase.States.None)
			{
				SetState(state);
			}
			UIEvent.Send("OnClickedShare");
			StartCoroutine(GetComponent<ScreenshotShare>().TakeScreenshot(shareType, buttonShare, shareBadge, showUiCallback, completedCallback));
		}
		else if (base.gameObject.activeSelf)
		{
			DebugLogError("Could not find ScreenshotShare!");
		}
		else
		{
			DebugLogError("gameObject.activeSelf NOT active could not StartCoroutine!");
		}
	}

	private void ShareUnlockCompleteCallback()
	{
		if (survivorModel.IsHero)
		{
			if (unlockView != null)
			{
				unlockView.ShowButtons(value: false);
			}
			Helpers.GameObjectSetActive(genericOkButton, value: true);
			Helpers.GameObjectSetActive(shareButton, value: false);
			if (shareRewardGiven)
			{
				(SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.IAPConfirmPopupNew) as IAPConfirmPopupNew).OpenForCurrency(GameManager.Instance.gameEconomyData.GetUnlockShareRewardForSurvivor(survivorModel.Definition));
				shareRewardGiven = false;
			}
		}
	}

	private void GiveShareReward()
	{
		if (GameManager.Instance.gameEconomyData.IsUnlockShareRewardEnabled() && survivorModel.UnlockShareRewardedAmount <= 0)
		{
			RewardCurrency unlockShareRewardForSurvivor = GameManager.Instance.gameEconomyData.GetUnlockShareRewardForSurvivor(survivorModel.Definition);
			if (unlockShareRewardForSurvivor != null && unlockShareRewardForSurvivor.Amount > 0 && Helpers.ExecuteCommand(new GiveShareRewardCommand(survivorModel)) == TWDModelResult.OK)
			{
				shareRewardGiven = true;
			}
		}
	}

	private void OnRejectFromMissionClicked(UIButtonExtended button)
	{
		if (rejectButton != null && acceptButton != null)
		{
			rejectButton.isEnabled = false;
			acceptButton.isEnabled = false;
			ConfirmationPopup confirmationPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.ConfirmationPopup) as ConfirmationPopup;
			confirmationPopup.SetContent(LocalizationManager.GetText("Popup.RejectConfirmation.Title{Name}", survivorModel.Name), LocalizationManager.GetText("Popup.RejectConfirmation.Message{Name}", survivorModel.Name));
			confirmationPopup.SetOkButtonLabel(LocalizationManager.GetText("Button.Ok"));
			confirmationPopup.SetCurrencies(survivorModel.GetDemoteCashier());
			confirmationPopup.SetCallbacks(OnRejectFromMissionConfirmed, OnRejectFromMissionCancel);
			confirmationPopup.Open();
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_click");
		}
	}

	private void OnRejectFromMissionCancel()
	{
		if (rejectButton != null)
		{
			rejectButton.isEnabled = true;
			UpdateUI();
		}
	}

	private void OnRejectFromMissionConfirmed()
	{
		if (rejectButton != null && acceptButton != null && !acceptOrRejectBlock)
		{
			acceptOrRejectBlock = true;
			rejectButton.isEnabled = false;
			rejectButton.Clear();
			acceptButton.isEnabled = false;
			acceptButton.Clear();
			AcceptClassTokensFromMission(survivorModel, NewSurvivorSource.Mission);
		}
	}

	private void OnAcceptFromMissionClicked(UIButtonExtended button)
	{
		if (rejectButton != null && acceptButton != null && !acceptOrRejectBlock)
		{
			acceptOrRejectBlock = true;
			rejectButton.isEnabled = false;
			rejectButton.Clear();
			acceptButton.isEnabled = false;
			acceptButton.Clear();
			AcceptSurvivorFromMission(survivorModel, NewSurvivorSource.Mission);
		}
	}

	private void OnManageClicked(UIButtonExtended button)
	{
		Close();
		survivorModelFromMission = survivorModel;
		SurvivorManagementPopUp obj = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CampTrainingGrounds) as SurvivorManagementPopUp;
		obj.OnClose += OnCloseTrainingGrounds;
		obj.IsAcceptingSurvivor = true;
		obj.Open();
	}

	private void OnGenericOkClicked(UIButtonExtended button)
	{
		SetState(SurvivorInfoStateBase.States.SurvivorOverview);
	}

	private void OnClickedCraftBadges(UIButtonExtended button)
	{
		OpenResidenceAtIndex(0);
	}

	private void OnClickedInventory(UIButtonExtended button)
	{
		OpenResidenceAtIndex(1);
	}

	private void OpenResidenceAtIndex(int index)
	{
		if (GameManager.Instance.playerModel.Camp.GetBuilding("Residence") != null && index > -1)
		{
			ResidencePopup residencePopup = OfflineManager.IsLoadDataManager ? ResidencePopup.Instance : SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CampResidencePopup) as ResidencePopup;
			if (residencePopup != null && stateMachine != null)
			{
				residencePopup.SetUITypeOpenOnClose(UIType.CampSurvivorInfoPopup, new SurvivorInfoPopupStateData
				{
					model = survivorModel,
					state = (SurvivorInfoStateBase.States)stateMachine.currentStateId,
					stateMachineHistory = stateMachine.GetHistory(),
					currentFilter = ((currentSurvivorFilterList != null) ? currentSurvivorFilterList.Copy() : null)
				});
				residencePopup.OpenAtTabIndex(index);
				SingularityMonoBehaviour<HUDManager>.Instance.CloseAllElementsOfType(base.UIType);
			}
		}
	}

	private void OnCloseTrainingGrounds(HUDElement element, HUDElementConfig hudElementConfig)
	{
		HUDElement hUDElement = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CampTrainingGrounds, null, createIfNotExist: false);
		if (hUDElement != null)
		{
			hUDElement.OnClose -= OnCloseTrainingGrounds;
		}
		SurvivorInfoPopup survivorInfoPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CampSurvivorInfoPopup) as SurvivorInfoPopup;
		if (survivorInfoPopup != null)
		{
			survivorInfoPopup.currentStateMachineState = SurvivorInfoStateBase.States.SurvivoreMissionAccept;
			survivorInfoPopup.OpenForModel(survivorModelFromMission);
			survivorModelFromMission = null;
		}
	}

	private void OnToggleSetChange(UIButtonExtended button)
	{
		if (survivorRightSidePanel != null)
		{
			if (survivorRightSidePanel.GetSelectedIndex() == 0)
			{
				currentStateMachineState = SurvivorInfoStateBase.States.SurvivorOverview;
				SetState(SurvivorInfoStateBase.States.SurvivorOverview);
				if (OfflineManager.IsLoadDataManager)
				{
					DebugTWD.LogMycode("if (OfflineManager.IsLoadDataManager)");
					Helpers.GameObjectSetActive(survivorBadgesPanel, false);
				}
			}
			else if (survivorRightSidePanel.GetSelectedIndex() == 1)
			{
				currentStateMachineState = SurvivorInfoStateBase.States.SurvivosBadgesOverview;
				SetState(SurvivorInfoStateBase.States.SurvivosBadgesOverview);
			}
			else if (survivorRightSidePanel.GetSelectedIndex() == 2)
			{
				currentStateMachineState = SurvivorInfoStateBase.States.SurvivalManual;
				SetState(SurvivorInfoStateBase.States.SurvivalManual);
			}
		}
	}

	private void AcceptSurvivorFromMission(SurvivorModel survivorModel, NewSurvivorSource source)
	{
		if (survivorModel != null)
		{
			Helpers.ExecuteCommand(new AcceptSurvivorCommand(survivorModel, source));
			EventManager.NotifyClick("AcceptSurvivor");
			EventManager.NotifyEvent(EventManager.EventType.AcceptSurvivor);
			Close();
		}
		else
		{
			Debug.LogError("Cannot accept NULL survivorModel");
		}
	}

	private void AcceptClassTokensFromMission(SurvivorModel survivorModel, NewSurvivorSource source)
	{
		if (survivorModel != null)
		{
			Helpers.ExecuteCommand(new RejectSurvivorCommand(survivorModel, source));
			EventManager.NotifyEvent(EventManager.EventType.RejectSurvivor);
			Close();
		}
		else
		{
			Debug.LogError("Cannot accept NULL survivorModel");
		}
	}

	private void ShowSurvivorUIForScreenshot(bool show)
	{
		Helpers.GameObjectSetActive(sharePanel, show);
	}

	private void ShowHeroUnlockUIForScreenshot(bool show)
	{
		Helpers.GameObjectSetActive(sharePanel, show);
		Helpers.GameObjectSetActive(shareButton, value: false);
		if (unlockView != null)
		{
			unlockView.ShowButtons(!show);
		}
	}

	private void OnRetireClicked(UIButtonExtended button)
	{
		if (!TutorialView.Instance.Model.StaticTutorialComplete && survivorModel.SurvivorClass == SurvivorClass.Bruiser)
		{
			HUDNotification.Error(LocalizationManager.GetText("Popup.UpgradeSurvivor.TutorialCantRetireBruiser"));
			return;
		}
		if (GameManager.Instance.playerModel.SurvivorContainer.Survivors.Count <= 3)
		{
			AlertPopup.ShowPopupGetText("Generic.Info", "Popup.DemoteConfirmation.NotEnoughSurvivor", "Button.Ok", OnPopupsCancel);
			return;
		}
		if (GameManager.Instance.playerModel.SurvivorContainer.IsOutpostDefending(survivorModel))
		{
			AlertPopup.ShowPopupGetText("Generic.Info", "Popup.DemoteConfirmation.CannotDemoteOutpostDefender", "Button.Ok", OnPopupsCancel);
			return;
		}
		List<SurvivorMockData> gvGDefenders = GameManager.Instance.playerModel.GvGDefenders;
		if (GameManager.Instance.playerModel.IsGuildMember && gvGDefenders != null && gvGDefenders.Any((SurvivorMockData x) => x.AnalyticsId == survivorModel.IdForAnalytics))
		{
			AlertPopup.ShowPopupGetText("Generic.Info", "Popup.DemoteConfirmation.CannotDemoteGvgDefender", "Button.Ok", OnPopupsCancel);
			return;
		}
		ConfirmationPopup confirmationPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.ConfirmationPopup) as ConfirmationPopup;
		confirmationPopup.SetContent(LocalizationManager.GetText("Popup.DemoteConfirmation.Title{Name}", survivorModel.Name), LocalizationManager.GetText("Popup.DemoteConfirmation.Message{Name}", survivorModel.Name));
		confirmationPopup.SetOkButtonLabel(LocalizationManager.GetText("Button.Ok"));
		confirmationPopup.SetCurrencies(survivorModel.GetDemoteCashier());
		confirmationPopup.SetCallbacks(OnDemoteConfirmed, OnPopupsCancel);
		confirmationPopup.Open();
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_click");
	}

	private void OnClickFeatureInfoButton(UIButtonExtended button)
	{
		FeaturedHeroDefinition activeFeaturedHero = GameManager.Instance.gameEconomyData.GetActiveFeaturedHero(GameManager.Instance.playerModel.UtcTimeStamp);
		if (activeFeaturedHero != null)
		{
			if (OfflineManager.IsLoadDataManager && activeFeaturedHero.ActorDefinitionID != survivorModel.ActorDefinitionID)
			{
				activeFeaturedHero = GameManager.Instance.gameEconomyData.GetActiveFeaturedHero(GameManager.Instance.playerModel.UtcTimeStamp + MyTools.TimeSpanToLong(TimeSpan.FromDays(7)));
			}
			FeaturedHeroPopup featuredHeroPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.FeaturedHeroPopup) as FeaturedHeroPopup;
			if (featuredHeroPopup != null)
			{
				SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_click");
				featuredHeroPopup.OpenWithStateData(activeFeaturedHero);
			}
		}
	}

	private void OnDemoteConfirmed()
	{
		Cashier demoteCashier = survivorModel.GetDemoteCashier();
		if (CampView.Instance != null && CampView.Instance.BuildingsHud != null)
		{
			CampView.Instance.BuildingsHud.CreateCollectAnim(demoteCashier);
		}
		if (Helpers.ExecuteCommand(new DemoteSurvivorCommand(survivorModel)) == TWDModelResult.OK)
		{
			UIEvent.Send("SurvivorDeleted", survivorModel);
			PortraitManager.Instance.RemovePortrait(PortraitRenderSource.fromActorModel(survivorModel));
			HandleSurvivorUpgradeViewed(survivorModel);
		}
		Close();
	}

	private void OnPopupsCancel()
	{
		UpdateUI();
	}

	public void OnClickExitOutfits()
	{
		if (stateMachine.GetHistory().Count() > 0)
		{
			SwitchToPreviousState();
		}
		else
		{
			SetState(SurvivorInfoStateBase.States.SurvivorOverview);
			OnClickClose();
		}
		UIEvent.Send("OnOutfitViewClosed", survivorModel);
		Helpers.GameObjectSetActive(closeButtonPanel, value: true);
	}

	private void reactivateTrainingGroundPopup()
	{
		if (OfflineManager.IsLoadDataManager)
		{
			DebugTWD.LogMycode("if (OfflineManager.IsLoadDataManager) return");
			return;
		}
		if (SingularityMonoBehaviour<HUDManager>.Instance != null)
		{
			SurvivorManagementPopUp survivorManagementPopUp = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CampTrainingGrounds, null, createIfNotExist: false) as SurvivorManagementPopUp;
			if (survivorManagementPopUp != null)
			{
				survivorManagementPopUp.SetSurvivorListPanelVisibility(visibility: true);
			}
		}
	}

	private void OnSurvivorHealCallBack(TWDModelResult result)
	{
		if (result == TWDModelResult.OK)
		{
			Helpers.InstantiateToParentAndLayer(survivorHealEffect, base.gameObject);
		}
	}

	public void OpenForOutfitPreview(SurvivorModel survivor, OutfitDefinition outfit)
	{
		SurvivorInfoPopupStateData survivorInfoPopupStateData = new SurvivorInfoPopupStateData();
		survivorInfoPopupStateData.state = SurvivorInfoStateBase.States.SurvivorOutfits;
		survivorInfoPopupStateData.model = survivor;
		OpenWithStateData(survivorInfoPopupStateData);
		survivorOutfitsView.Show(outfit.ID);
		UIEvent.Send("OnNewOutfitSeleted", outfit);
		SetOutfitRightSideActive(active: false);
		Helpers.GameObjectSetActive(closeButtonPanel, value: false);
	}

	public void OpenForHeroSkinPreview(SurvivorModel hero, HeroSkinDefinition skinDefinition)
	{
		if (OfflineManager.IsLoadDataManager)
		{
			DebugTWD.LogMycode("if (OfflineManager.IsLoadDataManager) return");
			DebugTWD.Log("No preview for Hero " + hero.Name);
			return;
		}
		SurvivorInfoPopupStateData survivorInfoPopupStateData = new SurvivorInfoPopupStateData();
		survivorInfoPopupStateData.state = SurvivorInfoStateBase.States.SurvivorHeroSkins;
		survivorInfoPopupStateData.model = hero;
		OpenWithStateData(survivorInfoPopupStateData);
		HeroSkinInfo heroSkinInfoEntry = GameManager.Instance.GetHeroSkinInfoEntry(skinDefinition.ID);
		heroSkinsView.ShowSkinPreview(skinDefinition.ID, hero);
		UIEvent.Send("OnNewOutfitSeleted", heroSkinInfoEntry);
		Helpers.GameObjectSetActive(closeButtonPanel, value: false);
	}

	private void SetOutfitRightSideActive(bool active)
	{
		Helpers.GameObjectSetActive(hideForOutfitPreviewContainer, active);
	}

	public void OnClickSurvivalManualButton()
	{
		HUDElement hUDElement = null;
		if (OfflineManager.IsLoadDataManager)
		{
			var root = HUDManager.Instance.UIContainerTopCameras;
			hUDElement = (HUDElement)(SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.SurvivalManualMainPopup, root) as SurvivalManualMainPopup);
		}
		else
		{
			hUDElement = (!Helpers.IsSurvivalManualPlotGuidenOpened()) ? ((HUDElement)(SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.SurvivalManualPlotGuidePopup) as SurvivalManualPlotGuidePopup)) : ((HUDElement)(SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.SurvivalManualMainPopup) as SurvivalManualMainPopup));
		}
		if (hUDElement != null)
		{
			hUDElement.Open();
			SetState(SurvivorInfoStateBase.States.SurvivorOverview);
			OnClickClose();
			if (CampManager.Instance != null && !OfflineManager.IsLoadDataManager)
			{
				CampManager.Instance.FullscreenPopupShowCamp(SingularityMonoBehaviour<HUDManager>.Instance.CanEnableCamp(UIType.SurvivalManualMainPopup));
			}
		}
	}



	#region myparams
	public UIButtonExtended BadgeUnequipButton;
	public UIButtonExtended BadgeDetailPopupButton;
	public UIButtonExtended RebuildPortraitButton;
	public int SurvivorBadgeIndex { get; set; }
	#endregion

	#region mycode
	public void UnequipBadge()
	{
		DebugTWD.Log("Try Remove Badge: " + SurvivorBadgeIndex);
		TryRemoveBadgeAtIndex(SurvivorBadgeIndex, null);
	}

	public void OpenDetailPopup()
	{
		BadgeModel badgeWithSlotIndex = survivorModel.GetBadgeWithSlotIndex(SurvivorBadgeIndex);

		if (badgeWithSlotIndex == null) return;

		BadgeDetailsPopup badgeDetailsPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.BadgeDetailsPopup) as BadgeDetailsPopup;
		if (badgeDetailsPopup == null) return;

		BadgeCraft.Instance.modelRandomReroll = new ModelRandom(BadgeCraft.Instance.modelRandomLast);
		DebugTWD.Log("Change random to Reroll " + BadgeCraft.Instance.modelRandomLast.State);

		badgeDetailsPopup.SetData(DataManager.Instance.InventoryTab);
		badgeDetailsPopup.OpenForModel(badgeWithSlotIndex);

		//сохраняем исходные параметры значка
		BadgeCraft.Instance.SetOriginBadgeData(badgeWithSlotIndex);
	}

	public void SwitchRerollTypeToggle(UIToggle tg)
	{
		DataManager.Instance.SurvivorManagementPopUp.IsOpenTraitsTree = tg.value;
	}

	public void ShowTraits()
	{
		survivorBadgesPanel.gameObject.SetActive(false);
		survivorRightSidePanel.SetSelectedIndex(0);
		currentStateMachineState = SurvivorInfoStateBase.States.SurvivorOverview;
		SetState(SurvivorInfoStateBase.States.SurvivorOverview, true);
		UpdateUI();
	}

	private IEnumerator WaitForUpdateUI()
	{
		DataManager.Instance.SurvivorManagementPopUp.EquipmentButtonClicked = true;
		yield return new WaitForSecondsRealtime(.5f);
		DataManager.Instance.SurvivorManagementPopUp.EquipmentButtonClicked = false;
	}

	public void RebuildPortrait()
	{
		var card = DataManager.Instance.SurvivorManagementPopUp.survivorCardSelected;
		if (card)
		{
			var lockedCard = card.GetComponent<SurvivorCardHeroLocked>();
			if (lockedCard)
			{
				lockedCard.RebuildPortrait();
			}
			var cardUnlocked = card.GetComponent<SurvivorCard>();
			if (cardUnlocked)
			{
				cardUnlocked.RebuildPortrait();
			}
		}
	}
	#endregion
}
