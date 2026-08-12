using System.Collections.Generic;
using BaseModel;

namespace TWDModel
{
	public class UpdateGvgBattleEntriesGroupCommand : TWDValidationGroupCommand
	{
		private const long OneHour = 3600000L;

		protected override TWDValidationCommandResult Validate(ModelManager manager)
		{
			if ((GuildModel)manager.GetGroupModel(GroupId) == null)
			{
				manager.GvGLogError("UpdateGvgBattleEntriesGroupCommand. No Guild found with GroupId: " + GroupId);
				return TWDValidationCommandResult.Error;
			}
			return TWDValidationCommandResult.OK;
		}

		protected override bool ExecuteInternal(ModelManager modelManager)
		{
			TWDModelManager tWDModelManager = modelManager as TWDModelManager;
			GuildModel guildModel = (GuildModel)tWDModelManager.GetGroupModel(GroupId);
			TWDModelResult result;
			List<long> timeSlotsForGvgBattleEntriesObsolete = guildModel.GuildWarModel.GetTimeSlotsForGvgBattleEntriesObsolete(guildModel, out result, tWDModelManager.Player.UtcTimeStamp);
			if (result != TWDModelResult.OK)
			{
				tWDModelManager.GvGLogError("UpdateGvgBattleEntriesGroupCommand failed to update battle entries");
				return false;
			}
			guildModel.GuildWarModel.timeNextUpdateForGvgBattleEntries = tWDModelManager.Player.UtcTimeStamp + 3600000;
			if (timeSlotsForGvgBattleEntriesObsolete.Count == 0)
			{
				return true;
			}
			if (SenderId != tWDModelManager.Player.HashedId || tWDModelManager.ServerService == null)
			{
				return true;
			}
			foreach (long item in timeSlotsForGvgBattleEntriesObsolete)
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
					tWDModelManager.GvGLog(string.Format("{0} updated battle entry {1}", "UpdateGvgBattleEntriesGroupCommand", item));
				}
				else
				{
					tWDModelManager.GvGLogError(string.Format("{0} failed to update battle entry {1}", "UpdateGvgBattleEntriesGroupCommand", item));
				}
			}
			return true;
		}
	}
}
