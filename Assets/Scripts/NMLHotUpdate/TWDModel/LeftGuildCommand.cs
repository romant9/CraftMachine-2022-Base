using BaseModel;

namespace TWDModel
{
	public class LeftGuildCommand : ModelCommand
	{
		public string GuildId { get; set; }

		public string LeaverId { get; set; }

		public GuildLeaveType LeaveType { get; set; }

		public override IModelCommandRespond Execute(ModelManager modelManager)
		{
			TWDModelManager tWDModelManager = modelManager as TWDModelManager;
			if (tWDModelManager.ServerService != null)
			{
				LeftGuildGroupCommand leftGuildGroupCommand = new LeftGuildGroupCommand();
				leftGuildGroupCommand.SenderId = tWDModelManager.Player.HashedId;
				leftGuildGroupCommand.GroupId = GuildId;
				leftGuildGroupCommand.LeaverId = LeaverId;
				leftGuildGroupCommand.LeaveType = LeaveType;
				JsonCommand jsonCommand = new JsonCommand();
				jsonCommand.Type = leftGuildGroupCommand.GetType().FullName;
				jsonCommand.Command = tWDModelManager.GetMessageSerializer().SerializeObject(leftGuildGroupCommand);
				tWDModelManager.ServerService.RemoveGroupMember(GuildId, LeaverId, jsonCommand);
			}
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
