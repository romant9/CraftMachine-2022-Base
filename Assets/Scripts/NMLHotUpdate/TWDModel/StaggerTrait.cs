using System.Collections.Generic;

namespace TWDModel
{
	public class StaggerTrait : ActionModifier
	{
		private int turns;

		private FixedPoint addChargeChance;

		public StaggerTrait()
		{
		}

		public StaggerTrait(int turns, FixedPoint addChargeChance)
		{
			this.turns = turns;
			this.addChargeChance = addChargeChance;
		}

		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			PostDamageAction postDamageAction = action as PostDamageAction;
			if (postDamageAction != null && postDamageAction.DamagerActor != null)
			{
				if (actor != null && postDamageAction.TargetActor.Faction != Faction.Environmental && postDamageAction.DamagerActor.Faction != Faction.Environmental && !postDamageAction.DamagerActor.IsDead && !postDamageAction.TargetActor.IsDead && !postDamageAction.DamageAction.IsPushDamage && !CombatHelpers.ShouldActorExplode(postDamageAction.DamagerActor, postDamageAction.TargetActor, postDamageAction.DamageAction.DamageType, postDamageAction.DamageAction.FinalDamage) && (postDamageAction.DamagerActor.CanBenefitFromStaggerInstantly || postDamageAction.TargetActor.CanBenefitFromStaggerInstantly))
				{
					if (postDamageAction.DamagerActor.CanReceiveChargePointFromStagger)
					{
						postDamageAction.DamagerActor.AddChargePoints(1);
						postDamageAction.DamagerActor.CanReceiveChargePointFromStagger = false;
					}
					if (postDamageAction.TargetActor.CanReceiveChargePointFromStagger)
					{
						postDamageAction.TargetActor.AddChargePoints(1);
						postDamageAction.TargetActor.CanReceiveChargePointFromStagger = false;
					}
					if (postDamageAction.DamagerActor.Faction != Faction.Walker)
					{
						addedActions.Add(new StaggerAction(postDamageAction.DamagerActor, postDamageAction.TargetActor, turns, addChargeChance, ignoreSourceBeingDead: false, null, () => postDamageAction.DamageAction.FinalDamage));
					}
				}
				postDamageAction.DamagerActor.CanBenefitFromStaggerInstantly = false;
				postDamageAction.TargetActor.CanBenefitFromStaggerInstantly = false;
			}
			return ActionListClearFlag.Keep;
		}
	}
}
