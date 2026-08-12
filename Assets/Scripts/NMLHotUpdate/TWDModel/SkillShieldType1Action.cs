using BaseModel;

namespace TWDModel
{
	public class SkillShieldType1Action : ModelAction
	{
		public ActorModel Target { get; set; }

		public ActorModel Source { get; private set; }

		public int Turns { get; private set; }

		public int Shield { get; private set; }

		public bool Avoided { get; set; }

		public SkillShieldType1Action(ActorModel actor, ActorModel target, int turns, int shield)
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
			if (combatModel != null && Source != null && Source.IsValid() && Target != null && Target.IsValid() && !Avoided && !Target.IsDead && !Target.IsShieldBreaker())
			{
				Target.StartSkillShieldType1(Turns, Shield, Source);
				return true;
			}
			(manager as TWDModelManager)?.Debug.LogError("SkillShieldType1 action failed - CombatModel: " + ((combatModel != null) ? "not null" : "NULL") + " Source Actor: " + ((Source != null) ? "not null" : "NULL") + " Target Actor: " + ((Target != null) ? "not null" : "NULL"));
			return false;
		}

		public override string ToString()
		{
			return "SourceActor = " + ((Source != null) ? Source.DebugInfo : "null") + ", TargetActor = " + ((Target != null) ? Target.DebugInfo : "null");
		}
	}
}
