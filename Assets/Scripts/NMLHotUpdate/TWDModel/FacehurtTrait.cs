using System.Collections.Generic;

namespace TWDModel
{
	public class FacehurtTrait : ActionModifier
	{
		private int range;

		private FixedPoint rootChance;

		public FacehurtTrait()
		{
		}

		public FacehurtTrait(int range, FixedPoint rootChance)
		{
			this.range = range;
			this.rootChance = rootChance;
		}

		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			if (action is PostDamageAction postDamageAction)
			{
				if (postDamageAction.DamagerActor != actor || postDamageAction.TargetActor.IsDead || postDamageAction.DamageAction.SourceSupport != null || actor.IsStruggling)
				{
					return ActionListClearFlag.Keep;
				}
				DamageAction damageAction = postDamageAction.DamageAction;
				if (damageAction.DealDamagePostAbility && damageAction.FinalDamage >= damageAction.TargetActor.Hitpoints && !damageAction.SavedFromDeath)
				{
					return ActionListClearFlag.Keep;
				}
				if (CombatHelpers.ShouldActorExplode(postDamageAction.DamagerActor, postDamageAction.TargetActor, postDamageAction.DamageAction.DamageType, postDamageAction.DamageAction.FinalDamage))
				{
					return ActionListClearFlag.Keep;
				}
				CombatModel combatModel = actor.manager.CombatModel;
				if (combatModel == null)
				{
					return ActionListClearFlag.Keep;
				}
				if (!CombatHelpers.IsWithinRange(combatModel, range, postDamageAction.DamagerActor.GridCoordinate, postDamageAction.TargetActor.GridCoordinate))
				{
					return ActionListClearFlag.Keep;
				}
				FixedPoint value = 0.0;
				base.manager.CombatModel.AbilityManager.VisitParameter("ExtendProbability", ref value, actor);
				if (base.manager.Player.RollDice(RollDiceType.Root, rootChance, value) != PlayerRandomChanceResult.Failed)
				{
					addedActions.Add(new RootAction(postDamageAction.DamagerActor, postDamageAction.TargetActor, 1, ignoreSourceBeingDead: false, null, () => damageAction.FinalDamage));
				}
			}
			return ActionListClearFlag.Keep;
		}
	}
}
