using System.Collections.Generic;
using BaseModel;
using TWDModel;
using UnityEngine;

public class DailyQuestsHudNotification : MonoBehaviour
{
	[SerializeField]
	private UILabel label;

	[SerializeField]
	private UILabel labelAmount;

	[SerializeField]
	private int ProgressTweenGroup = 5;

	[SerializeField]
	private int CompleteTweenGroup = 5;

	private DailyQuestModel dailyModel;

	private List<DailyQuestItemModel> queueList;

	private DailyQuestItemModel currentItemModel;

	public bool Animating => currentItemModel != null;

	public string GetLocalizedProgress(DailyQuestItemModel modelItem)
	{
		if (modelItem == null)
		{
			return "";
		}
		string text = LocalizationManager.GetText(modelItem.DisplayName, modelItem.CompletedCount, modelItem.CompletionTotalCap);
		return LocalizationManager.GetText("DailyQuest.Notification.Progess{name}{count}{max}", text, modelItem.CompletedCount, modelItem.CompletionTotalCap);
	}

	public string GetLocalizedComplete(DailyQuestItemModel modelItem)
	{
		if (modelItem == null)
		{
			return "";
		}
		string text = LocalizationManager.GetText(modelItem.DisplayName, modelItem.CompletedCount, modelItem.CompletionTotalCap);
		return LocalizationManager.GetText("DailyQuest.Notification.Complete{name}{count}{max}", text, modelItem.CompletedCount, modelItem.CompletionTotalCap);
	}

	public static bool TryGetDailyModel(out DailyQuestModel model)
	{
		if (GameManager.Instance != null && DailyQuestModel.GetIsSupported(GameManager.Instance.gameEconomyData) && GameManager.Instance.playerModel != null && GameManager.Instance.playerModel.DailyQuestManager != null)
		{
			model = GameManager.Instance.playerModel.DailyQuestManager;
			return true;
		}
		model = null;
		return false;
	}

	private void UpdateData()
	{
		if (dailyModel != null && !Animating)
		{
			if (queueList != null)
			{
				queueList.Clear();
			}
			PopulateWithQuests(ref queueList, dailyModel);
		}
	}

	private void OnEnable()
	{
		if (TryGetDailyModel(out dailyModel))
		{
			dailyModel.Changed -= OnDailyQuestChanged;
			dailyModel.Changed += OnDailyQuestChanged;
			AttachToActivatedQuests();
		}
		UpdateData();
	}

	private void OnDisable()
	{
		if (dailyModel != null)
		{
			if (dailyModel.ActiveQuests != null)
			{
				for (int i = 0; i < dailyModel.ActiveQuests.Count; i++)
				{
					if (dailyModel.ActiveQuests[i] != null)
					{
						dailyModel.ActiveQuests[i].Changed -= OnDailyQuestChanged;
					}
				}
			}
			dailyModel.Changed -= OnDailyQuestChanged;
			dailyModel = null;
		}
		currentItemModel = null;
	}

	private void AttachToActivatedQuests()
	{
		if (dailyModel == null || dailyModel.ActiveQuests == null)
		{
			return;
		}
		for (int i = 0; i < dailyModel.ActiveQuests.Count; i++)
		{
			if (dailyModel.ActiveQuests[i] != null)
			{
				dailyModel.ActiveQuests[i].Changed -= OnDailyQuestChanged;
				dailyModel.ActiveQuests[i].Changed += OnDailyQuestChanged;
			}
		}
	}

	private void Update()
	{
		if (Animating || queueList == null || queueList.Count <= 0)
		{
			return;
		}
		QuestsPopup questsPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.QuestsPopup, null, createIfNotExist: false) as QuestsPopup;
		if (questsPopup != null && questsPopup.IsOpen)
		{
			for (int i = 0; i < queueList.Count; i++)
			{
				MarkQuestAsSeen(queueList[i]);
			}
			queueList.Clear();
		}
		else
		{
			StartAnimating(queueList[0]);
		}
	}

	private void OnDailyQuestChanged(ModelObject model, string changed, object args)
	{
		if (changed == "ActiveQuests")
		{
			AttachToActivatedQuests();
		}
		UpdateData();
	}

	private void StartAnimating(DailyQuestItemModel modelItem)
	{
		if (modelItem != null && !Animating)
		{
			if (IsQuestCompleteUnseen(modelItem))
			{
				HelpersUI.SetContentToLabel(label, GetLocalizedComplete(modelItem));
				HelpersUI.SetContentToLabel(labelAmount, modelItem.CompletedCount + "/" + modelItem.CompletionTotalCap);
				TweenManager.PlayTweenGroup(base.gameObject, CompleteTweenGroup, forward: true, OnTweenComplete);
				currentItemModel = modelItem;
			}
			else if (IsQuestProgessUnseen(modelItem))
			{
				HelpersUI.SetContentToLabel(label, GetLocalizedProgress(modelItem));
				HelpersUI.SetContentToLabel(labelAmount, modelItem.CompletedCount + "/" + modelItem.CompletionTotalCap);
				TweenManager.PlayTweenGroup(base.gameObject, ProgressTweenGroup, forward: true, OnTweenComplete);
				currentItemModel = modelItem;
			}
		}
	}

	private void MarkQuestAsSeen(DailyQuestItemModel quest)
	{
		Helpers.ExecuteCommand(new MarkDailyQuestSeenComand
		{
			QuestId = quest.ModelId,
			SeenValue = MarkDailyQuestSeenComand.Value.CompletedCount
		});
	}

	private void OnTweenComplete()
	{
		if (currentItemModel != null)
		{
			MarkQuestAsSeen(currentItemModel);
			currentItemModel = null;
		}
		UpdateData();
	}

	private void PopulateWithQuests(ref List<DailyQuestItemModel> list, DailyQuestModel model)
	{
		if (model == null)
		{
			return;
		}
		if (list == null)
		{
			list = new List<DailyQuestItemModel>();
		}
		for (int i = 0; i < ((model.ActiveQuests != null) ? model.ActiveQuests.Count : 0); i++)
		{
			if (IsQuestCompleteUnseen(model.ActiveQuests[i]) || IsQuestProgessUnseen(model.ActiveQuests[i]))
			{
				list.Add(model.ActiveQuests[i]);
			}
		}
	}

	private bool IsQuestCompleteUnseen(DailyQuestItemModel model)
	{
		if (model == null)
		{
			return false;
		}
		if (!model.CompletedCountSeen)
		{
			return model.IsCompleted;
		}
		return false;
	}

	private bool IsQuestProgessUnseen(DailyQuestItemModel model)
	{
		if (model == null)
		{
			return false;
		}
		return !model.CompletedCountSeen;
	}
}
