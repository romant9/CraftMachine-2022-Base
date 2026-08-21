using BaseModel;

namespace TWDModel
{
	public class MarkWorldBossSettlementShownCommand : ModelCommand
	{
		public int SeasonId { get; set; }

		public int CycleId { get; set; }

		public MarkWorldBossSettlementShownCommand()
		{
		}

		public MarkWorldBossSettlementShownCommand(int seasonId, int cycleId)
		{
			SeasonId = seasonId;
			CycleId = cycleId;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			WorldBossModelManager worldBossModelManager = (manager as TWDModelManager)?.Player?.WorldBossModelManager;
			if (worldBossModelManager == null || SeasonId <= 0 || CycleId <= 0)
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			worldBossModelManager.MarkSettlementShown(SeasonId, CycleId);
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
