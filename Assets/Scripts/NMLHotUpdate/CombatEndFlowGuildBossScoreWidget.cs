using UnityEngine;

public class CombatEndFlowGuildBossScoreWidget : CombatEndFlowStatsWidget
{
	[SerializeField]
	private UILabel[] scoreDigitLabels;

	[SerializeField]
	private float scoreRollDuration = 0.6f;

	private int targetScore;

	public void SetScore(int score, bool animateOnActivate = true)
	{
		targetScore = Mathf.Max(0, score);
		if (!animateOnActivate)
		{
			SetScoreImmediate(targetScore);
		}
	}

	public override void Activate()
	{
		base.Activate();
		if (scoreDigitLabels != null && scoreDigitLabels.Length != 0)
		{
			UIRollingNumberUtil.AnimateTo(scoreDigitLabels, 0, targetScore, scoreRollDuration);
		}
	}

	private void SetScoreImmediate(int score)
	{
		if (scoreDigitLabels != null && scoreDigitLabels.Length != 0)
		{
			UIRollingNumberUtil.SetValue(scoreDigitLabels, score);
		}
	}
}
