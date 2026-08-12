using System.Collections;
using BaseModel;
using TWDModel;
using UnityEngine;

public class QuestPopup : HUDElement
{
	[SerializeField]
	private GameObject ongoingQuestContainer;

	[SerializeField]
	private UILabel episodeLabel;

	[SerializeField]
	private UISprite questGiver;

	[SerializeField]
	private UILabel questNameLabel;

	[SerializeField]
	private UILabel questDescriptionLabel;

	[SerializeField]
	private UISprite rewardSprite;

	[SerializeField]
	private UILabel rewardLabel;

	[SerializeField]
	private UIProgressBar missionProgressBar;

	[SerializeField]
	private ButtonSimple rewardInfoButton;

	[SerializeField]
	private UISeasonProggressBar seasonProgressBar;

	[SerializeField]
	private GameObject currencyRewardContainer;

	[SerializeField]
	private UILabel currencyRewardAmount;

	[SerializeField]
	private UISprite currencyRewardIcon;

	[Header("Missions completed")]
	[SerializeField]
	private GameObject missionsCompletedContainer;

	[SerializeField]
	private UILabel missionsCompletedLabel;

	[SerializeField]
	private ButtonSimple okButton;

	[Header("Mission car alert")]
	[SerializeField]
	private GameObject missionCarUpgradeContainer;

	[SerializeField]
	private UILabel missionCarNeededLabel;

	[Header("Get Reward")]
	[SerializeField]
	private GameObject getRewardContainer;

	[SerializeField]
	private UISprite rewardSpriteComplete;

	[SerializeField]
	private UILabel getRewardLabel;

	[SerializeField]
	private UILabel episodeCompleteLabel;

	[SerializeField]
	private UILabel episodeUnlocksLabel;

	[SerializeField]
	private UILabel getRewardDescriptionLabel;

	[SerializeField]
	private GameObject getRewardButtonContainer;

	[SerializeField]
	private UILabel getReward2ndTitle;

	[SerializeField]
	private GameObject unlocksContainer;

	[SerializeField]
	private GameObject collectAnimationPrefab;

	[SerializeField]
	private GameObject collectAnimationStartPosition;

	[SerializeField]
	private float closeWaitTime;

	[SerializeField]
	private GameObject completedCurrencyRewardContainer;

	[SerializeField]
	private UILabel completedCurrencyRewardAmount;

	[SerializeField]
	private UISprite completedCurrencyRewardIcon;

	[Header("Share screen")]
	[SerializeField]
	private UIButton shareButton;

	[SerializeField]
	private GameObject sharePanel;

	[SerializeField]
	private UITexture shareBadge;

	[SerializeField]
	private UISprite normalImage;

	[SerializeField]
	private UILabel seasonUnlockText;

	[SerializeField]
	private GameObject seasonRewardContainer;

	[SerializeField]
	private UITexture seasonRewardHeroTexture;

	private StoryTellerModel storyTellerModel;

	private IReward reward;

	private SeasonDefinition rewardedSeasonDefinition;

	private bool seasonRewardMode => rewardedSeasonDefinition != null;

	public override void OpenForModel(ModelObject model)
	{
		rewardedSeasonDefinition = null;
		base.OpenForModel(model);
		storyTellerModel = GetModel<StoryTellerModel>();
		UpdateUI();
		EventManager.NotifyClick("StoryTeller");
	}

	public void OpenForSeasonReward(SeasonDefinition season)
	{
		Open();
		rewardedSeasonDefinition = season;
		Material seasonHeroMaterial = HelpersGfx.GetSeasonHeroMaterial(season.Id);
		if (seasonHeroMaterial != null)
		{
			_ = seasonHeroMaterial.name;
		}
		if (seasonRewardContainer != null)
		{
			seasonRewardContainer.SetActive(value: true);
		}
		if (seasonRewardHeroTexture != null)
		{
			seasonRewardHeroTexture.material = HelpersGfx.GetSeasonHeroMaterial(season.Id);
		}
		if (normalImage != null)
		{
			normalImage.enabled = false;
		}
		if (seasonUnlockText != null)
		{
			seasonUnlockText.text = LocalizationManager.GetText("Popup.Quest.SeasonUnlockText");
		}
		getRewardButtonContainer.SetActive(value: false);
		ongoingQuestContainer.SetActive(value: false);
		unlocksContainer.SetActive(value: false);
		currencyRewardContainer.SetActive(value: false);
		completedCurrencyRewardContainer.SetActive(value: false);
		string heroId = SurvivorToken.GetHeroId(season.RewardCurrency);
		ActorDefinition actorDefinition = GameManager.Instance.gameEconomyData.GetActorDefinition(heroId);
		if (seasonProgressBar != null && seasonProgressBar.gameObject != null)
		{
			seasonProgressBar.SetSeason(season);
			seasonProgressBar.UpdateUI();
		}
		currencyRewardAmount.text = LocalizationManager.GetText("Quest.Reward.Currency{amount}", Helpers.FormatNumber(actorDefinition.TokensToUnlock));
		currencyRewardIcon.spriteName = HelpersGfx.GetCurrencyIconName(season.RewardCurrency);
		HelpersUI.SetContentToLabel(getRewardLabel, LocalizationManager.GetText("Popup.Quest.SeasonRewardName", HelpersLocalization.GetCurrencyContext(season.RewardCurrency)));
		episodeCompleteLabel.text = LocalizationManager.GetText("Popup.Quest.SeasonCompleted");
		getRewardDescriptionLabel.text = "";
		if (getReward2ndTitle != null)
		{
			getReward2ndTitle.text = LocalizationManager.GetText("Popup.Quest.SeasonRewardDescription");
		}
	}

	public override void UpdateUI()
	{
		if (seasonRewardMode)
		{
			return;
		}
		if (seasonRewardContainer != null)
		{
			seasonRewardContainer.SetActive(value: false);
		}
		if (normalImage != null)
		{
			normalImage.enabled = true;
		}
		if (getReward2ndTitle != null)
		{
			getReward2ndTitle.text = LocalizationManager.GetText("Popup.Quest.Reward");
		}
		QuestDefinition currentQuestDefinition = storyTellerModel.CurrentQuestDefinition;
		MapMissionGroupModel unlockedEpisode = currentQuestDefinition.GetUnlockedEpisode(GameManager.Instance.modelManager);
		ongoingQuestContainer.SetActive(!storyTellerModel.CanCompleteQuest);
		getRewardContainer.SetActive(storyTellerModel.CanCompleteQuest);
		getRewardButtonContainer.SetActive(storyTellerModel.CanCompleteQuest);
		questNameLabel.text = LocalizationManager.GetText(currentQuestDefinition.TitleKey);
		string localizedString = null;
		questGiver.spriteName = HelpersLocalization.GetSpriteAndText(currentQuestDefinition.BriefingKey, out localizedString);
		if (!string.IsNullOrEmpty(localizedString))
		{
			questDescriptionLabel.text = localizedString;
		}
		episodeLabel.text = HelpersLocalization.GetEpisodeTitle(unlockedEpisode);
		episodeCompleteLabel.text = HelpersLocalization.GetEpisodeTitleComplete(unlockedEpisode);
		episodeUnlocksLabel.text = HelpersLocalization.GetEpisodeUnlocks(unlockedEpisode);
		reward = currentQuestDefinition.GetRewards().GetRewardAt(0);
		rewardInfoButton.gameObject.SetActive(value: false);
		currencyRewardContainer.SetActive(value: false);
		completedCurrencyRewardContainer.SetActive(value: false);
		if (seasonProgressBar != null && seasonProgressBar.gameObject != null)
		{
			Helpers.GameObjectSetActive(seasonProgressBar.gameObject, value: false);
		}
		if (reward is RewardSurvivorClass)
		{
			RewardSurvivorClass rewardSurvivorClass = reward as RewardSurvivorClass;
			rewardLabel.text = LocalizationManager.GetText("Popup.Quest.UnlockClass{ClassName}", HelpersLocalization.GetSurvivorClassName(rewardSurvivorClass.SurvivorClass));
			getRewardLabel.text = rewardLabel.text;
			getRewardDescriptionLabel.text = HelpersLocalization.GetSurvivorClassDescription(rewardSurvivorClass.SurvivorClass);
			ShowClassInfoButton();
		}
		else if (reward is RewardCurrency)
		{
			currencyRewardContainer.SetActive(value: true);
			completedCurrencyRewardContainer.SetActive(value: true);
			RewardCurrency rewardCurrency = reward as RewardCurrency;
			currencyRewardAmount.text = LocalizationManager.GetText("Quest.Reward.Currency{amount}", Helpers.FormatNumber(rewardCurrency.Amount));
			currencyRewardIcon.spriteName = HelpersGfx.GetCurrencyIconName(rewardCurrency.CurrencyType);
			completedCurrencyRewardAmount.text = currencyRewardAmount.text;
			completedCurrencyRewardIcon.spriteName = currencyRewardIcon.spriteName;
			if (rewardCurrency.CurrencyType == CurrencyType.DarylToken && currentQuestDefinition.Identifier == "BackToTerminus")
			{
				rewardLabel.text = LocalizationManager.GetText("Popup.Quest.UnlockDaryl");
				getRewardLabel.text = rewardLabel.text;
			}
			else
			{
				rewardLabel.text = HelpersLocalization.GetCurrencyName(rewardCurrency.CurrencyType);
				getRewardLabel.text = rewardLabel.text;
			}
			getRewardDescriptionLabel.text = HelpersLocalization.GetCurrencyDescription(rewardCurrency.CurrencyType);
		}
		else if (reward is RewardEquipment)
		{
			RewardEquipment rewardEquipment = reward as RewardEquipment;
			rewardLabel.text = HelpersLocalization.GetEquipmentName(rewardEquipment.EquipmentId);
			getRewardLabel.text = rewardLabel.text;
			getRewardDescriptionLabel.text = "";
			ShowEquipmentInfoButton();
		}
		rewardSprite.spriteName = "Ui_Episode_Art" + (currentQuestDefinition.Order + 1);
		rewardSpriteComplete.spriteName = "Ui_Episode_Art" + (currentQuestDefinition.Order + 1);
		bool active = false;
		bool active2 = false;
		bool active3 = true;
		bool active4 = currentQuestDefinition.Order != 0;
		if (storyTellerModel.CanAcceptQuest)
		{
			if (unlockedEpisode.HasRequiredCarLevel)
			{
				ShowAcceptQuestDialog();
			}
		}
		else if (storyTellerModel.CanCompleteQuest)
		{
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("camp/quest_complete");
		}
		else
		{
			active2 = true;
			int numberCompletedStoryMissions = unlockedEpisode.GetNumberCompletedStoryMissions();
			int numberStoryMissions = unlockedEpisode.GetNumberStoryMissions();
			missionsCompletedLabel.text = LocalizationManager.GetText("Popup.Quest.MissionsCompleted{Number}{Total}", numberCompletedStoryMissions, numberStoryMissions);
			missionProgressBar.value = (float)numberCompletedStoryMissions / (float)numberStoryMissions;
			ShowOngoingQuestDialog();
		}
		okButton.gameObject.SetActive(active3);
		missionsCompletedContainer.SetActive(active2);
		missionCarUpgradeContainer.SetActive(active);
		unlocksContainer.SetActive(active4);
	}

	public void ShowClassInfoButton()
	{
		rewardInfoButton.gameObject.SetActive(value: true);
		rewardInfoButton.SetLabel(LocalizationManager.GetText("Popup.Quest.ClassInfo"));
		rewardInfoButton.SetCallback(OnClassInfoClick);
	}

	public void ShowEquipmentInfoButton()
	{
		rewardInfoButton.gameObject.SetActive(value: true);
		rewardInfoButton.SetLabel(LocalizationManager.GetText("Popup.Quest.EquipmentInfo"));
		rewardInfoButton.SetCallback(OnEquipmentInfoClick);
	}

	public void ShowAcceptQuestDialog()
	{
		okButton.SetLabel(LocalizationManager.GetText("Popup.Quest.AcceptQuest"));
		okButton.SetCallback(OnAcceptQuestClick);
	}

	public void ShowOngoingQuestDialog()
	{
		okButton.SetLabel(LocalizationManager.GetText("Popup.Quest.ContinueQuest"));
		okButton.SetCallback(OnContinueQuestClick);
	}

	public void OnShareClick()
	{
		StartCoroutine(GetComponent<ScreenshotShare>().TakeScreenshot("MissionReward", shareButton, shareBadge, ShowUiForScreenshot));
	}

	private void ShowUiForScreenshot(bool show)
	{
		sharePanel.SetActive(show);
		getRewardButtonContainer.SetActive(!show);
		if ((bool)shareButton)
		{
			shareButton.gameObject.SetActive(!show);
		}
		if (!show && seasonRewardMode)
		{
			OpenForSeasonReward(rewardedSeasonDefinition);
		}
	}

	public void OnUpgradeCar()
	{
		SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CampUpgradeBuildingPopup).OpenForModel(CampView.Instance.Model.GetBuilding("MissionCar"));
	}

	public void OnAcceptSeasonReward()
	{
		SingularityMonoBehaviour<HUDManager>.Instance.CloseAllOpenPopupsAndDialogs();
		(SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CampTrainingGrounds) as SurvivorManagementPopUp).Open();
	}

	private void OnClassInfoClick()
	{
		if (reward is RewardSurvivorClass rewardSurvivorClass)
		{
			UnlockClassPopup obj = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.UnlockClassPopup) as UnlockClassPopup;
			obj.StoryTellerModel = null;
			obj.OpenSingleInfo(rewardSurvivorClass.SurvivorClass);
		}
	}

	private void OnEquipmentInfoClick()
	{
		if (reward is RewardEquipment rewardEquipment)
		{
			EquipmentUpgradePopup obj = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CampEquipmentLevelUpPopup) as EquipmentUpgradePopup;
			obj.ShowNextLevel = false;
			obj.OpenForBundleReward(rewardEquipment);
			obj.HideShareButton();
		}
	}

	private void OnAcceptQuestClick()
	{
		EventManager.NotifyClick("PositiveButton");
		Close();
		if (storyTellerModel.CanAcceptQuest)
		{
			Helpers.ExecuteCommand(new AcceptQuestCommand(storyTellerModel));
		}
		ShowNewQuestUnlock();
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("camp/quest_accept");
	}

	private void OnContinueQuestClick()
	{
		EventManager.NotifyClick("PositiveButton");
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_click");
		Close();
		if (storyTellerModel.CurrentQuest is MissionQuest)
		{
			CampManager.Instance.GoToMap(((MissionQuest)storyTellerModel.CurrentQuest).GetUnlockedEpisode());
		}
	}

	private void ShowNewQuestUnlock()
	{
		if (storyTellerModel.CurrentQuest is MissionQuest)
		{
			if (storyTellerModel.CurrentQuest.DefinitionID == "BackToTerminus" && GameManager.Instance.playerModel.Tutorial.CurrentStep == 10)
			{
				OnClickClose();
			}
			else
			{
				CampManager.Instance.GoToMap(((MissionQuest)storyTellerModel.CurrentQuest).GetUnlockedEpisode());
			}
		}
	}

	public override void OnClickClose()
	{
		if (storyTellerModel != null && storyTellerModel.CanCompleteQuest && !seasonRewardMode)
		{
			OnCompleteQuestClick();
		}
		base.OnClickClose();
	}

	public void OnCompleteQuestClick()
	{
		EventManager.NotifyClick("PositiveButton");
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_click");
		QuestDefinition currentQuestDefinition = storyTellerModel.CurrentQuestDefinition;
		if (storyTellerModel.CanCompleteQuest)
		{
			Helpers.ExecuteCommand(new CompleteQuestCommand(storyTellerModel));
		}
		bool flag = false;
		if (currentQuestDefinition != null)
		{
			if (currentQuestDefinition.GetRewards().GetRewardAt(0) is RewardSurvivorClass rewardSurvivorClass)
			{
				flag = true;
				UnlockClassPopup obj = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.UnlockClassPopup) as UnlockClassPopup;
				obj.StoryTellerModel = storyTellerModel;
				obj.ForceOpenSurvivorClass = rewardSurvivorClass.SurvivorClass;
				obj.Open();
			}
			if (currentQuestDefinition.GetRewards().GetRewardAt(0) is RewardCurrency rewardCurrency)
			{
				CampHUD campHUD = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CampCampMapHud) as CampHUD;
				if (!(campHUD != null))
				{
					return;
				}
				if (rewardCurrency.CurrencyType == CurrencyType.DarylToken && !GameManager.Instance.playerModel.Tutorial.HasCompletedPart("HeroUnlock"))
				{
					string heroId = SurvivorToken.GetHeroId(CurrencyType.DarylToken);
					SurvivorModel heroById = GameManager.Instance.playerModel.SurvivorContainer.GetHeroById(heroId);
					Cashier heroUnlockCashier = GameManager.Instance.playerModel.SurvivorContainer.GetHeroUnlockCashier(CurrencyType.DarylToken);
					if (GameManager.Instance.playerModel.GetCurrency(CurrencyType.DarylToken).Value >= heroUnlockCashier.GetTotalCost(CurrencyType.DarylToken) && heroById == null)
					{
						TutorialView.Instance.StartPart("HeroUnlock");
					}
				}
				getRewardButtonContainer.SetActive(value: false);
				CreateCollectAnimation(rewardCurrency, collectAnimationStartPosition, campHUD.GetTeamManagementButton());
				return;
			}
		}
		if (!flag)
		{
			Close();
			StoryTellerFlow.StartFlow(storyTellerModel);
		}
	}

	private void CreateCollectAnimation(RewardCurrency reward, GameObject from, GameObject to)
	{
		int amount = reward.Amount;
		int b = (PlatformInfo.HasFlag(PlatformFlag.SlowGPU) ? 10 : 20);
		int num = Mathf.Min(amount, b);
		if (!(from != null) || !(to != null))
		{
			return;
		}
		if (SingularityMonoBehaviour<AudioManager>.Instance != null)
		{
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/collect_token");
		}
		for (int i = 0; i < num; i++)
		{
			CollectAnimation component = Helpers.InstantiateToParentAndLayer(collectAnimationPrefab, base.gameObject).GetComponent<CollectAnimation>();
			if (component != null)
			{
				component.FollowTarget(from);
			}
			bool isFirst = i == 0;
			component.StartAnimation(amount, reward.CurrencyType, to.transform, CurrencyAnimComplete, isFirst);
		}
	}

	private void CurrencyAnimComplete(bool isComplete, CurrencyType currencyType)
	{
		StartCoroutine(DelayedClose(closeWaitTime));
	}

	private IEnumerator DelayedClose(float delay)
	{
		yield return new WaitForSeconds(delay);
		Close();
	}
}
