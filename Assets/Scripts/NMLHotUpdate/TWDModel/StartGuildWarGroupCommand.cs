using BaseModel;

namespace TWDModel
{
	public class StartGuildWarGroupCommand : TWDValidationGroupCommand
	{
		public int SeasonDefinitionId { get; private set; }

		public int WarDefinitionId { get; private set; }

		public StartGuildWarGroupCommand()
		{
		}

		public StartGuildWarGroupCommand(int seasonId, int warId)
		{
			SeasonDefinitionId = seasonId;
			WarDefinitionId = warId;
		}

		protected override TWDValidationCommandResult Validate(ModelManager manager)
		{
			GuildModel guildModel = (GuildModel)(manager as TWDModelManager).GetGroupModel(GroupId);
			if (guildModel == null)
			{
				manager.GvGLogError("StartGuildWarGroupCommand: No Guild found with GroupId: " + GroupId, guildModel);
				return TWDValidationCommandResult.Error;
			}
			if (!guildModel.GvGSeasonModel.IsCurrentSeasonOpen(guildModel.TimeStamp))
			{
				manager.GvGLogError("StartGuildWarGroupCommand: The season has not been started: " + GroupId, guildModel);
				return TWDValidationCommandResult.Error;
			}
			if (guildModel.GvGSeasonModel.GuildWarModel.IsCurrentWarOpen(guildModel.TimeStamp))
			{
				manager.GvGLog("StartGuildWarGroupCommand: Current war is open: " + GroupId, guildModel);
				return TWDValidationCommandResult.Canceled;
			}
			return TWDValidationCommandResult.OK;
		}

		protected override bool ExecuteInternal(ModelManager modelManager)
		{
			TWDModelManager tWDModelManager = modelManager as TWDModelManager;
			GuildModel guildModel = (GuildModel)tWDModelManager.GetGroupModel(GroupId);
			int num = 0 | (guildModel.GuildWarModel.StartWarIfNeeded(WarDefinitionId, guildModel, tWDModelManager) ? 1 : 0);
			if (num != 0)
			{
				modelManager.GvGLog("StartGuildWarGroupCommand: War started successfully", guildModel);
			}
			return (byte)num != 0;
		}
	}
}
