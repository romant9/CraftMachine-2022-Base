using BaseModel;

namespace TWDModel
{
	public class ClearGvGStateGroupCommand : TWDGroupCommand
	{
		public bool FullHardReset { get; set; }

		public bool FullWarReset { get; set; }

		public bool ResetNextBattle { get; set; }

		public bool ResetCurrentBattle { get; set; }

		public ClearGvGStateGroupCommand()
		{
			FullHardReset = false;
			FullWarReset = false;
		}

		public override GroupCommandBase Execute(ModelManager manager)
		{
			TWDModelManager tWDModelManager = manager as TWDModelManager;
			GuildModel guildModel = (GuildModel)tWDModelManager.GetGroupModel(GroupId);
			if (guildModel == null)
			{
				manager.GvGLogError("ClearGuildWarStateCommand: No Guild found with GroupId: " + GroupId);
				return this;
			}
			if (FullHardReset)
			{
				guildModel.GvGSeasonModel = null;
				guildModel.GuildBattleMatchmakingInfo = null;
				guildModel.StartGroupChildren(null, tWDModelManager.GameEconomyData);
				return this;
			}
			GuildWarModel guildWarModel = guildModel.GuildWarModel;
			if (guildWarModel == null)
			{
				manager.GvGLogError("ClearGuildWarStateCommand: War model is null", guildModel);
				return this;
			}
			if (FullWarReset)
			{
				guildModel.GvGSeasonModel.GuildWarModel = new GuildWarModel();
				guildWarModel.CurrentBattle.SetPlayerOwnerAndGameEconomyData(tWDModelManager.GameEconomyData, guildModel.GvGSeasonModel, null);
			}
			if (ResetNextBattle)
			{
				guildModel.GuildBattleMatchmakingInfo.ResetParticipants();
				guildWarModel.RegisteredPlayersForBattleSlot.Clear();
			}
			if (ResetCurrentBattle)
			{
				guildWarModel.CurrentBattle = new GuildBattleModel();
			}
			if (FullWarReset || ResetCurrentBattle)
			{
				guildModel.GvGSeasonModel.SetPlayerOwnerAndGameEconomyData(tWDModelManager.GameEconomyData, guildModel.GvGSeasonModel, null);
			}
			SaveGroupModel(tWDModelManager);
			return this;
		}
	}
}
