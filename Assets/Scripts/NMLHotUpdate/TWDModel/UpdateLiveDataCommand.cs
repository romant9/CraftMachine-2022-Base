using BaseModel;

namespace TWDModel
{
	public class UpdateLiveDataCommand : TWDSocialModelCommand
	{
		public long Timestamp;

		public string UniqueMissionId { get; set; }

		protected override TWDModelResult ValidateCommand(TWDModelManager modelManager)
		{
			GuildModel guildModel = modelManager.Player.GuildModel;
			if (!modelManager.Player.IsGuildMember)
			{
				modelManager.Debug.LogError("UpdateLiveDataCommand: Player is not a Guild Member");
				return TWDModelResult.Error;
			}
			if (guildModel.GuildWarModel == null)
			{
				modelManager.Debug.LogError("UpdateLiveDataCommand: GuildWarModel is null");
				return TWDModelResult.Error;
			}
			if (!guildModel.GuildWarModel.CurrentBattle.IsOngoing(modelManager.Player.UtcTimeStamp))
			{
				modelManager.Debug.LogError("UpdateLiveDataCommand: Battle not active");
				return TWDModelResult.Error;
			}
			return TWDModelResult.OK;
		}

		protected override GroupCommandBase CreateGroupCommand(TWDModelManager modelManager)
		{
			return new UpdateLiveDataGroupCommand
			{
				Timestamp = Timestamp,
				UniqueMissionId = UniqueMissionId,
				Attacks = modelManager.Player.GetCurrencyAmount(CurrencyType.GvGMissionKey)
			};
		}
	}
}
