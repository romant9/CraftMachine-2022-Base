using System;
using System.Collections.Generic;
using BaseModel;

namespace TWDModel
{
	public class ABTesterAction : StatusEffectAction
	{
		public string CausedByTrait;

		public int Turns { get; private set; }

		public bool IgnoreSourceBeingDead { get; private set; }

		public ABTesterAction(ActorModel sourceActor, ActorModel targetActor, int turns, bool ignoreSourceBeingDead = false, SupportModel sourceSupport = null, Func<int> damage = null)
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
				if (!base.Avoided)
				{
					List<ActorModel> oneGridWalkRaiderModels = combatModel.GetOneGridWalkRaiderModels(base.TargetActor);
					oneGridWalkRaiderModels.RemoveAll((ActorModel t) => t.IsABTesterAed);
					if (oneGridWalkRaiderModels != null && oneGridWalkRaiderModels.Count > 0)
					{
						FixedPoint value = 0.0;
						combatModel.AbilityManager.VisitParameter("LeaderBuffABTesterAMaxNum", ref value, base.SourceActor);
						int num = 0;
						for (int num2 = 0; num2 < oneGridWalkRaiderModels.Count; num2++)
						{
							if (!oneGridWalkRaiderModels[num2].IsDisoriented && !oneGridWalkRaiderModels[num2].IsDisorientedLock && !oneGridWalkRaiderModels[num2].IsRaider)
							{
								if (oneGridWalkRaiderModels[num2].IsStruggling || oneGridWalkRaiderModels[num2].IsStunned || oneGridWalkRaiderModels[num2].IsElectricShocked || oneGridWalkRaiderModels[num2].IsRooted || oneGridWalkRaiderModels[num2].IsCrippled || oneGridWalkRaiderModels[num2].IsHerded || oneGridWalkRaiderModels[num2].IsLured || oneGridWalkRaiderModels[num2].IsEatingLure)
								{
									oneGridWalkRaiderModels[num2].FinishTimedEffect(interrupted: true);
								}
								oneGridWalkRaiderModels[num2].AIController.AttackTarget(base.TargetActor);
								oneGridWalkRaiderModels[num2].StartABTesterA2(1, oneGridWalkRaiderModels[num2]);
								num++;
								if (num >= value)
								{
									break;
								}
							}
						}
					}
					if (!base.TargetActor.IsDead && (IgnoreSourceBeingDead || !base.SourceActor.IsDead))
					{
						base.TargetActor.StartABTesterA(base.SourceActor, Turns);
						tWDModelManager.ExecuteAction(new PostStatusEffectAction(base.SourceActor, base.TargetActor, TimedEffectType.ABTesterA, base.SourceSupport, Turns, CausedByTrait));
					}
				}
				return true;
			}
			(manager as TWDModelManager).Debug.LogError("ABTesterA action failed - CombatModel: " + ((combatModel != null) ? "not null" : "NULL") + " Source Actor: " + ((base.SourceActor != null) ? "not null" : "NULL") + " Target Actor: " + ((base.TargetActor != null) ? "not null" : "NULL"));
			return false;
		}

		public override string ToString()
		{
			return "SourceActor = " + ((base.SourceActor != null) ? base.SourceActor.DebugInfo : "null") + ", TargetActor = " + ((base.TargetActor != null) ? base.TargetActor.DebugInfo : "null") + ", Avoided = " + base.Avoided + ", Turns = " + Turns;
		}
	}
}
