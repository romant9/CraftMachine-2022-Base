using BaseModel;

namespace TWDModel
{
	public class StartGuildWarForPlayerCommand : TWDValidationModelCommand
	{
		protected override TWDValidationCommandResult Validate(ModelManager manager)
		{
			TWDModelManager tWDModelManager = manager as TWDModelManager;
			if (!tWDModelManager.Player.IsGuildMember)
			{
				manager.GvGLogError("StartGuildWarForPlayerCommand: Player Is Not In Guild");
				return TWDValidationCommandResult.Error;
			}
			if (!tWDModelManager.Player.GvGSeasonModelPlayer.HasGvGSeasonStarted())
			{
				manager.GvGLogError("StartGuildWarForPlayerCommand: GvG Season has not started for player", tWDModelManager.Player);
				return TWDValidationCommandResult.Error;
			}
			if (!tWDModelManager.Player.GvGSeasonModel.IsCurrentSeasonOpen(tWDModelManager.Player.UtcTimeStamp))
			{
				manager.GvGLogError("StartGuildWarForPlayerCommand: GvG Season was not active!", tWDModelManager.Player);
				return TWDValidationCommandResult.Error;
			}
			if (tWDModelManager.Player.GvGSeasonModelPlayer.GuildWarModelPlayer.HasWarStarted())
			{
				manager.GvGLogError("StartGuildWarForPlayerCommand: War already started for player", tWDModelManager.Player);
				return TWDValidationCommandResult.Error;
			}
			if (!tWDModelManager.Player.GuildWarModel.IsCurrentWarOpen(tWDModelManager.Player.UtcTimeStamp))
			{
				manager.GvGLogError("StartGuildWarForPlayerCommand: War or Battle was not active!", tWDModelManager.Player);
				return TWDValidationCommandResult.Error;
			}
			return TWDValidationCommandResult.OK;
		}

		protected override IModelCommandRespond ExecuteInternal(ModelManager modelManager)
		{
			TWDModelManager tWDModelManager = modelManager as TWDModelManager;
			tWDModelManager.Player.GvGSeasonModelPlayer.GuildWarModelPlayer.StartWar(tWDModelManager.Player.GuildWarModel.WarDefinitionId);
			modelManager.GvGLog("StartGuildWarForPlayerCommand: War started for player successfully", tWDModelManager.Player);
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
