using System.Collections.Generic;

namespace TWDModel
{
	public class StunTrait : ActionModifier
	{
		private int turns;

		private int chancePercentage;

		public StunTrait()
		{
		}

		public StunTrait(int turns, int chancePercentage)
		{
			this.turns = turns;
			this.chancePercentage = chancePercentage;
		}

		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			PostDamageAction postDamageAction = action as PostDamageAction;
			if (postDamageAction != null && postDamageAction.DamagerActor == actor && !postDamageAction.TargetActor.IsDead && !postDamageAction.DamageAction.BodyShot && !actor.HasAnyLevelTrait("DebuffMarkEnemy") && postDamageAction.DamageAction.SourceSupport == null && !postDamageAction.DamageAction.TargetActor.IsEnvironmental && base.manager.Player.RollDice(RollDiceType.Stun, (FixedPoint)chancePercentage / (FixedPoint)100.0, 0.0) != PlayerRandomChanceResult.Failed)
			{
				int num = turns;
				FixedPoint value = 0.0;
				if (base.manager.CombatModel.AbilityManager.VisitParameter("AbilityModifierIncreaseChanceStunTurns", ref value, actor))
				{
					FixedPoint value2 = 0.0;
					base.manager.CombatModel.AbilityManager.VisitParameter("ExtendProbability", ref value2, actor);
					if (base.manager.Player.RollDice(RollDiceType.Stun, value, value2) != PlayerRandomChanceResult.Failed)
					{
						num++;
					}
				}
				addedActions.Add(new StunAction(postDamageAction.DamagerActor, postDamageAction.TargetActor, num, ignoreSourceBeingDead: false, null, () => postDamageAction.DamageAction.FinalDamage));
			}
			return ActionListClearFlag.Keep;
		}
	}
}
