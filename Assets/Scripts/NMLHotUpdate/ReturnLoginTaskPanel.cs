using System;
using System.Collections.Generic;
using BaseModel;
using TWDModel;
using UnityEngine;

public class ReturnLoginTaskPanel : ScrollableListPanel<ReturnLoginTaskItem.TaskData>
{
	[SerializeField]
	private Transform collectTarget;

	[SerializeField]
	private UILabel refreshTimeLabel;

	private const long DailyRefreshIntervalMilliseconds = 86400000L;

	private ReturnQuestAndExchangeModel _model;

	public void Open()
	{
		Helpers.GameObjectSetActive(base.gameObject, value: true);
		RefreshList();
		SubscribeModelChanges(subscribe: true);
	}

	public void Close()
	{
		SubscribeModelChanges(subscribe: false);
		ClearCards();
		Helpers.GameObjectSetActive(base.gameObject, value: false);
	}

	private void OnDisable()
	{
		SubscribeModelChanges(subscribe: false);
	}

	private void OnModelChanged(ModelObject model, string changed, object args)
	{
	}

	private void Update()
	{
		if (base.gameObject.activeInHierarchy && !(refreshTimeLabel == null) && _model?.DailyQuest != null)
		{
			long lastRefreshTimestamp = _model.DailyQuest.LastRefreshTimestamp;
			long valueOrDefault = (GameManager.Instance?.playerModel?.UtcTimeStamp).GetValueOrDefault();
			long milliSeconds = ((lastRefreshTimestamp > 0) ? Math.Max(lastRefreshTimestamp + 86400000 - valueOrDefault, 0L) : 0);
			HelpersUI.SetContentToLabel(refreshTimeLabel, LocalizationManager.GetText("return.quest.refresh", Helpers.FormatTimeNoZero(milliSeconds)));
		}
	}

	private void SubscribeModelChanges(bool subscribe)
	{
		if (_model == null)
		{
			return;
		}
		_model.Changed -= OnModelChanged;
		if (_model.RepeatQuest != null)
		{
			_model.RepeatQuest.Changed -= OnModelChanged;
		}
		if (subscribe)
		{
			_model.Changed += OnModelChanged;
			if (_model.RepeatQuest != null)
			{
				_model.RepeatQuest.Changed += OnModelChanged;
			}
		}
	}

	protected override void SetCard(UIListCard<ReturnLoginTaskItem.TaskData> card)
	{
		if (card is ReturnLoginTaskItem returnLoginTaskItem)
		{
			returnLoginTaskItem.SetCollectTarget(collectTarget);
		}
	}

	private void RefreshList()
	{
		_model = GetModel();
		if (_model == null)
		{
			ClearCards();
			return;
		}
		List<ReturnLoginTaskItem.TaskData> items = CollectTasks(_model);
		Helpers.GameObjectSetActive(cardPrefab, value: true);
		SetCards(items);
		Helpers.GameObjectSetActive(cardPrefab, value: false);
	}

	private List<ReturnLoginTaskItem.TaskData> CollectTasks(ReturnQuestAndExchangeModel model)
	{
		List<ReturnLoginTaskItem.TaskData> list = new List<ReturnLoginTaskItem.TaskData>();
		CollectDailyTasks(model, list);
		CollectRepeatTasks(model, list);
		return list;
	}

	private void CollectDailyTasks(ReturnQuestAndExchangeModel model, List<ReturnLoginTaskItem.TaskData> entries)
	{
		if (model.DailyQuest?.Tasks == null)
		{
			return;
		}
		for (int i = 0; i < model.DailyQuest.Tasks.Count; i++)
		{
			ReturnDailyQuestItemModel returnDailyQuestItemModel = model.DailyQuest.Tasks[i];
			if (returnDailyQuestItemModel?.Definition != null)
			{
				entries.Add(new ReturnLoginTaskItem.TaskData
				{
					DailyTask = returnDailyQuestItemModel
				});
			}
		}
	}

	private void CollectRepeatTasks(ReturnQuestAndExchangeModel model, List<ReturnLoginTaskItem.TaskData> entries)
	{
		if (model.RepeatQuest?.Tasks == null)
		{
			return;
		}
		for (int i = 0; i < model.RepeatQuest.Tasks.Count; i++)
		{
			ReturnRepeatQuestItemModel returnRepeatQuestItemModel = model.RepeatQuest.Tasks[i];
			if (returnRepeatQuestItemModel?.Definition != null)
			{
				entries.Add(new ReturnLoginTaskItem.TaskData
				{
					RepeatTask = returnRepeatQuestItemModel,
					RepeatQuest = model.RepeatQuest
				});
			}
		}
	}

	private ReturnQuestAndExchangeModel GetModel()
	{
		return GameManager.Instance?.playerModel?.ReturnActivityManager?.ReturnQuestAndExchange;
	}
}
