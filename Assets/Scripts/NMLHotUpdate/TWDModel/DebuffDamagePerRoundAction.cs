using BaseModel;

namespace TWDModel
{
	public class DebuffDamagePerRoundAction : StatusEffectAction
	{
		public int Param0 { get; private set; }

		public int Param1 { get; private set; }

		public int Param2 { get; private set; }

		public FixedPoint Param3 { get; private set; }

		public DebuffDamagePerRoundAction(ActorModel targetActor, int param0, int param1, int param2, FixedPoint param3)
			: base(null, targetActor)
		{
			Param0 = param0;
			Param1 = param1;
			Param2 = param2;
			Param3 = param3;
		}

		public override bool Execute(ModelManager manager)
		{
			CombatModel combatModel = ((TWDModelManager)manager).CombatModel;
			if (combatModel != null && base.TargetActor != null && base.TargetActor.IsValid() && !base.TargetActor.IsDead)
			{
				base.TargetActor.StartDebuffDamagePerRound(Param0, Param1, Param2, Param3);
				return true;
			}
			(manager as TWDModelManager).Debug.LogError("DebuffDamagePerRound action failed - CombatModel: " + ((combatModel != null) ? "not null" : "NULL") + " Target Actor: " + ((base.TargetActor != null) ? "not null" : "NULL"));
			return false;
		}

		public override string ToString()
		{
			return "TargetActor = " + ((base.TargetActor != null) ? base.TargetActor.DebugInfo : "null") + ", Avoided = " + base.Avoided;
		}
	}
}
