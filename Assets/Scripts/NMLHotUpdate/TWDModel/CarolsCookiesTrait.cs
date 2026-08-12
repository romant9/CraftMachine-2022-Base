using System.Collections.Generic;

namespace TWDModel
{
	public class CarolsCookiesTrait : ActionModifier
	{
		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			TraitEntry trait = actor.TraitContainer.GetTrait("CarolsCookiesActive");
			if (trait == null)
			{
				return ActionListClearFlag.Keep;
			}
			if (action is ChangeTurnAction && actor.manager.CombatModel.TurnManager.ActiveFaction == actor.Faction)
			{
				trait.TraitDuration--;
				if (trait.TraitDuration <= 0)
				{
					actor.RemoveTrait("CarolsCookiesActive");
				}
			}
			return ActionListClearFlag.Keep;
		}
	}
}
