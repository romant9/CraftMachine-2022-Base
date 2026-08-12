using BaseModel;

namespace TWDModel
{
	public class GuildBattleMapSectorStateSeenCommand : ModelCommand
	{
		public string sectorId { get; private set; }

		public int state { get; private set; }

		public GuildBattleMapSectorStateSeenCommand()
		{
		}

		public GuildBattleMapSectorStateSeenCommand(string sectorId, int state)
		{
			this.sectorId = sectorId;
			this.state = state;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelManager tWDModelManager = manager as TWDModelManager;
			if (!tWDModelManager.Player.IsGuildMember)
			{
				manager.GvGLogError("GuildBattleMapSectorStateSeenCommand: Player Is Not In Guild", tWDModelManager.Player);
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			if (!tWDModelManager.Player.GuildModel.GuildWarModel.CurrentBattle.IsOngoing(tWDModelManager.Player.UtcTimeStamp))
			{
				manager.GvGLogWarning("GuildBattleMapSectorStateSeenCommand: Battle is not active", tWDModelManager.Player);
				return new NGModelCommandRespond(this, TWDModelResult.Skip);
			}
			tWDModelManager.Player.GvGSeasonModelPlayer.GuildWarModelPlayer.GuildBattleModel.CurrentCompletionSnapshot.SectorStateSeenUpdate(sectorId, state);
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
