using Client.Utils;
using TWDModel;

public class CommandSkillGridSelectInputHandler : PlayerInputHandler
{
	public override int Priority => 3100;

	public CommandSkillGridSelectInputHandler()
	{
		base.ClickThrough = false;
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
		if (curBaseCommandSkill?.Definition?.TargetType == null || !curBaseCommandSkill.Definition.TargetType.Contains(CommandSkillTargetType.Grid))
		{
			return false;
		}
		GridCoordinate mouseGridCoordinate = PlayerInputManager.Instance.GetMouseGridCoordinate();
		if (base.TurnManager.CanSwitchActiveActor)
		{
			return mouseGridCoordinate.IsValid;
		}
		return false;
	}

	public override void InteractionStarted()
	{
		if (!Helpers.IsCombatSkillSelectableStatus())
		{
			return;
		}
		CombatHUD combatHUD = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CombatHUD) as CombatHUD;
		CombatModel combat = GameManager.Instance.playerModel.Combat;
		if (combatHUD == null || combat == null || !combatHUD.CanSelectSkill)
		{
			return;
		}
		GridCoordinate mouseGridCoordinate = PlayerInputManager.Instance.GetMouseGridCoordinate();
		if (mouseGridCoordinate.IsValid)
		{
			BaseCommandSkill curBaseCommandSkill = combatHUD.GetCurBaseCommandSkill();
			GridCoordinate sourceCell = CommandSkillGridHelpers.GetSourceCell(curBaseCommandSkill, combat.ActiveActor);
			if (!CommandSkillGridHelpers.IsGridCellOnPlayableMap(combat, mouseGridCoordinate))
			{
				combatHUD.RefreshCommandSkillGridHighlightsIfActive();
				return;
			}
			if (!CommandSkillGridHelpers.IsGridCellVisibleFrom(combat, sourceCell, mouseGridCoordinate))
			{
				combatHUD.RefreshCommandSkillGridHighlightsIfActive();
				return;
			}
			if (!curBaseCommandSkill.CanExecute(mouseGridCoordinate))
			{
				combatHUD.RefreshCommandSkillGridHighlightsIfActive();
				return;
			}
			FixedVec3 position = base.Grid.GetPosition(mouseGridCoordinate);
			PlayerInputManager.Instance.GetHandler<CameraInputHandler>().FocusCameraOnTargetIfFarFromCenter(position.ToVector3());
			combatHUD.SetActiveSkillGridCell(mouseGridCoordinate);
		}
	}
}
