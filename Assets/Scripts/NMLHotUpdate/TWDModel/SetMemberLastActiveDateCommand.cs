using BaseModel;

namespace TWDModel
{
	public class SetMemberLastActiveDateCommand : TWDSocialModelCommand
	{
		public string MemberId { get; set; }

		public long MemberUTCTimestamp { get; set; }

		protected override GroupCommandBase CreateGroupCommand(TWDModelManager manager)
		{
			return new SetMemberLastActiveDateGroupCommand
			{
				MemberId = MemberId,
				MemberUTCTimestamp = MemberUTCTimestamp
			};
		}

		public override IModelCommandRespond Execute(ModelManager modelManager)
		{
			return base.Execute(modelManager) as NGModelCommandRespond;
		}
	}
}
