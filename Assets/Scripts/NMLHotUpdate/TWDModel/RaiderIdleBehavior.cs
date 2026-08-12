namespace TWDModel
{
	public class RaiderIdleBehavior : BehaviorBase
	{
		public RaiderIdleBehavior(AIController controller)
			: base(controller)
		{
		}

		public override int GetPriority()
		{
			if (base.AIDataModel.Alertness != AIAlertness.Idle)
			{
				return 0;
			}
			return 100;
		}

		public override void ExecuteAction()
		{
			base.Controller.CombatModel.HealActorStatus(base.Actor);
			base.Actor.EndAction();
		}
	}
}
