using TWDModel;
using UnityEngine;

public class WeeklyChallengeMasterMissionInfo : HUDElement
{
	[SerializeField]
	private UILabel councilLevelLabel;

	private void Awake()
	{
		HelpersUI.SetContentToLabel(councilLevelLabel, LocalizationManager.GetText("Popup.Challenge.RoundSkip.Info.UnlockAtLevel{CouncilLevel}", GameManager.Instance.gameEconomyData.ConfigData.ChallangeMasterMissionCouncilLevelUnlock));
	}

	public static void TryOpenOnChallengeEnter()
	{
		if (!GameManager.Instance.playerModel.IsMasterMissionUnlocked || GameManager.Instance.Blackboard.IsToggleOn("NewChallengeMasterMissionSeen"))
		{
			return;
		}
		bool num = SingularityMonoBehaviour<HUDManager>.Instance.GetNoCreation(UIType.WeeklyChallengeStartSkipping) != null && SingularityMonoBehaviour<HUDManager>.Instance.GetNoCreation(UIType.WeeklyChallengeStartSkipping).IsOpen;
		bool flag = SingularityMonoBehaviour<HUDManager>.Instance.GetNoCreation(UIType.WeeklyChallengeInfo) != null && SingularityMonoBehaviour<HUDManager>.Instance.GetNoCreation(UIType.WeeklyChallengeInfo).IsOpen;
		if (!num && !flag)
		{
			WeeklyChallengeMasterMissionInfo weeklyChallengeMasterMissionInfo = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.WeeklyChallengeMasterMissionInfo) as WeeklyChallengeMasterMissionInfo;
			if (weeklyChallengeMasterMissionInfo != null)
			{
				weeklyChallengeMasterMissionInfo.Open();
				weeklyChallengeMasterMissionInfo.OnClose += PlightIntroductionPopup.OnDependentWindowClosed;
				Helpers.ExecuteCommand(new FeatureUnlockedSeenCommand("NewChallengeMasterMissionSeen"));
			}
		}
	}

	public static void OnDependentWindowClosed(HUDElement element, HUDElementConfig hudElementConfig)
	{
		TryOpenOnChallengeEnter();
	}
}
