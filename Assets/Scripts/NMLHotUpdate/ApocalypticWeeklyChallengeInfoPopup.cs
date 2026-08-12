public class ApocalypticWeeklyChallengeInfoPopup : HUDElement
{
	public static bool TryOpenFromClick()
	{
		ApocalypticWeeklyChallengeInfoPopup apocalypticWeeklyChallengeInfoPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.ApocalypticWeeklyChallengeInfoPopup) as ApocalypticWeeklyChallengeInfoPopup;
		if (apocalypticWeeklyChallengeInfoPopup != null)
		{
			apocalypticWeeklyChallengeInfoPopup.Open();
			return true;
		}
		return false;
	}
}
