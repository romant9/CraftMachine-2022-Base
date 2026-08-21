using System.Collections.Generic;

namespace TWDModel
{
	public class PoisonTrait : ActionModifier
	{
		private FixedPoint Percentage;

		private int Turns;

		private FixedPoint AttackerDamagePercentage;

		private int MaxLayerCount;

		public PoisonTrait(FixedPoint percentage, int turns, FixedPoint attackerDamagePercentage, int maxLayerCount)
		{
			Percentage = percentage;
			Turns = turns;
			AttackerDamagePercentage = attackerDamagePercentage;
			MaxLayerCount = maxLayerCount;
		}

		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			if (action is PostDamageAction postDamageAction && postDamageAction.DamagerActor == actor && postDamageAction.TargetActor != null && !postDamageAction.TargetActor.IsDead && postDamageAction.DamageAction.SourceSupport == null && postDamageAction.TargetActor.Faction != Faction.Environmental && postDamageAction.DamagerActor.Faction != Faction.Environmental && postDamageAction.DamagerActor.Faction != postDamageAction.TargetActor.Faction && postDamageAction.DamageAction.DamageType != DamageType.Poison && !(postDamageAction.DamageAction is DamageConsumableAction))
			{
				FixedPoint value = 0.0;
				base.manager.CombatModel.AbilityManager.VisitParameter("ExtendProbability", ref value, actor);
				if (base.manager.Player.RollDice(RollDiceType.Poison, Percentage, value) != PlayerRandomChanceResult.Failed)
				{
					CreatePoisonRelation(postDamageAction.DamagerActor, postDamageAction.TargetActor);
				}
				if (postDamageAction.IsMainTarget && postDamageAction.TargetActor.GetBePoisonedLayerList().Count > 0)
				{
					addedActions.Add(new PestilenceAction(postDamageAction.DamagerActor, postDamageAction.TargetActor, Turns, postDamageAction.IsMainTarget));
				}
			}
			return ActionListClearFlag.Keep;
		}

		private void CreatePoisonRelation(ActorModel source, ActorModel target)
		{
			if (!EquipmentPassivePreventControlTrait.TryResistEffect(target, "Poison", RollDiceType.Poison))
			{
				CombatModel combatModel = source.manager.CombatModel;
				PoisonRelationsManager poisonRelationsManager = combatModel.GetModel<PoisonRelationsManager>();
				if (poisonRelationsManager == null)
				{
					poisonRelationsManager = new PoisonRelationsManager();
					poisonRelationsManager.SetManager(source.manager);
					combatModel.AddModel(poisonRelationsManager);
				}
				PoisonRelation newRelation = new PoisonRelation(source, target, source.Faction, combatModel.TurnManager.TurnCount + Turns, AttackerDamagePercentage, MaxLayerCount, Turns);
				poisonRelationsManager.AddRelation(newRelation);
			}
		}
	}
}
