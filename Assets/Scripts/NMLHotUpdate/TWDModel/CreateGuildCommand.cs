using BaseModel;

namespace TWDModel
{
	public class CreateGuildCommand : TWDSocialModelCommand
	{
		public GuildModel GuildData;

		public GuildMemberInfo GuildLeader;

		public string GuildLeaderCountryCode;

		protected override GroupCommandBase CreateGroupCommand(TWDModelManager manager)
		{
			return new CreateGuildGroupCommand
			{
				GroupId = GuildData.Id,
				Description = GuildData.Description,
				Name = GuildData.Name,
				Leader = GuildLeader,
				LeaderCountryCode = GuildLeaderCountryCode,
				JoinType = GuildData.JoinType,
				Purpose = GuildData.Purpose
			};
		}

		public override IModelCommandRespond Execute(ModelManager modelManager)
		{
			TWDModelManager tWDModelManager = modelManager as TWDModelManager;
			TWDModelResult tWDModelResult = tWDModelManager.Player.PayForGuildCreation();
			if (tWDModelResult == TWDModelResult.OK)
			{
				NGModelCommandRespond nGModelCommandRespond = base.Execute(modelManager) as NGModelCommandRespond;
				if (nGModelCommandRespond != null && nGModelCommandRespond.GetModelResult() == TWDModelResult.OK && GuildData != null && tWDModelManager != null)
				{
					tWDModelManager.Player.ClearGuildRelatedData();
					if (tWDModelManager.Metrics != null)
					{
						tWDModelManager.Metrics.AddGuild(GuildData).AddModerator(GuildLeader).AddCreateGuild()
							.Send();
					}
				}
				return nGModelCommandRespond;
			}
			return new NGModelCommandRespond(this, tWDModelResult);
		}
	}
}
