using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class TooltipEndlessModeReward : TooltipTextbox
{
	[SerializeField]
	private GameObject rewardPrefab;

	[SerializeField]
	private GameObject rewardsContainer;

	[SerializeField]
	private UISprite backgroundSprite;

	[SerializeField]
	private int backGroundSpritePadding;

	[SerializeField]
	private UITable rewardTable;

	private List<GameObject> rewardsItems = new List<GameObject>();

	public override void Show()
	{
		base.Show();
		rewardTable.Reposition();
		backgroundSprite.width = CalculateBackGroundSpriteWidth();
	}

	public void UpdateWithParams(string title, int rewardRank, SurvivorClass survivorClass = SurvivorClass.None)
	{
		SetText(title);
		ClearRewardIcons();
		Rewards rewards = null;
		rewards = ((survivorClass != SurvivorClass.None) ? new Rewards(EndlessModeHelpers.GetLeaderBoardRewardBySurvivorClass(EndlessModeHelpers.GetEndlessModeLeaderBoardRewardByRank(rewardRank), survivorClass)) : new Rewards(EndlessModeHelpers.GetEndlessModeLeaderBoardRewardByRank(rewardRank).Rewards));
		if (rewards.RewardsList == null || rewards.Count <= 0)
		{
			return;
		}
		foreach (IReward rewards2 in rewards.RewardsList)
		{
			GameObject gameObject = rewardsContainer.AddChild(rewardPrefab);
			NGUITools.SetActive(gameObject, state: true);
			if (gameObject.TryGetComponent<RewardIcon>(out var component))
			{
				component.SetReward(rewards2);
			}
			rewardsItems.Add(gameObject);
		}
		rewardTable.Reposition();
	}

	private void ClearRewardIcons()
	{
		for (int i = 0; i < rewardsItems.Count; i++)
		{
			NGUITools.Destroy(rewardsItems[i]);
		}
		rewardsItems.Clear();
	}

	private int CalculateBackGroundSpriteWidth()
	{
		return (int)NGUIMath.CalculateRelativeWidgetBounds(rewardsContainer.transform).size.x + backGroundSpritePadding;
	}
}
