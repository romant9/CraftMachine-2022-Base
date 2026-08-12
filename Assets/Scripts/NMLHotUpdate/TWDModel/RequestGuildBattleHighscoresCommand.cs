using BaseModel;

namespace TWDModel
{
	public class RequestGuildBattleHighscoresCommand : TWDSocialModelCommand
	{
		public bool ForceBroadcast { get; private set; }

		public RequestGuildBattleHighscoresCommand()
		{
		}

		public RequestGuildBattleHighscoresCommand(bool forceBroadcast)
		{
			ForceBroadcast = forceBroadcast;
		}

		protected override TWDModelResult ValidateCommand(TWDModelManager modelManager)
		{
			GuildModel guildModel = modelManager.Player.GuildModel;
			if (!modelManager.Player.IsGuildMember)
			{
				modelManager.Debug.LogError("RequestGuildBattleHighscoresCommand: Player is not a Guild Member");
				return TWDModelResult.Error;
			}
			if (guildModel.TimeStamp > guildModel.TimeStamp + modelManager.GameEconomyData.GuildWarConfig.BattleLeaderboardsCacheDurationInMilliseconds)
			{
				modelManager.GvGLog("RequestGuildBattleHighscoresCommand: Requested before cache time expiration", guildModel);
				return TWDModelResult.Skip;
			}
			modelManager.GvGLog("RequestGuildBattleHighscoresCommand" + guildModel.Name + ForceBroadcast);
			return TWDModelResult.OK;
		}

		protected override GroupCommandBase CreateGroupCommand(TWDModelManager manager)
		{
			return new RequestGuildBattleHighscoresGroupCommand(ForceBroadcast);
		}
	}
}
