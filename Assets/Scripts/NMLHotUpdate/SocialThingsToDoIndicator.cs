using BaseModel;
using TWDModel;
using UnityEngine;

public class SocialThingsToDoIndicator : MonoBehaviour
{
	public enum NotificationTypes
	{
		All = 0,
		ChatOnly = 1,
		RequestsOnly = 2,
		GiftsOnly = 3
	}

	[SerializeField]
	protected NotificationTypes notificationsShown;

	[SerializeField]
	protected UILabel requestsNumberLabel;

	private void OnEnable()
	{
		if (GameManager.Instance != null)
		{
			GameManager.Instance.playerModel.Changed += OnPlayerChanged;
			UIEvent.OnUIEvent += OnUIEvent;
			ListenToGuild();
			UpdateUI();
		}
	}

	private void OnDisable()
	{
		if (GameManager.Instance != null)
		{
			GameManager.Instance.playerModel.Changed -= OnPlayerChanged;
			GuildModel guildModel = GameManager.Instance.guildModel;
			UIEvent.OnUIEvent -= OnUIEvent;
			if (guildModel != null)
			{
				guildModel.Changed -= OnGuildChanged;
			}
		}
	}

	protected void ListenToGuild()
	{
		GuildModel guildModel = GameManager.Instance.guildModel;
		if (guildModel != null)
		{
			guildModel.Changed += OnGuildChanged;
		}
	}

	protected virtual void OnPlayerChanged(ModelObject modelObject, string changed, object args)
	{
		switch (changed)
		{
		case "guildChanged":
			ListenToGuild();
			break;
		case "guildGiftAvailable":
		case "guildGiftClaimed":
			UpdateUI();
			break;
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
		case "MessageAdded":
			UpdateUI();
			break;
		}
	}

	private void OnUIEvent(string type, object parameter)
	{
		if (type == "SocialChatRead" || type == "SocialChatNewMessage")
		{
			UpdateUI();
		}
	}

	public virtual void UpdateUI()
	{
		int num = 0;
		GameManager instance = GameManager.Instance;
		GuildModel guildModel = instance.guildModel;
		string hashedId = instance.playerModel.HashedId;
		if (guildModel != null)
		{
			if (IsEnabled(NotificationTypes.RequestsOnly) && guildModel.CanAcceptRequests(hashedId))
			{
				num += guildModel.GuildMembersPending.Count;
			}
			if (IsEnabled(NotificationTypes.ChatOnly) && instance.playerModel.IsGuildMember)
			{
				num += guildModel.GetUnreadChatAmount(hashedId, instance.playerModel.LastReadChatTime);
			}
			if (IsEnabled(NotificationTypes.GiftsOnly) && instance.playerModel.PendingGuildGiftsToOpen != null)
			{
				num += instance.playerModel.PendingGuildGiftsToOpen.Count;
			}
		}
		if (num == 0)
		{
			NGUITools.SetActiveChildren(base.gameObject, state: false);
			return;
		}
		NGUITools.SetActiveChildren(base.gameObject, state: true);
		requestsNumberLabel.text = num.ToString();
	}

	private bool IsEnabled(NotificationTypes type)
	{
		if (notificationsShown != NotificationTypes.All)
		{
			return notificationsShown == type;
		}
		return true;
	}
}
