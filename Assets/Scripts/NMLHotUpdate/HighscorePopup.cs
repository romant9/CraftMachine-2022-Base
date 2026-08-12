using UnityEngine;

public class HighscorePopup : HUDElement
{
	[SerializeField]
	private UILabel challengeLabel;

	[SerializeField]
	private GameObject tab2;

	[SerializeField]
	private GameObject tab3;

	public override void Open()
	{
		base.Open();
		if (challengeLabel != null)
		{
			HelpersUI.SetContentToLabel(challengeLabel, WeeklyChallengeHelper.IsNormalChallenge ? LocalizationManager.GetText("Popup.Challenge.Tab.RecentHighScores") : LocalizationManager.GetText("Popup.Apocalyptic.Challenge.Tab.RecentHighScores"));
		}
		Helpers.GameObjectSetActive(tab2, WeeklyChallengeHelper.IsNormalChallenge);
		Helpers.GameObjectSetActive(tab3, WeeklyChallengeHelper.IsNormalChallenge);
	}
}
