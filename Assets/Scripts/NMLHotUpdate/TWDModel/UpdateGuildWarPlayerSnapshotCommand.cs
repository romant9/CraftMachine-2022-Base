using BaseModel;

namespace TWDModel
{
	public class UpdateGuildWarPlayerSnapshotCommand : TWDSocialModelCommand
	{
		private int matchmakingVersion;

		public GuildBattleParticipantInfo PlayerInfo { get; private set; }

		public UpdateGuildWarPlayerSnapshotCommand()
		{
		}

		public UpdateGuildWarPlayerSnapshotCommand(GuildBattleParticipantInfo playerInfo)
		{
			PlayerInfo = playerInfo;
		}

		protected override TWDModelResult ValidateCommand(TWDModelManager modelManager)
		{
			GuildModel guildModel = modelManager.Player.GuildModel;
			if (!modelManager.Player.IsGuildMember)
			{
				modelManager.GvGLogWarning("UpdateGuildWarPlayerSnapshotGroupCommand: Skip - Player is not a Guild Member");
				return TWDModelResult.Skip;
			}
			if (PlayerInfo == null)
			{
				modelManager.GvGLogError("UpdateGuildWarPlayerSnapshotGroupCommand: No player info", modelManager.Player);
				return TWDModelResult.Error;
			}
			matchmakingVersion = modelManager.GameEconomyData.GuildWarConfig.MatchmakingVersion;
			if (matchmakingVersion < guildModel.MatchmakingVersion)
			{
				modelManager.GvGLogWarning($"UpdateGuildWarPlayerSnapshotGroupCommand: player matchmakingversion [{matchmakingVersion}] is lower than group [{guildModel.MatchmakingVersion}]", guildModel);
				return TWDModelResult.Skip;
			}
			if (guildModel.GuildBattleMatchmakingInfo.ShouldUpdateGuildBattlePlayerSnapshot(PlayerInfo) || guildModel.MatchmakingVersion < modelManager.Player.gameEconomyData.GuildWarConfig.MatchmakingVersion)
			{
				return TWDModelResult.OK;
			}
			modelManager.GvGLogWarning("UpdateGuildWarPlayerSnapshotGroupCommand: Cancelled - Player snapshot and matchmaking version not changed", modelManager.Player);
			return TWDModelResult.Skip;
		}

		protected override GroupCommandBase CreateGroupCommand(TWDModelManager manager)
		{
			return new UpdateGuildWarPlayerSnapshotGroupCommand(PlayerInfo, matchmakingVersion);
		}
	}
}
