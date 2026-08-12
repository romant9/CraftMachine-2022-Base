using BaseModel;

namespace TWDModel
{
	public class GuildBattleCreateIndicatorCommand : TWDSocialModelCommand
	{
		public GuildBattleModel.GuildBattleIndicatorData data { get; private set; }

		public GuildBattleCreateIndicatorCommand()
		{
		}

		public GuildBattleCreateIndicatorCommand(GuildBattleModel.GuildBattleIndicatorData data)
		{
			this.data = data;
		}

		protected override GroupCommandBase CreateGroupCommand(TWDModelManager manager)
		{
			return new GuildBattleCreateIndicatorGroupCommand(new GuildBattleModel.GuildBattleIndicatorData(data.SectorId, data.X, data.Y)
			{
				PlayerHashedId = manager.Player.HashedId,
				UtcTimeStamp = manager.Player.UtcTimeStamp
			});
		}
	}
}
