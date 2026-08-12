using System.Collections.Generic;

namespace TWDModel
{
	public class AbilityModifierGainChargePointWithHerd : ActionModifier
	{
		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			if (action is HerdAction herdAction && herdAction.SourceActor == actor && !actor.GainedChargePointOnMove)
			{
				FixedPoint value = 0.0;
				FixedPoint value2 = 0.0;
				base.manager.Player.AbilityManager.VisitParameter("LeaderBuffModifierGainChargePointAtTaunt", ref value, actor);
				base.manager.Player.AbilityManager.VisitParameter("LeaderBuffGainExtraChargePointAtTauntIncreaseChance", ref value2, actor);
				value += value2 * herdAction.AffectedActors;
				if (value != 0.0 && actor.ChargeMeter != null)
				{
					PlayerRandomChanceResult playerRandomChanceResult = base.manager.Player.RollDice(RollDiceType.GainChargePoint, value);
					if (playerRandomChanceResult != PlayerRandomChanceResult.Failed)
					{
						actor.GainedChargePointOnMove = true;
						actor.AddChargePoints(1);
						actor.NotifyChange("AbilityVisited", new object[2]
						{
							"LeaderBuffOneWithTheHerd",
							playerRandomChanceResult == PlayerRandomChanceResult.SuccessDueToExtension
						});
					}
				}
			}
			return ActionListClearFlag.Keep;
		}
	}
}
