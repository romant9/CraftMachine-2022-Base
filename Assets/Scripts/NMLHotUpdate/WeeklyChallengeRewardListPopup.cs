using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class WeeklyChallengeRewardListPopup : HUDElement
{
	public static string featureDisableId = "FeatureGuildRewardList";

	[SerializeField]
	private UIButtonExtended claimButton;

	[SerializeField]
	private UIChallengeRewardsWidgetList rewardsList;

	[SerializeField]
	private GameObject currencyEffectTarget;

	private List<LootEntry> lootEntryList = new List<LootEntry>();

	private LootEntryType openedForType;

	private bool isCollected;

	public static bool TryOpenForGuildGifts()
	{
		if (!GameManager.Instance.gameEconomyData.GetFeature("GuildRewardList").Enabled)
		{
			return false;
		}
		WeeklyChallengeRewardListPopup weeklyChallengeRewardListPopup = null;
		if (WeeklyChallengeHelper.GetWeeklyChallengeModel().GetRewardsPerType(LootEntryType.ChallengeGuildReward).Count > 0)
		{
			weeklyChallengeRewardListPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.WeeklyChallengeRewardListPopup) as WeeklyChallengeRewardListPopup;
			if (weeklyChallengeRewardListPopup != null && WeeklyChallengeHelper.GetWeeklyChallengeModel() != null)
			{
				weeklyChallengeRewardListPopup.OpenForType(LootEntryType.ChallengeGuildReward);
				return true;
			}
		}
		return false;
	}

	public void OnEnable()
	{
		AddListeners();
	}

	public void OnDisable()
	{
		RemoveListeners();
	}

	public void OpenForType(LootEntryType type)
	{
		openedForType = type;
		Open();
	}

	public override void Open()
	{
		base.Open();
		if (!(rewardsList != null) || WeeklyChallengeHelper.GetWeeklyChallengeModel() == null)
		{
			return;
		}
		isCollected = false;
		rewardsList.ClearCards();
		lootEntryList = WeeklyChallengeHelper.GetWeeklyChallengeModel().GetRewardsPerType(openedForType);
		if (lootEntryList == null)
		{
			return;
		}
		for (int i = 0; i < lootEntryList.Count; i++)
		{
			if (lootEntryList[i] != null)
			{
				rewardsList.CreateItemForLootEntry(lootEntryList[i]);
			}
		}
		rewardsList.Position();
	}

	protected override void OnCloseAnimOver()
	{
		base.OnCloseAnimOver();
		Clean();
		if (GameManager.Instance.playerModel.WeeklyChallenge.CanCollectRewards)
		{
			SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.OpenLootInUi).OpenForModel(GameManager.Instance.playerModel.WeeklyChallenge);
		}
	}

	public void Clean()
	{
		if (rewardsList != null)
		{
			rewardsList.ClearCards();
		}
	}

	private void OnClickClaim(UIButtonExtended button)
	{
		RemoveListeners();
		if (!isCollected)
		{
			isCollected = true;
			UIChallengeRewardsWidget uIChallengeRewardsWidget = null;
			if (rewardsList != null && !OfflineManager.IsLoadDataManager)
			{
				List<UIListCard<string>> cards = rewardsList.GetCards();
				if (cards != null && cards.Count > 0)
				{
					for (int i = 0; i < cards.Count; i++)
					{
						if (cards[i] != null)
						{
							uIChallengeRewardsWidget = cards[i].GetComponent<UIChallengeRewardsWidget>();
							if (uIChallengeRewardsWidget != null && uIChallengeRewardsWidget.lootEntry != null && currencyEffectTarget != null)
							{
								CampView.Instance.BuildingsHud.CreateCollectAnim(uIChallengeRewardsWidget.lootEntry.RewardedCurrency, currencyEffectTarget, uIChallengeRewardsWidget.lootEntry.RewardedAmount);
							}
						}
					}
				}
			}
			Helpers.ExecuteCommand(new ClaimChallengeRewardsCommand(openedForType));
		}
		UIEvent.Send("UpdateWeeklyChallengeDifficultyPopup");
		OnClickClose();
	}

	public override void OnBackButtonClicked()
	{
	}

	private void AddListeners()
	{
		if (claimButton != null)
		{
			claimButton.SetClickCallback(OnClickClaim);
		}
	}

	private void RemoveListeners()
	{
		if (claimButton != null)
		{
			claimButton.Clear();
		}
	}
}
