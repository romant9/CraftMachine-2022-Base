using BaseModel;

namespace TWDModel
{
	public class GuildBattleCreateIndicatorGroupCommand : TWDGroupCommand
	{
		public GuildBattleModel.GuildBattleIndicatorData data { get; private set; }

		public GuildBattleCreateIndicatorGroupCommand()
		{
		}

		public GuildBattleCreateIndicatorGroupCommand(GuildBattleModel.GuildBattleIndicatorData data)
		{
			this.data = data;
		}

		public override GroupCommandBase Execute(ModelManager manager)
		{
			GuildModel guildModel = (GuildModel)(manager as TWDModelManager).GetGroupModel(GroupId);
			if (guildModel == null)
			{
				manager.GvGLogError("GuildBattleCreateIndicatorGroupCommand: No Guild found with GroupId: " + GroupId);
				return this;
			}
			guildModel.GuildWarModel.CurrentBattle.UpdateIndicatorState(data);
			SaveGroupModel(manager);
			return base.Execute(manager);
		}
	}
}
