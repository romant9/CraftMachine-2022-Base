using System.Collections.Generic;

namespace TWDModel
{
	public class NegativeFatalTrait : ActionModifier
	{
		private List<FixedPoint> turns;

		private List<string> effectIndex;

		public NegativeFatalTrait()
		{
		}

		public NegativeFatalTrait(List<FixedPoint> turns, List<string> effectIndex)
		{
			this.turns = turns;
			this.effectIndex = effectIndex;
		}

		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			if (action is AbilityAction { Actor: not null } abilityAction)
			{
				IChallengeDebuffProvider challengeDebuffProvider = MapMissionDebuffHelper.CanUseDebuffMission(base.manager);
				if (challengeDebuffProvider != null)
				{
					int chance = (int)ChallengeDebufHelps.GetDebufTotalFirstParam(challengeDebuffProvider.GetChallengeDebuffs(), ChallengeDebuffType.DebuffInstanKill);
					if (base.manager.Player.RollDice(RollDiceType.AvoidNegativeFatal, chance) == PlayerRandomChanceResult.Success)
					{
						return ActionListClearFlag.Keep;
					}
				}
				EquipmentItemModel weaponEquipment = abilityAction.Actor.GetWeaponEquipment();
				CombatModel combatModel = actor.manager.CombatModel;
				_ = combatModel.TurnManager.ActiveActor;
				ActorModel actor2 = abilityAction.Actor;
				if (!actor2.HasTraitsThatContains("NegativeFatal"))
				{
					return ActionListClearFlag.Keep;
				}
				if (abilityAction.Actor == actor && combatModel != null && combatModel.TurnManager.ActiveActor == actor)
				{
					List<ActorModel> listOfActorsToBeTargetted = combatModel.AbilityManager.GetListOfActorsToBeTargetted(weaponEquipment.Ability, actor2, actor2.GridCoordinate, abilityAction.TargetCell);
					bool flag = false;
					for (int i = 0; i < listOfActorsToBeTargetted.Count; i++)
					{
						int negativeEffCount = listOfActorsToBeTargetted[i].GetNegativeEffCount(effectIndex);
						FixedPoint fixedPoint = 0L;
						if (turns != null && turns.Count > 0 && negativeEffCount > 0)
						{
							fixedPoint = ((turns.Count >= negativeEffCount) ? turns[negativeEffCount - 1] : turns[turns.Count - 1]);
							if (base.manager.Player.RollDice(RollDiceType.Root, fixedPoint, 0.0) != PlayerRandomChanceResult.Failed)
							{
								listOfActorsToBeTargetted[i].Iskill = true;
								flag = true;
							}
						}
					}
					if (flag)
					{
						actor2.AddTemporaryTrait("NegativeFlagFatalFlag", default(FixedPoint), null, 0L);
						actor2.NotifyChange("AbilityVisited", new object[2] { "NegativeFatal", false });
					}
				}
			}
			if (action is PostAbilityExecuteAction { DamagerActor: not null } postAbilityExecuteAction)
			{
				IChallengeDebuffProvider challengeDebuffProvider2 = MapMissionDebuffHelper.CanUseDebuffMission(base.manager);
				if (challengeDebuffProvider2 != null)
				{
					int chance2 = (int)ChallengeDebufHelps.GetDebufTotalFirstParam(challengeDebuffProvider2.GetChallengeDebuffs(), ChallengeDebuffType.DebuffInstanKill);
					if (base.manager.Player.RollDice(RollDiceType.AvoidNegativeFatal, chance2) == PlayerRandomChanceResult.Success)
					{
						return ActionListClearFlag.Keep;
					}
				}
				CombatModel combatModel2 = actor.manager.CombatModel;
				if (postAbilityExecuteAction.DamagerActor == actor && combatModel2 != null)
				{
					if (postAbilityExecuteAction.DamagerActor.HasTrait("NegativeFlagFatalFlag"))
					{
						postAbilityExecuteAction.DamagerActor.RemoveTrait("NegativeFlagFatalFlag");
					}
					if (postAbilityExecuteAction.DamagerActor == actor && combatModel2 != null)
					{
						List<ActorModel> enemyFactionsActors = combatModel2.GetEnemyFactionsActors(actor.Faction);
						for (int j = 0; j < enemyFactionsActors.Count; j++)
						{
							enemyFactionsActors[j].Iskill = false;
						}
					}
				}
			}
			return ActionListClearFlag.Keep;
		}
	}
}
