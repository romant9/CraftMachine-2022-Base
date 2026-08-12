using BaseModel;

namespace TWDModel
{
	public class CheatGuildLeaderboardCommand : TWDSocialModelCommand
	{
		public string GuildId;

		public string MemberId;

		public int ElapsedDays;

		public int DayStars;

		public int DayMissions;

		protected override GroupCommandBase CreateGroupCommand(TWDModelManager manager)
		{
			return new CheatGuildLeaderboardGroupCommand
			{
				GroupId = GuildId,
				MemberId = MemberId,
				ElapsedDays = ElapsedDays,
				DayStars = DayStars,
				DayMissions = DayMissions
			};
		}
	}
}
