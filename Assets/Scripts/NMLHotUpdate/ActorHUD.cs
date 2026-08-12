using UnityEngine;

public class ActorHUD : MonoBehaviour
{
	[SerializeField]
	[Tooltip("The GameObject that will holds all the hud GameObject.")]
	private GameObject actorHUDParent;

	[SerializeField]
	[Tooltip("New quest indicators. Icon shown on top of the building")]
	private GameObject newQuestIndicator;

	[SerializeField]
	[Tooltip("Ongoing quest indicator. Icon shown on top of the building")]
	private GameObject ongoingQuestIndicator;

	[SerializeField]
	[Tooltip("Complete quest indicator. Icon shown on top of the building")]
	private GameObject completeQuestIndicator;

	[SerializeField]
	[Tooltip("Actor production indicator. Icon shown on top of the actor")]
	private GameObject actorProductionIndicator;

	[SerializeField]
	[Tooltip("Camp defense kill walker indicator. Icon shown on top of the walker")]
	private GameObject actorKillWalkerIndicator;

	public QuestIndicator CreateNewQuestIndicator(StoryTellerView storyTellerView)
	{
		return CreateQuestIndicator(newQuestIndicator, storyTellerView);
	}

	public QuestIndicator CreateOngoingQuestIndicator(StoryTellerView storyTellerView)
	{
		return CreateQuestIndicator(ongoingQuestIndicator, storyTellerView);
	}

	public QuestIndicator CreateCompleteQuestIndicator(StoryTellerView storyTellerView)
	{
		return CreateQuestIndicator(completeQuestIndicator, storyTellerView);
	}

	protected QuestIndicator CreateQuestIndicator(GameObject prefab, StoryTellerView storyTeller)
	{
		QuestIndicator component = Helpers.InstantiateToParent(prefab, actorHUDParent).GetComponent<QuestIndicator>();
		component.StoryTellerView = storyTeller;
		return component;
	}

	public ActorProductionIndicator CreateActorProductionIndicator(ActorView actorView)
	{
		ActorProductionIndicator actorProductionIndicator = null;
		GameObject gameObject = Helpers.InstantiateToParent(this.actorProductionIndicator, actorHUDParent);
		if (gameObject != null)
		{
			actorProductionIndicator = gameObject.GetComponent<ActorProductionIndicator>();
			actorProductionIndicator.ParentView = actorView;
		}
		return actorProductionIndicator;
	}

	public CampDefenseKillWalkerIndicator CreateKillWalkerIndicator(ActorView actorView)
	{
		CampDefenseKillWalkerIndicator campDefenseKillWalkerIndicator = null;
		GameObject gameObject = Helpers.InstantiateToParent(actorKillWalkerIndicator, actorHUDParent);
		if (gameObject != null)
		{
			campDefenseKillWalkerIndicator = gameObject.GetComponent<CampDefenseKillWalkerIndicator>();
			campDefenseKillWalkerIndicator.FollowTarget(actorView.gameObject);
		}
		return campDefenseKillWalkerIndicator;
	}
}
