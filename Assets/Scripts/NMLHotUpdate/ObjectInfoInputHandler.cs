using System.Collections.Generic;
using Client.Utils;
using TWDModel;
using UnityEngine;

public class ObjectInfoInputHandler : PlayerInputHandler
{
	private GameObject actorInfoPopup;

	private GameObject interactiveObjectInfoPopup;

	private GameObject exitLocation;

	private bool SurvivorAtMouse;

	private ActorView targetActorView;

	public override int Priority => -1;

	public override void Initialize()
	{
		base.Initialize();
		CreateInfoPopups();
	}

	public override void Reset()
	{
		base.Reset();
		ClearInfoPopups();
	}

	public override bool UpdateInteraction(float deltaTime)
	{
		if (base.PlayerInputManager.IsDragging)
		{
			return false;
		}
		return !SurvivorAtMouse;
	}

	public override bool CanHandleInteraction()
	{
		if (Helpers.IsCombatSkillSelectableStatus())
		{
			return false;
		}
		ClearInfoPopups();
		ActorModel actorModel = base.PlayerInputManager.GetActorAtMouseCoordinate();
		if (actorModel != null && actorModel.Faction == Faction.Survivor && !actorModel.TurnComplete)
		{
			actorModel = null;
		}
		bool flag = actorModel?.IsVisibleToSurvivors ?? false;
		InteractiveObjectModel interactiveObjectAtMouseCoordinate = base.PlayerInputManager.GetInteractiveObjectAtMouseCoordinate();
		CombatExitModel exitLocationAtMouse = base.PlayerInputManager.GetExitLocationAtMouse();
		bool flag2 = interactiveObjectAtMouseCoordinate != null && !interactiveObjectAtMouseCoordinate.InteractionDisabled;
		if (base.PlayerInputManager.PlayerSelectionEnabled)
		{
			if (!(flag || flag2))
			{
				if (actorModel == null)
				{
					return exitLocationAtMouse != null;
				}
				return false;
			}
			return true;
		}
		return false;
	}

	public override void InteractionStarted()
	{
		ActorModel survivorAtMouseCoordinate = base.PlayerInputManager.GetSurvivorAtMouseCoordinate();
		SurvivorAtMouse = false;
		if (survivorAtMouseCoordinate != null)
		{
			SurvivorAtMouse = true;
			TooltipManager.OpenTextBoxWithText((GameManager.Instance.GetViewForModel(survivorAtMouseCoordinate) as ActorView).gameObject, LocalizationManager.GetText("Tooltip.SurvivorUsedAllActions"), TooltipManager.Prefabs.TooltipCombatTextbox);
			return;
		}
		ActorModel actorAtMouseCoordinate = base.PlayerInputManager.GetActorAtMouseCoordinate();
		bool num = actorAtMouseCoordinate != null && actorAtMouseCoordinate.Faction != Faction.Survivor;
		InteractiveObjectModel interactiveObjectAtMouseCoordinate = base.PlayerInputManager.GetInteractiveObjectAtMouseCoordinate();
		CombatExitModel exitLocationAtMouse = base.PlayerInputManager.GetExitLocationAtMouse();
		if (num && actorAtMouseCoordinate.IsVisibleToSurvivors)
		{
			FixedVec3 position = base.Grid.GetPosition(actorAtMouseCoordinate.GridCoordinate);
			PlayerInputManager.Instance.GetHandler<CameraInputHandler>().FocusCameraOnTargetIfFarFromCenter(position.ToVector3());
			targetActorView = GameManager.Instance.GetViewForModel(actorAtMouseCoordinate) as ActorView;
			if (targetActorView != null)
			{
				TooltipManager.OpenTextBoxActorInfo(targetActorView.gameObject, actorAtMouseCoordinate);
			}
			else
			{
				Debug.LogWarning("ObjectInfoInputHandler: Could not show actor info - the popup or targetView is NULL");
			}
		}
		else if (interactiveObjectAtMouseCoordinate != null && actorAtMouseCoordinate == null && interactiveObjectAtMouseCoordinate.IsVisibleToSurvivors)
		{
			FixedVec3 position2 = base.Grid.GetPosition(base.PlayerInputManager.GetMouseGridCoordinate());
			PlayerInputManager.Instance.GetHandler<CameraInputHandler>().FocusCameraOnTargetIfFarFromCenter(position2.ToVector3());
			InteractiveObjectView interactiveObjectView = GameManager.Instance.GetViewForModel(interactiveObjectAtMouseCoordinate) as InteractiveObjectView;
			if (interactiveObjectInfoPopup != null && interactiveObjectView != null)
			{
				InteractiveObjectInfoPopup component = interactiveObjectInfoPopup.GetComponent<InteractiveObjectInfoPopup>();
				if (!(component != null))
				{
					return;
				}
				string iconName = "Ui_Icon_Info_Hand";
				string text = "";
				string description = "";
				OutpostObjectiveView outpostObjectiveView = interactiveObjectView.gameObject.GetComponentInChildren<OutpostObjectiveView>();
				if (outpostObjectiveView == null)
				{
					outpostObjectiveView = interactiveObjectView.gameObject.FindComponentInParents<OutpostObjectiveView>();
				}
				if (outpostObjectiveView != null)
				{
					text = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Tooltip.PvPObjectiveTitle");
					if (outpostObjectiveView.OutpostObjectiveType == OutpostObjectiveType.Flag)
					{
						iconName = "Ui_Icon_Flag";
						description = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Tooltip.PvPFlag{Parameter}", interactiveObjectAtMouseCoordinate.TurnsToComplete);
					}
					else if (outpostObjectiveView.OutpostObjectiveType == OutpostObjectiveType.ResourceContainer)
					{
						iconName = "Ui_Icon_Crate";
						description = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Tooltip.PvPLootBox{Parameter}", interactiveObjectAtMouseCoordinate.TurnsToComplete);
					}
				}
				else
				{
					text = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Tooltip.InteractiveObjectTitle");
					description = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Tooltip.InteractiveObject{Parameter}", interactiveObjectAtMouseCoordinate.TurnsToComplete);
				}
				component.SetText(iconName, text, description);
				component.FollowTarget(interactiveObjectView.gameObject);
				interactiveObjectInfoPopup.SetActive(value: true);
			}
			else
			{
				Debug.LogWarning("ObjectInfoInputHandler: Could not show interactive object info - the popup or objectView is NULL");
			}
		}
		else
		{
			if (exitLocationAtMouse == null)
			{
				return;
			}
			FixedVec3 position3 = base.Grid.GetPosition(base.PlayerInputManager.GetMouseGridCoordinate());
			PlayerInputManager.Instance.GetHandler<CameraInputHandler>().FocusCameraOnTargetIfFarFromCenter(position3.ToVector3());
			if (interactiveObjectInfoPopup != null)
			{
				InteractiveObjectInfoPopup component2 = interactiveObjectInfoPopup.GetComponent<InteractiveObjectInfoPopup>();
				if (component2 != null)
				{
					component2.SetText("Ui_Icon_Info_Glass", LocalizationManager.GetText("Tooltip.ExitZone.InteractiveObjectTitle"), LocalizationManager.GetText("Tooltip.ExitZone.InteractiveObjectDescription"));
					if (exitLocation == null)
					{
						exitLocation = new GameObject("exitLocationInfoLocation");
					}
					exitLocation.transform.position = position3.ToVector3();
					component2.FollowTarget(exitLocation);
					interactiveObjectInfoPopup.SetActive(value: true);
				}
			}
			else
			{
				Debug.LogWarning("ObjectInfoInputHandler: Could not show exit location info - the popup is NULL");
			}
		}
	}

	public override void InteractionStopped()
	{
		if (!(base.PlayerInputManager == null) && base.PlayerInputManager.ControlledActor != null && !base.PlayerInputManager.ControlledActor.TurnComplete)
		{
			PlayerInputManager.Instance.GetHandler<ActorMoveInputHandler>().OnControlledActorChanged(base.PlayerInputManager.ControlledActor);
		}
	}

	public void ClearInfoPopups()
	{
		targetActorView = null;
		if (interactiveObjectInfoPopup != null)
		{
			interactiveObjectInfoPopup.SetActive(value: false);
		}
		if (exitLocation != null)
		{
			Object.Destroy(exitLocation);
		}
		ResetEnemyVisibility();
	}

	private void CreateInfoPopups()
	{
		if (actorInfoPopup == null)
		{
			actorInfoPopup = CombatView.Instance.CombatHUD.CreateActorInfoPopup();
		}
		actorInfoPopup.SetActive(value: false);
		if (interactiveObjectInfoPopup == null)
		{
			interactiveObjectInfoPopup = CombatView.Instance.CombatHUD.CreateInteractiveObjectInfoPopup();
		}
		interactiveObjectInfoPopup.SetActive(value: false);
	}

	private void ResetEnemyVisibility()
	{
		if (base.Combat == null)
		{
			return;
		}
		List<ActorModel> enemyFactionsActors = base.Combat.GetEnemyFactionsActors(Faction.Survivor);
		if (enemyFactionsActors == null)
		{
			return;
		}
		for (int i = 0; i < enemyFactionsActors.Count; i++)
		{
			ActorModel actorModel = enemyFactionsActors[i];
			ActorView actorView = GameManager.Instance.GetViewForModel(actorModel) as ActorView;
			if (actorModel != null && actorView != null)
			{
				actorView.RefreshUI(updateAll: true);
			}
		}
	}
}
