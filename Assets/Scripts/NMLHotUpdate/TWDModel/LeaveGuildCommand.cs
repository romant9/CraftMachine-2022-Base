using BaseModel;

namespace TWDModel
{
	public class LeaveGuildCommand : TWDSocialModelCommand
	{
		public string GuildId;

		public string LeaverId;

		public GuildLeaveType LeaveType;

		protected override GroupCommandBase CreateGroupCommand(TWDModelManager manager)
		{
			return new LeaveGuildGroupCommand
			{
				GroupId = GuildId,
				LeaverId = LeaverId,
				LeaveType = LeaveType
			};
		}

		public override IModelCommandRespond Execute(ModelManager modelManager)
		{
			(modelManager as TWDModelManager).Player.ClearGuildRelatedData();
			return base.Execute(modelManager);
		}
	}
}
