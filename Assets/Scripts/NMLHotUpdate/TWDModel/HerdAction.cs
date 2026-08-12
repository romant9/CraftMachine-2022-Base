using System;
using System.Collections.Generic;
using BaseModel;

namespace TWDModel
{
	public class HerdAction : StatusEffectAction
	{
		public int Turns { get; private set; }

		public int AffectedActors { get; private set; }

		public HerdAction(ActorModel sourceActor, ActorModel targetActor, int turns = 1, int affectedActors = 0, SupportModel sourceSupport = null, Func<int> damage = null)
			: base(sourceActor, targetActor, sourceSupport, damage)
		{
			Turns = turns;
			AffectedActors = affectedActors;
		}

		public override bool Execute(ModelManager manager)
		{
			TWDModelManager tWDModelManager = (TWDModelManager)manager;
			CombatModel combatModel = tWDModelManager.CombatModel;
			if (combatModel != null && base.SourceActor != null && base.SourceActor.IsValid() && base.TargetActor != null && base.TargetActor.IsValid())
			{
				if (!base.Avoided && !base.TargetActor.IsDead && !base.TargetActor.IsStruggling && !base.TargetActor.IsPitfalled && !base.TargetActor.IsStunned && !base.TargetActor.IsElectricShocked && !base.TargetActor.IsEatingLure && base.TargetActor.Faction == Faction.Walker && !base.TargetActor.IsDisoriented && !base.TargetActor.IsDisorientedLock && !base.TargetActor.IsABTesterA2ed && !base.TargetActor.IsABTesterAed)
				{
					MapMissionModel mapMissionModel = MapMissionDebuffHelper.CanUseDebuffMission(combatModel.manager);
					if (mapMissionModel != null)
					{
						List<DifficultyIncrementalDebuff> challengeDebuffs = mapMissionModel.GetChallengeDebuffs();
						if (base.TargetActor.IsWalker)
						{
							int chance = (int)ChallengeDebufHelps.GetDebufTotalFirstParam(challengeDebuffs, ChallengeDebuffType.DebuffHerdRate);
							if (tWDModelManager.Player.RollDice(RollDiceType.AvoidHerd, chance) == PlayerRandomChanceResult.Success)
							{
								base.Avoided = true;
								return true;
							}
						}
					}
					if (!base.SourceActor.IsDead)
					{
						base.TargetActor.Herd(Turns, base.SourceActor);
						tWDModelManager.ExecuteAction(new PostStatusEffectAction(base.SourceActor, base.TargetActor, TimedEffectType.Herd, base.SourceSupport, Turns));
					}
				}
				return true;
			}
			(manager as TWDModelManager).Debug.LogError("Taunt action failed - CombatModel: " + ((combatModel != null) ? "not null" : "NULL") + " Source Actor: " + ((base.SourceActor != null) ? "not null" : "NULL") + " Target Actor: " + ((base.TargetActor != null) ? "not null" : "NULL"));
			return false;
		}

		public override string ToString()
		{
			return "SourceActor = " + ((base.SourceActor != null) ? base.SourceActor.DebugInfo : "null") + ", TargetActor = " + ((base.TargetActor != null) ? base.TargetActor.DebugInfo : "null") + ", Turns = " + Turns;
		}
	}
}
