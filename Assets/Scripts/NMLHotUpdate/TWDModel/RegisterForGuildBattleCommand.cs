using BaseModel;

namespace TWDModel
{
	public class RegisterForGuildBattleCommand : TWDSocialModelCommand
	{
		private GuildBattleParticipantInfo playerInfo;

		public int SeasonDefinitionId { get; private set; }

		public int WarDefinitionId { get; private set; }

		public long TimeSlot { get; private set; }

		public RegisterForGuildBattleCommand()
		{
		}

		public RegisterForGuildBattleCommand(int seasonId, int warId, long timeslot)
		{
			SeasonDefinitionId = seasonId;
			WarDefinitionId = warId;
			TimeSlot = timeslot;
		}

		protected override TWDModelResult ValidateCommand(TWDModelManager modelManager)
		{
			GuildModel guildModel = modelManager.Player.GuildModel;
			if (!modelManager.Player.IsGuildMember)
			{
				modelManager.Debug.LogError("RegisterForGuildBattleCommand: Player is not a Guild Member");
				return TWDModelResult.Error;
			}
			GvGSeasonDefinition gvGSeasonDefinition = modelManager.GameEconomyData.FindGvGSeasonDefinition(SeasonDefinitionId);
			if (gvGSeasonDefinition == null)
			{
				modelManager.GvGLogError("RegisterForGuildBattleCommand: Could not find Season with id: " + SeasonDefinitionId, modelManager.Player);
				return TWDModelResult.Error;
			}
			if (!gvGSeasonDefinition.IsOpen(modelManager.Player.UtcTimeStamp))
			{
				modelManager.GvGLogError($"RegisterForGuildBattleCommand: Season not open Id {SeasonDefinitionId}, PlayerUtc: {modelManager.Player.UtcTimeStamp}, Definition: {gvGSeasonDefinition}", modelManager.Player);
				return TWDModelResult.Error;
			}
			GuildWarDefinition guildWarDefinition = modelManager.GameEconomyData.FindGuildWarWithId(WarDefinitionId);
			if (guildWarDefinition == null)
			{
				modelManager.GvGLogError("RegisterForGuildBattleCommand: Could not find War with id: " + WarDefinitionId);
				return TWDModelResult.Error;
			}
			if (!guildWarDefinition.IsOpen(modelManager.Player.UtcTimeStamp))
			{
				modelManager.GvGLogError($"RegisterForGuildBattleCommand: War not open Id {WarDefinitionId}, PlayerUtc: {modelManager.Player.UtcTimeStamp}, Definition: {guildWarDefinition}", modelManager.Player);
				return TWDModelResult.Error;
			}
			int matchmakingVersion = modelManager.GameEconomyData.GuildWarConfig.MatchmakingVersion;
			if (matchmakingVersion < guildModel.MatchmakingVersion)
			{
				modelManager.GvGLogWarning($"RegisterForGuildBattleCommand: player matchmakingversion [{matchmakingVersion}] is lower than group [{guildModel.MatchmakingVersion}]", guildModel);
				return TWDModelResult.Skip;
			}
			if (guildModel.GuildWarModel.GetWarAndRegisteredCount(modelManager.Player.UtcTimeStamp) >= modelManager.GameEconomyData.GuildWarConfig.GuildWarRegistrationLimit)
			{
				modelManager.GvGLogError("RegisterForGuildBattleCommand: Can't register player. Reached maximum players per war " + modelManager.Player.HashedId, modelManager.Player);
				return TWDModelResult.Error;
			}
			if (guildModel.GuildWarModel.IsBattleSlotLocked(TimeSlot, modelManager.Player.UtcTimeStamp))
			{
				modelManager.GvGLogError("RegisterForGuildBattleCommand: Can't register player in lockdown");
				return TWDModelResult.Error;
			}
			if (modelManager.Player.UtcTimeStamp >= TimeSlot)
			{
				modelManager.GvGLogError("RegisterForGuildBattleCommand: Can't register player for a past battle");
				return TWDModelResult.Error;
			}
			if (!guildModel.GuildWarModel.RegisteredPlayersForBattleSlot.ContainsKey(TimeSlot))
			{
				modelManager.GvGLogError("There is no timeslot " + TimeSlot + " for player" + modelManager.Player.HashedId, modelManager.Player);
				return TWDModelResult.Error;
			}
			if (modelManager.Player.GvGDefenders == null || modelManager.Player.GvGDefenders.Count == 0)
			{
				modelManager.GvGLogError("Player " + modelManager.Player.HashedId + " has not setup their defenders", modelManager.Player);
				return TWDModelResult.Error;
			}
			if (!guildModel.GuildWarModel.CanPlayerRegisterForBattleSlot(TimeSlot, modelManager.Player.HashedId, modelManager.Player.UtcTimeStamp))
			{
				modelManager.GvGLogError("RegisterForGuildBattleCommand: Can't register player " + modelManager.Player.HashedId, modelManager.Player);
				return TWDModelResult.Skip;
			}
			int num = guildModel.GuildWarModel.GetAllValidRegisteredDaysForPlayer(modelManager.Player.HashedId, modelManager.Player.UtcTimeStamp) + modelManager.Player.GvGSeasonModelPlayer.GuildWarModelPlayer.GetBattleParticipationsOnPreviousGuilds();
			if (modelManager.Player.GetCurrencyAmount(CurrencyType.BattlePass) <= num)
			{
				modelManager.GvGLog("Player cannot register in more than " + modelManager.Player.GetCurrencyAmount(CurrencyType.BattlePass) + " timeslots " + modelManager.Player.HashedId, modelManager.Player);
				return TWDModelResult.Skip;
			}
			return TWDModelResult.OK;
		}

		public override IModelCommandRespond Execute(ModelManager modelManager)
		{
			if (modelManager is TWDModelManager tWDModelManager)
			{
				GvGSeasonModelPlayer gvGSeasonModelPlayer = tWDModelManager.Player.GvGSeasonModelPlayer;
				playerInfo = GvGModelHelper.CreateEnemyPlayerData(tWDModelManager.Player, tWDModelManager.GameEconomyData);
				gvGSeasonModelPlayer.GuildWarModelPlayer.RegisteredBattleSlots.Add(TimeSlot);
				return base.Execute(modelManager) as NGModelCommandRespond;
			}
			return new NGModelCommandRespond(this, TWDModelResult.Error);
		}

		protected override GroupCommandBase CreateGroupCommand(TWDModelManager manager)
		{
			return new RegisterForGuildBattleGroupCommand(SeasonDefinitionId, WarDefinitionId, playerInfo, TimeSlot);
		}
	}
}
