using System.Collections.Generic;

namespace TWDModel
{
	public class AbilityModifierDodgeShot : ActionModifier
	{
		private FixedPoint DodgeShotChance;

		private FixedPoint DamageTimes;

		private FixedPoint Turns;

		private FixedPoint InjureDodgedChance;

		public AbilityModifierDodgeShot(FixedPoint dodgeShotChance, FixedPoint damageTimes, FixedPoint turns, FixedPoint injureDodgedChance)
		{
			DodgeShotChance = dodgeShotChance;
			DamageTimes = damageTimes;
			Turns = turns;
			InjureDodgedChance = injureDodgedChance;
		}

		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			if (action is PostDamageAction { DamagerActor: not null } postDamageAction && postDamageAction.DamagerActor.Faction == actor.Faction && postDamageAction.DamagerActor == actor && !postDamageAction.TargetActor.IsDead && !postDamageAction.TargetActor.IsStunned && postDamageAction.DamageAction.SourceSupport == null)
			{
				PlayerRandomChanceResult playerRandomChanceResult = PlayerRandomChanceResult.Failed;
				FixedPoint fixedPoint = 0L;
				if (postDamageAction.TargetActor.Faction == Faction.Walker || postDamageAction.TargetActor.Faction == Faction.Raider)
				{
					IChallengeDebuffProvider challengeDebuffProvider = MapMissionDebuffHelper.CanUseDebuffMission(base.manager);
					if (challengeDebuffProvider != null)
					{
						List<DifficultyIncrementalDebuff> challengeDebuffs = challengeDebuffProvider.GetChallengeDebuffs();
						if (ChallengeDebufHelps.GetDebufConfig(challengeDebuffs, ChallengeDebuffType.WalkerStateRefDodgedShotInjurerFlag) != null)
						{
							fixedPoint = (int)ChallengeDebufHelps.GetDebufTotalFirstParam(challengeDebuffs, ChallengeDebuffType.WalkerStateRefDodgedShotInjurerFlag);
							fixedPoint *= (FixedPoint)0.01;
						}
					}
				}
				FixedPoint value = 0.0;
				if (ResistNegativeEffectsTrait.TryResist(postDamageAction.TargetActor, "DodgedShotInjurerFlag"))
				{
					return ActionListClearFlag.Keep;
				}
				if (DodgeShotChance - fixedPoint > 0.0)
				{
					base.manager.CombatModel.AbilityManager.VisitParameter("ExtendProbability", ref value, actor);
					playerRandomChanceResult = base.manager.Player.RollDice(RollDiceType.Dodge, DodgeShotChance - fixedPoint, value);
				}
				if (HelpersModel.IsDodge)
				{
					if (!actor.IsEnemy(actor) && playerRandomChanceResult == PlayerRandomChanceResult.Failed)
					{
						playerRandomChanceResult = PlayerRandomChanceResult.Success;
					}
				}
				if (playerRandomChanceResult != PlayerRandomChanceResult.Failed)
				{
					if (EquipmentPassivePreventControlTrait.TryResistEffect(postDamageAction.TargetActor, "DodgedShotInjurerFlag", RollDiceType.Dodge))
					{
						return ActionListClearFlag.Keep;
					}
					postDamageAction.TargetActor.DodgeShotTimes = (int)DamageTimes;
					postDamageAction.TargetActor.DodgeShotTurns = (int)Turns;
					postDamageAction.TargetActor.NotifyChange("AbilityVisited", new object[2] { "DodgeShot", false });
					postDamageAction.TargetActor.NotifyChange("RefreshDodgeShot");
					postDamageAction.TargetActor.AddTemporaryTrait("DodgedShotInjurerFlag", InjureDodgedChance, null, 0L);
				}
			}
			return ActionListClearFlag.Keep;
		}
	}
}
