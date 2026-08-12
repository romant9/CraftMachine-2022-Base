using System.Collections.Generic;

namespace TWDModel
{
	public class AbilityModifierAvoidStun : ActionModifier
	{
		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			if (action is StunAction stunAction && stunAction.TargetActor == actor)
			{
				if (stunAction.CanNotAvoidStunType != CanNotAvoidStunType.None)
				{
					return ActionListClearFlag.Keep;
				}
				FixedPoint value = 0.0;
				base.manager.CombatModel.AbilityManager.VisitParameter(AbilityModifierIncreaseStunAvoidChance.StunAvoidChance, ref value, actor);
				FixedPoint value2 = 0.0;
				base.manager.CombatModel.AbilityManager.VisitParameter("ExtendProbability", ref value2, actor);
				FixedPoint fixedPoint = value * (1.0 + value2);
				PlayerRandomChanceResult playerRandomChanceResult = PlayerRandomChanceResult.Failed;
				if (((actor == null || actor.Faction != Faction.Survivor || !(fixedPoint > base.manager.GameEconomyData.ConfigData.MaximumStunResistance / 100.0)) ? base.manager.Player.RollDice(RollDiceType.AvoidStun, value, value2) : base.manager.Player.RollDice(RollDiceType.AvoidStun, base.manager.GameEconomyData.ConfigData.MaximumStunResistance / 100.0, 0.0)) != PlayerRandomChanceResult.Failed)
				{
					stunAction.Avoided = true;
					if (base.manager.CurrentCommandLogEntry != null)
					{
						base.manager.CurrentCommandLogEntry.StunAvoided(stunAction.SourceActor, stunAction.TargetActor);
					}
				}
			}
			return ActionListClearFlag.Keep;
		}
	}
}
