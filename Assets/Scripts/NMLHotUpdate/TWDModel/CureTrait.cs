using System.Collections.Generic;

namespace TWDModel
{
	public class CureTrait : ActionModifier
	{
		private FixedPoint multiplier = 1.0;

		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			if (actor.Definition.Class != SurvivorClass.Shooter.ToString())
			{
				return ActionListClearFlag.Keep;
			}
			if (action is PreChangeTurnAction preChangeTurnAction && actor != null && preChangeTurnAction.CurrentActiveFaction == actor.Faction)
			{
				List<ActorModel> factionActors = base.manager.CombatModel.GetFactionActors(actor.Faction);
				new List<ActorModel>();
				FixedPoint value = 0.0;
				base.manager.CombatModel.AbilityManager.VisitParameter("SupportTalent_CureParm2", ref value, actor);
				foreach (ActorModel item in factionActors)
				{
					if (!item.GridCoordinate.Equals(actor.GridCoordinate))
					{
						FixedPoint value2 = 0.0;
						base.manager.CombatModel.AbilityManager.VisitParameter("SupportTalent_CureParm1", ref value2, actor);
						if (item.GridCoordinate.ChebyshevDistance(actor.GridCoordinate) <= value2)
						{
							int amountHealed = (int)((item as SurvivorModel).GetDamageForPreferredWeapon() * value);
							base.manager.ExecuteAction(new HealAction(actor, item, amountHealed));
						}
					}
				}
			}
			return ActionListClearFlag.Keep;
		}
	}
}
