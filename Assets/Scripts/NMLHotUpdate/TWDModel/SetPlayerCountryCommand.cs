using BaseModel;

namespace TWDModel
{
	public class SetPlayerCountryCommand : ModelCommand
	{
		public string Country { get; set; }

		public string OldCountry { get; set; }

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			PlayerModel playerModel = manager.GetPlayer() as PlayerModel;
			if (!string.IsNullOrEmpty(Country))
			{
				playerModel.Country = Country;
				if (manager.ServerService != null)
				{
					LeaderboardEntry leaderboardEntry = Leaderboards.CreateChallengeLeaderboardEntry(playerModel);
					if (leaderboardEntry.Score > 0)
					{
						manager.ServerService.SaveLeaderboardEntry(Leaderboards.ChallengeStarsCountryPrefix + Country, leaderboardEntry);
					}
					string challengeId = playerModel.WeeklyChallenge.Id.ToString();
					LeaderboardEntry leaderboardEntry2 = Leaderboards.CreateCurrentChallengeLeaderboardEntry(playerModel);
					if (leaderboardEntry2.Score > 0)
					{
						manager.ServerService.SaveLeaderboardEntry(Leaderboards.GetPlayerChallengeWeeklyCountryLeaderboardName(Country, challengeId), leaderboardEntry2);
					}
					if (!string.IsNullOrEmpty(OldCountry) && OldCountry != Country)
					{
						LeaderboardEntry leaderboardEntry3 = Leaderboards.CreateChallengeLeaderboardEntry(playerModel);
						leaderboardEntry3.Score = 0L;
						manager.ServerService.SaveLeaderboardEntry(Leaderboards.ChallengeStarsCountryPrefix + OldCountry, leaderboardEntry3);
						LeaderboardEntry leaderboardEntry4 = Leaderboards.CreateChallengeLeaderboardEntry(playerModel);
						leaderboardEntry4.Score = 0L;
						manager.ServerService.SaveLeaderboardEntry(Leaderboards.GetPlayerChallengeWeeklyCountryLeaderboardName(OldCountry, challengeId), leaderboardEntry4);
					}
				}
			}
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
