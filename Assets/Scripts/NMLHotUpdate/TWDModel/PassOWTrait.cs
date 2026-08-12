using System.Collections.Generic;

namespace TWDModel
{
	public class PassOWTrait : ActionModifier
	{
		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			if (action is PostMoveSuccessAction { Actor: not null } postMoveSuccessAction && postMoveSuccessAction.Actor.IsTriggerPassOW)
			{
				if (postMoveSuccessAction.Actor.IsTriggerPassOW && !postMoveSuccessAction.Actor.IsElectricShocked && !postMoveSuccessAction.Actor.IsStunned)
				{
					postMoveSuccessAction.Actor.HasGainedExtraAP = true;
					postMoveSuccessAction.Actor.EnsureGainedExtraMoveAp = false;
					postMoveSuccessAction.Actor.NextCanTriggerPassOW = base.manager.CombatModel.TurnManager.TurnCount + 1;
					string text = "Equipment_Passive_PassOW";
					if (!string.IsNullOrEmpty(text))
					{
						postMoveSuccessAction.Actor.NotifyChange("AbilityVisited", new object[2] { text, false });
					}
					postMoveSuccessAction.Actor.EnsureExtraAction(text, dueToLuck: false);
				}
				postMoveSuccessAction.Actor.IsTriggerPassOW = false;
			}
			return ActionListClearFlag.Keep;
		}
	}
}
