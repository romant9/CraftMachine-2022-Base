using UnityEngine;

public class ApocalypticIntroductionPopup : HUDElement
{
	[SerializeField]
	private UILabel challengeLevelLabel;

	public override void Open()
	{
		base.Open();
		HelpersUI.SetContentToLabel(challengeLevelLabel, LocalizationManager.GetText("Challenge.Apocalyptic.Modeboard.Subtitle", GameManager.Instance.gameEconomyData.ConfigData.ChallengeApocalypticModeStartRound));
	}
}
