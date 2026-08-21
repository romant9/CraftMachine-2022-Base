using System;
using System.Collections.Generic;
using BaseModel;

namespace TWDModel
{
	public class ElectricShockAction : StatusEffectAction
	{
		public string CausedByTrait;

		public int Turns { get; private set; }

		public int AsElectronChargeLayer { get; private set; }

		public ElectricShockAction(ActorModel sourceActor, ActorModel targetActor, int turns, int asElectronChargeLayer, SupportModel sourceSupport = null, Func<int> damage = null)
			: base(sourceActor, targetActor, sourceSupport, damage)
		{
			Turns = turns;
			AsElectronChargeLayer = asElectronChargeLayer;
			base.Avoided = false;
		}

		public override bool Execute(ModelManager manager)
		{
			TWDModelManager tWDModelManager = (TWDModelManager)manager;
			CombatModel combatModel = tWDModelManager.CombatModel;
			if (combatModel != null && base.SourceActor != null && base.SourceActor.IsValid() && base.TargetActor != null && base.TargetActor.IsValid())
			{
				if (!base.Avoided && !base.TargetActor.IsDead && !base.TargetActor.IsDisoriented && !base.TargetActor.IsDisorientedLock)
				{
					FixedPoint successProbability = 0L;
					if (base.TargetActor.Faction == Faction.Walker || base.TargetActor.Faction == Faction.Raider)
					{
						IChallengeDebuffProvider challengeDebuffProvider = MapMissionDebuffHelper.CanUseDebuffMission(tWDModelManager);
						if (challengeDebuffProvider != null)
						{
							List<DifficultyIncrementalDebuff> challengeDebuffs = challengeDebuffProvider.GetChallengeDebuffs();
							if (ChallengeDebufHelps.GetDebufConfig(challengeDebuffs, ChallengeDebuffType.WalkerStateRefSpecialStun) != null)
							{
								successProbability = (int)ChallengeDebufHelps.GetDebufTotalFirstParam(challengeDebuffs, ChallengeDebuffType.WalkerStateRefSpecialStun);
								successProbability *= (FixedPoint)0.01;
							}
						}
					}
					FixedPoint value = 0.0;
					tWDModelManager.CombatModel.AbilityManager.VisitParameter("ExtendProbability", ref value, base.TargetActor);
					if (tWDModelManager.Player.RollDice(RollDiceType.Stun, successProbability, value) != PlayerRandomChanceResult.Failed)
					{
						return true;
					}
					base.TargetActor.StartElectricShock(Turns, base.SourceActor, AsElectronChargeLayer);
					tWDModelManager.ExecuteAction(new PostStatusEffectAction(base.SourceActor, base.TargetActor, TimedEffectType.ElectricShock, base.SourceSupport, Turns, CausedByTrait));
				}
				return true;
			}
			(manager as TWDModelManager).Debug.LogError("ElectricShock action failed - CombatModel: " + ((combatModel != null) ? "not null" : "NULL") + " Source Actor: " + ((base.SourceActor != null) ? "not null" : "NULL") + " Target Actor: " + ((base.TargetActor != null) ? "not null" : "NULL"));
			return false;
		}

		public override string ToString()
		{
			return "SourceActor = " + ((base.SourceActor != null) ? base.SourceActor.DebugInfo : "null") + ", TargetActor = " + ((base.TargetActor != null) ? base.TargetActor.DebugInfo : "null") + ", Avoided = " + base.Avoided + ", Turns = " + Turns;
		}
	}
}
