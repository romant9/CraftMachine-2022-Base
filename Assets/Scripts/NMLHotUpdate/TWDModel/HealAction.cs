using BaseModel;

namespace TWDModel
{
	public class HealAction : ModelActorAction
	{
		public ActorModel SourceActor { get; private set; }

		public ActorModel TargetActor { get; private set; }

		public int AmountHealed { get; private set; }

		public string Notification { get; private set; }

		public HealAction(ActorModel sourceActor, ActorModel targetActor, int amountHealed, string notification = "")
			: base(sourceActor)
		{
			SourceActor = sourceActor;
			TargetActor = targetActor;
			AmountHealed = amountHealed;
			Notification = notification;
		}

		public override bool Execute(ModelManager manager)
		{
			if (((TWDModelManager)manager)?.CombatModel == null)
			{
				return false;
			}
			FixedPoint fixedPoint = 1.0;
			if (TargetActor.DebuffReduceRecoveryTimedEffect != null)
			{
				fixedPoint = (float)(100 - TargetActor.DebuffReduceRecoveryTimedEffect.HealReduceAmount) / 100f;
				TargetActor.NotifyChange("AbilityVisited", new object[2] { "ReduceRecovery", false });
			}
			if (fixedPoint <= 0.0)
			{
				return true;
			}
			AmountHealed = (int)(AmountHealed * fixedPoint);
			TargetActor.Heal(AmountHealed);
			if (TargetActor is SurvivorModel survivorModel)
			{
				survivorModel.MinHitpoints = survivorModel.Hitpoints;
			}
			return true;
		}

		public override bool CanExecute()
		{
			return TargetActor != null;
		}
	}
}
