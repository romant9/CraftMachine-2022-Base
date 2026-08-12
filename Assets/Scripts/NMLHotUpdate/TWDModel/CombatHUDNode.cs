using System;

namespace TWDModel
{
	[Serializable]
	public class CombatHUDNode : NodeBase
	{
		public const string StatesChanged = "States";

		[GraphItVariable("Delayed execution will perform operations through visualization queue for proper timing.")]
		public bool DelayedExecution;

		public CombatHUDNode()
		{
		}

		public CombatHUDNode(CombatHUDNode node)
			: base(node)
		{
		}

		public override NodeBase RecordValue()
		{
			return new CombatHUDNode(this);
		}

		public override void Initialize()
		{
			base.Initialize();
			NotifyChange("States");
		}

		[GraphItInput("Show Objectives", "")]
		public void ShowObjectives()
		{
			base.manager.CombatModel.CombatHUDState.ShowObjectiveState = true;
			NotifyChange("States");
			ChangedHUD();
		}

		[GraphItInput("Hide Objectives", "")]
		public void HideObjectives()
		{
			base.manager.CombatModel.CombatHUDState.ShowObjectiveState = false;
			NotifyChange("States");
			ChangedHUD();
		}

		[GraphItInput("Show Charge", "")]
		public void ShowCharge()
		{
			base.manager.CombatModel.CombatHUDState.ShowChargeState = true;
			NotifyChange("States");
			ChangedHUD();
		}

		[GraphItInput("Hide Charge", "")]
		public void HideCharge()
		{
			base.manager.CombatModel.CombatHUDState.ShowChargeState = false;
			NotifyChange("States");
			ChangedHUD();
		}

		[GraphItInput("Show Flee", "")]
		public void ShowFlee()
		{
			base.manager.CombatModel.CombatHUDState.ShowFleeState = true;
			NotifyChange("States");
			ChangedHUD();
		}

		[GraphItInput("Hide Flee", "")]
		public void HideFlee()
		{
			base.manager.CombatModel.CombatHUDState.ShowFleeState = false;
			NotifyChange("States");
			ChangedHUD();
		}

		[GraphItInput("Show Skip Turn", "")]
		public void ShowSkipTurn()
		{
			base.manager.CombatModel.CombatHUDState.ShowSkipTurnState = true;
			NotifyChange("States");
			ChangedHUD();
		}

		[GraphItInput("Hide Skip Turn", "")]
		public void HideSkipTurn()
		{
			base.manager.CombatModel.CombatHUDState.ShowSkipTurnState = false;
			NotifyChange("States");
			ChangedHUD();
		}

		[GraphItInput("Show Threat", "")]
		public void ShowThreat()
		{
			base.manager.CombatModel.CombatHUDState.ShowThreatState = true;
			NotifyChange("States");
			ChangedHUD();
		}

		[GraphItInput("Hide Threat", "")]
		public void HideThreat()
		{
			base.manager.CombatModel.CombatHUDState.ShowThreatState = false;
			NotifyChange("States");
			ChangedHUD();
		}

		[GraphItInput("Show Keys", "")]
		public void ShowKeys()
		{
			base.manager.CombatModel.CombatHUDState.ShowKeysState = true;
			NotifyChange("States");
			ChangedHUD();
		}

		[GraphItInput("Hide Keys", "")]
		public void HideKeys()
		{
			base.manager.CombatModel.CombatHUDState.ShowKeysState = false;
			NotifyChange("States");
			ChangedHUD();
		}

		[GraphItInput("Show SpeedUp", "")]
		public void ShowSpeedUp()
		{
			base.manager.CombatModel.CombatHUDState.ShowSpeedUpState = true;
			NotifyChange("States");
			ChangedHUD();
		}

		[GraphItInput("Hide SpeedUp", "")]
		public void HideSpeedUp()
		{
			base.manager.CombatModel.CombatHUDState.ShowSpeedUpState = false;
			NotifyChange("States");
			ChangedHUD();
		}

		[GraphItOutput("Changed", "")]
		public void ChangedHUD()
		{
			Fire("Changed");
		}
	}
}
