using BaseModel;

namespace TWDModel
{
	public class StartGvGSeasonCommand : TWDSocialModelCommand
	{
		public int SeasonDefinitionId { get; private set; }

		public StartGvGSeasonCommand()
		{
		}

		public StartGvGSeasonCommand(int seasonId)
		{
			SeasonDefinitionId = seasonId;
		}

		protected override TWDModelResult ValidateCommand(TWDModelManager modelManager)
		{
			GuildModel guildModel = modelManager.Player.GuildModel;
			if (!modelManager.Player.IsGuildMember)
			{
				modelManager.GvGLogError("StartGvGSeasonCommand: Player is not a Guild Member");
				return TWDModelResult.Error;
			}
			if (guildModel.GvGSeasonModel.IsCurrentSeasonOpen(modelManager.Player.UtcTimeStamp))
			{
				modelManager.GvGLogWarning("StartGvGSeasonCommand: Cancelled - The season has already started", modelManager.Player);
				return TWDModelResult.Skip;
			}
			return TWDModelResult.OK;
		}

		protected override GroupCommandBase CreateGroupCommand(TWDModelManager modelManager)
		{
			return new StartGvGSeasonGroupCommand(SeasonDefinitionId);
		}
	}
}
