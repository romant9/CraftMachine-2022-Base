using BaseModel;
using TWDModel;
using UnityEngine;

public class GuildChallengeMenu : UIToggleContent
{
	[Header("Guild Info")]
	[SerializeField]
	private UILabel guildNameLabel;

	[SerializeField]
	private UILabel joinTypeLabel;

	[Header("Challenge Stats")]
	[SerializeField]
	private UILabel numberPlayedLabel;

	[SerializeField]
	private UILabel currentChallengeStarsLabel;

	[SerializeField]
	private UILabel allTimeStarsLabel;

	private void OnEnable()
	{
		Setup();
	}

	public void OnClickChallenge()
	{
		PlayerModel playerModel = GameManager.Instance.playerModel;
		if (playerModel == null || playerModel.IsGuildMember)
		{
			CampHUD.TryToAccessChallenges(OpenChallengeMap);
		}
	}

	private void OpenChallengeMap()
	{
		if (MissionHubNavigation.CanAccessChallengeMap(out var mapMissionGroupModel))
		{
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("camp/quest_accept");
			DetailMapPopUp detailMapPopUp = SingularityMonoBehaviour<HUDManager>.Instance.GetNoCreation(UIType.DetailMapPopUp) as DetailMapPopUp;
			if (detailMapPopUp != null && detailMapPopUp.MapCategory == MapCategory.Challenge)
			{
				SingularityMonoBehaviour<HUDManager>.Instance.CloseIfExists(UIType.SocialPopupGuild);
				detailMapPopUp.Open();
				detailMapPopUp.LoadChallengeMap();
			}
			else
			{
				CampManager.Instance.GoToMap(mapMissionGroupModel);
			}
		}
	}

	private void Setup()
	{
		if (GameManager.Instance.playerModel.IsGuildMember)
		{
			SetupMember();
		}
	}

	public void OnClickSettings()
	{
		GuildModel guildModel = GameManager.Instance.playerModel.GuildModel;
		if (guildModel != null)
		{
			GuildModelWrapper model = new GuildModelWrapper(guildModel);
			GuildInfoPopup obj = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.SocialGuildInfoPopup) as GuildInfoPopup;
			obj.GuildInfoPopupType = GuildInfoPopup.GuildPopupType.OwnGuild;
			obj.OpenForModel(model);
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_click");
		}
	}

	private void SetupMember()
	{
		GuildModel guildModel = GameManager.Instance.playerModel.GuildModel;
		HelpersUI.SetContentToLabel(guildNameLabel, GameManager.Instance.GetFilteredText(guildModel.Name));
		HelpersUI.SetContentToLabel(joinTypeLabel, LocalizationManager.GetText("Generic.Guild.JoinType." + guildModel.JoinType));
		HelpersUI.SetContentToLabel(numberPlayedLabel, guildModel.NumberChallengeStarted.ToString());
		HelpersUI.SetContentToLabel(currentChallengeStarsLabel, guildModel.CurrentChallengeStars.ToString());
		HelpersUI.SetContentToLabel(allTimeStarsLabel, guildModel.TotalChallengeStars.ToString());
	}

	public void OnClickHighscores()
	{
		HUDElement hUDElement = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.HighscorePopup);
		if (hUDElement != null)
		{
			hUDElement.Open();
		}
	}

	private void OnGuildChanged(GroupModelBase model, string changed, object args)
	{
		Setup();
	}
}
