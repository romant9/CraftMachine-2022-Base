using BaseModel;

namespace TWDModel
{
	public class DisbandGuildCommand : ModelCommand
	{
		public string GuildId { get; set; }

		public override IModelCommandRespond Execute(ModelManager modelManager)
		{
			TWDModelManager tWDModelManager = modelManager as TWDModelManager;
			if (tWDModelManager.ServerService != null && tWDModelManager.GetGroupModel(GuildId) != null)
			{
				LeftGuildGroupCommand leftGuildGroupCommand = new LeftGuildGroupCommand();
				leftGuildGroupCommand.GroupId = GuildId;
				leftGuildGroupCommand.LeaverId = tWDModelManager.Player.HashedId;
				leftGuildGroupCommand.LeaveType = GuildLeaveType.LeaderLeave;
				JsonCommand jsonCommand = new JsonCommand();
				jsonCommand.Type = leftGuildGroupCommand.GetType().FullName;
				jsonCommand.Command = tWDModelManager.GetMessageSerializer().SerializeObject(leftGuildGroupCommand);
				tWDModelManager.ServerService.DisbandGroup(GuildId, jsonCommand);
			}
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
