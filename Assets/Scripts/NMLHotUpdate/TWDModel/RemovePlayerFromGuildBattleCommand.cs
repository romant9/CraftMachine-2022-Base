using BaseModel;

namespace TWDModel
{
	public class RemovePlayerFromGuildBattleCommand : TWDSocialModelCommand
	{
		private string playerName;

		public int SeasonDefinitionId { get; private set; }

		public int WarDefinitionId { get; private set; }

		public long Timeslot { get; private set; }

		public string RemovedPlayerHashedId { get; set; }

		public NotificationHubSendPushRequest NotificationHubSendPushRequest { get; set; }

		public RemovePlayerFromGuildBattleCommand()
		{
		}

		public RemovePlayerFromGuildBattleCommand(int seasonId, int warId, long timeSlot, string removedPlayerHashedId, NotificationHubSendPushRequest notificationHubSendPushRequest)
		{
			SeasonDefinitionId = seasonId;
			WarDefinitionId = warId;
			Timeslot = timeSlot;
			RemovedPlayerHashedId = removedPlayerHashedId;
			NotificationHubSendPushRequest = notificationHubSendPushRequest;
		}

		protected override TWDModelResult ValidateCommand(TWDModelManager modelManager)
		{
			GuildModel guildModel = modelManager.Player.GuildModel;
			if (!modelManager.Player.IsGuildMember)
			{
				modelManager.GvGLogError("RemovePlayerFromGuildBattleCommand: Player is not a Guild Member");
				return TWDModelResult.Error;
			}
			GuildMemberInfo memberInfo = modelManager.Player.GuildModel.GetMemberInfo(modelManager.Player.HashedId);
			GuildMemberInfo memberInfo2 = modelManager.Player.GuildModel.GetMemberInfo(RemovedPlayerHashedId);
			if (memberInfo != null && memberInfo2 != null)
			{
				if (memberInfo.Role <= GuildMemberRole.Elder)
				{
					modelManager.GvGLogError("RemovePlayerFromGuildBattleCommand: Player is not leader or co-leader");
					return TWDModelResult.Error;
				}
				if (memberInfo2.Role == memberInfo.Role)
				{
					modelManager.GvGLogError("RemovePlayerFromGuildBattleCommand: To remove and player has same rank");
					return TWDModelResult.Error;
				}
				if (memberInfo2.Role > memberInfo.Role)
				{
					modelManager.GvGLogError("RemovePlayerFromGuildBattleCommand: Cannot remove player with greater rank");
					return TWDModelResult.Error;
				}
			}
			GvGSeasonDefinition gvGSeasonDefinition = modelManager.GameEconomyData.FindGvGSeasonDefinition(SeasonDefinitionId);
			if (gvGSeasonDefinition == null)
			{
				modelManager.GvGLogError("RemovePlayerFromGuildBattleCommand: Could not find Season with id: " + SeasonDefinitionId, modelManager.Player);
				return TWDModelResult.Error;
			}
			if (!gvGSeasonDefinition.IsOpen(modelManager.Player.UtcTimeStamp))
			{
				modelManager.GvGLogError($"RemovePlayerFromGuildBattleCommand: Season not open Id {SeasonDefinitionId}, PlayerUtc: {modelManager.Player.UtcTimeStamp}, Definition: {gvGSeasonDefinition}", modelManager.Player);
				return TWDModelResult.Error;
			}
			GuildWarDefinition guildWarDefinition = modelManager.GameEconomyData.FindGuildWarWithId(WarDefinitionId);
			if (guildWarDefinition == null)
			{
				modelManager.GvGLogError("RemovePlayerFromGuildBattleCommand: Could not find War with id: " + WarDefinitionId);
				return TWDModelResult.Error;
			}
			if (!guildWarDefinition.IsOpen(modelManager.Player.UtcTimeStamp))
			{
				modelManager.GvGLogError($"RemovePlayerFromGuildBattleCommand: War not open Id {WarDefinitionId}, PlayerUtc: {modelManager.Player.UtcTimeStamp}, Definition: {guildWarDefinition}", modelManager.Player);
				return TWDModelResult.Error;
			}
			if (!guildModel.GuildWarModel.CanPlayerResignFromBattleSlot(Timeslot, RemovedPlayerHashedId, modelManager.Player.UtcTimeStamp))
			{
				modelManager.GvGLogError("RemovePlayerFromGuildBattleCommand: Resign player failed - " + RemovedPlayerHashedId, modelManager.Player);
				return TWDModelResult.Error;
			}
			return TWDModelResult.OK;
		}

		public override IModelCommandRespond Execute(ModelManager modelManager)
		{
			TWDModelResult result = TWDModelResult.Error;
			if (modelManager is TWDModelManager tWDModelManager)
			{
				playerName = tWDModelManager.Player.Name;
				return base.Execute(modelManager) as NGModelCommandRespond;
			}
			return new NGModelCommandRespond(this, result);
		}

		protected override GroupCommandBase CreateGroupCommand(TWDModelManager manager)
		{
			return new RemovePlayerFromGuildBattleGroupCommand(SeasonDefinitionId, WarDefinitionId, playerName, Timeslot, RemovedPlayerHashedId, NotificationHubSendPushRequest);
		}
	}
}
