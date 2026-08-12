public class SurvivorInfoRightSidePanel : UIButtonToggleSet
{
	public const int TabsIndexTrait = 0;

	public const int TabsIndexBadges = 1;

	public const int TabsIndexSurvivalManual = 2;

	public const int currentPosIndex = 0;

	public override void OnEnable()
	{
		base.OnEnable();
	}

	public override void SetActiveButtons(bool value)
	{
		base.SetActiveButtons(value);
		Helpers.GameObjectSetActive(base.GetUIButtonToggleList[2], value && Helpers.IsActorSheetSurvivalManualOpen());
	}
}
