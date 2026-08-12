using BaseModel;

namespace TWDModel
{
	public class RefreshGuildBattleHighscoresCommand : TWDSocialModelCommand
	{
		public bool ForceBroadcast { get; private set; }

		public RefreshGuildBattleHighscoresCommand()
		{
		}

		public RefreshGuildBattleHighscoresCommand(bool forceBroadcast)
		{
			ForceBroadcast = forceBroadcast;
		}

		protected override TWDModelResult ValidateCommand(TWDModelManager modelManager)
		{
			GuildModel guildModel = modelManager.Player.GuildModel;
			if (!modelManager.Player.IsGuildMember)
			{
				modelManager.Debug.LogError("RefreshGuildBattleHighscoresCommand: Player is not a Guild Member");
				return TWDModelResult.Error;
			}
			modelManager.GvGLog("RefreshGuildBattleHighscoresCommand" + guildModel.Name + ForceBroadcast);
			return TWDModelResult.OK;
		}

		protected override GroupCommandBase CreateGroupCommand(TWDModelManager manager)
		{
			return new RefreshGuildBattleHighscoresGroupCommand();
		}
	}
}
