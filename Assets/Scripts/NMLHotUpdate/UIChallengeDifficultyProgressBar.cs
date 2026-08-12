using System.Collections;
using Client.Tweener;
using TWDModel;
using UnityEngine;

public class UIChallengeDifficultyProgressBar : UIProgressBarExtended
{
	[SerializeField]
	protected UISprite segmentSprite;

	[SerializeField]
	protected UISprite backgroundSprite;

	private FixedPoint progressionCurrent;

	private FixedPoint progressionOld;

	private int difficultyCurrent;

	private float difficultyOld;

	private float genricTweenDuration = 1f;

	private bool coroutineBusy;

	public override void OnEnable()
	{
		base.OnEnable();
		coroutineBusy = false;
		UpdateToOldProgression();
	}

	public override void OnDisable()
	{
		base.OnDisable();
		Clear();
	}

	public void UpdateUIAfterSeconds(float delay)
	{
		bool isNormalChallenge = WeeklyChallengeHelper.IsNormalChallenge;
		if (base.gameObject != null && base.gameObject.activeInHierarchy && !coroutineBusy && WeeklyChallengeHelper.GetWeeklyChallengeModel() != null && WeeklyChallengeHelper.GetWeeklyApocalypticChallengeModel() != null)
		{
			coroutineBusy = true;
			progressionCurrent = WeeklyChallengeHelper.GetProgressUntilNextDifficulty();
			progressionOld = (isNormalChallenge ? WeeklyChallengeHelper.GetWeeklyChallengeModel().LastSeenChallengeDifficultyProgression : WeeklyChallengeHelper.GetWeeklyApocalypticChallengeModel().LastSeenChallengeDifficultyProgression);
			difficultyCurrent = WeeklyChallengeHelper.GetCurrentDifficulty();
			difficultyOld = (isNormalChallenge ? WeeklyChallengeHelper.GetWeeklyChallengeModel().LastSeenChallengeDifficulty : WeeklyChallengeHelper.GetWeeklyApocalypticChallengeModel().LastSeenChallengeDifficulty);
			if ((float)(isNormalChallenge ? WeeklyChallengeHelper.GetWeeklyChallengeModel().LastSeenChallengeDifficulty : WeeklyChallengeHelper.GetWeeklyApocalypticChallengeModel().LastSeenChallengeDifficulty) <= 0f || (progressionOld == progressionCurrent && (float)difficultyCurrent == difficultyOld))
			{
				coroutineBusy = false;
				OnEasingComplete();
			}
			else
			{
				UpdateToOldProgression();
				StartCoroutine(UpdateAfterDelay(delay));
			}
		}
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
		if (base.IsAnimating || WeeklyChallengeHelper.GetWeeklyChallengeModel() == null || WeeklyChallengeHelper.GetWeeklyApocalypticChallengeModel() == null)
		{
			return;
		}
		if ((float)difficultyCurrent > difficultyOld)
		{
			if (progressionCurrent == 0.0 && progressionOld == 0.0)
			{
				SetTextToLabel(difficultyOld.ToString());
				TweenToProgress(1f, 0f, genricTweenDuration, Easing.All.CubicEaseOut);
			}
			else
			{
				SetTextToLabel(difficultyOld.ToString());
				TweenToProgress(1f, (float)progressionOld, genricTweenDuration, Easing.All.CubicEaseOut);
			}
		}
		else if (progressionOld != progressionCurrent)
		{
			SetTextToLabel((WeeklyChallengeHelper.IsNormalChallenge ? WeeklyChallengeHelper.GetWeeklyChallengeModel().LastSeenChallengeDifficulty : WeeklyChallengeHelper.GetWeeklyApocalypticChallengeModel().LastSeenChallengeDifficulty).ToString());
			TweenToProgress((float)progressionCurrent, (float)progressionOld, genricTweenDuration, Easing.All.CubicEaseOut);
		}
		else
		{
			OnEasingComplete();
		}
	}

	public void UpdateToOldProgression()
	{
		if (WeeklyChallengeHelper.IsNormalChallenge)
		{
			if (WeeklyChallengeHelper.GetWeeklyChallengeModel() != null)
			{
				base.CurrentTweener = null;
				SetProgress((float)WeeklyChallengeHelper.GetWeeklyChallengeModel().LastSeenChallengeDifficultyProgression);
				SetTextToLabel(WeeklyChallengeHelper.GetWeeklyChallengeModel().LastSeenChallengeDifficulty.ToString());
				HelpersUI.SetSprite(segmentSprite, HelpersGfx.GetIconNameForChallengeSegments(WeeklyChallengeHelper.GetWeeklyChallengeModel().LastSeenCycleCount));
			}
		}
		else if (WeeklyChallengeHelper.GetWeeklyApocalypticChallengeModel() != null)
		{
			base.CurrentTweener = null;
			SetProgress((float)WeeklyChallengeHelper.GetWeeklyApocalypticChallengeModel().LastSeenChallengeDifficultyProgression);
			SetTextToLabel(WeeklyChallengeHelper.GetWeeklyApocalypticChallengeModel().LastSeenChallengeDifficulty.ToString());
			HelpersUI.SetSprite(segmentSprite, HelpersGfx.GetIconNameForChallengeSegments(WeeklyChallengeHelper.GetWeeklyApocalypticChallengeModel().LastSeenCycleCount));
		}
	}

	public void UpdateToCurrentProgression()
	{
		if (WeeklyChallengeHelper.IsNormalChallenge)
		{
			if (WeeklyChallengeHelper.GetWeeklyChallengeModel() != null)
			{
				base.CurrentTweener = null;
				SetProgress((float)WeeklyChallengeHelper.GetProgressUntilNextDifficulty());
				SetTextToLabel(WeeklyChallengeHelper.GetCurrentDifficulty().ToString());
				HelpersUI.SetSprite(segmentSprite, HelpersGfx.GetIconNameForChallengeSegments(WeeklyChallengeHelper.TotalCyclesInCurrent()));
			}
		}
		else if (WeeklyChallengeHelper.GetWeeklyApocalypticChallengeModel() != null)
		{
			base.CurrentTweener = null;
			SetProgress((float)WeeklyChallengeHelper.GetProgressUntilNextDifficulty());
			SetTextToLabel(WeeklyChallengeHelper.GetCurrentDifficulty().ToString());
			HelpersUI.SetSprite(segmentSprite, HelpersGfx.GetIconNameForChallengeSegments(WeeklyChallengeHelper.TotalCyclesInCurrent()));
		}
	}

	public override void Clear()
	{
		base.Clear();
		coroutineBusy = false;
	}

	protected override void OnEasingComplete()
	{
		base.OnEasingComplete();
		UpdateToCurrentProgression();
		WeeklyChallengeHelper.MarkDifficultyAsSeen(WeeklyChallengeHelper.GetCurrentDifficulty());
		WeeklyChallengeHelper.MarkDifficultyProgressionAsSeen(WeeklyChallengeHelper.GetProgressUntilNextDifficulty());
		WeeklyChallengeHelper.MarkCycleAsSeen(WeeklyChallengeHelper.TotalCyclesInCurrent());
	}

	private IEnumerator UpdateAfterDelay(float delay)
	{
		yield return new WaitForSeconds(delay);
		if (base.gameObject != null)
		{
			UpdateUI();
		}
		coroutineBusy = false;
	}
}
