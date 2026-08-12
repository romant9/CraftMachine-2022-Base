using BaseModel;

namespace TWDModel
{
	public class ClaimGuildBattleSectorRewardCommand : ModelCommand
	{
		public int SectorId { get; private set; }

		public ClaimGuildBattleSectorRewardCommand()
		{
		}

		public ClaimGuildBattleSectorRewardCommand(int sectorId)
		{
			SectorId = sectorId;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			PlayerModel playerModel = manager.GetPlayer() as PlayerModel;
			GuildBattleModelPlayer guildBattleModel = playerModel.GvGSeasonModelPlayer.GuildWarModelPlayer.GuildBattleModel;
			GuildBattleModel currentBattle = playerModel.GuildModel.GuildWarModel.CurrentBattle;
			if (!currentBattle.BattleId.Equals(guildBattleModel.CurrentBattleId))
			{
				manager.GvGLogWarning($"ClaimGuildBattleSectorRewardCommand: Cancelled - Trying to claim sector reward for different battle: Player Battle Id = {guildBattleModel.CurrentBattleId} - Guild battle Id = {currentBattle.BattleId}", playerModel);
				return new NGModelCommandRespond(this, TWDModelResult.Skip);
			}
			if (currentBattle.CompletedSectors.Contains(SectorId))
			{
				TWDModelResult result = guildBattleModel.GiveSectorBonusRewards(SectorId);
				return new NGModelCommandRespond(this, result);
			}
			manager.GvGLogWarning($"ClaimGuildBattleSectorRewardCommand: Trying to claim reward for a non completed sector. Sector {SectorId}", playerModel);
			return new NGModelCommandRespond(this, TWDModelResult.Skip);
		}
	}
}
