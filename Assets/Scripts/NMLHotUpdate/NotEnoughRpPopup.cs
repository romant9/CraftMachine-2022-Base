using UnityEngine;

public class NotEnoughRpPopup : ConfirmationPopup
{
	[SerializeField]
	private GameObject nextWarContainer;

	[SerializeField]
	private UILabel nextWarLabel;

	[SerializeField]
	private UILabel nextWarTimer;

	public override void Open()
	{
		base.Open();
		UpdateUI();
		SetCallbacks(OnClickOK);
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
		bool isGuildMember = GameManager.Instance.playerModel.IsGuildMember;
		bool flag = GuildWarHelper.IsSeasonOngoing();
		bool flag2 = GuildWarHelper.IsWarOngoing();
		bool flag3 = GameManager.Instance.gameEconomyData.FindNextGuildWarWithinSeason(GameManager.Instance.playerModel.UtcTimeStamp, GuildWarHelper.GetCurrentSeasonDefinitionId()) == null && !flag2;
		Helpers.GameObjectSetActive(nextWarContainer, value: false);
		string text = "";
		SetContent(info: (!isGuildMember) ? SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Popup.GuildShop.JoinGuildEarnMoreRP") : SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Popup.GuildShop.EarnMoreRP"), title: SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Popup.GuildShop.NotEnoughRP"));
		if (!flag2 && flag && !flag3)
		{
			Helpers.GameObjectSetActive(nextWarContainer, value: true);
			HelpersUI.SetContentToLabel(nextWarLabel, SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Popup.MissionHub.GuildWarStartsIn"));
			HelpersUI.SetContentToLabel(nextWarTimer, GuildWarHelper.GetFormatedTimeLeftToNextWar());
		}
		else if (!flag || flag3)
		{
			Helpers.GameObjectSetActive(nextWarContainer, value: true);
			HelpersUI.SetContentToLabel(nextWarLabel, SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Popup.MissionHub.GuildWarSeasonStartsIn"));
			HelpersUI.SetContentToLabel(nextWarTimer, GuildWarHelper.GetFormatedTimeLeftToNextSeason());
		}
	}

	private void OnClickOK()
	{
		bool isGuildMember = GameManager.Instance.playerModel.IsGuildMember;
		bool flag = GuildWarHelper.IsWarOngoing();
		if (!isGuildMember)
		{
			(SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.SocialPopupGuild) as SocialPopupGuild).OpenForTab(0);
		}
		else if (GuildWarHelper.IsLockedByCouncilLevelOrTutorial())
		{
			FeatureLockedPopup.Open(FeatureLockedPopup.FeatureType.GuildBattle, locked: true);
		}
		else if (flag)
		{
			SingularityMonoBehaviour<HUDManager>.Instance.CloseIfExists(UIType.SocialPopupGuild);
			if (!SingularityMonoBehaviour<HUDManager>.Instance.IsOpen(UIType.GuildBattleMapPopup))
			{
				MissionHubNavigation.OpenGuildBattleMap();
			}
		}
		else
		{
			SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.GuildBattleOverviewPopup).Open();
		}
	}
}
