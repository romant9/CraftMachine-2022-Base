using TWDModel;

public class MagazineAreaInputHandler : PlayerInputHandler
{
	public override int Priority => 1;

	public override bool TapOnly => true;

	public override bool CanHandleInteraction()
	{
		if (Helpers.IsCombatSkillSelectableStatus())
		{
			return false;
		}
		MagazineArea magazineAreaAtMouseCoordinate = base.PlayerInputManager.GetMagazineAreaAtMouseCoordinate();
		if (magazineAreaAtMouseCoordinate == null || magazineAreaAtMouseCoordinate.Faction != Faction.Survivor)
		{
			return false;
		}
		if (base.PlayerInputManager.PlayerSelectionEnabled)
		{
			return base.PlayerInputManager.GetActorAtMouseCoordinate() == null;
		}
		return false;
	}

	public override void InteractionStarted()
	{
		MagazineArea magazineAreaAtMouseCoordinate = base.PlayerInputManager.GetMagazineAreaAtMouseCoordinate();
		if (magazineAreaAtMouseCoordinate != null && !(CombatView.Instance == null))
		{
			CombatView.Instance.ShowMagazineAreaTooltip(magazineAreaAtMouseCoordinate.EffectiveAreaGridCoordinate);
		}
	}
}
