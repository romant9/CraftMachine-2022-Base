using System.Collections.Generic;

namespace TWDModel
{
	public class UndyingTrait : ActionModifier
	{
		private readonly int firstGrantRound;

		private readonly int regrantIntervalRounds;

		private readonly int maxTotalGrants;

		private readonly FixedPoint healPercentage;

		private readonly int immuneHits;

		private readonly int immuneHitsRounds;

		public UndyingTrait(int firstGrantRound, int regrantIntervalRounds, int maxTotalGrants, FixedPoint healPercentage, int immuneHits, int immuneHitsRounds)
		{
			this.firstGrantRound = firstGrantRound;
			this.regrantIntervalRounds = regrantIntervalRounds;
			this.maxTotalGrants = maxTotalGrants;
			this.healPercentage = healPercentage;
			this.immuneHits = immuneHits;
			this.immuneHitsRounds = immuneHitsRounds;
		}

		private void EnsureInitialized(ActorModel actor)
		{
			if (!actor.UndyingState.BattleStartInitialized && actor.UndyingState.MaxTotalGrants == 0)
			{
				actor.UndyingState.TurnsUntilNextGrant = firstGrantRound;
				actor.UndyingState.MaxTotalGrants = maxTotalGrants;
			}
		}

		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			CombatModel combatModel = actor?.manager?.CombatModel;
			if (combatModel == null)
			{
				return ActionListClearFlag.Keep;
			}
			EnsureInitialized(actor);
			if (!actor.UndyingState.BattleStartInitialized && combatModel.TurnManager.TurnCount == 0)
			{
				actor.UndyingState.BattleStartInitialized = true;
				actor.UndyingState.TurnsUntilNextGrant--;
				TryGrantUndying(actor, addedActions);
			}
			if (!(action is ChangeTurnAction))
			{
				if (!(action is PostChangeTurnAction))
				{
					if (!(action is DamageAction damageAction))
					{
						if (action is PostMoveSuccessAction postMoveSuccessAction && postMoveSuccessAction.Actor == actor && actor.UndyingState.IsUndying && actor.UndyingState.ImmuneHitsRemaining > 0)
						{
							ClearUndying(actor);
						}
					}
					else if (damageAction.TargetActor == actor && actor.ShieldHitPoints <= 0 && actor.UndyingState.IsUndying)
					{
						if (actor.UndyingState.ImmuneHitsRemaining > 0)
						{
							if (damageAction.DamagerActor != null && damageAction.DamagerActor.DeathsBlockSecondChance)
							{
								damageAction.DamagerActor.NotifyChange("DeathsDoorBlockSecondChance");
							}
							else if (damageAction.BaseDamage > 0)
							{
								damageAction.ZeroDamage();
								actor.UndyingState.ImmuneHitsRemaining--;
							}
						}
						else if (!actor.OnRedHealthBar && damageAction.BaseDamage + damageAction.AdditionalCriticalDamage + damageAction.ModifyDamage >= actor.Hitpoints)
						{
							if (damageAction.DamagerActor != null && damageAction.DamagerActor.DeathsBlockSecondChance)
							{
								damageAction.DamagerActor.NotifyChange("DeathsDoorBlockSecondChance");
							}
							else
							{
								int num = actor.Hitpoints - 1;
								if (num < 0)
								{
									num = 0;
								}
								damageAction.UpBaseDamage(num);
								damageAction.UpAdditionalCriticalDamage(0);
								damageAction.ModifyDamage = 0;
								TriggerUndying(actor, addedActions);
							}
						}
					}
				}
				else if (combatModel.TurnManager.ActiveFaction == Faction.Survivor)
				{
					actor.NotifyChange("actorUndyingUpdateEvent");
				}
			}
			else if (combatModel.TurnManager.ActiveFaction == Faction.Survivor)
			{
				actor.UndyingState.BattleStartInitialized = true;
				if (!actor.UndyingState.IsUndying && actor.UndyingState.TurnsUntilNextGrant > 0)
				{
					actor.UndyingState.TurnsUntilNextGrant--;
				}
				if (actor.UndyingState.ImmuneHitsRemaining > 0 && immuneHitsRounds > 0)
				{
					actor.UndyingState.ImmuneRoundsRemaining--;
					if (actor.UndyingState.ImmuneRoundsRemaining <= 0)
					{
						ClearUndying(actor);
					}
				}
				TryGrantUndying(actor, addedActions);
			}
			return ActionListClearFlag.Keep;
		}

		private void TryGrantUndying(ActorModel actor, List<ModelAction> addedActions)
		{
			if (actor.UndyingState.TurnsUntilNextGrant <= 0 && !actor.UndyingState.IsUndying && !actor.OnRedHealthBar && actor.UndyingState.TotalGrantedCount < maxTotalGrants)
			{
				actor.UndyingState.TotalGrantedCount++;
				addedActions.Add(new UndyingAction(actor, actor));
				actor.NotifyChange("AbilityVisited", new object[2] { "Undying", false });
			}
		}

		private void TriggerUndying(ActorModel actor, List<ModelAction> addedActions)
		{
			actor.UndyingState.ImmuneHitsRemaining = immuneHits;
			actor.UndyingState.ImmuneRoundsRemaining = immuneHitsRounds;
			int num = (int)(actor.MaxHitPoints * healPercentage) - 1;
			if (num > 0)
			{
				addedActions.Add(new HealAction(actor, actor, num));
			}
			actor.NotifyChange("AbilityVisited", new object[2] { "Undying", false });
		}

		private void ClearUndying(ActorModel actor)
		{
			actor.UndyingState.IsUndying = false;
			actor.UndyingState.ImmuneHitsRemaining = 0;
			actor.UndyingState.ImmuneRoundsRemaining = 0;
			actor.UndyingState.TurnsUntilNextGrant = regrantIntervalRounds;
			actor.NotifyChange("actorUndyingUpdateEvent");
		}
	}
}
