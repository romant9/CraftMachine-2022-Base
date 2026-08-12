using TWDModel;
using UnityEngine;

public class MissionHubPanelStory : MissionHubGameModePanel
{
	[Header("Story Dialog")]
	[SerializeField]
	private GameObject storyDialogParent;

	[SerializeField]
	private UISprite storyDialogPortraitSprite;

	[SerializeField]
	private UILabel storyDialogLabel;

	[SerializeField]
	private UILabel storyDialogBottomLabel;

	private StoryTellerModel storyTellerCached;

	private MapMissionGroupModel currentStoryGroupModel;

	private MapMissionModel currentStoryMissionModel;

	public override void Init(MissionHubContent content, HUDElement parent)
	{
		base.Init(content, parent);
		storyTellerCached = GameManager.Instance.playerModel.SurvivorContainer.StoryTeller;
		if (storyTellerCached == null)
		{
			return;
		}
		QuestDefinition questDefinition = storyTellerCached.CurrentQuestDefinition;
		if (questDefinition == null)
		{
			questDefinition = storyTellerCached.GetCurrentUncompletedQuestDefinition();
		}
		if (questDefinition != null)
		{
			currentStoryGroupModel = questDefinition.GetUnlockedEpisode(GameManager.Instance.modelManager);
			if (currentStoryGroupModel != null)
			{
				currentStoryMissionModel = currentStoryGroupModel.GetFirstUnlockedMissionModel();
			}
		}
	}

	protected override void OpenDialog()
	{
		EventManager.NotifyClick("Story");
		MissionHubNavigation.ContinueStoryMap();
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
		string text = "";
		IReward reward = null;
		if (currentStoryGroupModel != null)
		{
			int numberCompletedStoryMissions = currentStoryGroupModel.GetNumberCompletedStoryMissions();
			int numberStoryMissions = currentStoryGroupModel.GetNumberStoryMissions();
			string character = "";
			if (storyTellerCached != null && storyTellerCached.CurrentQuestDefinition != null)
			{
				if (storyTellerCached.CanAcceptQuest)
				{
					TutorialUi.TryParsePortraitInLocalization(LocalizationManager.GetText(storyTellerCached.CurrentQuestDefinition.IntroKey), out character);
					text = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Popup.MissionHub.Story.CanAcceptQuest.SubTitle");
				}
				else if (storyTellerCached.CurrentQuest != null && storyTellerCached.CurrentQuest.HasCompleted)
				{
					TutorialUi.TryParsePortraitInLocalization(LocalizationManager.GetText(storyTellerCached.CurrentQuestDefinition.CompletionKey), out character);
					text = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Popup.MissionHub.Story.HasCompleted.SubTitle{CompletedAmount}{TotalAmount}", numberCompletedStoryMissions, numberStoryMissions);
				}
			}
			if (text == "")
			{
				text = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Popup.MissionHub.Story.SubTitle{CompletedAmount}{TotalAmount}", numberCompletedStoryMissions, numberStoryMissions);
			}
			if (!string.IsNullOrEmpty(character))
			{
				HelpersUI.SetSprite(storyDialogPortraitSprite, character);
				Helpers.GameObjectSetActive(storyDialogParent, value: true);
				Helpers.GameObjectSetActive(storyDialogBottomLabel, value: true);
				SetLocationTexture(null);
			}
			else
			{
				Helpers.GameObjectSetActive(storyDialogParent, value: false);
				PreviewSingleReward(null);
				SetLocationTexture(currentStoryGroupModel);
				if (currentStoryMissionModel != null)
				{
					Rewards storyMissionRewards = currentStoryMissionModel.GetStoryMissionRewards();
					if (storyMissionRewards != null && storyMissionRewards.RewardsList != null && storyMissionRewards.RewardsList.Count > 0)
					{
						reward = storyMissionRewards.RewardsList[0];
					}
				}
			}
		}
		PreviewSingleReward(reward);
		HelpersUI.SetContentToLabel(titleSubLabel, text, text != "");
		if (progressBar != null)
		{
			progressBar.UpdateUI();
		}
	}

	public override void SetTitleLocalisation()
	{
		if (missionHubContent != null && !string.IsNullOrEmpty(missionHubContent.TitleLocalizationKey))
		{
			HelpersUI.SetContentToLabel(titleLabel, LocalizationManager.GetText(missionHubContent.TitleLocalizationKey));
		}
		else if (storyTellerCached != null && storyTellerCached.CurrentQuest != null && storyTellerCached.CurrentQuest.HasCompleted)
		{
			HelpersUI.SetContentToLabel(titleLabel, HelpersLocalization.GetEpisodeTitle(currentStoryGroupModel));
		}
		else if (currentStoryGroupModel != null && currentStoryGroupModel.MissionSpawnPointGroup != null)
		{
			HelpersUI.SetContentToLabel(titleLabel, HelpersLocalization.GetEpisodeTitle(currentStoryGroupModel));
		}
	}

	private static bool AreAllStoryMissionsCompleted()
	{
		MapContainerModel mapContainerModel = GameManager.Instance.playerModel.MapContainerModel;
		int num = 0;
		foreach (MissionSpawnPointGroup mapDefinition in GameManager.Instance.gameEconomyData.MapDefinitions)
		{
			if (mapDefinition == null || mapDefinition.Category != MapCategory.Story)
			{
				continue;
			}
			num++;
			if (num > 15)
			{
				return true;
			}
			foreach (MissionSpawnPoint missionSpawnPoint in mapDefinition.MissionSpawnPoints)
			{
				if (!mapContainerModel.IsMissionCompleted(missionSpawnPoint))
				{
					return false;
				}
			}
		}
		return true;
	}
}
