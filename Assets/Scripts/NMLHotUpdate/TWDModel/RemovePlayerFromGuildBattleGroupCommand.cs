using System.Collections.Generic;
using BaseModel;

namespace TWDModel
{
	public class RemovePlayerFromGuildBattleGroupCommand : TWDValidationGroupCommand
	{
		public string RemovedPlayerId;

		public int SeasonDefinitionId { get; private set; }

		public int WarDefinitionId { get; private set; }

		public string PlayerName { get; private set; }

		public long Timeslot { get; private set; }

		public NotificationHubSendPushRequest NotificationHubSendPushRequest { get; set; }

		public RemovePlayerFromGuildBattleGroupCommand()
		{
		}

		public RemovePlayerFromGuildBattleGroupCommand(int seasonId, int warId, string playerName, long timeslot, string removedPlayerId, NotificationHubSendPushRequest notificationHubSendPushRequest)
		{
			WarDefinitionId = warId;
			SeasonDefinitionId = seasonId;
			PlayerName = playerName;
			Timeslot = timeslot;
			RemovedPlayerId = removedPlayerId;
			NotificationHubSendPushRequest = notificationHubSendPushRequest;
		}

		protected override TWDValidationCommandResult Validate(ModelManager manager)
		{
			GuildModel guildModel = (GuildModel)manager.GetGroupModel(GroupId);
			if (guildModel == null)
			{
				manager.GvGLogError("RemovePlayerFromGuildBattleGroupCommand: No Guild found with GroupId: " + GroupId, guildModel);
				return TWDValidationCommandResult.Error;
			}
			if (!guildModel.GvGSeasonModel.IsCurrentSeasonOpen(guildModel.TimeStamp))
			{
				manager.GvGLogError("RemovePlayerFromGuildBattleGroupCommand: The season has not been started: " + GroupId, guildModel);
				return TWDValidationCommandResult.Error;
			}
			return TWDValidationCommandResult.OK;
		}

		protected override bool ExecuteInternal(ModelManager modelManager)
		{
			TWDModelManager tWDModelManager = modelManager as TWDModelManager;
			GuildModel guildModel = (GuildModel)modelManager.GetGroupModel(GroupId);
			modelManager.GvGLog("RemovePlayerFromGuildBattleGroupCommand: Trying to remove player - " + RemovedPlayerId, guildModel);
			int num = 0 | (guildModel.GuildWarModel.ResignPlayerFromBattle(RemovedPlayerId, tWDModelManager.Player.UtcTimeStamp, Timeslot) ? 1 : 0);
			if (num != 0)
			{
				guildModel.GuildBattleMatchmakingInfo.UpdateInfoOnPlayerChanged(guildModel.GuildWarModel, null);
				tWDModelManager.GvGLog("RemovePlayerFromGuildBattleGroupCommand: Remove successful - " + SenderId, guildModel);
				if (tWDModelManager.Player.HashedId == SenderId && tWDModelManager.ServerService != null)
				{
					if (guildModel.GvGSeasonModel.GuildWarModel.RegisteredPlayersForBattleSlot[Timeslot].Count == tWDModelManager.GameEconomyData.GuildWarConfig.MinPlayersToStartBattle - 1)
					{
						guildModel.GuildRemotePushNotification.CancelRemotePushNotification(tWDModelManager, Timeslot, SenderId);
						tWDModelManager.ServerService.GvgLeaveBattle(GroupId, guildModel.GuildWarModel.GetLockDownTimeForBattleSlot(Timeslot));
						guildModel.GuildWarModel.RemoveBattleEntry(Timeslot);
					}
					else if (guildModel.GvGSeasonModel.GuildWarModel.RegisteredPlayersForBattleSlot[Timeslot].Count >= tWDModelManager.GameEconomyData.GuildWarConfig.MinPlayersToStartBattle)
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
							tWDModelManager.GvGLogError("RemovePlayerFromGuildBattleGroupCommand: Couldn't save the matchmaking info - " + SenderId, guildModel);
						}
						else
						{
							guildModel.GuildWarModel.AddBattleEntry(Timeslot, gvgBattleEntry);
							tWDModelManager.GvGLog("RemovePlayerFromGuildBattleGroupCommand: matchmaking stored with matchmaking time:" + guildModel.GuildWarModel.GetLockDownTimeForBattleSlot(Timeslot) + " " + SenderId, guildModel);
						}
					}
					SendPushNotificationToRemovedPlayer(tWDModelManager, NotificationHubSendPushRequest);
				}
			}
			return (byte)num != 0;
		}

		protected void SendPushNotificationToRemovedPlayer(TWDModelManager modelManager, NotificationHubSendPushRequest notificationHubSendPushRequest)
		{
			if (modelManager.ServerService != null)
			{
				notificationHubSendPushRequest.AndroidTitle = modelManager.Player.ValidateStringsAgainstProfanity(notificationHubSendPushRequest.AndroidTitle);
				notificationHubSendPushRequest.Message = modelManager.Player.ValidateStringsAgainstProfanity(notificationHubSendPushRequest.Message);
				modelManager.ServerService.SendRemotePush(notificationHubSendPushRequest);
			}
		}
	}
}
