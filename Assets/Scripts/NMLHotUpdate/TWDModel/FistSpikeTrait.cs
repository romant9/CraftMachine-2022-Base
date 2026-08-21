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
					IChallengeDebuffProvider challengeDebuffProvider = MapMissionDebuffHelper.CanUseDebuffMission(base.manager);
					if (challengeDebuffProvider != null)
					{
						List<DifficultyIncrementalDebuff> challengeDebuffs = challengeDebuffProvider.GetChallengeDebuffs();
						if (ChallengeDebufHelps.GetDebufConfig(challengeDebuffs, ChallengeDebuffType.WalkerStateRefFistSpike) != null)
						{
							fixedPoint = (int)ChallengeDebufHelps.GetDebufTotalFirstParam(challengeDebuffs, ChallengeDebuffType.WalkerStateRefFistSpike);
							fixedPoint *= (FixedPoint)0.01;
						}
					}
				}
				FixedPoint fixedPoint2 = 0.0;
				if (ResistNegativeEffectsTrait.TryResist(postDamageAction.TargetActor, "FistSpike"))
				{
					return ActionListClearFlag.Keep;
				}
				fixedPoint2 = 0.0;
				base.manager.CombatModel.AbilityManager.VisitParameter("ExtendProbability", ref fixedPoint2, actor);
				if (base.manager.Player.RollDice(RollDiceType.FistSpike, Percentage - fixedPoint, fixedPoint2) != PlayerRandomChanceResult.Failed)
				{
					if (EquipmentPassivePreventControlTrait.TryResistFistSpike(postDamageAction.TargetActor))
					{
						return ActionListClearFlag.Keep;
					}
					postDamageAction.TargetActor.FistSpikeTurns = Turns;
					postDamageAction.TargetActor.NotifyChange("AbilityVisited", new object[2] { "FistSpike", false });
					postDamageAction.TargetActor.NotifyChange("RefreshFistSpikeTurns");
				}
			}
			return ActionListClearFlag.Keep;
		}
	}
}
