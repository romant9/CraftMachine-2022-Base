using TWDModel;
using UnityEngine;

public class MissionHubPanelSeasonSeven : MissionHubGameModePanel
{
	[Header("Season Reward")]
	[SerializeField]
	private UISeasonRewardIcon seasonRewardIcon;

	[Header("Season Special Timer")]
	[SerializeField]
	private NUICountdownTimer seasonTimer;

	[Tooltip("This will be the season character by default if not overriden in the GED: CharacterMaterialOverride")]
	[Header("Season Special Character")]
	[SerializeField]
	private UITexture characterTexture;

	[Tooltip("Can be overriden in the GED: BackgroundMaterialOverride")]
	[Header("Season Special background")]
	[SerializeField]
	private UITexture backgroundTexture;

	[Tooltip("Will be shown until player has completed tutorial part: EndTutorial")]
	[Header("Tutorial Locked")]
	[SerializeField]
	private GameObject tutorialLocked;

	[SerializeField]
	private GameObject newEpisodeContainer;

	private const string materialsFolder = "UI/Materials/";

	private long seasonUnlockTimestamp = -1L;

	private bool seasonsUnlocked;

	private SeasonDefinition currentSeason;

	public override void Awake()
	{
		base.Awake();
		DebugIdString = "MissionHubPanelSeasonSeven";
	}

	public override void Start()
	{
		base.Start();
		Helpers.GameObjectSetActive(seasonTimer, value: false);
		Helpers.GameObjectSetActive(buttonMain, value: false);
		Helpers.GameObjectSetActive(progressBar, value: false);
		Helpers.GameObjectSetActive(characterTexture, value: false);
		Helpers.GameObjectSetActive(tutorialLocked, value: false);
		if (GameManager.Instance.playerModel.SurvivorContainer.StoryTeller != null)
		{
			StoryTellerModel storyTeller = GameManager.Instance.playerModel.SurvivorContainer.StoryTeller;
			QuestDefinition currentUncompletedQuestDefinition = storyTeller.GetCurrentUncompletedQuestDefinition();
			seasonsUnlocked = currentUncompletedQuestDefinition != null && storyTeller.GetCurrentUncompletedQuestDefinition().Order > 0;
		}
	}

	public override void Update()
	{
		base.Update();
		if (base.isLocked && seasonTimer != null)
		{
			seasonTimer.SetCurrentMilliseconds(gameModeTimeLeft);
		}
		Helpers.GameObjectSetActive(tutorialLocked, base.isLocked && !seasonsUnlocked);
		Helpers.GameObjectSetActive(seasonTimer, base.isLocked && seasonsUnlocked);
		Helpers.GameObjectSetActive(buttonMain, !base.isLocked);
		Helpers.GameObjectSetActive(progressBar, !base.isLocked);
		Helpers.GameObjectSetActive(characterTexture, !base.isLocked);
	}

	public override void UpdateUI()
	{
		currentSeason = GameManager.Instance.gameEconomyData.GetHighlightedSeasonDefinition();
		if (progressBar != null)
		{
			UISeasonProggressBar component = progressBar.GetComponent<UISeasonProggressBar>();
			if (component != null)
			{
				component.SetSeason(currentSeason);
			}
		}
		base.UpdateUI();
		if (GameManager.Instance.playerModel.SurvivorContainer.StoryTeller != null)
		{
			StoryTellerModel storyTeller = GameManager.Instance.playerModel.SurvivorContainer.StoryTeller;
			QuestDefinition currentUncompletedQuestDefinition = storyTeller.GetCurrentUncompletedQuestDefinition();
			seasonsUnlocked = currentUncompletedQuestDefinition != null && storyTeller.GetCurrentUncompletedQuestDefinition().Order > 0;
		}
		if (seasonRewardIcon != null)
		{
			if (seasonsUnlocked)
			{
				seasonRewardIcon.UpdateUI(currentSeason);
			}
			else
			{
				Helpers.GameObjectSetActive(seasonRewardIcon, value: false);
			}
		}
		if (seasonUnlockTimestamp == -1)
		{
			seasonUnlockTimestamp = GameManager.Instance.gameEconomyData.GetFirstSeasonMissionUnlockTime(currentSeason);
		}
		if (seasonUnlockTimestamp != -1)
		{
			if (GameManager.Instance.playerModel.UtcTimeStamp < seasonUnlockTimestamp)
			{
				if (!seasonsUnlocked)
				{
					HelpersUI.SetContentToLabel(lockedLabel, LocalizationManager.GetText("Popup.MissionHub.SeasonUnlockAfterTutorial"));
				}
				gameModeTimeLeft = seasonUnlockTimestamp - GameManager.Instance.playerModel.UtcTimeStamp;
				gameModeTimeLeft = ((gameModeTimeLeft < 0) ? 0 : gameModeTimeLeft);
				UpdateLockedState(locked: true);
			}
			else
			{
				if (!seasonsUnlocked)
				{
					HelpersUI.SetContentToLabel(lockedLabel, LocalizationManager.GetText("Popup.MissionHub.SeasonUnlockAfterTutorial"));
				}
				UpdateLockedState(!seasonsUnlocked);
			}
		}
		else
		{
			UpdateLockedState(locked: true);
			DebugLogError("Could not find suitable season with unlock time");
		}
		if (missionHubContent != null)
		{
			if (characterTexture != null)
			{
				if (!string.IsNullOrEmpty(missionHubContent.CharacterMaterialOverride))
				{
					Material material = UnityUtils.LoadAsset<Material>("UI/Materials/" + missionHubContent.CharacterMaterialOverride);
					if (material != null)
					{
						characterTexture.material = material;
					}
				}
				else
				{
					MapMissionGroupModel seasonCurrentMapMissionGroup = DetailMapPopUp.GetSeasonCurrentMapMissionGroup(currentSeason);
					if (seasonCurrentMapMissionGroup != null && seasonCurrentMapMissionGroup.MissionSpawnPointGroup != null)
					{
						HelpersGfx.SetSeasonHeroMaterial(characterTexture, seasonCurrentMapMissionGroup.MissionSpawnPointGroup.MapId);
					}
				}
			}
			if (backgroundTexture != null && !string.IsNullOrEmpty(missionHubContent.BackgroundMaterialOverride))
			{
				Material material = UnityUtils.LoadAsset<Material>("UI/Materials/" + missionHubContent.BackgroundMaterialOverride);
				if (material != null)
				{
					backgroundTexture.material = material;
				}
			}
			if (OfflineManager.IsLoadDataManager)
			{
				backgroundTexture.mainTexture = UnityUtils.LoadAsset<Texture2D>("UI/Textures/" + "Map_BG_GVG");
			}
		}
		if (GameManager.Instance.playerModel.MapContainerModel.HasUnseenContent(currentSeason) && seasonsUnlocked)
		{
			Helpers.GameObjectSetActive(newEpisodeContainer, value: true);
		}
		else
		{
			Helpers.GameObjectSetActive(newEpisodeContainer, value: false);
		}
	}

	public override void SetTitleLocalisation()
	{
		if (missionHubContent != null && currentSeason != null)
		{
			HelpersUI.SetContentToLabel(titleLabel, LocalizationManager.GetText(missionHubContent.TitleLocalizationKey, currentSeason.Id));
		}
	}

	protected override void ButtonMainClicked(UIButtonExtended button)
	{
		if (seasonsUnlocked)
		{
			base.ButtonMainClicked(button);
		}
	}

	protected override void OpenDialog()
	{
		MissionHubNavigation.OpenSeasonMap(currentSeason);
	}
}
