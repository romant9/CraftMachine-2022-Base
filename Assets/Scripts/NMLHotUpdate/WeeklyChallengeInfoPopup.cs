using TWDModel;

public class WeeklyChallengeInfoPopup : HUDElement
{
	public static bool TryOpenOnChallengeEnter()
	{
		if (!GameManager.Instance.Blackboard.IsToggleOn("NewChallengesSeen"))
		{
			Helpers.ExecuteCommand(new FeatureUnlockedSeenCommand("NewChallengesSeen"));
			return TryOpenFromClick();
		}
		return false;
	}

	public static bool TryOpenFromClick()
	{
		HUDElement hUDElement = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.WeeklyChallengeInfo);
		if (hUDElement != null)
		{
			hUDElement.Open();
			hUDElement.OnClose += WeeklyChallengeMasterMissionInfo.OnDependentWindowClosed;
			hUDElement.OnClose += PlightIntroductionPopup.OnDependentWindowClosed;
			return true;
		}
		return false;
	}
}
