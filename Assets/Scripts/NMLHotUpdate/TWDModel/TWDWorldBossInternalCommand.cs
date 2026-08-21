using BaseModel;

namespace TWDModel
{
	public abstract class TWDWorldBossInternalCommand : TWDWorldBossBaseCommand
	{
		protected WorldBossGuildFullSnapshot WorldBossGuildFullSnapshot;

		public TWDWorldBossInternalCommand()
		{
		}

		public TWDWorldBossInternalCommand(int seasonId, int cycleId)
			: base(seasonId, cycleId)
		{
		}

		public override IModelCommandRespond Execute(ModelManager modelManager)
		{
			TWDModelManager tWDModelManager = (TWDModelManager)modelManager;
			if (tWDModelManager == null)
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			GuildModel = tWDModelManager.Player?.GuildModel;
			WorldBossGuildFullSnapshot = tWDModelManager.Player?.WorldBossModelManager?.WorldBossGuildFullSnapshot;
			if (GuildModel == null)
			{
				tWDModelManager.Debug.LogError("TWDWorldBossCommand: GuildModel is null");
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			if (tWDModelManager.Player == null || !tWDModelManager.Player.IsGuildMember)
			{
				modelManager.Debug.LogError("TWDWorldBossCommand: Player is not a guild member");
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			if (base.SeasonId <= 0)
			{
				tWDModelManager.Debug.LogError("TWDWorldBossCommand: Invalid SeasonId: " + base.SeasonId);
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			if (base.CycleId <= 0)
			{
				tWDModelManager.Debug.LogError("TWDWorldBossCommand: Invalid CycleId: " + base.CycleId);
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			TWDModelResult tWDModelResult = ValidateCommand(tWDModelManager);
			if (tWDModelResult != TWDModelResult.OK)
			{
				return new NGModelCommandRespond(this, tWDModelResult);
			}
			if (tWDModelManager.ServerService != null)
			{
				tWDModelResult = ExecuteOnServer(tWDModelManager);
			}
			return new NGModelCommandRespond(this, tWDModelResult);
		}
	}
}
