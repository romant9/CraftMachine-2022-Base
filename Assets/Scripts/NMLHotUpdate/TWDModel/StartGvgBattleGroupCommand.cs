using BaseModel;

namespace TWDModel
{
	public class StartGvgBattleGroupCommand : TWDValidationGroupCommand
	{
		private GuildBattleMatchmakingInfo matchmakingInfoDeserialized;

		protected GuildBattleOpponentMatchmakingEntry opponentMatchmakingEntry;

		public long TimeSlot { get; set; }

		protected override TWDValidationCommandResult Validate(ModelManager manager)
		{
			TWDModelManager tWDModelManager = manager as TWDModelManager;
			GuildModel guildModel = (GuildModel)tWDModelManager.GetGroupModel(GroupId);
			if (guildModel == null)
			{
				manager.GvGLogError("StartGvgBattleGroupCommand: No Guild found with GroupId: " + GroupId);
				return TWDValidationCommandResult.Error;
			}
			GuildWarDefinition currentWarDefinition = guildModel.GuildWarModel.CurrentWarDefinition;
			if (currentWarDefinition == null)
			{
				manager.GvGLogError("StartGvgBattleGroupCommand: no war model", guildModel);
				return TWDValidationCommandResult.Error;
			}
			if (!currentWarDefinition.IsOpen(guildModel.TimeStamp))
			{
				manager.GvGLogWarning("StartGvgBattleGroupCommand : Trying to start battle when war is not open");
				return TWDValidationCommandResult.Canceled;
			}
			opponentMatchmakingEntry = guildModel.GuildWarModel.GetNextGuildBattleOpponentMatchmakingEntry();
			if (opponentMatchmakingEntry == null)
			{
				manager.GvGLogError("StartGvgBattleGroupCommand: NextBattleOpponentMatchmakingInfo is null ", guildModel);
				return TWDValidationCommandResult.Error;
			}
			if (opponentMatchmakingEntry.StartBattleTimeSlot != TimeSlot)
			{
				manager.GvGLogError("StartGvgBattleGroupCommand: NextBattleOpponentMatchmakingInfo has a different timeslot than the one passed StartBattleTimeBattleStamp:" + opponentMatchmakingEntry.StartBattleTimeSlot + " Timeslot:" + TimeSlot, guildModel);
				return TWDValidationCommandResult.Error;
			}
			if (!string.IsNullOrEmpty(opponentMatchmakingEntry.OpponentMatchmakingInfo))
			{
				matchmakingInfoDeserialized = manager.GetMessageSerializer().DeserializeObject<GuildBattleMatchmakingInfo>(opponentMatchmakingEntry.OpponentMatchmakingInfo);
				if (matchmakingInfoDeserialized == null)
				{
					manager.GvGLogError("StartGvgBattleGroupCommand: Failed to deserialize the GuildBattleMatchmakingInfo", guildModel);
					return TWDValidationCommandResult.Error;
				}
				if (!opponentMatchmakingEntry.IsFakeBattle && matchmakingInfoDeserialized.RegisteredPlayersList.Count == 0)
				{
					manager.GvGLogError("StartGvgBattleGroupCommand: Starting battle with 0 participants", guildModel);
					return TWDValidationCommandResult.Error;
				}
				if (matchmakingInfoDeserialized.PlayerInfoSnapshot.Count < tWDModelManager.GameEconomyData.GuildWarConfig.MinPlayersToStartBattle)
				{
					manager.GvGLogError("StartGvgBattleGroupCommand: PlayerInfoSnapshot doesnt contain enough data", guildModel);
					return TWDValidationCommandResult.Error;
				}
			}
			GuildWarModel guildWarModel = guildModel.GuildWarModel;
			if (guildWarModel.CurrentBattle.IsOngoing(guildModel.TimeStamp))
			{
				manager.GvGLogWarning("StartGuildBattleGroupCommand : Trying to start battle when other battle is active");
				return TWDValidationCommandResult.Canceled;
			}
			long battleSlotForTimeStamp = guildWarModel.GetBattleSlotForTimeStamp(opponentMatchmakingEntry.StartBattleTimeSlot);
			if (!guildWarModel.HasEnoughRegisteredPlayersToStartBattleForTimeSlot(battleSlotForTimeStamp))
			{
				manager.GvGLogWarning("StartGuildBattleGroupCommand : Not enough players to start battle");
				return TWDValidationCommandResult.Canceled;
			}
			return TWDValidationCommandResult.OK;
		}

		protected override bool ExecuteInternal(ModelManager manager)
		{
			TWDModelManager tWDModelManager = manager as TWDModelManager;
			GuildModel guildModel = (GuildModel)tWDModelManager.GetGroupModel(GroupId);
			bool flag = false;
			if (matchmakingInfoDeserialized != null)
			{
				if (opponentMatchmakingEntry.IsFakeBattle)
				{
					GvGModelHelper.ObfuscateOpponentGuildData(matchmakingInfoDeserialized, tWDModelManager.GameEconomyData.FindFakeBattleDefinition(guildModel.GuildBattleTier), guildModel.GuildBattleTier, opponentMatchmakingEntry.RandomSeed);
				}
				flag |= guildModel.GuildWarModel.StartNewBattle(opponentMatchmakingEntry.RandomSeed, guildModel, matchmakingInfoDeserialized, tWDModelManager, TimeSlot, opponentMatchmakingEntry.IsFakeBattle);
				string text = (opponentMatchmakingEntry.IsFakeBattle ? "Fake " : "");
				manager.GvGLog("StartGvgBattleGroupCommand: " + text + "Battle started = " + opponentMatchmakingEntry.RandomSeed, guildModel);
				if (flag)
				{
					guildModel.UpdateGuildBattleLeaderboards(tWDModelManager, SenderId, guildModel.Id, guildModel.Name, battleEnd: false, updateMembers: false);
				}
				if (tWDModelManager.Player.HashedId == SenderId && guildModel.GuildRemotePushNotification.RemotePushDataExists(TimeSlot))
				{
					guildModel.GuildRemotePushNotification.CancelRemotePushNotification(tWDModelManager, TimeSlot, SenderId);
					BattleStartedRemoteNotification notification = new BattleStartedRemoteNotification(tWDModelManager, guildModel, TimeSlot);
					guildModel.GuildRemotePushNotification.TryToSendPushNotification(tWDModelManager, guildModel, SenderId, notification);
				}
				if (tWDModelManager.Player.HashedId == SenderId)
				{
					Metrics metrics = tWDModelManager.Metrics;
					metrics.AddStart();
					metrics.AddGvG(fromPlayer: false);
					metrics.AddBattleSignup(TimeSlot, guildModel.GuildWarModel.GetRegisteredPlayersForCurrentOrNextBattle(TimeSlot));
					metrics.Send();
				}
			}
			else
			{
				manager.GvGLogError("StartGvgBattleGroupCommand: No valid matchmaking result, requesting for a fake battle.", guildModel);
			}
			return flag;
		}
	}
}
