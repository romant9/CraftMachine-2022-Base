using TWDModel;
using UnityEngine;

public class ChallengeActiveHeader : MonoBehaviour
{
	[Header("Generic")]
	[SerializeField]
	private UILabel TitleLabel;

	[SerializeField]
	private UILabel TitleTwoLabel;

	[SerializeField]
	private UILabel TimeLabel;

	protected long timeLeft = -1L;

	public virtual void Activate()
	{
		WeeklyChallengeModel currentWeeklyModel = GetCurrentWeeklyModel();
		if (currentWeeklyModel != null)
		{
			bool flag = !currentWeeklyModel.Finished && currentWeeklyModel.CurrentDefinition != null;
			base.gameObject.SetActive(flag);
			if (flag)
			{
				WeeklyChallenge currentDefinition = currentWeeklyModel.CurrentDefinition;
				if (currentDefinition != null)
				{
					timeLeft = currentDefinition.EndTimeMilliseconds - GameManager.Instance.playerModel.UtcTimeStamp;
				}
				UpdateTexts();
			}
			else
			{
				timeLeft = -1L;
			}
		}
		else
		{
			base.gameObject.SetActive(value: false);
		}
	}

	public virtual void Update()
	{
		if (timeLeft > 0)
		{
			timeLeft -= (long)(Time.deltaTime * 1000f);
			if (timeLeft <= 0)
			{
				Activate();
			}
			if (TimeLabel != null)
			{
				TimeLabel.text = ((timeLeft > -1) ? Helpers.FormatTimeNoZero(timeLeft) : "0");
			}
		}
	}

	protected virtual void UpdateTexts()
	{
		string text = "";
		WeeklyChallengeModel currentWeeklyModel = GetCurrentWeeklyModel();
		if (currentWeeklyModel != null)
		{
			if (currentWeeklyModel.Finished)
			{
				WeeklyChallenge nextWeeklyChallenge = currentWeeklyModel.NextWeeklyChallenge;
				if (nextWeeklyChallenge != null)
				{
					MissionSpawnPointGroup spawnPointGroup = GameManager.Instance.gameEconomyData.MissionSpawnPointData.GetSpawnPointGroup(nextWeeklyChallenge.DetailMapId);
					if (spawnPointGroup != null)
					{
						text = HelpersLocalization.GetEpisodeName(spawnPointGroup);
					}
				}
			}
			else if (currentWeeklyModel.CurrentDefinition != null)
			{
				text = HelpersLocalization.GetEpisodeName(currentWeeklyModel.GetMissionSpawnPointGroup());
			}
		}
		if (TitleLabel != null)
		{
			TitleLabel.text = text;
		}
		_ = TitleTwoLabel != null;
	}

	protected WeeklyChallengeModel GetCurrentWeeklyModel()
	{
		if (GameManager.Instance != null && GameManager.Instance.playerModel.WeeklyChallenge != null)
		{
			return GameManager.Instance.playerModel.WeeklyChallenge;
		}
		return null;
	}
}
