using BaseModel;

namespace TWDModel
{
	public class ModifyMemberRoleCommand : TWDSocialModelCommand
	{
		public string MemberId;

		public GuildMemberRole NewRole;

		public ModifyMemberRoleCommand()
		{
		}

		public ModifyMemberRoleCommand(string memberId)
		{
			MemberId = memberId;
		}

		protected override GroupCommandBase CreateGroupCommand(TWDModelManager manager)
		{
			return new ModifyMemberRoleGroupCommand
			{
				MemberId = MemberId,
				NewRole = NewRole
			};
		}
	}
}
