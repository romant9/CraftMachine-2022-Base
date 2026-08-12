using BaseModel;

namespace TWDModel
{
	public class StartGuildBattleForPlayerCommand : TWDValidationModelCommand
	{
		protected override TWDValidationCommandResult Validate(ModelManager manager)
		{
			TWDModelManager tWDModelManager = manager as TWDModelManager;
			if (!tWDModelManager.Player.IsGuildMember)
			{
				manager.GvGLogError("StartGuildBattleForPlayerCommand: Player Is Not In Guild");
				return TWDValidationCommandResult.Error;
			}
			if (tWDModelManager.Player.GvGSeasonModelPlayer.GuildWarModelPlayer.GuildBattleModel.IsOngoingForPlayer())
			{
				manager.GvGLogWarning("StartGuildBattleForPlayerCommand: Battle is ongoing for player, trying to start battle when previous has not ended", tWDModelManager.Player);
				return TWDValidationCommandResult.Canceled;
			}
			if (tWDModelManager.Player.GvGSeasonModelPlayer.GuildWarModelPlayer.GuildBattleModel.IsCurrentBattleActiveForPlayer())
			{
				manager.GvGLogWarning("StartGuildBattleForPlayerCommand: Player is already in battle. According to StartBattleTimestamp", tWDModelManager.Player);
				return TWDValidationCommandResult.Canceled;
			}
			if (!tWDModelManager.Player.GuildWarModel.IsWarAndBattleOngoing(tWDModelManager.Player.UtcTimeStamp))
			{
				manager.GvGLogWarning("StartGuildBattleForPlayerCommand: War or Battle was not active!", tWDModelManager.Player);
				return TWDValidationCommandResult.Canceled;
			}
			return TWDValidationCommandResult.OK;
		}

		protected override IModelCommandRespond ExecuteInternal(ModelManager modelManager)
		{
			TWDModelManager tWDModelManager = modelManager as TWDModelManager;
			long battleSlotForTimeStamp = tWDModelManager.Player.GuildModel.GuildWarModel.GetBattleSlotForTimeStamp(tWDModelManager.Player.UtcTimeStamp);
			GuildWarModelPlayer guildWarModelPlayer = tWDModelManager.Player.GvGSeasonModelPlayer.GuildWarModelPlayer;
			guildWarModelPlayer.UpdateBattleLogEntryOnStart(battleSlotForTimeStamp, GuildBattleLogPlayerEntry.Status.ValidatedSuccess);
			guildWarModelPlayer.StartBattle(tWDModelManager.Player.GuildWarModel.CurrentBattle.TimeSlot);
			string text = (guildWarModelPlayer.GuildBattleModel.IsFakeBattle ? "Fake " : "");
			modelManager.GvGLog("StartGuildBattleForPlayerCommand: " + text + "Battle started for player successfully", tWDModelManager.Player);
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
