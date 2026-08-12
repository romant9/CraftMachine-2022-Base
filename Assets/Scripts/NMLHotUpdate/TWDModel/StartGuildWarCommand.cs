using BaseModel;

namespace TWDModel
{
	public class StartGuildWarCommand : TWDSocialModelCommand
	{
		public int SeasonDefinitionId { get; private set; }

		public int WarDefinitionId { get; private set; }

		public StartGuildWarCommand()
		{
		}

		public StartGuildWarCommand(int seasonId, int warId)
		{
			SeasonDefinitionId = seasonId;
			WarDefinitionId = warId;
		}

		protected override TWDModelResult ValidateCommand(TWDModelManager modelManager)
		{
			GuildModel guildModel = modelManager.Player.GuildModel;
			if (!modelManager.Player.IsGuildMember)
			{
				modelManager.GvGLogError("StartGuildWarCommand: Player is not a Guild Member");
				return TWDModelResult.Error;
			}
			if (!guildModel.GvGSeasonModel.IsCurrentSeasonOpen(modelManager.Player.UtcTimeStamp))
			{
				modelManager.GvGLogWarning("StartGuildWarCommand: Cancelled - The season has not been started", modelManager.Player);
				return TWDModelResult.Skip;
			}
			if (guildModel.GuildWarModel.IsCurrentWarOpen(modelManager.Player.UtcTimeStamp))
			{
				modelManager.GvGLogWarning("StartGuildWarCommand: Cancelled - war is active", modelManager.Player);
				return TWDModelResult.Skip;
			}
			return TWDModelResult.OK;
		}

		protected override GroupCommandBase CreateGroupCommand(TWDModelManager manager)
		{
			return new StartGuildWarGroupCommand(SeasonDefinitionId, WarDefinitionId);
		}
	}
}
