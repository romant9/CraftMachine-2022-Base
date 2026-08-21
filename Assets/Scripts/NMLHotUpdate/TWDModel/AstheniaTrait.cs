using System.Collections.Generic;

namespace TWDModel
{
	public class AstheniaTrait : ActionModifier
	{
		private FixedPoint AddAstheniaPercentage;

		private FixedPoint MakeEnemyDecreaseAttackPercentage;

		private FixedPoint MakeEnemyDecreaseDecreaseDamagePercentage;

		private int Turns;

		public AstheniaTrait(FixedPoint addAstheniaPercentage, FixedPoint makeEnemyDecreaseAttackPercentage, FixedPoint makeEnemyDecreaseDecreaseDamagePercentage, int turns)
		{
			AddAstheniaPercentage = addAstheniaPercentage;
			MakeEnemyDecreaseAttackPercentage = makeEnemyDecreaseAttackPercentage;
			MakeEnemyDecreaseDecreaseDamagePercentage = makeEnemyDecreaseDecreaseDamagePercentage;
			Turns = turns;
		}

		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			if (action is AbilityAction { Actor: not null } abilityAction && actor == abilityAction.Actor && !abilityAction.Ability.IsConsumableAbility)
			{
				if (!actor.manager.CombatModel.AbilityManager.GetListOfActorsToBeTargetted(abilityAction.Ability, abilityAction.Actor, abilityAction.Actor.GridCoordinate, abilityAction.TargetCell).Exists((ActorModel x) => x.GetAstheniaLeftTurns() > 0))
				{
					return ActionListClearFlag.Keep;
				}
				actor.ThisAbilityActionAttackUseAsthenia = true;
			}
			if (action is PostAbilityExecuteAction postAbilityExecuteAction && actor == postAbilityExecuteAction.DamagerActor)
			{
				actor.ThisAbilityActionAttackUseAsthenia = false;
			}
			if (action is PostDamageAction { DamagerActor: not null } postDamageAction && actor == postDamageAction.DamagerActor && postDamageAction.TargetActor.Faction != Faction.Environmental && postDamageAction.DamagerActor.Faction != Faction.Environmental && !postDamageAction.TargetActor.IsDead && postDamageAction.DamageAction.SourceSupport == null && !(postDamageAction.DamageAction is DamageConsumableAction))
			{
				FixedPoint fixedPoint = 0.0;
				if (ResistNegativeEffectsTrait.TryResist(postDamageAction.TargetActor, "Asthenia"))
				{
					return ActionListClearFlag.Keep;
				}
				fixedPoint = 0.0;
				base.manager.CombatModel.AbilityManager.VisitParameter("ExtendProbability", ref fixedPoint, actor);
				if (base.manager.Player.RollDice(RollDiceType.Asthenia, AddAstheniaPercentage, fixedPoint) == PlayerRandomChanceResult.Failed)
				{
					return ActionListClearFlag.Keep;
				}
				if (EquipmentPassivePreventControlTrait.TryResistEffect(postDamageAction.TargetActor, "Asthenia", RollDiceType.Asthenia))
				{
					return ActionListClearFlag.Keep;
				}
				CreateRelation(actor, postDamageAction.TargetActor);
			}
			return ActionListClearFlag.Keep;
		}

		private void CreateRelation(ActorModel source, ActorModel target)
		{
			CombatModel combatModel = source.manager.CombatModel;
			AstheniaRelationsManager astheniaRelationsManager = combatModel.GetModel<AstheniaRelationsManager>();
			if (astheniaRelationsManager == null)
			{
				astheniaRelationsManager = new AstheniaRelationsManager();
				astheniaRelationsManager.SetManager(source.manager);
				combatModel.AddModel(astheniaRelationsManager);
			}
			AstheniaRelation newRelation = new AstheniaRelation(source, target, source.Faction, combatModel.TurnManager.TurnCount + Turns, Turns, MakeEnemyDecreaseAttackPercentage, MakeEnemyDecreaseDecreaseDamagePercentage);
			astheniaRelationsManager.AddRelation(newRelation);
		}
	}
}
