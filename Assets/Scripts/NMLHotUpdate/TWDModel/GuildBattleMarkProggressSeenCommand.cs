using BaseModel;

namespace TWDModel
{
	public class GuildBattleMarkProggressSeenCommand : ModelCommand
	{
		public int sectorId { get; private set; }

		public GuildBattleMarkProggressSeenCommand()
		{
		}

		public GuildBattleMarkProggressSeenCommand(int sectorIdForMissons)
		{
			sectorId = sectorIdForMissons;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelManager tWDModelManager = manager as TWDModelManager;
			if (!tWDModelManager.Player.IsGuildMember)
			{
				manager.GvGLogError("GuildBattleMarkProggressSeenCommand: Player Is Not In Guild");
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			if (sectorId == -1)
			{
				tWDModelManager.Player.GvGSeasonModelPlayer.GuildWarModelPlayer.GuildBattleModel.ResetProgressionSnapshot();
			}
			else
			{
				GuildBattleMapSectorModel sectorModel = tWDModelManager.Player.GuildWarModel.CurrentBattle.CurrentMapModel.GetSectorModel(sectorId);
				if (sectorModel == null)
				{
					manager.GvGLogError("GuildBattleMarkProggressSeenCommand: Could not find sector with id: " + sectorId, tWDModelManager.Player);
					return new NGModelCommandRespond(this, TWDModelResult.Error);
				}
				tWDModelManager.Player.GvGSeasonModelPlayer.GuildWarModelPlayer.GuildBattleModel.CurrentCompletionSnapshot.CopyProgressFromSector(sectorModel);
			}
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
