using System.Collections;
using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class ApocalypticWeeklyChallengeStartSkippingPopup : HUDElement
{
	[SerializeField]
	private UISprite skipTokenIcon;

	[SerializeField]
	private UILabel earnedPassesLabel;

	[SerializeField]
	private UIChallengeDifficultyProgressBarHeadStart difficultyProgressBar;

	[SerializeField]
	private GameObject closeButton;

	[SerializeField]
	private UILabel roundLabel;

	[SerializeField]
	private UIApocalypticChallengeRewardsWidgetList rewardsList;

	private int availableSkipTokens;

	public static bool TryOpenOnChallengeEnter()
	{
		ApocalypseWeeklyChallengeModel apocalypseWeeklyChallenge = GameManager.Instance.playerModel.ApocalypseWeeklyChallenge;
		Debug.Assert(apocalypseWeeklyChallenge != null);
		if (!apocalypseWeeklyChallenge.SkipTokensAvailableSeen)
		{
			Helpers.ExecuteCommand(new ApocalypticWeeklyChallengeSeenCommand(apocalypseWeeklyChallenge)
			{
				MarkActiveSkipTokensAsSeen = true
			});
			return TryOpenFromClick();
		}
		return false;
	}

	public static bool TryOpenFromClick()
	{
		ApocalypticWeeklyChallengeStartSkippingPopup apocalypticWeeklyChallengeStartSkippingPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.ApocalypticWeeklyChallengeStartSkipping) as ApocalypticWeeklyChallengeStartSkippingPopup;
		if (apocalypticWeeklyChallengeStartSkippingPopup != null)
		{
			apocalypticWeeklyChallengeStartSkippingPopup.Open();
			return true;
		}
		return false;
	}

	public void OnClickSkipToken()
	{
		ApocalypseWeeklyChallengeModel weeklyApocalypticChallengeModel = WeeklyChallengeHelper.GetWeeklyApocalypticChallengeModel();
		if (weeklyApocalypticChallengeModel != null && weeklyApocalypticChallengeModel.CurrentDefinition != null)
		{
			TooltipManager.OpenTextBoxWithText(skipTokenIcon.gameObject, LocalizationManager.GetText("Map.ApocalypticWeeklyChallenge.RoundPassInfo"));
		}
		else
		{
			TooltipManager.OpenTextBoxWithText(skipTokenIcon.gameObject, LocalizationManager.GetText("Map.WeeklyChallenge.RoundPassInfoGeneral"));
		}
	}

	private IEnumerator AnimateDifficulty(float skipTokenTimeStep)
	{
		ApocalypseWeeklyChallengeModel weeklyChallengeModel = GameManager.Instance.playerModel.ApocalypseWeeklyChallenge;
		int difficultyBeforeSkips = weeklyChallengeModel.DifficultyBeforeSkips;
		int toDifficulty = WeeklyChallengeHelper.GetCurrentDifficulty();
		int num = toDifficulty - difficultyBeforeSkips;
		if (num != 0)
		{
			float timeStep = (float)availableSkipTokens / (float)num * skipTokenTimeStep;
			int i = difficultyBeforeSkips;
			while (i < toDifficulty)
			{
				difficultyProgressBar.UpdateBetweenDifficulties(i, i + 1, 1f, timeStep);
				yield return new WaitForSeconds(timeStep);
				int num2 = i + 1;
				i = num2;
			}
			difficultyProgressBar.SetProgressionStepCount(weeklyChallengeModel.LastSeenCycleCount);
			float num3 = (float)weeklyChallengeModel.LastSeenChallengeDifficultyProgression;
			if (num3 > 0f)
			{
				difficultyProgressBar.UpdateBetweenDifficulties(toDifficulty, toDifficulty, num3, timeStep);
			}
		}
	}

	private IEnumerator CloseAfterWait()
	{
		closeButton.SetActive(value: false);
		StartCoroutine(AnimateDifficulty(0.8f));
		int i = availableSkipTokens - 1;
		while (i >= 0)
		{
			HelpersUI.SetContentToLabel(earnedPassesLabel, i.ToString());
			HelpersUI.SetContentToLabel(roundLabel, LocalizationManager.GetText("Map.WeeklyChallenge.Round{Number}", availableSkipTokens - i + 1));
			yield return new WaitForSeconds(0.8f);
			int num = i - 1;
			i = num;
		}
		yield return new WaitForSeconds(2f);
		if (rewardsList != null)
		{
			rewardsList.ClearCards();
		}
		base.OnClickClose();
	}

	public override void OnClickClose()
	{
		StartCoroutine(CloseAfterWait());
	}

	public override void Open()
	{
		base.Open();
		ApocalypseWeeklyChallengeModel apocalypseWeeklyChallenge = GameManager.Instance.playerModel.ApocalypseWeeklyChallenge;
		availableSkipTokens = apocalypseWeeklyChallenge.PreviousChallengeSkipTokens;
		difficultyProgressBar.SetDifficulty(apocalypseWeeklyChallenge.DifficultyBeforeSkips);
		HelpersUI.SetContentToLabel(earnedPassesLabel, apocalypseWeeklyChallenge.PreviousChallengeSkipTokens.ToString());
		HelpersUI.SetContentToLabel(roundLabel, LocalizationManager.GetText("Map.WeeklyChallenge.Round{Number}", 1));
		CreateRewardCard();
	}

	private void CreateRewardCard()
	{
		if (WeeklyChallengeHelper.GetWeeklyChallengeModel() == null)
		{
			return;
		}
		ClaimChallengeRewardsCommand claimChallengeRewardsCommand = new ClaimChallengeRewardsCommand(LootEntryType.ApocalypticStars, LootEntryType.ApocalypticRoundStars);
		claimChallengeRewardsCommand.ClaimWeeklyChallengeClassTeamSkipRewards = true;
		if (Helpers.ExecuteCommand(claimChallengeRewardsCommand) != TWDModelResult.OK)
		{
			return;
		}
		List<LootEntry> list = MergeSameCurrencyLootEntries(claimChallengeRewardsCommand.LootEntries);
		if (list == null || !(rewardsList != null))
		{
			return;
		}
		rewardsList.ClearCards();
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i] != null)
			{
				rewardsList.CreateItemForLootEntry(list[i]);
			}
		}
		rewardsList.Position();
	}

	private static List<LootEntry> MergeSameCurrencyLootEntries(List<LootEntry> source)
	{
		if (source == null)
		{
			return null;
		}
		List<LootEntry> list = new List<LootEntry>(source.Count);
		Dictionary<CurrencyType, int> dictionary = new Dictionary<CurrencyType, int>();
		for (int i = 0; i < source.Count; i++)
		{
			LootEntry lootEntry = source[i];
			if (lootEntry != null)
			{
				if (lootEntry.RewardedCurrency == CurrencyType.None || lootEntry.IsComponent())
				{
					list.Add(lootEntry);
					continue;
				}
				if (dictionary.TryGetValue(lootEntry.RewardedCurrency, out var value))
				{
					list[value].RewardedAmount += lootEntry.RewardedAmount;
					continue;
				}
				dictionary.Add(lootEntry.RewardedCurrency, list.Count);
				list.Add(lootEntry);
			}
		}
		return list;
	}
}
