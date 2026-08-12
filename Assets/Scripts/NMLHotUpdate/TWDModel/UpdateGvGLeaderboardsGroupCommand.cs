using BaseModel;

namespace TWDModel
{
	public class UpdateGvGLeaderboardsGroupCommand : TWDGroupCommand
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
				manager.GvGLogError("UpdateGvGLeaderboardsGroupCommand: No Guild found with GroupId: " + GroupId);
				return this;
			}
			if (SenderId == tWDModelManager.Player.HashedId)
			{
				guildModel.UpdateGuildGvGLeaderboards(tWDModelManager.ServerService, tWDModelManager, battleEnd: false, updateOnlyFailedSaves: true);
			}
			return this;
		}
	}
}
