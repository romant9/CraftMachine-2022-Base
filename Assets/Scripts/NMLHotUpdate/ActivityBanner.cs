using TWDModel;
using UnityEngine;

public class ActivityBanner : MonoBehaviour
{
	[SerializeField]
	private UITexture cdnTexture;

	[SerializeField]
	private UILabel timeLabel;

	[SerializeField]
	private UILabel nameLabel;

	public IActivityManagerIntegrationInterface integrationInterface;

	private long _gameModeTimeLeft;

	private void Update()
	{
		if (_gameModeTimeLeft >= 0)
		{
			_gameModeTimeLeft -= (long)(Time.deltaTime * 1000f);
			if (_gameModeTimeLeft <= 0)
			{
				_gameModeTimeLeft = 0L;
			}
		}
		if (timeLabel != null)
		{
			HelpersUI.SetContentToLabel(timeLabel, Helpers.FormatTimeNoZero(_gameModeTimeLeft));
		}
	}

	public void Init(string contentPath, long time, string nameStr, IActivityManagerIntegrationInterface activityData)
	{
		_gameModeTimeLeft = time - GameManager.Instance.playerModel.UtcTimeStamp;
		integrationInterface = activityData;
		LoadImageFromCdn.LoadImageToTarget(cdnTexture, contentPath);
		if (activityData is WeeklyChallengeClassTeamActivityModel weeklyChallengeClassTeamActivityModel)
		{
			string survivorClassName = HelpersLocalization.GetSurvivorClassName(weeklyChallengeClassTeamActivityModel.CurrentDefinition.GetClasses()[0]);
			HelpersUI.SetContentToLabel(nameLabel, LocalizationManager.GetText(nameStr, survivorClassName));
		}
		else
		{
			HelpersUI.SetContentToLabel(nameLabel, LocalizationManager.GetText(nameStr));
		}
	}

	private string FormatTimeLeft(long timeLeft)
	{
		if (timeLeft <= 0)
		{
			return "0";
		}
		return Helpers.FormatTimeNoZero(timeLeft);
	}
}
