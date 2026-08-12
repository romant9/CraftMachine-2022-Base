using BaseModel;

namespace TWDModel
{
	public class AcceptMembershipCommand : TWDSocialModelCommand
	{
		public string MemberId;

		public AcceptMembershipCommand()
		{
		}

		public AcceptMembershipCommand(string MemberId)
		{
			this.MemberId = MemberId;
		}

		protected override GroupCommandBase CreateGroupCommand(TWDModelManager manager)
		{
			return new AcceptMembershipGroupCommand(MemberId);
		}
	}
}
