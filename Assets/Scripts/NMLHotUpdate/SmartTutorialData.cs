using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class SmartTutorialData : ScriptableObject
{
	[Tooltip("Smart tutorial configurations")]
	public List<SmartTutorialConfiguration> SmartTutorials = new List<SmartTutorialConfiguration>();

	public bool HasShown(SmartTutorialType tutorialType)
	{
		string toggleKey = "Toggle.SmartTutorialShown." + tutorialType;
		return GameManager.Instance.playerModel.Blackboard.IsToggleOn(toggleKey);
	}

	public bool CanShow(SmartTutorialConfiguration config)
	{
		_ = GameManager.DevFastTrackLoad;
		MapMissionModel attackTargetMissionModel = GameManager.Instance.playerModel.MapContainerModel.AttackTargetMissionModel;
		if (attackTargetMissionModel != null)
		{
			MissionData missionData = attackTargetMissionModel.MissionData;
			if (missionData != null)
			{
				if (config.IncludeMissionTags != null)
				{
					for (int i = 0; i < config.IncludeMissionTags.Count; i++)
					{
						int item = config.IncludeMissionTags[i];
						if (missionData.MissionTags == null || !missionData.MissionTags.Contains(item))
						{
							return false;
						}
					}
				}
				if (config.ExcludeMissionTags != null)
				{
					for (int j = 0; j < config.ExcludeMissionTags.Count; j++)
					{
						int item2 = config.ExcludeMissionTags[j];
						if (missionData.MissionTags != null && missionData.MissionTags.Contains(item2))
						{
							return false;
						}
					}
				}
			}
		}
		return true;
	}

	public void StartSmartTutorial(SmartTutorialType tutorialType, ActorModel instigator = null)
	{
		for (int i = 0; i < SmartTutorials.Count; i++)
		{
			SmartTutorialConfiguration config = SmartTutorials[i];
			if (config.Type == tutorialType && !HasShown(tutorialType) && CanShow(config))
			{
				Helpers.ExecuteCommand(new SetBlackboardToggleCommand("Toggle.SmartTutorialShown." + tutorialType));
				GameObject gameObject = Object.Instantiate(config.PrefabResource.GetPrefab());
				gameObject.GetComponent<NodeGraphWrapper>().BindToModels(null);
				SmartTutorialNode componentInChildren = gameObject.GetComponentInChildren<SmartTutorialNode>();
				if (componentInChildren != null)
				{
					componentInChildren.Activate(instigator);
				}
			}
		}
	}
}
