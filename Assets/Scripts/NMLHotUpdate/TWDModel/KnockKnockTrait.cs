using System.Collections.Generic;

namespace TWDModel
{
	public class KnockKnockTrait : ActionModifier
	{
		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			return ActionListClearFlag.Keep;
		}
	}
}
