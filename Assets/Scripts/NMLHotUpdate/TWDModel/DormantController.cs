namespace TWDModel
{
	public class DormantController : AIController
	{
		public bool ForceWakeUp { get; set; }

		public bool HasWoken => base.AIDataModel.HasEvent(AIDataModel.WakeUp);

		public DormantController(ActorModel actor)
			: base(actor)
		{
			base.Enabled = true;
		}

		public override void ExecuteTurn()
		{
			base.ExecuteTurn();
			base.Actor.EndAction();
			if (base.CombatModel == null || !base.CombatModel.IsValid())
			{
				return;
			}
			bool flag = false;
			FixedPoint fixedPoint = 1.0;
			foreach (ActorModel enemyFactionsActor in base.CombatModel.GetEnemyFactionsActors(base.Actor.Faction))
			{
				if (base.Actor.GridCoordinate.DistanceTo(enemyFactionsActor.GridCoordinate) <= fixedPoint)
				{
					flag = true;
				}
			}
			if (base.AIDataModel.HasEvent(AIDataModel.HeardNoise))
			{
				flag = true;
			}
			if (flag || ForceWakeUp)
			{
				base.AIDataModel.SetEvent(AIDataModel.WakeUp);
			}
		}
	}
}
