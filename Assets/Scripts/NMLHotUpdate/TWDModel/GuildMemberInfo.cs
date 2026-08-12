namespace TWDModel
{
	public class GuildMemberInfo
	{
		public string MemberId { get; set; }

		public string GuildId { get; set; }

		public string GuildLeaderboardName { get; set; }

		public string Name { get; set; }

		public int PlayerLevel { get; set; }

		public GuildMemberRole Role { get; set; }

		public GuildMemberState State { get; set; }

		public int CurrentChallengeStars { get; set; }

		public int TotalChallengeStars { get; set; }

		public int TotalVP { get; set; }

		public int PreviousChallengeStars { get; set; }

		public int HighestChallengeStars { get; set; }

		public bool ExcludedFromChallenge { get; set; }

		public int TotalChallengeStarsAtChallengeStart { get; set; }

		public long LastActiveDate { get; set; }

		public long GuildJoinedDate { get; set; }

		public PlayerEmblem PlayerEmblem { get; set; }

		public int GetMinutesSinceLastActive(long utcTimeStamp)
		{
			if (LastActiveDate <= 0 || utcTimeStamp <= 0 || utcTimeStamp < LastActiveDate)
			{
				return -1;
			}
			return UtilsMath.Max((int)((utcTimeStamp - LastActiveDate) / 60000), 0);
		}

		public bool IsOnline(long utcTimeStamp)
		{
			int minutesSinceLastActive = GetMinutesSinceLastActive(utcTimeStamp);
			if (minutesSinceLastActive < 0)
			{
				return false;
			}
			return minutesSinceLastActive < 5;
		}
	}
}
