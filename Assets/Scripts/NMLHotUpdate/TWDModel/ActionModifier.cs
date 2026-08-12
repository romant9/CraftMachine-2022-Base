using System.Collections.Generic;

namespace TWDModel
{
	public abstract class ActionModifier : ModelModifier
	{
		public abstract ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions);
	}
}
