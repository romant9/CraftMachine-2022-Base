using BaseModel;

namespace TWDModel
{
	public class SendGuildInviteMetricsCommand : ModelCommand
	{
		public enum EventType
		{
			None = 0,
			InviteSent = 1,
			ReceivedCombat = 2,
			ReceivedTutorial = 3,
			InviteResult = 4
		}

		public EventType eventType { get; set; }

		public string GuildInvitedId { get; set; }

		public string PlayerInvitedId { get; set; }

		public string GuildStatus { get; set; }

		public bool IsInGuild { get; set; }

		public bool LeftGuild { get; set; }

		public SendGuildInviteMetricsCommand()
		{
		}

		public SendGuildInviteMetricsCommand(EventType eventType)
		{
			this.eventType = eventType;
		}

		public SendGuildInviteMetricsCommand(EventType eventType, string guildInvitedId, string playerInvitedId)
		{
			this.eventType = eventType;
			GuildInvitedId = guildInvitedId;
			PlayerInvitedId = playerInvitedId;
		}

		public SendGuildInviteMetricsCommand(EventType eventType, string guildInvitedId, string guildStatus, bool isInGuild, bool leftGuild)
		{
			this.eventType = eventType;
			GuildInvitedId = guildInvitedId;
			GuildStatus = guildStatus;
			IsInGuild = isInGuild;
			LeftGuild = leftGuild;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelManager tWDModelManager = (TWDModelManager)manager;
			if (eventType == EventType.InviteSent)
			{
				tWDModelManager.Metrics.AddEnd().AddGuild(tWDModelManager.Player.GuildModel).AddGuildInvite()
					.Send();
			}
			if (eventType == EventType.ReceivedCombat)
			{
				tWDModelManager.Metrics.AddStart().AddInvitedToGuildCombat(GuildInvitedId, PlayerInvitedId).Send();
			}
			else if (eventType == EventType.ReceivedTutorial)
			{
				tWDModelManager.Metrics.AddStart().AddInvitedToGuildTutorial(GuildInvitedId, PlayerInvitedId).Send();
			}
			else if (eventType == EventType.InviteResult)
			{
				tWDModelManager.Metrics.AddEnd().AddInvitedToGuildPopup(GuildInvitedId, GuildStatus, IsInGuild, LeftGuild).Send();
			}
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
