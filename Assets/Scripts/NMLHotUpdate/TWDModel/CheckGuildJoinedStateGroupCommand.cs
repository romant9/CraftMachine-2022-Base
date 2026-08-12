using System.Collections.Generic;
using BaseModel;

namespace TWDModel
{
	public class CheckGuildJoinedStateGroupCommand : TWDValidationGroupCommand
	{
		protected override TWDValidationCommandResult Validate(ModelManager manager)
		{
			GuildModel guildModel = (GuildModel)(manager as TWDModelManager).GetGroupModel(GroupId);
			if (guildModel == null)
			{
				manager.GvGLogError("CheckGuildJoinedStateGroupCommand: No Guild found with GroupId: " + GroupId);
				return TWDValidationCommandResult.Error;
			}
			GuildWarDefinition currentWarDefinition = guildModel.GuildWarModel.CurrentWarDefinition;
			if (currentWarDefinition == null)
			{
				manager.GvGLogError("CheckGuildJoinedStateGroupCommand: no war model", guildModel);
				return TWDValidationCommandResult.Error;
			}
			if (!currentWarDefinition.IsOpen(guildModel.TimeStamp))
			{
				manager.GvGLogWarning("CheckGuildJoinedStateGroupCommand : The war is not open");
				return TWDValidationCommandResult.Canceled;
			}
			return TWDValidationCommandResult.OK;
		}

		protected override bool ExecuteInternal(ModelManager manager)
		{
			TWDModelManager tWDModelManager = (TWDModelManager)manager;
			if (tWDModelManager.ServerService != null && tWDModelManager.Player.HashedId == SenderId)
			{
				GuildModel guildModel = (GuildModel)tWDModelManager.GetGroupModel(GroupId);
				if (guildModel != null)
				{
					GuildWarModel guildWarModel = guildModel.GuildWarModel;
					foreach (KeyValuePair<long, List<string>> item in guildModel.GuildWarModel.RegisteredPlayersForBattleSlot)
					{
						if (item.Value.Count == tWDModelManager.GameEconomyData.GuildWarConfig.MinPlayersToStartBattle && guildWarModel.CurrentBattle.TimeSlot != item.Key && !guildWarModel.GuildBattleResults.ContainsKey(item.Key) && !guildWarModel.IsBattleSlotLocked(item.Key, tWDModelManager.Player.UtcTimeStamp) && !tWDModelManager.ServerService.GvgHasJoinedBattle(GroupId, guildModel.GuildWarModel.GetLockDownTimeForBattleSlot(item.Key)))
						{
							guildModel.GuildBattleMatchmakingInfo.RegisteredPlayersList = new List<string>(guildModel.GuildWarModel.RegisteredPlayersForBattleSlot[item.Key]);
							string guildBattleMatchmakingInfo = tWDModelManager.GetMessageSerializer().Serialize(guildModel.GuildBattleMatchmakingInfo);
							GvgBattleEntry gvgBattleEntry = new GvgBattleEntry
							{
								GroupId = GroupId,
								MatchmakingEpochMsec = guildModel.GuildWarModel.GetLockDownTimeForBattleSlot(item.Key),
								StartBattleTimestamp = item.Key,
								Tier = guildModel.GuildBattleTier,
								MatchmakingVersion = guildModel.MatchmakingVersion,
								GuildBattleMatchmakingInfo = guildBattleMatchmakingInfo,
								RegisteredPlayers = guildModel.GuildBattleMatchmakingInfo.RegisteredPlayersList.Count,
								VictoryPoints = guildModel.CurrentVictoryPoints,
								LastOpponents = guildModel.GuildWarModel.GetAllOpponentsGroupIds()
							};
							gvgBattleEntry.SetRegisteredPlayersList(guildModel.GuildBattleMatchmakingInfo.RegisteredPlayersList);
							if (!tWDModelManager.ServerService.GvgJoinBattle(gvgBattleEntry))
							{
								tWDModelManager.GvGLogError("CheckGuildJoinedStateGroupCommand: Couldn't save the matchmaking info - " + SenderId, guildModel);
							}
							else
							{
								guildModel.GuildWarModel.AddBattleEntry(item.Key, gvgBattleEntry);
							}
						}
					}
				}
			}
			return false;
		}
	}
}
