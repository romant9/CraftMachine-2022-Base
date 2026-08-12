using System.Collections.Generic;

namespace TWDModel
{
	public class AbilityModifierAvoidHerd : ActionModifier
	{
		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			if (action is HerdAction herdAction && herdAction.TargetActor == actor)
			{
				FixedPoint value = 0.0;
				base.manager.CombatModel.AbilityManager.VisitParameter(AbilityModifierIncreaseAvoidHerdModifer.AvoidHerdChance, ref value, actor);
				if (base.manager.Player.RollDice(RollDiceType.AvoidHerd, value, 0.0) != PlayerRandomChanceResult.Failed)
				{
					herdAction.Avoided = true;
					if (base.manager.CurrentCommandLogEntry != null)
					{
						base.manager.CurrentCommandLogEntry.HerdAvoided(herdAction.SourceActor, herdAction.TargetActor);
					}
				}
			}
			return ActionListClearFlag.Keep;
		}
	}
}
