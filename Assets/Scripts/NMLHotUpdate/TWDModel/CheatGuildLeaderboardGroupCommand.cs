using BaseModel;

namespace TWDModel
{
	public class CheatGuildLeaderboardGroupCommand : TWDGroupCommand
	{
		public string MemberId;

		public int ElapsedDays;

		public int DayStars;

		public int DayMissions;

		public CheatGuildLeaderboardGroupCommand()
		{
		}

		public CheatGuildLeaderboardGroupCommand(string memberId, int elapsedDays, int daysStars, int dayMissions)
		{
		}

		public override GroupCommandBase Execute(ModelManager manager)
		{
			GuildModel guildModel = (GuildModel)manager.GetGroupModel(GroupId);
			if (guildModel != null)
			{
				if (guildModel.DEBUG_cheatSimulateNewStarsInANewDay(MemberId, ElapsedDays, DayStars, DayMissions))
				{
					SaveGroupModel(manager);
				}
				guildModel.NotifyChange("GuildModified");
			}
			return this;
		}
	}
}
