using BaseModel;

namespace TWDModel
{
	public class UpdateGvGLeaderboardsCommand : TWDSocialModelCommand
	{
		protected override TWDModelResult ValidateCommand(TWDModelManager modelManager)
		{
			GuildModel guildModel = modelManager.Player.GuildModel;
			if (!modelManager.Player.IsGuildMember)
			{
				modelManager.Debug.LogError("UpdateGvGLeaderboardsCommand: Player is not a Guild Member");
				return TWDModelResult.Error;
			}
			long num = (guildModel.GuildWarModel.CurrentBattle.LeaderboardUpdated ? UtilsDateTime.HourInMilliseconds : (UtilsDateTime.HourInMilliseconds / 2));
			if (guildModel.TimeStamp < guildModel.LastGvGLeaderboardUpdateTime + num)
			{
				modelManager.GvGLog("UpdateGvGLeaderboardsCommand: Requested before cache time expiration", guildModel);
				return TWDModelResult.Skip;
			}
			if (guildModel.LeaderboardUpdated && guildModel.GvGSeasonModel.LeaderboardUpdated && guildModel.GuildWarModel.LeaderboardUpdated && guildModel.GuildWarModel.CurrentBattle.LeaderboardUpdated)
			{
				modelManager.GvGLogWarning("UpdateGvGLeaderboardsCommand: The Leaderboards are up to date", guildModel);
				return TWDModelResult.Skip;
			}
			return TWDModelResult.OK;
		}

		protected override GroupCommandBase CreateGroupCommand(TWDModelManager modelManager)
		{
			return new UpdateGvGLeaderboardsGroupCommand();
		}
	}
}
