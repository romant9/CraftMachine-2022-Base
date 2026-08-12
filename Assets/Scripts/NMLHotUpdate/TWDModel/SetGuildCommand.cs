using BaseModel;

namespace TWDModel
{
	public class SetGuildCommand : ModelCommand
	{
		public string GuildId { get; set; }

		public SetGuildCommand()
		{
		}

		public SetGuildCommand(PlayerModel player)
			: base(player)
		{
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			PlayerModel model = manager.GetModel<PlayerModel>(base.ModelId);
			if (string.IsNullOrEmpty(GuildId) && !string.IsNullOrEmpty(model.GuildId))
			{
				((TWDModelManager)manager).RemoveGroupModel(model.GuildId);
			}
			if (GuildId != model.GuildId)
			{
				model.WeeklyChallenge.ResetLastNumberOfGuildStars();
				model.Blackboard.SetToggle("HasSeenGuildBattleEnd");
			}
			model.GuildId = GuildId;
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
