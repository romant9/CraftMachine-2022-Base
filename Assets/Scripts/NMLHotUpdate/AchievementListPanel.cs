using System.Collections.Generic;
using TWDModel;

public class AchievementListPanel : ScrollableListPanel<Achievement>
{
	protected override bool LastEntryAtTop => false;

	private void OnEnable()
	{
	}

	public void Init()
	{
		if (GameManager.Instance.playerModel.AchievementManager != null)
		{
			SetCards(GameManager.Instance.playerModel.AchievementManager.Achievements);
			GameManager.Instance.playerModel.AchievementManager.OnAchievementsChanged += OnAchievementsChanged;
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("camp/ui_tab_change");
		}
	}

	private void OnAchievementsChanged()
	{
		if (GameManager.Instance.playerModel.AchievementManager != null)
		{
			List<Achievement> achievements = GameManager.Instance.playerModel.AchievementManager.Achievements;
			SetCards(achievements);
		}
	}

	private void OnDisable()
	{
		if (GameManager.Instance.playerModel.AchievementManager != null)
		{
			GameManager.Instance.playerModel.AchievementManager.OnAchievementsChanged -= OnAchievementsChanged;
		}
	}
}
