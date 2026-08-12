using TWDModel;

public class ChallengeNotActiveHeader : ChallengeActiveHeader
{
	public override void Activate()
	{
		WeeklyChallengeModel currentWeeklyModel = GetCurrentWeeklyModel();
		if (currentWeeklyModel != null)
		{
			bool flag = currentWeeklyModel.Finished && currentWeeklyModel.NextWeeklyChallenge != null;
			base.gameObject.SetActive(flag);
			if (flag)
			{
				WeeklyChallenge nextWeeklyChallenge = currentWeeklyModel.NextWeeklyChallenge;
				if (nextWeeklyChallenge != null)
				{
					timeLeft = nextWeeklyChallenge.StartTimeMilliseconds - GameManager.Instance.playerModel.UtcTimeStamp;
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
}
