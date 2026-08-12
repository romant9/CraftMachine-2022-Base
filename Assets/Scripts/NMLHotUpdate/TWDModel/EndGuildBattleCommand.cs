using BaseModel;

namespace TWDModel
{
	public class EndGuildBattleCommand : TWDSocialModelCommand
	{
		protected override TWDModelResult ValidateCommand(TWDModelManager modelManager)
		{
			GuildModel guildModel = modelManager.Player.GuildModel;
			if (!modelManager.Player.IsGuildMember)
			{
				modelManager.GvGLogError("EndGuildBattleCommand: Player is not a Guild Member");
				return TWDModelResult.Error;
			}
			if (!guildModel.GuildWarModel.CurrentBattle.IsBiggerThanEndBattleTimeStamp(modelManager.Player.UtcTimeStamp))
			{
				modelManager.GvGLogWarning("EndGuildBattleCommand: Cancelled - Battle time has not ended", modelManager.Player);
				return TWDModelResult.Skip;
			}
			if (guildModel.GuildWarModel.CurrentBattle.HasEnded())
			{
				modelManager.GvGLogWarning("EndGuildBattleCommand: Cancelled - Battle already ended", modelManager.Player);
				return TWDModelResult.Skip;
			}
			return TWDModelResult.OK;
		}

		protected override GroupCommandBase CreateGroupCommand(TWDModelManager manager)
		{
			int vp = 0;
			int opponentVP = 0;
			bool validResult = true;
			GuildModel guildModel = manager.Player.GuildModel;
			if (manager.ServerService != null)
			{
				validResult = guildModel.GuildWarModel.CurrentBattle.GetBattleScoresFromLeaderboard(manager, ref vp, ref opponentVP);
			}
			else if (HelpersModel.IsOfflineMode)
			{
				vp = guildModel.GuildWarModel.CurrentBattle.CalculateTotalVictoryPoints();
				opponentVP = manager.GameEconomyData.FindFakeBattleDefinition(guildModel.GuildWarModel.CurrentBattle.GuildTier).TargetScore;
			}
			return new EndGuildBattleGroupCommand(vp, opponentVP, validResult, guildModel.GuildWarModel.CurrentBattle.WarId);
		}
	}
}
