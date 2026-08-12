using BaseModel;

namespace TWDModel
{
	public class ShieldAction : ModelAction
	{
		public ActorModel Target { get; set; }

		public ActorModel Source { get; private set; }

		public int Turns { get; private set; }

		public int Shield { get; private set; }

		public ShieldAction(ActorModel actor, ActorModel target, int turns, int shield)
			: base(actor)
		{
			Source = actor;
			Target = target;
			Turns = turns;
			Shield = shield;
		}

		public override bool Execute(ModelManager manager)
		{
			CombatModel combatModel = (manager as TWDModelManager)?.CombatModel;
			if (combatModel != null && Source != null && Source.IsValid() && Target != null && Target.IsValid() && !Source.IsShieldBreaker())
			{
				if (!Source.IsDead)
				{
					Source.Shield(Turns, Shield, Source);
				}
				return true;
			}
			(manager as TWDModelManager)?.Debug.LogError("Shield action failed - CombatModel: " + ((combatModel != null) ? "not null" : "NULL") + " Source Actor: " + ((Source != null) ? "not null" : "NULL") + " Target Actor: " + ((Source != null) ? "not null" : "NULL"));
			return false;
		}

		public override string ToString()
		{
			return "SourceActor = " + ((Source != null) ? Source.DebugInfo : "null") + ", TargetActor = " + ((Target != null) ? Target.DebugInfo : "null");
		}
	}
}
