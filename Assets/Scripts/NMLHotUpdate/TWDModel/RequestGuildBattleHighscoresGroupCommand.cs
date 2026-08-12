using BaseModel;

namespace TWDModel
{
	public class RequestGuildBattleHighscoresGroupCommand : TWDGroupCommand
	{
		public bool ForceBroadcast { get; private set; }

		public RequestGuildBattleHighscoresGroupCommand()
		{
		}

		public RequestGuildBattleHighscoresGroupCommand(bool forceBroadcast)
		{
			ForceBroadcast = forceBroadcast;
		}

		public override GroupCommandBase Execute(ModelManager manager)
		{
			TWDModelManager tWDModelManager = manager as TWDModelManager;
			if (!HelpersModel.IsOfflineMode && tWDModelManager.ServerService == null)
			{
				return this;
			}
			GuildModel guildModel = (GuildModel)tWDModelManager.GetGroupModel(GroupId);
			if (guildModel == null)
			{
				manager.GvGLogError("UpdateGuildBattleHighscoresGroupCommand: No Guild found with GroupId: " + GroupId);
				return this;
			}
			if (SenderId == tWDModelManager.Player.HashedId)
			{
				guildModel.GuildWarModel.CurrentBattle.FetchBattleHighscores(guildModel.TimeStamp, tWDModelManager, ForceBroadcast, forceUpdate: false, updateGuildBattleResults: false);
			}
			return this;
		}
	}
}
