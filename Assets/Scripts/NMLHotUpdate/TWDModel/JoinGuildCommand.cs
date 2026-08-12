using BaseModel;

namespace TWDModel
{
	public class JoinGuildCommand : ModelCommand
	{
		public string GuildId;

		public string JoinerName;

		public PlayerEmblem JoinerPlayerEmblem;

		public int JoinerLevel;

		public string SearchId;

		public int SearchPosition = -1;

		public int totalVpPoints;

		public bool AutomaticallyJoinIfOpen { get; set; }

		public override IModelCommandRespond Execute(ModelManager modelManager)
		{
			if (modelManager is TWDModelManager { ServerService: not null } tWDModelManager)
			{
				AddGuildMemberGroupCommand addGuildMemberGroupCommand = new AddGuildMemberGroupCommand();
				addGuildMemberGroupCommand.GroupId = GuildId;
				addGuildMemberGroupCommand.SenderId = tWDModelManager.Player.HashedId;
				addGuildMemberGroupCommand.MemberId = tWDModelManager.Player.HashedId;
				addGuildMemberGroupCommand.MemberName = JoinerName;
				addGuildMemberGroupCommand.PlayerEmblem = JoinerPlayerEmblem;
				addGuildMemberGroupCommand.MemberLevel = JoinerLevel;
				addGuildMemberGroupCommand.totalVPPoints = totalVpPoints;
				addGuildMemberGroupCommand.AutomaticallyJoinIfOpen = AutomaticallyJoinIfOpen;
				addGuildMemberGroupCommand.SearchId = SearchId;
				addGuildMemberGroupCommand.SearchPosition = SearchPosition;
				JsonCommand jsonCommand = new JsonCommand();
				jsonCommand.Type = addGuildMemberGroupCommand.GetType().FullName;
				jsonCommand.Command = tWDModelManager.GetMessageSerializer().SerializeObject(addGuildMemberGroupCommand);
				tWDModelManager.ServerService.JoinGroup(GuildId, tWDModelManager.Player.HashedId, jsonCommand);
			}
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
