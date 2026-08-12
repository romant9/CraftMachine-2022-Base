using BaseModel;

namespace TWDModel
{
	public class RefreshGuildBattleHighscoresGroupCommand : TWDGroupCommand
	{
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
				manager.GvGLogError("RefreshGuildBattleHighscoresGroupCommand: No Guild found with GroupId: " + GroupId);
				return this;
			}
			if (SenderId == tWDModelManager.Player.HashedId)
			{
				guildModel.GuildWarModel.CurrentBattle.FetchBattleHighscores(guildModel.TimeStamp, tWDModelManager, forceBroadcast: true, forceUpdate: false, updateGuildBattleResults: false, requireUpdate: true);
			}
			return this;
		}
	}
}
