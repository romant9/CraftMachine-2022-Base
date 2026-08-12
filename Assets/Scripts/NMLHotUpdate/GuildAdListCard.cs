using TWDModel;
using UnityEngine;

public class GuildAdListCard : GuildListCard
{
	[SerializeField]
	protected UILabel pendingApprovalLabel;

	[SerializeField]
	protected UIButton requestButton;

	[SerializeField]
	protected UILabel requestButtonLabel;

	private bool needsToRefreshRequestLabel;

	public override void UpdateUI()
	{
		if (base.Item != null)
		{
			base.UpdateUI();
			string filteredText = GameManager.Instance.GetFilteredText(base.Item.Name);
			if (nameLabel != null)
			{
				nameLabel.text = ((base.Item.JoinType == GuildJoinType.Open) ? LocalizationManager.GetText("GuildAdvertisement.Title.Open{GuildName}", filteredText) : LocalizationManager.GetText("GuildAdvertisement.Title.ByRequest{GuildName}", filteredText));
			}
			if (numberMembersLabel != null)
			{
				numberMembersLabel.gameObject.SetActive(value: false);
			}
			if (scoreLabel != null)
			{
				scoreLabel.text = base.Item.PreviousChallengeStars.ToString();
			}
			if (base.Item.CanEdit(GameManager.Instance.playerModel.HashedId))
			{
				SetupForGuildOwner();
			}
			else
			{
				SetupForCandidate();
			}
		}
	}

	private void SetupForGuildOwner()
	{
		if (pendingApprovalLabel != null)
		{
			pendingApprovalLabel.gameObject.SetActive(value: false);
		}
		if (requestButton != null)
		{
			requestButton.gameObject.SetActive(value: true);
			if (requestButtonLabel != null)
			{
				requestButtonLabel.gameObject.SetActive(value: true);
				requestButtonLabel.text = ((base.Item.JoinType == GuildJoinType.Open && base.Item.NumberMembers < 20) ? LocalizationManager.GetText("Button.JoinGuild") : LocalizationManager.GetText("Button.RequestGuildJoin"));
			}
		}
	}

	private void SetupForCandidate()
	{
		bool flag = GameManager.Instance.guildModel != null && GameManager.Instance.guildModel.Id == base.Item.Id && GameManager.Instance.guildModel.GetMemberPendingInfo(GameManager.Instance.playerModel.HashedId) != null;
		if (pendingApprovalLabel != null)
		{
			pendingApprovalLabel.gameObject.SetActive(flag);
		}
		if (needsToRefreshRequestLabel)
		{
			needsToRefreshRequestLabel = !flag;
		}
		if (requestButton != null)
		{
			requestButton.gameObject.SetActive(!flag);
			requestButton.isEnabled = !flag;
			if (requestButtonLabel != null)
			{
				requestButtonLabel.gameObject.SetActive(value: true);
				requestButtonLabel.text = ((base.Item.JoinType == GuildJoinType.Open) ? LocalizationManager.GetText("Button.JoinGuild") : LocalizationManager.GetText("Button.RequestGuildJoin"));
			}
		}
	}

	public void Update()
	{
		if (needsToRefreshRequestLabel)
		{
			UpdateUI();
		}
	}

	public override void OnClick()
	{
		if (!base.Item.CanEdit(GameManager.Instance.playerModel.HashedId))
		{
			GuildModelWrapper model = new GuildModelWrapper(base.Item);
			GuildInfoPopup obj = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.SocialGuildInfoPopup) as GuildInfoPopup;
			obj.GuildInfoPopupType = GuildInfoPopup.GuildPopupType.GuildSearch;
			obj.OpenForModel(model);
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_click");
		}
	}

	public void OnRequestClick()
	{
		if (base.Item.IsFull)
		{
			AlertPopup.ShowPopup(LocalizationManager.GetText("Popup.GuildAdFull.Title"), LocalizationManager.GetText("Popup.GuildAdFull.Body"), LocalizationManager.GetText("Button.Ok"));
			return;
		}
		if (base.Item.JoinType == GuildJoinType.Invite)
		{
			GuildModel guildModel = GameManager.Instance.guildModel;
			if (guildModel != null)
			{
				if (guildModel.Id == base.Item.Id)
				{
					AlertPopup.ShowPopup(LocalizationManager.GetText("Popup.Guild.AlreadyRequested.Title"), LocalizationManager.GetText("Popup.Guild.AlreadyRequested.Message"), LocalizationManager.GetText("Button.Ok"));
				}
				else
				{
					ConfirmationPopup.ShowPopup(LocalizationManager.GetText("Popup.ConfirmationCancelOtherGuildRequest.Title{GuildToLeave}{NewGuild}", guildModel.Name, base.Item.Name), LocalizationManager.GetText("Popup.ConfirmationCancelOtherGuildRequest.Message{GuildToLeave}{NewGuild}", guildModel.Name, base.Item.Name), LocalizationManager.GetText("Button.Ok"), OnGuildJoinRequestConfirmed, LocalizationManager.GetText("Button.Cancel"));
				}
			}
			else
			{
				ConfirmationPopup.ShowPopup(LocalizationManager.GetText("Popup.Guild.Button.RequestMembership"), LocalizationManager.GetText("Popup.Guild.WannaRequestMembership"), LocalizationManager.GetText("Button.Yes"), OnGuildJoinRequestConfirmed, LocalizationManager.GetText("Button.Cancel"));
			}
		}
		else if (base.Item.JoinType == GuildJoinType.Open)
		{
			ConfirmationPopup.ShowPopup(LocalizationManager.GetText("GuildAdvertisement.ConfirmJoinTitle"), LocalizationManager.GetText("GuildAdvertisement.ConfirmJoinMessage"), LocalizationManager.GetText("Button.Yes"), OnOpenGuildJoin, LocalizationManager.GetText("Button.Cancel"));
		}
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_click");
	}

	public async void OnOpenGuildJoin()
	{
		GameManager.Instance.GuildManager.JoinGuild(base.Item.Id);
		GuildModel guildModel = await GuildManager.GetGuild(base.Item.Id);
		PlayerModel playerModel = GameManager.Instance.playerModel;
		GuildManager.ShowGuildJoinResultMessage(immediateJoin: true, guildModel?.IsBanned(playerModel.HashedId, playerModel.UtcTimeStamp) ?? true);
	}

	public void OnGuildJoinRequestConfirmed()
	{
		GameManager.Instance.GuildManager.JoinGuild(base.Item.Id);
		GuildManager.ShowGuildJoinResultMessage(immediateJoin: false, banned: false);
		needsToRefreshRequestLabel = true;
	}
}
