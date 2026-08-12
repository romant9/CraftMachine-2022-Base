using BaseModel;

namespace TWDModel
{
	public class StartGvGSeasonForPlayerCommand : TWDValidationModelCommand
	{
		protected override TWDValidationCommandResult Validate(ModelManager manager)
		{
			TWDModelManager tWDModelManager = manager as TWDModelManager;
			if (!tWDModelManager.Player.IsGuildMember)
			{
				manager.GvGLogError("StartGvGSeasonForPlayerCommand: Player Is Not In Guild");
				return TWDValidationCommandResult.Error;
			}
			if (tWDModelManager.Player.GvGSeasonModelPlayer.HasGvGSeasonStarted())
			{
				manager.GvGLogError("StartGvGSeasonForPlayerCommand: GvG Season already started for player", tWDModelManager.Player);
				return TWDValidationCommandResult.Error;
			}
			if (!tWDModelManager.Player.GvGSeasonModel.IsCurrentSeasonOpen(tWDModelManager.Player.UtcTimeStamp))
			{
				manager.GvGLogError("StartGvGSeasonForPlayerCommand: GvG Season was not active!", tWDModelManager.Player);
				return TWDValidationCommandResult.Error;
			}
			return TWDValidationCommandResult.OK;
		}

		protected override IModelCommandRespond ExecuteInternal(ModelManager manager)
		{
			TWDModelManager tWDModelManager = manager as TWDModelManager;
			manager.GvGLog("StartGvGSeasonForPlayerCommand: GvG Season started successfully for player", tWDModelManager.Player);
			tWDModelManager.Player.GvGSeasonModelPlayer.StartSeason(tWDModelManager.Player.GvGSeasonModel.SeasonDefinitionId);
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
