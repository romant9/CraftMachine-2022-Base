using BaseModel;

namespace TWDModel
{
	internal class UpdateLeaderboardSaveStateGroupCommand : TWDGroupCommand
	{
		public bool GuildAllTimeScoreUpdated { get; private set; }

		public bool GvGSeasonScoreUpdated { get; private set; }

		public bool GuildWarScoreUpdated { get; private set; }

		public bool GuildBattleScoreUpdated { get; private set; }

		public long UpdateStateTimeStamp { get; private set; }

		public UpdateLeaderboardSaveStateGroupCommand(bool guildAllTimeScoreUpdated, bool gvgSeasonScoreUpdated, bool guildWarScoreUpdated, bool guildBattleScoreUpdated, long timeStamp)
		{
			GuildAllTimeScoreUpdated = guildAllTimeScoreUpdated;
			GvGSeasonScoreUpdated = gvgSeasonScoreUpdated;
			GuildWarScoreUpdated = guildWarScoreUpdated;
			GuildBattleScoreUpdated = guildBattleScoreUpdated;
			UpdateStateTimeStamp = timeStamp;
		}

		public override GroupCommandBase Execute(ModelManager manager)
		{
			GuildModel obj = (GuildModel)(manager as TWDModelManager).GetGroupModel(GroupId);
			obj.LeaderboardUpdated = GuildAllTimeScoreUpdated;
			obj.GvGSeasonModel.LeaderboardUpdated = GvGSeasonScoreUpdated;
			obj.GuildWarModel.LeaderboardUpdated = GuildWarScoreUpdated;
			obj.GuildWarModel.CurrentBattle.LeaderboardUpdated = GuildBattleScoreUpdated;
			obj.LastGvGLeaderboardUpdateTime = UpdateStateTimeStamp;
			return this;
		}
	}
}
