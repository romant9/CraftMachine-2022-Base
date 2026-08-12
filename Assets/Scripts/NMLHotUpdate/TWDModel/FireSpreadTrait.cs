using System.Collections.Generic;

namespace TWDModel
{
	public class FireSpreadTrait : ActionModifier
	{
		private readonly FixedPoint burnChance;

		private readonly FixedPoint spreadChance;

		private readonly int spreadRange;

		private readonly int spreadMaxTargets;

		private readonly int burnTurns;

		private readonly int spreadBurnTurns;

		public FireSpreadTrait(FixedPoint burnChance, FixedPoint spreadChance, int spreadRange, int spreadMaxTargets, int burnTurns, int spreadBurnTurns)
		{
			this.burnChance = burnChance;
			this.spreadChance = spreadChance;
			this.spreadRange = spreadRange;
			this.spreadMaxTargets = spreadMaxTargets;
			this.burnTurns = burnTurns;
			this.spreadBurnTurns = spreadBurnTurns;
		}

		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			if (action is PostDamageAction postDamageAction)
			{
				if (actor.IsDead)
				{
					return ActionListClearFlag.Keep;
				}
				DamageAction damageAction = postDamageAction.DamageAction;
				if (postDamageAction.DamagerActor != actor || postDamageAction.TargetActor == null || postDamageAction.TargetActor.IsDead || postDamageAction.TargetActor.IsEnvironmental || damageAction == null || damageAction.IsPushDamage || damageAction.SourceSupport != null || damageAction is DamageConsumableAction)
				{
					return ActionListClearFlag.Keep;
				}
				if (damageAction.FinalDamage <= 0)
				{
					return ActionListClearFlag.Keep;
				}
				if (actor.HasAnyLevelTrait("DebuffMarkEnemy"))
				{
					return ActionListClearFlag.Keep;
				}
				ActorModel targetActor = postDamageAction.TargetActor;
				CombatModel combatModel = actor.manager.CombatModel;
				if (combatModel == null)
				{
					return ActionListClearFlag.Keep;
				}
				FixedPoint value = 0.0;
				combatModel.AbilityManager.VisitParameter("ExtendProbability", ref value, actor);
				FixedPoint value2 = burnChance;
				if (targetActor.AttributeModel?.GetAttributeModelValue("burn_be_ratio") != 0L)
				{
					FixedPoint value3 = burnChance;
					FixedPoint value4 = 1L;
					FixedPoint? obj = targetActor.AttributeModel?.GetAttributeModelValue("burn_be_ratio");
					FixedPoint? fixedPoint = value4 + obj;
					value2 = (value3 * fixedPoint).Value;
				}
				if (actor.manager.Player.RollDice(RollDiceType.ActivateChance, value2, value) != PlayerRandomChanceResult.Failed)
				{
					addedActions.Add(new BurningOutAction(actor, targetActor, onRedHealthBar: false, null, () => damageAction.FinalDamage, burnTurns));
				}
				if (actor.manager.Player.RollDice(RollDiceType.ActivateChance, spreadChance, value) != PlayerRandomChanceResult.Failed)
				{
					SpreadFire(actor, targetActor, combatModel, addedActions);
				}
			}
			return ActionListClearFlag.Keep;
		}

		private void SpreadFire(ActorModel actor, ActorModel mainTarget, CombatModel combatModel, List<ModelAction> addedActions)
		{
			List<ActorModel> allActors = combatModel.GetAllActors();
			if (allActors == null || allActors.Count == 0)
			{
				return;
			}
			int num = 0;
			for (int i = 0; i < allActors.Count; i++)
			{
				if (num >= spreadMaxTargets)
				{
					break;
				}
				ActorModel actorModel = allActors[i];
				if (actorModel != null && actorModel != mainTarget && !actorModel.IsDead && !actorModel.IsEnvironmental && actorModel.IsEnemy(actor) && !actorModel.IsBurning && CombatHelpers.IsWithinRange(combatModel, spreadRange, mainTarget.GridCoordinate, actorModel.GridCoordinate))
				{
					addedActions.Add(new BurningOutAction(actor, actorModel, onRedHealthBar: false, null, null, spreadBurnTurns));
					num++;
				}
			}
		}
	}
}
