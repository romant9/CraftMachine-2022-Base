using TWDModel;

public class UIStoryProgressBar : UIProgressBarExtended
{
	private StoryTellerModel storyTellerCached;

	private MapMissionGroupModel currentStoryGroupModel;

	private MapMissionModel currentStoryMissionModel;

	public override void OnEnable()
	{
		base.OnEnable();
		storyTellerCached = GameManager.Instance.playerModel.SurvivorContainer.StoryTeller;
		if (storyTellerCached != null && storyTellerCached.CurrentQuest is MissionQuest)
		{
			currentStoryGroupModel = ((MissionQuest)storyTellerCached.CurrentQuest).GetUnlockedEpisode();
		}
		if (currentStoryGroupModel != null)
		{
			currentStoryMissionModel = currentStoryGroupModel.GetFirstUnlockedMissionModel();
		}
		UpdateUI();
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
		int num = 0;
		float value = 0f;
		if (currentStoryGroupModel != null)
		{
			int numberCompletedStoryMissions = currentStoryGroupModel.GetNumberCompletedStoryMissions();
			num = currentStoryGroupModel.GetNumberStoryMissions();
			value = (float)numberCompletedStoryMissions / (float)num;
		}
		if (progressBar != null)
		{
			progressBar.value = value;
		}
		Helpers.GameObjectSetActive(progressBar, currentStoryMissionModel != null);
	}

	public override void Clear()
	{
		base.Clear();
		storyTellerCached = null;
		currentStoryGroupModel = null;
		currentStoryMissionModel = null;
	}
}
