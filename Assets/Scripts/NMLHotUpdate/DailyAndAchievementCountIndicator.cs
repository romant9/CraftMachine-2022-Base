using BaseModel;
using TWDModel;
using UnityEngine;

public class DailyAndAchievementCountIndicator : ThingsToDoIndicator
{
	[SerializeField]
	private bool includeDailyQuests;

	[SerializeField]
	private bool includeAchievements;

	private DailyQuestModel dailyModel;

	private void OnEnable()
	{
		if (includeDailyQuests && DailyQuestsHudNotification.TryGetDailyModel(out dailyModel))
		{
			dailyModel.Changed -= OnDailyQuestChanged;
			dailyModel.Changed += OnDailyQuestChanged;
		}
		if (includeAchievements && GameManager.Instance.playerModel.AchievementManager != null)
		{
			GameManager.Instance.playerModel.AchievementManager.OnAchievementsChanged -= OnAchievementsChanged;
			GameManager.Instance.playerModel.AchievementManager.OnAchievementsChanged += OnAchievementsChanged;
		}
		UpdateUI();
	}

	private void OnDisable()
	{
		if (dailyModel != null)
		{
			dailyModel.Changed -= OnDailyQuestChanged;
			dailyModel = null;
		}
		if (GameManager.Instance.playerModel.AchievementManager != null)
		{
			GameManager.Instance.playerModel.AchievementManager.OnAchievementsChanged -= OnAchievementsChanged;
		}
	}

	private void UpdateUI()
	{
		int num = 0;
		if (includeDailyQuests && dailyModel != null)
		{
			num += dailyModel.CalculateUnclaimedCount();
		}
		if (includeAchievements && GameManager.Instance.playerModel.AchievementManager != null)
		{
			num += GameManager.Instance.playerModel.AchievementManager.GetClaimCount(includeAchievements: true, includeDailyQuest: false);
		}
		SetNumber(num);
	}

	private void OnDailyQuestChanged(ModelObject model, string changed, object args)
	{
		UpdateUI();
	}

	private void OnAchievementsChanged()
	{
		UpdateUI();
	}
}
