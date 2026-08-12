using System;
using BaseModel;
using TWDModel;
using UnityEngine;

public class CombatSupportsUIView : ModelView<CombatSupportManager>
{
	private enum Instruction
	{
		None = 0,
		Regular = 1,
		Targeted = 2,
		Failed = 3
	}

	[SerializeField]
	private CombatSupportCard[] combatSupportViews;

	[SerializeField]
	private AnimatedTextBehaviour instructionLabel;

	private SupportInteractionManager supportInteractionManager;

	private IDisposable instructionTimeout;

	private void Start()
	{
		Initialize(GameManager.Instance.modelManager.CombatModel.SupportManager);
	}

	public override void Initialize(ModelObject model)
	{
		base.Initialize(model);
		supportInteractionManager = PlayerInputManager.Instance.GetHandler<SupportInputHandler>().SupportInteractionManager;
		TurnManager turnManager = base.Model.manager.CombatModel.TurnManager;
		turnManager.FactionChanged += OnFactionChanged;
		turnManager.ActorChanged += TurnManagerOnActorChanged;
		if (supportInteractionManager != null)
		{
			supportInteractionManager.SupportDeactivated += OnSupportInteraction;
			supportInteractionManager.SupportExecuted += OnSupportInteraction;
			supportInteractionManager.SupportExecutionFailed += OnSupportExecutionFailed;
		}
		for (int i = 0; i < combatSupportViews.Length; i++)
		{
			CombatSupportCard obj = combatSupportViews[i];
			obj.SupportCancelClicked += OnCancelClick;
			obj.Initialize(i);
		}
		RefreshButtonStates();
	}

	private void OnDestroy()
	{
		TurnManager turnManager = base.Model?.manager?.CombatModel?.TurnManager;
		if (turnManager != null)
		{
			turnManager.FactionChanged -= OnFactionChanged;
			turnManager.ActorChanged -= TurnManagerOnActorChanged;
		}
		if (supportInteractionManager != null)
		{
			supportInteractionManager.SupportDeactivated -= OnSupportInteraction;
			supportInteractionManager.SupportExecuted -= OnSupportInteraction;
		}
	}

	private void OnFactionChanged(Faction currentFaction, Faction newFaction)
	{
		if (newFaction == Faction.Survivor)
		{
			RefreshButtonStates();
		}
	}

	private void TurnManagerOnActorChanged(ActorModel actor)
	{
		RefreshButtonStates();
	}

	public void OnSupportClick(int actorSlotIndex)
	{
		int supportIndexBy_actorSlotIndex = supportInteractionManager.GetSupportIndexBy_actorSlotIndex(actorSlotIndex);
		ISupportInteraction activeSupportInteraction = supportInteractionManager.ActiveSupportInteraction;
		if (activeSupportInteraction != null && activeSupportInteraction.EquipIndex == supportIndexBy_actorSlotIndex)
		{
			if (activeSupportInteraction.Targeted)
			{
				supportInteractionManager.Deactivate();
			}
			else
			{
				supportInteractionManager.Execute();
			}
		}
		else if (supportInteractionManager.Activate(supportIndexBy_actorSlotIndex))
		{
			RefreshButtonStates();
		}
	}

	private void OnSupportInteraction(int equipIndex)
	{
		RefreshButtonStates();
	}

	private void OnSupportExecutionFailed(int equipIndex, SupportTargetsMessage targetsMessage)
	{
		ShowInstruction(Instruction.Failed, targetsMessage);
	}

	public void OnCancelClick()
	{
		supportInteractionManager.Deactivate();
	}

	private void RefreshButtonStates()
	{
		ISupportInteraction supportInteraction = supportInteractionManager?.ActiveSupportInteraction;
		int num = supportInteraction?.EquipIndex ?? (-1);
		Instruction tooltip = Instruction.None;
		_ = base.Model.manager.CombatModel;
		for (int i = 0; i < base.Model.Supports.Count; i++)
		{
			CombatSupportModel combatSupportModel = base.Model.Supports[i];
			_ = combatSupportViews[combatSupportModel.SlotIndex];
			SurvivorModel attachedSurvivor = combatSupportModel.AttachedSurvivor;
			if (attachedSurvivor != null && !attachedSurvivor.IsDead)
			{
				CombatSupportCard.ActivationState activationState = ((combatSupportModel.SlotIndex == num) ? ((supportInteraction == null || !supportInteraction.Targeted) ? CombatSupportCard.ActivationState.Regular : CombatSupportCard.ActivationState.Targeted) : CombatSupportCard.ActivationState.Inactive);
				switch (activationState)
				{
				case CombatSupportCard.ActivationState.Regular:
					tooltip = Instruction.Regular;
					break;
				case CombatSupportCard.ActivationState.Targeted:
					tooltip = Instruction.Targeted;
					break;
				}
				if (activationState != CombatSupportCard.ActivationState.Inactive && base.Model.GetAvailability(combatSupportModel) != CombatSupportAvailability.Executable)
				{
					supportInteractionManager.Deactivate();
					tooltip = Instruction.None;
				}
			}
		}
		for (int j = 0; j < combatSupportViews.Length; j++)
		{
			combatSupportViews[j].UpdateUI();
		}
		ShowInstruction(tooltip);
	}

	private void ShowInstruction(Instruction tooltip, SupportTargetsMessage targetsMessage = SupportTargetsMessage.NoTargetsInRange)
	{
		TimeSpan? timeSpan = null;
		string textId;
		switch (tooltip)
		{
		default:
			return;
		case Instruction.None:
			if (instructionTimeout == null)
			{
				instructionLabel.Hide();
			}
			return;
		case Instruction.Regular:
			textId = "Support.Tooltip.RegularActivation";
			break;
		case Instruction.Targeted:
			textId = "Support.Tooltip.TargetedActivation";
			break;
		case Instruction.Failed:
			textId = ((targetsMessage != SupportTargetsMessage.NoTargets) ? "Support.Tooltip.ExecutionFailed" : "Support.Tooltip.NotApplicable");
			timeSpan = TimeSpan.FromSeconds(3.0);
			break;
		}
		instructionTimeout?.Dispose();
		instructionTimeout = null;
		instructionLabel.Show(SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText(textId));
		if (timeSpan.HasValue)
		{
			instructionTimeout = GameManager.Instance.TimingManager.Timer(timeSpan.Value, delegate
			{
				instructionLabel.Hide();
			});
		}
	}

	public void UpdateUI()
	{
		RefreshButtonStates();
	}
}
