using TwdCustomMod;
using TWDModel;
using UnityEngine;

public class SurvivorCardHeroLocked : UIListCard<SurvivorModel>
{
	[SerializeField]
	private UILabel nameLabel;

	[SerializeField]
	private UILabel amountLabel;

	[SerializeField]
	private UISprite currencyIconSprite;

	[SerializeField]
	private UIProgressBar progressBar;

	[SerializeField]
	private UIButtonWithLabel unlockButton;

	[SerializeField]
	private UIButtonWithLabel infoButton;

	[SerializeField]
	private UITexture portraitTexture;

	[SerializeField]
	private GameObject unknownPortrait;

	[SerializeField]
	private UILabel unlockTimer;

	[SerializeField]
	private UISprite classIcon;

	[SerializeField]
	private UISprite rarityBorder;

	[SerializeField]
	private GameObject starsPanelContainer;

	[SerializeField]
	private UISprite[] starsPanels;

	[SerializeField]
	private UISprite talkingDeadIcon;

	[SerializeField]
	private GameObject altHeroContainer;

	[SerializeField]
	private GameObject heroContainer;

	[SerializeField]
	private GameObject featuredHeroBadge;

	private ActorDefinition actorDefinition;

	private int ownedTokensAmount;

	private Color portraitColor;

	private TutorialArrowParent tutorialArrowUnlockButton;

	public ActorDefinition ActorDefinition => actorDefinition;

	public void OnEnable()
	{
		UpdateUI();
		if (unlockButton != null)
		{
			unlockButton.SetClickCallback(OnClickedUnlock);
		}
		if (tutorialArrowUnlockButton == null && unlockButton != null)
		{
			tutorialArrowUnlockButton = unlockButton.GetComponentInChildren<TutorialArrowParent>();
		}
		if (infoButton != null)
		{
			infoButton.SetClickCallback(OnClickedUnlock);
		}
		UIEvent.OnUIEvent += OnUIEvent;
	}

	public void OnDisable()
	{
		if (unlockButton != null)
		{
			unlockButton.Clear();
		}
		if (infoButton != null)
		{
			infoButton.Clear();
		}
		UIEvent.OnUIEvent -= OnUIEvent;
	}

	public void SetActorDefinition(ActorDefinition definition)
	{
		actorDefinition = definition;
		if (actorDefinition != null)
		{
			ownedTokensAmount = GameManager.Instance.playerModel.GetCurrency(actorDefinition.TraitUpgradeCurrency).Value;
		}
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
		if (actorDefinition == null)
		{
			return;
		}
		long utcTimeStamp = GameManager.Instance.playerModel.UtcTimeStamp;
		bool flag = actorDefinition.IsAvailableToUnlock(utcTimeStamp);
		long num = (actorDefinition.UnlockTimeMilliseconds - utcTimeStamp) / 1000;
		bool flag2 = num > 0 && num < actorDefinition.MinTimeToShowCountdown;
		HelpersUI.SetSprite(currencyIconSprite, HelpersGfx.GetCurrencyIconName(actorDefinition.TraitUpgradeCurrency));
		HelpersUI.SetContentToLabel(nameLabel, actorDefinition.Name);
		Texture portrait = PortraitManager.Instance.GetPortrait(PortraitRenderSource.fromActorDefinition(actorDefinition));
		if (flag || flag2)
		{
			if (portrait == null)
			{
				ModularCharacter modularCharacter = ActorView.GetPrefabForActor(actorDefinition.ID, actorDefinition.VisualAsset);
				if (modularCharacter == null)
				{
					modularCharacter = ActorView.SelectRandomPrefabForActorDefinition(actorDefinition.ID, actorDefinition.Gender);
				}
				PortraitManager.Instance.CreatePortrait(PortraitRenderSource.fromActorDefinition(actorDefinition), modularCharacter, OnMissingPortraitRendered);
			}
			else
			{
				Helpers.GameObjectSetActive(unknownPortrait, value: false);
				portraitTexture.mainTexture = portrait;
				Helpers.GameObjectSetActive(portraitTexture, value: true);
			}
			if (classIcon != null)
			{
				Helpers.GameObjectSetActive(classIcon, value: true);
				classIcon.spriteName = HelpersGfx.GetSurvivorClassIconName(actorDefinition.Class, actorDefinition.RarityLevel);
			}
			if (rarityBorder != null)
			{
				HelpersGfx.UpdateSpriteAndKeepScale(rarityBorder, HelpersGfx.GetRarityBorderSpriteName(actorDefinition.RarityLevel));
			}
			Helpers.GameObjectSetActive(talkingDeadIcon, actorDefinition.ID.ToLower().Contains("talkingdead"));
			Helpers.GameObjectSetActive(starsPanelContainer, value: true);
			UpdateRarityStars(starsPanels, actorDefinition);
		}
		else
		{
			Helpers.GameObjectSetActive(unknownPortrait, value: true);
			Helpers.GameObjectSetActive(portraitTexture, value: false);
			Helpers.GameObjectSetActive(classIcon, value: false);
			Helpers.GameObjectSetActive(starsPanelContainer, value: false);
			Helpers.GameObjectSetActive(talkingDeadIcon, value: false);
			HelpersGfx.UpdateSpriteAndKeepScale(rarityBorder, HelpersGfx.GetRarityBorderSpriteName(0));
		}
		HelpersUI.SetContentToLabel(amountLabel, ownedTokensAmount + "/" + actorDefinition.TokensToUnlock);
		bool flag3 = UnlockPossible();
		Helpers.GameObjectSetActive(unlockButton, flag3);
		Helpers.GameObjectSetActive(infoButton, !flag3);
		Helpers.GameObjectSetActive(amountLabel, flag);
		Helpers.GameObjectSetActive(currencyIconSprite, flag);
		if (progressBar != null)
		{
			float val = Mathf.Clamp((float)ownedTokensAmount / (float)actorDefinition.TokensToUnlock, 0f, 1f);
			progressBar.Set(val);
			Helpers.GameObjectSetActive(progressBar, flag);
		}
		if (tutorialArrowUnlockButton != null)
		{
			tutorialArrowUnlockButton.Id = "UnlockHero_" + actorDefinition.ID;
			Helpers.GameObjectSetActive(tutorialArrowUnlockButton, flag);
		}
		if (unlockTimer != null)
		{
			Helpers.GameObjectSetActive(unlockTimer, !flag && flag2);
			if (!flag && flag2)
			{
				unlockTimer.text = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("HeroLocked.AvailableTimer", Helpers.FormatTimeNoZero(num * 1000));
			}
		}
		if (nameLabel != null && !flag && !flag2)
		{
			nameLabel.text = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("HeroLocked.Title.Hidden");
		}
		Helpers.GameObjectSetActive(altHeroContainer, actorDefinition.IsAltHero);
		Helpers.GameObjectSetActive(heroContainer, !actorDefinition.IsAltHero);
		bool value = false;
		if (GameManager.Instance.playerModel.Tutorial.HasCompletedPart("Phone"))
		{
			FeaturedHeroDefinition activeFeaturedHero = GameManager.Instance.gameEconomyData.GetActiveFeaturedHero(utcTimeStamp);
			if (activeFeaturedHero != null)
			{
				value = activeFeaturedHero.ActorDefinitionID == ActorDefinition.ID;
			}
		}
		Helpers.GameObjectSetActive(featuredHeroBadge, value);
	}

	private void UpdateRarityStars(UISprite[] starsArray, ActorDefinition actorDefinition)
	{
		if (starsArray == null || actorDefinition == null)
		{
			return;
		}
		for (int i = 0; i < starsArray.Length; i++)
		{
			if (starsArray[i] != null && (bool)starsArray[i].gameObject)
			{
				if (actorDefinition.RarityLevel >= i)
				{
					starsArray[i].gameObject.SetActive(value: true);
				}
				else
				{
					starsArray[i].gameObject.SetActive(value: false);
				}
			}
		}
	}

	private void OnUIEvent(string type, object parameter)
	{
	}

	private void OnMissingPortraitRendered(IPortraitRenderSource info)
	{
		if (portraitTexture != null && info != null && actorDefinition.ID == info.ActorDefinitionId)
		{
			Helpers.GameObjectSetActive(unknownPortrait, value: false);
			portraitTexture.mainTexture = PortraitManager.Instance.GetPortrait(info);
			portraitTexture.gameObject.SetActive(value: true);
		}
	}

	public override int GetSortValue()
	{
		if (UnlockPossible())
		{
			if (GameManager.Instance.playerModel.Tutorial.CurrentPartId == "HeroUnlock" && actorDefinition != null && actorDefinition.ID == "Hero_Daryl")
			{
				return UIListCard<SurvivorModel>.GetSortIntFor(SurvivorSortOrder.FirstSurvivorCard, 1000);
			}
			return UIListCard<SurvivorModel>.GetSortIntFor(SurvivorSortOrder.SurvivorHeroUnlock, 1000);
		}
		if (actorDefinition != null && !actorDefinition.IsAvailableToUnlock(GameManager.Instance.playerModel.UtcTimeStamp))
		{
			return UIListCard<SurvivorModel>.GetSortIntFor(SurvivorSortOrder.SurvivorOutpost, 1000);
		}
		return UIListCard<SurvivorModel>.GetSortIntFor(SurvivorSortOrder.SurvivorGeneric, 1000);
	}

	public void OnCardClicked()
	{
		LockedHeroClicked();
	}

	private void OnClickedUnlock(UIButtonExtended button)
	{
		if (HeroUnlockHelper.UnlockHero(actorDefinition))
		{
			EventManager.NotifyClick("UnlockHero_" + actorDefinition.ID);
		}
		else
		{
			LockedHeroClicked();
		}
	}

	private void LockedHeroClicked()
	{
		if (OfflineManager.IsLoadDataManager && IsDisableCardClick)
		{
			DebugTWD.LogMycode("if (OfflineManager.IsLoadDataManager && IsDisableCardClick) return");
			return;
		}
		if (actorDefinition == null || !actorDefinition.IsAvailableToUnlock(GameManager.Instance.playerModel.UtcTimeStamp) || TutorialView.Instance == null || TutorialView.Instance.Running)
		{
			return;
		}
		if (GameManager.Instance.gameEconomyData.GetFeature("LockedHeroPreview").Enabled)
		{
			base.Item = HeroUnlockHelper.GetOrCreateMockSurvivorModel(actorDefinition);
		}
		if (base.Item != null)
		{
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/survivor_card_click");
			UIEvent.Send("SurvivorHeroPreviewSelected", base.Item);
			if (OfflineManager.IsLoadDataManager)
			{
				DebugTWD.LogMycode("if (OfflineManager.IsLoadDataManager)");
				SurvivorManagementPopUp SurvivorManagementPopUp = DataManager.Instance.SurvivorManagementPopUp;
				if (SurvivorManagementPopUp.SurvivorCardParent.childCount > 0)
				{
					Destroy(SurvivorManagementPopUp.SurvivorCardParent.GetChild(0).gameObject);
				}
				SurvivorManagementPopUp.survivorCardSelected = this.gameObject;
				SurvivorCardHeroLocked cardCurrentLocked = Instantiate(this, parent: SurvivorManagementPopUp.SurvivorCardParent);
				cardCurrentLocked.IsDisableCardClick = true;
				cardCurrentLocked.transform.localPosition = Vector3.zero;
				SurvivorManagementPopUp.SurvivorCardCurrent = null;
				DebugTWD.Log("OnCardClicked " + base.Item.SurvivorName, DebugType.OnClick);
			}
			return;
		}
		MissingTokensPopup missingTokensPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.MissingTokensPopup) as MissingTokensPopup;
		if (missingTokensPopup != null && TutorialView.Instance != null && !TutorialView.Instance.Running)
		{
			missingTokensPopup.IsHeroLocked = true;
			missingTokensPopup.IsHeroContent = true;
			missingTokensPopup.Open();
		}
	}

	private bool IsHeroFound()
	{
		return ownedTokensAmount > 0;
	}

	private bool UnlockPossible()
	{
		if (actorDefinition != null)
		{
			if (ownedTokensAmount >= actorDefinition.TokensToUnlock)
			{
				return actorDefinition.IsAvailableToUnlock(GameManager.Instance.playerModel.UtcTimeStamp);
			}
			return false;
		}
		return false;
	}



	#region myparams
	public bool IsDisableCardClick { get; set; }
	#endregion


	#region mycode
	public void RebuildPortrait()
	{
		//     if (OfflineManager.IsLoadDataManager)
		//     {
		//			PortraitRenderSource info = PortraitRenderSource.fromActorDefinition(actorDefinition);
		//         string OutfitDefinitionId = "";
		//         ModularCharacter modularCharacter;
		//         if (info.OutfitDefinitionId == null)
		//         {
		//             modularCharacter = ActorView.SelectRandomPrefabForActor(survivor);
		//         }
		//         else
		//         {
		//             modularCharacter = ActorView.GetPrefabOverrideForActorDefinition(OutfitDefinitionId, info.Gender);
		//         }
		//         if (modularCharacter != null)
		//         {
		//             PortraitManager.Instance.CreatePortrait(info, modularCharacter, OnMissingPortraitRendered);
		//         }
		//     }
		ModularCharacter modularCharacter = ActorView.GetPrefabForActor(actorDefinition.ID, actorDefinition.VisualAsset);
		if (modularCharacter == null)
		{
			modularCharacter = ActorView.SelectRandomPrefabForActorDefinition(actorDefinition.ID, actorDefinition.Gender);
		}
		PortraitRenderSource portraitRenderSource = PortraitRenderSource.fromActorDefinition(actorDefinition);
		portraitRenderSource.IsRebuild = true;
		PortraitManager.Instance.CreatePortrait(portraitRenderSource, modularCharacter, OnMissingPortraitRendered);
	}
	#endregion
}
