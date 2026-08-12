using System.Collections.Generic;

namespace TWDModel
{
	public class SharpTrait : ActionModifier
	{
		private int chancePercentage;

		public SharpTrait()
		{
		}

		public SharpTrait(int chancePercentage)
		{
			this.chancePercentage = chancePercentage;
		}

		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			if (action is PostDamageAction postDamageAction && postDamageAction.DamagerActor == actor && !postDamageAction.TargetActor.IsDead && !actor.HasAnyLevelTrait("DebuffMarkEnemy") && postDamageAction.DamageAction.SourceSupport == null && base.manager.Player.RollDice(RollDiceType.Bleed, (FixedPoint)chancePercentage / (FixedPoint)100.0, 0.0) != PlayerRandomChanceResult.Failed)
			{
				addedActions.Add(new BleedingOutAction(postDamageAction.DamagerActor, postDamageAction.TargetActor, giveFullHealth: false));
			}
			return ActionListClearFlag.Keep;
		}
	}
}
