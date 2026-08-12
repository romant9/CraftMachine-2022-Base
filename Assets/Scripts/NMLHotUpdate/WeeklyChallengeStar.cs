public class WeeklyChallengeStar : MonoBehaviourExtended
{
	private void OnEnable()
	{
		bool isNormalChallenge = WeeklyChallengeHelper.IsNormalChallenge;
		HelpersUI.SetSprite(base.transform.GetComponent<UISprite>(), isNormalChallenge ? "Ui_Mission_Star_Large" : "Ui_Mission_Star_Large_Apocalyptic");
	}
}
