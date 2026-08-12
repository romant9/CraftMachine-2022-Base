using BaseModel;

namespace TWDModel
{
	public class SetGuildWarPlayerSnapshotCommand : TWDSocialModelCommand
	{
		private GuildBattleParticipantInfo playerInfo;

		private int matchmakingVersion;

		public long MatchmakingEpochMsec { get; private set; }

		public long StartBattleTimestamp { get; private set; }

		public SetGuildWarPlayerSnapshotCommand()
		{
		}

		public SetGuildWarPlayerSnapshotCommand(long matchmakingEpochMsec, long startBattleTimestamp)
		{
			MatchmakingEpochMsec = matchmakingEpochMsec;
			StartBattleTimestamp = startBattleTimestamp;
		}

		protected override TWDModelResult ValidateCommand(TWDModelManager modelManager)
		{
			return TWDModelResult.OK;
		}

		public override IModelCommandRespond Execute(ModelManager modelManager)
		{
			TWDModelResult result = TWDModelResult.Error;
			if (modelManager is TWDModelManager tWDModelManager)
			{
				if (tWDModelManager.Player.Camp.GetCouncilLevel() < tWDModelManager.GameEconomyData.GuildWarConfig.GuildWarUnlockAtCouncilLevel)
				{
					tWDModelManager.GvGLogWarning("SetGuildWarPlayerSnapshotCommand: Skip, player - " + tWDModelManager.Player.HashedId + " level is not high enough", tWDModelManager.Player);
					return new NGModelCommandRespond(this, TWDModelResult.Skip);
				}
				matchmakingVersion = tWDModelManager.GameEconomyData.GuildWarConfig.MatchmakingVersion;
				playerInfo = GvGModelHelper.CreateEnemyPlayerData(tWDModelManager.Player, tWDModelManager.GameEconomyData);
				tWDModelManager.GvGLog("SetGuildWarPlayerSnapshotCommand: Player snapshot created " + tWDModelManager.Player.HashedId, tWDModelManager.Player);
				return base.Execute(modelManager) as NGModelCommandRespond;
			}
			return new NGModelCommandRespond(this, result);
		}

		protected override GroupCommandBase CreateGroupCommand(TWDModelManager manager)
		{
			return new SetGuildWarPlayerSnapshotGroupCommand(playerInfo, matchmakingVersion, MatchmakingEpochMsec, StartBattleTimestamp);
		}
	}
}
