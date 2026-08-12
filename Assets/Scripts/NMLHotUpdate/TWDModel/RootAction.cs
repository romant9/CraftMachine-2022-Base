using System;
using BaseModel;

namespace TWDModel
{
	public class RootAction : StatusEffectAction
	{
		public int Turns { get; private set; }

		public bool IgnoreSourceBeingDead { get; private set; }

		public RootAction(ActorModel sourceActor, ActorModel targetActor, int turns, bool ignoreSourceBeingDead = false, SupportModel sourceSupport = null, Func<int> damage = null)
			: base(sourceActor, targetActor, sourceSupport, damage)
		{
			Turns = turns;
			base.Avoided = false;
			IgnoreSourceBeingDead = ignoreSourceBeingDead;
		}

		public override bool Execute(ModelManager manager)
		{
			TWDModelManager tWDModelManager = (TWDModelManager)manager;
			CombatModel combatModel = tWDModelManager.CombatModel;
			if (combatModel != null && base.SourceActor != null && base.SourceActor.IsValid() && base.TargetActor != null && base.TargetActor.IsValid())
			{
				bool flag = base.TargetActor.IsStruggling && base.TargetActor.ExclusiveTimedEffect.Target == null;
				if (!base.Avoided && !base.TargetActor.IsDead && !flag && !base.TargetActor.IsStunned && !base.TargetActor.IsElectricShocked && !base.TargetActor.IsDisoriented && !base.TargetActor.IsDisorientedLock && !base.TargetActor.IsPitfalled && !base.TargetActor.IsABTesterAed && !base.TargetActor.IsABTesterA2ed && (IgnoreSourceBeingDead || !base.SourceActor.IsDead))
				{
					FixedPoint value = 0.0;
					tWDModelManager.CombatModel.AbilityManager.VisitParameter("ExtendProbability", ref value, base.SourceActor);
					FixedPoint value2 = 0.0;
					tWDModelManager.CombatModel.AbilityManager.VisitParameter("SupportTalent_MoveHitrateParm1", ref value2, base.SourceActor);
					PlayerRandomChanceResult num = tWDModelManager.Player.RollDice(RollDiceType.Root, value2, value);
					FixedPoint value3 = 0L;
					if (num != PlayerRandomChanceResult.Failed)
					{
						tWDModelManager.CombatModel.AbilityManager.VisitParameter("SupportTalent_MoveHitrateParm2", ref value3, base.SourceActor);
					}
					base.TargetActor.Root(Turns + (int)value3, base.SourceActor);
					tWDModelManager.ExecuteAction(new PostStatusEffectAction(base.SourceActor, base.TargetActor, TimedEffectType.Root, base.SourceSupport, Turns + (int)value3));
				}
				return true;
			}
			(manager as TWDModelManager).Debug.LogError("Root action failed - CombatModel: " + ((combatModel != null) ? "not null" : "NULL") + " Source Actor: " + ((base.SourceActor != null) ? "not null" : "NULL") + " Target Actor: " + ((base.TargetActor != null) ? "not null" : "NULL"));
			return false;
		}

		public override string ToString()
		{
			return "SourceActor = " + ((base.SourceActor != null) ? base.SourceActor.DebugInfo : "null") + ", TargetActor = " + ((base.TargetActor != null) ? base.TargetActor.DebugInfo : "null") + ", Avoided = " + base.Avoided + ", Turns = " + Turns;
		}
	}
}
