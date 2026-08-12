using BaseModel;

namespace TWDModel
{
	public class AcceptMembershipGroupCommand : TWDGroupCommand
	{
		public string MemberId { get; set; }

		public AcceptMembershipGroupCommand()
		{
		}

		public AcceptMembershipGroupCommand(string memberId)
		{
			MemberId = memberId;
		}

		public override GroupCommandBase Execute(ModelManager manager)
		{
			GuildModel guildModel = (GuildModel)manager.GetGroupModel(GroupId);
			if (guildModel != null && guildModel.CanAcceptRequests(SenderId))
			{
				TWDModelManager tWDModelManager = manager as TWDModelManager;
				long timestamp = ((tWDModelManager != null && tWDModelManager.Player != null) ? tWDModelManager.Player.UtcTimeStamp : 0);
				if (guildModel.AcceptMemberRequest(SenderId, MemberId, timestamp) == TWDModelResult.OK)
				{
					SaveGroupModel(manager);
					if (tWDModelManager != null)
					{
						GuildMemberInfo memberPendingInfo = guildModel.GetMemberPendingInfo(MemberId);
						GuildMemberInfo memberPendingInfo2 = guildModel.GetMemberPendingInfo(SenderId);
						if (tWDModelManager.Metrics != null)
						{
							tWDModelManager.Metrics.AddGuild(guildModel).AddModerator(memberPendingInfo2).AddSend()
								.AddMember(memberPendingInfo)
								.AddJoinAcceptance()
								.Send();
						}
					}
				}
			}
			return this;
		}
	}
}
