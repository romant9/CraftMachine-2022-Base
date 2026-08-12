using BaseModel;

namespace TWDModel
{
	public class SetupAttackTargetMissionData : ModelCommand
	{
		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelManager tWDModelManager = (TWDModelManager)manager;
			PlayerModel player = tWDModelManager.Player;
			if (player == null)
			{
				tWDModelManager.Debug.LogError("Null player model");
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			GuildBattleModelPlayer guildBattlePlayer = player.GuildBattlePlayer;
			if (guildBattlePlayer == null)
			{
				tWDModelManager.Debug.LogError("Null battle when trying to setup mission data");
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			guildBattlePlayer.AttackTargetMission.Setup(tWDModelManager);
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
