namespace TWDModel
{
	public class ActorEndTurnBehavior : BehaviorBase
	{
		public ActorEndTurnBehavior(AIController controller)
			: base(controller)
		{
		}

		public override int GetPriority()
		{
			return 0;
		}

		public override void ExecuteAction()
		{
			if (base.Actor.IsHuman)
			{
				base.Controller.CombatModel.HealActorStatus(base.Actor);
			}
			base.Actor.EndAction();
		}
	}
}
