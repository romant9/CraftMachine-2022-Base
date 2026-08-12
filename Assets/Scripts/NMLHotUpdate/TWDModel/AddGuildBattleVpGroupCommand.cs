using BaseModel;

namespace TWDModel
{
	public class AddGuildBattleVpGroupCommand : TWDValidationGroupCommand
	{
		public int VictoryPoints { get; set; }

		public AddGuildBattleVpGroupCommand()
		{
		}

		public AddGuildBattleVpGroupCommand(int vp)
		{
			VictoryPoints = vp;
		}

		protected override TWDValidationCommandResult Validate(ModelManager manager)
		{
			GuildModel guildModel = (GuildModel)manager.GetGroupModel(GroupId);
			if (guildModel == null)
			{
				manager.GvGLogError("AddGuildBattleVpGroupCommand: No Guild found with GroupId: " + GroupId);
				return TWDValidationCommandResult.Error;
			}
			if (guildModel.GvGSeasonModel == null)
			{
				manager.GvGLogError("AddGuildBattleVpGroupCommand: No GvGSeasonModel: ");
				return TWDValidationCommandResult.Error;
			}
			return TWDValidationCommandResult.OK;
		}

		protected override bool ExecuteInternal(ModelManager modelManager)
		{
			TWDModelManager tWDModelManager = modelManager as TWDModelManager;
			GuildModel guildModel = (GuildModel)tWDModelManager.GetGroupModel(GroupId);
			if (guildModel.GvGSeasonModel != null)
			{
				guildModel.GvGSeasonModel.CurrentSeasonStats.UpdateWithResult(GuildBattleModel.GuildBattleResult.NotEnded, VictoryPoints);
				guildModel.GvGSeasonModel.CheckForTierIncrease();
				guildModel.GvGSeasonModel.LeaderboardUpdated = true;
				tWDModelManager.Player.GuildShopModel.UpdateGuildShopItemsOnNewTier();
				return true;
			}
			return false;
		}
	}
}
