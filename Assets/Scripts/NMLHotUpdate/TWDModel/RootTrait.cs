using System.Collections.Generic;

namespace TWDModel
{
	public class RootTrait : ActionModifier
	{
		private int turns;

		private int chancePercentage;

		public RootTrait()
		{
		}

		public RootTrait(int turns, int chancePercentage)
		{
			this.turns = turns;
			this.chancePercentage = chancePercentage;
		}

		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			PostDamageAction postDamageAction = action as PostDamageAction;
			if (postDamageAction != null && postDamageAction.DamagerActor == actor && !postDamageAction.TargetActor.IsDead && !postDamageAction.DamageAction.BodyShot && postDamageAction.DamageAction.SourceSupport == null && postDamageAction.DamagerActor.Faction != Faction.Environmental && base.manager.Player.RollDice(RollDiceType.Root, (FixedPoint)chancePercentage / (FixedPoint)100.0, 0.0) != PlayerRandomChanceResult.Failed)
			{
				addedActions.Add(new RootAction(postDamageAction.DamagerActor, postDamageAction.TargetActor, turns, ignoreSourceBeingDead: false, null, () => postDamageAction.DamageAction.FinalDamage));
			}
			return ActionListClearFlag.Keep;
		}
	}
}
