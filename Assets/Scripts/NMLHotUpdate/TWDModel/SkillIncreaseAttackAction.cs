using BaseModel;

namespace TWDModel
{
	public class SkillIncreaseAttackAction : ModelAction
	{
		public ActorModel Target { get; set; }

		public ActorModel Source { get; private set; }

		public FixedPoint NormalAttackMultiplier { get; private set; }

		public FixedPoint ChargeAttackMultiplier { get; private set; }

		public int Turns { get; private set; }

		public bool Avoided { get; set; }

		public SkillIncreaseAttackAction(ActorModel actor, ActorModel target, FixedPoint normalAttackMultiplier, FixedPoint chargeAttackMultiplier, int turns)
			: base(actor)
		{
			Source = actor;
			Target = target;
			NormalAttackMultiplier = normalAttackMultiplier;
			ChargeAttackMultiplier = chargeAttackMultiplier;
			Turns = turns;
		}

		public override bool Execute(ModelManager manager)
		{
			CombatModel combatModel = (manager as TWDModelManager)?.CombatModel;
			if (combatModel != null && Source != null && Source.IsValid() && Target != null && Target.IsValid() && !Avoided && !Target.IsDead)
			{
				Target.StartSkillIncreaseAttack(NormalAttackMultiplier, ChargeAttackMultiplier, Turns, Source);
				return true;
			}
			(manager as TWDModelManager)?.Debug.LogError("SkillIncreaseAttackAction action failed - CombatModel: " + ((combatModel != null) ? "not null" : "NULL") + " Source Actor: " + ((Source != null) ? "not null" : "NULL") + " Target Actor: " + ((Target != null) ? "not null" : "NULL"));
			return false;
		}

		public override string ToString()
		{
			return "SourceActor = " + ((Source != null) ? Source.DebugInfo : "null") + ", TargetActor = " + ((Target != null) ? Target.DebugInfo : "null");
		}
	}
}
