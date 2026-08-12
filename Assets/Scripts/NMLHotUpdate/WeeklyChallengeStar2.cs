public class WeeklyChallengeStar2 : MonoBehaviourExtended
{
	private void OnEnable()
	{
		bool isNormalChallenge = WeeklyChallengeHelper.IsNormalChallenge;
		HelpersUI.SetSprite(base.transform.GetComponent<UISprite>(), isNormalChallenge ? "Ui_Mission_Marker_Star" : "Ui_Mission_Star_Large_Apocalyptic");
	}
}
