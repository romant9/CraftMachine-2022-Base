using BaseModel;

namespace TWDModel
{
	public class ResolveEndBattleCommand : ModelCommand
	{
		public override IModelCommandRespond Execute(ModelManager manager)
		{
			PlayerModel playerModel = manager.GetPlayer() as PlayerModel;
			playerModel.Blackboard.SetToggle("HasSeenGuildBattleEnd");
			playerModel.GvGSeasonModelPlayer.GuildWarModelPlayer.EndBattle();
			manager.GvGLog("ResolveEndBattleCommand : Successful", playerModel);
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
