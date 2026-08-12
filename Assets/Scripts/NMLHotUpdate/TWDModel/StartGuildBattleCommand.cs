using BaseModel;

namespace TWDModel
{
	public class StartGuildBattleCommand : TWDSocialModelCommand
	{
		public int WarDefinitionId;

		public long Timeslot;

		protected override TWDModelResult ValidateCommand(TWDModelManager modelManager)
		{
			GuildModel guildModel = modelManager.Player.GuildModel;
			if (!modelManager.Player.IsGuildMember)
			{
				modelManager.GvGLogError("StartGuildBattleCommand: Player is not a Guild Member");
				return TWDModelResult.Error;
			}
			GuildWarDefinition guildWarDefinition = modelManager.GameEconomyData.FindGuildWarWithId(WarDefinitionId);
			if (guildWarDefinition == null)
			{
				modelManager.GvGLogError("StartGuildBattleCommand: Could not find War with id: " + WarDefinitionId, modelManager.Player);
				return TWDModelResult.Error;
			}
			if (!guildWarDefinition.IsOpen(modelManager.Player.UtcTimeStamp))
			{
				modelManager.GvGLogError($"StartGuildBattleCommand: War not open Id {WarDefinitionId}, PlayerUtc: {modelManager.Player.UtcTimeStamp}, Definition: {guildWarDefinition}", modelManager.Player);
				return TWDModelResult.Error;
			}
			if (modelManager.Player.UtcTimeStamp < Timeslot)
			{
				modelManager.GvGLogWarning("StartGuildBattleCommand: Player is trying to start a battle too early ", modelManager.Player);
				return TWDModelResult.Error;
			}
			if (!guildModel.GuildWarModel.HasEnoughRegisteredPlayersToStartBattleForTimeSlot(Timeslot))
			{
				modelManager.GvGLogError("StartGuildBattleCommand : Not enough participants", modelManager.Player);
				return TWDModelResult.Error;
			}
			return TWDModelResult.OK;
		}

		protected override GroupCommandBase CreateGroupCommand(TWDModelManager manager)
		{
			return new StartGvgBattleGroupCommand
			{
				TimeSlot = Timeslot
			};
		}
	}
}
