using Client.Utils;
using TWDModel;

public class ActorSelectionSkillSelectInputHandler : PlayerInputHandler
{
	public override int Priority => 3000;

	public ActorSelectionSkillSelectInputHandler()
	{
		base.ClickThrough = true;
	}

	public override bool CanHandleInteraction()
	{
		if (!Helpers.IsCombatSkillSelectableStatus())
		{
			return false;
		}
		CombatHUD combatHUD = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CombatHUD) as CombatHUD;
		if (combatHUD == null)
		{
			return false;
		}
		BaseCommandSkill curBaseCommandSkill = combatHUD.GetCurBaseCommandSkill();
		if (curBaseCommandSkill?.Definition?.TargetType == null)
		{
			return false;
		}
		if (!curBaseCommandSkill.Definition.TargetType.Contains(CommandSkillTargetType.Enemy) && !curBaseCommandSkill.Definition.TargetType.Contains(CommandSkillTargetType.Friendly) && !curBaseCommandSkill.Definition.TargetType.Contains(CommandSkillTargetType.ActorItself))
		{
			return false;
		}
		ActorModel actorAtMouseCoordinate = PlayerInputManager.Instance.GetActorAtMouseCoordinate();
		if (base.TurnManager.CanSwitchActiveActor)
		{
			return actorAtMouseCoordinate != null;
		}
		return false;
	}

	public override void InteractionStarted()
	{
		if (!Helpers.IsCombatSkillSelectableStatus())
		{
			return;
		}
		ActorModel actorAtMouseCoordinate = PlayerInputManager.Instance.GetActorAtMouseCoordinate();
		if (actorAtMouseCoordinate != null && (actorAtMouseCoordinate.Faction == Faction.Walker || actorAtMouseCoordinate.Faction == Faction.Survivor || actorAtMouseCoordinate.Faction == Faction.Raider))
		{
			CombatModel combat = GameManager.Instance.playerModel.Combat;
			CombatHUD combatHUD = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CombatHUD) as CombatHUD;
			if (combat != null && !(combatHUD == null) && combatHUD.CanSelectSkill && combat.IsGridCellVisibleByAnySurvivor(actorAtMouseCoordinate.GridCoordinate))
			{
				FixedVec3 position = base.Grid.GetPosition(actorAtMouseCoordinate.GridCoordinate);
				PlayerInputManager.Instance.GetHandler<CameraInputHandler>().FocusCameraOnTargetIfFarFromCenter(position.ToVector3());
				combatHUD.SetActiveSkillActor(actorAtMouseCoordinate);
			}
		}
	}
}
