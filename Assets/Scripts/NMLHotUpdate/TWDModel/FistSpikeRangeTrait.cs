using System;
using System.Collections.Generic;

namespace TWDModel
{
	public class FistSpikeRangeTrait : ActionModifier
	{
		private int Range;

		private FixedPoint Percentage;

		private int Turns;

		private bool IsCanSpike;

		private bool IsClearCarolAttackTurn;

		public FistSpikeRangeTrait(int range, FixedPoint percentage, int turns)
		{
			Range = range;
			Percentage = percentage;
			Turns = turns;
		}

		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			try
			{
				if (action is AbilityAction)
				{
					IsClearCarolAttackTurn = false;
				}
				if (action is AbilityAction abilityAction && abilityAction.Actor == actor)
				{
					IsCanSpike = true;
				}
				if (action is DamageAction { TargetActor: not null } damageAction && actor != null && damageAction.TargetActor == actor)
				{
					IsClearCarolAttackTurn = true;
				}
				if (action is PostDamageAction postDamageAction)
				{
					if (actor == null)
					{
						return ActionListClearFlag.Keep;
					}
					CombatModel combatModel = actor.manager.CombatModel;
					if (combatModel == null)
					{
						return ActionListClearFlag.Keep;
					}
					if (postDamageAction.DamagerActor == null)
					{
						return ActionListClearFlag.Keep;
					}
					if (postDamageAction.TargetActor == null)
					{
						return ActionListClearFlag.Keep;
					}
					if (!postDamageAction.DamagerActor.IsSneak)
					{
						return ActionListClearFlag.Keep;
					}
					if (!IsCanSpike)
					{
						return ActionListClearFlag.Keep;
					}
					List<ActorModel> allActors = combatModel.GetAllActors();
					if (allActors == null || allActors.Count <= 0)
					{
						return ActionListClearFlag.Keep;
					}
					foreach (ActorModel item in allActors)
					{
						if (postDamageAction.DamagerActor.IsEnemy(item))
						{
							AddSpike(combatModel, postDamageAction, item, actor);
						}
					}
					IsCanSpike = false;
				}
				if (action is AbilityBeforeRemoveActiveTraitAction abilityBeforeRemoveActiveTraitAction)
				{
					if (actor != null && IsClearCarolAttackTurn)
					{
						actor.ClearCarolAttackTurn();
					}
					if (abilityBeforeRemoveActiveTraitAction.Source != null && actor != null && abilityBeforeRemoveActiveTraitAction.Source == actor)
					{
						if (abilityBeforeRemoveActiveTraitAction.AbilityAction != null && abilityBeforeRemoveActiveTraitAction.AbilityAction.Ability != null && abilityBeforeRemoveActiveTraitAction.AbilityAction.Ability.IsConsumableAbility && !abilityBeforeRemoveActiveTraitAction.AbilityAction.Ability.IsAttackAbility)
						{
							return ActionListClearFlag.Keep;
						}
						abilityBeforeRemoveActiveTraitAction.Source.ClearCarolAttackTurn();
					}
				}
			}
			catch (Exception arg)
			{
				base.Debug.LogError($"FistSpikeRangeTrait error:{arg}");
			}
			return ActionListClearFlag.Keep;
		}

		private void AddSpike(CombatModel combatModel, PostDamageAction postDamageAction, ActorModel target, ActorModel actor)
		{
			if (combatModel == null || postDamageAction == null || target == null || actor == null || !CombatHelpers.IsWithinRange(combatModel, Range, postDamageAction.DamagerActor.GridCoordinate, target.GridCoordinate) || postDamageAction.DamagerActor != actor || target.IsDead || base.manager.CombatModel.TurnManager.ActiveFaction != actor.Faction || postDamageAction.DamageAction.SourceSupport != null || postDamageAction.DamagerActor.Faction == Faction.Environmental || target.IsStunned)
			{
				return;
			}
			FixedPoint fixedPoint = 0L;
			if (target.Faction == Faction.Walker || target.Faction == Faction.Raider)
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
				target.FistSpikeTurns = Turns;
				target.NotifyChange("AbilityVisited", new object[2] { "FistSpike", false });
				target.NotifyChange("RefreshFistSpikeTurns");
			}
		}
	}
}
