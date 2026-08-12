using System.Collections.Generic;

namespace TWDModel
{
	public class SufferCreatorTrait : ActionModifier
	{
		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			if (action is AbilityAction abilityAction && abilityAction.Actor == actor && actor.IsMeleeClass && abilityAction.Ability.IsChargeAttack && !abilityAction.Ability.IsConsumableAbility)
			{
				CreateEmitterArea(actor, abilityAction.TargetCell);
			}
			return ActionListClearFlag.Keep;
		}

		private void CreateEmitterArea(ActorModel actor, GridCoordinate target)
		{
			CombatModel combatModel = actor.manager.CombatModel;
			AbilityManagerModel abilityManager = combatModel.AbilityManager;
			SufferAreasManager sufferAreasManager = combatModel.GetModel<SufferAreasManager>();
			if (sufferAreasManager == null)
			{
				FixedPoint value = 0L;
				string paramName = (LeaderHasTheTrait(actor) ? "LeaderBuffMadeToSufferMaxAreasLeader" : "LeaderBuffMadeToSufferMaxAreasNonLeader");
				abilityManager.VisitParameter(paramName, ref value, actor);
				sufferAreasManager = new SufferAreasManager((int)value);
				sufferAreasManager.SetManager(actor.manager);
				combatModel.AddModel(sufferAreasManager);
			}
			FixedPoint value2 = 0.0;
			FixedPoint value3 = 0.0;
			abilityManager.VisitParameter("LeaderBuffMadeToSufferRadius", ref value2, actor);
			abilityManager.VisitParameter("LeaderBuffMadeToSufferMaxAreasDuration", ref value3, actor);
			SufferArea sufferArea = new SufferArea(value2, target, actor.Faction, combatModel.TurnManager.TurnCount + (int)value3, actor);
			sufferArea.SetManager(actor.manager);
			sufferAreasManager.AddArea(sufferArea);
		}

		private bool LeaderHasTheTrait(ActorModel actor)
		{
			SurvivorModel survivorModel = (SurvivorModel)((actor.Faction == Faction.Raider) ? base.manager.CombatModel.Raiders[0] : base.manager.CombatModel.Survivors[0]);
			if (survivorModel.IsLeader)
			{
				return survivorModel.HasAnyLevelTrait("LeaderBuffMadeToSuffer");
			}
			return false;
		}
	}
}
