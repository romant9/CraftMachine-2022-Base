using System.Collections.Generic;
using BaseModel;
using TWDModel;
using UnityEngine;

public class SurvivorUpgradeHudNotification : MonoBehaviour
{
	[SerializeField]
	private UILabel NotificationLabel;

	[SerializeField]
	private bool CombatTeamOnly;

	[SerializeField]
	private bool ShowUpgrade = true;

	[SerializeField]
	private int TweenGroup;

	private bool updagradeActive;

	private BuildingModel TrainingGrounds;

	private static bool IsAnySurvivorUpgrading => GameManager.Instance.playerModel.SurvivorContainer.HasUpgradingSurvivor;

	public void UpdateList()
	{
		List<SurvivorModel> list = ((!CombatTeamOnly) ? GameManager.Instance.playerModel.SurvivorContainer.GetUpgradeableSurvivors() : GameManager.Instance.playerModel.SurvivorContainer.GetUpgradeableCombatSurvivors());
		foreach (SurvivorModel item in list)
		{
			if (item != null)
			{
				SurvivorUpgradeHudNotificationData.Animate(item);
			}
		}
		if (ShowUpgrade)
		{
			SurvivorModel survivorModel = TrainingGroundsUpgradedUnseenModel();
			if (survivorModel != null)
			{
				SurvivorUpgradeHudNotificationData.Animate(survivorModel);
			}
		}
	}

	private void Update()
	{
		if (SingularityMonoBehaviour<HUDManager>.Instance.NumberDialogsOpen != 0)
		{
			return;
		}
		if (SurvivorUpgradeHudNotificationData.CurrentDelayTime > 0f)
		{
			SurvivorUpgradeHudNotificationData.CurrentDelayTime -= Time.deltaTime;
			return;
		}
		bool isAnySurvivorUpgrading = IsAnySurvivorUpgrading;
		if (isAnySurvivorUpgrading)
		{
			return;
		}
		if (!isAnySurvivorUpgrading && updagradeActive)
		{
			updagradeActive = false;
			UpdateList();
		}
		if (IsTrainingGroundsUpgrading() || SurvivorUpgradeHudNotificationData.Instance.animationRunning || SurvivorUpgradeHudNotificationData.Instance.AnimationList.Count <= 0 || CampHUD.IsTweenGroupEnabled(base.gameObject, TweenGroup))
		{
			return;
		}
		SurvivorModel nextModelToShow = SurvivorUpgradeHudNotificationData.GetNextModelToShow();
		if (nextModelToShow != null && NotificationLabel != null)
		{
			SurvivorUpgradeHudNotificationData.SetCurrentModel(nextModelToShow);
			SurvivorModel survivorModel = TrainingGroundsUpgradedUnseenModel();
			if (survivorModel != null && survivorModel == nextModelToShow && ShowUpgrade)
			{
				NotificationLabel.text = LocalizationManager.GetText("SurvivorManagement.Button.Notification.SurvivorUpgraded{Parameter}", survivorModel.Name);
			}
			else if (nextModelToShow.CanUpgrade)
			{
				NotificationLabel.text = LocalizationManager.GetText("SurvivorManagement.Button.Notification.SurvivorUpgradeable{Parameter}", nextModelToShow.Name);
			}
			else
			{
				NotificationLabel.text = "";
			}
			if (NotificationLabel.text != "")
			{
				CampHUD.PlayTweenGroupInGameObject(base.gameObject, TweenGroup, SurvivorUpgradeHudNotificationData.TweenDoneCallback);
			}
		}
	}

	private void OnEnable()
	{
		SurvivorUpgradeHudNotificationData.ResetStartDelayIfNotAnimating();
		updagradeActive = IsAnySurvivorUpgrading;
		if (GameManager.Instance != null)
		{
			UpdateList();
			GameManager.Instance.playerModel.SurvivorContainer.Changed += PlayerModelChanged;
			GameManager.Instance.playerModel.Camp.Changed += CampModelChanged;
			UIEvent.OnUIEvent += OnUIEvent;
		}
	}

	private void OnDisable()
	{
		if (GameManager.Instance != null)
		{
			GameManager.Instance.playerModel.SurvivorContainer.Changed -= PlayerModelChanged;
			GameManager.Instance.playerModel.Camp.Changed -= CampModelChanged;
			UIEvent.OnUIEvent -= OnUIEvent;
		}
		TrainingGrounds = null;
	}

	private void PlayerModelChanged(ModelObject m, string changed, object args)
	{
		if (changed == "addSurvivor")
		{
			UpdateList();
		}
		else if (changed == "survivorDemoted" && args is SurvivorModel)
		{
			SurvivorUpgradeHudNotificationData.Remove(args as SurvivorModel);
		}
	}

	private void OnUIEvent(string type, object parameter)
	{
		switch (type)
		{
		case "OnSurvivorUpgradeStarted":
		case "OnSurvivorUpgradeComplete":
		case "OnSurvivorInstantUpgraded":
			if (parameter is SurvivorModel)
			{
				SurvivorUpgradeHudNotificationData.Remove((SurvivorModel)parameter);
			}
			UpdateList();
			break;
		}
	}

	private void CampModelChanged(ModelObject m, string changed, object args)
	{
		switch (changed)
		{
		case "EventLevelUpBuilding":
		case "EventBuildingCollected":
		case "EventAddBuilding":
		case "EventUpgradeBuilding":
			UpdateList();
			break;
		}
	}

	private bool IsTrainingGroundsUpgrading()
	{
		if (TrainingGrounds == null)
		{
			TrainingGrounds = GameManager.Instance.playerModel.Camp.GetBuilding("TrainingGround");
		}
		if (TrainingGrounds == null)
		{
			return false;
		}
		return TrainingGrounds.IsUpgrading;
	}

	private SurvivorModel TrainingGroundsUpgradedUnseenModel()
	{
		if (TrainingGrounds == null)
		{
			TrainingGrounds = GameManager.Instance.playerModel.Camp.GetBuilding("TrainingGround");
		}
		if (TrainingGrounds != null && TrainingGrounds is ModelUpgraderBuildingModel)
		{
			return (TrainingGrounds as ModelUpgraderBuildingModel).UpgradedUnseenModel as SurvivorModel;
		}
		return null;
	}
}
