using System.Collections.Generic;

namespace TWDModel
{
	public class RandomStatusTrait : ActionModifier
	{
		private int NumberOfAttacks;

		private FixedPoint ProbabilityOfTriggering;

		private List<KeyValuePair<string, FixedPoint>> TriggerWeightList;

		public RandomStatusTrait(int numberOfAttacks, FixedPoint probabilityOfTriggering, List<KeyValuePair<string, FixedPoint>> triggerWeightList)
		{
			NumberOfAttacks = numberOfAttacks;
			ProbabilityOfTriggering = probabilityOfTriggering;
			TriggerWeightList = triggerWeightList;
		}

		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			if (action is AbilityAction abilityAction)
			{
				ActorModel actorModel = base.manager.CombatModel.Occupiers[abilityAction.TargetCell];
				if (actorModel != null && abilityAction.Actor != null && abilityAction.Actor == actor && abilityAction.Actor.Faction != Faction.Environmental && actorModel.Faction != Faction.Environmental && !actorModel.IsDead)
				{
					abilityAction.Actor.IncreaseAttackCount();
					if (abilityAction.Actor.RandomStatusNumberOfAttack > NumberOfAttacks)
					{
						FixedPoint value = 0.0;
						base.manager.CombatModel.AbilityManager.VisitParameter("ExtendProbability", ref value, actor);
						if (base.manager.Player.RollDice(RollDiceType.RandomStatus, ProbabilityOfTriggering, value) == PlayerRandomChanceResult.Failed)
						{
							return ActionListClearFlag.Keep;
						}
						List<KeyValuePair<string, FixedPoint>> list = base.manager.Player.PlayerRandom.WeightedRandomList(TriggerWeightList, 1, (KeyValuePair<string, FixedPoint> x) => x.Value, isRepeat: false);
						if (list.Count != 1)
						{
							return ActionListClearFlag.Keep;
						}
						abilityAction.Actor.RandomStatusTraitIdentifier = list[0].Key;
						abilityAction.Actor.AddTemporaryTrait(abilityAction.Actor.RandomStatusTraitIdentifier, default(FixedPoint), null, 0L);
						abilityAction.Actor.ClearRandomStatusNumberOfAttacks();
					}
				}
			}
			if (action is PostDamageAction { DamagerActor: not null } postDamageAction && actor == postDamageAction.DamagerActor && !string.IsNullOrEmpty(postDamageAction.DamagerActor.RandomStatusTraitIdentifier))
			{
				postDamageAction.DamagerActor.NotifyChange("AbilityVisited", new object[2] { "RandomStatus", false });
			}
			if (action is AbilityBeforeRemoveActiveTraitAction abilityBeforeRemoveActiveTraitAction && abilityBeforeRemoveActiveTraitAction?.AbilityAction?.Actor != null && actor == abilityBeforeRemoveActiveTraitAction?.AbilityAction?.Actor && abilityBeforeRemoveActiveTraitAction != null && abilityBeforeRemoveActiveTraitAction.AbilityAction?.Actor?.RandomStatusTraitIdentifier != null)
			{
				abilityBeforeRemoveActiveTraitAction.AbilityAction.Actor.RemoveTrait(abilityBeforeRemoveActiveTraitAction.AbilityAction.Actor.RandomStatusTraitIdentifier);
				abilityBeforeRemoveActiveTraitAction.AbilityAction.Actor.RandomStatusTraitIdentifier = null;
			}
			return ActionListClearFlag.Keep;
		}
	}
}
