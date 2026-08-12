using System.Collections.Generic;

namespace TWDModel
{
	public class FistSpikeTrait : ActionModifier
	{
		private FixedPoint Percentage;

		private int Turns;

		public FistSpikeTrait(FixedPoint percentage, int turns)
		{
			Percentage = percentage;
			Turns = turns;
		}

		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			if (action is PostDamageAction postDamageAction && postDamageAction.DamagerActor == actor && !postDamageAction.TargetActor.IsDead && base.manager.CombatModel.TurnManager.ActiveFaction == actor.Faction && postDamageAction.DamageAction.SourceSupport == null && postDamageAction.DamagerActor.Faction != Faction.Environmental && !postDamageAction.TargetActor.IsStunned && postDamageAction.TargetActor.ParryRiposteIncreaseStorey <= 0)
			{
				FixedPoint fixedPoint = 0L;
				if (postDamageAction.TargetActor.Faction == Faction.Walker || postDamageAction.TargetActor.Faction == Faction.Raider)
				{
					MapMissionModel mapMissionModel = MapMissionDebuffHelper.CanUseDebuffMission(base.manager);
					if (mapMissionModel != null)
					{
						List<DifficultyIncrementalDebuff> challengeDebuffs = mapMissionModel.GetChallengeDebuffs();
						if (ChallengeDebufHelps.GetDebufConfig(challengeDebuffs, ChallengeDebuffType.WalkerStateRefFistSpike) != null)
						{
							fixedPoint = (int)ChallengeDebufHelps.GetDebufTotalFirstParam(challengeDebuffs, ChallengeDebuffType.WalkerStateRefFistSpike);
							fixedPoint *= (FixedPoint)0.01;
						}
					}
				}
				FixedPoint value = 0.0;
				base.manager.CombatModel.AbilityManager.VisitParameter("ExtendProbability", ref value, actor);
				if (base.manager.Player.RollDice(RollDiceType.FistSpike, Percentage - fixedPoint, value) != PlayerRandomChanceResult.Failed)
				{
					postDamageAction.TargetActor.FistSpikeTurns = Turns;
					postDamageAction.TargetActor.NotifyChange("AbilityVisited", new object[2] { "FistSpike", false });
					postDamageAction.TargetActor.NotifyChange("RefreshFistSpikeTurns");
				}
			}
			return ActionListClearFlag.Keep;
		}
	}
}
