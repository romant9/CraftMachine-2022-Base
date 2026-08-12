using UnityEngine;

public class AchievementThingsToDoIndicator : MonoBehaviour
{
	[SerializeField]
	private UILabel requestsNumberLabel;

	[SerializeField]
	private bool includeAchievements;

	[SerializeField]
	private bool includeDailyQuest;

	private void OnEnable()
	{
		if (GameManager.Instance.playerModel.AchievementManager != null)
		{
			GameManager.Instance.playerModel.AchievementManager.OnAchievementsChanged += OnAchievementsChanged;
			GameManager.Instance.playerModel.AchievementManager.OnDailyQuestsChanged += OnAchievementsChanged;
		}
		UpdateUI();
	}

	private void OnDisable()
	{
		if (GameManager.Instance.playerModel.AchievementManager != null)
		{
			GameManager.Instance.playerModel.AchievementManager.OnAchievementsChanged -= OnAchievementsChanged;
			GameManager.Instance.playerModel.AchievementManager.OnDailyQuestsChanged -= OnAchievementsChanged;
		}
	}

	private void OnAchievementsChanged()
	{
		UpdateUI();
	}

	public void UpdateUI()
	{
		int num = 0;
		if (GameManager.Instance.playerModel.AchievementManager != null)
		{
			num = GameManager.Instance.playerModel.AchievementManager.GetClaimCount(includeAchievements, includeDailyQuest);
		}
		if (num == 0)
		{
			NGUITools.SetActiveChildren(base.gameObject, state: false);
			return;
		}
		NGUITools.SetActiveChildren(base.gameObject, state: true);
		requestsNumberLabel.text = num.ToString();
	}
}
