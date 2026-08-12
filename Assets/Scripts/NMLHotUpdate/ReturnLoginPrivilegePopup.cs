using System;
using TWD.Externals;
using TWDModel;
using UnityEngine;

public class ReturnLoginPrivilegePopup : MonoBehaviour
{
	private const long MillisecondsPerDay = 86400000L;

	[SerializeField]
	private UILabel timeLabel;

	[SerializeField]
	private GameObject taskObj;

	[SerializeField]
	private UILabel taskLabel;

	[SerializeField]
	private GameObject completedObj;

	[SerializeField]
	private UILabel completedLabel;

	[SerializeField]
	private UIButton btnDetail;

	[SerializeField]
	private UIButton btnTask;

	[Header("四项特权")]
	[SerializeField]
	private ReturnLoginPrivilegePopupItem itemDoubleSurvivalPoints;

	[SerializeField]
	private ReturnLoginPrivilegePopupItem itemDoubleSupplies;

	[SerializeField]
	private ReturnLoginPrivilegePopupItem itemFastBuildingUpgrade;

	[SerializeField]
	private ReturnLoginPrivilegePopupItem itemFastSurvivorUpgrade;

	private long _privilegeTimeLeft;

	private bool _currentTaskCompleted;

	private long _privilegeRefreshTimeLeft;

	public const string DailyQuestsId = "DailyQuest_CompleteMissions1";

	private void Awake()
	{
		btnDetail.onClick.Add(new EventDelegate(OnInfoButtonClicked));
		btnTask.onClick.Add(new EventDelegate(OnJumpButtonClicked));
	}

	public void Open()
	{
		Helpers.GameObjectSetActive(base.gameObject, value: true);
		ReturnPrivilegeModel returnPrivilegeModel = GetReturnPrivilegeModel();
		if (returnPrivilegeModel != null)
		{
			long utcTimeStamp = GameManager.Instance.playerModel.UtcTimeStamp;
			_privilegeTimeLeft = returnPrivilegeModel.PrivilegeEndTimestamp - utcTimeStamp;
			_privilegeRefreshTimeLeft = returnPrivilegeModel.LastTaskRefreshTimestamp + 86400000 - utcTimeStamp;
			_currentTaskCompleted = returnPrivilegeModel.CurrentTaskCompleted;
			RefreshTaskState(returnPrivilegeModel);
			RefreshPrivilegeState(returnPrivilegeModel);
		}
	}

	public void Update()
	{
		if (_privilegeTimeLeft >= 0)
		{
			_privilegeTimeLeft -= (long)(Time.deltaTime * 1000f);
			if (_privilegeTimeLeft <= 0)
			{
				_privilegeTimeLeft = 0L;
				RefreshPrivilegeState(GetReturnPrivilegeModel());
			}
		}
		if (timeLabel != null)
		{
			string text = LocalizationManager.GetText("return.privilege.deadline", FormatTimeLeft(_privilegeTimeLeft));
			HelpersUI.SetContentToLabel(timeLabel, text);
		}
		if (!_currentTaskCompleted)
		{
			return;
		}
		if (_privilegeRefreshTimeLeft >= 0)
		{
			_privilegeRefreshTimeLeft -= (long)(Time.deltaTime * 1000f);
			if (_privilegeRefreshTimeLeft <= 0)
			{
				_privilegeRefreshTimeLeft = 0L;
			}
		}
		RefreshTaskState(GetReturnPrivilegeModel());
	}

	public void Close()
	{
		Helpers.GameObjectSetActive(base.gameObject, value: false);
	}

	private static ReturnPrivilegeModel GetReturnPrivilegeModel()
	{
		return GameManager.Instance?.playerModel?.ReturnActivityManager?.ReturnPrivilege;
	}

	private string FormatTimeLeft(long timeLeft)
	{
		if (timeLeft <= 0)
		{
			return "0";
		}
		return Helpers.FormatTimeNoZero(timeLeft);
	}

	private void OnInfoButtonClicked()
	{
		ReturnPrivilegeModel returnPrivilegeModel = GetReturnPrivilegeModel();
		if (returnPrivilegeModel != null)
		{
			string text = LocalizationManager.GetText("return.privilege.deadline.tips", returnPrivilegeModel.CompletedTaskCount);
			TooltipManager.OpenTextBoxWithText(btnDetail.gameObject, text, TooltipManager.Prefabs.TooltipTextboxGold);
		}
	}

	private void OnJumpButtonClicked()
	{
		DeepLinkNavigation.HandleDeepLink("MISSION_HUB");
	}

	private void RefreshTaskState(ReturnPrivilegeModel model)
	{
		if (model != null)
		{
			bool flag = model.RemainingRefreshCount <= 0 && model.CurrentTaskCompleted;
			bool isEnabled = !model.CurrentTaskCompleted;
			Helpers.GameObjectSetActive(taskObj, !model.CurrentTaskCompleted);
			Helpers.GameObjectSetActive(completedObj, model.CurrentTaskCompleted);
			if (model.CurrentTaskCompleted)
			{
				string content = (flag ? LocalizationManager.GetText("return.privilege.task.all.done") : LocalizationManager.GetText("return.privilege.task.done", FormatTimeLeft(_privilegeRefreshTimeLeft)));
				HelpersUI.SetContentToLabel(completedLabel, content);
			}
			else
			{
				int num = 1;
				int num2 = Math.Min(model.CurrentProgress, num);
				string text = LocalizationManager.GetText("return.privilege.task.desc", $"{num}");
				text = $"{text} ({num2}/{num})";
				HelpersUI.SetContentToLabel(taskLabel, text);
			}
			btnTask.isEnabled = isEnabled;
		}
	}

	private void RefreshPrivilegeState(ReturnPrivilegeModel model)
	{
		int timeInSeconds;
		bool active = model?.TryGetFastUpgradeTime(out timeInSeconds) ?? false;
		if (itemDoubleSurvivalPoints != null)
		{
			itemDoubleSurvivalPoints.SetActive(model?.HasDoubleSurvivalPointsBonus() ?? false);
		}
		if (itemDoubleSupplies != null)
		{
			itemDoubleSupplies.SetActive(model?.HasDoubleSuppliesBonus() ?? false);
		}
		if (itemFastBuildingUpgrade != null)
		{
			itemFastBuildingUpgrade.SetActive(active);
		}
		if (itemFastSurvivorUpgrade != null)
		{
			itemFastSurvivorUpgrade.SetActive(active);
		}
	}
}
