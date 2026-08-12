using System.Collections.Generic;

namespace TWDModel
{
	public class CrippleTrait : ActionModifier
	{
		private readonly int turns;

		private FixedPoint chancePercentage;

		private readonly bool workOnlyOnBodyShots;

		public CrippleTrait()
		{
		}

		public CrippleTrait(int turns, FixedPoint chancePercentage, bool workOnlyOnBodyShots)
		{
			this.turns = turns;
			this.chancePercentage = chancePercentage;
			this.workOnlyOnBodyShots = workOnlyOnBodyShots;
		}

		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			PostDamageAction postDamageAction = action as PostDamageAction;
			if (postDamageAction != null)
			{
				if (CombatHelpers.ShouldActorExplode(postDamageAction.DamagerActor, postDamageAction.TargetActor, postDamageAction.DamageAction.DamageType, postDamageAction.DamageAction.FinalDamage))
				{
					return ActionListClearFlag.Keep;
				}
				if (postDamageAction.DamagerActor != actor || postDamageAction.TargetActor.IsDead || postDamageAction.TargetActor.IsStruggling)
				{
					return ActionListClearFlag.Keep;
				}
				if (workOnlyOnBodyShots && !postDamageAction.DamageAction.BodyShot)
				{
					return ActionListClearFlag.Keep;
				}
				FixedPoint value = 0.0;
				FixedPoint fixedPoint = 0.0;
				FixedPoint value2 = 0.0;
				base.manager.CombatModel.AbilityManager.VisitParameter("ExtendProbability", ref value2, actor);
				base.manager.CombatModel.AbilityManager.VisitParameter("LeaderBuffGoodEnoughCrippleChance", ref value, postDamageAction.DamagerActor);
				value /= (FixedPoint)100.0;
				fixedPoint = value + chancePercentage;
				if (base.manager.Player.RollDice(RollDiceType.Cripple, fixedPoint, value2) != PlayerRandomChanceResult.Failed)
				{
					addedActions.Add(new CrippleAction(postDamageAction.DamagerActor, postDamageAction.TargetActor, turns, ignoreSourceBeingDead: false, null, () => postDamageAction.DamageAction.FinalDamage));
				}
				return ActionListClearFlag.Keep;
			}
			return ActionListClearFlag.Keep;
		}
	}
}
