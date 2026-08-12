using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class StoryTellerView : ActorView
{
	private QuestIndicator newQuestIndicator;

	private QuestIndicator ongoingQuestIndicator;

	private QuestIndicator completeQuestIndicator;

	private GameObject storyTellerPosition;

	public StoryTellerModel StoryTeller => base.Model as StoryTellerModel;

	private GameObject StoryTellerPosition
	{
		get
		{
			if (storyTellerPosition == null)
			{
				GameObject[] array = GameObject.FindGameObjectsWithTag("StoryTellerPosition");
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i].name == base.Model.ActorDefinitionID + "_Position")
					{
						storyTellerPosition = array[i];
					}
				}
				if (storyTellerPosition == null && array.Length != 0)
				{
					storyTellerPosition = array[0];
				}
			}
			return storyTellerPosition;
		}
	}

	private bool CanShowIndicators
	{
		get
		{
			if (TutorialView.Instance == null || !TutorialView.Instance.Running)
			{
				return true;
			}
			return TutorialView.Instance.Allow("StoryTeller");
		}
	}

	private void CreateQuestIndicators()
	{
		if (CampView.Instance.ActorHUD != null)
		{
			if (newQuestIndicator == null)
			{
				newQuestIndicator = CampView.Instance.ActorHUD.CreateNewQuestIndicator(this);
				newQuestIndicator.name = "NewQuestIndicator: " + StoryTeller.ModelId;
			}
			if (ongoingQuestIndicator == null)
			{
				ongoingQuestIndicator = CampView.Instance.ActorHUD.CreateOngoingQuestIndicator(this);
				ongoingQuestIndicator.name = "OnGoingQuestIndicator: " + StoryTeller.ModelId;
			}
			if (completeQuestIndicator == null)
			{
				completeQuestIndicator = CampView.Instance.ActorHUD.CreateCompleteQuestIndicator(this);
				completeQuestIndicator.name = "CompleteQuestIndicator: " + StoryTeller.ModelId;
			}
		}
	}

	private void DestroyQuestIndicators()
	{
		if (newQuestIndicator != null)
		{
			Object.Destroy(newQuestIndicator.gameObject);
			newQuestIndicator = null;
		}
		if (ongoingQuestIndicator != null)
		{
			Object.Destroy(ongoingQuestIndicator.gameObject);
			ongoingQuestIndicator = null;
		}
		if (completeQuestIndicator != null)
		{
			Object.Destroy(completeQuestIndicator.gameObject);
			completeQuestIndicator = null;
		}
	}

	public void UpdateIndicators()
	{
		if (newQuestIndicator != null)
		{
			newQuestIndicator.UpdateVisualisation();
		}
		if (ongoingQuestIndicator != null)
		{
			ongoingQuestIndicator.UpdateVisualisation();
		}
		if (completeQuestIndicator != null)
		{
			completeQuestIndicator.UpdateVisualisation();
		}
	}

	private void Update()
	{
		if (CanShowIndicators)
		{
			CreateQuestIndicators();
		}
		else
		{
			DestroyQuestIndicators();
		}
		if (StoryTellerPosition != null)
		{
			base.transform.rotation = StoryTellerPosition.transform.rotation;
			base.transform.position = StoryTellerPosition.transform.position;
		}
	}

	private void OnDestroy()
	{
		if (newQuestIndicator != null)
		{
			Object.Destroy(newQuestIndicator.gameObject);
		}
		if (ongoingQuestIndicator != null)
		{
			Object.Destroy(ongoingQuestIndicator.gameObject);
		}
		if (completeQuestIndicator != null)
		{
			Object.Destroy(completeQuestIndicator.gameObject);
		}
	}

	public static string GetPortait()
	{
		ActorResourceEntry resources = GameManager.Instance.GetResources<ActorResourceEntry>(GameManager.Instance.playerModel.SurvivorContainer.StoryTeller.Definition.ID);
		if (resources == null)
		{
			Debug.LogError("Could not find resources for actor prefab list " + GameManager.Instance.playerModel.SurvivorContainer.StoryTeller.Definition.ID + "!");
			return "";
		}
		return resources.IconSprite;
	}

	public static void Say(string key, Callback callback = null)
	{
		string portait = GetPortait();
		List<string> list = new List<string>();
		list.Add("Dialog," + portait + "," + key);
		int num = 2;
		bool flag;
		do
		{
			flag = SingularityMonoBehaviour<LocalizationManager>.Instance.LocalizationExists(key + num);
			if (flag)
			{
				list.Add("Dialog," + portait + "," + key + num);
				num++;
			}
		}
		while (flag);
		list.Add("Dialog,hide");
		TutorialView.Instance.StartCutScene(list, callback);
	}
}
