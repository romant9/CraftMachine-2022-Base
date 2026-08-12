using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class DailyQuestListPanel : ScrollableListPanel<DailyQuest>
{
	[SerializeField]
	private UILabel nextDailyQuestTimerLabel;

	private int nextDailyQuestTimer;

	private int previousTimeSeconds = -1;

	protected override bool LastEntryAtTop => false;

	public void Init()
	{
		if (GameManager.Instance.playerModel.AchievementManager != null)
		{
			SetCards(GameManager.Instance.playerModel.DailyQuests);
			GameManager.Instance.playerModel.AchievementManager.OnDailyQuestsChanged += OnDailyQuestsChanged;
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("camp/ui_tab_change");
		}
		InitNextDailyQuestTimer();
	}

	private void OnDailyQuestsChanged()
	{
		List<DailyQuest> list = new List<DailyQuest>();
		List<DailyQuest> dailyQuests = GameManager.Instance.playerModel.DailyQuests;
		if (dailyQuests == null)
		{
			return;
		}
		for (int i = 0; i < cards.Count; i++)
		{
			UIListCard<DailyQuest> uIListCard = cards[i];
			if (uIListCard != null && uIListCard.gameObject != null && !dailyQuests.Contains(uIListCard.Item))
			{
				list.Add(uIListCard.Item);
			}
		}
		for (int j = 0; j < list.Count; j++)
		{
			RemoveCard(list[j]);
		}
		for (int k = 0; k < dailyQuests.Count; k++)
		{
			if (GetCard(dailyQuests[k]) == null)
			{
				AddCard(dailyQuests[k], setupInitialPosition: true);
			}
		}
		for (int l = 0; l < cards.Count; l++)
		{
			UIListCard<DailyQuest> uIListCard2 = cards[l];
			if (uIListCard2 != null && uIListCard2.gameObject != null)
			{
				uIListCard2.UpdateUI();
			}
		}
		InitNextDailyQuestTimer();
	}

	public void Update()
	{
		if (ShowNextQuestTime())
		{
			nextDailyQuestTimer = GameManager.Instance.playerModel.AchievementManager.TimeToNextDailyQuest();
			if (nextDailyQuestTimer != previousTimeSeconds)
			{
				previousTimeSeconds = nextDailyQuestTimer;
				nextDailyQuestTimerLabel.gameObject.SetActive(value: true);
				nextDailyQuestTimerLabel.text = LocalizationManager.GetText("Popup.Achievement.NextQuestIn{Time}", Helpers.FormatTimeNoZero(nextDailyQuestTimer * 1000));
			}
			if (nextDailyQuestTimer <= 0)
			{
				InitNextDailyQuestTimer();
			}
		}
	}

	private void InitNextDailyQuestTimer()
	{
		AchievementManager achievementManager = GameManager.Instance.playerModel.AchievementManager;
		nextDailyQuestTimer = achievementManager.TimeToNextDailyQuest();
		if (nextDailyQuestTimer <= 0 && !achievementManager.HasMaxDailyQuest())
		{
			Helpers.ExecuteCommand(new CheckAchievementsCommand());
		}
		nextDailyQuestTimerLabel.gameObject.SetActive(ShowNextQuestTime());
	}

	private bool ShowNextQuestTime()
	{
		if (nextDailyQuestTimer > 0)
		{
			if (cards != null)
			{
				return cards.Count < 3;
			}
			return true;
		}
		return false;
	}

	private void OnDisable()
	{
		nextDailyQuestTimerLabel.gameObject.SetActive(value: false);
		if (GameManager.Instance.playerModel.AchievementManager != null)
		{
			GameManager.Instance.playerModel.AchievementManager.OnDailyQuestsChanged -= OnDailyQuestsChanged;
		}
	}
}
