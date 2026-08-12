using System.Collections;
using BaseModel;
using TWDModel;

public class QuestIndicator : HUDElementFollowTarget
{
	public QuestIndicatorType QuestIndicatorType;

	public StoryTellerView StoryTellerView;

	private StoryTellerModel storyTellerModel;

	private StoryTellerModel StoryTeller => StoryTellerView.Model as StoryTellerModel;

	private IEnumerator Start()
	{
		GameManager.Instance.playerModel.Changed += OnPlayerChange;
		storyTellerModel = StoryTellerView.Model as StoryTellerModel;
		storyTellerModel.Changed += OnStoryTellerChange;
		yield return null;
		UpdateVisualisation();
	}

	private void OnDestroy()
	{
		GameManager.Instance.playerModel.Changed -= OnPlayerChange;
		if (storyTellerModel != null)
		{
			storyTellerModel.Changed -= OnStoryTellerChange;
		}
	}

	public void UpdateVisualisation()
	{
		if (QuestIndicatorType == QuestIndicatorType.NewQuest)
		{
			base.transform.GetChild(0).gameObject.SetActive(StoryTeller.CanAcceptQuest);
		}
		else if (QuestIndicatorType == QuestIndicatorType.OngoingQuest)
		{
			base.transform.GetChild(0).gameObject.SetActive(value: false);
			base.transform.GetChild(1).gameObject.SetActive(value: false);
			if (GameManager.Instance.ShowTipsDone)
			{
				base.transform.GetChild(1).gameObject.SetActive(StoryTeller.CurrentQuest != null && !StoryTeller.CurrentQuest.HasCompleted);
			}
			else
			{
				base.transform.GetChild(0).gameObject.SetActive(StoryTeller.CurrentQuest != null && !StoryTeller.CurrentQuest.HasCompleted);
			}
		}
		else if (QuestIndicatorType == QuestIndicatorType.CompletedQuest)
		{
			base.transform.GetChild(0).gameObject.SetActive(StoryTeller.CurrentQuest != null && StoryTeller.CurrentQuest.HasCompleted);
		}
		FollowTarget(StoryTellerView.IndicatorParent);
	}

	private void OnPlayerChange(ModelObject m, string changed, object args)
	{
	}

	private void OnStoryTellerChange(ModelObject m, string changed, object args)
	{
		UpdateVisualisation();
	}

	public void OnClick()
	{
		GameManager.Instance.ShowTipsDone = true;
		if (TutorialView.Instance.Running)
		{
			TutorialView.Instance.HideArrow();
		}
		CampView.Instance.CampViewBuildings.UnselectBuilding();
		StoryTellerFlow.StartFlow(storyTellerModel);
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("camp/storyteller_click");
		UpdateVisualisation();
	}
}
