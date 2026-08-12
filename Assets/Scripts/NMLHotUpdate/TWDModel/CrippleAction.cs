using System;
using BaseModel;

namespace TWDModel
{
	public class CrippleAction : StatusEffectAction
	{
		public int Turns { get; private set; }

		public bool IgnoreSourceBeingDead { get; private set; }

		public CrippleAction(ActorModel sourceActor, ActorModel targetActor, int turns, bool ignoreSourceBeingDead = false, SupportModel sourceSupport = null, Func<int> damage = null)
			: base(sourceActor, targetActor, sourceSupport, damage)
		{
			Turns = turns;
			base.Avoided = false;
			IgnoreSourceBeingDead = ignoreSourceBeingDead;
		}

		public override bool Execute(ModelManager manager)
		{
			TWDModelManager tWDModelManager = manager as TWDModelManager;
			CombatModel combatModel = tWDModelManager?.CombatModel;
			if (combatModel != null && base.SourceActor != null && base.SourceActor.IsValid() && base.TargetActor != null && base.TargetActor.IsValid())
			{
				bool flag = base.TargetActor.IsStruggling && base.TargetActor.ExclusiveTimedEffect.Target == null;
				if (!base.Avoided && !base.TargetActor.IsDead && !flag && !base.TargetActor.IsStunned && !base.TargetActor.IsElectricShocked && !base.TargetActor.IsRooted && !base.TargetActor.IsPitfalled && !base.TargetActor.IsDisoriented && !base.TargetActor.IsDisorientedLock && !base.TargetActor.IsABTesterA2ed && !base.TargetActor.IsABTesterAed && (IgnoreSourceBeingDead || !base.SourceActor.IsDead))
				{
					FixedPoint value = 0.0;
					tWDModelManager.CombatModel.AbilityManager.VisitParameter("ExtendProbability", ref value, base.SourceActor);
					FixedPoint value2 = 0.0;
					tWDModelManager.CombatModel.AbilityManager.VisitParameter("SupportTalent_MoveCritRateParm1", ref value2, base.SourceActor);
					PlayerRandomChanceResult num = tWDModelManager.Player.RollDice(RollDiceType.Cripple, value2, value);
					FixedPoint value3 = 0L;
					if (num != PlayerRandomChanceResult.Failed)
					{
						tWDModelManager.CombatModel.AbilityManager.VisitParameter("SupportTalent_MoveCritRateParm2", ref value3, base.SourceActor);
					}
					base.TargetActor.Cripple(Turns + (int)value3, base.SourceActor);
					tWDModelManager.ExecuteAction(new PostStatusEffectAction(base.SourceActor, base.TargetActor, TimedEffectType.Crippled, base.SourceSupport, Turns + (int)value3));
				}
				return true;
			}
			(manager as TWDModelManager)?.Debug.LogError("Cripple action failed - CombatModel: " + ((combatModel != null) ? "not null" : "NULL") + " Source Actor: " + ((base.SourceActor != null) ? "not null" : "NULL") + " Target Actor: " + ((base.TargetActor != null) ? "not null" : "NULL"));
			return false;
		}

		public override string ToString()
		{
			return "SourceActor = " + ((base.SourceActor != null) ? base.SourceActor.DebugInfo : "null") + ", TargetActor = " + ((base.TargetActor != null) ? base.TargetActor.DebugInfo : "null") + ", Avoided = " + base.Avoided + ", Turns = " + Turns;
		}
	}
}
