using System.Collections.Generic;

namespace TWDModel
{
	public class BaseCitadelTrait : ActionModifier
	{
		private ActorModel attackMainTarget;

		private bool isChargeAttack;

		private AbilityModel ability;

		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			_ = action is AbilityAction;
			_ = action is PostDamageAction;
			return ActionListClearFlag.Keep;
		}
	}
}
