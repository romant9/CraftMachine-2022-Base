using System.Collections.Generic;
using Client.Tweener;
using TwdCustomMod;
using TWDModel;
using UnityEngine;

public class SurvivorInfoStateBase : UIStateObjectBase
{
	public enum States
	{
		None = 0,
		SurvivorOverview = 1,
		SurvivorOverviewLimited = 2,
		SurvivorTrainPreview = 3,
		SurvivorTrainDone = 4,
		SurvivorUpgradeDone = 5,
		SurvivorPromoteDone = 6,
		SurvivorHeroUnlock = 7,
		SurvivorOutfits = 8,
		SurvivorShare = 9,
		SurvivoreMissionAccept = 10,
		SurvivoreRejectOnly = 11,
		SurvivosBadgesOverview = 12,
		SurvivorHeroPreview = 13,
		SurvivorTraitRerolled = 14,
		SurvivorHeroSkins = 15,
		SurvivalManual = 16
	}

	public enum TweenAnchorId
	{
		None = 0,
		Show = 1,
		Hide = 2,
		Extend = 3
	}

	public static List<GameObject> AllGameObjects = new List<GameObject>();

	public SurvivorNamePanel SurvivorNamePanel { get; set; }

	public SurvivorStatisticsPanel SurvivorStatistics { get; set; }

	public SurvivorTraitsList SurvivorTraitsList { get; set; }

	public SurvivorSurvivalManualPanel SurvivalManualPanel { get; set; }

	public SurvivorBadgesPanel SurvivorBadgesPanel { get; set; }

	public SurvivorInfoRightSidePanel SurvivorRightSidePanel { get; set; }

	public SurvivorUpgradeView LevelUpPanel { get; set; }

	public SurvivorUpgradeView PromotedPanel { get; set; }

	public SurvivorUpgradeView PromotedPanelNoTrait { get; set; }

	public SurvivorUpgradeView TraitUpgradePanel { get; set; }

	public SurvivorTraitRerollView TraitRerollPanel { get; set; }

	public SurvivorOutfitsView SurvivorOutfitsView { get; set; }

	public HeroSkinsView HeroSkinsView { get; set; }

	public SurvivorUpgradePanel UpgradePanel { get; set; }

	public GameObject SharePanel { get; set; }

	public SurvivorRarityAndClassPanel RarityAndClass { get; set; }

	public GameObject TrainingLockedParent { get; set; }

	public UILabel TrainingLockedUILabel { get; set; }

	public SurvivorInfoUnlockView UnlockView { get; set; }

	public UIButtonExtended OpenTrainButton { get; set; }

	public UIButtonExtended SpeedUpButton { get; set; }

	public PayButton SpeedUpPayButton { get; set; }

	public UIButtonExtended TrainButton { get; set; }

	public UIButtonExtended TrainInstanButton { get; set; }

	public UIButtonExtended TrainInstantWithTokensButton { get; set; }

	public UIButtonExtended CancelButton { get; set; }

	public UIButtonWithLabelAndIcon UpgradeButton { get; set; }

	public UILabel MaxUpgradesLabel { get; set; }

	public UIButtonWithLabelAndIcon PromoteButton { get; set; }

	public UIButtonExtended RetireButton { get; set; }

	public UIButtonExtended OutfitButton { get; set; }

	public UIButtonExtended ShareButton { get; set; }

	public UIButton FavoriteButton { get; set; }

	public UIButton CloseButton { get; set; }

	public UIButtonExtended GenericOkButton { get; set; }

	public UIButton NextSurvivorButton { get; set; }

	public UIButton PreviousSurvivorButton { get; set; }

	public UIButton BounsButton { get; set; }

	public UIButtonWithLabel PreviewMaxStatsButton { get; set; }

	public UIButtonWithLabel PreviewMaxStatsReturnButton { get; set; }

	public UIButtonWithLabelAndIcon UnlockUiButton { get; set; }

	public UIButtonWithLabelAndIcon MoreTokensUiButton { get; set; }

	public UILabel SurvivorDescriptionLabel { get; set; }

	public SurvivorAcceptPanel AcceptFromMissionParent { get; set; }

	public UIButtonExtended AcceptButton { get; set; }

	public UIButtonExtended FeatureInfoButton { get; set; }

	public UIButtonExtended RejectButton { get; set; }

	public UIButtonExtended ManageButton { get; set; }

	public MedicTentModel MedicTent { get; set; }

	public SurvivorModel SurvivorModel { get; set; }

	public virtual States CurrentState { get; set; }

	public override int Id
	{
		get
		{
			return (int)CurrentState;
		}
		set
		{
			CurrentState = (States)value;
		}
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
		Helpers.GameObjectSetActive(CloseButton, value: true);
		UpdateAndShowStats();
		UpdateAndShowTraits();
		UpdateAndShowBadges();
		UpdateAndShowName();
		UpdateSurvivalManualPanel();
	}

	public override void Enter()
	{
		if (SurvivorModel == null)
		{
			Debug.LogError("SurvivorInfoStateBase: survivorModel is NULL! Could not Enter() state with id: " + CurrentState);
			Debug.LogError("SurvivorInfoStateBase: Trying to switch back to previous state");
			SwitchToPreviousState();
		}
		else
		{
			HideAllContent();
			if (!OfflineManager.IsLoadDataManager)
			{
                FullscreenActorOverlay.BackgroundType backgroundType = (SurvivorModel.IsAlternativeHero ? FullscreenActorOverlay.BackgroundType.AltHero : (SurvivorModel.IsHero ? FullscreenActorOverlay.BackgroundType.Hero : FullscreenActorOverlay.BackgroundType.Survivor));
                SingularityMonoBehaviour<FullscreenActorOverlay>.Instance.OpenForSelected(SurvivorModel, locked: false, backgroundType);
            }
            base.Enter();
		}
	}

	public virtual void AllReferencesGiven()
	{
		AllGameObjects = new List<GameObject>();
		AddToList(SurvivorNamePanel);
		AddToList(SurvivorStatistics);
		AddToList(SurvivorTraitsList);
		AddToList(SurvivalManualPanel);
		AddToList(SurvivorBadgesPanel);
		AddToList(LevelUpPanel);
		AddToList(PromotedPanel);
		AddToList(PromotedPanelNoTrait);
		AddToList(TraitUpgradePanel);
		AddToList(SurvivorOutfitsView);
		AddToList(HeroSkinsView);
		AddToList(UpgradePanel);
		AddToList(SharePanel);
		AddToList(RarityAndClass);
		AddToList(TrainingLockedParent);
		AddToList(UnlockView);
		AddToList(PromoteButton);
		AddToList(UpgradeButton);
		AddToList(OpenTrainButton);
		AddToList(SpeedUpButton);
		AddToList(RetireButton);
		AddToList(FavoriteButton);
		AddToList(OutfitButton);
		AddToList(ShareButton);
		AddToList(GenericOkButton);
		AddToList(NextSurvivorButton);
		AddToList(PreviousSurvivorButton);
		AddToList(BounsButton);
		AddToList(PreviewMaxStatsButton);
		AddToList(PreviewMaxStatsReturnButton);
		AddToList(UnlockUiButton);
		AddToList(MoreTokensUiButton);
		AddToList(SurvivorDescriptionLabel);
		AddToList(AcceptFromMissionParent);
		AddToList(TraitRerollPanel);
	}

	public virtual void HideAllContent()
	{
		if (AllGameObjects != null)
		{
			for (int i = 0; i < AllGameObjects.Count; i++)
			{
				Helpers.GameObjectSetActive(AllGameObjects[i], value: false);
			}
		}
	}

	public void SetState(States state)
	{
		if (stateMachine != null)
		{
			stateMachine.TrySwitchToState((int)state);
		}
	}

	public void SwitchToPreviousState()
	{
		if (stateMachine != null)
		{
			stateMachine.SwitchToPreviousState();
		}
	}

	public void LockStateMachine(bool lockState)
	{
		if (stateMachine != null)
		{
			stateMachine.LockCurrentState = lockState;
		}
	}

	protected virtual void UpdateAndShowStats()
	{
		if (SurvivorStatistics != null)
		{
			SurvivorStatistics.SetInfo(SurvivorModel, SurvivorInfoPopup.AllowWeapons);
			Helpers.GameObjectSetActive(SurvivorStatistics, value: true);
		}
		if (RarityAndClass != null)
		{
			RarityAndClass.UpdateWithSurvivor(SurvivorModel);
			Helpers.GameObjectSetActive(RarityAndClass, value: true);
		}
	}

	protected virtual void UpdateAndShowTraits()
	{
		if (SurvivorTraitsList != null && SurvivorModel != null)
		{
			SurvivorTraitsList.UpdateWith(SurvivorModel);
			Helpers.GameObjectSetActive(SurvivorRightSidePanel, value: true);
			Helpers.GameObjectSetActive(SurvivorTraitsList, value: true);
		}
	}

	protected virtual void UpdateAndShowBadges()
	{
		if (SurvivorBadgesPanel != null && SurvivorModel != null)
		{
			SurvivorBadgesPanel.UpdateWith(SurvivorModel);
			Helpers.GameObjectSetActive(SurvivorRightSidePanel, value: true);
			if (SurvivorRightSidePanel != null)
			{
				SurvivorRightSidePanel.SetActiveButtons(value: true);
			}
		}
	}

	protected virtual void UpdateAndShowName()
	{
		if (!Helpers.GameObjectSetActive(SurvivorNamePanel, value: true) || OfflineManager.IsLoadDataManager || SingularityMonoBehaviour<FullscreenActorOverlay>.Instance.NameTargets.Length > 1)
		{
			if (!OfflineManager.IsLoadDataManager) 
			{
                SurvivorNamePanel.transform.OverlayPosition(SingularityMonoBehaviour<FullscreenActorOverlay>.Instance.NameTargets[1].transform.position, SingularityMonoBehaviour<FullscreenActorOverlay>.Instance.gameObject.GetComponentInChildren<Camera>());
            }
            Vector3 localPosition = SurvivorNamePanel.transform.localPosition;
			localPosition.z = 0f;
			SurvivorNamePanel.transform.transform.localPosition = localPosition;
			SurvivorNamePanel.setInfo(SurvivorModel);
			SurvivorNamePanel.EnableNameInput(!SurvivorModel.IsHero);
		}
	}

	protected virtual void UpdateAndShowExtraButtons()
	{
		bool flag = TutorialView.Instance == null || TutorialView.Instance.Model == null || TutorialView.Instance.Model.StaticTutorialComplete;
		if (!OfflineManager.IsLoadDataManager && OutfitButton != null)
		{
			Helpers.GameObjectSetActive(OutfitButton, flag && (!SurvivorModel.IsHero || GameManager.Instance.GetHeroSkinResourceEntry(SurvivorModel.Definition.ID) != null));
		}
		if (ShareButton != null)
		{
			Helpers.GameObjectSetActive(ShareButton, flag);
		}
		if (RetireButton != null && SurvivorModel != null)
		{
			Helpers.GameObjectSetActive(RetireButton, !SurvivorModel.IsHero && !SurvivorModel.IsUpgrading() && !SurvivorModel.IsFavourite);
		}
		if (FavoriteButton != null)
		{
			Helpers.GameObjectSetActive(FavoriteButton, CurrentState == States.SurvivorOverview);
		}
		if (BounsButton != null)
		{
			List<BounsInfoDefinition> bounsInfoDefinitionsByOwner = GameManager.Instance.gameEconomyData.GetBounsInfoDefinitionsByOwner(SurvivorModel?.Definition.ID);
			Helpers.GameObjectSetActive(BounsButton, bounsInfoDefinitionsByOwner.Count > 0);
		}
		UpdateAndShowSurvivorNavigationButtons();
	}

	protected void UpdateAndShowSurvivorNavigationButtons()
	{
		SurvivorInfoPopup survivorInfoPopup = OfflineManager.IsLoadDataManager ? DataManager.Instance.SurvivorManagementPopUp.SurvivorInfoPopupCurrent : (SurvivorInfoPopup)SingularityMonoBehaviour<HUDManager>.Instance.GetNoCreation(UIType.CampSurvivorInfoPopup);
		bool value = survivorInfoPopup != null && survivorInfoPopup.CanSwitchSurvivor();
		Helpers.GameObjectSetActive(NextSurvivorButton, value);
		Helpers.GameObjectSetActive(PreviousSurvivorButton, value);
	}

	protected void PlayAnchorTween(MonoBehaviour obj, TweenAnchorId id)
	{
		if (obj != null)
		{
			PlayAnchorTween(obj.gameObject, id);
		}
	}

	protected void PlayAnchorTween(GameObject obj, TweenAnchorId id)
	{
		if (!(obj != null))
		{
			return;
		}
		TweenAnchors[] components = obj.GetComponents<TweenAnchors>();
		for (int i = 0; i < components.Length; i++)
		{
			if (components[i] != null && !string.IsNullOrEmpty(components[i].id))
			{
				if (components[i].id == id.ToString())
				{
					components[i].PlayForward();
				}
				else
				{
					components[i].Reset();
				}
			}
		}
	}

	private void AddToList(GameObject gameObject)
	{
		if (gameObject != null && AllGameObjects != null && !AllGameObjects.Contains(gameObject))
		{
			AllGameObjects.Add(gameObject);
		}
	}

	private void AddToList(MonoBehaviour monoObj)
	{
		if (monoObj != null)
		{
			AddToList(monoObj.gameObject);
		}
	}

	protected virtual void UpdateSurvivalManualPanel()
	{
		if (SurvivalManualPanel != null && SurvivorModel != null)
		{
			SurvivalManualPanel.UpdateUI(SurvivorModel);
		}
	}
}
