using System.Collections.Generic;

namespace TWDModel
{
	public class DisorientTrait : ActionModifier
	{
		private int turns;

		private int chancePercentage;

		public DisorientTrait(int turns, int chancePercentage)
		{
			this.turns = turns;
			this.chancePercentage = chancePercentage;
		}

		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			PostDamageAction postDamageAction = action as PostDamageAction;
			if (postDamageAction != null)
			{
				if (actor.Faction != base.manager.CombatModel.TurnManager.ActiveFaction)
				{
					return ActionListClearFlag.Keep;
				}
				if (postDamageAction.DamagerActor == actor && !postDamageAction.TargetActor.IsDead && postDamageAction.DamageAction.SourceSupport == null && postDamageAction.DamageAction.TargetActor.IsWalker)
				{
					FixedPoint value = 0.0;
					base.manager.CombatModel.AbilityManager.VisitParameter("ExtendProbability", ref value, actor);
					if (base.manager.Player.RollDice(RollDiceType.Disorient, (FixedPoint)chancePercentage / (FixedPoint)100.0, value) != PlayerRandomChanceResult.Failed)
					{
						int num = turns;
						addedActions.Add(new DisorientAction(postDamageAction.DamagerActor, postDamageAction.TargetActor, num, ignoreSourceBeingDead: false, null, () => postDamageAction.DamageAction.FinalDamage));
					}
				}
			}
			return ActionListClearFlag.Keep;
		}
	}
}
