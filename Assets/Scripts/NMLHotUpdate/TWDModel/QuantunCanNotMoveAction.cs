using System;
using BaseModel;

namespace TWDModel
{
	public class QuantunCanNotMoveAction : StatusEffectAction
	{
		public int Turns { get; private set; }

		public bool IgnoreSourceBeingDead { get; private set; }

		public QuantunCanNotMoveAction(ActorModel sourceActor, ActorModel targetActor, int turns, bool ignoreSourceBeingDead = false, SupportModel sourceSupport = null, Func<int> damage = null)
			: base(sourceActor, targetActor, sourceSupport, damage)
		{
			Turns = turns;
			base.Avoided = false;
			IgnoreSourceBeingDead = ignoreSourceBeingDead;
		}

		public override bool Execute(ModelManager manager)
		{
			CombatModel combatModel = ((TWDModelManager)manager).CombatModel;
			if (combatModel != null && base.TargetActor != null && base.TargetActor.IsValid())
			{
				if (!base.Avoided && !base.TargetActor.IsDead && !base.TargetActor.IsDisoriented && !base.TargetActor.IsABTesterA2ed && !base.TargetActor.IsABTesterAed && !base.TargetActor.IsElectricShocked && !base.TargetActor.IsStunned)
				{
					base.TargetActor.StartQuantunCanNotMove(Turns);
				}
				return true;
			}
			(manager as TWDModelManager).Debug.LogError("Quantun can not move action failed - CombatModel: " + ((combatModel != null) ? "not null" : "NULL") + " Source Actor: " + ((base.SourceActor != null) ? "not null" : "NULL") + " Target Actor: " + ((base.TargetActor != null) ? "not null" : "NULL"));
			return false;
		}

		public override string ToString()
		{
			return "SourceActor = " + ((base.SourceActor != null) ? base.SourceActor.DebugInfo : "null") + ", TargetActor = " + ((base.TargetActor != null) ? base.TargetActor.DebugInfo : "null") + ", Avoided = " + base.Avoided + ", Turns = " + Turns;
		}
	}
}
