using BaseModel;

namespace TWDModel
{
	public class RequestMatchmakingEntryCommand : TWDSocialModelCommand
	{
		private IGvgBattleOpponentMatchmakingEntry opponent;

		public long TimeSlot { get; private set; }

		public RequestMatchmakingEntryCommand()
		{
		}

		public RequestMatchmakingEntryCommand(long timeSlot)
		{
			TimeSlot = timeSlot;
		}

		protected override TWDModelResult ValidateCommand(TWDModelManager modelManager)
		{
			if (!modelManager.Player.IsGuildMember)
			{
				modelManager.Debug.LogError("RequestMatchmakingEntryCommand: Player is not a Guild Member");
				return TWDModelResult.Error;
			}
			GuildWarModel guildWarModel = modelManager.Player.GuildWarModel;
			GvGSeasonModel gvGSeasonModel = modelManager.Player.GvGSeasonModel;
			if (guildWarModel.NextBattlesOpponentMatchmakingInfo.Exists((GuildBattleOpponentMatchmakingEntry m) => m.StartBattleTimeSlot == TimeSlot))
			{
				modelManager.GvGLog("RequestMatchmakingEntryCommand: Matchmaking Entry Exists");
				return TWDModelResult.Skip;
			}
			if (!guildWarModel.RegisteredPlayersForBattleSlot.ContainsKey(TimeSlot) || !guildWarModel.HasEnoughRegisteredPlayersToStartBattleForTimeSlot(TimeSlot))
			{
				modelManager.GvGLog("RequestMatchmakingEntryCommand: There are not enough players registers for the TimeSlot");
				return TWDModelResult.Skip;
			}
			if (modelManager.Player.GvGSeasonModelPlayer.GuildWarModelPlayer.LastOpponentRequestTime + UtilsDateTime.HourInMilliseconds > modelManager.Player.UtcTimeStamp)
			{
				modelManager.GvGLog("RequestMatchmakingEntryCommand: Time delay has not been ellapsed yet");
				return TWDModelResult.Skip;
			}
			if (gvGSeasonModel.BattleLog.ContainsKey(guildWarModel.WarDefinitionId) && gvGSeasonModel.BattleLog[guildWarModel.WarDefinitionId].Exists((GvGSeasonModel.GuildBattleLogEntry b) => b.EndedTimeStamp > TimeSlot))
			{
				modelManager.GvGLog("RequestMatchmakingEntryCommand: Battle already happpened");
				return TWDModelResult.Skip;
			}
			modelManager.Player.GvGSeasonModelPlayer.GuildWarModelPlayer.LastOpponentRequestTime = modelManager.Player.UtcTimeStamp;
			if (modelManager.ServerService != null)
			{
				opponent = modelManager.ServerService.GvgGetBattleOpponent(modelManager.Player.GuildId, guildWarModel.GetLockDownTimeForBattleSlot(TimeSlot));
				if (opponent == null)
				{
					modelManager.GvGLogWarning("RequestMatchmakingEntryCommand: No opponent found in the matchmaking entry");
					return TWDModelResult.Skip;
				}
			}
			return TWDModelResult.OK;
		}

		protected override GroupCommandBase CreateGroupCommand(TWDModelManager modelManager)
		{
			if (modelManager.ServerService != null)
			{
				return new SetNextBattleMatchmakingInfoGroupCommand(opponent.RandomSeed, opponent.GuildBattleMatchmakingInfo, opponent.IsFakeBattle, TimeSlot);
			}
			if (HelpersModel.IsOfflineMode)
			{
				GuildModel guildModel = modelManager.Player.GuildModel;
				return new SetNextBattleMatchmakingInfoGroupCommand((int)guildModel.TimeStamp, modelManager.GetMessageSerializer().SerializeObject(guildModel.GuildBattleMatchmakingInfo), isFakeBattle: true, TimeSlot);
			}
			return new SetNextBattleMatchmakingInfoGroupCommand();
		}
	}
}
