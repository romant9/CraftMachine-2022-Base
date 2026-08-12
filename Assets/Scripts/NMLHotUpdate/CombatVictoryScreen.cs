using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class CombatVictoryScreen : HUDElement
{
	private class FlowScreenState
	{
		public enum FlowState
		{
			DisplayingVictory = 0,
			DisplayingDefeat = 1,
			DisplayingTeamStatus = 2,
			DisplayingResources = 3,
			DisplayingRescuedSurvivors = 4,
			DisplayingMainLootEquipmentAndSurvivors = 5,
			Idle = 6,
			Invalid = 7
		}

		public FlowState State;

		public CombatEndFlowStep FlowStep;

		public bool ShowWalkerBackground;

		public FlowScreenState(FlowState state, CombatEndFlowStep step, bool showWalkerBackground = false)
		{
			State = state;
			FlowStep = step;
			ShowWalkerBackground = showWalkerBackground;
		}
	}

	private List<FlowScreenState> steps = new List<FlowScreenState>();

	public void OnReturnToCampButtonButton(GameObject button)
	{
		if (OfflineManager.IsLoadDataManager)
		{
			DebugTWD.LogMycode("if (OfflineManager.IsLoadDataManager)");
			DebugTWD.Log("OnReturnToCampButtonButton ", DebugType.OnClick);
			OfflineManager.Instance.IsReturnToResidence = true;
		}
		GameManager.Instance.ReturnFromVisit();
	}

	public void Setup(List<SurvivorModel> deployedTeam, List<ActorModel> rescuedSurvivors, int numLootCollected)
	{
		bool hasPvPRules = GameManager.Instance.playerModel.Combat.HasPvPRules;
		bool isSurvivalMission = GameManager.Instance.playerModel.Combat.IsSurvivalMission;
		bool isGuildBattleMission = GameManager.Instance.playerModel.Combat.IsGuildBattleMission;
		steps.Clear();
		if (rescuedSurvivors != null && rescuedSurvivors.Count > 0)
		{
			ActorView.PrepareActor(rescuedSurvivors[0]);
			CombatEndFlowRescuedSurvivor combatEndFlowRescuedSurvivor = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CombatEndFlowRescuedSurvivor) as CombatEndFlowRescuedSurvivor;
			combatEndFlowRescuedSurvivor.RescuedSurvivors = rescuedSurvivors;
			combatEndFlowRescuedSurvivor.ReturnToCampAllowed = false;
			combatEndFlowRescuedSurvivor.ShowNextButton = false;
			combatEndFlowRescuedSurvivor.VictoryScreen = this;
			steps.Add(new FlowScreenState(FlowScreenState.FlowState.DisplayingRescuedSurvivors, combatEndFlowRescuedSurvivor));
		}
		else if (GameManager.Instance.playerModel.Combat.MissionType != MissionType.Rescue && !hasPvPRules && !isSurvivalMission && !isGuildBattleMission)
		{
			List<LootEntry> openedRewardBoxes = RewardScreenHandler.Instance.GetOpenedRewardBoxes();
			if (openedRewardBoxes != null && openedRewardBoxes.Count > 0)
			{
				steps.Clear();
			}
			CombatEndFlowThreeByThree combatEndFlowThreeByThree = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CombatEndFlowThreeByThree) as CombatEndFlowThreeByThree;
			combatEndFlowThreeByThree.ReturnToCampAllowed = false;
			combatEndFlowThreeByThree.ShowNextButton = false;
			steps.Add(new FlowScreenState(FlowScreenState.FlowState.DisplayingMainLootEquipmentAndSurvivors, combatEndFlowThreeByThree));
		}
		AddListeners();
	}

	private void AddListeners()
	{
		foreach (FlowScreenState step in steps)
		{
			step.FlowStep.OnFlowStepEnd += OnFlowStepEnded;
		}
	}

	public override void Start()
	{
		startCurrentStep();
	}

	private void OnDisable()
	{
		foreach (FlowScreenState step in steps)
		{
			step.FlowStep.OnFlowStepEnd -= OnFlowStepEnded;
		}
	}

	private void startCurrentStep()
	{
		if (steps != null && steps.Count > 0)
		{
			steps[0].FlowStep.Open();
		}
		else
		{
			OnReturnToCampButtonButton(null);
		}
	}

	private void OnFlowStepEnded(CombatEndFlowStep step)
	{
		if (step != null && steps.Count > 0)
		{
			steps.RemoveAt(0);
		}
		if (step != null && steps.Count > 0)
		{
			startCurrentStep();
		}
	}
}
