using System.Collections.Generic;
using BaseModel;

namespace TWDModel
{
	public class ResignFromGuildBattleGroupCommand : TWDValidationGroupCommand
	{
		public int SeasonDefinitionId { get; private set; }

		public int WarDefinitionId { get; private set; }

		public string PlayerName { get; private set; }

		public long Timeslot { get; private set; }

		public ResignFromGuildBattleGroupCommand()
		{
		}

		public ResignFromGuildBattleGroupCommand(int seasonId, int warId, string playerName, long timeslot)
		{
			WarDefinitionId = warId;
			SeasonDefinitionId = seasonId;
			PlayerName = playerName;
			Timeslot = timeslot;
		}

		protected override TWDValidationCommandResult Validate(ModelManager manager)
		{
			GuildModel guildModel = (GuildModel)manager.GetGroupModel(GroupId);
			if (guildModel == null)
			{
				manager.GvGLogError("ResignFromGuildBattleGroupCommand: No Guild found with GroupId: " + GroupId, guildModel);
				return TWDValidationCommandResult.Error;
			}
			if (!guildModel.GvGSeasonModel.IsCurrentSeasonOpen(guildModel.TimeStamp))
			{
				manager.GvGLogError("StartGuildWarGroupCommand: The season has not been started: " + GroupId, guildModel);
				return TWDValidationCommandResult.Error;
			}
			manager.GvGLog("ResignFromGuildBattleGroupCommand#" + GroupId + "#" + SenderId);
			return TWDValidationCommandResult.OK;
		}

		protected override bool ExecuteInternal(ModelManager modelManager)
		{
			TWDModelManager tWDModelManager = modelManager as TWDModelManager;
			GuildModel guildModel = (GuildModel)modelManager.GetGroupModel(GroupId);
			modelManager.GvGLog("ResignFromGuildBattleGroupCommand: Trying to resign player - " + SenderId, guildModel);
			int num = 0 | (guildModel.GuildWarModel.ResignPlayerFromBattle(SenderId, tWDModelManager.Player.UtcTimeStamp, Timeslot) ? 1 : 0);
			if (num != 0)
			{
				guildModel.GuildBattleMatchmakingInfo.UpdateInfoOnPlayerChanged(guildModel.GuildWarModel, null);
				tWDModelManager.GvGLog("ResignFromGuildBattleGroupCommand: Resign successful - " + SenderId, guildModel);
				if (tWDModelManager.Player.HashedId == SenderId && tWDModelManager.ServerService != null)
				{
					if (guildModel.GvGSeasonModel.GuildWarModel.RegisteredPlayersForBattleSlot[Timeslot].Count == tWDModelManager.GameEconomyData.GuildWarConfig.MinPlayersToStartBattle - 1)
					{
						guildModel.GuildRemotePushNotification.CancelRemotePushNotification(tWDModelManager, Timeslot, SenderId);
						tWDModelManager.ServerService.GvgLeaveBattle(GroupId, guildModel.GuildWarModel.GetLockDownTimeForBattleSlot(Timeslot));
						guildModel.GuildWarModel.RemoveBattleEntry(Timeslot);
						return (byte)num != 0;
					}
					if (guildModel.GvGSeasonModel.GuildWarModel.RegisteredPlayersForBattleSlot[Timeslot].Count >= tWDModelManager.GameEconomyData.GuildWarConfig.MinPlayersToStartBattle)
					{
						guildModel.GuildBattleMatchmakingInfo.RegisteredPlayersList = new List<string>(guildModel.GuildWarModel.RegisteredPlayersForBattleSlot[Timeslot]);
						string guildBattleMatchmakingInfo = tWDModelManager.GetMessageSerializer().Serialize(guildModel.GuildBattleMatchmakingInfo);
						GvgBattleEntry gvgBattleEntry = new GvgBattleEntry
						{
							GroupId = GroupId,
							MatchmakingEpochMsec = guildModel.GuildWarModel.GetLockDownTimeForBattleSlot(Timeslot),
							StartBattleTimestamp = Timeslot,
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
							return (byte)num != 0;
						}
						guildModel.GuildWarModel.AddBattleEntry(Timeslot, gvgBattleEntry);
						tWDModelManager.GvGLog("RegisterForGuildBattleGroupCommand: matchmaking stored with matchmaking time:" + guildModel.GuildWarModel.GetLockDownTimeForBattleSlot(Timeslot) + " " + SenderId, guildModel);
					}
				}
			}
			return (byte)num != 0;
		}
	}
}
