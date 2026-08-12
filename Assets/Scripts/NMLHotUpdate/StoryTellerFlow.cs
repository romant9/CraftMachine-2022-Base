using TWDModel;

public class StoryTellerFlow
{
	private static StoryTellerModel storyTellerModel;

	public static void StartFlow(StoryTellerModel model)
	{
		if (model != null)
		{
			storyTellerModel = model;
			if (storyTellerModel.CanAcceptQuest)
			{
				StoryTellerView.Say(storyTellerModel.CurrentQuestDefinition.IntroKey, OpenQuestPopup);
			}
			else if (storyTellerModel.CurrentQuest != null && storyTellerModel.CurrentQuest.HasCompleted)
			{
				StoryTellerView.Say(storyTellerModel.CurrentQuestDefinition.CompletionKey, OpenQuestPopup);
			}
			else if (storyTellerModel.CurrentQuestDefinition != null)
			{
				OpenQuestPopup();
			}
		}
	}

	private static void OpenQuestPopup()
	{
		SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.QuestPopup).OpenForModel(storyTellerModel);
	}
}
