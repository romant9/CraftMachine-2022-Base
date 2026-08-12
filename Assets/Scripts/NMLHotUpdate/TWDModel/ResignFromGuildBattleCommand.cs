using BaseModel;

namespace TWDModel
{
	public class ResignFromGuildBattleCommand : TWDSocialModelCommand
	{
		private string playerName;

		public int SeasonDefinitionId { get; private set; }

		public int WarDefinitionId { get; private set; }

		public long Timeslot { get; private set; }

		public ResignFromGuildBattleCommand()
		{
		}

		public ResignFromGuildBattleCommand(int seasonId, int warId, long timeSlot)
		{
			SeasonDefinitionId = seasonId;
			WarDefinitionId = warId;
			Timeslot = timeSlot;
		}

		protected override TWDModelResult ValidateCommand(TWDModelManager modelManager)
		{
			GuildModel guildModel = modelManager.Player.GuildModel;
			if (!modelManager.Player.IsGuildMember)
			{
				modelManager.GvGLogError("ResignFromGuildBattleCommand: Player is not a Guild Member");
				return TWDModelResult.Error;
			}
			GvGSeasonDefinition gvGSeasonDefinition = modelManager.GameEconomyData.FindGvGSeasonDefinition(SeasonDefinitionId);
			if (gvGSeasonDefinition == null)
			{
				modelManager.GvGLogError("ResignFromGuildBattleCommand: Could not find Season with id: " + SeasonDefinitionId, modelManager.Player);
				return TWDModelResult.Error;
			}
			if (!gvGSeasonDefinition.IsOpen(modelManager.Player.UtcTimeStamp))
			{
				modelManager.GvGLogError($"ResignFromGuildBattleCommand: Season not open Id {SeasonDefinitionId}, PlayerUtc: {modelManager.Player.UtcTimeStamp}, Definition: {gvGSeasonDefinition}", modelManager.Player);
				return TWDModelResult.Error;
			}
			GuildWarDefinition guildWarDefinition = modelManager.GameEconomyData.FindGuildWarWithId(WarDefinitionId);
			if (guildWarDefinition == null)
			{
				modelManager.GvGLogError("ResignFromGuildBattleCommand: Could not find War with id: " + WarDefinitionId);
				return TWDModelResult.Error;
			}
			if (!guildWarDefinition.IsOpen(modelManager.Player.UtcTimeStamp))
			{
				modelManager.GvGLogError($"ResignFromGuildBattleCommand: War not open Id {WarDefinitionId}, PlayerUtc: {modelManager.Player.UtcTimeStamp}, Definition: {guildWarDefinition}", modelManager.Player);
				return TWDModelResult.Error;
			}
			if (!guildModel.GuildWarModel.CanPlayerResignFromBattleSlot(Timeslot, modelManager.Player.HashedId, modelManager.Player.UtcTimeStamp))
			{
				modelManager.GvGLogError("ResignFromGuildBattleCommand: Resign player failed - " + modelManager.Player.HashedId, modelManager.Player);
				return TWDModelResult.Skip;
			}
			return TWDModelResult.OK;
		}

		public override IModelCommandRespond Execute(ModelManager modelManager)
		{
			TWDModelResult result = TWDModelResult.Error;
			if (modelManager is TWDModelManager tWDModelManager)
			{
				playerName = tWDModelManager.Player.Name;
				tWDModelManager.Player.GvGSeasonModelPlayer.GuildWarModelPlayer.RegisteredBattleSlots.Remove(Timeslot);
				return base.Execute(modelManager) as NGModelCommandRespond;
			}
			return new NGModelCommandRespond(this, result);
		}

		protected override GroupCommandBase CreateGroupCommand(TWDModelManager manager)
		{
			return new ResignFromGuildBattleGroupCommand(SeasonDefinitionId, WarDefinitionId, playerName, Timeslot);
		}
	}
}
