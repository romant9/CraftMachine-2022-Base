using System.Collections;
using TWDModel;
using UnityEngine;

public class WeeklyChallengeStartSkippingPopup : HUDElement
{
	[SerializeField]
	private UISprite skipTokenIcon;

	[SerializeField]
	private UILabel earnedPassesLabel;

	[SerializeField]
	private UILabel activeForRoundsLabel;

	[SerializeField]
	private UIChallengeDifficultyProgressBarHeadStart difficultyProgressBar;

	[SerializeField]
	private GameObject closeButton;

	[SerializeField]
	private UILabel roundLabel;

	private int availableSkipTokens;

	public static bool TryOpenOnChallengeEnter()
	{
		WeeklyChallengeModel weeklyChallenge = GameManager.Instance.playerModel.WeeklyChallenge;
		Debug.Assert(weeklyChallenge != null);
		if (!weeklyChallenge.SkipTokensAvailableSeen)
		{
			Helpers.ExecuteCommand(new WeeklyChallengeSeenCommand(weeklyChallenge)
			{
				MarkActiveSkipTokensAsSeen = true
			});
			return TryOpenFromClick();
		}
		return false;
	}

	public static bool TryOpenFromClick()
	{
		WeeklyChallengeStartSkippingPopup weeklyChallengeStartSkippingPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.WeeklyChallengeStartSkipping) as WeeklyChallengeStartSkippingPopup;
		if (weeklyChallengeStartSkippingPopup != null)
		{
			weeklyChallengeStartSkippingPopup.Open();
			weeklyChallengeStartSkippingPopup.OnClose += WeeklyChallengeMasterMissionInfo.OnDependentWindowClosed;
			return true;
		}
		return false;
	}

	public void OnClickSkipToken()
	{
		WeeklyChallengeModel weeklyChallengeModel = WeeklyChallengeHelper.GetWeeklyChallengeModel();
		if (weeklyChallengeModel != null && weeklyChallengeModel.CurrentDefinition != null)
		{
			TooltipManager.OpenTextBoxWithText(skipTokenIcon.gameObject, LocalizationManager.GetText("Map.WeeklyChallenge.RoundPassInfo{RoundsToGetPass}{RoundsLeftToGetPass}", weeklyChallengeModel.GetCurrentCycleRoundsToSkipToken(), weeklyChallengeModel.CalculateRoundsToNextSkipToken()));
		}
		else
		{
			TooltipManager.OpenTextBoxWithText(skipTokenIcon.gameObject, LocalizationManager.GetText("Map.WeeklyChallenge.RoundPassInfoGeneral"));
		}
	}

	private IEnumerator AnimateDifficulty(float skipTokenTimeStep)
	{
		WeeklyChallengeModel weeklyChallengeModel = GameManager.Instance.playerModel.WeeklyChallenge;
		int num = (int)weeklyChallengeModel.DifficultyBeforeSkips;
		int toDifficulty = WeeklyChallengeHelper.GetCurrentDifficulty();
		int num2 = toDifficulty - num;
		if (num2 != 0)
		{
			float timeStep = (float)availableSkipTokens / (float)num2 * skipTokenTimeStep;
			int i = num;
			while (i < toDifficulty)
			{
				difficultyProgressBar.UpdateBetweenDifficulties(i, i + 1, 1f, timeStep);
				yield return new WaitForSeconds(timeStep);
				int num3 = i + 1;
				i = num3;
			}
			difficultyProgressBar.SetProgressionStepCount(weeklyChallengeModel.LastSeenCycleCount);
			float num4 = (float)weeklyChallengeModel.LastSeenChallengeDifficultyProgression;
			if (num4 > 0f)
			{
				difficultyProgressBar.UpdateBetweenDifficulties(toDifficulty, toDifficulty, num4, timeStep);
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
		base.OnClickClose();
	}

	public override void OnClickClose()
	{
		StartCoroutine(CloseAfterWait());
	}

	public override void Open()
	{
		base.Open();
		WeeklyChallengeModel weeklyChallenge = GameManager.Instance.playerModel.WeeklyChallenge;
		availableSkipTokens = weeklyChallenge.PreviousChallengeSkipTokens;
		difficultyProgressBar.SetDifficulty((int)weeklyChallenge.DifficultyBeforeSkips);
		string text = null;
		text = ((weeklyChallenge.ActiveSkipTokens > 1) ? LocalizationManager.GetText("Map.WeeklyChallenge.ActiveForRounds{Count}", weeklyChallenge.ActiveSkipTokens) : LocalizationManager.GetText("Map.WeeklyChallenge.ActiveForRound"));
		HelpersUI.SetContentToLabel(earnedPassesLabel, weeklyChallenge.ActiveSkipTokens.ToString());
		HelpersUI.SetContentToLabel(activeForRoundsLabel, text);
		HelpersUI.SetContentToLabel(roundLabel, LocalizationManager.GetText("Map.WeeklyChallenge.Round{Number}", 1));
	}
}
