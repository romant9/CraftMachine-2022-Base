using TWDModel;
using UnityEngine;

public class PlightIntroductionPopup : HUDElement
{
	[SerializeField]
	private UILabel challengeLevelLabel;

	[SerializeField]
	private UILabel challengeDescriptionLabel;

	private void Awake()
	{
		HelpersUI.SetContentToLabel(challengeLevelLabel, LocalizationManager.GetText("Challenge.Debuff.Modeboard.Subtitle", GameManager.Instance.gameEconomyData.ConfigData.ChallengDebuffStartsRound));
		HelpersUI.SetContentToLabel(challengeDescriptionLabel, LocalizationManager.GetText("Challenge.Debuff.Modeboard.Description", GameManager.Instance.gameEconomyData.ConfigData.ChallengDebuffStartsRound));
	}

	public static void TryOpenOnChallengeEnter()
	{
		if (GameManager.Instance.Blackboard.IsToggleOn("NewChallengePlightSeen"))
		{
			return;
		}
		bool num = SingularityMonoBehaviour<HUDManager>.Instance.GetNoCreation(UIType.WeeklyChallengeMasterMissionInfo) != null && SingularityMonoBehaviour<HUDManager>.Instance.GetNoCreation(UIType.WeeklyChallengeMasterMissionInfo).IsOpen;
		bool flag = SingularityMonoBehaviour<HUDManager>.Instance.GetNoCreation(UIType.WeeklyChallengeInfo) != null && SingularityMonoBehaviour<HUDManager>.Instance.GetNoCreation(UIType.WeeklyChallengeInfo).IsOpen;
		if (!num && !flag)
		{
			PlightIntroductionPopup plightIntroductionPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.PlightIntroductionPopup) as PlightIntroductionPopup;
			if (plightIntroductionPopup != null)
			{
				plightIntroductionPopup.Open();
				Helpers.ExecuteCommand(new FeatureUnlockedSeenCommand("NewChallengePlightSeen"));
			}
		}
	}

	public static void OnDependentWindowClosed(HUDElement element, HUDElementConfig hudElementConfig)
	{
		TryOpenOnChallengeEnter();
	}
}
