using BaseModel;

namespace TWDModel
{
	public class UndyingAction : StatusEffectAction
	{
		public UndyingAction(ActorModel sourceActor, ActorModel targetActor)
			: base(sourceActor, targetActor)
		{
			base.Avoided = false;
		}

		public override bool Execute(ModelManager manager)
		{
			if (((TWDModelManager)manager).CombatModel != null && base.TargetActor != null && base.TargetActor.IsValid() && !base.Avoided && !base.TargetActor.IsDead && !base.TargetActor.OnRedHealthBar && !base.TargetActor.UndyingState.IsUndying)
			{
				base.TargetActor.GrantUndying();
				return true;
			}
			return base.TargetActor != null;
		}

		public override string ToString()
		{
			return "UndyingAction - TargetActor = " + ((base.TargetActor != null) ? base.TargetActor.DebugInfo : "null") + ", Avoided = " + base.Avoided;
		}
	}
}
