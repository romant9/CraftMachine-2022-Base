public class EndlessModeIntroductionPopup : HUDElement
{
	public override void OnClickClose()
	{
		FeatureUIHighlights.MarkHighlightExpired(FeatureUIHighlights.FeaturesIds.EndlessModeUnlocked);
		base.OnClickClose();
	}
}
