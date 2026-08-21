using BaseModel;

namespace TWDModel
{
	public abstract class TWDWorldBossBaseCommand : ModelCommand
	{
		protected GuildModel GuildModel;

		public int SeasonId { get; private set; }

		public int CycleId { get; private set; }

		public TWDWorldBossBaseCommand()
		{
		}

		public TWDWorldBossBaseCommand(int seasonId, int cycleId)
		{
			SeasonId = seasonId;
			CycleId = cycleId;
		}

		protected virtual TWDModelResult ValidateCommand(TWDModelManager modelManager)
		{
			return TWDModelResult.OK;
		}

		protected abstract TWDModelResult ExecuteOnServer(TWDModelManager modelManager);

		public override IModelCommandRespond Execute(ModelManager modelManager)
		{
			TWDModelManager tWDModelManager = (TWDModelManager)modelManager;
			if (tWDModelManager == null)
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			GuildModel = tWDModelManager.Player?.GuildModel;
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
			if (SeasonId <= 0)
			{
				tWDModelManager.Debug.LogError("TWDWorldBossCommand: Invalid SeasonId: " + SeasonId);
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			if (CycleId <= 0)
			{
				tWDModelManager.Debug.LogError("TWDWorldBossCommand: Invalid CycleId: " + CycleId);
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
