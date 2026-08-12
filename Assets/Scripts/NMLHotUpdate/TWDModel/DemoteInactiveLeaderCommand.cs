using BaseModel;

namespace TWDModel
{
	public class DemoteInactiveLeaderCommand : TWDSocialModelCommand
	{
		protected override TWDModelResult ValidateCommand(TWDModelManager modelManager)
		{
			PlayerModel player = modelManager.Player;
			GameEconomyData gameEconomyData = modelManager.GameEconomyData;
			if (player.IsGuildMember && player.GuildModel != null)
			{
				GuildMemberInfo leaderMemberInfo = player.GuildModel.GetLeaderMemberInfo();
				if (leaderMemberInfo == null)
				{
					if (player.GuildModel.SetUpNewLeader(gameEconomyData.ConfigData.LeaderInactivityTimeThreshold) == TWDModelResult.OK)
					{
						return TWDModelResult.OK;
					}
					modelManager.Debug.LogError("DemoteInactiveLeaderCommand: Guild " + player.GuildModel.Id + " has no leader");
					return TWDModelResult.Error;
				}
				if (leaderMemberInfo != null && player.UtcTimeStamp - leaderMemberInfo.LastActiveDate <= gameEconomyData.ConfigData.LeaderInactivityTimeThreshold)
				{
					modelManager.Debug.LogError($"DemoteInactiveLeaderCommand: Leader has not been inactive for more than {gameEconomyData.ConfigData.LeaderInactivityTimeThreshold} milliseconds");
					return TWDModelResult.Error;
				}
			}
			return TWDModelResult.OK;
		}

		protected override GroupCommandBase CreateGroupCommand(TWDModelManager modelManager)
		{
			return new DemoteInactiveLeaderGroupCommand();
		}
	}
}
