using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class UIRewardsList : MonoBehaviourExtended
{
	[SerializeField]
	private RewardIcon prefabIcon;

	[SerializeField]
	private GameObject iconsParent;

	private List<RewardIcon> iconsList = new List<RewardIcon>();

	private void Awake()
	{
		DebugIdString = "MissionHubRewardsPreview";
		Helpers.GameObjectSetActive(base.gameObject, value: false);
	}

	public void PreviewRewardsForMission(MapMissionModel mapMissionModel)
	{
		List<IReward> previewRewards = new List<IReward>();
		if (mapMissionModel != null)
		{
			if (prefabIcon != null)
			{
				Rewards storyMissionRewards = mapMissionModel.GetStoryMissionRewards();
				if (storyMissionRewards != null)
				{
					previewRewards = storyMissionRewards.RewardsList;
				}
				SetPreviewRewards(previewRewards);
			}
		}
		else
		{
			DebugLogError("mapMissionModel was NULL!");
		}
	}

	public virtual void SetPreviewRewards(List<IReward> rewardsList)
	{
		if (rewardsList == null || iconsList == null)
		{
			return;
		}
		for (int i = 0; i < rewardsList.Count; i++)
		{
			if (rewardsList[i] != null)
			{
				RewardIcon icon = GetIcon(i);
				if (icon != null)
				{
					icon.SetReward(rewardsList[i]);
				}
			}
		}
		PositionIcons(rewardsList.Count);
	}

	public virtual void SetPreviewRewards(List<DropEventDefinition.DropEventTag> rewardsList)
	{
		if (rewardsList == null || iconsList == null)
		{
			return;
		}
		for (int i = 0; i < rewardsList.Count; i++)
		{
			RewardIcon icon = GetIcon(i);
			if (icon != null)
			{
				icon.SetReward(rewardsList[i]);
			}
		}
		PositionIcons(rewardsList.Count);
	}

	private RewardIcon GetIcon(int index)
	{
		if (iconsList.Count <= index || iconsList[index] == null)
		{
			return Helpers.InstantiateToList(prefabIcon.gameObject, iconsParent, iconsList);
		}
		return iconsList[index];
	}

	public override void Clear()
	{
		base.Clear();
		for (int i = 0; i < iconsList.Count; i++)
		{
			iconsList[i].Clear();
			Object.Destroy(iconsList[i].gameObject);
		}
		iconsList = new List<RewardIcon>();
	}

	private void PositionIcons(int rewardsCount)
	{
		for (int i = 0; i < iconsList.Count; i++)
		{
			if (iconsList[i] != null)
			{
				if (i >= rewardsCount)
				{
					Helpers.GameObjectSetActive(iconsList[i], value: false);
					continue;
				}
				iconsList[i].transform.localPosition = HelpersUI.GetRowPositionX(i, rewardsCount, iconsList[i].GetLocalSize());
				Helpers.GameObjectSetActive(iconsList[i], value: true);
			}
		}
		Helpers.GameObjectSetActive(base.gameObject, rewardsCount > 0);
	}
}
