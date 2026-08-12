using System.Collections;
using BaseModel;
using TWDModel;
using UnityEngine;

public class GuildInvitedInfoPopup : HUDElement
{
	public enum GuildStatus
	{
		GuildFull = 0,
		GuildCannotReceiveRequest = 1,
		GuildOpen = 2,
		GuildInviteOnly = 3,
		GuildOpenPlayerInOtherGuild = 4,
		GuildInviteOnlyPlayerInOtherGuild = 5,
		PlayerAlreadyInGuild = 6
	}

	[SerializeField]
	private UILabel nameLabel;

	[SerializeField]
	private UILabel descriptionLabel;

	[SerializeField]
	private UILabel numberMembersLabel;

	[SerializeField]
	private UILabel joinTypeLabel;

	[SerializeField]
	private UILabel purposeTypeLabel;

	[SerializeField]
	private UILabel challengeStarsAmountLabel;

	[Header("Inviter")]
	[SerializeField]
	private UILabel inviterNameLabel;

	[Header("Join guild confirmation")]
	[SerializeField]
	private GameObject containerJoinGuildConfirm;

	[SerializeField]
	private UIButton joinGuildConfirmPositveButton;

	[SerializeField]
	private UILabel joinGuildConfirmPositveButtonLabel;

	[SerializeField]
	private UIButton joinGuildConfirmNegativeButton;

	[SerializeField]
	private UILabel joinGuildConfirmNegativeButtonLabel;

	[SerializeField]
	private UILabel joinGuildConfirmQuestionLabel;

	[SerializeField]
	private GameObject guildInvitation;

	[SerializeField]
	private GameObject loadingGuilds;

	private GuildModel guild;

	private GuildStatus guildStatus;

	private bool isAlreadyInGuild;

	public string InviterId { get; set; }

	public override void OpenForModel(ModelObject model)
	{
		base.OpenForModel(model);
		guild = ((GuildModelWrapper)model).GuildModel;
		SetupGuildStatus();
		isAlreadyInGuild = guildStatus == GuildStatus.PlayerAlreadyInGuild;
		UIEvent.OnUIEvent += OnUIEvent;
		StartCoroutine(LoadingGuilds());
	}

	public override void Close()
	{
		base.Close();
		UIEvent.OnUIEvent -= OnUIEvent;
		SingularityMonoBehaviour<HUDManager>.Instance.GetNoCreation(UIType.IngameLoading).Close();
		bool leftGuild = isAlreadyInGuild && guildStatus != GuildStatus.PlayerAlreadyInGuild;
		Helpers.ExecuteCommandDelayed(new SendGuildInviteMetricsCommand(SendGuildInviteMetricsCommand.EventType.InviteResult, guild.Id, guildStatus.ToString(), isAlreadyInGuild, leftGuild));
	}

	private void SetupGuildStatus()
	{
		GuildModel guildModel = GameManager.Instance.guildModel;
		if (guildModel != null && guildModel.Id == guild.Id && guildModel.GetMemberInfo(GameManager.Instance.playerModel.HashedId) != null)
		{
			guildStatus = GuildStatus.PlayerAlreadyInGuild;
		}
		else if (guild.NumberMembers >= 20)
		{
			guildStatus = GuildStatus.GuildFull;
		}
		else if (!guild.CanReceiveRequest)
		{
			guildStatus = GuildStatus.GuildCannotReceiveRequest;
		}
		else if (guildModel != null)
		{
			if (guild.JoinType == GuildJoinType.Open)
			{
				guildStatus = GuildStatus.GuildOpenPlayerInOtherGuild;
			}
			else if (guild.JoinType == GuildJoinType.Invite)
			{
				guildStatus = GuildStatus.GuildInviteOnlyPlayerInOtherGuild;
			}
		}
		else if (guild.JoinType == GuildJoinType.Open)
		{
			guildStatus = GuildStatus.GuildOpen;
		}
		else if (guild.JoinType == GuildJoinType.Invite)
		{
			guildStatus = GuildStatus.GuildInviteOnly;
		}
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
		ShowStatus();
		ShowGuildInfo();
	}

	private void ShowStatus()
	{
		SetJoinGuildConfirmatioText(LocalizationManager.GetText("Popup.GuildInvite.Status." + guildStatus, GetCurrentGuildName()));
		if (guildStatus == GuildStatus.GuildOpenPlayerInOtherGuild || guildStatus == GuildStatus.GuildInviteOnlyPlayerInOtherGuild || guildStatus == GuildStatus.GuildOpen || guildStatus == GuildStatus.GuildInviteOnly)
		{
			SetJoinGuildConfirmatioPositiveButton(available: true, LocalizationManager.GetText("Button.Yes"), JoinGuild);
			SetJoinGuildConfirmatioNegativeButton(available: true, LocalizationManager.GetText("Button.No"), Close);
			ShowJoinGuildConfirmation(show: true);
		}
		else
		{
			ShowJoinGuildConfirmation(show: false);
		}
	}

	private string GetCurrentGuildName()
	{
		string result = "";
		GuildModel guildModel = GameManager.Instance.guildModel;
		if (guildModel != null)
		{
			result = guildModel.Name;
		}
		return result;
	}

	private void JoinGuild()
	{
		if (GuildWarHelper.IsGuildMember() && !GameManager.Instance.GuildManager.CheckCanLeaveGuild())
		{
			AlertPopup.ShowPopup(LocalizationManager.GetText("Popup.DeniedLeaveGuild.Title"), LocalizationManager.GetText("Popup.DeniedLeaveGuild.Message"), LocalizationManager.GetText("Button.Ok"));
			Close();
			return;
		}
		if (!GameManager.Instance.GuildManager.JoinGuild(guild.Id))
		{
			HUDNotification.Error(LocalizationManager.GetText("Error.ErrorGeneric"));
			return;
		}
		if (guildStatus == GuildStatus.GuildInviteOnly || guildStatus == GuildStatus.GuildInviteOnlyPlayerInOtherGuild)
		{
			Close();
			GuildManager.ShowGuildJoinResultMessage(immediateJoin: false, banned: false);
		}
		else
		{
			SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.IngameLoading).Open();
		}
		GameManager.Instance.RequestPltv();
	}

	private IEnumerator LoadingGuilds()
	{
		if (GameManager.Instance.GuildManager != null && GameManager.Instance.GuildManager.IsBusy)
		{
			Helpers.GameObjectSetActive(guildInvitation, value: false);
			Helpers.GameObjectSetActive(loadingGuilds, value: true);
			while (GameManager.Instance.GuildManager.IsBusy)
			{
				yield return null;
			}
		}
		Helpers.GameObjectSetActive(loadingGuilds, value: false);
		UpdateUI();
		Helpers.GameObjectSetActive(guildInvitation, value: true);
	}

	private void OnUIEvent(string type, object parameter)
	{
		if (type == "SocialGuildJoined" && guildStatus != GuildStatus.GuildInviteOnlyPlayerInOtherGuild && guildStatus != GuildStatus.GuildInviteOnly)
		{
			Close();
			SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.SocialPopupGuild).Open();
		}
	}

	private void ShowGuildInfo()
	{
		descriptionLabel.gameObject.SetActive(value: true);
		nameLabel.text = GameManager.Instance.GetFilteredText(guild.Name);
		descriptionLabel.text = GameManager.Instance.GetFilteredText(guild.Description);
		if (purposeTypeLabel != null)
		{
			Helpers.GameObjectSetActive(purposeTypeLabel, value: true);
			string text = guild.Purpose;
			if (text == null)
			{
				text = GuildModel.GetDefaultPurpose(GameManager.Instance.gameEconomyData.ConfigData.GuildPurposeTypes);
			}
			if (text != null)
			{
				purposeTypeLabel.text = HelpersLocalization.GetGuildPurpose(text);
			}
		}
		numberMembersLabel.text = guild.NumberMembers + "/" + 20;
		challengeStarsAmountLabel.text = guild.CurrentChallengeStars.ToString();
		ShowInviter();
		HelpersUI.SetContentToLabel(joinTypeLabel, LocalizationManager.GetText("Generic.Guild.JoinType." + guild.JoinType));
	}

	private void ShowInviter()
	{
		GuildMemberInfo memberInfo = guild.GetMemberInfo(InviterId);
		if (memberInfo != null)
		{
			inviterNameLabel.text = LocalizationManager.GetText("Popup.GuildInvite.Inviter{InviterName}", GameManager.Instance.GetFilteredText(memberInfo.Name));
		}
	}

	private void ShowJoinGuildConfirmation(bool show)
	{
		containerJoinGuildConfirm.SetActive(show);
	}

	private void SetJoinGuildConfirmatioPositiveButton(bool available, string text = null, EventDelegate.Callback callback = null)
	{
		SetJoinGuildConfirmatioButton(joinGuildConfirmPositveButton, joinGuildConfirmPositveButtonLabel, available, text, callback);
	}

	private void SetJoinGuildConfirmatioNegativeButton(bool available, string text = null, EventDelegate.Callback callback = null)
	{
		SetJoinGuildConfirmatioButton(joinGuildConfirmNegativeButton, joinGuildConfirmNegativeButtonLabel, available, text, callback);
	}

	private void SetJoinGuildConfirmatioButton(UIButton button, UILabel label, bool available, string text, EventDelegate.Callback callback)
	{
		button.gameObject.SetActive(available);
		label.text = text;
		if (callback != null)
		{
			button.onClick.Clear();
			button.onClick.Add(new EventDelegate(callback));
		}
	}

	private void SetJoinGuildConfirmatioText(string text)
	{
		joinGuildConfirmQuestionLabel.gameObject.SetActive(text != null);
		joinGuildConfirmQuestionLabel.text = text;
	}
}
