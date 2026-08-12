using System.Collections.Generic;

namespace TWDModel
{
	public class LowerUnluckyTrait : ActionModifier
	{
		public static string UnluckTrait;

		private FixedPoint multiplier = 1.0;

		public LowerUnluckyTrait(string unluckTrait)
		{
			UnluckTrait = unluckTrait;
		}

		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			if (action is PreChangeTurnAction preChangeTurnAction && actor != null && preChangeTurnAction.CurrentActiveFaction == actor.Faction)
			{
				List<ActorModel> enemyFactionsActors = base.manager.CombatModel.GetEnemyFactionsActors(actor.Faction);
				new List<ActorModel>();
				FixedPoint value = 0.0;
				base.manager.CombatModel.AbilityManager.VisitParameter("SupportTalent_LowerluckyParm3", ref value, actor);
				FixedPoint value2 = 0.0;
				base.manager.CombatModel.AbilityManager.VisitParameter("SupportTalent_LowerluckyParm4", ref value2, actor);
				int num = 0;
				foreach (ActorModel item in enemyFactionsActors)
				{
					if (item.GridCoordinate.Equals(actor.GridCoordinate))
					{
						continue;
					}
					FixedPoint value3 = 0.0;
					base.manager.CombatModel.AbilityManager.VisitParameter("SupportTalent_LowerluckyParm1", ref value3, actor);
					if (item.GridCoordinate.DistanceTo(actor.GridCoordinate) <= value3)
					{
						item.RemoveTrait(UnluckTrait);
						item.AddTemporaryTrait(UnluckTrait, default(FixedPoint), null, 0L);
						item.UnluckyFlagTurns = (int)value2 + 1;
						item.NotifyChange("SupportTalent_Lowerlucky");
						num++;
						if (num >= (int)value)
						{
							break;
						}
					}
				}
			}
			return ActionListClearFlag.Keep;
		}
	}
}
