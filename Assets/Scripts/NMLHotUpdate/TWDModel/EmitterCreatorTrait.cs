using System.Collections.Generic;

namespace TWDModel
{
	public class EmitterCreatorTrait : ActionModifier
	{
		private ActorModel attackTarget;

		private bool isChargeAttack;

		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			if (action is PostDamageAction postDamageAction)
			{
				if (postDamageAction.DamagerActor == actor && postDamageAction.TargetActor == attackTarget && !attackTarget.IsEnvironmental)
				{
					DamageAction damageAction = postDamageAction.DamageAction;
					if (isChargeAttack || attackTarget.IsDead || CombatHelpers.ShouldActorExplode(postDamageAction.DamagerActor, postDamageAction.TargetActor, damageAction.DamageType, damageAction.FinalDamage) || (damageAction.DealDamagePostAbility && damageAction.FinalDamage >= damageAction.TargetActor.Hitpoints && !damageAction.SavedFromDeath))
					{
						CreateEmitterArea(actor, attackTarget);
					}
					attackTarget = null;
				}
			}
			else if (action is AbilityAction abilityAction && abilityAction.Actor == actor && !abilityAction.Ability.IsConsumableAbility)
			{
				attackTarget = base.manager.CombatModel.Occupiers[abilityAction.TargetCell];
				isChargeAttack = abilityAction.Ability.IsChargeAttack;
			}
			return ActionListClearFlag.Keep;
		}

		private void CreateEmitterArea(ActorModel actor, ActorModel targetActor)
		{
			CombatModel combatModel = actor.manager.CombatModel;
			AbilityManagerModel abilityManager = combatModel.AbilityManager;
			EmitAreasManager emitAreasManager = combatModel.GetModel<EmitAreasManager>();
			if (emitAreasManager == null)
			{
				FixedPoint value = 0.0;
				abilityManager.VisitParameter("LeaderBuffEmitterMaxMergedRadius", ref value, actor);
				emitAreasManager = new EmitAreasManager(value);
				emitAreasManager.SetManager(actor.manager);
				combatModel.AddModel(emitAreasManager);
			}
			FixedPoint value2 = 0.0;
			FixedPoint value3 = 0L;
			FixedPoint value4 = 1.0;
			abilityManager.VisitParameter("LeaderBuffEmitterRadius", ref value2, actor);
			abilityManager.VisitParameter("LeaderBuffEmitterDuration", ref value3, actor);
			abilityManager.VisitParameter("LeaderBuffEmitterDamageMultiplier", ref value4, actor);
			EmitArea emitArea = new EmitArea(value2, targetActor.GridCoordinate, actor.Faction, combatModel.TurnManager.TurnCount + (int)value3, value4);
			emitArea.SetManager(actor.manager);
			emitAreasManager.AddArea(emitArea);
		}
	}
}
