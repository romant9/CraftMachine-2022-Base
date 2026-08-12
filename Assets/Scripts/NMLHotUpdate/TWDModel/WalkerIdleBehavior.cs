namespace TWDModel
{
	public class WalkerIdleBehavior : BehaviorBase
	{
		public WalkerIdleBehavior(AIController controller)
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
			base.Actor.EndAction();
		}
	}
}
