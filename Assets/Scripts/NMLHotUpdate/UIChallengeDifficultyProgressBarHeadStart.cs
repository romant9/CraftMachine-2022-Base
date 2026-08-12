using System.Collections;
using Client.Tweener;
using UnityEngine;

public class UIChallengeDifficultyProgressBarHeadStart : UIProgressBarExtended
{
	[SerializeField]
	private UISprite segmentSprite;

	public override void OnEnable()
	{
		base.OnEnable();
		HelpersUI.SetSprite(segmentSprite, HelpersGfx.GetIconNameForChallengeSegments(1));
	}

	public override void OnDisable()
	{
		base.OnDisable();
		Clear();
	}

	public void SetDifficulty(int difficulty)
	{
		SetProgress(0f);
		SetTextToLabel(difficulty.ToString());
	}

	public void SetProgressionStepCount(int progressionStepCount)
	{
		HelpersUI.SetSprite(segmentSprite, HelpersGfx.GetIconNameForChallengeSegments(progressionStepCount));
	}

	private IEnumerator AnimateDifficulty(int fromDifficulty, int toDifficulty, float toProgression, float timeStep)
	{
		SetTextToLabel(fromDifficulty.ToString());
		TweenToProgress(toProgression, 0f, timeStep, Easing.All.CubicEaseInOut);
		yield return new WaitForSeconds(timeStep);
		SetTextToLabel(toDifficulty.ToString());
	}

	public void UpdateBetweenDifficulties(int fromDifficulty, int toDifficulty, float toProgression, float timeStep)
	{
		StartCoroutine(AnimateDifficulty(fromDifficulty, toDifficulty, toProgression, timeStep));
	}
}
