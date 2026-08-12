using System;
using BaseModel;
using TWDModel;
using UnityEngine;

public class CombatHUDNodeWrapper : NodeBaseWrapper
{
	[HideInInspector]
	public CombatHUDNode NodeBaseInternal = new CombatHUDNode();

	private CombatHUD CombatHUD;

	private CombatHUDNode CombatHUDNodeRef => NodeBase as CombatHUDNode;

	public override void OnNodeBind()
	{
		CombatHUD = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CombatHUD) as CombatHUD;
		CombatHUDNodeRef.Changed += OnChanged;
		UpdateState();
	}

	private void OnChanged(ModelObject m, string changed, object args)
	{
		UpdateState();
	}

	private void UpdateState()
	{
		PerformAction(delegate
		{
			CombatHUD.ShowCharge(CombatHUDNodeRef.manager.CombatModel.CombatHUDState.ShowChargeState, tutorial: true);
		});
		PerformAction(delegate
		{
			CombatHUD.ShowFlee(CombatHUDNodeRef.manager.CombatModel.CombatHUDState.ShowFleeState, tutorial: true);
		});
		PerformAction(delegate
		{
			CombatHUD.ShowKeys(CombatHUDNodeRef.manager.CombatModel.CombatHUDState.ShowKeysState, tutorial: true);
		});
		PerformAction(delegate
		{
			CombatHUD.ShowObjectives(CombatHUDNodeRef.manager.CombatModel.CombatHUDState.ShowObjectiveState, tutorial: true);
		});
		PerformAction(delegate
		{
			CombatHUD.ShowSkipTurn(CombatHUDNodeRef.manager.CombatModel.CombatHUDState.ShowSkipTurnState, tutorial: true);
		});
		PerformAction(delegate
		{
			CombatHUD.ShowSpeedUp(CombatHUDNodeRef.manager.CombatModel.CombatHUDState.ShowSpeedUpState, tutorial: true);
		});
		PerformAction(delegate
		{
			CombatHUD.ShowThreatTurnCount(CombatHUDNodeRef.manager.CombatModel.CombatHUDState.ShowThreatState, tutorial: true);
		});
	}

	private void PerformAction(Action action)
	{
		if (CombatHUDNodeRef.DelayedExecution)
		{
			VisualizationQueue.Instance.Add(new DelayedNotificationVisualizationTask(null, delegate
			{
				action();
			}));
		}
		else
		{
			action();
		}
	}
}
