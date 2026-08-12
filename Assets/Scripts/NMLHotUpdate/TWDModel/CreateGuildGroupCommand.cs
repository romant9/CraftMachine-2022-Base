using BaseModel;

namespace TWDModel
{
	public class CreateGuildGroupCommand : TWDGroupCommand
	{
		public string Name { get; set; }

		public string Description { get; set; }

		public GuildJoinType JoinType { get; set; }

		public string Purpose { get; set; }

		public GuildMemberInfo Leader { get; set; }

		public string LeaderCountryCode { get; set; }

		public override GroupCommandBase Execute(ModelManager manager)
		{
			TWDModelManager tWDModelManager = manager as TWDModelManager;
			GuildModel obj = manager.GetGroupModel(GroupId) as GuildModel;
			PlayerModel playerModel = manager.GetPlayer() as PlayerModel;
			long timeStamp = playerModel?.UtcTimeStamp ?? 0;
			if (!obj.IsValidPurpose(Purpose, tWDModelManager.GameEconomyData.ConfigData.GuildPurposeTypes))
			{
				Purpose = GuildModel.GetDefaultPurpose(tWDModelManager.GameEconomyData.ConfigData.GuildPurposeTypes);
			}
			obj.CreateGuild(Name, Description, JoinType, Leader, LeaderCountryCode, timeStamp, Purpose, tWDModelManager.GameEconomyData);
			playerModel.GuildId = GroupId;
			obj.Version = manager.GetVersion();
			if (!string.IsNullOrEmpty(playerModel.GuildId))
			{
				playerModel.ClearGuildRelatedData();
				playerModel.DailyQuestManager.StartAction("JoinGuild");
				playerModel.DailyQuestManager.CommitAction();
			}
			SaveGroupModel(manager);
			return this;
		}
	}
}
