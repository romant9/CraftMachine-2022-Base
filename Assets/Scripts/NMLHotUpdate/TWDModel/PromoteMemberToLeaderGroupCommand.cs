using BaseModel;

namespace TWDModel
{
	public class PromoteMemberToLeaderGroupCommand : TWDGroupCommand
	{
		public string MemberId;

		public PromoteMemberToLeaderGroupCommand()
		{
		}

		public PromoteMemberToLeaderGroupCommand(string memberId)
		{
			MemberId = memberId;
		}

		public override GroupCommandBase Execute(ModelManager manager)
		{
			GuildModel guildModel = (GuildModel)manager.GetGroupModel(GroupId);
			if (guildModel != null)
			{
				bool isPromotion = false;
				if (guildModel.SetMemberRole(MemberId, SenderId, GuildMemberRole.Leader, ref isPromotion) == TWDModelResult.OK)
				{
					guildModel.SetMemberRole(SenderId, SenderId, GuildMemberRole.CoLeader, ref isPromotion);
				}
				SaveGroupModel(manager);
			}
			return this;
		}
	}
}
