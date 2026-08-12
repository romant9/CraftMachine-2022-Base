using BaseModel;

namespace TWDModel
{
	public class DemoteInactiveLeaderGroupCommand : TWDValidationGroupCommand
	{
		protected override TWDValidationCommandResult Validate(ModelManager manager)
		{
			GuildModel guildModel = (GuildModel)manager.GetGroupModel(GroupId);
			GameEconomyData gameEconomyData = ((TWDModelManager)manager).GameEconomyData;
			if (guildModel == null)
			{
				manager.Debug.LogError("DemoteInactiveLeaderGroupCommand: No Guild found with GroupId: " + GroupId);
				return TWDValidationCommandResult.Error;
			}
			GuildMemberInfo leaderMemberInfo = guildModel.GetLeaderMemberInfo();
			if (leaderMemberInfo == null)
			{
				manager.Debug.LogError("DemoteInactiveLeaderGroupCommand: Guild " + GroupId + " has no leader");
				return TWDValidationCommandResult.Error;
			}
			if (guildModel != null && guildModel.TimeStamp - leaderMemberInfo.LastActiveDate <= gameEconomyData.ConfigData.LeaderInactivityTimeThreshold)
			{
				manager.Debug.LogError($"DemoteInactiveLeaderGroupCommand: Leader has not been inactive for more than {gameEconomyData.ConfigData.LeaderInactivityTimeThreshold} milliseconds");
				return TWDValidationCommandResult.Error;
			}
			return TWDValidationCommandResult.OK;
		}

		protected override bool ExecuteInternal(ModelManager modelManager)
		{
			GuildModel obj = (GuildModel)modelManager.GetGroupModel(GroupId);
			GameEconomyData gameEconomyData = ((TWDModelManager)modelManager).GameEconomyData;
			if (obj.DemoteLeader(gameEconomyData.ConfigData.LeaderInactivityTimeThreshold) == TWDModelResult.OK)
			{
				return true;
			}
			return false;
		}
	}
}
