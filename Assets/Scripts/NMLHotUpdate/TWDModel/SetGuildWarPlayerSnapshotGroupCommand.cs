using System.Collections.Generic;
using BaseModel;

namespace TWDModel
{
	public class SetGuildWarPlayerSnapshotGroupCommand : TWDGroupCommand
	{
		public GuildBattleParticipantInfo PlayerInfo { get; private set; }

		public int MatchmakingVersion { get; private set; }

		public long MatchmakingEpochMsec { get; private set; }

		public long StartBattleTimestamp { get; private set; }

		public SetGuildWarPlayerSnapshotGroupCommand(GuildBattleParticipantInfo playerInfo, int matchmakingVersion, long matchmakingEpochMsec, long startBattleTimestamp)
		{
			PlayerInfo = playerInfo;
			MatchmakingVersion = matchmakingVersion;
			MatchmakingEpochMsec = matchmakingEpochMsec;
			StartBattleTimestamp = startBattleTimestamp;
		}

		public override GroupCommandBase Execute(ModelManager manager)
		{
			TWDModelManager tWDModelManager = manager as TWDModelManager;
			GuildModel guildModel = (GuildModel)tWDModelManager.GetGroupModel(GroupId);
			if (guildModel == null)
			{
				manager.GvGLogError("SetGuildWarPlayerSnapshotGroupCommand: No Guild found with GroupId: " + GroupId);
				return this;
			}
			if (PlayerInfo == null)
			{
				manager.GvGLogError("SetGuildWarPlayerSnapshotGroupCommand: playerInfo null after deserialization", guildModel);
				return this;
			}
			if (MatchmakingVersion < guildModel.MatchmakingVersion)
			{
				manager.GvGLogWarning($"SetGuildWarPlayerSnapshotGroupCommand: player matchmakingversion [{MatchmakingVersion}] is lower than group [{guildModel.MatchmakingVersion}]", guildModel);
				return this;
			}
			GuildBattleMatchmakingInfo guildBattleMatchmakingInfo = guildModel.GuildBattleMatchmakingInfo;
			guildBattleMatchmakingInfo.DeleteGuildBattlePlayerSnapshot(PlayerInfo.HashedPlayerId);
			if (guildModel.UpdateMatchmakingVersion(MatchmakingVersion, ref guildBattleMatchmakingInfo))
			{
				manager.GvGLog("SetGuildWarPlayerSnapshotGroupCommand: Guild matchmaking version updated", guildModel);
			}
			guildBattleMatchmakingInfo.UpdateInfoOnPlayerChanged(guildModel.GuildWarModel, PlayerInfo);
			manager.GvGLog("SetGuildWarPlayerSnapshotGroupCommand: snapshot updated", guildModel);
			List<string> list = new List<string>();
			for (int i = 0; i < tWDModelManager.GameEconomyData.GuildWarConfig.MaxPlayerCountInBattle; i++)
			{
				list.Add(SenderId);
			}
			guildBattleMatchmakingInfo.RegisteredPlayersList = list;
			string guildBattleMatchmakingInfo2 = tWDModelManager.GetMessageSerializer().Serialize(guildBattleMatchmakingInfo);
			GvgBattleEntry gvgBattleEntry = new GvgBattleEntry
			{
				GroupId = GroupId,
				MatchmakingEpochMsec = MatchmakingEpochMsec,
				StartBattleTimestamp = StartBattleTimestamp,
				Tier = guildModel.GuildBattleTier,
				MatchmakingVersion = guildModel.MatchmakingVersion,
				GuildBattleMatchmakingInfo = guildBattleMatchmakingInfo2,
				RegisteredPlayers = guildModel.GuildBattleMatchmakingInfo.RegisteredPlayersList.Count,
				VictoryPoints = guildModel.CurrentVictoryPoints,
				LastOpponents = guildModel.GuildWarModel.GetAllOpponentsGroupIds()
			};
			gvgBattleEntry.SetRegisteredPlayersList(guildModel.GuildBattleMatchmakingInfo.RegisteredPlayersList);
			if (!tWDModelManager.ServerService.GvgJoinBattle(gvgBattleEntry))
			{
				tWDModelManager.GvGLogError("SetGuildWarPlayerSnapshotGroupCommand: Couldn't save the matchmaking info - " + SenderId, guildModel);
			}
			else
			{
				guildModel.GuildWarModel.AddBattleEntry(StartBattleTimestamp, gvgBattleEntry);
			}
			SaveGroupModel(tWDModelManager);
			return this;
		}
	}
}
