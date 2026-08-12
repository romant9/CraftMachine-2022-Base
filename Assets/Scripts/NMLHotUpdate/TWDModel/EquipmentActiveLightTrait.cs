using System.Collections.Generic;

namespace TWDModel
{
	public class EquipmentActiveLightTrait : ActionModifier
	{
		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			if (action is PostChangeTurnAction)
			{
				CombatModel combatModel = actor.manager.CombatModel;
				if (combatModel != null && !combatModel.MissionCompleted && combatModel.TurnManager.ActiveFaction == Faction.Survivor)
				{
					actor.NotifyChange("AbilityVisited", new object[2] { "Equipment_Active_Light", false });
				}
			}
			return ActionListClearFlag.Keep;
		}
	}
}
