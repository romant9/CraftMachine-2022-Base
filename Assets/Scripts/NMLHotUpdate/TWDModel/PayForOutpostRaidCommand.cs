using BaseModel;

namespace TWDModel
{
	public class PayForOutpostRaidCommand : ConsumeCurrencyCommand
	{
		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelManager obj = manager as TWDModelManager;
			PlayerModel playerModel = manager.GetPlayer() as PlayerModel;
			if (obj.Player.OutpostModel.GetRaidCashier().CanAffordWithDiamonds())
			{
				playerModel.ShouldConsumeMissionCurrency = true;
				playerModel.MapContainerModel.ClearMissionModelReferences();
				if (playerModel.GvGSeasonModelPlayer != null)
				{
					playerModel.GvGSeasonModelPlayer.GuildWarModelPlayer.GuildBattleModel.ReturnFromCombat();
				}
				return new NGModelCommandRespond(this, TWDModelResult.OK);
			}
			return new NGModelCommandRespond(this, TWDModelResult.NotEnoughCurrency);
		}
	}
}
