using BaseModel;

namespace TWDModel
{
	public class EnterWorldBossCommand : ModelCommand
	{
		public int SeasonId { get; private set; }

		public int CycleId { get; private set; }

		public EnterWorldBossCommand()
		{
		}

		public EnterWorldBossCommand(int seasonId, int cycleId)
		{
			SeasonId = seasonId;
			CycleId = cycleId;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			if (!(manager is TWDModelManager { Player: not null } tWDModelManager))
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			if (SeasonId <= 0)
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			if (CycleId <= 0)
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			tWDModelManager.Player.WorldBossModelManager?.ClearHeroFatigueIfOutdated(SeasonId, CycleId);
			tWDModelManager.Player.WorldBossLastEnteredSeasonId = SeasonId;
			tWDModelManager.Player.WorldBossLastEnteredCycleId = CycleId;
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
