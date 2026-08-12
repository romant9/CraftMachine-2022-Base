using System.Collections.Generic;
using BaseModel;

namespace TWDModel
{
	public class RegisterForGuildBattleGroupCommand : TWDValidationGroupCommand
	{
		public int SeasonDefinitionId { get; private set; }

		public int WarDefinitionId { get; private set; }

		public GuildBattleParticipantInfo PlayerInfo { get; private set; }

		public long TimeSlot { get; private set; }

		public RegisterForGuildBattleGroupCommand()
		{
		}

		public RegisterForGuildBattleGroupCommand(int seasonId, int warId, GuildBattleParticipantInfo playerInfo, long timeSlot)
		{
			WarDefinitionId = warId;
			SeasonDefinitionId = seasonId;
			PlayerInfo = playerInfo;
			TimeSlot = timeSlot;
		}

		protected override TWDValidationCommandResult Validate(ModelManager manager)
		{
			GuildModel guildModel = (GuildModel)manager.GetGroupModel(GroupId);
			if (guildModel == null)
			{
				manager.GvGLogError("RegisterForGuildBattleGroupCommand: No Guild found with GroupId: " + GroupId);
				return TWDValidationCommandResult.Error;
			}
			if (!guildModel.GvGSeasonModel.IsCurrentSeasonOpen(guildModel.TimeStamp))
			{
				manager.GvGLogError("RegisterForGuildBattleGroupCommand: The season has not been started: " + SeasonDefinitionId, guildModel);
				return TWDValidationCommandResult.Error;
			}
			if (guildModel.GvGSeasonModel.GuildWarModel.CurrentBattle.IsOngoing(TimeSlot))
			{
				manager.GvGLogError("RegisterForGuildBattleGroupCommand: The player cannot join an ongoing battle");
				return TWDValidationCommandResult.Error;
			}
			if (guildModel.GvGSeasonModel.GuildWarModel.IsBattleSlotLocked(TimeSlot, guildModel.TimeStamp))
			{
				manager.GvGLogError("RegisterForGuildBattleGroupCommand: The player cannot join during lockdown");
				return TWDValidationCommandResult.Error;
			}
			if (!PlayerInfo.HasValidDefense())
			{
				manager.GvGLogError("RegisterForGuildBattleGroupCommand: The player has not setup their defenders");
				return TWDValidationCommandResult.Error;
			}
			if (SenderId == ((TWDModelManager)manager).Player.HashedId)
			{
				PlayerModel player = ((TWDModelManager)manager).Player;
				if (player.GetCurrencyAmount(CurrencyType.BattlePass) <= guildModel.GuildWarModel.GetAllValidRegisteredDaysForPlayer(player.HashedId, player.UtcTimeStamp))
				{
					manager.GvGLogError("RegisterForGuildBattleGroupCommand: Player cannot register in more than " + player.GetCurrencyAmount(CurrencyType.BattlePass) + " timeslots " + player.HashedId, player);
					return TWDValidationCommandResult.Error;
				}
				manager.GvGLog("RegisterForGuildBattleGroupCommand#" + GroupId + "#" + SenderId);
			}
			return TWDValidationCommandResult.OK;
		}

		protected override bool ExecuteInternal(ModelManager modelManager)
		{
			TWDModelManager tWDModelManager = modelManager as TWDModelManager;
			GuildModel guildModel = (GuildModel)tWDModelManager.GetGroupModel(GroupId);
			TWDModelResult result;
			int num = 0 | (guildModel.GuildWarModel.RegisterPlayerForBattle(SenderId, out result, TimeSlot, tWDModelManager.Player.UtcTimeStamp) ? 1 : 0);
			if (num != 0)
			{
				guildModel.GuildBattleMatchmakingInfo.UpdateInfoOnPlayerChanged(guildModel.GuildWarModel, PlayerInfo);
				if (SenderId == tWDModelManager.Player.HashedId && tWDModelManager.ServerService != null && guildModel.GuildWarModel.HasEnoughRegisteredPlayersToStartBattleForTimeSlot(TimeSlot))
				{
					guildModel.GuildRemotePushNotification.CancelRemotePushNotification(tWDModelManager, TimeSlot, SenderId);
					BattleStartedRemoteNotification notification = new BattleStartedRemoteNotification(tWDModelManager, guildModel, TimeSlot);
					guildModel.GuildRemotePushNotification.TryToSendPushNotification(tWDModelManager, guildModel, SenderId, notification);
					guildModel.GuildBattleMatchmakingInfo.RegisteredPlayersList = new List<string>(guildModel.GuildWarModel.RegisteredPlayersForBattleSlot[TimeSlot]);
					string guildBattleMatchmakingInfo = tWDModelManager.GetMessageSerializer().Serialize(guildModel.GuildBattleMatchmakingInfo);
					GvgBattleEntry gvgBattleEntry = new GvgBattleEntry
					{
						GroupId = GroupId,
						MatchmakingEpochMsec = guildModel.GuildWarModel.GetLockDownTimeForBattleSlot(TimeSlot),
						StartBattleTimestamp = TimeSlot,
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
					}
					else
					{
						guildModel.GuildWarModel.AddBattleEntry(TimeSlot, gvgBattleEntry);
						tWDModelManager.GvGLog("RegisterForGuildBattleGroupCommand: matchmaking stored with matchmaking time:" + guildModel.GuildWarModel.GetLockDownTimeForBattleSlot(TimeSlot) + " " + SenderId, guildModel);
					}
				}
				tWDModelManager.GvGLog("RegisterForGuildBattleGroupCommand: Player registered for battle - " + SenderId, guildModel);
			}
			return (byte)num != 0;
		}
	}
}
