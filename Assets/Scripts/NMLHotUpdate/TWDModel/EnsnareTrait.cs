using System.Collections.Generic;

namespace TWDModel
{
	public class EnsnareTrait : ActionModifier
	{
		private int turns;

		private FixedPoint meleePercentage;

		private FixedPoint rangedPercentage;

		public EnsnareTrait()
		{
		}

		public EnsnareTrait(int turns, FixedPoint meleePercentage, FixedPoint rangedPercentage)
		{
			this.turns = turns;
			this.meleePercentage = meleePercentage;
			this.rangedPercentage = rangedPercentage;
		}

		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			if (action is PostDamageAction postDamageAction && postDamageAction.DamagerActor == actor && !postDamageAction.TargetActor.IsDead && !postDamageAction.DamageAction.BodyShot && !actor.HasAnyLevelTrait("DebuffMarkEnemy") && postDamageAction.DamageAction.SourceSupport == null && !actor.IsStruggling)
			{
				DamageAction damageAction = postDamageAction.DamageAction;
				if ((!damageAction.DealDamagePostAbility || damageAction.FinalDamage < damageAction.TargetActor.Hitpoints || damageAction.SavedFromDeath) && !CombatHelpers.ShouldActorExplode(postDamageAction.DamagerActor, postDamageAction.TargetActor, postDamageAction.DamageAction.DamageType, postDamageAction.DamageAction.FinalDamage))
				{
					FixedPoint successProbability = 0.0;
					FixedPoint value = 0.0;
					base.manager.CombatModel.AbilityManager.VisitParameter("ExtendProbability", ref value, actor);
					if (postDamageAction.DamageAction.DamageType == DamageType.Melee)
					{
						successProbability = meleePercentage;
					}
					else if (postDamageAction.DamageAction.DamageType == DamageType.Ranged)
					{
						successProbability = rangedPercentage;
					}
					if (base.manager.Player.RollDice(RollDiceType.Root, successProbability, value) != PlayerRandomChanceResult.Failed)
					{
						addedActions.Add(new RootAction(postDamageAction.DamagerActor, postDamageAction.TargetActor, turns, ignoreSourceBeingDead: false, null, () => damageAction.FinalDamage));
					}
				}
			}
			return ActionListClearFlag.Keep;
		}
	}
}
