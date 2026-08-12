using BaseModel;

namespace TWDModel
{
	public class StartGvGSeasonGroupCommand : TWDValidationGroupCommand
	{
		public int SeasonDefinitionId { get; private set; }

		public StartGvGSeasonGroupCommand(int seasonId)
		{
			SeasonDefinitionId = seasonId;
		}

		protected override TWDValidationCommandResult Validate(ModelManager manager)
		{
			GuildModel guildModel = (GuildModel)(manager as TWDModelManager).GetGroupModel(GroupId);
			if (guildModel == null)
			{
				manager.GvGLogError("StartGvGSeasonGroupCommand: No Guild found with GroupId: " + GroupId);
				return TWDValidationCommandResult.Error;
			}
			if (guildModel.GvGSeasonModel.IsCurrentSeasonOpen(guildModel.TimeStamp))
			{
				manager.GvGLogWarning("StartGvGSeasonGroupCommand: Cancelled - The season has already started: " + SeasonDefinitionId, guildModel);
				return TWDValidationCommandResult.Error;
			}
			return TWDValidationCommandResult.OK;
		}

		protected override bool ExecuteInternal(ModelManager manager)
		{
			GuildModel guildModel = (GuildModel)(manager as TWDModelManager).GetGroupModel(GroupId);
			int num = 0 | (guildModel.GvGSeasonModel.StartSeason(SeasonDefinitionId, guildModel) ? 1 : 0);
			if (num != 0)
			{
				guildModel.GuildInfoCurrentVP = 0;
				manager.GvGLog("StartGvGSeasonGroupCommand: season started successfully: " + SeasonDefinitionId, guildModel);
			}
			return (byte)num != 0;
		}
	}
}
