using System.Collections.Generic;
using BaseModel;
using TWDModel;

public class GuildRequestsPanel : ScrollableListPanel<GuildMemberInfo>
{
	private List<GuildMemberInfo> memberRequests;

	public List<GuildMemberInfo> MemberRequestList => memberRequests;

	private void OnEnable()
	{
		UIEvent.OnUIEvent += OnUiEvent;
		GuildModel guildModel = GameManager.Instance.guildModel;
		if (guildModel != null)
		{
			guildModel.Changed += OnGuildChanged;
		}
	}

	private void OnDisable()
	{
		UIEvent.OnUIEvent -= OnUiEvent;
		GuildModel guildModel = GameManager.Instance.guildModel;
		if (guildModel != null)
		{
			guildModel.Changed -= OnGuildChanged;
		}
	}

	private void OnGuildChanged(GroupModelBase groupModelBase, string changed, object args)
	{
		switch (changed)
		{
		case "MemberAdded":
		case "MemberRemoved":
		case "MemberAccepted":
		case "MemberRefused":
			SetupRequestNotification();
			break;
		}
	}

	private void OnUiEvent(string type, object parameter)
	{
		if (type == "SocialGuildPlayerChanged")
		{
			SetupRequestNotification();
			SetNotificationsCards();
		}
	}

	public void SetNotificationsCards()
	{
		SetCards(memberRequests);
	}

	public void SetupRequestNotification()
	{
		GuildModel guildModel = GameManager.Instance.guildModel;
		if (guildModel != null && guildModel.CanAcceptRequests(GameManager.Instance.playerModel.HashedId))
		{
			memberRequests = guildModel.GuildMembersPending;
		}
	}
}
