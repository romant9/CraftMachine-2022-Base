public class HideTooltipsButton : UIButtonExtended
{
	protected override void OnClick()
	{
		base.OnClick();
		TooltipManager.HideAll();
	}
}
