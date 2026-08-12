using BaseModel;

namespace TWDModel
{
	public class DebuffReduceRecoveryAction : StatusEffectAction
	{
		public int Param0 { get; private set; }

		public int Param1 { get; private set; }

		public DebuffReduceRecoveryAction(ActorModel targetActor, int param0, int param1)
			: base(null, targetActor)
		{
			Param0 = param0;
			Param1 = param1;
		}

		public override bool Execute(ModelManager manager)
		{
			CombatModel combatModel = ((TWDModelManager)manager).CombatModel;
			if (combatModel != null && base.TargetActor != null && base.TargetActor.IsValid() && !base.TargetActor.IsDead)
			{
				base.TargetActor.StartDebuffReduceRecovery(Param0, Param1);
				return true;
			}
			(manager as TWDModelManager).Debug.LogError("DebuffReduceRecovery action failed - CombatModel: " + ((combatModel != null) ? "not null" : "NULL") + " Target Actor: " + ((base.TargetActor != null) ? "not null" : "NULL"));
			return false;
		}

		public override string ToString()
		{
			return "TargetActor = " + ((base.TargetActor != null) ? base.TargetActor.DebugInfo : "null") + ", Avoided = " + base.Avoided;
		}
	}
}
