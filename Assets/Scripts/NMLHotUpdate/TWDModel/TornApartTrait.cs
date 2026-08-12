using System.Collections.Generic;

namespace TWDModel
{
	public class TornApartTrait : ActionModifier
	{
		private int TornApartMarkMaxNum;

		public TornApartTrait(int tornApartMarkMaxNum)
		{
			TornApartMarkMaxNum = tornApartMarkMaxNum;
		}

		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			if (action is PostDamageAction postDamageAction && actor != null && postDamageAction.DamagerActor != null && actor == postDamageAction.DamagerActor && postDamageAction.TargetActor != null && postDamageAction.DamageAction.Critical && postDamageAction.DamagerActor is SurvivorModel)
			{
				if (!postDamageAction.DamagerActor.HasTraitsThatContains("Equipment_Passive_TornApart") || postDamageAction.TargetActor == null || postDamageAction.TargetActor.IsDead)
				{
					return ActionListClearFlag.Keep;
				}
				CombatModel combatModel = postDamageAction.DamagerActor.manager.CombatModel;
				if (combatModel == null || combatModel.MissionCompleted)
				{
					return ActionListClearFlag.Keep;
				}
				postDamageAction.TargetActor.TornApartMarkCount = postDamageAction.TargetActor.TornApartMarkCount + 1L;
				if (postDamageAction.TargetActor.TornApartMarkCount >= TornApartMarkMaxNum)
				{
					postDamageAction.TargetActor.TornApartMarkCount = TornApartMarkMaxNum;
				}
				postDamageAction.TargetActor.NotifyChange("TornApartUpdateEvent", new object[2] { "Equipment_Passive_TornApart", false });
			}
			return ActionListClearFlag.Keep;
		}
	}
}
