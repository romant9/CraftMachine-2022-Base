using BaseModel;

namespace TWDModel
{
	public class SetMemberLastActiveDateGroupCommand : TWDGroupCommand
	{
		public string MemberId { get; set; }

		public long MemberUTCTimestamp { get; set; }

		public override GroupCommandBase Execute(ModelManager manager)
		{
			if (((GuildModel)manager.GetGroupModel(GroupId)).SetMemberLastActiveDate(MemberId, MemberUTCTimestamp) == TWDModelResult.OK)
			{
				SaveGroupModel(manager);
			}
			return this;
		}
	}
}
