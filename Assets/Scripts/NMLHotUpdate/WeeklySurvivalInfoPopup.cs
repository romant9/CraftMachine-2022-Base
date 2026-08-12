using TWDModel;

public class WeeklySurvivalInfoPopup : HUDElement
{
	public static bool TryOpenOnSurvivalEnter()
	{
		if (!GameManager.Instance.Blackboard.IsToggleOn("NewSurvivalSeen"))
		{
			Helpers.ExecuteCommand(new FeatureUnlockedSeenCommand("NewSurvivalSeen"));
			return TryOpenFromClick();
		}
		return false;
	}

	public static bool TryOpenFromClick()
	{
		WeeklySurvivalInfoPopup weeklySurvivalInfoPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.WeeklySurvivalInfo) as WeeklySurvivalInfoPopup;
		if (weeklySurvivalInfoPopup != null)
		{
			weeklySurvivalInfoPopup.Open();
			return true;
		}
		return false;
	}
}
