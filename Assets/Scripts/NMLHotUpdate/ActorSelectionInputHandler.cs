using System.Collections.Generic;
using Client.Utils;
using TWDModel;

public class ActorSelectionInputHandler : PlayerInputHandler
{
	public override int Priority => 1000;

	public ActorSelectionInputHandler()
	{
		base.ClickThrough = true;
	}

	public override bool CanHandleInteraction()
	{
		if (Helpers.IsCombatSkillSelectableStatus())
		{
			return false;
		}
		ActorModel survivorAtMouseCoordinate = base.PlayerInputManager.GetSurvivorAtMouseCoordinate();
		if (base.TurnManager.CanSwitchActiveActor && base.PlayerInputManager.PlayerSelectionEnabled && survivorAtMouseCoordinate != null && !survivorAtMouseCoordinate.TurnComplete)
		{
			return survivorAtMouseCoordinate != base.TurnManager.ActiveActor;
		}
		return false;
	}

	public override void InteractionStarted()
	{
		ActorModel survivorAtMouseCoordinate = base.PlayerInputManager.GetSurvivorAtMouseCoordinate();
		if (survivorAtMouseCoordinate == null)
		{
			return;
		}
		if (base.TurnManager.ActiveActor != survivorAtMouseCoordinate)
		{
			Helpers.ExecuteCommand(new SetActiveActorCommand(survivorAtMouseCoordinate));
			if (base.TurnManager.ActiveActor == survivorAtMouseCoordinate)
			{
				if (SingularityMonoBehaviour<AudioManager>.Instance != null)
				{
					SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("combat_ui/select_survivor");
				}
				FixedVec3 position = base.Grid.GetPosition(survivorAtMouseCoordinate.GridCoordinate);
				PlayerInputManager.Instance.GetHandler<CameraInputHandler>().FocusCameraOnTargetIfFarFromCenter(position.ToVector3());
			}
		}
		CombatModel combat = GameManager.Instance.playerModel.Combat;
		CombatHUD combatHUD = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CombatHUD) as CombatHUD;
		if (combat == null || !(combatHUD != null))
		{
			return;
		}
		combatHUD.ConsumableUnselected();
		List<ActorModel> factionActors = combat.GetFactionActors(Faction.Survivor);
		for (int i = 0; i < factionActors.Count; i++)
		{
			if (factionActors[i].ChargeMeter.ChargeEnabled)
			{
				Helpers.ExecuteCommand(new EnableChargeCommand(factionActors[i], enabled: false));
				combatHUD.UnequipChargeEquipment(factionActors[i]);
			}
		}
	}
}
