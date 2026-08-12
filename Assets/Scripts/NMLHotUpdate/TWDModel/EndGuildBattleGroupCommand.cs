using BaseModel;

namespace TWDModel
{
	public class EndGuildBattleGroupCommand : TWDValidationGroupCommand
	{
		public int EndVictoryPoints { get; private set; }

		public int EnemyEndVictoryPoints { get; private set; }

		public bool ValidResult { get; private set; }

		public int WarId { get; private set; }

		public EndGuildBattleGroupCommand()
		{
		}

		public EndGuildBattleGroupCommand(int vp, int enemyVp, bool validResult, int warId)
		{
			EndVictoryPoints = vp;
			EnemyEndVictoryPoints = enemyVp;
			ValidResult = validResult;
			WarId = warId;
		}

		protected override TWDValidationCommandResult Validate(ModelManager manager)
		{
			GuildModel guildModel = (GuildModel)(manager as TWDModelManager).GetGroupModel(GroupId);
			if (guildModel == null)
			{
				manager.GvGLog("EndGuildBattleGroupCommand: No Guild found with GroupId: " + GroupId);
				return TWDValidationCommandResult.Error;
			}
			if (!guildModel.GuildWarModel.CurrentBattle.IsBiggerThanEndBattleTimeStamp(guildModel.TimeStamp))
			{
				manager.GvGLogWarning("EndGuildBattleGroupCommand: Cancelled - Tried to end battle before time has ended!", guildModel);
				return TWDValidationCommandResult.Canceled;
			}
			if (!ValidResult)
			{
				manager.GvGLogWarning("EndGuildBattleGroupCommand: Cancelled - couldn't get the final scores from the leaderboards!", guildModel);
				return TWDValidationCommandResult.Canceled;
			}
			if (guildModel.GuildWarModel.CurrentBattle.HasEnded())
			{
				manager.GvGLogWarning("EndGuildBattleGroupCommand: Cancelled - Battle for guild has ended already", guildModel);
				return TWDValidationCommandResult.Canceled;
			}
			if (guildModel.GuildWarModel.WarDefinitionId != WarId)
			{
				manager.GvGLogWarning("EndGuildBattleGroupCommand: Cancelled - War Id is different", guildModel);
				return TWDValidationCommandResult.Canceled;
			}
			return TWDValidationCommandResult.OK;
		}

		protected override bool ExecuteInternal(ModelManager modelManager)
		{
			TWDModelManager tWDModelManager = modelManager as TWDModelManager;
			GuildModel guildModel = (GuildModel)tWDModelManager.GetGroupModel(GroupId);
			int guildBattleTier = guildModel.GuildBattleTier;
			GuildBattleModel currentBattle = guildModel.GuildWarModel.CurrentBattle;
			int num = currentBattle.CalculateTotalVictoryPoints();
			if (num != EndVictoryPoints)
			{
				tWDModelManager.GvGLogWarning($"EndGuildBattleGroupCommand:{num}/{EndVictoryPoints}");
			}
			currentBattle.SetEndVictoryPoints(EndVictoryPoints, EnemyEndVictoryPoints);
			guildModel.EndCurrentGuildBattle();
			guildModel.TotalAllTimeAccumulatedVp += currentBattle.FinalVictoryPoints;
			guildModel.UpdateGuildBattleLeaderboards(tWDModelManager, SenderId, guildModel.Id, guildModel.Name, battleEnd: true);
			if (tWDModelManager.Player.HashedId == SenderId)
			{
				tWDModelManager.Metrics.AddEnd().AddGvG(fromPlayer: false).AddGvGBattle(fromPlayer: false)
					.AddGvGBattleResult()
					.Send();
				int guildBattleTier2 = guildModel.GuildBattleTier;
				for (int num2 = guildBattleTier - 1; num2 >= guildBattleTier2; num2--)
				{
					GuildTierDefinition guildTierDefinition = tWDModelManager.GameEconomyData.GetGuildTierDefinition(num2);
					tWDModelManager.Metrics.AddEnd().AddGvG(fromPlayer: false).AddGvGTierUp(guildTierDefinition)
						.Send();
				}
			}
			modelManager.GvGLog("EndGuildBattleGroupCommand: Battle for guild has ended sucessfully", guildModel);
			return true;
		}
	}
}
