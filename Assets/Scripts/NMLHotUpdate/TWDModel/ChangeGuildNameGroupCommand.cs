using System.Collections.Generic;
using BaseModel;

namespace TWDModel
{
	public class ChangeGuildNameGroupCommand : TWDValidationGroupCommand
	{
		public string Name { get; set; }

		protected override TWDValidationCommandResult Validate(ModelManager manager)
		{
			GuildModel guildModel = (GuildModel)manager.GetGroupModel(GroupId);
			_ = ((TWDModelManager)manager).GameEconomyData;
			if (guildModel == null)
			{
				manager.Debug.LogError("ChangeGuildNameGroupCommand: No Guild found with GroupId: " + GroupId);
				return TWDValidationCommandResult.Error;
			}
			if (guildModel.ChangeNameColdTimeSeconds > 0)
			{
				manager.Debug.LogError($"ChangeGuildNameGroupCommand: has been cold Time {guildModel.ChangeNameColdTimeSeconds} seconds");
				return TWDValidationCommandResult.Error;
			}
			return TWDValidationCommandResult.OK;
		}

		protected override bool ExecuteInternal(ModelManager manager)
		{
			TWDModelManager tWDModelManager = manager as TWDModelManager;
			GameEconomyData gameEconomyData = ((TWDModelManager)manager).GameEconomyData;
			GuildModel guildModel = (GuildModel)manager.GetGroupModel(GroupId);
			GuildMemberInfo memberInfo = guildModel.GetMemberInfo(SenderId);
			if (memberInfo == null)
			{
				manager.Debug.LogError("ChangeGuildNameGroupCommand: SenderID:" + SenderId + " Had Not MemeberInfo");
				return false;
			}
			guildModel.AddGuildNameChangedNotification(SenderId, memberInfo.Name);
			guildModel.Name = Name;
			guildModel.GuildBattleMatchmakingInfo.GuildName = Name;
			guildModel.NextChangeNameTimeStampSeconds = (guildModel.TimeStamp + gameEconomyData.ConfigData.ChangeGuildNameColdTime) / 1000;
			if (tWDModelManager.ServerService != null && tWDModelManager.Player.HashedId == SenderId)
			{
				foreach (KeyValuePair<long, List<string>> item in guildModel.GvGSeasonModel.GuildWarModel.RegisteredPlayersForBattleSlot)
				{
					if (!guildModel.GvGSeasonModel.GuildWarModel.IsBattleSlotLocked(item.Key, guildModel.TimeStamp) && item.Value.Count >= tWDModelManager.GameEconomyData.GuildWarConfig.MinPlayersToStartBattle)
					{
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
							tWDModelManager.GvGLogError("RegisterForGuildBattleGroupCommand: Couldn't save the matchmaking info - " + SenderId, guildModel);
							continue;
						}
						guildModel.GuildWarModel.AddBattleEntry(item.Key, gvgBattleEntry);
						tWDModelManager.GvGLog("RegisterForGuildBattleGroupCommand: matchmaking stored with matchmaking time:" + guildModel.GuildWarModel.GetLockDownTimeForBattleSlot(item.Key) + " " + SenderId, guildModel);
					}
				}
				WorldBossModelManager worldBossModelManager = tWDModelManager.Player.WorldBossModelManager;
				int num = worldBossModelManager?.GetCurrentSeasonId() ?? 0;
				int num2 = worldBossModelManager?.GetCurrentCycleId() ?? 0;
				if (num > 0 && num2 > 0)
				{
					WorldBossOperationResult worldBossOperationResult = tWDModelManager.ServerService.WorldBossUpdateGuildName(new WorldBossUpdateGuildNameOperationRequest
					{
						GroupId = GroupId,
						PlayerHashedId = SenderId,
						SeasonId = num,
						CycleId = num2,
						GuildName = Name
					});
					if (worldBossOperationResult == null || !worldBossOperationResult.Success)
					{
						manager.Debug.LogError($"ChangeGuildNameGroupCommand: WorldBossUpdateGuildName failed. GroupId:{GroupId} SeasonId:{num} CycleId:{num2} Message:{worldBossOperationResult?.Message}");
					}
				}
			}
			guildModel.NotifyChange("GuildNameChanged");
			return true;
		}
	}
}
