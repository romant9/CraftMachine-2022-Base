using BaseModel;

namespace TWDModel
{
	public class PromoteMemberToLeaderCommand : TWDSocialModelCommand
	{
		public string MemberId;

		public PromoteMemberToLeaderCommand()
		{
		}

		public PromoteMemberToLeaderCommand(string memberId)
		{
			MemberId = memberId;
		}

		protected override GroupCommandBase CreateGroupCommand(TWDModelManager manager)
		{
			return new PromoteMemberToLeaderGroupCommand
			{
				MemberId = MemberId
			};
		}
	}
}
