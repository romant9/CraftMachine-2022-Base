using System;
using System.Collections.Generic;
using BaseModel;

namespace TWDModel
{
	public class LeaveGuildGroupCommand : TWDGroupCommand
	{
		public string LeaverId { get; set; }

		public long ModificationTime { get; set; }

		public GuildLeaveType LeaveType { get; set; }

		public override GroupCommandBase Execute(ModelManager manager)
		{
			GuildModel guildModel = (GuildModel)manager.GetGroupModel(GroupId);
			TWDModelManager tWDModelManager = (TWDModelManager)manager;
			long gWKickSoftBanDurationMinutes = tWDModelManager.GameEconomyData.ConfigData.GWKickSoftBanDurationMinutes;
			if (ModificationTime == 0L)
			{
				ModificationTime = Time;
			}
			TWDModelResult tWDModelResult;
			if (LeaveType == GuildLeaveType.RejectRequest)
			{
				tWDModelResult = guildModel.RefuseMemberRequest(LeaverId);
			}
			else if (LeaveType == GuildLeaveType.Kick)
			{
				tWDModelResult = guildModel.KickOutMember(SenderId, LeaverId);
			}
			else if (LeaveType == GuildLeaveType.KickAndSoftBan)
			{
				tWDModelResult = guildModel.KickOutMember(SenderId, LeaverId);
				if (tWDModelResult == TWDModelResult.OK)
				{
					long utcTimeStamp = tWDModelManager.Player.UtcTimeStamp;
					long num = (long)TimeSpan.FromMinutes(gWKickSoftBanDurationMinutes).TotalMilliseconds;
					guildModel.BanPlayer(LeaverId, utcTimeStamp + num, utcTimeStamp);
				}
			}
			else
			{
				tWDModelResult = guildModel.LeaveMember(SenderId, LeaverId);
			}
			if (tWDModelResult == TWDModelResult.OK)
			{
				foreach (KeyValuePair<long, List<string>> item in guildModel.GvGSeasonModel.GuildWarModel.RegisteredPlayersForBattleSlot)
				{
					if (guildModel.GvGSeasonModel.GuildWarModel.IsBattleSlotLocked(item.Key, guildModel.TimeStamp))
					{
						continue;
					}
					bool flag = item.Value.Remove(LeaverId);
					if (tWDModelManager.ServerService == null || !(tWDModelManager.Player.HashedId == SenderId))
					{
						continue;
					}
					if (flag && item.Value.Count == tWDModelManager.GameEconomyData.GuildWarConfig.MinPlayersToStartBattle - 1)
					{
						manager.Debug.Log("Sender " + SenderId + " left the guild and removed guild from timeslot");
						tWDModelManager.ServerService.GvgLeaveBattle(GroupId, guildModel.GuildWarModel.GetLockDownTimeForBattleSlot(item.Key));
						guildModel.GuildWarModel.RemoveBattleEntry(item.Key);
					}
					else if (item.Value.Count >= tWDModelManager.GameEconomyData.GuildWarConfig.MinPlayersToStartBattle)
					{
						guildModel.GuildBattleMatchmakingInfo.RegisteredPlayersList = new List<string>(guildModel.GuildWarModel.RegisteredPlayersForBattleSlot[item.Key]);
						string guildBattleMatchmakingInfo = tWDModelManager.GetMessageSerializer().Serialize(guildModel.GuildBattleMatchmakingInfo);
						GvgBattleEntry gvgBattleEntry = new GvgBattleEntry
						{
							GroupId = GroupId,
							MatchmakingEpochMsec = guildModel.GuildWarModel.GetLockDownTimeForBattleSlot(item.Key),
							StartBattleTimestamp = item.Key,
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
							continue;
						}
						guildModel.GuildWarModel.AddBattleEntry(item.Key, gvgBattleEntry);
						tWDModelManager.GvGLog("RegisterForGuildBattleGroupCommand: matchmaking stored with matchmaking time:" + guildModel.GuildWarModel.GetLockDownTimeForBattleSlot(item.Key) + " " + SenderId, guildModel);
					}
				}
				SaveGroupModel(manager);
				GuildMemberInfo memberPendingInfo = guildModel.GetMemberPendingInfo(LeaverId);
				GuildMemberInfo memberInfo = guildModel.GetMemberInfo(SenderId);
				Metrics metrics = tWDModelManager.Metrics;
				if (metrics != null)
				{
					if (LeaveType == GuildLeaveType.MemberLeave)
					{
						metrics.AddGuild(guildModel).AddMember(memberPendingInfo).AddLeaves()
							.Send();
					}
					else if (LeaveType == GuildLeaveType.Kick || LeaveType == GuildLeaveType.KickAndSoftBan)
					{
						metrics.AddGuild(guildModel).AddModerator(memberInfo).AddSend()
							.AddMember(memberPendingInfo);
						if (LeaveType == GuildLeaveType.KickAndSoftBan)
						{
							metrics.AddKick(gWKickSoftBanDurationMinutes);
						}
						else
						{
							metrics.AddKick();
						}
						metrics.Send();
					}
					else if (LeaveType == GuildLeaveType.RejectRequest)
					{
						metrics.AddGuild(guildModel).AddModerator(memberInfo).AddSend()
							.AddMember(memberPendingInfo)
							.AddJoinRefusal()
							.Send();
					}
				}
				guildModel.GuildBattleMatchmakingInfo.UpdateInfoOnPlayerChanged(guildModel.GuildWarModel, null);
			}
			return this;
		}
	}
}
