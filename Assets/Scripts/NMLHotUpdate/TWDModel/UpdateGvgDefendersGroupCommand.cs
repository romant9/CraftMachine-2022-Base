using System.Collections.Generic;
using BaseModel;

namespace TWDModel
{
	public class UpdateGvgDefendersGroupCommand : TWDValidationGroupCommand
	{
		public GuildBattleParticipantInfo PlayerInfo { get; private set; }

		public UpdateGvgDefendersGroupCommand()
		{
		}

		public UpdateGvgDefendersGroupCommand(GuildBattleParticipantInfo playerInfo)
		{
			PlayerInfo = playerInfo;
		}

		protected override TWDValidationCommandResult Validate(ModelManager manager)
		{
			if ((GuildModel)manager.GetGroupModel(GroupId) == null)
			{
				manager.GvGLogError("UpdateGvgBattleEntriesGroupCommand. No Guild found with GroupId: " + GroupId);
				return TWDValidationCommandResult.Error;
			}
			if (!PlayerInfo.HasValidDefense())
			{
				manager.GvGLogError("UpdateGvgBattleEntriesGroupCommand. No survivors set as defenders");
				return TWDValidationCommandResult.Error;
			}
			return TWDValidationCommandResult.OK;
		}

		protected override bool ExecuteInternal(ModelManager modelManager)
		{
			TWDModelManager tWDModelManager = modelManager as TWDModelManager;
			GuildModel guildModel = (GuildModel)tWDModelManager.GetGroupModel(GroupId);
			guildModel.GuildBattleMatchmakingInfo.UpdatePlayerInfo(PlayerInfo);
			List<long> list = guildModel.GuildWarModel?.GetFutureTimeSlots(tWDModelManager.Player.UtcTimeStamp);
			if (list == null || list.Count == 0)
			{
				return true;
			}
			if (SenderId != tWDModelManager.Player.HashedId || tWDModelManager.ServerService == null)
			{
				return true;
			}
			foreach (long item in list)
			{
				guildModel.GuildBattleMatchmakingInfo.RegisteredPlayersList = new List<string>(guildModel.GuildWarModel.RegisteredPlayersForBattleSlot[item]);
				string guildBattleMatchmakingInfo = tWDModelManager.GetMessageSerializer().Serialize(guildModel.GuildBattleMatchmakingInfo);
				GvgBattleEntry gvgBattleEntry = new GvgBattleEntry
				{
					GroupId = GroupId,
					MatchmakingEpochMsec = guildModel.GuildWarModel.GetLockDownTimeForBattleSlot(item),
					StartBattleTimestamp = item,
					Tier = guildModel.GuildBattleTier,
					MatchmakingVersion = guildModel.MatchmakingVersion,
					GuildBattleMatchmakingInfo = guildBattleMatchmakingInfo,
					RegisteredPlayers = guildModel.GuildBattleMatchmakingInfo.RegisteredPlayersList.Count,
					VictoryPoints = guildModel.CurrentVictoryPoints,
					LastOpponents = guildModel.GuildWarModel.GetAllOpponentsGroupIds()
				};
				gvgBattleEntry.SetRegisteredPlayersList(guildModel.GuildBattleMatchmakingInfo.RegisteredPlayersList);
				if (tWDModelManager.ServerService.GvgJoinBattle(gvgBattleEntry))
				{
					guildModel.GuildWarModel.AddBattleEntry(item, gvgBattleEntry);
					tWDModelManager.GvGLog(string.Format("{0} updated battle entry {1}", "UpdateGvgDefendersGroupCommand", item));
				}
				else
				{
					tWDModelManager.GvGLogError(string.Format("{0} failed to update battle entry {1}", "UpdateGvgDefendersGroupCommand", item));
				}
			}
			return true;
		}
	}
}
