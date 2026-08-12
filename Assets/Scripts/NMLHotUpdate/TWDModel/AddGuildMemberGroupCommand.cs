using BaseModel;

namespace TWDModel
{
	public class AddGuildMemberGroupCommand : TWDGroupCommand
	{
		public string MemberName;

		public string MemberId;

		public PlayerEmblem PlayerEmblem;

		public int MemberLevel;

		public string SearchId;

		public int SearchPosition;

		public int totalVPPoints;

		public bool AutomaticallyJoinIfOpen { get; set; }

		public override GroupCommandBase Execute(ModelManager manager)
		{
			TWDModelManager tWDModelManager = manager as TWDModelManager;
			GuildModel guildModel = (GuildModel)manager.GetGroupModel(GroupId);
			bool flag = AutomaticallyJoinIfOpen && guildModel.JoinType == GuildJoinType.Open;
			TWDModelResult tWDModelResult = TWDModelResult.Error;
			long num = 0L;
			PlayerModel playerModel = null;
			if (tWDModelManager != null)
			{
				num = tWDModelManager.Player?.UtcTimeStamp ?? 0;
			}
			if (!flag || (guildModel.NumberMembers < 20 && !guildModel.IsBanned(MemberId, num)))
			{
				tWDModelResult = guildModel.AddMember(MemberId, MemberName, PlayerEmblem, MemberLevel, totalVPPoints, num);
			}
			if (tWDModelResult == TWDModelResult.OK)
			{
				if (flag)
				{
					tWDModelResult = guildModel.AcceptMemberRequest(SenderId, MemberId, num);
				}
				if (tWDModelResult == TWDModelResult.OK)
				{
					SaveGroupModel(manager);
					GuildMemberInfo guildMemberInfo = null;
					if (MemberId.Equals(manager.GetPlayer().HashedId))
					{
						playerModel = manager.GetPlayer() as PlayerModel;
						playerModel.GuildId = GroupId;
						guildMemberInfo = guildModel.GetMemberInfo(MemberId);
						if (!string.IsNullOrEmpty(playerModel.GuildId))
						{
							playerModel.ClearGuildRelatedData();
							playerModel.DailyQuestManager.StartAction("JoinGuild");
							playerModel.DailyQuestManager.CommitAction();
						}
					}
					else
					{
						guildMemberInfo = guildModel.GetMemberPendingInfo(MemberId);
					}
					if (manager is TWDModelManager { Metrics: not null } tWDModelManager2)
					{
						tWDModelManager2.Metrics.AddGuild(guildModel).AddMember(guildMemberInfo).AddJoinRequest(SearchId, SearchPosition)
							.Send();
					}
				}
			}
			return this;
		}
	}
}
