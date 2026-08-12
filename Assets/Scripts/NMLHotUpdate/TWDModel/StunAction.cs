using System;
using BaseModel;

namespace TWDModel
{
	public class StunAction : StatusEffectAction
	{
		public string CausedByTrait;

		public int Turns { get; private set; }

		public bool IgnoreSourceBeingDead { get; private set; }

		public CanNotAvoidStunType CanNotAvoidStunType { get; set; }

		public StunAction(ActorModel sourceActor, ActorModel targetActor, int turns, bool ignoreSourceBeingDead = false, SupportModel sourceSupport = null, Func<int> damage = null, CanNotAvoidStunType canNotAvoidStunType = CanNotAvoidStunType.None)
			: base(sourceActor, targetActor, sourceSupport, damage)
		{
			Turns = turns;
			base.Avoided = false;
			IgnoreSourceBeingDead = ignoreSourceBeingDead;
			CanNotAvoidStunType = canNotAvoidStunType;
		}

		public override bool Execute(ModelManager manager)
		{
			TWDModelManager tWDModelManager = (TWDModelManager)manager;
			CombatModel combatModel = tWDModelManager.CombatModel;
			if (combatModel != null && base.SourceActor != null && base.SourceActor.IsValid() && base.TargetActor != null && base.TargetActor.IsValid())
			{
				bool flag = base.TargetActor.IsStruggling && base.TargetActor.ExclusiveTimedEffect.Target == null;
				if (!base.Avoided && !base.TargetActor.IsDead && !flag && !base.TargetActor.IsDisoriented && !base.TargetActor.IsElectricShocked && !base.TargetActor.IsABTesterAed && !base.TargetActor.IsABTesterA2ed && (IgnoreSourceBeingDead || !base.SourceActor.IsDead))
				{
					base.TargetActor.Stun(Turns, base.SourceActor);
					tWDModelManager.ExecuteAction(new PostStatusEffectAction(base.SourceActor, base.TargetActor, TimedEffectType.Stun, base.SourceSupport, Turns, CausedByTrait));
				}
				return true;
			}
			(manager as TWDModelManager).Debug.LogError("Stun action failed - CombatModel: " + ((combatModel != null) ? "not null" : "NULL") + " Source Actor: " + ((base.SourceActor != null) ? "not null" : "NULL") + " Target Actor: " + ((base.TargetActor != null) ? "not null" : "NULL"));
			return false;
		}

		public override string ToString()
		{
			return "SourceActor = " + ((base.SourceActor != null) ? base.SourceActor.DebugInfo : "null") + ", TargetActor = " + ((base.TargetActor != null) ? base.TargetActor.DebugInfo : "null") + ", Avoided = " + base.Avoided + ", Turns = " + Turns;
		}
	}
}
