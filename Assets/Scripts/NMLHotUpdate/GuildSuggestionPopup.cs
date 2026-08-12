using BaseModel;
using Client.Connectivity;
using TWDModel;
using UnityEngine;

public class GuildSuggestionPopup : HUDElement
{
	[Header("Buttons")]
	[SerializeField]
	private UIButtonExtended JoinGuildButton;

	[SerializeField]
	private UIButtonExtended MoreGuildsButton;

	[SerializeField]
	private UIButtonExtended CloseButton;

	[Header("Guild Info")]
	[SerializeField]
	private UILabel NameLabel;

	[SerializeField]
	private UILabel DescriptionLabel;

	[SerializeField]
	private UILabel PurposeLabel;

	[SerializeField]
	private UILabel MemberCountLabel;

	[SerializeField]
	private UILabel LastChallengeStarsLabel;

	[SerializeField]
	private UILabel CurrentSeasonVP;

	private GuildModel guild;

	public override void Open()
	{
		base.Open();
		if (JoinGuildButton != null)
		{
			JoinGuildButton.SetClickCallback(OnJoinGuildClicked);
		}
		if (MoreGuildsButton != null)
		{
			MoreGuildsButton.SetClickCallback(OnMoreGuildsClicked);
		}
		if (CloseButton != null)
		{
			CloseButton.SetClickCallback(OnCloseButtonClicked);
		}
	}

	public override void Close()
	{
		base.Close();
		if (JoinGuildButton != null)
		{
			JoinGuildButton.Clear();
		}
		if (MoreGuildsButton != null)
		{
			MoreGuildsButton.Clear();
		}
		if (CloseButton != null)
		{
			CloseButton.Clear();
		}
	}

	public override void Update()
	{
		base.Update();
	}

	public override void UpdateUI()
	{
		if (guild != null)
		{
			string text = guild.Purpose;
			if (text == null)
			{
				text = GuildModel.GetDefaultPurpose(GameManager.Instance.gameEconomyData.ConfigData.GuildPurposeTypes);
			}
			HelpersUI.SetContentToLabel(NameLabel, GameManager.Instance.GetFilteredText(guild.Name));
			string filteredText = GameManager.Instance.GetFilteredText(guild.Description);
			HelpersUI.SetContentToLabel(DescriptionLabel, string.IsNullOrEmpty(filteredText) ? SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Generic.Guild.NoDescription") : filteredText);
			HelpersUI.SetContentToLabel(PurposeLabel, HelpersLocalization.GetGuildPurpose(text));
			HelpersUI.SetContentToLabel(MemberCountLabel, guild.NumberMembers + "/" + 20);
			HelpersUI.SetContentToLabel(LastChallengeStarsLabel, guild.PreviousChallengeStars.ToString());
			HelpersUI.SetContentToLabel(CurrentSeasonVP, guild.GuildInfoCurrentVP.ToString());
		}
	}

	public static void OpenForGuildId(string guildId)
	{
		SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.IngameLoading).Open();
		SignalRClient.Instance.RequestCommand("GetGroupInfo", guildId, OnGuildReceived, waitForResponse: true);
	}

	private static void OnGuildReceived(string message)
	{
		SingularityMonoBehaviour<HUDManager>.Instance.CloseIfExists(UIType.IngameLoading);
		bool flag = false;
		GuildModel guildModel = null;
		if (string.IsNullOrEmpty(message) || message == "null")
		{
			flag = true;
		}
		else
		{
			guildModel = GameManager.Instance.modelManager.GetMessageSerializer().DeserializeObject<GuildModel>(message);
			if (guildModel == null)
			{
				flag = true;
			}
		}
		if (flag)
		{
			AlertPopup.ShowPopupGetText("Error.Error", "Generic.Guild.NoGuildFound", "Button.Ok", null);
			SignalRClient.Instance.ClearError();
		}
		else
		{
			GuildModelWrapper guildModelWrapper = new GuildModelWrapper(guildModel);
			(SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.GuildSuggestionPopup) as GuildSuggestionPopup).OpenForModel(guildModelWrapper);
		}
	}

	public override void OpenForModel(ModelObject model)
	{
		guild = ((GuildModelWrapper)model).GuildModel;
		base.OpenForModel(model);
		UpdateUI();
	}

	private async void OnPlayerNameSubmitted(UIType uiType)
	{
		if (GameManager.Instance.GuildManager.JoinGuild(guild.Id))
		{
			if (!string.IsNullOrEmpty(GameManager.Instance.modelManager.Player.GuildId) && CampView.Instance != null)
			{
				CampView.Instance.ShowGuildInfoPendingOnJoin = true;
			}
			GuildModel guildModel = await GuildManager.GetGuild(guild.Id);
			PlayerModel playerModel = GameManager.Instance.playerModel;
			GuildManager.ShowGuildJoinResultMessage(immediateJoin: true, guildModel?.IsBanned(playerModel.HashedId, playerModel.UtcTimeStamp) ?? false);
		}
	}

	private void OnJoinGuildClicked(UIButtonExtended button)
	{
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_click");
		Close();
		if (guild != null)
		{
			CampHUD.OpenIfPlayerHasName(UIType.GuildSuggestionPopup, OnPlayerNameSubmitted);
		}
	}

	private void OnMoreGuildsClicked(UIButtonExtended button)
	{
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_click");
		Close();
		CampHUD campHUD = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CampCampMapHud) as CampHUD;
		if (campHUD != null)
		{
			campHUD.OnClickGuild();
		}
	}

	private void OnCloseButtonClicked(UIButtonExtended button)
	{
		base.OnClickClose();
	}
}
