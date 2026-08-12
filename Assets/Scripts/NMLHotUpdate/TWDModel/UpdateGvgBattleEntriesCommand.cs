using BaseModel;

namespace TWDModel
{
	public class UpdateGvgBattleEntriesCommand : TWDSocialModelCommand
	{
		protected override TWDModelResult ValidateCommand(TWDModelManager manager)
		{
			GuildModel guildModel = manager.Player.GuildModel;
			if (!manager.Player.IsGuildMember || !guildModel.GvGSeasonModel.IsCurrentSeasonOpen(manager.Player.UtcTimeStamp) || !guildModel.GvGSeasonModel.GuildWarModel.IsCurrentWarOpen(manager.Player.UtcTimeStamp))
			{
				return TWDModelResult.Error;
			}
			if (guildModel.GuildWarModel.timeNextUpdateForGvgBattleEntries > manager.Player.UtcTimeStamp)
			{
				return TWDModelResult.Error;
			}
			return TWDModelResult.OK;
		}

		public override IModelCommandRespond Execute(ModelManager modelManager)
		{
			TWDModelResult result = TWDModelResult.Error;
			if (modelManager is TWDModelManager)
			{
				return base.Execute(modelManager) as NGModelCommandRespond;
			}
			return new NGModelCommandRespond(this, result);
		}

		protected override GroupCommandBase CreateGroupCommand(TWDModelManager modelManager)
		{
			return new UpdateGvgBattleEntriesGroupCommand();
		}
	}
}
