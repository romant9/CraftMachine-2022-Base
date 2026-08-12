using BaseModel;

namespace TWDModel
{
	public class GuildBattleBuffAnimationSeenCommand : ModelCommand
	{
		public string bonusName { get; private set; }

		public int stackedBuffsNum { get; private set; }

		public GuildBattleBuffAnimationSeenCommand()
		{
		}

		public GuildBattleBuffAnimationSeenCommand(string bonusName, int stackedBuffNum)
		{
			this.bonusName = bonusName;
			stackedBuffsNum = stackedBuffNum;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelManager tWDModelManager = manager as TWDModelManager;
			if (!tWDModelManager.Player.IsGuildMember)
			{
				manager.GvGLogError("GuildBattleBuffAnimationSeenCommand: Player Is Not In Guild", tWDModelManager.Player);
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			tWDModelManager.Player.GvGSeasonModelPlayer.GuildWarModelPlayer.GuildBattleModel.CurrentCompletionSnapshot.UpdateAnimateSectorAnimationSeen(bonusName, stackedBuffsNum);
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
