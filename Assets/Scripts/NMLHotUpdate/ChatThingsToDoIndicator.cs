using BaseModel;
using TWDModel;

public class ChatThingsToDoIndicator : SocialThingsToDoIndicator
{
	protected override void OnPlayerChanged(ModelObject modelObject, string changed, object args)
	{
		if (changed == "guildChanged")
		{
			ListenToGuild();
		}
	}

	public override void UpdateUI()
	{
		int num = 0;
		GameManager instance = GameManager.Instance;
		GuildModel guildModel = instance.guildModel;
		string hashedId = instance.playerModel.HashedId;
		if (guildModel != null && instance.playerModel.IsGuildMember)
		{
			num += guildModel.GetUnreadChatAmount(hashedId, instance.playerModel.LastReadChatTime);
		}
		if (num == 0)
		{
			NGUITools.SetActiveChildren(base.gameObject, state: false);
			return;
		}
		NGUITools.SetActiveChildren(base.gameObject, state: true);
		requestsNumberLabel.text = num.ToString();
	}
}
