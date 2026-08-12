using BaseModel;

namespace TWDModel
{
	public class UpdateGuildWarPlayerSnapshotGroupCommand : TWDGroupCommand
	{
		public GuildBattleParticipantInfo PlayerInfo { get; private set; }

		public int MatchmakingVersion { get; private set; }

		public UpdateGuildWarPlayerSnapshotGroupCommand(GuildBattleParticipantInfo playerInfo, int matchmakingVersion)
		{
			PlayerInfo = playerInfo;
			MatchmakingVersion = matchmakingVersion;
		}

		public override GroupCommandBase Execute(ModelManager manager)
		{
			TWDModelManager tWDModelManager = manager as TWDModelManager;
			GuildModel guildModel = (GuildModel)tWDModelManager.GetGroupModel(GroupId);
			if (guildModel == null)
			{
				manager.GvGLogError("UpdateGuildWarPlayerSnapshotGroupCommand: No Guild found with GroupId: " + GroupId);
				return this;
			}
			if (PlayerInfo == null)
			{
				manager.GvGLogError("UpdateGuildWarPlayerSnapshotGroupCommand: playerInfo null after deserialization", guildModel);
				return this;
			}
			if (MatchmakingVersion < guildModel.MatchmakingVersion)
			{
				manager.GvGLogWarning($"UpdateGuildWarPlayerSnapshotGroupCommand: player matchmakingversion [{MatchmakingVersion}] is lower than group [{guildModel.MatchmakingVersion}]", guildModel);
				return this;
			}
			if (!PlayerInfo.HasValidDefense())
			{
				manager.GvGLogError("UpdateGuildWarPlayerSnapshotGroupCommand: playerInfo doesn't have valid defenders", guildModel);
				return this;
			}
			GuildBattleMatchmakingInfo guildBattleMatchmakingInfo = guildModel.GuildBattleMatchmakingInfo;
			bool num = guildModel.UpdateMatchmakingVersion(MatchmakingVersion, ref guildBattleMatchmakingInfo);
			if (num)
			{
				manager.GvGLog("UpdateGuildWarPlayerSnapshotGroupCommand: Guild matchmaking version updated", guildModel);
			}
			if (num | guildBattleMatchmakingInfo.UpdateInfoOnPlayerChanged(guildModel.GuildWarModel, PlayerInfo))
			{
				manager.GvGLog("UpdateGuildWarPlayerSnapshotGroupCommand: snapshot updated", guildModel);
				SaveGroupModel(tWDModelManager);
			}
			return this;
		}
	}
}
