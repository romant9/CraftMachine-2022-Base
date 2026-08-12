using System;
using BaseModel;

namespace TWDModel
{
	public class StaggerAction : StatusEffectAction
	{
		public int Turns { get; private set; }

		public FixedPoint Chance { get; private set; }

		public bool IgnoreSourceBeingDead { get; private set; }

		public StaggerAction(ActorModel sourceActor, ActorModel targetActor, int turns, FixedPoint chance, bool ignoreSourceBeingDead = false, SupportModel sourceSupport = null, Func<int> damage = null)
			: base(sourceActor, targetActor, sourceSupport, damage)
		{
			Chance = chance;
			Turns = turns;
			base.Avoided = false;
			IgnoreSourceBeingDead = ignoreSourceBeingDead;
		}

		public override bool Execute(ModelManager manager)
		{
			CombatModel combatModel = ((TWDModelManager)manager).CombatModel;
			if (combatModel != null && base.SourceActor != null && base.SourceActor.IsValid() && base.TargetActor != null && base.TargetActor.IsValid())
			{
				if (!base.Avoided && !base.TargetActor.IsDead && (IgnoreSourceBeingDead || !base.SourceActor.IsDead))
				{
					base.TargetActor.StartStagger(Turns, base.TargetActor, Chance);
				}
				return true;
			}
			(manager as TWDModelManager).Debug.LogError("Stagger action failed - CombatModel: " + ((combatModel != null) ? "not null" : "NULL") + " Source Actor: " + ((base.SourceActor != null) ? "not null" : "NULL") + " Target Actor: " + ((base.TargetActor != null) ? "not null" : "NULL"));
			return false;
		}

		public override string ToString()
		{
			return "SourceActor = " + ((base.SourceActor != null) ? base.SourceActor.DebugInfo : "null") + ", TargetActor = " + ((base.TargetActor != null) ? base.TargetActor.DebugInfo : "null") + ", Avoided = " + base.Avoided + ", Turns = " + Turns;
		}
	}
}
