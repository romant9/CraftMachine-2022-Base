using System.Collections.Generic;

namespace TWDModel
{
	public class UnleashedCreatorTrait : ActionModifier
	{
		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			if (action is AbilityAction abilityAction && abilityAction.Actor == actor && abilityAction.Ability.IsChargeAttack && !abilityAction.Ability.IsConsumableAbility && canCreatePitfallArea(abilityAction, actor))
			{
				CreatePitfallArea(actor, abilityAction);
			}
			return ActionListClearFlag.Keep;
		}

		private void CreatePitfallArea(ActorModel actor, AbilityAction abilityAction)
		{
			CombatModel combatModel = actor.manager.CombatModel;
			AbilityManagerModel abilityManager = combatModel.AbilityManager;
			PitfallAreasManager pitfallAreasManager = combatModel.GetModel<PitfallAreasManager>();
			if (pitfallAreasManager == null)
			{
				FixedPoint value = 0L;
				string paramName = "LeaderBuffUnleashedMaxAreas";
				ActorModel actorModel = null;
				actorModel = ((!LeaderHasTheTrait(actor)) ? actor : getLeader(actor));
				abilityManager.VisitParameter(paramName, ref value, actorModel);
				pitfallAreasManager = new PitfallAreasManager((int)value);
				pitfallAreasManager.SetManager(actor.manager);
				combatModel.AddModel(pitfallAreasManager);
			}
			FixedPoint value2 = 0.0;
			FixedPoint value3 = 0.0;
			FixedPoint value4 = 0.0;
			string paramName2 = (isRangedClass(actor) ? "LeaderBuffUnleashedFighterRemoteAreaGridLength" : "LeaderBuffUnleashedFighterAreaGridLength");
			FixedPoint radius = (isRangedClass(actor) ? ((FixedPoint)1.0) : ((FixedPoint)2.0));
			abilityManager.VisitParameter(paramName2, ref value2, actor);
			abilityManager.VisitParameter("LeaderBuffUnleashedFighterAreasDurationLeader", ref value3, actor);
			string paramName3 = (LeaderHasTheTrait(actor) ? "LeaderBuffUnleashedFighterCoolingPeriodShare" : "LeaderBuffUnleashedFighterCoolingPeriodLeader");
			abilityManager.VisitParameter(paramName3, ref value4, actor);
			pitfallAreasManager.SetActorCooldownUntilTurn(actor.Faction.ToString() + ":" + actor.Name, combatModel.TurnManager.TurnCount + (int)value4);
			PitfallArea pitfallArea = new PitfallArea(value2, value2, radius, abilityAction.TargetCell, actor.Faction, combatModel.TurnManager.TurnCount + (int)value3, actor);
			pitfallArea.SetManager(actor.manager);
			pitfallAreasManager.AddArea(pitfallArea);
		}

		private bool canCreatePitfallArea(AbilityAction abilityAction, ActorModel actor)
		{
			PitfallAreasManager model = actor.manager.CombatModel.GetModel<PitfallAreasManager>();
			if (model == null)
			{
				return true;
			}
			int actorCooldownUntilTurn = model.GetActorCooldownUntilTurn(actor.Faction.ToString() + ":" + actor.Name);
			return actor.manager.CombatModel.TurnManager.TurnCount > actorCooldownUntilTurn;
		}

		private bool isRangedClass(ActorModel actor)
		{
			return actor.IsRangedClass;
		}

		private bool LeaderHasTheTrait(ActorModel actor)
		{
			SurvivorModel survivorModel = (SurvivorModel)((actor.Faction == Faction.Raider) ? base.manager.CombatModel.Raiders[0] : base.manager.CombatModel.Survivors[0]);
			if (survivorModel.IsLeader)
			{
				return survivorModel.HasAnyLevelTrait("LeaderBuffUnleashedFighter");
			}
			return false;
		}

		private ActorModel getLeader(ActorModel actor)
		{
			return (SurvivorModel)((actor.Faction == Faction.Raider) ? base.manager.CombatModel.Raiders[0] : base.manager.CombatModel.Survivors[0]);
		}
	}
}
