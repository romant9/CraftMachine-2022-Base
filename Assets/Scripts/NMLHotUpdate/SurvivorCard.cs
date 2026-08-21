using System;
using System.Collections.Generic;
using System.Linq;
using BaseModel;
using TWDModel;
using UnityEngine;
using TwdCustomMod;

public class SurvivorCard : UIListCard<SurvivorModel>
{
	public enum CardType
	{
		Basic = 0,
		TrainingGround = 1,
		TrainingGroundAcceptingSurvivor = 2,
		TeamSelect = 3,
		RadioPhone = 4,
		RadioPhoneForReroll = 5,
		EnemyPreview = 6
	}

	[Header("WorldBoss info")]
	[SerializeField]
	private GameObject worldBossTiredContainer;

	[SerializeField]
	private GameObject worldBossTiredUpContainer;

	[SerializeField]
	private GameObject worldBossTiredTimeContainer;

	[SerializeField]
	private UILabel worldBossTiredTimer;

	[SerializeField]
	private GameObject worldBossNoTiredContainer;

	[SerializeField]
	private GameObject worldBossSetContainer;

	[SerializeField]
	private UIButton worldBossSetButton;

	[SerializeField]
	private UILabel worldBossSetTimeTxt;

	[SerializeField]
	private UILabel worldBossSetNameTxt;

	[SerializeField]
	private GameObject worldBossGetContainer;

	[SerializeField]
	private UIButton worldBossGetButton;

	[SerializeField]
	private UILabel worldBossGetTimeTxt;

	[SerializeField]
	private UILabel worldBossGetNameTxt;

	private static readonly Color WorldBossTiredFullColor = new Color(32f / 51f, 67f / 85f, 0.18431373f, 1f);

	private static readonly Color WorldBossTiredTwoColor = new Color(0.96862745f, 0.7607843f, 0.14509805f, 1f);

	private static readonly Color WorldBossTiredOneColor = new Color(0.99215686f, 0.20784314f, 0.20784314f, 1f);

	private bool isWorldBossTiredTimerActive;

	private bool worldBossTiredTimerUseFullTimeFormat;

	private bool isWorldBossGetTimerActive;

	[Header("Main surivor info")]
	[SerializeField]
	private UILabel nameLabel;

	[SerializeField]
	private UILabel descriptionLabel;

	[SerializeField]
	private UILabel damageLabel;

	[SerializeField]
	private UILabel healthLabel;

	[SerializeField]
	private UITexture portrait;

	[SerializeField]
	private UISprite classIcon;

	[SerializeField]
	private UILabel levelLabel;

	[SerializeField]
	private UILabel maxLevelLabel;

	[SerializeField]
	private PayButton speedupButton;

	[Header("Various indicators")]
	[SerializeField]
	private GameObject featuredHeroTab;

	[SerializeField]
	private GameObject featuredHeroGlow;

	[SerializeField]
	private UITable cardTabsParent;

	[SerializeField]
	private GameObject heroEffect;

	[SerializeField]
	private GameObject heroTab;

	[SerializeField]
	private GameObject altHeroEffect;

	[SerializeField]
	private GameObject altHeroTab;

	[SerializeField]
	private UISprite raritySprite;

	[SerializeField]
	private UISprite rarityBorderLeft;

	[SerializeField]
	private UISprite rarityBorderRight;

	[SerializeField]
	private UISprite rarityBorderSmallLeft;

	[SerializeField]
	private UISprite rarityBorderSmallRight;

	[SerializeField]
	private UILabel newSurvivorLabel;

	[SerializeField]
	private UISprite unknownSurvivorSprite;

	[SerializeField]
	private GameObject injuredOverlay;

	[SerializeField]
	private GameObject incapacitatedOverlay;

	[SerializeField]
	private UILabel incapacitatedLabel;

	[SerializeField]
	private UILabel incapacitatedTurnCounter;

	[SerializeField]
	private UIButton infoButton;

	[SerializeField]
	private GameObject bittenIcon;

	[SerializeField]
	private GameObject statsContainer;

	[SerializeField]
	private UILabel statsDamageLabel;

	[SerializeField]
	private UILabel extraDamageLabel;

	[SerializeField]
	private UILabel statsHealthLabel;

	[SerializeField]
	private GameObject SelectedParent;

	[SerializeField]
	private GameObject SelectedGlow;

	[SerializeField]
	private Color NormalStatsColor;

	[SerializeField]
	private Color FeaturedStatsColor;

	[Header("Upgrades")]
	[SerializeField]
	private GameObject upgradeBox;

	[SerializeField]
	private UILabel upgradeBoxLabel;

	[SerializeField]
	private GameObject indicatorContainer;

	[SerializeField]
	private UILabel indicatorLabel;

	[SerializeField]
	private UIProgressBar upgradeProgressBar;

	[SerializeField]
	private GameObject possibleBetterWeapon;

	[SerializeField]
	private GameObject possibleBetterArmor;

	[Header("Survival Mode")]
	[SerializeField]
	private GameObject survivalContainer;

	[SerializeField]
	private GameObject survivalOutOfActionContainer;

	[SerializeField]
	private GameObject survivalHealthBar;

	[SerializeField]
	private UISprite survivalHealthTintablePart;

	[SerializeField]
	private GameObject survivalRestHealthBar;

	[SerializeField]
	private GameObject survivalRestEffectContainer;

	[SerializeField]
	private List<UISprite> survivalChargePoints = new List<UISprite>();

	[SerializeField]
	[Tooltip("Tint color of the health bar depending on the struggles left. Index 0: No strugges left, 1 for one struggle left.")]
	private Color[] survivalHealthBarTints;

	[Header("Bottom Part")]
	[SerializeField]
	private GameObject upgradeCanAffordParent;

	[SerializeField]
	private GameObject upgradeCanNotAffordParent;

	[SerializeField]
	private UILabel upgradePriceLabel;

	[SerializeField]
	private GameObject bottomMessageParent;

	[SerializeField]
	private UILabel bottomMessageLabel;

	[SerializeField]
	private UILabel tokenPriceLabel;

	[SerializeField]
	private UISprite tokenIcon;

	[SerializeField]
	private GameObject healTokenSpeedupContainer;

	[SerializeField]
	public SurvivorCardTokenAccept surviorCardTokenAccept;

	[SerializeField]
	private SurvivorCardRerollLocking survivorCardRerollLocking;

	[SerializeField]
	[Tooltip("The tint color for labels when the user has enough currency.")]
	private Color availableCurrencyColor = Color.white;

	[SerializeField]
	[Tooltip("The tint color for labels when the user has NOT enough currency.")]
	private Color unavailableCurrencyColor = new Color(0.511f, 0.129f, 0.027f, 1f);

	[Header("Injuries: curing timer")]
	[SerializeField]
	private GameObject injuryTimerContainer;

	[SerializeField]
	private UILabel injuryTimerLabel;

	[Header("Injuries: used after combat in end screen")]
	[SerializeField]
	private GameObject injuryEndScreenContainer;

	[SerializeField]
	private UIProgressBar injuryEndScreenTimerProgressBar;

	[SerializeField]
	private UILabel injuryEndScreenDeadLabel;

	[SerializeField]
	private UILabel injuryEndScreenTypeLabel;

	[SerializeField]
	private UILabel injuryEndScreenTimerLabel;

	[SerializeField]
	private GameObject injuryEndScreenDeadContainer;

	[SerializeField]
	private UISprite injuryEndScreenInjuryTypeContainer;

	[SerializeField]
	[Tooltip("Color of the health bar depending on the injury. Index 0: not injured. Then minor, major, critical.")]
	private Color[] injuryEndScreenProgressBarColors;

	[SerializeField]
	[Tooltip("Color of the health bar depending on the injury. Index 0: not injured. Then minor, major, critical.")]
	private Color[] injuryEndScreenCardBgColors;

	[SerializeField]
	[Tooltip("Color of the health bar depending on the injury. Index 0: not injured. Then minor, major, critical.")]
	private Color[] injuryEndScreenCardTextColors;

	[Header("Equipment")]
	[SerializeField]
	private GameObject equipmentPrefab;

	[SerializeField]
	private GameObject equipmentContainer;

	[SerializeField]
	private GameObject weaponPosition;

	[SerializeField]
	private GameObject armorPosition;

	[Header("Sacrifice")]
	[SerializeField]
	private GameObject sacrificeContainer;

	[SerializeField]
	private UILabel sacrificeLabel;

	[Header("Team select")]
	[SerializeField]
	private GameObject teamSelectionContainer;

	[Header("Survivor unavailable")]
	[SerializeField]
	private GameObject survivorUnavailableContainer;

	[Header("Full Info Button")]
	[SerializeField]
	private UIButton fullInfoButton;

	[Header("Traits Sprites Panels")]
	[SerializeField]
	private UISprite[] traitsPanels;

	[SerializeField]
	private Color traitsLockedColor;

	[Header("Rarity Stars Sprite Panels")]
	[SerializeField]
	private UISprite[] starsPanels;

	[SerializeField]
	private UISprite[] featuredStars;

	[SerializeField]
	private UILabel teamSelectionLabel;

	[SerializeField]
	[Tooltip("Prefab for an leader trait card")]
	private GameObject leaderTraitCardPrefab;

	[SerializeField]
	private GameObject leaderSlotTutorialArrow;

	[SerializeField]
	private GameObject leaderSelectTutorialArrow;

	[SerializeField]
	private GameObject survivorHealEffect;

	[Header("Tutorial arrows")]
	[SerializeField]
	private TutorialArrowParent tutorialArrowHeroSelect;

	[SerializeField]
	private UISprite talkingDeadIcon;

	[SerializeField]
	private SurvivorCardBadgeElement badgeElement;

	[SerializeField]
	private SurvivorFavouriteVisibility favouriteVisibility;

	private LeaderTraitVisual leaderTraitVisual;

	private TutorialArrowParent tutorialArrow;

	private bool isUnrevealed;

	private List<UIWidget> itemsToReveal = new List<UIWidget>();

	private List<UIWidget> itemsToHide = new List<UIWidget>();

	private static float REVEAL_ANIMATION_DURATION = 1f;

	private float revealAnimationTime = -1f;

	private int upgradeProgressBarHeight;

	private MedicTentModel medicTentModelCached;

	private UIWidget widgetCached;

	private float survivalHealthNormalized;

	public bool SurvivorUnavailable;

	private bool repositionTable;

	public Func<List<SurvivorModel>> SurvivorsFilterDelegate;

	private TimedQueueItemModel injuryTimer;

	private EquipmentButton weaponCard;

	private EquipmentButton armorCard;

	private bool canClick = true;

	private bool hasOpendInfoPopup;

	private SurvivorInfoPopup survivorInfoPopup;

	public bool WorldBossSelectedTeamTiredOnlyDisplay { get; set; }

	public bool Locked { get; set; }

	public bool IsMissionSurvivor { get; set; }

	public bool IsEndlessModeExpertActor { get; set; }

	public bool IsSurvivalMode { get; set; }

	public SurvivorContainerModel.SurvivorType TeamSelectionSurvivorType { get; set; }

	public bool IsOutOfAction { get; set; }

	public bool IsGuildWarMode { get; set; }

	public CardType Type { get; set; }

	public bool Selected { get; set; }

	public UIWidget widget => widgetCached;

	private MedicTentModel medicTentModel
	{
		get
		{
			if (medicTentModelCached == null && GameManager.Instance != null && GameManager.Instance.playerModel != null && GameManager.Instance.playerModel.Camp != null)
			{
				medicTentModelCached = GameManager.Instance.playerModel.Camp.GetBuilding("MedicTent") as MedicTentModel;
			}
			return medicTentModelCached;
		}
	}

	public bool IsAnySurvivorUpgrading => GameManager.Instance.playerModel.SurvivorContainer.HasUpgradingSurvivor;

	private void Awake()
	{
		if (tutorialArrow == null)
		{
			tutorialArrow = GetComponentInChildren<TutorialArrowParent>();
		}
		if (leaderSlotTutorialArrow != null)
		{
			leaderSlotTutorialArrow.SetActive(value: false);
		}
		if (leaderSelectTutorialArrow != null)
		{
			leaderSelectTutorialArrow.SetActive(value: false);
		}
		isUnrevealed = false;
		if (upgradeProgressBar != null)
		{
			upgradeProgressBarHeight = upgradeProgressBar.backgroundWidget.height;
		}
		widgetCached = GetComponent<UIWidget>();
	}

	private void OnEnable()
	{
		if (medicTentModel != null)
		{
			medicTentModel.Changed += OnMedicTentChanged;
		}
		UIEvent.OnUIEvent += OnUIEvent;
		SurvivorUnavailable = false;
	}

	private void OnDisable()
	{
		SurvivorModel item = base.Item;
		if (item != null)
		{
			item.Changed -= OnSurvivorModelChanged;
		}
		if (medicTentModel != null)
		{
			medicTentModel.Changed -= OnMedicTentChanged;
		}
		UIEvent.OnUIEvent -= OnUIEvent;
		IsSurvivalMode = false;
		TeamSelectionSurvivorType = SurvivorContainerModel.SurvivorType.Combat;
		isWorldBossTiredTimerActive = false;
		isWorldBossGetTimerActive = false;
		WorldBossSelectedTeamTiredOnlyDisplay = false;
	}

	public void OnWorldBossSetButtonClicked()
	{
		SurvivorModel survivor = base.Item;
		WorldBossModelManager worldBossModelManager = GameManager.Instance?.playerModel?.WorldBossModelManager;
		if (survivor == null || worldBossModelManager == null || string.IsNullOrEmpty(survivor.IdForAnalytics))
		{
			return;
		}
		WorldBossDispatchedTeamView worldBossDispatchedTeamView = worldBossModelManager.GetMyDispatchedTeams().FirstOrDefault((WorldBossDispatchedTeamView team) => team.SurvivorIds != null && team.SurvivorIds.Contains(survivor.IdForAnalytics));
		if (worldBossDispatchedTeamView == null)
		{
			Debug.LogError("SurvivorCard: deployed team not found for survivor " + survivor.IdForAnalytics);
			return;
		}
		WorldBossRetreatConfirmPopup worldBossRetreatConfirmPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.WorldBossRetreatConfirmPopup) as WorldBossRetreatConfirmPopup;
		if (!(worldBossRetreatConfirmPopup == null))
		{
			worldBossRetreatConfirmPopup.SetTeam(worldBossDispatchedTeamView);
			worldBossRetreatConfirmPopup.Open();
		}
	}

	public void OnWorldBossGetButtonClicked()
	{
		SurvivorModel survivor = base.Item;
		WorldBossModelManager worldBossModelManager = GameManager.Instance?.playerModel?.WorldBossModelManager;
		if (survivor == null || worldBossModelManager == null || string.IsNullOrEmpty(survivor.IdForAnalytics) || worldBossModelManager.WorldBossGuildFullSnapshot == null)
		{
			return;
		}
		WorldBossReturningTeamView worldBossReturningTeamView = (from team in worldBossModelManager.GetMyReturningTeams()
			where team.SurvivorIds != null && team.SurvivorIds.Contains(survivor.IdForAnalytics)
			orderby team.RemainingMs descending
			select team).FirstOrDefault();
		if (worldBossReturningTeamView == null)
		{
			Debug.LogError("SurvivorCard: returning team not found for survivor " + survivor.IdForAnalytics);
			return;
		}
		WorldBossRecallPopup worldBossRecallPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.WorldBossRecallPopup) as WorldBossRecallPopup;
		if (!(worldBossRecallPopup == null))
		{
			worldBossRecallPopup.SetReturningTeam(worldBossReturningTeamView);
			worldBossRecallPopup.Open();
		}
	}

	private void OnUIEvent(string type, object parameter)
	{
		if (type == "SurvivorExtraSlotBought")
		{
			UpdateUI();
			return;
		}
		if (type == "OnNewEquipmentEquiped" && parameter != null && parameter is EquipmentButton)
		{
			if (base.Item != null && base.Item != null)
			{
				SurvivorModel item = base.Item;
				EquipmentButton equipmentButton = parameter as EquipmentButton;
				if (equipmentButton != null && equipmentButton.GetOwningSurvivor() != null && item != null && equipmentButton.GetOwningSurvivor() == item)
				{
					UpdateUI();
				}
			}
			return;
		}
		switch (type)
		{
		case "OnPopUpOpen":
		{
			bool num = base.Item != null;
			SurvivorModel item2 = base.Item;
			if (num && item2 != null && item2.IsFavourite && favouriteVisibility != null)
			{
				repositionTable = true;
			}
			break;
		}
		case "OnHeroSkinViewClosed":
			if (base.Item != null && parameter is SurvivorModel survivorModel2 && base.Item == survivorModel2)
			{
				UpdateUI();
			}
			break;
		case "OnOutfitViewClosed":
			if (base.Item != null && parameter is SurvivorModel survivorModel && base.Item == survivorModel)
			{
				UpdateUI();
			}
			break;
		}
	}

	public override void UpdateUI()
	{
		try
		{
			bool flag = base.Item != null;
			SurvivorModel item = base.Item;
			if (flag && item != null)
			{
				bool flag2 = item.InjuryType != InjuryType.None;
				if (item.IsUpgrading() && !IsSurvivalMode)
				{
					item.Changed -= OnSurvivorModelChanged;
					item.Changed += OnSurvivorModelChanged;
				}
				UpdateEquipmentHint(item);
				UpdateBasicStats(item);
				UpdatePortrait(item);
				UpdateWeaponAndEquipmentCard(item);
				if (IsSurvivalMode)
				{
					SurvivalCharacterStateModel survivorStateInSurvivalMode = item.manager.Player.SurvivorContainer.SurvivalCharacters.GetSurvivorStateInSurvivalMode(item);
					if (survivorStateInSurvivalMode != null)
					{
						UpdateSurvivalUIEnabled(item, enabled: true);
						UpdateSurvivalHealthUI(item, survivorStateInSurvivalMode);
						UpdateSurvivalChargeUI(item, survivorStateInSurvivalMode);
						UpdateSurvivalOutOfActionUI(item, survivorStateInSurvivalMode);
					}
					else if (Type != CardType.RadioPhone && Type != CardType.RadioPhoneForReroll)
					{
						UpdateSurvivalNotIncludedInMode(item);
					}
				}
				else
				{
					UpdateSurvivalUIEnabled(item, enabled: false);
					Helpers.GameObjectSetActive(survivalOutOfActionContainer, IsOutOfAction);
				}
				UpdateUpgradeIndicator(item);
				UpdateUpgradeInProgress(item);
				UpdateInjuredUI(item);
				survivorUnavailableContainer.SetActive(SurvivorUnavailable);
				UpdateTraits(traitsPanels, item);
				HelpersGfx.SetSurvivorRarityRating(starsPanels, item.SurvivorRarityLevel);
				if (!IsMissionSurvivor)
				{
					HelpersGfx.SetSurvivorFeaturedStars(featuredStars, item);
				}
				ShowEquipmentContainers(!flag2);
				UpdateSurviorCardTokenAccept(item);
				UpdateSelectedState();
				UpdateSurviorCardReroll(item);
				UpdateLeaderTraitVisual(item);
				UpdateBadgeBonusAmount(item);
				if (tutorialArrow != null)
				{
					tutorialArrow.Id = (IsInCombatTeam() ? ("SurvivorCard_" + item.Name) : "SurvivorCardReserve");
				}
				if (tutorialArrowHeroSelect != null)
				{
					tutorialArrowHeroSelect.Id = "HeroSelect_" + base.Item.ActorDefinitionID;
				}
				if (bittenIcon != null && bittenIcon.gameObject != null)
				{
					bittenIcon.gameObject.SetActive(value: false);
				}
				if (sacrificeContainer != null)
				{
					sacrificeContainer.SetActive(value: false);
				}
				Helpers.GameObjectSetActive(talkingDeadIcon, item.ActorDefinitionID.ToLower().Contains("talkingdead"));
				favouriteVisibility.UpdateVisibility(item);
			}
			if (upgradeCanAffordParent != null && upgradeCanAffordParent.gameObject != null)
			{
				upgradeCanAffordParent.gameObject.SetActive(value: false);
			}
			if (upgradeCanNotAffordParent != null && upgradeCanNotAffordParent.gameObject != null)
			{
				upgradeCanNotAffordParent.gameObject.SetActive(value: false);
			}
			if (bottomMessageParent != null && bottomMessageParent.gameObject != null)
			{
				bottomMessageParent.gameObject.SetActive(value: false);
			}
			if (nameLabel != null && nameLabel.gameObject != null)
			{
				nameLabel.gameObject.SetActive(flag);
			}
			if (damageLabel != null && damageLabel.gameObject != null)
			{
				damageLabel.gameObject.SetActive(flag);
			}
			BoxCollider boxCollider = ((base.gameObject != null) ? base.gameObject.GetComponent<BoxCollider>() : null);
			if (boxCollider != null)
			{
				boxCollider.enabled = flag;
			}
			if (infoButton != null && infoButton.gameObject != null)
			{
				infoButton.gameObject.SetActive(flag);
			}
			if (Type != CardType.TeamSelect)
			{
				ShowTeamSelection(null);
			}
			if (fullInfoButton != null && fullInfoButton.gameObject != null)
			{
				if (Type == CardType.TeamSelect || ((Type == CardType.RadioPhone || Type == CardType.RadioPhoneForReroll || Type == CardType.EnemyPreview) && !Locked))
				{
					fullInfoButton.gameObject.SetActive(value: true);
				}
				else
				{
					fullInfoButton.gameObject.SetActive(value: false);
				}
			}
			if (base.gameObject.activeInHierarchy)
			{
				repositionTable = true;
			}
			InitActorHitMessage();
			SetExtraAttackLabel();
			UpdateTeamSelectionWorldBossInfo();
			if (survivalPanel != null && survivalPanel.gameObject.activeSelf) survivalPanel.UpdateUI(base.Item);
		}
		catch (Exception ex)
		{
			Debug.LogError("Error happend when updating SurvivorCard: " + ex.ToString());
		}
	}

	private void UpdateBadgeBonusAmount(SurvivorModel survivor)
	{
		if (survivor.BadgeContainer != null && survivor.BadgeContainer.Badges.Count > 0)
		{
			if (!(badgeElement != null))
			{
				return;
			}
			List<ActorModel> list = new List<ActorModel>();
			bool flag = false;
			for (int i = 0; i < GameManager.Instance.playerModel.SurvivorContainer.CombatSurvivors.Count; i++)
			{
				if (GameManager.Instance.playerModel.SurvivorContainer.CombatSurvivors[i] == survivor)
				{
					flag = true;
				}
				list.Add(GameManager.Instance.playerModel.SurvivorContainer.CombatSurvivors[i]);
			}
			BadgeContext context = new BadgeContext(survivor, flag ? list : null);
			Helpers.GameObjectSetActive(badgeElement, value: true);
			badgeElement.SetData(survivor.BadgeContainer, context);
			badgeElement.UpdateUI();
		}
		else
		{
			Helpers.GameObjectSetActive(badgeElement, value: false);
		}
	}

	private void UpdateSelectedState()
	{
		if (Type == CardType.RadioPhoneForReroll)
		{
			Helpers.GameObjectSetActive(SelectedGlow, value: false);
			Helpers.GameObjectSetActive(SelectedParent, value: false);
		}
		else if (Type == CardType.RadioPhone)
		{
			bool value = IsLoadDataManager ? !Locked : Selected && !Locked;
			Helpers.GameObjectSetActive(SelectedParent, value);
			bool value2 = true;
			if (GameManager.Instance.playerModel != null && GameManager.Instance.playerModel.PhoneCall != null)
			{
				value2 = !GameManager.Instance.playerModel.PhoneCall.CanClaimEntireMultiLootsList();
			}
			Helpers.GameObjectSetActive(SelectedGlow, value2);
			if (!OfflineManager.IsNoEffects) TweenManager.PlayTweenGroup(SelectedParent, 3);
		}
		else
		{
			Helpers.GameObjectSetActive(SelectedGlow, value: false);
			Helpers.GameObjectSetActive(SelectedParent, value: false);
		}
	}

	private void UpdateBasicStats(SurvivorModel survivor)
	{
		if (survivor == null)
		{
			return;
		}
		if (nameLabel != null)
		{
			bool flag = false;
			if (survivor != null && survivor.manager != null && survivor.manager.Player != null && survivor.manager.Player.SurvivorContainer != null && survivor.manager.Player.SurvivorContainer.Survivors != null)
			{
				flag = GameManager.Instance.playerModel.SurvivorContainer.Survivors.Contains(survivor);
			}
			nameLabel.text = (flag ? survivor.Name : GameManager.Instance.GetFilteredText(survivor.Name));
		}
		if (descriptionLabel != null)
		{
			string text = "";
			TraitDefinition traitWithTag = survivor.GetTraitWithTag("Personality");
			if (traitWithTag != null && SingularityMonoBehaviour<LocalizationManager>.Instance != null)
			{
				text = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Personality." + traitWithTag.Identifier + ".Description{Name}", survivor.Name);
			}
			descriptionLabel.text = text;
		}
		if (damageLabel != null)
		{
			damageLabel.text = survivor.GetDamageForPreferredWeapon().ToString();
		}
		if (levelLabel != null)
		{
			levelLabel.text = survivor.Level.ToString();
		}
		if (maxLevelLabel != null && SingularityMonoBehaviour<LocalizationManager>.Instance != null)
		{
			maxLevelLabel.text = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("SurvivorCard.Of") + " " + survivor.MaxUpgradeLevel;
		}
		if (classIcon != null)
		{
			classIcon.spriteName = HelpersGfx.GetSurvivorClassIconName(survivor);
		}
		if (healthLabel != null)
		{
			healthLabel.text = survivor.GetHitpoints().ToString();
		}
		if (raritySprite != null)
		{
			HelpersGfx.UpdateSpriteAndKeepScale(raritySprite, HelpersGfx.GetSurvivorRarityEdgeSpriteName(survivor.SurvivorRarityLevel));
		}
		if (rarityBorderLeft != null)
		{
			HelpersGfx.UpdateSpriteAndKeepScale(rarityBorderLeft, HelpersGfx.GetRarityBorderSpriteName(survivor.SurvivorRarityLevel));
			HelpersGfx.UpdateSpriteAndKeepScale(rarityBorderRight, HelpersGfx.GetRarityBorderSpriteName(survivor.SurvivorRarityLevel));
			HelpersGfx.UpdateSpriteAndKeepScale(rarityBorderSmallLeft, HelpersGfx.GetRarityBorderSpriteName(survivor.SurvivorRarityLevel));
			HelpersGfx.UpdateSpriteAndKeepScale(rarityBorderSmallRight, HelpersGfx.GetRarityBorderSpriteName(survivor.SurvivorRarityLevel));
		}
		int hitpoints = survivor.GetHitpoints();
		int damageForPreferredWeapon = survivor.GetDamageForPreferredWeapon();
		bool flag2 = false;
		bool flag2_next = false;
		bool flag3 = survivor.IsHero || survivor.GetTraitWithTag("FactionBuffTrait") != null;
		if (GameManager.Instance.playerModel.Tutorial.HasCompletedPart("Phone"))
		{
			FeaturedHeroDefinition featuredDefinition = survivor.FeaturedDefinition;
			if (featuredDefinition != null)
			{
				flag2 = !IsMissionSurvivor;
				hitpoints += (int)((float)hitpoints * ((float)featuredDefinition.HealthBoostMultiplier / 100f));
				damageForPreferredWeapon += (int)((float)damageForPreferredWeapon * ((float)featuredDefinition.DamageBoostMultiplier / 100f));
			}
			else
			{
				FeaturedHeroDefinition featuredDefinitionNext = survivor.FeaturedDefinitionNext;
				if (featuredDefinitionNext != null)
				{
					flag2_next = !IsMissionSurvivor;
					hitpoints += (int)((float)hitpoints * ((float)featuredDefinitionNext.HealthBoostMultiplier / 100f));
					damageForPreferredWeapon += (int)((float)damageForPreferredWeapon * ((float)featuredDefinitionNext.DamageBoostMultiplier / 100f));
				}
			}
		}

		HelpersUI.SetColor(statsHealthLabel, flag2 ? FeaturedStatsColor : NormalStatsColor);
		HelpersUI.SetColor(statsDamageLabel, flag2 ? FeaturedStatsColor : NormalStatsColor);
		HelpersUI.SetContentToLabel(statsDamageLabel, survivor.GetCommonDamage().ToString());
		HelpersUI.SetContentToLabel(statsHealthLabel, survivor.GetCommonHealth().ToString());
		Helpers.GameObjectSetActive(featuredHeroTab, flag2);
		Helpers.GameObjectSetActive(featuredHeroGlow, flag2);
		Helpers.GameObjectSetActive(heroEffect, flag3 && !survivor.IsAlternativeHero);
		Helpers.GameObjectSetActive(heroTab, flag3 && !survivor.IsAlternativeHero);
		Helpers.GameObjectSetActive(altHeroEffect, survivor.IsAlternativeHero);
		Helpers.GameObjectSetActive(altHeroTab, survivor.IsAlternativeHero);
		Helpers.GameObjectSetActive(leaderSelectTutorialArrow, survivor.IsHero);
		cardTabsParent.repositionNow = true;

		if (flag2_next)
		{
			HelpersUI.SetColor(statsHealthLabel, FeaturedStatsColorNext);
			HelpersUI.SetColor(statsDamageLabel, FeaturedStatsColorNext);
			Helpers.GameObjectSetActive(featuredHeroTab, true);
			Helpers.GameObjectSetActive(featuredHeroGlow, true);
			var tabBg = featuredHeroTab.FindInChildren("Bg");
			if (tabBg) HelpersUI.SetColor(tabBg.GetComponent<UISprite>(), FeaturedStatsColorNext);
			var tabGlow = featuredHeroGlow.FindInChildren("Star_Hero_Glow");
			if (tabGlow) HelpersUI.SetColor(tabGlow.GetComponent<UISprite>(), FeaturedStatsColorNext);
			var tabGlowBg = featuredHeroGlow.FindInChildren("Star_Hero_Glow_Bg");
			if (tabGlowBg) HelpersUI.SetColor(tabGlowBg.GetComponent<UISprite>(), FeaturedStatsColorNext);
		}
	}

	public void UpdateSurvivorUnavailableContainerState()
	{
		survivorUnavailableContainer.SetActive(SurvivorUnavailable);
		SurvivorModel item = base.Item;
		bool flag = item == null || item.InjuryType != InjuryType.None;
		if (SurvivorUnavailable && flag)
		{
			Helpers.GameObjectSetActive(speedupButton, value: false);
		}
	}

	private void UpdateEquipmentHint(SurvivorModel survivor)
	{
		if (survivor == null)
		{
			return;
		}
		if (possibleBetterWeapon != null)
		{
			possibleBetterWeapon.SetActive(value: false);
			if (!IsMissionSurvivor && SurvivorStatisticsPanel.CheckForWeaponUpgrades(survivor) && Type != CardType.RadioPhone && Type != CardType.RadioPhoneForReroll && Type != CardType.EnemyPreview)
			{
				possibleBetterWeapon.SetActive(value: true);
			}
		}
		if (possibleBetterArmor != null)
		{
			possibleBetterArmor.SetActive(value: false);
			if (!IsMissionSurvivor && SurvivorStatisticsPanel.CheckForArmorUpgrades(survivor) && Type != CardType.RadioPhone && Type != CardType.RadioPhoneForReroll && Type != CardType.EnemyPreview)
			{
				possibleBetterArmor.SetActive(value: true);
			}
		}
	}

	private void UpdatePortrait(SurvivorModel survivor, bool isRebuild = false)
	{
		if (survivor != null && GameManager.Instance != null)
		{
			if (!(portrait != null) || !(portrait.gameObject != null) || !(PortraitManager.Instance != null))
			{
				return;
			}
			PortraitRenderSource info = PortraitRenderSource.fromActorModel(survivor);
			Texture texture = PortraitManager.Instance.GetPortrait(info);
			if (texture == null || isRebuild)
			{
				if (IsLoadDataManager)
				{
					DebugTWD.LogMycode("if (IsLoadDataManager)");
					ModularCharacter modularCharacter;
					if (info.OutfitDefinitionId == null)
					{
						modularCharacter = ActorView.SelectRandomPrefabForActor(survivor);
					}
					else
					{
						modularCharacter = ActorView.GetPrefabOverrideForActorDefinition(info.OutfitDefinitionId, info.Gender);
					}

					if (modularCharacter != null)
					{
						info.IsRebuild = isRebuild;
						PortraitManager.Instance.CreatePortrait(info, modularCharacter, OnMissingPortraitRendered);
					}
				}
				else
				{
					ModularCharacter prefabForActor = ActorView.GetPrefabForActor(survivor);
					if (prefabForActor != null)
					{
						PortraitManager.Instance.CreatePortrait(info, prefabForActor, OnMissingPortraitRendered);
					}
				}
				portrait.gameObject.SetActive(value: false);
			}
			else
			{
				portrait.mainTexture = texture;
				portrait.gameObject.SetActive(value: true);
			}
		}
		else if (portrait != null && portrait.gameObject != null)
		{
			portrait.gameObject.SetActive(value: false);
		}
	}

	private void UpdateWeaponAndEquipmentCard(SurvivorModel survivor)
	{
		if (survivor == null)
		{
			return;
		}
		GameObject gameObject = null;
		if (weaponCard == null && equipmentPrefab != null)
		{
			gameObject = Helpers.InstantiateToParent(equipmentPrefab, weaponPosition);
			if (gameObject != null)
			{
				weaponCard = gameObject.GetComponent<EquipmentButton>();
			}
		}
		gameObject = null;
		if (armorCard == null && equipmentPrefab != null)
		{
			gameObject = Helpers.InstantiateToParent(equipmentPrefab, armorPosition);
			if (gameObject != null)
			{
				armorCard = gameObject.GetComponent<EquipmentButton>();
			}
		}
		if (weaponCard != null)
		{
			weaponCard.SetupWeapon(survivor);
		}
		if (armorCard != null)
		{
			armorCard.SetupArmor(survivor);
		}
		if (weaponPosition != null)
		{
			weaponPosition.SetActive(value: true);
		}
		if (armorPosition != null)
		{
			armorPosition.SetActive(value: true);
		}
	}

	private void UpdateUpgradeIndicator(SurvivorModel survivor)
	{
		if (survivor == null)
		{
			return;
		}
		if (Type != CardType.TrainingGround)
		{
			if (indicatorContainer != null)
			{
				indicatorContainer.SetActive(value: false);
			}
			return;
		}
		if (indicatorContainer != null)
		{
			indicatorContainer.SetActive(survivor.CanUpgrade && survivor.GetUpgradeCashier(instantUpgrade: false).CanAfford());
		}
		if (indicatorLabel != null)
		{
			indicatorLabel.text = LocalizationManager.GetText("Indicator.UpgradeInside.TrainingGround");
		}
	}

	private void UpdateUpgradeInProgress(SurvivorModel survivor)
	{
		if (survivor == null)
		{
			return;
		}
		bool flag = survivor.IsUpgrading();
		bool flag2 = survivor.InjuryType != InjuryType.None;
		bool flag3 = flag && Type == CardType.TeamSelect;
		if (upgradeProgressBar != null && upgradeProgressBar.backgroundWidget != null && upgradeProgressBar.backgroundWidget.GetComponent<UISprite>() != null)
		{
			int maxNumberOfUpgrades = survivor.manager.Player.gameEconomyData.GetMaxNumberOfUpgrades(TWDModel.UpgradeType.SurvivorUpgrade);
			upgradeProgressBar.backgroundWidget.GetComponent<UISprite>().height = 2 * upgradeProgressBarHeight / maxNumberOfUpgrades;
			upgradeProgressBar.value = (float)(survivor.Level - survivor.StartingLevel) * 0.1f;
		}
		if (upgradeBox != null)
		{
			upgradeBox.SetActive(flag);
		}
		if (upgradeBoxLabel != null && survivor.TimedActionModel != null)
		{
			upgradeBoxLabel.text = Helpers.FormatTimeNoZero(survivor.TimedActionModel.MillisecondsTillCompletion);
		}
		if (IsSurvivalMode || Type == CardType.EnemyPreview)
		{
			Helpers.GameObjectSetActive(speedupButton, flag3);
		}
		else
		{
			Helpers.GameObjectSetActive(speedupButton, flag2 || flag3);
			if (!flag3 && flag2)
			{
				speedupButton.UpdateUI(medicTentModel.GetFinishOneCashier(survivor), LocalizationManager.GetText("Popup.MedicTent.Button.SpeedupOneSurvivor"));
			}
		}
		if (flag3)
		{
			speedupButton.UpdateUI(survivor.TimedActionModel.GetSpeedUpCashier(), LocalizationManager.GetText("Popup.MedicTent.Button.SpeedupOneSurvivor"));
		}
		if (tokenPriceLabel != null && flag)
		{
			Cashier speedUpCashierWithTokens = survivor.TimedActionModel.GetSpeedUpCashierWithTokens(CurrencyType.TrainingTokenBP);
			tokenPriceLabel.text = speedUpCashierWithTokens.GetTotalCost(CurrencyType.TrainingTokenBP).ToString();
			tokenPriceLabel.color = HelpersGfx.GetAvailabilityColor(speedUpCashierWithTokens.CanAfford());
			tokenIcon.spriteName = HelpersGfx.GetCurrencyIconName(CurrencyType.TrainingTokenBP_N);
		}
	}

	private void UpdateSurvivalUIEnabled(SurvivorModel survivor, bool enabled)
	{
		Helpers.GameObjectSetActive(survivalContainer, enabled);
		if (!enabled)
		{
			Helpers.GameObjectSetActive(survivalOutOfActionContainer, value: false);
		}
	}

	private void UpdateSurvivalHealthUI(SurvivorModel survivor, SurvivalCharacterStateModel state)
	{
		if (survivalHealthBar != null)
		{
			if (state.OutOfAction)
			{
				Helpers.GameObjectSetActive(survivalHealthBar, value: false);
			}
			else
			{
				int num = 0;
				Helpers.GameObjectSetActive(survivalHealthBar, value: true);
				num = survivor.MaxHitPoints * ((int)state.HealthPercentage / 100);
				if (num < 1)
				{
					num = 1;
				}
				int num2 = survivor.MaxHitPoints;
				if (num2 < 1)
				{
					num2 = 1;
				}
				survivalHealthNormalized = (float)num / (float)num2;
				UIProgressBar component = survivalHealthBar.gameObject.GetComponent<UIProgressBar>();
				if (component != null)
				{
					component.value = survivalHealthNormalized;
				}
				if (survivalHealthTintablePart != null && survivalHealthBarTints.Length != 0)
				{
					int num3 = state.StrugglesLeft;
					if (num3 >= survivalHealthBarTints.Length)
					{
						num3 = survivalHealthBarTints.Length - 1;
					}
					survivalHealthTintablePart.color = survivalHealthBarTints[num3];
				}
			}
		}
		Helpers.GameObjectSetActive(survivalRestHealthBar, value: false);
		Helpers.GameObjectSetActive(survivalRestEffectContainer, value: false);
	}

	public void StartSurvivalRestAnimation()
	{
		Helpers.GameObjectSetActive(survivalRestEffectContainer, value: true);
		Helpers.GameObjectSetActive(survivalRestHealthBar, value: true);
	}

	public void EndSurvivalRestAnimation()
	{
		Helpers.GameObjectSetActive(survivalRestEffectContainer, value: false);
		Helpers.GameObjectSetActive(survivalRestHealthBar, value: false);
		if (survivalHealthBar != null)
		{
			UIProgressBar component = survivalHealthBar.gameObject.GetComponent<UIProgressBar>();
			if (component != null)
			{
				component.value = survivalHealthNormalized;
			}
		}
	}

	public void UpdateSurvivalRestAnimation(float normalizedAnimationTime)
	{
		if (survivalRestHealthBar != null)
		{
			float num = survivalHealthNormalized - (float)GameManager.Instance.gameEconomyData.ConfigData.SurvivalRestEffectPercentage * 0.01f;
			if (num < 0f)
			{
				num = 0f;
			}
			float value = UtilsMath.Map(normalizedAnimationTime, 0f, 1f, num, survivalHealthNormalized);
			UIProgressBar component = survivalRestHealthBar.gameObject.GetComponent<UIProgressBar>();
			if (component != null)
			{
				component.value = survivalHealthNormalized;
			}
			UIProgressBar component2 = survivalHealthBar.gameObject.GetComponent<UIProgressBar>();
			if (component2 != null)
			{
				component2.value = value;
			}
		}
	}

	private void UpdateSurvivalChargeUI(SurvivorModel survivor, SurvivalCharacterStateModel state)
	{
		if (survivalChargePoints == null)
		{
			return;
		}
		int num = 0;
		int num2 = 0;
		if (!state.OutOfAction)
		{
			num = state.ChargePoints;
			num2 = survivor.ChargeMeter.MaxLevel;
		}
		for (int i = 0; i < survivalChargePoints.Count; i++)
		{
			if (i < num2)
			{
				survivalChargePoints[i].spriteName = ((i < num) ? "Ui_Charge_Point_Fill_Yellow" : "Ui_Charge_Point_Bg");
				Helpers.GameObjectSetActive(survivalChargePoints[i].gameObject, value: true);
			}
			else
			{
				Helpers.GameObjectSetActive(survivalChargePoints[i].gameObject, value: false);
			}
		}
	}

	private void UpdateSurvivalOutOfActionUI(SurvivorModel survivor, SurvivalCharacterStateModel state)
	{
		Helpers.GameObjectSetActive(survivalOutOfActionContainer, state.OutOfAction);
	}

	private void UpdateSurvivalNotIncludedInMode(SurvivorModel survivor)
	{
		Helpers.GameObjectSetActive(survivalOutOfActionContainer, value: true);
	}

	private void UpdateInjuredUI(SurvivorModel survivor)
	{
		if (survivor == null)
		{
			return;
		}
		bool flag = survivor.InjuryType != InjuryType.None && !SurvivorUnavailable && !IsSurvivalMode && Type != CardType.EnemyPreview;
		if (injuredOverlay != null && injuredOverlay.gameObject != null)
		{
			Helpers.GameObjectSetActive(injuredOverlay, flag);
		}
		if (injuryTimerContainer != null)
		{
			injuryTimerContainer.SetActive(flag);
			if (survivor.InjuryType != InjuryType.None && injuryTimerLabel != null && medicTentModel != null && medicTentModel.TimedQueueModel != null)
			{
				injuryTimer = medicTentModel.TimedQueueModel.GetQueueItemFromItem(survivor);
				if (medicTentModel.TimedQueueModel.IsQueued(injuryTimer))
				{
					injuryTimerLabel.text = LocalizationManager.GetText("SurvivorCard.Healing.Waiting");
				}
				else
				{
					injuryTimerLabel.text = Helpers.FormatTimeNoZero(injuryTimer.MillisecondsTillCompletion);
				}
			}
		}
		if (injuryEndScreenContainer != null && (bool)injuryEndScreenContainer.gameObject)
		{
			injuryEndScreenContainer.gameObject.SetActive(value: false);
		}
		if (tokenPriceLabel != null && flag)
		{
			Cashier speedUpCashierWithTokens = survivor.TimedActionModel.GetSpeedUpCashierWithTokens(CurrencyType.HealingTokenBP);
			tokenPriceLabel.text = speedUpCashierWithTokens.GetTotalCost(CurrencyType.HealingTokenBP).ToString();
			tokenPriceLabel.color = HelpersGfx.GetAvailabilityColor(speedUpCashierWithTokens.CanAfford());
			tokenIcon.spriteName = HelpersGfx.GetCurrencyIconName(CurrencyType.HealingTokenBP_N);
		}
	}

	private void UpdateTraits(UISprite[] traitsArray, SurvivorModel survivorModel)
	{
		if (traitsArray == null || traitsArray.Length == 0 || survivorModel == null)
		{
			return;
		}
		if (IsLoadDataManager)
		{
			DebugTWD.LogMycode("if (IsLoadDataManager)");
			if (!IsProtector) IsProtector = DataManager.Instance.SurvivorManagementPopUp.IsOpen || Type == CardType.TeamSelect;
			if (IsProtector)
			{
				var traitLeader = survivorModel.UpgradeTraits.FirstOrDefault(x => x.Identifier.ToLower().Contains("leader"));
				if (traitLeader != null)
				{
					var traitFirst = survivorModel.UpgradeTraits[1];
					var indexLeader = survivorModel.UpgradeTraits.IndexOf(traitLeader);

					if (indexLeader != 1)
					{
						survivorModel.UpgradeTraits[1] = traitLeader;
						survivorModel.UpgradeTraits[indexLeader] = traitFirst;
					}
				}
			}
		}
		for (int i = 0; i < traitsArray.Length; i++)
		{
			if (!(traitsArray[i] != null) || !traitsArray[i].gameObject)
			{
				continue;
			}
			int sign = IsProtector || !IsLoadDataManager ? i + 1 : i;
			if (survivorModel.UpgradeTraits.Count > sign)
			{
				traitsArray[i].gameObject.SetActive(value: true);
				traitsArray[i].spriteName = HelpersGfx.GetSurvivorTraitIconName(survivorModel.UpgradeTraits[sign]);
				if (survivorModel.UpgradeTraits[sign].IsLocked)
				{
					traitsArray[i].color = traitsLockedColor;
				}
				else
				{
					traitsArray[i].color = Color.white;
				}
			}
			else
			{
				traitsArray[i].gameObject.SetActive(value: false);
			}
		}
	}

	private void UpdateSurviorCardTokenAccept(SurvivorModel survivorModel)
	{
		if (Helpers.GameObjectSetActive(surviorCardTokenAccept, Type == CardType.RadioPhone))
		{
			surviorCardTokenAccept.UpdateWithModel(survivorModel, Selected);
		}
	}

	private void UpdateSurviorCardReroll(SurvivorModel survivorModel)
	{
		if (Helpers.GameObjectSetActive(survivorCardRerollLocking, Type == CardType.RadioPhoneForReroll))
		{
			survivorCardRerollLocking.UpdateWithModel(survivorModel);
		}
	}

	public void UpdateLeaderTraitVisual(SurvivorModel survivorModel)
	{
		if (leaderTraitVisual != null)
		{
			Helpers.GameObjectSetActive(leaderTraitVisual, survivorModel.InjuryType == InjuryType.None || IsSurvivalMode);
		}
	}

	public void ShowTeamSelection(string txt)
	{
		if (teamSelectionContainer != null)
		{
			if (txt == null || IsMissionSurvivor || Type == CardType.EnemyPreview)
			{
				teamSelectionContainer.SetActive(value: false);
				return;
			}
			teamSelectionContainer.SetActive(value: true);
			teamSelectionLabel.text = txt;
		}
	}

	public void ShowInTeamIndicator(string txt)
	{
		if (indicatorContainer != null)
		{
			indicatorContainer.SetActive(txt != null);
		}
		if (indicatorLabel != null && txt != null)
		{
			indicatorLabel.text = txt;
		}
	}

	public void OnFullInfoClicked()
	{
		if (TutorialView.Instance.RunningButNotSuggesting || Locked)
		{
			return;
		}
		if (Type == CardType.TeamSelect || Type == CardType.RadioPhone || Type == CardType.RadioPhoneForReroll || Type == CardType.EnemyPreview)
		{
			SurvivorModel item = base.Item;
			survivorInfoPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CampSurvivorInfoPopup) as SurvivorInfoPopup;
			if (IsLoadDataManager)
			{
				DebugTWD.LogMycode("if (IsLoadDataManager)");
				DataManager.Instance.SurvivorManagementPopUp.SurvivorInfoPopupCurrent = survivorInfoPopup;
				survivorInfoPopup.transform.localScale = Vector3.one * .9f;
			}
			if (Type == CardType.TeamSelect && !IsMissionSurvivor)
			{
				survivorInfoPopup.currentStateMachineState = SurvivorInfoStateBase.States.SurvivorOverview;
			}
			else if (Type == CardType.RadioPhone || Type == CardType.RadioPhoneForReroll || IsMissionSurvivor || Type == CardType.EnemyPreview)
			{
				survivorInfoPopup.currentStateMachineState = SurvivorInfoStateBase.States.SurvivorOverviewLimited;
			}
			SurvivorFilterList currentSurvivorFilterList = new SurvivorFilterList((SurvivorsFilterDelegate != null) ? SurvivorsFilterDelegate() : null);
			survivorInfoPopup.OpenForModel(item, currentSurvivorFilterList);
			hasOpendInfoPopup = true;

			if (IsLoadDataManager && Type == CardType.TeamSelect)
			{
				DebugTWD.LogMycode("if (IsLoadDataManager)");

				TweenPosition tween = CraftSettings.Instance.TeamSelectionTween;
				if (tween) tween.PlayReverse();
			}
		}
		else
		{
			OnCardClicked();
		}
	}

	public void SetInfoButtonActive(bool active)
	{
		Helpers.GameObjectSetActive(infoButton.gameObject, active);
	}

	private void OnMissingPortraitRendered(IPortraitRenderSource info)
	{
		if (portrait != null && portrait.gameObject != null && base.Item != null && info != null && base.Item.ModelId.ToString() == info.UniqueId && base.Item.ActorDefinitionID == info.ActorDefinitionId && PortraitManager.Instance != null)
		{
			portrait.mainTexture = PortraitManager.Instance.GetPortrait(info);
			portrait.gameObject.SetActive(value: true);
		}
	}

	public void ShowSacrifice(bool show, bool isDeadly, int incapacitatedCounter, bool survivalMode)
	{
		sacrificeContainer.SetActive(show);
		injuredOverlay.gameObject.SetActive(show);
		incapacitatedOverlay.gameObject.SetActive(incapacitatedCounter > 0);
		incapacitatedLabel.text = LocalizationManager.GetText("SurvivorCard.Incapacitated");
		incapacitatedTurnCounter.text = incapacitatedCounter.ToString();
		if (survivalMode)
		{
			sacrificeLabel.text = LocalizationManager.GetText("SurvivorCard.Sacrifice.Survival");
		}
		else
		{
			sacrificeLabel.text = LocalizationManager.GetText(isDeadly ? "SurvivorCard.Sacrifice.Deadly" : "SurvivorCard.Sacrifice.NotDeadly");
		}
	}

	public void OnlyShowSurvivorTop()
	{
		UpdateUI();
		ShowEquipmentContainers(show: false);
		injuredOverlay.gameObject.SetActive(value: false);
		incapacitatedOverlay.gameObject.SetActive(value: false);
		injuryTimerContainer.gameObject.SetActive(value: false);
		indicatorContainer.SetActive(value: false);
		Helpers.GameObjectSetActive(speedupButton, value: false);
	}

	public void UpdateWorldBossTiredInfo()
	{
		SurvivorModel item = base.Item;
		WorldBossModelManager worldBossModelManager = GameManager.Instance?.playerModel?.WorldBossModelManager;
		if (item == null || worldBossModelManager == null || string.IsNullOrEmpty(item.IdForAnalytics))
		{
			isWorldBossTiredTimerActive = false;
			return;
		}
		int heroChargeLimit = worldBossModelManager.GetHeroChargeLimit();
		int heroCharges = worldBossModelManager.GetHeroCharges(item.IdForAnalytics);
		long heroNextRecoverRemainingMs = worldBossModelManager.GetHeroNextRecoverRemainingMs(item.IdForAnalytics);
		if (heroChargeLimit <= 0 || heroCharges >= heroChargeLimit)
		{
			Helpers.GameObjectSetActive(worldBossTiredTimeContainer, value: false);
			Helpers.GameObjectSetActive(worldBossNoTiredContainer, value: false);
			Helpers.GameObjectSetActive(worldBossTiredUpContainer, value: true);
			SetWorldBossTiredUpIndicators(3, WorldBossTiredFullColor);
			isWorldBossTiredTimerActive = false;
			return;
		}
		switch (heroCharges)
		{
		case 2:
			Helpers.GameObjectSetActive(worldBossTiredTimeContainer, value: true);
			Helpers.GameObjectSetActive(worldBossNoTiredContainer, value: false);
			Helpers.GameObjectSetActive(worldBossTiredUpContainer, value: true);
			SetWorldBossTiredUpIndicators(2, WorldBossTiredTwoColor);
			StartWorldBossTiredTimer(heroNextRecoverRemainingMs, useFullTimeFormat: false);
			break;
		case 1:
			Helpers.GameObjectSetActive(worldBossTiredTimeContainer, value: true);
			Helpers.GameObjectSetActive(worldBossNoTiredContainer, value: false);
			Helpers.GameObjectSetActive(worldBossTiredUpContainer, value: true);
			SetWorldBossTiredUpIndicators(1, WorldBossTiredOneColor);
			StartWorldBossTiredTimer(heroNextRecoverRemainingMs, useFullTimeFormat: false);
			break;
		case 0:
			Helpers.GameObjectSetActive(worldBossTiredTimeContainer, value: true);
			Helpers.GameObjectSetActive(worldBossNoTiredContainer, value: true);
			Helpers.GameObjectSetActive(worldBossTiredUpContainer, value: false);
			StartWorldBossTiredTimer(heroNextRecoverRemainingMs, useFullTimeFormat: true);
			break;
		default:
			isWorldBossTiredTimerActive = false;
			break;
		}
	}

	private void UpdateWorldBossSelectedTeamTiredDisplay()
	{
		SurvivorModel item = base.Item;
		WorldBossModelManager worldBossModelManager = GameManager.Instance?.playerModel?.WorldBossModelManager;
		if (item == null || worldBossModelManager == null || string.IsNullOrEmpty(item.IdForAnalytics))
		{
			isWorldBossTiredTimerActive = false;
			return;
		}
		Helpers.GameObjectSetActive(worldBossTiredTimeContainer, value: false);
		Helpers.GameObjectSetActive(worldBossNoTiredContainer, value: false);
		isWorldBossTiredTimerActive = false;
		SetWorldBossTiredDownVisible(visible: true);
		Helpers.GameObjectSetActive(worldBossTiredUpContainer, value: true);
		int heroChargeLimit = worldBossModelManager.GetHeroChargeLimit();
		int heroCharges = worldBossModelManager.GetHeroCharges(item.IdForAnalytics);
		if (heroChargeLimit <= 0 || heroCharges >= heroChargeLimit)
		{
			SetWorldBossTiredUpIndicators(3, WorldBossTiredFullColor);
			return;
		}
		switch (heroCharges)
		{
		case 2:
			SetWorldBossTiredUpIndicators(2, WorldBossTiredTwoColor);
			break;
		case 1:
			SetWorldBossTiredUpIndicators(1, WorldBossTiredOneColor);
			break;
		case 0:
			SetWorldBossTiredUpIndicators(0, WorldBossTiredOneColor);
			break;
		}
	}

	private void StartWorldBossTiredTimer(long remainingMs, bool useFullTimeFormat)
	{
		worldBossTiredTimerUseFullTimeFormat = useFullTimeFormat;
		UpdateWorldBossTiredTimerLabel(remainingMs);
		isWorldBossTiredTimerActive = remainingMs > 0;
	}

	private void UpdateWorldBossTiredTimerLabel(long remainingMs)
	{
		if (!(worldBossTiredTimer == null))
		{
			remainingMs = Math.Max(0L, remainingMs);
			string text = Helpers.FormatTimeAsHms(remainingMs);
			worldBossTiredTimer.text = LocalizationManager.GetText("World.Boss.DeployedTeam.RecoverAfterTime", text);
		}
	}

	private void SetWorldBossTiredDownVisible(bool visible)
	{
		if (!(worldBossTiredContainer == null))
		{
			Transform transform = worldBossTiredContainer.transform.Find("Down");
			if (transform != null)
			{
				Helpers.GameObjectSetActive(transform.gameObject, visible);
			}
		}
	}

	private void SetWorldBossTiredUpIndicators(int visibleCount, Color color)
	{
		if (worldBossTiredUpContainer == null)
		{
			return;
		}
		for (int i = 1; i <= 3; i++)
		{
			Transform transform = FindWorldBossTiredIndicatorTransform(i);
			if (transform == null)
			{
				continue;
			}
			bool flag = i <= visibleCount;
			Helpers.GameObjectSetActive(transform.gameObject, flag);
			if (flag)
			{
				UISprite component = transform.GetComponent<UISprite>();
				if (component != null)
				{
					component.color = color;
				}
			}
		}
	}

	private Transform FindWorldBossTiredIndicatorTransform(int index)
	{
		if (worldBossTiredUpContainer == null)
		{
			return null;
		}
		Transform transform = worldBossTiredUpContainer.transform.Find("Tired " + index);
		if (transform == null)
		{
			transform = worldBossTiredUpContainer.transform.Find("Tired" + index);
		}
		return transform;
	}

	private void UpdateWorldBossTiredTimer()
	{
		if (WorldBossSelectedTeamTiredOnlyDisplay || !isWorldBossTiredTimerActive || worldBossTiredTimeContainer == null || !worldBossTiredTimeContainer.activeSelf)
		{
			return;
		}
		SurvivorModel item = base.Item;
		WorldBossModelManager worldBossModelManager = GameManager.Instance?.playerModel?.WorldBossModelManager;
		if (item == null || worldBossModelManager == null || string.IsNullOrEmpty(item.IdForAnalytics))
		{
			isWorldBossTiredTimerActive = false;
			return;
		}
		long heroNextRecoverRemainingMs = worldBossModelManager.GetHeroNextRecoverRemainingMs(item.IdForAnalytics);
		if (heroNextRecoverRemainingMs <= 0)
		{
			isWorldBossTiredTimerActive = false;
			UpdateWorldBossTiredInfo();
		}
		else
		{
			UpdateWorldBossTiredTimerLabel(heroNextRecoverRemainingMs);
		}
	}

	public void UpdateWorldBossSetInfo()
	{
		if (GameManager.Instance.playerModel.WorldBossModelManager.IsHeroDispatched(base.Item.IdForAnalytics))
		{
			Helpers.GameObjectSetActive(worldBossSetContainer, value: true);
			WorldBossSurvivorStatusView survivorStatus = GameManager.Instance.playerModel.WorldBossModelManager.GetSurvivorStatus(base.Item.IdForAnalytics);
			if (survivorStatus != null)
			{
				string dispatchedCapturePoint = survivorStatus.DispatchedCapturePoint;
				WorldBossModelManager worldBossModelManager = GameManager.Instance.playerModel.WorldBossModelManager;
				WorldBossBattlegroundDefinition worldBossBattlegroundDefinition = GameManager.Instance.gameEconomyData.FindWorldBossBattlegroundDefinitionByCapturePoint(dispatchedCapturePoint, worldBossModelManager.GetCurrentBattleDifficulty());
				HelpersUI.SetContentToLabel(worldBossSetNameTxt, (worldBossBattlegroundDefinition != null && !string.IsNullOrEmpty(worldBossBattlegroundDefinition.BuildingName)) ? LocalizationManager.GetText(worldBossBattlegroundDefinition.BuildingName) : dispatchedCapturePoint);
				UpdateWorldBossSetIcon((worldBossBattlegroundDefinition != null) ? worldBossBattlegroundDefinition.CapturePoint : dispatchedCapturePoint);
			}
		}
		else
		{
			Helpers.GameObjectSetActive(worldBossSetContainer, value: false);
		}
	}

	private void UpdateWorldBossSetIcon(string capturePoint)
	{
		if (worldBossSetContainer == null || string.IsNullOrEmpty(capturePoint))
		{
			return;
		}
		Transform transform = worldBossSetContainer.transform.Find("Icon");
		UISprite uISprite = ((transform != null) ? transform.GetComponent<UISprite>() : null);
		if (!(uISprite == null))
		{
			switch (capturePoint)
			{
			case "TOWER-A":
			case "TOWER-B":
				uISprite.spriteName = "Ui_Icon_Outpost";
				break;
			case "DEPOT":
				uISprite.spriteName = "Ui_Icon_Tents";
				break;
			}
		}
	}

	public void UpdateWorldBossGetInfo()
	{
		if (GameManager.Instance.playerModel.WorldBossModelManager.IsHeroReturning(base.Item.IdForAnalytics))
		{
			Helpers.GameObjectSetActive(worldBossGetContainer, value: true);
			WorldBossSurvivorStatusView survivorStatus = GameManager.Instance.playerModel.WorldBossModelManager.GetSurvivorStatus(base.Item.IdForAnalytics);
			if (survivorStatus != null)
			{
				long returnRemainingMs = survivorStatus.ReturnRemainingMs;
				isWorldBossGetTimerActive = returnRemainingMs > 0;
				HelpersUI.SetContentToLabel(worldBossGetTimeTxt, (returnRemainingMs > 0) ? Helpers.FormatTimeAsHms(returnRemainingMs) : "00:00:00");
			}
		}
		else
		{
			isWorldBossGetTimerActive = false;
			Helpers.GameObjectSetActive(worldBossGetContainer, value: false);
		}
	}

	private void UpdateWorldBossGetTimer()
	{
		if (!isWorldBossGetTimerActive || worldBossGetTimeTxt == null || !worldBossGetTimeTxt.gameObject.activeInHierarchy)
		{
			return;
		}
		SurvivorModel item = base.Item;
		WorldBossModelManager worldBossModelManager = GameManager.Instance?.playerModel?.WorldBossModelManager;
		if (item == null || worldBossModelManager == null || string.IsNullOrEmpty(item.IdForAnalytics))
		{
			isWorldBossGetTimerActive = false;
			return;
		}
		long survivorReturnRemainingMs = worldBossModelManager.GetSurvivorReturnRemainingMs(item.IdForAnalytics);
		if (survivorReturnRemainingMs <= 0)
		{
			isWorldBossGetTimerActive = false;
			UpdateWorldBossGetInfo();
		}
		else
		{
			HelpersUI.SetContentToLabel(worldBossGetTimeTxt, LocalizationManager.GetText("World.Boss.PVP.ReturningNeedTime", Helpers.FormatTimeAsHms(survivorReturnRemainingMs)));
		}
	}

	private void UpdateTeamSelectionWorldBossInfo()
	{
		bool flag = Type == CardType.TeamSelect && (TeamSelectionSurvivorType == SurvivorContainerModel.SurvivorType.WorldBossPVE || TeamSelectionSurvivorType == SurvivorContainerModel.SurvivorType.WorldBossPVP || TeamSelectionSurvivorType == SurvivorContainerModel.SurvivorType.WorldBoss) && base.Item != null;
		if (!flag)
		{
			WorldBossSelectedTeamTiredOnlyDisplay = false;
			Helpers.GameObjectSetActive(worldBossTiredContainer, value: false);
			Helpers.GameObjectSetActive(worldBossSetContainer, value: false);
			Helpers.GameObjectSetActive(worldBossGetContainer, value: false);
		}
		else if (WorldBossSelectedTeamTiredOnlyDisplay)
		{
			Helpers.GameObjectSetActive(worldBossTiredContainer, value: true);
			Helpers.GameObjectSetActive(worldBossSetContainer, value: false);
			Helpers.GameObjectSetActive(worldBossGetContainer, value: false);
			isWorldBossGetTimerActive = false;
			UpdateWorldBossSelectedTeamTiredDisplay();
		}
		else
		{
			Helpers.GameObjectSetActive(worldBossTiredContainer, flag);
			Helpers.GameObjectSetActive(worldBossSetContainer, flag);
			Helpers.GameObjectSetActive(worldBossGetContainer, flag);
			UpdateWorldBossTiredInfo();
			UpdateWorldBossSetInfo();
			UpdateWorldBossGetInfo();
		}
	}

	private bool IsWorldBossTeamSelectWithZeroCharges()
	{
		if (Type != CardType.TeamSelect || base.Item == null || string.IsNullOrEmpty(base.Item.IdForAnalytics) || (TeamSelectionSurvivorType != SurvivorContainerModel.SurvivorType.WorldBossPVE && TeamSelectionSurvivorType != SurvivorContainerModel.SurvivorType.WorldBossPVP && TeamSelectionSurvivorType != SurvivorContainerModel.SurvivorType.WorldBoss))
		{
			return false;
		}
		WorldBossModelManager worldBossModelManager = GameManager.Instance?.playerModel?.WorldBossModelManager;
		if (worldBossModelManager == null || worldBossModelManager.GetHeroChargeLimit() <= 0)
		{
			return false;
		}
		return worldBossModelManager.GetHeroCharges(base.Item.IdForAnalytics) <= 0;
	}

	public void ShowTrainingGroundsInfo(bool isAcceptingSurvivor)
	{
		SurvivorModel item = base.Item;
		if (item == null)
		{
			Debug.LogError("Survivor Card: Survivor is NULL!");
			return;
		}
		string txt = null;
		if (IsInCombatTeam())
		{
			txt = LocalizationManager.GetText("Indicator.InTeam.TrainingGround");
		}
		else if (IsInDefendingTeam())
		{
			txt = LocalizationManager.GetText("Indicator.InDefendingTeam.TrainingGround");
		}
		ShowInTeamIndicator(txt);
		ShowEquipmentContainers(show: true);
		EnableEquipmentContainers(enable: false);
		if (weaponPosition != null)
		{
			weaponPosition.SetActive(value: false);
		}
		if (armorPosition != null)
		{
			armorPosition.SetActive(value: false);
		}
		bool flag = item.CanUpgrade && !isAcceptingSurvivor && item.InjuryType == InjuryType.None && !item.HasReachedMaxLevel;
		bool isAnySurvivorUpgrading = IsAnySurvivorUpgrading;
		bool flag2 = !IsLoadDataManager && item.GetUpgradeCashier(instantUpgrade: false).CanAfford();
		if (upgradeCanAffordParent != null && upgradeCanNotAffordParent != null)
		{
			if (flag2)
			{
				upgradeCanAffordParent.SetActive(flag && !isAnySurvivorUpgrading);
			}
			else
			{
				upgradeCanNotAffordParent.SetActive(flag && !isAnySurvivorUpgrading);
			}
			if (upgradeCanNotAffordParent.activeSelf)
			{
				Cashier upgradeCashier = item.GetUpgradeCashier(instantUpgrade: false);
				upgradePriceLabel.text = Helpers.FormatNumber(upgradeCashier.GetTotalCost(CurrencyType.SurvivalPoints));
				upgradePriceLabel.color = (upgradeCashier.CanPay(CurrencyType.SurvivalPoints) ? availableCurrencyColor : unavailableCurrencyColor);
			}
		}
		if (bottomMessageParent != null && bottomMessageLabel != null && (bool)bottomMessageParent.gameObject)
		{
			if (isAcceptingSurvivor)
			{
				bottomMessageLabel.text = LocalizationManager.GetText("SurvivorCard.Button.Choose");
				bottomMessageParent.gameObject.SetActive(value: true);
			}
			else if (item.InjuryType == InjuryType.None && item.HasReachedMaxLevel)
			{
				bottomMessageLabel.text = LocalizationManager.GetText("SurvivorCard.Button.FullyTrained");
				bottomMessageParent.gameObject.SetActive(value: true);
			}
			else if (item.IsUpgrading() && !IsSurvivalMode)
			{
				bottomMessageLabel.text = LocalizationManager.GetText("SurvivorCard.Button.Upgrading");
				bottomMessageParent.gameObject.SetActive(value: true);
			}
			else
			{
				bottomMessageParent.gameObject.SetActive(value: false);
			}
		}
	}

	public void ShowActorHitMessage()
	{
		SurvivorModel item = base.Item;
		if (bottomMessageParent != null && bottomMessageLabel != null && (bool)bottomMessageParent.gameObject && item != null)
		{
			int challengeActorHit = WeeklyChallengeHelper.GetChallengeActorHit(item);
			if (challengeActorHit > 0)
			{
				bottomMessageLabel.text = LocalizationManager.GetText("SurvivorCard.Button.ChallengeActorHit{0}", challengeActorHit);
				bottomMessageParent.gameObject.SetActive(value: true);
			}
		}
	}

	private void InitActorHitMessage()
	{
		SurvivorModel item = base.Item;
		if (bottomMessageParent != null && bottomMessageLabel != null && (bool)bottomMessageParent.gameObject && item != null)
		{
			int challengeActorHit = WeeklyChallengeHelper.GetChallengeActorHit(item);
			bool flag = SingularityMonoBehaviour<HUDManager>.Instance.IsActive(UIType.MapTeamSelection);
			if (challengeActorHit > 0 && flag)
			{
				bottomMessageLabel.text = LocalizationManager.GetText("SurvivorCard.Button.ChallengeActorHit{0}", challengeActorHit);
				bottomMessageParent.gameObject.SetActive(value: true);
			}
		}
	}

	public void UpdateUIForEndScreenStatus(EventDelegate.Callback animationOver, bool isSpecialCharacter, bool isSurvivalEndScreen)
	{
		UpdateUI();
		ShowEquipmentContainers(show: false);
		injuredOverlay.gameObject.SetActive(value: false);
		injuryTimerContainer.gameObject.SetActive(value: false);
		indicatorContainer.SetActive(value: false);
		injuryEndScreenContainer.gameObject.SetActive(value: true);
		injuryEndScreenDeadContainer.SetActive(base.Item.IsDead);
		injuryEndScreenInjuryTypeContainer.gameObject.SetActive(value: false);
		Helpers.GameObjectSetActive(speedupButton, value: false);
		Helpers.GameObjectSetActive(possibleBetterWeapon, value: false);
		Helpers.GameObjectSetActive(possibleBetterArmor, value: false);
		if (medicTentModel != null)
		{
			medicTentModel.Changed -= OnMedicTentChanged;
		}
		if (injuredOverlay != null)
		{
			injuredOverlay.gameObject.SetActive(base.Item.IsDead);
		}
		injuryEndScreenInjuryTypeContainer.gameObject.SetActive(!base.Item.IsDead);
		injuryEndScreenDeadContainer.SetActive(base.Item.IsDead);
		injuryEndScreenTimerProgressBar.foregroundWidget.color = injuryEndScreenProgressBarColors[(int)base.Item.InjuryType];
		if (base.Item.IsDead)
		{
			injuryEndScreenTimerProgressBar.gameObject.SetActive(value: false);
			injuryEndScreenDeadLabel.text = LocalizationManager.GetText("SurvivorStatus.Dead");
			TweenManager.PlayTweenGroup(base.gameObject, 11, forward: true, animationOver);
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("combat_ui/permadeath_status");
		}
		else
		{
			InjuryType injuryType = base.Item.InjuryType;
			injuryEndScreenTimerProgressBar.gameObject.SetActive(value: true);
			injuryEndScreenTimerProgressBar.value = (isSpecialCharacter ? 1f : ((float)base.Item.Hitpoints / (float)base.Item.MaxHitPoints));
			if (isSurvivalEndScreen)
			{
				if (base.Item.PreviousCombatInjuryType == InjuryType.OutOfAction)
				{
					injuryEndScreenTypeLabel.text = LocalizationManager.GetText("SurvivorStatus.Injury.OutOfAction");
					injuryType = InjuryType.OutOfAction;
				}
				else
				{
					injuryEndScreenTypeLabel.text = LocalizationManager.GetText("SurvivorStatus.Injury.Survived");
					injuryType = InjuryType.None;
				}
			}
			else
			{
				injuryEndScreenTypeLabel.text = LocalizationManager.GetText("SurvivorStatus.Injury." + injuryType);
			}
			Helpers.GameObjectSetActive(injuryEndScreenTimerLabel, injuryType != InjuryType.None);
			if (base.Item.InjuryType != InjuryType.None)
			{
				injuryEndScreenTimerLabel.text = LocalizationManager.GetText("SurvivorStatus.RecoveryTime{Time}", Helpers.FormatTime(medicTentModel.TimedQueueModel.GetQueueItemFromItem(base.Item).OriginalActionTime));
			}
			else
			{
				injuryEndScreenTimerLabel.text = "";
			}
			injuryEndScreenTypeLabel.color = injuryEndScreenCardTextColors[(int)injuryType];
			injuryEndScreenTimerLabel.color = injuryEndScreenCardTextColors[(int)injuryType];
			injuryEndScreenInjuryTypeContainer.color = injuryEndScreenCardBgColors[(int)injuryType];
			TweenManager.PlayTweenGroup(base.gameObject, 10, forward: true, animationOver);
		}
		incapacitatedOverlay.gameObject.SetActive(value: false);
		sacrificeContainer.SetActive(value: false);
	}

	public void ForceFinishInjuryTween()
	{
		TweenManager.FinishTweenGroup(base.gameObject, 10);
	}

	private void OnSurvivorModelChanged(ModelObject model, string changed, object args)
	{
		if (base.Item != null && args != null && args == base.Item && (changed == "ActionFinishedEvent" || changed == "ActionUpdatedEvent"))
		{
			UpdateAfterChange();
		}
	}

	private void OnMedicTentChanged(ModelObject model, string changed, object args)
	{
		if (base.Item != null && args != null && args == base.Item && (changed == "ActionFinishedEvent" || changed == "ActionUpdatedEvent"))
		{
			UpdateAfterChange();
		}
	}

	private void UpdateAfterChange()
	{
		UpdateUI();
		if (Type == CardType.TrainingGround)
		{
			ShowTrainingGroundsInfo(isAcceptingSurvivor: false);
		}
		else if (Type == CardType.TrainingGroundAcceptingSurvivor)
		{
			ShowTrainingGroundsInfo(isAcceptingSurvivor: true);
		}
	}

	public void EnableEquipmentContainers(bool enable)
	{
		if (weaponCard != null)
		{
			weaponCard.gameObject.GetComponent<UIButton>().enabled = enable;
		}
		if (armorCard != null)
		{
			armorCard.gameObject.GetComponent<UIButton>().enabled = enable;
		}
	}

	public void ShowEquipmentContainers(bool show)
	{
		if (equipmentContainer != null && equipmentContainer.gameObject != null)
		{
			equipmentContainer.gameObject.SetActive(show);
		}
	}

	protected void LateUpdate()
	{
		SurvivorModel item = base.Item;
		if (item != null)
		{
			if (upgradeBox != null)
			{
				bool flag = item.IsUpgrading();
				upgradeBox.SetActive(flag && !IsSurvivalMode);
				if (upgradeBoxLabel != null && flag && !IsSurvivalMode)
				{
					upgradeBoxLabel.text = Helpers.FormatTimeNoZero(item.TimedActionModel.MillisecondsTillCompletion);
				}
			}
			if (injuryTimerContainer != null && item.InjuryType != InjuryType.None && medicTentModel.TimedQueueModel.IsActive(injuryTimer))
			{
				injuryTimerLabel.text = Helpers.FormatTimeNoZero(injuryTimer.MillisecondsTillCompletion);
			}
		}
		if (repositionTable)
		{
			repositionTable = false;
			favouriteVisibility.GetComponent<UITable>()?.Reposition();
		}
	}

	public void SetPicture(Texture picture)
	{
		if (portrait != null && picture != null)
		{
			portrait.mainTexture = picture;
		}
	}

	public void EnableCardFlipping(bool enable)
	{
		UIButton component = base.gameObject.GetComponent<UIButton>();
		if (component != null)
		{
			component.enabled = enable;
		}
	}

	public void SetupForUnrevealed()
	{
		if (!isUnrevealed)
		{
			isUnrevealed = true;
			List<UIWidget> items = new List<UIWidget> { portrait, levelLabel };
			List<UIWidget> items2 = new List<UIWidget> { unknownSurvivorSprite, newSurvivorLabel };
			AddItemsToAnimationList(items2, itemsToHide);
			AddItemsToAnimationList(items, itemsToReveal);
			SetItemsEnableState(itemsToReveal, active: false);
			SetItemsEnableState(itemsToHide, active: true);
		}
	}

	public bool IsUnrevealed()
	{
		return isUnrevealed;
	}

	public void Update()
	{
		if (revealAnimationTime > -1f)
		{
			revealAnimationTime += Time.deltaTime;
			revealAnimationTime = Mathf.Min(revealAnimationTime, REVEAL_ANIMATION_DURATION);
			float num = revealAnimationTime / REVEAL_ANIMATION_DURATION;
			if (num >= 1f)
			{
				revealAnimationTime = -1f;
				isUnrevealed = false;
				SetItemsEnableState(itemsToReveal, active: true);
				SetItemsEnableState(itemsToHide, active: false);
			}
			foreach (UIWidget item in itemsToHide)
			{
				item.alpha = 1f - num;
			}
			foreach (UIWidget item2 in itemsToReveal)
			{
				item2.alpha = num;
			}
		}
		UpdateWorldBossTiredTimer();
		UpdateWorldBossGetTimer();
		_ = hasOpendInfoPopup;
	}

	private void AddItemsToAnimationList(List<UIWidget> items, List<UIWidget> list)
	{
		foreach (UIWidget item in items)
		{
			if (item != null)
			{
				list.Add(item);
			}
		}
	}

	private void SetItemsEnableState(List<UIWidget> items, bool active)
	{
		foreach (UIWidget item in items)
		{
			item.gameObject.SetActive(active);
		}
	}

	public override int GetSortValue()
	{
		if (base.Item == null)
		{
			return 0;
		}
		string currentPartId = GameManager.Instance.playerModel.Tutorial.CurrentPartId;
		if (!(currentPartId == "HeroTrait"))
		{
			if (currentPartId == "HeroPromote" && base.Item.CanUpgradeSurvivorRarity() && base.Item.IsHero)
			{
				if (base.Item.ActorDefinitionID == "Hero_Daryl")
				{
					return UIListCard<SurvivorModel>.GetSortIntFor(SurvivorSortOrder.FirstSurvivorCard, 1000);
				}
				return UIListCard<SurvivorModel>.GetSortIntFor(SurvivorSortOrder.SurvivorHeroTutorial, 1000);
			}
		}
		else if (base.Item.IsHero)
		{
			return UIListCard<SurvivorModel>.GetSortIntFor(SurvivorSortOrder.SurvivorHeroTutorial, 1000);
		}
		if (base.Item.IsUpgrading())
		{
			return UIListCard<SurvivorModel>.GetSortIntFor(SurvivorSortOrder.SurvivorUpgrading, 1000);
		}
		if (!OfflineManager.IsTutorialDisable)
		{
			if (TutorialView.Instance.Model.CurrentPartId == "Phone" && !IsInCombatTeam() && base.Item.SurvivorClass == SurvivorClass.Bruiser)
			{
				return UIListCard<SurvivorModel>.GetSortIntFor(SurvivorSortOrder.SurvivorBruiserInPhoneTutorial, 1000);
			}
		}
		int num = UIListCard<SurvivorModel>.GetSortIntFor(SurvivorSortOrder.SurvivorGeneric, 1000);
		if (base.Item.IsFeaturedHero)
		{
			num = UIListCard<SurvivorModel>.GetSortIntFor(SurvivorSortOrder.FeaturedStarHero, 1000);
		}
		else if (IsInCombatTeam())
		{
			num = UIListCard<SurvivorModel>.GetSortIntFor(SurvivorSortOrder.SurvivorCombatTeam, 1000);
		}
		else if (IsSurvivalMode)
		{
			SurvivalCharacterStateModel survivorStateInSurvivalMode = GameManager.Instance.playerModel.SurvivorContainer.SurvivalCharacters.GetSurvivorStateInSurvivalMode(base.Item);
			if (survivorStateInSurvivalMode != null && survivorStateInSurvivalMode.OutOfAction)
			{
				num = UIListCard<SurvivorModel>.GetSortIntFor(SurvivorSortOrder.SurvivorSurvivalOutOfAction, 1000);
			}
			else if (base.Item.IsFavourite)
			{
				num = UIListCard<SurvivorModel>.GetSortIntFor(SurvivorSortOrder.SurvivorFavourite, 1000);
			}
		}
		else if (IsInDefendingTeam(isSort: true))
		{
			num = UIListCard<SurvivorModel>.GetSortIntFor(SurvivorSortOrder.SurvivorOutpost, 1000);
		}
		else if (base.Item.IsFavourite)
		{
			num = UIListCard<SurvivorModel>.GetSortIntFor(SurvivorSortOrder.SurvivorFavourite, 1000);
		}
		if (IsWorldBossTeamSelectWithZeroCharges())
		{
			num = UIListCard<SurvivorModel>.GetSortIntFor(SurvivorSortOrder.SurvivorSurvivalOutOfAction, 1000);
		}
		if (Type == CardType.TeamSelect && base.Item.IsHero)
		{
			num += GameManager.Instance.gameEconomyData.ConfigData.MaxRarityLevel;
		}
		if (!IsEndlessModeExpertActor)
		{
			num -= UIListCard<SurvivorModel>.GetSortIntFor(SurvivorSortOrder.SurvivorUnavailable, 1000);
		}
		return num + base.Item.Level * 11 + base.Item.SurvivorRarityLevel;
	}

	public int GetSortValueForCombatType(List<SurvivorModel> currentTeam)
	{
		int num = GetSortValue();
		if (IsWorldBossTeamSelectWithZeroCharges())
		{
			return num;
		}
		int num2 = -UIListCard<SurvivorModel>.GetSortIntFor(SurvivorSortOrder.SurvivorInCurrentTeam, 4000);
		if (currentTeam != null && currentTeam.Contains(base.Item))
		{
			num *= num2;
			if (base.Item.IsFeaturedHero)
			{
				num *= UIListCard<SurvivorModel>.GetSortIntFor(SurvivorSortOrder.FeaturedStarHero, 10000);
			}
			num *= currentTeam.Count - currentTeam.IndexOf(base.Item);
			num = Mathf.Abs(num);
		}
		else if (base.Item.IsFeaturedHero)
		{
			num *= num2 * UIListCard<SurvivorModel>.GetSortIntFor(SurvivorSortOrder.FeaturedStarHero, 10000);
			num = Mathf.Abs(num);
		}
		return num;
	}

	//OnClick
	public void OnCardClicked()
	{
		if (IsLoadDataManager)
		{
			DebugTWD.LogMycode("if (IsLoadDataManager && IsDisableCardClick) return");
			if (survivalPanel != null)
			{
				survivalPanel.gameObject.SetActive(!survivalPanel.gameObject.activeSelf);
				if (survivalPanel.gameObject.activeSelf) survivalPanel.UpdateUI(base.Item);
			}
			if (IsDisableCardClick) return;
		}
		if (Type == CardType.TeamSelect && (TeamSelectionSurvivorType == SurvivorContainerModel.SurvivorType.WorldBossPVE || TeamSelectionSurvivorType == SurvivorContainerModel.SurvivorType.WorldBossPVP || TeamSelectionSurvivorType == SurvivorContainerModel.SurvivorType.WorldBoss) && base.Item != null)
		{
			WorldBossModelManager worldBossModelManager = GameManager.Instance.playerModel.WorldBossModelManager;
			if ((worldBossModelManager != null && (worldBossModelManager.IsHeroDispatched(base.Item.IdForAnalytics) || worldBossModelManager.IsHeroReturning(base.Item.IdForAnalytics))) || !worldBossModelManager.CanHeroBattle(base.Item.IdForAnalytics))
			{
				return;
			}
		}
		if (!Locked && !SurvivorUnavailable && !IsMissionSurvivor && Type != CardType.EnemyPreview && canClick)
		{
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/survivor_card_click");
			UIEvent.Send("OnNewSurvivorSelected", base.Item);
			TutorialPartDefinition currentPartDefinition = GameManager.Instance.playerModel.Tutorial.CurrentPartDefinition;
			if (IsLoadDataManager)
			{
				SurvivorManagementPopUp SurvivorManagementPopUp = DataManager.Instance.SurvivorManagementPopUp;
				if (SurvivorManagementPopUp.SurvivorCardParent.childCount > 0)
				{
					Destroy(SurvivorManagementPopUp.SurvivorCardParent.GetChild(0).gameObject);
				}
				SurvivorManagementPopUp.survivorCardSelected = this.gameObject;
				SurvivorCard cardCurrent = Instantiate(this, parent: SurvivorManagementPopUp.SurvivorCardParent);
				cardCurrent.IsDisableCardClick = true;
				cardCurrent.transform.localPosition = Vector3.zero;
				SurvivorManagementPopUp.SurvivorCardCurrent = cardCurrent;
				DebugTWD.Log("OnCardClicked " + base.Item.SurvivorName, DebugType.OnClick);
			}
			string clickType = "SurvivorCard";
			if (leaderSlotTutorialArrow.activeInHierarchy && currentPartDefinition != null && currentPartDefinition.Id == "HeroTrait")
			{
				clickType = "HeroSlot";
			}
			else if (heroEffect.activeInHierarchy && currentPartDefinition != null && currentPartDefinition.Id == "HeroTrait" && base.Item != null)
			{
				clickType = "HeroSelect_" + base.Item.ActorDefinitionID;
			}
			else if (currentPartDefinition != null && currentPartDefinition.Id == "HeroPromote" && base.Item != null)
			{
				clickType = "HeroSelect_" + base.Item.ActorDefinitionID;
			}
			else if (tutorialArrow != null && tutorialArrow.Id == "SurvivorCardReserve")
			{
				clickType = "SurvivorCardReserve";
			}
			EventManager.NotifyClick(clickType);
		}
	}

	public void RevealCard()
	{
		if (isUnrevealed)
		{
			isUnrevealed = false;
			revealAnimationTime = 0f;
			SetItemsEnableState(itemsToReveal, active: true);
		}
	}

	public void OnCardStateChanged()
	{
		if (!Locked && UIToggle.current.value)
		{
			UIEvent.Send("OnNewSurvivorSelected", base.Item);
			EventManager.NotifyClick(base.Item.SurvivorName);
		}
	}

	public void OnEquipmentClicked(GameObject buttonObject)
	{
		EquipmentButton component = buttonObject.GetComponent<EquipmentButton>();
		UIEvent.Send("OnNewEquipmentCardSelected", component.GetEquipment());
	}

	public void OnSpeedUp()
	{
		SurvivorModel item = base.Item;
		if (item.IsUpgrading())
		{
			ConsumeCurrencyCommandUtils.Execute(new SpeedUpUpgradeSurvivorCommand(item)
			{
				Cashier = item.TimedActionModel.GetSpeedUpCashier()
			}, OnSpeedUpSurvivorCallback);
		}
		else
		{
			ConsumeCurrencyCommandUtils.Execute(new SpeedUpCuringSurvivorCommand(item)
			{
				Cashier = medicTentModel.GetFinishOneCashier(item)
			}, OnSurvivorHealCallBack);
		}
	}

	private void OnSpeedUpSurvivorCallback(TWDModelResult result)
	{
		if (result == TWDModelResult.OK)
		{
			SurvivorInfoPopup obj = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CampSurvivorInfoPopup) as SurvivorInfoPopup;
			obj.currentStateMachineState = SurvivorInfoStateBase.States.SurvivorTrainDone;
			obj.OpenForModel(base.Item);
		}
	}

	private void OnSurvivorHealCallBack(TWDModelResult result)
	{
		if (result == TWDModelResult.OK)
		{
			Helpers.InstantiateToParent(survivorHealEffect, base.gameObject);
		}
	}

	public bool IsInCombatTeam()
	{
		return GameManager.Instance.playerModel.SurvivorContainer.CombatSurvivors.Contains(base.Item);
	}

	public bool IsInDefendingTeam(bool isSort = false)
	{
		if (isSort && GameManager.Instance.gameEconomyData.ConfigData.DisableOutpostHeroLimits)
		{
			return false;
		}
		return GameManager.Instance.playerModel.SurvivorContainer.OutpostDefendingSurvivors.Contains(base.Item);
	}

	public SurvivorCardRerollLocking GetSurvivorCardRerollLocking()
	{
		return survivorCardRerollLocking;
	}

	public void SetLootIndex(int lootEntryIndex)
	{
		surviorCardTokenAccept.LootEntryIndex = lootEntryIndex;
	}

	protected void OnPoolReturn()
	{
		IsSurvivalMode = false;
	}

	public void SetLeaderTraitVisual(bool visible)
	{
		if (leaderSlotTutorialArrow != null)
		{
			leaderSlotTutorialArrow.SetActive(value: true);
		}
		if (visible)
		{
			if (leaderTraitVisual == null)
			{
				GameObject gameObject = Helpers.InstantiateToParent(leaderTraitCardPrefab, base.gameObject);
				leaderTraitVisual = gameObject.GetComponentInChildren<LeaderTraitVisual>();
			}
			if (base.Item != null)
			{
				TraitDefinition traitWithTag = base.Item.GetTraitWithTag("FactionBuffTrait");
				bool traitPresent = base.Item.IsHero || traitWithTag != null;
				leaderTraitVisual.SetTrait(traitPresent, traitWithTag, IsGuildWarMode);
			}
		}
		else if (leaderTraitVisual != null)
		{
			UnityEngine.Object.Destroy(leaderTraitVisual.gameObject);
			leaderTraitVisual = null;
		}
	}

	public void SetCanClick(bool canClick)
	{
		this.canClick = canClick;
	}

	public void SetExtraAttackLabel()
	{
		Helpers.GameObjectSetActive(extraDamageLabel, value: false);
		if (base.Item != null && base.Item != null && !(GameManager.Instance == null) && GameManager.Instance.playerModel != null && GameManager.Instance.playerModel.Combat != null)
		{
			SurvivorModel item = base.Item;
			if (item != null && item.ShadowedGuard_Atk > 0)
			{
				HelpersUI.SetContentToLabel(extraDamageLabel, "+" + base.Item.ShadowedGuard_Atk);
			}
		}
	}



	#region myparams
	private bool IsLoadDataManager => OfflineManager.IsLoadDataManager;
	public bool IsProtector;
	public bool IsDisableCardClick { get; set; }
	public SurvivorSurvivalManualPanel survivalPanel;
	public UILabel SurvivalManualLevel;
	[SerializeField]
	private Color FeaturedStatsColorNext;
	#endregion

	#region mycode
	public int GetHealthValue()
	{
		return !string.IsNullOrEmpty(statsHealthLabel.text) ? int.Parse(statsHealthLabel.text) : 0;
	}
	public int GetStrengthValue()
	{
		return !string.IsNullOrEmpty(statsDamageLabel.text) ? int.Parse(statsDamageLabel.text) : 0;
	}

	public void SetDamageValue(int value)
	{
		statsDamageLabel.text = value.ToString();
	}
	public void SetHealthValue(int value)
	{
		statsHealthLabel.text = value.ToString();
	}

	public void ChangeTrait(int index, UpgradeTraitsData data)
	{
		if (data == null)
		{
			traitsPanels[index].gameObject.SetActive(value: false);
			return;
		}
		traitsPanels[index].gameObject.SetActive(value: true);
		traitsPanels[index].spriteName = HelpersGfx.GetSurvivorTraitIconName(data);
	}

	public void RebuildPortrait()
	{
		UpdatePortrait(base.Item, true);
	}
	#endregion
}
