using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class EndlessModeLeaderBoardRewardEntry : GuildPlayerListCard
{
	[SerializeField]
	private UILabel bracketLabel;

	[SerializeField]
	private GameObject rewardPrefab;

	[SerializeField]
	private GameObject lowerRankContainer;

	[SerializeField]
	private GameObject ownRankingContainer;

	[SerializeField]
	private GameObject rewardsScrollView;

	[SerializeField]
	private GameObject topContainer;

	[SerializeField]
	private GameObject rewardsContainer;

	[SerializeField]
	private UILabel defaultLabel;

	[SerializeField]
	private UISprite actorIcon;

	[SerializeField]
	public UIButton button;

	[SerializeField]
	public GameObject target;

	private List<GameObject> rewardsItems = new List<GameObject>();

	public void SetContent(EndlessModeLeaderBoardReward endlessModeLeaderBoardReward, bool isOwnRank, bool isLowerRank, bool isLastEntry, SurvivorClass survivorClass)
	{
		Helpers.GameObjectSetActive(ownRankingContainer, isOwnRank);
		Helpers.GameObjectSetActive(lowerRankContainer, isLowerRank);
		Helpers.GameObjectSetActive(topContainer, value: false);
		Helpers.GameObjectSetActive(rewardsContainer, value: true);
		string content = (isLastEntry ? EndlessModeHelpers.GetLastPlaceLocalisedLeaderBoardRewardBracketTitle() : EndlessModeHelpers.GetLocalisedLeaderBoardRewardBracketTitle(endlessModeLeaderBoardReward.RewardBracket, endlessModeLeaderBoardReward.RewardType));
		HelpersUI.SetContentToLabel(bracketLabel, content);
		ClearRewardIcons();
		SetupLeaderBoardRewards(EndlessModeHelpers.GetLeaderBoardRewardBySurvivorClass(endlessModeLeaderBoardReward, survivorClass));
	}

	public override void UpdateUI()
	{
		Helpers.GameObjectSetActive(topContainer, value: true);
		Helpers.GameObjectSetActive(rewardsContainer, value: false);
		Helpers.GameObjectSetActive(scoreLabel, value: true);
		Helpers.GameObjectSetActive(playerEmblemIcon, value: true);
		Helpers.GameObjectSetActive(playerEmblemIcon, value: true);
		Helpers.GameObjectSetActive(scoreContainer, value: true);
		Helpers.GameObjectSetActive(nameLabel, value: true);
		Helpers.GameObjectSetActive(defaultLabel, value: false);
		base.UpdateUI();
		if (base.Item is EndlessModePlayersScoreDataEntry { LeaderActorDefinitionId: not null } endlessModePlayersScoreDataEntry)
		{
			Helpers.GameObjectSetActive(actorIcon, value: true);
			CurrencyType survivorTraitUpgradeCurrencyType = HelpersGfx.GetSurvivorTraitUpgradeCurrencyType(GameManager.Instance.gameEconomyData.GetActorDefinition(endlessModePlayersScoreDataEntry.LeaderActorDefinitionId));
			actorIcon.spriteName = HelpersGfx.GetCurrencyIconName(survivorTraitUpgradeCurrencyType);
		}
		else
		{
			Helpers.GameObjectSetActive(actorIcon, value: false);
		}
	}

	public void SetTop3DefaultUI()
	{
		Helpers.GameObjectSetActive(scoreLabel, value: false);
		Helpers.GameObjectSetActive(playerEmblemIcon, value: false);
		Helpers.GameObjectSetActive(playerEmblemIcon, value: false);
		Helpers.GameObjectSetActive(scoreContainer, value: false);
		Helpers.GameObjectSetActive(nameLabel, value: false);
		Helpers.GameObjectSetActive(defaultLabel, value: true);
		Helpers.GameObjectSetActive(actorIcon, value: false);
	}

	public void SetDefaultUI()
	{
		HelpersUI.SetContentToLabel(bracketLabel, "-");
		HelpersUI.SetContentToLabel(scoreLabel, "0");
		HelpersUI.SetContentToLabel(nameLabel, GameManager.Instance.playerModel.Name);
		playerEmblemIcon.SetEmblem(GameManager.Instance.playerModel.PlayerEmblem);
		Helpers.GameObjectSetActive(actorIcon, value: false);
	}

	private void SetupLeaderBoardRewards(string rewards)
	{
		Rewards rewards2 = new Rewards(rewards);
		if (rewards2.RewardsList == null || rewards2.Count <= 0)
		{
			return;
		}
		UITable component = rewardsScrollView.GetComponent<UITable>();
		UIScrollView componentInParent = rewardsScrollView.GetComponentInParent<UIScrollView>();
		foreach (IReward rewards3 in rewards2.RewardsList)
		{
			GameObject gameObject = rewardsScrollView.AddChild(rewardPrefab);
			NGUITools.SetActive(gameObject, state: true);
			if (gameObject.TryGetComponent<RewardIcon>(out var component2))
			{
				component2.SetReward(rewards3);
			}
			rewardsItems.Add(gameObject);
		}
		component.Reposition();
		if ((bool)componentInParent)
		{
			componentInParent.ResetPosition();
		}
	}

	private void ClearRewardIcons()
	{
		for (int i = 0; i < rewardsItems.Count; i++)
		{
			NGUITools.Destroy(rewardsItems[i]);
		}
		rewardsItems.Clear();
	}
}
