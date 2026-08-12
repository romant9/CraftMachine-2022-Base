using BaseModel;

namespace TWDModel
{
	public class AddChallengeStarsGroupCommand : TWDGroupCommand
	{
		public string MemberId;

		public string ChallengeId;

		public int NewCurrentChallengeStars;

		public bool IsChallengeFinished;

		public AddChallengeStarsGroupCommand()
		{
		}

		public AddChallengeStarsGroupCommand(string memberId, string challengeId, int totalMemberChallengeStars)
		{
		}

		public override GroupCommandBase Execute(ModelManager manager)
		{
			GuildModel guildModel = (GuildModel)manager.GetGroupModel(GroupId);
			if (guildModel != null)
			{
				TWDModelResult tWDModelResult = guildModel.SetChallengeStars(ChallengeId, MemberId, NewCurrentChallengeStars, IsChallengeFinished);
				if (tWDModelResult == TWDModelResult.OK)
				{
					SaveGroupModel(manager);
					IServerService serverService = manager.ServerService;
					if (serverService != null && SenderId == manager.GetPlayer().HashedId)
					{
						LeaderboardEntry entry = Leaderboards.CreateChallengeLeaderboardEntry(guildModel, manager);
						serverService.SaveLeaderboardEntry(Leaderboards.GetGuildChallengeGlobalLeaderboardName(ChallengeId), entry);
						serverService.SaveLeaderboardEntry(Leaderboards.GetGuildChallengeCountryLeaderboardName(guildModel.CountryCode.ToLower(), ChallengeId), entry);
					}
				}
				else
				{
					manager.Debug.LogError("GuildModel.SetChallengeStars failed, reason: '" + tWDModelResult.ToString() + "', ChallengeId: '" + ChallengeId + "', MemberId: '" + MemberId + "', groupModel.Id: '" + guildModel.Id + "', groupModel.CurrentChallengeId: '" + guildModel.CurrentChallengeId + "'");
				}
				guildModel.NotifyChange("GuildModified");
			}
			return this;
		}
	}
}
