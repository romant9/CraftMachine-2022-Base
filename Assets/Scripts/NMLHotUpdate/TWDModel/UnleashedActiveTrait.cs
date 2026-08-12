using System.Collections.Generic;

namespace TWDModel
{
	public class UnleashedActiveTrait : ActionModifier
	{
		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			if (action is PostAbilityExecuteAction && !actor.IsDead)
			{
				PitfallAreasManager model = actor.manager.CombatModel.GetModel<PitfallAreasManager>();
				if (model == null)
				{
					return ActionListClearFlag.Keep;
				}
				model.RemoveActorPitfallArea(actor);
			}
			if (action is PostDamageAction postDamageAction && postDamageAction.TargetActor == actor && !actor.IsDead)
			{
				PitfallAreasManager model2 = actor.manager.CombatModel.GetModel<PitfallAreasManager>();
				if (model2 == null)
				{
					return ActionListClearFlag.Keep;
				}
				model2.RemoveActorPitfallArea(actor);
			}
			return ActionListClearFlag.Keep;
		}
	}
}
